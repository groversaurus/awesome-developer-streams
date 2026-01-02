using System.Text.Json.Serialization;

namespace AwesomeDevStreams.Models;

public class Platform
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class Streamer
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("aka")]
    public string? Aka { get; set; }
    
    [JsonPropertyName("handle")]
    public string? Handle { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("topics")]
    public List<string> Topics { get; set; } = new();
    
    [JsonPropertyName("platforms")]
    public List<Platform> Platforms { get; set; } = new();
    
    [JsonPropertyName("links")]
    public Dictionary<string, string> Links { get; set; } = new();
    
    [JsonPropertyName("languages")]
    public List<string> Languages { get; set; } = new();
    
    [JsonPropertyName("alphabeticalSection")]
    public string AlphabeticalSection { get; set; } = string.Empty;
}

public class TopicCount
{
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class Statistics
{
    [JsonPropertyName("totalStreamers")]
    public int TotalStreamers { get; set; }
    
    [JsonPropertyName("totalTopics")]
    public int TotalTopics { get; set; }
    
    [JsonPropertyName("totalPlatforms")]
    public int TotalPlatforms { get; set; }
    
    [JsonPropertyName("totalLanguages")]
    public int TotalLanguages { get; set; }
    
    [JsonPropertyName("platformBreakdown")]
    public Dictionary<string, int> PlatformBreakdown { get; set; } = new();
    
    [JsonPropertyName("languageBreakdown")]
    public Dictionary<string, int> LanguageBreakdown { get; set; } = new();
    
    [JsonPropertyName("topTopics")]
    public List<TopicCount> TopTopics { get; set; } = new();
}

public class StreamerData
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
    
    [JsonPropertyName("lastUpdated")]
    public string LastUpdated { get; set; } = string.Empty;
    
    [JsonPropertyName("statistics")]
    public Statistics Statistics { get; set; } = new();
    
    [JsonPropertyName("streamers")]
    public List<Streamer> Streamers { get; set; } = new();
}
