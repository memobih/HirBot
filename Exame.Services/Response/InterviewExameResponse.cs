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
        public string name ;
        public int duration;
        public int id { get; set; }
        public List<InterviewExamQuestion> Questions { get; set; } = new List<InterviewExamQuestion>();
        public int QuestionsNumber { get; set; } 

    }
    public class InterviewExamQuestion
    {
        public int id { get; set; }
        public string question { get; set; }
        public List<InterviewExamoption> options { get; set; }=new List<InterviewExamoption>();
        public string Correctanswer { get; set; }
    }
    public class InterviewExamoption
    {
        public int id { get; set; }
        public string option { get; set; }
    }
}
