using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using FluentValidation;
using CineCatalog_API.Domain.Interfaces;
using CineCatalog_API.Infrastructure.Data;
using CineCatalog_API.Infrastructure.Repositories;
using CineCatalog_API.Infrastructure.Security;
using CineCatalog_API.Application.Interfaces;
using CineCatalog_API.Application.Services;
using CineCatalog_API.Application.Mappings;
using CineCatalog_API.Infrastructure.ExternalServices.Tmdb;

namespace CineCatalog_API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Connection
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            services.AddDbContext<CineCatalogDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();

            // Business Logic Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IFavoriteService, FavoriteService>();

            // TMDb Integration Services
            services.Configure<TmdbSettings>(configuration.GetSection("Tmdb"));
            services.AddHttpClient<ITmdbClient, TmdbClient>();
            services.AddScoped<IMovieCatalogSyncService, MovieCatalogSyncService>();

            // Security Helpers
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            // JWT Authentication
            var keyString = configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Secret Key 'Jwt:Key' is not configured.");
            var key = Encoding.ASCII.GetBytes(keyString);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };
            });

            return services;
        }
    }
}