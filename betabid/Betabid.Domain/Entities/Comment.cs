using System.ComponentModel.DataAnnotations.Schema;
using Betabid.Domain.Common;

namespace Betabid.Domain.Entities;

public class Comment : BaseEntity
{
    public string Body { get; set; } = default!;

    [ForeignKey(nameof(Lot))]
    public int LotId { get; set; }

    public Lot Lot { get; set; } = default!;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public int? ParentCommentId { get; set; }
}