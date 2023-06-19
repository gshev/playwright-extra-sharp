using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class LoadTimes : PlaywrightExtraPlugin
{
    public override string Name => "stealth-loadTimes";

    public override Func<IPage, Task> OnPageCreated => async page => { await EvaluateScript(page, "LoadTimes.js"); };
}