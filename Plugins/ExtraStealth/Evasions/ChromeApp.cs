using System.Runtime.CompilerServices;
using Microsoft.Playwright;

[assembly: InternalsVisibleTo("Extra.Tests")]

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class ChromeApp : PlaywrightExtraPlugin
{
    public override string Name => "stealth-chromeApp";

    public override Func<IPage, Task> OnPageCreated => async page => { await EvaluateScript(page, "ChromeApp.js"); };
}