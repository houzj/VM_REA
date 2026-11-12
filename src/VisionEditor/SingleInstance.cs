using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace VisionEditor;

/// <summary>
/// 单实例守卫。原厂使用 <c>VM.Utility.SingleInstance.SingleInstanceManager</c>
/// （成员体未反编译，见契约 U01），本项目实现同语义的守卫 + 参数转发。
/// 证据等级：独立实现。
/// </summary>
internal sealed class StartupInstance : IDisposable
{
    const string MutexName = "VisionEditor.C01.SingleInstance";
    readonly Mutex mutex;

    StartupInstance(Mutex mutex) => this.mutex = mutex;

    internal static StartupInstance? TryAcquire()
    {
        var mutex = new Mutex(true, MutexName, out var created);
        if (created) return new StartupInstance(mutex);
        // 未取得所有权时不得释放或拥有该互斥体。
        mutex.Dispose();
        return null;
    }

    public void Dispose()
    {
        try { mutex.ReleaseMutex(); }
        catch (ApplicationException) { }
        finally { mutex.Dispose(); }
    }
}

/// <summary>
/// 二次实例命令行转发。原厂通过 <c>SingleInstanceManagerSetup.ArgumentsHandler</c>
/// 调用 <c>App.ProcessCommandLineArgs</c>（契约 S00 / S11）；本项目用命名管道实现同语义。
/// 证据等级：独立实现。
/// </summary>
internal static class StartupForwarding
{
    const string PipeName = "VisionEditor.C01.Forward";

    internal static bool Forward(string[] args)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out, PipeOptions.None);
            client.Connect(1500);
            var payload = string.Join('\n', args.Where(value => !string.IsNullOrWhiteSpace(value)));
            var bytes = Encoding.UTF8.GetBytes(payload);
            client.Write(bytes, 0, bytes.Length);
            client.Flush();
            return true;
        }
        catch (Exception error) when (error is IOException or TimeoutException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    internal static void Listen(Action<string[]> handler)
    {
        var thread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    using var server = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.None);
                    server.WaitForConnection();
                    using var reader = new StreamReader(server, Encoding.UTF8);
                    var payload = reader.ReadToEnd();
                    var args = payload.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (args.Length > 0) handler(args);
                }
                catch (Exception error) when (error is IOException or UnauthorizedAccessException or ObjectDisposedException)
                {
                    return;
                }
            }
        })
        { IsBackground = true, Name = "VisionEditor.Forwarding" };
        thread.Start();
    }
}
