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

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [MaxLength(40, ErrorMessage = "Password cannot exceed 40 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }

    }
}