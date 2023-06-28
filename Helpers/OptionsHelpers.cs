using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Helpers;

public static class OptionsHelpers
{
    public static BrowserNewContextOptions ToContextOptions(this BrowserNewPageOptions options, BrowserNewContextOptions? contextOptions = null)
    {
        return new BrowserNewContextOptions(contextOptions)
        {
            AcceptDownloads = options.AcceptDownloads,
            IgnoreHTTPSErrors = options.IgnoreHTTPSErrors,
            BypassCSP = options.BypassCSP,
            ViewportSize = options.ViewportSize,
            ScreenSize = options.ScreenSize,
            UserAgent = options.UserAgent,
            DeviceScaleFactor = options.DeviceScaleFactor,
            IsMobile = options.IsMobile,
            HasTouch = options.HasTouch,
            JavaScriptEnabled = options.JavaScriptEnabled,
            TimezoneId = options.TimezoneId,
            Geolocation = options.Geolocation,
            Locale = options.Locale,
            Permissions = options.Permissions,
            ExtraHTTPHeaders = options.ExtraHTTPHeaders,
            Offline = options.Offline,
            HttpCredentials = options.HttpCredentials,
            ColorScheme = options.ColorScheme,
            ReducedMotion = options.ReducedMotion,
            ForcedColors = options.ForcedColors,
            RecordHarPath = options.RecordHarPath,
            RecordHarContent = options.RecordHarContent,
            RecordHarMode = options.RecordHarMode,
            RecordHarOmitContent = options.RecordHarOmitContent,
            RecordHarUrlFilter = options.RecordHarUrlFilter,
            RecordHarUrlFilterString = options.RecordHarUrlFilterString,
            RecordHarUrlFilterRegex = options.RecordHarUrlFilterRegex,
            RecordVideoDir = options.RecordVideoDir,
            RecordVideoSize = options.RecordVideoSize,
            Proxy = options.Proxy,
            ServiceWorkers = options.ServiceWorkers,
            BaseURL = options.BaseURL,
            StrictSelectors = options.StrictSelectors
        };
    }
    
    public static BrowserTypeLaunchPersistentContextOptions ToPersistentContextOptions(this BrowserNewPageOptions options, BrowserTypeLaunchPersistentContextOptions? persistentContextOptions)
    {
        return new BrowserTypeLaunchPersistentContextOptions(persistentContextOptions)
        {
            AcceptDownloads = options.AcceptDownloads,
            IgnoreHTTPSErrors = options.IgnoreHTTPSErrors,
            BypassCSP = options.BypassCSP,
            ViewportSize = options.ViewportSize,
            ScreenSize = options.ScreenSize,
            UserAgent = options.UserAgent,
            DeviceScaleFactor = options.DeviceScaleFactor,
            IsMobile = options.IsMobile,
            HasTouch = options.HasTouch,
            JavaScriptEnabled = options.JavaScriptEnabled,
            TimezoneId = options.TimezoneId,
            Geolocation = options.Geolocation,
            Locale = options.Locale,
            Permissions = options.Permissions,
            ExtraHTTPHeaders = options.ExtraHTTPHeaders,
            Offline = options.Offline,
            HttpCredentials = options.HttpCredentials,
            ColorScheme = options.ColorScheme,
            ReducedMotion = options.ReducedMotion,
            ForcedColors = options.ForcedColors,
            RecordHarPath = options.RecordHarPath,
            RecordHarContent = options.RecordHarContent,
            RecordHarMode = options.RecordHarMode,
            RecordHarOmitContent = options.RecordHarOmitContent,
            RecordHarUrlFilter = options.RecordHarUrlFilter,
            RecordHarUrlFilterString = options.RecordHarUrlFilterString,
            RecordHarUrlFilterRegex = options.RecordHarUrlFilterRegex,
            RecordVideoDir = options.RecordVideoDir,
            RecordVideoSize = options.RecordVideoSize,
            Proxy = options.Proxy,
            ServiceWorkers = options.ServiceWorkers,
            BaseURL = options.BaseURL,
            StrictSelectors = options.StrictSelectors
        };
    }
}