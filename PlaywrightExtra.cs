using Microsoft.Playwright;
using PlaywrightExtraSharp.Helpers;
using PlaywrightExtraSharp.Models;
using PlaywrightExtraSharp.Plugins;

namespace PlaywrightExtraSharp;

public class PlaywrightExtra : IBrowser, IDisposable
{
    private readonly BrowserTypeEnum _browserTypeEnum;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPlaywright? _playwright;
    private List<PlaywrightExtraPlugin> _plugins = new();
    private bool _persistContext;
    private bool _persistentLaunch;
    private string? _userDataDir;
    private BrowserNewContextOptions? _contextOptions;
    private BrowserTypeLaunchPersistentContextOptions? _persistentContextOptions;

    public PlaywrightExtra(BrowserTypeEnum browserTypeEnum)
    {
        _browserTypeEnum = browserTypeEnum;

        //Use(new DisposeContext());
    }

    public PlaywrightExtra Use(PlaywrightExtraPlugin extraPlugin)
    {
        _plugins.Add(extraPlugin);
        ResolveDependencies(extraPlugin);
        extraPlugin.OnPluginRegistered();

        return this;
    }

    public PlaywrightExtra Install()
    {
        var exitCode = Program.Main(new[] { "install", "--with-deps", _browserTypeEnum.GetBrowserName() });
        if (exitCode != 0) throw new Exception($"Playwright exited with code {exitCode}");

        return this;
    }

    public async Task<PlaywrightExtra> LaunchPersistentAsync(BrowserTypeLaunchPersistentContextOptions? options = null, bool persistContext = true, string? userDataDir = null)
    {
        _persistContext = persistContext;
        _persistentLaunch = true;
        _userDataDir = userDataDir;
        _persistentContextOptions = options ?? new BrowserTypeLaunchPersistentContextOptions();
        
        _playwright = await Playwright.CreateAsync();
        
        await TriggerEventAndWait(x => x.BeforeLaunch(null, options));

        if (_persistContext)
        {
            await TriggerEventAndWait(x => x.BeforeContext(null, options));
            _browserContext = await _playwright[_browserTypeEnum.GetBrowserName()]
                .LaunchPersistentContextAsync(userDataDir ?? "", options);
            
            await TriggerEventAndWait(x => x.OnContextCreated(_browserContext, null, options));
            
            _browser = _browserContext.Browser;

            await TriggerEventAndWait(x => x.OnBrowser(_browser, null));

            _browserContext.Close += async (_, targetBrowserContext) =>
                await TriggerEventAndWait(x => x.OnDisconnected(null, targetBrowserContext));

            await TriggerEventAndWait(x => x.AfterLaunch(_browser));
        }

        OrderPlugins();
        CheckPluginRequirements(new BrowserStartContext
        {
            StartType = StartType.Launch,
            IsHeadless = options?.Headless ?? false
        });

        return this;
    }
    
    public async Task<PlaywrightExtra> LaunchAsync(BrowserTypeLaunchOptions? options = null, bool persistContext = true, BrowserNewContextOptions? contextOptions = null)
    {
        _persistContext = persistContext;
        _persistentLaunch = false;
        _contextOptions = contextOptions ?? new BrowserNewContextOptions();

        await TriggerEventAndWait(x => x.BeforeLaunch(options, null));

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright[_browserTypeEnum.GetBrowserName()].LaunchAsync(options);
        await TriggerEventAndWait(x => x.OnBrowser(_browser, null));

        _browser.Disconnected += async (_, targetBrowser) =>
            await TriggerEventAndWait(x => x.OnDisconnected(_browser, null));

        if (_persistContext)
        {
            await TriggerEventAndWait(x => x.BeforeContext(contextOptions, null));
            _browserContext = await _browser.NewContextAsync(contextOptions);
            await TriggerEventAndWait(x => x.OnContextCreated(_browserContext, contextOptions, null));

            _browserContext.Close += async (_, context) =>
                await TriggerEventAndWait(x => x.OnDisconnected(null, context));

            var blankPage = await _browserContext.NewPageAsync();
            await blankPage.GotoAsync("about:blank");
        }

        await TriggerEventAndWait(x => x.AfterLaunch(_browser));

        OrderPlugins();
        CheckPluginRequirements(new BrowserStartContext
        {
            StartType = StartType.Launch,
            IsHeadless = options?.Headless ?? false
        });

        return this;
    }

    public async Task<PlaywrightExtra> ConnectAsync(string wsEndpoint, BrowserTypeConnectOptions? options = null)
    {
        _persistContext = true;
        _persistentLaunch = false;
        
        await TriggerEventAndWait(x => x.BeforeConnect(options));

        _playwright = await Playwright.CreateAsync();
        _browser = (await _playwright[_browserTypeEnum.GetBrowserName()].ConnectAsync(wsEndpoint, options));
        _browserContext = _browser.Contexts.First();

        await TriggerEventAndWait(x => x.AfterConnect(_browser));
        await TriggerEventAndWait(x => x.OnBrowser(_browser, null));

        _browser.Disconnected += async (_, targetBrowser) =>
            await TriggerEventAndWait(x => x.OnDisconnected(targetBrowser, null));

        await TriggerEventAndWait(x => x.AfterLaunch(_browser));

        OrderPlugins();
        CheckPluginRequirements(new BrowserStartContext
        {
            StartType = StartType.Connect
        });

        return this;
    }
    
    public async Task<PlaywrightExtra> ConnectOverCDPAsync(string endpointURL, BrowserTypeConnectOverCDPOptions? options = null)
    {
        if (_browserTypeEnum != BrowserTypeEnum.Chromium)
            throw new InvalidOperationException("ConnectOverCDPAsync is only supported for chromium browser");
            
        _persistContext = true;
        _persistentLaunch = false;

        await TriggerEventAndWait(x => x.BeforeConnect(
            options != null
                ? new BrowserTypeConnectOptions
                {
                    Headers = options.Headers,
                    Timeout = options.Timeout,
                    SlowMo = options.SlowMo
                }
                : null));

        _playwright = await Playwright.CreateAsync();
        _browser = (await _playwright[_browserTypeEnum.GetBrowserName()].ConnectOverCDPAsync(endpointURL, options));
        _browserContext = _browser.Contexts.First();

        await TriggerEventAndWait(x => x.AfterConnect(_browser));
        await TriggerEventAndWait(x => x.OnBrowser(_browser, null));

        _browser.Disconnected += async (_, targetBrowser) =>
            await TriggerEventAndWait(x => x.OnDisconnected(targetBrowser, null));

        await TriggerEventAndWait(x => x.AfterLaunch(_browser));

        OrderPlugins();
        CheckPluginRequirements(new BrowserStartContext
        {
            StartType = StartType.Connect
        });

        return this;
    }

    public Task<IPage> NewPageAsync(BrowserNewPageOptions? options = default)
        => NewPageAsync(null, options);
    
    /// <summary>
    /// <para>
    /// Creates a new page in a new browser context. Closing this page will close the context
    /// as well.
    /// </para>
    /// </summary>
    /// /// <param name="userDataDir">User data dir to store cache</param>
    /// <param name="options">Call options</param>
    public async Task<IPage> NewPageAsync(string? userDataDir = null, BrowserNewPageOptions? options = default)
    {
        options ??= new BrowserNewPageOptions();

        IPage page = null!;
        
        if (_persistContext)
        {
            page = await _browserContext!.NewPageAsync();
            page.Close += async (_, page1) => await TriggerEventAndWait(x => x.OnPageClose(page1));
        }
        else
        {
            if (_persistentLaunch)
            {
                var persistentContextOptions = options.ToPersistentContextOptions(_persistentContextOptions);
            
                await TriggerEventAndWait(x => x.BeforeContext(null, persistentContextOptions));
                var browserContext = await _playwright![_browserTypeEnum.GetBrowserName()]
                    .LaunchPersistentContextAsync(_userDataDir ?? userDataDir ?? "", persistentContextOptions);
                await TriggerEventAndWait(x => x.OnContextCreated(browserContext, null, persistentContextOptions));

                await TriggerEventAndWait(x => x.OnBrowser(null, browserContext));

                browserContext.Close += async (_, targetBrowserContext) =>
                    await TriggerEventAndWait(x => x.OnDisconnected(null, targetBrowserContext));

                _browser = browserContext.Browser;
                
                await TriggerEventAndWait(x => x.AfterLaunch(_browser));
            
                page = await browserContext.NewPageAsync();
                page.Close += async (_, page1) =>
                {
                    await TriggerEventAndWait(x => x.OnPageClose(page1));
                    await page1.Context.CloseAsync();
                };
            }
            else
            {
                var contextOptions = options.ToContextOptions(_contextOptions);

                await TriggerEventAndWait(x => x.BeforeContext(contextOptions, null));
                var browserContext = await _browser.NewContextAsync(contextOptions);
                await TriggerEventAndWait(x => x.OnContextCreated(browserContext, contextOptions, null));

                browserContext.Close += async (_, context) =>
                    await TriggerEventAndWait(x => x.OnDisconnected(null, context));

                page = await browserContext.NewPageAsync();
                page.Close += async (_, page1) => await TriggerEventAndWait(x => x.OnPageClose(page1));
            }
        }

        await TriggerEventAndWait(x => x.OnPageCreated(page));

        page.Request += async (_, request) => await TriggerEventAndWait(x => x.OnRequest(page, request));

        return page;
    }

    public ValueTask DisposeAsync()
    {
        if(_browser is not null)
            return _browser.DisposeAsync();
        return _browserContext.DisposeAsync();
    }

    public async Task CloseAsync()
    {
        if(_browser is not null)
            await _browser.CloseAsync();
        if(_browserContext is not null)
            await _browserContext.CloseAsync();
    }

    public Task<ICDPSession> NewBrowserCDPSessionAsync()
    {
        return _browser.NewBrowserCDPSessionAsync();
    }

    public Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
    {
        return _browser.NewContextAsync();
    }

    public IBrowserType BrowserType => _browser.BrowserType;
    public IReadOnlyList<IBrowserContext> Contexts => _browser?.Contexts ?? new []{ _browserContext! };
    public bool IsConnected => _browser.IsConnected;
    public string Version => _browser.Version;

    event EventHandler<IBrowser>? IBrowser.Disconnected
    {
        add => _browser.Disconnected += value;
        remove => _browser.Disconnected -= value;
    }

    public async void Dispose()
    {
        await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    private void ResolveDependencies(PlaywrightExtraPlugin extraPlugin)
    {
        foreach (var puppeteerExtraPlugin in extraPlugin.Dependencies)
        {
            Use(puppeteerExtraPlugin);
            puppeteerExtraPlugin.Dependencies.ToList().ForEach(ResolveDependencies);
        }
    }

    private void OrderPlugins()
    {
        _plugins = _plugins.OrderBy(e => e.Requirements.Contains(PluginRequirement.RunLast)).ToList();
    }

    private void CheckPluginRequirements(BrowserStartContext context)
    {
        foreach (var puppeteerExtraPlugin in _plugins)
        foreach (var requirement in puppeteerExtraPlugin.Requirements)
            switch (context.StartType)
            {
                case StartType.Launch when requirement == PluginRequirement.Headful && context.IsHeadless:
                    throw new NotSupportedException(
                        $"Plugin - {puppeteerExtraPlugin.Name} is not supported in headless mode");
                case StartType.Connect when requirement == PluginRequirement.Launch:
                    throw new NotSupportedException(
                        $"Plugin - {puppeteerExtraPlugin.Name} doesn't support connect");
            }
    }

    private async Task TriggerEventAndWait(Func<PlaywrightExtraPlugin, Task> action)
    {
        try
        {
            await Task.WhenAll(_plugins.Select(action));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}