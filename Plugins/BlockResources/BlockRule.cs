using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.BlockResources;

public class BlockRule
{
    private readonly IPage? _page;
    private readonly IEnumerable<string>? _resourceType;
    private readonly string? _sitePattern;

    public BlockRule(IPage? page = null, string? sitePattern = null, IEnumerable<string>? resourceType = null)
    {
        _page = page;
        _sitePattern = sitePattern;
        _resourceType = resourceType;
    }

    public bool IsRequestBlocked(IPage? fromPage, IRequest request)
    {
        return IsResourcesBlocked(request.ResourceType) || IsSiteBlocked(request.Url) || IsPageBlocked(fromPage);
    }


    private bool IsPageBlocked(IPage? page)
    {
        return _page != null && page.Equals(_page);
    }

    private bool IsSiteBlocked(string siteUrl)
    {
        return !string.IsNullOrWhiteSpace(_sitePattern) && Regex.IsMatch(siteUrl, _sitePattern!);
    }

    private bool IsResourcesBlocked(string resource)
    {
        return _resourceType?.Contains(resource) ?? false;
    }
}