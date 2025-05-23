using Exame.Services.DataTransferObjects;
using Exame.Services.Interfaces;
using HirBot.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.ResponseHandler.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuestionController : ApiControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost("{examID}")]
    public async Task<IActionResult> Create(int examID, [FromBody] QuestionDto dto)
    {
        var response = await _questionService.Create(examID, dto);
        if (response.StatusCode == 200)
            return Ok(new { status = true, response.Data,message = response.Message });

        return StatusCode(response.StatusCode, new { status = false, message = response.Message });
    }

    [HttpPut("{questionId}")]
    public async Task<IActionResult> Edit(int questionId, [FromBody] QuestionDto dto)
    {
        var response = await _questionService.Edit(questionId, dto);
        if (response.StatusCode == 200)
            return Ok(new { status = true, response.Data,message = response.Message });

        return StatusCode(response.StatusCode, new { status = false, message = response.Message });
    }

    [HttpDelete("{questionId}")]
    public async Task<IActionResult> Delete(int questionId)
    {
        var response = await _questionService.Delete(questionId);
        if (response.StatusCode == 200)
            return Ok(new { status = true, message = response.Message });

        return StatusCode(response.StatusCode, new { status = false, message = response.Message });
    }

    [HttpGet("{questionId}")]
    public async Task<IActionResult> GetQuestion(int questionId)
    {
        var response = await _questionService.GetQuestion(questionId);
        if (response.StatusCode == 200)
            return Ok(new { status = true, data = response.Data });

        return StatusCode(response.StatusCode, new { status = false, message = response.Message });
    }
}
