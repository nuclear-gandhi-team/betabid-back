namespace Betabid.Application.DTOs.BetDtos;

public class BetDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Username { get; set; }
    
    public DateTime Time { get; set; }
}