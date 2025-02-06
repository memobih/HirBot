using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.images
{
    public  class ImageDto
    {
        [Required (ErrorMessage ="this field is rquired") ]
        [Base64String (ErrorMessage ="can not uplode this file")]
        public string base64Data {  get; set; }
    }
}
