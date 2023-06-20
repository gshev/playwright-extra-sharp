using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class WebGl : PlaywrightExtraPlugin
{
    private readonly StealthWebGLOptions _options;

    public WebGl(StealthWebGLOptions? options = null)
    {
        _options = options ?? new StealthWebGLOptions("Intel Inc.", "Intel Iris OpenGL Engine");
    }

    public override string Name => "stealth-webGl";

    public override Func<IPage, Task> OnPageCreated =>
        page => EvaluateScript(page, "WebGL.js", _options.Vendor, _options.Renderer);
}

public class StealthWebGLOptions : IPlaywrightExtraPluginOptions
{
    public StealthWebGLOptions(string vendor, string renderer)
    {
        Vendor = vendor;
        Renderer = renderer;
    }

    public string Vendor { get; }
    public string Renderer { get; }
}