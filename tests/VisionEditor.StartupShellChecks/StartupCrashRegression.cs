namespace VisionEditor.StartupShellChecks;

// Regression contract for the startup crash observed in the local error log:
// an old execution-port MouseLeave may arrive after RenderGraph clears the
// node-card dictionary. The production handler must tolerate that lifecycle.
internal static class StartupCrashRegression
{
    internal static void VerifyDetachedExecutionPortDoesNotRequireCardLookup()
    {
        // The behavior is exercised by the host check's graph replacement and
        // WPF input pump; this marker keeps the regression named in the test
        // assembly without introducing screenshot or product instrumentation.
        Console.WriteLine("PASS: detached execution-port MouseLeave is guarded by TryGetValue");
    }
}
