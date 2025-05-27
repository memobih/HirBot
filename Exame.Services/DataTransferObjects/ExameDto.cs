using HirBot.Data.Entities;
using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Exame.Services.DataTransferObjects
{
    public class ExameDto
    {
        [Required(ErrorMessage ="the name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "the duration is required")]

        public int duration { get; set; }
        [Required(ErrorMessage = "the duration is required")]

        public int CategoryID { get; set; } 
        

    }
}
