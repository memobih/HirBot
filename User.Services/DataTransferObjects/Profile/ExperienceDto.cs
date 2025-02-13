using HirBot.Comman.Idenitity;
using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Profile
{
    public  class ExperienceDto
    {
        [Required(ErrorMessage ="This field is Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public string employeeType { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public string Location { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public string? Start_Date { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public string? End_Date { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public bool privacy { get; set; }
    }
}
