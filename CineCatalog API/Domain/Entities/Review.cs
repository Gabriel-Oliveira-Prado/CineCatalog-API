using System;

namespace CineCatalog_API.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Movie Movie { get; set; } = null!;
    }
}
