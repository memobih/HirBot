using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill.services.DataTransferObjects
{
    public class LevelDto
    {
        [Required(ErrorMessage ="this field is required")]
        
        public string Name { get; set; }
        [Required(ErrorMessage = "this field is required")]

        public int min { get; set; }
        [Required(ErrorMessage = "this field is required")]
        public int max { get; set; }
    }
}
