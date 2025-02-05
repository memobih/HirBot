using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [MaxLength(40, ErrorMessage = "Password cannot exceed 40 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!#%?&])[A-Za-z\d@$!#%?&]{8,20}$", ErrorMessage = "Password must be between 8 and 20 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string NewPassword { get; set; }
        

    }
}