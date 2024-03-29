using Microsoft.AspNetCore.Http;

namespace Betabid.Application.DTOs.LotsDTOs;

public class AddLotDto
{
    public string Name { get; set; } = null!;

    public decimal StartPrice { get; set; }

    public string Description { get; set; } = null!;

    public DateTime DateStarted { get; set; }

    public DateTime Deadline { get; set; }

    public decimal BetStep { get; set; }

    public List<int> TagIds { get; set; } = new();
}