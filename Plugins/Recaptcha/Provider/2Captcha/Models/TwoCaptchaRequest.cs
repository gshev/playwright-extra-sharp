using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;

internal class TwoCaptchaRequest
{
    public TwoCaptchaRequest(string key)
    {
        Key = key;
    }

    [JsonPropertyName("key")] public string Key { get; }
}

internal class TwoCaptchaTask : TwoCaptchaRequest
{
    public TwoCaptchaTask(string key, string method, string googleKey, string pageUrl) : base(key)
    {
        Method = method;
        GoogleKey = googleKey;
        PageUrl = pageUrl;
    }

    [JsonPropertyName("method")] public string Method { get; } = "userrecaptcha";
    [JsonPropertyName("googlekey")] public string GoogleKey { get; }
    [JsonPropertyName("pageurl")] public string PageUrl { get; }
}

internal class TwoCaptchaRequestForResult : TwoCaptchaRequest
{
    public TwoCaptchaRequestForResult(string key, string action, string id) : base(key)
    {
        Action = action;
        Id = id;
    }

    [JsonPropertyName("action")] public string Action { get; } = "get";
    [JsonPropertyName("id")] public string Id { get; }
}