using VisionFramework;

namespace VisionPluginContracts;

public enum ConfigurationRunRejection { None, Busy, NotReady, Loading, AwaitingResult, ContinuousActive, Closed }
public sealed record ConfigurationRunAttempt(ConfigurationRunRejection Rejection, NodeRunRecord? Result = null);
public sealed record ConfigurationExecutionHostStatus(bool SolutionBusy, bool ProcedureBusy, bool Ready, bool Loading, bool RunEnabled);

/// <summary>Source-guided BottomCommand scheduling. Stop drains the polling loop;
/// closing additionally drains an already dispatched operation without aborting it.
/// Task ownership and observer isolation are independent managed host safeguards.</summary>
public sealed class ToolConfigurationExecution : IAsyncDisposable
{
    readonly object gate = new();
    readonly Func<Task<NodeRunRecord>> execute;
    readonly Func<bool> busy, ready, loading, procedureBusy;
    readonly Func<TimeSpan, CancellationToken, Task> delay;
    Flight? flight;
    Polling? polling;
    Task? stopTask, disposeTask;
    bool closed, running, admitting;
    NodeRunRecord? lastResult;
    Exception? lastError;
    public bool IsRunning { get { lock (gate) return running; } }
    public bool IsContinuous { get { lock (gate) return !closed && polling is { StopRequested: false }; } }
    public NodeRunRecord? LastResult { get { lock (gate) return lastResult; } }
    public Exception? LastError { get { lock (gate) return lastError; } }
    public event Action? Changed;

    public ToolConfigurationExecution(Func<Task<NodeRunRecord>> execute, Func<bool> busy, Func<bool> ready,
        Func<bool> loading, Func<bool>? procedureBusy = null, Func<TimeSpan, CancellationToken, Task>? delay = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.busy = busy ?? throw new ArgumentNullException(nameof(busy));
        this.ready = ready ?? throw new ArgumentNullException(nameof(ready));
        this.loading = loading ?? throw new ArgumentNullException(nameof(loading));
        this.procedureBusy = procedureBusy ?? busy;
        this.delay = delay ?? ((duration, token) => Task.Delay(duration, token));
    }

    ConfigurationRunRejection Admit(bool continuous, out Flight? operation, out Polling? cycle)
    {
        operation = null; cycle = null;
        lock (gate)
        {
            if (closed) return ConfigurationRunRejection.Closed;
            if (continuous && polling is not null) return ConfigurationRunRejection.ContinuousActive;
            if (admitting) return ConfigurationRunRejection.AwaitingResult;
            admitting = true;
        }
        try
        {
            // Host services may reenter or close the owner. Never invoke them while holding gate.
            var rejection = busy() ? ConfigurationRunRejection.Busy : !ready() ? ConfigurationRunRejection.NotReady
                : loading() ? ConfigurationRunRejection.Loading : ConfigurationRunRejection.None;
            lock (gate)
            {
                if (closed) return ConfigurationRunRejection.Closed;
                if (rejection != ConfigurationRunRejection.None) return rejection;
                if (flight is not null) return ConfigurationRunRejection.AwaitingResult;
                if (polling is not null || stopTask is { IsCompleted: false }) return ConfigurationRunRejection.ContinuousActive;
                stopTask = null;
                if (continuous) { lastError = null; polling = cycle = new(); }
                else operation = ReserveFlight();
                return ConfigurationRunRejection.None;
            }
        }
        finally { lock (gate) admitting = false; }
    }

    Flight ReserveFlight()
    {
        running = true; lastError = null; lastResult = null;
        return flight = new();
    }

    public async Task<ConfigurationRunAttempt> RunOnceAsync()
    {
        var rejection = Admit(false, out var operation, out _);
        if (rejection != ConfigurationRunRejection.None) return new ConfigurationRunAttempt(rejection);
        _ = Dispatch(operation!, null);
        return await operation!.Done.Task;
    }

    public ConfigurationRunRejection StartContinuous()
    {
        var rejection = Admit(true, out _, out var cycle);
        if (rejection != ConfigurationRunRejection.None) return rejection;
        _ = Poll(cycle!);
        Notify();
        return ConfigurationRunRejection.None;
    }

    async Task Poll(Polling cycle)
    {
        var token = cycle.Cancellation.Token;
        try
        {
            while (!token.IsCancellationRequested)
            {
                bool awaiting;
                lock (gate) { if (closed || cycle.StopRequested) break; awaiting = flight is not null; }
                // Original callback/procedure busy adds 300 ms before the outer 300 ms wait.
                if (awaiting || procedureBusy()) await delay(TimeSpan.FromMilliseconds(300), token);
                else if (loading()) { Observe(RequestStop(cycle)); break; }
                else
                {
                    Flight? operation = null;
                    lock (gate)
                        if (!closed && !cycle.StopRequested && ReferenceEquals(polling, cycle) && flight is null)
                            operation = ReserveFlight();
                    if (operation is not null) _ = Dispatch(operation, cycle);
                }
                await delay(TimeSpan.FromMilliseconds(300), token);
                // A synchronous delay implementation must not trap StartContinuous in a tight loop.
                await Task.Yield();
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception error) { lock (gate) lastError = error; Observe(RequestStop(cycle)); }
        finally { cycle.Done.TrySetResult(); }
    }

    async Task Dispatch(Flight operation, Polling? cycle)
    {
        var attempt = new ConfigurationRunAttempt(ConfigurationRunRejection.None);
        Notify();
        try
        {
            ConfigurationRunRejection rejection;
            lock (gate) rejection = closed ? ConfigurationRunRejection.Closed
                : cycle is { StopRequested: true } ? ConfigurationRunRejection.ContinuousActive : ConfigurationRunRejection.None;
            if (rejection != ConfigurationRunRejection.None) attempt = new(rejection);
            else
            {
                var result = await execute();
                lock (gate) lastResult = result;
                attempt = new(ConfigurationRunRejection.None, result);
            }
        }
        catch (Exception error)
        {
            lock (gate) lastError = error;
            if (cycle is not null) Observe(RequestStop(cycle));
        }
        finally
        {
            lock (gate) running = false;
            Notify();
            lock (gate)
            {
                if (ReferenceEquals(flight, operation)) flight = null;
                operation.Done.TrySetResult(attempt);
            }
        }
    }

    void Notify()
    {
        if (Changed is not { } changed) return;
        foreach (Action observer in changed.GetInvocationList())
            try { observer(); } catch { /* Each observer is isolated from scheduling and other observers. */ }
    }

    public Task StopAsync()
    {
        Polling? cycle;
        lock (gate) { cycle = polling; if (cycle is null) return stopTask ?? Task.CompletedTask; }
        return RequestStop(cycle);
    }

    Task RequestStop(Polling cycle)
    {
        Task result;
        lock (gate)
        {
            if (cycle.Stopped is not null) return cycle.Stopped.Task;
            cycle.StopRequested = true;
            cycle.Stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);
            result = cycle.Stopped.Task;
            if (ReferenceEquals(polling, cycle)) stopTask = result;
        }
        _ = DrainStop(cycle);
        return result;
    }

    async Task DrainStop(Polling cycle)
    {
        Exception? failure = null;
        try
        {
            Task callbacks;
            try { callbacks = cycle.Cancellation.CancelAsync(); }
            catch (Exception error) { callbacks = Task.FromException(error); }
            await Task.WhenAll(cycle.Done.Task, callbacks);
        }
        catch (Exception error) { failure = error; }
        finally
        {
            try { cycle.Cancellation.Dispose(); }
            catch (Exception error) { failure = failure is null ? error : new AggregateException(failure, error); }
            lock (gate)
            {
                if (ReferenceEquals(polling, cycle)) polling = null;
                if (failure is not null) lastError = failure;
            }
            Notify();
            if (failure is null) cycle.Stopped!.TrySetResult();
            else cycle.Stopped!.TrySetException(failure);
        }
    }

    public ValueTask DisposeAsync()
    {
        TaskCompletionSource done;
        Task? active;
        lock (gate)
        {
            if (disposeTask is not null) return new(disposeTask);
            closed = true;
            done = new(TaskCreationOptions.RunContinuationsAsynchronously);
            disposeTask = done.Task;
            active = flight?.Done.Task;
        }
        _ = DrainClose(done, active);
        return new(done.Task);
    }

    async Task DrainClose(TaskCompletionSource done, Task? active)
    {
        try { await Task.WhenAll(StopAsync(), active ?? Task.CompletedTask); done.TrySetResult(); }
        catch (Exception error) { done.TrySetException(error); }
    }

    static void Observe(Task task) => _ = task.ContinueWith(t => { _ = t.Exception; },
        CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    sealed class Flight { public readonly TaskCompletionSource<ConfigurationRunAttempt> Done = new(TaskCreationOptions.RunContinuationsAsynchronously); }
    sealed class Polling
    {
        public readonly CancellationTokenSource Cancellation = new();
        public readonly TaskCompletionSource Done = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource? Stopped;
        public bool StopRequested;
    }
}
