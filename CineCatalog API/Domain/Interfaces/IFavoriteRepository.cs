using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Domain.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetAsync(Guid userId, Guid movieId);
        Task<IEnumerable<Movie>> GetUserFavoritesAsync(Guid userId);
        Task AddAsync(Favorite favorite);
        Task DeleteAsync(Favorite favorite);
    }
}
