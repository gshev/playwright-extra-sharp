using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider._2Captcha.Models;

internal class TwoCaptchaResponse
{
    public TwoCaptchaResponse(int status, string request)
    {
        Status = status;
        Request = request;
    }

    [JsonPropertyName("status")] public int Status { get; }
    [JsonPropertyName("request")] public string Request { get; }
}