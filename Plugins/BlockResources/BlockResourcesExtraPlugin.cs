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

    public override Func<BrowserTypeLaunchOptions?, Task> BeforeLaunch => options =>
    {
        if (options != null)
            options.Args = options.Args?.Append("--site-per-process").Append("--disable-features=IsolateOrigins")
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