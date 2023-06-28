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
    public virtual Func<BrowserTypeLaunchOptions?, BrowserTypeLaunchPersistentContextOptions?, Task> BeforeLaunch { get; set; } = (_,_) => Task.CompletedTask;
    public virtual Func<IBrowser?, IBrowserContext?, Task> AfterLaunch { get; set; } = (_,_) => Task.CompletedTask;
    public virtual Func<BrowserTypeConnectOptions?, Task> BeforeConnect { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IBrowser, Task> AfterConnect { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IBrowser?, IBrowserContext?, Task> OnBrowser { get; set; } = (_,_) => Task.CompletedTask;
    public virtual Func<IPage, Task> OnPageCreated { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IPage, Task> OnPageClose { get; set; } = _ => Task.CompletedTask;
    public virtual Func<IPage, IRequest, Task> OnRequest { get; set; } = (_, _) => Task.CompletedTask;
    public virtual Func<IBrowser?, IBrowserContext?, Task> OnDisconnected { get; set; } = (_,_) => Task.CompletedTask;

    public virtual Func<BrowserNewContextOptions?, BrowserTypeLaunchPersistentContextOptions?, Task> BeforeContext { get; set; } =
        (_,_) => Task.CompletedTask;

    public virtual Func<IBrowserContext, BrowserNewContextOptions?, BrowserTypeLaunchPersistentContextOptions?, Task> OnContextCreated { get; set; } =
        (_, _, _) => Task.CompletedTask;

    protected async Task EvaluateScript(IPage page, string scriptName, params object[] args)
    {
        var scriptList = ScriptDependencies.Select(
            x => ResourceReader.ReadFile($"{typeof(PlaywrightExtraPlugin).Namespace}.Scripts.{x}")).ToList();

        scriptList.Add(ResourceReader.ReadFile($"{typeof(PlaywrightExtraPlugin).Namespace}.Scripts.{scriptName}"));

        if (!page.IsClosed)
            await page.EvaluateAsync(string.Join("\n", scriptList), args);
    }
}