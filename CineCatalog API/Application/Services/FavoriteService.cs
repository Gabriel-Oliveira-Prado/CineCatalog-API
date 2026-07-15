using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Interfaces;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;

namespace CineCatalog_API.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IMovieRepository movieRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(Guid userId, Guid movieId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                throw new NotFoundException("Filme não encontrado.");
            }

            var existingFavorite = await _favoriteRepository.GetAsync(userId, movieId);
            if (existingFavorite != null)
            {
                throw new ConflictException("Este filme já está na sua lista de favoritos.");
            }

            var favorite = new Favorite
            {
                UserId = userId,
                MovieId = movieId
            };

            await _favoriteRepository.AddAsync(favorite);
        }

        public async Task RemoveAsync(Guid userId, Guid movieId)
        {
            var existingFavorite = await _favoriteRepository.GetAsync(userId, movieId);
            if (existingFavorite == null)
            {
                throw new NotFoundException("Este filme não está na sua lista de favoritos.");
            }

            await _favoriteRepository.DeleteAsync(existingFavorite);
        }

        public async Task<IEnumerable<MovieResponse>> GetUserFavoritesAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var movies = await _favoriteRepository.GetUserFavoritesAsync(userId);
            return _mapper.Map<IEnumerable<MovieResponse>>(movies);
        }
    }
}
