using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation.ProfileDto
{
    public class PersonalInfoDto
    {
        [Required(ErrorMessage ="full name is required")]
        [MaxLength(100)]
        public string  FullName { get; set; }
        [MaxLength(70)]
        public string ?  Title { get; set; }
        
    }
}