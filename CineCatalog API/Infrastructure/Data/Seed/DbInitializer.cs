using System;
using System.Collections.Generic;
using System.Linq;
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

            // 3. Seed Default User (Admin / Test user)
            if (!await context.Users.AnyAsync())
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin CineCatalog",
                    Email = "admin@cinecatalog.com",
                    PasswordHash = passwordHasher.HashPassword("AdminPassword123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }

            // 4. Seed Movies
            if (!await context.Movies.AnyAsync())
            {
                var actionGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Ação");
                var sciFiGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Ficção Científica");
                var dramaGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Drama");

                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Id = Guid.NewGuid(),
                        Title = "A Origem",
                        Description = "Um ladrão que rouba segredos corporativos por meio do uso de tecnologia de compartilhamento de sonhos.",
                        Synopsis = "Dom Cobb é um ladrão habilidoso, o melhor na perigosa arte da extração, roubando segredos valiosos do fundo do subconsciente durante o estado de sonho. A habilidade rara de Cobb fez dele um jogador cobiçado no mundo da espionagem corporativa, mas também o tornou um fugitivo internacional. Agora, é oferecida a Cobb uma chance de redenção.",
                        Director = "Christopher Nolan",
                        Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt, Elliot Page, Tom Hardy",
                        ReleaseYear = 2010,
                        DurationMinutes = 148,
                        Rating = "14+",
                        AverageRating = 0.0,
                        ReviewsCount = 0,
                        ImageUrl = "https://images.unsplash.com/photo-1536440136628-849c177e76a1?w=500",
                        TrailerUrl = "https://www.youtube.com/watch?v=R_VX0zYnxPM",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                    },
                    new Movie
                    {
                        Id = Guid.NewGuid(),
                        Title = "Interestelar",
                        Description = "Uma equipe de exploradores viaja através de um buraco de minhoca no espaço em uma tentativa de garantir a sobrevivência da humanidade.",
                        Synopsis = "As reservas naturais da Terra estão se esgotando e um grupo de astronautas recebe a missão de verificar possíveis planetas para receberem a população mundial, possibilitando a continuação da espécie humana. Cooper é chamado para liderar o grupo e aceita a missão sabendo que pode nunca mais ver os filhos.",
                        Director = "Christopher Nolan",
                        Cast = "Matthew McConaughey, Anne Hathaway, Jessica Chastain, Michael Caine",
                        ReleaseYear = 2014,
                        DurationMinutes = 169,
                        Rating = "Livre",
                        AverageRating = 0.0,
                        ReviewsCount = 0,
                        ImageUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=500",
                        TrailerUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Genres = new List<Genre> { sciFiGenre!, dramaGenre! }
                    },
                    new Movie
                    {
                        Id = Guid.NewGuid(),
                        Title = "O Cavaleiro das Trevas",
                        Description = "Quando a ameaça conhecida como o Coringa surge do seu passado, ela causa estragos e caos no povo de Gotham.",
                        Synopsis = "Com a ajuda de Jim Gordon e Harvey Dent, Batman mantém a ordem em Gotham City. Mas um jovem e brilhante criminoso conhecido como Coringa chega para espalhar o caos e empurrar o herói de Gotham até o limite entre a justiça e o vigilantismo.",
                        Director = "Christopher Nolan",
                        Cast = "Christian Bale, Heath Ledger, Aaron Eckhart, Maggie Gyllenhaal",
                        ReleaseYear = 2008,
                        DurationMinutes = 152,
                        Rating = "12+",
                        AverageRating = 0.0,
                        ReviewsCount = 0,
                        ImageUrl = "https://images.unsplash.com/photo-1478760329108-5c3ed9d495a0?w=500",
                        TrailerUrl = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Genres = new List<Genre> { actionGenre!, dramaGenre! }
                    }
                };

                await context.Movies.AddRangeAsync(movies);
                await context.SaveChangesAsync();
            }
        }
    }
}
