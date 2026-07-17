using AutoMapper;
using CineCatalog_API.Domain.Entities;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserResponse>();

            // Genre mappings
            CreateMap<Genre, GenreResponse>();
            CreateMap<GenreCreateRequest, Genre>();
            CreateMap<GenreUpdateRequest, Genre>();

            // Movie mappings
            CreateMap<Movie, MovieResponse>()
                .ForMember(dest => dest.StreamingPlatforms, opt => opt.MapFrom(src => StreamingPlatformsJson.Deserialize(src.StreamingPlatforms)));
            CreateMap<Movie, MovieDetailResponse>()
                .ForMember(dest => dest.StreamingPlatforms, opt => opt.MapFrom(src => StreamingPlatformsJson.Deserialize(src.StreamingPlatforms)));
            CreateMap<MovieCreateRequest, Movie>()
                .ForMember(dest => dest.Genres, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());
                
            CreateMap<MovieUpdateRequest, Movie>()
                .ForMember(dest => dest.Genres, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());

            // Review mappings
            CreateMap<Review, ReviewResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl));

            CreateMap<ReviewUpdateRequest, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MovieId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Movie, opt => opt.Ignore());
        }
    }
}
