using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Interfaces
{
    public interface ITmdbClient
    {
        Task<List<TmdbSearchResult>> SearchMoviesAsync(string query);
        Task<TmdbMovieDetails> GetMovieDetailsAsync(int tmdbId);
        Task<TmdbCredits> GetCreditsAsync(int tmdbId);
        Task<string?> GetTrailerUrlAsync(int tmdbId);
        Task<TmdbWatchProvidersResponse?> GetWatchProvidersAsync(int tmdbId);
        Task<List<TmdbSearchResult>> GetTrendingMoviesAsync();
        Task<string> GetReleaseDateCertificationAsync(int tmdbId);
    }
}