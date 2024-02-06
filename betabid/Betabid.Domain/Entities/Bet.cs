using System.ComponentModel.DataAnnotations.Schema;
using Betabid.Domain.Common;

namespace Betabid.Domain.Entities;

public class Bet : BaseEntity
{
    public DateTime Time { get; set; }

    public decimal Amount { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = default!;

    public virtual User User { get; set; } = default!;
    
    [ForeignKey(nameof(Lot))]
    public int LotId { get; set; }

    public virtual Lot Lot { get; set; } = default!;
}