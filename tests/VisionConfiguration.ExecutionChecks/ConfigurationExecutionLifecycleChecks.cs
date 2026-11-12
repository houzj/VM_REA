using VisionFramework;
using VisionPluginContracts;

internal static class ConfigurationExecutionLifecycleChecks
{
    static NodeRunRecord Record(int n = 1) => new("node", NodeRunStatus.Success, new Dictionary<string, PortValue> { ["n"] = PortValue.Integer(n) }, 1);
    public static async Task<int> RunAsync()
    {
        int count = 0;
        void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); count++; }
        var release = new TaskCompletionSource<NodeRunRecord>(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        var service = new ToolConfigurationExecution(() => { calls++; return release.Task; }, () => false, () => true, () => false);
        Task? close = null; bool ownedBeforeCallback = false;
        service.Changed += () => { if (service.IsRunning && close is null) { close = service.DisposeAsync().AsTask(); ownedBeforeCallback = !close.IsCompleted; } };
        var run = service.RunOnceAsync();
        release.TrySetResult(Record());
        var result = await run.WaitAsync(TimeSpan.FromSeconds(5));
        if (close is not null) await close.WaitAsync(TimeSpan.FromSeconds(5));
        Check(ownedBeforeCallback, "Closing from the running notification must await the registered operation");
        Check(calls == 0 && result.Rejection == ConfigurationRunRejection.Closed,
            "A configuration closed by its running observer must not dispatch an algorithm afterward");
        await CheckReentrantStop(Check);
        await CheckAdmission(Check);
        await CheckPendingStopAndClose(Check);
        await CheckCancellationFailure(Check);
        await CheckNotifications(Check);
        await CheckPollingFailures(Check);
        return count;
    }

    static async Task CheckReentrantStop(Action<bool, string> check)
    {
        int calls = 0; Task? stopped = null; bool pendingInsideObserver = false;
        await using var service = new ToolConfigurationExecution(() => { calls++; return Task.FromResult(Record()); }, () => false, () => true, () => false);
        service.Changed += () => { if (service.IsRunning && stopped is null) { stopped = service.StopAsync(); pendingInsideObserver = !stopped.IsCompleted; } };
        check(service.StartContinuous() == ConfigurationRunRejection.None, "Continuous admission succeeds before a running observer requests stop");
        await stopped!.WaitAsync(TimeSpan.FromSeconds(5));
        check(pendingInsideObserver && calls == 0 && !service.IsContinuous && !service.IsRunning, "Reentrant stop owns the published loop and prevents dispatch after stop");
    }

    static async Task CheckAdmission(Action<bool, string> check)
    {
        ToolConfigurationExecution? service = null; Task? close = null; int calls = 0;
        service = new(() => { calls++; return Task.FromResult(Record()); }, () => false,
            () => { close = service!.DisposeAsync().AsTask(); return true; }, () => false);
        check((await service.RunOnceAsync()).Rejection == ConfigurationRunRejection.Closed && calls == 0, "Closing inside a host readiness callback invalidates admission before dispatch");
        await close!;
        Task<ConfigurationRunAttempt>? nested = null;
        await using var reentrant = service = new(() => { calls++; return Task.FromResult(Record()); }, () => false,
            () => { nested ??= service!.RunOnceAsync(); return true; }, () => false);
        var outer = await reentrant.RunOnceAsync();
        check(outer.Result is not null && (await nested!).Rejection == ConfigurationRunRejection.AwaitingResult && calls == 1, "Reentrant readiness cannot recursively admit a second operation");
        bool fail = true;
        await using var faulty = new ToolConfigurationExecution(() => Task.FromResult(Record()), () => false,
            () => fail ? throw new IOException("readiness failure") : true, () => false);
        bool observed = false;
        try { await faulty.RunOnceAsync(); } catch (IOException) { observed = true; }
        fail = false;
        check(observed && (await faulty.RunOnceAsync()).Result is not null, "Failed readiness evaluation releases its admission reservation for retry");
    }

    static async Task CheckPendingStopAndClose(Action<bool, string> check)
    {
        var pending = new TaskCompletionSource<NodeRunRecord>(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        var service = new ToolConfigurationExecution(() => { calls++; return pending.Task; }, () => false, () => true, () => false);
        service.StartContinuous();
        var stop = service.StopAsync(); await stop.WaitAsync(TimeSpan.FromSeconds(5));
        check(service.IsRunning && !service.IsContinuous && !pending.Task.IsCompleted, "Stop drains scheduling without aborting the dispatched operation");
        check(service.StartContinuous() == ConfigurationRunRejection.AwaitingResult && (await service.RunOnceAsync()).Rejection == ConfigurationRunRejection.AwaitingResult, "Stop cannot open a second run while the prior callback is outstanding");
        var close = service.DisposeAsync().AsTask(); var closeAgain = service.DisposeAsync().AsTask();
        check(ReferenceEquals(close, closeAgain) && !close.IsCompleted, "Concurrent close calls share the outstanding-operation drain");
        pending.SetResult(Record(7)); await close.WaitAsync(TimeSpan.FromSeconds(5));
        check(calls == 1 && !service.IsRunning && service.LastResult!.Outputs["n"].As<long>() == 7, "Close retains the completed owned result and releases running state once");
        check(service.StartContinuous() == ConfigurationRunRejection.Closed && (await service.RunOnceAsync()).Rejection == ConfigurationRunRejection.Closed, "Closed execution rejects both entry points");
    }

    static async Task CheckCancellationFailure(Action<bool, string> check)
    {
        var callbackEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseCallback = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var algorithm = new TaskCompletionSource<NodeRunRecord>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task Delay(TimeSpan _, CancellationToken token)
        {
            var wake = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            token.Register(() => {
                wake.TrySetCanceled(token); callbackEntered.TrySetResult();
                releaseCallback.Task.GetAwaiter().GetResult();
                throw new InvalidOperationException("owned polling cancellation failure");
            });
            return wake.Task;
        }
        var service = new ToolConfigurationExecution(() => algorithm.Task, () => false, () => true, () => false, delay: Delay);
        service.StartContinuous();
        var stop = service.StopAsync(); var stopAgain = service.StopAsync(); var close = service.DisposeAsync().AsTask();
        try
        {
            await callbackEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            check(ReferenceEquals(stop, stopAgain) && !stop.IsCompleted && !close.IsCompleted, "Concurrent stop and close retain cancellation callback ownership");
            check(!service.IsContinuous && service.IsRunning, "Stopping is visible while the polling callback is still draining");
        }
        finally { releaseCallback.TrySetResult(); }
        bool stopFailed = false;
        try { await stop.WaitAsync(TimeSpan.FromSeconds(5)); } catch (Exception error) when (error.ToString().Contains("owned polling cancellation failure")) { stopFailed = true; }
        check(stopFailed && !close.IsCompleted && !algorithm.Task.IsCompleted, "Cancellation callback failure is reported without letting close bypass the active algorithm");
        algorithm.TrySetResult(Record(9));
        bool closeFailed = false;
        try { await close.WaitAsync(TimeSpan.FromSeconds(5)); } catch (Exception error) when (error.ToString().Contains("owned polling cancellation failure")) { closeFailed = true; }
        check(closeFailed && !service.IsRunning && service.LastError!.ToString().Contains("owned polling cancellation failure"), "Close reports cancellation failure only after all owned work drains");
        check(ReferenceEquals(close, service.DisposeAsync().AsTask()), "Repeated close preserves the completed close outcome");
    }

    static async Task CheckNotifications(Action<bool, string> check)
    {
        int calls = 0, observed = 0; bool attempted = false; Task<ConfigurationRunAttempt>? nested = null;
        ToolConfigurationExecution? service = null;
        await using var owner = service = new(() => Task.FromResult(Record(++calls)), () => false, () => true, () => false);
        owner.Changed += () => throw new InvalidOperationException("view observer failure");
        owner.Changed += () => { observed++; if (!owner.IsRunning && !attempted) { attempted = true; nested = owner.RunOnceAsync(); } };
        var first = await owner.RunOnceAsync();
        check(observed == 2 && owner.LastError is null, "A throwing view observer does not skip later observers or corrupt execution");
        check((await nested!).Rejection == ConfigurationRunRejection.AwaitingResult && calls == 1, "Idle notification cannot overlap the operation whose completion is being published");
        var second = await owner.RunOnceAsync();
        check(first.Result!.Outputs["n"].As<long>() == 1 && second.Result!.Outputs["n"].As<long>() == 2, "Each once caller receives its own result rather than a later shared LastResult");
    }

    static async Task CheckPollingFailures(Action<bool, string> check)
    {
        await using var faultyDelay = new ToolConfigurationExecution(() => Task.FromResult(Record()), () => false, () => true, () => false,
            delay: (_, _) => Task.FromException(new IOException("owned delay failure")));
        faultyDelay.StartContinuous(); await faultyDelay.StopAsync().WaitAsync(TimeSpan.FromSeconds(5));
        check(!faultyDelay.IsContinuous && faultyDelay.LastError?.Message == "owned delay failure", "Polling failure releases its own cycle and remains diagnosable");
        check((await faultyDelay.RunOnceAsync()).Result is not null && faultyDelay.LastError is null, "A later explicit once can recover after a polling failure");
        int calls = 0, delays = 0; ToolConfigurationExecution? service = null; Task? stopped = null;
        await using var owner = service = new(() => Task.FromResult(Record(++calls)), () => false, () => true, () => false,
            delay: (_, _) => { if (++delays == 3) stopped = service!.StopAsync(); return Task.CompletedTask; });
        check(owner.StartContinuous() == ConfigurationRunRejection.None, "Synchronous injected waits cannot trap continuous admission in a busy loop");
        var deadline = Task.Delay(TimeSpan.FromSeconds(5));
        while (stopped is null) { if (deadline.IsCompleted) throw new TimeoutException("Polling did not yield"); await Task.Yield(); }
        await stopped.WaitAsync(TimeSpan.FromSeconds(5));
        check(calls == 3 && !owner.IsContinuous && !owner.IsRunning, "Synchronous operations remain serial and a stop request ends their cycle");
        stopped = null;
        check(owner.StartContinuous() == ConfigurationRunRejection.None, "Completed polling stop permits a new continuous cycle");
        await owner.StopAsync().WaitAsync(TimeSpan.FromSeconds(5));
    }
}
