using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Mappings;
using CineCatalog_API.Application.Services;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Domain.Interfaces;

namespace CineCatalog.Tests.Services
{
    public class MovieServiceTests
    {
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _mapper;
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _movieService = new MovieService(
                _movieRepositoryMock.Object,
                _genreRepositoryMock.Object,
                _reviewRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task AddReviewAsync_ShouldAddReviewAndUpdateMovieRating_WhenValid()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Reviewer" };
            var movie = new Movie { Id = movieId, Title = "Movie Title", AverageRating = 0.0, ReviewsCount = 0 };

            var request = new ReviewCreateRequest
            {
                Rating = 5,
                Comment = "Incrível!"
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _movieRepositoryMock
                .Setup(r => r.GetByIdWithDetailsAsync(movieId))
                .ReturnsAsync(movie);

            _reviewRepositoryMock
                .Setup(r => r.GetByUserAndMovieAsync(userId, movieId))
                .ReturnsAsync((Review?)null);

            // Mock database reviews list returning our new review + a pre-existing 4-star review
            var seededReviews = new List<Review>
            {
                new Review { Rating = 5, MovieId = movieId, UserId = userId },
                new Review { Rating = 4, MovieId = movieId, UserId = Guid.NewGuid() }
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByMovieIdAsync(movieId))
                .ReturnsAsync(seededReviews);

            // Act
            var result = await _movieService.AddReviewAsync(movieId, userId, request);

            // Assert
            result.Should().NotBeNull();
            result.Rating.Should().Be(request.Rating);
            result.Comment.Should().Be(request.Comment);
            result.UserName.Should().Be(user.Name);

            // Recalculated: (5 + 4) / 2 = 4.5
            movie.AverageRating.Should().Be(4.5);
            movie.ReviewsCount.Should().Be(2);

            _reviewRepositoryMock.Verify(r => r.AddAsync(It.Is<Review>(rev => 
                rev.MovieId == movieId && 
                rev.UserId == userId && 
                rev.Rating == request.Rating)), Times.Once);

            _movieRepositoryMock.Verify(r => r.UpdateAsync(movie), Times.Once);
        }

        [Fact]
        public async Task AddReviewAsync_ShouldThrowConflictException_WhenUserHasAlreadyReviewedMovie()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var movie = new Movie { Id = movieId };
            var existingReview = new Review { MovieId = movieId, UserId = userId };

            var request = new ReviewCreateRequest { Rating = 3 };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _movieRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(movieId)).ReturnsAsync(movie);
            
            _reviewRepositoryMock
                .Setup(r => r.GetByUserAndMovieAsync(userId, movieId))
                .ReturnsAsync(existingReview);

            // Act
            Func<Task> act = async () => await _movieService.AddReviewAsync(movieId, userId, request);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Você já avaliou este filme anteriormente.");

            _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        }

        [Fact]
        public async Task AddReviewAsync_ShouldThrowNotFoundException_WhenMovieDoesNotExist()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            var request = new ReviewCreateRequest { Rating = 4 };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _movieRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(movieId)).ReturnsAsync((Movie?)null);

            // Act
            Func<Task> act = async () => await _movieService.AddReviewAsync(movieId, userId, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Filme não encontrado.");
        }
    }
}
