using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using VMControls.Interface;

namespace VM.UnRedo;

internal class RootUnRedoOperation : DisplayNotify, IUnRedoOperation, IDisposable, IBuilder, IRootParam
{
	public bool IsPart { get; set; }

	public ObservableCollection<IUnRedoActionOperation> Actions { get; set; }

	public ObservableCollection<IUnRedoActionOperation> EnumEntrysA
	{
		get
		{
			return Actions;
		}
		set
		{
			if (Actions != value)
			{
				Actions = value;
				OnPropertyChanged("EnumEntrysA");
			}
		}
	}

	public bool ForbidBuild { get; set; }

	public string RootOperType { get; set; }

	public void Redo()
	{
		if (RootOperType == "CopyModule")
		{
			Actions.Where((IUnRedoActionOperation x) => x.ExecutePriority != ExecutePriority.Lower).ToList().ForEach((IUnRedoActionOperation x) =>
			{
				x.Redo();
			});
			Actions.Where((IUnRedoActionOperation x) => x.ExecutePriority == ExecutePriority.Lower).ToList().ForEach((IUnRedoActionOperation x) =>
			{
				x.Redo();
			});
		}
		else
		{
			Actions.Where((IUnRedoActionOperation x) => x.ExecutePriority == ExecutePriority.Heigher).ToList().ForEach((IUnRedoActionOperation x) =>
			{
				x.Redo();
			});
			Actions.Where((IUnRedoActionOperation x) => x.ExecutePriority != ExecutePriority.Heigher).ToList().ForEach((IUnRedoActionOperation x) =>
			{
				x.Redo();
			});
		}
	}

	public void Undo()
	{
		if (RootOperType == "CopyModule")
		{
			Actions.Reverse().ToList().ForEach((IUnRedoActionOperation x) =>
			{
				x.Undo();
			});
			return;
		}
		Actions.Where((IUnRedoActionOperation x) => x.ExecutePriority == ExecutePriority.Heigher).Reverse().ToList()
			.ForEach((IUnRedoActionOperation x) =>
			{
				x.Undo();
			});
		Actions.Where((IUnRedoActionOperation x) => x.ExecutePriority != ExecutePriority.Heigher).Reverse().ToList()
			.ForEach((IUnRedoActionOperation x) =>
			{
				x.Undo();
			});
	}

	public void BuildPart(IUnRedoActionOperation unRedoOperation)
	{
		Actions.Add(unRedoOperation);
	}

	public void BuildParams(object dispatcher, Expression<Action> recover)
	{
		Actions.ToList().ForEach((IUnRedoActionOperation x) =>
		{
			x.Params = new UnRedoParams
			{
				Dispatcher = dispatcher,
				RecoverCallBack = recover
			};
		});
	}

	public void BuildDispalyName(string displayName)
	{
		DisplayNameA = displayName;
	}

	public void BuildInStack(bool isUndoStack)
	{
		IsUndoStack = isUndoStack;
		EnumEntrysA.ToList().ForEach((IUnRedoActionOperation x) =>
		{
			(x as DisplayNotify).IsUndoStack = isUndoStack;
		});
	}

	public void BuildOperType(IRootParam param)
	{
		RootOperType = param.RootOperType;
		ForbidBuild = param.ForbidBuild;
	}

	public void Dispose()
	{
		Actions.ToList().ForEach((IUnRedoActionOperation x) =>
		{
			x.Dispose();
		});
		Actions = null;
	}
}
