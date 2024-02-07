using System.ComponentModel.DataAnnotations;

namespace Betabid.Features.UserFeatures;

public record UpdateUserDto
{
    [Required]
    public string Id { get; set; } = default!;
    
    public string? NewName { get; set; }

    public string? NewEmail { get; set; }
}