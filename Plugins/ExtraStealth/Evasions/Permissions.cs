using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class Permissions : PlaywrightExtraPlugin
{
    public override string Name => "stealth-permissions";

    public override Func<IPage, Task> OnPageCreated => async page => { await EvaluateScript(page, "Permissions.js"); };
}