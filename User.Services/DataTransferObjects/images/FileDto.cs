using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Services.DataTransferObjects.images
{
    public class FileDto
    {
        public IFormFile File { get; set; }

    }
}
