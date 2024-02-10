using System.ComponentModel.DataAnnotations;

namespace Betabid.Application.DTOs.CommentDtos;

public record CreateCommentDto
{
    [Required]
    public int LotId { get; set; }

    public int? ParentCommentId { get; set; }

    [Required]
    public string Body { get; set; } = default!;
}