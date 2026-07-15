using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;
using CineCatalog_API.Infrastructure.Data;

namespace CineCatalog_API.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly CineCatalogDbContext _context;

        public MovieRepository(CineCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<Movie?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<(IEnumerable<Movie> Movies, int TotalCount)> GetFilteredAndPaginatedAsync(
            string? search,
            string? genre,
            string? director,
            int? releaseYear,
            double? minRating,
            int? minDuration,
            int pageNumber,
            int pageSize,
            string? sortBy,
            bool isDescending)
        {
            var query = _context.Movies
                .Include(m => m.Genres)
                .AsQueryable();

            // 1. Filtering
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(m => m.Title.ToLower().Contains(lowerSearch) || 
                                         m.Description.ToLower().Contains(lowerSearch) ||
                                         m.Synopsis.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                var lowerGenre = genre.ToLower();
                query = query.Where(m => m.Genres.Any(g => g.Name.ToLower() == lowerGenre));
            }

            if (!string.IsNullOrWhiteSpace(director))
            {
                var lowerDirector = director.ToLower();
                query = query.Where(m => m.Director.ToLower().Contains(lowerDirector));
            }

            if (releaseYear.HasValue)
            {
                query = query.Where(m => m.ReleaseYear == releaseYear.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(m => m.AverageRating >= minRating.Value);
            }

            if (minDuration.HasValue)
            {
                query = query.Where(m => m.DurationMinutes >= minDuration.Value);
            }

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            // 2. Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "title" => isDescending ? query.OrderByDescending(m => m.Title) : query.OrderBy(m => m.Title),
                    "releaseyear" => isDescending ? query.OrderByDescending(m => m.ReleaseYear) : query.OrderBy(m => m.ReleaseYear),
                    "durationminutes" => isDescending ? query.OrderByDescending(m => m.DurationMinutes) : query.OrderBy(m => m.DurationMinutes),
                    "averagerating" => isDescending ? query.OrderByDescending(m => m.AverageRating) : query.OrderBy(m => m.AverageRating),
                    "createdat" => isDescending ? query.OrderByDescending(m => m.CreatedAt) : query.OrderBy(m => m.CreatedAt),
                    _ => query.OrderBy(m => m.Title) // Default sort
                };
            }
            else
            {
                query = query.OrderBy(m => m.Title);
            }

            // 3. Pagination
            var movies = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (movies, totalCount);
        }

        public async Task AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }
}
