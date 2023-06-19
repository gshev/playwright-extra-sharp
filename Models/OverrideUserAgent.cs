using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Models;

internal class OverrideUserAgent
{
    public OverrideUserAgent(string userAgent, string platform, string acceptLanguage,
        UserAgentMetadata userAgentMetadata)
    {
        UserAgent = userAgent;
        Platform = platform;
        AcceptLanguage = acceptLanguage;
        UserAgentMetadata = userAgentMetadata;
    }

    [JsonPropertyName("userAgent")] public string UserAgent { get; }
    [JsonPropertyName("platform")] public string Platform { get; }
    [JsonPropertyName("acceptLanguage")] public string AcceptLanguage { get; }

    [JsonPropertyName("userAgentMetadata")]
    public UserAgentMetadata UserAgentMetadata { get; }
}