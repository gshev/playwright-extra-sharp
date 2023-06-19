using PlaywrightExtraSharp.Models;

namespace PlaywrightExtraSharp.Helpers;

public static class EnumHelpers
{
    public static string GetBrowserName(this BrowserTypeEnum browserTypeEnum)
    {
        return browserTypeEnum.ToString().ToLowerInvariant();
    }
}