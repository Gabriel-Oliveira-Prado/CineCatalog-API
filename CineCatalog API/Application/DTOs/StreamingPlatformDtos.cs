using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CineCatalog_API.Application.DTOs
{
    public class StreamingPlatformResponse
    {
        public int ProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string? Link { get; set; }
        public string? LogoPath { get; set; }
    }

    public class StreamingAvailabilityResponse
    {
        public string Region { get; set; } = "BR";
        public string Source { get; set; } = "JustWatch via TMDB";
        public string? Link { get; set; }
        public List<StreamingPlatformResponse> Platforms { get; set; } = new List<StreamingPlatformResponse>();
    }

    /// <summary>
    /// Mantém o valor persistido no banco compatível com os formatos antigos e sempre
    /// o devolve no formato canônico usado pela API.
    /// </summary>
    public static class StreamingPlatformsJson
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(IEnumerable<StreamingPlatformResponse> platforms)
        {
            return JsonSerializer.Serialize(Normalize(platforms), SerializerOptions);
        }

        public static List<StreamingPlatformResponse> Deserialize(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<StreamingPlatformResponse>();
            }

            try
            {
                using var document = JsonDocument.Parse(json);
                if (document.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return new List<StreamingPlatformResponse>();
                }

                var platforms = new List<StreamingPlatformResponse>();
                foreach (var item in document.RootElement.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    var name = GetString(item, "name") ?? GetString(item, "providerName");
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    platforms.Add(new StreamingPlatformResponse
                    {
                        ProviderId = GetInt(item, "providerId"),
                        Name = name,
                        Availability = GetString(item, "availability") ?? "Disponível",
                        // "url" era o formato salvo pela integração anterior.
                        Link = GetString(item, "link") ?? GetString(item, "url"),
                        LogoPath = GetString(item, "logoPath")
                    });
                }

                return Normalize(platforms);
            }
            catch (JsonException)
            {
                return new List<StreamingPlatformResponse>();
            }
        }

        private static string? GetString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
                ? property.GetString()
                : null;
        }

        private static int GetInt(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
                ? value
                : 0;
        }

        private static List<StreamingPlatformResponse> Normalize(IEnumerable<StreamingPlatformResponse> platforms)
        {
            return platforms
                .Where(platform => !string.IsNullOrWhiteSpace(platform.Name))
                .GroupBy(platform => platform.ProviderId > 0
                    ? $"id:{platform.ProviderId}"
                    : $"name:{platform.Name.Trim().ToUpperInvariant()}")
                .Select(group => new StreamingPlatformResponse
                {
                    ProviderId = group.First().ProviderId,
                    Name = group.First().Name.Trim(),
                    Availability = string.Join(" · ", group
                        .Select(platform => string.IsNullOrWhiteSpace(platform.Availability) ? "Disponível" : platform.Availability.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)),
                    Link = group.Select(platform => platform.Link).FirstOrDefault(link => !string.IsNullOrWhiteSpace(link)),
                    LogoPath = group.Select(platform => platform.LogoPath).FirstOrDefault(path => !string.IsNullOrWhiteSpace(path))
                })
                .OrderBy(platform => platform.Name)
                .ToList();
        }
    }
}