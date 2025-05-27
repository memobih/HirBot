using HirBot.Data.Enums;
using Moujam.Casiher.Comman.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HirBot.Data.Entities
{
    public class Interview : AuditEntity
    {

        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Exam")]
        public int? ExamID { get; set; }
        [ForeignKey("Application")]
        public int ApplicationID { get; set; }
        public virtual Exam? Exam { get; set; }
        public virtual Application Application { get; set; }
        public DateTime StartTime { get; set; }
        public int durationInMinutes { get; set; }
        public string? Location { get; set; }
        public InterviewMode Mode { get; set; }
        public InterviewType Type { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public List<string> Notes { get; set; } = new List<string>();
        public string? ZoomMeetinLink { get; set; }
        public string? InterviewerName { get; set; } 
        public DateTime? TechStartTime { get; set; }
        
    }
}
