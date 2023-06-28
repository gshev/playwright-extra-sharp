using System.Diagnostics;
using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Helpers;

public static class PageHelpers
{
    public static async Task<IResponse?> GotoAndWaitForIdleAsync(this IPage page, string url, PageGotoOptions? options = null, TimeSpan? idleTime = null)
    {
        idleTime ??= TimeSpan.FromMilliseconds(500);
        
        //var requestsStarted = 0;
        //var requestsFinished = 0;

        //var autoResetEvent = new AutoResetEvent(false);

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
                break;
            }
            catch (PlaywrightException pwe)
            {
                
            }    
        }

        var lastRequestFinishedAt = DateTime.UtcNow;

        while (DateTime.UtcNow - lastRequestFinishedAt < idleTime)
        {
            await Task.Delay(10);
            //autoResetEvent.WaitOne(10);

            // if (requestsStarted == requestsFinished || DateTime.UtcNow - lastRequestFinishedAt >= timeout)
            //     break;
        }
        
        page.Request -= PageOnRequestStarted;
        page.RequestFinished -= PageOnRequestFinished;
        page.RequestFailed -= PageOnRequestFinished;

        return response;
        
        void PageOnRequestStarted(object sender, IRequest request)
        {
            //requestsStarted++;
            lastRequestFinishedAt = DateTime.UtcNow;
            //autoResetEvent.Reset();
        }

        void PageOnRequestFinished(object sender, IRequest request)
        {
            //requestsFinished++;
            lastRequestFinishedAt = DateTime.UtcNow;
            //autoResetEvent.Set();
        }
    }
}