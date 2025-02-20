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
        public string title { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public EmployeeType jobType { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public LocationType workType { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public string? startDate { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public string? endDate { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        public PrivacyEnum privacy { get; set; } 
         
        public string  ? location { get; set; }
    }
}
