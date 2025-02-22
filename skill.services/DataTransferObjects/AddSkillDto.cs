using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace skill.services.DataTransferObjects
{
    public class AddSkillDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
        [MinLength(3, ErrorMessage = "Name length can't be less than 3.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters are not allowed.")]
        public string Name { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Status length can't be more than 50.")]
        [MinLength(3, ErrorMessage = "Status length can't be less than 3.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Special characters are not allowed.")]
        public string Status { get; set; }
        public IFormFile? ImagePath { get; set; }
        
    }
}