using PlaywrightExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;
using PlaywrightExtraSharp.Plugins.Recaptcha.RestClient;
using RestSharp;

namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha;

public class AntiCaptchaApi
{
    private readonly RestClient.RestClient _client = new("http://api.anti-captcha.com");
    private readonly ProviderOptions _options;
    private readonly string _userKey;

    public AntiCaptchaApi(string userKey, ProviderOptions options)
    {
        _userKey = userKey;
        _options = options;
    }

    public Task<AntiCaptchaTaskResult?> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default)
    {
        var content = new AntiCaptchaRequest
        (
            _userKey,
            new AntiCaptchaTask
            (
                "NoCaptchaTaskProxyless",
                pageUrl,
                key
            )
        );

        var result = _client.PostWithJsonAsync<AntiCaptchaTaskResult>("createTask", content, token);
        return result;
    }


    public async Task<TaskResultModel?> PendingForResult(int taskId, CancellationToken token = default)
    {
        var content = new RequestForResultTask
        (
            _userKey,
            taskId
        );


        var request = new RestRequest("getTaskResult");
        request.AddJsonBody(content);
        request.Method = Method.Post;

        var result = await _client.CreatePollingBuilder<TaskResultModel>(request).TriesLimit(_options.PendingCount)
            .WithTimeoutSeconds(5).ActivatePollingAsync(
                response =>
                {
                    if (response.Data?.Status == "ready" || response.Data?.ErrorId != 0)
                        return PollingAction.Break;

                    return PollingAction.ContinuePolling;
                });
        return result.Data;
    }
}