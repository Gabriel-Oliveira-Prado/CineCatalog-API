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
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly CineCatalogDbContext _context;

        public FavoriteRepository(CineCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Favorite?> GetAsync(Guid userId, Guid movieId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);
        }

        public async Task<IEnumerable<Movie>> GetUserFavoritesAsync(Guid userId)
        {
            return await _context.Favorites
                .Include(f => f.Movie)
                    .ThenInclude(m => m.Genres)
                .Where(f => f.UserId == userId)
                .Select(f => f.Movie)
                .ToListAsync();
        }

        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }
}
