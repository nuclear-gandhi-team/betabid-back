using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using Betabid.Domain.Common;

namespace Betabid.Domain.Entities;

public class Lot : BaseEntity
{
    public string Name { get; set; } = default!;

    public decimal StartPrice { get; set; }

    public string Description { get; set; } = default!;

    public DateTime DateStarted { get; set; }

    public DateTime Deadline { get; set; }

    public decimal BetStep { get; set; }

    [ForeignKey(nameof(Owner))]
    public string OwnerId { get; set; } = default!;

    public User Owner { get; set; } = default!;

    public virtual IList<Tag> Tags { get; set; } = default!;
    
    public virtual IList<Picture> Pictures { get; set; } = default!;
    
    public virtual IEnumerable<Saved>? SavedBy { get; set; } = default!;
    
    public virtual IList<Bet> Bets { get; set; } = default!;
}