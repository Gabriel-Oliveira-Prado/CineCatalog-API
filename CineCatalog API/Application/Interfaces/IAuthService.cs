using System;
using System.Threading.Tasks;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(UserRegisterRequest request);
        Task<TokenResponse> LoginAsync(UserLoginRequest request);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<UserResponse> GetProfileAsync(Guid userId);
        Task<UserResponse> UpdateProfileAsync(Guid userId, UserUpdateProfileRequest request);
    }
}
