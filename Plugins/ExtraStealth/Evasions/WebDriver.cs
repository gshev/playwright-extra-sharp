using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class WebDriver : PlaywrightExtraPlugin
{
    public override string Name => "stealth-webDriver";

    public override Func<IPage, Task> OnPageCreated => page => EvaluateScript(page, "WebDriver.js");

    public override Func<BrowserTypeLaunchOptions?, BrowserTypeLaunchPersistentContextOptions?, Task> BeforeLaunch =>
        (options1, options2) =>
        {
            var args = options1?.Args?.ToList() ?? options2?.Args?.ToList() ?? new List<string>();
            var idx = args.FindIndex(e => e.StartsWith("--disable-blink-features="));
            if (idx != -1)
            {
                var arg = args[idx];
                args[idx] = $"{arg}, AutomationControlled";
                return Task.CompletedTask;
            }

            args.Add("--disable-blink-features=AutomationControlled");

            if (options1 != null) options1.Args = args.ToArray();
            if (options2 != null) options2.Args = args.ToArray();
            return Task.CompletedTask;
        };
}