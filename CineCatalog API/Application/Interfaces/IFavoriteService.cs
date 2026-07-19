using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task AddAsync(Guid userId, Guid movieId);
        Task RemoveAsync(Guid userId, Guid movieId);
        Task<IEnumerable<MovieResponse>> GetUserFavoritesAsync(Guid userId);
    }
}