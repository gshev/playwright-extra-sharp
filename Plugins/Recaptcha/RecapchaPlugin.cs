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

    public override Func<BrowserNewContextOptions?, BrowserTypeLaunchPersistentContextOptions?, Task> BeforeContext =>
        (options1, options2) =>
        {
            if(options1 != null)
                options1.BypassCSP = true;
            if(options2 != null)
                options2.BypassCSP = true;
            return Task.CompletedTask;
        };

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page)
    {
        return await _recaptcha.Solve(page);
    }
}