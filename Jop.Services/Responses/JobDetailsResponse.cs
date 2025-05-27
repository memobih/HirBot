using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.Responses
{
    public  class JobDetailsResponse
              {
      
            public int ID { get; set; }
            public string Title { get; set; }
            public string? Description { get; set; }

            public string location { get; set; }
            public int? Salary { get; set; }
            public LocationType LocationType { get; set; }
            public EmployeeType EmployeeType { get; set; }
            public JobStatus status { get; set; }
            public string Experience { get; set; }

           public List<Requiremnts> ? requiremnts { get; set; }
        public Company Company { get; set; } = new Company();
       
            
        }
        public class Requiremnts
        {
       public int   ? skillID { get; set; }
        public int ? levelID { get; set; }
            public string Skill { get; set; }
            public string level { get; set; }
        }
        
    
}
