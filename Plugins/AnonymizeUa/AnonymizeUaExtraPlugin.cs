using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PlaywrightExtraSharp.Models;

namespace PlaywrightExtraSharp.Plugins.AnonymizeUa;

public class AnonymizeUaExtraPlugin : PlaywrightExtraPlugin
{
    private Func<string, string>? _customAction;
    public override string Name => "anonymize-ua";

    public override Func<IPage, Task> OnPageCreated => async page =>
    {
        var ua = await page.EvaluateAsync<string>("() => navigator.userAgent").ConfigureAwait(false);
        ua = ua.Replace("HeadlessChrome", "Chrome");

        var uaVersion = ua.Contains("Chrome/")
            ? Regex.Match(ua, @"Chrome\/([\d|.]+)").Groups[1].Value
            : Regex.Match(ua, @"\/([\d|.]+)").Groups is { } groups
                ? groups[groups.Count - 1].Value
                : "";

        var regex = new Regex(@"/\(([^)]+)\)/");
        ua = regex.Replace(ua, "(Windows NT 10.0; Win64; x64)");

        if (_customAction != null)
            ua = _customAction(ua);

        var platform = GetPlatform(ua);
        var brand = GetBrands(uaVersion);

        var isMobile = GetIsMobile(ua);
        var platformVersion = GetPlatformVersion(ua);
        var platformArch = GetPlatformArch(isMobile);
        var platformModel = GetPlatformModel(isMobile, ua);

        var overrideObject = new Dictionary<string, object>
        {
            { "userAgent", ua },
            { "platform", platform },
            { "acceptLanguage", "en-US, en" },
            {
                "userAgentMetadata", new Dictionary<string, object>
                {
                    { "brands", brand.ToArray() },
                    { "fullVersion", uaVersion },
                    { "platform", platform },
                    { "platformVersion", platformVersion },
                    { "architecture", platformArch },
                    { "model", platformModel },
                    { "mobile", isMobile }
                }
            }
        };

        var session = await page.Context.NewCDPSessionAsync(page).ConfigureAwait(false);
        await session.SendAsync("Network.setUserAgentOverride", overrideObject).ConfigureAwait(false);
    };

    public void CustomizeUa(Func<string, string>? uaAction = default)
    {
        _customAction = uaAction;
    }

    private string GetPlatform(string ua)
    {
        if (ua.Contains("Mac OS X")) return "Mac OS X";

        if (ua.Contains("Android")) return "Android";

        if (ua.Contains("Linux")) return "Linux";

        return "Windows";
    }

    private string GetPlatformVersion(string ua)
    {
        if (ua.Contains("Mac OS X ")) return Regex.Match(ua, "Mac OS X ([^)]+)").Groups[1].Value;

        if (ua.Contains("Android ")) return Regex.Match(ua, "Android ([^;]+)").Groups[1].Value;

        if (ua.Contains("Windows ")) return Regex.Match(ua, @"Windows .*?([\d|.]+);").Groups[1].Value;

        return string.Empty;
    }

    private string GetPlatformArch(bool isMobile)
    {
        return isMobile ? string.Empty : "x86";
    }

    protected string GetPlatformModel(bool isMobile, string ua)
    {
        return isMobile ? Regex.Match(ua, @"Android.*?;\s([^)]+)").Groups[1].Value : string.Empty;
    }

    private bool GetIsMobile(string ua)
    {
        return ua.Contains("Android");
    }

    private List<UserAgentBrand> GetBrands(string uaVersion)
    {
        var seed = int.Parse(uaVersion.Split('.')[0]);

        var order = new List<List<int>>
        {
            new()
            {
                0, 1, 2
            },
            new()
            {
                0, 2, 1
            },
            new()
            {
                1, 0, 2
            },
            new()
            {
                1, 2, 0
            },
            new()
            {
                2, 0, 1
            },
            new()
            {
                2, 1, 0
            }
        }[seed % 6];

        var escapedChars = new List<string>
        {
            " ",
            " ",
            ";"
        };

        var greaseyBrand = $"{escapedChars[order[0]]}Not{escapedChars[order[1]]}A{escapedChars[order[2]]}Brand";
        var greasedBrandVersionList = new Dictionary<int, UserAgentBrand>();

        greasedBrandVersionList.Add(order[0], new UserAgentBrand
        (
            greaseyBrand,
            "99"
        ));
        greasedBrandVersionList.Add(order[1], new UserAgentBrand
        (
            "Chromium",
            seed.ToString()
        ));

        greasedBrandVersionList.Add(order[2], new UserAgentBrand
        (
            "Google Chrome",
            seed.ToString()
        ));

        return greasedBrandVersionList.OrderBy(e => e.Key).Select(e => e.Value).ToList();
    }
}