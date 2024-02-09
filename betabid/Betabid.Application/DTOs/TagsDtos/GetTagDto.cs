namespace Betabid.Application.DTOs.TagsDtos;

public record GetTagDto
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
}