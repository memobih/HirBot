using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.Authencation
{
    public class PasswordDto
    {

    
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(pattern: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!#%*?&])[A-Za-z\\d@$!#%*?&]{8,20}$",
            ErrorMessage = "Password must be 8-20 characters long, include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
    

}
}
