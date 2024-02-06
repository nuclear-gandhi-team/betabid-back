using Betabid.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Context;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Bet> Bets { get; set; }
    
    public virtual DbSet<Lot> Lots { get; set; }
    
    public virtual DbSet<Picture> Pictures { get; set; }
    
    public virtual DbSet<Tag> Tags { get; set; }
    
    public virtual DbSet<Saved> Saved { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Lot>()
            .HasOne(l => l.Owner)
            .WithMany(u => u.Lots)
            .HasForeignKey(l => l.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Saved>()
            .HasOne(ul => ul.User)
            .WithMany(u => u.Saved)
            .HasForeignKey(ul => ul.UserId);
        
        builder.Entity<Saved>()
            .HasOne(ul => ul.Lot)
            .WithMany(l => l.SavedBy)
            .HasForeignKey(ul => ul.LotId);
    }
}