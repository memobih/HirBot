using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.DataTransferObjects
{
    public class QuestionDto
    {
        [Required(ErrorMessage = "this field is required")]

        public string question {  get; set; } 
        
        public List<optionDto> options { get; set; } = new List<optionDto>();

    }
    public class optionDto
    {
         public int  ? id { get; set; }
        [Required(ErrorMessage ="this field is required")]
        public string option { get; set; }
        public bool IsCorrect { get; set; }
    }
}
