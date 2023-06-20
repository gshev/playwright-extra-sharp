using System.Web;
using Microsoft.Playwright;
using PlaywrightExtraSharp.Plugins.Recaptcha.Provider;
using PlaywrightExtraSharp.Utils;

namespace PlaywrightExtraSharp.Plugins.Recaptcha;

public class Recaptcha
{
    private readonly CaptchaOptions _options;
    private readonly IRecaptchaProvider _provider;

    public Recaptcha(IRecaptchaProvider provider, CaptchaOptions options)
    {
        _provider = provider;
        _options = options;
    }

    public async Task<RecaptchaResult> Solve(IPage page)
    {
        try
        {
            var key = await GetKeyAsync(page);
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException();
            var solution = await GetSolutionAsync(key!, page.Url);
            if (string.IsNullOrEmpty(solution))
                throw new InvalidOperationException();
            await WriteToInput(page, solution!);

            return new RecaptchaResult();
        }
        catch (CaptchaException ex)
        {
            return new RecaptchaResult(false, ex);
        }
    }

    private async Task<string?> GetKeyAsync(IPage page)
    {
        var element =
            await page.QuerySelectorAsync("iframe[src^='https://www.google.com/recaptcha/api2/anchor'][name^=\"a-\"]");

        if (element == null)
            throw new CaptchaException(page.Url, "Recaptcha key not found!");

        var src = await element.GetPropertyAsync("src");

        if (src == null)
            throw new CaptchaException(page.Url, "Recaptcha key not found!");

        var key = HttpUtility.ParseQueryString(src.ToString()!).Get("k");
        return key;
    }

    private async Task<string?> GetSolutionAsync(string key, string urlPage)
    {
        return await _provider.GetSolution(key, urlPage);
    }

    private async Task WriteToInput(IPage page, string value)
    {
        await page.EvaluateAsync(
            $"() => {{document.getElementById('g-recaptcha-response').innerHTML='{value}'}}");


        var script = ResourceReader.ReadFile(GetType().Namespace + ".Scripts.EnterRecaptchaCallBackScript.js");

        try
        {
            await page.EvaluateAsync($@"(value) => {{{script}}}", value);
        }
        catch
        {
            // ignored
        }
    }
}