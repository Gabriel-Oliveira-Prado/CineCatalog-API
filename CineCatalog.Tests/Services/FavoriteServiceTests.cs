using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Mappings;
using CineCatalog_API.Application.Services;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;

namespace CineCatalog.Tests.Services
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> _favoriteRepositoryMock;
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _mapper;
        private readonly FavoriteService _favoriteService;

        public FavoriteServiceTests()
        {
            _favoriteRepositoryMock = new Mock<IFavoriteRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _favoriteService = new FavoriteService(
                _favoriteRepositoryMock.Object,
                _movieRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task AddAsync_ShouldAddFavorite_WhenValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var movieId = Guid.NewGuid();
            var user = new User { Id = userId };
            var movie = new Movie { Id = movieId };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _movieRepositoryMock.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync(movie);
            _favoriteRepositoryMock.Setup(r => r.GetAsync(userId, movieId)).ReturnsAsync((Favorite?)null);

            // Act
            await _favoriteService.AddAsync(userId, movieId);

            // Assert
            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.Is<Favorite>(f => 
                f.UserId == userId && f.MovieId == movieId)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowConflictException_WhenAlreadyFavorited()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var movieId = Guid.NewGuid();
            var user = new User { Id = userId };
            var movie = new Movie { Id = movieId };
            var existingFavorite = new Favorite { UserId = userId, MovieId = movieId };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _movieRepositoryMock.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync(movie);
            _favoriteRepositoryMock.Setup(r => r.GetAsync(userId, movieId)).ReturnsAsync(existingFavorite);

            // Act
            Func<Task> act = async () => await _favoriteService.AddAsync(userId, movieId);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Este filme já está na sua lista de favoritos.");

            _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowNotFoundException_WhenMovieDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var movieId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _movieRepositoryMock.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync((Movie?)null);

            // Act
            Func<Task> act = async () => await _favoriteService.AddAsync(userId, movieId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Filme não encontrado.");
        }
    }
}
