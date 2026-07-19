using System;

namespace CineCatalog_API.Application.DTOs
{
    public class GenreCreateRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GenreUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GenreResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}