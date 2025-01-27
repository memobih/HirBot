using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "email is requred")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        [RegularExpression(pattern: @"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]

        public string Email { get; set; }
        [Required(ErrorMessage = "otp is required")]
        [RegularExpression(pattern: "^\\d{6}$", ErrorMessage = "Invalid OTP")]
        public string otp {  get; set; }
    }
}
