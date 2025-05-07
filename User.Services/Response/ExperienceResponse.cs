using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.Response
{
    public class ExperienceResponse
    {
        public int id { get; set;  }
        public string title { get; set; }
        public EmployeeType jobType { get; set; }
        public LocationType workType { get; set; }
        public string? location { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public PrivacyEnum? privacy { get; set; }
     
        public int rate { get; set; }


        public ExpreienceCompany  ?  company {get; set; }

    }
   

}


