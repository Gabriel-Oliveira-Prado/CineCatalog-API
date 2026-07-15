using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;
using CineCatalog_API.Infrastructure.Data;

namespace CineCatalog_API.Infrastructure.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly CineCatalogDbContext _context;

        public GenreRepository(CineCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Genre?> GetByIdAsync(Guid id)
        {
            return await _context.Genres
                .Include(g => g.Movies)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _context.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .ToListAsync();
        }

        public async Task AddAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
    }
}
