using System.ComponentModel.DataAnnotations;

namespace Betabid.Features.UserFeatures;

public record LoginUserDto
{
    [Required]
    public string Login { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}