using HirBot.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jop.Services.DataTransferObjects
{
    public class EditStatusDto
    {
        [Required(ErrorMessage ="this field is required")]
       public JobStatus status {  get; set; }
    }
}
