using System.Reflection;

namespace PlaywrightExtraSharp.Utils;

internal static class ResourceReader
{
    public static string ReadFile(string path, Assembly? customAssembly = null)
    {
        var assembly = customAssembly ?? Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream is null)
            throw new FileNotFoundException($"File with path {path} not found!");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}