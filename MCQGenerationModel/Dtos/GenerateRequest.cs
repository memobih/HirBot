using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCQGenerationModel.Dtos
{
    public class GenerateRequest
    {
        public string prompt { get; set; }
      
        public int max_length { get; set; } = 200;

    }

}
