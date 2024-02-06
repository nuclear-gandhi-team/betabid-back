using Microsoft.AspNetCore.Identity;

namespace Betabid.Domain.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = default!;

    public decimal Balance { get; set; }

    public virtual IList<Lot> Lots { get; set; } = default!;

    public virtual IList<Bet> Bets { get; set; } = default!;

    public virtual IEnumerable<Saved>? Saved { get; set; } = default!;
}