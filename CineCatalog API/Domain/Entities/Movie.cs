using System;
using System.Collections.Generic;

namespace CineCatalog_API.Domain.Entities
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty; // Elenco
        public int ReleaseYear { get; set; }
        public int DurationMinutes { get; set; }
        public string Rating { get; set; } = string.Empty; // Classificação indicativa
        public double AverageRating { get; set; } = 0.0;
        public int ReviewsCount { get; set; } = 0;
        public string ImageUrl { get; set; } = string.Empty; // Poster/Imagem
        public string TrailerUrl { get; set; } = string.Empty; // Link do trailer
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
