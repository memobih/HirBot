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

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!#%?&])[A-Za-z\d@$!#%?&]{8,20}$", ErrorMessage = "Password must be between 8 and 20 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]

        public string Password { get; set; }
    

}
}
