using MCQGenerationModel.Dtos;
using HirBot.Data.Entities;
using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HirBot.EntityFramework.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using HirBot.Data.IGenericRepository_IUOW;
using Project.Repository.Repository;
using MCQGenerationModel.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MCQGenerationModel.Services
{
    public class QuestionGeneration : IQuestionGenration
    {
        private readonly HttpClient _httpClient;
        private readonly UnitOfWork _unitOfWork;
        public QuestionGeneration(HttpClient httpClient, ApplicationDbContext context, UnitOfWork unitOfWork)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;

        }
        public async Task<List<Question>> GenerateQuestionsAsync( int questionCount, int id)
        {
            var randomQuestions = _unitOfWork._context.Questions.Include(q=>q.Options)
                                  .Where(q => q.SKillID == id)
                                  .OrderBy(q => Guid.NewGuid())  // ترتيب عشوائي
                                  .Take(questionCount)                       // أخذ n عنصر
                                  .ToList();
            var questions=new List<Question>(); 
            foreach (var randomQuestion in randomQuestions)
            {
                var question = new Question();
                question.Content = randomQuestion.Content;

                foreach (var option in randomQuestion.Options)
                {
                    question.Options.Add(new Option { option = option.option, IsCorrect = option.IsCorrect });

                }
                questions.Add(question);


            }
            return questions;
        }
    }

    }
