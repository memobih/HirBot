using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jop.Services.DataTransferObjects
{
    public class AppuserDto
    {
        public string Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(50, ErrorMessage = "Phone number cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Job Title can only contain letters and spaces.")]
        public string jobTitle { get; set; } = string.Empty;

        public string? image { get; set; } = string.Empty;
    }
}