using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress (ErrorMessage ="Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [MinLength(6, ErrorMessage = "Email must be at least 6 characters long")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(pattern: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!#%*?&])[A-Za-z\\d@$!#%*?&]{8,20}$",
                 ErrorMessage = "Password must be 8-20 characters long, include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
    }
}
