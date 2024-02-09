using Betabid.Domain.Enums;

namespace Betabid.Application.DTOs.FilteringDto;

public interface IPageable
{
    public int? Page { get; set; }

    public int PageCount { get; set; }
}