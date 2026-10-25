using System;

namespace VM.Utility;

public class ThreadService : IDisposable
{
	private static SmartThreadPool _defaultThreadPool;

	private bool _dispose = false;

	private static readonly ThreadService instance;

	static ThreadService()
	{
		instance = new ThreadService();
		_defaultThreadPool = new SmartThreadPool(new STPStartInfo
		{
			MaxWorkerThreads = 30,
			MinWorkerThreads = 1
		});
	}

	~ThreadService()
	{
		Dispose(dispose: false);
	}

	private void Dispose(bool dispose)
	{
		if (!_dispose)
		{
			_dispose = true;
			_defaultThreadPool.Shutdown();
		}
	}

	public void Dispose()
	{
		Dispose(dispose: true);
		GC.SuppressFinalize(this);
	}

	public static IWorkItemsGroup GetDefaultThreadPool()
	{
		return _defaultThreadPool;
	}

	public static SmartThreadPool CreateNewPool(int maxThreadCount = 25, int minThreadCount = 0, bool isSuspend = false)
	{
		STPStartInfo sTPStartInfo = new STPStartInfo();
		sTPStartInfo.MaxWorkerThreads = maxThreadCount;
		sTPStartInfo.MinWorkerThreads = minThreadCount;
		sTPStartInfo.StartSuspended = isSuspend;
		return new SmartThreadPool(sTPStartInfo);
	}
}
