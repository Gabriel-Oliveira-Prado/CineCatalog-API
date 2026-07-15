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
    public class ReviewRepository : IReviewRepository
    {
        private readonly CineCatalogDbContext _context;

        public ReviewRepository(CineCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Review?> GetByUserAndMovieAsync(Guid userId, Guid movieId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        public async Task<IEnumerable<Review>> GetByMovieIdAsync(Guid movieId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
