using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.DataTransferObjects
{
    public class UserSkillDto
    {
        [Required(ErrorMessage ="this field is required")]
       public int skillId {  get; set; }
        [Required(ErrorMessage = "this field is required")]

        public int levelId {  get; set; } 
    }
}
