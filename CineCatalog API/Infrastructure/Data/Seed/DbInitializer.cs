using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;
using CineCatalog_API.Application.Interfaces;

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
                    Synopsis = "Dom Cobb é um ladrão habilidoso, o melhor na perigosa arte da extração, roubando segredos valiosos do fundo do subconsciente durante o estado de sonho. A habilidade rara de Cobb fez dele um jogador cobiçado no mundo da espionagem corporativa, mas também o tornou um fugitivo internacional.",
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
                    Description = "Um pianista de jazz e uma aspirante a actress se apaixonam enquanto tentam fazer sucesso em Los Angeles.",
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
                    Synopsis = "Um jovem leão é enganado por seu tio orgulhoso e fuge após a morte do pai, aprendendo o real significado de responsabilidade.",
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
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Um Sonho de Liberdade",
                    Description = "Dois homens presos se ligam ao longo de vários anos, encontrando consolo e eventual redenção através de atos de decência comum.",
                    Synopsis = "Andy Dufresne é um jovem e bem sucedido banqueiro cuja vida sofre uma reviravolta dramática quando é condenado por um crime que não cometeu: o homicídio da sua esposa e do amante. Ele é enviado para uma prisão de segurança máxima onde faz amizade com Red.",
                    Director = "Frank Darabont",
                    Cast = "Tim Robbins, Morgan Freeman, Bob Gunton",
                    ReleaseYear = 1994,
                    DurationMinutes = 142,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images-na.ssl-images-amazon.com/images/M/MV5BODU4MjU4NjIwNl5BMl5BanBnXkFtZTgwMDU2MjEyMDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=PLl99DlL6b4",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Beetlejuice",
                    Description = "Um casal de fantasmas contrata os serviços de um \"bio-exorcista\" para remover os proprietários obnóxios de sua antiga casa.",
                    Synopsis = "Depois de morrerem em um acidente de carro, Barbara e Adam Maitland se encontram presos, assombrando sua antiga casa de campo. Quando uma nova família se muda para lá, os Maitland tentam assustá-los, mas sem sucesso. Eles decidem contratar Beetlejuice para ajudar.",
                    Director = "Tim Burton",
                    Cast = "Alec Baldwin, Geena Davis, Michael Keaton",
                    ReleaseYear = 1988,
                    DurationMinutes = 92,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images-na.ssl-images-amazon.com/images/M/MV5BMTUwODE3MDE0MV5BMl5BanBnXkFtZTgwNTk1MjI4MzE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=2hovKm9oFiM",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Ratatouille",
                    Description = "Um rato que sabe cozinhar faz uma aliança incomum com um jovem trabalhador de cozinha.",
                    Synopsis = "Remy é um rato residente em Paris que sonha em se tornar um grande chef de cozinha. O problema é que sua família é contra e a cozinha não é o lugar ideal para um roedor. Ao se aliar com Linguini, um jovem lavador de pratos, Remy começa a realizar seu sonho.",
                    Director = "Brad Bird, Jan Pinkava",
                    Cast = "Patton Oswalt, Ian Holm, Lou Romano",
                    ReleaseYear = 2007,
                    DurationMinutes = 111,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images-na.ssl-images-amazon.com/images/M/MV5BMTMzODU0NTkxMF5BMl5BanBnXkFtZTcwMjQ4MzMzMw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=c3sBBRxDAqk",
                    StreamingPlatforms = "[{\"name\":\"Disney+\",\"url\":\"https://www.disneyplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Memento",
                    Description = "Um homem tenta encontrar o assassino de sua esposa enquanto lida com perda de memória recente.",
                    Synopsis = "Leonard Shelby está determinado a vingar o brutal assassinato de sua esposa. No entanto, sua busca é dificultada por uma forma rara e incurável de perda de memória recente. Embora ele se lembre de tudo antes do acidente, ele não consegue reter novas memórias por mais de alguns minutos.",
                    Director = "Christopher Nolan",
                    Cast = "Guy Pearce, Carrie-Anne Moss, Joe Pantoliano",
                    ReleaseYear = 2000,
                    DurationMinutes = 113,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://images-na.ssl-images-amazon.com/images/M/MV5BNThiYjM3MzktMDg3Yy00ZWQ3LTk3YWEtN2M0YmNmNWEwYTE3XkEyXkFqcGdeQXVyMTQxNzMzNDI@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=4CV41hoyS8A",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Avatar",
                    Description = "Um fuzileiro naval paraplégico enviado a uma lua alienígena se divide entre seguir suas ordens e proteger o mundo.",
                    Synopsis = "No exuberante mundo alienígena de Pandora vivem os Na'vi, seres que parecem primitivos, mas são altamente evoluídos. Jake Sully, um ex-fuzileiro naval confinado a uma cadeira de rodas, se torna um avatar humano para explorar o local, mas se apaixona por uma nativa e decide lutar por sua sobrevivência.",
                    Director = "James Cameron",
                    Cast = "Sam Worthington, Zoe Saldana, Sigourney Weaver",
                    ReleaseYear = 2009,
                    DurationMinutes = 162,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjA4NzU1NDA0OF5BMl5BanBnXkFtZTcwODA5MTU2Mw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=5PSNL1q3tVY",
                    StreamingPlatforms = "[{\"name\":\"Disney+\",\"url\":\"https://www.disneyplus.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { sciFiGenre!, actionGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Exterminador do Futuro 2",
                    Description = "Um ciborgue idêntico ao que tentou matar Sarah Connor é enviado de volta para proteger seu filho adolescente.",
                    Synopsis = "O jovem John Connor é a chave para a vitória dos humanos sobre as máquinas no futuro. Um novo exterminador de metal líquido, o T-1000, é enviado para eliminá-lo, mas os humanos reprogramam um antigo T-800 para voltar no tempo e protegê-lo.",
                    Director = "James Cameron",
                    Cast = "Arnold Schwarzenegger, Linda Hamilton, Edward Furlong",
                    ReleaseYear = 1991,
                    DurationMinutes = 137,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMGU2NzQyMTEtNGQ0OC00Y2UxLWIwNjMtM2NlMTg0Y2FjYTg0XkFtZTgwMDA5MTU5MDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=CRRl3-b68p8",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "De Volta para o Futuro",
                    Description = "Um adolescente viaja acidentalmente no tempo em um carro modificado por um cientista excêntrico.",
                    Synopsis = "Marty McFly é um adolescente comum que acidentalmente viaja no tempo de 1985 para 1955 em uma máquina do tempo construída em um DeLorean por seu amigo cientista Doc Brown. Ele precisa garantir que seus pais se conheçam para não apagar sua própria existência.",
                    Director = "Robert Zemeckis",
                    Cast = "Michael J. Fox, Christopher Lloyd, Lea Thompson",
                    ReleaseYear = 1985,
                    DurationMinutes = 116,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZmU0M2Y1OGUtZjIxNi00ZjBkLTg1MjgtOWIyODFjNWYwYjMxXkFtZTgwNjU4MjQ4MTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=yofn4BfdbW8",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { sciFiGenre!, comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Jurassic Park",
                    Description = "Dinossauros recriados em um parque temático escapam de seus cercados após sabotagem industrial.",
                    Synopsis = "O milionário John Hammond convida cientistas e seus netos para conhecer um incrível parque temático de dinossauros vivos clonados a partir de DNA fóssil. No entanto, uma sabotagem de segurança desativa os cercados elétricos, deixando os animais livres para caçar.",
                    Director = "Steven Spielberg",
                    Cast = "Sam Neill, Laura Dern, Jeff Goldblum",
                    ReleaseYear = 1993,
                    DurationMinutes = 127,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjM2MDgxMDg0Nl5BMl5BanBnXkFtZTgwNTM0MDgxOTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=QWBK1e4cgh4",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { sciFiGenre!, actionGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Rei da Estrela",
                    Description = "Um jovem viaja a um reino mágico para resgatar uma estrela cadente que se transformou em uma bela mulher.",
                    Synopsis = "Tristan Thorne é um jovem apaixonado que promete buscar uma estrela cadente para sua amada Victoria. Ao cruzar a muralha de sua aldeia rumo ao reino místico de Stormhold, Tristan descobre que a estrela é na verdade Yvaine, uma garota perseguida por bruxas e príncipes gananciosos.",
                    Director = "Matthew Vaughn",
                    Cast = "Charlie Cox, Claire Danes, Michelle Pfeiffer",
                    ReleaseYear = 2007,
                    DurationMinutes = 127,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjkyMTE1OTYwNF5BMl5BanBnXkFtZTcwMDIxODYzMw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=mD6n_p05rL8",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { romanceGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Taxi Driver",
                    Description = "Um veterano de guerra mentalmente instável trabalha como motorista de táxi noturno em Nova York.",
                    Synopsis = "Travis Bickle é um ex-fuzileiro naval solitário no Alabama que sofre de insônia e começa a dirigir um táxi nas noites de Nova York. Consumido pela repulsa contra o crime e a decadência da cidade, Travis inicia uma espiral violenta com o objetivo de salvar uma jovem prostituta chamada Iris.",
                    Director = "Martin Scorsese",
                    Cast = "Robert De Niro, Jodie Foster, Cybill Shepherd",
                    ReleaseYear = 1976,
                    DurationMinutes = 113,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNGQxNDgzZWQtZTNjNi00M2RkLWExZmEtNmE1NjEyZDEwMzA5XkEyXkFqcGdeQXVyMTQxNzMzNDI@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=cM34t151Q8Q",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Onde os Fracos Não Têm Vez",
                    Description = "Um caçador se depara com um negócio de drogas fracassado e decide fugir com mais de dois milhões de dólares.",
                    Synopsis = "Llewelyn Moss encontra uma caminhonete cercada por mortos em pleno deserto texano, além de um carregamento de heroína e dois milhões de dólares. Ao pegar o dinheiro, ele vira alvo de Anton Chigurh, um psicopata implacável contratado para recuperar o montante.",
                    Director = "Joel Coen, Ethan Coen",
                    Cast = "Tommy Lee Jones, Javier Bardem, Josh Brolin",
                    ReleaseYear = 2007,
                    DurationMinutes = 122,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjA5Njk3MjM4OV5BMl5BanBnXkFtZTcwMTc5MTE1MQ@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=38A__WT3-o0",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "A Viagem de Chihiro",
                    Description = "Uma menina de 10 anos entra acidentalmente no mundo dos espíritos governado por deuses e bruxas.",
                    Synopsis = "Chihiro está de mudança com seus pais e acabam errando o caminho, chegando a um misterioso parque temático abandonado. Quando seus pais se transformam em porcos após comer comida de um restaurante local, ela descobre que o local é na verdade um balneário para espíritos administrado pela bruxa Yubaba.",
                    Director = "Hayao Miyazaki",
                    Cast = "Rumi Hiiragi, Miyu Irino, Mari Natsuki",
                    ReleaseYear = 2001,
                    DurationMinutes = 125,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjlmZmI5MDctNDE2YS00YWE0LWE5ZWItZDBhYWQ0NTcxNWRiXkFtZTgwOTkyOTUxMDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=ByXuk9QqQkk",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { sciFiGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Show de Truman",
                    Description = "Um vendedor de seguros descobre que sua vida inteira é na verdade um reality show de TV transmitido mundialmente.",
                    Synopsis = "Truman Burbank é um pacato morador de Seahaven que leva uma vida comum. Porém, ele descobre gradualmente que sua pacífica cidade natal é na verdade um gigantesco estúdio de gravação comandado pelo diretor artístico Christof, e que todos os seus amigos e parentes são atores profissionais contratados.",
                    Director = "Peter Weir",
                    Cast = "Jim Carrey, Laura Linney, Ed Harris",
                    ReleaseYear = 1998,
                    DurationMinutes = 103,
                    Rating = "Livre",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMDIzODcyY2EtMmY2MC00ZWVlLTgwMzAtMjQwOWUyNmJjNTYyXkFtZTgwNjc4NDgwNzE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=dlnmQbPGuls",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre!, comedyGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Fabuloso Destino de Amélie Poulain",
                    Description = "Uma jovem parisiense decide secretamente ajudar as pessoas ao seu redor enquanto tenta encontrar o amor próprio.",
                    Synopsis = "Amélie é uma jovem garçonete que cresceu isolada do mundo devido a um diagnóstico cardíaco incorreto de seu pai. Ela encontra uma pequena caixa de recordações escondida em seu apartamento parisiense e decide devolvê-la ao antigo dono. Ao ver sua alegria, Amélie passa a realizar pequenos gestos benevolentes pelos outros.",
                    Director = "Jean-Pierre Jeunet",
                    Cast = "Audrey Tautou, Mathieu Kassovitz, Rufus",
                    ReleaseYear = 2001,
                    DurationMinutes = 122,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDg4NjM1YjMtYmNhZC00MjM0LWIyODgtNDA2ODM0NDJiNWFjXkFtZTgwNDM4NTU2MDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=6m8gV0X5C0I",
                    StreamingPlatforms = "[{\"name\":\"Globoplay\",\"url\":\"https://globoplay.globo.com/\"},{\"name\":\"Telecine\",\"url\":\"https://www.telecine.com.br/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre!, romanceGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Ilha do Medo",
                    Description = "Um agente federal investiga o desaparecimento misterioso de um paciente de um hospital psiquiátrico prisional na ilha de Shutter.",
                    Synopsis = "Teddy Daniels e seu parceiro Chuck Aule são enviados a Shutter Island, no hospital psiquiátrico de criminosos de Boston, para investigar a fuga inexplicável da infanticida Rachel Solando. Teddy suspeita que experimentos clandestinos ilegais e cruéis estejam sendo praticados na ilha isolada.",
                    Director = "Martin Scorsese",
                    Cast = "Leonardo DiCaprio, Mark Ruffalo, Ben Kingsley",
                    ReleaseYear = 2010,
                    DurationMinutes = 138,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzhiNDkyNzktNTZmYS00ZTBkLTk2MDAtM2U0YjU1MzgxZjgzXkFtZTgwNzM1OTM2MDQ@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=L39a2gK4qLI",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { horrorGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Whiplash: Em Busca da Perfeição",
                    Description = "Um jovem baterista de jazz ingressa em um conservatório musical de elite sob a tutela de um instrutor implacável.",
                    Synopsis = "Andrew Neiman sonha em se tornar um dos grandes bateristas de jazz de sua geração. Ele entra no Conservatório de Shaffer e é selecionado pelo temido regente Terence Fletcher para fazer parte da orquestra principal. Fletcher utiliza métodos abusivos e violentos para forçar o máximo desempenho dos alunos.",
                    Director = "Damien Chazelle",
                    Cast = "Miles Teller, J.K. Simmons, Paul Reiser",
                    ReleaseYear = 2014,
                    DurationMinutes = 106,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTg1MTY2MjM0Nl5BMl5BanBnXkFtZTgwMTc4NjUxMjE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=7d_jQyG8SnU",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "A Vida é Bela",
                    Description = "Um pai judeu usa de humor e imaginação para proteger seu filho pequeno dos horrores de um campo de concentração nazista.",
                    Synopsis = "Guido Orefice é um bem-humorado livreiro judeu que se apaixona e se casa com a bela professora Dora na Itália fascista. Anos depois, ele e seu filho pequeno Giosuè são capturados e enviados para um campo de concentração alemão. Guido inventa um jogo imaginário para disfarçar o terror.",
                    Director = "Roberto Benigni",
                    Cast = "Roberto Benigni, Nicoletta Braschi, Giorgio Cantarini",
                    ReleaseYear = 1997,
                    DurationMinutes = 116,
                    Rating = "12",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYmJhYjQ1MzItOTM0Mi00YThmLWEyYjktOTkwMzA1YjBkN2U5XkFtZTgwMjcxMDA4NDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=16R_s6VCL24",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre!, romanceGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Gênio Indomável",
                    Description = "Um jovem gênio rebelde da classe trabalhadora de Boston é confrontado por um psicólogo para superar seus traumas.",
                    Synopsis = "Will Hunting trabalha como faxineiro na prestigiada universidade do MIT. Ele possui uma genialidade matemática inexplicável, resolvendo problemas matemáticos complexos anonimamente no quadro de giz. Ao se envolver em uma briga de rua, é obrigado a passar por sessões de terapia com Sean Maguire.",
                    Director = "Gus Van Sant",
                    Cast = "Robin Williams, Matt Damon, Ben Affleck",
                    ReleaseYear = 1997,
                    DurationMinutes = 126,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTI0MzcxMTYtZDVkMy00NjY1LTgyMTYtZmUxN2M3NmQ2NWJhXkFtZTgwOTM4MTc0MDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=QSMvyuMecTw",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Os Infiltrados",
                    Description = "Um policial disfarçado e uma toupeira na polícia tentam se identificar mutuamente enquanto estão infiltrados em uma gangue irlandesa de Boston.",
                    Synopsis = "Billy Costigan é um jovem policial designado para se infiltrar na quadrilha do perigoso chefão do crime Frank Costello. Paralelamente, o criminoso Colin Sullivan é colocado por Costello como infiltrado e promovido a cargos de chefia dentro da polícia para espionar as investigações.",
                    Director = "Martin Scorsese",
                    Cast = "Leonardo DiCaprio, Matt Damon, Jack Nicholson",
                    ReleaseYear = 2006,
                    DurationMinutes = 151,
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTI1MTYyNzQ3OV5BMl5BanBnXkFtZTcwOTUxODYzMw@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=iojhqm0JYi4",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Grande Truque",
                    Description = "Dois mágicos rivais de Londres travam uma disputa implacável pela criação do melhor truque de ilusão.",
                    Synopsis = "No final do século XIX, Robert Angier e Alfred Borden trabalham juntos como ajudantes de palco. Quando a esposa de Angier morre afogada durante um truque de fuga, ele culpa Borden pelo acidente. Ambos se tornam ilusionistas solos renomados, travando uma disputa trágica por segredos industriais.",
                    Director = "Christopher Nolan",
                    Cast = "Hugh Jackman, Christian Bale, Scarlett Johansson",
                    ReleaseYear = 2006,
                    DurationMinutes = 130,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjA4NDI0MTIxNF5BMl5BanBnXkFtZTYwNTM3NzY2._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=R2jZpQ3aLdY",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Bastardos Inglórios",
                    Description = "Na França ocupada pelos nazistas, um grupo de soldados judeus-americanos planeja assassinar a liderança do Terceiro Reich.",
                    Synopsis = "O tenente Aldo Raine comanda os \"Bastardos\", um esquadrão especial encarregado de espalhar o medo na Europa ocupada escalpelando e assassinando oficiais nazistas. Eles se aliam à atriz alemã infiltrada Bridget von Hammersmark e planejam um atentado contra Hitler em uma estreia cinematográfica em Paris.",
                    Director = "Quentin Tarantino",
                    Cast = "Brad Pitt, Mélanie Laurent, Christoph Waltz",
                    ReleaseYear = 2009,
                    DurationMinutes = 153,
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTJiNDExOWUtYjBmYy00M2RmLWFlMDEtZGE3YWQ3NGYwZTA1XkFtZTgwNjkyNjgxMTE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=KzJNYYkkhp4",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Cisne Negro",
                    Description = "Uma bailarina dedicada começa a perder o controle sobre a realidade à medida que se aproxima do papel principal de sua vida.",
                    Synopsis = "Nina Sayers é uma bailarina talentosa de Nova York que vive com sua mãe possessiva. Quando o diretor artístico Thomas Leroy decide substituir a principal do espetáculo O Lago dos Cisnes, Nina é selecionada para o papel. No entanto, ela precisa expressar a doçura do Cisne Branco e a sedução sombria do Cisne Negro.",
                    Director = "Darren Aronofsky",
                    Cast = "Natalie Portman, Mila Kunis, Vincent Cassel",
                    ReleaseYear = 2010,
                    DurationMinutes = 108,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzQyMzgzMjkyOF5BMl5BanBnXkFtZTcwMzMzNDAzNA@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=5jaI1XOB-F8",
                    StreamingPlatforms = "[{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"},{\"name\":\"Globoplay\",\"url\":\"https://globoplay.globo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre!, horrorGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Django Livre",
                    Description = "Com a ajuda de um caçador de recompensas alemão, um escravo liberto viaja para resgatar sua esposa das mãos de um fazendeiro implacável.",
                    Synopsis = "Django é um escravo vendido cujo destino muda ao cruzar com o Dr. King Schultz, um ex-dentista alemão transformado em caçador de recompensas. Schultz compra Django e promete alforriá-lo se ele o ajudar a rastrear criminosos procurados, ajudando-o depois a resgatar sua esposa Broomhilda.",
                    Director = "Quentin Tarantino",
                    Cast = "Jamie Foxx, Christoph Waltz, Leonardo DiCaprio",
                    ReleaseYear = 2012,
                    DurationMinutes = 165,
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjIyNTQ5NjUxM15BMl5BanBnXkFtZTcwODg1MDU4OA@@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=s8CRQ3Psk5k",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { actionGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "O Lobo de Wall Street",
                    Description = "A ascensão meteórica e queda trágica de Jordan Belfort, um corretor de ações corrupto de Nova York.",
                    Synopsis = "Jordan Belfort inicia sua carreira como corretor júnior de Wall Street. Após a Black Monday, ele cria a Stratton Oakmont, uma corretora que utiliza táticas agressivas fraudulentas para vender ações baratas de baixo valor e embolsar altas taxas de comissão. Jordan adquire riqueza extrema e descontrolada.",
                    Director = "Martin Scorsese",
                    Cast = "Leonardo DiCaprio, Jonah Hill, Margot Robbie",
                    ReleaseYear = 2013,
                    DurationMinutes = 180,
                    Rating = "18",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjIxMjgxNTk0MF5BMl5BanBnXkFtZTgwNjIyOTg2MDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=iszwuX1AK6A",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { comedyGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Se7en: Os Sete Crimes Capitais",
                    Description = "Dois detetives da polícia rastreiam um serial killer meticuloso cujos assassinatos são inspirados nos sete pecados capitais.",
                    Synopsis = "O veterano detetive William Somerset está a apenas uma semana de sua aposentadoria quando é designado junto com o jovem e impulsivo David Mills para investigar uma série de assassinatos macabros e teatrais. O criminoso John Doe justifica seus crimes como punições para os pecadores.",
                    Director = "David Fincher",
                    Cast = "Morgan Freeman, Brad Pitt, Kevin Spacey",
                    ReleaseYear = 1995,
                    DurationMinutes = 127,
                    Rating = "16",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTc2ZTlmYmItMDBhYS00Yzk2LWIyNzgtY2JiMWQ4YWFmODdlXkFtZTgwMjAzNzAxMDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=znmZoBf9M38",
                    StreamingPlatforms = "[{\"name\":\"Max\",\"url\":\"https://www.max.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { horrorGenre!, dramaGenre! }
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "A Lista de Schindler",
                    Description = "A história verídica do industrial alemão Oskar Schindler, que salvou mais de mil judeus do Holocausto.",
                    Synopsis = "Oskar Schindler é um oportunista e carismático empresário alemão que chega à Polônia ocupada na Segunda Guerra Mundial para lucrar com fábricas de esmalte de baixo custo utilizando trabalho forçado judeu. Ao ver o terror promovido pelo comandante Amon Goeth, Schindler gasta sua fortuna para proteger seus operários.",
                    Director = "Steven Spielberg",
                    Cast = "Liam Neeson, Ben Kingsley, Ralph Fiennes",
                    ReleaseYear = 1993,
                    DurationMinutes = 195,
                    Rating = "14",
                    AverageRating = 0.0,
                    ReviewsCount = 0,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDE4OTMxMTctNmRhYy00Nzk2LTg4OTItY2U3ODIwMjlkMWFhXkFtZTgwODMyMjEyMDE@._V1_SX300.jpg",
                    TrailerUrl = "https://www.youtube.com/watch?v=gG228leZy94",
                    StreamingPlatforms = "[{\"name\":\"Netflix\",\"url\":\"https://www.netflix.com/\"},{\"name\":\"Prime Video\",\"url\":\"https://www.primevideo.com/\"}]",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = new List<Genre> { dramaGenre! }
                }
            };

            // Remove qualquer filme que não esteja na lista padrão de sementes (candidateMovies) para limpar adições manuais/antigas
            var seededTitles = candidateMovies.Select(m => m.Title.Trim().ToLowerInvariant()).ToHashSet();
            var currentMovies = await context.Movies.ToListAsync();
            var moviesToDelete = currentMovies
                .Where(m => !seededTitles.Contains(m.Title.Trim().ToLowerInvariant()))
                .ToList();

            if (moviesToDelete.Count > 0)
            {
                context.Movies.RemoveRange(moviesToDelete);
                await context.SaveChangesAsync();
            }

            if (!await context.Movies.AnyAsync())
            {
                foreach (var movie in candidateMovies)
                {
                    await context.Movies.AddAsync(movie);
                }
                await context.SaveChangesAsync();
            }

            // Inicia a atualização em segundo plano das capas e dados dos filmes usando o TMDb
            _ = Task.Run(async () =>
            {
                using var bgScope = serviceProvider.CreateScope();
                var dbContext = bgScope.ServiceProvider.GetRequiredService<CineCatalogDbContext>();
                var tmdbClient = bgScope.ServiceProvider.GetRequiredService<ITmdbClient>();
                
                try
                {
                    var moviesToUpdate = await dbContext.Movies
                        .Where(m => !m.TmdbId.HasValue || m.ImageUrl.Contains("amazon") || string.IsNullOrEmpty(m.ImageUrl))
                        .ToListAsync();

                    foreach (var movie in moviesToUpdate)
                    {
                        try
                        {
                            var tmdbResults = await tmdbClient.SearchMoviesAsync(movie.Title);
                            var bestMatch = tmdbResults.FirstOrDefault(r => 
                                r.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase));
                            
                            if (bestMatch == null)
                            {
                                bestMatch = tmdbResults.FirstOrDefault();
                            }

                            if (bestMatch != null)
                            {
                                var alreadyExists = await dbContext.Movies.AnyAsync(m => m.TmdbId == bestMatch.Id && m.Id != movie.Id);
                                if (!alreadyExists)
                                {
                                    movie.TmdbId = bestMatch.Id;
                                
                                    var details = await tmdbClient.GetMovieDetailsAsync(bestMatch.Id);
                                    var rating = await tmdbClient.GetReleaseDateCertificationAsync(bestMatch.Id);
                                    
                                    if (!string.IsNullOrEmpty(rating))
                                    {
                                        movie.Rating = rating;
                                    }
                                    
                                    if (!string.IsNullOrEmpty(details.PosterPath))
                                    {
                                        movie.ImageUrl = $"https://image.tmdb.org/t/p/w500/{details.PosterPath.TrimStart('/')}";
                                    }

                                    if (!string.IsNullOrEmpty(details.BackdropPath))
                                    {
                                        movie.BackdropUrl = $"https://image.tmdb.org/t/p/w1280/{details.BackdropPath.TrimStart('/')}";
                                    }

                                    if (!string.IsNullOrEmpty(details.Overview))
                                    {
                                        movie.Synopsis = details.Overview;
                                        movie.Description = details.Overview.Length > 200 
                                            ? details.Overview.Substring(0, 197) + "..." 
                                            : details.Overview;
                                    }

                                    dbContext.Movies.Update(movie);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                            
                            await Task.Delay(500); // Evita sobrecarga na API do TMDb
                        }
                        catch (Exception)
                        {
                            // Ignora erros individuais de filmes
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignora erro geral
                }
            });
        }
    }
}
