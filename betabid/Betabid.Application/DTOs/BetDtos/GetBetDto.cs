namespace Betabid.Application.DTOs.BetDtos;

public class GetBetDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Username { get; set; } = default!;
    
    public DateTime Time { get; set; }
    
    public string UserEmail { get; set; } = default!;
}