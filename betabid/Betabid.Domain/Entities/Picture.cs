using System.ComponentModel.DataAnnotations.Schema;
using Betabid.Domain.Common;

namespace Betabid.Domain.Entities;

public class Picture : BaseEntity
{
    public byte[]? Data { get; set; } = default!;

    [ForeignKey(nameof(Lot))]
    public int LotId { get; set; }

    public Lot Lot { get; set; } = default!;
}