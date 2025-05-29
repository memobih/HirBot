using Exame.Services.Interfaces;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using Exame.services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exame.Services.DataTransferObjects;
using Exame.Services.Response;
using ZstdSharp.Unsafe;

namespace Exame.Services.Implemntation
{
    public class ExameService : IExameService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        public ExameService(IAuthenticationService authenticationService, UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;

            _unitOfWork = unitOfWork;

        }

        public async Task<APIOperationResponse<object>> CreateExame(ExameDto exame)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var company = await _unitOfWork.Companies.GetEntityByPropertyWithIncludeAsync(c => c.UserID == user.Id);
                Exam exam = new Exam();
                exam.duration = exame.duration;
                exam.Name = exame.Name;
                exam.Type = ExamType.Interview;
                exam.CreatedBy = user.Id;
                exam.CategoryID = exame.CategoryID;
                _unitOfWork._context.Exams.Add(exam);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(exam, "the exame created succuful ");
            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> DeleteExame(int exameID)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var exame = _unitOfWork._context.Exams.Where(e => e.ID == exameID).FirstOrDefault();
                if (exame == null || exame.Type != ExamType.Interview || exame.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("this exam is not found");
                _unitOfWork._context.Exams.Remove(exame);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success("this exame deleted sussful ", "this exame deleted sussful ");
            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> DoExame(int skillId)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var userSkill = _unitOfWork._context.UserSkills.Include(s => s.Skill).Where(s => s.SkillID == skillId && user.Id == s.UserID).FirstOrDefault();

                if (userSkill == null)
                {
                    userSkill = new UserSkill();
                    var skill = await _unitOfWork.Skills.GetLastOrDefaultAsync(s => s.ID == skillId);
                    userSkill.Skill = skill;
                    userSkill.SkillID = skillId;
                    userSkill.UserID = user.Id;
                }
                else
                    return APIOperationResponse<object>.Conflict("you are do this exame before");
                //ID Name    Type Points  UserSkillID

                Exam newExam = new
                    Exam
                {
                    UserSkill = userSkill,
                    Points = 0,
                    Type = ExamType.Exame,
                    Name = "exame for" + userSkill.Skill.Name,

                };
                //ID ExamID  Content points  QuestionType
                List<Question> Questions = new List<Question>();
                //ID content IsCorrect QuestionID
                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which of the following is the correct way to declare a variable in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { option = "int x = 10;", IsCorrect = true },
        new Option { option = "x = 10;", IsCorrect = false },
        new Option { option = "int x; x == 10;", IsCorrect = false },
        new Option { option = "integer x = 10;", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "What is the default access modifier for a class in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { option = "private", IsCorrect = false },
        new Option { option = "protected", IsCorrect = false },
        new Option { option = "internal", IsCorrect = true },
        new Option { option = "public", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which keyword is used to define a method that does not return a value in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { option = "void", IsCorrect = true },
        new Option { option = "null", IsCorrect = false },
        new Option { option = "return", IsCorrect = false },
        new Option { option = "empty", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which of the following is NOT a valid data type in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { option = "int", IsCorrect = false },
        new Option { option = "float", IsCorrect = false },
        new Option { option = "real", IsCorrect = true },
        new Option { option = "decimal", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which of the following statements about C# arrays is true?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { option = "Arrays in C# are fixed-size", IsCorrect = true },
        new Option { option = "Arrays can only store integers", IsCorrect = false },
        new Option { option = "Arrays are allocated on the stack", IsCorrect = false },
        new Option { option = "Arrays cannot have null values", IsCorrect = false }
    }
                });
                newExam.Questions = Questions;
                _unitOfWork._context.Exams.Add(newExam);
                await _unitOfWork.SaveAsync();
                var respone = new ExameResponse();
                respone.name = newExam.Name;
                respone.skill = userSkill.Skill.Name;
                respone.level = "immediate";
                respone.id = newExam.ID;
                foreach (var question in newExam.Questions)
                {
                    if (question.Options != null)
                    {
                        var ques = new Questions();
                        ques.question = question.Content;
                        ques.id = question.ID;
                        respone.QuestionsNumber += 1;
                        respone.points += question.Points;
                        foreach (var option in question.Options)
                        {
                            ques.options.Add(new services.Response.Options { id = option.ID, option = option.option });
                        }
                        respone.Questions.Add(ques);
                    }

                }
                return APIOperationResponse<Object>.Success(respone, "the exame");
            }
            catch (Exception e)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> EditExame(int exameID, ExameDto exame)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                Exam exam = _unitOfWork._context.Exams.FirstOrDefault(e => e.ID == exameID);
                if (exam == null || exam.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("this exam is not found");
                exam.duration = exame.duration;
                exam.Name = exame.Name;
                exam.Type = ExamType.Interview;
                exam.CreatedBy = user.Id;
                exam.CategoryID = exam.CategoryID;

                _unitOfWork._context.Exams.Update(exam);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(exam, "the exame updated succuful ");
            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public Task<APIOperationResponse<object>> FinishExame(int id, List<AnswerDto> answers)
        {

            var exame = _unitOfWork._context.Exams.Where(e => e.ID == id).FirstOrDefault();
            if (exame != null && exame.Type != ExamType.Interview)
                return FinishSkillExame(id, answers);
            else return FinishInterviewExame(id, answers);

        }

        public async Task<APIOperationResponse<object>> FinishInterviewExame(int id, List<AnswerDto> answers)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var exam = _unitOfWork._context.Exams.Include(e => e.Questions).Where(e => e.ID == id).FirstOrDefault();
                if (exam == null || exam.Type != ExamType.Interview)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");
                var interviews = _unitOfWork._context.Interviews.Include(a => a.Application).Where(i => i.ExamID == exam.ID).ToList();
                var interview = interviews.Where(e => e.Application.UserID == user.Id).FirstOrDefault();
                if (interview == null)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");

                var response = new ResultResponse();
                foreach (var answer in answers)
                {
                    var question = _unitOfWork._context.Questions.FirstOrDefault(q => q.ID == answer.questionId);
                    var option = _unitOfWork._context.Options.FirstOrDefault(q => q.ID == answer.optionId);
                    if (option != null && question != null)
                    {
                        UserAnwer userAnswer = new UserAnwer();
                        userAnswer.QuestionID = question.ID;
                        userAnswer.OptionID = question.ID;
                        if (option.QuestionID == question.ID && option.IsCorrect == true)
                        {
                            response.CorrectQuestion += 1;
                            userAnswer.Point += question.Points;

                        }
                        _unitOfWork._context.UserAnswers.Add(userAnswer);
                    }

                }
                response.TotalQuestion = exam.Questions.Count();
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<Object>.Success(new { response.CorrectQuestion, response.TotalQuestion });

            }
            catch (Exception ex)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> FinishSkillExame(int id, List<AnswerDto> answers)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var exam = _unitOfWork._context.Exams.Include(e => e.Questions).Include(e => e.UserSkill).ThenInclude(e => e.Skill).Where(e => e.ID == id).FirstOrDefault();

                if (exam == null || exam.UserSkill.UserID != user.Id)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");
                var response = new ResultResponse();
                foreach (var answer in answers)
                {
                    var question = _unitOfWork._context.Questions.FirstOrDefault(q => q.ID == answer.questionId);
                    var option = _unitOfWork._context.Options.FirstOrDefault(q => q.ID == answer.optionId);
                    if (option != null && question != null)
                    {
                        if (option.QuestionID == question.ID && option.IsCorrect == true)
                        {
                            exam.Points += question.Points;
                            response.CorrectQuestion += 1;
                        }
                    }

                }
                exam.IsAnswerd = true;
                exam.UserSkill.Rate += exam.Points;
                response.TotalQuestion = exam.Questions.Count();
                response.skill = exam.UserSkill.Skill.Name;
                _unitOfWork._context.Exams.Update(exam);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<Object>.Success(response);

            }
            catch (Exception ex)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> GetALLExams(int categoryID)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();

                var caategory = _unitOfWork._context.Categories.Include(c => c.exams).ThenInclude(c => c.Questions).ThenInclude(c => c.Options).Where(e => e.ID == categoryID && e.UserID == user.Id).FirstOrDefault();
                if (caategory == null)
                    return APIOperationResponse<object>.NotFound("this category is not found");
                var exames = caategory.exams;

                return APIOperationResponse<object>.Success(exames);
            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> GetExame(int exameID)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var exame = _unitOfWork._context.Exams.Where(e => e.ID == exameID).FirstOrDefault();
                if (exame == null || exame.Type != ExamType.Interview || exame.CreatedBy != user.Id)
                    return APIOperationResponse<object>.NotFound("this exam is not found");

                return APIOperationResponse<object>.Success(new { exame.ID, exame.Name, exame.duration, Questions = exame.Questions.Count() });
            }
            catch
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        public async Task<APIOperationResponse<object>> GetQuestions(int exameID)
        {

            var user = await _authenticationService.GetCurrentUserAsync();
            var exame = _unitOfWork._context.Exams.Include(exame => exame.Questions).ThenInclude(q => q.Options).Where(e => e.ID == exameID).FirstOrDefault();
            if (exame == null || exame.Type != ExamType.Interview || exame.CreatedBy != user.Id)
                return APIOperationResponse<object>.NotFound("this exam is not found");
            var response = new InterviewExameResponse();

            foreach (var question in exame.Questions)
            {

                var addquestion = new InterviewExamQuestion { examId = exame.ID, id = question.ID, question = question.Content, createdAt = question.CreationDate, updatedAt = question.ModificationDate };
                foreach (var option in question.Options)
                {
                    addquestion.options.Add(new InterviewExamoption { id = option.ID, option = option.option, isCorrect = option.IsCorrect });

                }
                response.Questions.Add(addquestion);
            }
            return APIOperationResponse<object>.Success(response);
        }
        public async Task<object> GetExamForinterview(int examid)
        {
            var user = await _authenticationService.GetCurrentUserAsync();
            var exam = _unitOfWork._context.Exams.Include(e => e.Questions).ThenInclude(q => q.Options).Where(e => e.ID == examid && e.Type == ExamType.Interview ).FirstOrDefault();
            if (exam == null)
                return null;
            var response = new InterviewExameResponse();

            foreach (var question in exam.Questions)
            {

                var addquestion = new InterviewExamQuestion { examId = exam.ID, id = question.ID, question = question.Content, createdAt = question.CreationDate, updatedAt = question.ModificationDate };
                foreach (var option in question.Options)
                {
                    addquestion.options.Add(new InterviewExamoption { id = option.ID, option = option.option, isCorrect = option.IsCorrect });

                }
                response.Questions.Add(addquestion);
            }
            return response;
        }
    }
}
