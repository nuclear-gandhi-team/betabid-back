using System.ComponentModel.DataAnnotations;

namespace Betabid.Features.UserFeatures;

public record LoginResponseDto
{
    [Required]
    public string Token { get; set; } = default!;
}