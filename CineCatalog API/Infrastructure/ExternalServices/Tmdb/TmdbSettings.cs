namespace CineCatalog_API.Infrastructure.ExternalServices.Tmdb
{
    public class TmdbSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";
        public string ImageBaseUrl { get; set; } = "https://image.tmdb.org/t/p/w500";
        public string WatchRegion { get; set; } = "BR";
    }
}