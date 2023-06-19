using Microsoft.Playwright;
using PlaywrightExtraSharp.Plugins.Recaptcha.Provider;

namespace PlaywrightExtraSharp.Plugins.Recaptcha;

public class RecaptchaExtraPlugin : PlaywrightExtraPlugin
{
    private readonly Recaptcha _recaptcha;

    public RecaptchaExtraPlugin(IRecaptchaProvider provider, CaptchaOptions? opt = null)
    {
        _recaptcha = new Recaptcha(provider, opt ?? new CaptchaOptions());
    }

    public override string Name => "recaptcha";

    public override Func<BrowserNewContextOptions, IBrowser?, Task> BeforeContext =>
        (options, browser) =>
        {
            options.BypassCSP = true;
            return Task.CompletedTask;
        };

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page)
    {
        return await _recaptcha.Solve(page);
    }
}