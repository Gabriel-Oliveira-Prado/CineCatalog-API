using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Domain.Interfaces
{
    public interface IGenreRepository
    {
        Task<Genre?> GetByIdAsync(Guid id);
        Task<Genre?> GetByNameAsync(string name);
        Task<IEnumerable<Genre>> GetAllAsync();
        Task AddAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(Genre genre);
    }
}