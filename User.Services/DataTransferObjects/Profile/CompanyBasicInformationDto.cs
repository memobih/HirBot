using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Profile
{
    public class CompanyBasicInformationDto
    {
        [Required(ErrorMessage ="this field is required")]
        public string  CompanyType { get; set; }
        [Required(ErrorMessage = "this field is required")]
        public string name { get; set; }

    }
}
