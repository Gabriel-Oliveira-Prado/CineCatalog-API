using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(Guid id);
        Task<Review?> GetByUserAndMovieAsync(Guid userId, Guid movieId);
        Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Review>> GetByMovieIdAsync(Guid movieId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
    }
}