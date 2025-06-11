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
using Org.BouncyCastle.Cms;
using HirBot.Comman.Idenitity;
using System.Runtime.InteropServices;
using MCQGenerationModel.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Option = HirBot.Data.Entities.Option;

namespace Exame.Services.Implemntation
{
    public class ExameService : IExameService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        private readonly IQuestionGenration _questionGenration;
        public ExameService(IAuthenticationService authenticationService, UnitOfWork unitOfWork, IQuestionGenration questionGenration)
        {
            _authenticationService = authenticationService;

            _unitOfWork = unitOfWork;
            _questionGenration = questionGenration;

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

        public async Task<APIOperationResponse<object>> DoExame(UserSkillDto dto)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var skill = await _unitOfWork.Skills.GetEntityByPropertyWithIncludeAsync(s => s.ID == dto.skillId);

                var userSkill = _unitOfWork._context.UserSkills.Include(s => s.Skill).Where(s => s.SkillID == dto.skillId && user.Id == s.UserID).FirstOrDefault();
                var level = await _unitOfWork.Levels.GetEntityByPropertyWithIncludeAsync(l => l.ID == dto.levelId);
                var levels = await _unitOfWork.Levels.GetAllAsync();
                if (level == null)
                    return APIOperationResponse<Object>.NotFound("this level is not found");
                if (skill == null)
                    return APIOperationResponse<Object>.NotFound("this skill is not found");
                if (userSkill == null)
                {
                    userSkill = new UserSkill();
                    userSkill.Skill = skill;
                    userSkill.SkillID = dto.skillId;
                    userSkill.UserID = user.Id;

                }
                else if (userSkill.Delete_at != null)
                    return APIOperationResponse<object>.BadRequest("you need to restore this skill");
                else return APIOperationResponse<object>.Conflict("you add this skilll before");
                //ID Name    Type Points  UserSkillID
                Exam newExam = new
                    Exam
                {
                    UserSkill = userSkill,
                    Points = 0,
                    Type = ExamType.Exame,
                    duration=10,
                    Name = "exame  for  " + userSkill.Skill.Name,


                };

                int questionNUmber = 5;
                int points = 0;
                foreach (var l in levels)
                {
                    questionNUmber += 3;
                    points = (l.min + l.max) / 2;
                    if (l.ID == level.ID) break;
                }
                points += (points % questionNUmber);
                newExam.Questions = await _questionGenration.GenerateQuestionsAsync(questionNUmber ,skill.ID);
                newExam.Points = points;
                _unitOfWork._context.Exams.Add(newExam);
                _unitOfWork._context.SaveChanges();
                return APIOperationResponse<object>.Success(new { examId = newExam.ID }, "the exame");
            }
            catch (Exception e)
            {
                return APIOperationResponse<Object>.ServerError(e.Message);
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
                answers = answers.DistinctBy(a => a.questionId).ToList();
                var exam = _unitOfWork._context.Exams.Include(e => e.Questions).Where(e => e.ID == id).FirstOrDefault();
                if (exam == null || exam.Type != ExamType.Interview)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");
                var interview = _unitOfWork._context.Interviews.Include(a => a.Application).Where(i => i.ExamID == exam.ID && i.Application.UserID == user.Id).FirstOrDefault();
                if (interview == null || interview.TechStartTime==null)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");
                if (interview.TechStartTime.HasValue)
                {
                    var endTime = interview.TechStartTime.Value.AddMinutes(exam.duration+1);
                         DateTime now=
TimeZoneInfo.ConvertTimeFromUtc(
    DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById(
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "Egypt Standard Time"
            : "Africa/Cairo"
    )
);
                    if (now > endTime)
                    {
                        return APIOperationResponse<Object>.BadRequest("time is expired");
                    }
                 
                }
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
                        if (option.QuestionID == question.ID && option.IsCorrect == true && question.ExamID== interview.ExamID)
                        {
                            response.CorrectQuestion += 1;
                            userAnswer.Point += 1;
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
                answers = answers.DistinctBy(a => a.questionId).ToList();
                if (exam == null || exam.UserSkill.UserID != user.Id)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");
                DateTime Now =
   TimeZoneInfo.ConvertTimeFromUtc(
       DateTime.UtcNow,
       TimeZoneInfo.FindSystemTimeZoneById(
           RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
               ? "Egypt Standard Time"
               : "Africa/Cairo"
       )
   );

                //if (exam.CreationDate.AddMinutes(exam.duration + 1) < Now || exam.IsAnswerd)
                //{
                //    return APIOperationResponse<Object>.BadRequest("the exame is end");
                //}
                var response = new ResultResponse();
                int points = 0;
                response.TotalQuestion = exam.Questions.Count();

                foreach (var answer in answers)
                {
                    var question = _unitOfWork._context.Questions.FirstOrDefault(q => q.ID == answer.questionId);
                    var option = _unitOfWork._context.Options.FirstOrDefault(q => q.ID == answer.optionId);

                    var userAnswer=new UserAnwer();
                    if (option != null && question != null)
                    {
                        if (option.QuestionID == question.ID && option.IsCorrect == true)
                        {
                            points+= (exam.Points/response.TotalQuestion);
                            response.CorrectQuestion += 1;
                            userAnswer.QuestionID=question.ID;
                            userAnswer.OptionID=option.ID;
                            userAnswer.UserID = user.Id; 
                            _unitOfWork._context.UserAnswers.Add( userAnswer );
                        }
                    }

                }
                
                exam.IsAnswerd = true;
                exam.UserSkill.Rate += points;
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
            var exam = _unitOfWork._context.Exams.Include(e => e.Questions).ThenInclude(q => q.Options).Where(e => e.ID == examid && e.Type == ExamType.Interview && e.CreatedBy==user.Id).FirstOrDefault();
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

        public async Task<APIOperationResponse<object>> GetExameByid(int exameID)
        {
          try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var Exam=_unitOfWork._context.Exams.Where(e=>e.ID==exameID).FirstOrDefault();

                if(Exam==null)
                    return APIOperationResponse<object>.NotFound("this exme is not found ");
                if (Exam.Type == ExamType.Exame)
                 return   GetSkillExame(Exam , user);
                else if (Exam.Type == ExamType.Interview)
               return GetInterviewExame(Exam , user);

            return APIOperationResponse<object>.NotFound("this exme is not found ");

            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured"); 
            }
        } 
        private  APIOperationResponse<object> GetInterviewExame(Exam exame , ApplicationUser user)
        {

            try
            {
                var interview = _unitOfWork._context.Interviews.Include(i => i.Application).ThenInclude(a => a.Job).ThenInclude(j => j.Company).ThenInclude(c => c.account).Where(i => i.ExamID == exame.ID && i.Application.UserID == user.Id).FirstOrDefault();
                if (interview == null)
                    return APIOperationResponse<object>.NotFound("this exme is not found ");
                DateTime CreationDate =
TimeZoneInfo.ConvertTimeFromUtc(
 DateTime.UtcNow,
 TimeZoneInfo.FindSystemTimeZoneById(
     RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
         ? "Egypt Standard Time"
         : "Africa/Cairo"
 )
);
                if (interview.TechStartTime == null || interview.TechStartTime>CreationDate)
                {
                    return APIOperationResponse<object>.CreateResponse(
                        Project.ResponseHandler.Consts.ResponseType.BadRequest,
                        message: "the start date is not now ",
                        errors:null
                         ,
                        data: new {
                            startTime = interview.TechStartTime ,
                            currentTime= CreationDate
                        }
                         );
                }
                var respone = new ExameResponse();
                exame.Questions = _unitOfWork._context.Questions.Include(q=>q.Options).Where(q => q.ExamID == exame.ID).ToList();
                respone.name = exame.Name;
               respone.id = exame.ID;
                respone.icon = interview.Application.Job.Company.account.ImagePath;
          
                foreach (var question in exame.Questions)
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
                respone.startTime = interview.TechStartTime;
                return APIOperationResponse<object>.Success(new
                {
                    id = exame.ID,

                    startTime = respone.startTime,

                    respone.Questions,
                    respone.QuestionsNumber,
                    respone.name,
                    respone.duration,
                    respone.points,
                    respone.currentTime,
                    jobTitle = interview.Application.Job.Title,
                    companyLogo= respone.icon,
                    companyName = interview.Application.Job.Company.account.FullName,
                    jobDescription = interview.Application.Job.Description,
                }, "the exame");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured ");
            }

   
        }
        private  APIOperationResponse<object> GetSkillExame(Exam exame , ApplicationUser user)
        {
            try
            {
                var newExam = _unitOfWork._context.Exams.Include(e => e.UserSkill).
              ThenInclude(e => e.Skill).Include(e => e.Questions)
              .ThenInclude(q => q.Options)
             .Where(e => e.ID == exame.ID && user.Id == e.UserSkill.UserID).
              FirstOrDefault();
                var respone = new ExameResponse();
                respone.name = newExam.Name;
                respone.duration = newExam.duration;
                respone.id = newExam.ID;
                respone.skill.name = newExam.UserSkill.Skill.Name;
                respone.skill.id = newExam.UserSkill.Skill.ID;
                respone.skill.logo = newExam.UserSkill.Skill.ImagePath;
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
                respone.startTime = newExam.CreationDate;

                return APIOperationResponse<object>.Success(respone, "the exame");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured ");

            }
        }
    }
}
