using Exame.services.Response;
using Exame.Services.Interfaces;
using Exame.Services.Response;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
using MCQGenerationModel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Azure;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mysqlx.Notice.Warning.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Exame.Services.Implemntation
{
    public class DailyChanalgeService : IDailyChanalgeService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        private readonly IQuestionGenration _questionGenration;

        public DailyChanalgeService(IAuthenticationService authentication , UnitOfWork unitOfWork , IQuestionGenration questionGenration)
        {
            _authenticationService = authentication;

            _unitOfWork = unitOfWork;
            _questionGenration = questionGenration;

        }
        public async Task<APIOperationResponse<object>> GetAll()
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var userSkills = _unitOfWork._context.UserSkills.Include(s => s.Skill).Include(s => s.Exams).Where(s => s.UserID == user.Id).ToList();
                var result = new List<GetDailyChanlenge>();

                DateTime dateTime = DateTime.Today;
                var levels =await _unitOfWork.Levels.GetAllAsync();

                foreach (var userSkill in userSkills)
                {
                    if (userSkill.Exams.Count() == 0 || userSkill.Exams.Last().CreationDate != dateTime)

                    {
                        if (userSkill.Exams == null)
                            userSkill.Exams = new List<Exam>();



                        var newExam = createExame(userSkill.Skill.Name, user.Id, userSkill.ID, userSkill.Rate, userSkill.ID);
                        int number = 5;

                        foreach (var level in levels)
                        {
                            number++;
                            newExam.duration++;
                            newExam.Points++;
                            if (level.min <= userSkill.Rate && userSkill.Rate <= level.max)
                            {
                                break;
                            }

                        }
                        newExam.Questions = await _questionGenration.GenerateQuestionsAsync(number,userSkill.SkillID);
                        userSkill.Exams.Add(newExam);
                        await _unitOfWork.userSKils.UpdateAsync(userSkill);
                        await _unitOfWork.SaveAsync();
                    }
                    string ?levelName = levels.First()?.Name;
                    foreach (var level in levels)
                    {
                        levelName = level.Name;
                        if (level.min <= userSkill.Rate && userSkill.Rate <= level.max)
                        {
                            break;
                        }
                    }
                    if(userSkill.Exams.Last().IsAnswerd==false)
                    result.Add(new GetDailyChanlenge { ExameID = userSkill.Exams.Last().ID, Rate = userSkill.Rate,skill=userSkill.Skill.Name , level=levelName , logo= userSkill.Skill.ImagePath, exameDuration=userSkill.Exams.Last().duration});
                } 
                return  APIOperationResponse<object>.Success(result);
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured "); 
            }
               
        }

        public async Task<APIOperationResponse<object>> GetDailyChanalge(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var newExam = _unitOfWork._context.Exams.Include(e => e.UserSkill).ThenInclude(e=>e.Skill).Include(e=>e.Questions).ThenInclude(q=>q.Options).Where(e => e.ID == id && e.IsAnswerd == false).FirstOrDefault();
                if (newExam == null || newExam.UserSkill.UserID != user.Id)
                    return APIOperationResponse<object>.NotFound("this exame is answerd before or not found");
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
                        respone.QuestionsNumber +=1;
                        respone.points+=1;
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
            catch (Exception ex) {
                return APIOperationResponse<object>.ServerError("there are error eccured");
                }
        }

        private   Exam createExame(string skill, string userid, int userskillID  , int rate , int id )
        {
            Exam newExam = new Exam
            {
                UserSkillID = userskillID,
                Points = 5,
                Type = ExamType.DailyChallenge,
                Name = "daily question for " + skill,
                duration = 8,
                CreationDate = DateTime.Today
            };
       
            return newExam;
        }



    }


}

