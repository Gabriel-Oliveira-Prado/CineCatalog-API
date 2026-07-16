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
            // Limpa filmes existentes para forçar a atualização com as capas reais, classificações e plataformas
            var existingMovies = await context.Movies.ToListAsync();
            if (existingMovies.Any())
            {
                context.Movies.RemoveRange(existingMovies);
                await context.SaveChangesAsync();
            }

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
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=R_VX0zYnxPM",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
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
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzdjMDAxODItMjVjMi00ODAyLTkanYtYzUxMjI0Xi03NDBkXkFtZTgwMDUwMzM1MjE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
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
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"}]",
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
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzQzOTk3OTAtNDQ0Zi00ZTVkLWI0MTEtMDlkQjRkNzc0ZTkyXkFtZTgwNTkyMDc5MTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=m8e-FF8MsqU",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
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
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZWFlYmY2M2ItZjViNi00MzVlLTg2MTktYWJhYTkyOWFjMTE0XkFtZTcwNjY1NDcyMQ@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=5Cb3ik6zP2I",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"}]",
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
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMDliOTIzNmUtODc0Mi00MDY1LTkyYjktMDMyYjkyYjg3ODk3XkFtZTcwOTkwNDIyMw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=P5ieIbInFpg",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Telecine\",\"url\":\"https://www.telecine.com.br/\"}]",
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
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDIzNDU0NzEtYzUxYy00ZGQ5LTk5ODAtNWE4YmJkZmIyNzQ1XkFtZTcwNDUxMjc5Mg@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=Fs0E69krO_Y",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"}]",
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
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjA0Nzg1ODExM15BMl5BanBnXkFtZTcwMjcxMTk2Mw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=Q_2AbjYeSMI",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
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
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTkxMTA5OTAzMl5BMl5BanBnXkFtZTgwNjA5MDc3NjE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=s7EdQ4FqbhY",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Telecine\",\"url\":\"https://www.telecine.com.br/\"}]",
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
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTc5MDE2ODcwNV5BMl5BanBnXkFtZTgwMzI2NzQ2NzM@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=TcMBFSGVi1c",
                    StreamingPlatforms = "[{\"name\":\"Disney+\",\"url\":\"https://www.disneyplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "La La Land: Cantando Estações",
                    Description = "Um pianista de jazz e uma aspirante a atriz se apaixonam enquanto tentam fazer sucesso em Los Angeles.",
                    Synopsis = "Ao chegarem em Los Angeles, Sebastian, um pianista de jazz, e Mia, uma actress iniciante, cruzam caminhos e se apaixonam. Enquanto buscam oportunidades para suas carreiras, eles tentam manter o relacionamento amoroso aceso em meio à competitividade do show business.",
                    Director = "Damien Chazelle",
                    Cast = "Ryan Gosling, Emma Stone, John Legend",
                    ReleaseYear = 2016,
                    DurationMinutes = 128,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMzUzNDM2NzM2MV5BMl5BanBnXkFtZTgwNTM3NTg4OTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=0pdqf4P9MB8",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { romanceGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Forrest Gump",
                    Description = "A vida extraordinária de um homem simples do Alabama.",
                    Synopsis = "As presidências de Kennedy e Johnson, os eventos do Vietnã, Watergate e outras histórias se desenrolam sob a perspectiva de um homem do Alabama com um QI de 75.",
                    Director = "Robert Zemeckis",
                    Cast = "Tom Hanks, Robin Wright, Gary Sinise",
                    ReleaseYear = 1994,
                    DurationMinutes = 142,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYThjM2UxMDUtN2Y1YS00NzE1LWIwOTUtMWFhNWJhNzAxMDZhXkFtZTgwMTk0ODMyMTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=bLvqoHBptjg",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre!, romanceGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Poderoso Chefão",
                    Description = "O clássico de máfia de Francis Ford Coppola.",
                    Synopsis = "O patriarca idoso de uma dinastia do crime organizado transfere o controle de seu império clandestino para seu filho relutante.",
                    Director = "Francis Ford Coppola",
                    Cast = "Marlon Brando, Al Pacino, James Caan",
                    ReleaseYear = 1972,
                    DurationMinutes = 175,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BM2MyNjYxNmUtYTAwZi00MTYxLWJmNWYtYzZlODY3ZTk3OTFlXkFtZTgwNzM4MzM1MjE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=sY1S34973zA",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Paramount+\",\"url\":\"https://www.paramountplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Senhor dos Anéis: A Sociedade do Anel",
                    Description = "A primeira parte da jornada épica da Terra-média.",
                    Synopsis = "Um manso hobbit do Condado e oito companheiros partem em uma jornada para destruir o poderoso Um Anel e salvar a Terra-média.",
                    Director = "Peter Jackson",
                    Cast = "Elijah Wood, Ian McKellen, Orlando Bloom, Viggo Mortensen",
                    ReleaseYear = 2001,
                    DurationMinutes = 178,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BN2EyZjM3NzUtNWUzMi00MTgxLWI0NTctMzY4M2VlOTdjZWRiXkFtZTgwMjM5MTM1MjE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=V75dMMBU2K0",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Cidade de Deus",
                    Description = "Obra-prima nacional sobre a violência urbana no Rio de Janeiro.",
                    Synopsis = "Nas favelas do Rio de Janeiro, dois caminhos de jovens divergem: um se torna fotógrafo e o outro traficante.",
                    Director = "Fernando Meirelles, Kátia Lund",
                    Cast = "Alexandre Rodrigues, Leandro Firmino, Seu Jorge",
                    ReleaseYear = 2002,
                    DurationMinutes = 130,
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMGUwZjliMTUtMWM1Yi00NGFiLWEwODctNDUzYmY3N2FmODA2XkFtZTcwODUxNy15Bg@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=gpv7sEs_8w0",
                    StreamingPlatforms = "[{\"name\":\"Globoplay\",\"url\":\"https://globoplay.globo.com/\"},{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Rei Leão",
                    Description = "Clássico animado da Disney sobre o ciclo da vida.",
                    Synopsis = "Um jovem leão é enganado por seu tio orgulhoso e foge após a morte do pai, aprendendo o real significado de responsabilidade.",
                    Director = "Roger Allers, Rob Minkoff",
                    Cast = "Matthew Broderick, Jeremy Irons, James Earl Jones",
                    ReleaseYear = 1994,
                    DurationMinutes = 88,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYTYxNGMyZTYtYTg3Yy00OGYwLTg5YTAtNGEwMDU2NDFlM2RlXkFtZTgwOTg1ODc1MDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=lFzVJEksoDY",
                    StreamingPlatforms = "[{\"name\":\"Disney+\",\"url\":\"https://www.disneyplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Se Beber, Não Case!",
                    Description = "Comédia insana sobre ressaca extrema e mistério.",
                    Synopsis = "Três amigos acordam de uma despedida de solteiro em Las Vegas sem memória da noite anterior e sem o noivo.",
                    Director = "Todd Phillips",
                    Cast = "Bradley Cooper, Ed Helms, Zach Galifianakis",
                    ReleaseYear = 2009,
                    DurationMinutes = 100,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTnjNDU2NTEtNzA3OS00YmQ1LTlhNTctMDkyYTc5M2JhNDc3XkFtZTcwNTI1NjUyMg@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=tcdUhdOlz9M",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Parasita",
                    Description = "Vencedor do Oscar de Melhor Filme sobre desigualdade social.",
                    Synopsis = "A ganância e a discriminação de classe ameaçam a recém-descoberta relação simbiótica entre a rica família Park e o clã Kim.",
                    Director = "Bong Joon Ho",
                    Cast = "Song Kang-ho, Lee Sun-kyun, Cho Yeo-jeong",
                    ReleaseYear = 2019,
                    DurationMinutes = 132,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYWZjMjk3ZTItODQ2ZC00NTY5LWE0ZDYtZTI3MjcwN2Q5NTVkXkFtZTgwNTk5MDM5NDM@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=m4ncgLyAj9g",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre!, comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Coringa",
                    Description = "O trágico estudo de personagem do icônico vilão.",
                    Synopsis = "Em Gotham City, o comediante Arthur Fleck, negligenciado pela sociedade, inicia uma espiral de loucura e revolta.",
                    Director = "Todd Phillips",
                    Cast = "Joaquin Phoenix, Robert De Niro, Zazie Beetz",
                    ReleaseYear = 2019,
                    DurationMinutes = 122,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYmQyNTA1ZGItNjZjMi00NzFlLWIyM2UtNzFhNjQ5YmLiYTY1XkFtZTgwODQ5Nzg3NzM@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=jGP7LYF3Fns",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Duna",
                    Description = "Nova e grandiosa adaptação do clássico de ficção científica.",
                    Synopsis = "Paul Atreides, um jovem brilhante e talentoso nascido para um grande destino além de sua compreensão, deve viajar para o planeta mais perigoso do universo.",
                    Director = "Denis Villeneuve",
                    Cast = "Timothée Chalamet, Rebecca Ferguson, Zendaya, Oscar Isaac",
                    ReleaseYear = 2021,
                    DurationMinutes = 155,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMzU0NDY0NDM4NV5BMl5BanBnXkFtZTgwOTMxMTY2NTM@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=n9xhJrPXy4g",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Homem-Aranha: Sem Volta Para Casa",
                    Description = "A aventura épica que reúne gerações de Homens-Aranha.",
                    Synopsis = "Com a identidade do Homem-Aranha revelada, Peter pede ajuda ao Doutor Estranho, mas o feitiço dá errado e abre o multiverso.",
                    Director = "Jon Watts",
                    Cast = "Tom Holland, Zendaya, Tobey Maguire, Andrew Garfield",
                    ReleaseYear = 2021,
                    DurationMinutes = 148,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZWMyYzFjYTYtNTRjYi00OGExLWE2YzgtOGRmYjAxZTU3NzBiXkFtZTgwNzMyNDY1MDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=wzDQC-LgH4k",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Max\",\"url\":\"https://www.max.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Silêncio dos Inocentes",
                    Description = "Um thriller psicológico tenso e aclamado.",
                    Synopsis = "Uma jovem cadete do FBI pede ajuda de um assassino canibal encarcerado para capturar outro serial killer.",
                    Director = "Jonathan Demme",
                    Cast = "Jodie Foster, Anthony Hopkins, Scott Glenn",
                    ReleaseYear = 1991,
                    DurationMinutes = 118,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BHNDdhOGYtNzQyZi00YTc5LTg2N2UtYjlkY2ZhYWIyMGRjXkFtZTgwNTExNzY3MjE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=W6Mm8SowbEk",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { horrorGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Titanic",
                    Description = "O épico romance de James Cameron.",
                    Synopsis = "Uma aristocrata de dezessete anos se apaixona por um artista gentil, mas pobre, a bordo do luxuoso R.M.S. Titanic.",
                    Director = "James Cameron",
                    Cast = "Leonardo DiCaprio, Kate Winslet, Billy Zane",
                    ReleaseYear = 1997,
                    DurationMinutes = 194,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMDdmZGU3NDQtY2E5My00ZTliLWIzOTUtMTY4ZGI1NWIwXkFtZTcwMTI0NzE2MQ@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=cUTtS5v9b1c",
                    StreamingPlatforms = "[{\"name\":\"Disney+\",\"url\":\"https://www.disneyplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { romanceGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Star Wars: Uma Nova Esperança",
                    Description = "O filme original da maior saga espacial do cinema.",
                    Synopsis = "Luke Skywalker se junta a um cavaleiro Jedi, um piloto arrogante, um Wookiee e dois droides para salvar a galáxia da estação de batalha do Império.",
                    Director = "George Lucas",
                    Cast = "Mark Hamill, Harrison Ford, Carrie Fisher",
                    ReleaseYear = 1977,
                    DurationMinutes = 121,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTA5NjhiOTAtMWVjNC00YTg4LTlmYTUtYmI1NDFiMGIyYTlhXkFtZTgwMzUxMTkyNTM@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=yfhUNOKsHIA",
                    StreamingPlatforms = "[{\"name\":\"Disney+\",\"url\":\"https://www.disneyplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { sciFiGenre!, actionGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Shrek",
                    Description = "A clássica e irônica animação da DreamWorks.",
                    Synopsis = "Um ogro tem sua paz invadida por personagens de contos de fadas e faz um acordo para resgatar uma princesa.",
                    Director = "Andrew Adamson, Vicky Jenson",
                    Cast = "Mike Myers, Eddie Murphy, Cameron Diaz",
                    ReleaseYear = 2001,
                    DurationMinutes = 90,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BODlhZTA0ODMtYzhkMy00OTc5LWEyODItYTM0YWNjOWRmY2NhXkFtZTgwNjMyNTg4OTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=OoRdp5Sdfc0",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Resgate do Soldado Ryan",
                    Description = "O impactante retrato de guerra de Steven Spielberg.",
                    Synopsis = "Durante a Segunda Guerra Mundial, um grupo de soldados americanos parte em uma perigosa missão para trazer de volta para casa um soldado cujos irmãos morreram em combate.",
                    Director = "Steven Spielberg",
                    Cast = "Tom Hanks, Matt Damon, Tom Sizemore",
                    ReleaseYear = 1998,
                    DurationMinutes = 169,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTUxMzQyNjA5MF5BMl5BanBnXkFtZTcwOTU2NTY3Ng@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=9Bh35e78B64",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                }
            };

            foreach (var movie in candidateMovies)
            {
                await context.Movies.AddAsync(movie);
            }
            await context.SaveChangesAsync();
        }
    }
}
