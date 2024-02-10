using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Betabid.Application.DTOs.CommentDtos;

public record GetCommentDto
{
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string UserName { get; set; } = default!;
    
    public string Body { get; set; } = default!;

    public IEnumerable<GetCommentDto> ChildComments { get; set; } = default!;

}