using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using VM.Utility.Internal;

namespace VM.Utility;

public class SmartThreadPool : WorkItemsGroupBase, IDisposable
{
	private class ChoiceIndex
	{
		public int _index = -1;
	}

	internal class ThreadEntry
	{
		private readonly DateTime _creationTime;

		private DateTime _lastAliveTime;

		private readonly SmartThreadPool _associatedSmartThreadPool;

		public WorkItem CurrentWorkItem { get; set; }

		public SmartThreadPool AssociatedSmartThreadPool => _associatedSmartThreadPool;

		public ThreadEntry(SmartThreadPool stp)
		{
			_associatedSmartThreadPool = stp;
			_creationTime = DateTime.UtcNow;
			_lastAliveTime = DateTime.MinValue;
		}

		public void IAmAlive()
		{
			_lastAliveTime = DateTime.UtcNow;
		}
	}

	public const int DefaultMinWorkerThreads = 0;

	public const int DefaultMaxWorkerThreads = 25;

	public const int DefaultIdleTimeout = 60000;

	public const bool DefaultUseCallerCallContext = false;

	public const bool DefaultUseCallerHttpContext = false;

	public const bool DefaultDisposeOfStateObjects = false;

	public const CallToPostExecute DefaultCallToPostExecute = CallToPostExecute.Always;

	public static readonly PostExecuteWorkItemCallback DefaultPostExecuteWorkItemCallback;

	public const WorkItemPriority DefaultWorkItemPriority = WorkItemPriority.Normal;

	public const bool DefaultStartSuspended = false;

	public static readonly string DefaultPerformanceCounterInstanceName;

	public const ThreadPriority DefaultThreadPriority = ThreadPriority.Normal;

	public const string DefaultThreadPoolName = "SmartThreadPool";

	public static readonly int? DefaultMaxStackSize = null;

	public static readonly int? DefaultMaxQueueLength = null;

	public const bool DefaultFillStateWithArgs = false;

	public const bool DefaultAreThreadsBackground = true;

	public const ApartmentState DefaultApartmentState = ApartmentState.Unknown;

	private readonly SynchronizedDictionary<Thread, ThreadEntry> _workerThreads = new SynchronizedDictionary<Thread, ThreadEntry>();

	private readonly WorkItemsQueue _workItemsQueue = new WorkItemsQueue();

	private int _workItemsProcessed;

	private int _inUseWorkerThreads;

	private STPStartInfo _stpStartInfo;

	private volatile int _currentWorkItemsCount;

	private ManualResetEvent _isIdleWaitHandle = EventWaitHandleFactory.CreateManualResetEvent(initialState: true);

	private ManualResetEvent _shuttingDownEvent = EventWaitHandleFactory.CreateManualResetEvent(initialState: false);

	private bool _isSuspended;

	private bool _shutdown;

	private int _threadCounter;

	private bool _isDisposed;

	private readonly SynchronizedDictionary<IWorkItemsGroup, IWorkItemsGroup> _workItemsGroups = new SynchronizedDictionary<IWorkItemsGroup, IWorkItemsGroup>();

	private CanceledWorkItemsGroup _canceledSmartThreadPool = new CanceledWorkItemsGroup();

	private ISTPInstancePerformanceCounters _windowsPCs = NullSTPInstancePerformanceCounters.Instance;

	private ISTPInstancePerformanceCounters _localPCs = NullSTPInstancePerformanceCounters.Instance;

	[ThreadStatic]
	private static ThreadEntry _threadEntry;

	internal static ThreadEntry CurrentThreadEntry
	{
		get
		{
			return _threadEntry;
		}
		set
		{
			_threadEntry = value;
		}
	}

	public int MinThreads
	{
		get
		{
			ValidateNotDisposed();
			return _stpStartInfo.MinWorkerThreads;
		}
		set
		{
			if (_stpStartInfo.MaxWorkerThreads < value)
			{
				_stpStartInfo.MaxWorkerThreads = value;
			}
			_stpStartInfo.MinWorkerThreads = value;
			StartOptimalNumberOfThreads();
		}
	}

	public int MaxThreads
	{
		get
		{
			ValidateNotDisposed();
			return _stpStartInfo.MaxWorkerThreads;
		}
		set
		{
			if (_stpStartInfo.MinWorkerThreads > value)
			{
				_stpStartInfo.MinWorkerThreads = value;
			}
			_stpStartInfo.MaxWorkerThreads = value;
			StartOptimalNumberOfThreads();
		}
	}

	public int? MaxQueueLength
	{
		get
		{
			ValidateNotDisposed();
			return _stpStartInfo.MaxQueueLength;
		}
		set
		{
			_stpStartInfo.MaxQueueLength = value;
		}
	}

	public int ActiveThreads
	{
		get
		{
			ValidateNotDisposed();
			return _workerThreads.Count;
		}
	}

	public int CurrentWorkItemsCount
	{
		get
		{
			ValidateNotDisposed();
			return _currentWorkItemsCount;
		}
	}

	public static bool IsWorkItemCanceled => CurrentThreadEntry.CurrentWorkItem.IsCanceled;

	public STPStartInfo STPStartInfo => _stpStartInfo.AsReadOnly();

	public bool IsShuttingdown => _shutdown;

	public ISTPPerformanceCountersReader PerformanceCountersReader => (ISTPPerformanceCountersReader)_localPCs;

	public override int Concurrency
	{
		get
		{
			return MaxThreads;
		}
		set
		{
			MaxThreads = value;
		}
	}

	public override int InUseThreads
	{
		get
		{
			ValidateNotDisposed();
			return _inUseWorkerThreads;
		}
	}

	public override int WaitingCallbacks
	{
		get
		{
			ValidateNotDisposed();
			return _workItemsQueue.Count;
		}
	}

	public override WIGStartInfo WIGStartInfo => _stpStartInfo.AsReadOnly();

	private event ThreadInitializationHandler _onThreadInitialization;

	private event ThreadTerminationHandler _onThreadTermination;

	public event ThreadInitializationHandler OnThreadInitialization
	{
		add
		{
			_onThreadInitialization += value;
		}
		remove
		{
			_onThreadInitialization -= value;
		}
	}

	public event ThreadTerminationHandler OnThreadTermination
	{
		add
		{
			_onThreadTermination += value;
		}
		remove
		{
			_onThreadTermination -= value;
		}
	}

	public override event WorkItemsGroupIdleHandler OnIdle
	{
		add
		{
			throw new NotImplementedException("This event is not implemented in the SmartThreadPool class. Please create a WorkItemsGroup in order to use this feature.");
		}
		remove
		{
			throw new NotImplementedException("This event is not implemented in the SmartThreadPool class. Please create a WorkItemsGroup in order to use this feature.");
		}
	}

	public SmartThreadPool()
	{
		_stpStartInfo = new STPStartInfo();
		Initialize();
	}

	public SmartThreadPool(int idleTimeout)
	{
		_stpStartInfo = new STPStartInfo
		{
			IdleTimeout = idleTimeout
		};
		Initialize();
	}

	public SmartThreadPool(bool startSuspended)
	{
		_stpStartInfo = new STPStartInfo
		{
			StartSuspended = startSuspended
		};
		Initialize();
	}

	public SmartThreadPool(int idleTimeout, int maxWorkerThreads)
	{
		_stpStartInfo = new STPStartInfo
		{
			IdleTimeout = idleTimeout,
			MaxWorkerThreads = maxWorkerThreads
		};
		Initialize();
	}

	public SmartThreadPool(int idleTimeout, int maxWorkerThreads, int minWorkerThreads)
	{
		_stpStartInfo = new STPStartInfo
		{
			IdleTimeout = idleTimeout,
			MaxWorkerThreads = maxWorkerThreads,
			MinWorkerThreads = minWorkerThreads
		};
		Initialize();
	}

	public SmartThreadPool(STPStartInfo stpStartInfo)
	{
		_stpStartInfo = new STPStartInfo(stpStartInfo);
		Initialize();
	}

	private void Initialize()
	{
		Name = _stpStartInfo.ThreadPoolName;
		ValidateSTPStartInfo();
		_isSuspended = _stpStartInfo.StartSuspended;
		if (_stpStartInfo.EnableLocalPerformanceCounters)
		{
			_localPCs = new LocalSTPInstancePerformanceCounters();
		}
		if (!_isSuspended)
		{
			StartOptimalNumberOfThreads();
		}
	}

	private void StartOptimalNumberOfThreads()
	{
		int val = Math.Max(_workItemsQueue.Count, _stpStartInfo.MinWorkerThreads);
		val = Math.Min(val, _stpStartInfo.MaxWorkerThreads);
		val -= _workerThreads.Count;
		if (val > 0)
		{
			StartThreads(val);
		}
	}

	private void ValidateSTPStartInfo()
	{
		if (_stpStartInfo.MinWorkerThreads < 0)
		{
			throw new ArgumentOutOfRangeException("MinWorkerThreads", "MinWorkerThreads cannot be negative");
		}
		if (_stpStartInfo.MaxWorkerThreads <= 0)
		{
			throw new ArgumentOutOfRangeException("MaxWorkerThreads", "MaxWorkerThreads must be greater than zero");
		}
		if (_stpStartInfo.MinWorkerThreads > _stpStartInfo.MaxWorkerThreads)
		{
			throw new ArgumentOutOfRangeException("MinWorkerThreads, maxWorkerThreads", "MaxWorkerThreads must be greater or equal to MinWorkerThreads");
		}
		if (_stpStartInfo.MaxQueueLength < 0)
		{
			throw new ArgumentOutOfRangeException("MaxQueueLength", "MaxQueueLength must be >= 0 or null (for unbounded)");
		}
	}

	private static void ValidateCallback(Delegate callback)
	{
		if (callback.GetInvocationList().Length > 1)
		{
			throw new NotSupportedException("SmartThreadPool doesn't support delegates chains");
		}
	}

	private WorkItem Dequeue()
	{
		return _workItemsQueue.DequeueWorkItem(_stpStartInfo.IdleTimeout, _shuttingDownEvent);
	}

	internal override void Enqueue(WorkItem workItem)
	{
		IncrementWorkItemsCount();
		workItem.CanceledSmartThreadPool = _canceledSmartThreadPool;
		_workItemsQueue.EnqueueWorkItem(workItem);
		workItem.WorkItemIsQueued();
		if (_currentWorkItemsCount > _workerThreads.Count)
		{
			StartThreads(1);
		}
	}

	private void IncrementWorkItemsCount()
	{
		_windowsPCs.SampleWorkItems(_workItemsQueue.Count, _workItemsProcessed);
		_localPCs.SampleWorkItems(_workItemsQueue.Count, _workItemsProcessed);
		int num = Interlocked.Increment(ref _currentWorkItemsCount);
		if (num == 1)
		{
			IsIdle = false;
			_isIdleWaitHandle.Reset();
		}
	}

	private void DecrementWorkItemsCount()
	{
		if (Interlocked.Decrement(ref _currentWorkItemsCount) == 0)
		{
			IsIdle = true;
			_isIdleWaitHandle.Set();
		}
		Interlocked.Increment(ref _workItemsProcessed);
		if (!_shutdown)
		{
			_windowsPCs.SampleWorkItems(_workItemsQueue.Count, _workItemsProcessed);
			_localPCs.SampleWorkItems(_workItemsQueue.Count, _workItemsProcessed);
		}
	}

	internal void RegisterWorkItemsGroup(IWorkItemsGroup workItemsGroup)
	{
		_workItemsGroups[workItemsGroup] = workItemsGroup;
	}

	internal void UnregisterWorkItemsGroup(IWorkItemsGroup workItemsGroup)
	{
		if (_workItemsGroups.Contains(workItemsGroup))
		{
			_workItemsGroups.Remove(workItemsGroup);
		}
	}

	private void InformCompleted()
	{
		if (_workerThreads.Contains(Thread.CurrentThread))
		{
			_workerThreads.Remove(Thread.CurrentThread);
			_windowsPCs.SampleThreads(_workerThreads.Count, _inUseWorkerThreads);
			_localPCs.SampleThreads(_workerThreads.Count, _inUseWorkerThreads);
		}
	}

	private void StartThreads(int threadsCount)
	{
		if (_isSuspended)
		{
			return;
		}
		lock (_workerThreads.SyncRoot)
		{
			if (_shutdown)
			{
				return;
			}
			for (int i = 0; i < threadsCount; i++)
			{
				if (_workerThreads.Count >= _stpStartInfo.MaxWorkerThreads)
				{
					break;
				}
				Thread thread = (_stpStartInfo.MaxStackSize.HasValue ? new Thread(ProcessQueuedItems, _stpStartInfo.MaxStackSize.Value) : new Thread(ProcessQueuedItems));
				thread.Name = "STP " + Name + " Thread #" + _threadCounter;
				thread.IsBackground = _stpStartInfo.AreThreadsBackground;
				if (_stpStartInfo.ApartmentState != ApartmentState.Unknown)
				{
					thread.SetApartmentState(_stpStartInfo.ApartmentState);
				}
				thread.Priority = _stpStartInfo.ThreadPriority;
				thread.Start();
				_threadCounter++;
				_workerThreads[thread] = new ThreadEntry(this);
				_windowsPCs.SampleThreads(_workerThreads.Count, _inUseWorkerThreads);
				_localPCs.SampleThreads(_workerThreads.Count, _inUseWorkerThreads);
			}
		}
	}

	private void ProcessQueuedItems()
	{
		CurrentThreadEntry = _workerThreads[Thread.CurrentThread];
		FireOnThreadInitialization();
		try
		{
			bool flag = false;
			while (!_shutdown)
			{
				CurrentThreadEntry.IAmAlive();
				if (_workerThreads.Count > _stpStartInfo.MaxWorkerThreads)
				{
					lock (_workerThreads.SyncRoot)
					{
						if (_workerThreads.Count > _stpStartInfo.MaxWorkerThreads)
						{
							InformCompleted();
							break;
						}
					}
				}
				WorkItem workItem = Dequeue();
				CurrentThreadEntry.IAmAlive();
				if (workItem == null && _workerThreads.Count > _stpStartInfo.MinWorkerThreads)
				{
					lock (_workerThreads.SyncRoot)
					{
						if (_workerThreads.Count > _stpStartInfo.MinWorkerThreads)
						{
							InformCompleted();
							break;
						}
					}
				}
				if (workItem == null)
				{
					continue;
				}
				try
				{
					flag = false;
					CurrentThreadEntry.CurrentWorkItem = workItem;
					if (workItem.StartingWorkItem())
					{
						int num = Interlocked.Increment(ref _inUseWorkerThreads);
						_windowsPCs.SampleThreads(_workerThreads.Count, num);
						_localPCs.SampleThreads(_workerThreads.Count, num);
						flag = true;
						workItem.FireWorkItemStarted();
						ExecuteWorkItem(workItem);
					}
				}
				catch (Exception ex)
				{
					ex.GetHashCode();
				}
				finally
				{
					workItem.DisposeOfState();
					CurrentThreadEntry.CurrentWorkItem = null;
					if (flag)
					{
						int num2 = Interlocked.Decrement(ref _inUseWorkerThreads);
						_windowsPCs.SampleThreads(_workerThreads.Count, num2);
						_localPCs.SampleThreads(_workerThreads.Count, num2);
					}
					workItem.FireWorkItemCompleted();
					DecrementWorkItemsCount();
				}
			}
		}
		catch (ThreadAbortException ex2)
		{
			ex2.GetHashCode();
			Thread.ResetAbort();
		}
		catch (Exception)
		{
		}
		finally
		{
			InformCompleted();
			FireOnThreadTermination();
		}
	}

	private void ExecuteWorkItem(WorkItem workItem)
	{
		_windowsPCs.SampleWorkItemsWaitTime(workItem.WaitingTime);
		_localPCs.SampleWorkItemsWaitTime(workItem.WaitingTime);
		try
		{
			workItem.Execute();
		}
		finally
		{
			_windowsPCs.SampleWorkItemsProcessTime(workItem.ProcessTime);
			_localPCs.SampleWorkItemsProcessTime(workItem.ProcessTime);
		}
	}

	private void ValidateWaitForIdle()
	{
		if (CurrentThreadEntry != null && CurrentThreadEntry.AssociatedSmartThreadPool == this)
		{
			throw new NotSupportedException("WaitForIdle cannot be called from a thread on its SmartThreadPool, it causes a deadlock");
		}
	}

	internal static void ValidateWorkItemsGroupWaitForIdle(IWorkItemsGroup workItemsGroup)
	{
		if (CurrentThreadEntry != null)
		{
			WorkItem currentWorkItem = CurrentThreadEntry.CurrentWorkItem;
			ValidateWorkItemsGroupWaitForIdleImpl(workItemsGroup, currentWorkItem);
			if (workItemsGroup != null && currentWorkItem != null && CurrentThreadEntry.CurrentWorkItem.WasQueuedBy(workItemsGroup))
			{
				throw new NotSupportedException("WaitForIdle cannot be called from a thread on its SmartThreadPool, it causes a deadlock");
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ValidateWorkItemsGroupWaitForIdleImpl(IWorkItemsGroup workItemsGroup, WorkItem workItem)
	{
		if (workItemsGroup != null && workItem != null && workItem.WasQueuedBy(workItemsGroup))
		{
			throw new NotSupportedException("WaitForIdle cannot be called from a thread on its SmartThreadPool, it causes a deadlock");
		}
	}

	public void Shutdown()
	{
		ShutdownImpl(forceAbort: false, 0);
	}

	public void Shutdown(TimeSpan timeout)
	{
		ShutdownImpl(forceAbort: false, (int)timeout.TotalMilliseconds);
	}

	public void Shutdown(int millisecondsTimeout)
	{
		ShutdownImpl(forceAbort: false, millisecondsTimeout);
	}

	public void Shutdown(bool forceAbort)
	{
		ShutdownImpl(forceAbort, 0);
	}

	public void Shutdown(bool forceAbort, TimeSpan timeout)
	{
		ShutdownImpl(forceAbort, (int)timeout.TotalMilliseconds);
	}

	public void Shutdown(bool forceAbort, int millisecondsTimeout)
	{
		ShutdownImpl(forceAbort, millisecondsTimeout);
	}

	private void ShutdownImpl(bool forceAbort, int millisecondsTimeout)
	{
		ValidateNotDisposed();
		ISTPInstancePerformanceCounters windowsPCs = _windowsPCs;
		if (NullSTPInstancePerformanceCounters.Instance != _windowsPCs)
		{
			_windowsPCs = NullSTPInstancePerformanceCounters.Instance;
			windowsPCs.Dispose();
		}
		Thread[] array;
		lock (_workerThreads.SyncRoot)
		{
			_workItemsQueue.Dispose();
			_shutdown = true;
			_shuttingDownEvent.Set();
			array = new Thread[_workerThreads.Count];
			_workerThreads.Keys.CopyTo(array, 0);
		}
		int num = millisecondsTimeout;
		System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
		bool flag = -1 == millisecondsTimeout;
		bool flag2 = false;
		Thread[] array2 = array;
		foreach (Thread thread in array2)
		{
			if (!flag && num < 0)
			{
				flag2 = true;
				break;
			}
			if (!thread.Join(num))
			{
				flag2 = true;
				break;
			}
			if (!flag)
			{
				num = millisecondsTimeout - (int)stopwatch.ElapsedMilliseconds;
			}
		}
		if (!(flag2 & forceAbort))
		{
			return;
		}
		Thread[] array3 = array;
		foreach (Thread thread2 in array3)
		{
			if (thread2 != null && thread2.IsAlive)
			{
				try
				{
					thread2.Abort();
				}
				catch (SecurityException ex)
				{
					ex.GetHashCode();
				}
				catch (ThreadStateException ex2)
				{
					ex2.GetHashCode();
				}
			}
		}
	}

	public static bool WaitAll(IWaitableResult[] waitableResults)
	{
		return WaitAll(waitableResults, -1, exitContext: true);
	}

	public static bool WaitAll(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext)
	{
		return WaitAll(waitableResults, (int)timeout.TotalMilliseconds, exitContext);
	}

	public static bool WaitAll(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
	{
		return WaitAll(waitableResults, (int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
	}

	public static bool WaitAll(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext)
	{
		return WorkItem.WaitAll(waitableResults, millisecondsTimeout, exitContext, null);
	}

	public static bool WaitAll(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
	{
		return WorkItem.WaitAll(waitableResults, millisecondsTimeout, exitContext, cancelWaitHandle);
	}

	public static int WaitAny(IWaitableResult[] waitableResults)
	{
		return WaitAny(waitableResults, -1, exitContext: true);
	}

	public static int WaitAny(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext)
	{
		return WaitAny(waitableResults, (int)timeout.TotalMilliseconds, exitContext);
	}

	public static int WaitAny(IWaitableResult[] waitableResults, TimeSpan timeout, bool exitContext, WaitHandle cancelWaitHandle)
	{
		return WaitAny(waitableResults, (int)timeout.TotalMilliseconds, exitContext, cancelWaitHandle);
	}

	public static int WaitAny(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext)
	{
		return WorkItem.WaitAny(waitableResults, millisecondsTimeout, exitContext, null);
	}

	public static int WaitAny(IWaitableResult[] waitableResults, int millisecondsTimeout, bool exitContext, WaitHandle cancelWaitHandle)
	{
		return WorkItem.WaitAny(waitableResults, millisecondsTimeout, exitContext, cancelWaitHandle);
	}

	public IWorkItemsGroup CreateWorkItemsGroup(int concurrency)
	{
		return new WorkItemsGroup(this, concurrency, _stpStartInfo);
	}

	public IWorkItemsGroup CreateWorkItemsGroup(int concurrency, WIGStartInfo wigStartInfo)
	{
		return new WorkItemsGroup(this, concurrency, wigStartInfo);
	}

	private void FireOnThreadInitialization()
	{
		if (_onThreadInitialization == null)
		{
			return;
		}
		Delegate[] invocationList = _onThreadInitialization.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			ThreadInitializationHandler threadInitializationHandler = (ThreadInitializationHandler)invocationList[i];
			try
			{
				threadInitializationHandler();
			}
			catch (Exception ex)
			{
				ex.GetHashCode();
				throw;
			}
		}
	}

	private void FireOnThreadTermination()
	{
		if (_onThreadTermination == null)
		{
			return;
		}
		Delegate[] invocationList = _onThreadTermination.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			ThreadTerminationHandler threadTerminationHandler = (ThreadTerminationHandler)invocationList[i];
			try
			{
				threadTerminationHandler();
			}
			catch (Exception ex)
			{
				ex.GetHashCode();
				throw;
			}
		}
	}

	internal void CancelAbortWorkItemsGroup(WorkItemsGroup wig)
	{
		foreach (ThreadEntry value in _workerThreads.Values)
		{
			WorkItem currentWorkItem = value.CurrentWorkItem;
			if (currentWorkItem != null && currentWorkItem.WasQueuedBy(wig) && !currentWorkItem.IsCanceled)
			{
				value.CurrentWorkItem.GetWorkItemResult().Cancel(abortExecution: true);
			}
		}
	}

	private void ValidateQueueIsWithinLimits()
	{
		int? maxQueueLength = _stpStartInfo.MaxQueueLength;
		if (!maxQueueLength.HasValue || !(_currentWorkItemsCount >= maxQueueLength + MaxThreads))
		{
			return;
		}
		throw new QueueRejectedException("Queue is at its maximum (" + maxQueueLength + ")");
	}

	public static void AbortOnWorkItemCancel()
	{
		if (IsWorkItemCanceled)
		{
			Thread.CurrentThread.Abort();
		}
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			if (!_shutdown)
			{
				Shutdown();
			}
			if (_shuttingDownEvent != null)
			{
				_shuttingDownEvent.Close();
				_shuttingDownEvent = null;
			}
			_workerThreads.Clear();
			if (_isIdleWaitHandle != null)
			{
				_isIdleWaitHandle.Close();
				_isIdleWaitHandle = null;
			}
			_isDisposed = true;
		}
	}

	private void ValidateNotDisposed()
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException(GetType().ToString(), "The SmartThreadPool has been shutdown");
		}
	}

	public override object[] GetStates()
	{
		return _workItemsQueue.GetStates();
	}

	public override void Start()
	{
		if (!_isSuspended)
		{
			return;
		}
		_isSuspended = false;
		ICollection values = _workItemsGroups.Values;
		foreach (WorkItemsGroup item in values)
		{
			item.OnSTPIsStarting();
		}
		StartOptimalNumberOfThreads();
	}

	public override void Cancel(bool abortExecution)
	{
		_canceledSmartThreadPool.IsCanceled = true;
		_canceledSmartThreadPool = new CanceledWorkItemsGroup();
		ICollection values = _workItemsGroups.Values;
		foreach (WorkItemsGroup item in values)
		{
			item.Cancel(abortExecution);
		}
		if (!abortExecution)
		{
			return;
		}
		foreach (ThreadEntry value in _workerThreads.Values)
		{
			WorkItem currentWorkItem = value.CurrentWorkItem;
			if (currentWorkItem != null && value.AssociatedSmartThreadPool == this && !currentWorkItem.IsCanceled)
			{
				value.CurrentWorkItem.GetWorkItemResult().Cancel(abortExecution: true);
			}
		}
	}

	public override bool WaitForIdle(int millisecondsTimeout)
	{
		ValidateWaitForIdle();
		return STPEventWaitHandle.WaitOne(_isIdleWaitHandle, millisecondsTimeout, exitContext: false);
	}

	internal override void PreQueueWorkItem()
	{
		ValidateNotDisposed();
		ValidateQueueIsWithinLimits();
	}

	public void Join(IEnumerable<Action> actions)
	{
		WIGStartInfo wigStartInfo = new WIGStartInfo
		{
			StartSuspended = true
		};
		IWorkItemsGroup workItemsGroup = CreateWorkItemsGroup(int.MaxValue, wigStartInfo);
		foreach (Action action in actions)
		{
			workItemsGroup.QueueWorkItem(action);
		}
		workItemsGroup.Start();
		workItemsGroup.WaitForIdle();
	}

	public void Join(params Action[] actions)
	{
		Join((IEnumerable<Action>)actions);
	}

	public int Choice(IEnumerable<Action> actions)
	{
		WIGStartInfo wigStartInfo = new WIGStartInfo
		{
			StartSuspended = true
		};
		IWorkItemsGroup workItemsGroup = CreateWorkItemsGroup(int.MaxValue, wigStartInfo);
		ManualResetEvent anActionCompleted = new ManualResetEvent(initialState: false);
		ChoiceIndex choiceIndex = new ChoiceIndex();
		int num = 0;
		foreach (Action action in actions)
		{
			Action act = action;
			int value = num;
			workItemsGroup.QueueWorkItem(() =>
			{
				act();
				Interlocked.CompareExchange(ref choiceIndex._index, value, -1);
				anActionCompleted.Set();
			});
			num++;
		}
		workItemsGroup.Start();
		anActionCompleted.WaitOne();
		return choiceIndex._index;
	}

	public int Choice(params Action[] actions)
	{
		return Choice((IEnumerable<Action>)actions);
	}

	public void Pipe<T>(T pipeState, IEnumerable<Action<T>> actions)
	{
		WIGStartInfo wigStartInfo = new WIGStartInfo
		{
			StartSuspended = true
		};
		IWorkItemsGroup workItemsGroup = CreateWorkItemsGroup(1, wigStartInfo);
		foreach (Action<T> action in actions)
		{
			Action<T> act = action;
			workItemsGroup.QueueWorkItem(() =>
			{
				act(pipeState);
			});
		}
		workItemsGroup.Start();
		workItemsGroup.WaitForIdle();
	}

	public void Pipe<T>(T pipeState, params Action<T>[] actions)
	{
		Pipe(pipeState, (IEnumerable<Action<T>>)actions);
	}
}
