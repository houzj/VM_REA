using System;
using System.Runtime.InteropServices;

namespace VM.PlatformSDKCS;

internal static class NativeMethods
{
	private const string platformSDK_lib = ".\\iMVS-6000PlatformSDK.dll";

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ModifyModuleSubInputName(IntPtr handle, uint nModuleID, string strNameOld, string strNameNew);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetBuildVersion(ref uint nYear, ref uint nMonth, ref uint nDay);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CreateHandle(ref IntPtr handle, ref ImvsSdkDefine.IMVS_PLATFORM_BASIC_INFO pstPlatformBasicInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DestroyHandle(IntPtr handle, uint nTakeoverType = 0u);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CharSet = CharSet.Ansi)]
	public static extern int IMVS_RegisterOutputPlatformInfoCallBack(IntPtr handle, delegateOutputCallBack pOutputCallBack, IntPtr user);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CharSet = CharSet.Ansi)]
	public static extern int IMVS_RegisterOutputPlatformInfoCallBack_V32(IntPtr handle, delegateOutputCallBack pOutputCallBack, IntPtr user);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ClearCustomOutputInfo(IntPtr handle, uint nProcessID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetPlatformDetailedInfo(IntPtr handle, IntPtr pstPlatformDetailedInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SubscribeResultEx(IntPtr handle, IntPtr pstSubscribeInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_UnSubscribeResultEx(IntPtr handle, IntPtr pstSubscribeInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_StopExecute_V30(IntPtr handle, uint nProcessID, uint nWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_StopExecute(IntPtr handle, uint nWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ConnectOneModule(IntPtr handle, uint nPreModuleId, uint nNextModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DisConnectOneModule(IntPtr handle, uint nPreModuleId, uint nNextModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CloseOneModule(IntPtr handle, uint nModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_EnableModule(IntPtr handle, uint nModuleID, uint nEnableFlag = 0u);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetModuleState(IntPtr handle, uint nModuleID, ref uint nModuleState);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ExecuteOnce(IntPtr handle, IntPtr p);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ContinousRunStart(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetModuleListByType(IntPtr handle, uint nModuleType, IntPtr pstModuleInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAllModuleList(IntPtr handle, IntPtr pstModuleInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetModuleListInProcess(IntPtr handle, uint nProcessID, IntPtr pstModuleInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ExecuteOnce_V30(IntPtr handle, uint nProcessID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strCommand, bool bIsP2P = false);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ContinousRunStart_V30(IntPtr handle, uint nProcessID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CloseSolution(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetContinousRunInterval(IntPtr handle, uint millSecond);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetContinousRunInterval_V30(IntPtr handle, uint processID, uint millSecond);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetProcContinousRunInterval(IntPtr handle, uint processID, ref uint millSecond);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SelfExecuteOneModule(IntPtr handle, uint nModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SyncSelfExecuteOneModule(IntPtr handle, uint nModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern string IMVS_GetLastError();

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_AddOneModule(IntPtr handle, ref ImvsSdkDefine.IMVS_SYNC_ADD_ONE_MODULE_INFO pstSyncAddModuInput, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAllProcessList(IntPtr handle, IntPtr pstProcessInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetModuDisplayName(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strDisplayName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CtrlCallBackModuResult(IntPtr handle, uint nModuleID, uint nCallbackStatus);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SaveSolution(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassWord, IntPtr pData, uint nDataLen);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CheckSolution([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, ref uint pnHasPwd, ref uint pnValidFormat);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_LoadSolution(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassWord, bool bCheckSolFlag, bool bReportSilenceRunFlag);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CheckPassword([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassWord, ref int pnIsCorrect);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetSolutionVersion(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassWord, ref ImvsSdkDefine.IMVS_SOLUTION_VERSION_INFO pstSolutionVersionInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetCustomOutputSubscribeInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_CUSTOM_SUBSCRIBE_INFO pstCustomSubInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetCustomOutputSubscribeInfo(IntPtr handle, uint nProcessID, uint nIndexNum, IntPtr pstCustomSubInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_OpenGlobalScriptInteface(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ExecuteOnce4GlobalScript(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAllExecutableProcedure(IntPtr handle, IntPtr pstProcessInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ContinousRunStart4GlobalScript(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_StopExecute4GlobalScript(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetParamValue4GlobalScript(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strTransData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetParamValue4GlobalScript(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strReqData, uint nTransDataSize, IntPtr strTransData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_StartGlobalScript(IntPtr handle, uint nTransDataSize, IntPtr strTransData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetParamValue(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, uint nStrValueSize, IntPtr strValue);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetParamValue(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strValue);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetParamValueForImageSource(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetBinaryLength(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_GET_BINARY_LENGTH_INFO strValue, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetBinaryData(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetBinaryDataEx(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetBinaryData(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO strValue, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_AddOneProcess(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strDisplayName, ref ImvsSdkDefine.IMVS_PROCESS_INFO pstPrcInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetProcName(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_DeleteOneProcess(IntPtr handle, uint nProcessID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_AddOneModuleWithProcId(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strModuleName, uint nProcessID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetTakeoverStatus(IntPtr handle, ref ImvsSdkDefine.IMVS_STATUS_TAKEOVER_INFO stTakeoverInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetPlatformDetailedInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_PLATFORM_DETAILED_INFO stPlatformInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAutoLoadSolution(IntPtr handle, ref ImvsSdkDefine.IMVS_AUTO_LOAD_SOLUTION_INFO stSolInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetAutoLoadSolution(IntPtr handle, ref ImvsSdkDefine.IMVS_AUTO_LOAD_SOLUTION_INFO stSolInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ShieldModuleCbResult(IntPtr handle, uint nShiledFlag = 0u);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ShieldProcess(IntPtr handle, uint nProcessID, uint nShiledFlag);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetProcedureShieldGlobalCtrl(IntPtr handle, uint nProcessID, ref uint nShiledFlag);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CopyModuleParam(IntPtr handle, uint nSrcModuleId, uint nDstModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetServerReadyEvent(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_MakeModulesPrepared(IntPtr handle, IntPtr pstCamPicInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_MakeModulesPreparedBySelfRun(IntPtr handle, IntPtr pstCamPicInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetProcedureRunPolicy(IntPtr handle, ref ImvsSdkDefine.IMVS_PROCEDURE_RUN_POLICY stProcRunPolicyInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetProcedureRunPolicy(IntPtr handle, ref ImvsSdkDefine.IMVS_PROCEDURE_RUN_POLICY stProcRunPolicyInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetSolutionPath(IntPtr handle, uint nPathSize, IntPtr strSolPath, ref uint nRealPathSize);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetProcessorInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_PROCESSOR_INFO pstProcessorInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_CopyModuleAlgParam(IntPtr handle, uint nSrcModuID, uint nDstModuID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetDefaultParamList(IntPtr handle, uint nModuleID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_AddGroupSonModule(IntPtr handle, ref ImvsSdkDefine.IMVS_GROUP_MODULE_INFO pstGroupModuInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_RemoveGroupSonModule(IntPtr handle, ref ImvsSdkDefine.IMVS_GROUP_MODULE_INFO pstGroupModuInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_DeleteGroupModule(IntPtr handle, uint nGroupModuleID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ExportGroupModule(IntPtr handle, uint nGroupId, uint nGroupLevel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SyncExportGroupModule(IntPtr handle, uint nGroupId, uint nGroupLevel, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ImportGroupModule(IntPtr handle, uint nProcessID, uint nGroupID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword, uint nIdAllocateWay);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_VersionUpgradeFlag(IntPtr handle, bool bToolFlag);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ImportIndependentGroup(IntPtr handle, uint nProcessID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SyncImportGroupModule(IntPtr handle, uint nProcessID, uint nGroupID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetGroupIO(IntPtr handle, IntPtr pstGroupIOInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_BindGlobalVarParam(IntPtr handle, IntPtr pGlobalVarInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_UnBindGlobalVarParam(IntPtr handle, IntPtr pstGlobalVarInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetBindGlobalVarParam(IntPtr handle, uint nModuleId, int nIndex, int nQueNum, IntPtr pGlobalVarInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_LightSaveSolution(IntPtr handle, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_NotifyLoadSolutionEnd(IntPtr handle, uint nClientType, int nErrorCode);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ExportProcess(IntPtr handle, uint nProcessId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SyncExportProcess(IntPtr handle, uint nProcessId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ImportProcess(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword, uint nIsIgnoreGlobal, uint nIdAllocateWay);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SyncImportProcess(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassword, uint nIsIgnoreGlobal, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_PROCESS_INFO pstProcessInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetProcRunParam(IntPtr handle, uint nProcessID, uint nStopNGAction);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetProcRunParam(IntPtr handle, uint nProcessID, ref uint pnStopNGAction);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetObjectLimits(IntPtr handle, ref uint pnMaxProcessNum, ref uint pnMaxModuleNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetMaxModuleNum(IntPtr handle, uint pnMaxModuleNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CheckUIParamExist(IntPtr handle, uint nObjectID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetUIParamValue(IntPtr handle, uint nObjectID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName, IntPtr pData, uint nDataLen, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetUIParamValueByKey(IntPtr handle, uint nObjectID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName, ref IntPtr pData, ref uint nDataLen, uint nRecvWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetUIParamValueByObject(IntPtr handle, uint nObjectID, ref IntPtr pUIObjParamInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetFirstLayerModuleTree(IntPtr handle, uint nObjectID, uint nObjectType, IntPtr pNodeTreeInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetFirstLayerModuleConnect(IntPtr handle, uint nProcessID, uint nObjectType, ref IntPtr pModuConnectInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetFirstLayerModuleSubscribe(IntPtr handle, uint nProcessID, uint nObjectType, ref IntPtr pModuSubInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ReleaseUIParamData(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetReportModuleResult(IntPtr handle, IntPtr pReportModuInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetReportModuleResult_V2(IntPtr handle, ImvsSdkDefine.IMVS_REPORT_RESULT_TYPE nMode, uint nModuId = 0u, bool bIsEnable = false);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetAllReportModuleResult(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetNoneReportModuleResult(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_KeepModuleLastResult(IntPtr handle, uint nModuleID, bool bIsUnchanged);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_QueryModuleRunningState(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, ref IntPtr pQueryModuStaInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_QueryModuleHistoryResult(IntPtr handle, uint nModuleID, int nIndex, uint nRequestNum, ref IntPtr pQueryModuResInfoList, bool bResBuffFirst, bool bNeedImage, bool bUseResStructMap, bool bQueryByRunCount, uint nRunCount);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetDataForExportOperator(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, IntPtr pDataBuff, uint nBuffSize, ref uint nDataLen);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetModuleExportData(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strModuDisplayName, uint nWaitTime, ref IntPtr pstModuExportData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ImportModuleData(IntPtr handle, ref ImvsSdkDefine.IMVS_IMPORT_MODULE_DATA_INPUT pstImportData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ImportModuleDataEx(IntPtr handle, ref ImvsSdkDefine.IMVS_IMPORT_MODULE_DATA_INFO pstModuImportData, ref ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetInputInt(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_INPUT_INT_LIST_INFO pstIntListInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetInputFloat(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_INPUT_FLOAT_LIST_INFO pstFloatListInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetInputString(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_INPUT_STRING_LIST_INFO pstStringListInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetInputImage(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_MODULE_INPUT_DATA_INFO pstImageInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetInputPointset(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_MODULE_INPUT_DATA_INFO pstPointsetInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetInputBytes(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_MODULE_INPUT_DATA_INFO pstBytesInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DeleteUIParamValue(IntPtr handle, uint nObjectID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName, uint nBlurDelete);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ExportMultiModuleData(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, uint nWaitTime, ref IntPtr pstModuExportData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetMultiModuleConnect(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, ref IntPtr pstModuConnectInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetMultiModuleSubscribe(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, ref IntPtr pstModuSubInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAllGlobalModule(IntPtr handle, IntPtr pstModuleInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetParamList(IntPtr handle, uint nModuleID, IntPtr pstModuleParamList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetParamList(IntPtr handle, uint nModuleID, IntPtr pstModuleParamList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetSolModifyInfo(IntPtr handle, ref uint nSolModify);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_FuzzyQueryUIParamValue(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName, bool bIsFuzzyQuery, ref IntPtr pUiParamInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DeleteAllUIParamValue(IntPtr handle, uint nObjectID);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetSolModifyFlag(IntPtr handle, uint nModifyFlag);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_QueryIOBeSubscribed(IntPtr handle, uint nModuleId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strIOName, ImvsSdkDefine.IMVS_IO_BE_SUBSCRIBED_TYPE eIsIoBeSubscribed, ref IntPtr pstIOSubInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DeleteIOBeSubscribedByParams(IntPtr handle, uint nModuleId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strIOName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_ClearModuleHistoryResult(IntPtr handle, uint nModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAllProcedureShieldState(IntPtr handle, ref uint nShiledFlag);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetMultiModuEnableState(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, IntPtr pstModuStateInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetMultiObjectLoadState(IntPtr handle, ref ImvsSdkDefine.IMVS_MODULE_ID_LIST pastModuleId, IntPtr pstModuStateInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetProcedureIO(IntPtr handle, IntPtr pstPrcIOInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetAutoSaveSolState(IntPtr handle, ref uint pnStatus);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetAutoSaveSolState(IntPtr handle, uint nStatus);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_LightSaveSolutionEx(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetThirdSoftwareInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_THIRD_SOFTWARE_INFO_LIST pstThirdSoftwareInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetProcedureWaitTime(IntPtr handle, uint nProcessID, uint nWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetProcedureWaitTime(IntPtr handle, uint nProcessID, ref uint nWaitTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetExecuteDelayTime(IntPtr handle, uint nDelayTime);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetPrcVersion(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassWord, ref ImvsSdkDefine.IMVS_FILE_VERSION_INFO pstVersionInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetGroVersion(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, string strPassWord, ref ImvsSdkDefine.IMVS_FILE_VERSION_INFO pstVersionInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetModuInputIOInfo(IntPtr handle, uint nModuId, ref IntPtr pstModuIOInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetModuOutputIOInfo(IntPtr handle, uint nModuId, uint nDataType, ref IntPtr pstModuIOInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetModuInputIOInfo(IntPtr handle, uint nObjectId, IntPtr pstModuInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetModuOutputIOInfo(IntPtr handle, uint nObjectId, IntPtr pstModuInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetModuleParamBinding(IntPtr handle, IntPtr pstModuParamInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_DelModuleParamBinding(IntPtr handle, ref ImvsSdkDefine.IMVS_UNBIND_MODULE_PARAM_INFO_LIST pstModuParamInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetModuleParamBindingList(IntPtr handle, uint nModuleId, int nIndex, int nQueNum, IntPtr pGlobalVarInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_AddDynamicParam(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ModifyDynamicParam(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strNameOld, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strNameNew);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_DeleteDynamicParam(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetAllDynamicParam(IntPtr handle, uint nObjectId, IntPtr pstParamList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_AddDynamicParamRelation(IntPtr handle, uint nDestinID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strDestinName, uint nSourceID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strSourceName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetDynamicParamRelationByKey(IntPtr handle, uint nObjectID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, IntPtr pstRelationList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_DeleteDynamicParamRelation(IntPtr handle, uint nDestinID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strDestinName, uint nSourceID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strSourceName);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetDynamicRunParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetDynamicRunParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, uint nValueSize, IntPtr strValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetDynamicBinaryParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, IntPtr pData, uint nDataLen, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetDynamicBinaryParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, ref IntPtr pData, ref uint pnDataLen, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetDynamicUIParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, IntPtr pData, uint nDataLen, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetDynamicUIParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, ref IntPtr pData, ref uint pnDataLen, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_DeleteDynamicRelateUIParamValue(IntPtr handle, uint nObjectId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, ImvsSdkDefine.IMVS_FUZZY_MATCH eFuzzyMatchType, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOptionalParam);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ReleaseDynamicMemory(IntPtr handle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE eMemType, IntPtr pData);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_CheckDataEncryptByDongle(IntPtr handle, IntPtr pData, uint nDataLen, ref bool bIsEncrypted);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetEncryptionFromDongle(IntPtr handle, IntPtr pKeyBuff, uint nBuffSize, ref bool bIsContainKey);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DataEncryptByDongle(IntPtr handle, IntPtr pDataIn, uint nDataLenIn, IntPtr pKeyBuff, IntPtr pDataOut, uint nDataSizeOut, ref uint nDataLenOut);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_DataDecryptByDongle(IntPtr handle, IntPtr pDataIn, uint nDataLenIn, IntPtr pKeyBuff, IntPtr pDataOut, uint nDataSizeOut, ref uint nDataLenOut);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_AddContainerRelateModule(IntPtr handle, ref ImvsSdkDefine.IMVS_CONTAINER_RELATE_MODULE_INFO pstRelateModuleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetContainerRelateModuleInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_CONTAINER_RELATE_MODULE_INFO pstRelateModuleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetDongleSeries(IntPtr handle, ref int nDongleSeries);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetVarReportState(IntPtr handle, uint nModuleID, int nState);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetModuErrorInfoList(IntPtr handle, uint nObjectID, ref ImvsSdkDefine.IMVS_MODULE_RUN_ERROR_INFO_LIST pstErrorInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetSaveImageModuBigFile(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPath, bool bIsEnableSetBigFile);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetSaveImageModuBigFile(IntPtr handle, uint nModuleID, IntPtr strPath, ref bool bIsEnableSetBigFile);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetGlobalConfig(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strValue);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetGlobalConfig(IntPtr handle, uint nModuleID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strName, uint nStrValueSize, IntPtr strValue);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetGroupIOReportMode(IntPtr handle, IntPtr pstGroupIOInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetGroupIOReportMode(IntPtr handle, IntPtr pstGroupIOInfoList);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern IntPtr IMVS_PF_GetErrorMsg(uint nErrorCode);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_CreateHandle(ref IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strServerPath);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_ExecuteOnceEX_V30(IntPtr handle, uint nProcessID, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strCommand, ref uint pnExecuteCount);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_CtrlCallBackModuResult(IntPtr handle, uint nModuleID, uint nCallbackStatus);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_PF_ImportProcess(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strImportPath, string strPassword, bool bOverrideGlobal, uint nIdAllocateWay, ref ImvsSdkDefine.IMVS_SYNC_IMPORT_PROCESS_INFO pstProcessInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_SetImageData(IntPtr handle, IntPtr pInputImageInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_SetImageBufferNodeNum(IntPtr handle, uint nModuleId, uint nBufferNodeNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_DestroyHandle(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_SetModuleResultBuffer(IntPtr handle, bool bBufferEnable, uint nBufferNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_GetModuleResultBuffer(IntPtr handle, ref bool bBufferEnable, ref uint nBufferNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetProcedureReentrant(IntPtr handle, bool bReentrantEnable, uint nReentrantNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetProcedureReentrant(IntPtr handle, ref bool bReentrantEnable, ref uint nReentrantNum);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_GetDongleAuthority(IntPtr handle);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_SetFileSuffixVerify(IntPtr handle, bool bVerifyEnable);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_GetModuleIDByFullName(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strFullName, bool bIsIndependentGro, ref uint nModuleId);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GetDongleInfo();

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_MakeServerReportDongleInfo(IntPtr handle, ref ImvsSdkDefine.IMVS_DONGLE_INFO pstDongleInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_PF_SetImageData3D(IntPtr handle, IntPtr pInputImageInfo);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpInit(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strHost, uint uPort, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strLogin, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strPassword);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpDownloadFile(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strLocalFile, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteFile, bool bCover = false);

	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern bool IMVS_FtpDownloadDir(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strLocalDir, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteDir, bool bCover = false);

	/// <summary>
	/// 从本地上传单个文件到远程指定目录
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpUploadFile(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strLocalFile, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteFile, bool bCreatDir = true, bool bCover = false);

	/// <summary>
	/// 从本地上传整个文件夹到远程目录
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpUploadDir(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strLocalFile, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteFile, bool bCover = false);

	/// <summary>
	/// 检查远程指定目录下是否存在重名文件
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpCheckFileName(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteFile);

	/// <summary>
	/// 在远程上创建目录，允许创建多级目录
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpCreateDir(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteDir);

	/// <summary>
	/// 移除远程上的文件
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpRemoveFile(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteFile);

	/// <summary>
	/// 移除远程上的文件夹，不允许删除多级目录
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpRemoveDir(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteDir);

	/// <summary>
	/// 重命名远程上的文件/文件夹
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpRename(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strOldName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strNewName);

	/// <summary>
	/// 获取远程上的单个文件夹信息
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpRemoteList(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteDir, ref int nDataLength, IntPtr pData, bool bOnlyName = false);

	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpRemoteFileList(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strRemoteDir, IntPtr struFTPFileInfo, bool bOnlyName = false);

	/// @fn     IMVS_CheckServerPasswordStatus
	/// @brief  获取Server的密码状态（跨机使用），该接口在CreateHandle前调用，与IMVS_SetServerPassword配合使用
	/// @param  serverIP            [IN]  服务IP
	/// @param  serverPort          [IN]  服务端口
	/// @param  waitingTime         [OUT] 等待时间，锁定时有效
	/// @return IMVS_EC_OK（本机时返回）、需设置密码（IMVS_EC_AUTH_REMOTE_NO_PASSWORD）、已有密码（IMVS_EC_AUTH_ASSWORD_IS_EXIST）、账户已锁定（IMVS_EC_AUTH_USER_LOCKOUT）
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_CheckServerPasswordStatus([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string serverIP, uint serverPort, ref uint waitingTime);

	/// @fn     IMVS_SetServerPassword
	/// @brief  设置Server密码， 该接口在CreateHandle前调用，与IMVS_CheckServerPasswordStatus配合使用
	/// @param  serverIP            [IN]  服务IP
	/// @param  serverPort          [IN]  服务端口
	/// @param  password            [IN]  密码
	/// @return 成功，返回CM_OK；失败，返回错误码
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_SetServerPassword([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string serverIP, uint serverPort, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string password);

	/// @fn     IMVS_ModifyServerPassword
	/// @brief  修改Server密码，该接口需要连接服务成功后才能运行
	/// @param  oldPassword         [IN]  旧密码
	/// @param  newPassword         [IN]  新密码
	/// @return 成功，返回CM_OK；失败，返回错误码
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_ModifyServerPassword([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string oldPassword, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string newPassword);

	/// @fn     IMVS_StartupVmFrontendWindowOnLinux
	/// @brief  拉起前端运行界面，在DestroyHandle后执行
	/// @param  serverAddress       [IN]  服务地址，形式如: 127.0.0.1:5556
	/// @return 成功，返回IMVS_EC_OK；失败，返回错误码
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_StartupVmFrontendWindowOnLinux([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string serverIP);

	/// @fn     IMVS_GetAlgorithmVersion
	/// @brief  获取算子版本号，包括2d算子和深度算子
	/// @param  normalVersion       [OUT]  2d算子版本
	/// @param  deeplearningVersion [OUT]  深度算子版本
	/// @return 成功，返回IMVS_EC_OK；失败，返回错误码
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern int IMVS_GetAlgorithmVersion(ref ImvsSdkDefine.IMVS_ALGORITHM_VERSION_INFO versions);

	/// <summary>
	/// 反初始化
	/// </summary>
	/// <returns></returns>
	[DllImport(".\\iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool IMVS_FtpDeInit(IntPtr handle);

	/// @fn    IMVS_SetFilePathFromWindows
	/// @brief  把windows下的文件传递到linux，上层传入的是文件路径
	/// @brief  如果nOutPathSize为0，则接口会读出文件中内容然后传递到模块
	/// @brief  如果nOutPathSize为非0，则通过FTP上传文件到linux，然后把linux路径给模块，同时把linux中的路径给调用者
	/// @param  handle                  [IN]        句柄
	/// @param  pInputData              [IN]        传入的windows文件路径
	/// @param  strParamName            [IN]        设置模块参数时的参数名称，如果没有可以置NULL
	/// @param  nModuleID               [IN]        设置模块参数时模块ID
	/// @param  pData                   [OUT]       如果文件需要上传到linux，则里边会赋值liunx文件路径
	/// @param  nDataLen                [IN]        调用者为pOutFilePath分配的长度，默认为0
	/// @return 成功，返回CM_OK；失败，返回对应错误码
	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_SetFilePathFromWindows(IntPtr handle, IntPtr pInputData, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName, uint nModuleID, IntPtr pData, ref int nDataLen);

	/// @fn     IMVS_GenerateFileToWindows
	/// @brief  SDK先产生一个linux的临时路径传递到底层，接口根据bDelTempFile配置选择操作步骤
	/// @param  handle                  [IN]        句柄
	/// @param  strLocalPath            [IN]        文件路径
	/// @param  strParamName            [OUT]       参数名
	/// @param  nModuleID               [IN]        设置模块参数时模块ID
	/// @param  pTempPath               [OUT]       liunx端文件路径
	/// @param  bDelTempFile            [IN]        true：则SDK需要把linux文件拷贝到strLocalPath，然后删除linux下的文件。false：则不进行删除操作
	/// @return 成功，返回CM_OK；失败，返回错误码
	[DllImport(".\\iMVS-6000PlatformSDK.dll")]
	public static extern int IMVS_GenerateFileToWindows(IntPtr handle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strLocalPath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "VM.PlatformSDKCS.UTF8Marshaler")] string strParamName, uint nModuleID, IntPtr pTempPath, bool bDelTempFile = true);
}
