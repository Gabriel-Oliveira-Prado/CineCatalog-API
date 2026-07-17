using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CineCatalog_API.Application.DTOs
{
    public class TmdbWatchProvidersResponse
    {
        [JsonPropertyName("results")]
        public Dictionary<string, TmdbWatchProviderRegion> Results { get; set; } = new Dictionary<string, TmdbWatchProviderRegion>();
    }

    public class TmdbWatchProviderRegion
    {
        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("flatrate")]
        public List<TmdbWatchProvider> Flatrate { get; set; } = new List<TmdbWatchProvider>();

        [JsonPropertyName("free")]
        public List<TmdbWatchProvider> Free { get; set; } = new List<TmdbWatchProvider>();

        [JsonPropertyName("ads")]
        public List<TmdbWatchProvider> Ads { get; set; } = new List<TmdbWatchProvider>();

        [JsonPropertyName("rent")]
        public List<TmdbWatchProvider> Rent { get; set; } = new List<TmdbWatchProvider>();

        [JsonPropertyName("buy")]
        public List<TmdbWatchProvider> Buy { get; set; } = new List<TmdbWatchProvider>();
    }

    public class TmdbWatchProvider
    {
        [JsonPropertyName("provider_id")]
        public int ProviderId { get; set; }

        [JsonPropertyName("provider_name")]
        public string ProviderName { get; set; } = string.Empty;

        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }
    }
}
