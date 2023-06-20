# PlaywrightExtraSharp

[![NuGet Badge](https://buildstats.info/nuget/PlaywrightExtraSharp)](https://www.nuget.org/packages/PlaywrightExtraSharp)

Based on [Puppeteer extra sharp](https://github.com/Overmiind/Puppeteer-sharp-extra)
and [Node.js Playwright extra](https://github.com/berstend/puppeteer-extra/tree/master/packages/playwright-extra)

## Quickstart

Long way

```c#
// Initialization plugin builder
var playwrightExtra = new PlaywrightExtra(BrowserTypeEnum.Cromium); 

// Install browser
playwrightExtra.Install();

// Use stealth plugin
playwrightExtra.Use(new StealthPlugin());

// Launch the puppeteer browser with plugins
await playwrightExtra.LaunchAsync(new LaunchOptions()
{
    Headless = false
});

// Create a new page
var page = await playwrightExtra.NewPageAsync();
await page.GoToAsync("http://google.com");
```

Compact way

```c#
// Initialize, install, use plugin and launch
var playwrightExtra = await new PlaywrightExtra(BrowserTypeEnum.Cromium)
    .Install()
    .Use(new StealthPlugin())
    .LaunchAsync(new LaunchOptions()
    {
        Headless = false
    });
    
// Create a new page
var page = await playwrightExtra.NewPageAsync();
await page.GoToAsync("http://google.com");
```

## Note

Because of how Playwright behaves with pages, you should only create new pages through PlaywrightExtra wrapper and not
IBrowser.
Please refer to examples above.