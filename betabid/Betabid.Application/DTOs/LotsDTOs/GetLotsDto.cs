namespace Betabid.Application.DTOs.LotsDTOs;

public class GetLotsDto
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public DateTime Deadline { get; set; }
    
    public decimal CurrentPrice { get; set; }
    
    //public decimal SoldFor { get; set; }
    
    public List<string> Tags { get; set; } = new List<string>();
    
    public string Status { get; set; }
    
    public bool IsSaved { get; set; }

    public string Image { get; set; }
}