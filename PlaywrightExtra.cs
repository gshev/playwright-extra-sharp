using Microsoft.Playwright;
using PlaywrightExtraSharp.Helpers;
using PlaywrightExtraSharp.Models;
using PlaywrightExtraSharp.Plugins;

namespace PlaywrightExtraSharp;

public class PlaywrightExtra : IBrowser, IDisposable
{
    private readonly BrowserTypeEnum _browserTypeEnum;
    private IBrowser _browser = null!;
    private IPlaywright? _playwright;
    private List<PlaywrightExtraPlugin> _plugins = new();

    public PlaywrightExtra(BrowserTypeEnum browserTypeEnum)
    {
        _browserTypeEnum = browserTypeEnum;

        Use(new DisposeContext());
    }

    public async Task<IPage> NewPageAsync(BrowserNewPageOptions? options = default)
    {
        options ??= new BrowserNewPageOptions();

        var contextOptions = new BrowserNewContextOptions
        {
            AcceptDownloads = options.AcceptDownloads,
            IgnoreHTTPSErrors = options.IgnoreHTTPSErrors,
            BypassCSP = options.BypassCSP,
            ViewportSize = options.ViewportSize,
            ScreenSize = options.ScreenSize,
            UserAgent = options.UserAgent,
            DeviceScaleFactor = options.DeviceScaleFactor,
            IsMobile = options.IsMobile,
            HasTouch = options.HasTouch,
            JavaScriptEnabled = options.JavaScriptEnabled,
            TimezoneId = options.TimezoneId,
            Geolocation = options.Geolocation,
            Locale = options.Locale,
            Permissions = options.Permissions,
            ExtraHTTPHeaders = options.ExtraHTTPHeaders,
            Offline = options.Offline,
            HttpCredentials = options.HttpCredentials,
            ColorScheme = options.ColorScheme,
            ReducedMotion = options.ReducedMotion,
            ForcedColors = options.ForcedColors,
            RecordHarPath = options.RecordHarPath,
            RecordHarContent = options.RecordHarContent,
            RecordHarMode = options.RecordHarMode,
            RecordHarOmitContent = options.RecordHarOmitContent,
            RecordHarUrlFilter = options.RecordHarUrlFilter,
            RecordHarUrlFilterString = options.RecordHarUrlFilterString,
            RecordHarUrlFilterRegex = options.RecordHarUrlFilterRegex,
            RecordVideoDir = options.RecordVideoDir,
            RecordVideoSize = options.RecordVideoSize,
            Proxy = options.Proxy,
            StorageState = options.StorageState,
            StorageStatePath = options.StorageStatePath,
            ServiceWorkers = options.ServiceWorkers,
            BaseURL = options.BaseURL,
            StrictSelectors = options.StrictSelectors
        };

        await TriggerEventAndWait(x => x.BeforeContext(contextOptions, _browser));
        var browserContext = await _browser.NewContextAsync(contextOptions).ConfigureAwait(false);
        await TriggerEventAndWait(x => x.OnContextCreated(browserContext, contextOptions));

        browserContext.Close += async (_, context) => await TriggerEventAndWait(x => x.OnDisconnected(context.Browser));

        var page = await browserContext.NewPageAsync();
        await TriggerEventAndWait(x => x.OnPageCreated(page));

        page.Close += async (_, page1) => await TriggerEventAndWait(x => x.OnPageClose(page1));

        return page;
    }

    public ValueTask DisposeAsync()
    {
        return _browser.DisposeAsync();
    }

    public Task CloseAsync()
    {
        return _browser.CloseAsync();
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
    public IReadOnlyList<IBrowserContext> Contexts => _browser.Contexts;
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

    public async Task<PlaywrightExtra> LaunchAsync(BrowserTypeLaunchOptions? options = null)
    {
        await TriggerEventAndWait(x => x.BeforeLaunch(options));

        _playwright = await Playwright.CreateAsync().ConfigureAwait(false);
        _browser = await _playwright[_browserTypeEnum.GetBrowserName()].LaunchAsync(options).ConfigureAwait(false);

        await TriggerEventAndWait(x => x.OnBrowser(_browser));

        _browser.Disconnected += async (_, targetBrowser) =>
            await TriggerEventAndWait(x => x.OnDisconnected(targetBrowser));

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
        await TriggerEventAndWait(x => x.BeforeConnect(options));

        _playwright = await Playwright.CreateAsync().ConfigureAwait(false);
        _browser = await _playwright[_browserTypeEnum.GetBrowserName()].ConnectAsync(wsEndpoint, options)
            .ConfigureAwait(false);

        await TriggerEventAndWait(x => x.AfterConnect(_browser));
        await TriggerEventAndWait(x => x.OnBrowser(_browser));

        _browser.Disconnected += async (_, targetBrowser) =>
            await TriggerEventAndWait(x => x.OnDisconnected(targetBrowser));

        await TriggerEventAndWait(x => x.AfterLaunch(_browser));

        OrderPlugins();
        CheckPluginRequirements(new BrowserStartContext
        {
            StartType = StartType.Connect
        });

        return this;
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