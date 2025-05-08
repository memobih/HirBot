using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Jop.Services.DataTransferObjects;
using Jop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.ResponseHandler.Consts;
using Project.Services.Interfaces;

namespace Jop.Services.Implemntations
{
    public class InterviewService : IInterviewService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ZoomMeetingService _zoom;
        private readonly IAuthenticationService _authenticationService;

        public InterviewService(UnitOfWork unitOfWork, ZoomMeetingService zoom , IAuthenticationService authenticationService)
        {

            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _zoom = zoom ?? throw new ArgumentNullException(nameof(zoom));
            _authenticationService = authenticationService;
        }

        public async Task<APIOperationResponse<List<GetInterviewDto>>> GetAllAsync()
        {
            var interviews = await _unitOfWork._context.Interviews.ToListAsync();
            if (interviews == null || !interviews.Any())
                return APIOperationResponse<List<GetInterviewDto>>.NotFound("No interviews found.");
            var interviewsdtos = new List<GetInterviewDto>();
            foreach (var interview in interviews)
            {
                var interviewDto = new GetInterviewDto
                {
                    ID = interview.ID,
                    CandidateName = interview.CandidateName,
                    CandidateEmail = interview.CandidateEmail,
                    Type = interview.Type,
                    Mode = interview.Mode,
                    StartTime = interview.StartTime.ToLocalTime(),
                    DurationInMinutes = interview.durationInMinutes,
                    Location = interview.Location,
                    ZoomMeetinLink = interview.ZoomMeetinLink,
                    Notes = interview.Notes,
                    ApplicationId = interview.ApplicationID,
                    InterviewerName = interview.InterviewerName ?? string.Empty,
                };
                interviewsdtos.Add(interviewDto);
            }

            return APIOperationResponse<List<GetInterviewDto>>.Success(interviewsdtos);
        }

        public async Task<APIOperationResponse<GetInterviewDto>> GetByIdAsync(int id)
        {
            var user=await _authenticationService.GetCurrentUserAsync();
            var company = await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.UserID == user.Id);
            var application = _unitOfWork._context.Applications.Include(c => c.Interviews).Include(a => a.Job).Where(a => a.ID == id).FirstOrDefault();
            if (application == null || application.Interviews == null || application.Job.CompanyID!=company.ID )
                return APIOperationResponse<GetInterviewDto>.NotFound("Interview not found.");

            var interview = application.Interviews.Last();
            var GetInterviewDto_1 = new GetInterviewDto
            {
                ID = interview.ID,
                CandidateName = interview.CandidateName,
                CandidateEmail = interview.CandidateEmail,
                Type = interview.Type,
                Mode = interview.Mode,
                StartTime = interview.StartTime.ToLocalTime(),
                DurationInMinutes = interview.durationInMinutes,
                Location = interview.Location,
                ZoomMeetinLink = interview.ZoomMeetinLink,
                Notes = interview.Notes,
                ApplicationId = interview.ApplicationID,
                InterviewerName = interview.InterviewerName?? string.Empty,
            };
            return interview == null
                ? APIOperationResponse<GetInterviewDto>.NotFound("Interview not found.")
                : APIOperationResponse<GetInterviewDto>.Success(GetInterviewDto_1);
        }

        public async Task<APIOperationResponse<GetInterviewDto>> CreateAsync(InterviewDto dto)
        {
            var validation = ValidateInterviewDto(dto);
            if (validation != null)
                return APIOperationResponse<GetInterviewDto>.UnprocessableEntity("Validation errors occurred.", validation.Errors as List<string>);

            var application = await _unitOfWork._context.Applications.FindAsync(dto.ApplicationId);
            if (application == null)
                return APIOperationResponse<GetInterviewDto>.NotFound("Application not found.");

            var exists = await _unitOfWork._context.Interviews
                .AnyAsync(i => i.ApplicationID == dto.ApplicationId && i.StartTime == dto.StartTime);

            if (exists)
                return APIOperationResponse<GetInterviewDto>.BadRequest("An interview already exists for this application at the specified time.");

            try
            {
                string? zoomLink = null;

                if (dto.Mode == InterviewMode.Online)
                {
                    zoomLink = await _zoom.CreateMeetingAsync(
                        dto.StartTime,
                        $"{dto.Type} Interview with {dto.CandidateName}",
                        dto.durationInMinutes
                    );
                }

                var interview = new Interview
                {
                    CandidateEmail = dto.CandidateEmail,
                    CandidateName = dto.CandidateName,
                    Type = dto.Type,
                    Mode = dto.Mode,
                    StartTime = dto.StartTime,
                    durationInMinutes = dto.durationInMinutes,
                    ZoomMeetinLink = zoomLink,
                    Notes = dto.Notes,
                    Location = dto.Location,
                    ApplicationID = dto.ApplicationId,
                    InterviewerName = string.IsNullOrWhiteSpace(dto.InterviewerName) ? "Unknown" : dto.InterviewerName.Trim()
                };

                _unitOfWork._context.Interviews.Add(interview);
                await _unitOfWork._context.SaveChangesAsync();

                var interviewDto = new GetInterviewDto
                {
                    ID = interview.ID,
                    CandidateName = interview.CandidateName,
                    CandidateEmail = interview.CandidateEmail,
                    Type = interview.Type,
                    Mode = interview.Mode,
                    StartTime = interview.StartTime.ToLocalTime(),
                    DurationInMinutes = interview.durationInMinutes,
                    Location = interview.Location,
                    ZoomMeetinLink = interview.ZoomMeetinLink,
                    Notes = interview.Notes,
                    ApplicationId = interview.ApplicationID,
                    InterviewerName = interview.InterviewerName?? string.Empty,
                };

                return APIOperationResponse<GetInterviewDto>.Success(interviewDto, "Interview created successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<GetInterviewDto>.ServerError("An error occurred while creating the interview.", new List<string> { ex.Message });
            }
        }


        public async Task<APIOperationResponse<GetInterviewDto>> UpdateAsync(string id, InterviewDto dto)
        {
            if (string.IsNullOrEmpty(id))
                return APIOperationResponse<GetInterviewDto>.BadRequest("Invalid interview ID.");

            var validation = ValidateInterviewDto(dto);
            if (validation != null)
                return APIOperationResponse<GetInterviewDto>.UnprocessableEntity("Validation errors occurred.", validation.Errors as List<string>);

            var interview = await _unitOfWork._context.Interviews.FindAsync(id);
            if (interview == null)
                return APIOperationResponse<GetInterviewDto>.NotFound("Interview not found.");

            try
            {
                interview.CandidateEmail = dto.CandidateEmail;
                interview.CandidateName = dto.CandidateName;
                interview.Type = dto.Type;
                interview.Mode = dto.Mode;
                interview.StartTime = dto.StartTime;
                interview.durationInMinutes = dto.durationInMinutes;
                interview.Location = dto.Location;
                interview.Notes = dto.Notes;
                interview.InterviewerName = dto.InterviewerName?? string.Empty;
                if (dto.Mode == InterviewMode.Online && string.IsNullOrEmpty(interview.ZoomMeetinLink))
                {
                    interview.ZoomMeetinLink = await _zoom.CreateMeetingAsync(
    dto.StartTime,
    $"{dto.Type} Interview with {dto.CandidateName}",
    dto.durationInMinutes
);


                }
                else if (dto.Mode == InterviewMode.Offline)
                {
                    interview.ZoomMeetinLink = null;
                }

                await _unitOfWork._context.SaveChangesAsync();
                return APIOperationResponse<GetInterviewDto>.Updated("Interview updated successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<GetInterviewDto>.ServerError("An error occurred while updating the interview.", new List<string> { ex.Message });
            }
        }

        public async Task<APIOperationResponse<bool>> DeleteAsync(string id)
        {

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
        public async Task<APIOperationResponse<InterviewCandidateinfoDto>> GetInterviewCandidateInfoAsync(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
                return APIOperationResponse<InterviewCandidateinfoDto>.BadRequest("Invalid application ID.");

            if (!int.TryParse(applicationId, out var applicationIdInt))
                return APIOperationResponse<InterviewCandidateinfoDto>.BadRequest("Invalid application ID format.");

            var application = await _unitOfWork._context.Applications.Include(c => c.User).FirstOrDefaultAsync(a => a.ID == applicationIdInt);
            if (application == null)
                return APIOperationResponse<InterviewCandidateinfoDto>.NotFound("Application not found.");

            if (application.User is null)
                return APIOperationResponse<InterviewCandidateinfoDto>.NotFound("User not found for the application.");

            var user = await _unitOfWork._context.Users.Include(u => u.Portfolio).FirstOrDefaultAsync(u => u.Id == application.User.Id);
            if (user is null)
                return APIOperationResponse<InterviewCandidateinfoDto>.NotFound("User details not found.");

            var interviewCandidateInfoDto = new InterviewCandidateinfoDto
            {
                CandidateName = user.FullName,
                CandidateEmail = user.Email,
                ImagePath = user.ImagePath,
                CandidateId = user.Id.ToString(),
                Title = user.Portfolio?.Title ?? "No title available"
            };

            return APIOperationResponse<InterviewCandidateinfoDto>.Success(interviewCandidateInfoDto, "Candidate information retrieved successfully.");
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
            if (dto.StartTime < DateTime.UtcNow)
                errors.Add("Start time cannot be in the past.");
            if (dto.durationInMinutes <= 0)
                errors.Add("Duration must be a positive integer.");
            if (dto.durationInMinutes > 120)
                errors.Add("Duration cannot exceed 120 minutes.");
            if (!Enum.IsDefined(typeof(InterviewMode), dto.Mode))
                errors.Add("Invalid interview mode.");

            if (!Enum.IsDefined(typeof(InterviewType), dto.Type))
                errors.Add("Invalid interview type.");
            if (string.IsNullOrWhiteSpace(dto.InterviewerName))
                errors.Add("Interviewer name is required.");
            return errors.Any()
                ? APIOperationResponse<Interview>.UnprocessableEntity("Validation errors occurred.", errors)
                : null;
        }
    }
}
