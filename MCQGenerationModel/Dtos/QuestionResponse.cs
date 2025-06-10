using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCQGenerationModel.Dtos
{
    public class QuestionResponse
    {
        public string question { get; set; }
        public Dictionary<string, string>  options { get; set; }
        public string answer { get; set; }
    }

}
