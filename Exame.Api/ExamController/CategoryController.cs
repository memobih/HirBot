using Exame.Services.DataTransferObjects;
using Exame.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Api.ExamController
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CategoryDto dto)
        {
            var response = await _categoryService.create(dto);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message, response.Data });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryDto dto)
        {
            var response = await _categoryService.update(id, dto);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message ,response.Data});

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryService.Delete(id);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string ?search=null)
        {
            var response = await _categoryService.GetALL(search);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message, data = response.Data });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _categoryService.GetCategory(id);
            if (response.StatusCode == 200)
                return Ok(new { status = true, message = response.Message, data = response.Data });

            return StatusCode(response.StatusCode, new { status = false, message = response.Message });
        }


    }

}
