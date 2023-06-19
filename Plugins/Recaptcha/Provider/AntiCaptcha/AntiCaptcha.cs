namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptcha : IRecaptchaProvider
{
    private readonly AntiCaptchaApi _api;
    private readonly ProviderOptions _options;

    public AntiCaptcha(string userKey, ProviderOptions? options = null)
    {
        _options = options ?? ProviderOptions.CreateDefaultOptions();
        _api = new AntiCaptchaApi(userKey, _options);
    }

    public async Task<string?> GetSolution(string key, string pageUrl, string? proxyStr = null)
    {
        var task = await _api.CreateTaskAsync(pageUrl, key);

        if (task == null)
            throw new HttpRequestException("AntiCaptcha request failed");

        await Task.Delay(_options.StartTimeoutSeconds * 1000);
        var result = await _api.PendingForResult(task.TaskId);

        if (result?.Status != "ready" || result.Solution is null || result.ErrorId != 0)
            throw new HttpRequestException($"AntiCaptcha request ends with error - {result?.ErrorId}");

        return result.Solution.GRecaptchaResponse;
    }
}