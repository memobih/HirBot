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
        public async Task<List<Question>> GenerateQuestionsAsync(string prompt, int questionCount, string level)
        {
            var requestData = new GenerateRequest
            {
                prompt = prompt,
                max_length = 500
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var result = new List<Question>();

            while (result.Count()<questionCount)
            {
                var response = await _httpClient.PostAsync("http://192.168.1.14:8000/generate", content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<QuestionResponse>(json);

                if (apiResponse == null || apiResponse.options == null || string.IsNullOrEmpty(apiResponse.question))
                    continue;

                var options = new List<Option>();
                foreach (var kvp in apiResponse.options)
                {
                    options.Add(new Option
                    {
                        option = kvp.Value,
                        IsCorrect = kvp.Key.ToLower() == apiResponse.answer.ToLower()
                    });
                }

                var question = new Question
                {
                    Content = apiResponse.question,
                    Points = 5,
                    QuestionType = QuestionType.MCQ,
                    Options = options
                };

                result.Add(question);
            }

            return result;
        }
    }

    }
