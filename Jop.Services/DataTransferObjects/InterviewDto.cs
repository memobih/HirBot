using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Enums;

namespace Jop.Services.DataTransferObjects
{
    public class InterviewDto
    {
        [Required(ErrorMessage = "Candidate email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string CandidateEmail { get; set; }
        [Required(ErrorMessage = "Candidate name is required.")]
        [StringLength(100, ErrorMessage = "Candidate name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Candidate name can only contain letters and spaces.")]
        [MinLength(2, ErrorMessage = "Candidate name must be at least 2 characters long.")]
        public string CandidateName { get; set; }
        [Required(ErrorMessage ="Interview Type is required.")]
        
        public InterviewType Type { get; set; }
        [Required(ErrorMessage = "Interview Mode is required.")]
        

        public InterviewMode Mode { get; set; }
        [Required(ErrorMessage ="Interview Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "Duration is required.")]
        public int durationInMinutes { get; set; }
        
        public string? Location { get; set; }
        public List<string>? Notes { get; set; } = new List<string>();  
        public int ApplicationId { get; set; }
    }
}