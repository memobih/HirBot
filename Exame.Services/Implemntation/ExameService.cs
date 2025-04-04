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

namespace Exame.Services.Implemntation
{
    public class ExameService : IExameService
    { 
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        public ExameService(IAuthenticationService authenticationService , UnitOfWork unitOfWork ) { 
            _authenticationService = authenticationService;

            _unitOfWork = unitOfWork;
        
        }

        public async Task<APIOperationResponse<object>> DoExame(int skillId)
        {
            try
            {
              var user=await _authenticationService.GetCurrentUserAsync(); 
               var userSkill = _unitOfWork._context.UserSkills.Include(s=>s.Skill).Where(s=>s.SkillID == skillId && user.Id==s.UserID).FirstOrDefault();
                
                if (userSkill == null)
                    return APIOperationResponse<Object>.NotFound("this skill is not addded");
                //ID Name    Type Points  UserSkillID

                Exam newExam = new
                    Exam {
                    UserSkillID= userSkill.ID  , 
                    Points=0 , 
                   Type=ExamType.Exame , 
                   Name ="exame for" +  userSkill.Skill.Name ,

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
        new Option { Content = "int x = 10;", IsCorrect = true },
        new Option { Content = "x = 10;", IsCorrect = false },
        new Option { Content = "int x; x == 10;", IsCorrect = false },
        new Option { Content = "integer x = 10;", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "What is the default access modifier for a class in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { Content = "private", IsCorrect = false },
        new Option { Content = "protected", IsCorrect = false },
        new Option { Content = "internal", IsCorrect = true },
        new Option { Content = "public", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which keyword is used to define a method that does not return a value in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { Content = "void", IsCorrect = true },
        new Option { Content = "null", IsCorrect = false },
        new Option { Content = "return", IsCorrect = false },
        new Option { Content = "empty", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which of the following is NOT a valid data type in C#?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { Content = "int", IsCorrect = false },
        new Option { Content = "float", IsCorrect = false },
        new Option { Content = "real", IsCorrect = true },
        new Option { Content = "decimal", IsCorrect = false }
    }
                });

                Questions.Add(new Question
                {
                    ExamID = newExam.ID,
                    Content = "Which of the following statements about C# arrays is true?",
                    Points = 10,
                    QuestionType = QuestionType.MCQ,
                    Options = new List<Option> {
        new Option { Content = "Arrays in C# are fixed-size", IsCorrect = true },
        new Option { Content = "Arrays can only store integers", IsCorrect = false },
        new Option { Content = "Arrays are allocated on the stack", IsCorrect = false },
        new Option { Content = "Arrays cannot have null values", IsCorrect = false }
    }
                });
                newExam.Questions= Questions; 
                _unitOfWork._context.Exams.Add(newExam);
               await _unitOfWork.SaveAsync();
                var respone =new  ExameResponse();
                respone.name = newExam.Name;
                respone.skill = userSkill.Skill.Name;
                respone.level = "immediate";
                respone.id = newExam.ID;
                foreach(var question in newExam.Questions )
                {
                    if (question.Options != null)
                    {
                        var ques = new Questions();
                        ques.question=question.Content;
                        ques.id = question.ID;
                        respone.QuestionsNumber += 1;
                        respone.points += question.Points; 
                        foreach (var option in question.Options)
                        {
                            ques.options.Add(new services.Response.Options { id = option.ID, option = option.Content });
                        }
                        respone.Questions.Add( ques );
                    }
                    
                }
                return APIOperationResponse<Object>.Success(respone , "the exame");
            }
            catch (Exception e)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured");
            }
        }

        public async Task<APIOperationResponse<object>> FinishExame(int id, List<AnswerDto> answers)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                var exam = _unitOfWork._context.Exams.Include(e=>e.Questions).Include(e=>e.UserSkill).ThenInclude(e=>e.Skill).Where(e => e.ID == id).FirstOrDefault();
                if (exam == null || exam.UserSkill.UserID != user.Id)
                    return APIOperationResponse<Object>.NotFound("this exame is not found");
                var response = new ResultResponse();
                foreach (var answer in answers)
                {
                    var question=_unitOfWork._context.Questions.FirstOrDefault(q=>q.ID==answer.questionId);
                    var option = _unitOfWork._context.Options.FirstOrDefault(q=>q.ID==answer.optionId);
                    if(option != null && question!=null)
                    {
                        if (option.QuestionID == question.ID && option.IsCorrect == true)
                        {
                            exam.Points += question.Points;
                            response.CorrectQuestion += 1;
                        }
                    }

                }
                exam.UserSkill.Rate += exam.Points;
                response.TotalQuestion = exam.Questions.Count();
                response.skill = exam.UserSkill.Skill.Name;
                _unitOfWork._context.Exams.Update(exam);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<Object>.Success(response);

            }
            catch(Exception ex)
            {
                return APIOperationResponse<Object>.ServerError("there are error accured"); 
            }
        } 

    }
}
