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
        [Required(ErrorMessage = "this fieled is required")]

        public string? End_Date { get; set; }
        [Required(ErrorMessage = "this fieled is required")]

        public string? degree { get; set; }
        [Required(ErrorMessage = "this fieled is required")]

        public bool HighScool { get; set; } 

        public bool privacy { get; set; }
    }
}
