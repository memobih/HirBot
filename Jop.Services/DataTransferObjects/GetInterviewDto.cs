using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HirBot.Data.Enums;

namespace Jop.Services.DataTransferObjects
{
    public class GetInterviewDto
    {
        public string ID { get; set; }
        public string CandidateEmail { get; set; }

        public string CandidateName { get; set; }

        public InterviewType Type { get; set; }
        public InterviewMode Mode { get; set; }

        public DateTime StartTime { get; set; }
        public int? DurationInMinutes { get; set; } 
        public string? Location { get; set; }
        public List<string> Notes { get; set; } = new List<string>();
        public string? ZoomMeetinLink { get; set; }
        public int ApplicationId { get; set; }
        public string? InterviewerName { get; set; } = string.Empty;
    }
}