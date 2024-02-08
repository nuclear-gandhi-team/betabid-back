using Betabid.Domain.Enums;

namespace Betabid.Application.DTOs.FilteringDto;

public class FilteringOptionsDto : IPageable
{
    public string? NameStartsWith { get; set; }
    
    public SortingOptions? PriceOrder { get; set; } 
    
    public IList<string>? Tags { get; set; }
    
    public LotStatus? Status { get; set; }
    
    public int? Page { get; set; }
    
    public int PageCount { get; set; } = 10;
}