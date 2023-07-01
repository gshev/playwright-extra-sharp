using System.Diagnostics;
using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Helpers;

public static class PageHelpers
{
    /// <summary>
    /// Requests url and waits before network is idle or number of requests started equals finished
    /// </summary>
    /// <param name="page">Page from Playwright</param>
    /// <param name="url">Url to request</param>
    /// <param name="options">Page goto options</param>
    /// <param name="idleTime">Network idle time before request considered finished</param>
    /// <param name="timeBeforeScriptsActivate">Some sites load only one script and after setTimeout loads everything else. This time between page request completed and before any new requests will spawn will be awaited</param>
    /// <returns></returns>
    public static async Task<IResponse?> GotoAndWaitForIdleAsync(
        this IPage page,
        string url,
        PageGotoOptions? options = null,
        TimeSpan? idleTime = null,
        TimeSpan? timeBeforeScriptsActivate = null)
    {
        idleTime ??= TimeSpan.FromMilliseconds(500);
        timeBeforeScriptsActivate ??= TimeSpan.FromMilliseconds(500);

        var requestsStarted = 0;
        var requestsFinished = 0;

        var autoResetEvent = new AutoResetEvent(false);

        page.Request += PageOnRequestStarted;
        page.RequestFinished += PageOnRequestFinished;
        page.RequestFailed += PageOnRequestFinished;

        var retries = 10;
        IResponse? response = null;
        while (retries-- > 0)
        {
            try
            {
                response = await page.GotoAsync(url, options);
                await page.WaitForTimeoutAsync((float)timeBeforeScriptsActivate.Value.TotalMilliseconds);
                break;
            }
            catch (PlaywrightException pwe)
            {
            }
        }

        var lastRequestFinishedAt = DateTime.UtcNow;

        while (true)
        {
            autoResetEvent.WaitOne(100);

            if (requestsStarted == requestsFinished || DateTime.UtcNow - lastRequestFinishedAt >= idleTime)
                break;
        }

        page.Request -= PageOnRequestStarted;
        page.RequestFinished -= PageOnRequestFinished;
        page.RequestFailed -= PageOnRequestFinished;

        return response;

        void PageOnRequestStarted(object sender, IRequest request)
        {
            requestsStarted++;
            lastRequestFinishedAt = DateTime.UtcNow;
        }

        void PageOnRequestFinished(object sender, IRequest request)
        {
            requestsFinished++;
            lastRequestFinishedAt = DateTime.UtcNow;
            autoResetEvent.Set();
        }
    }
}