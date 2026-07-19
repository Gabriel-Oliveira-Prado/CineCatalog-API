using System;

namespace CineCatalog_API.Application.DTOs
{
    public class ReviewCreateRequest
    {
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewUpdateRequest
    {
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatarUrl { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}