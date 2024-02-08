namespace Betabid.Application.DTOs.LotsDTOs;

public class GetLotsDto
{
    public int Id { get; set; }
    
    public string Title { get; set; } = default!;

    public DateTime Deadline { get; set; }
    
    public decimal CurrentPrice { get; set; }
    
    public string Description { get; set; } = default!;
    
    public List<string> Tags { get; set; } = new();
    
    public string Status { get; set; } = default!;

    public bool IsSaved { get; set; }

    public string Image { get; set; } = default!;
}