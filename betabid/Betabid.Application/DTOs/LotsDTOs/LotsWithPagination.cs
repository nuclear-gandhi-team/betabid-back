namespace Betabid.Application.DTOs.LotsDTOs;

public class LotsWithPagination
{
    public IEnumerable<GetLotsDto> Lots { get; set; }
    
    public int TotalPages { get; set; }
    
    public int CurrentPage { get; set; }
}