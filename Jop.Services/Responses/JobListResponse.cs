using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Responses
{
    public  class JobListResponse
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public string location { get; set; }
        public int? Salary { get; set; }
        public LocationType LocationType { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public JobStatus status { get; set; }
        public string Experience {get ; set; }   

      public int ApplicantNumber {  get; set; } 

        public DateTime created_at { get; set; }

        public List<Skills>  ? Skills { get; set; }
    }
    public class Skills
    {
        public string name {  get; set; } 
        public string evaluation { get; set; }

    }
}
