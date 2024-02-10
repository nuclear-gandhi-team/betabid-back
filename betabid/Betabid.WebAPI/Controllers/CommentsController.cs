using Betabid.Application.DTOs.CommentDtos;
using Betabid.Application.Services.Interfaces;
using betabid.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace betabid.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> AddCommentAsync([FromBody] CreateCommentDto createCommentDto)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        
        await _commentService.AddCommentAsync(createCommentDto, userId);

        return Ok();
    }

    [HttpGet]
    [Route("games/{lotId:int}")]
    public async Task<IActionResult> GetAllCommentsAsync(int lotId)
    {
        var comments = await _commentService.GetAllLotCommentsAsync(lotId);

        return Ok(comments);
    }

}