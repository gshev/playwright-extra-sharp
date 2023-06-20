using Microsoft.Playwright;
using PlaywrightExtraSharp.Models;
using PlaywrightExtraSharp.Utils;

namespace PlaywrightExtraSharp.Plugins;

public abstract class PlaywrightExtraPlugin
{
    public abstract string Name { get; }
    public virtual PluginRequirement[] Requirements { get; set; } = Array.Empty<PluginRequirement>();
    public virtual PlaywrightExtraPlugin[] Dependencies { get; set; } = Array.Empty<PlaywrightExtraPlugin>();
    public virtual List<string> ScriptDependencies { get; set; } = new() { "Utils.js" };

    public virtual Func<Task> OnPluginRegistered { get; set; } = () => Task.CompletedTask;
    public virtual Func<BrowserTypeLaunchOptions?, Task> BeforeLaunch { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IBrowser, Task> AfterLaunch { get; set; } = _ => Task.CompletedTask;
    public virtual Func<BrowserTypeConnectOptions?, Task> BeforeConnect { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IBrowser, Task> AfterConnect { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IBrowser, Task> OnBrowser { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IPage, Task> OnPageCreated { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IPage, Task> OnPageClose { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IPage, IRequest, Task> OnRequest { get; set; } = (_, _) => Task.CompletedTask;
    public virtual Func<IBrowser?, Task> OnDisconnected { get; set; } = _ => Task.CompletedTask;

    public virtual Func<BrowserNewContextOptions, IBrowser?, Task> BeforeContext { get; set; } =
        (_, _) => Task.CompletedTask;

    public virtual Func<IBrowserContext, BrowserNewContextOptions, Task> OnContextCreated { get; set; } =
        (_, _) => Task.CompletedTask;

    protected async Task EvaluateScript(IPage page, string scriptName, params object[] args)
    {
        var scriptList = ScriptDependencies.Select(
            x => ResourceReader.ReadFile($"{typeof(PlaywrightExtraPlugin).Namespace}.Scripts.{x}")).ToList();

        scriptList.Add(ResourceReader.ReadFile($"{typeof(PlaywrightExtraPlugin).Namespace}.Scripts.{scriptName}"));

        if (!page.IsClosed)
            await page.EvaluateAsync(string.Join("\n", scriptList), args);
    }
}