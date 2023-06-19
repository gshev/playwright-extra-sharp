using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Models;

internal class UserAgentBrand
{
    public UserAgentBrand(string brand, string version)
    {
        Brand = brand;
        Version = version;
    }

    [JsonPropertyName("brand")] public string Brand { get; }
    [JsonPropertyName("version")] public string Version { get; }
}