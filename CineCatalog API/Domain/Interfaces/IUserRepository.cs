using System;
using System.Threading.Tasks;
using CineCatalog_API.Domain.Entities;

namespace CineCatalog_API.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
