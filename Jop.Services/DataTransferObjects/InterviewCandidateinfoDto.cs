using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jop.Services.DataTransferObjects
{
    public class InterviewCandidateinfoDto
    {
        public string CandidateId { get; set; } 
        public string CandidateEmail { get; set; }
        public string CandidateName { get; set; }
        public string? ImagePath { get; set; }
        public string?Title { get; set; } 
        public float?Score { get; set; }
    }
}