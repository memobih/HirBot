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
        [Required(ErrorMessage = "Name is required"),
MaxLength(1000, ErrorMessage = "Name cannot exceed 500 characters"),
MinLength(6, ErrorMessage = "Name must be at least 6 characters long")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
        public string Password { get; set; }
    }
}
