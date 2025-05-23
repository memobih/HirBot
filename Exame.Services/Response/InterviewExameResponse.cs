using Exame.services.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Response
{
    public class InterviewExameResponse
    {   
        public List<InterviewExamQuestion> Questions { get; set; } = new List<InterviewExamQuestion>();
      
       


    }
    public class InterviewExamQuestion
    {
        public int examId { get; set; }

        public int id { get; set; }
        public string question { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public List<InterviewExamoption> options { get; set; }=new List<InterviewExamoption>();
    }
    public class InterviewExamoption
    {
    public bool isCorrect { get; set; }
        public int id { get; set; }
        public string option { get; set; }
    }
}
