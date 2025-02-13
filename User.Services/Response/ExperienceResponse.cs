using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.Response
{
    public  class ExperienceResponse
    {
        public string Title { get; set; }
        public string employeeType { get; set; }
        public string Location { get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
        public bool ? privacy { get; set; } 
        public string ? CompanyName { get; set; }
        public string ? Logo { get; set; }
        public int   rate { get; set; }

        public int ?  id { get; set; }

    }

}
