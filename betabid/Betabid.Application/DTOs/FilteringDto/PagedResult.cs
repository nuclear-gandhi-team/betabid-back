namespace Betabid.Application.DTOs.FilteringDto;

public class PagedResult<T>
{
    public List<T> Items { get; set; }

    public int TotalPages { get; set; }
}