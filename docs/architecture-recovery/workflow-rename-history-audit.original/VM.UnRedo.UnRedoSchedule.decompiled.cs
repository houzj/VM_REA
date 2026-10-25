using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using VM.Utility;
using VMControls.Interface;

namespace VM.UnRedo;

public class UnRedoSchedule : IUnRedoSchedule, INotifyPropertyChanged
{
	private PoolStack<IUnRedoOperation> _undoStack;

	private PoolStack<IUnRedoOperation> _redoStack;

	private bool _lockAreaStatus = false;

	private int _nLaziLoading = 0;

	private bool isRunning;

	private int _unRedoing = 0;

	public bool IsOpenParam { get; set; } = false;

	public string MaskMessage { get; set; } = "";

	public bool isCopy { get; set; } = false;

	public bool IsRunning
	{
		get
		{
			return isRunning;
		}
		set
		{
			if (isRunning != value)
			{
				isRunning = value;
				OnPropertyChanged("IsRunning");
			}
		}
	}

	public bool CanUndo => _undoStack.Count > 0;

	public bool CanRedo => _redoStack.Count > 0;

	public bool LockAreaStatus
	{
		get
		{
			return _lockAreaStatus;
		}
		set
		{
			if (_lockAreaStatus != value)
			{
				_lockAreaStatus = value;
				OnPropertyChanged("LockAreaStatus");
			}
		}
	}

	public IList EnumEntrysA => _undoStack.Reverse().Union(_redoStack).ToList();

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	public UnRedoSchedule()
	{
		ClearTemporaryFiles();
		_undoStack = new PoolStack<IUnRedoOperation>();
		_redoStack = new PoolStack<IUnRedoOperation>();
		string config = ConfigurationHelper.GetConfig("UnRedoStackCount");
		uint result = 15u;
		if (!uint.TryParse(config, out result))
		{
			result = 15u;
		}
		_undoStack.SetLenght(result);
		_redoStack.SetLenght(result);
	}

	public void SetForbidRecord(bool isForbid)
	{
		if (_undoStack.Count != 0)
		{
			IRootParam rootParam = _undoStack.Peek() as IRootParam;
			rootParam.ForbidBuild = isForbid;
		}
	}

	private void RedoStackClear(IDisposeTempFile compareObject)
	{
		if (_redoStack.Count != 0)
		{
			Enumerable.ToList(_redoStack).ForEach((IUnRedoOperation x) =>
			{
				DisposeFileForBuilder(x as IBuilder, compareObject);
				x.Dispose();
			});
			_redoStack.Clear();
		}
	}

	private void DisposeFileForBuilder(IBuilder builder, IDisposeTempFile compareObject)
	{
		builder.Actions.Where((IUnRedoActionOperation x) => x is IDisposeTempFile)?.ToList().ForEach((IUnRedoActionOperation x) =>
		{
			(x as IDisposeTempFile)?.DeleteFile(compareObject);
		});
	}

	private void RedoStackClear()
	{
		if (_redoStack.Count != 0)
		{
			Enumerable.ToList(_redoStack).ForEach((IUnRedoOperation x) =>
			{
				x.Dispose();
			});
			_redoStack.Clear();
		}
	}

	private void NotifyProperty()
	{
		OnPropertyChanged("CanUndo");
		OnPropertyChanged("CanRedo");
		OnPropertyChanged("EnumEntrysA");
	}

	public void Register(IUnRedoOperation unRedoOperation, string displayStr, Expression<Func<IBuilder, IUnRedoOperation, bool>> CanRegister = null)
	{
		if (_unRedoing <= 0 && _nLaziLoading <= 0 && (CanRegister == null || _undoStack.Count != 0) && (!unRedoOperation.IsPart || _undoStack.Count != 0) && (!unRedoOperation.IsPart || !(_undoStack.Peek() as IRootParam).ForbidBuild) && (CanRegister == null || CanRegister.Compile()(_undoStack.Peek() as IBuilder, unRedoOperation)))
		{
			if (unRedoOperation.IsPart)
			{
				BuildPart(unRedoOperation as IUnRedoActionOperation, displayStr);
			}
			else
			{
				BuildRoot(displayStr, unRedoOperation as IRootParam);
			}
			NotifyProperty();
		}
	}

	public void Undo(object dispatcher)
	{
		try
		{
			Interlocked.Increment(ref _unRedoing);
			if (_undoStack.Count != 0)
			{
				UpdateActionData();
				IUnRedoOperation unRedoOperation = _undoStack.Pop();
				_redoStack.Push(unRedoOperation);
				(unRedoOperation as IBuilder).BuildParams(dispatcher, () => RecoverStack(_redoStack, _undoStack));
				(unRedoOperation as IBuilder).BuildInStack(isUndoStack: false);
				unRedoOperation.Undo();
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			Interlocked.Decrement(ref _unRedoing);
			NotifyProperty();
		}
	}

	public void Redo(object dispatcher)
	{
		try
		{
			Interlocked.Increment(ref _unRedoing);
			if (_redoStack.Count != 0)
			{
				IUnRedoOperation unRedoOperation = _redoStack.Pop();
				_undoStack.Push(unRedoOperation);
				(unRedoOperation as IBuilder).BuildParams(dispatcher, () => RecoverStack(_undoStack, _redoStack));
				(unRedoOperation as IBuilder).BuildInStack(isUndoStack: true);
				unRedoOperation.Redo();
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			Interlocked.Decrement(ref _unRedoing);
			NotifyProperty();
		}
	}

	public void UpdateUnRedoStep(double size)
	{
		uint num = uint.Parse(size.ToString());
		bool flag = ((_undoStack.Count + _redoStack.Count > num) ? true : false);
		_undoStack.SetLenght(num);
		_redoStack.SetLenght(num);
		if (flag)
		{
			RedoStackClear();
			while (_undoStack.Count > num)
			{
				IDisposable disposable = _undoStack.TryRemoveLast();
				disposable.Dispose();
			}
			NotifyProperty();
		}
	}

	private void RecoverStack(PoolStack<IUnRedoOperation> source, PoolStack<IUnRedoOperation> target)
	{
		IUnRedoOperation item = source.Pop();
		target.Push(item);
	}

	public void SetIsLaziLoading(bool isLaziLoading)
	{
		_nLaziLoading = (isLaziLoading ? Interlocked.Increment(ref _nLaziLoading) : Interlocked.Decrement(ref _nLaziLoading));
	}

	public void SetRunningStatus(bool isRunning)
	{
		IsRunning = isRunning;
	}

	public void SetLockStatus(bool isLock)
	{
		LockAreaStatus = isLock;
	}

	public void ModifyDisplayInfo(string displayStr)
	{
		if (_unRedoing <= 0 && _nLaziLoading <= 0 && _undoStack.Count != 0)
		{
			IUnRedoOperation unRedoOperation = _undoStack.Peek();
			if (unRedoOperation != null && unRedoOperation is DisplayNotify displayNotify)
			{
				displayNotify.DisplayNameA += $": {displayStr}";
			}
		}
	}

	public void RemoveEmptyOperation()
	{
		if (_undoStack.Count != 0)
		{
			IUnRedoOperation unRedoOperation = _undoStack.Peek();
			if ((unRedoOperation as IBuilder).Actions.Count == 0)
			{
				_undoStack.Pop();
				NotifyProperty();
			}
		}
	}

	private void ClearTemporaryFiles()
	{
		string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\TempFiles";
		if (Directory.Exists(path))
		{
			Directory.Delete(path, recursive: true);
		}
	}

	public bool IsNeedExport()
	{
		if (_unRedoing > 0 || _nLaziLoading > 0)
		{
			return false;
		}
		if (_undoStack.Count == 0)
		{
			return false;
		}
		if (_undoStack.Count != 0 && (_undoStack.Peek() as IRootParam).ForbidBuild)
		{
			return false;
		}
		return true;
	}

	public bool IsNeedCache()
	{
		if (_unRedoing > 0 || _nLaziLoading > 0)
		{
			return false;
		}
		if (_undoStack.Count == 0)
		{
			return false;
		}
		return true;
	}

	public bool ValiteType(string typeName)
	{
		if (_unRedoing > 0)
		{
			return false;
		}
		if (_nLaziLoading > 0)
		{
			return false;
		}
		if (_undoStack.Count == 0)
		{
			return false;
		}
		IRootParam rootParam = _undoStack.Peek() as IRootParam;
		if (rootParam.RootOperType != typeName)
		{
			return false;
		}
		return true;
	}

	public void DeleteLastOperation()
	{
		if (_unRedoing <= 0 && _nLaziLoading <= 0 && _undoStack.Count != 0)
		{
			IUnRedoOperation unRedoOperation = _undoStack.Pop();
			unRedoOperation.Dispose();
			NotifyProperty();
		}
	}

	private void BuildRoot(string displayName, IRootParam param)
	{
		IBuilder builder = new RootUnRedoOperation();
		builder.Actions = new SafeObservableCollection<IUnRedoActionOperation>();
		builder.BuildDispalyName(displayName);
		builder.BuildInStack(isUndoStack: true);
		builder.BuildOperType(param);
		if (_undoStack.TryPush(builder as IUnRedoOperation, out var removeItem))
		{
			(removeItem as IDisposable).Dispose();
		}
	}

	private void BuildPart(IUnRedoActionOperation unRedoOperation, string displayStr)
	{
		if (_undoStack.Count != 0)
		{
			if (unRedoOperation is IDisposeTempFile compareObject)
			{
				RedoStackClear(compareObject);
			}
			else
			{
				RedoStackClear();
			}
			IBuilder builder = (unRedoOperation.ParentNode = _undoStack.Peek() as IBuilder);
			(unRedoOperation as DisplayNotify).DisplayNameA = displayStr;
			(unRedoOperation as DisplayNotify).IsUndoStack = true;
			builder.BuildPart(unRedoOperation);
		}
	}

	private void UpdateActionData()
	{
		IBuilder builder = _undoStack.Peek() as IBuilder;
		builder.Actions?.Where((IUnRedoActionOperation x) => !x.IsDataComplete).ToList().ForEach((IUnRedoActionOperation x) =>
		{
			x.IsDataComplete = true;
			x.Update();
		});
	}

	public bool GetIsInUnRedo()
	{
		return _unRedoing > 0;
	}
}
