using System;

namespace VM.Utility.Internal;

public abstract class WorkItemsGroupBase : IWorkItemsGroup
{
	private string _name = "WorkItemsGroupBase";

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public abstract int Concurrency { get; set; }

	public abstract int WaitingCallbacks { get; }

	public abstract int InUseThreads { get; }

	public abstract WIGStartInfo WIGStartInfo { get; }

	public bool IsIdle { get; protected set; }

	public abstract event WorkItemsGroupIdleHandler OnIdle;

	public WorkItemsGroupBase()
	{
		IsIdle = true;
	}

	public abstract object[] GetStates();

	public abstract void Start();

	public abstract void Cancel(bool abortExecution);

	public abstract bool WaitForIdle(int millisecondsTimeout);

	internal abstract void Enqueue(WorkItem workItem);

	internal virtual void PreQueueWorkItem()
	{
	}

	public virtual void Cancel()
	{
		Cancel(abortExecution: false);
	}

	public void WaitForIdle()
	{
		WaitForIdle(-1);
	}

	public bool WaitForIdle(TimeSpan timeout)
	{
		return WaitForIdle((int)timeout.TotalMilliseconds);
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback)
	{
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, WorkItemPriority workItemPriority)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, workItemPriority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, workItemInfo, callback);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state)
	{
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, WorkItemPriority workItemPriority)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, workItemPriority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback, object state)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, workItemInfo, callback, state);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, WorkItemPriority workItemPriority)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, workItemPriority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, callToPostExecute);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute, WorkItemPriority workItemPriority)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, callToPostExecute, workItemPriority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem(Action action, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, delegate
		{
			action();
			return (object)null;
		}, priority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem<T>(Action<T> action, T arg, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) =>
		{
			action(arg);
			return (object)null;
		}, (!WIGStartInfo.FillStateWithArgs) ? null : new object[1] { arg }, priority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) =>
		{
			action(arg1, arg2);
			return (object)null;
		}, (!WIGStartInfo.FillStateWithArgs) ? null : new object[2] { arg1, arg2 }, priority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) =>
		{
			action(arg1, arg2, arg3);
			return (object)null;
		}, (!WIGStartInfo.FillStateWithArgs) ? null : new object[3] { arg1, arg2, arg3 }, priority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult QueueWorkItem<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) =>
		{
			action(arg1, arg2, arg3, arg4);
			return (object)null;
		}, (!WIGStartInfo.FillStateWithArgs) ? null : new object[4] { arg1, arg2, arg3, arg4 }, priority);
		Enqueue(workItem);
		return workItem.GetWorkItemResult();
	}

	public IWorkItemResult<TResult> QueueWorkItem<TResult>(Func<TResult> func, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) => func(), priority);
		Enqueue(workItem);
		return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
	}

	public IWorkItemResult<TResult> QueueWorkItem<T, TResult>(Func<T, TResult> func, T arg, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) => func(arg), (!WIGStartInfo.FillStateWithArgs) ? null : new object[1] { arg }, priority);
		Enqueue(workItem);
		return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
	}

	public IWorkItemResult<TResult> QueueWorkItem<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) => func(arg1, arg2), (!WIGStartInfo.FillStateWithArgs) ? null : new object[2] { arg1, arg2 }, priority);
		Enqueue(workItem);
		return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
	}

	public IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) => func(arg1, arg2, arg3), (!WIGStartInfo.FillStateWithArgs) ? null : new object[3] { arg1, arg2, arg3 }, priority);
		Enqueue(workItem);
		return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
	}

	public IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkItemPriority priority = WorkItemPriority.Normal)
	{
		PreQueueWorkItem();
		WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, (object state) => func(arg1, arg2, arg3, arg4), (!WIGStartInfo.FillStateWithArgs) ? null : new object[4] { arg1, arg2, arg3, arg4 }, priority);
		Enqueue(workItem);
		return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
	}
}
