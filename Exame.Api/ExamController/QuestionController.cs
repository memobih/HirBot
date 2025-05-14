using Exame.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Api.ExamController
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ApiControllerBase
    {
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionService questionService) { 
        _questionService = questionService;
        }

    }
}
