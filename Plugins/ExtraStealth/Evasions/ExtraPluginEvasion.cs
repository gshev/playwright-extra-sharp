using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class ExtraPluginEvasion : PlaywrightExtraPlugin
{
    public override string Name => "stealth-pluginEvasion";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "Plugin.js");
}