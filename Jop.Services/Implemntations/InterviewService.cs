using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.ResponseHandler.Consts;

namespace Jop.Services.Implemntations
{
    public class InterviewService : IInterviewService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ZoomMeetingService _zoom;

        public InterviewService(UnitOfWork unitOfWork, ZoomMeetingService zoom)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _zoom = zoom ?? throw new ArgumentNullException(nameof(zoom));
        }

        public async Task<APIOperationResponse<List<Interview>>> GetAllAsync()
        {
            var interviews = await _unitOfWork._context.Interviews.ToListAsync();
            return APIOperationResponse<List<Interview>>.Success(interviews);
        }

        public async Task<APIOperationResponse<Interview>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return APIOperationResponse<Interview>.BadRequest("Invalid interview ID.");

            var interview = await _unitOfWork._context.Interviews.FindAsync(id);
            return interview == null
                ? APIOperationResponse<Interview>.NotFound()
                : APIOperationResponse<Interview>.Success(interview);
        }

        public async Task<APIOperationResponse<Interview>> CreateAsync(InterviewDto dto)
        {
            var validation = ValidateInterviewDto(dto);
            if (validation != null)
                return validation;

            try
            {
                string? zoomLink = null;
                if (dto.Mode == InterviewMode.Online)
                {
                    zoomLink = await _zoom.CreateMeetingAsync(dto.StartTime, $"{dto.Type} Interview with {dto.CandidateName}");
                }

                var interview = new Interview
                {
                    CandidateEmail = dto.CandidateEmail,
                    CandidateName = dto.CandidateName,
                    Type = dto.Type,
                    Mode = dto.Mode,
                    StartTime = dto.StartTime,
                    Duration = dto.Duration!,
                    ZoomMeetinLink = zoomLink
                };

                _unitOfWork._context.Interviews.Add(interview);
                await _unitOfWork._context.SaveChangesAsync();

                return APIOperationResponse<Interview>.Success(interview, "Interview created successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<Interview>.ServerError("An error occurred while creating the interview.", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<Interview>> UpdateAsync(int id, InterviewDto dto)
        {
            if (id <= 0)
                return APIOperationResponse<Interview>.BadRequest("Invalid interview ID.");

            var validation = ValidateInterviewDto(dto);
            if (validation != null)
                return validation;

            var interview = await _unitOfWork._context.Interviews.FindAsync(id);
            if (interview == null)
                return APIOperationResponse<Interview>.NotFound("Interview not found.");

            try
            {
                interview.CandidateEmail = dto.CandidateEmail;
                interview.CandidateName = dto.CandidateName;
                interview.Type = dto.Type;
                interview.Mode = dto.Mode;
                interview.StartTime = dto.StartTime;
                interview.Duration = dto.Duration!;

                if (dto.Mode == InterviewMode.Online && string.IsNullOrEmpty(interview.ZoomMeetinLink))
                {
                    interview.ZoomMeetinLink = await _zoom.CreateMeetingAsync(dto.StartTime, $"{dto.Type} Interview with {dto.CandidateName}");
                }
                else if (dto.Mode == InterviewMode.Offline)
                {
                    interview.ZoomMeetinLink = null;
                }

                await _unitOfWork._context.SaveChangesAsync();
                return APIOperationResponse<Interview>.Updated("Interview updated successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<Interview>.ServerError("An error occurred while updating the interview.", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<bool>> DeleteAsync(int id)
        {
            if (id <= 0)
                return APIOperationResponse<bool>.BadRequest("Invalid interview ID.");

            var interview = await _unitOfWork._context.Interviews.FindAsync(id);
            if (interview == null)
                return APIOperationResponse<bool>.NotFound("Interview not found.");

            try
            {
                _unitOfWork._context.Interviews.Remove(interview);
                await _unitOfWork._context.SaveChangesAsync();
                return APIOperationResponse<bool>.Deleted("Interview deleted successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<bool>.ServerError("An error occurred while deleting the interview.", new List<string> { ex.Message });
            }
        }

        private APIOperationResponse<Interview>? ValidateInterviewDto(InterviewDto dto)
        {
            var errors = new List<string>();

            if (dto == null)
                return APIOperationResponse<Interview>.BadRequest("Interview data cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.CandidateEmail))
                errors.Add("Candidate email is required.");

            if (string.IsNullOrWhiteSpace(dto.CandidateName))
                errors.Add("Candidate name is required.");

            if (dto.StartTime == default)
                errors.Add("Start time is required.");

            if (string.IsNullOrWhiteSpace(dto.Duration))
                errors.Add("Duration is required.");
            else if (!TimeSpan.TryParse(dto.Duration, out var duration) || duration <= TimeSpan.Zero)
                errors.Add("Duration format is invalid or less than zero.");

            if (!Enum.IsDefined(typeof(InterviewMode), dto.Mode))
                errors.Add("Invalid interview mode.");

            if (!Enum.IsDefined(typeof(InterviewType), dto.Type))
                errors.Add("Invalid interview type.");

            return errors.Any()
                ? APIOperationResponse<Interview>.UnprocessableEntity("Validation errors occurred.", errors)
                : null;
        }
    }
}
