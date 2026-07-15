using System;
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
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenServiceMock = new Mock<ITokenService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenEmailIsUnique()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            _passwordHasherMock
                .Setup(h => h.HashPassword(request.Password))
                .Returns("hashed_password");

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            result.Email.Should().Be(request.Email);
            result.IsActive.Should().BeTrue();

            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => 
                u.Email == request.Email && 
                u.Name == request.Name && 
                u.PasswordHash == "hashed_password")), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "Password123!"
            };

            var existingUser = new User { Email = request.Email };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act
            Func<Task> act = async () => await _authService.RegisterAsync(request);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("E-mail já cadastrado no sistema.");

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new UserLoginRequest
            {
                Email = "john.doe@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = "hashed_password",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(true);

            _tokenServiceMock
                .Setup(t => t.GenerateAccessToken(user))
                .Returns("access_token");

            _tokenServiceMock
                .Setup(t => t.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("access_token");
            result.RefreshToken.Should().Be("refresh_token");

            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => 
                u.RefreshToken == "refresh_token")), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new UserLoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("E-mail ou senha inválidos.");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new UserLoginRequest
            {
                Email = "john.doe@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Email = request.Email,
                PasswordHash = "hashed_password",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("E-mail ou senha inválidos.");
        }
    }
}
