using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins;

public class DisposeContext : PlaywrightExtraPlugin
{
    public override string Name => "dispose-context";

    public override Func<IPage, Task> OnPageClose => page => page.Context.DisposeAsync().AsTask();
}