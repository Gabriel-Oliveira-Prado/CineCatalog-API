using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.DTOs.Common;

namespace CineCatalog_API.Application.Interfaces
{
    public interface IMovieService
    {
        Task<MovieDetailResponse> CreateAsync(MovieCreateRequest request);
        Task<PagedResult<MovieResponse>> GetFilteredAndPaginatedAsync(MovieQueryParameters queryParams, Guid? userId = null);
        Task<MovieDetailResponse> GetByIdAsync(Guid id);
        Task<MovieDetailResponse> UpdateAsync(Guid id, MovieUpdateRequest request);
        Task DeleteAsync(Guid id);
        
        // Review Operations under the Movie domain scope
        Task<ReviewResponse> AddReviewAsync(Guid movieId, Guid userId, ReviewCreateRequest request);
        Task<ReviewResponse> UpdateReviewAsync(Guid movieId, Guid reviewId, Guid userId, ReviewUpdateRequest request);
        Task DeleteReviewAsync(Guid movieId, Guid reviewId, Guid userId);
        Task<IEnumerable<ReviewResponse>> GetReviewsAsync(Guid movieId);
    }
}
