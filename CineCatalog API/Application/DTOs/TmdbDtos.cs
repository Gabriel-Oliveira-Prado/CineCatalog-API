using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CineCatalog_API.Application.DTOs
{
    public class TmdbSearchResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbSearchResult> Results { get; set; } = new List<TmdbSearchResult>();
    }

    public class TmdbSearchResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;
    }

    public class TmdbMovieDetails
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenre> Genres { get; set; } = new List<TmdbGenre>();
    }

    public class TmdbGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class TmdbCredits
    {
        [JsonPropertyName("cast")]
        public List<TmdbCastMember> Cast { get; set; } = new List<TmdbCastMember>();

        [JsonPropertyName("crew")]
        public List<TmdbCrewMember> Crew { get; set; } = new List<TmdbCrewMember>();
    }

    public class TmdbCastMember
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class TmdbCrewMember
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("job")]
        public string Job { get; set; } = string.Empty;
    }

    public class TmdbVideosResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbVideo> Results { get; set; } = new List<TmdbVideo>();
    }

    public class TmdbVideo
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("site")]
        public string Site { get; set; } = string.Empty;
    }

    public class TmdbReleaseDatesResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbReleaseDatesResult> Results { get; set; } = new List<TmdbReleaseDatesResult>();
    }

    public class TmdbReleaseDatesResult
    {
        [JsonPropertyName("iso_3166_1")]
        public string Iso3166 { get; set; } = string.Empty;

        [JsonPropertyName("release_dates")]
        public List<TmdbReleaseDateItem> ReleaseDates { get; set; } = new List<TmdbReleaseDateItem>();
    }

    public class TmdbReleaseDateItem
    {
        [JsonPropertyName("certification")]
        public string Certification { get; set; } = string.Empty;
    }
}
