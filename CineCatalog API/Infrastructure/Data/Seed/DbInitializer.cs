using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;

namespace CineCatalog_API.Infrastructure.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<CineCatalogDbContext>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();

            // 1. Automatically apply any pending EF Migrations
            await context.Database.MigrateAsync();

            // One-time database cleanup
            var flagPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db_cleaned.flag");
            if (!File.Exists(flagPath))
            {
                context.Favorites.RemoveRange(context.Favorites);
                context.Reviews.RemoveRange(context.Reviews);
                context.Movies.RemoveRange(context.Movies);
                context.Users.RemoveRange(context.Users);
                context.Genres.RemoveRange(context.Genres);
                await context.SaveChangesAsync();

                // Create flag file
                await File.WriteAllTextAsync(flagPath, "cleaned");
            }

            // 2. Seed Genres
            if (!await context.Genres.AnyAsync())
            {
                var genres = new List<Genre>
                {
                    new Genre { Id = Guid.NewGuid(), Name = "Ação" },
                    new Genre { Id = Guid.NewGuid(), Name = "Drama" },
                    new Genre { Id = Guid.NewGuid(), Name = "Ficção Científica" },
                    new Genre { Id = Guid.NewGuid(), Name = "Terror" },
                    new Genre { Id = Guid.NewGuid(), Name = "Romance" },
                    new Genre { Id = Guid.NewGuid(), Name = "Comédia" }
                };

                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }
        }
    }
}
