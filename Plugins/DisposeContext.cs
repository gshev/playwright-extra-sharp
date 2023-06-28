using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins;

public class DisposeContext : PlaywrightExtraPlugin
{
    public override string Name => "dispose-context";

    public override Func<IPage, Task> OnPageClose => page =>
    {
        //Console.WriteLine("Disposing page");
        return page.Context.DisposeAsync().AsTask();
    };
}