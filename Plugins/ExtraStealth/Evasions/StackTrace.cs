using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class StackTrace : PlaywrightExtraPlugin
{
    public override string Name => "stealth-stackTrace";

    public override Func<IPage, Task> OnPageCreated => async page => { await EvaluateScript(page, "Stacktrace.js"); };
}