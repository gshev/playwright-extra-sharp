using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class ChromeSci : PlaywrightExtraPlugin
{
    public override string Name => "stealth_sci";

    public override Func<IPage, Task> OnPageCreated => async page => { await EvaluateScript(page, "SCI.js"); };
}