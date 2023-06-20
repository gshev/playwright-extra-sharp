using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class LoadTimes : PlaywrightExtraPlugin
{
    public override string Name => "stealth-loadTimes";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "LoadTimes.js");
}