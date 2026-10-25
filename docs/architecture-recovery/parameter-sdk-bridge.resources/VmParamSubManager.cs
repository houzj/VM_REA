using System;
using System.Linq;
using Apps.Log;
using VM.PlatformSDKCS;
using VMControls.Interface;

namespace VM.Core;

/// <summary>
/// 模块运行参数订阅管理
/// </summary>
public class VmParamSubManager
{
	private VmModule Owner;

	public VmParamSubManager(VmModule module)
	{
		Owner = module;
	}

	/// <summary>
	/// 关联订阅
	/// </summary>
	/// <param name="paramName"></param>
	/// <param name="paramType"></param>
	/// <param name="subType"></param>
	/// <param name="constantValue"></param>
	/// <param name="targetModuleId"></param>
	/// <param name="targetParamName"></param>
	/// <param name="indexType"></param>
	/// <param name="indexConstantValue"></param>
	/// <param name="indexModuleId"></param>
	/// <param name="indexParamName"></param>
	/// <returns></returns>
	public bool SetModuleParamBinding(string paramName, string paramType, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE subType, string constantValue = "", uint targetModuleId = 0u, string targetParamName = "", ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE indexType = ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_INVALID, string indexConstantValue = "", uint indexModuleId = 0u, string indexParamName = "")
	{
		ImvsSdkDefine.IMVS_SUBSCRIBE_MODU_PARAM_INFO stBindValue = default;
		stBindValue.eVarType = subType;
		stBindValue.strConstantValue = StringUtil.UTF8GetFixLenBytes(constantValue, 256);
		stBindValue.stVariableValue.stModuleParam.nModuleId = (int)targetModuleId;
		stBindValue.stVariableValue.stModuleParam.byParamName = StringUtil.UTF8GetFixLenBytes(targetParamName, 128);
		stBindValue.stVariableValue.eIndexType = indexType;
		stBindValue.stVariableValue.strIndexConstant = StringUtil.UTF8GetFixLenBytes(indexConstantValue, 256);
		stBindValue.stVariableValue.stIndexVariable.nModuleId = (int)indexModuleId;
		stBindValue.stVariableValue.stIndexVariable.byParamName = StringUtil.UTF8GetFixLenBytes(indexParamName, 128);
		ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST moduleParamBinding = new ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST
		{
			nNum = 1u,
			astBindModuVarList = new ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO[128]
		};
		moduleParamBinding.astBindModuVarList[0].nModuleId = Owner.ID;
		moduleParamBinding.astBindModuVarList[0].eVarType = GetVarType(paramType);
		moduleParamBinding.astBindModuVarList[0].strParamName = StringUtil.UTF8GetFixLenBytes(paramName, 128);
		moduleParamBinding.astBindModuVarList[0].stBindValue = stBindValue;
		try
		{
			ServerSDKManager.serverSDKManager.mModuleManager.SetModuleParamBinding(moduleParamBinding);
			return true;
		}
		catch (VmException ex)
		{
			LogHelper.Error(ex.errorCode + ex.errorMessage);
		}
		catch (Exception ex2)
		{
			LogHelper.Error(ex2.Message);
		}
		return false;
	}

	/// <summary>
	/// 获取订阅
	/// </summary>
	/// <param name="paramName"></param>
	/// <returns></returns>
	public string GetModuleParamBinding(string paramName)
	{
		try
		{
			ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST? moduleParamBindingList = ServerSDKManager.serverSDKManager.mModuleManager.GetModuleParamBindingList(Owner.ID, 0, 100);
			if (moduleParamBindingList.HasValue)
			{
				ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO item = moduleParamBindingList.GetValueOrDefault().astBindModuVarList.FirstOrDefault((ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO p) => StringUtil.UTF8GetString(p.strParamName) == paramName);
				switch (item.stBindValue.eVarType)
				{
				case ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_CONSTANT:
					return StringUtil.UTF8GetString(item.stBindValue.strConstantValue);
				case ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_VARIABLE:
				{
					int targetModuleId = item.stBindValue.stVariableValue.stModuleParam.nModuleId;
					string sub = StringUtil.UTF8GetString(item.stBindValue.stVariableValue.stModuleParam.byParamName);
					VmModule vmModule = Owner.FrontModules.OfType<VmModule>().FirstOrDefault((VmModule p) => p.ID == targetModuleId);
					if (vmModule == null)
					{
						vmModule = VmSolution.Instance.Modules.FirstOrDefault((VmModule p) => p.ID == targetModuleId);
						if (vmModule == null && ServerSDKManager.serverSDKManager.mModuleManager.CheckLocalVariableModule(targetModuleId))
						{
							vmModule = (VmModule)((Owner is IVmModuleSet) ? (Owner as IVmModuleSet).LocalVariablModule : (Owner.Parent as IVmModuleSet).LocalVariablModule);
						}
					}
					VmIO vmIO = null;
					if (vmModule != null)
					{
						vmIO = (VmIO)vmModule[sub];
						if (vmIO == null && vmModule.ID == 13000)
						{
							vmModule.InitGlobalIo();
							vmIO = (VmIO)vmModule[sub];
						}
					}
					if (vmIO == null)
					{
						LogHelper.Error("Get target io error.");
						return "";
					}
					string text = "";
					switch (item.stBindValue.stVariableValue.eIndexType)
					{
					case ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_ALL:
						text = "[]";
						break;
					case ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_CONSTANT:
					{
						string arg = StringUtil.UTF8GetString(item.stBindValue.stVariableValue.strIndexConstant);
						text = $"[{arg}]";
						break;
					}
					case ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_VARIABLE:
					{
						VmModule vmModule2 = Owner.FrontModules.OfType<VmModule>().FirstOrDefault((VmModule p) => p.ID == item.stBindValue.stVariableValue.stIndexVariable.nModuleId);
						if (vmModule2 == null)
						{
							vmModule2 = VmSolution.Instance.Modules.FirstOrDefault((VmModule p) => p.ID == item.stBindValue.stVariableValue.stIndexVariable.nModuleId);
						}
						string sub2 = StringUtil.UTF8GetString(item.stBindValue.stVariableValue.stIndexVariable.byParamName);
						VmIO vmIO2 = (VmIO)vmModule2[sub2];
						text = $"[{vmIO2.ModulePathName}]";
						break;
					}
					}
					return vmIO.ModulePathName + text;
				}
				case ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_GLOBAL:
					return StringUtil.UTF8GetString(item.stBindValue.stVariableValue.stModuleParam.byParamName);
				}
			}
		}
		catch (VmException ex)
		{
			LogHelper.Error(ex.errorCode + ex.errorMessage);
		}
		catch (Exception ex2)
		{
			LogHelper.Error(ex2.Message);
		}
		return "";
	}

	/// <summary>
	/// 删除订阅
	/// </summary>
	/// <param name="paramName"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	public bool DelModuleParamBinding(string paramName, string type)
	{
		ImvsSdkDefine.IMVS_UNBIND_MODULE_PARAM_INFO_LIST stModuleVarInfo = new ImvsSdkDefine.IMVS_UNBIND_MODULE_PARAM_INFO_LIST
		{
			nNum = 1u,
			astBindModuVarList = new ImvsSdkDefine.IMVS_UNBIND_MODULE_PARAM_INFO[128]
		};
		stModuleVarInfo.astBindModuVarList[0].nModuleId = Owner.ID;
		stModuleVarInfo.astBindModuVarList[0].strParamName = StringUtil.UTF8GetFixLenBytes(paramName, 128);
		stModuleVarInfo.astBindModuVarList[0].eVarType = GetVarType(type);
		try
		{
			ServerSDKManager.serverSDKManager.mModuleManager.DelModuleParamBinding(stModuleVarInfo);
			return true;
		}
		catch (VmException ex)
		{
			LogHelper.Error(ex.errorCode + ex.errorMessage);
		}
		catch (Exception ex2)
		{
			LogHelper.Error(ex2.Message);
		}
		return false;
	}

	/// <summary>
	/// 类型转换
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE GetVarType(string type)
	{
		return type switch
		{
			"int" => ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_INT, 
			"float" => ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_FLOAT, 
			"string" => ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_STRING, 
			"double" => ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_DOUBLE, 
			_ => ImvsSdkDefine.IMVS_ENUM_MODULE_RESULT_TYPE.IMVS_MODURES_TYPE_INVALID, 
		};
	}
}
