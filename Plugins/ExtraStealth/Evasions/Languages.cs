using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class Languages : PlaywrightExtraPlugin
{
    private readonly StealthLanguagesOptions _options;

    public Languages(StealthLanguagesOptions? options = null)
    {
        _options = options ?? new StealthLanguagesOptions("en-US", "en");
    }

    public override string Name => "stealth-language";

    public override Func<IPage, Task> OnPageCreated => async page =>
    {
        await EvaluateScript(page, "Language.js", _options.Languages);
    };
}

public class StealthLanguagesOptions : IPlaywrightExtraPluginOptions
{
    public StealthLanguagesOptions(params string[] languages)
    {
        Languages = languages.Cast<object>().ToArray();
    }

    public object[] Languages { get; }
}