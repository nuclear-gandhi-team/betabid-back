namespace Betabid.Features.Common.Options;

public class JwtOptions
{
    public string Secret { get; set; } = default!;

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;
}