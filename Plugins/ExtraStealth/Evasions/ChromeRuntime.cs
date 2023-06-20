using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime : PlaywrightExtraPlugin
{
    public override string Name => "stealth-runtime";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "Runtime.js");
}