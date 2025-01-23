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
        public string Email { get; set; }
        [MinLength(6 ,ErrorMessage ="invalid otp")]
        [Required(ErrorMessage = "otp is required")]

        public int otp {  get; set; }
    }
}
