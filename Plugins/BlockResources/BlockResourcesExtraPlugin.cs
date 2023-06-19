using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.BlockResources;

public class BlockResourcesExtraPlugin : PlaywrightExtraPlugin
{
    private readonly List<BlockRule> _blockResources = new();

    public BlockResourcesExtraPlugin(IEnumerable<string>? blockResources = null)
    {
        if (blockResources != null)
            _blockResources.Add(new BlockRule(resourceType: blockResources));
    }

    public override string Name => "block-resources";

    public override Func<IPage, Task> OnPageCreated => page =>
    {
        page.Request += (_, request) => OnPageRequest(page, request);
        return Task.CompletedTask;
    };

    public override Func<BrowserTypeLaunchOptions?, Task> BeforeLaunch => options =>
    {
        if (options != null)
            options.Args = options.Args?.Append("--site-per-process").Append("--disable-features=IsolateOrigins")
                .ToArray();
        return Task.CompletedTask;
    };

    private async void OnPageRequest(IPage sender, IRequest request)
    {
        await sender.RouteAsync("**/*", async route =>
        {
            if (_blockResources.Any(rule => rule.IsRequestBlocked(sender, request)))
                await route.AbortAsync();
            else
                await route.ContinueAsync();
        });
    }
}