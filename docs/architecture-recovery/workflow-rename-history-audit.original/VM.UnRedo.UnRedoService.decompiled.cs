using System;
using System.Linq.Expressions;
using System.Windows.Controls;
using VMControls.Interface;

namespace VM.UnRedo;

public class UnRedoService : IUnRedoService
{
	private IUnRedoSchedule _unRedoSchedule;

	public UnRedoService()
	{
		_unRedoSchedule = new UnRedoSchedule();
	}

	public void Redo(object dispatcher)
	{
		_unRedoSchedule.Redo(dispatcher);
	}

	public void Register(IUnRedoOperation unRedoOperation, string displayName, Expression<Func<IBuilder, IUnRedoOperation, bool>> CanRegister = null)
	{
		_unRedoSchedule.Register(unRedoOperation, displayName, CanRegister);
	}

	public void SetIsLaziLoading(bool isLaziLoading)
	{
		_unRedoSchedule.SetIsLaziLoading(isLaziLoading);
	}

	public void SetOpenParamValue(bool isOpen)
	{
		_unRedoSchedule.IsOpenParam = isOpen;
	}

	public void SetRunningStatus(bool isRunning)
	{
		_unRedoSchedule.SetRunningStatus(isRunning);
	}

	public void SetLockStatus(bool isLock)
	{
		_unRedoSchedule.SetLockStatus(isLock);
	}

	public void Undo(object dispatcher)
	{
		_unRedoSchedule.Undo(dispatcher);
	}

	public bool IsNeedExport()
	{
		return _unRedoSchedule.IsNeedExport();
	}

	public bool ValiteType(string typeName)
	{
		return _unRedoSchedule.ValiteType(typeName);
	}

	public void ModifyDisplayInfo(string displayStr)
	{
		_unRedoSchedule.ModifyDisplayInfo(displayStr);
	}

	public void RemoveEmptyOperation()
	{
		_unRedoSchedule.RemoveEmptyOperation();
	}

	public void DeleteLastOperation()
	{
		_unRedoSchedule.DeleteLastOperation();
	}

	public IUnRedoSchedule GetScheduleInstance()
	{
		return _unRedoSchedule;
	}

	public UserControl GetUnRedoView()
	{
		return new UnRedoDispaly(_unRedoSchedule);
	}

	public void UpdateUnRedoStep(double size)
	{
		_unRedoSchedule.UpdateUnRedoStep(size);
	}

	public string GetExportMessage(string message = null)
	{
		if (message != null)
		{
			_unRedoSchedule.MaskMessage = message;
		}
		return _unRedoSchedule.MaskMessage;
	}

	public bool SetCopyStatus(string isCopy = null)
	{
		if (isCopy != null)
		{
			_unRedoSchedule.isCopy = bool.Parse(isCopy);
		}
		return _unRedoSchedule.isCopy;
	}

	public void SetForbidRecord(bool isForbid)
	{
		_unRedoSchedule.SetForbidRecord(isForbid);
	}

	public bool IsInUnRedo()
	{
		return _unRedoSchedule.GetIsInUnRedo();
	}

	public bool IsNeedCache()
	{
		return _unRedoSchedule.IsNeedCache();
	}
}
