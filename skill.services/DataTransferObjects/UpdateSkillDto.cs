using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace skill.services.DataTransferObjects
{
    public class UpdateSkillDto
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name must be less than 50 characters")]
        [MinLength(3, ErrorMessage = "Name must be more than 3 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must be alphabets")]
        public string Name { get; set; }
        public IFormFile ImagePath { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Status must be less than 50 characters")]
        [MinLength(3, ErrorMessage = "Status must be more than 3 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Status must be alphabets")]
        public string Status { get; set; } 
    }
}