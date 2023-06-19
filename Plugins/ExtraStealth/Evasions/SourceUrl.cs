using System.Reflection;
using Microsoft.Playwright;

namespace PlaywrightExtraSharp.Plugins.ExtraStealth.Evasions;

public class SourceUrl : PlaywrightExtraPlugin
{
    public override string Name => "SourceUrl";

    public override Func<IPage, Task> OnPageCreated => page =>
    {
        var mainWordProperty =
            page.MainFrame.GetType().GetProperty("MainWorld", BindingFlags.NonPublic
                                                              | BindingFlags.Public | BindingFlags.Instance);
        var mainWordGetters = mainWordProperty?.GetGetMethod(true);

        page.Load += async (_, _) =>
        {
            var mainWord = mainWordGetters?.Invoke(page.MainFrame, null);
            var contextField = mainWord?.GetType()
                .GetField("_contextResolveTaskWrapper", BindingFlags.NonPublic | BindingFlags.Instance);
            if (contextField is not null)
            {
                var context = (TaskCompletionSource<ExecutionContext>?)contextField.GetValue(mainWord);
                if (context?.Task == null)
                    throw new InvalidOperationException();
                var execution = await context.Task;
                var suffixField = execution.GetType()
                    .GetField("_evaluationScriptSuffix", BindingFlags.NonPublic | BindingFlags.Instance);
                suffixField?.SetValue(execution, "//# sourceURL=''");
            }
        };
        return Task.CompletedTask;
    };
}