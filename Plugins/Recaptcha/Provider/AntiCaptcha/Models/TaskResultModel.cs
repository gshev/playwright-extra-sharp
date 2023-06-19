using System.Text.Json.Serialization;

namespace PlaywrightExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

public class Solution
{
    public Solution(string gRecaptchaResponse)
    {
        GRecaptchaResponse = gRecaptchaResponse;
    }

    [JsonPropertyName("gRecaptchaResponse")]
    public string GRecaptchaResponse { get; }
}

public class TaskResultModel
{
    public TaskResultModel(int errorId, string status, Solution solution, string cost, string ip, int createTime,
        int endTime, string solveCount)
    {
        ErrorId = errorId;
        Status = status;
        Solution = solution;
        Cost = cost;
        Ip = ip;
        CreateTime = createTime;
        EndTime = endTime;
        SolveCount = solveCount;
    }

    [JsonPropertyName("errorId")] public int ErrorId { get; }

    [JsonPropertyName("status")] public string Status { get; }

    [JsonPropertyName("solution")] public Solution Solution { get; }

    [JsonPropertyName("cost")] public string Cost { get; }

    [JsonPropertyName("ip")] public string Ip { get; }

    [JsonPropertyName("createTime")] public int CreateTime { get; }

    [JsonPropertyName("endTime")] public int EndTime { get; }

    [JsonPropertyName("solveCount")] public string SolveCount { get; }
}