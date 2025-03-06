using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Profile
{
    public  class EducationDto
    {
        [Required(ErrorMessage ="this fieled is required")]
        public string? InstituationName { get; set; }
        [Required(ErrorMessage = "this fieled is required")]

        public string? FieldOfStudy { get; set; }
        [Required(ErrorMessage = "this fieled is required")]
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
        public string? degree { get; set; }
       
        [Required(ErrorMessage = "this fieled is required")]
        public EducationType Type { get; set; }  
        public PrivacyEnum privacy { get; set; }
       public bool   isGraduated { get; set;  }
        public  string  ? logo { get; set;  }
   
    }
}
