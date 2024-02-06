using Betabid.Domain.Common;

namespace Betabid.Domain.Entities;

public class Saved : BaseEntity
{
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public int LotId { get; set; }
    public Lot Lot { get; set; } = default!;
}