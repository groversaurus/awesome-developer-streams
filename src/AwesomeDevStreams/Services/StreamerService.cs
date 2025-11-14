using System.Net.Http.Json;
using AwesomeDevStreams.Models;

namespace AwesomeDevStreams.Services;

public class StreamerService
{
    private readonly HttpClient _httpClient;
    private StreamerData? _cachedData;

    public StreamerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StreamerData> GetStreamersAsync()
    {
        if (_cachedData != null)
            return _cachedData;

        _cachedData = await _httpClient.GetFromJsonAsync<StreamerData>("data/streamers.json")
            ?? new StreamerData();
        
        return _cachedData;
    }

    public async Task<List<Streamer>> SearchStreamersAsync(string searchTerm, string? platform = null, string? language = null)
    {
        var data = await GetStreamersAsync();
        var query = data.Streamers.AsEnumerable();

        // Filter by search term (name, topics, description)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(s =>
                s.Name.ToLowerInvariant().Contains(term) ||
                s.Description.ToLowerInvariant().Contains(term) ||
                s.Topics.Any(t => t.ToLowerInvariant().Contains(term)));
        }

        // Filter by platform
        if (!string.IsNullOrWhiteSpace(platform))
        {
            query = query.Where(s => s.Platforms.Any(p => 
                p.Name.Equals(platform, StringComparison.OrdinalIgnoreCase)));
        }

        // Filter by language
        if (!string.IsNullOrWhiteSpace(language))
        {
            query = query.Where(s => s.Languages.Any(l => 
                l.Equals(language, StringComparison.OrdinalIgnoreCase)));
        }

        return query.OrderBy(s => s.Name).ToList();
    }

    public async Task<List<string>> GetAllTopicsAsync()
    {
        var data = await GetStreamersAsync();
        return data.Streamers
            .SelectMany(s => s.Topics)
            .Distinct()
            .OrderBy(t => t)
            .ToList();
    }

    public async Task<List<string>> GetAllPlatformsAsync()
    {
        var data = await GetStreamersAsync();
        return data.Streamers
            .SelectMany(s => s.Platforms.Select(p => p.Name))
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }

    public async Task<List<string>> GetAllLanguagesAsync()
    {
        var data = await GetStreamersAsync();
        return data.Streamers
            .SelectMany(s => s.Languages)
            .Distinct()
            .OrderBy(l => l)
            .ToList();
    }
}
