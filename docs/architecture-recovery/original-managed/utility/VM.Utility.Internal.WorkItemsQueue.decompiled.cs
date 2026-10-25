using System;
using System.Threading;

namespace VM.Utility.Internal;

public class WorkItemsQueue : IDisposable
{
	public sealed class WaiterEntry : IDisposable
	{
		private AutoResetEvent _waitHandle = EventWaitHandleFactory.CreateAutoResetEvent();

		private bool _isTimedout = false;

		private bool _isSignaled = false;

		private WorkItem _workItem = null;

		private bool _isDisposed = false;

		internal WaiterEntry _nextWaiterEntry = null;

		internal WaiterEntry _prevWaiterEntry = null;

		public WaitHandle WaitHandle => _waitHandle;

		public WorkItem WorkItem => _workItem;

		public WaiterEntry()
		{
			Reset();
		}

		public bool Signal(WorkItem workItem)
		{
			lock (this)
			{
				if (!_isTimedout)
				{
					_workItem = workItem;
					_isSignaled = true;
					_waitHandle.Set();
					return true;
				}
			}
			return false;
		}

		public bool Timeout()
		{
			lock (this)
			{
				if (!_isSignaled)
				{
					_isTimedout = true;
					return true;
				}
			}
			return false;
		}

		public void Reset()
		{
			_workItem = null;
			_isTimedout = false;
			_isSignaled = false;
			_waitHandle.Reset();
		}

		public void Close()
		{
			if (_waitHandle != null)
			{
				_waitHandle.Close();
				_waitHandle = null;
			}
		}

		public void Dispose()
		{
			lock (this)
			{
				if (!_isDisposed)
				{
					Close();
				}
				_isDisposed = true;
			}
		}
	}

	private readonly WaiterEntry _headWaiterEntry = new WaiterEntry();

	private int _waitersCount = 0;

	private readonly PriorityQueue _workItems = new PriorityQueue();

	private bool _isWorkItemsQueueActive = true;

	[ThreadStatic]
	private static WaiterEntry _waiterEntry;

	private bool _isDisposed = false;

	private static WaiterEntry CurrentWaiterEntry
	{
		get
		{
			return _waiterEntry;
		}
		set
		{
			_waiterEntry = value;
		}
	}

	public int Count => _workItems.Count;

	public int WaitersCount => _waitersCount;

	public bool EnqueueWorkItem(WorkItem workItem)
	{
		if (workItem == null)
		{
			throw new ArgumentNullException("workItem", "workItem cannot be null");
		}
		bool flag = true;
		lock (this)
		{
			ValidateNotDisposed();
			if (!_isWorkItemsQueueActive)
			{
				return false;
			}
			while (_waitersCount > 0)
			{
				WaiterEntry waiterEntry = PopWaiter();
				if (waiterEntry.Signal(workItem))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				_workItems.Enqueue(workItem);
			}
		}
		return true;
	}

	public WorkItem DequeueWorkItem(int millisecondsTimeout, WaitHandle cancelEvent)
	{
		WorkItem workItem = null;
		WaiterEntry threadWaiterEntry;
		lock (this)
		{
			ValidateNotDisposed();
			if (_workItems.Count > 0)
			{
				workItem = _workItems.Dequeue() as WorkItem;
				return workItem;
			}
			threadWaiterEntry = GetThreadWaiterEntry();
			PushWaiter(threadWaiterEntry);
		}
		WaitHandle[] waitHandles = new WaitHandle[2] { threadWaiterEntry.WaitHandle, cancelEvent };
		int num = STPEventWaitHandle.WaitAny(waitHandles, millisecondsTimeout, exitContext: true);
		lock (this)
		{
			bool flag = num == 0;
			if (!flag)
			{
				bool flag2 = threadWaiterEntry.Timeout();
				if (flag2)
				{
					RemoveWaiter(threadWaiterEntry, popDecrement: false);
				}
				flag = !flag2;
			}
			if (flag)
			{
				workItem = threadWaiterEntry.WorkItem;
				if (workItem == null)
				{
					workItem = _workItems.Dequeue() as WorkItem;
				}
			}
		}
		return workItem;
	}

	private void Cleanup()
	{
		lock (this)
		{
			if (!_isWorkItemsQueueActive)
			{
				return;
			}
			_isWorkItemsQueueActive = false;
			foreach (WorkItem workItem in _workItems)
			{
				workItem.DisposeOfState();
			}
			_workItems.Clear();
			while (_waitersCount > 0)
			{
				WaiterEntry waiterEntry = PopWaiter();
				waiterEntry.Timeout();
			}
		}
	}

	public object[] GetStates()
	{
		lock (this)
		{
			object[] array = new object[_workItems.Count];
			int num = 0;
			foreach (WorkItem workItem in _workItems)
			{
				array[num] = workItem.GetWorkItemResult().State;
				num++;
			}
			return array;
		}
	}

	private static WaiterEntry GetThreadWaiterEntry()
	{
		if (CurrentWaiterEntry == null)
		{
			CurrentWaiterEntry = new WaiterEntry();
		}
		CurrentWaiterEntry.Reset();
		return CurrentWaiterEntry;
	}

	public void PushWaiter(WaiterEntry newWaiterEntry)
	{
		RemoveWaiter(newWaiterEntry, popDecrement: false);
		if (_headWaiterEntry._nextWaiterEntry == null)
		{
			_headWaiterEntry._nextWaiterEntry = newWaiterEntry;
			newWaiterEntry._prevWaiterEntry = _headWaiterEntry;
		}
		else
		{
			WaiterEntry nextWaiterEntry = _headWaiterEntry._nextWaiterEntry;
			_headWaiterEntry._nextWaiterEntry = newWaiterEntry;
			newWaiterEntry._nextWaiterEntry = nextWaiterEntry;
			newWaiterEntry._prevWaiterEntry = _headWaiterEntry;
			nextWaiterEntry._prevWaiterEntry = newWaiterEntry;
		}
		_waitersCount++;
	}

	private WaiterEntry PopWaiter()
	{
		WaiterEntry nextWaiterEntry = _headWaiterEntry._nextWaiterEntry;
		WaiterEntry nextWaiterEntry2 = nextWaiterEntry._nextWaiterEntry;
		RemoveWaiter(nextWaiterEntry, popDecrement: true);
		_headWaiterEntry._nextWaiterEntry = nextWaiterEntry2;
		if (nextWaiterEntry2 != null)
		{
			nextWaiterEntry2._prevWaiterEntry = _headWaiterEntry;
		}
		return nextWaiterEntry;
	}

	private void RemoveWaiter(WaiterEntry waiterEntry, bool popDecrement)
	{
		WaiterEntry prevWaiterEntry = waiterEntry._prevWaiterEntry;
		WaiterEntry nextWaiterEntry = waiterEntry._nextWaiterEntry;
		bool flag = popDecrement;
		waiterEntry._prevWaiterEntry = null;
		waiterEntry._nextWaiterEntry = null;
		if (prevWaiterEntry != null)
		{
			prevWaiterEntry._nextWaiterEntry = nextWaiterEntry;
			flag = true;
		}
		if (nextWaiterEntry != null)
		{
			nextWaiterEntry._prevWaiterEntry = prevWaiterEntry;
			flag = true;
		}
		if (flag)
		{
			_waitersCount--;
		}
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			Cleanup();
		}
		_isDisposed = true;
	}

	private void ValidateNotDisposed()
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException(GetType().ToString(), "The SmartThreadPool has been shutdown");
		}
	}
}
