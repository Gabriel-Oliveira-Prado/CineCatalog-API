using System;
using System.Collections.Generic;

namespace CineCatalog_API.Domain.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
