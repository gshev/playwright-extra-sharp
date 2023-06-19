namespace PlaywrightExtraSharp.Models;

public sealed class PluginData
{
    public PluginData(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
}