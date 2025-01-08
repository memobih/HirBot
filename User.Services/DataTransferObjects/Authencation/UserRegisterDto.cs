using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Name is required"),
        MaxLength(1000, ErrorMessage = "Name cannot exceed 500 characters"),
        MinLength(6, ErrorMessage = "Name must be at least 6 characters long")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters")]
        [DataType(DataType.Text, ErrorMessage = "Invalid name format")]  
        
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required"), DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        [MaxLength(1000, ErrorMessage = "Email cannot exceed 500 characters")] 
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [MaxLength(40, ErrorMessage = "Password cannot exceed 40 characters")]
        public string Password { get; set; }

    }
}
