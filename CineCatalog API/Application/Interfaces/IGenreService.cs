using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Interfaces
{
    public interface IGenreService
    {
        Task<GenreResponse> CreateAsync(GenreCreateRequest request);
        Task<IEnumerable<GenreResponse>> GetAllAsync();
        Task<GenreResponse> GetByIdAsync(Guid id);
        Task<GenreResponse> UpdateAsync(Guid id, GenreUpdateRequest request);
        Task DeleteAsync(Guid id);
    }
}
