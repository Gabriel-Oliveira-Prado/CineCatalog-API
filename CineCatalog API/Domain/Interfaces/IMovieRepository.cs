using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Domain.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie?> GetByIdAsync(Guid id);
        Task<Movie?> GetByIdWithDetailsAsync(Guid id);
        Task<(IEnumerable<Movie> Movies, int TotalCount)> GetFilteredAndPaginatedAsync(
            string? search,
            string? genre,
            string? director,
            int? releaseYear,
            double? minRating,
            int? minDuration,
            int pageNumber,
            int pageSize,
            string? sortBy,
            bool isDescending);
        Task<Movie?> GetByTmdbIdAsync(int tmdbId);
        Task<Movie?> GetByTitleAsync(string title);
        Task AddAsync(Movie movie);
        Task UpdateAsync(Movie movie);
        Task DeleteAsync(Movie movie);
    }
}
