using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Jop.Services.Responses
{
    public class EmployeeList
    {
       public int id { get; set; }
        public string  ? name {  get; set; }  
        public string Title { get; set; } 

        public  EmployeeType  jobType { get; set; } 
        public LocationType workType { get; set; } 

        public int Rate { get; set; } 
        public DateTime ?  start_date { get; set; }
        public DateTime? end_date { get; set; }


    }
}
