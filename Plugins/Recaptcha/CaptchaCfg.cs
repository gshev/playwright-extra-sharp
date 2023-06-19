namespace PlaywrightExtraSharp.Plugins.Recaptcha;

public class CaptchaCfg
{
    public CaptchaCfg(string callback)
    {
        Callback = callback;
    }

    public string Callback { get; }
}