using HirBot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCQGenerationModel.Interfaces
{
    public interface IQuestionGenration
    {
        public Task<List<Question>> GenerateQuestionsAsync( int questionCount, int id);
    }
}
