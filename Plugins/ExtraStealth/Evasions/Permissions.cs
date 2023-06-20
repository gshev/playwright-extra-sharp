using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class Permissions : PlaywrightExtraPlugin
{
    public override string Name => "stealth-permissions";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "Permissions.js");
}