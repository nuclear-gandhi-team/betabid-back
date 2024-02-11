using Betabid.Application.DTOs.BetDtos;

namespace Betabid.Application.DTOs.LotsDTOs;

public class GetLotDto
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public IEnumerable<string> Images { get; set; } = default!;

    public IEnumerable<string> Tags { get; set; } = default!;

    public string Status { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime DateStarted { get; set; }

    public DateTime Deadline { get; set; }

    public decimal StartPrice { get; set; }

    public decimal CurrentPrice { get; set; }
    
    public decimal MinNextPrice { get; set; }
    
    public int ActiveBetsCount { get; set; }
    
    public int ActiveUsersCount { get; set; }

    public decimal MinBetStep { get; set; }
    
    public string OwnerName { get; set; } = default!;

    public IEnumerable<GetBetDto> BidHistory { get; set; } = default!;

    public bool IsSaved { get; set; }
}