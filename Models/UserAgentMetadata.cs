using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Models;

internal class UserAgentMetadata
{
    public UserAgentMetadata(List<UserAgentBrand> brands, string fullVersion, string platform,
        string platformVersion, string architecture, string model, bool mobile)
    {
        Brands = brands;
        FullVersion = fullVersion;
        Platform = platform;
        PlatformVersion = platformVersion;
        Architecture = architecture;
        Model = model;
        Mobile = mobile;
    }

    [JsonPropertyName("brands")] public List<UserAgentBrand> Brands { get; }
    [JsonPropertyName("fullVersion")] public string FullVersion { get; }
    [JsonPropertyName("platform")] public string Platform { get; }
    [JsonPropertyName("platformVersion")] public string PlatformVersion { get; }
    [JsonPropertyName("architecture")] public string Architecture { get; }
    [JsonPropertyName("model")] public string Model { get; }
    [JsonPropertyName("mobile")] public bool Mobile { get; }
}