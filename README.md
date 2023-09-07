# PlaywrightExtraSharp

[![NuGet Badge](https://buildstats.info/nuget/PlaywrightExtraSharp)](https://www.nuget.org/packages/PlaywrightExtraSharp)

Based on [Puppeteer extra sharp](https://github.com/Overmiind/Puppeteer-sharp-extra)
and [Node.js Playwright extra](https://github.com/berstend/puppeteer-extra/tree/master/packages/playwright-extra)

## Quickstart

Long way

```csharp
// Initialization plugin builder
var playwrightExtra = new PlaywrightExtra(BrowserTypeEnum.Chromium); 

// Install browser
playwrightExtra.Install();

// Use stealth plugin
playwrightExtra.Use(new StealthExtraPlugin());

// Launch the puppeteer browser with plugins
await playwrightExtra.LaunchAsync(new ()
{
    Headless = false
});

// Create a new page
var page = await playwrightExtra.NewPageAsync();
await page.GoToAsync("http://google.com");
```

Compact way

```csharp
// Initialize, install, use plugin and launch
var playwrightExtra = await new PlaywrightExtra(BrowserTypeEnum.Chromium)
    .Install()
    .Use(new StealthExtraPlugin())
    .LaunchAsync(new ()
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

## Context Persistence

There are 4 different ways to work with context in PlaywrightExtra:

1. launch incognito (as default was) with permanent context

```csharp
.LaunchAsync(new ()
    {
        Headless = false
    },
    persistContext: true)
```

2. launch incognito with new context per page (so when page is closed, context is disposed)

```csharp
.LaunchAsync(new ()
    {
        Headless = false
    },
    persistContext: false)
```

3. launch persistent (user data dir is considered) with permanent context

```csharp
.LaunchPersistentAsync(new ()
    {
        Headless = false
    },
    persistContext: true)
```

4. launch persistent with new context per page

```csharp
.LaunchPersistentAsync(new ()
    {
        Headless = false
    },
    persistContext: false)
```

## User data directory

While running persistent mode you can specify path to user data directory.

When context is persisted, specify directory at launch:

```csharp
.LaunchPersistentAsync(new ()
    {
        Headless = false
    },
    persistContext: true,
    userDataDir: "/path/to/userdir")
```

When context is created for each page (```persistContext: false```), specify directory at page creation:

```csharp
var page = await _playwrightExtra.NewPageAsync(userDataDir: "/path/to/userdir");
```

## Helpers

For convenience, you can use helper method when performing request:

```csharp
await page.GotoAndWaitForIdleAsync("http://google.com/",
    idleTime: TimeSpan.FromMilliseconds(1000));
```

Basically this works like if you perform request with option ```WaitUntil = WaitUntilState.NetworkIdle```

But unlike builtin method, you can specify amount of time to wait after last request has fired. 