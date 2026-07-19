using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Interfaces;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;
using CineCatalog_API.Infrastructure.ExternalServices.Tmdb;

namespace CineCatalog_API.Application.Services
{
    public class MovieCatalogSyncService : IMovieCatalogSyncService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ITmdbClient _tmdbClient;
        private readonly IMapper _mapper;
        private readonly TmdbSettings _tmdbSettings;

        public MovieCatalogSyncService(
            IMovieRepository movieRepository,
            IGenreRepository genreRepository,
            ITmdbClient tmdbClient,
            IMapper mapper,
            IOptions<TmdbSettings> tmdbSettings)
        {
            _movieRepository = movieRepository;
            _genreRepository = genreRepository;
            _tmdbClient = tmdbClient;
            _mapper = mapper;
            _tmdbSettings = tmdbSettings.Value;
        }

        public async Task<List<MovieResponse>> SearchWithFallbackAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<MovieResponse>();

            // 1. Busca local no banco de dados
            var (localMovies, totalCount) = await _movieRepository.GetFilteredAndPaginatedAsync(
                searchTerm, null, null, null, null, null, 1, 50, "Title", false);

            // Se encontrou pelo menos 5 resultados localmente, retorna direto (cache-aside)
            if (totalCount >= 5)
            {
                return _mapper.Map<List<MovieResponse>>(localMovies.ToList());
            }

            // 2. Busca no TMDb caso tenha poucos resultados localmente (< 5) para enriquecer o catálogo
            var tmdbResults = await _tmdbClient.SearchMoviesAsync(searchTerm);
            var resultsToImport = tmdbResults.Take(10).ToList(); // Limita a 10 resultados para evitar sobrecarga

            var importedMovies = new List<Movie>();

            foreach (var result in resultsToImport)
            {
                // Checa se o filme já existe localmente por TmdbId ou por Título para não duplicar
                var existingMovie = await _movieRepository.GetByTmdbIdAsync(result.Id);
                if (existingMovie == null)
                {
                    existingMovie = await _movieRepository.GetByTitleAsync(result.Title);
                    if (existingMovie != null)
                    {
                        // Se encontramos por título mas ele não tinha TmdbId associado, associamos agora
                        if (!existingMovie.TmdbId.HasValue)
                        {
                            existingMovie.TmdbId = result.Id;
                            // Se o filme existente não tiver imagem de capa ou tiver uma capa antiga/Amazon, atualiza também
                            if ((string.IsNullOrEmpty(existingMovie.ImageUrl) || existingMovie.ImageUrl.Contains("amazon")) && !string.IsNullOrEmpty(result.PosterPath))
                            {
                                existingMovie.ImageUrl = $"{_tmdbSettings.ImageBaseUrl.TrimEnd('/')}/{result.PosterPath.TrimStart('/')}";
                            }
                            await _movieRepository.UpdateAsync(existingMovie);
                        }
                    }
                }

                if (existingMovie != null)
                {
                    importedMovies.Add(existingMovie);
                    continue;
                }

                try
                {
                    // Busca detalhes, créditos, trailer e classificação indicativa em paralelo para otimizar desempenho
                    var detailsTask = _tmdbClient.GetMovieDetailsAsync(result.Id);
                    var creditsTask = _tmdbClient.GetCreditsAsync(result.Id);
                    var trailerTask = _tmdbClient.GetTrailerUrlAsync(result.Id);
                    var ratingTask = _tmdbClient.GetReleaseDateCertificationAsync(result.Id);

                    await Task.WhenAll(detailsTask, creditsTask, trailerTask, ratingTask);

                    var details = await detailsTask;
                    var credits = await creditsTask;
                    var trailerUrl = await trailerTask;
                    var rating = await ratingTask;

                    // Mapeamento dos campos objetivos
                    var director = credits.Crew?.FirstOrDefault(c => c.Job.Equals("Director", StringComparison.OrdinalIgnoreCase))?.Name ?? "Desconhecido";
                    var cast = string.Join(", ", credits.Cast?.Take(5).Select(c => c.Name) ?? Array.Empty<string>());
                    
                    int releaseYear = 0;
                    if (DateTime.TryParse(details.ReleaseDate, out var date))
                    {
                        releaseYear = date.Year;
                    }
                    else if (!string.IsNullOrEmpty(details.ReleaseDate) && details.ReleaseDate.Length >= 4)
                    {
                        int.TryParse(details.ReleaseDate.Substring(0, 4), out releaseYear);
                    }

                    var duration = details.Runtime ?? 0;

                    // Gêneros
                    var genreNames = details.Genres?.Select(g => g.Name).ToList() ?? new List<string>();
                    var genresStr = genreNames.Count > 0 ? string.Join(", ", genreNames) : "Gênero não informado";

                    // Geração de descrição e sinopse: usa a original do TMDb se disponível, senão gera baseada em fatos
                    var synopsis = !string.IsNullOrWhiteSpace(details.Overview) 
                        ? details.Overview 
                        : $"Acompanhe a história desta produção de {genresStr.ToLower()} que conta com direção de {director} e grande elenco incluindo {cast}. Lançado originalmente no ano de {releaseYear}.";
                    
                    var description = synopsis.Length > 200 
                        ? synopsis.Substring(0, 197) + "..." 
                        : synopsis;

                    var movie = new Movie
                    {
                        Id = Guid.NewGuid(),
                        Title = details.Title,
                        TmdbId = details.Id,
                        ReleaseYear = releaseYear,
                        DurationMinutes = duration,
                        ImageUrl = string.IsNullOrEmpty(details.PosterPath) ? string.Empty : $"{_tmdbSettings.ImageBaseUrl.TrimEnd('/')}/{details.PosterPath.TrimStart('/')}",
                        BackdropUrl = string.IsNullOrEmpty(details.BackdropPath) ? string.Empty : $"https://image.tmdb.org/t/p/w1280/{details.BackdropPath.TrimStart('/')}",
                        TrailerUrl = trailerUrl ?? string.Empty,
                        Director = director,
                        Cast = cast,
                        Description = description.Length > 1000 ? description.Substring(0, 997) + "..." : description,
                        Synopsis = synopsis.Length > 4000 ? synopsis.Substring(0, 3997) + "..." : synopsis,
                        Rating = rating ?? "Livre",
                        AverageRating = 0.0,
                        ReviewsCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Genres = new List<Genre>()
                    };

                    // Mapeia os gêneros para as entidades locais existentes ou cria novos se necessário
                    foreach (var tg in details.Genres ?? new List<TmdbGenre>())
                    {
                        var localGenre = await _genreRepository.GetByNameAsync(tg.Name);
                        if (localGenre == null)
                        {
                            localGenre = new Genre
                            {
                                Id = Guid.NewGuid(),
                                Name = tg.Name
                            };
                            await _genreRepository.AddAsync(localGenre);
                        }
                        movie.Genres.Add(localGenre);
                    }

                    // Salva o filme importado no banco local
                    await _movieRepository.AddAsync(movie);
                    importedMovies.Add(movie);
                }
                catch (Exception ex)
                {
                    // Loga o erro de importação do filme específico e continua com os outros
                    // Desta forma, uma falha em um filme não quebra a busca inteira
                    System.Diagnostics.Debug.WriteLine($"Erro ao importar filme TMDb ID {result.Id}: {ex.Message}");
                }
            }

            // Mescla os resultados locais iniciais com os importados do TMDb, removendo duplicados por Id
            var mergedMovies = localMovies.ToList();
            foreach (var movie in importedMovies)
            {
                if (!mergedMovies.Any(m => m.Id == movie.Id))
                {
                    mergedMovies.Add(movie);
                }
            }

            return _mapper.Map<List<MovieResponse>>(mergedMovies);
        }

        public async Task<List<MovieResponse>> GetTrendingMoviesAsync()
        {
            var tmdbResults = await _tmdbClient.GetTrendingMoviesAsync();
            var resultsToImport = tmdbResults.Take(12).ToList(); // Top 12 populares em alta

            var importedMovies = new List<Movie>();

            foreach (var result in resultsToImport)
            {
                var existingMovie = await _movieRepository.GetByTmdbIdAsync(result.Id);
                if (existingMovie == null)
                {
                    existingMovie = await _movieRepository.GetByTitleAsync(result.Title);
                    if (existingMovie != null)
                    {
                        if (!existingMovie.TmdbId.HasValue)
                        {
                            existingMovie.TmdbId = result.Id;
                            if ((string.IsNullOrEmpty(existingMovie.ImageUrl) || existingMovie.ImageUrl.Contains("amazon")) && !string.IsNullOrEmpty(result.PosterPath))
                            {
                                existingMovie.ImageUrl = $"{_tmdbSettings.ImageBaseUrl.TrimEnd('/')}/{result.PosterPath.TrimStart('/')}";
                            }
                            await _movieRepository.UpdateAsync(existingMovie);
                        }
                    }
                }

                if (existingMovie != null)
                {
                    importedMovies.Add(existingMovie);
                    continue;
                }

                try
                {
                    var detailsTask = _tmdbClient.GetMovieDetailsAsync(result.Id);
                    var creditsTask = _tmdbClient.GetCreditsAsync(result.Id);
                    var trailerTask = _tmdbClient.GetTrailerUrlAsync(result.Id);
                    var ratingTask = _tmdbClient.GetReleaseDateCertificationAsync(result.Id);

                    await Task.WhenAll(detailsTask, creditsTask, trailerTask, ratingTask);

                    var details = await detailsTask;
                    var credits = await creditsTask;
                    var trailerUrl = await trailerTask;
                    var rating = await ratingTask;

                    var director = credits.Crew?.FirstOrDefault(c => c.Job.Equals("Director", StringComparison.OrdinalIgnoreCase))?.Name ?? "Desconhecido";
                    var cast = string.Join(", ", credits.Cast?.Take(5).Select(c => c.Name) ?? Array.Empty<string>());
                    
                    int releaseYear = 0;
                    if (DateTime.TryParse(details.ReleaseDate, out var date))
                    {
                        releaseYear = date.Year;
                    }
                    else if (!string.IsNullOrEmpty(details.ReleaseDate) && details.ReleaseDate.Length >= 4)
                    {
                        int.TryParse(details.ReleaseDate.Substring(0, 4), out releaseYear);
                    }

                    var duration = details.Runtime ?? 0;
                    var genreNames = details.Genres?.Select(g => g.Name).ToList() ?? new List<string>();
                    var genresStr = genreNames.Count > 0 ? string.Join(", ", genreNames) : "Gênero não informado";

                    var synopsis = !string.IsNullOrWhiteSpace(details.Overview) 
                        ? details.Overview 
                        : $"Acompanhe a história desta produção de {genresStr.ToLower()} que conta com direção de {director} e grande elenco incluindo {cast}. Lançado originalmente no ano de {releaseYear}.";
                    
                    var description = synopsis.Length > 200 
                        ? synopsis.Substring(0, 197) + "..." 
                        : synopsis;

                    var movie = new Movie
                    {
                        Id = Guid.NewGuid(),
                        Title = details.Title,
                        TmdbId = details.Id,
                        ReleaseYear = releaseYear,
                        DurationMinutes = duration,
                        ImageUrl = string.IsNullOrEmpty(details.PosterPath) ? string.Empty : $"{_tmdbSettings.ImageBaseUrl.TrimEnd('/')}/{details.PosterPath.TrimStart('/')}",
                        BackdropUrl = string.IsNullOrEmpty(details.BackdropPath) ? string.Empty : $"https://image.tmdb.org/t/p/w1280/{details.BackdropPath.TrimStart('/')}",
                        TrailerUrl = trailerUrl ?? string.Empty,
                        Director = director,
                        Cast = cast,
                        Description = description.Length > 1000 ? description.Substring(0, 997) + "..." : description,
                        Synopsis = synopsis.Length > 4000 ? synopsis.Substring(0, 3997) + "..." : synopsis,
                        Rating = rating ?? "Livre",
                        AverageRating = 0.0,
                        ReviewsCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Genres = new List<Genre>()
                    };

                    foreach (var tg in details.Genres ?? new List<TmdbGenre>())
                    {
                        var localGenre = await _genreRepository.GetByNameAsync(tg.Name);
                        if (localGenre == null)
                        {
                            localGenre = new Genre
                            {
                                Id = Guid.NewGuid(),
                                Name = tg.Name
                            };
                            await _genreRepository.AddAsync(localGenre);
                        }
                        movie.Genres.Add(localGenre);
                    }

                    await _movieRepository.AddAsync(movie);
                    importedMovies.Add(movie);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao importar filme trending TMDb ID {result.Id}: {ex.Message}");
                }
            }

            return _mapper.Map<List<MovieResponse>>(importedMovies);
        }

        public async Task<StreamingAvailabilityResponse> GetStreamingAvailabilityAsync(Guid movieId)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            var region = string.IsNullOrWhiteSpace(_tmdbSettings.WatchRegion)
                ? "BR"
                : _tmdbSettings.WatchRegion.ToUpperInvariant();
            var storedPlatforms = StreamingPlatformsJson.Deserialize(movie.StreamingPlatforms);

            // Filmes cadastrados manualmente continuam exibindo o que foi salvo localmente.
            if (!movie.TmdbId.HasValue)
            {
                return CreateAvailabilityResponse(region, storedPlatforms, movie.Title);
            }

            var providersResponse = await _tmdbClient.GetWatchProvidersAsync(movie.TmdbId.Value);
            if (providersResponse == null)
            {
                // Não apagamos uma resposta válida se o provedor externo estiver temporariamente indisponível.
                return CreateAvailabilityResponse(region, storedPlatforms, movie.Title);
            }

            providersResponse.Results.TryGetValue(region, out var providersForRegion);
            var platforms = MapProviders(providersForRegion, movie.Title);
            var link = providersForRegion?.Link;

            if (platforms.Count == 0 && storedPlatforms.Count > 0)
            {
                // Mantém as plataformas já salvas no banco (p. ex., dados semeados)
                platforms = storedPlatforms;
                link = storedPlatforms.Select(p => p.Link).FirstOrDefault(l => !string.IsNullOrEmpty(l));
            }
            else
            {
                movie.StreamingPlatforms = StreamingPlatformsJson.Serialize(platforms);
                movie.UpdatedAt = DateTime.UtcNow;
                await _movieRepository.UpdateAsync(movie);
            }

            if (string.IsNullOrEmpty(link) || link.Contains("justwatch.com"))
            {
                link = $"https://www.google.com/search?q=onde+assistir+{Uri.EscapeDataString(movie.Title)}";
            }

            return new StreamingAvailabilityResponse
            {
                Region = region,
                Link = link,
                Platforms = platforms
            };
        }

        private static StreamingAvailabilityResponse CreateAvailabilityResponse(string region, List<StreamingPlatformResponse> platforms, string movieTitle)
        {
            var link = platforms.Select(platform => platform.Link).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
            if (string.IsNullOrEmpty(link) || link.Contains("justwatch.com"))
            {
                link = $"https://www.google.com/search?q=onde+assistir+{Uri.EscapeDataString(movieTitle)}";
            }
            return new StreamingAvailabilityResponse
            {
                Region = region,
                Link = link,
                Platforms = platforms
            };
        }

        private static List<StreamingPlatformResponse> MapProviders(TmdbWatchProviderRegion? providersForRegion, string movieTitle)
        {
            if (providersForRegion == null)
            {
                return new List<StreamingPlatformResponse>();
            }

            var providers = new List<StreamingPlatformResponse>();
            AddProviders(providers, providersForRegion.Flatrate, "Assinatura", movieTitle);
            AddProviders(providers, providersForRegion.Free, "Grátis", movieTitle);
            AddProviders(providers, providersForRegion.Ads, "Com anúncios", movieTitle);
            AddProviders(providers, providersForRegion.Rent, "Aluguel", movieTitle);
            AddProviders(providers, providersForRegion.Buy, "Compra", movieTitle);

            return StreamingPlatformsJson.Deserialize(StreamingPlatformsJson.Serialize(providers));
        }

        private static void AddProviders(
            ICollection<StreamingPlatformResponse> destination,
            IEnumerable<TmdbWatchProvider> providers,
            string availability,
            string movieTitle)
        {
            foreach (var provider in providers)
            {
                destination.Add(new StreamingPlatformResponse
                {
                    ProviderId = provider.ProviderId,
                    Name = provider.ProviderName,
                    Availability = availability,
                    Link = GetDirectPlatformLink(provider.ProviderName, movieTitle),
                    LogoPath = provider.LogoPath
                });
            }
        }

        private static string GetDirectPlatformLink(string providerName, string movieTitle)
        {
            var query = Uri.EscapeDataString(movieTitle);
            var name = providerName.ToLowerInvariant();

            if (name.Contains("netflix"))
                return $"https://www.netflix.com/search?q={query}";
            if (name.Contains("disney"))
                return "https://www.disneyplus.com/";
            if (name.Contains("prime video") || name.Contains("amazon"))
                return $"https://www.primevideo.com/search/ref=atv_sr_filter?phrase={query}";
            if (name.Contains("hbo") || name.Contains("max"))
                return "https://www.max.com/";
            if (name.Contains("globoplay"))
                return $"https://globoplay.globo.com/busca/?q={query}";
            if (name.Contains("apple") || name.Contains("itunes"))
                return "https://tv.apple.com/";
            if (name.Contains("google play") || name.Contains("googleplay"))
                return $"https://play.google.com/store/search?q={query}&c=movies";
            if (name.Contains("paramount"))
                return "https://www.paramountplus.com/";
            if (name.Contains("telecine"))
                return $"https://globoplay.globo.com/busca/?q={query}";
            if (name.Contains("claro"))
                return "https://www.clarotvmais.com.br/";
            if (name.Contains("youtube"))
                return $"https://www.youtube.com/results?search_query={query}";
            if (name.Contains("mercado"))
                return "https://play.mercadolivre.com.br/";

            var platformQuery = Uri.EscapeDataString($"{movieTitle} {providerName}");
            return $"https://www.google.com/search?q=assistir+{platformQuery}";
        }
    }
}
