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
            var actionGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Ação");
            var sciFiGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Ficção Científica");
            var dramaGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Drama");
            var horrorGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Terror");
            var romanceGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Romance");
            var comedyGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Comédia");

            var candidateMovies = new List<Movie>
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
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Matrix",
                    Description = "Um jovem programador é atraído para uma rebelião cibernética contra as máquinas que controlam a realidade humana.",
                    Synopsis = "O jovem programador Thomas Anderson é atormentado por estranhos pesadelos em que se vê conectado por cabos a um computador central. À medida que o sonho se repete, ele começa a ter dúvidas sobre a realidade. Ao encontrar os rebeldes liderados por Morpheus, ele descobre que é a vítima de um sistema inteligente chamado Matrix.",
                    Director = "Lana Wachowski, Lilly Wachowski",
                    Cast = "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss",
                    ReleaseYear = 1999,
                    DurationMinutes = 136,
                    Rating = "14+",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=m8e-FF8MsqU",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Iluminado",
                    Description = "Um escritor se torna o zelador de um hotel isolado e começa a perder a sanidade devido a forças sobrenaturais.",
                    Synopsis = "Jack Torrance consegue um emprego como zelador de inverno no isolado Hotel Overlook. Ele se muda com sua esposa Wendy e seu filho Danny, que possui poderes psíquicos. Com o passar do tempo, as forças sobrenaturais do hotel começam a influenciar Jack até a insanidade.",
                    Director = "Stanley Kubrick",
                    Cast = "Jack Nicholson, Shelley Duvall, Danny Lloyd",
                    ReleaseYear = 1980,
                    DurationMinutes = 146,
                    Rating = "16+",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1509248961158-e54f6934749c?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=5Cb3ik6zP2I",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { horrorGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Gladiador",
                    Description = "Um ex-general romano busca vingança contra o imperador corrupto que assassinou sua família e o enviou à escravidão.",
                    Synopsis = "Nos últimos dias do reinado de Marco Aurélio, o imperador enfurece seu filho Commodus ao escolher o general Maximus como seu herdeiro. Commodus mata seu pai, assume o poder e ordena a morte de Maximus e de sua família. Maximus consegue escapar, mas é capturado como escravo e treinado como gladiador.",
                    Director = "Ridley Scott",
                    Cast = "Russell Crowe, Joaquin Phoenix, Connie Nielsen",
                    ReleaseYear = 2000,
                    DurationMinutes = 155,
                    Rating = "14+",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1558591710-4b4a1ae0f04d?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=P5ieIbInFpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Clube da Luta",
                    Description = "Um trabalhador insone e um vendedor de sabão criam um clube de lutas clandestino.",
                    Synopsis = "Um homem deprimido que sofre de insônia conhece um vendedor de sabão chamado Tyler Durden. Juntos, eles criam uma sociedade secreta onde homens lutam para extravasar suas frustrações, o Clube da Luta, que logo se expande em uma organização anarquista de proporções gigantescas.",
                    Director = "David Fincher",
                    Cast = "Brad Pitt, Edward Norton, Helena Bonham Carter",
                    ReleaseYear = 1999,
                    DurationMinutes = 139,
                    Rating = "18+",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1579783900882-c0d3dad7b119?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=Fs0E69krO_Y",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Como Se Fosse a Primeira Vez",
                    Description = "Um veterinário marinho tenta conquistar uma mulher com perda de memória recente todos os dias.",
                    Synopsis = "Henry Roth é um veterinário marinho conquistador que se apaixona por Lucy Whitmore. No entanto, Lucy sofre de perda de memória recente devido a um acidente de carro, esquecendo de tudo o que aconteceu no dia anterior. Henry precisa bolar novas formas de conquistá-la todos os dias.",
                    Director = "Peter Segal",
                    Cast = "Adam Sandler, Drew Barrymore, Rob Schneider",
                    ReleaseYear = 2004,
                    DurationMinutes = 99,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1518199266791-5375a83190b7?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=Q_2AbjYeSMI",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre!, romanceGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Pulp Fiction",
                    Description = "Várias histórias de crime e redenção se entrelaçam nos subúrbios de Los Angeles.",
                    Synopsis = "Os caminhos de vários criminosos, incluindo assassinos profissionais, a esposa de um gângster, um boxeador pago para perder e uma dupla de assaltantes de restaurante, se cruzam em uma narrativa não-linear repleta de diálogos marcantes e violência estilizada.",
                    Director = "Quentin Tarantino",
                    Cast = "John Travolta, Uma Thurman, Samuel L. Jackson, Bruce Willis",
                    ReleaseYear = 1994,
                    DurationMinutes = 154,
                    Rating = "18+",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1594909122845-11baa439b7bf?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=s7EdQ4FqbhY",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Vingadores: Ultimato",
                    Description = "Os heróis sobreviventes tentam reverter o estalo dizimador de Thanos.",
                    Synopsis = "Após Thanos eliminar metade de toda a vida no universo, os Vingadores sobreviventes se reúnem mais uma vez para reverter as ações do Titã Louco e restaurar o equilíbrio do universo de uma vez por todas, custe o que custar.",
                    Director = "Anthony Russo, Joe Russo",
                    Cast = "Robert Downey Jr., Chris Evans, Mark Ruffalo, Chris Hemsworth",
                    ReleaseYear = 2019,
                    DurationMinutes = 181,
                    Rating = "12+",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1635805737707-575885ab0820?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=TcMBFSGVi1c",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "La La Land: Cantando Estações",
                    Description = "Um pianista de jazz e uma aspirante a atriz se apaixonam enquanto tentam fazer sucesso em Los Angeles.",
                    Synopsis = "Ao chegarem em Los Angeles, Sebastian, um pianista de jazz, e Mia, uma atriz iniciante, cruzam caminhos e se apaixonam. Enquanto buscam oportunidades para suas carreiras, eles tentam manter o relacionamento amoroso aceso em meio à competitividade do show business.",
                    Director = "Damien Chazelle",
                    Cast = "Ryan Gosling, Emma Stone, John Legend",
                    ReleaseYear = 2016,
                    DurationMinutes = 128,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=500",
                    TrailerUrl = "https://www.youtube.com/watch?v=0pdqf4P9MB8",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { romanceGenre!, dramaGenre! }
                }
            };

            foreach (var movie in candidateMovies)
            {
                if (!await context.Movies.AnyAsync(m => m.Title == movie.Title))
                {
                    await context.Movies.AddAsync(movie);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
