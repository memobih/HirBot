using Exame.services.Response;
using Exame.Services.Interfaces;
using Exame.Services.Response;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using HirBot.ResponseHandler.Models;
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

namespace Exame.Services.Implemntation
{
    public class DailyChanalgeService : IDailyChanalgeService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork; 
        public DailyChanalgeService(IAuthenticationService authentication , UnitOfWork unitOfWork)
        {
            _authenticationService = authentication;
            _unitOfWork = unitOfWork;
        }
        public async Task<APIOperationResponse<object>> GetAll()
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var userSkills = _unitOfWork._context.UserSkills.Include(s => s.Skill).Include(s => s.Exams).Where(s => s.UserID == user.Id).ToList();
                var result = new List<GetDailyChanlenge>();

                DateTime dateTime = DateTime.Today;

                foreach (var userSkill in userSkills)
                {
                    if (userSkill.Exams.Count()==0 || userSkill.Exams.Last().CreationDate !=dateTime)

                    {
                        if (userSkill.Exams == null)
                            userSkill.Exams = new List<Exam>();
                        userSkill.Exams.Add(createExame(userSkill.Skill.Name, user.Id, userSkill.ID));
                     await _unitOfWork.userSKils.UpdateAsync(userSkill);
                       await _unitOfWork.SaveAsync();
                    }
                    if(userSkill.Exams.Last().IsAnswerd==false)
                    result.Add(new GetDailyChanlenge { ExameID = userSkill.Exams.Last().ID, Rate = userSkill.Rate,skill=userSkill.Skill.Name , level="immediate" , logo= userSkill.Skill.ImagePath, exameDuration=userSkill.Exams.Last().duration});
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
                var newExam = _unitOfWork._context.Exams.Include(e => e.UserSkill).Include(e=>e.Questions).ThenInclude(q=>q.Options).Where(e => e.ID == id && e.IsAnswerd == false).FirstOrDefault();
                if (newExam == null || newExam.UserSkill.UserID != user.Id)
                    return APIOperationResponse<Object>.NotFound("this exame is answerd before or not found");

                var respone = new ExameResponse();
                respone.name = newExam.Name;
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
            catch (Exception ex) {
                return APIOperationResponse<object>.ServerError("there are error eccured");
                }
        }

        private  Exam createExame(string skill  , string userid  , int userskillID)
        {


            //ID Name    Type Points  UserSkillID

            Exam newExam = new
                Exam
            {
                UserSkillID = userskillID,
                Points = 0,
                Type = ExamType.DailyChallenge,
                Name = "daily question for  " + skill,
                duration = 10,
                CreationDate=DateTime.Today

            };
            //_unitOfWork._context.Exams.Add(newExam);
            //ID ExamID  Content points  QuestionType
            List<Question> Questions = new List<Question>();
            //ID content IsCorrect QuestionID
            Questions.Add(new Question
            {
                Content = "test  the error ?",
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

            return newExam;
           
        }
     

    }
}
