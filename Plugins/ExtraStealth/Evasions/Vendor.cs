using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class Vendor : PlaywrightExtraPlugin
{
    private readonly StealthVendorSettings _settings;

    public Vendor(StealthVendorSettings? settings = null)
    {
        _settings = settings ?? new StealthVendorSettings("Google Inc.");
    }

    public override string Name => "stealth-vendor";

    public override Func<IPage, Task> OnPageCreated => async page =>
    {
        await EvaluateScript(page, "Vendor.js", _settings.Vendor);
    };
}

public class StealthVendorSettings : IPlaywrightExtraPluginOptions
{
    public StealthVendorSettings(string vendor)
    {
        Vendor = vendor;
    }

    public string Vendor { get; }
}