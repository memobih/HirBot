using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects
{
    public class ContactInfoDto
    {

        [Url (ErrorMessage ="this must be Url")]
        public string ?  PortfolioURL { get; set; }
        
         //public string ?  Location { get; set; }
        [MaxLength(11)]
        [RegularExpression(@"^\d+$")]
        public string  ? ContactNumber { get; set; }
        [Url(ErrorMessage = "this must be Url")]
        public string ? GithubURL { get; set; } 
        
    }
}