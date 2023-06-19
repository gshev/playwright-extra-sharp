using Microsoft.Playwright;
using PlaywrightExtraSharp.Models;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class ContentWindow : PlaywrightExtraPlugin
{
    public override string Name => "Iframe.ContentWindow";

    public override PluginRequirement[] Requirements => new[]
    {
        PluginRequirement.RunLast
    };

    public override Func<IPage, Task> OnPageCreated =>
        async page => { await EvaluateScript(page, "ContentWindow.js"); };
}