using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.BlockResources;

public class BlockResourcesExtraPlugin : PlaywrightExtraPlugin
{
    private readonly List<BlockRule> _blockResources = new();

    public BlockResourcesExtraPlugin(IEnumerable<string>? blockResources = null, string? blockPattern = null)
    {
        if (blockResources != null)
            _blockResources.Add(new BlockRule(resourceType: blockResources));
        if (blockPattern != null)
            _blockResources.Add(new BlockRule(sitePattern: blockPattern));
    }

    public override string Name => "block-resources";

    public override Func<IPage, IRequest, Task> OnRequest =>
        (page, request) =>
        {
            var tcs = new TaskCompletionSource<bool>();

            page.RouteAsync("**/*", async route =>
            {
                if (_blockResources.Any(rule => rule.IsRequestBlocked(page, request)))
                    await route.AbortAsync();
                else
                    await route.ContinueAsync();

                tcs.SetResult(true);
            });

            return tcs.Task;
        };

    public override Func<BrowserTypeLaunchOptions?, BrowserTypeLaunchPersistentContextOptions?, Task> BeforeLaunch => (options1, options2) =>
    {
        if (options1 != null)
            options1.Args = options1.Args?.Append("--site-per-process").Append("--disable-features=IsolateOrigins")
                .ToArray();
        
        if (options2 != null)
            options2.Args = options2.Args?.Append("--site-per-process").Append("--disable-features=IsolateOrigins")
                .ToArray();
        
        return Task.CompletedTask;
    };

    private async Task OnPageRequest(object sender, IRequest request, TaskCompletionSource<bool> tcs)
    {
        if (sender is not IPage senderPage)
        {
            tcs.SetResult(true);
            return;
        }

        await senderPage.RouteAsync("**/*", async route =>
        {
            if (_blockResources.Any(rule => rule.IsRequestBlocked(senderPage, request)))
                await route.AbortAsync();
            else
                await route.ContinueAsync();

            tcs.SetResult(true);
        });
    }
}