using System;
using System.Runtime.InteropServices;
using System.Threading;
using Apps.Log;
using Hik.Util;
using HikExternalCall.Common;

namespace VM.PlatformSDKCS;

/// <summary>
/// 模块管理
/// </summary>
/// <summary>
/// ModuleManager3D
/// </summary>
public class ModuleManager
{
	public enum ParamTypeEnum
	{
		IntType = 1,
		FloatType,
		StringType,
		ImageType,
		ChunkType,
		ByteType,
		DoubleType
	}

	/// <summary>
	/// 通知事件
	/// </summary>
	private AutoResetEvent mWaitModuRegisterEvent = new AutoResetEvent(initialState: false);

	/// <summary>
	/// 通知事件
	/// </summary>
	private AutoResetEvent mWaitMoveToGroupEvent = new AutoResetEvent(initialState: false);

	/// <summary>
	/// 自执行标志
	/// </summary>
	private bool[] mModuSelfExecuteFlag;

	private bool[] mComGroupSelfExecuteFlag;

	private bool[] mSinGroupSelfExecuteFlag;

	private bool[] mSinModuleSelfExecuteFlag;

	private bool[] mProcSelfExecuteFlag;

	/// <summary>
	/// 自执行模块ID
	/// </summary>
	private uint mSelfExecuteId;

	private const int MaxRequestnum = 10000;

	private const int SplitPageRequestnum = 200;

	/// <summary>
	/// 参数设置标志，保存和加载成功后复位
	/// </summary>
	public bool ParamSeted { get; set; }

	/// <summary>
	/// 模块注册
	/// </summary>
	public event ModuleRegisterCallBack OnModuleRegisterCallBack;

	/// <summary>
	/// 模块结果回调
	/// </summary>
	public event ModuleResultExDataCallBack OnModuleResultCallBack;

	/// <summary>
	/// 开始Group导出
	/// </summary>
	public event GroupExportBeginCallBack OnExportGroupBeginCallBack;

	/// <summary>
	/// Group导出进度
	/// </summary>
	public event GroupExportProcessCallBack OnExportGroupProcessCallBack;

	/// <summary>
	/// Group导出结束
	/// </summary>
	public event GroupExportEndCallBack OnExportGroupEndCallBack;

	/// <summary>
	/// 开始Group导入
	/// </summary>
	public event GroupImportBeginCallBack OnImportGroupBeginCallBack;

	/// <summary>
	/// Group导入进度
	/// </summary>
	public event GroupImportProcessCallBack OnImportGroupProcessCallBack;

	/// <summary>
	/// Group导入结束
	/// </summary>
	public event GroupImportEndCallBack OnImportGroupEndCallBack;

	/// <summary>
	/// Group导入后重新分配ID
	/// </summary>
	public event GroupImportGuidCallBack OnImportGroupGuidCallBack;

	/// <summary>
	/// /导入Group之后，返回的界面文件数据
	/// </summary>
	public event GroupImportInterfaceFileCallBack OnImportGroupInterfaceFileCallBack;

	/// <summary>
	/// 导入Group时通知获取界面数据
	/// </summary>
	public event GroupImportUiDataCallBack OnImportGroupUiDataCallBack;

	/// <summary>
	/// 上报模块状态信息
	/// </summary>
	public event ReportModuleStatusCallBack OnReportModuleStatusCallBack;

	public ModuleManager()
	{
		CVmObjectBase.OnModuleRegisterCallBack += GetModuleRegisterCallBackInfo;
		CVmObjectBase.OnModuleResultExCallBack += GetModuleResultCallBack;
		CVmObjectBase.OnGroupExportBeginCallBack += GetGroupExportBeginCallBack;
		CVmObjectBase.OnGroupExportProcessCallBack += GetGroupExportProcessCallBack;
		CVmObjectBase.OnGroupExportEndCallBack += GetGroupExportEndCallBack;
		CVmObjectBase.OnGroupImportBeginCallBack += GetGroupImportBeginCallBack;
		CVmObjectBase.OnGroupImportProcessCallBack += GetGroupImportProgressCallBack;
		CVmObjectBase.OnGroupImportEndCallBack += GetGroupImportEndCallBack;
		CVmObjectBase.OnGroupImportGuidCallBack += GetGroupImportGuidCallBack;
		CVmObjectBase.OnGroupImportInterfaceFileCallBack += GetGroupImportInterfaceFileCallBack;
		CVmObjectBase.OnGroupImportUiDataCallBack += GetGroupImportUiDataCallBackInfo;
		CVmObjectBase.OnReportModuleStatusCallBack += GetReportModuleStatusCallBackInfo;
		mModuSelfExecuteFlag = new bool[2064];
		mComGroupSelfExecuteFlag = new bool[1000];
		mSinGroupSelfExecuteFlag = new bool[5000];
		mSinModuleSelfExecuteFlag = new bool[10000];
		mProcSelfExecuteFlag = new bool[1000];
	}

	public void ModifyModuleSubInputName(uint nModuleId, string strNameOld, string strNameNew)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_ModifyModuleSubInputName(CVmObjectBase.mhandle, nModuleId, strNameOld, strNameNew);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_MODULE_INFO SyncAddOneModule(ImvsSdkDefine.IMVS_SYNC_ADD_ONE_MODULE_INFO stSyncAddModuInput)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo = default;
		num = ServerSDKAPI.IMVS_AddOneModule(CVmObjectBase.mhandle, ref stSyncAddModuInput, ref pstModuleInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		return pstModuleInfo;
	}

	public void CloseOneModule(uint nModuleId)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_CloseOneModule(CVmObjectBase.mhandle, nModuleId);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void ConnectOneModule(uint nPreModuleId, uint nNextModuleId)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_ConnectOneModule(CVmObjectBase.mhandle, nPreModuleId, nNextModuleId);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void DisConnectOneModule(uint nPreModuleId, uint nNextModuleId)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DisConnectOneModule(CVmObjectBase.mhandle, nPreModuleId, nNextModuleId);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void SubscribeResult(ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO stSubInfo, ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO stPubInfo)
	{
		int num = -536870657;
		IntPtr subscribeResult = GetSubscribeResult(stSubInfo, stPubInfo);
		num = ServerSDKAPI.IMVS_SubscribeResultEx(CVmObjectBase.mhandle, subscribeResult);
		if (IntPtr.Zero != subscribeResult)
		{
			Marshal.FreeHGlobal(subscribeResult);
			subscribeResult = IntPtr.Zero;
		}
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void UnSubscribeResult(ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO stSubInfo, ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO stPubInfo)
	{
		int num = -536870657;
		IntPtr subscribeResult = GetSubscribeResult(stSubInfo, stPubInfo);
		num = ServerSDKAPI.IMVS_UnSubscribeResultEx(CVmObjectBase.mhandle, subscribeResult);
		if (IntPtr.Zero != subscribeResult)
		{
			Marshal.FreeHGlobal(subscribeResult);
			subscribeResult = IntPtr.Zero;
		}
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	private static IntPtr GetSubscribeResult(ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO stSubInfo, ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO stPubInfo)
	{
		ImvsSdkDefine.IMVS_SUBSCRIBE_INFO structure = new ImvsSdkDefine.IMVS_SUBSCRIBE_INFO
		{
			nSubModuleID = stSubInfo.nSubModuleID,
			strSubInput = StringUtil.UTF8GetFixLenBytes(stSubInfo.strParamName, 256),
			nPubModuleID = stPubInfo.nPubModuleID,
			strPubOutput = StringUtil.UTF8GetFixLenBytes(stPubInfo.strParamName, 256),
			nIsPubConstant = stPubInfo.nIsPubConstant,
			nIndexModuleID = stPubInfo.nIndexModuleID,
			strIndex = StringUtil.UTF8GetFixLenBytes(stPubInfo.strIndexContent, 256),
			nIsIndexConstant = stPubInfo.nIsIndexConstant
		};
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_SUBSCRIBE_INFO)));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
		return intPtr;
	}

	public void SelfRun(uint nModuleId)
	{
		int num = ServerSDKAPI.IMVS_SelfExecuteOneModule(CVmObjectBase.mhandle, nModuleId);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void SyncSelfRun(uint nModuleId, bool bIsRespondToUI = true)
	{
		mSelfExecuteId = nModuleId;
		uint num = nModuleId;
		bool flag = false;
		if (nModuleId < 2064)
		{
			num = nModuleId;
			mModuSelfExecuteFlag[num] = false;
			flag = true;
		}
		if (nModuleId >= 10000 && nModuleId <= 10999)
		{
			num = nModuleId - 10000;
			mProcSelfExecuteFlag[num] = false;
			flag = true;
		}
		if (nModuleId >= 21000 && nModuleId <= 21999)
		{
			num = nModuleId - 21000;
			mComGroupSelfExecuteFlag[num] = false;
			flag = true;
		}
		if (nModuleId >= 101000 && nModuleId <= 105999)
		{
			num = nModuleId - 101000;
			mSinGroupSelfExecuteFlag[num] = false;
			flag = true;
		}
		if (nModuleId >= 110000 && nModuleId <= 119999)
		{
			num = nModuleId - 110000;
			mSinModuleSelfExecuteFlag[num] = false;
			flag = true;
		}
		if (!flag)
		{
			throw new VmException(-536870911);
		}
		int num2 = ServerSDKAPI.IMVS_SelfExecuteOneModule(CVmObjectBase.mhandle, nModuleId);
		if (num2 != 0)
		{
			VmException ex = new VmException(num2);
			throw ex;
		}
		uint nModuleState = 0u;
		if (ServerSDKAPI.IMVS_GetModuleState(CVmObjectBase.mhandle, nModuleId, ref nModuleState) == 0 && nModuleState == 0)
		{
			VmException ex2 = new VmException(-536870905);
			throw ex2;
		}
		if (nModuleId < 2064)
		{
			num = nModuleId;
			SyncWaitor.PrecisionWait(ref mModuSelfExecuteFlag[num], bIsRespondToUI);
		}
		if (nModuleId >= 10000 && nModuleId <= 10999)
		{
			num = nModuleId - 10000;
			SyncWaitor.PrecisionWait(ref mProcSelfExecuteFlag[num], bIsRespondToUI);
		}
		if (nModuleId >= 21000 && nModuleId <= 21999)
		{
			num = nModuleId - 21000;
			SyncWaitor.PrecisionWait(ref mComGroupSelfExecuteFlag[num], bIsRespondToUI);
		}
		if (nModuleId >= 101000 && nModuleId <= 105999)
		{
			num = nModuleId - 101000;
			SyncWaitor.PrecisionWait(ref mSinGroupSelfExecuteFlag[num], bIsRespondToUI);
		}
		if (nModuleId >= 110000 && nModuleId <= 119999)
		{
			num = nModuleId - 110000;
			SyncWaitor.PrecisionWait(ref mSinModuleSelfExecuteFlag[num], bIsRespondToUI);
		}
	}

	public void SetModuDisplayName(uint nModuleID, string strDisplayName)
	{
		int num = ServerSDKAPI.IMVS_SetModuDisplayName(CVmObjectBase.mhandle, nModuleID, strDisplayName);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void SetDefaultParamList(uint nModuleID)
	{
		int num = ServerSDKAPI.IMVS_SetDefaultParamList(CVmObjectBase.mhandle, nModuleID);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void EnableModule(uint nModuleID, uint nEnableFlag)
	{
		int num = ServerSDKAPI.IMVS_EnableModule(CVmObjectBase.mhandle, nModuleID, nEnableFlag);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void GetModuleState(uint nModuleID, ref uint nEnableFlag)
	{
		int num = ServerSDKAPI.IMVS_GetModuleState(CVmObjectBase.mhandle, nModuleID, ref nEnableFlag);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void GetModuleFailedState(uint nModuleID, ref uint nEnableFlag)
	{
		ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId = new ImvsSdkDefine.IMVS_MODULE_ID_LIST
		{
			anModuleId = new uint[2064],
			nReserved = new uint[4]
		};
		stModuleId.anModuleId[0] = nModuleID;
		stModuleId.nNum = 1;
		ImvsSdkDefine.IMVS_MODULE_STATE_LIST value = ServerSDKManager.serverSDKManager.mModuleManager.GetMultiObjectLoadState(stModuleId).Value;
		if (value.nNum > 0)
		{
			ImvsSdkDefine.IMVS_MODULE_STATE_INFO iMVS_MODULE_STATE_INFO = value.astModuState[0];
			nEnableFlag = iMVS_MODULE_STATE_INFO.nEnable;
		}
	}

	public void ClearModuleHistoryResult(uint nModuleID)
	{
		int num = ServerSDKAPI.IMVS_ClearModuleHistoryResult(CVmObjectBase.mhandle, nModuleID);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void GetModuErrorInfoList(uint nObjectID, ref ImvsSdkDefine.IMVS_MODULE_RUN_ERROR_INFO_LIST pstErrorInfoList)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_GetModuErrorInfoList(CVmObjectBase.mhandle, nObjectID, ref pstErrorInfoList);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	private void GetModuleRegisterCallBackInfo(ImvsSdkDefine.IMVS_MODULE_INFO moduleInfo)
	{
		OnModuleRegisterCallBack?.Invoke(moduleInfo);
	}

	public void GetModuleResultCallBack(ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX moduleResInfo)
	{
		OnModuleResultCallBack?.Invoke(PreProData(moduleResInfo));
	}

	private void GetGroupExportBeginCallBack(ImvsSdkDefine.IMVS_EXPORT_GROUP_BEGIN_INFO exportGroupBeginInfo)
	{
		OnExportGroupBeginCallBack?.Invoke(exportGroupBeginInfo);
	}

	private void GetGroupExportEndCallBack(ImvsSdkDefine.IMVS_EXPORT_GROUP_END_INFO exportGroupEndInfo)
	{
		OnExportGroupEndCallBack?.Invoke(exportGroupEndInfo);
	}

	private void GetGroupExportProcessCallBack(ImvsSdkDefine.IMVS_EXPORT_GROUP_PROGRESS_INFO exportGroupProcessInfo)
	{
		OnExportGroupProcessCallBack?.Invoke(exportGroupProcessInfo);
	}

	private void GetGroupImportBeginCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_BEGIN_INFO importGroupBeginInfo)
	{
		OnImportGroupBeginCallBack?.Invoke(importGroupBeginInfo);
	}

	private void GetGroupImportEndCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_END_INFO importGroupEndInfo)
	{
		OnImportGroupEndCallBack?.Invoke(importGroupEndInfo);
	}

	private void GetGroupImportProgressCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_PROGRESS_INFO groupImportProcessInfo)
	{
		OnImportGroupProcessCallBack?.Invoke(groupImportProcessInfo);
	}

	private void GetGroupImportGuidCallBack(ImvsSdkDefine.IMVS_GROUP_NEW_MODULEID_NAME_INFO_LIST groupImportGuidInfoList)
	{
		OnImportGroupGuidCallBack?.Invoke(groupImportGuidInfoList);
	}

	private void GetGroupImportInterfaceFileCallBack(ImvsSdkDefine.IMVS_GROUP_IMPORT_INTERFACE_FILE_INFO groupImportInterfaceFileInfo)
	{
		OnImportGroupInterfaceFileCallBack?.Invoke(groupImportInterfaceFileInfo);
	}

	private void GetGroupImportUiDataCallBackInfo(ImvsSdkDefine.IMVS_IMPORT_GROUP_GET_UIDATA_INFO groupImportUiDataInfo)
	{
		OnImportGroupUiDataCallBack?.Invoke(groupImportUiDataInfo);
	}

	private void GetReportModuleStatusCallBackInfo(ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST reportModuleStatusInfo)
	{
		ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO[] pstModuleStatuInfo = reportModuleStatusInfo.pstModuleStatuInfo;
		for (int i = 0; i < reportModuleStatusInfo.nNum; i++)
		{
			if (1 == pstModuleStatuInfo[i].bIsSelfRun)
			{
				uint nModuleID = pstModuleStatuInfo[i].nModuleID;
				uint num = nModuleID;
				if (nModuleID < 2064)
				{
					num = nModuleID;
					mModuSelfExecuteFlag[num] = true;
				}
				if (nModuleID >= 10000 && nModuleID <= 10999)
				{
					num = nModuleID - 10000;
					mProcSelfExecuteFlag[num] = true;
				}
				if (nModuleID >= 21000 && nModuleID <= 21999)
				{
					num = nModuleID - 21000;
					mComGroupSelfExecuteFlag[num] = true;
				}
				if (nModuleID >= 101000 && nModuleID <= 105999)
				{
					num = nModuleID - 101000;
					mSinGroupSelfExecuteFlag[num] = true;
				}
				if (nModuleID >= 110000 && nModuleID <= 119999)
				{
					num = nModuleID - 110000;
					mSinModuleSelfExecuteFlag[num] = true;
				}
			}
		}
		OnReportModuleStatusCallBack?.Invoke(reportModuleStatusInfo);
	}

	private DateTime GetDateTime(long sec, int milliSec)
	{
		long ticks = sec * 10000000 + milliSec * 10000;
		TimeSpan value = new TimeSpan(ticks);
		return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1).Add(value));
	}

	public ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data PreProData(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX resultInfo, bool isFilter = false)
	{
		ImvsSdkDefine.IMVS_MODULE_RESULT_RESERVED_INFO iMVS_MODULE_RESULT_RESERVED_INFO = default;
		if (resultInfo.pstResultReservedInfo != IntPtr.Zero)
		{
			iMVS_MODULE_RESULT_RESERVED_INFO = (ImvsSdkDefine.IMVS_MODULE_RESULT_RESERVED_INFO)Marshal.PtrToStructure(resultInfo.pstResultReservedInfo, typeof(ImvsSdkDefine.IMVS_MODULE_RESULT_RESERVED_INFO));
		}
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data result = new ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data
		{
			nStatus = resultInfo.nStatus,
			nErrorCode = resultInfo.nErrorCode,
			nCycleID = resultInfo.nCycleID,
			nModuleID = resultInfo.nModuleID,
			strPubAddress = resultInfo.strPubAddress,
			fModuleTime = resultInfo.fModuleTime,
			fAlgorithmTime = resultInfo.fAlgorithmTime,
			bInCycle = resultInfo.bInCycle,
			bIsSelfExecute = resultInfo.bIsSelfExecute,
			nExecuteCount = resultInfo.nExecuteCount,
			nResultNum = resultInfo.nResultNum,
			dtEndTime = GetDateTime(iMVS_MODULE_RESULT_RESERVED_INFO.nRstTime, iMVS_MODULE_RESULT_RESERVED_INFO.nMilliSec),
			pInfo = new ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data[resultInfo.nResultNum]
		};
		ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX[] array = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX>(resultInfo.pInfo, resultInfo.nResultNum);
		for (int i = 0; i < resultInfo.nResultNum; i++)
		{
			result.pInfo[i].strParamName = StringUtil.UTF8GetString(array[i].strParamName);
			NativeData2Data(ref array[i], ref result.pInfo[i], isFilter);
		}
		return result;
	}

	public static Array GetValueFromServer(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data struResultInfo)
	{
		return struResultInfo.nParamType switch
		{
			1 => (Array)Array.ConvertAll(struResultInfo.pIntValue, (Converter<int, int?>)((int s) => s)), 
			2 => Array.ConvertAll(struResultInfo.pFloatValue, (Converter<float, float?>)((float s) => s)), 
			3 => struResultInfo.pStringValue, 
			4 => struResultInfo.pImageValue, 
			5 => struResultInfo.pChunkValue, 
			6 => struResultInfo.pByteValue, 
			7 => struResultInfo.pDoubleValue, 
			_ => null, 
		};
	}

	/// <summary>
	/// c++大数据转为软件运行所需数据
	/// </summary>
	/// <param name="nativeData"></param>
	/// <param name="targetData"></param>
	public void LargeObject(ref ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX nativeData, ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data targetData, ref bool isExcOccur)
	{
		if (targetData == null)
		{
			targetData = new ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data();
		}
		bool isMemory = nativeData.nUseRealData == 0 || nativeData.nUseRealData == 2;
		targetData.IsMemory = isMemory;
		targetData.MemoryType = nativeData.nUseRealData;
		if (nativeData.nUseRealData == 0)
		{
			targetData.MemoryAddress = nativeData.strValue;
		}
		else if (2 == nativeData.nUseRealData)
		{
			targetData.MemoryAddress = nativeData.strValue;
			targetData.MemoryPtr = nativeData.pData;
			targetData.Size = (int)nativeData.nLen;
		}
		else if (1 == nativeData.nUseRealData)
		{
			targetData.MemoryPtr = nativeData.pData;
			targetData.Size = (int)nativeData.nLen;
			targetData.FillData(ref isExcOccur);
		}
	}

	/// <summary>
	/// 将native数据转为软件运行数据
	/// </summary>
	/// <param name="nativedata"></param>
	/// <param name="targeData"></param>
	private void NativeData2Data(ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX nativedata, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data targeData, bool isFilter = false)
	{
		targeData.strParamName = StringUtil.UTF8GetString(nativedata.strParamName);
		targeData.nParamType = nativedata.nParamType;
		targeData.nValueNum = nativedata.nValueNum;
		targeData.nValueStatus = nativedata.nValueStatus;
		switch (targeData.nParamType)
		{
		case 1:
			if (nativedata.nValueStatus == 0)
			{
				int[] pIntValue = new int[1];
				targeData.pIntValue = pIntValue;
			}
			else
			{
				targeData.pIntValue = ExternalCallHelper.IntPtr2Ts<int>(nativedata.pIntValue, nativedata.nValueNum);
			}
			break;
		case 2:
			if (nativedata.nValueStatus == 0)
			{
				float[] pFloatValue = new float[1];
				targeData.pFloatValue = pFloatValue;
			}
			else
			{
				targeData.pFloatValue = ExternalCallHelper.IntPtr2Ts<float>(nativedata.pFloatValue, nativedata.nValueNum);
			}
			break;
		case 3:
		{
			if (nativedata.nValueStatus == 0)
			{
				ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[] array5 = new ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[1];
				array5[0].strValue = "";
				targeData.pStringValue = array5;
				break;
			}
			ImvsSdkDefine.IMVS_MODULE_STRING_VALUE[] array6 = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_STRING_VALUE>(nativedata.pStringValue, nativedata.nValueNum);
			targeData.pStringValue = new ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[array6.Length];
			for (int l = 0; l < array6.Length; l++)
			{
				targeData.pStringValue[l].strValue = StringUtil.UTF8GetString(array6[l].strValue);
			}
			break;
		}
		case 4:
			if (!isFilter)
			{
				bool isExcOccur = false;
				ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX[] array = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX>(nativedata.pImageValue, nativedata.nValueNum);
				targeData.pImageValue = new ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data[nativedata.nValueNum];
				for (int i = 0; i < nativedata.nValueNum; i++)
				{
					targeData.pImageValue[i] = new ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data();
					LargeObject(ref array[i], targeData.pImageValue[i], ref isExcOccur);
				}
			}
			break;
		case 5:
		{
			if (isFilter)
			{
				break;
			}
			bool isExcOccur2 = false;
			ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX[] array4 = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX>(nativedata.pChunkValue, nativedata.nValueNum);
			targeData.pChunkValue = new ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data[nativedata.nValueNum];
			for (int k = 0; k < nativedata.nValueNum; k++)
			{
				targeData.pChunkValue[k] = new ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data();
				LargeObject(ref array4[k], targeData.pChunkValue[k], ref isExcOccur2);
				if (isExcOccur2)
				{
					targeData.nValueNum = 0;
					break;
				}
			}
			break;
		}
		case 6:
		{
			ImvsSdkDefine.IMVS_MODULE_BYTEDATA_VALUE_EX[] array2 = ExternalCallHelper.PtrToStructurs<ImvsSdkDefine.IMVS_MODULE_BYTEDATA_VALUE_EX>(nativedata.pByteValue, nativedata.nValueNum);
			targeData.pByteValue = new ImvsSdkDefine.IMVS_MODULE_BYTEDATA_VALUE_EX_Data[nativedata.nValueNum];
			for (int j = 0; j < nativedata.nValueNum; j++)
			{
				try
				{
					ImvsSdkDefine.IMVS_MODULE_BYTEDATA_VALUE_EX iMVS_MODULE_BYTEDATA_VALUE_EX = array2[j];
					byte[] array3 = new byte[iMVS_MODULE_BYTEDATA_VALUE_EX.nLen];
					Marshal.Copy(iMVS_MODULE_BYTEDATA_VALUE_EX.pData, array3, 0, array3.Length);
					string empty = string.Empty;
					uint num = ((iMVS_MODULE_BYTEDATA_VALUE_EX.nLen > 4096) ? 4096u : iMVS_MODULE_BYTEDATA_VALUE_EX.nLen);
					empty = BitConverter.ToString(array3, 0, (int)num).Replace("-", string.Empty).ToUpper();
					targeData.pByteValue[j].strValue = empty;
					targeData.pByteValue[j].hexValue = array3;
				}
				catch (Exception)
				{
				}
			}
			break;
		}
		case 7:
			if (nativedata.nValueStatus == 0)
			{
				double[] pDoubleValue = new double[1];
				targeData.pDoubleValue = pDoubleValue;
			}
			break;
		}
	}

	public void SetParamValue(uint nModuleId, string strParamName, string strParamValue, bool bThrowException = false)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_SetParamValue(CVmObjectBase.mhandle, nModuleId, strParamName, strParamValue);
		if (num != 0 && bThrowException)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		string text = strParamName.ToLower();
		if (!(text == "password") && !(text == "pwd") && !(text == "key"))
		{
			LogHelper.Info($"设置参数:(ID){nModuleId} (参数名){strParamName} (参数值){strParamValue}");
		}
		ParamSeted = true;
	}

	public void SetParamValueForImageSource(uint nModuleId, string strParamName, string strParamValue)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_SetParamValueForImageSource(CVmObjectBase.mhandle, nModuleId, strParamName, strParamValue);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void GetParamValue(uint nModuleID, string strName, int nStrValueSize, ref string strValue, bool bThrowException = false)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_GetParamValue(CVmObjectBase.mhandle, nModuleID, strName, nStrValueSize, ref strValue);
		if (num != 0 && bThrowException)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void GetParamValue(uint nModuleID, string strName, ref string strValue, bool bThrowException = false)
	{
		GetParamValue(nModuleID, strName, 4096, ref strValue, bThrowException);
	}

	public void SetUIParamValue(uint nObjectID, string strParamName, byte[] data, uint nRecvWaitTime = 10000u)
	{
		if (strParamName == "IsForbidden" && data != null)
		{
			uint nEnableFlag = ((!ExternalCallHelper.BytesToStruct<bool>(data, 0, data.Length)) ? 1u : 0u);
			EnableModule(nObjectID, nEnableFlag);
			return;
		}
		int num = -536870657;
		if (data != null && data.Length != 0)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, intPtr, data.Length);
			num = ServerSDKAPI.IMVS_SetUIParamValue(CVmObjectBase.mhandle, nObjectID, strParamName, intPtr, (uint)data.Length, nRecvWaitTime);
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
		}
		else
		{
			try
			{
				DeleteUIParamValue(nObjectID, strParamName);
				num = 0;
			}
			catch (VmException ex)
			{
				num = ex.errorCode;
			}
		}
		if (num != 0)
		{
			VmException ex2 = new VmException(num);
			throw ex2;
		}
	}

	public bool CheckUIParamExist(uint nObjectID, string strParamName)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_CheckUIParamExist(CVmObjectBase.mhandle, nObjectID, strParamName);
		return num == 0;
	}

	public byte[] GetUIParamValueByKey(uint nObjectID, string strParamName)
	{
		int num = -536870657;
		IntPtr pData = IntPtr.Zero;
		uint nDataLen = 0u;
		byte[] array = null;
		num = ServerSDKAPI.IMVS_GetUIParamValueByKey(CVmObjectBase.mhandle, nObjectID, strParamName, ref pData, ref nDataLen, 10000u);
		switch (num)
		{
		case -536870911:
			return null;
		default:
		{
			VmException ex3 = new VmException(num);
			throw ex3;
		}
		case 0:
			array = new byte[nDataLen];
			if (IntPtr.Zero != pData && array != null)
			{
				try
				{
					Marshal.Copy(pData, array, 0, (int)nDataLen);
				}
				catch (Exception)
				{
				}
			}
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_BASE_ARRAY, pData);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pData = IntPtr.Zero;
			return array;
		}
	}

	/// <summary>
	/// 删除对象UI参数
	/// </summary>
	/// <param name="nObjectID"> 对象ID </param>
	/// <param name="strParamName"></param>
	/// <param name="nBlurDelete">是否模糊删除（0：否，1：是）</param>
	/// <returns></returns>
	public void DeleteUIParamValue(uint nObjectID, string strParamName, uint nBlurDelete = 0u)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteUIParamValue(CVmObjectBase.mhandle, nObjectID, strParamName, nBlurDelete);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void DeleteAllUIParamValue(uint nObjectID)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteAllUIParamValue(CVmObjectBase.mhandle, nObjectID);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST GetUIParamValueByObject(uint nObjectID)
	{
		try
		{
			int num = -536870657;
			ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST_Ex structure = default;
			IntPtr pUIObjParamInfoList = IntPtr.Zero;
			num = ServerSDKAPI.IMVS_GetUIParamValueByObject(CVmObjectBase.mhandle, nObjectID, ref pUIObjParamInfoList);
			if (num != 0)
			{
				VmException ex = new VmException(num);
				throw ex;
			}
			if (pUIObjParamInfoList != IntPtr.Zero)
			{
				ExternalCallHelper.PtrToStructureWithoutFree(pUIObjParamInfoList, ref structure);
				ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST iMVS_UI_OBJECT_PARAM_INFO_LIST = (ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST)new ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST(structure).Clone();
				iMVS_UI_OBJECT_PARAM_INFO_LIST.ResetPtr();
				num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_UI_OBJECT_PARAM_LIST, pUIObjParamInfoList);
				if (num != 0)
				{
					VmException ex2 = new VmException(num);
					throw ex2;
				}
				pUIObjParamInfoList = IntPtr.Zero;
				return iMVS_UI_OBJECT_PARAM_INFO_LIST;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public void ReleaseUIParamData()
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_ReleaseUIParamData(CVmObjectBase.mhandle);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 查询模块历史结果 数据浅拷贝 需要主动释放内存,默认10000
	/// hu:查询模块历史结果和当前结果都调用该接口,历史结果是执行多次的结果,当前结果是最后一次执行结果
	/// </summary>
	public ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST? QueryModuleHistoryResult(uint nModuleID, int nIndex, ref IntPtr pstModuleResBuff)
	{
		return QueryModuleHistoryResult(nModuleID, nIndex, 10000u, ref pstModuleResBuff);
	}

	public ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST? QueryModuleRecentlyResult(uint nModuleID, int nIndex, uint nRunCount, ref IntPtr pstModuleResBuff)
	{
		return QueryModuleHistoryResult(nModuleID, nIndex, 200u, ref pstModuleResBuff, bQueryByRunCount: true, nRunCount);
	}

	/// <summary>
	/// 查询模块最近一次数据
	/// </summary>
	/// <param name="nModuleID">模块ID</param>
	/// <param name="nRunCount">执行次数</param>
	/// <returns></returns>
	public ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST_Class QueryModuleRecentlyResult(uint nModuleID, uint nRunCount)
	{
		int num = 0;
		ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST_Class iMVS_QUERY_MODULE_RES_INFO_LIST_Class = new ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST_Class();
		ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST? iMVS_QUERY_MODULE_RES_INFO_LIST;
		do
		{
			IntPtr pstModuleResBuff = IntPtr.Zero;
			iMVS_QUERY_MODULE_RES_INFO_LIST = QueryModuleRecentlyResult(nModuleID, num, nRunCount, ref pstModuleResBuff);
			if (!iMVS_QUERY_MODULE_RES_INFO_LIST.HasValue)
			{
				iMVS_QUERY_MODULE_RES_INFO_LIST_Class.Release();
				return null;
			}
			iMVS_QUERY_MODULE_RES_INFO_LIST_Class.ls.Add(pstModuleResBuff);
			num += iMVS_QUERY_MODULE_RES_INFO_LIST.Value.nNum;
			iMVS_QUERY_MODULE_RES_INFO_LIST_Class.nNums.Add(iMVS_QUERY_MODULE_RES_INFO_LIST.Value.nNum);
			iMVS_QUERY_MODULE_RES_INFO_LIST_Class.pPbResultList.Add(iMVS_QUERY_MODULE_RES_INFO_LIST.Value.pPbResultList);
		}
		while (num < iMVS_QUERY_MODULE_RES_INFO_LIST.Value.nTotalNum);
		iMVS_QUERY_MODULE_RES_INFO_LIST_Class.nNum = num;
		return iMVS_QUERY_MODULE_RES_INFO_LIST_Class;
	}

	/// <summary>
	/// 查询模块历史结果 数据浅拷贝 需要主动释放内存
	/// </summary>
	public ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST? QueryModuleHistoryResult(uint nModuleID, int nIndex, uint nRequestNum, ref IntPtr pstModuleResBuff, bool bQueryByRunCount = false, uint nRunCount = 0u)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST structure = default;
		IntPtr pQueryModuResInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_QueryModuleHistoryResult(CVmObjectBase.mhandle, nModuleID, nIndex, nRequestNum, ref pQueryModuResInfoList, bResBuffFirst: false, bNeedImage: true, bUseResStructMap: false, bQueryByRunCount, nRunCount);
		switch (num)
		{
		case -536870123:
			return null;
		default:
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		case 0:
			if (IntPtr.Zero != pQueryModuResInfoList)
			{
				pstModuleResBuff = pQueryModuResInfoList;
				ExternalCallHelper.PtrToStructureWithoutFree(pQueryModuResInfoList, ref structure);
				return structure;
			}
			return null;
		}
	}

	/// <summary>
	/// 释放模块历史结果内存
	/// </summary>
	public int ReleaseModuleHistoryResult(ref IntPtr pstModuleResBuff)
	{
		int num = 0;
		num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_HISTORY_RES_INFO_LIST, pstModuleResBuff);
		if (num != 0)
		{
			return num;
		}
		pstModuleResBuff = IntPtr.Zero;
		return num;
	}

	/// <summary>
	/// 查询模块历史结果 数据深拷贝 效率较低
	/// </summary>
	public ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data[] QueryModuleHistoryResultData(uint nModuleID, int nIndex, uint nRequestNum)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST structure = default;
		IntPtr pQueryModuResInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_QueryModuleHistoryResult(CVmObjectBase.mhandle, nModuleID, nIndex, nRequestNum, ref pQueryModuResInfoList, bResBuffFirst: false, bNeedImage: true, bUseResStructMap: false);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (IntPtr.Zero != pQueryModuResInfoList)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pQueryModuResInfoList, ref structure);
			ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX[] array = structure.pastModuResultList();
			ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data[] array2 = new ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(array[i]);
			}
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_HISTORY_RES_INFO_LIST, pQueryModuResInfoList);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pQueryModuResInfoList = IntPtr.Zero;
			return array2;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? QueryModuleOneResult(uint nModuleID, bool bResBuffFirst = true)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST structure = default;
		IntPtr pQueryModuResInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_QueryModuleHistoryResult(CVmObjectBase.mhandle, nModuleID, 0, 1u, ref pQueryModuResInfoList, bResBuffFirst);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (pQueryModuResInfoList != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pQueryModuResInfoList, ref structure);
			if (1 == structure.nNum)
			{
				ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX[] array = structure.pastModuResultList();
				return array[0];
			}
			return null;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO? GetModuleExportData(uint nModuleID, string strDisplayName, uint nWaitTime = 5u)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO_EX structure = default;
		IntPtr pstModuExportData = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_GetModuleExportData(CVmObjectBase.mhandle, nModuleID, strDisplayName, nWaitTime, ref pstModuExportData);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (IntPtr.Zero != pstModuExportData)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuExportData, ref structure);
			ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO value = new ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO(structure);
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_EXPORT_DATA_INFO, pstModuExportData);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pstModuExportData = IntPtr.Zero;
			return value;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO? GetExportMultiModuleData(ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleIdList, uint nWaitTime = 5u)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO_EX structure = default;
		IntPtr pstModuExportData = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_ExportMultiModuleData(CVmObjectBase.mhandle, ref stModuleIdList, nWaitTime, ref pstModuExportData);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (IntPtr.Zero != pstModuExportData)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuExportData, ref structure);
			ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO value = new ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO(structure);
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_EXPORT_DATA_INFO, pstModuExportData);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pstModuExportData = IntPtr.Zero;
			return value;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_MODULE_INFO ImportModuleDataEx(ImvsSdkDefine.IMVS_IMPORT_MODULE_DATA_INFO stModuImportData)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo = default;
		num = ServerSDKAPI.IMVS_ImportModuleDataEx(CVmObjectBase.mhandle, stModuImportData, ref pstModuleInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		return pstModuleInfo;
	}

	public ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO_LIST GetMultiModuleConnect(ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO_LIST_EX structure = default;
		IntPtr pstModuConnectInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_GetMultiModuleConnect(CVmObjectBase.mhandle, ref stModuleId, ref pstModuConnectInfoList);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO_LIST result = default;
		if (pstModuConnectInfoList != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuConnectInfoList, ref structure);
			result = new ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO_LIST(structure);
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_CONNECT_INFO_LIST, pstModuConnectInfoList);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pstModuConnectInfoList = IntPtr.Zero;
		}
		return result;
	}

	public ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST GetMultiModuleSubscribe(ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST_Ex structure = default;
		IntPtr pstModuSubInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_GetMultiModuleSubscribe(CVmObjectBase.mhandle, ref stModuleId, ref pstModuSubInfoList);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = new ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST();
		if (pstModuSubInfoList != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuSubInfoList, ref structure);
			iMVS_MODULE_SUBSCRIBE_INFO_LIST = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)new ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST(structure).Clone();
			iMVS_MODULE_SUBSCRIBE_INFO_LIST.ResetPtr();
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_SUBSCRIBE_INFO_LIST, pstModuSubInfoList);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pstModuSubInfoList = IntPtr.Zero;
		}
		return iMVS_MODULE_SUBSCRIBE_INFO_LIST;
	}

	public ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST QueryModuleIOBeSubscribed(uint nModuleId, string strIOName)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST_Ex structure = default;
		IntPtr pstIOSubInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_QueryIOBeSubscribed(CVmObjectBase.mhandle, nModuleId, strIOName, ImvsSdkDefine.IMVS_IO_BE_SUBSCRIBED_TYPE.IO_BE_SUBSCRIBED_TYPE_GENARAL, ref pstIOSubInfoList);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = new ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST();
		if (pstIOSubInfoList != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstIOSubInfoList, ref structure);
			iMVS_MODULE_SUBSCRIBE_INFO_LIST = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)new ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST(structure).Clone();
			iMVS_MODULE_SUBSCRIBE_INFO_LIST.ResetPtr();
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_SUBSCRIBE_INFO_LIST, pstIOSubInfoList);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pstIOSubInfoList = IntPtr.Zero;
		}
		return iMVS_MODULE_SUBSCRIBE_INFO_LIST;
	}

	public ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST QueryModuleIOBeSubscribedByRunParams(uint nModuleId, string strIOName)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST_Ex structure = default;
		IntPtr pstIOSubInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_QueryIOBeSubscribed(CVmObjectBase.mhandle, nModuleId, strIOName, ImvsSdkDefine.IMVS_IO_BE_SUBSCRIBED_TYPE.IO_BE_SUBSCRIBED_TYPE_PARAMS, ref pstIOSubInfoList);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = new ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST();
		if (pstIOSubInfoList != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstIOSubInfoList, ref structure);
			iMVS_MODULE_SUBSCRIBE_INFO_LIST = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)new ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST(structure).Clone();
			iMVS_MODULE_SUBSCRIBE_INFO_LIST.ResetPtr();
			num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_MODU_SUBSCRIBE_INFO_LIST, pstIOSubInfoList);
			if (num != 0)
			{
				VmException ex2 = new VmException(num);
				throw ex2;
			}
			pstIOSubInfoList = IntPtr.Zero;
		}
		return iMVS_MODULE_SUBSCRIBE_INFO_LIST;
	}

	public void DeleteIOBeSubscribedByParams(uint nModuleId, string strIOName)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteIOBeSubscribedByParams(CVmObjectBase.mhandle, nModuleId, strIOName);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void AddGroupSonModule(ImvsSdkDefine.IMVS_GROUP_MODULE_INFO stGroupModuInfo)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_AddGroupSonModule(CVmObjectBase.mhandle, stGroupModuInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void MoveToGroup(uint groupId, uint moduleId)
	{
		ImvsSdkDefine.IMVS_GROUP_MODULE_INFO pstGroupModuInfo = new ImvsSdkDefine.IMVS_GROUP_MODULE_INFO
		{
			nGroupId = groupId,
			nSonModuNum = 1,
			anModuleId = new uint[2064]
		};
		pstGroupModuInfo.anModuleId[0] = moduleId;
		pstGroupModuInfo.nReserved = new uint[4];
		int num = ServerSDKAPI.IMVS_AddGroupSonModule(CVmObjectBase.mhandle, pstGroupModuInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void RemoveGroupSonModule(uint groupId, uint sonId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_GROUP_MODULE_INFO pstGroupModuInfo = new ImvsSdkDefine.IMVS_GROUP_MODULE_INFO
		{
			nGroupId = groupId,
			nSonModuNum = 1,
			anModuleId = new uint[2064]
		};
		pstGroupModuInfo.anModuleId[0] = sonId;
		pstGroupModuInfo.nReserved = new uint[4];
		num = ServerSDKAPI.IMVS_RemoveGroupSonModule(CVmObjectBase.mhandle, pstGroupModuInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void RemoveGroupSonModule(ImvsSdkDefine.IMVS_GROUP_MODULE_INFO stGroupModuInfo)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_RemoveGroupSonModule(CVmObjectBase.mhandle, stGroupModuInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void DeleteGroupModule(uint nGroupModuleID)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteGroupModule(CVmObjectBase.mhandle, nGroupModuleID);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void ExportGroupModule(uint nGroupId, uint nGroupLevel, string strPath, string strPassword)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_ExportGroupModule(CVmObjectBase.mhandle, nGroupId, nGroupLevel, strPath, strPassword);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void SyncExportGroupModule(uint nGroupId, uint nGroupLevel, string strPath, string strPassword)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_SyncExportGroupModule(CVmObjectBase.mhandle, nGroupId, nGroupLevel, strPath, strPassword);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void ImportGroupModule(uint nProcessID, uint nGroupID, string strPath, string strPassword, uint nIdAllocateWay)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_ImportGroupModule(CVmObjectBase.mhandle, nProcessID, nGroupID, strPath, strPassword, nIdAllocateWay);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_MODULE_INFO ImportIndependentGroup(uint nProcessID, string strPath, string strPassword, uint nIdAllocateWay)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo = default;
		num = ServerSDKAPI.IMVS_ImportIndependentGroup(CVmObjectBase.mhandle, nProcessID, strPath, strPassword, nIdAllocateWay, ref pstModuleInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		return pstModuleInfo;
	}

	public ImvsSdkDefine.IMVS_MODULE_INFO SyncImportGroupModule(uint nProcessID, uint nGroupID, string strPath, string strPassword, uint nIdAllocateWay)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_INFO pstModuleInfo = default;
		num = ServerSDKAPI.IMVS_SyncImportGroupModule(CVmObjectBase.mhandle, nProcessID, nGroupID, strPath, strPassword, nIdAllocateWay, ref pstModuleInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		return pstModuleInfo;
	}

	public void StopCallBackModuResult(uint nModuleId)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_CtrlCallBackModuResult(CVmObjectBase.mhandle, nModuleId, 0u);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void EnableCallBackModuResult(uint nModuleId)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_CtrlCallBackModuResult(CVmObjectBase.mhandle, nModuleId, 1u);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void CopyModuleAlgParam(uint nSrcModuID, uint nDstModuID)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_CopyModuleAlgParam(CVmObjectBase.mhandle, nSrcModuID, nDstModuID);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void CopyModuleParam(uint nSrcModuID, uint nDstModuID)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_CopyModuleParam(CVmObjectBase.mhandle, nSrcModuID, nDstModuID);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public int GetBinaryData(uint moduleId, string paramName, ref byte[] data, uint recvTimeOut = 3000u)
	{
		ImvsSdkDefine.IMVS_GET_BINARY_LENGTH_INFO strValue = new ImvsSdkDefine.IMVS_GET_BINARY_LENGTH_INFO
		{
			strName = StringUtil.UTF8GetFixLenBytes(paramName, 256)
		};
		int num = ServerSDKAPI.IMVS_GetBinaryLength(CVmObjectBase.mhandle, moduleId, ref strValue, recvTimeOut);
		if (num != 0)
		{
			return num;
		}
		if (strValue.nBinaryLength == 0)
		{
			return -536870911;
		}
		ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue2 = new ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO
		{
			strName = StringUtil.UTF8GetFixLenBytes(paramName, 256),
			nBinaryDataMallocSize = strValue.nBinaryLength,
			nBinaryLength = strValue.nBinaryLength
		};
		data = new byte[strValue.nBinaryLength];
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		try
		{
			strValue2.pBinaryData = gCHandle.AddrOfPinnedObject();
			if (!gCHandle.IsAllocated)
			{
				return -536870911;
			}
			num = ServerSDKAPI.IMVS_GetBinaryData(CVmObjectBase.mhandle, moduleId, ref strValue2, recvTimeOut);
			if (num != 0)
			{
				data = null;
				return num;
			}
			gCHandle.Free();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
		}
		return 0;
	}

	public int GetBinaryDataForDataqueue(uint moduleId, int nMallocSize, string paramName, ref byte[] data, uint recvTimeOut = 3000u)
	{
		try
		{
			int num = -536870657;
			if (nMallocSize <= 0)
			{
				return -536870911;
			}
			ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue = new ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO
			{
				strName = StringUtil.UTF8GetFixLenBytes(paramName, 256),
				nBinaryDataMallocSize = (uint)nMallocSize,
				nBinaryLength = (uint)nMallocSize
			};
			data = new byte[nMallocSize];
			GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				strValue.pBinaryData = gCHandle.AddrOfPinnedObject();
				if (!gCHandle.IsAllocated)
				{
					return -536870911;
				}
				num = ServerSDKAPI.IMVS_GetBinaryData(CVmObjectBase.mhandle, moduleId, ref strValue, recvTimeOut);
				if (num != 0)
				{
					data = null;
					return num;
				}
				gCHandle.Free();
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (gCHandle.IsAllocated)
				{
					gCHandle.Free();
				}
			}
		}
		catch (Exception)
		{
			return -536870910;
		}
		return 0;
	}

	public int GetBinaryData(uint moduleId, string paramName, out IntPtr data, uint recvTimeOut = 3000u)
	{
		data = IntPtr.Zero;
		ImvsSdkDefine.IMVS_GET_BINARY_LENGTH_INFO strValue = new ImvsSdkDefine.IMVS_GET_BINARY_LENGTH_INFO
		{
			strName = StringUtil.UTF8GetFixLenBytes(paramName, 256)
		};
		if (ServerSDKAPI.IMVS_GetBinaryLength(CVmObjectBase.mhandle, moduleId, ref strValue, recvTimeOut) != 0)
		{
			return -536870911;
		}
		if (strValue.nBinaryLength == 0)
		{
			return -536870894;
		}
		ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO strValue2 = new ImvsSdkDefine.IMVS_GET_BINARY_DATA_INFO
		{
			strName = StringUtil.UTF8GetFixLenBytes(paramName, 256),
			nBinaryDataMallocSize = strValue.nBinaryLength,
			nBinaryLength = strValue.nBinaryLength
		};
		data = Marshal.AllocHGlobal((int)strValue.nBinaryLength);
		strValue2.pBinaryData = data;
		if (ServerSDKAPI.IMVS_GetBinaryData(CVmObjectBase.mhandle, moduleId, ref strValue2, recvTimeOut) != 0)
		{
			return -536870911;
		}
		return (int)strValue.nBinaryLength;
	}

	public int SetBinaryData(uint moduleId, string paramName, IntPtr dataPtr, int len, uint recvTimeOut = 3000u)
	{
		ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO strValue = new ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO
		{
			pBinaryData = dataPtr,
			nBinaryLenth = (uint)len,
			strName = StringUtil.UTF8GetFixLenBytes(paramName, 256)
		};
		int num = ServerSDKAPI.IMVS_SetBinaryData(CVmObjectBase.mhandle, moduleId, ref strValue, recvTimeOut);
		if (num == 0)
		{
			ParamSeted = true;
		}
		return num;
	}

	public int SetBinaryData(uint moduleId, string paramName, byte[] data, uint recvTimeOut = 3000u)
	{
		if (data == null)
		{
			return -536870911;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(data.Length);
		Marshal.Copy(data, 0, intPtr, data.Length);
		int num = SetBinaryData(moduleId, paramName, intPtr, data.Length, recvTimeOut);
		Marshal.FreeHGlobal(intPtr);
		if (num == 0)
		{
			ParamSeted = true;
		}
		return num;
	}

	public ImvsSdkDefine.IMVS_MODULE_PARAM_LIST? GetParamList(uint nModuleID)
	{
		ImvsSdkDefine.IMVS_MODULE_PARAM_LIST structure = default;
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_PARAM_LIST)));
		if (IntPtr.Zero != intPtr)
		{
			Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
			int num = -536870657;
			num = ServerSDKAPI.IMVS_GetParamList(CVmObjectBase.mhandle, nModuleID, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			structure = (ImvsSdkDefine.IMVS_MODULE_PARAM_LIST)Marshal.PtrToStructure(intPtr, typeof(ImvsSdkDefine.IMVS_MODULE_PARAM_LIST));
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
			return structure;
		}
		return null;
	}

	public void SetParamList(uint nModuleID, ImvsSdkDefine.IMVS_MODULE_PARAM_LIST stParamList)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_PARAM_LIST)));
		if (IntPtr.Zero != intPtr)
		{
			Marshal.StructureToPtr(stParamList, intPtr, fDeleteOld: true);
			int num = ServerSDKAPI.IMVS_SetParamList(CVmObjectBase.mhandle, nModuleID, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
			return;
		}
		VmException ex2 = new VmException(-536870910);
		throw ex2;
	}

	/// <summary>
	/// 通知组模块更改输入输出
	/// </summary>
	/// <param name="stGroupIOInfoList"></param>
	public void SetGroupIO(ImvsSdkDefine.IMVS_GROUP_IO_INFO_LIST stGroupIOInfoList)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_SetGroupIO(CVmObjectBase.mhandle, stGroupIOInfoList);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_NODE_TREE_INFO? GetFirstLayerModuleTree(uint nObjectID, uint nObjectType)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_NODE_TREE_INFO structure = default;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_NODE_TREE_INFO)));
		if (intPtr != IntPtr.Zero)
		{
			num = ServerSDKAPI.IMVS_GetFirstLayerModuleTree(CVmObjectBase.mhandle, nObjectID, nObjectType, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			ExternalCallHelper.PtrToStructure(intPtr, ref structure);
			return structure;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_NODE_TREE_INFO? GetFirstLayerModuleTreeEx(uint nObjectID)
	{
		int num = -536870657;
		uint nObjectType = 0u;
		if (nObjectID >= 10000 && nObjectID <= 10999)
		{
			nObjectType = 1u;
		}
		else if ((nObjectID >= 21000 && nObjectID <= 21999) || (nObjectID >= 101000 && nObjectID <= 105999))
		{
			nObjectType = 20u;
		}
		ImvsSdkDefine.IMVS_NODE_TREE_INFO structure = default;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_NODE_TREE_INFO)));
		if (intPtr != IntPtr.Zero)
		{
			num = ServerSDKAPI.IMVS_GetFirstLayerModuleTree(CVmObjectBase.mhandle, nObjectID, nObjectType, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			ExternalCallHelper.PtrToStructure(intPtr, ref structure);
			return structure;
		}
		return null;
	}

	public void CtrlCallBackModuResult(uint nModuleID, uint nCallbackStatus)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_PF_CtrlCallBackModuResult(CVmObjectBase.mhandle, nModuleID, nCallbackStatus);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_UIPARAM_INFO_LIST FuzzyQueryUIParamValue(ImvsSdkDefine.IMVS_MODULE_ID_LIST astModuleId, string strParamName, bool bIsFuzzyQuery)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_UIPARAM_INFO_LIST result = default;
		IntPtr pUiParamInfoList = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_FuzzyQueryUIParamValue(CVmObjectBase.mhandle, ref astModuleId, strParamName, bIsFuzzyQuery, ref pUiParamInfoList);
		switch (num)
		{
		case -536870911:
			return result;
		default:
		{
			VmException ex2 = new VmException(num);
			throw ex2;
		}
		case 0:
			if (pUiParamInfoList != IntPtr.Zero)
			{
				ImvsSdkDefine.IMVS_UIPARAM_INFO_LIST_EX structure = default;
				ExternalCallHelper.PtrToStructureWithoutFree(pUiParamInfoList, ref structure);
				result = new ImvsSdkDefine.IMVS_UIPARAM_INFO_LIST(structure);
				num = ServerSDKAPI.IMVS_ReleaseDynamicMemory(CVmObjectBase.mhandle, ImvsSdkDefine.IMVS_RELEASE_MEMORY_TYPE.RELEASE_MEMORY_STRUCT_UI_PARAM_INFO_LIST, pUiParamInfoList);
				if (num != 0)
				{
					VmException ex = new VmException(num);
					throw ex;
				}
				pUiParamInfoList = IntPtr.Zero;
			}
			return result;
		}
	}

	public void SetImageBufferNodeNum(uint nModuleId, uint nBufferNodeNum)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_PF_SetImageBufferNodeNum(CVmObjectBase.mhandle, nModuleId, nBufferNodeNum);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void KeepModuleLastResult(uint nModuleId, bool bIsUnchanged)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_KeepModuleLastResult(CVmObjectBase.mhandle, nModuleId, bIsUnchanged);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void SetImageData(ImvsSdkDefine.IMVS_PF_INPUT_IMAGE_INFO stInputImageInfo)
	{
		int num = ServerSDKAPI.IMVS_PF_SetImageData(CVmObjectBase.mhandle, stInputImageInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_MODULE_STATE_LIST? GetMultiObjectLoadState(ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_STATE_LIST structure = default;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_STATE_LIST)));
		if (IntPtr.Zero == intPtr)
		{
			VmException ex = new VmException(-536870909);
			throw ex;
		}
		num = ServerSDKAPI.IMVS_GetMultiObjectLoadState(CVmObjectBase.mhandle, ref stModuleId, intPtr);
		if (num != 0)
		{
			Marshal.FreeHGlobal(intPtr);
			VmException ex2 = new VmException(num);
			throw ex2;
		}
		ExternalCallHelper.PtrToStructure(intPtr, ref structure);
		return structure;
	}

	public ImvsSdkDefine.IMVS_MODULE_STATE_LIST? GetMultiModuEnableState(ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_STATE_LIST structure = default;
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_STATE_LIST)));
		if (IntPtr.Zero == intPtr)
		{
			VmException ex = new VmException(-536870909);
			throw ex;
		}
		num = ServerSDKAPI.IMVS_GetMultiModuEnableState(CVmObjectBase.mhandle, ref stModuleId, intPtr);
		if (num != 0)
		{
			Marshal.FreeHGlobal(intPtr);
			VmException ex2 = new VmException(num);
			throw ex2;
		}
		ExternalCallHelper.PtrToStructure(intPtr, ref structure);
		return structure;
	}

	public string GetGroVersion(string strPath, string strPassWord)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_FILE_VERSION_INFO pstVersionInfo = default;
		num = ServerSDKAPI.IMVS_GetGroVersion(CVmObjectBase.mhandle, strPath, strPassWord, ref pstVersionInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		return pstVersionInfo.strCurrentVesionInfo;
	}

	public ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST GetModuInputIOInfo(uint nModuId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST structure = new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST();
		IntPtr pstModuIOInfo = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_GetModuInputIOInfo(CVmObjectBase.mhandle, nModuId, ref pstModuIOInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (pstModuIOInfo != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuIOInfo, ref structure);
			Marshal.FreeHGlobal(pstModuIOInfo);
			pstModuIOInfo = IntPtr.Zero;
			return structure;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST GetModuOutputIOInfo(uint nModuId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST structure = new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST();
		IntPtr pstModuIOInfo = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_GetModuOutputIOInfo(CVmObjectBase.mhandle, nModuId, 2u, ref pstModuIOInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (pstModuIOInfo != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuIOInfo, ref structure);
			Marshal.FreeHGlobal(pstModuIOInfo);
			pstModuIOInfo = IntPtr.Zero;
			return structure;
		}
		return null;
	}

	public ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST GetModuDisplayIOInfo(uint nModuId)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST structure = new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST();
		IntPtr pstModuIOInfo = IntPtr.Zero;
		num = ServerSDKAPI.IMVS_GetModuOutputIOInfo(CVmObjectBase.mhandle, nModuId, 3u, ref pstModuIOInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		if (pstModuIOInfo != IntPtr.Zero)
		{
			ExternalCallHelper.PtrToStructureWithoutFree(pstModuIOInfo, ref structure);
			Marshal.FreeHGlobal(pstModuIOInfo);
			pstModuIOInfo = IntPtr.Zero;
			return structure;
		}
		return null;
	}

	public void SetModuInputIOInfo(uint nObjectId, ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST stModuInfo)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST)));
		if (IntPtr.Zero != intPtr)
		{
			Marshal.StructureToPtr(stModuInfo, intPtr, fDeleteOld: true);
			int num = ServerSDKAPI.IMVS_SetModuInputIOInfo(CVmObjectBase.mhandle, nObjectId, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			GC.KeepAlive(stModuInfo);
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
			return;
		}
		VmException ex2 = new VmException(-536870910);
		throw ex2;
	}

	public void SetModuOutputIOInfo(uint nObjectId, ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST stOutputInfo)
	{
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST)));
		if (IntPtr.Zero != intPtr)
		{
			Marshal.StructureToPtr(stOutputInfo, intPtr, fDeleteOld: true);
			int num = ServerSDKAPI.IMVS_SetModuOutputIOInfo(CVmObjectBase.mhandle, nObjectId, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			GC.KeepAlive(stOutputInfo);
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
			return;
		}
		VmException ex2 = new VmException(-536870910);
		throw ex2;
	}

	public void SetModuleParamBinding(ImvsSdkDefine.IMVS_BIND_MODULE_PARAM_INFO_LIST stModuleVarInfo)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_SetModuleParamBinding(CVmObjectBase.mhandle, stModuleVarInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public void DelModuleParamBinding(ImvsSdkDefine.IMVS_UNBIND_MODULE_PARAM_INFO_LIST stModuleVarInfo)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DelModuleParamBinding(CVmObjectBase.mhandle, stModuleVarInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	public ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST? GetModuleParamBindingList(uint nModuleId, int nIndex, int nQueNum)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_GET_BIND_MODULE_PARAM_INFO_LIST stModuleVarInfo = default;
		num = ServerSDKAPI.IMVS_GetModuleParamBindingList(CVmObjectBase.mhandle, nModuleId, nIndex, nQueNum, ref stModuleVarInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		return stModuleVarInfo;
	}

	/// <summary>
	/// 添加动态参数
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	public void AddDynamicParam(uint nObjectId, string strName)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_AddDynamicParam(CVmObjectBase.mhandle, nObjectId, strName);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 修改动态参数
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strNameOld">参数名</param>
	/// <param name="strNameNew">参数名</param>
	public void ModifyDynamicParam(uint nObjectId, string strNameOld, string strNameNew)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_ModifyDynamicParam(CVmObjectBase.mhandle, nObjectId, strNameOld, strNameNew);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 删除动态参数
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	public void DeleteDynamicParam(uint nObjectId, string strName)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteDynamicParam(CVmObjectBase.mhandle, nObjectId, strName);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 获取所有动态参数
	/// </summary>
	/// <param name="nObjectId">ID</param>
	public ImvsSdkDefine.IMVS_MODULE_PARAM_LIST? GetAllDynamicParam(uint nObjectId)
	{
		ImvsSdkDefine.IMVS_MODULE_PARAM_LIST structure = default;
		IntPtr intPtr = new IntPtr(0);
		intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_PARAM_LIST)));
		if (IntPtr.Zero != intPtr)
		{
			Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
			int num = -536870657;
			num = ServerSDKAPI.IMVS_GetAllDynamicParam(CVmObjectBase.mhandle, nObjectId, intPtr);
			if (num != 0)
			{
				Marshal.FreeHGlobal(intPtr);
				intPtr = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			structure = (ImvsSdkDefine.IMVS_MODULE_PARAM_LIST)Marshal.PtrToStructure(intPtr, typeof(ImvsSdkDefine.IMVS_MODULE_PARAM_LIST));
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
			return structure;
		}
		return null;
	}

	/// <summary>
	/// 添加动态参数关联关系
	/// </summary>
	/// <param name="nDestinID">目标ID</param>
	/// <param name="strDestinName">目标参数名</param>
	/// <param name="nSourceID">来源ID</param>
	/// <param name="strSourceName">来源参数名</param>
	public void AddDynamicParamRelation(uint nDestinID, string strDestinName, uint nSourceID, string strSourceName)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_AddDynamicParamRelation(CVmObjectBase.mhandle, nDestinID, strDestinName, nSourceID, strSourceName);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 获取动态参数关联关系
	/// </summary>
	/// <param name="nObjectID">ID</param>
	/// <param name="strName">参数名</param>
	/// <returns></returns>
	public ImvsSdkDefine.IMVS_DYNAMIC_PARAM_RELATION_LIST? GetDynamicParamRelationByKey(uint nObjectID, string strName)
	{
		int num = -536870657;
		ImvsSdkDefine.IMVS_DYNAMIC_PARAM_RELATION_LIST structure = default;
		IntPtr zero = IntPtr.Zero;
		zero = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_DYNAMIC_PARAM_RELATION_LIST)));
		if (IntPtr.Zero != zero)
		{
			Marshal.StructureToPtr(structure, zero, fDeleteOld: true);
			num = ServerSDKAPI.IMVS_GetDynamicParamRelationByKey(CVmObjectBase.mhandle, nObjectID, strName, zero);
			if (num != 0)
			{
				Marshal.FreeHGlobal(zero);
				zero = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			structure = (ImvsSdkDefine.IMVS_DYNAMIC_PARAM_RELATION_LIST)Marshal.PtrToStructure(zero, typeof(ImvsSdkDefine.IMVS_DYNAMIC_PARAM_RELATION_LIST));
			Marshal.FreeHGlobal(zero);
			zero = IntPtr.Zero;
			return structure;
		}
		return null;
	}

	/// <summary>
	/// 删除动态参数关联关系
	/// </summary>
	/// <param name="nDestinID">目标ID</param>
	/// <param name="strDestinName">目标参数名</param>
	/// <param name="nSourceID">来源ID</param>
	/// <param name="strSourceName">来源参数名</param>
	public void DeleteDynamicParamRelation(uint nDestinID, string strDestinName, uint nSourceID, string strSourceName)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteDynamicParamRelation(CVmObjectBase.mhandle, nDestinID, strDestinName, nSourceID, strSourceName);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 设置动态运行参数值
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	/// <param name="strValue">参数值</param>
	public void SetDynamicRunParamValue(uint nObjectId, string strName, string strValue, bool bThrowException = false, string strOptionalParam = "")
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_SetDynamicRunParamValue(CVmObjectBase.mhandle, nObjectId, strName, strValue, strOptionalParam);
		if (num != 0 && bThrowException)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 获取动态运行参数值(参数值的默认分配大小4096)
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	public void GetDynamicRunParamValue(uint nObjectId, string strName, ref string strValue, bool bThrowException = false, string strOptionalParam = "")
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_GetDynamicRunParamValue(CVmObjectBase.mhandle, nObjectId, strName, ref strValue, strOptionalParam);
		if (num != 0 && bThrowException)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 设置动态二进制参数值
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	/// <param name="data">参数数据</param>
	public int SetDynamicBinaryParamValue(uint nObjectId, string strName, byte[] data, string strOptionalParam = "")
	{
		if (data == null)
		{
			return -536870911;
		}
		int num = -536870657;
		IntPtr intPtr = Marshal.AllocHGlobal(data.Length);
		Marshal.Copy(data, 0, intPtr, data.Length);
		num = SetDynamicBinaryParamValue(nObjectId, strName, intPtr, data.Length, strOptionalParam);
		Marshal.FreeHGlobal(intPtr);
		intPtr = IntPtr.Zero;
		return num;
	}

	/// <summary>
	/// 设置动态二进制参数值
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	/// <param name="data">参数数据</param>
	public int SetDynamicBinaryParamValue(uint nObjectId, string strName, IntPtr pData, int dataLen, string strOptionalParam = "")
	{
		int num = -536870657;
		return ServerSDKAPI.IMVS_SetDynamicBinaryParamValue(CVmObjectBase.mhandle, nObjectId, strName, pData, (uint)dataLen, strOptionalParam);
	}

	/// <summary>
	/// 获取动态二进制参数值
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	/// <param name="data">参数值</param>
	public void GetDynamicBinaryParamValue(uint nObjectId, string strName, ref byte[] data, string strOptionalParam = "")
	{
		int num = -536870657;
		uint nDataLen = 1024u;
		num = ServerSDKAPI.IMVS_GetDynamicBinaryParamValue(CVmObjectBase.mhandle, nObjectId, strName, ref data, ref nDataLen, strOptionalParam);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 设置动态UI参数值
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	/// <param name="pData">参数数据</param>
	/// <param name="nDataLen">数据长度</param>
	public void SetDynamicUIParamValue(uint nObjectId, string strName, byte[] data, string strOptionalParam = "")
	{
		int num = -536870657;
		if (data != null && data.Length != 0)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, intPtr, data.Length);
			num = ServerSDKAPI.IMVS_SetDynamicUIParamValue(CVmObjectBase.mhandle, nObjectId, strName, intPtr, (uint)data.Length, strOptionalParam);
			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;
		}
		else
		{
			try
			{
				DeleteDynamicRelateUIParamValue(nObjectId, strName, strOptionalParam);
				num = 0;
			}
			catch (VmException ex)
			{
				num = ex.errorCode;
			}
		}
		if (num != 0)
		{
			VmException ex2 = new VmException(num);
			throw ex2;
		}
	}

	/// <summary>
	/// 获取动态UI参数值
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">参数名</param>
	public byte[] GetDynamicUIParamValue(uint nObjectId, string strName, string strOptionalParam = "")
	{
		int num = -536870657;
		byte[] data = null;
		uint nDataLen = 0u;
		num = ServerSDKAPI.IMVS_GetDynamicUIParamValue(CVmObjectBase.mhandle, nObjectId, strName, ref data, ref nDataLen, strOptionalParam);
		switch (num)
		{
		case -536870911:
			return null;
		default:
		{
			VmException ex = new VmException(num);
			throw ex;
		}
		case 0:
			return data;
		}
	}

	/// <summary>
	/// 删除动态关联的Userdata
	/// </summary>
	/// <param name="nObjectId">ID</param>
	/// <param name="strName">对象Key</param>
	/// <param name="strOptionalParam">模块参数名称（不可为空）</param>
	public void DeleteDynamicRelateUIParamValue(uint nObjectId, string strName, string strOptionalParam)
	{
		int num = -536870657;
		num = ServerSDKAPI.IMVS_DeleteDynamicRelateUIParamValue(CVmObjectBase.mhandle, nObjectId, strName, ImvsSdkDefine.IMVS_FUZZY_MATCH.FUZZY_MATCH_NO, strOptionalParam);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 设置变量模块是否上报值修改事件
	/// </summary>
	/// <param name="nModuleID">模块id</param>
	/// <param name="nState">状态</param>
	public void SetVarReportState(uint nModuleID, int nState)
	{
		int num = -536870657;
		num = ServerSDKAPI.SetVarReportState(CVmObjectBase.mhandle, nModuleID, nState);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}

	/// <summary>
	/// 校验ID是否局部变量模块
	/// </summary>
	public bool CheckLocalVariableModule(int nVarModuleID)
	{
		bool flag = (long)nVarModuleID >= 24000L && (long)nVarModuleID <= 25999L;
		bool flag2 = nVarModuleID >= 31000 && nVarModuleID <= 33999;
		return (flag | flag2) || (nVarModuleID >= 105000 && nVarModuleID <= 108999);
	}

	/// <summary>
	/// 设置Group输出IO模式列表
	/// </summary>
	/// <param name="stGroupIOModeList"></param>
	public void SetGroupIOReportMode(ImvsSdkDefine.IMVS_GROUP_IO_REPORT_MODE_INFO_LIST stGroupIOModeList)
	{
		IntPtr zero = IntPtr.Zero;
		zero = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_GROUP_IO_REPORT_MODE_INFO_LIST)));
		if (zero != IntPtr.Zero)
		{
			Marshal.StructureToPtr(stGroupIOModeList, zero, fDeleteOld: true);
			int num = ServerSDKAPI.IMVS_SetGroupIOReportMode(CVmObjectBase.mhandle, zero);
			Marshal.FreeHGlobal(zero);
			zero = IntPtr.Zero;
			if (num != 0)
			{
				VmException ex = new VmException(num);
				throw ex;
			}
		}
	}

	/// <summary>
	/// 获取Group输出IO模式列表
	/// </summary>
	/// <param name="stGroupIOModeList"></param>
	public void GetGroupIOReportMode(ref ImvsSdkDefine.IMVS_GROUP_IO_REPORT_MODE_INFO_LIST stGroupIOModeList)
	{
		IntPtr zero = IntPtr.Zero;
		zero = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_GROUP_IO_REPORT_MODE_INFO_LIST)));
		if (zero != IntPtr.Zero)
		{
			Marshal.StructureToPtr(stGroupIOModeList, zero, fDeleteOld: true);
			int num = ServerSDKAPI.IMVS_GetGroupIOReportMode(CVmObjectBase.mhandle, zero);
			if (num != 0)
			{
				Marshal.FreeHGlobal(zero);
				zero = IntPtr.Zero;
				VmException ex = new VmException(num);
				throw ex;
			}
			ExternalCallHelper.PtrToStructure(zero, ref stGroupIOModeList);
		}
	}

	public void SetImageData3D(ImvsSdkDefine.IMVS_PF_INPUT_IMAGE_3D_INFO stInputImageInfo)
	{
		int num = ServerSDKAPI.IMVS_PF_SetImageData3D(CVmObjectBase.mhandle, stInputImageInfo);
		if (num != 0)
		{
			VmException ex = new VmException(num);
			throw ex;
		}
	}
}
