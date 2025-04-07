using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jop.Services.DataTransferObjects
{
    public class InitialInterviewDto
    {
        [Required]
        public string ApplicantId { get; set; }
        [Required]
        public int ApplicationId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Interview type cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Interview type can only contain letters and spaces.")]
        public string InterviewType { get; set; } 
        [Required]
        [StringLength(50, ErrorMessage = "Interviewer name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Interviewer name can only contain letters and spaces.")]
        public string InterviewerName { get; set; } 
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InterviewDate { get; set; } 
        [Required]
        public string InterviewTime { get; set; } 
        [Required]
        [StringLength(50, ErrorMessage = "Interview location cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Interview location can only contain letters and spaces.")]
        public string InterviewLocation { get; set; } 

        public List<string> Notes { get; set; } = new List<string>();

    }
}