using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.images;
using User.Services.Interfaces;

namespace User.Api.Controllers.images
{
    [Route("api/[controller]")]
    [ApiController]

    public class ImageController : ApiControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IIamge _image;
        public ImageController (IAuthenticationService authenticationService , IIamge Image)
        {
            _authenticationService = authenticationService;
            _image = Image;
        }


        [HttpPost("editProfileImage")]
        [Authorize] 
        public async Task<IActionResult> editProfileImage([FromForm] ImageDto image)
        {
            bool result = await _image.editProfileImage(image);
            if (result) return Ok();
            else return BadRequest();
        }
        [HttpDelete("deleteProfileImage")]
        [Authorize]
        public async Task<IActionResult> deleteProfileImage()
        {
            bool result = await _image.deleteProfileImage();
            if (result) return Ok();
            else return BadRequest();
        }

        [HttpPost("editCoverImage")]
        [Authorize]
        public async Task<IActionResult> editCoverImage([FromForm] ImageDto image)
        {
            bool result = await _image.editCoverImage(image);
            if (result) return Ok();
            else return BadRequest();
        }
        [HttpDelete("deleteCoverImage")]
        [Authorize]
        public async Task<IActionResult> deleteCoverImage()
        {
            bool result = await _image.deleteCoverImage();
            if (result) return Ok();
            else return BadRequest();
        }
    }
}
