using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Interfaces
{
    public interface IMovieCatalogSyncService
    {
        Task<List<MovieResponse>> SearchWithFallbackAsync(string searchTerm);
        Task<StreamingAvailabilityResponse> GetStreamingAvailabilityAsync(Guid movieId);
        Task<List<MovieResponse>> GetTrendingMoviesAsync();
    }
}
