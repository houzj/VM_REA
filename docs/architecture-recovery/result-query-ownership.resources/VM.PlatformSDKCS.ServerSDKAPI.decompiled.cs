using System;
using System.IO;
using System.Runtime.InteropServices;
using HikExternalCall.Common;

namespace VM.PlatformSDKCS;

/// <summary>
/// 服务SDK接口类
/// </summary>
/// <summary>
/// ServerSDKAPI3D
/// </summary>
public static class ServerSDKAPI
{
	/// <summary>
	/// 修改模块订阅输入名称
	/// </summary>
	/// <returns></returns>
	public static int IMVS_ModifyModuleSubInputName(IntPtr handle, uint nModuleID, string strNameOld, string strNameNew)
	{
		return NativeMethods.IMVS_ModifyModuleSubInputName(handle, nModuleID, strNameOld, strNameNew);
	}

	/// <summary>
	/// 获取SDK4Server版本信息
	/// </summary>
	/// <returns></returns>
	public static int IMVS_GetBuildVersion(ref uint nYear, ref uint nMonth, ref uint nDay)
	{
		return NativeMethods.IMVS_GetBuildVersion(ref nYear, ref nMonth, ref nDay);
	}

	/// <summary>
	/// 创建SDK4Server的句柄
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pstPlatformBasicInfo"></param>
	/// <returns></returns>
	public static int IMVS_CreateHandle(ref IntPtr handle, ref ImvsSdkDefine.IMVS_PLATFORM_BASIC_INFO pstPlatformBasicInfo)
	{
		return NativeMethods.IMVS_CreateHandle(ref handle, ref pstPlatformBasicInfo);
	}

	/// <summary>
	/// 销毁SDK4Server的句柄
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static int IMVS_DestroyHandle(IntPtr handle, uint nTakeoverType = 0u)
	{
		return NativeMethods.IMVS_DestroyHandle(handle, nTakeoverType);
	}

	/// <summary>
	/// 销毁句柄
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static int IMVS_PF_DestroyHandle(IntPtr handle)
	{
		return NativeMethods.IMVS_PF_DestroyHandle(handle);
	}

	/// <summary>
	/// 注册注册输入算法平台回调
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pOutputCallBack"></param>
	/// <param name="user"></param>
	/// <returns></returns>
	public static int IMVS_RegisterOutputPlatformInfoCallBack(IntPtr handle, delegateOutputCallBack pOutputCallBack, IntPtr user)
	{
		return NativeMethods.IMVS_RegisterOutputPlatformInfoCallBack(handle, pOutputCallBack, user);
	}

	/// <summary>
	/// 注册注册输入算法平台回调（筛选后）
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pOutputCallBack"></param>
	/// <param name="user"></param>
	/// <returns></returns>
	public static int IMVS_RegisterOutputPlatformInfoCallBack_V32(IntPtr handle, delegateOutputCallBack pOutputCallBack, IntPtr user)
	{
		return NativeMethods.IMVS_RegisterOutputPlatformInfoCallBack_V32(handle, pOutputCallBack, user);
	}

	/// <summary>
	/// 清除所有自定义导出结果
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nProcessID"></param>
	/// <returns></returns>
	public static int IMVS_ClearCustomOutputInfo(IntPtr handle, uint nProcessID)
	{
		return NativeMethods.IMVS_ClearCustomOutputInfo(handle, nProcessID);
	}

	/// <summary>
	/// 获取server平台的详细
	/// </summary>
	public static int IMVS_GetPlatformDetailedInfo(IntPtr pHandle, IntPtr pstPlatformDetailedInfo)
	{
		return NativeMethods.IMVS_GetPlatformDetailedInfo(pHandle, pstPlatformDetailedInfo);
	}

	/// <summary>
	/// 订阅(含下标)
	/// </summary>
	public static int IMVS_SubscribeResultEx(IntPtr pHandle, IntPtr pstSubscribeInfo)
	{
		return NativeMethods.IMVS_SubscribeResultEx(pHandle, pstSubscribeInfo);
	}

	/// <summary>
	/// 解订阅(含下标)
	/// </summary>
	public static int IMVS_UnSubscribeResultEx(IntPtr pHhandle, IntPtr pstSubscribeInfo)
	{
		return NativeMethods.IMVS_UnSubscribeResultEx(pHhandle, pstSubscribeInfo);
	}

	/// <summary>
	/// 所有流程停止执行
	/// </summary>
	public static int IMVS_StopExecute(IntPtr pHandle, uint waitTime)
	{
		return NativeMethods.IMVS_StopExecute(pHandle, waitTime);
	}

	/// <summary>
	/// 单流程停止执行
	/// </summary>
	public static int IMVS_StopExecute_V30(IntPtr pHandle, uint processID, uint waitTime)
	{
		return NativeMethods.IMVS_StopExecute_V30(pHandle, processID, waitTime);
	}

	/// <summary>
	/// 关闭一个模块
	/// </summary>
	public static int IMVS_CloseOneModule(IntPtr pHandle, uint nModuleId)
	{
		return NativeMethods.IMVS_CloseOneModule(pHandle, nModuleId);
	}

	/// <summary>
	/// 使能一个模块，0表示禁用，1表示启用
	/// </summary>
	public static int IMVS_EnableModule(IntPtr pHandle, uint nModuleID, uint nEnableFlag = 0u)
	{
		return NativeMethods.IMVS_EnableModule(pHandle, nModuleID, nEnableFlag);
	}

	public static int IMVS_GetModuleState(IntPtr handle, uint nModuleID, ref uint nModuleState)
	{
		return NativeMethods.IMVS_GetModuleState(handle, nModuleID, ref nModuleState);
	}

	public static int IMVS_ConnectOneModule(IntPtr pHandle, uint nPreModuleId, uint nNextModuleId)
	{
		return NativeMethods.IMVS_ConnectOneModule(pHandle, nPreModuleId, nNextModuleId);
	}

	public static int IMVS_DisConnectOneModule(IntPtr pHandle, uint nPreModuleId, uint nNextModuleId)
	{
		return NativeMethods.IMVS_DisConnectOneModule(pHandle, nPreModuleId, nNextModuleId);
	}

	/// <summary>
	/// 执行一次
	/// </summary>
	public static int IMVS_ExecuteOnce(IntPtr pHandle)
	{
		return NativeMethods.IMVS_ExecuteOnce(pHandle, IntPtr.Zero);
	}

	/// <summary>
	/// 连续执行开始
	/// </summary>
	public static int IMVS_ContinousRunStart(IntPtr pHandle)
	{
		return NativeMethods.IMVS_ContinousRunStart(pHandle);
	}

	/// <summary>
	/// 执行一次
	/// </summary>
	public static int IMVS_ExecuteOnce_V30(IntPtr pHandle, uint nProcessID, string strCommand = null, bool bIsP2P = false)
	{
		return NativeMethods.IMVS_ExecuteOnce_V30(pHandle, nProcessID, strCommand, bIsP2P);
	}

	/// <summary>
	/// 连续执行开始
	/// </summary>
	public static int IMVS_ContinousRunStart_V30(IntPtr pHandle, uint nProcessID)
	{
		return NativeMethods.IMVS_ContinousRunStart_V30(pHandle, nProcessID);
	}

	/// <summary>
	/// 关闭方案接口
	/// </summary>
	public static int IMVS_CloseSolution(IntPtr handle)
	{
		return NativeMethods.IMVS_CloseSolution(handle);
	}

	public static int IMVS_SetContinousRunInterval_V30(IntPtr handle, uint processID, uint millSecond)
	{
		return NativeMethods.IMVS_SetContinousRunInterval_V30(handle, processID, millSecond);
	}

	public static int IMVS_GetProcContinousRunInterval(IntPtr handle, uint processID, ref uint millSecond)
	{
		return NativeMethods.IMVS_GetProcContinousRunInterval(handle, processID, ref millSecond);
	}

	public static int IMVS_SetContinousRunInterval(IntPtr handle, uint millSecond)
	{
		return NativeMethods.IMVS_SetContinousRunInterval(handle, millSecond);
	}

	/// <summary>
	/// 模块自执行接口
	/// </summary>
	public static int IMVS_SelfExecuteOneModule(IntPtr handle, uint nModuleId)
	{
		return NativeMethods.IMVS_SelfExecuteOneModule(handle, nModuleId);
	}

	/// <summary>
	/// 模块同步自执行接口
	/// </summary>
	public static int IMVS_SyncSelfExecuteOneModule(IntPtr handle, uint nModuleId)
	{
		return NativeMethods.IMVS_SyncSelfExecuteOneModule(handle, nModuleId);
	}

	/// <summary>
	/// 获取所有模块
	/// </summary>
	public static int IMVS_GetAllModuleList(IntPtr handle, IntPtr pstModuleInfoList)
	{
		return NativeMethods.IMVS_GetAllModuleList(handle, pstModuleInfoList);
	}

	/// <summary>
	/// 获取所有流程
	/// </summary>
	public static int IMVS_GetAllProcessList(IntPtr handle, IntPtr pstProcessInfoList)
	{
		return NativeMethods.IMVS_GetAllProcessList(handle, pstProcessInfoList);
	}

	/// <summary>
	/// 设置模块显示名
	/// </summary>
	public static int IMVS_SetModuDisplayName(IntPtr handle, uint nModuleID, string strDisplayName)
	{
		return NativeMethods.IMVS_SetModuDisplayName(handle, nModuleID, strDisplayName);
	}

	/// <summary>
	/// 设置流程显示名
	/// </summary>
	public static int IMVS_SetProcName(IntPtr handle, uint nProcessID, string strProcName)
	{
		return NativeMethods.IMVS_SetProcName(handle, nProcessID, strProcName);
	}

	/// <summary>
	/// 模块开启/停止回调信息
	/// </summary>
	/// <param name="handle">操作句柄</param>
	/// <param name="nModuleID">模块ID，全部：30000</param>
	/// <param name="nCallbackStatus">回调状态 0：停止回调； 1：开启回调</param>
	/// <returns></returns>
	public static int IMVS_CtrlCallBackModuResult(IntPtr handle, uint nModuleID, uint nCallbackStatus)
	{
		return NativeMethods.IMVS_CtrlCallBackModuResult(handle, nModuleID, nCallbackStatus);
	}

	public static int IMVS_SaveSolution(IntPtr handle, string strPath, string strPassWord, IntPtr pData, uint nDataLen)
	{
		return NativeMethods.IMVS_SaveSolution(handle, strPath, strPassWord, pData, nDataLen);
	}

	public static int IMVS_CheckSolution(string strPath, ref uint pnHasPwd, ref uint pnValidFormat)
	{
		return NativeMethods.IMVS_CheckSolution(strPath, ref pnHasPwd, ref pnValidFormat);
	}

	public static int IMVS_CheckPassword(string strPath, string strPassWord, ref int pnIsCorrect)
	{
		return NativeMethods.IMVS_CheckPassword(strPath, strPassWord, ref pnIsCorrect);
	}

	public static int IMVS_GetSolutionVersion(IntPtr handle, string strPath, string strPassWord, ref ImvsSdkDefine.IMVS_SOLUTION_VERSION_INFO pstSolutionVersionInfo)
	{
		return NativeMethods.IMVS_GetSolutionVersion(handle, strPath, strPassWord, ref pstSolutionVersionInfo);
	}

	public static int IMVS_LoadSolution(IntPtr handle, string strPath, string strPassWord, bool bCheckSolFlag = true, bool bReportSilenceRunFlag = true)
	{
		return NativeMethods.IMVS_LoadSolution(handle, strPath, strPassWord, bCheckSolFlag, bReportSilenceRunFlag);
	}

	public static string IMVS_GetLastError()
	{
		return NativeMethods.IMVS_GetLastError();
	}

	public static int IMVS_AddOneModule(IntPtr handle, ref ImvsSdkDefine.IMVS_SYNC_ADD_ONE_MODULE_INFO pstSyncAddModuInput, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo)
	{
		return NativeMethods.IMVS_AddOneModule(handle, ref pstSyncAddModuInput, ref pstModuleInfo);
	}

	public static int IMVS_AddOneModule(IntPtr handle, string strModuleName, uint nProcessId)
	{
		return NativeMethods.IMVS_AddOneModuleWithProcId(handle, strModuleName, nProcessId);
	}

	public static int IMVS_AddOneProcess(IntPtr handle, string strDisplayName, ref ImvsSdkDefine.IMVS_PROCESS_INFO stPrcInfo)
	{
		return NativeMethods.IMVS_AddOneProcess(handle, strDisplayName, ref stPrcInfo);
	}

	public static int IMVS_GetParamValue(IntPtr handle, uint nModuleID, string strName, int nStrValueSize, ref string strValue)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(nStrValueSize);
		int num = NativeMethods.IMVS_GetParamValue(handle, nModuleID, strName, (uint)nStrValueSize, intPtr);
		if (num == 0)
		{
			strValue = (string)UTF8Marshaler.GetInstance("cookie").MarshalNativeToManaged(intPtr);
		}
		Marshal.FreeHGlobal(intPtr);
		return num;
	}

	public static int IMVS_GetParamValue(IntPtr handle, uint nModuleID, string strName, ref string strValue)
	{
		return IMVS_GetParamValue(handle, nModuleID, strName, 4096, ref strValue);
	}

	public static int IMVS_SetParamValue(IntPtr handle, uint nModuleID, string strName, string strValue)
	{
		return NativeMethods.IMVS_SetParamValue(handle, nModuleID, strName, strValue);
	}

	public static int IMVS_SetGlobalConfig(IntPtr handle, uint nModuleID, string strName, string strValue)
	{
		return NativeMethods.IMVS_SetGlobalConfig(handle, nModuleID, strName, strValue);
	}

	public static int IMVS_GetGlobalConfig(IntPtr handle, uint nModuleID, string strName, ref string strValue)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(4096);
		int num = NativeMethods.IMVS_GetGlobalConfig(handle, nModuleID, strName, 4096u, intPtr);
		if (num == 0)
		{
			strValue = (string)UTF8Marshaler.GetInstance("cookie").MarshalNativeToManaged(intPtr);
		}
		Marshal.FreeHGlobal(intPtr);
		return num;
	}

	public static int IMVS_SetParamValueForImageSource(IntPtr handle, uint nModuleID, string strParam, string strPath)
	{
		return NativeMethods.IMVS_SetParamValueForImageSource(handle, nModuleID, strParam, strPath);
	}

	public static int IMVS_DeleteOneProcess(IntPtr handle, uint nProcessID)
	{
		return NativeMethods.IMVS_DeleteOneProcess(handle, nProcessID);
	}

	public static int IMVS_ExecuteOnce4GlobalScript(IntPtr handle)
	{
		return NativeMethods.IMVS_ExecuteOnce4GlobalScript(handle);
	}

	public static int IMVS_ContinousRunStart4GlobalScript(IntPtr handle)
	{
		return NativeMethods.IMVS_ContinousRunStart4GlobalScript(handle);
	}

	public static int IMVS_StopExecute4GlobalScript(IntPtr handle)
	{
		return NativeMethods.IMVS_StopExecute4GlobalScript(handle);
	}

	public static int IMVS_GetModuleListInProcess(IntPtr handle, uint nProcessID, IntPtr pModuList)
	{
		return NativeMethods.IMVS_GetModuleListInProcess(handle, nProcessID, pModuList);
	}

	public static int IMVS_GetModuleListByType(IntPtr handle, uint nModuleType, IntPtr pstModuleInfoList)
	{
		return NativeMethods.IMVS_GetModuleListByType(handle, nModuleType, pstModuleInfoList);
	}

	public static int IMVS_GetTakeoverStatus(IntPtr handle, ref ImvsSdkDefine.IMVS_STATUS_TAKEOVER_INFO takeoverInfo)
	{
		return NativeMethods.IMVS_GetTakeoverStatus(handle, ref takeoverInfo);
	}

	public static int IMVS_GetPlatformDetailedInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_PLATFORM_DETAILED_INFO platformInfo)
	{
		return NativeMethods.IMVS_GetPlatformDetailedInfo(handle, ref platformInfo);
	}

	public static int IMVS_GetAutoLoadSolution(IntPtr handle, ref ImvsSdkDefine.IMVS_AUTO_LOAD_SOLUTION_INFO solInfo)
	{
		return NativeMethods.IMVS_GetAutoLoadSolution(handle, ref solInfo);
	}

	public static int IMVS_SetAutoLoadSolution(IntPtr handle, ref ImvsSdkDefine.IMVS_AUTO_LOAD_SOLUTION_INFO solInfo)
	{
		return NativeMethods.IMVS_SetAutoLoadSolution(handle, ref solInfo);
	}

	public static int IMVS_ShieldModuleCbResult(IntPtr handle, uint shieldFlag = 0u)
	{
		return NativeMethods.IMVS_ShieldModuleCbResult(handle, shieldFlag);
	}

	public static int IMVS_ShieldProcess(IntPtr handle, uint nProcessID, uint nShieldFlag)
	{
		return NativeMethods.IMVS_ShieldProcess(handle, nProcessID, nShieldFlag);
	}

	public static int IMVS_GetProcedureShieldGlobalCtrl(IntPtr handle, uint nProcessID, ref uint nShieldFlag)
	{
		return NativeMethods.IMVS_GetProcedureShieldGlobalCtrl(handle, nProcessID, ref nShieldFlag);
	}

	public static int IMVS_CopyModuleParam(IntPtr handle, uint nSrcModuleId, uint nDstModuleId)
	{
		return NativeMethods.IMVS_CopyModuleParam(handle, nSrcModuleId, nDstModuleId);
	}

	public static int IMVS_SetServerReadyEvent(IntPtr handle)
	{
		return NativeMethods.IMVS_SetServerReadyEvent(handle);
	}

	public static int IMVS_MakeModulesPrepared(IntPtr handle, ImvsSdkDefine.IMVS_CAMERA_PIC_INFO_LIST stCameraInfoList)
	{
		ImvsSdkDefine.IMVS_CAMERA_PIC_INFO_LIST structure = new ImvsSdkDefine.IMVS_CAMERA_PIC_INFO_LIST
		{
			stCamPicInfoList = new ImvsSdkDefine.IMVS_CAMERA_PIC_INFO[256],
			nNum = stCameraInfoList.nNum,
			nIsAllModuRun = stCameraInfoList.nIsAllModuRun
		};
		int num = 0;
		if (stCameraInfoList.stCamPicInfoList != null && stCameraInfoList.nNum > 0)
		{
			for (num = 0; num < stCameraInfoList.nNum; num++)
			{
				structure.stCamPicInfoList[num] = stCameraInfoList.stCamPicInfoList[num];
			}
		}
		if (stCameraInfoList.nReserved != null)
		{
			for (num = 0; num < 4; num++)
			{
				structure.nReserved[num] = stCameraInfoList.nReserved[num];
			}
		}
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_CAMERA_PIC_INFO_LIST)));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_MakeModulesPreparedBySelfRun(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_SetProcedureRunPolicy(IntPtr handle, ImvsSdkDefine.IMVS_PROCEDURE_RUN_POLICY stProcRunPolicyInfo)
	{
		return NativeMethods.IMVS_SetProcedureRunPolicy(handle, ref stProcRunPolicyInfo);
	}

	public static int IMVS_GetProcedureRunPolicy(IntPtr handle, ref ImvsSdkDefine.IMVS_PROCEDURE_RUN_POLICY stProcRunPolicyInfo)
	{
		return NativeMethods.IMVS_GetProcedureRunPolicy(handle, ref stProcRunPolicyInfo);
	}

	public static int IMVS_GetSolutionPath(IntPtr handle, uint nPathSize, IntPtr strSolPath, ref uint nRealPathSize)
	{
		return NativeMethods.IMVS_GetSolutionPath(handle, nPathSize, strSolPath, ref nRealPathSize);
	}

	public static int IMVS_GetProcessorInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_PROCESSOR_INFO stProcessorInfo)
	{
		return NativeMethods.IMVS_GetProcessorInfo(handle, ref stProcessorInfo);
	}

	public static int IMVS_CopyModuleAlgParam(IntPtr handle, uint nSrcModuID, uint nDstModuID)
	{
		return NativeMethods.IMVS_CopyModuleAlgParam(handle, nSrcModuID, nDstModuID);
	}

	public static int IMVS_SetDefaultParamList(IntPtr handle, uint nModuleID)
	{
		return NativeMethods.IMVS_SetDefaultParamList(handle, nModuleID);
	}

	public static int IMVS_SetGroupIO(IntPtr handle, ImvsSdkDefine.IMVS_GROUP_IO_INFO_LIST stGroupIOInfoList)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_GROUP_IO_INFO_LIST)));
		Marshal.StructureToPtr(stGroupIOInfoList, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_SetGroupIO(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_SetProcedureIO(IntPtr handle, ImvsSdkDefine.IMVS_PROCEDURE_IO_INFO_LIST stPrcIOInfoList)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_PROCEDURE_IO_INFO_LIST)));
		Marshal.StructureToPtr(stPrcIOInfoList, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_SetProcedureIO(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_BindGlobalVarParam(IntPtr handle, ImvsSdkDefine.IMVS_BIND_GLOBALVAR_PARAM_INFO_LIST stGlobalVarInfo)
	{
		ImvsSdkDefine.IMVS_BIND_GLOBALVAR_PARAM_INFO_LIST structure = new ImvsSdkDefine.IMVS_BIND_GLOBALVAR_PARAM_INFO_LIST
		{
			astBindGlobalVarList = new ImvsSdkDefine.IMVS_BIND_GLOBALVAR_PARAM_INFO[128],
			nNum = stGlobalVarInfo.nNum
		};
		if (stGlobalVarInfo.astBindGlobalVarList != null)
		{
			for (int i = 0; i < stGlobalVarInfo.nNum; i++)
			{
				structure.astBindGlobalVarList[i] = stGlobalVarInfo.astBindGlobalVarList[i];
			}
		}
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_BIND_GLOBALVAR_PARAM_INFO_LIST)));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_BindGlobalVarParam(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_UnBindGlobalVarParam(IntPtr handle, ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST stGlobalVarInfo)
	{
		ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST structure = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST
		{
			astBindGlobalVarList = new ImvsSdkDefine.IMVS_UNBIND_GLOBALVAR_PARAM_INFO[128],
			nNum = stGlobalVarInfo.nNum
		};
		if (stGlobalVarInfo.astBindGlobalVarList != null)
		{
			for (int i = 0; i < stGlobalVarInfo.nNum; i++)
			{
				structure.astBindGlobalVarList[i] = stGlobalVarInfo.astBindGlobalVarList[i];
			}
		}
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_BIND_GLOBALVAR_PARAM_INFO_LIST)));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_UnBindGlobalVarParam(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_GetBindGlobalVarParam(IntPtr handle, uint nModuleId, int nIndex, int nQueNum, ref ImvsSdkDefine.IMVS_GET_BIND_GLOBALVAR_PARAM_INFO_LIST stGlobalVarInfo)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_GET_BIND_GLOBALVAR_PARAM_INFO_LIST)));
		Marshal.StructureToPtr(stGlobalVarInfo, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_GetBindGlobalVarParam(handle, nModuleId, nIndex, nQueNum, intPtr);
		stGlobalVarInfo = (ImvsSdkDefine.IMVS_GET_BIND_GLOBALVAR_PARAM_INFO_LIST)Marshal.PtrToStructure(intPtr, typeof(ImvsSdkDefine.IMVS_GET_BIND_GLOBALVAR_PARAM_INFO_LIST));
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_SetModuleParamBinding(IntPtr handle, ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST stModuleVarInfo)
	{
		ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST structure = new ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST
		{
			astBindModuVarList = new ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO[128],
			nNum = stModuleVarInfo.nNum
		};
		if (stModuleVarInfo.astBindModuVarList != null)
		{
			for (int i = 0; i < stModuleVarInfo.nNum; i++)
			{
				structure.astBindModuVarList[i] = stModuleVarInfo.astBindModuVarList[i];
			}
		}
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST)));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_SetModuleParamBinding(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_DelModuleParamBinding(IntPtr handle, ImvsSdkDefine.IMVS_UNBIND_MODULE_PARAM_INFO_LIST stModuleVarInfo)
	{
		return NativeMethods.IMVS_DelModuleParamBinding(handle, ref stModuleVarInfo);
	}

	public static int IMVS_GetModuleParamBindingList(IntPtr handle, uint nModuleId, int nIndex, int nQueNum, ref ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST stModuleVarInfo)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST)));
		Marshal.StructureToPtr(stModuleVarInfo, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_GetModuleParamBindingList(handle, nModuleId, nIndex, nQueNum, intPtr);
		stModuleVarInfo = (ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST)Marshal.PtrToStructure(intPtr, typeof(ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST));
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	public static int IMVS_LightSaveSolution(IntPtr handle, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_LightSaveSolution(handle, nRecvWaitTime);
	}

	public static int IMVS_NotifyLoadSolutionEnd(IntPtr handle, uint nClientType, int nErrorCode)
	{
		return NativeMethods.IMVS_NotifyLoadSolutionEnd(handle, nClientType, nErrorCode);
	}

	public static int IMVS_ExportProcess(IntPtr handle, uint nProcessId, string strPath, string strPassword)
	{
		return NativeMethods.IMVS_ExportProcess(handle, nProcessId, strPath, strPassword);
	}

	public static int IMVS_SyncExportProcess(IntPtr handle, uint nProcessId, string strPath, string strPassword)
	{
		return NativeMethods.IMVS_SyncExportProcess(handle, nProcessId, strPath, strPassword);
	}

	public static int IMVS_ImportProcess(IntPtr handle, string strPath, string strPassword, uint nIsIgnoreGlobal, uint nIdAllocateWay)
	{
		return NativeMethods.IMVS_ImportProcess(handle, strPath, strPassword, nIsIgnoreGlobal, nIdAllocateWay);
	}

	public static int IMVS_SyncImportProcess(IntPtr handle, string strPath, string strPassword, uint nIsIgnoreGlobal, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_PROCESS_INFO stProcessInfo)
	{
		return NativeMethods.IMVS_SyncImportProcess(handle, strPath, strPassword, nIsIgnoreGlobal, nIdAllocateWay, ref stProcessInfo);
	}

	public static int IMVS_SetProcRunParam(IntPtr handle, uint nProcessId, uint nStopAction)
	{
		return NativeMethods.IMVS_SetProcRunParam(handle, nProcessId, nStopAction);
	}

	public static int IMVS_GetProcRunParam(IntPtr handle, uint nProcessId, ref uint nStopAction)
	{
		return NativeMethods.IMVS_GetProcRunParam(handle, nProcessId, ref nStopAction);
	}

	public static int IMVS_GetObjectLimits(IntPtr handle, ref uint nMaxProcessNum, ref uint nMaxModuleNum)
	{
		return NativeMethods.IMVS_GetObjectLimits(handle, ref nMaxProcessNum, ref nMaxModuleNum);
	}

	public static int IMVS_SetMaxModuleNum(IntPtr handle, uint nMaxModuleNum)
	{
		return NativeMethods.IMVS_SetMaxModuleNum(handle, nMaxModuleNum);
	}

	public static int IMVS_GetCustomOutputSubscribeInfo(IntPtr handle, uint nProcessID, uint nIndexNum, IntPtr pstCustomSubInfoList)
	{
		return NativeMethods.IMVS_GetCustomOutputSubscribeInfo(handle, nProcessID, nIndexNum, pstCustomSubInfoList);
	}

	public static int IMVS_SetCustomOutputSubscribeInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_CUSTOM_SUBSCRIBE_INFO pstCustomSubInfo)
	{
		return NativeMethods.IMVS_SetCustomOutputSubscribeInfo(handle, ref pstCustomSubInfo);
	}

	public static int IMVS_GetAllProcedureShieldState(IntPtr handle, ref uint nShieldFlag)
	{
		return NativeMethods.IMVS_GetAllProcedureShieldState(handle, ref nShieldFlag);
	}

	public static int IMVS_GetPrcVersion(IntPtr handle, string strPath, string strPassWord, ref ImvsSdkDefine.IMVS_FILE_VERSION_INFO pstVersionInfo)
	{
		return NativeMethods.IMVS_GetPrcVersion(handle, strPath, strPassWord, ref pstVersionInfo);
	}

	public static int IMVS_GetGroVersion(IntPtr handle, string strPath, string strPassWord, ref ImvsSdkDefine.IMVS_FILE_VERSION_INFO pstVersionInfo)
	{
		return NativeMethods.IMVS_GetGroVersion(handle, strPath, strPassWord, ref pstVersionInfo);
	}

	public static int IMVS_SetModuInputIOInfo(IntPtr handle, uint nObjectId, IntPtr pstModuInfo)
	{
		return NativeMethods.IMVS_SetModuInputIOInfo(handle, nObjectId, pstModuInfo);
	}

	public static int IMVS_SetModuOutputIOInfo(IntPtr handle, uint nObjectId, IntPtr pstModuInfo)
	{
		return NativeMethods.IMVS_SetModuOutputIOInfo(handle, nObjectId, pstModuInfo);
	}

	public static int IMVS_GetModuInputIOInfo(IntPtr handle, uint nModuId, ref IntPtr pstModuIOInfo)
	{
		return NativeMethods.IMVS_GetModuInputIOInfo(handle, nModuId, ref pstModuIOInfo);
	}

	public static int IMVS_GetModuOutputIOInfo(IntPtr handle, uint nModuId, uint nDataType, ref IntPtr pstModuIOInfo)
	{
		return NativeMethods.IMVS_GetModuOutputIOInfo(handle, nModuId, nDataType, ref pstModuIOInfo);
	}

	public static int IMVS_AddDynamicParam(IntPtr handle, uint nObjectId, string strName)
	{
		return NativeMethods.IMVS_AddDynamicParam(handle, nObjectId, strName);
	}

	public static int IMVS_ModifyDynamicParam(IntPtr handle, uint nObjectId, string strNameOld, string strNameNew)
	{
		return NativeMethods.IMVS_ModifyDynamicParam(handle, nObjectId, strNameOld, strNameNew);
	}

	public static int IMVS_DeleteDynamicParam(IntPtr handle, uint nObjectId, string strName)
	{
		return NativeMethods.IMVS_DeleteDynamicParam(handle, nObjectId, strName);
	}

	public static int IMVS_GetAllDynamicParam(IntPtr handle, uint nObjectId, IntPtr pstParamList)
	{
		return NativeMethods.IMVS_GetAllDynamicParam(handle, nObjectId, pstParamList);
	}

	public static int IMVS_AddDynamicParamRelation(IntPtr handle, uint nDestinID, string strDestinName, uint nSourceID, string strSourceName)
	{
		return NativeMethods.IMVS_AddDynamicParamRelation(handle, nDestinID, strDestinName, nSourceID, strSourceName);
	}

	public static int IMVS_GetDynamicParamRelationByKey(IntPtr handle, uint nObjectID, string strName, IntPtr pstRelationList)
	{
		return NativeMethods.IMVS_GetDynamicParamRelationByKey(handle, nObjectID, strName, pstRelationList);
	}

	public static int IMVS_DeleteDynamicParamRelation(IntPtr handle, uint nDestinID, string strDestinName, uint nSourceID, string strSourceName)
	{
		return NativeMethods.IMVS_DeleteDynamicParamRelation(handle, nDestinID, strDestinName, nSourceID, strSourceName);
	}

	public static int IMVS_SetDynamicRunParamValue(IntPtr handle, uint nObjectId, string strName, string strValue, string strOptionalParam)
	{
		return NativeMethods.IMVS_SetDynamicRunParamValue(handle, nObjectId, strName, strValue, strOptionalParam);
	}

	public static int IMVS_GetDynamicRunParamValue(IntPtr handle, uint nObjectId, string strName, ref string strValue, string strOptionalParam)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(4096);
		int num = NativeMethods.IMVS_GetDynamicRunParamValue(handle, nObjectId, strName, 4096u, intPtr, strOptionalParam);
		if (num == 0)
		{
			strValue = (string)UTF8Marshaler.GetInstance("cookie").MarshalNativeToManaged(intPtr);
		}
		Marshal.FreeHGlobal(intPtr);
		return num;
	}

	public static int IMVS_SetDynamicBinaryParamValue(IntPtr handle, uint nObjectId, string strName, IntPtr pData, uint nDataLen, string strOptionalParam)
	{
		return NativeMethods.IMVS_SetDynamicBinaryParamValue(handle, nObjectId, strName, pData, nDataLen, strOptionalParam);
	}

	public static int IMVS_GetDynamicBinaryParamValue(IntPtr handle, uint nObjectId, string strName, ref byte[] data, ref uint nDataLen, string strOptionalParam)
	{
		int num = -536870657;
		IntPtr pData = IntPtr.Zero;
		num = NativeMethods.IMVS_GetDynamicBinaryParamValue(handle, nObjectId, strName, ref pData, ref nDataLen, strOptionalParam);
		if (num == 0)
		{
			data = new byte[nDataLen];
			if (IntPtr.Zero != pData && data != null)
			{
				try
				{
					Marshal.Copy(pData, data, 0, (int)nDataLen);
				}
				catch (Exception)
				{
				}
				num = IMVS_ReleaseDynamicMemory(handle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_BASE_ARRAY, pData);
				if (num != 0)
				{
					return num;
				}
				pData = IntPtr.Zero;
			}
		}
		return num;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectId"></param>
	/// <param name="strName"></param>
	/// <param name="pData"></param>
	/// <param name="nDataLen"></param>
	/// <param name="strOptionalParam"></param>
	/// <returns></returns>
	public static int IMVS_SetDynamicUIParamValue(IntPtr handle, uint nObjectId, string strName, IntPtr pData, uint nDataLen, string strOptionalParam)
	{
		return NativeMethods.IMVS_SetDynamicUIParamValue(handle, nObjectId, strName, pData, nDataLen, strOptionalParam);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectId"></param>
	/// <param name="strName"></param>
	/// <param name="data"></param>
	/// <param name="nDataLen"></param>
	/// <param name="strOptionalParam"></param>
	/// <returns></returns>
	public static int IMVS_GetDynamicUIParamValue(IntPtr handle, uint nObjectId, string strName, ref byte[] data, ref uint nDataLen, string strOptionalParam)
	{
		int num = -536870657;
		IntPtr pData = IntPtr.Zero;
		num = NativeMethods.IMVS_GetDynamicUIParamValue(handle, nObjectId, strName, ref pData, ref nDataLen, strOptionalParam);
		if (num == 0)
		{
			data = new byte[nDataLen];
			if (IntPtr.Zero != pData && data != null)
			{
				try
				{
					Marshal.Copy(pData, data, 0, (int)nDataLen);
				}
				catch (Exception)
				{
				}
				num = NativeMethods.IMVS_ReleaseDynamicMemory(handle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_BASE_ARRAY, pData);
				if (num != 0)
				{
					return num;
				}
				pData = IntPtr.Zero;
			}
		}
		return num;
	}

	/// <summary>
	/// 删除动态关联的Userdata
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectId">对象ID</param>
	/// <param name="strName">对象Key</param>
	/// <param name="eFuzzyMatchType">是否模糊匹配</param>
	/// <param name="strModuParamName">模块参数名称（不可为空）</param>
	/// <returns></returns>
	public static int IMVS_DeleteDynamicRelateUIParamValue(IntPtr handle, uint nObjectId, string strName, ImvsSdkDefine.IMVS_FUZZY_MATCH eFuzzyMatchType, string strModuParamName)
	{
		return NativeMethods.IMVS_DeleteDynamicRelateUIParamValue(handle, nObjectId, strName, eFuzzyMatchType, strModuParamName);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="eMemType"></param>
	/// <param name="pData"></param>
	/// <returns></returns>
	public static int IMVS_ReleaseDynamicMemory(IntPtr handle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE eMemType, IntPtr pData)
	{
		return NativeMethods.IMVS_ReleaseDynamicMemory(handle, eMemType, pData);
	}

	/// <summary>
	/// 获取方案中所有可执行流程列表
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pstProcessInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetAllExecutableProcedure(IntPtr handle, IntPtr pstProcessInfoList)
	{
		return NativeMethods.IMVS_GetAllExecutableProcedure(handle, pstProcessInfoList);
	}

	public static int IMVS_CheckDataEncryptByDongle(IntPtr handle, IntPtr pData, uint nDataLen, ref bool bIsEncrypted)
	{
		return NativeMethods.IMVS_CheckDataEncryptByDongle(handle, pData, nDataLen, ref bIsEncrypted);
	}

	public static int IMVS_GetEncryptionFromDongle(IntPtr handle, IntPtr pKeyBuff, uint nBuffSize, ref bool bIsContainKey)
	{
		return NativeMethods.IMVS_GetEncryptionFromDongle(handle, pKeyBuff, nBuffSize, ref bIsContainKey);
	}

	public static int IMVS_DataEncryptByDongle(IntPtr handle, IntPtr pDataIn, uint nDataLenIn, IntPtr pKeyBuff, IntPtr pDataOut, uint nDataSizeOut, ref uint nDataLenOut)
	{
		return NativeMethods.IMVS_DataEncryptByDongle(handle, pDataIn, nDataLenIn, pKeyBuff, pDataOut, nDataSizeOut, ref nDataLenOut);
	}

	public static int IMVS_DataDecryptByDongle(IntPtr handle, IntPtr pDataIn, uint nDataLenIn, IntPtr pKeyBuff, IntPtr pDataOut, uint nDataSizeOut, ref uint nDataLenOut)
	{
		return NativeMethods.IMVS_DataDecryptByDongle(handle, pDataIn, nDataLenIn, pKeyBuff, pDataOut, nDataSizeOut, ref nDataLenOut);
	}

	public static int AddContainerRelateModule(IntPtr handle, ref ImvsSdkDefine.IMVS_CONTAINER_RELATE_MODULE_INFO pstRelateModuleInfo)
	{
		return NativeMethods.IMVS_AddContainerRelateModule(handle, ref pstRelateModuleInfo);
	}

	public static int GetContainerRelateModuleInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_CONTAINER_RELATE_MODULE_INFO pstRelateModuleInfo)
	{
		return NativeMethods.IMVS_GetContainerRelateModuleInfo(handle, ref pstRelateModuleInfo);
	}

	public static int GetDongleSeries(IntPtr handle, ref int nDongleSeries)
	{
		return NativeMethods.IMVS_GetDongleSeries(handle, ref nDongleSeries);
	}

	public static int SetVarReportState(IntPtr handle, uint nModuleID, int nState)
	{
		return NativeMethods.IMVS_SetVarReportState(handle, nModuleID, nState);
	}

	public static int IMVS_GetModuErrorInfoList(IntPtr handle, uint nObjectID, ref ImvsSdkDefine.IMVS_MODULE_RUN_ERROR_INFO_LIST pstErrorInfoList)
	{
		return NativeMethods.IMVS_GetModuErrorInfoList(handle, nObjectID, ref pstErrorInfoList);
	}

	public static int IMVS_SetSaveImageModuBigFile(IntPtr handle, uint nModuleID, string strPath, bool bIsEnableSetBigFile)
	{
		return NativeMethods.IMVS_SetSaveImageModuBigFile(handle, nModuleID, strPath, bIsEnableSetBigFile);
	}

	public static int IMVS_GetSaveImageModuBigFile(IntPtr handle, uint nModuleID, ref string strPath, ref bool bIsEnableSetBigFile)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(260);
		int num = NativeMethods.IMVS_GetSaveImageModuBigFile(handle, nModuleID, intPtr, ref bIsEnableSetBigFile);
		if (num == 0)
		{
			strPath = (string)UTF8Marshaler.GetInstance("cookie").MarshalNativeToManaged(intPtr);
		}
		Marshal.FreeHGlobal(intPtr);
		return num;
	}

	public static int IMVS_SetGroupIOReportMode(IntPtr handle, IntPtr pstGroupIOInfoList)
	{
		return NativeMethods.IMVS_SetGroupIOReportMode(handle, pstGroupIOInfoList);
	}

	public static int IMVS_GetGroupIOReportMode(IntPtr handle, IntPtr pstGroupIOInfoList)
	{
		return NativeMethods.IMVS_GetGroupIOReportMode(handle, pstGroupIOInfoList);
	}

	/// <summary>
	/// 设置对象界面参数
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectID"></param>
	/// <param name="strParamName"></param>
	/// <param name="pData"></param>
	/// <param name="nDataLen"></param>
	/// <returns></returns>
	public static int IMVS_SetUIParamValue(IntPtr handle, uint nObjectID, string strParamName, IntPtr pData, uint nDataLen, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_SetUIParamValue(handle, nObjectID, strParamName, pData, nDataLen, nRecvWaitTime);
	}

	/// <summary>
	/// 检查数据是否存在
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectID"></param>
	/// <param name="strParamName"></param>
	/// <returns></returns>
	public static int IMVS_CheckUIParamExist(IntPtr handle, uint nObjectID, string strParamName)
	{
		return NativeMethods.IMVS_CheckUIParamExist(handle, nObjectID, strParamName);
	}

	/// <summary>
	/// 根据对象Key获取对象界面参数值，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectID"></param>
	/// <param name="strParamName"></param>
	/// <param name="pData"></param>
	/// <param name="nDataLen"></param>
	/// <returns></returns>
	public static int IMVS_GetUIParamValueByKey(IntPtr handle, uint nObjectID, string strParamName, ref IntPtr pData, ref uint nDataLen, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_GetUIParamValueByKey(handle, nObjectID, strParamName, ref pData, ref nDataLen, nRecvWaitTime);
	}

	/// <summary>
	/// 根据对象Key获取对象界面参数值，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="astModuleId"></param>
	/// <param name="strParamName"></param>
	/// <param name="bIsFuzzyQuery"></param>
	/// <param name="pUiParamInfoList"></param>
	/// <returns></returns>
	public static int IMVS_FuzzyQueryUIParamValue(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST astModuleId, string strParamName, bool bIsFuzzyQuery, ref IntPtr pUiParamInfoList)
	{
		return NativeMethods.IMVS_FuzzyQueryUIParamValue(handle, ref astModuleId, strParamName, bIsFuzzyQuery, ref pUiParamInfoList);
	}

	/// <summary>
	/// 删除对象中所有界面参数
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectID"></param>
	/// <returns></returns>
	public static int IMVS_DeleteAllUIParamValue(IntPtr handle, uint nObjectID)
	{
		return NativeMethods.IMVS_DeleteAllUIParamValue(handle, nObjectID);
	}

	/// <summary>
	/// 按对象获取对象界面参数值，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectID"></param>
	/// <param name="pUIObjParamInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetUIParamValueByObject(IntPtr handle, uint nObjectID, ref IntPtr pUIObjParamInfoList)
	{
		return NativeMethods.IMVS_GetUIParamValueByObject(handle, nObjectID, ref pUIObjParamInfoList);
	}

	/// <summary>
	/// 获取流程内部的模块树
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nObjectID"></param>
	/// <param name="nObjectType"></param>
	/// <param name="pNodeTreeInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetFirstLayerModuleTree(IntPtr handle, uint nObjectID, uint nObjectType, IntPtr pNodeTreeInfoList)
	{
		return NativeMethods.IMVS_GetFirstLayerModuleTree(handle, nObjectID, nObjectType, pNodeTreeInfoList);
	}

	/// <summary>
	/// 获取流程内模块连接关系
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nProcessID"></param>
	/// <param name="nObjectType"></param>
	/// <param name="pNodeTreeInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetFirstLayerModuleConnect(IntPtr handle, uint nProcessID, uint nObjectType, ref IntPtr pModuConnectInfoList)
	{
		return NativeMethods.IMVS_GetFirstLayerModuleConnect(handle, nProcessID, nObjectType, ref pModuConnectInfoList);
	}

	/// <summary>
	/// 获取流程内模块订阅关系
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nProcessID"></param>
	/// <param name="nObjectType"></param>
	/// <param name="pNodeTreeInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetFirstLayerModuleSubscribe(IntPtr handle, uint nProcessID, uint nObjectType, ref IntPtr pModuSubInfoList)
	{
		return NativeMethods.IMVS_GetFirstLayerModuleSubscribe(handle, nProcessID, nObjectType, ref pModuSubInfoList);
	}

	/// <summary>
	/// 释放获取数据内存
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static int IMVS_ReleaseUIParamData(IntPtr handle)
	{
		return NativeMethods.IMVS_ReleaseUIParamData(handle);
	}

	/// <summary>
	/// 上传指定模块结果
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pReportModuInfoList"></param>
	/// <returns></returns>
	public static int IMVS_SetReportModuleResult(IntPtr handle, IntPtr pReportModuInfoList)
	{
		return NativeMethods.IMVS_SetReportModuleResult(handle, pReportModuInfoList);
	}

	/// <summary>
	/// 上传指定模块结果（二次开发专用）
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nMode"></param>
	/// <param name="nModuId"></param>
	/// <param name="bIsEnable"></param>
	/// <returns></returns>
	public static int IMVS_SetReportModuleResult_V2(IntPtr handle, ImvsSdkDefine.IMVS_REPORT_RESULT_TYPE nMode, uint nModuId = 0u, bool bIsEnable = false)
	{
		return NativeMethods.IMVS_SetReportModuleResult_V2(handle, nMode, nModuId, bIsEnable);
	}

	/// <summary>
	/// 上传所有模块结果
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static int IMVS_SetAllReportModuleResult(IntPtr handle)
	{
		return NativeMethods.IMVS_SetAllReportModuleResult(handle);
	}

	/// <summary>
	/// 停止上传所有模块结果
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static int IMVS_SetNoneReportModuleResult(IntPtr handle)
	{
		return NativeMethods.IMVS_SetNoneReportModuleResult(handle);
	}

	/// <summary>
	/// 保持模块上次结果
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nModuleID"></param>
	/// <param name="bIsUnchanged"></param>
	/// <returns></returns>
	public static int IMVS_KeepModuleLastResult(IntPtr handle, uint nModuleID, bool bIsUnchanged)
	{
		return NativeMethods.IMVS_KeepModuleLastResult(handle, nModuleID, bIsUnchanged);
	}

	/// <summary>
	/// 查询模块结果状态，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="handle"></param>
	/// <returns></returns>
	public static int IMVS_QueryModuleRunningState(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, ref IntPtr pQueryModuStaInfoList)
	{
		return NativeMethods.IMVS_QueryModuleRunningState(handle, ref pastModuleId, ref pQueryModuStaInfoList);
	}

	/// <summary>
	/// 查询模块历史结果
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nModuleID"></param>
	/// <param name="pQueryModuResInfoList"></param>
	/// <returns></returns>
	public static int IMVS_QueryModuleHistoryResult(IntPtr handle, uint nModuleID, int nIndex, uint nRequestNum, ref IntPtr pQueryModuResInfoList, bool bResBuffFirst = true, bool bNeedImage = true, bool bUseResStructMap = true, bool bQueryByRunCount = false, uint nRunCount = 0u)
	{
		return NativeMethods.IMVS_QueryModuleHistoryResult(handle, nModuleID, nIndex, nRequestNum, ref pQueryModuResInfoList, bResBuffFirst, bNeedImage, bUseResStructMap, bQueryByRunCount, nRunCount);
	}

	/// <summary>
	/// 导出算子
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pastModuleId"></param>
	/// <param name="pDataBuff"></param>
	/// <param name="nBuffSize"></param>
	/// <param name="nDataLen"></param>
	/// <returns></returns>
	public static int IMVS_GetDataForExportOperator(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, IntPtr pDataBuff, uint nBuffSize, ref uint nDataLen)
	{
		return NativeMethods.IMVS_GetDataForExportOperator(handle, ref pastModuleId, pDataBuff, nBuffSize, ref nDataLen);
	}

	/// <summary>
	/// 获取模块导出数据，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nModuleID"></param>
	/// <param name="strModuDisplayName"></param>
	/// <param name="nWaitTime"></param>
	/// <param name="pstModuExportData"></param>
	/// <returns></returns>
	public static int IMVS_GetModuleExportData(IntPtr handle, uint nModuleID, string strModuDisplayName, uint nWaitTime, ref IntPtr pstModuExportData)
	{
		return NativeMethods.IMVS_GetModuleExportData(handle, nModuleID, strModuDisplayName, nWaitTime, ref pstModuExportData);
	}

	/// <summary>
	/// 导入模块数据
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pstImportData"></param>
	/// <returns></returns>
	public static int IMVS_ImportModuleData(IntPtr handle, ImvsSdkDefine.IMVS_IMPORT_MODULE_DATA_INPUT pstImportData)
	{
		return NativeMethods.IMVS_ImportModuleData(handle, ref pstImportData);
	}

	/// <summary>
	/// 获取模块导出数据
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="pstModuImportData"></param>
	/// <returns></returns>
	public static int IMVS_ImportModuleDataEx(IntPtr handle, ImvsSdkDefine.IMVS_IMPORT_MODULE_DATA_INFO pstModuImportData, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo)
	{
		return NativeMethods.IMVS_ImportModuleDataEx(handle, ref pstModuImportData, ref pstModuleInfo);
	}

	/// <summary>
	/// 启动全局脚本
	/// </summary>
	/// <returns></returns>
	public static int IMVS_StartGlobalScript(IntPtr handle, uint nTransDataSize, IntPtr strTransData)
	{
		return NativeMethods.IMVS_StartGlobalScript(handle, nTransDataSize, strTransData);
	}

	/// <summary>
	/// 添加Group子模块
	/// </summary>
	/// <returns></returns>
	public static int IMVS_AddGroupSonModule(IntPtr handle, ImvsSdkDefine.IMVS_GROUP_MODULE_INFO pstGroupModuInfo)
	{
		return NativeMethods.IMVS_AddGroupSonModule(handle, ref pstGroupModuInfo);
	}

	/// <summary>
	/// 移除Group子模块
	/// </summary>
	/// <returns></returns>
	public static int IMVS_RemoveGroupSonModule(IntPtr handle, ImvsSdkDefine.IMVS_GROUP_MODULE_INFO pstGroupModuInfo)
	{
		return NativeMethods.IMVS_RemoveGroupSonModule(handle, ref pstGroupModuInfo);
	}

	/// <summary>
	/// 删除Group及其子模块
	/// </summary>
	/// <returns></returns>
	public static int IMVS_DeleteGroupModule(IntPtr handle, uint nGroupModuleID)
	{
		return NativeMethods.IMVS_DeleteGroupModule(handle, nGroupModuleID);
	}

	/// <summary>
	/// 导出Group
	/// </summary>
	/// <returns></returns>
	public static int IMVS_ExportGroupModule(IntPtr handle, uint nGroupId, uint nGroupLevel, string strPath, string strPassword)
	{
		return NativeMethods.IMVS_ExportGroupModule(handle, nGroupId, nGroupLevel, strPath, strPassword);
	}

	/// <summary>
	/// 同步导出Group
	/// </summary>
	/// <returns></returns>
	public static int IMVS_SyncExportGroupModule(IntPtr handle, uint nGroupId, uint nGroupLevel, string strPath, string strPassword)
	{
		return NativeMethods.IMVS_SyncExportGroupModule(handle, nGroupId, nGroupLevel, strPath, strPassword);
	}

	/// <summary>
	/// 导入Group
	/// </summary>
	/// <param name="nIdAllocateWay"> ID分配方式 (0：自动分配ID，1：按文件中的ID分配) </param>
	/// <returns></returns>
	public static int IMVS_ImportGroupModule(IntPtr handle, uint nProcessID, uint nGroupID, string strPath, string strPassword, uint nIdAllocateWay)
	{
		return NativeMethods.IMVS_ImportGroupModule(handle, nProcessID, nGroupID, strPath, strPassword, nIdAllocateWay);
	}

	/// <summary>
	/// 版本兼容工具标识
	/// </summary>
	/// <param name="bToolFlag"> ID分配方式 (true：工具，false：非工具) </param>
	/// <returns></returns>
	public static int IMVS_VersionUpgradeFlag(IntPtr handle, bool bToolFlag)
	{
		return NativeMethods.IMVS_VersionUpgradeFlag(handle, bToolFlag);
	}

	/// <summary>
	/// 导入Group
	/// </summary>
	/// <param name="nIdAllocateWay"> ID分配方式 (0：自动分配ID，1：按文件中的ID分配) </param>
	/// <returns></returns>
	public static int IMVS_ImportIndependentGroup(IntPtr handle, uint nProcessID, string strPath, string strPassword, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo)
	{
		return NativeMethods.IMVS_ImportIndependentGroup(handle, nProcessID, strPath, strPassword, nIdAllocateWay, ref pstModuleInfo);
	}

	/// <summary>
	/// 同步导入Group
	/// </summary>
	/// <param name="nIdAllocateWay"> ID分配方式 (0：自动分配ID，1：按文件中的ID分配) </param>
	/// <returns></returns>
	public static int IMVS_SyncImportGroupModule(IntPtr handle, uint nProcessID, uint nGroupID, string strPath, string strPassword, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo)
	{
		return NativeMethods.IMVS_SyncImportGroupModule(handle, nProcessID, nGroupID, strPath, strPassword, nIdAllocateWay, ref pstModuleInfo);
	}

	/// <summary>
	/// 获取二进制数据长度
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="strValue"></param>
	/// <returns></returns>
	public static int IMVS_GetBinaryLength(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_GET_BINARY_LENGTH_INFO strValue, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_GetBinaryLength(handle, nModuleID, ref strValue, nRecvWaitTime);
	}

	/// <summary>
	/// 获取二进制数据
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="strValue"></param>
	/// <returns></returns>
	public static int IMVS_GetBinaryData(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_GetBinaryData(handle, nModuleID, ref strValue, nRecvWaitTime);
	}

	/// <summary>
	/// 获取二进制数据(接口内部申请内存)
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="strValue"></param>
	/// <returns></returns>
	public static int IMVS_GetBinaryDataEx(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_GetBinaryDataEx(handle, nModuleID, ref strValue, nRecvWaitTime);
	}

	/// <summary>
	/// 设置二进制数据
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="strValue"></param>
	/// <returns></returns>
	public static int IMVS_SetBinaryData(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO strValue, uint nRecvWaitTime)
	{
		return NativeMethods.IMVS_SetBinaryData(handle, nModuleID, ref strValue, nRecvWaitTime);
	}

	/// <summary>
	/// 设置整型输入
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="pstIntListInfo"></param>
	/// <returns></returns>
	public static int IMVS_SetInputInt(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_INPUT_INT_LIST_INFO pstIntListInfo)
	{
		return NativeMethods.IMVS_SetInputInt(handle, nModuleID, ref pstIntListInfo);
	}

	/// <summary>
	/// 设置浮点型输入
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="pstFloatListInfo"></param>
	/// <returns></returns>
	public static int IMVS_SetInputFloat(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_INPUT_FLOAT_LIST_INFO pstFloatListInfo)
	{
		return NativeMethods.IMVS_SetInputFloat(handle, nModuleID, ref pstFloatListInfo);
	}

	/// <summary>
	/// 设置字符串型输入
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="pstStringListInfo"></param>
	/// <returns></returns>
	public static int IMVS_SetInputString(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_INPUT_STRING_LIST_INFO pstStringListInfo)
	{
		return NativeMethods.IMVS_SetInputString(handle, nModuleID, ref pstStringListInfo);
	}

	/// <summary>
	/// 设置图像型输入
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="pstImageInfo"></param>
	/// <returns></returns>
	public static int IMVS_SetInputImage(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_MODULE_INPUT_DATA_INFO pstImageInfo)
	{
		return NativeMethods.IMVS_SetInputImage(handle, nModuleID, ref pstImageInfo);
	}

	/// <summary>
	/// 设置点集型输入
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="pstPointsetInfo"></param>
	/// <returns></returns>
	public static int IMVS_SetInputPointset(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_MODULE_INPUT_DATA_INFO pstPointsetInfo)
	{
		return NativeMethods.IMVS_SetInputPointset(handle, nModuleID, ref pstPointsetInfo);
	}

	/// <summary>
	/// 设置二进制型输入
	/// </summary>
	/// <param name="nModuleID"> 模块ID </param>
	/// <param name="pstBytesInfo"></param>
	/// <returns></returns>
	public static int IMVS_SetInputBytes(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_MODULE_INPUT_DATA_INFO pstBytesInfo)
	{
		return NativeMethods.IMVS_SetInputBytes(handle, nModuleID, ref pstBytesInfo);
	}

	/// <summary>
	/// 删除对象UI参数
	/// </summary>
	/// <param name="nObjectID"> 对象ID </param>
	/// <param name="strParamName"></param>
	/// <param name="nBlurDelete">是否模糊删除（0：否，1：是）</param>
	/// <returns></returns>
	public static int IMVS_DeleteUIParamValue(IntPtr handle, uint nObjectID, string strParamName, uint nBlurDelete)
	{
		return NativeMethods.IMVS_DeleteUIParamValue(handle, nObjectID, strParamName, nBlurDelete);
	}

	/// <summary>
	/// 导出多个模块数据，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="pastModuleId"> 模块ID列表 </param>
	/// <param name="nWaitTime"></param>
	/// <returns></returns>
	public static int IMVS_ExportMultiModuleData(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, uint nWaitTime, ref IntPtr pstModuExportData)
	{
		return NativeMethods.IMVS_ExportMultiModuleData(handle, ref pastModuleId, nWaitTime, ref pstModuExportData);
	}

	/// <summary>
	/// 获取多个模块连接关系，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="pastModuleId"> 模块ID列表 </param>
	/// <param name="pstModuConnectInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetMultiModuleConnect(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, ref IntPtr pstModuConnectInfoList)
	{
		return NativeMethods.IMVS_GetMultiModuleConnect(handle, ref pastModuleId, ref pstModuConnectInfoList);
	}

	/// <summary>
	/// 获取多个模块订阅关系，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="pastModuleId"> 模块ID列表 </param>
	/// <param name="pstModuSubInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetMultiModuleSubscribe(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, ref IntPtr pstModuSubInfoList)
	{
		return NativeMethods.IMVS_GetMultiModuleSubscribe(handle, ref pastModuleId, ref pstModuSubInfoList);
	}

	/// <summary>
	/// 获取所有全局模块列表
	/// </summary>
	/// <param name="pstModuleInfoList"> 模块信息列表 </param>
	/// <returns></returns>
	public static int IMVS_GetAllGlobalModule(IntPtr handle, IntPtr pstModuleInfoList)
	{
		return NativeMethods.IMVS_GetAllGlobalModule(handle, pstModuleInfoList);
	}

	/// <summary>
	/// 获取模块参数列表
	/// </summary>
	/// <param name="pstModuleParamList"> 模块参数列表 </param>
	/// <returns></returns>
	public static int IMVS_GetParamList(IntPtr handle, uint nModuleID, IntPtr pstModuleParamList)
	{
		return NativeMethods.IMVS_GetParamList(handle, nModuleID, pstModuleParamList);
	}

	/// <summary>
	/// 设置模块参数列表
	/// </summary>
	/// <param name="pstModuleParamList"> 模块参数列表 </param>
	/// <returns></returns>
	public static int IMVS_SetParamList(IntPtr handle, uint nModuleID, IntPtr pstModuleParamList)
	{
		return NativeMethods.IMVS_SetParamList(handle, nModuleID, pstModuleParamList);
	}

	/// <summary>
	/// 获取方案是否修改信息
	/// </summary>
	/// <param name="nSolModify"> 1：有修改，0：无修改 </param>
	/// <returns></returns>
	public static int IMVS_GetSolModifyInfo(IntPtr handle, ref uint nSolModify)
	{
		return NativeMethods.IMVS_GetSolModifyInfo(handle, ref nSolModify);
	}

	/// <summary>
	/// 设置当前方案是否修改标记
	/// </summary>
	/// <param name="nModifyFlag"> 1：有修改，0：无修改 </param>
	/// <returns></returns>
	public static int IMVS_SetSolModifyFlag(IntPtr handle, uint nModifyFlag)
	{
		return NativeMethods.IMVS_SetSolModifyFlag(handle, nModifyFlag);
	}

	/// <summary>
	/// 查询IO被订阅信息，数据拷贝后使用接口释放指针-IMVS_ReleaseDynamicMemory
	/// </summary>
	/// <param name="nModuleId">模块ID</param>
	/// <param name="strIOName">IO名称</param>
	/// <param name="eIsIoBeSubscribed">IO被订阅类型</param>
	/// <param name="pstIOSubInfoList">IO订阅信息</param>
	/// <returns></returns>
	public static int IMVS_QueryIOBeSubscribed(IntPtr handle, uint nModuleId, string strIOName, ImvsSdkDefine.IMVS_IO_BE_SUBSCRIBED_TYPE eIsIoBeSubscribed, ref IntPtr pstIOSubInfoList)
	{
		return NativeMethods.IMVS_QueryIOBeSubscribed(handle, nModuleId, strIOName, eIsIoBeSubscribed, ref pstIOSubInfoList);
	}

	/// <summary>
	/// 删除IO被参数订阅信息
	/// </summary>
	/// <param name="nModuleId">模块ID</param>
	/// <param name="strIOName">IO名称</param>
	/// <returns></returns>
	public static int IMVS_DeleteIOBeSubscribedByParams(IntPtr handle, uint nModuleId, string strIOName)
	{
		return NativeMethods.IMVS_DeleteIOBeSubscribedByParams(handle, nModuleId, strIOName);
	}

	/// <summary>
	/// 清空历史结果信息
	/// </summary>
	/// <param name="nModuleId">模块ID</param>
	/// <returns></returns>
	public static int IMVS_ClearModuleHistoryResult(IntPtr handle, uint nModuleId)
	{
		return NativeMethods.IMVS_ClearModuleHistoryResult(handle, nModuleId);
	}

	/// <summary>
	/// 获取多个模块连接关系
	/// </summary>
	/// <param name="pastModuleId"> 模块ID列表 </param>
	/// <param name="pstModuConnectInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetMultiModuEnableState(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, IntPtr pstModuStateInfo)
	{
		return NativeMethods.IMVS_GetMultiModuEnableState(handle, ref pastModuleId, pstModuStateInfo);
	}

	/// <summary>
	/// 获取多个模块加载失败状态
	/// </summary>
	/// <param name="pastModuleId"> 模块ID列表 </param>
	/// <param name="pstModuConnectInfoList"></param>
	/// <returns></returns>
	public static int IMVS_GetMultiObjectLoadState(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, IntPtr pstModuStateInfo)
	{
		return NativeMethods.IMVS_GetMultiObjectLoadState(handle, ref pastModuleId, pstModuStateInfo);
	}

	/// <summary>
	/// 获取自动保存方案状态信息
	/// </summary>
	/// <param name="pnStatus"> 自动保存方案状态（1：开启，0：未开启） </param>
	/// <returns></returns>
	public static int IMVS_GetAutoSaveSolState(IntPtr handle, ref uint pnStatus)
	{
		return NativeMethods.IMVS_GetAutoSaveSolState(handle, ref pnStatus);
	}

	/// <summary>
	/// 设置自动保存方案状态信息
	/// </summary>
	/// <param name="nStatus"> 自动保存方案状态（1：开启，0：关闭） </param>
	/// <returns></returns>
	public static int IMVS_SetAutoSaveSolState(IntPtr handle, uint nStatus)
	{
		return NativeMethods.IMVS_SetAutoSaveSolState(handle, nStatus);
	}

	/// <summary>
	/// 轻量保存方案接口
	/// </summary>
	public static int IMVS_LightSaveSolutionEx(IntPtr handle)
	{
		return NativeMethods.IMVS_LightSaveSolutionEx(handle);
	}

	/// <summary>
	/// 获取二次开发进程信息接口
	/// </summary>
	public static int IMVS_GetThirdSoftwareInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_THIRD_SOFTWARE_INFO_LIST stThirdSoftwareInfo)
	{
		return NativeMethods.IMVS_GetThirdSoftwareInfo(handle, ref stThirdSoftwareInfo);
	}

	/// <summary>
	/// 设置流程等待超时接口
	/// </summary>
	public static int IMVS_SetProcedureWaitTime(IntPtr handle, uint nProcessID, uint nWaitTime)
	{
		return NativeMethods.IMVS_SetProcedureWaitTime(handle, nProcessID, nWaitTime);
	}

	/// <summary>
	/// 获取流程等待超时接口
	/// </summary>
	public static int IMVS_GetProcedureWaitTime(IntPtr handle, uint nProcessID, ref uint nWaitTime)
	{
		return NativeMethods.IMVS_GetProcedureWaitTime(handle, nProcessID, ref nWaitTime);
	}

	/// <summary>
	/// 设置全局执行延迟时间
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="nDelayTime"></param>
	/// <returns></returns>
	public static int IMVS_SetExecuteDelayTime(IntPtr handle, uint nDelayTime)
	{
		return NativeMethods.IMVS_SetExecuteDelayTime(handle, nDelayTime);
	}

	public static int IMVS_PF_GetDongleAuthority(IntPtr handle)
	{
		return NativeMethods.IMVS_PF_GetDongleAuthority(handle);
	}

	/// <summary>
	/// 获取错误信息
	/// </summary>
	/// <param name="nErrorCode"></param>
	/// <returns></returns>
	public static string IMVS_PF_GetErrorMsg_CS(int nErrorCode)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = NativeMethods.IMVS_PF_GetErrorMsg((uint)nErrorCode);
		return Marshal.PtrToStringAnsi(intPtr);
	}

	public static int CheckServerPasswordStatus(string serverIP, uint serverPort, ref uint waitingTime)
	{
		return NativeMethods.IMVS_CheckServerPasswordStatus(serverIP, serverPort, ref waitingTime);
	}

	public static int SetServerPassword(string serverIP, uint serverPort, string password)
	{
		return NativeMethods.IMVS_SetServerPassword(serverIP, serverPort, password);
	}

	public static int ModifyServerPassword(string oldPassword, string newPassword)
	{
		return NativeMethods.IMVS_ModifyServerPassword(oldPassword, newPassword);
	}

	public static int StartupVmFrontendWindowOnLinux(string strAddress)
	{
		return NativeMethods.IMVS_StartupVmFrontendWindowOnLinux(strAddress);
	}

	public static int GetAlgorithmVersion(ref ImvsSdkDefine.IMVS_ALGORITHM_VERSION_INFO versions)
	{
		return NativeMethods.IMVS_GetAlgorithmVersion(ref versions);
	}

	/// <summary>
	/// 创建SDK的句柄
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="strServerPath"></param>
	/// <returns></returns>
	public static int IMVS_PF_CreateHandle(ref IntPtr handle, string strServerPath = "")
	{
		return NativeMethods.IMVS_PF_CreateHandle(ref handle, strServerPath);
	}

	/// <summary>
	/// 执行一次带执行次数
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="strServerPath"></param>
	/// <returns></returns>
	public static int IMVS_PF_ExecuteOnceEX_V30(IntPtr handle, uint nProcessID, string strCommand, ref uint pnExecuteCount)
	{
		return NativeMethods.IMVS_PF_ExecuteOnceEX_V30(handle, nProcessID, strCommand, ref pnExecuteCount);
	}

	/// <summary>
	/// 模块开启/停止回调信息 | Start/Stop module callback information
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="nModuleID">模块ID (模块ID为IMVS_PF_ALL_MODULE_CTRL_CALLBACK时表示所有模块开启/停止回调信息) | Module ID (The module ID is IMVS_PF_ALL_MODULE_CTRL_CALLBACK indicates that all modules are starting/stopping callback information)</param>
	/// <param name="nCallbackStatus">回调状态（0：停止回调；1：开启回调） | Callback status (0:off; 1:on)</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_CtrlCallBackModuResult(IntPtr handle, uint nModuleID, uint nCallbackStatus)
	{
		return NativeMethods.IMVS_PF_CtrlCallBackModuResult(handle, nModuleID, nCallbackStatus);
	}

	/// <summary>
	/// 带回调同步导入流程
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_ImportProcess(IntPtr handle, string strImportPath, string strPassword, bool bOverrideGlobal, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_SYNC_IMPORT_PROCESS_INFO pstProcessInfo)
	{
		return NativeMethods.IMVS_PF_ImportProcess(handle, strImportPath, strPassword, bOverrideGlobal, nIdAllocateWay, ref pstProcessInfo);
	}

	/// <summary>
	/// 设置模块输出图像的缓存节点数量
	/// </summary>
	/// <param name="handle">句柄</param>
	/// <param name="nModuleId">模块ID</param>
	/// <param name="nBufferNodeNum">缓存节点数量</param>
	/// <returns></returns>
	public static int IMVS_PF_SetImageBufferNodeNum(IntPtr handle, uint nModuleId, uint nBufferNodeNum)
	{
		return NativeMethods.IMVS_PF_SetImageBufferNodeNum(handle, nModuleId, nBufferNodeNum);
	}

	/// <summary>
	/// 设置用户图像数据 | Set user image data
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="stInputImageInfo">用户图像数据 | User image data</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_SetImageData(IntPtr handle, ImvsSdkDefine.IMVS_PF_INPUT_IMAGE_INFO stInputImageInfo)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_PF_INPUT_IMAGE_INFO)));
		Marshal.StructureToPtr(stInputImageInfo, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_PF_SetImageData(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}

	/// <summary>
	/// 设置模块结果缓存配置 |Set module result buffer configuration
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="bBufferEnable">缓存使能 | Buffer enable</param>
	/// <param name="nBufferNum">缓存结果数量 | Number of module result</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_SetModuleResultBuffer(IntPtr handle, bool bBufferEnable, uint nBufferNum)
	{
		return NativeMethods.IMVS_PF_SetModuleResultBuffer(handle, bBufferEnable, nBufferNum);
	}

	/// <summary>
	/// 获取模块结果缓存配置 | Get module result buffer configuration
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="bBufferEnable">缓存使能 | Buffer enable</param>
	/// <param name="nBufferNum">缓存结果数量 | Number of module result</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_GetModuleResultBuffer(IntPtr handle, ref bool bBufferEnable, ref uint nBufferNum)
	{
		return NativeMethods.IMVS_PF_GetModuleResultBuffer(handle, ref bBufferEnable, ref nBufferNum);
	}

	/// <summary>
	/// 设置流程可重入 |Set process multi
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="bReentrantEnable">可重入使能 | process multi enable</param>
	/// <param name="nReentrantNum"></param>
	/// <returns></returns>
	public static int IMVS_SetProcedureReentrant(IntPtr handle, bool bReentrantEnable, uint nReentrantNum)
	{
		return NativeMethods.IMVS_SetProcedureReentrant(handle, bReentrantEnable, nReentrantNum);
	}

	/// <summary>
	/// 获取流程可重入 |Get process multi
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="bReentrantEnable">可重入使能 | process multi enable</param>
	/// <param name="nReentrantNum"></param>
	/// <returns></returns>
	public static int IMVS_GetProcedureReentrant(IntPtr handle, ref bool bReentrantEnable, ref uint nReentrantNum)
	{
		return NativeMethods.IMVS_GetProcedureReentrant(handle, ref bReentrantEnable, ref nReentrantNum);
	}

	/// <summary>
	/// 设置方案后缀是否校验 | Set whether the solution suffix is verified
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="bVerifyEnable">校验使能 | Verify enable</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_SetFileSuffixVerify(IntPtr handle, bool bVerifyEnable)
	{
		return NativeMethods.IMVS_PF_SetFileSuffixVerify(handle, bVerifyEnable);
	}

	public static int IMVS_PF_GetModuleIDByFullName(IntPtr handle, string strFullName, bool bIsIndependentGro, ref uint nModuleId)
	{
		return NativeMethods.IMVS_PF_GetModuleIDByFullName(handle, strFullName, bIsIndependentGro, ref nModuleId);
	}

	public static int IMVS_GetDongleInfo()
	{
		return NativeMethods.IMVS_GetDongleInfo();
	}

	public static int IMVS_MakeServerReportDongleInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_DONGLE_INFO pstDongleInfo)
	{
		return NativeMethods.IMVS_MakeServerReportDongleInfo(handle, ref pstDongleInfo);
	}

	public static bool FtpInit(IntPtr handle, string strHost, int uPort, string strLogin, string strPassword)
	{
		bool flag = false;
		return NativeMethods.IMVS_FtpInit(handle, strHost, (uint)uPort, strLogin, strPassword);
	}

	public static bool FtpDownloadFile(IntPtr handle, string strOutputfile, string strPath, bool bCover = false)
	{
		bool flag = false;
		return NativeMethods.IMVS_FtpDownloadFile(handle, strOutputfile, strPath, bCover);
	}

	public static bool FtpDownloadDir(IntPtr handle, string strOutputfile, string strPath, bool bCover = false)
	{
		bool flag = false;
		return NativeMethods.IMVS_FtpDownloadDir(handle, strOutputfile, strPath, bCover);
	}

	public static bool FtpUploadFile(IntPtr handle, string strLocalFile, string strRemoteFile, bool bCreatDir = true, bool bCover = false)
	{
		return NativeMethods.IMVS_FtpUploadFile(handle, strLocalFile, strRemoteFile, bCreatDir, bCover);
	}

	public static bool FtpUploadDir(IntPtr handle, string strLocalDir, string strRemoteDir, bool bCover = false)
	{
		return NativeMethods.IMVS_FtpUploadDir(handle, strLocalDir, strRemoteDir, bCover);
	}

	public static bool FtpCheckFileName(IntPtr handle, string strRemoteFile)
	{
		return NativeMethods.IMVS_FtpCheckFileName(handle, strRemoteFile);
	}

	public static bool FtpCreatDir(IntPtr handle, string strRemoteDir)
	{
		return NativeMethods.IMVS_FtpCreateDir(handle, strRemoteDir);
	}

	public static bool FtpRemoveFile(IntPtr handle, string strRemoteFile)
	{
		return NativeMethods.IMVS_FtpRemoveFile(handle, strRemoteFile);
	}

	public static bool FtpRemoveDir(IntPtr handle, string strRemoteDir)
	{
		return NativeMethods.IMVS_FtpRemoveDir(handle, strRemoteDir);
	}

	public static bool FtpRename(IntPtr handle, string strOldName, string strNewName)
	{
		return NativeMethods.IMVS_FtpRename(handle, strOldName, strNewName);
	}

	public static bool FtpUploadFile(IntPtr handle, string strLocalFile, ref string strRemoteFile, bool bCreatDir = true, bool bCover = false)
	{
		if (strRemoteFile.EndsWith("/"))
		{
			string fileName = Path.GetFileName(strLocalFile);
			strRemoteFile += fileName;
		}
		if (NativeMethods.IMVS_FtpUploadFile(handle, strLocalFile, strRemoteFile, bCreatDir, bCover))
		{
			strRemoteFile = $"/root{strRemoteFile}";
			return true;
		}
		return false;
	}

	public static bool FtpRemoteFileList(IntPtr handle, string strRemoteDir, ref ImvsSdkDefine.FTP_REMOTE_FILE_LIST_INFO ftpFileInfo, bool bOnlyName = false)
	{
		IntPtr intPtr = ExternalCallHelper.StructureToPtr(ftpFileInfo);
		bool flag = NativeMethods.IMVS_FtpRemoteFileList(handle, strRemoteDir, intPtr, bOnlyName);
		if (!flag)
		{
			ExternalCallHelper.PtrToStructure(intPtr, ref ftpFileInfo);
			int fileNum = ftpFileInfo.FileNum;
			if (fileNum <= 0)
			{
				return false;
			}
			ImvsSdkDefine.FTP_REMOTE_FILE_INFO[] array = new ImvsSdkDefine.FTP_REMOTE_FILE_INFO[fileNum];
			IntPtr intPtr2 = Marshal.AllocHGlobal(fileNum * Marshal.SizeOf(typeof(ImvsSdkDefine.FTP_REMOTE_FILE_INFO)));
			if (IntPtr.Zero == intPtr2)
			{
				return false;
			}
			long num = intPtr2.ToInt64();
			for (int i = 0; i < fileNum; i++)
			{
				Marshal.StructureToPtr(ptr: new IntPtr(num), structure: array[i], fDeleteOld: false);
				num += Marshal.SizeOf(typeof(ImvsSdkDefine.FTP_REMOTE_FILE_INFO));
			}
			ftpFileInfo.FTPFileInfos = intPtr2;
			intPtr = ExternalCallHelper.StructureToPtr(ftpFileInfo);
			flag = NativeMethods.IMVS_FtpRemoteFileList(handle, strRemoteDir, intPtr, bOnlyName);
		}
		ExternalCallHelper.PtrToStructure(intPtr, ref ftpFileInfo);
		return flag;
	}

	public static bool FtpRemoteList(IntPtr handle, string strRemoteDir, ref int nDataLength, ref byte[] dataByte, bool bOnlyName = false)
	{
		IntPtr intPtr = IntPtr.Zero;
		bool flag = false;
		try
		{
			if (nDataLength == 0)
			{
				return NativeMethods.IMVS_FtpRemoteList(handle, strRemoteDir, ref nDataLength, intPtr, bOnlyName);
			}
			intPtr = Marshal.AllocHGlobal(nDataLength);
			flag = NativeMethods.IMVS_FtpRemoteList(handle, strRemoteDir, ref nDataLength, intPtr, bOnlyName);
			if (flag)
			{
				Marshal.Copy(intPtr, dataByte, 0, nDataLength);
			}
		}
		catch (Exception)
		{
			flag = false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr);
				intPtr = IntPtr.Zero;
			}
		}
		return flag;
	}

	public static bool FtpDeInit(IntPtr handle)
	{
		return NativeMethods.IMVS_FtpDeInit(handle);
	}

	public static int SetFilePathFromWindows(IntPtr handle, byte[] dataByte, string strParamName, uint nModuleID, IntPtr dataPtr, ref int nDataLen)
	{
		int result = -1;
		IntPtr intPtr = Marshal.AllocHGlobal(dataByte.Length);
		Marshal.Copy(dataByte, 0, intPtr, dataByte.Length);
		IntPtr intPtr2 = new IntPtr(0);
		try
		{
			if (nDataLen == 0)
			{
				ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO structure = new ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO
				{
					pBinaryData = intPtr,
					nBinaryLenth = (uint)dataByte.Length,
					strName = StringUtil.UTF8GetFixLenBytes(strParamName, 256)
				};
				intPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO)));
				if (IntPtr.Zero != intPtr2)
				{
					Marshal.StructureToPtr(structure, intPtr2, fDeleteOld: true);
				}
				result = NativeMethods.IMVS_SetFilePathFromWindows(handle, intPtr2, strParamName, nModuleID, dataPtr, ref nDataLen);
			}
			else
			{
				result = NativeMethods.IMVS_SetFilePathFromWindows(handle, intPtr, strParamName, nModuleID, dataPtr, ref nDataLen);
			}
		}
		catch (Exception)
		{
			return -1;
		}
		finally
		{
			if (IntPtr.Zero != intPtr)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
			}
			if (IntPtr.Zero != intPtr2)
			{
				Marshal.FreeHGlobal(intPtr2);
				intPtr2 = IntPtr.Zero;
			}
		}
		return result;
	}

	public static int GenerateFileToWindows(IntPtr handle, string strLocalPath, string strParamName, uint nModuleID, IntPtr pTempPath, bool bDeleteTempFile = true)
	{
		int num = -1;
		return NativeMethods.IMVS_GenerateFileToWindows(handle, strLocalPath, strParamName, nModuleID, pTempPath, bDeleteTempFile);
	}

	/// <summary>
	/// 设置用户3D图像数据 | Set user image data
	/// </summary>
	/// <param name="handle">操作句柄 | Handle</param>
	/// <param name="stInputImageInfo">用户图像数据 | User image data</param>
	/// <returns> 成功,返回IMVS_EC_OK;失败,返回错误码 | Return IMVS_EC_OK for success, and return Error Code for failure </returns>
	public static int IMVS_PF_SetImageData3D(IntPtr handle, ImvsSdkDefine.IMVS_PF_INPUT_IMAGE_3D_INFO stInputImageInfo)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_PF_INPUT_IMAGE_3D_INFO)));
		Marshal.StructureToPtr(stInputImageInfo, intPtr, fDeleteOld: true);
		int result = NativeMethods.IMVS_PF_SetImageData3D(handle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return result;
	}
}
