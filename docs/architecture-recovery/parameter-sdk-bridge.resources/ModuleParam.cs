using System;
using System.Linq;
using System.Linq.Expressions;
using Apps.Localization;
using VM.PlatformSDKCS;
using VMControls.BaseInterface;
using VMControls.Interface;

namespace VM.Core;

/// <summary>
/// 数据设置到底层模块，包括算法参数和二进制数据
/// </summary>
public class ModuleParam : IParamsConfig, IUserStringData, IUserBytesData, IParent, IData
{
	public VmModule Parent;

	public uint TimeOut { get; set; }

	public virtual string this[string key]
	{
		get
		{
			string strValue = "";
			if (!Parent.IsOnDispose)
			{
				if (Parent is VmModuleSet && key.StartsWith("%"))
				{
					string[] array = key.Split(',');
					ServerSDKManager.serverSDKManager.mModuleManager.GetDynamicRunParamValue(Parent.ID, array[0], ref strValue, bThrowException: true, (array.Count() > 1) ? array[1] : "");
				}
				else
				{
					ServerSDKManager.serverSDKManager.mModuleManager.GetParamValue(Parent.ID, key, ref strValue, bThrowException: true);
				}
			}
			return strValue;
		}
		set
		{
			if (!Parent.IsOnDispose)
			{
				if (Parent.IsGlobalModu)
				{
					SetValueBySDK(key, value);
					return;
				}
				RecordParamUnRedo(key);
				SetValueBySDK(key, value);
				Parent.OnModuleParamChanged(key);
			}
		}
	}

	byte[] IUserBytesData.this[string key]
	{
		get
		{
			byte[] data = null;
			if (!Parent.IsOnDispose)
			{
				if (Parent is VmModuleSet && key.StartsWith("%"))
				{
					string[] array = key.Split(',');
					ServerSDKManager.serverSDKManager.mModuleManager.GetDynamicBinaryParamValue(Parent.ID, array[0], ref data, (array.Count() > 1) ? array[1] : "");
				}
				else
				{
					int binaryData = ServerSDKManager.serverSDKManager.mModuleManager.GetBinaryData(Parent.ID, key, ref data, TimeOut);
					if (binaryData != 0)
					{
						throw new Exception($"{binaryData:X000}");
					}
				}
			}
			return data;
		}
		set
		{
			if (Parent.IsOnDispose)
			{
				return;
			}
			if (Parent.IsContainsModel)
			{
				Expression<Func<object>> oldModel = () => Parent.GetModel();
				string displayName = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), Parent.FullName, AppLocalizationService.GetLang("模型"));
				Ioc.Get<IUnRedoService>()?.Register(new ModelUnRedoOperation(Model_Redo, Model_Undo, Model_Update, new UnRedo_ModelData
				{
					ModuleFullName = Parent.FullName,
					OldModel = oldModel,
					NewModel = null
				}), displayName, (IBuilder builder, IUnRedoOperation operation) => CanRegisterModel(builder, operation));
			}
			int num = -536870657;
			if (Parent is VmModuleSet && key.StartsWith("%"))
			{
				string[] array = key.Split(',');
				num = ServerSDKManager.serverSDKManager.mModuleManager.SetDynamicBinaryParamValue(Parent.ID, array[0], value, (array.Count() > 1) ? array[1] : "");
			}
			else
			{
				num = ServerSDKManager.serverSDKManager.mModuleManager.SetBinaryData(Parent.ID, key, value, TimeOut);
			}
			if (num != 0)
			{
				throw new Exception($"{num:X000}");
			}
		}
	}

	public string XmlPath { get; set; }

	object IParent.Parent
	{
		get
		{
			return Parent;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public object Data { get; set; }

	private void RecordParamUnRedo(string key)
	{
		if (Parent is VmModuleSet && key.StartsWith("%"))
		{
			return;
		}
		Expression<Func<object>> oldParamList = () => Parent.GetModuleAllParams();
		string displayName = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), Parent.FullName, AppLocalizationService.GetLang("Tab_Params"));
		Ioc.Get<IUnRedoService>()?.Register(new ParamUnRedoOperation(Param_Redo, Param_Undo, Param_Update, new UnRedo_ParamsData
		{
			ModuleFullName = Parent.FullName,
			OldParamList = oldParamList,
			NewParamList = null
		}, Parent.IsGression), displayName, (IBuilder builder, IUnRedoOperation operation) => CanRegisterParam(builder, operation));
		if (Parent.IsContainsModel && Parent.IsConfigTriggerSaveModel)
		{
			Expression<Func<object>> oldModel = () => Parent.GetModel();
			string displayName2 = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), Parent.FullName, AppLocalizationService.GetLang("模型"));
			Ioc.Get<IUnRedoService>()?.Register(new ModelUnRedoOperation(Model_Redo, Model_Undo, Model_Update, new UnRedo_ModelData
			{
				ModuleFullName = Parent.FullName,
				OldModel = oldModel,
				NewModel = null
			}), displayName2, (IBuilder builder, IUnRedoOperation operation) => CanRegisterModel(builder, operation));
		}
	}

	private void SetValueBySDK(string key, string value)
	{
		if (Parent is VmModuleSet && key.StartsWith("%"))
		{
			string[] array = key.Split(',');
			ServerSDKManager.serverSDKManager.mModuleManager.SetDynamicRunParamValue(Parent.ID, array[0], value, bThrowException: true, (array.Count() > 1) ? array[1] : "");
		}
		else
		{
			ServerSDKManager.serverSDKManager.mModuleManager.SetParamValue(Parent.ID, key, value, bThrowException: true);
		}
	}

	private bool CanRegisterModel(IBuilder builder, IUnRedoOperation unRedoOperation)
	{
		IUnRedoDataDone<UnRedo_ModelData> unRedoDone = unRedoOperation as IUnRedoDataDone<UnRedo_ModelData>;
		if (builder.Actions.FirstOrDefault((IUnRedoActionOperation p) => p is IUnRedoDataDone<UnRedo_ModelData> unRedoDataDone && unRedoDataDone.Data.ModuleFullName == unRedoDone.Data.ModuleFullName) != null)
		{
			return false;
		}
		if ((builder as IRootParam).RootOperType != "ModifyConfig")
		{
			return false;
		}
		if (unRedoDone.Data.OldModel is Expression<Func<object>> expression)
		{
			unRedoDone.Data.OldModel = expression.Compile()();
		}
		return true;
	}

	private void Model_Undo(UnRedo_ModelData data, IUnRedoParams unRedoParams)
	{
		object obj = VmSolution.Instance[data.ModuleFullName];
		(obj as VmModule)?.ImportModel(data.OldModel);
	}

	private void Model_Redo(UnRedo_ModelData data, IUnRedoParams unRedoParams)
	{
		object obj = VmSolution.Instance[data.ModuleFullName];
		(obj as VmModule)?.ImportModel(data.NewModel);
	}

	private void Model_Update(UnRedo_ModelData data)
	{
		byte[] newModel = ((VmModule)VmSolution.Instance[data.ModuleFullName])?.GetModel();
		data.NewModel = newModel;
	}

	private bool CanRegisterParam(IBuilder builder, IUnRedoOperation unRedoOperation)
	{
		IUnRedoDataDone<UnRedo_ParamsData> unRedoDone = unRedoOperation as IUnRedoDataDone<UnRedo_ParamsData>;
		if (builder.Actions.FirstOrDefault((IUnRedoActionOperation p) => p is IUnRedoDataDone<UnRedo_ParamsData> unRedoDataDone && unRedoDataDone.Data.ModuleFullName == unRedoDone.Data.ModuleFullName) != null)
		{
			return false;
		}
		if ((builder as IRootParam).RootOperType != "ModifyConfig")
		{
			return false;
		}
		if (VmSolution.Instance[unRedoDone.Data.ModuleFullName] is IVmProcedure)
		{
			return false;
		}
		if (unRedoDone.Data.OldParamList is Expression<Func<object>> expression)
		{
			unRedoDone.Data.OldParamList = expression.Compile()();
		}
		return true;
	}

	private void Param_Undo(UnRedo_ParamsData data, IUnRedoParams unRedoParams)
	{
		object obj = VmSolution.Instance[data.ModuleFullName];
		try
		{
			(obj as VmModule)?.SetAllParams(data.OldParamList);
		}
		catch (Exception)
		{
		}
		if (!(obj is IMVSGroup))
		{
			(obj as VmModule)?.ReFlashReadyState();
		}
	}

	private void Param_Redo(UnRedo_ParamsData data, IUnRedoParams unRedoParams)
	{
		object obj = VmSolution.Instance[data.ModuleFullName];
		try
		{
			(obj as VmModule)?.SetAllParams(data.NewParamList);
		}
		catch (Exception)
		{
		}
		if (!(obj is IMVSGroup))
		{
			(obj as VmModule)?.ReFlashReadyState();
		}
	}

	private void Param_Update(UnRedo_ParamsData data)
	{
		object newParamList = ((VmModule)VmSolution.Instance[data.ModuleFullName])?.GetModuleAllParams();
		data.NewParamList = newParamList;
	}
}
