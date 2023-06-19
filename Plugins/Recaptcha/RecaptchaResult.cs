namespace PlaywrightExtraSharp.Plugins.Recaptcha;

public class RecaptchaResult
{
    public RecaptchaResult(bool isSuccess = true, CaptchaException? exception = null)
    {
        IsSuccess = isSuccess;
        Exception = exception;
    }

    public bool IsSuccess { get; }
    public CaptchaException? Exception { get; }
}