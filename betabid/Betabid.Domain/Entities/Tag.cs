using Betabid.Domain.Common;

namespace Betabid.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = default!;

    public virtual IList<Lot> Lots { get; set; }
}