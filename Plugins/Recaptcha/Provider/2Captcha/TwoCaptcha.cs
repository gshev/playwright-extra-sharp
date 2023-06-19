using PlaywrightExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;

namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider._2Captcha;

public class TwoCaptcha : IRecaptchaProvider
{
    private readonly TwoCaptchaApi _api;
    private readonly ProviderOptions _options;

    public TwoCaptcha(string key, ProviderOptions? options = null)
    {
        _options = options ?? ProviderOptions.CreateDefaultOptions();
        _api = new TwoCaptchaApi(key, _options);
    }

    public async Task<string?> GetSolution(string key, string pageUrl, string? proxyStr = null)
    {
        var task = await _api.CreateTaskAsync(key, pageUrl);

        ThrowErrorIfBadStatus(task);

        await Task.Delay(_options.StartTimeoutSeconds * 1000);

        var result = await _api.GetSolution(task!.Request);

        ThrowErrorIfBadStatus(result.Data);

        return result.Data?.Request;
    }

    private static void ThrowErrorIfBadStatus(TwoCaptchaResponse? response)
    {
        if (response == null)
            throw new HttpRequestException("Two captcha request ends with empty response");
        if (response.Status != 1 || string.IsNullOrEmpty(response.Request))
            throw new HttpRequestException(
                $"Two captcha request ends with error [{response.Status}] {response.Request}");
    }
}