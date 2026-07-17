using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CineCatalog_API.Application.DTOs;
using CineCatalog_API.Application.Exceptions;
using CineCatalog_API.Application.Interfaces;

namespace CineCatalog_API.Infrastructure.ExternalServices.Tmdb
{
    public class TmdbClient : ITmdbClient
    {
        private readonly HttpClient _httpClient;
        private readonly TmdbSettings _settings;

        public TmdbClient(HttpClient httpClient, IOptions<TmdbSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // Suporte para tokens de leitura v4 (maiores que 50 chars) via Header
            if (!string.IsNullOrEmpty(_settings.ApiKey) && _settings.ApiKey.Length > 50)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            }
        }

        public async Task<List<TmdbSearchResult>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TmdbSearchResult>();

            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "query", query },
                    { "language", "pt-BR" }
                };

                var url = BuildUrl("search/movie", queryParams);
                var response = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);
                return response?.Results ?? new List<TmdbSearchResult>();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                throw new TmdbUnavailableException("O serviço do TMDb está temporariamente indisponível. Erro ao buscar filmes.", ex);
            }
        }

        public async Task<TmdbMovieDetails> GetMovieDetailsAsync(int tmdbId)
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "language", "pt-BR" }
                };

                var url = BuildUrl($"movie/{tmdbId}", queryParams);
                var details = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(url);
                
                if (details == null)
                    throw new TmdbUnavailableException($"Não foi possível obter os detalhes do filme TMDb ID {tmdbId}.");

                return details;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                throw new TmdbUnavailableException($"O serviço do TMDb está temporariamente indisponível. Erro ao obter detalhes do filme ID {tmdbId}.", ex);
            }
        }

        public async Task<TmdbCredits> GetCreditsAsync(int tmdbId)
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "language", "pt-BR" }
                };

                var url = BuildUrl($"movie/{tmdbId}/credits", queryParams);
                var credits = await _httpClient.GetFromJsonAsync<TmdbCredits>(url);
                return credits ?? new TmdbCredits();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                throw new TmdbUnavailableException($"O serviço do TMDb está temporariamente indisponível. Erro ao obter créditos do filme ID {tmdbId}.", ex);
            }
        }

        public async Task<string?> GetTrailerUrlAsync(int tmdbId)
        {
            try
            {
                // Tenta buscar vídeos em pt-BR
                var queryParams = new Dictionary<string, string>
                {
                    { "language", "pt-BR" }
                };

                var url = BuildUrl($"movie/{tmdbId}/videos", queryParams);
                var response = await _httpClient.GetFromJsonAsync<TmdbVideosResponse>(url);
                var video = FindTrailer(response?.Results);

                // Se não encontrar trailer em português, tenta em inglês
                if (video == null)
                {
                    queryParams["language"] = "en-US";
                    url = BuildUrl($"movie/{tmdbId}/videos", queryParams);
                    response = await _httpClient.GetFromJsonAsync<TmdbVideosResponse>(url);
                    video = FindTrailer(response?.Results);
                }

                return video != null ? $"https://www.youtube.com/watch?v={video.Key}" : null;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                // Falha silenciosa no trailer: retorna null para não quebrar o fluxo principal
                return null;
            }
        }

        public async Task<TmdbWatchProvidersResponse?> GetWatchProvidersAsync(int tmdbId)
        {
            try
            {
                var url = BuildUrl($"movie/{tmdbId}/watch/providers");
                return await _httpClient.GetFromJsonAsync<TmdbWatchProvidersResponse>(url);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                // A disponibilidade é complementar: preservamos os dados já salvos se o TMDB estiver indisponível.
                return null;
            }
        }

        private TmdbVideo? FindTrailer(List<TmdbVideo>? videos)
        {
            if (videos == null) return null;

            // Busca por Trailer no YouTube
            return videos.FirstOrDefault(v => 
                v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase) && 
                v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase))
                ?? videos.FirstOrDefault(v => v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<TmdbSearchResult>> GetTrendingMoviesAsync()
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "language", "pt-BR" }
                };

                var url = BuildUrl("movie/popular", queryParams);
                var response = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);
                return response?.Results ?? new List<TmdbSearchResult>();
            }
            catch (Exception ex)
            {
                throw new TmdbUnavailableException("O serviço do TMDb está temporariamente indisponível. Erro ao obter filmes populares.", ex);
            }
        }

        public async Task<string> GetReleaseDateCertificationAsync(int tmdbId)
        {
            try
            {
                var url = BuildUrl($"movie/{tmdbId}/release_dates");
                var response = await _httpClient.GetFromJsonAsync<TmdbReleaseDatesResponse>(url);
                if (response == null) return "Livre";

                // Procura por Brasil (BR)
                var brResult = response.Results.FirstOrDefault(r => r.Iso3166.Equals("BR", StringComparison.OrdinalIgnoreCase));
                if (brResult != null)
                {
                    var cert = brResult.ReleaseDates.FirstOrDefault(d => !string.IsNullOrWhiteSpace(d.Certification))?.Certification;
                    if (!string.IsNullOrWhiteSpace(cert))
                    {
                        return cert;
                    }
                }

                // Fallback para EUA (US)
                var usResult = response.Results.FirstOrDefault(r => r.Iso3166.Equals("US", StringComparison.OrdinalIgnoreCase));
                if (usResult != null)
                {
                    var cert = usResult.ReleaseDates.FirstOrDefault(d => !string.IsNullOrWhiteSpace(d.Certification))?.Certification;
                    if (!string.IsNullOrWhiteSpace(cert))
                    {
                        return MapUsCertificationToBr(cert);
                    }
                }

                return "Livre";
            }
            catch
            {
                return "Livre";
            }
        }

        private static string MapUsCertificationToBr(string cert)
        {
            return cert.ToUpperInvariant() switch
            {
                "G" => "Livre",
                "PG" => "Livre",
                "PG-13" => "12",
                "R" => "16",
                "NC-17" => "18",
                _ => "Livre"
            };
        }

        private string BuildUrl(string path, Dictionary<string, string>? queryParams = null)
        {
            var baseUrl = _settings.BaseUrl.TrimEnd('/');
            var url = $"{baseUrl}/{path.TrimStart('/')}";
            var queries = new List<string>();

            // Se for chave v3 curta, adicionamos na query string.
            // Se for token v4 longo, ele vai no Header de autorização.
            bool isV4 = !string.IsNullOrEmpty(_settings.ApiKey) && _settings.ApiKey.Length > 50;
            if (!isV4 && !string.IsNullOrEmpty(_settings.ApiKey))
            {
                queries.Add($"api_key={_settings.ApiKey}");
            }

            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    queries.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
                }
            }

            if (queries.Count > 0)
            {
                url += "?" + string.Join("&", queries);
            }

            return url;
        }
    }
}
