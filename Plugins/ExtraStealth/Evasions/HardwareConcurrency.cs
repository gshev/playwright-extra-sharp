using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class HardwareConcurrency : PlaywrightExtraPlugin
{
    private readonly StealthHardwareConcurrencyOptions _options;

    public HardwareConcurrency(StealthHardwareConcurrencyOptions? options = null)
    {
        _options = options ?? new StealthHardwareConcurrencyOptions(4);
    }

    public override string Name => "stealth/hardwareConcurrency";

    public override Func<IPage, Task> OnPageCreated =>
        page => EvaluateScript(page, "HardwareConcurrency.js", _options.Concurrency);
}

public class StealthHardwareConcurrencyOptions : IPlaywrightExtraPluginOptions
{
    public StealthHardwareConcurrencyOptions(int concurrency)
    {
        Concurrency = concurrency;
    }

    public int Concurrency { get; }
}