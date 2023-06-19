using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class Codec : PlaywrightExtraPlugin
{
    public override string Name => "stealth-codec";

    public override Func<IPage, Task> OnPageCreated => async page => { await EvaluateScript(page, "Codec.js"); };
}