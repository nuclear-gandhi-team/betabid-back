using Betabid.Application.DTOs.BetDtos;

namespace Betabid.Application.DTOs.LotsDTOs;

public class GetLotDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public IEnumerable<string> Images { get; set; }

    public IEnumerable<string> Tags { get; set; }

    public string Status { get; set; } = "Pending";

    public string Description { get; set; }

    public DateTime DateStarted { get; set; }

    public DateTime Deadline { get; set; }

    public decimal StartPrice { get; set; }

    public decimal CurrentPrice { get; set; }
    
    //public decimal SoldFor { get; set; }

    public decimal MinBetStep { get; set; }
    
    public string OwnerUsername { get; set; }

    public IEnumerable<BetDto> BidHistory { get; set; }

    public bool IsSaved { get; set; }
}