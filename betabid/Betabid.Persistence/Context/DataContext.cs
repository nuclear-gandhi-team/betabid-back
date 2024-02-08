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

        builder.Entity<Tag>()
            .HasData(
                new Tag
                {
                    Id = 1,
                    Name = "Rare Collectible",
                },
                
                new Tag
                {
                    Id = 2,
                    Name = "Luxury Experience",
                },
                
                new Tag
                {
                    Id = 3,
                    Name = "Vintage Treasure",
                },
                
                new Tag
                {
                    Id = 4,
                    Name = "Fine Art",
                },
                
                new Tag
                {
                    Id = 5,
                    Name = "Exclusive Getaway",
                }, 
                
                new Tag
                {
                    Id = 6,
                    Name = "Handcrafted Goods",
                },
                
                new Tag
                {
                    Id = 7,
                    Name = "Signed Memorabilia",
                },
                
                new Tag
                {
                    Id = 8,
                    Name = "Gourmet Delights",
                },
                
                new Tag
                {
                    Id = 9,
                    Name = "Technological Marvel",
                },
                
                new Tag
                {
                    Id = 10,
                    Name = "Historical Artifact",
                }
                );
    }
}