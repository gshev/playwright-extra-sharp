using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class AntiCaptchaRequest
{
    public AntiCaptchaRequest(string clientKey, AntiCaptchaTask task)
    {
        ClientKey = clientKey;
        Task = task;
    }

    [JsonPropertyName("clientKey")] public string ClientKey { get; }

    [JsonPropertyName("task")] public AntiCaptchaTask Task { get; }
}

public class RequestForResultTask
{
    public RequestForResultTask(string clientKey, int taskId)
    {
        ClientKey = clientKey;
        TaskId = taskId;
    }

    [JsonPropertyName("clientKey")] public string ClientKey { get; }

    [JsonPropertyName("taskId")] public int TaskId { get; }
}

public class AntiCaptchaTaskResult
{
    public AntiCaptchaTaskResult(int errorId, int taskId)
    {
        ErrorId = errorId;
        TaskId = taskId;
    }

    [JsonPropertyName("errorId")] public int ErrorId { get; }

    [JsonPropertyName("taskId")] public int TaskId { get; }
}

public class AntiCaptchaTask
{
    public AntiCaptchaTask(string type, string websiteUrl, string websiteKey)
    {
        Type = type;
        WebsiteUrl = websiteUrl;
        WebsiteKey = websiteKey;
    }

    [JsonPropertyName("type")] public string Type { get; }

    [JsonPropertyName("websiteURL")] public string WebsiteUrl { get; }

    [JsonPropertyName("websiteKey")] public string WebsiteKey { get; }
}