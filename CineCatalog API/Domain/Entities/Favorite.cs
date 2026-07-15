using System;

namespace CineCatalog_API.Domain.Entities
{
    public class Favorite
    {
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Movie Movie { get; set; } = null!;
    }
}
