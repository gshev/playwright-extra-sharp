using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class OutDimensions : PlaywrightExtraPlugin
{
    public override string Name => "stealth-dimensions";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "Outdimensions.js");
}