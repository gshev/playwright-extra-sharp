using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class Codec : PlaywrightExtraPlugin
{
    public override string Name => "stealth-codec";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "Codec.js");
}