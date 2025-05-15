using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.DataTransferObjects
{
    public  class CategoryDto
    { 
        public IFormFile   ? image {  get; set; }
        [Required(ErrorMessage ="this field is Required")]
        public string Name { get; set; }
        public string  ? Description { get; set; }
    }
}
