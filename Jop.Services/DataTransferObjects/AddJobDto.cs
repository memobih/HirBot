using HirBot.Data.Entities;
using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.DataTransferObjects
{
    public  class AddJobDto
    {
        [Required(ErrorMessage ="this field is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "this field is required")]

        public string ? Description { get; set; }
        [Required(ErrorMessage = "this field is required")]

        public LocationType LocationType { get; set; }
        [Required(ErrorMessage = "this field is required")]

        public EmployeeType EmployeeType { get; set; }

        [Required(ErrorMessage = "this field is required")]

        public string Experience { get; set; }
         
        public int   ? Salary { get; set; }

        public JobStatus status { get; set; }
        [Required(ErrorMessage = "this field is required")]

        public string location { get; set; }


        public virtual List<Requirment> ? Requirments { get; set; }

    }
    public class Requirment
    {
        
        public int SkillID { get; set; }
      
        public int LevelID { get; set; }

    }
   
}
