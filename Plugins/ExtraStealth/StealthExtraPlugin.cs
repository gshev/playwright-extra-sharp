using PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth;

public class StealthExtraPlugin : PlaywrightExtraPlugin
{
    private readonly IPlaywrightExtraPluginOptions[] _options;
    private readonly List<PlaywrightExtraPlugin> _standardEvasions;

    public StealthExtraPlugin(params IPlaywrightExtraPluginOptions[] options)
    {
        _options = options;
        _standardEvasions = GetStandardEvasions();
    }

    public override string Name => "stealth";

    public override PlaywrightExtraPlugin[] Dependencies => _standardEvasions.ToArray();

    private List<PlaywrightExtraPlugin> GetStandardEvasions()
    {
        return new List<PlaywrightExtraPlugin>
        {
            new WebDriver(),
            // new ChromeApp(),
            new ChromeSci(),
            new ChromeRuntime(),
            new Codec(),
            new Languages(GetOptionByType<StealthLanguagesOptions>()),
            new OutDimensions(),
            new Permissions(),
            new UserAgent(),
            new Vendor(GetOptionByType<StealthVendorSettings>()),
            new WebGl(GetOptionByType<StealthWebGLOptions>()),
            new ExtraPluginEvasion(),
            new StackTrace(),
            new HardwareConcurrency(GetOptionByType<StealthHardwareConcurrencyOptions>()),
            new ContentWindow()
            // playwright does not seems to have problem with SourceUrl
            // new SourceUrl()
        };
    }

    private T? GetOptionByType<T>() where T : IPlaywrightExtraPluginOptions
    {
        return _options.OfType<T>().FirstOrDefault();
    }

    public void RemoveEvasionByType<T>() where T : PlaywrightExtraPlugin
    {
        _standardEvasions.RemoveAll(ev => ev is T);
    }
}