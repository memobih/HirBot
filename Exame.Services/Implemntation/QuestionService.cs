using Exame.services.Response;
using Exame.Services.DataTransferObjects;
using Exame.Services.Interfaces;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Implemntation
{
    public class QuestionService : IQuestionService
    {

        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        public QuestionService(IAuthenticationService authenticationService, UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;

            _unitOfWork = unitOfWork;
        }

        public async Task<APIOperationResponse<object>> Create(int examID, QuestionDto dto)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var exame = _unitOfWork._context.Exams.FirstOrDefault(e => e.ID == examID);
                if (exame == null || exame.Type != ExamType.Interview || exame.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("this exame is not found");
                Question newQuestion = new Question();
                newQuestion.ExamID=examID;
                newQuestion.Content=dto.question;
                foreach (var item in dto.options)
                {
                    newQuestion.Options.Add(new Option { IsCorrect = item.IsCorrect, Content = item.option });
                }
                _unitOfWork._context.Questions.Add(newQuestion);
               await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(new
                {
                    newQuestion.ID,
                    newQuestion.Content,
                    Options = newQuestion.Options
                }, "new question added sussful");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");

            }
        }

        public async Task<APIOperationResponse<object>> Delete(int questionId)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

                var question = _unitOfWork._context.Questions
                    .FirstOrDefault(q => q.ID == questionId);

                if (question == null)
                    return APIOperationResponse<object>.NotFound("Question not found");

                var exam = _unitOfWork._context.Exams.FirstOrDefault(e => e.ID == question.ExamID);

                if (exam == null || exam.Type != ExamType.Interview || exam.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("Exam not found or not authorized");

                _unitOfWork._context.Options.RemoveRange(question.Options);
                _unitOfWork._context.Questions.Remove(question);

                await _unitOfWork.SaveAsync();

                return APIOperationResponse<object>.Success("Question deleted successfully");
            }
            catch (Exception)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while deleting the question");
            }
        }


        public async Task<APIOperationResponse<object>> Edit(int questionId, QuestionDto dto)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

                var question = _unitOfWork._context.Questions.Include(q=>q.Options)
                    .FirstOrDefault(q => q.ID == questionId);

                if (question == null)
                    return APIOperationResponse<object>.NotFound("Question not found");

                var exam = _unitOfWork._context.Exams.FirstOrDefault(e => e.ID == question.ExamID);

                if (exam == null || exam.Type != ExamType.Interview || exam.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("Exam not found or not authorized");

                question.Content = dto.question;

                // Remove old options
                _unitOfWork._context.Options.RemoveRange(question.Options);

                // Add new options
                question.Options = dto.options.Select(o => new Option
                {
                    Content = o.option,
                    IsCorrect = o.IsCorrect
                }).ToList();
                _unitOfWork._context.Questions.Update(question);
                await _unitOfWork.SaveAsync();

                return APIOperationResponse<object>.Success(new
                {
                    question.ID,
                    question.Content,
                    Options = question.Options
                },"Question updated successfully");
            }
            catch (Exception)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while updating the question");
            }
        }

        public async Task<APIOperationResponse<object>> GetQuestion(int questionId)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

                var question = _unitOfWork._context.Questions
                    .Where(q => q.ID == questionId)
                    .Select(q => new
                    {
                        q.ID,
                        q.Content,
                        Exam = q.Exam,
                        Options = q.Options.Select(o => new { o.ID, o.Content, o.IsCorrect }).ToList()
                    })
                    .FirstOrDefault();

                if (question == null)
                    return APIOperationResponse<object>.NotFound("Question not found");

                if (question.Exam == null || question.Exam.Type != ExamType.Interview || question.Exam.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("Unauthorized access");

                var result = new
                {
                    question.ID,
                    question.Content,
                    Options = question.Options
                };

                return APIOperationResponse<object>.Success(result);
            }
            catch (Exception)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while retrieving the question");
            }
        }

    }
}
