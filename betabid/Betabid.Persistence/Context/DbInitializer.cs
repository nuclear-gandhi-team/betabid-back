using Betabid.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Context;

public class DbInitializer
{
    public static void SeedTags(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
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
                });
    }
}