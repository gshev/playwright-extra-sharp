using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class OutDimensions : PlaywrightExtraPlugin
{
    public override string Name => "stealth-dimensions";

    public override Func<IPage, Task> OnPageCreated =>
        async page => { await EvaluateScript(page, "Outdimensions.js"); };
}