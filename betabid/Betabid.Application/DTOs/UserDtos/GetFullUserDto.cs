using System.ComponentModel.DataAnnotations;
using Betabid.Domain.Entities;

namespace Betabid.Application.DTOs.UserDtos;

public record GetFullUserDto
{
    [Required]
     public string Id { get; set; } = default!;

    [Required]
    public string Login { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    
    [Required]
    public decimal Balance { get; set; } = default!;

    public IList<Lot> SavedLots { get; set; } = new List<Lot>();
    
    public IList<Bet> Bets { get; set; } = new List<Bet>();
}