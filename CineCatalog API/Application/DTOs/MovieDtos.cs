using System;
using System.Collections.Generic;

namespace CineCatalog_API.Application.DTOs
{
    public class MovieCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int DurationMinutes { get; set; }
        public string Rating { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public List<Guid> GenreIds { get; set; } = new List<Guid>();
    }

    public class MovieUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int DurationMinutes { get; set; }
        public string Rating { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public List<Guid> GenreIds { get; set; } = new List<Guid>();
    }

    public class MovieResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int DurationMinutes { get; set; }
        public string Rating { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<GenreResponse> Genres { get; set; } = new List<GenreResponse>();
    }

    public class MovieDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public int DurationMinutes { get; set; }
        public string Rating { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<GenreResponse> Genres { get; set; } = new List<GenreResponse>();
        public List<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>();
    }

    public class MovieQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public string? Search { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public int? ReleaseYear { get; set; }
        public double? MinRating { get; set; }
        public int? MinDuration { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortBy { get; set; } = "Title";
        public bool IsDescending { get; set; } = false;
    }
}
