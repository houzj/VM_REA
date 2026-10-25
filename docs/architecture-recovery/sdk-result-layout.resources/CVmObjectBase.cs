using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Apps.Log;
using VM.Utility;

namespace VM.PlatformSDKCS;

public class CVmObjectBase
{
	/// <summary>
	/// 操作句柄 | Handle
	/// </summary>
	public static IntPtr mhandle;

	private static delegateOutputCallBack PlatformInfoCallBack;

	/// <summary>
	/// 模块ID列表 | The list of module id
	/// </summary>
	protected static List<uint> moduleIdList;

	/// <summary>
	/// 模块结果列表 | The list of module results
	/// </summary>
	protected static List<CModuleResultBase> moduleResultList;

	/// <summary>
	/// 模块名称映射表 | Module name map
	/// </summary>
	protected static Dictionary<string, int> moduleNameIndexMap;

	/// <summary>
	/// 是否二次开发模式 | Is SDK
	/// </summary>
	protected static bool IsSDK;

	private const string GlobalScriptExeName = "GlobalScript";

	private static readonly object syncObj;

	/// <summary>
	/// 只读静态实例,保证唯一性
	/// </summary>
	private static readonly CVmObjectBase m_VmBase;

	/// <summary>
	/// 服务SDK管理单例
	/// </summary>
	public static CVmObjectBase VmBaseInstance => m_VmBase;

	/// <summary>
	/// 模块注册
	/// </summary>
	public static event ModuleRegisterCallBack OnModuleRegisterCallBack;

	/// <summary>
	/// 流程注册
	/// </summary>
	public static event ProcessRegisterCallBack OnProcessRegisterCallBack;

	/// <summary>
	/// 模块更新
	/// </summary>
	public static event ModuleRegisterCallBack OnModuleUpDateCallBack;

	/// <summary>
	/// 模块解注册
	/// </summary>
	public static event ModuleUnRegisterCallBack OnModuleUnRegisterCallBack;

	/// <summary>
	/// 结果回调
	/// </summary>
	public static event ServerResultCallBack OnServerResultCallBack;

	/// <summary>
	/// 加密狗回调
	/// </summary>
	public static event DongleCallBack OnDongleCallBack;

	/// <summary>
	/// 服务状态回调
	/// </summary>
	public static event ServerStatusCallBack OnServerStatusCallBack;

	/// <summary>
	/// 执行结果回调
	/// </summary>
	public static event ModuleExtraInfoCallBack OnModuleExtraInfoCallBack;

	/// <summary>
	/// 模块状态回调
	/// </summary>
	public static event ModuleWorkStatusCallBack OnModuleWorkStatusCallBack;

	/// <summary>
	/// 模块状态回调
	/// </summary>
	public static event ModuleDataStatusCallBack OnModuleDataStatusCallBack;

	/// <summary>
	/// 流程运行状态回调
	/// </summary>
	public static event ProcessExecuteStatusCallBack OnProcessExecuteStatusCallBack;

	/// <summary>
	/// 模块开始运行状态回调
	/// </summary>
	public static event ModuleStartRunStatusCallBack OnModuleStartRunStatusCallBack;

	/// <summary>
	/// 模块停止运行状态回调
	/// </summary>
	public static event ModuleStartRunStatusCallBack OnModuleStopRunStatusCallBack;

	/// <summary>
	/// 流程停止运行状态回调
	/// </summary>
	public static event ModuleStopRunProcessStatusCallBack OnModuleStopRunProcessStatusCallBack;

	/// <summary>
	/// 流程停止运行状态回调
	/// </summary>
	public static event ModuleStartContinuouslyProcessStatusCallBack OnModuleStartContinuouslyProcessStatusCallBack;

	/// <summary>
	/// 连续运行回调
	/// </summary>
	public static event ModuleStartRunStatusCallBack OnModuleContinousRunStatusCallBack;

	/// <summary>
	/// 模块xml信息回调
	/// </summary>
	public static event ModuleXmlinfoCallBack OnModuleXMLInfoCallBack;

	/// <summary>
	/// 开始方案保存
	/// </summary>
	public static event ModuleSolutionSaveBeginCallBack OnModuleSolutionSaveBeginCallBack;

	/// <summary>
	/// 方案保存进度
	/// </summary>
	public static event ModuleSolutionSaveProcessCallBack OnModuleSolutionSaveProcessCallBack;

	/// <summary>
	/// 方案保存结束
	/// </summary>
	public static event ModuleSolutionSaveEndCallBack OnModuleSolutionSaveEndCallBack;

	/// <summary>
	/// 开始加载方案
	/// </summary>
	public static event ModuleSolutionLoadBeginCallBack OnModuleSolutionLoadBeginCallBack;

	/// <summary>
	/// 方案加载结束
	/// </summary>
	public static event ModuleSolutionLoadEndCallBack OnModuleSolutionLoadEndCallBack;

	/// <summary>
	/// 模块方案加载进度
	/// </summary>
	public static event ModuleSolutionLoadProcessCallBack OnModuleSolutionLoadProcessCallBack;

	/// <summary>
	/// 模块方法内部文件
	/// </summary>
	public static event ModuleSolutionInterfaceFileCallBack OnModuleSolutionInterfaceFileCallBack;

	/// <summary>
	/// 模块结果回调
	/// </summary>
	public static event ModuleResultExCallBack OnModuleResultExCallBack;

	/// <summary>
	/// 运行状态回调
	/// </summary>
	public static event WorkStatusCallBack OnWorkStatusCallBack;

	/// <summary>
	/// 相机取图结束信息回调
	/// </summary>
	public static event CameraCollectCallBack OnCameraCollectCallBack;

	/// <summary>
	/// 相机取图开始信息回调
	/// </summary>
	public static event CameraCollectCallBack OnCameraCollectStartCallBack;

	/// <summary>
	/// 相机连接状态信息回调（相机断线后，打开全局相机模块配置界面需要较长时间，请耐心等待！）
	/// </summary>
	public static event CameraConnectStatusCallBack OnCameraConnectStatusCallBack;

	/// <summary>
	/// 通信连接状态回调
	/// </summary>
	public static event CommuConnectCallBack OnCommuConnectCallBack;

	/// <summary>
	/// 变量值修改上报
	/// </summary>
	public static event VarValueChangedCallBack OnVarValueChangeCallBack;

	/// <summary>
	/// 方案信息回调
	/// </summary>
	public static event SolutionInfoCallBack OnSolutionInfoCallBack;

	/// <summary>
	/// 底层二进制结果回调_即时
	/// </summary>
	public static event ChunkResultExCallBack OnNativeChunkResultCallBack;

	/// <summary>
	///
	/// </summary>
	public static event ModuleSolutionSaveByExtern OnModuleSolutionSaveByExtern;

	/// <summary>
	/// 资源占用率
	/// </summary>
	public static event ResourceUsageCallBack OnResourceUsagePush;

	/// <summary>
	/// 底层模块上报数据到主界面
	/// </summary>
	public static event ReportDataCallBack OnReportDataToVM;

	public static event SetNeedShowSaveNotice OnSetNeedShowSaveNotice;

	/// <summary>
	/// 界面心跳异常
	/// </summary>
	public static event ClientHeartNotice OnClientHeartNotice;

	/// <summary>
	/// 代理崩溃
	/// </summary>
	public static event ProxyCrashNotice OnProxyCrashNotice;

	/// <summary>
	/// 全局脚本崩溃
	/// </summary>
	public static event GlobalScriptCrashMotice OnGlobalScriptCrashNotice;

	public static event ModelLoadWarn OnModelLoadWarn;

	/// <summary>
	/// 接收全局脚本上报数据事件
	/// </summary>
	public static event GlobalScriptReportCallBack OnGlobalScriptRecvDataCallBack;

	/// <summary>
	/// 加载方案后是否执行资源预分配
	/// </summary>
	public static event ExecuteAfterLoad OnExecuteAfterLoad;

	/// <summary>
	/// 开始Group导出
	/// </summary>
	public static event GroupExportBeginCallBack OnGroupExportBeginCallBack;

	/// <summary>
	/// Group导出进度
	/// </summary>
	public static event GroupExportProcessCallBack OnGroupExportProcessCallBack;

	/// <summary>
	/// Group导出结束
	/// </summary>
	public static event GroupExportEndCallBack OnGroupExportEndCallBack;

	/// <summary>
	/// 开始Group导入
	/// </summary>
	public static event GroupImportBeginCallBack OnGroupImportBeginCallBack;

	/// <summary>
	/// Group导入进度
	/// </summary>
	public static event GroupImportProcessCallBack OnGroupImportProcessCallBack;

	/// <summary>
	/// Group导入结束
	/// </summary>
	public static event GroupImportEndCallBack OnGroupImportEndCallBack;

	/// <summary>
	/// Group导入后重新分配ID
	/// </summary>
	public static event GroupImportGuidCallBack OnGroupImportGuidCallBack;

	/// <summary>
	/// Group导入服务返回界面数据
	/// </summary>
	public static event GroupImportInterfaceFileCallBack OnGroupImportInterfaceFileCallBack;

	/// <summary>
	/// 加载方案时通知获取界面数据
	/// </summary>
	public static event UIGlobalProcessObjectIDCallBack OnUIGlobalProcessObjectIDCallBack;

	/// <summary>
	/// 导入流程时通知获取界面数据
	/// </summary>
	public static event ProcessImportUiDataCallBack OnProcessImportUiDataCallBack;

	/// <summary>
	/// 导入Group时通知获取界面数据
	/// </summary>
	public static event GroupImportUiDataCallBack OnGroupImportUiDataCallBack;

	/// <summary>
	/// 流程导出开始
	/// </summary>
	public static event ProcessExportBeginCallBack OnProcessExportBeginCallBack;

	/// <summary>
	/// 流程导出进度
	/// </summary>
	public static event ProcessExportProgressCallBack OnProcessExportProgressCallBack;

	/// <summary>
	/// 流程导出结束
	/// </summary>
	public static event ProcessExportEndCallBack OnProcessExportEndCallBack;

	/// <summary>
	/// 流程导入开始
	/// </summary>
	public static event ProcessImportBeginCallBack OnProcessImportBeginCallBack;

	/// <summary>
	/// 流程导入进度
	/// </summary>
	public static event ProcessImportProgressCallBack OnProcessImportProgressCallBack;

	/// <summary>
	/// 流程导入结束
	/// </summary>
	public static event ProcessImportEndCallBack OnProcessImportEndCallBack;

	/// <summary>
	/// 流程导入后重新分配ID
	/// </summary>
	public static event ProcessImportGuidCallBack OnProcessImportGuidCallBack;

	/// <summary>
	/// 流程导入服务返回界面数据
	/// </summary>
	public static event ProcessImportInterfaceFileCallBack OnProcessImportInterfaceFileCallBack;

	/// <summary>
	/// 流程导入忽略全局变量信息
	/// </summary>
	public static event ProcessImportIgnoreGlobalVarCallBack OnProcessImportIgnoreGlobalVarCallBack;

	/// <summary>
	/// 上报模块状态信息
	/// </summary>
	public static event ReportModuleStatusCallBack OnReportModuleStatusCallBack;

	/// <summary>
	/// 流程解注册状态信息
	/// </summary>
	public static event ProcedureUnRegisterCallBack OnProcedureUnRegisterCallBack;

	/// <summary>
	/// 关闭方案状态信息
	/// </summary>
	public static event CloseSolutionCallBack OnCloseSolutionCallBack;

	/// <summary>
	/// 数据队列上报信息
	/// </summary>
	public static event DataQueueReportDataCallBack OnDataQueueReportDataCallBack;

	/// <summary>
	/// 通信发送数据信息
	/// </summary>
	public static event CommunicationDataCallBack OnCommuRecvDataCallBack;

	/// <summary>
	/// 通信状态上报信息
	/// </summary>
	public static event CommunicationDataCallBack OnCommuStatusCallBack;

	/// <summary>
	/// 方案执行结束
	/// </summary>
	public static event SolutionRunEndCallBack OnSolutionRunEndCallBack;

	public static bool GetIsSDKMode()
	{
		return IsSDK;
	}

	static CVmObjectBase()
	{
		IsSDK = false;
		syncObj = new object();
		if (m_VmBase == null)
		{
			lock (syncObj)
			{
				if (m_VmBase == null)
				{
					m_VmBase = new CVmObjectBase();
				}
			}
		}
		mhandle = IntPtr.Zero;
		int num = -536870657;
		if (IntPtr.Zero == mhandle)
		{
			ImvsSdkDefine.IMVS_PLATFORM_BASIC_INFO pstPlatformBasicInfo = default;
			string config = ConfigurationHelper.GetConfig("StartServerByExe");
			string config2 = ConfigurationHelper.GetConfig("ServerSetting");
			string config3 = ConfigurationHelper.GetConfig("ServerPath");
			string config4 = ConfigurationHelper.GetConfig("ClientType");
			if (config2 != null)
			{
				string[] array = config2.Split(':');
				if (array.Length != 2)
				{
					throw new Exception("SDK: Address is invalid.");
				}
				pstPlatformBasicInfo.nIp = IPAddrConvert.StringToInt(array[0]);
				pstPlatformBasicInfo.nPort = Convert.ToUInt16(array[1]);
				if ("1" == config)
				{
					pstPlatformBasicInfo.nHandleType = 1u;
				}
				else
				{
					pstPlatformBasicInfo.nHandleType = 0u;
				}
				if (config4 == "2")
				{
					pstPlatformBasicInfo.nClientType = 2u;
				}
				num = ServerSDKAPI.IMVS_CreateHandle(ref mhandle, ref pstPlatformBasicInfo);
				if (num != 0)
				{
					VmException ex = new VmException(num);
					throw ex;
				}
			}
			else
			{
				AssemblyResolveHelper.GetInstance();
				if ("" == config3)
				{
					num = ServerSDKAPI.IMVS_PF_CreateHandle(ref mhandle);
					if (num != 0)
					{
						VmException ex2 = new VmException(num);
						throw ex2;
					}
				}
				else
				{
					num = ServerSDKAPI.IMVS_PF_CreateHandle(ref mhandle, config3);
					if (num != 0)
					{
						VmException ex3 = new VmException(num);
						throw ex3;
					}
				}
				IsSDK = true;
			}
			string config5 = ConfigurationHelper.GetConfig("IsRunBackground");
			string config6 = ConfigurationHelper.GetConfig("IsAutoLoadPath");
			Process[] processesByName = Process.GetProcessesByName("GlobalScript");
			if ((!(config5 == "1") && !(config6 == "True")) || processesByName == null || processesByName.Length == 0)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(2048);
				num = ServerSDKAPI.IMVS_StartGlobalScript(mhandle, 2048u, intPtr);
				Marshal.FreeHGlobal(intPtr);
				if (num != 0 && num != -536870899)
				{
					VmException ex4 = new VmException(num);
					throw ex4;
				}
			}
		}
		IntPtr user = default;
		PlatformInfoCallBack = delegateOutputCallBackFunc;
		num = ServerSDKAPI.IMVS_RegisterOutputPlatformInfoCallBack(mhandle, PlatformInfoCallBack, user);
		if (num != 0)
		{
			VmException ex5 = new VmException(num);
		}
		if (moduleNameIndexMap == null)
		{
			moduleNameIndexMap = new Dictionary<string, int>();
			addModuleNameIndex();
		}
		moduleIdList = new List<uint>();
		moduleResultList = new List<CModuleResultBase>();
	}

	public static uint ReconnectServer(string address, ref int waitingTime)
	{
		DisconnectServer();
		return ConnectServer(address, ref waitingTime);
	}

	private static void DisconnectServer()
	{
		LogHelper.Info("IMVS_DestroyHandle.");
		ServerSDKAPI.FtpDeInit(mhandle);
		ServerSDKAPI.IMVS_DestroyHandle(mhandle);
		mhandle = IntPtr.Zero;
	}

	private static uint ConnectServer(string address, ref int waitingTime)
	{
		if (address != null)
		{
			int num = 0;
			if (num != 0)
			{
				return 0u;
			}
		}
		else
		{
			string config = ConfigurationHelper.GetConfig("ServerPath");
		}
		return 0u;
	}

	private static void addModuleNameIndex()
	{
		moduleNameIndexMap.Add("ImageSourceModule", 105);
		moduleNameIndexMap.Add("SaveImage", 2);
		moduleNameIndexMap.Add("IMVSHPFeatureMatchModu", 3);
		moduleNameIndexMap.Add("IMVSFastFeatureMatchModu", 4);
		moduleNameIndexMap.Add("IMVSCircleFindModu", 5);
		moduleNameIndexMap.Add("IMVSLineFindModu", 6);
		moduleNameIndexMap.Add("IMVSBlobFindModu", 7);
		moduleNameIndexMap.Add("IMVSCaliperModu", 8);
		moduleNameIndexMap.Add("IMVSCaliperEdgeModu", 9);
		moduleNameIndexMap.Add("IMVSEdgeWidthFindModu", 10);
		moduleNameIndexMap.Add("IMVSFixtureModu", 11);
		moduleNameIndexMap.Add("IMVSRectFindModu", 12);
		moduleNameIndexMap.Add("IMVSPeakFindModu", 13);
		moduleNameIndexMap.Add("IMVSCaliperCornerModu", 14);
		moduleNameIndexMap.Add("IMVSPairLineModu", 15);
		moduleNameIndexMap.Add("IMVSL2CMeasureModu", 16);
		moduleNameIndexMap.Add("IMVSC2CMeasureModu", 17);
		moduleNameIndexMap.Add("IMVSP2CMeasureModu", 18);
		moduleNameIndexMap.Add("IMVSP2LMeasureModu", 19);
		moduleNameIndexMap.Add("IMVSL2LMeasureModu", 20);
		moduleNameIndexMap.Add("IMVSP2PMeasureModu", 21);
		moduleNameIndexMap.Add("IMVSCircleFitModu", 22);
		moduleNameIndexMap.Add("IMVSLineFitModu", 23);
		moduleNameIndexMap.Add("IMVSIntensityMeasureModu", 24);
		moduleNameIndexMap.Add("IMVSPixelCountModu", 25);
		moduleNameIndexMap.Add("IMVSHistToolModu", 26);
		moduleNameIndexMap.Add("GeometryCreate", 27);
		moduleNameIndexMap.Add("IMVS2dBcrModu", 28);
		moduleNameIndexMap.Add("IMVSBcrModu", 29);
		moduleNameIndexMap.Add("IMVSOcrModu", 30);
		moduleNameIndexMap.Add("IMVSVericodeModu", 31);
		moduleNameIndexMap.Add("IMVSCalibBoardCalibModu", 32);
		moduleNameIndexMap.Add("IMVSAlignCalibModu", 33);
		moduleNameIndexMap.Add("IMVSCalibTransformModu", 34);
		moduleNameIndexMap.Add("IMVSScaleTransformModu", 35);
		moduleNameIndexMap.Add("IMVSImageCorrectModu", 36);
		moduleNameIndexMap.Add("IMVSImageMorphModu", 37);
		moduleNameIndexMap.Add("IMVSBinaryModu", 38);
		moduleNameIndexMap.Add("IMVSImageFilterModu", 39);
		moduleNameIndexMap.Add("IMVSImageEnhanceModu", 40);
		moduleNameIndexMap.Add("IMVSImageMathModu", 41);
		moduleNameIndexMap.Add("IMVSImageSharpnessModu", 42);
		moduleNameIndexMap.Add("IMVSImageFixtureModu", 43);
		moduleNameIndexMap.Add("IMVSShadeCorrectModu", 44);
		moduleNameIndexMap.Add("IMVSAffineTransformModu", 45);
		moduleNameIndexMap.Add("IMVSPolarUnwarpModu", 46);
		moduleNameIndexMap.Add("IMVSColorExtractModu", 47);
		moduleNameIndexMap.Add("IMVSColorMeasureModu", 48);
		moduleNameIndexMap.Add("IMVSColorTransformModu", 49);
		moduleNameIndexMap.Add("IfModule", 50);
		moduleNameIndexMap.Add("BranchModule_STD", 51);
		moduleNameIndexMap.Add("AndModule", 52);
		moduleNameIndexMap.Add("FormatModule", 53);
		moduleNameIndexMap.Add("CalculatorModule", 54);
		moduleNameIndexMap.Add("StringCompareModule", 55);
		moduleNameIndexMap.Add("ShellModule", 56);
		moduleNameIndexMap.Add("PointSetMODU_STD", 58);
		moduleNameIndexMap.Add("IMVSOcrDlModu", 65);
		moduleNameIndexMap.Add("BranchStringCpm", 66);
		moduleNameIndexMap.Add("IMVSFrameMeanModu", 67);
		moduleNameIndexMap.Add("IMVSImageCalibModu", 68);
		moduleNameIndexMap.Add("IMVSImageCombineProcessModu", 69);
		moduleNameIndexMap.Add("IMVSImageNormlizeModu", 70);
		moduleNameIndexMap.Add("IMVSMarkInspModu", 71);
		moduleNameIndexMap.Add("IMVSRegionCopyModu", 72);
		moduleNameIndexMap.Add("ReadDatasModule", 74);
		moduleNameIndexMap.Add("SaveTextModule", 75);
		moduleNameIndexMap.Add("SendDatasModule", 76);
		moduleNameIndexMap.Add("TimeStatisticModule", 77);
		moduleNameIndexMap.Add("IMVSCameraMapModu", 78);
		moduleNameIndexMap.Add("IMVSSinglePointAlignModu", 79);
		moduleNameIndexMap.Add("IMVSMultiPointAlignModu", 80);
		moduleNameIndexMap.Add("IMVSLineAlignModu", 81);
		moduleNameIndexMap.Add("IMVSCnnFlawModu", 83);
		moduleNameIndexMap.Add("IMVSCnnCharDetectModu", 84);
		moduleNameIndexMap.Add("IMVSCnnClassifyModu", 85);
		moduleNameIndexMap.Add("IMVSCnnDetectModu", 86);
		moduleNameIndexMap.Add("IMVSCnnTrainToolModu", 87);
		moduleNameIndexMap.Add("IMVSCircleEdgeInspModu", 88);
		moduleNameIndexMap.Add("IMVSLineEdgeInspModu", 89);
		moduleNameIndexMap.Add("IMVSCircleEdgePairInspModu", 90);
		moduleNameIndexMap.Add("IMVSLineEdgePairInspModu", 91);
		moduleNameIndexMap.Add("IMVSEdgeInspGroupModu", 92);
		moduleNameIndexMap.Add("IMVSEdgePairInspGroupModu", 93);
		moduleNameIndexMap.Add("IMVSEdgeFlawInspModu", 94);
		moduleNameIndexMap.Add("IMVSEdgePairFlawInspModu", 95);
		moduleNameIndexMap.Add("BranchStringCpmL", 96);
		moduleNameIndexMap.Add("IMVSCnnCharDetectModuC", 97);
		moduleNameIndexMap.Add("IMVSCnnClassifyModuC", 98);
		moduleNameIndexMap.Add("IMVSCnnDetectModuC", 99);
		moduleNameIndexMap.Add("ImageBufferModule", 100);
		moduleNameIndexMap.Add("IMVSGrayMatchModu", 101);
		moduleNameIndexMap.Add("IMVSColorDistinguishModu", 102);
		moduleNameIndexMap.Add("IMVSImageUndoDistortModu", 103);
		moduleNameIndexMap.Add("CameraIOModule", 104);
		moduleNameIndexMap.Add("LightModule", 106);
		moduleNameIndexMap.Add("IMVSQuadrangleFindModu", 107);
		moduleNameIndexMap.Add("IMVSLineFindGroupModu", 108);
		moduleNameIndexMap.Add("IMVSMultiLineFindModu", 109);
		moduleNameIndexMap.Add("IMVSTwoImgCalibModu", 110);
		moduleNameIndexMap.Add("IMVSNImgCalibModu", 111);
		moduleNameIndexMap.Add("IMVSImgStitchCalibModu", 112);
		moduleNameIndexMap.Add("IMVSGeometricTransformModu", 113);
		moduleNameIndexMap.Add("DataSetModule", 114);
		moduleNameIndexMap.Add("DataAssembleModule", 115);
		moduleNameIndexMap.Add("DataAnalysisModule", 116);
		moduleNameIndexMap.Add("LocalImageView", 0);
		moduleNameIndexMap.Add("IMVSCnnSingleCharDetectModu", 117);
		moduleNameIndexMap.Add("IMVSCnnSingleCharDetectModuC", 118);
		moduleNameIndexMap.Add("IMVSGroup", 119);
	}

	private static void delegateOutputCallBackFunc(IntPtr pInputStruct, IntPtr pUser)
	{
		try
		{
			ImvsSdkDefine.IMVS_OUTPUT_PLATFORM_INFO iMVS_OUTPUT_PLATFORM_INFO = (ImvsSdkDefine.IMVS_OUTPUT_PLATFORM_INFO)Marshal.PtrToStructure(pInputStruct, typeof(ImvsSdkDefine.IMVS_OUTPUT_PLATFORM_INFO));
			switch (iMVS_OUTPUT_PLATFORM_INFO.nInfoType)
			{
			case 769u:
			{
				ImvsSdkDefine.IMVS_PROCESS_INFO processInfo = (ImvsSdkDefine.IMVS_PROCESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_PROCESS_INFO));
				if (OnProcessRegisterCallBack != null)
				{
					OnProcessRegisterCallBack(processInfo);
				}
				break;
			}
			case 1u:
			{
				ImvsSdkDefine.IMVS_MODULE_INFO moduleInfo = (ImvsSdkDefine.IMVS_MODULE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_MODULE_INFO));
				if (OnModuleRegisterCallBack != null)
				{
					OnModuleRegisterCallBack(moduleInfo);
				}
				break;
			}
			case 257u:
			{
				ImvsSdkDefine.IMVS_EXCEPTION_HB_MODULE_INFO iMVS_EXCEPTION_HB_MODULE_INFO = (ImvsSdkDefine.IMVS_EXCEPTION_HB_MODULE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXCEPTION_HB_MODULE_INFO));
				break;
			}
			case 6u:
			{
				ImvsSdkDefine.IMVS_MODULE_INFO iMVS_MODULE_INFO = (ImvsSdkDefine.IMVS_MODULE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_MODULE_INFO));
				break;
			}
			case 9u:
			{
				ImvsSdkDefine.IMVS_DONGLE_INFO moduleInfo2 = (ImvsSdkDefine.IMVS_DONGLE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_DONGLE_INFO));
				OnDongleCallBack?.Invoke(moduleInfo2);
				break;
			}
			case 10u:
			{
				ImvsSdkDefine.IMVS_SERVER_INFO moduleInfo3 = (ImvsSdkDefine.IMVS_SERVER_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SERVER_INFO));
				if (OnServerStatusCallBack != null)
				{
					OnServerStatusCallBack(moduleInfo3);
				}
				break;
			}
			case 13u:
			{
				ImvsSdkDefine.IMVS_MODULE_RESULT_EXTRA_INFO iMVS_MODULE_RESULT_EXTRA_INFO = (ImvsSdkDefine.IMVS_MODULE_RESULT_EXTRA_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_MODULE_RESULT_EXTRA_INFO));
				break;
			}
			case 260u:
			{
				ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO iMVS_STATUS_START_RUN_INFO2 = (ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO));
				break;
			}
			case 259u:
			{
				ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO iMVS_STATUS_START_RUN_INFO = (ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO));
				break;
			}
			case 772u:
			{
				ImvsSdkDefine.IMVS_STATUS_PROCESS_STOP_INFO statusInfo4 = (ImvsSdkDefine.IMVS_STATUS_PROCESS_STOP_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_STATUS_PROCESS_STOP_INFO));
				if (OnModuleStopRunProcessStatusCallBack != null)
				{
					OnModuleStopRunProcessStatusCallBack(statusInfo4);
				}
				break;
			}
			case 771u:
			{
				ImvsSdkDefine.IMVS_STATUS_PROCESS_START_CONTINUOUSLY_INFO statusInfo3 = (ImvsSdkDefine.IMVS_STATUS_PROCESS_START_CONTINUOUSLY_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_STATUS_PROCESS_START_CONTINUOUSLY_INFO));
				if (OnModuleStartContinuouslyProcessStatusCallBack != null)
				{
					OnModuleStartContinuouslyProcessStatusCallBack(statusInfo3);
				}
				break;
			}
			case 258u:
			{
				ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO statusInfo2 = (ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO));
				if (OnModuleContinousRunStatusCallBack != null)
				{
					OnModuleContinousRunStatusCallBack(statusInfo2);
				}
				break;
			}
			case 774u:
			{
				ImvsSdkDefine.IMVS_PROC_CONTIEXECUTE_INFO_LIST statusInfo = (ImvsSdkDefine.IMVS_PROC_CONTIEXECUTE_INFO_LIST)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_PROC_CONTIEXECUTE_INFO_LIST));
				if (OnProcessExecuteStatusCallBack != null)
				{
					OnProcessExecuteStatusCallBack(statusInfo);
				}
				break;
			}
			case 15u:
			{
				ImvsSdkDefine.IMVS_XML_INFO iMVS_XML_INFO = (ImvsSdkDefine.IMVS_XML_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_XML_INFO));
				break;
			}
			case 16u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_SAVE_BEGEIN_INFO solutionSaveBeginInfo = (ImvsSdkDefine.IMVS_SOLUTION_SAVE_BEGEIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_SAVE_BEGEIN_INFO));
				if (OnModuleSolutionSaveBeginCallBack != null)
				{
					OnModuleSolutionSaveBeginCallBack(solutionSaveBeginInfo);
				}
				break;
			}
			case 17u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_SAVE_END_INFO solutionSaveEndInfo = (ImvsSdkDefine.IMVS_SOLUTION_SAVE_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_SAVE_END_INFO));
				if (OnModuleSolutionSaveEndCallBack != null)
				{
					OnModuleSolutionSaveEndCallBack(solutionSaveEndInfo);
				}
				break;
			}
			case 18u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_SAVE_PROCESS_INFO solutionSaveProcessInfo = (ImvsSdkDefine.IMVS_SOLUTION_SAVE_PROCESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_SAVE_PROCESS_INFO));
				if (OnModuleSolutionSaveProcessCallBack != null)
				{
					OnModuleSolutionSaveProcessCallBack(solutionSaveProcessInfo);
				}
				break;
			}
			case 19u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_LOAD_BEGEIN_INFO solutionLoadBeginInfo = (ImvsSdkDefine.IMVS_SOLUTION_LOAD_BEGEIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_LOAD_BEGEIN_INFO));
				if (OnModuleSolutionLoadBeginCallBack != null)
				{
					OnModuleSolutionLoadBeginCallBack(solutionLoadBeginInfo);
				}
				break;
			}
			case 20u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_LOAD_END_INFO solutionLoadEndInfo = (ImvsSdkDefine.IMVS_SOLUTION_LOAD_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_LOAD_END_INFO));
				if (OnModuleSolutionLoadEndCallBack != null)
				{
					OnModuleSolutionLoadEndCallBack(solutionLoadEndInfo);
				}
				break;
			}
			case 21u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_LOAD_PROCESS_INFO solutionLoadProcessInfo = (ImvsSdkDefine.IMVS_SOLUTION_LOAD_PROCESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_LOAD_PROCESS_INFO));
				if (OnModuleSolutionLoadProcessCallBack != null)
				{
					OnModuleSolutionLoadProcessCallBack(solutionLoadProcessInfo);
				}
				break;
			}
			case 22u:
			{
				ImvsSdkDefine.IMVS_SOLUTION_LOAD_INTERFACE_FILE_INFO solutionLoadInterfaceFileInfo = (ImvsSdkDefine.IMVS_SOLUTION_LOAD_INTERFACE_FILE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_SOLUTION_LOAD_INTERFACE_FILE_INFO));
				if (OnModuleSolutionInterfaceFileCallBack != null)
				{
					OnModuleSolutionInterfaceFileCallBack(solutionLoadInterfaceFileInfo);
				}
				break;
			}
			case 773u:
			{
				ImvsSdkDefine.IMVS_DETAIL_PLATFORM_BEGIN_INFO iMVS_DETAIL_PLATFORM_BEGIN_INFO = (ImvsSdkDefine.IMVS_DETAIL_PLATFORM_BEGIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_DETAIL_PLATFORM_BEGIN_INFO));
				break;
			}
			case 775u:
			{
				ImvsSdkDefine.IMVS_DETAIL_PLATFORM_END_INFO iMVS_DETAIL_PLATFORM_END_INFO = (ImvsSdkDefine.IMVS_DETAIL_PLATFORM_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_DETAIL_PLATFORM_END_INFO));
				break;
			}
			case 14u:
			{
				ImvsSdkDefine.IMVS_MODULE_WORK_STAUS workStatusInfo = (ImvsSdkDefine.IMVS_MODULE_WORK_STAUS)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_MODULE_WORK_STAUS));
				if (OnWorkStatusCallBack != null)
				{
					OnWorkStatusCallBack(workStatusInfo);
				}
				break;
			}
			case 23u:
			{
				ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX moduleResultExInfo = (ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX));
				if (OnModuleResultExCallBack != null)
				{
					OnModuleResultExCallBack(ref moduleResultExInfo);
				}
				break;
			}
			case 24u:
			{
				ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO cameraCollectInfo2 = (ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO));
				if (OnCameraCollectCallBack != null)
				{
					OnCameraCollectCallBack(cameraCollectInfo2);
				}
				break;
			}
			case 32u:
			{
				ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO cameraCollectInfo = (ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO));
				if (OnCameraCollectStartCallBack != null)
				{
					OnCameraCollectStartCallBack(cameraCollectInfo);
				}
				break;
			}
			case 33u:
			{
				ImvsSdkDefine.IMVS_CAMERA_CONNECT_STATUS_INFO cameraConnectStatusInfo = (ImvsSdkDefine.IMVS_CAMERA_CONNECT_STATUS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_CAMERA_CONNECT_STATUS_INFO));
				if (OnCameraConnectStatusCallBack != null)
				{
					OnCameraConnectStatusCallBack(cameraConnectStatusInfo);
				}
				break;
			}
			case 25u:
			{
				ImvsSdkDefine.IMVS_COMMUNICATION_CONNECT_INFO connectCollectInfo = (ImvsSdkDefine.IMVS_COMMUNICATION_CONNECT_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_COMMUNICATION_CONNECT_INFO));
				if (OnCommuConnectCallBack != null)
				{
					OnCommuConnectCallBack(connectCollectInfo);
				}
				break;
			}
			case 34u:
			{
				ImvsSdkDefine.IMVS_VAR_REPORT_VALUE_INFO varChangeInfo = (ImvsSdkDefine.IMVS_VAR_REPORT_VALUE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_VAR_REPORT_VALUE_INFO));
				if (OnVarValueChangeCallBack != null)
				{
					OnVarValueChangeCallBack(varChangeInfo);
				}
				break;
			}
			case 776u:
			{
				ImvsSdkDefine.IMVS_RESOURCE_USAGE_INFO resourceInfo = (ImvsSdkDefine.IMVS_RESOURCE_USAGE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_RESOURCE_USAGE_INFO));
				OnResourceUsagePush?.Invoke(resourceInfo);
				break;
			}
			case 770u:
			{
				ImvsSdkDefine.IMVS_ALGORITHM_INIT_RESULT_INFO algResultInfor = (ImvsSdkDefine.IMVS_ALGORITHM_INIT_RESULT_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_ALGORITHM_INIT_RESULT_INFO));
				if (OnReportDataToVM != null)
				{
					OnReportDataToVM(algResultInfor);
				}
				break;
			}
			case 777u:
			{
				ImvsSdkDefine.IMVS_PROXY_CRASH_SP_INFO proxyCrashInfo = (ImvsSdkDefine.IMVS_PROXY_CRASH_SP_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_PROXY_CRASH_SP_INFO));
				if (OnProxyCrashNotice != null)
				{
					OnProxyCrashNotice(proxyCrashInfo);
				}
				break;
			}
			case 784u:
			{
				ImvsSdkDefine.IMVS_GLOBALSCRIPT_CRASH_INFO iMVS_GLOBALSCRIPT_CRASH_INFO = (ImvsSdkDefine.IMVS_GLOBALSCRIPT_CRASH_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_GLOBALSCRIPT_CRASH_INFO));
				break;
			}
			case 785u:
			{
				ImvsSdkDefine.IMVS_GLOBALSCRIPT_REPORTDATA_INFO globalScrptReportInfo = (ImvsSdkDefine.IMVS_GLOBALSCRIPT_REPORTDATA_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_GLOBALSCRIPT_REPORTDATA_INFO));
				if (OnGlobalScriptRecvDataCallBack != null)
				{
					OnGlobalScriptRecvDataCallBack(globalScrptReportInfo);
				}
				break;
			}
			case 786u:
			{
				ImvsSdkDefine.IMVS_LOAD_MODULE_ERROR_INFO_LIST loadModuleInfoList = (ImvsSdkDefine.IMVS_LOAD_MODULE_ERROR_INFO_LIST)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_LOAD_MODULE_ERROR_INFO_LIST));
				if (OnModelLoadWarn != null)
				{
					OnModelLoadWarn(loadModuleInfoList);
				}
				break;
			}
			case 787u:
			{
				ImvsSdkDefine.IMVS_EXECUTE_STATE_AFTER_LOAD_INFO stExecuteState = (ImvsSdkDefine.IMVS_EXECUTE_STATE_AFTER_LOAD_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXECUTE_STATE_AFTER_LOAD_INFO));
				if (OnExecuteAfterLoad != null)
				{
					OnExecuteAfterLoad(stExecuteState);
				}
				break;
			}
			case 789u:
			{
				ImvsSdkDefine.IMVS_EXPORT_GROUP_BEGIN_INFO groupExportBeginInfo = (ImvsSdkDefine.IMVS_EXPORT_GROUP_BEGIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXPORT_GROUP_BEGIN_INFO));
				if (OnGroupExportBeginCallBack != null)
				{
					OnGroupExportBeginCallBack(groupExportBeginInfo);
				}
				break;
			}
			case 790u:
			{
				ImvsSdkDefine.IMVS_EXPORT_GROUP_END_INFO groupExportEndInfo = (ImvsSdkDefine.IMVS_EXPORT_GROUP_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXPORT_GROUP_END_INFO));
				if (OnGroupExportEndCallBack != null)
				{
					OnGroupExportEndCallBack(groupExportEndInfo);
				}
				break;
			}
			case 791u:
			{
				ImvsSdkDefine.IMVS_EXPORT_GROUP_PROGRESS_INFO groupExportProcessInfo = (ImvsSdkDefine.IMVS_EXPORT_GROUP_PROGRESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXPORT_GROUP_PROGRESS_INFO));
				if (OnGroupExportProcessCallBack != null)
				{
					OnGroupExportProcessCallBack(groupExportProcessInfo);
				}
				break;
			}
			case 792u:
			{
				ImvsSdkDefine.IMVS_IMPORT_GROUP_BEGIN_INFO groupImportBeginInfo = (ImvsSdkDefine.IMVS_IMPORT_GROUP_BEGIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_GROUP_BEGIN_INFO));
				if (OnGroupImportBeginCallBack != null)
				{
					OnGroupImportBeginCallBack(groupImportBeginInfo);
				}
				break;
			}
			case 793u:
			{
				ImvsSdkDefine.IMVS_IMPORT_GROUP_END_INFO groupImportEndInfo = (ImvsSdkDefine.IMVS_IMPORT_GROUP_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_GROUP_END_INFO));
				if (OnGroupImportEndCallBack != null)
				{
					OnGroupImportEndCallBack(groupImportEndInfo);
				}
				break;
			}
			case 800u:
			{
				ImvsSdkDefine.IMVS_IMPORT_GROUP_PROGRESS_INFO groupImportProcessInfo = (ImvsSdkDefine.IMVS_IMPORT_GROUP_PROGRESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_GROUP_PROGRESS_INFO));
				if (OnGroupImportProcessCallBack != null)
				{
					OnGroupImportProcessCallBack(groupImportProcessInfo);
				}
				break;
			}
			case 801u:
			{
				ImvsSdkDefine.IMVS_GROUP_NEW_MODULEID_NAME_INFO_LIST groupImportGuidInfoList = (ImvsSdkDefine.IMVS_GROUP_NEW_MODULEID_NAME_INFO_LIST)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_GROUP_NEW_MODULEID_NAME_INFO_LIST));
				if (OnGroupImportGuidCallBack != null)
				{
					OnGroupImportGuidCallBack(groupImportGuidInfoList);
				}
				break;
			}
			case 802u:
			{
				ImvsSdkDefine.IMVS_GROUP_IMPORT_INTERFACE_FILE_INFO groupImportInterfaceFileInfo = (ImvsSdkDefine.IMVS_GROUP_IMPORT_INTERFACE_FILE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_GROUP_IMPORT_INTERFACE_FILE_INFO));
				if (OnGroupImportInterfaceFileCallBack != null)
				{
					OnGroupImportInterfaceFileCallBack(groupImportInterfaceFileInfo);
				}
				break;
			}
			case 822u:
			{
				ImvsSdkDefine.IMVS_GLOBAL_PROCESS_UI_OBJECT_INFO uiGlobalProcessObjectInfo = (ImvsSdkDefine.IMVS_GLOBAL_PROCESS_UI_OBJECT_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_GLOBAL_PROCESS_UI_OBJECT_INFO));
				if (OnUIGlobalProcessObjectIDCallBack != null)
				{
					OnUIGlobalProcessObjectIDCallBack(uiGlobalProcessObjectInfo);
				}
				break;
			}
			case 823u:
			{
				ImvsSdkDefine.IMVS_IMPORT_PROCESS_GET_UIDATA_INFO processImportUiDataInfo = (ImvsSdkDefine.IMVS_IMPORT_PROCESS_GET_UIDATA_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_PROCESS_GET_UIDATA_INFO));
				if (OnProcessImportUiDataCallBack != null)
				{
					OnProcessImportUiDataCallBack(processImportUiDataInfo);
				}
				break;
			}
			case 824u:
			{
				ImvsSdkDefine.IMVS_IMPORT_GROUP_GET_UIDATA_INFO groupImportUiDataInfo = (ImvsSdkDefine.IMVS_IMPORT_GROUP_GET_UIDATA_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_GROUP_GET_UIDATA_INFO));
				if (OnGroupImportUiDataCallBack != null)
				{
					OnGroupImportUiDataCallBack(groupImportUiDataInfo);
				}
				break;
			}
			case 805u:
			{
				ImvsSdkDefine.IMVS_EXPORT_PROCESS_BEGIN_INFO processExportBeginInfo = (ImvsSdkDefine.IMVS_EXPORT_PROCESS_BEGIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXPORT_PROCESS_BEGIN_INFO));
				if (OnProcessExportBeginCallBack != null)
				{
					OnProcessExportBeginCallBack(processExportBeginInfo);
				}
				break;
			}
			case 806u:
			{
				ImvsSdkDefine.IMVS_EXPORT_PROCESS_END_INFO processExportEndInfo = (ImvsSdkDefine.IMVS_EXPORT_PROCESS_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXPORT_PROCESS_END_INFO));
				if (OnProcessExportEndCallBack != null)
				{
					OnProcessExportEndCallBack(processExportEndInfo);
				}
				break;
			}
			case 807u:
			{
				ImvsSdkDefine.IMVS_EXPORT_PROCESS_PROGRESS_INFO processExportProgressInfo = (ImvsSdkDefine.IMVS_EXPORT_PROCESS_PROGRESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_EXPORT_PROCESS_PROGRESS_INFO));
				if (OnProcessExportProgressCallBack != null)
				{
					OnProcessExportProgressCallBack(processExportProgressInfo);
				}
				break;
			}
			case 808u:
			{
				ImvsSdkDefine.IMVS_IMPORT_PROCESS_BEGIN_INFO processImportBeginInfo = (ImvsSdkDefine.IMVS_IMPORT_PROCESS_BEGIN_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_PROCESS_BEGIN_INFO));
				if (OnProcessImportBeginCallBack != null)
				{
					OnProcessImportBeginCallBack(processImportBeginInfo);
				}
				break;
			}
			case 809u:
			{
				ImvsSdkDefine.IMVS_IMPORT_PROCESS_END_INFO processImportEndInfo = (ImvsSdkDefine.IMVS_IMPORT_PROCESS_END_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_PROCESS_END_INFO));
				if (OnProcessImportEndCallBack != null)
				{
					OnProcessImportEndCallBack(processImportEndInfo);
				}
				break;
			}
			case 816u:
			{
				ImvsSdkDefine.IMVS_IMPORT_PROCESS_PROGRESS_INFO processImportProgressInfo = (ImvsSdkDefine.IMVS_IMPORT_PROCESS_PROGRESS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_PROCESS_PROGRESS_INFO));
				if (OnProcessImportProgressCallBack != null)
				{
					OnProcessImportProgressCallBack(processImportProgressInfo);
				}
				break;
			}
			case 818u:
			{
				ImvsSdkDefine.IMVS_IMPORT_PROCESS_MODULE_ID_INFO_LIST processImportGuidInfoList = (ImvsSdkDefine.IMVS_IMPORT_PROCESS_MODULE_ID_INFO_LIST)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_PROCESS_MODULE_ID_INFO_LIST));
				if (OnProcessImportGuidCallBack != null)
				{
					OnProcessImportGuidCallBack(processImportGuidInfoList);
				}
				break;
			}
			case 817u:
			{
				ImvsSdkDefine.IMVS_PROCESS_IMPORT_INTERFACE_FILE_INFO processImportInterfaceFileInfo = (ImvsSdkDefine.IMVS_PROCESS_IMPORT_INTERFACE_FILE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_PROCESS_IMPORT_INTERFACE_FILE_INFO));
				if (OnProcessImportInterfaceFileCallBack != null)
				{
					OnProcessImportInterfaceFileCallBack(processImportInterfaceFileInfo);
				}
				break;
			}
			case 821u:
			{
				ImvsSdkDefine.IMVS_IMPORT_PROCESS_IGNOREGLOBALVAR_INFO processImportIgnoreGlobalVarInfo = (ImvsSdkDefine.IMVS_IMPORT_PROCESS_IGNOREGLOBALVAR_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_IMPORT_PROCESS_IGNOREGLOBALVAR_INFO));
				if (OnProcessImportIgnoreGlobalVarCallBack != null)
				{
					OnProcessImportIgnoreGlobalVarCallBack(processImportIgnoreGlobalVarInfo);
				}
				break;
			}
			case 825u:
			{
				ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST_EX stReportModuStatList = (ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST_EX)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST_EX));
				if (OnReportModuleStatusCallBack != null)
				{
					ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST reportModuleStatusInfo = new ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST(stReportModuStatList);
					OnReportModuleStatusCallBack(reportModuleStatusInfo);
				}
				break;
			}
			case 788u:
			{
				ImvsSdkDefine.IMVS_PROCEDURE_UNREGISTER_INFO procedureUnregisterInfo = (ImvsSdkDefine.IMVS_PROCEDURE_UNREGISTER_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_PROCEDURE_UNREGISTER_INFO));
				if (OnProcedureUnRegisterCallBack != null)
				{
					OnProcedureUnRegisterCallBack(procedureUnregisterInfo);
				}
				break;
			}
			case 832u:
			{
				ImvsSdkDefine.IMVS_CLOSE_SOLUTION_STATUS_INFO closeSolInfo = (ImvsSdkDefine.IMVS_CLOSE_SOLUTION_STATUS_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_CLOSE_SOLUTION_STATUS_INFO));
				if (OnCloseSolutionCallBack != null)
				{
					OnCloseSolutionCallBack(closeSolInfo);
				}
				break;
			}
			case 1280u:
			{
				ImvsSdkDefine.IMVS_REPORT_DATAQUEUE_INFO dataQueueReportInfo = (ImvsSdkDefine.IMVS_REPORT_DATAQUEUE_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_REPORT_DATAQUEUE_INFO));
				if (OnDataQueueReportDataCallBack != null)
				{
					OnDataQueueReportDataCallBack(dataQueueReportInfo);
				}
				break;
			}
			case 1024u:
			{
				ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO reportDataInfo2 = (ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO));
				if (OnCommuRecvDataCallBack != null)
				{
					OnCommuRecvDataCallBack(reportDataInfo2);
				}
				break;
			}
			case 1025u:
			{
				ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO reportDataInfo = (ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO)Marshal.PtrToStructure(iMVS_OUTPUT_PLATFORM_INFO.pData, typeof(ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO));
				if (OnCommuStatusCallBack != null)
				{
					OnCommuStatusCallBack(reportDataInfo);
				}
				break;
			}
			case 35u:
				if (OnSolutionRunEndCallBack != null)
				{
					OnSolutionRunEndCallBack();
				}
				break;
			}
		}
		catch (VmException)
		{
		}
		catch (Exception ex2)
		{
			LogHelper.Error("delegateOutputCallBackFunc内部抛出异常：" + ex2.Message + ", 堆栈信息：" + ex2.StackTrace);
			throw;
		}
	}
}
