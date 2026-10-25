using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HikExternalCall.Common;

namespace VM.PlatformSDKCS;

/// <summary>
/// ImvsSdkDefine
/// </summary>
public class ImvsSdkDefine
{
	/// <summary>
	/// 模块类型枚举定义
	/// </summary>
	public enum IMVS_MODULE_TYPE
	{
		IMVS_ENUM_MODULE_TYPE_NORMAL = 0,
		IMVS_ENUM_MODULE_TYPE_PROCESS_CONTROL = 1,
		IMVS_ENUM_MODULE_TYPE_LOOP_CONTROL = 2,
		IMVS_ENUM_MODULE_TYPE_GLOBAL_COMMUNICATION = 3,
		IMVS_ENUM_MODULE_TYPE_GLOBAL_QUEUE = 4,
		IMVS_ENUM_MODULE_TYPE_GLOBAL_VARIABLE = 5,
		IMVS_ENUM_MODULE_TYPE_GLOBAL_CAMERA = 6,
		IMVS_ENUM_MODULE_TYPE_GLOBAL_LIGHT = 7,
		IMVS_ENUM_MODULE_TYPE_GROUP = 20
	}

	/// <summary>
	/// 导入数据类型枚举定义
	/// </summary>
	public enum IMVS_IMPORT_MODULE_DATA_TYPE
	{
		IMVS_PF_IMPORT_TYPE_TEMP = 1,
		IMVS_PF_IMPORT_TYPE_FONT,
		IMVS_PF_IMPORT_TYPE_CALIB,
		IMVS_PF_IMPORT_TYPE_DLN,
		IMVS_PF_IMPORT_TYPE_IMAGE,
		IMVS_PF_IMPORT_TYPE_MARKINSP,
		IMVS_PF_IMPORT_TYPE_EDGEFLAW,
		IMVS_PF_IMPORT_TYPE_COLORRECOGNITION,
		IMVS_PF_IMPORT_TYPE_MARKFIND,
		IMVS_PF_IMPORT_TYPE_GLUEPATHCONDUCT,
		IMVS_PF_IMPORT_TYPE_IMGSTITCHCALIB,
		IMVS_PF_IMPORT_TYPE_INSPECT,
		IMVS_PF_IMPORT_TYPE_GALLERY,
		IMVS_PF_IMPORT_TYPE_MLCLASSIFIER
	}

	public enum IMVS_PIXEL_FORMAT
	{
		IMVS_UNDEFINE,
		IMVS_MONO8,
		IMVS_RGB24
	}

	/// @struct IMVS_INPUT_INFO
	/// @brief 输入信息
	[Serializable]
	public struct IMVS_INPUT_INFO
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string strInput;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_INPUT_INFO_LIST
	/// @brief 输入信息列表
	[Serializable]
	public struct IMVS_INPUT_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public IMVS_INPUT_INFO[] astInputInfoList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_OUTPUT_INFO
	/// @brief 输出信息
	[Serializable]
	public struct IMVS_OUTPUT_INFO
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string strOutput;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_OUTPUT_INFO_LIST
	/// @brief 输出信息列表
	[Serializable]
	public struct IMVS_OUTPUT_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public IMVS_OUTPUT_INFO[] astOutputInfoList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// 模块流程信息
	[Serializable]
	public struct IMVS_PROCESS_INFO
	{
		public uint nProcID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strProcName;

		public uint nModuleType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strProcGUID;

		public int nInitErrorCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_MODULE_INFO
	/// @brief 模块信息
	[Serializable]
	public struct IMVS_MODULE_INFO
	{
		public uint nNodeID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strDisplayName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strModuleName;

		public uint nIsFatherLoop;

		public uint nAlgorithmInitResult;

		public uint nProcessID;

		public uint nModuleType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strModuleGUID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_DONGLE_INFO
	/// @brief 加密狗信息
	public struct IMVS_DONGLE_INFO
	{
		public int nDongleStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strDongleType;

		public int nCameraNum;

		public int nRemainTime;

		public int nDongleSeries;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strDongleSerialNo;

		public int nSoftLanguage;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;
	}

	public struct IMVS_SERVER_INFO
	{
		public uint nServerStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_MODULE_INFO_LIST
	/// @brief 模块信息列表
	public struct IMVS_MODULE_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public IMVS_MODULE_INFO[] astModuleInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_RUN_ERROR_INFO
	{
		public uint nModuleID;

		public int nErrorCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strModuleName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strDisplayName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] bReserve;
	}

	public struct IMVS_MODULE_RUN_ERROR_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public IMVS_MODULE_RUN_ERROR_INFO[] stModuErrorInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] bReserve;
	}

	public struct IMVS_PROCESS_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
		public IMVS_PROCESS_INFO[] astProcessInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_SUBSCRIBE_INFO
	/// @brief 订阅信息
	[Serializable]
	public struct IMVS_SUBSCRIBE_INFO
	{
		public uint nSubModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strSubInput;

		public uint nPubModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strPubOutput;

		public uint nIsPubConstant;

		public uint nIndexModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strIndex;

		public uint nIsIndexConstant;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_SUBSCRIBE_BASIC_INFO
	/// @brief 订阅方信息
	public struct IMVS_SUBSCRIBE_BASIC_INFO
	{
		public uint nSubModuleID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string strParamName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_SUBSCRIBE_BASIC_INFO
	/// @brief 发布方信息
	public struct IMVS_PUBLISH_BASIC_INFO
	{
		public uint nPubModuleID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string strParamName;

		public uint nIsPubConstant;

		public uint nIndexModuleID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string strIndexContent;

		public uint nIsIndexConstant;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_SUBSCRIBE_INFO_LIST
	/// @brief 订阅信息列表
	public struct IMVS_SUBSCRIBE_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public IMVS_SUBSCRIBE_INFO[] astSubscribeInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_PLATFORM_DETAILED_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strSolutionPath;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] strPassWord;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_AUTO_LOAD_SOLUTION_INFO
	{
		public uint nIfAutoLoad;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strSolutionPath;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] strPassWord;

		public uint nExecuteStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// @struct IMVS_PLATFORM_INFO
	/// @brief 平台基本信息
	public struct IMVS_PLATFORM_BASIC_INFO
	{
		public uint nIp;

		public ushort nPort;

		public uint nSdkIp;

		public ushort nSdkPort;

		public uint nNetAdapterIp;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strUserName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public byte[] strPassWord;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strVendorName;

		public uint nMacLow;

		public ushort nMacHigh;

		public uint nClientType;

		public uint nServerType;

		public uint nHandleType;

		public uint nServerRepIp;

		public ushort nServerRepPort;

		public uint nServerProcID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public uint[] nReserved;
	}

	/// @struct IMVS_PLATFORM_INFO_LIST
	/// @brief 平台基本信息列表
	public struct IMVS_PLATFORM_BASIC_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public IMVS_PLATFORM_BASIC_INFO[] astPlatformBasicInfo;
	}

	/// <summary>
	/// SDK4Server回调消息类型
	/// </summary>
	public enum IMVS_OUTPUT_PlATFORM_INFO_TYPE
	{
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_ERROR = 0,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE = 1,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SUBSCRIBE = 2,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_UNSUBSCRIBE = 3,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_UNREGISTER = 4,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_RESULT = 5,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_UPDATE = 6,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_RESTART_BEGIN = 7,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_RESTART_END = 8,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_DONGLE = 9,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SERVER = 10,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_RESULT_FLAG = 11,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_HEART = 12,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_RESULT_EXTRA_INFO = 13,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_WORK_STATUS = 14,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_XML_INFO = 15,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_SAVE_BEGIN = 16,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_SAVE_END = 17,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_SAVE_PROCESS = 18,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_LOAD_BEGIN = 19,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_LOAD_END = 20,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_LOAD_PROCESS = 21,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_LOAD_INTERFACE_FILE = 22,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_MODULE_RESULT_EX = 23,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_CAMERA_COLLECT = 24,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_COMMUNICATION_CONNECT = 25,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_CAMERA_COLLECT_START = 32,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_CAMERA_CONNECT_STATUS = 33,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_VAR_VALUE_CHANGE = 34,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_SOLUTION_RUN_END = 35,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_HB_CLIENT = 256,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_HB_MODU = 257,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_START_CONTINUOUSLY = 258,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_STOP_EXECUTE = 259,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_START_RUN = 260,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_SOLUTION_SAVE_THIRD = 261,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_SOLUTION_LOAD_DEVELOPER_DATA = 262,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_PROCEDURE_REGISTER_INFO = 769,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_ALGORITHM_INIT_RESULT = 770,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_PROCESS_START_CONTINUOUSLY = 771,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_PROCESS_STOP_EXECUTE = 772,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_DETAIL_PLATFORM_BEGIN_INFO = 773,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_PLATFORM_PROCESS_INFO = 774,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_DETAIL_PLATFORM_END_INFO = 775,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_RESOURCE_USAGE_INFO = 776,
		/// <summary>
		/// 代理进程崩溃状态信息
		/// </summary>
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_SP_PROXY_CRASH = 777,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_GLOBALSCRIPT_CRASH = 784,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_GLOBALSCRIPT_REPORT_DATA = 785,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_LOAD_MODULE_WARN_INFO = 786,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_EXECUTE_STATE_AFTER_LOAD_INFO = 787,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_PROCEDURE_UNREGISTER_INFO = 788,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_EXPORT_GROUP_BEGIN = 789,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_EXPORT_GROUP_END = 790,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_EXPORT_GROUP_PROGRESS = 791,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_IMPORT_GROUP_BEGIN = 792,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_GROUP_END = 793,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_GROUP_PROGRESS = 800,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_GROUP_NAME_NEWID = 801,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_GROUP_INTERFACE_FILE = 802,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_UPDATE_PROCESS_NAME = 803,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_UPDATE_MODULE_NAME = 804,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_EXPORT_PROCESS_BEGIN = 805,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_EXPORT_PROCESS_END = 806,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_EXPORT_PROCESS_PROGRESS = 807,
		IMVS_ENUM_OUTPUT_PLATFORM_INFO_IMPORT_PROCESS_BEGIN = 808,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_PROCESS_END = 809,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_PROCESS_PROGRESS = 816,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_PROCESS_INTERFACE_FILE = 817,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_PROCESS_NEW_NAME_ID = 818,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_TAKEOVER_PRC_INTERFACE_FILE = 819,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_TAKEOVER_SOL_INTERFACE_FILE = 820,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_PROCESS_IGNOREGLOBALVAR = 821,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_UI_GLOBAL_PROCESS_OBJECT_ID = 822,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_PROCESS_GET_UIDATA = 823,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_IMPORT_GROUP_GET_UIDATA = 824,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_REPORT_MODULE_STATUS_INFO = 825,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_REPORT_CLOSE_SOLUTION_INFO = 832,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_REPORT_FRAME_LOSS_INFO = 1280,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_REPORT_COMMU_RECV_DATA_INFO = 1024,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_REPORT_COMMU_DEVICE_STATUS_INFO = 1025,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_OTHER_CLIENT_CONN_SERVER = 1027,
		IMVS_ENUM_OUTPUT_PlATFORM_INFO_REPORT_PROXY_IMAGE_INFO = 1028
	}

	/// <summary>
	/// 按需回调类型
	/// </summary>
	public enum IMVS_REPORT_RESULT_TYPE
	{
		REPORT_RESULT_TYPE_INVALID,
		REPORT_RESULT_TYPE_NONE,
		REPORT_RESULT_TYPE_ALL,
		REPORT_RESULT_TYPE_PART
	}

	/// <summary>
	/// 回调信息结构体
	/// </summary>
	public struct IMVS_OUTPUT_PLATFORM_INFO
	{
		public uint nInfoType;

		public IntPtr pData;

		public uint nDataSize;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_RESULT_FLAG_INFO
	{
		public uint nFlagType;

		public uint nResultNum;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strPubAddress;

		public uint nIsSelfExecute;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 解注册回调信息结构体
	/// </summary>
	public struct IMVS_UNREGISTER_INFO
	{
		public uint nPubIp;

		public ushort nPubPort;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	///  模块心跳异常信息结构
	/// </summary>
	public struct IMVS_EXCEPTION_HB_MODULE_INFO
	{
		public uint nClientStatus;

		public uint nModuleID;

		public uint nProcessID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] strModuleName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] strDisPlayName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 模块输出结果值
	/// </summary>
	public struct IMVS_RESULT_PARAM_VALUE
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamValue;
	}

	public struct IMVS_PROC_CONTIEXECUTE_INFO
	{
		public uint nProcessID;

		public uint nProcRunStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROC_CONTIEXECUTE_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
		public IMVS_PROC_CONTIEXECUTE_INFO[] stProcExecuteInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_STATUS_START_CONTINUOUSLY_INFO
	{
		private uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_STATUS_STOP_INFO
	{
		private uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_STATUS_PROCESS_STOP_INFO
	{
		private uint nStatus;

		public uint nProcessID;

		public uint nStopAction;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;
	}

	public struct IMVS_STATUS_START_RUN_INFO
	{
		private uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_STATUS_PROCESS_START_CONTINUOUSLY_INFO
	{
		public uint nStatus;

		public uint nProcessID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 模块输出结构体
	/// </summary>
	public struct IMVS_MODULE_RESULT_INFO
	{
		/// <summary>
		/// 模块结果名称
		/// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		/// <summary>
		///  模块结果流程ID
		/// </summary>              
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string strCycleID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string strParamType;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string strValueNum;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string strValueStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
		public IMVS_RESULT_PARAM_VALUE[] stResultParamValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	///  模块结果附加信息, 后续增加成员
	/// </summary>
	public struct IMVS_MODULE_RESULT_EXTRA_INFO
	{
		public uint nStatus;

		public uint nCycleID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strPubAddress;

		public float fModuleTime;

		public float fAlgorithmTime;

		public float fCallBackTime;

		public float fCallBackGetTime;

		public float fCallBackSetTime;

		public float fTotalCycleTime;

		public float fTimeTestA;

		public float fTimeTestB;

		public float fTimeTestC;

		public float fTimeTestD;

		public float fTimeTestE;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_WORK_STAUS
	{
		/// <summary>
		/// 0是空闲，1是忙碌，2是停止
		/// </summary>
		public uint nWorkStatus;

		public uint nProcessID;

		public uint nIsTimeValid;

		public float fProcessTime;

		public uint nExecuteCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strReCommand;

		public uint nIsExecuteNormal;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;
	}

	public struct IMVS_IO_PARAM
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strStructName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string strValueType;

		public uint nIsForce;

		public uint nIsShow;
	}

	public struct IMVS_IO_PARAM_LIST
	{
		public uint nParamNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public IMVS_IO_PARAM[] astModuleParam;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SET_BINARY_DATA_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strName;

		public IntPtr pBinaryData;

		public uint nBinaryLenth;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_GET_BINARY_LENGTH_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strName;

		public uint nBinaryLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_GET_BINARY_DATA_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strName;

		public uint nBinaryDataMallocSize;

		public IntPtr pBinaryData;

		public uint nBinaryLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_XML_INFO
	{
		public uint nModuleID;

		public IMVS_IO_PARAM_LIST stInputParamList;

		public IMVS_IO_PARAM_LIST stOutputParamList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public enum CLIENT_TYPE
	{
		CLIENT_TYPE_VISIONMASTER,
		CLIENT_TYPE_THIRD,
		CLIENT_TYPE_GLOBALSCRIPT
	}

	public struct IMVS_SOLUTION_SAVE_BEGEIN_INFO
	{
		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strSolPath;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SOLUTION_SAVE_END_INFO
	{
		public uint nClientType;

		public uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strSolPath;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SOLUTION_SAVE_PROCESS_INFO
	{
		public uint nProcess;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SOLUTION_LOAD_BEGEIN_INFO
	{
		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strSolPath;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SOLUTION_LOAD_END_INFO
	{
		public uint nClientType;

		public uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strSolPath;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SOLUTION_LOAD_PROCESS_INFO
	{
		public uint nProcess;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 接管状态信息
	/// </summary>
	public struct IMVS_STATUS_TAKEOVER_INFO
	{
		public uint nTakeoverStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_DETAIL_PLATFORM_BEGIN_INFO
	{
		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_DETAIL_PLATFORM_END_INFO
	{
		public uint nClientType;

		public uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_SOLUTION_LOAD_INTERFACE_FILE_INFO
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_STRING_VALUE
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
		public byte[] strValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_STRING_VALUE_EX
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
		public string strValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_CHUNK_VALUE_EX
	{
		public IntPtr pData;

		public uint nLen;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string strValue;

		public uint nUseRealData;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_BYTEDATA_VALUE_EX
	{
		public IntPtr pData;

		public uint nLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_BYTEDATA_VALUE_EX_Data
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
		public string strValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;

		/// 16进制数据，直接使用byte
		public byte[] hexValue;
	}

	public class IMVS_MODULE_CHUNK_VALUE_EX_Data
	{
		public bool IsMemory;

		public uint MemoryType;

		internal int Size;

		public string MemoryAddress;

		internal IntPtr MemoryPtr;

		private bool _failed;

		private object _lock = new object();

		private IntPtr pData;

		/// <summary>
		/// 填充失败
		/// </summary>
		private bool FillFailed => _failed;

		public void FillData()
		{
			lock (_lock)
			{
				if (FillFailed || pData != IntPtr.Zero)
				{
					return;
				}
				try
				{
					if (IsMemory)
					{
						string realMemoryAddress = GetRealMemoryAddress();
						pData = GetByteFromMemoryAddress(realMemoryAddress, out var lenth);
						Size = lenth;
					}
					else if (MemoryPtr == IntPtr.Zero)
					{
						_failed = true;
					}
					else
					{
						bool isExcOccur = false;
						pData = GetByteFromMemoryIntptr(MemoryPtr, Size, ref isExcOccur);
					}
				}
				catch (Exception)
				{
					_failed = true;
				}
			}
		}

		public void FillData(ref bool isExcOccur)
		{
			lock (_lock)
			{
				if (FillFailed || pData != IntPtr.Zero)
				{
					return;
				}
				try
				{
					if (IsMemory)
					{
						string realMemoryAddress = GetRealMemoryAddress();
						pData = GetByteFromMemoryAddress(realMemoryAddress, out var lenth);
						Size = lenth;
					}
					else if (MemoryPtr == IntPtr.Zero)
					{
						_failed = true;
					}
					else
					{
						pData = GetByteFromMemoryIntptr(MemoryPtr, Size, ref isExcOccur);
					}
				}
				catch (Exception)
				{
					_failed = true;
				}
			}
		}

		public IntPtr GetData(out int lenth)
		{
			lock (_lock)
			{
				FillData();
				lenth = Size;
				return FillFailed ? IntPtr.Zero : pData;
			}
		}

		public byte[] GetData()
		{
			IntPtr data = GetData(out var lenth);
			if (data != IntPtr.Zero)
			{
				byte[] array = new byte[lenth];
				Marshal.Copy(data, array, 0, lenth);
				return array;
			}
			return null;
		}

		public byte[] GetData(bool bUseMemPool)
		{
			lock (_lock)
			{
				if (FillFailed)
				{
					return null;
				}
				try
				{
					if (IsMemory)
					{
						string realMemoryAddress = GetRealMemoryAddress();
						byte[] byteData = null;
						IntPtr byteFromMemoryAddress = GetByteFromMemoryAddress(realMemoryAddress, out var _, ref byteData, bUseMemPool);
						return byteData;
					}
				}
				catch (Exception)
				{
					_failed = true;
				}
			}
			return null;
		}

		public string GetRealMemoryAddress()
		{
			string result = (IsMemory ? MemoryAddress : null);
			if (MemoryType == 2)
			{
			}
			return result;
		}
	}

	public struct IMVS_MODULE_RESULT_INFO_EX
	{
		public int nValueStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public int nParamType;

		public int nValueNum;

		public IntPtr pIntValue;

		public IntPtr pFloatValue;

		public IntPtr pStringValue;

		public IntPtr pImageValue;

		public IntPtr pChunkValue;

		public IntPtr pByteValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_RESULT_INFO_EX_Data
	{
		public int nValueStatus;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string strParamName;

		public int nParamType;

		public int nValueNum;

		public int[] pIntValue;

		public float[] pFloatValue;

		public IMVS_MODULE_STRING_VALUE_EX[] pStringValue;

		public IMVS_MODULE_CHUNK_VALUE_EX_Data[] pImageValue;

		public IMVS_MODULE_CHUNK_VALUE_EX_Data[] pChunkValue;

		public IMVS_MODULE_BYTEDATA_VALUE_EX_Data[] pByteValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;

		public double[] pDoubleValue;
	}

	public struct IMVS_MODULE_RESULT_RESERVED_INFO
	{
		public int nArrivalTimeStampLow;

		public int nArrivalTimeStampHigh;

		public int nLeaveTimeStampLow;

		public int nLeaveTimeStampHigh;

		public long nRstTime;

		public int nMilliSec;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 61)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_RESULT_INFO_LIST_EX
	{
		public uint nStatus;

		public uint nErrorCode;

		public uint nCycleID;

		public uint nModuleID;

		public float fModuleTime;

		public float fAlgorithmTime;

		public uint bInCycle;

		public IntPtr pstrModuleName;

		public IntPtr pstrDisplayName;

		public IntPtr pstResultReservedInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public IntPtr[] pReserved;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strPubAddress;

		public uint bIsSelfExecute;

		public uint nExecuteCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public uint[] nReserved;

		public int nResultNum;

		public IntPtr pInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strReCommand;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReservedEx;
	}

	public struct IMVS_CAMERA_COLLECT_INFO
	{
		public uint nCameraID;

		public uint nFrameNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strCameraSN;

		public uint nCollectStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;
	}

	public struct IMVS_CAMERA_CONNECT_STATUS_INFO
	{
		public uint nCameraID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strCameraSN;

		public uint nConnectStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	[Serializable]
	public struct IMVS_CAMERA_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strCameraName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strCameraSN;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	[Serializable]
	public struct IMVS_CAMERA_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public IMVS_CAMERA_INFO[] astCameraInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_COMMUNICATION_CONNECT_INFO
	{
		public uint nDeviceStatus;

		public uint nDeviceID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_VAR_REPORT_VALUE_INFO
	{
		public uint nModuleID;

		public IntPtr pData;

		public int nLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public int[] nReserved;
	}

	public struct IMVS_CUSTOM_SUBSCRIBE_INFO
	{
		public int nIfDelete;

		public int nOutputType;

		public int nSubProcessID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strInputParamName;

		public int nPubModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strOutputParamName;

		public int nIsPubConstant;

		public int nIndexModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strIndexContent;

		public int nIsIndexConstant;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_VARIABLE_PARAM_VALUE
	{
		public int nModuleId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] chParamName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] nReserved;
	}

	public struct IMVS_CUSTOM_SUBSCRIBE_VALUE
	{
		public int nOutputType;

		public int nVarType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] chOutputName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] chConstantValue;

		public IMVS_VARIABLE_PARAM_VALUE stVariableValue;
	}

	public struct IMVS_GET_CUSTOM_SUBSCRIBE_INFO_LIST
	{
		public int nOutputNum;

		public int nTotalNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public IMVS_CUSTOM_SUBSCRIBE_VALUE[] stCustomSubInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public int[] nReservedEx;
	}

	public struct IMVS_MODULE_RESULT_INFO_LIST_EX_Data
	{
		public uint nStatus;

		public uint nErrorCode;

		public uint nCycleID;

		public uint nModuleID;

		public float fModuleTime;

		public float fAlgorithmTime;

		public uint bInCycle;

		public uint bIsSelfExecute;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string strPubAddress;

		public uint nExecuteCount;

		public DateTime dtEndTime;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;

		public int nResultNum;

		public IMVS_MODULE_RESULT_INFO_EX_Data[] pInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] strReCommand;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReservedEx;
	}

	public struct IMVS_SOLUTION_VERSION_INFO
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string strTargetSolutionInfo;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string strCurrentSolutionInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_FILE_VERSION_INFO
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string strCurrentVesionInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_BASIC_INFO
	{
		/// unsigned int
		public uint nModuleID;

		/// char[256]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strDisplayName;

		/// char[64]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strModuleName;

		/// unsigned int[16]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_BASIC_INFO_LIST
	{
		/// unsigned int
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.Struct)]
		public IMVS_MODULE_BASIC_INFO[] astModuleInfo;

		/// unsigned int[4]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_RESOURCE_USAGE_INFO
	{
		public int nMemUsage;

		public int nCPUUsage;

		public int nHardSpace;

		public int nNetWorkUsage;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_ADD_MODULE_INFO_LIST
	{
		public int nAddNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2060, ArraySubType = UnmanagedType.Struct)]
		public int[] nAddModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_DEL_MODULE_INFO_LIST
	{
		public int nDelNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2060, ArraySubType = UnmanagedType.Struct)]
		public int[] nDelModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 底层模块上报数据给主界面的结构体
	/// </summary>
	public struct IMVS_ALGORITHM_INIT_RESULT_INFO
	{
		/// <summary>
		/// 模块ID
		/// </summary>
		public uint nModuleID;

		/// <summary>
		/// 界面与模块传输内容类型
		/// </summary>
		public uint nType;

		/// <summary>
		/// 算法结果数据
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string strTransData;

		/// unsigned int[4]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 底层模块上报通信数据
	/// </summary>
	public struct IMVS_COMMU_REPORT_DATA_INFO
	{
		public int nType;

		public IntPtr pData;

		public int nLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 代理崩溃信息
	/// </summary>
	public struct IMVS_PROXY_CRASH_SP_INFO
	{
		public int nHasSolStatu;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 全局脚本崩溃状态
	/// </summary>
	public struct IMVS_GLOBALSCRIPT_CRASH_INFO
	{
		public uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 全局脚本上传数据信息 add by zhouriwen 2019-9-13
	/// </summary>
	public struct IMVS_GLOBALSCRIPT_REPORTDATA_INFO
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_LOAD_MODULE_ERROR_INFO_LIST
	{
		public uint nClientType;

		public uint nModuleNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.Struct)]
		public IMVS_LOAD_MODULE_ERROR_INFO[] astLoadModuErrInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_LOAD_MODULE_ERROR_INFO
	{
		public uint nModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strModuleName;

		public int nErrorCode;
	}

	public struct IMVS_CAMERA_PIC_INFO
	{
		public int nCameraId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] szLocalPicPath;
	}

	public struct IMVS_CAMERA_PIC_INFO_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.Struct)]
		public IMVS_CAMERA_PIC_INFO[] stCamPicInfoList;

		public int nTimeout;

		public int nIsAllModuRun;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROCEDURE_CUSTOM_RUN_POLICY
	{
		public int nProcessID;

		public int nThreadNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		public byte[] chCpuCore;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROCEDURE_CUSTOM_RUN_POLICY_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.Struct)]
		public IMVS_PROCEDURE_CUSTOM_RUN_POLICY[] stPolicyList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public enum IMVS_PROCEDURE_RUN_POLICY_TYPE
	{
		PROCEDURE_RUN_POLICY_INVALID,
		PROCEDURE_RUN_POLICY_HIGH_SPEED,
		PROCEDURE_RUN_POLICY_CUSTOM,
		PROCEDURE_RUN_POLICY_TIME_STABILITY
	}

	public struct IMVS_PROCEDURE_RUN_POLICY
	{
		public int nRunPolicy;

		public int nCustomIndex;

		public IMVS_PROCEDURE_CUSTOM_RUN_POLICY_LIST stCustomRunPolicy;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROCESSOR_INFO
	{
		public int nLogicProcessorNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		public byte[] strReserved;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXECUTE_STATE_AFTER_LOAD_INFO
	{
		public int nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_GROUP_MODULE_INFO
	{
		public uint nGroupId;

		public int nSonModuNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.U4)]
		public uint[] anModuleId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_GROUP_BEGIN_INFO
	{
		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_GROUP_END_INFO
	{
		public uint nGroupId;

		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strGroupPath;

		public int nErrorCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_GROUP_PROGRESS_INFO
	{
		public uint nProgress;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_GROUP_BEGIN_INFO
	{
		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_GROUP_END_INFO
	{
		public uint nProcessId;

		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strGroupPath;

		public int nErrorCode;

		public uint nGroupNewId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strGroupNewName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_GROUP_PROGRESS_INFO
	{
		public uint nProgress;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_GROUP_NEW_MODULEID_NAME_INFO
	{
		public uint nNewModuID;

		public uint nOldModuID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strModuNewName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_GROUP_NEW_MODULEID_NAME_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.Struct)]
		public IMVS_GROUP_NEW_MODULEID_NAME_INFO[] astModuIdGUID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_GROUP_IMPORT_INTERFACE_FILE_INFO
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_GROUP_IO_INFO
	{
		public uint nValueType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;
	}

	public struct IMVS_GROUP_IO_INFO_LIST
	{
		public uint nGroupId;

		public uint nInputNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType = UnmanagedType.Struct)]
		public IMVS_GROUP_IO_INFO[] stGroupInput;

		public uint nOutputNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType = UnmanagedType.Struct)]
		public IMVS_GROUP_IO_INFO[] stGroupOutput;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROCEDURE_IO_INFO
	{
		public uint nValueType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;
	}

	public struct IMVS_PROCEDURE_IO_INFO_LIST
	{
		public uint nProcedureId;

		public uint nInputNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType = UnmanagedType.Struct)]
		public IMVS_PROCEDURE_IO_INFO[] stProcInput;

		public uint nOutputNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType = UnmanagedType.Struct)]
		public IMVS_PROCEDURE_IO_INFO[] stProcOutput;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public enum IMVS_ENUM_SUBSCRIBE_INDEX_TYPE
	{
		SUBSCRIBE_INDEX_INVALID,
		SUBSCRIBE_INDEX_ALL,
		SUBSCRIBE_INDEX_CONSTANT,
		SUBSCRIBE_INDEX_VARIABLE
	}

	public struct IMVS_SUBSCRIBE_MODULE_PARAM_INFO
	{
		public int nModuleId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] byParamName;
	}

	public struct IMVS_VARIABLE_VALUE_INFO
	{
		public IMVS_ENUM_SUBSCRIBE_INDEX_TYPE eIndexType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strIndexConstant;

		public IMVS_SUBSCRIBE_MODULE_PARAM_INFO stIndexVariable;

		public IMVS_SUBSCRIBE_MODULE_PARAM_INFO stModuleParam;
	}

	public enum IMVS_ENUM_SUBSCRIBE_VAR_TYPE
	{
		SUBSCRIBE_VAR_INVAlID,
		SUBSCRIBE_VAR_CONSTANT,
		SUBSCRIBE_VAR_GLOBAL,
		SUBSCRIBE_VAR_VARIABLE
	}

	public struct IMVS_SUBSCRIBE_MODU_PARAM_INFO
	{
		public IMVS_ENUM_SUBSCRIBE_VAR_TYPE eVarType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strConstantValue;

		public IMVS_VARIABLE_VALUE_INFO stVariableValue;
	}

	/// <summary>
	/// 内存释放类型
	/// </summary>
	public enum IMVS_RELEASE_MEMORY_TYPE : uint
	{
		RELEASE_MEMORY_BASE_VOID,
		RELEASE_MEMORY_BASE_POINTER,
		RELEASE_MEMORY_BASE_ARRAY,
		RELEASE_MEMORY_STRUCT_UI_OBJECT_PARAM_LIST,
		RELEASE_MEMORY_STRUCT_UI_PARAM_INFO_LIST,
		RELEASE_MEMORY_STRUCT_MODU_CONNECT_INFO_LIST,
		RELEASE_MEMORY_STRUCT_MODU_SUBSCRIBE_INFO_LIST,
		RELEASE_MEMORY_STRUCT_MODU_STATUS_INFO_LIST,
		RELEASE_MEMORY_STRUCT_MODU_EXPORT_DATA_INFO,
		RELEASE_MEMORY_STRUCT_MODU_HISTORY_RES_INFO_LIST
	}

	public enum IMVS_ENUM_MODULE_RESULT_TYPE : uint
	{
		IMVS_MODURES_TYPE_INVALID,
		IMVS_MODURES_TYPE_INT,
		IMVS_MODURES_TYPE_FLOAT,
		IMVS_MODURES_TYPE_STRING,
		IMVS_MODURES_TYPE_IMAGE,
		IMVS_MODURES_TYPE_CHUNK,
		IMVS_MODURES_TYPE_BYTEARRAY,
		IMVS_MODURES_TYPE_DOUBLE
	}

	public struct IMVS_BIND_GLOBALVAR_PARAM_INFO
	{
		public uint nModuleId;

		public IMVS_ENUM_MODULE_RESULT_TYPE eGlobalVarType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public IMVS_SUBSCRIBE_MODU_PARAM_INFO stBindValue;
	}

	public struct IMVS_BIND_GLOBALVAR_PARAM_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_BIND_GLOBALVAR_PARAM_INFO[] astBindGlobalVarList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_UNBIND_GLOBALVAR_PARAM_INFO
	{
		public uint nModuleId;

		public IMVS_ENUM_MODULE_RESULT_TYPE eGlobalVarType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public IMVS_SUBSCRIBE_MODU_PARAM_INFO stBindValue;
	}

	public struct IMVS_UNBIND_GLOBALVAR_PARAM_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_UNBIND_GLOBALVAR_PARAM_INFO[] astBindGlobalVarList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_GET_BIND_GLOBALVAR_PARAM_INFO_LIST
	{
		public uint nTotalNum;

		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_BIND_GLOBALVAR_PARAM_INFO[] astBindGlobalVarList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_BIND_MODULE_PARAM_INFO
	{
		public uint nModuleId;

		public IMVS_ENUM_MODULE_RESULT_TYPE eVarType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public IMVS_SUBSCRIBE_MODU_PARAM_INFO stBindValue;
	}

	public struct IMVS_BIND_MODULE_PARAM_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_BIND_MODULE_PARAM_INFO[] astBindModuVarList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_GET_BIND_MODULE_PARAM_INFO_LIST
	{
		public uint nTotalNum;

		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_BIND_MODULE_PARAM_INFO[] astBindModuVarList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_UNBIND_MODULE_PARAM_INFO
	{
		public uint nModuleId;

		public IMVS_ENUM_MODULE_RESULT_TYPE eVarType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;
	}

	public struct IMVS_UNBIND_MODULE_PARAM_INFO_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_UNBIND_MODULE_PARAM_INFO[] astBindModuVarList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_PROCESS_BEGIN_INFO
	{
		public uint nProcessId;

		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_PROCESS_END_INFO
	{
		public uint nProcessId;

		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strProcPath;

		public int nErrorCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_PROCESS_PROGRESS_INFO
	{
		public uint nProgress;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_PROCESS_BEGIN_INFO
	{
		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_PROCESS_END_INFO
	{
		public uint nIgnoreGlobalModu;

		public uint nProcessId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strProcessName;

		public uint nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 780)]
		public byte[] strProcPath;

		public int nErrorCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_PROCESS_PROGRESS_INFO
	{
		public uint nProgress;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROCESS_IMPORT_INTERFACE_FILE_INFO
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_PROCESS_MODULE_ID_INFO
	{
		public uint nOldId;

		public uint nNewId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strNewName;
	}

	public struct IMVS_IMPORT_PROCESS_MODULE_ID_INFO_LIST
	{
		public int nProcessNum;

		public int nModuleNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000, ArraySubType = UnmanagedType.Struct)]
		public IMVS_IMPORT_PROCESS_MODULE_ID_INFO[] astProcessIdInfoList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.Struct)]
		public IMVS_IMPORT_PROCESS_MODULE_ID_INFO[] astModuleIdInfoList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_TAKEOVER_PRC_INTERFACE_DATA_INFO
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_TAKEOVER_SOL_INTERFACE_DATA_INFO
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_PROCEDURE_UNREGISTER_INFO
	{
		public uint nClientType;

		public uint nProcessID;

		public int nErrorCode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strProcessName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_CLOSE_SOLUTION_STATUS_INFO
	{
		public int nClientType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_REPORT_DATAQUEUE_INFO
	{
		public int nModuleId;

		public IntPtr pData;

		public uint nLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_UPDATE_PROCESS_NAME_INFO
	{
		public uint nProcessID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strProcessName;
	}

	public struct IMVS_UPDATE_PROCESS_NAME_INFO_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000, ArraySubType = UnmanagedType.Struct)]
		public IMVS_UPDATE_PROCESS_NAME_INFO[] astProcNameInfoList;
	}

	public struct IMVS_UPDATE_MODULE_NAME_INFO
	{
		public uint nModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strModuleName;
	}

	public struct IMVS_UPDATE_MODULE_NAME_INFO_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.Struct)]
		public IMVS_UPDATE_MODULE_NAME_INFO[] astModuleNameInfoList;
	}

	public struct IMVS_IMPORT_PROCESS_IGNOREGLOBALVAR_INFO
	{
		public uint nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_UI_OBJECT_PARAM_INFO_Ex
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public IntPtr pData;

		public uint nDataLen;
	}

	public class IMVS_UI_OBJECT_PARAM_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		private IntPtr pDataPtr;

		public uint nDataLen;

		private byte[] pBytes;

		public byte[] pData => pBytes;

		public IMVS_UI_OBJECT_PARAM_INFO()
		{
			strParamName = null;
			pDataPtr = IntPtr.Zero;
			nDataLen = 0u;
			pBytes = null;
		}

		public IMVS_UI_OBJECT_PARAM_INFO(IMVS_UI_OBJECT_PARAM_INFO_Ex stObjParam)
		{
			strParamName = stObjParam.strParamName;
			pDataPtr = stObjParam.pData;
			nDataLen = stObjParam.nDataLen;
			_ = pDataPtr;
			if (true)
			{
				pBytes = new byte[stObjParam.nDataLen];
				pBytes = ExternalCallHelper.IntPtr2Bytes(pDataPtr, (int)nDataLen);
			}
		}

		/// <summary>
		/// 置空指针变量 避免野指针
		/// </summary>
		public void ResetPtr()
		{
			pDataPtr = IntPtr.Zero;
		}
	}

	public struct IMVS_UI_OBJECT_PARAM_INFO_LIST_Ex
	{
		public int nParamNum;

		internal IntPtr pUIObjParamInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IMVS_UI_OBJECT_PARAM_INFO[] pstUIObjParamInfo()
		{
			IMVS_UI_OBJECT_PARAM_INFO[] array = new IMVS_UI_OBJECT_PARAM_INFO[nParamNum];
			return ExternalCallHelper.IntPtr2Structures<IMVS_UI_OBJECT_PARAM_INFO>(pUIObjParamInfo, nParamNum);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public class IMVS_UI_OBJECT_PARAM_INFO_LIST : ICloneable
	{
		public int nParamNum;

		private IntPtr pUIObjParamInfoPtr;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		private bool bIsClone;

		private IMVS_UI_OBJECT_PARAM_INFO[] parrUiObjParamInfo;

		public IMVS_UI_OBJECT_PARAM_INFO_LIST()
		{
			nParamNum = 0;
			pUIObjParamInfoPtr = IntPtr.Zero;
			bIsClone = false;
			parrUiObjParamInfo = null;
		}

		public IMVS_UI_OBJECT_PARAM_INFO_LIST(IMVS_UI_OBJECT_PARAM_INFO_LIST_Ex stUiObjParamInfo)
		{
			nParamNum = stUiObjParamInfo.nParamNum;
			pUIObjParamInfoPtr = stUiObjParamInfo.pUIObjParamInfo;
			nReserved = stUiObjParamInfo.nReserved;
			parrUiObjParamInfo = null;
			bIsClone = false;
		}

		/// <summary>
		/// 置空指针变量 避免野指针
		/// </summary>
		public void ResetPtr()
		{
			pUIObjParamInfoPtr = IntPtr.Zero;
			IMVS_UI_OBJECT_PARAM_INFO[] array = pUIObjParamInfo();
			foreach (IMVS_UI_OBJECT_PARAM_INFO iMVS_UI_OBJECT_PARAM_INFO in array)
			{
				iMVS_UI_OBJECT_PARAM_INFO.ResetPtr();
			}
		}

		public IMVS_UI_OBJECT_PARAM_INFO[] pUIObjParamInfo()
		{
			if (!bIsClone)
			{
				IMVS_UI_OBJECT_PARAM_INFO[] array = new IMVS_UI_OBJECT_PARAM_INFO[nParamNum];
				IMVS_UI_OBJECT_PARAM_INFO_Ex[] array2 = new IMVS_UI_OBJECT_PARAM_INFO_Ex[nParamNum];
				if (IntPtr.Zero != pUIObjParamInfoPtr)
				{
					array2 = ExternalCallHelper.IntPtr2Structures<IMVS_UI_OBJECT_PARAM_INFO_Ex>(pUIObjParamInfoPtr, nParamNum);
					for (int i = 0; i < nParamNum; i++)
					{
						array[i] = new IMVS_UI_OBJECT_PARAM_INFO(array2[i]);
					}
				}
				return array;
			}
			return parrUiObjParamInfo;
		}

		public object Clone()
		{
			if (!bIsClone)
			{
				bIsClone = true;
				parrUiObjParamInfo = new IMVS_UI_OBJECT_PARAM_INFO[nParamNum];
				IMVS_UI_OBJECT_PARAM_INFO_Ex[] array = new IMVS_UI_OBJECT_PARAM_INFO_Ex[nParamNum];
				if (IntPtr.Zero != pUIObjParamInfoPtr)
				{
					array = ExternalCallHelper.IntPtr2Structures<IMVS_UI_OBJECT_PARAM_INFO_Ex>(pUIObjParamInfoPtr, nParamNum);
					for (int i = 0; i < nParamNum; i++)
					{
						parrUiObjParamInfo[i] = new IMVS_UI_OBJECT_PARAM_INFO(array[i]);
					}
				}
			}
			return this;
		}
	}

	public struct IMVS_PROC_UI_OBJECT_PARAM_INFO
	{
		public int nObjectId;

		public IMVS_UI_OBJECT_PARAM_INFO_LIST_Ex stUIObjParamInfoList;
	}

	public struct IMVS_PROC_UI_OBJECT_PARAM_INFO_LIST
	{
		public int nObjectNum;

		private IntPtr pProcUIObjParamInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IMVS_PROC_UI_OBJECT_PARAM_INFO[] pstProcUIObjParamInfo()
		{
			IMVS_PROC_UI_OBJECT_PARAM_INFO[] array = new IMVS_PROC_UI_OBJECT_PARAM_INFO[nObjectNum];
			return ExternalCallHelper.IntPtr2Structures<IMVS_PROC_UI_OBJECT_PARAM_INFO>(pProcUIObjParamInfo, nObjectNum);
		}
	}

	public struct IMVS_NODE_TREE_INFO
	{
		public int nChildNodeNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public IMVS_MODULE_INFO[] astChildNodeInfo;
	}

	[Serializable]
	public struct IMVS_MODULE_CONNECT_INFO
	{
		public int nModuleId;

		public int nNextModuNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public uint[] anNextModuleId;

		public int nPreModuNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public uint[] anPreModuleId;
	}

	[Serializable]
	public struct IMVS_MODULE_CONNECT_INFO_LIST_EX
	{
		public int nNum;

		private IntPtr pModuleConnectInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IMVS_MODULE_CONNECT_INFO[] pstModuleConnectInfo()
		{
			IMVS_MODULE_CONNECT_INFO[] array = new IMVS_MODULE_CONNECT_INFO[nNum];
			return ExternalCallHelper.IntPtr2Structures<IMVS_MODULE_CONNECT_INFO>(pModuleConnectInfo, nNum);
		}
	}

	[Serializable]
	public struct IMVS_MODULE_CONNECT_INFO_LIST(IMVS_MODULE_CONNECT_INFO_LIST_EX stModuConnectInfo)
	{
		public int nNum = stModuConnectInfo.nNum;

		public IMVS_MODULE_CONNECT_INFO[] pstModuleConnectInfo = stModuConnectInfo.pstModuleConnectInfo();
	}

	[Serializable]
	public struct IMVS_MODULE_SUBSCRIBE_INFO_LIST_Ex
	{
		public int nNum;

		internal IntPtr pSubscribeInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IMVS_SUBSCRIBE_INFO[] pstSubscribeInfo()
		{
			IMVS_SUBSCRIBE_INFO[] array = new IMVS_SUBSCRIBE_INFO[nNum];
			return ExternalCallHelper.IntPtr2Structures<IMVS_SUBSCRIBE_INFO>(pSubscribeInfo, nNum);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public class IMVS_MODULE_SUBSCRIBE_INFO_LIST : ICloneable
	{
		public int nNum;

		private IntPtr pSubscribeInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		private bool bIsClone;

		private IMVS_SUBSCRIBE_INFO[] parrSubscribeInfo;

		public IMVS_MODULE_SUBSCRIBE_INFO_LIST()
		{
			nNum = 0;
			pSubscribeInfo = IntPtr.Zero;
			bIsClone = false;
			parrSubscribeInfo = null;
		}

		public IMVS_MODULE_SUBSCRIBE_INFO_LIST(IMVS_MODULE_SUBSCRIBE_INFO_LIST_Ex stSubscribeInfo)
		{
			nNum = stSubscribeInfo.nNum;
			pSubscribeInfo = stSubscribeInfo.pSubscribeInfo;
			nReserved = stSubscribeInfo.nReserved;
			parrSubscribeInfo = null;
			bIsClone = false;
		}

		/// <summary>
		/// 置空指针变量 避免野指针
		/// </summary>
		public void ResetPtr()
		{
			pSubscribeInfo = IntPtr.Zero;
		}

		public IMVS_SUBSCRIBE_INFO[] pstSubscribeInfo()
		{
			if (!bIsClone)
			{
				IMVS_SUBSCRIBE_INFO[] result = new IMVS_SUBSCRIBE_INFO[nNum];
				if (IntPtr.Zero != pSubscribeInfo)
				{
					result = ExternalCallHelper.IntPtr2Structures<IMVS_SUBSCRIBE_INFO>(pSubscribeInfo, nNum);
				}
				return result;
			}
			return parrSubscribeInfo;
		}

		public object Clone()
		{
			if (!bIsClone)
			{
				bIsClone = true;
				parrSubscribeInfo = new IMVS_SUBSCRIBE_INFO[nNum];
				if (IntPtr.Zero != pSubscribeInfo)
				{
					parrSubscribeInfo = ExternalCallHelper.IntPtr2Structures<IMVS_SUBSCRIBE_INFO>(pSubscribeInfo, nNum);
				}
			}
			return this;
		}
	}

	public struct IMVS_REPORT_MODULE_RES_INFO_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public uint[] anReportModuIds;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_QUERY_MODULE_RES_INFO_LIST
	{
		public int nNum;

		internal IntPtr pPbResultList;

		public int nTotalNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IMVS_MODULE_RESULT_INFO_LIST_EX[] pastModuResultList()
		{
			return ExternalCallHelper.IntPtr2Structures<IMVS_MODULE_RESULT_INFO_LIST_EX>(pPbResultList, nNum);
		}
	}

	public class IMVS_QUERY_MODULE_RES_INFO_LIST_Class
	{
		internal List<IntPtr> ls = new List<IntPtr>();

		public List<int> nNums = new List<int>();

		public List<IntPtr> pPbResultList = new List<IntPtr>();

		public int nNum;

		public IMVS_MODULE_RESULT_INFO_LIST_EX[] pastModuResultList()
		{
			return ExternalCallHelper.IntPtrs2Structures<IMVS_MODULE_RESULT_INFO_LIST_EX>(pPbResultList, nNums);
		}

		public void Release()
		{
			for (int i = 0; i < ls.Count; i++)
			{
				IntPtr pstModuleResBuff = ls[i];
				ServerSDKManager.serverSDKManager.mModuleManager.ReleaseModuleHistoryResult(ref pstModuleResBuff);
			}
		}
	}

	public struct IMVS_EXPORT_MODULE_DATA_INFO_EX
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_EXPORT_MODULE_DATA_INFO
	{
		public byte[] pData;

		public uint nDataLen;

		public IMVS_EXPORT_MODULE_DATA_INFO(IMVS_EXPORT_MODULE_DATA_INFO_EX stExportModuData)
		{
			if (IntPtr.Zero != stExportModuData.pData)
			{
				pData = ExternalCallHelper.IntPtr2Bytes(stExportModuData.pData, (int)stExportModuData.nDataLen);
				nDataLen = stExportModuData.nDataLen;
			}
			else
			{
				pData = null;
				nDataLen = 0u;
			}
		}
	}

	public struct IMVS_PF_IMPORT_MODULE_DATA
	{
		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_MODULE_DATA_INPUT
	{
		public uint nModuleID;

		public uint nDataType;

		public uint nDataNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.Struct)]
		public IMVS_PF_IMPORT_MODULE_DATA[] stImportModuData;

		public uint nRecvTimeout;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_MODULE_DATA_INFO
	{
		public uint nParentNodeID;

		public uint nModuleID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strModuDisplayName;

		public uint nWaitTime;

		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_SYNC_ADD_ONE_MODULE_INFO
	{
		public uint nParentNodeType;

		public uint nParentNodeID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strModuleDllName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strDisplayName;

		public uint nWaitTime;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_INPUT_INT_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public int nValueNum;

		public IntPtr pIntValue;
	}

	public struct IMVS_INPUT_FLOAT_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public int nValueNum;

		public IntPtr pFloatValue;
	}

	public struct IMVS_STRING_DATA_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
		public byte[] strValue;
	}

	public struct IMVS_INPUT_STRING_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public int nValueNum;

		public IntPtr pstStringValue;
	}

	public struct IMVS_INPUT_INT_LIST_INFO
	{
		public int nNum;

		public IntPtr pstIntValueList;
	}

	public struct IMVS_INPUT_FLOAT_LIST_INFO
	{
		public int nNum;

		public IntPtr pstFloatValueList;
	}

	public struct IMVS_INPUT_STRING_LIST_INFO
	{
		public int nNum;

		public IntPtr pstStringValueList;
	}

	public struct IMVS_MODULE_INPUT_DATA_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public uint nDataLen;

		public IntPtr pData;
	}

	public struct IMVS_MODULE_ID_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064, ArraySubType = UnmanagedType.U4)]
		public uint[] anModuleId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_MODULE_LIST_DATA_INFO
	{
		public uint nParentId;

		public IMVS_MODULE_ID_LIST stModuIdList;

		public uint nWaitTime;

		public IntPtr pData;

		public uint nDataLen;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_NODE_UNIT_INFO
	{
		public uint nNodeId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strDisplayName;
	}

	public struct IMVS_GLOBAL_PROCESS_UI_OBJECT_INFO
	{
		public int nStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_PROCESS_GET_UIDATA_INFO
	{
		public uint nProcessId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strProcessName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_IMPORT_GROUP_GET_UIDATA_INFO
	{
		public uint nGroupId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strGroupName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_REPORT_MODULE_STATUS_INFO
	{
		public uint nModuleID;

		public uint nModuProcessStatus;

		public uint nModuleStatus;

		public float fModuleTime;

		public float fAlgorithmTime;

		public float fProcessTime;

		public int bIsSelfRun;
	}

	public struct IMVS_REPORT_MODULE_STATUS_INFO_LIST_EX
	{
		public int nNum;

		public uint nProcID;

		private IntPtr pstModuleStatuInfoPtr;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IMVS_REPORT_MODULE_STATUS_INFO[] pstModuleStatuInfo()
		{
			IMVS_REPORT_MODULE_STATUS_INFO[] array = new IMVS_REPORT_MODULE_STATUS_INFO[nNum];
			return ExternalCallHelper.IntPtr2Structures<IMVS_REPORT_MODULE_STATUS_INFO>(pstModuleStatuInfoPtr, nNum);
		}
	}

	public struct IMVS_REPORT_MODULE_STATUS_INFO_LIST(IMVS_REPORT_MODULE_STATUS_INFO_LIST_EX stReportModuStatList)
	{
		public int nNum = stReportModuStatList.nNum;

		public uint nProcID = stReportModuStatList.nProcID;

		public IMVS_REPORT_MODULE_STATUS_INFO[] pstModuleStatuInfo = stReportModuStatList.pstModuleStatuInfo();
	}

	public struct IMVS_MODULE_PARAM
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		public byte[] strParamValue;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_MODULE_PARAM_LIST
	{
		public uint nParamNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public IMVS_MODULE_PARAM[] stModuleParamList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_DYNAMIC_PARAM_RELATION
	{
		public uint nObjectID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_DYNAMIC_PARAM_RELATION_LIST
	{
		public uint nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public IMVS_DYNAMIC_PARAM_RELATION[] stRelationList;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_UIPARAM_VALUE_EX
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public IntPtr pData;

		public uint nLen;
	}

	public struct IMVS_UIPARAM_VALUE
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public byte[] pParamData;

		public IMVS_UIPARAM_VALUE(IMVS_UIPARAM_VALUE_EX stUIParamValue)
		{
			strParamName = stUIParamValue.strParamName;
			if (IntPtr.Zero != stUIParamValue.pData)
			{
				pParamData = ExternalCallHelper.IntPtr2Bytes(stUIParamValue.pData, (int)stUIParamValue.nLen);
			}
			else
			{
				pParamData = null;
			}
		}
	}

	public struct IMVS_UIPARAM_INFO_EX
	{
		public int nObjectId;

		public int nValNum;

		private IntPtr pastParamValPtr;

		public IMVS_UIPARAM_VALUE[] astParamVal()
		{
			IMVS_UIPARAM_VALUE_EX[] array = new IMVS_UIPARAM_VALUE_EX[nValNum];
			IMVS_UIPARAM_VALUE[] array2 = new IMVS_UIPARAM_VALUE[nValNum];
			if (IntPtr.Zero != pastParamValPtr)
			{
				array = ExternalCallHelper.IntPtr2Structures<IMVS_UIPARAM_VALUE_EX>(pastParamValPtr, nValNum);
				for (int i = 0; i < nValNum; i++)
				{
					array2[i] = new IMVS_UIPARAM_VALUE(array[i]);
				}
			}
			return array2;
		}
	}

	public struct IMVS_UIPARAM_INFO(IMVS_UIPARAM_INFO_EX stUIParamValueInfo)
	{
		public int nObjectId = stUIParamValueInfo.nObjectId;

		public int nValNum = stUIParamValueInfo.nValNum;

		public IMVS_UIPARAM_VALUE[] astParamVal = stUIParamValueInfo.astParamVal();
	}

	public struct IMVS_UIPARAM_INFO_LIST_EX
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public IMVS_UIPARAM_INFO_EX[] astUiParamInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	public struct IMVS_UIPARAM_INFO_LIST
	{
		public int nNum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public IMVS_UIPARAM_INFO[] astUiParamInfo = new IMVS_UIPARAM_INFO[2064];

		public IMVS_UIPARAM_INFO_LIST(IMVS_UIPARAM_INFO_LIST_EX stUiParamInfoList)
		{
			for (int i = 0; i < stUiParamInfoList.nNum; i++)
			{
				astUiParamInfo[i] = new IMVS_UIPARAM_INFO(stUiParamInfoList.astUiParamInfo[i]);
			}
			nNum = stUiParamInfoList.nNum;
		}
	}

	/// 模块流程信息
	[Serializable]
	public struct IMVS_SYNC_IMPORT_PROCESS_INFO
	{
		public uint nProcID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strProcName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// CH: 输入图像数据信息结构 | EN: Input image data information structure
	/// </summary>
	[Serializable]
	public struct IMVS_PF_IMAGE_DATA_INFO
	{
		/// <param name="pImgData">CH: 图像数据 | EN: Image data</param>
		public IntPtr pImgData;

		/// <param name="iImgDataLen">CH: 图像数据长度 | EN: Image data length</param>
		public int iImgDataLen;

		/// <param name="iWidth">CH: 图像宽度 | EN: Image width</param>
		public int iWidth;

		/// <param name="iHeight">CH: 图像高度 | EN: Image height</param>
		public int iHeight;

		/// <param name="iImgFormat">CH: 图像像素格式（1：Mono8 2：RGB） | EN: Image pixel format (1:Mono8 2:RGB)</param>
		public int iImgFormat;
	}

	/// <summary>
	/// CH: 用户输入图像信息结构 | EN: User input image information structure
	/// </summary>
	[Serializable]
	public struct IMVS_PF_INPUT_IMAGE_INFO
	{
		/// <param name="nModuleID">CH: 模块ID | EN: Module ID</param>
		public uint nModuleID;

		/// <param name="nDataType">CH: 图像数据类型(0:二进制数据, 1:路径) | EN: Image pixel format(0:Binary data, 1:Path)</param>
		public int nDataType;

		/// <param name="strImagePath">CH: 图像文件路径 | EN: Image file path</param>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string strImagePath;

		/// <param name="stImageDataInfo">CH: 图像数据 | EN: Image data</param>
		public IMVS_PF_IMAGE_DATA_INFO stImageDataInfo;

		public int nIsP2PSend;

		public int nIsWaitCopy;

		/// <param name="nReserved">CH: 保留字段 | EN: Reserved</param>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 模块禁用回调状态信息
	/// </summary>
	[Serializable]
	public struct IMVS_MODULE_STATE_INFO
	{
		/// <param name="nModuleId">模块ID</param>
		public uint nModuleId;

		/// <param name="nEnable">使能状态</param>
		public uint nEnable;

		/// <param name="nCallback">回调状态</param>
		public uint nCallback;
	}

	/// <summary>
	/// 模块禁用回调状态列表
	/// </summary>
	[Serializable]
	public struct IMVS_MODULE_STATE_LIST
	{
		/// <param name="nNum">模块数量</param>
		public int nNum;

		/// <param name="astModuState">模块状态列表</param>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2064)]
		public IMVS_MODULE_STATE_INFO[] astModuState;

		/// <param name="nReserved">CH: 保留字段 | EN: Reserved</param>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 第三方软件信息
	/// </summary>
	[Serializable]
	public struct IMVS_THIRD_SOFTWARE_INFO
	{
		/// <param name="nPid">进程ID</param>
		public uint nPid;

		/// <param name="strProcName">进程名称</param>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strProcName;
	}

	/// <summary>
	/// 第三方软件信息列表
	/// </summary>
	[Serializable]
	public struct IMVS_THIRD_SOFTWARE_INFO_LIST
	{
		/// <param name="nNum">模块数量</param>
		public int nNum;

		/// <param name="astSoftwareInfo">软件信息列表</param>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public IMVS_THIRD_SOFTWARE_INFO[] astSoftwareInfo;
	}

	/// @enum IMVS_VALUE_TYPE
	/// @brief 值类型
	public enum IMVS_VALUE_TYPE
	{
		VALUE_TYPE_INT = 1,
		VALUE_TYPE_FLOAT,
		VALUE_TYPE_STRING,
		VALUE_TYPE_IMAGE,
		VALUE_TYPE_POINTSET,
		VALUE_TYPE_BYTE
	}

	/// @enum IMVS_COMBINATION_TYPE
	/// @brief 组合类型
	public enum IMVS_COMBINATION_STYLE
	{
		COMBINATION_TYPE_COMBINATION,
		COMBINATION_TYPE_IMAGE,
		COMBINATION_TYPE_POINT,
		COMBINATION_TYPE_ROIBOX,
		COMBINATION_TYPE_ROIANNULUS,
		COMBINATION_TYPE_FIXTURE,
		COMBINATION_TYPE_CLASSINFO,
		COMBINATION_TYPE_LINE,
		COMBINATION_TYPE_CIRCLE,
		COMBINATION_TYPE_ACQUIMAGE,
		COMBINATION_TYPE_PIXELIMAGE,
		COMBINATION_TYPE_ANNULUS,
		COMBINATION_TYPE_CONTOURPOINT,
		COMBINATION_TYPE_RECT,
		COMBINATION_TYPE_DEBUG
	}

	[Serializable]
	public struct IMVS_BASE_DATA_VALUE
	{
		public IntPtr pData;

		public uint nLen;
	}

	[Serializable]
	public struct IMVS_VALUE_INFO
	{
		public IMVS_VALUE_TYPE eValType;

		public int nIntVal;

		public float fFloatVal;

		public IMVS_BASE_DATA_VALUE stStringInfo;

		public IMVS_BASE_DATA_VALUE stImgInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	public class IMVS_MODULE_IO_INFO_EX : ICloneable
	{
		public int nNum;

		public int nDataType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;

		public IMVS_COMBINATION_STYLE eCombinationType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] szCombinationType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public IntPtr[] pstInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		public byte[] szExtraData;

		public byte[] pstInfoData;

		public List<IMVS_MODULE_IO_INFO_EX> childInfo;

		public IMVS_MODULE_IO_INFO_EX(IMVS_MODULE_IO_INFO info)
		{
			nNum = info.nNum;
			nDataType = info.nDataType;
			szExtraData = info.szExtraData;
		}

		public IMVS_MODULE_IO_INFO GetSourceValue()
		{
			return new IMVS_MODULE_IO_INFO
			{
				nNum = nNum,
				nDataType = nDataType
			};
		}

		public object Clone()
		{
			if (nDataType == 1)
			{
				pstInfoData = new byte[Marshal.SizeOf(typeof(IMVS_VALUE_INFO))];
				Marshal.Copy(pstInfo[0], pstInfoData, 0, Marshal.SizeOf(typeof(IMVS_VALUE_INFO)));
			}
			else if (nDataType == 2)
			{
				childInfo = new List<IMVS_MODULE_IO_INFO_EX>();
				for (int i = 0; i < nNum; i++)
				{
					IMVS_MODULE_IO_INFO structure = default;
					ExternalCallHelper.PtrToStructureWithoutFree(pstInfo[i], ref structure);
					IMVS_MODULE_IO_INFO_EX iMVS_MODULE_IO_INFO_EX = new IMVS_MODULE_IO_INFO_EX(structure);
					iMVS_MODULE_IO_INFO_EX.Clone();
					childInfo.Add(iMVS_MODULE_IO_INFO_EX);
				}
			}
			return this;
		}
	}

	[Serializable]
	public struct KEY_NODE_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
		public byte[] strGroupName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
		public byte[] strNodeName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
		public byte[] strDisplayName;

		public int nNodeType;

		public int nDisplayStatus;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] strValue;
	}

	[Serializable]
	public struct KEY_NODE_INFO_LIST
	{
		public int nNodeNum;

		public int nNodeNumSize;

		public IntPtr stKeyNodeInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
		public int[] nReserved;

		public KEY_NODE_INFO[] pstValueRange()
		{
			KEY_NODE_INFO[] array = new KEY_NODE_INFO[nNodeNum];
			return ExternalCallHelper.IntPtr2Structures<KEY_NODE_INFO>(stKeyNodeInfo, nNodeNum);
		}
	}

	[Serializable]
	public struct KEY_NODE_INFO_LIST_EX(KEY_NODE_INFO_LIST list)
	{
		public int nNodeNum = list.nNodeNum;

		public KEY_NODE_INFO[] kEY_NODE_INFOs = list.pstValueRange();
	}

	[Serializable]
	public struct IMVS_MODULE_IO_INFO
	{
		public int nDataType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] szParamName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] szParamType;

		public int nNum;

		public IntPtr pstCombInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		public byte[] szExtraData;

		public void Release()
		{
			if (1 != nDataType && 2 == nDataType && IntPtr.Zero != pstCombInfo)
			{
				for (int i = 0; i < nNum; i++)
				{
					IMVS_MODULE_IO_INFO iMVS_MODULE_IO_INFO = default;
					((IMVS_MODULE_IO_INFO)Marshal.PtrToStructure(pstCombInfo + i * Marshal.SizeOf(typeof(IMVS_MODULE_IO_INFO)), typeof(IMVS_MODULE_IO_INFO))).Release();
				}
				Marshal.FreeHGlobal(pstCombInfo);
				pstCombInfo = IntPtr.Zero;
			}
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class IMVS_MODULE_IO_INFO_LIST
	{
		public int nIoClass;

		public int nNum;

		public IntPtr pstModuIoInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		~IMVS_MODULE_IO_INFO_LIST()
		{
			_ = pstModuIoInfo;
			if (IntPtr.Zero != pstModuIoInfo)
			{
				int i = 0;
				for (; i < nNum; i++)
				{
					IMVS_MODULE_IO_INFO iMVS_MODULE_IO_INFO = default;
					((IMVS_MODULE_IO_INFO)Marshal.PtrToStructure(pstModuIoInfo + i * Marshal.SizeOf(typeof(IMVS_MODULE_IO_INFO)), typeof(IMVS_MODULE_IO_INFO))).Release();
				}
				Marshal.FreeHGlobal(pstModuIoInfo);
				pstModuIoInfo = IntPtr.Zero;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public class IMVS_MODULE_IO_INFO_LIST_EX : ICloneable
	{
		public int nNum;

		public int nParamType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
		public IMVS_MODULE_IO_INFO[] astModuInputInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;

		public IList<IMVS_MODULE_IO_INFO_EX> astModuInputInfoData;

		public IMVS_MODULE_IO_INFO_LIST_EX(IMVS_MODULE_IO_INFO_LIST list)
		{
		}

		public IMVS_MODULE_IO_INFO_LIST GetSourceValue()
		{
			return new IMVS_MODULE_IO_INFO_LIST();
		}

		public object Clone()
		{
			astModuInputInfoData = new List<IMVS_MODULE_IO_INFO_EX>();
			for (int i = 0; i < nNum; i++)
			{
				IMVS_MODULE_IO_INFO_EX iMVS_MODULE_IO_INFO_EX = new IMVS_MODULE_IO_INFO_EX(astModuInputInfo[i]);
				iMVS_MODULE_IO_INFO_EX.Clone();
				astModuInputInfoData.Add(iMVS_MODULE_IO_INFO_EX);
			}
			return this;
		}
	}

	public enum IMVS_FUZZY_MATCH
	{
		FUZZY_MATCH_NO,
		FUZZY_MATCH_YES
	}

	public enum IMVS_IO_BE_SUBSCRIBED_TYPE
	{
		IO_BE_SUBSCRIBED_TYPE_GENARAL,
		IO_BE_SUBSCRIBED_TYPE_PARAMS
	}

	public struct FTP_REMOTE_FILE_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		public byte[] FilePermission;

		public int cFileHardLink;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		public byte[] FileUser;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		public byte[] FileGroup;

		public int FileSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		public byte[] FileTime;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		public byte[] FileName;
	}

	/// <summary>
	/// FTP文件夹信息
	/// </summary>
	public struct FTP_REMOTE_FILE_LIST_INFO
	{
		public IntPtr FTPFileInfos;

		public int FileNum;
	}

	/// <summary>
	/// 算法版本信息
	/// </summary>
	public struct IMVS_ALGORITHM_VERSION_INFO
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] Normal;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public byte[] Deeplearning;
	}

	/// <summary>
	/// 与模块的REAL_MODE_IAMGE_TRANS_INFO_结构体保持一致
	/// </summary>
	public struct IMVS_REAL_MODE_IAMGE_TRANS_INFO
	{
		public int nModuleId;

		public int nWidth;

		public int nHeight;

		public long nPixelFormat;

		public int nDataLen;

		public int nFrameNum;

		public int nLostFrameCount;

		public int nLostPacketCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public int[] nReserved;
	}

	/// <summary>
	/// 代理透传的相机取流数据
	/// </summary>
	public struct IMVS_REAL_MODE_IAMGE_TRANS_INFO_WITH_DATA
	{
		public IMVS_REAL_MODE_IAMGE_TRANS_INFO ImageInfo;

		public IntPtr ImageData;
	}

	/// <summary>
	/// 容器关联模块信息
	/// </summary>
	public struct IMVS_CONTAINER_RELATE_MODULE_INFO
	{
		public int nContainerModuleType;

		public uint nContainerModuleId;

		public int nRelateModuleType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strRelateModuleDefaultName;

		public int nRelateModuleId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public byte[] strRelateModuleName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public uint[] nReserved;
	}

	public struct IMVS_GROUP_IO_REPORT_MODE_INFO
	{
		public uint nMode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		public byte[] strParamName;
	}

	public struct IMVS_GROUP_IO_REPORT_MODE_INFO_LIST
	{
		public uint nGroupId;

		public uint nIONum;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType = UnmanagedType.Struct)]
		public IMVS_GROUP_IO_REPORT_MODE_INFO[] stGroupIOMode;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
		public uint[] nReserved;
	}

	/// <summary>
	/// 枚举定义：回调函数中的模块索引
	/// </summary>
	public enum IMVS_PF_MODU_NAME_KEY
	{
		IMVS_PF_MODU_LOCALIMAGEVIEW,
		IMVS_PF_MODU_CAMERAMODU,
		IMVS_PF_MODU_SAVEIMAGE,
		IMVS_PF_MODU_HPFEATUREMATCH,
		IMVS_PF_MODU_FASTFEATUREMATCH,
		IMVS_PF_MODU_CIRFINDMODU,
		IMVS_PF_MODU_LINEFINDMODU,
		IMVS_PF_MODU_BLOBFINDMODU,
		IMVS_PF_MODU_CALIPERMODU,
		IMVS_PF_MODU_CALIPEREDGEMODU,
		IMVS_PF_MODU_EDGEWIDTHFINDMODU,
		IMVS_PF_MODU_FIXTUREMODU,
		IMVS_PF_MODU_RECTFINDMODU,
		IMVS_PF_MODU_PEAKFINDMODU,
		IMVS_PF_MODU_CALIPERCORNERMODU,
		IMVS_PF_MODU_PAIRLINEMODU,
		IMVS_PF_MODU_L2CMEASUREMODU,
		IMVS_PF_MODU_C2CMEASUREMODU,
		IMVS_PF_MODU_P2CMEASUREMODU,
		IMVS_PF_MODU_P2LMEASUREMODU,
		IMVS_PF_MODU_L2LMEASUREMODU,
		IMVS_PF_MODU_P2PMEASUREMODU,
		IMVS_PF_MODU_CIRCLEFITMODU,
		IMVS_PF_MODU_LINEFITMODU,
		IMVS_PF_MODU_INTENSITYMEASUREMODU,
		IMVS_PF_MODU_PIXELCOUNTMODU,
		IMVS_PF_MODU_HISTTOOLMODU,
		IMVS_PF_MODU_GEOMETRYCREATE,
		IMVS_PF_MODU_2DCRMODU,
		IMVS_PF_MODU_BCRMODU,
		IMVS_PF_MODU_OCRMODU,
		IMVS_PF_MODU_VERICODEMODU,
		IMVS_PF_MODU_CALIBBOARDMODU,
		IMVS_PF_MODU_ALIGNCALIBMODU,
		IMVS_PF_MODU_CALIBTRANSFORMMODU,
		IMVS_PF_MODU_SCALETRANSFORMMODU,
		IMVS_PF_MODU_IMAGECORRECTMODU,
		IMVS_PF_MODU_IMAGEMORPHMODU,
		IMVS_PF_MODU_BINARYMODU,
		IMVS_PF_MODU_IMAGEFILTERMODU,
		IMVS_PF_MODU_IMAGEENHANCEMODU,
		IMVS_PF_MODU_IMAGEMATHMODU,
		IMVS_PF_MODU_IMAGESHARPNESSMODU,
		IMVS_PF_MODU_IMAGEFIXTUREMODU,
		IMVS_PF_MODU_SHADECORRECTMODU,
		IMVS_PF_MODU_AFFINETRANSFORMMODU,
		IMVS_PF_MODU_POLARUNWARMODU,
		IMVS_PF_MODU_COLOREXTRACTMODU,
		IMVS_PF_MODU_COLORMEASUREMODU,
		IMVS_PF_MODU_COLORTRANSFORMMODU,
		IMVS_PF_MODU_IFMODU,
		IMVS_PF_MODU_BRANCHMODU,
		IMVS_PF_MODU_ANDMODU,
		IMVS_PF_MODU_FORMATMODU,
		IMVS_PF_MODU_CALCULATORMODU,
		IMVS_PF_MODU_STRINGCOMPAREMODU,
		IMVS_PF_MODU_SHELLMODU,
		IMVS_PF_MODU_LOOPCONTROL,
		IMVS_PF_MODU_POINTSETMODU,
		IMVS_PF_MODU_UDPMODU,
		IMVS_PF_MODU_TCPCLIENTMODU,
		IMVS_PF_MODU_TCPSERVERMODU,
		IMVS_PF_MODU_SERIALCOMMMODU,
		IMVS_PF_MODU_IOMODU,
		IMVS_PF_MODU_MODBUSMODU,
		IMVS_PF_MODU_OCRDLMODU,
		IMVS_PF_MODU_BRANCHSTRINGCPMOLD,
		IMVS_PF_MODU_FRAMEMEAN,
		IMVS_PF_MODU_IMAGECALIB,
		IMVS_PF_MODU_IMAGECOMBINE,
		IMVS_PF_MODU_IMAGENORMLIZE,
		IMVS_PF_MODU_MARKINSP,
		IMVS_PF_MODU_REGIONCOPY,
		IMVS_PF_MODU_PLCMODULE,
		IMVS_PF_MODU_READDATAS,
		IMVS_PF_MODU_SAVETEXT,
		IMVS_PF_MODU_SENDDATAS,
		IMVS_PF_MODU_TIMESTATISTICS,
		IMVS_PF_MODU_CAMERAMAPMODU,
		IMVS_PF_MODU_SINGLEPOINTALIGNMODU,
		IMVS_PF_MODU_MULTIPOINTALIGNMODU,
		IMVS_PF_MODU_LINEALIGNMODU,
		IMVS_PF_MODU_XYT2UVWMODU,
		IMVS_PF_MODU_CNNFLAWMODU,
		IMVS_PF_MODU_CNNCHARDETECTMODU,
		IMVS_PF_MODU_CNNCLASSIFYMODU,
		IMVS_PF_MODU_CNNDETECTMODU,
		IMVS_PF_MODU_CNNTRAINTOOLMODU,
		IMVS_PF_MODU_CIRCLEEDGEINSPMODU,
		IMVS_PF_MODU_LINEEDGEINSPMODU,
		IMVS_PF_MODU_CIRCLEEDGEPAIRINSPMODU,
		IMVS_PF_MODU_LINEEDGEPAIRINSPMODU,
		IMVS_PF_MODU_EDGEINSPGROUPMODU,
		IMVS_PF_MODU_EDGEPAIRINSPGROUPMODU,
		IMVS_PF_MODU_EDGEFLAWINSPMODU,
		IMVS_PF_MODU_EDGEPAIRFLAWINSPMODU,
		IMVS_PF_MODU_BRANCHSTRINGCPML,
		IMVS_PF_MODU_CNNCHARDETECTMODUC,
		IMVS_PF_MODU_CNNCLASSIFYMODUC,
		IMVS_PF_MODU_CNNDETECTMODUC,
		IMVS_PF_MODU_IMAGEBUFFERMODULE,
		IMVS_PF_MODU_GRAYMATCHMODU,
		IMVS_PF_MODU_COLORDISTINGUISHMODU,
		IMVS_PF_MODU_IMAGEUNDODISTORTMODU,
		IMVS_PF_MODU_CAMERAIOMODULE,
		IMVS_PF_MODU_IMAGESOURCEMODULE,
		IMVS_PF_MODU_LIGHTMODULE,
		IMVS_PF_MODU_QUADRANGEFINDMODULE,
		IMVS_PF_MODU_LINEFINDGROUPMODULE,
		IMVS_PF_MODU_MULTILINEFINDMODULE,
		IMVS_PF_MODU_TWOIMAGECALIBMODULE,
		IMVS_PF_MODU_NIMGCALIBMODULE,
		IMVS_PF_MODU_IMGSTITCHMODULE,
		IMVS_PF_MODU_GEOMETRICTRANSFORMMODULE,
		IMVS_PF_MODU_DATASETMODULE,
		IMVS_PF_MODU_DATAASSEMBLEMODULE,
		IMVS_PF_MODU_DATAANALYSISMODULE,
		IMVS_PF_MODU_CNNSINGLECHARDETECTMODU,
		IMVS_PF_MODU_CNNSINGLECHARDETECTMODUC,
		IMVS_PF_MODU_GROUPMODULE,
		IMVS_PF_MODU_NAME_COUNT
	}

	/// <summary>
	/// Error code definition
	/// </summary>
	[Serializable]
	public enum MVD_ERROR_CODE
	{
		/// <summary>
		/// ch: 成功，无错误 | en:Successed, no error
		/// </summary>
		MVD_OK = 0,
		/// <summary>
		/// ch: 错误或无效的句柄 | en:Error or invalid handle
		/// </summary>
		MVD_E_HANDLE = -2146435072,
		/// <summary>
		/// ch: 不支持的功能 | en:Not supported function
		/// </summary>
		MVD_E_SUPPORT = -2146435071,
		/// <summary>
		/// ch: 超出限制的能力范围 | en: Beyond the limits of capacity
		/// </summary>
		MVD_E_OVER_ABILITY = -2146435070,
		/// <summary>
		/// ch: 函数调用顺序错误 | en:Function calling order error
		/// </summary>
		MVD_E_CALLORDER = -2146435069,
		/// <summary>
		/// ch: 前置条件有误 | en:Precondition error
		/// </summary>
		MVD_E_PRECONDITION = -2146435068,
		/// <summary>
		/// ch: 参数超出范围限制 | en:Parameter out of range limit
		/// </summary>
		MVD_E_PARAMETER_RANGE = -2146435067,
		/// <summary>
		/// ch: 参数非法 | en:Illegal parameter
		/// </summary>
		MVD_E_PARAMETER_ILLEGAL = -2146435066,
		/// <summary>
		/// ch: 参数互相不匹配 | en:Parameters do not match each other
		/// </summary>
		MVD_E_PARAMETER_MATCH = -2146435065,
		/// <summary>
		/// ch: 通用参数错误 | en:General parameter error
		/// </summary>
		MVD_E_PARAMETER_GENERIC = -2146435064,
		/// <summary>
		/// ch: 资源申请失败 | en:Applying resource failed
		/// </summary> 
		MVD_E_RESOURCE = -2146435063,
		/// <summary>
		/// ch: 缓存已满 | en:Out of memory
		/// </summary> 
		MVD_E_BUFOVER = -2146435062,
		/// <summary>
		/// ch: 没有可输出的缓存 | en:No Avaliable Buffer
		/// </summary>
		MVD_E_NOOUTBUF = -2146435061,
		/// <summary>
		/// ch: 传入的内存空间不足 | en:Insufficient memory
		/// </summary>
		MVD_E_NOENOUGH_BUF = -2146435060,
		/// <summary>
		/// ch: 不支持的文件路径 | en: Unsupported file path
		/// </summary> 
		MVD_E_FILE_PATH = -2146435059,
		/// <summary>
		/// ch: 不支持的文件格式 | en: Unsupported file format
		/// </summary> 
		MVD_E_FILE_FORMAT = -2146435058,
		/// <summary>
		/// ch: 文件损坏 | en: Corrupted file
		/// </summary> 
		MVD_E_FILE_CORRUPTED = -2146435057,
		/// <summary>
		/// ch: 通用文件错误 | en: General file error
		/// </summary>
		MVD_E_FILE_GENERIC = -2146435056,
		/// <summary>
		/// ch: 无数据 | en:No data
		/// </summary> 
		MVD_E_NODATA = -2146435055,
		/// <summary>
		/// ch: 异常图像 | en:Abnormal image
		/// </summary> 
		MVD_E_ABNORMAL_IMAGE = -2146435054,
		/// <summary>
		/// ch: 版本不匹配 | en:Version mismatches
		/// </summary> 
		MVD_E_VERSION = -2146435053,
		/// <summary>
		/// ch: 动态导入DLL失败 | en:Load library failed
		/// </summary> 
		MVD_E_LOAD_LIBRARY = -2146435052,
		/// <summary>
		/// ch: 运行环境错误 | en: Wrong with running environment
		/// </summary> 
		MVD_E_RUNTIME = -2146435051,
		/// <summary>
		/// 没有可用设备，不存在可用的显卡或者显卡驱动未更新| en: No avaliable device
		/// </summary> 
		MVD_E_NO_AVAILABLE_DEVICE = -2146435050,
		/// <summary>
		/// 模型类型错误| en: Wrong with model type
		/// </summary> 
		MVD_E_MODEL_TYPE = -2146435049,
		/// <summary>
		/// 模型版本错误| en: Wrong with model
		/// </summary> 
		MVD_E_MODEL_VERSION = -2146435048,
		/// <summary>
		/// 模型解析错误| en: Wrong with model
		/// </summary> 
		MVD_E_MODEL_ANALYSIS = -2146435047,
		/// <summary>
		/// 申请cpu内存失败| en: Fail to allocate cpu memory
		/// </summary> 
		MVD_E_OUTOF_MEMORY = -2146435046,
		/// <summary>
		/// 申请gpu内存失败| en: Fail to allocate gpu memory
		/// </summary> 
		MVD_E_OUTOF_GPUMEMORY = -2146435045,
		/// <summary>
		/// ROI类型不支持| en: Unsupported ROI shape type
		/// </summary> 
		MVD_E_ROI_TYPE_ERROR = -2146435044,
		/// <summary>
		/// 不支持带角度的ROI| en: Unsupported ROI with angle
		/// </summary> 
		MVD_E_ROI_ANGLE_ERROR = -2146435043,
		/// <summary>
		/// 运行参数名有误| en: Run param name error
		/// </summary> 
		MVD_E_PARAM_KEYNAME = -2146435042,
		/// <summary>
		/// 环境变量配置不正确| en: The environment variable is misconfigured
		/// </summary> 
		MVD_E_MVDALGO_DEV_ENV = -2146435041,
		/// <summary>
		/// 获取加密狗Key失败| en: Failed to get key from lock
		/// </summary> 
		MVD_E_GET_LOCK_KEY = -2146435040,
		/// /// <summary>
		///             数据加密失败| en: Failed to exceed data encryption
		///             </summary> 
		MVD_E_DATA_ENCRYPTION = -2146435039,
		/// /// <summary>
		///             数据解密失败| en: Failed to exceed data decryption
		///             </summary> 
		MVD_E_DATA_DECRYPTION = -2146435038,
		/// /// <summary>
		///             数据解析失败| en: Failed to parse data
		///             </summary> 
		MVD_E_DATA_PARSE = -2146435037,
		/// <summary>
		/// ROI超出范围 | en: ROI out of range limit
		/// </summary>
		MVD_E_ROI_RANGE = -2146435035,
		/// <summary>
		/// ch: 未知的错误 | en:Unknown error
		/// </summary> 
		MVD_E_UNKNOW = -2146434817,
		/// <summary>
		/// ch: 算子版本错误 | en:Unknown error on algorithms
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM = -2146434559,
		/// <summary>
		/// ch: 配置错误 | en: Wrong configuration
		/// </summary>     
		MVD_ALGORITHM_E_CONFIG = -2146434558,
		/// <summary>
		/// ch: 未知错误 | en:Unknown error on algorithm SDK
		/// </summary>     
		MVD_ALGORITHM_E_UNKNOW = -2146434557,
		/// <summary>
		/// ch: 字符缺陷检测粗定位失败
		/// </summary>     
		MVD_ALGORITHM_E_MKINSP_ROUGH_MATCH_FAILED = -2146434556,
		/// <summary>
		/// ch: 字符缺陷检测模板版本号不支持
		/// </summary>     
		MVD_ALGORITHM_E_MKINSP_MODEL_VERSION_NOT_SUPPORT = -2146434555,
		/// <summary>
		/// 图像像素格式不正确或者不支持 | en: Image pixel format is not supported or incorrect
		/// </summary> 
		MVD_E_RENDER_IMG_FORMAT = -2146434303,
		/// <summary>
		/// 图像类型不支持或不正确 | en: Image type is not supported or incorrect
		/// </summary> 
		MVD_E_RENDER_IMG_TYPE = -2146434302,
		/// <summary>
		/// 图像宽高不正确或者超出范围| en: Image width or height is incorrect or unsupported
		/// </summary> 
		MVD_E_RENDER_IMG_SIZE = -2146434301,
		/// <summary>
		/// 图像编解码失败 | en: Image encoding and decoding failed
		/// </summary> 
		MVD_E_RENDER_IMG_ENCOD = -2146434300,
		/// <summary>
		/// 画布缩放超出限制 | en: Canvas zoom is out of scope
		/// </summary> 
		MVD_E_RENDER_CANVAS_ZOOM = -2146434299,
		/// <summary>
		/// 画布大小范围非法 | en: Canvas size range is illegal
		/// </summary> 
		MVD_E_RENDER_CANVAS_PARAM = -2146434298,
		/// <summary>
		/// 渲染引擎通用错误 | en: Rendering engine generic error
		/// </summary> 
		MVD_E_RENDER_GENERIC = -2146434297,
		/// <summary>
		/// 渲染引擎绘制范围有误的图形 | en: Graphics with the wrong rendering engine scope
		/// </summary> 
		MVD_E_RENDER_ILLEGAL_SHAPE = -2146434296,
		/// <summary>
		/// 渲染引擎画布拷贝错误 | en: Render engine canvas copy error
		/// </summary> 
		MVD_E_RENDER_MEMORY = -2146434295,
		/// <summary>
		/// 图像或图形句柄不存在或无效 | en: Render Shape or Img Handle not Find
		/// </summary> 
		MVD_E_RENDER_HANDLD_NOTFIND = -2146434294,
		/// <summary>
		/// ch: 不确定类型错误（接口函数共用）
		/// </summary>
		MVALG_STS_ERR = 268435456,
		/// <summary>
		/// ch: ABILITY存在无效参数
		/// </summary>
		MVALG_STS_ERR_ABILITY_ARG = 268435457,
		/// <summary>
		/// ch: 内存地址为空
		/// </summary>
		MVALG_STS_ERR_MEM_NULL = 268435458,
		/// <summary>
		/// ch: 内存对齐不满足要求
		/// </summary>
		MVALG_STS_ERR_MEM_ALIGN = 268435459,
		/// <summary>
		/// ch: 内存空间大小不够
		/// </summary>
		MVALG_STS_ERR_MEM_LACK = 268435460,
		/// <summary>
		/// ch: 内存空间大小不满足对齐要求
		/// </summary>
		MVALG_STS_ERR_MEM_SIZE_ALIGN = 268435461,
		/// <summary>
		/// ch: 内存地址不满足对齐要求
		/// </summary>
		MVALG_STS_ERR_MEM_ADDR_ALIGN = 268435462,
		/// <summary>
		/// ch: 图像格式不正确或者不支持
		/// </summary>
		MVALG_STS_ERR_IMG_FORMAT = 268435463,
		/// <summary>
		/// ch: 图像宽高不正确或者超出范围
		/// </summary>
		MVALG_STS_ERR_IMG_SIZE = 268435464,
		/// <summary>
		/// ch: 图像宽高与step参数不匹配
		/// </summary>
		MVALG_STS_ERR_IMG_STEP = 268435465,
		/// <summary>
		/// ch: 图像数据存储地址为空（某个分量）
		/// </summary>
		MVALG_STS_ERR_IMG_DATA_NULL = 268435466,
		/// <summary>
		/// ch: 设置、获取参数类型不正确
		/// </summary>
		MVALG_STS_ERR_CFG_TYPE = 268435467,
		/// <summary>
		/// ch: 设置、获取参数输入、输出结构体大小不正确
		/// </summary>
		MVALG_STS_ERR_CFG_SIZE = 268435468,
		/// <summary>
		/// ch: 处理类型不正确
		/// </summary>
		MVALG_STS_ERR_PRC_TYPE = 268435469,
		/// <summary>
		/// ch: 处理时输入、输出参数大小不正确
		/// </summary>
		MVALG_STS_ERR_PRC_SIZE = 268435470,
		/// <summary>
		/// ch: 子处理类型不正确
		/// </summary>
		MVALG_STS_ERR_FUNC_TYPE = 268435471,
		/// <summary>
		/// ch: 子处理时输入、输出参数大小不正确
		/// </summary>
		MVALG_STS_ERR_FUNC_SIZE = 268435472,
		/// <summary>
		/// ch: index参数不正确
		/// </summary>
		MVALG_STS_ERR_PARAM_INDEX = 268435473,
		/// <summary>
		/// ch: value参数不正确或者超出范围
		/// </summary>
		MVALG_STS_ERR_PARAM_VALUE = 268435474,
		/// <summary>
		/// ch: param_num参数不正确
		/// </summary>
		MVALG_STS_ERR_PARAM_NUM = 268435475,
		/// <summary>
		/// ch: 函数参数指针为空（共用）
		/// </summary>
		MVALG_STS_ERR_NULL_PTR = 268435476,
		/// <summary>
		/// ch: 超过HKA限定最大内存
		/// </summary>
		MVALG_STS_ERR_OVER_MAX_MEM = 268435477,
		/// <summary>
		/// ch: 回调函数出错
		/// </summary>
		MVALG_STS_ERR_CALL_BACK = 268435478,
		/// <summary>
		/// ch: 加密错误
		/// </summary>
		MVALG_STS_ERR_ENCRYPT = 268435479,
		/// <summary>
		/// ch: 算法库使用期限错误
		/// </summary>
		MVALG_STS_ERR_EXPIRE = 268435480,
		/// <summary>
		/// ch: 参数范围不正确
		/// </summary>
		MVALG_STS_ERR_BAD_ARG = 268435481,
		/// <summary>
		/// ch: 数据大小不正确（一维数据len，二维数据的HKA_SIZE）
		/// </summary>
		MVALG_STS_ERR_DATA_SIZE = 268435482,
		/// <summary>
		/// ch: 数据step不正确（除HKA_IMAGE结构体之外）
		/// </summary>
		MVALG_STS_ERR_STEP = 268435483,
		/// <summary>
		/// ch: cpu不支持优化代码中的指令集
		/// </summary>
		MVALG_STS_ERR_CPUID = 268435484,
		/// <summary>
		/// ch: 警告
		/// </summary>
		MVALG_STS_WARNING = 268435485,
		/// <summary>
		/// ch: 算法库超时
		/// </summary>
		MVALG_STS_ERR_TIME_OUT = 268435486,
		/// <summary>
		/// ch: 算法版本号出错
		/// </summary>
		MVALG_STS_ERR_LIB_VERSION = 268435487,
		/// <summary>
		/// ch: 模型版本号出错：模板版本号与当前版本不符
		/// </summary>
		MVALG_STS_ERR_MODEL_VERSION = 268435488,
		/// <summary>
		/// ch: GPU内存分配错误
		/// </summary>
		MVALG_STS_ERR_GPU_MEM_ALLOC = 268435489,
		/// <summary>
		/// ch: 文件不存在
		/// </summary>
		MVALG_STS_ERR_FILE_NON_EXIST = 268435490,
		/// <summary>
		/// ch: 字符串为空
		/// </summary>
		MVALG_STS_ERR_NONE_STRING = 268435491,
		/// <summary>
		/// ch: 图像解码器错误
		/// </summary>
		MVALG_STS_ERR_IMAGE_CODEC = 268435492,
		/// <summary>
		/// ch: 打开文件错误
		/// </summary>
		MVALG_STS_ERR_FILE_OPEN = 268435493,
		/// <summary>
		/// ch: 文件读取错误
		/// </summary>
		MVALG_STS_ERR_FILE_READ = 268435494,
		/// <summary>
		/// ch: 文件写错误
		/// </summary>
		MVALG_STS_ERR_FILE_WRITE = 268435495,
		/// <summary>
		/// ch: 文件读取大小错误
		/// </summary>
		MVALG_STS_ERR_FILE_READ_SIZE = 268435496,
		/// <summary>
		/// ch: 文件类型错误
		/// </summary>
		MVALG_STS_ERR_FILE_TYPE = 268435497,
		/// <summary>
		/// ch: 模型类型错误
		/// </summary>
		MVALG_STS_ERR_MODEL_TYPE = 268435498,
		/// <summary>
		/// ch: 分配内存错误
		/// </summary>
		MVALG_STS_ERR_MALLOC_MEM = 268435499,
		/// <summary>
		/// ch: 线程绑核失败
		/// </summary>
		MVALG_STS_ERR_BIND_CORE_FAILED = 268435500,
		/// <summary>
		/// ch: 感兴趣区域ROI参数错误
		/// </summary>
		MVALG_STS_ERR_ROI_RECT_PARAM = 268435501,
		/// <summary>
		/// ch: 能力集中模块使能错误
		/// </summary>
		MVALG_STS_ERR_ABILITY_MODELU_ERR = 268435502,
		/// <summary>
		/// ch: 感兴趣区域ROI掩膜图像错误
		/// </summary>
		MVALG_STS_ERR_ROI_MASK_IMAGE = 268435503,
		/// <summary>
		/// ch: CPU核数目错误
		/// </summary>
		MVD_ALG_STS_ERR_CPU_CORE_NUM = 268435504,
		/// <summary>
		/// ch: 多线程调度句柄为空
		/// </summary>
		MVD_ALG_STS_ERR_THREAD_POOL_NULL_PTR = 268435505,
		/// <summary>
		/// ch: 数据值无效
		/// </summary>
		MVD_ALG_STS_ERR_DATA_VALUE_INVALID = 268435506,
		/// <summary>
		/// ch: 除0
		/// </summary>
		MVD_ALG_STS_ERR_DIV_ZERO = 268435507,
		/// <summary>
		/// ch: JPEG库错误
		/// </summary>
		MVD_ALG_STS_ERR_JEPG_LIB_ERR = 268435508,
		/// <summary>
		/// ch: PNG库错误
		/// </summary>
		MVD_ALG_STS_ERR_PNG_LIB_ERR = 268435509,
		/// <summary>
		/// ch: TIFF库错误
		/// </summary>
		MVD_ALG_STS_ERR_TIFF_LIB_ERR = 268435510,
		/// <summary>
		///  ch: 矩阵求逆失败
		/// </summary>
		MVD_ALG_STS_ERR_MAT_INVERT_ERR = 268435511,
		/// <summary>
		/// ch: 矩阵最小二乘失败
		/// </summary>
		MVD_ALG_STS_ERR_MAT_LEAST_SQUARE_ERR = 268435512,
		/// <summary>
		/// ch: 矩阵LU分解失败
		/// </summary>
		MVD_ALG_STS_ERR_MAT_LU_ERR = 268435513,
		/// <summary>
		/// ch: 图像组靶标提取获得靶标数量小于最小可标定靶标数量
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_NUM = 268435713,
		/// <summary>
		/// ch: 靶标图像进行特征点提取时，角点提取无误但是靶标特殊标志识别失败（如三角标志）
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_SIGN_DETEC = 268435714,
		/// <summary>
		/// ch: 靶标图像进行特征点提取时，未提取到任何角点/角点数量少于可标定数量
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_FEATURE_FIND = 268435715,
		/// <summary>
		/// ch: 标定板角点检测信息失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_CHERCK_CORENR_INFO_FAIL = 268435716,
		/// <summary>
		/// ch: 标定板角点关系检测失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_CHERCK_CORENR_RELATION_FAIL = 268435717,
		/// <summary>
		/// ch: 标定板角点连通失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_CORENR_GROUP_FAIL = 268435718,
		/// <summary>
		/// ch: 圆标定板定位失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_LOC_FAIL = 268435719,
		/// <summary>
		/// ch: 标定板角点个数太少
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_CORNEO_NUM_ERROR = 268435720,
		/// <summary>
		/// ch: 标定板角点提取失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_CORNEO_EXTRACT_FAIL = 268435721,
		/// <summary>
		/// ch: 标定输入图像个数太少
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_IMG_NUM_ERR = 268435722,
		/// <summary>
		/// ch: 标定板规格不符合要求
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_SPEC_ERR = 268435723,
		/// <summary>
		/// ch: 标定板亚像素角点角点校验失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_SUB_CORENR_CHECK_FAIL = 268435724,
		/// <summary>
		/// ch: 畸变标定失败
		/// </summary>
		MVD_ALG_STS_ERR_DISTORT_CALIB_FAIL = 268435725,
		/// <summary>
		///  ch: 获取两个点集之间的透视变换矩阵失败
		/// </summary>
		MVD_ALG_STS_ERR_GET_PERSPECTIVE_TRANS_MAT_FAIL = 268435726,
		/// <summary>
		/// ch: Homo矩阵转2维变换参数失败
		/// </summary>
		MVD_ALG_STS_ERR_HOMO_TO_TRANS_MAT_FAIL = 268435727,
		/// <summary>
		/// ch: Homo矩阵估计失败
		/// </summary>
		MVD_ALG_STS_ERR_HOMO_ESTIMATE_FAIL = 268435728,
		/// <summary>
		///  ch: Homo转换失败
		/// </summary>
		MVD_ALG_STS_ERR_HOMO_TRANSFORM_FAIL = 268435729,
		/// <summary>
		/// ch: 相机内参估计失败
		/// </summary>
		MVD_ALG_STS_ERR_CAMERA_PARA_ESTIMATE_FAIL = 268435730,
		/// <summary>
		/// ch: 双目相机标定失败
		/// </summary>
		MVD_ALG_STS_ERR_STEREO_CAMERA_PARA_ESTIMATE_FAIL = 268435731,
		/// <summary>
		///  ch: 自研标定板译码失败
		/// </summary>
		MVD_ALG_STS_ERR_CALIB_BOARD_DECODE_FAIL = 268435732,
		/// <summary>
		/// ch: 点点测量失败
		/// </summary>
		MVD_ALG_STS_ERR_PP_MEASURE_FAIL = 268435968,
		/// <summary>
		/// ch: 圆拟合失败
		/// </summary>
		MVD_ALG_STS_ERR_CIRCLE_FIT_FAIL = 268435969,
		/// <summary>
		/// ch: 直线拟合失败
		/// </summary>
		MVD_ALG_STS_ERR_LINE_FIT_FAIL = 268435970,
		/// <summary>
		/// ch: 输入几何基元的位置关系没有交集
		/// </summary>
		MVD_ALG_STS_ERR_NO_INTERSEC = 268435971,
		/// <summary>
		/// ch: 输入几何基元的位置关系重合
		/// </summary>
		MVD_ALG_STS_ERR_COINCIDENCE = 268435972,
		/// <summary>
		/// ch: 3D直线拟合失败
		/// </summary>
		MVD_ALG_STS_ERR_3D_LINE_FIT_FAIL = 268435973,
		/// <summary>
		/// ch: 3D平面拟合失败
		/// </summary>
		MVD_ALG_STS_ERR_3D_PLANE_FIT_FAIL = 268435974,
		/// <summary>
		/// ch: IPP错误
		/// </summary>
		MVD_ALG_STS_ERR_IPP = 271581184,
		/// <summary>
		/// ch: 直线数量过少或过多,无法对位
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_LINE_ERROR = 269524992,
		/// <summary>
		/// ch: 邻边平行
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_LINE_PARALLEL = 269524993,
		/// <summary>
		/// ch: 边不存在,即边的两端点重合
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_LINE_NOEXIST = 269524994,
		/// <summary>
		/// ch: 给定状态和实际对位状态不一致
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_LINE_STATENOFIT = 269524995,
		/// <summary>
		/// ch: 位置补正模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_MODU_POSE_FIXTURE_DISABLE = 269524996,
		/// <summary>
		/// ch: 平台对位模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_MODU_PLATFORM_ALIGN_DISABLE = 269524997,
		/// <summary>
		/// ch: 坐标系模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_MODU_COORDINATE_DISABLE = 269524998,
		/// <summary>
		/// ch: 对位平台类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_PARAM_PLATFORM_TYPE = 269524999,
		/// <summary>
		/// ch: 对位平台移动类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_PARAM_PLATFORM_MOVE_TYPE = 269525000,
		/// <summary>
		/// ch: 旋转中心是否随对位平台移动参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_PARAM_IS_ROTATE_CENTER_MOVE = 269525001,
		/// <summary>
		/// ch: 边对位边异常角度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_PARAM_ALIGNBYLINE_EXCEP_ANGLE = 269525002,
		/// <summary>
		/// ch: 对齐方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_PARAM_ALIGN_MODE = 269525003,
		/// <summary>
		/// ch: 对齐角度参数不在正常范围
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_PARAM_ALIGN_ANGLE = 269525004,
		/// <summary>
		/// ch: 对位算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_ABILITY_ARG_MODU_ENABLE = 269525005,
		/// <summary>
		/// ch: 对位算法能力集最大对齐个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_ABILITY_ARG_MAX_ALIGN_NUM = 269525006,
		/// <summary>
		/// ch: 对位算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_ABILITY_ARG_THREAD_NUM = 269525007,
		/// <summary>
		/// ch: 对位算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBALIGN_ABILITY_ARG_THREAD_HANDLE = 269525008,
		/// <summary>
		/// ch: 矩阵计算异常，通常为数入数据有问题，需检查输入，重新计算
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_HOMO_CAL_WRONG = 269496322,
		/// <summary>
		/// ch: 矩阵异常，原因通常是计算出的矩阵行列式为0),或最后一位为0，需检查输入，重新计算
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_HOMO_WRONG = 269496327,
		/// <summary>
		/// ch: 标定板角点图像到物理左边转换矩阵计算异常（角点不够）
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_HOMO_EST_WRONG = 269496321,
		/// <summary>
		/// ch: 圆拟合失败，请检查输入点
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CIRCLE_FIT_FAILED = 269496328,
		/// <summary>
		/// ch: 圆提取失败，请检查输入图像
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CIRCLR_LOC_FAILED = 269496329,
		/// <summary>
		/// ch: 圆排序失败，请检查输入图像
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CIRCLR_SORT_FAILED = 269496330,
		/// <summary>
		/// ch: 径向畸变计算失败，请检查输入图像
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_RADIAL_CAL_WRONG = 269496331,
		/// <summary>
		/// ch: 角点提取结果异常（行列数过少），需调节参数
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PAT_SIZE_NOT_ENOUGH = 269496320,
		/// <summary>
		/// ch: 标定读码失败，需检查标定图像或调节参数
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_READ_CODE_FAILED = 269496332,
		/// <summary>
		/// ch: 相机标定错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CAMERA_PARA_EST_ERR = 269496333,
		/// <summary>
		/// ch: 图像大小与畸变标定文件不匹配
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_WARP_IMG_NOT_MATCH = 269496323,
		/// <summary>
		/// ch: 角点数目过少
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_FEW_CALIB_POINTS = 269496324,
		/// <summary>
		/// ch: 标定成功的图像过少
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_FEW_CALIB_SUCCEED_IMGS = 269496325,
		/// <summary>
		/// ch: 标定图像中点对过少
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_FEW_CALIB_POINTS_PAIR = 269496326,
		/// <summary>
		/// ch: 手眼类型错误
		/// </summary>
		MVALG_STS_ERR_MVB3D_ROBOT_HEC_HAND_EYE_TYPE = 269496335,
		/// <summary>
		/// ch: 标定板大小错误
		/// </summary>
		MVALG_STS_ERR_MVB3D_ROBOT_HEC_CALIB_BOARD_SIZE = 269496336,
		/// <summary>
		/// ch: 标定板物理尺寸错误
		/// </summary>
		MVALG_STS_ERR_MVB3D_ROBOT_HEC_SQUARE_SIZE = 269496337,
		/// <summary>
		/// ch: 标定图像数量错误
		/// </summary>
		MVALG_STS_ERR_MVB3D_ROBOT_HEC_IMG_NUM = 269496338,
		/// <summary>
		/// ch: 手眼标定方法错误
		/// </summary>
		MVALG_STS_ERR_MVB3D_ROBOT_HEC_CALIB_METHOD = 269496339,
		/// <summary>
		/// ch: Homo估计(N点法相机标定)模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_HOMO_EST_DISABLE = 269496340,
		/// <summary>
		/// ch: Homo transformation转换模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_HOMO_TRANS_DISABLE = 269496341,
		/// <summary>
		/// ch: 自动对位系统标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_ALIGN_CALIB_DISABLE = 269496342,
		/// <summary>
		/// ch: 标定板角点(或圆心)定位模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALIBBOARD_LOC_DISABLE = 269496343,
		/// <summary>
		/// ch: 标定板标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALIBBOARD_CALIB_DISABLE = 269496344,
		/// <summary>
		/// ch: 单幅图畸变矫正模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_DISTORT_CORRECT_DISABLE = 269496345,
		/// <summary>
		/// ch: 单相机内外参标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_SINGLE_CAMERA_CALIB_DISABLE = 269496346,
		/// <summary>
		/// ch: 双相机内外参标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_STEREO_CAMERA_CALIB_DISABLE = 269496347,
		/// <summary>
		/// ch: 手眼标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_HANDEYE_CALIB_DISABLE = 269496348,
		/// <summary>
		/// ch: 自研标定版生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALBOARD_CREATE_DISABLE = 269496349,
		/// <summary>
		/// ch: 标定板位姿检查（用于引导标定）模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALIBBOARD_POSE_CHECK_DISABLE = 269496350,
		/// <summary>
		/// ch: 镜头畸变矫正模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_LENS_DISTORT_CORRECT_DISABLE = 269496351,
		/// <summary>
		/// ch: 映射标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALIBBOARD_MAP_CALIB_DISABLE = 269496352,
		/// <summary>
		/// ch: N图像标定模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALIBBOARD_N_POINT_DISABLE = 269496353,
		/// <summary>
		/// ch: 倾斜度评估模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_INCLINE_ASSESS_DISABLE = 269496354,
		/// <summary>
		/// ch: Homo估计\自动对位系统标定\标定板标定模块\标定板位姿检查（用于引导标定）模块\倾斜度评估模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_HACCI_DISABLE = 269496355,
		/// <summary>
		/// ch: 标定板标定模块和单幅图畸变矫正模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CALIBBOARD_CALIB_DISTORT_CORRECT_DISABLE = 269496356,
		/// <summary>
		/// ch: 标定板标定模块\标定板标定模块\单相机内外参标定模块\双相机内外参标定模块\手眼标定模块\标定板位姿检查（用于引导标定）模块\映射标定模块\N图像标定模块\倾斜度评估模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MODU_CCSSHCCCI_DISABLE = 269496357,
		/// <summary>
		/// ch: Homo估计自由度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_HOMO_EST_FREEDOM = 269496358,
		/// <summary>
		/// ch: 回归函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_HOMO_REGRESSION_FUN = 269496359,
		/// <summary>
		/// ch: 权重函数削波因数系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_HOMO_WFUN_PARAM = 269496360,
		/// <summary>
		/// ch: 判定数据是否适应于模型的距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_HOMO_DIST_TH = 269496361,
		/// <summary>
		/// ch: 样本率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_HOMO_SAMPLE_RATIO = 269496362,
		/// <summary>
		/// ch: Homo变换类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_HOMO_TRANSFORM_TYPE = 269496363,
		/// <summary>
		/// ch: 两相机模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ALIGN_TWO_CAMERA_MODE = 269496364,
		/// <summary>
		/// ch: 阈值处理方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_THRESH = 269496365,
		/// <summary>
		/// ch: 亚像素窗口大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_SUBPIXEL_WIN = 269496366,
		/// <summary>
		/// ch: 定位时标定板基准点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_LOC_FIDUCIAL_MARK = 269496367,
		/// <summary>
		/// ch: 黑白模块灰度对比度最小值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_GRAY_CONTRAST = 269496368,
		/// <summary>
		/// ch: 亚像素窗口大小自适应标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_SUBPIXEL_WIN_AUTO = 269496369,
		/// <summary>
		/// ch: 中值滤波状态参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_FILT_STS = 269496370,
		/// <summary>
		/// ch: 平面弱排序模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_WEAK_SORT_FLAG = 269496371,
		/// <summary>
		/// ch: 亚像素窗口大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_SUBPIXEL_WIN2 = 269496372,
		/// <summary>
		/// ch: 黑白模块灰度对比度最小值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_GRAY_CONTRAST2 = 269496373,
		/// <summary>
		/// ch: 亚像素窗口大小自适应标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_SUBPIXEL_WIN_AUTO2 = 269496374,
		/// <summary>
		/// ch: 中值滤波状态参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_FILT_STS2 = 269496375,
		/// <summary>
		/// ch: 平面弱排序模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_WEAK_SORT_FLAG2 = 269496376,
		/// <summary>
		/// ch: Mark点圆度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CIRCLEBOARD_CIRCULARITY = 269496377,
		/// <summary>
		/// ch: 亚像素边缘提取低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CIRCLEBOARD_EDGE_THRESH_LOW = 269496378,
		/// <summary>
		/// ch: 亚像素边缘提取高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CIRCLEBOARD_EDGE_THRESH_HIGH = 269496379,
		/// <summary>
		/// ch: 圆点标定板圆点类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CIRCLEBOARD_CIRCLE_MODE = 269496380,
		/// <summary>
		/// ch: 标定板校正模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CALIBBOARD_CORRECT_MODEL = 269496381,
		/// <summary>
		/// ch: 标定板透视畸变校正中心x坐标参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CALIBBOARD_PERSPECTIVE_CORRECT_CENTER_X = 269496382,
		/// <summary>
		/// ch: 标定板透视畸变校正中心y坐标参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_CALIBBOARD_PERSPECTIVE_CORRECT_CENTER_Y = 269496383,
		/// <summary>
		/// ch: 轴标定方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ALIGN_AXIS_METHOD = 269496384,
		/// <summary>
		/// ch: 双目参数的优化模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_STEREO_CAMERA_PARA_LM_MODE = 269496385,
		/// <summary>
		/// ch: 手眼类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ROBOT_HEC_HAND_EYE_TYPE = 269496386,
		/// <summary>
		/// ch: 旋转轴参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ROBOT_HEC_ROT_TYPE = 269496387,
		/// <summary>
		/// ch: 手眼标定方程求解方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ROBOT_HEC_CALIB_METHOD = 269496388,
		/// <summary>
		/// ch:  LM算法Lagrange乘子参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ROBOT_HEC_PARAM_SX_LM_LAMBDA = 269496389,
		/// <summary>
		/// ch: LM算法最大迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ROBOT_HEC_PARAM_SX_LM_MAX_ITER_NUM = 269496390,
		/// <summary>
		/// ch: LM算法精度要求参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_ROBOT_HEC_PARAM_SX_LM_EPS = 269496391,
		/// <summary>
		/// ch: 畸变系数个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_PARAM_SINGLE_CAMERA_CALIB_DIST_NUM = 269496392,
		/// <summary>
		/// ch: 误差检查正常
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ERR_CHECK_NORMAL = 269496393,
		/// <summary>
		/// ch: 平移估计最大误差过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_TRANS_ERR_MAX_BEYOND = 269496394,
		/// <summary>
		/// ch: 平移估计平均误差过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_TRANS_ERR_AVG_BEYOND = 269496395,
		/// <summary>
		/// ch: 旋转估计最大误差过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ROT_ERR_MAX_BEYOND = 269496396,
		/// <summary>
		/// ch: 旋转误差平均误差过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ROT_ERR_AVG_BEYOND = 269496397,
		/// <summary>
		/// ch: 机构最大平移误差率过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_TRANS_MOT_ERR_MAX_BEYOND = 269496398,
		/// <summary>
		/// ch: 机构最大旋转误差率过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ROT_MOT_ERR_MAX_BEYOND = 269496399,
		/// <summary>
		/// ch: 映射标定误差过大
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_IMG_MAP_ERR_MAX_BEYOND = 269496400,
		/// <summary>
		/// ch: 输入点集全部共线——减少输入点(不超过3个点)，或者检查点集数据
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_POINTS_COLLINEATION = 269496401,
		/// <summary>
		/// ch: 创建标定板模块生成物理点失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CALBOARD_CREATE_WORLD_POINT_FAIL = 269496402,
		/// <summary>
		/// ch: 标定算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_MODU_ENABLE = 269496403,
		/// <summary>
		/// ch: 标定算法能力集图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_MAX_IMAGE_SIZE = 269496404,
		/// <summary>
		/// ch: 标定算法能力集最大标定点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_MAX_POINT_NUM = 269496405,
		/// <summary>
		/// ch: 标定算法能力集最大图像个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_MAX_IMAGE_NUM = 269496406,
		/// <summary>
		/// ch: 标定算法能力集最大标定板宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_MAX_PAT_SIZE = 269496407,
		/// <summary>
		/// ch: 标定算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_THREAD_NUM = 269496408,
		/// <summary>
		/// ch: 标定算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_ABILITY_ARG_THREAD_HANDLE = 269496409,
		/// <summary>
		/// ch: 标定算法矩阵求逆失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_MAT_INVERT_ERR = 269496410,
		/// <summary>
		/// ch: 双目标定输入图像个数太少
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_STEREO_CALIB_PARA_IMG_NUM_ERR = 269496411,
		/// <summary>
		/// ch: 标定板角点检测信息失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CALIB_BOARD_CHERCK_CORENR_INFO_FAIL = 269496412,
		/// <summary>
		/// ch: 标定板角点关系检测失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CALIB_BOARD_CHERCK_CORENR_RELATION_FAIL = 269496413,
		/// <summary>
		/// ch: 标定板角点连通失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CALIB_BOARD_CORENR_GROUP_FAIL = 269496414,
		/// <summary>
		/// ch: 标定板亚像素角点角点校验失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CALIB_BOARD_SUB_CORENR_CHECK_FAIL = 269496415,
		/// <summary>
		/// ch: 圆标定板定位失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CIRCLE_CALIB_BOARD_LOC_FAIL = 269496416,
		/// <summary>
		/// ch: 自研标定板译码失败
		/// </summary>
		MVALG_STS_ERR_MVBCALIB_CALIB_BOARD_DECODE_FAIL = 269496417,
		/// <summary>
		///  ch:棋盘格角点检测方法选择错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_LOC_METHOD_TYPE = 269496418,
		/// <summary>
		/// ch:角点梯度主方向夹角低阈值
		/// </summary>
		MVD_ALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_CORNER_ANGLE_LOW = 269496419,
		/// <summary>
		/// ch:角点梯度主方向夹角高阈值
		/// </summary>
		MVD_ALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_CORNER_ANGLE_HIGH = 269496420,
		/// <summary>
		/// ch:单目相机标定模型类型选择错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCALIB_PARAM_SINGLE_CAMERA_CALIB_DIST_MODEL_TYPE = 269496421,
		/// <summary>
		/// ch:单目相机标定模型类型选择错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCALIB_PARAM_CHECKERBOARD_CORNER_SAMPLE_NUM = 269496422,
		/// <summary>
		/// ch: 颜色模型不存在
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_NO_FOUND_MODEL = 269512704,
		/// <summary>
		/// ch: 当前模型数量无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODEL_NUM_INVALID = 269512705,
		/// <summary>
		/// ch: 当前二值图像索引无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_BINARY_INDEX_INVALID = 269512706,
		/// <summary>
		/// ch: 颜色模型之间发生混淆
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_COLOR_MODEL_CONFUSE = 269512707,
		/// <summary>
		/// ch: 颜色模型区分不显著
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_COLOR_MODEL_MIX = 269512708,
		/// <summary>
		/// ch: Hue通道鲁棒性参数输入有误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_HUE_SCALE_ERR = 269512709,
		/// <summary>
		/// ch: 饱和度通道鲁棒性参数输入有误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_SATURATION_SCALE_ERR = 269512710,
		/// <summary>
		/// ch: 亮度通道鲁棒性参数输入有误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_INTENSITY_SCALE_ERR = 269512711,
		/// <summary>
		/// ch: 颜色库无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_COLOR_FILE_INVALID = 269512712,
		/// <summary>
		/// ch: 颜色库有效，但无图像数据（训练时未存图） ch: 不再使用
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_NO_IMG_DATA_IN_MODEL = 269512713,
		/// <summary>
		/// ch: 颜色识别K值过大，建议减小K值或增大样本数
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_RECOGNITION_K_ERROR = 269512714,
		/// <summary>
		/// ch: 模型版本正确，但内部指针错误，说明内存数据被更改
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODEL_DATA_PTR_ERROR = 269512715,
		/// <summary>
		/// ch: 颜色库有效，但基本数据错误（被更改，需重新训练）
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODEL_ARG_ERROR = 269512716,
		/// <summary>
		/// ch: 颜色空间转换模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_SPACE_CONVERT_DISABLE = 269512717,
		/// <summary>
		/// ch: 颜色抽取模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_EXTRACT_DISABLE = 269512718,
		/// <summary>
		/// ch: 颜色多范围抽取模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_MULTI_EXTRACT_DISABLE = 269512719,
		/// <summary>
		/// ch: 颜色测量模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_MEASURE_DISABLE = 269512720,
		/// <summary>
		/// ch: 颜色模型创建模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_SEGMENT_MODEL_CREATE_DISABLE = 269512721,
		/// <summary>
		/// ch: 颜色分割模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_SEGMENT_DISABLE = 269512722,
		/// <summary>
		/// ch: 颜色分割分析模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_SEGMENT_ANALYSIS_DISABLE = 269512723,
		/// <summary>
		/// ch: 颜色识别模型创建模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_RECOGNITION_MODEL_CREATE_DISABLE = 269512724,
		/// <summary>
		/// ch: 颜色识别模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_RECOGNITION_DISABLE = 269512725,
		/// <summary>
		/// ch: 颜色空间类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_SPACE_TYPE = 269512726,
		/// <summary>
		/// ch: 颜色空间转换类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_SPACE_CONVERT_TYPE = 269512727,
		/// <summary>
		/// ch: RGB转灰度类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_TYPE = 269512728,
		/// <summary>
		/// ch: RGB转灰度，R自设转换比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_R_RATIO = 269512729,
		/// <summary>
		/// ch: RGB转灰度，G自设转换比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_G_RATIO = 269512730,
		/// <summary>
		/// ch: RGB转灰度，B自设转换比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_B_RATIO = 269512731,
		/// <summary>
		/// ch: 第一通道(R or H)的低容忍值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_C1_LOW_TOLERANCE = 269512732,
		/// <summary>
		/// ch: 第一通道(R or H)的高容忍值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_C1_HIGH_TOLERANCE = 269512733,
		/// <summary>
		/// ch: 第二通道(R or H)的低容忍值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_C2_LOW_TOLERANCE = 269512734,
		/// <summary>
		/// ch: 第二通道(R or H)的高容忍值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_C2_HIGH_TOLERANCE = 269512735,
		/// <summary>
		/// ch: 第三通道(R or H)的低容忍值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_C3_LOW_TOLERANCE = 269512736,
		/// <summary>
		/// ch: 第三通道(R or H)的高容忍值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_C3_HIGH_TOLERANCE = 269512737,
		/// <summary>
		/// ch: 颜色直方图bin数目参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_MEASURE_HIST_BIN_NUM = 269512738,
		/// <summary>
		/// ch: 滤波内核大小, 颜色模型创建与分割共用参数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_KERNEL_SIZE = 269512739,
		/// <summary>
		/// ch: 颜色显著性阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_SALIENCY_THRESH = 269512740,
		/// <summary>
		/// ch: 颜色饱和度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_SATURATION_THRESH = 269512741,
		/// <summary>
		/// ch: 颜色亮度低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_INTENSITY_LOW_THRESH = 269512742,
		/// <summary>
		/// ch: 颜色亮度高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_INTENSITY_HIGH_THRESH = 269512743,
		/// <summary>
		/// ch: 颜色分割输出类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_SEGMENT_OUTPUT_TYPE = 269512744,
		/// <summary>
		/// ch: 最小像素计数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_MINIMUM_PIXEL_COUNT = 269512745,
		/// <summary>
		/// ch: 颜色特征类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_COLOR_FEATURE_TYPE = 269512746,
		/// <summary>
		/// ch: 敏感度模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_SENSITIVITY_MODE = 269512747,
		/// <summary>
		/// ch: 饱和度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_SATURATION_THRESH = 269512748,
		/// <summary>
		/// ch: 暗灰阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_BLACK_GRAY_THRESH = 269512749,
		/// <summary>
		/// ch: 亮灰阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_WHITE_GRAY_THRESH = 269512750,
		/// <summary>
		/// ch: 亮度使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_INTENSITY_ENABLE = 269512751,
		/// <summary>
		/// ch: KNN分类器的k值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_K = 269512752,
		/// <summary>
		/// ch: KNN分类器的距离类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_DISTANCE_TYPE = 269512753,
		/// <summary>
		/// ch: 颜色测量/颜色抽取模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_EXTRACT_MEASURE_DISABLE = 269512754,
		/// <summary>
		/// ch: 颜色分割/颜色模型创建模块模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_SEGMENT_SEGMENT_MODEL_CREATE_DISABLE = 269512755,
		/// <summary>
		/// ch: 颜色识别模型创建模块/颜色识别模块模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_RECOGNITION_MODEL_CREATE_RECOGNITION_DISABLE = 269512756,
		/// <summary>
		/// ch: 颜色抽取/颜色多范围抽取/颜色测量模块无效
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MODU_COLOR_EXTRACT_MULTI_EXTRACT_MEASURE_DISABLE = 269512757,
		/// <summary>
		/// ch: 颜色抽取获取中心方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_CENTER_METHOD = 269512758,
		/// <summary>
		/// ch: 颜色抽取最小间隔参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_FEATURE_INTERVAL_MIN = 269512759,
		/// <summary>
		/// ch: 颜色抽取特征阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_FEATURE_THRESH_VALUE = 269512760,
		/// <summary>
		/// ch: 通道未使能
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RANGE_UNENABLE = 269512761,
		/// <summary>
		/// ch: C1通道低值大于高值
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RANGE_C1_LOW_HIGH_TOLERANCE = 269512762,
		/// <summary>
		/// ch: C2通道低值大于高值
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RANGE_C2_LOW_HIGH_TOLERANCE = 269512763,
		/// <summary>
		/// ch: C3通道低值大于高值
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RANGE_C3_LOW_HIGH_TOLERANCE = 269512764,
		/// <summary>
		/// ch: 模型内存大小不足
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_COLOR_MODEL_MEM_ERR = 269512765,
		/// <summary>
		/// ch: 颜色处理算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_ABILITY_ARG_MODU_ENABLE = 269512766,
		/// <summary>
		/// ch: 颜色处理算法能力集最大图像大小错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_ABILITY_ARG_MAX_IMAGE_SIZE = 269512767,
		/// <summary>
		/// ch: 颜色处理算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_ABILITY_ARG_THREAD_NUM = 269512768,
		/// <summary>
		/// ch: 颜色处理算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_ABILITY_ARG_THREAD_HANDLE = 269512769,
		/// <summary>
		/// ch: RGB转灰度，R自设转换比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_R_RATIO_NS = 269512770,
		/// <summary>
		/// ch: RGB转灰度，G自设转换比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_G_RATIO_NS = 269512771,
		/// <summary>
		/// ch: RGB转灰度，B自设转换比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_RGB_CONVERT_GRAY_B_RATIO_NS = 269512772,
		/// <summary>
		/// ch: 输出组编号设置错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_GROUP_IDX_OUTPUT = 269512773,
		/// <summary>
		/// ch: 孔洞面积阈值设置错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_GROUP_MAX_HOLE_AREA = 269512774,
		/// <summary>
		/// ch: 颜色范围可视化仅支持HSI和HSV颜色空间
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_MEASURE_VISTUALIZE_BACKGROUND_COLOR_SPACE = 269512775,
		/// <summary>
		/// ch: 杂斑面积阈值设置错误
		/// </summary>
		MVALG_STS_ERR_MVBCOLOR_PARAM_EXTRACT_GROUP_MAX_CLUTTER_AREA = 269512776,
		/// <summary>
		/// ch: 卡尺（边缘查找）模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_EDGE_FINDER = 269488128,
		/// <summary>
		/// ch:卡尺（边缘对查找）模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_EDGE_PAIR_FINDER = 269488129,
		/// <summary>
		/// ch:直线查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_LINE_FINDER = 269488130,
		/// <summary>
		/// ch:平行线检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_PAIR_LINE_FINDER = 269488131,
		/// <summary>
		/// ch:多平行线检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_PARALLEL_FINDER = 269488132,
		/// <summary>
		/// ch:多线查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_MULTILINE_FINDER = 269488133,
		/// <summary>
		/// ch:顶点检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_PEAK_FINDER = 269488134,
		/// <summary>
		/// ch:边缘交点检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_CORNER_FINDER = 269488135,
		/// <summary>
		/// ch:矩形检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_RECTANGLE_FINDER = 269488136,
		/// <summary>
		/// ch:圆查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_CIRCLE_FINDER = 269488137,
		/// <summary>
		/// ch:椭圆查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_ELLIPSE_FINDER = 269488138,
		/// <summary>
		/// ch:blob分析模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_BLOB_FINDER = 269488139,
		/// <summary>
		/// ch:亮度有无模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_LUM_CHECK = 269488140,
		/// <summary>
		/// ch:四边形查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_QUADRANGLE_FINDER = 269488141,
		/// <summary>
		/// ch:圆查找V2模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_CIRCLE_FINDER_V2 = 269488142,
		/// <summary>
		/// ch:mark点定位模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_MARK_FIND = 269488143,
		/// <summary>
		/// ch:区域投影模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_REGION_PROJECTION = 269488144,
		/// <summary>
		/// ch: 圆最小半径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_MIN_RADIUS = 269488145,
		/// <summary>
		/// ch: 圆最大半径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_MAX_RADIUS = 269488146,
		/// <summary>
		/// ch: 圆边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_EDGE_THRESHOLD = 269488147,
		/// <summary>
		/// ch: 圆边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_EDGE_POLARITY = 269488148,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_FIND_MODE = 269488149,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_CALIPER_NUM = 269488150,
		/// <summary>
		/// ch: 圆环起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_START_ANGLE = 269488151,
		/// <summary>
		/// ch: 圆环角度范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_ANGLE_EXTEND = 269488152,
		/// <summary>
		/// ch: 圆下采样系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_CCD_SAMPLE_SCALE = 269488153,
		/// <summary>
		/// ch: 圆定位敏感度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_CCD_CIRCLE_THRESH = 269488154,
		/// <summary>
		/// ch: 圆滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_HALF_KERNEL_SIZE = 269488155,
		/// <summary>
		/// ch: 圆卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_PROJECTION_LENGTH = 269488156,
		/// <summary>
		/// ch: 圆拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_FIT_FUN = 269488157,
		/// <summary>
		/// ch: 圆初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_FIT_INIT_TYPE = 269488158,
		/// <summary>
		/// ch: 圆忽略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_FIT_REJECT_NUM = 269488159,
		/// <summary>
		/// ch: 圆剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_FIT_REJECT_DIST = 269488160,
		/// <summary>
		/// ch: 圆查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_FIND_MODE = 269488161,
		/// <summary>
		/// ch: 圆卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_CALIPER_NUM = 269488162,
		/// <summary>
		/// ch: 圆卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_PROJECTION_LENGTH = 269488163,
		/// <summary>
		/// ch: 圆边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_EDGE_POLARITY = 269488164,
		/// <summary>
		/// ch: 圆边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_EDGE_THRESHOLD = 269488165,
		/// <summary>
		/// ch: 圆滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_HALF_KERNEL_SIZE = 269488166,
		/// <summary>
		/// ch: 圆拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_FIT_FUN = 269488167,
		/// <summary>
		/// ch: 圆初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_FIT_INIT_TYPE = 269488168,
		/// <summary>
		/// ch: 圆忽略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_FIT_REJECT_NUM = 269488169,
		/// <summary>
		/// ch: 圆剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_FIT_FIT_REJECT_DIST = 269488170,
		/// <summary>
		/// ch: 直线查找边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_EDGE_POLARITY = 269488171,
		/// <summary>
		/// ch: 直线查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_FIND_MODE = 269488172,
		/// <summary>
		/// ch: 直线查找搜索方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_FIND_ORIENT = 269488173,
		/// <summary>
		/// ch: 直线查找边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_EDGE_THRESHOLD = 269488174,
		/// <summary>
		/// ch: 直线查找卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_CALIPER_NUM = 269488175,
		/// <summary>
		/// ch: 直线查找卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_REGION_WIDTH = 269488176,
		/// <summary>
		/// ch: 直线查找剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_REJECT_DIST = 269488177,
		/// <summary>
		/// ch: 直线查找滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_HALF_KERNEL_SIZE = 269488178,
		/// <summary>
		/// ch: 直线查找拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_FIT_FUN = 269488179,
		/// <summary>
		/// ch: 直线查找初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_FIT_INIT_TYPE = 269488180,
		/// <summary>
		/// ch: 直线查找略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_REJECT_NUM = 269488181,
		/// <summary>
		/// ch: 圆环查找未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_CIRCLR_ANN_ROI = 269488182,
		/// <summary>
		/// ch: 直线box查找未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_LINE_BOX_ROI = 269488183,
		/// <summary>
		/// ch: 多类别Blob查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MUDO_MULTI_CLS_BLOB_FINDER = 269488184,
		/// <summary>
		/// ch: 获取BOXROI未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MUDO_BOX_ROI = 269488185,
		/// <summary>
		/// ch: 获取圆环型ROI未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MUDO_ANN_ROI = 269488186,
		/// <summary>
		/// ch: 搜索用户输入最近点位置未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MUDO_NEARST_POINT = 269488187,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_EDGE0_POLARITY = 269488188,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_EDGE1_POLARITY = 269488189,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_HALF_KERNEL_SIZE = 269488190,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_EDGE_THRESHOLD = 269488191,
		/// <summary>
		/// ch: 边缘对宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_EDGE_PAIR_WIDTH = 269488192,
		/// <summary>
		/// ch: 最大结果数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_MAXIMUM = 269488193,
		/// <summary>
		/// ch: 投影类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_PROJECTION_TYPE = 269488194,
		/// <summary>
		/// ch: 计分类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_SCORING_TYPE = 269488195,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_CONTRAST_DROP = 269488196,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_CONTRAST_X0 = 269488197,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_CONTRAST_X1 = 269488198,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_CONTRAST_XC = 269488199,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_CONTRAST_Y0 = 269488200,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_CONTRAST_Y1 = 269488201,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_DROP = 269488202,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_X0 = 269488203,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_X1 = 269488204,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_XC = 269488205,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_Y0 = 269488206,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_Y1 = 269488207,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NEG_DROP = 269488208,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NEG_X0 = 269488209,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NEG_X1 = 269488210,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NEG_XC = 269488211,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NEG_Y0 = 269488212,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NEG_Y1 = 269488213,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_DROP = 269488214,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_X0 = 269488215,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_X1 = 269488216,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_XC = 269488217,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_Y0 = 269488218,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_Y1 = 269488219,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_NEG_DROP = 269488220,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_NEG_X0 = 269488221,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_NEG_X1 = 269488222,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_NEG_XC = 269488223,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_NEG_Y0 = 269488224,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_POSITION_NORM_NEG_Y1 = 269488225,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_NORM_DROP = 269488226,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_NORM_X0 = 269488227,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_NORM_X1 = 269488228,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_NORM_XC = 269488229,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_NORM_Y0 = 269488230,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_NORM_Y1 = 269488231,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_DROP = 269488232,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_X0 = 269488233,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_X1 = 269488234,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_XC = 269488235,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_Y0 = 269488236,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_Y1 = 269488237,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_DROP = 269488238,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_X0 = 269488239,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_X1 = 269488240,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_XC = 269488241,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_Y0 = 269488242,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_Y1 = 269488243,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_DROPH = 269488244,
		/// <summary>
		/// ch: 起点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_X0H = 269488245,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_X1H = 269488246,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_XCH = 269488247,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_Y0H = 269488248,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_SIZE_DIFF_NORM_ASYM_Y1H = 269488249,
		/// <summary>
		/// ch: 曲线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_GRAY_DROP = 269488250,
		/// <summary>
		/// ch: 起点
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_GRAY_X0 = 269488251,
		/// <summary>
		/// ch: 中点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_GRAY_X1 = 269488252,
		/// <summary>
		/// ch: 终点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_GRAY_XC = 269488253,
		/// <summary>
		/// ch: 最低评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_GRAY_Y0 = 269488254,
		/// <summary>
		/// ch: 最高评分参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_S_GRAY_Y1 = 269488255,
		/// <summary>
		/// ch: 模糊边缘标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIND_FUZZY_FLAG = 269488256,
		/// <summary>
		/// ch: 输出的排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_SORT_TYPE = 269488257,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_HALF_KERNEL_SIZE = 269488258,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_THRESHOLD = 269488259,
		/// <summary>
		/// ch: 边线极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_POLARITY = 269488260,
		/// <summary>
		/// ch: 查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_FIND_ORIENT = 269488261,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_FIND_MODE = 269488262,
		/// <summary>
		/// ch: 最大结果数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_MAXIMUM = 269488263,
		/// <summary>
		/// ch: 最小分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_MIN_SCORE = 269488264,
		/// <summary>
		/// ch: 输出的排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_SORT_TYPE = 269488265,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_HALF_KERNEL_SIZE = 269488266,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_EDGE_THRESHOLD = 269488267,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_EDGE0_POLARITY = 269488268,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_EDGE1_POLARITY = 269488269,
		/// <summary>
		/// ch: 查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_FIND_ORIENT = 269488270,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_FIND_MODE = 269488271,
		/// <summary>
		/// ch: 最大结果数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_MAXIMUM = 269488272,
		/// <summary>
		/// ch: 理想边缘宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_IDEA_WIDTH = 269488273,
		/// <summary>
		/// ch: 最小分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_MIN_SCORE = 269488274,
		/// <summary>
		/// ch: 输出的排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_SORT_TYPE = 269488275,
		/// <summary>
		/// ch: 点边距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_WIDTH_P2BOX_DIST = 269488276,
		/// <summary>
		/// ch: 点边距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_V2_FIND_P2BOX_DIST = 269488277,
		/// <summary>
		/// ch: 边界提取低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_DETECT_LOW_THRESH = 269488278,
		/// <summary>
		/// ch: 边界提取高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_DETECT_HIGH_THRESH = 269488279,
		/// <summary>
		/// ch: 输入点容忍距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_DISTANCE_TOLERANCE = 269488280,
		/// <summary>
		/// ch: 边界查找点数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_EDGE_COLLECT_POINT_NUM = 269488281,
		/// <summary>
		/// ch: 点周围邻域宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_POINT_ROI_WIDTH = 269488282,
		/// <summary>
		/// ch: 点周围邻域高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_POINT_ROI_HEIGHT = 269488283,
		/// <summary>
		/// ch: 最小拟合点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MIN_POINT_FIT_NUM = 269488284,
		/// <summary>
		/// ch: 最大拟合点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MAX_POINT_FIT_NUM = 269488285,
		/// <summary>
		/// ch: 圆查找中拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_INIT_TYPE = 269488286,
		/// <summary>
		/// ch: 圆查找中拟合权重函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_WEIGHT_FUN = 269488287,
		/// <summary>
		/// ch: 圆查找中拟合最大迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_MAX_ITERS = 269488288,
		/// <summary>
		/// ch: 圆查找中拟合剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_REJECT_NUM = 269488289,
		/// <summary>
		/// ch: 圆查找中拟合剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_REJECT_DIST = 269488290,
		/// <summary>
		/// ch: 圆查找中拟合期望半径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_EXPECT_RADIUS = 269488291,
		/// <summary>
		/// ch: 圆查找中拟合半径容忍参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIR_FIND_FIT_RADIUS_TOLERANCE = 269488292,
		/// <summary>
		/// ch: 点边距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LINE_FIND_P2BOX_DIST = 269488293,
		/// <summary>
		/// ch: 直线类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_LINE_TYPE = 269488294,
		/// <summary>
		/// ch: 高级参数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIND_ORIENT = 269488295,
		/// <summary>
		/// ch: 边缘极性0参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_EDGE_POLARITY = 269488296,
		/// <summary>
		/// ch: 边缘强度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_EDGE_STRENGTH = 269488297,
		/// <summary>
		/// ch: 滤波半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_KERNER_SIZE = 269488298,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_COUNT = 269488299,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_PROJECTION_LENGTH = 269488300,
		/// <summary>
		/// ch: 拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIT_INIT_TYPE = 269488301,
		/// <summary>
		/// ch: 拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIT_FUN = 269488302,
		/// <summary>
		/// ch: 最大迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIT_MAX_ITER = 269488303,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIT_REJECT_NUM = 269488304,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CALIPER_FIT_REJECT_DIST = 269488305,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_EDGE_TYPE = 269488306,
		/// <summary>
		/// ch: 边缘查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_EDGE_ORIENT = 269488307,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_EDGE0_POLARITY = 269488308,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_EDGE1_POLARITY = 269488309,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_EDGE_THRESHOLD = 269488310,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_HALF_KERNEL_SIZE = 269488311,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_CALIPER_NUM = 269488312,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_PROJECTION_LENGTH = 269488313,
		/// <summary>
		/// ch: 理想直线对宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_IDEA_WIDTH = 269488314,
		/// <summary>
		/// ch: 拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_FIT_INIT_TYPE = 269488315,
		/// <summary>
		/// ch: 拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_FIT_FUN = 269488316,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_FIT_REJECT_NUM = 269488317,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_FIT_REJECT_DIST = 269488318,
		/// <summary>
		/// ch: 平行线角度容忍参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PAIR_LINE_DETECT_ANGLE_TOL = 269488319,
		/// <summary>
		/// ch: 查找类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_FIND_MODE = 269488320,
		/// <summary>
		/// ch: 边缘查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_EDGE_ORIENT = 269488321,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_EDGE_POLARITY = 269488322,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_EDGE_THRESHOLD = 269488323,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_HALF_KERNEL_SIZE = 269488324,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_CALIPER_NUM = 269488325,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_PROJECTION_LENGTH = 269488326,
		/// <summary>
		/// ch: 理想直线间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_IDEA_WIDTH = 269488327,
		/// <summary>
		/// ch: 拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_FIT_INIT_TYPE = 269488328,
		/// <summary>
		/// ch: 拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_FIT_FUN = 269488329,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_FIT_REJECT_DIST = 269488330,
		/// <summary>
		/// ch: 平行线最大条数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_MAXIMUM = 269488331,
		/// <summary>
		/// ch: 指定序列号参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_SEQ_NUM = 269488332,
		/// <summary>
		/// ch: 排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PARALLEL_DETECT_SORT_TYPE = 269488333,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_GRADIENT_FIELD_SIZE = 269488334,
		/// <summary>
		/// ch: 投影长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_PROJECTION_LENGTH = 269488335,
		/// <summary>
		/// ch: 绝对边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_EDGE_THRESHOLD = 269488336,
		/// <summary>
		/// ch: 相对边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_NORMAL_CONTRAST_TH = 269488337,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_EDGE_POLARITY = 269488338,
		/// <summary>
		/// ch: 边缘角度容忍参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_EDGE_ANGLE_TOLERANCE = 269488339,
		/// <summary>
		/// ch: 边缘距离容忍参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_EDGE_DIST_TOLERANCE = 269488340,
		/// <summary>
		/// ch: 最大直线数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_MAX_NUM = 269488341,
		/// <summary>
		/// ch: 覆盖率阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_COVERAGE_TH = 269488342,
		/// <summary>
		/// ch: 直线旋转容忍参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_ROTATION_TOLERANCE = 269488343,
		/// <summary>
		/// ch: 拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_FIT_FUN = 269488344,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PEAK_DETECT_EDGE_POLARITY = 269488345,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PEAK_DETECT_EDGE_THRESHOLD = 269488346,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PEAK_DETECT_HALF_KERNEL_SIZE = 269488347,
		/// <summary>
		/// ch: 扫描宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PEAK_DETECT_SCAN_WIDTH = 269488348,
		/// <summary>
		/// ch: 查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PEAK_DETECT_FIND_ORIENT = 269488349,
		/// <summary>
		/// ch: 边缘0类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE0_TYPE = 269488350,
		/// <summary>
		/// ch: 边缘0查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE0_ORIENT = 269488351,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE0_POLARITY = 269488352,
		/// <summary>
		/// ch: 边缘1类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE1_TYPE = 269488353,
		/// <summary>
		/// ch: 边缘1查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE1_ORIENT = 269488354,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE1_POLARITY = 269488355,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_EDGE_THRESHOLD = 269488356,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_HALF_KERNEL_SIZE = 269488357,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_CALIPER_NUM = 269488358,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_PROJECTION_LENGTH = 269488359,
		/// <summary>
		/// ch: 拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_FIT_INIT_TYPE = 269488360,
		/// <summary>
		/// ch: 拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_FIT_FUN = 269488361,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_FIT_REJECT_NUM = 269488362,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CORNER_DETECT_FIT_REJECT_DIST = 269488363,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_EDGE_TYPE = 269488364,
		/// <summary>
		/// ch: 上边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_EDGE_UP_POLARITY = 269488365,
		/// <summary>
		/// ch: 下边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_EDGE_DOWN_POLARITY = 269488366,
		/// <summary>
		/// ch: 左边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_EDGE_LEFT_POLARITY = 269488367,
		/// <summary>
		/// ch: 右边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_EDGE_RIGHT_POLARITY = 269488368,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_EDGE_THRESHOLD = 269488369,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_HALF_KERNEL_SIZE = 269488370,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_CALIPER_NUM = 269488371,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_PROJECTION_LENGTH = 269488372,
		/// <summary>
		/// ch: 理想矩形宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_IDEA_WIDTH = 269488373,
		/// <summary>
		/// ch: 理想矩形高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_IDEA_HEIGHT = 269488374,
		/// <summary>
		/// ch: 拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_FIT_INIT_TYPE = 269488375,
		/// <summary>
		/// ch: 拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_FIT_FUN = 269488376,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_FIT_REJECT_NUM = 269488377,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_RECT_DETECT_FIT_REJECT_DIST = 269488378,
		/// <summary>
		/// ch: 阈值方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_TYPE = 269488379,
		/// <summary>
		/// ch: 二值化极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_POLARITY = 269488380,
		/// <summary>
		/// ch: 低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_LOW_TH = 269488381,
		/// <summary>
		/// ch: 高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_HIGH_TH = 269488382,
		/// <summary>
		/// ch: 柔和度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_SOFTNESS = 269488383,
		/// <summary>
		/// ch: 固定软阈值的低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_SOFT_LOW_TH = 269488384,
		/// <summary>
		/// ch: 固定软阈值的高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_SOFT_HIGH_TH = 269488385,
		/// <summary>
		/// ch: 低尾部参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_LEFT_RATIO = 269488386,
		/// <summary>
		/// ch: 高尾部参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_RIGHT_RATIO = 269488387,
		/// <summary>
		/// ch: 低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_LOW_TH_RATIO = 269488388,
		/// <summary>
		/// ch: 高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BIN_HIGHT_TH_RATIO = 269488389,
		/// <summary>
		/// ch: 连通类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_CONNECTIVITY = 269488390,
		/// <summary>
		/// ch: 孔洞最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_HOLE_MIN_AREA = 269488391,
		/// <summary>
		/// ch: blob最大个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_SELECT_BLOB_MAXIMUM = 269488392,
		/// <summary>
		/// ch: 排序特征参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_SORT_FEATURE = 269488393,
		/// <summary>
		/// ch: 排序方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_SORT_MODE = 269488394,
		/// <summary>
		/// ch: 特征使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_SELECT_ENABLE = 269488395,
		/// <summary>
		/// ch: 面积下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_AREA = 269488396,
		/// <summary>
		/// ch: 面积上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_AREA = 269488397,
		/// <summary>
		/// ch: 面积权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_AREA_WEIGHT = 269488398,
		/// <summary>
		/// ch: 周长下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_PERIMETER = 269488399,
		/// <summary>
		/// ch: 周长上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_PERIMETER = 269488400,
		/// <summary>
		/// ch: 周长权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_PERIMETER_WEIGHT = 269488401,
		/// <summary>
		/// ch: 短轴长度下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_SHORT_AXIS = 269488402,
		/// <summary>
		/// ch: 短轴长度上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_SHORT_AXIS = 269488403,
		/// <summary>
		/// ch: 短轴长度权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_SHORT_AXIS_WEIGHT = 269488404,
		/// <summary>
		/// ch: 长轴长度下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_LONG_AXIS = 269488405,
		/// <summary>
		/// ch: 长轴长度上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_LONG_AXIS = 269488406,
		/// <summary>
		/// ch: 长轴长度权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_LONG_AXIS_WEIGHT = 269488407,
		/// <summary>
		/// ch: 最小轴比参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_AXIS_RATIO = 269488408,
		/// <summary>
		/// ch: 最大轴比参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_AXIS_RATIO = 269488409,
		/// <summary>
		/// ch: 轴比率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_AXIS_RATIO_WEIGHT = 269488410,
		/// <summary>
		/// ch: 圆形度下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_CIRCULARITY = 269488411,
		/// <summary>
		/// ch: 圆形度上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_CIRCULARITY = 269488412,
		/// <summary>
		/// ch: 圆形度权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_CIRCULARITY_WEIGHT = 269488413,
		/// <summary>
		/// ch: 矩形度下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_RECTANGULARITY = 269488414,
		/// <summary>
		/// ch: 矩形度上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_RECTANGULARITY = 269488415,
		/// <summary>
		/// ch: 矩形度权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_RECTANGULARITY_WEIGHT = 269488416,
		/// <summary>
		/// ch: 质心相对于矩形中心的偏移值下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_CENTRA_BIAS = 269488417,
		/// <summary>
		/// ch: 质心相对于矩形中心的偏移值上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_CENTRA_BIAS = 269488418,
		/// <summary>
		/// ch: 质心相对于矩形中心的偏移值权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_CENTRA_BIAS_WEIGHT = 269488419,
		/// <summary>
		/// ch: 最小重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MIN_OVERLAP_RATIO = 269488420,
		/// <summary>
		/// ch: 最小亮度值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LUM_CHECK_MIN_LUM = 269488421,
		/// <summary>
		/// ch: 最大亮度值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LUM_CHECK_MAX_LUM = 269488422,
		/// <summary>
		/// ch: 最小像素个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_LUM_CHECK_MIN_AREA = 269488423,
		/// <summary>
		/// ch: 查找最小半径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_MIN_RADIUS = 269488424,
		/// <summary>
		/// ch: 查找最大半径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_MAX_RADIUS = 269488425,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_EDGE_POLARITY = 269488426,
		/// <summary>
		/// ch: 椭圆类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_FIND_MODE = 269488427,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_EDGE_THRESHOLD = 269488428,
		/// <summary>
		/// ch: 射线数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_RAY_NUM = 269488429,
		/// <summary>
		/// ch: 边缘宽度
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_EDGE_WIDTH = 269488430,
		/// <summary>
		/// ch: 投影宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_REGION_WIDTH = 269488431,
		/// <summary>
		/// ch: 椭圆环起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_ANGLE_START = 269488432,
		/// <summary>
		/// ch: 椭圆环角度范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_ANGLE_EXTEND = 269488433,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_EDGE_POLARITY = 269488434,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_FIND_MODE = 269488435,
		/// <summary>
		/// ch: 搜索方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_FIND_ORIENT = 269488436,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_EDGE_THRESHOLD = 269488437,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_CALIPER_NUM = 269488438,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_REGION_WIDTH = 269488439,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_HALF_KERNEL_SIZE = 269488440,
		/// <summary>
		/// ch: 拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_FIT_FUN = 269488441,
		/// <summary>
		/// ch: 初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_FIT_INIT_TYPE = 269488442,
		/// <summary>
		/// ch: 剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_REJECT_DIST = 269488443,
		/// <summary>
		/// ch: 忽略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_TOP_LINE_REJECT_NUM = 269488444,
		/// <summary>
		/// ch: 边缘极性
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_EDGE_POLARITY = 269488445,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_FIND_MODE = 269488446,
		/// <summary>
		/// ch: 搜索方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_FIND_ORIENT = 269488447,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_EDGE_THRESHOLD = 269488448,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_CALIPER_NUM = 269488449,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_REGION_WIDTH = 269488450,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_HALF_KERNEL_SIZE = 269488451,
		/// <summary>
		/// ch: 拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_FIT_FUN = 269488452,
		/// <summary>
		/// ch: 初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_FIT_INIT_TYPE = 269488453,
		/// <summary>
		/// ch: 剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_REJECT_DIST = 269488454,
		/// <summary>
		/// ch: 忽略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_RIGHT_LINE_REJECT_NUM = 269488455,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_EDGE_POLARITY = 269488456,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_FIND_MODE = 269488457,
		/// <summary>
		/// ch: 搜索方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_FIND_ORIENT = 269488458,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_EDGE_THRESHOLD = 269488459,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_CALIPER_NUM = 269488460,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_REGION_WIDTH = 269488461,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_HALF_KERNEL_SIZE = 269488462,
		/// <summary>
		/// ch: 拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_FIT_FUN = 269488463,
		/// <summary>
		/// ch: 初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_FIT_INIT_TYPE = 269488464,
		/// <summary>
		/// ch: 剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_REJECT_DIST = 269488465,
		/// <summary>
		/// ch: 忽略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_BOTTOM_LINE_REJECT_NUM = 269488466,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_EDGE_POLARITY = 269488467,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_FIND_MODE = 269488468,
		/// <summary>
		/// ch: 搜索方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_FIND_ORIENT = 269488469,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_EDGE_THRESHOLD = 269488470,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_CALIPER_NUM = 269488471,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_REGION_WIDTH = 269488472,
		/// <summary>
		/// ch: 滤波核半宽参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_HALF_KERNEL_SIZE = 269488473,
		/// <summary>
		/// ch: 拟合方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_FIT_FUN = 269488474,
		/// <summary>
		/// ch: 初始拟合类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_FIT_INIT_TYPE = 269488475,
		/// <summary>
		/// ch: 剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_REJECT_DIST = 269488476,
		/// <summary>
		/// ch: 忽略点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_QUADRANGLE_LEFT_LINE_REJECT_NUM = 269488477,
		/// <summary>
		/// ch: 旋转角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_FINDMARK_ROTATE_ANGLE = 269488478,
		/// <summary>
		/// ch: 旋转步长参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_FINDMARK_ROTATE_STEP = 269488479,
		/// <summary>
		/// ch: 投影间隔参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_FINDMARK_PROJECT_OFFSET = 269488480,
		/// <summary>
		/// ch: 定位类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_FINDMARK_LOCATION_TYPE = 269488481,
		/// <summary>
		/// ch: mark类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_FINDMARK_MARK_TYPE = 269488482,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_REGION_PROJECTION_KERNEL_SIZE = 269488483,
		/// <summary>
		/// ch: 插值类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_REGION_PROJECTION_INTERPOLATION = 269488484,
		/// <summary>
		/// ch: 圆度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_CIRCLE_RATE = 269488485,
		/// <summary>
		/// ch: 直线度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_LINE_RATE = 269488486,
		/// <summary>
		/// ch: 圆面积度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MODU_CIRCLE_AREA_RATE = 269488487,
		/// <summary>
		/// ch: 目标亮度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PITCH_FIND_OBJECT_BRIGHTNESS = 269488494,
		/// <summary>
		/// ch: 查找模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PITCH_FIND_FIND_MODE = 269488495,
		/// <summary>
		/// ch: 输入ROI类型不正确
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_PITCH_ROI_TYPE = 269488496,
		/// <summary>
		/// ch: box角度权重参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_BOX_ANGLE_WEIGHT = 269488497,
		/// <summary>
		/// ch: 边缘清晰度未使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_MUDO_EDGE_SHARPNESS_LOC = 269488498,
		/// <summary>
		/// ch: 椭圆拟合失败
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ELLIPSE_FIT_FAIL = 269488499,
		/// <summary>
		/// ch: 几何查找算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ABILITY_ARG_MODU_ENABLE = 269488500,
		/// <summary>
		/// ch: 几何查找算法能力集最大图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ABILITY_ARG_MAX_IMAGE_SIZE = 269488501,
		/// <summary>
		/// ch: 几何查找算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ABILITY_ARG_THREAD_NUM = 269488502,
		/// <summary>
		/// ch: 几何查找算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ABILITY_ARG_THREAD_HANDLE = 269488503,
		/// <summary>
		/// ch: 几何查找算法能力集ROI使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ABILITY_ARG_ROI_SIZE_ENABLE = 269488504,
		/// <summary>
		/// ch: box几何查找算法能力集连通域参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_ABILITY_ARG_CONNECTIVITY = 269488505,
		/// <summary>
		/// ch: 椭圆查找 误差容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_ELLIPSE_FIT_ERROR_TOLERANCE = 269488506,
		/// <summary>
		/// ch: blob分析：最大超界像素设置错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX_OUT_PIXEL_NUM = 269488507,
		/// <summary>
		/// ch: blob分析：包围框高度权重错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_RECT_H_WEIGHT = 269488508,
		/// <summary>
		/// ch: blob分析：包围框宽度权重错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_RECT_W_WEIGHT = 269488509,
		/// <summary>
		/// ch:圆边缘扫描方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_CIRCLE_EDGE_SCAN_ORIENT = 269488510,
		/// <summary>
		/// ch:标签级面积筛选和全局面积筛选无法同时使能
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_ENABLE_LABELWISE_AREA = 269488511,
		/// <summary>
		/// ch: blob最大内接矩形数量设置错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_MAX = 269488576,
		/// <summary>
		/// ch: blob输出直方图信息使能设置错误
		/// </summary>
		MVALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_ENABLE_HISTOGRAM_I = 269488577,
		/// <summary>
		/// ch: blob轮廓类型设置错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_BLOB_FIND_CONTOUR_TYPE = 269488512,
		/// <summary>
		///  ch:hough找多圆 边缘阈值   错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_HOUGH_CIRCLE_EDGE_THRESH = 269488513,
		/// <summary>
		///  ch:hough找多圆 圆最小半径 错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_HOUGH_CIRCLE_MIN_RADIUS = 269488514,
		/// <summary>
		///  ch:hough找多圆 圆最大半径 错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_HOUGH_CIRCLE_MAX_RADIUS = 269488515,
		/// <summary>
		///   ch:hough找多圆 hough阈值 错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_HOUGH_CIRCLE_HOUGH_THRESH = 269488516,
		/// <summary>
		///   ch:hough找多圆 圆搜索数量 错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_HOUGH_CIRCLE_CIRCLE_NUM = 269488517,
		/// <summary>
		///  ch:hough找多圆 圆心距离 错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_HOUGH_CIRCLE_CENTER_DIST = 269488518,
		/// <summary>
		///  ch:hough圆未使能
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_MUDO_HOUGH_CIRCLE = 269488519,
		/// <summary>
		///   ch:基于梯度多线的梯度阈值参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_GTHRESH = 269488520,
		/// <summary>
		///   ch:基于梯度多线的邻域梯度差阈值参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_ANCHOR_THRESH = 269488521,
		/// <summary>
		///  ch:基于梯度多线的图像处理步长参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_SCAN_INTERVAL = 269488522,
		/// <summary>
		///   ch:基于梯度多线的最少直线拟合点阈值参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_MIN_FIT_POIINT = 269488523,
		/// <summary>
		///  ch:基于梯度多线的直线拟合成功最大误差参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_LINE_FIT_THRESH = 269488524,
		/// <summary>
		///   ch:基于梯度多线的点到直线的最大距离阈值参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_PTOLINE_THRESH = 269488525,
		/// <summary>
		///  ch:基于梯度多线的是否连接被割断直线参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_IS_JUNCTION_LINE = 269488526,
		/// <summary>
		///  ch:基于梯度多线的直线最小长度参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_LINE_MIN_LEN = 269488527,
		/// <summary>
		///   ch:基于梯度多线的直线容忍角度偏差最大值参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_VAILD_ANGLE_THRESH = 269488528,
		/// <summary>
		///  ch:基于梯度多线的最大直线个数参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_GRAD_LINE_MAX_NUM = 269488529,
		/// <summary>
		///   ch:BOX查找模块未使能
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_MODU_FIND_BOX = 269488544,
		/// <summary>
		///  ch:BOX查找设置尺度出错
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_FIND_BOX_SIZE = 269488545,
		/// <summary>
		///   ch:BOX查找得分类型
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_FIND_BOX_SCORE_TYPE = 269488546,
		/// <summary>
		///  ch:BOX查找最小得分出错
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_FIND_BOX_MIN_SCORE = 269488547,
		/// <summary>
		///  ch:BOX查找最大数量出错
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_FIND_BOX_MAX_NUM = 269488548,
		/// <summary>
		///  ch:BOX查找最大重叠率出错
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_FIND_BOX_MAX_OVERLAP = 269488549,
		/// <summary>
		/// ch:BOX查找极性设置出错
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_FIND_BOX_POLARITY = 269488550,
		/// <summary>
		/// ch:直线角度开关参数异常
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_LINE_ANGLE_ENABLE = 269488551,
		/// <summary>
		/// ch:直线期望角度参数异常
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_LINE_EXPECT_ANGLE = 269488552,
		/// <summary>
		/// ch:直线角度偏离容忍参数异常
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_LINE_ROTATION_TOLERANCE = 269488553,
		/// <summary>
		/// ch:轮廓近似点数输入错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_CONTOUR_APPROX_POINT_NUM = 269488554,
		/// <summary>
		/// ch:排序类型错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_SORT_TYPE = 269488560,
		/// <summary>
		/// ch:边缘点最大数量参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_MAX_EDGES = 269488561,
		/// <summary>
		/// ch:边缘点提取太多，可能导致耗时过大，建议提升边缘阈值或者减少卡尺数量(增大投影长度)
		/// </summary>
		MVD_ALG_STS_ERR_MVBGEOFIND_PARAM_MULTILINE_FIND_LINE_TOO_MUCH_EDGES = 269488562,
		/// <summary>
		/// ch: 几何测量模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_MODU_GEOMETRY_MEASURE_DISABLE = 269492224,
		/// <summary>
		/// ch: 灰度测量模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_MODU_INTENSITY_MEASURE_DISABLE = 269492225,
		/// <summary>
		/// ch: 几何拟合模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_MODU_GEOMETRY_FIT_DISABLE = 269492226,
		/// <summary>
		/// ch: 圆拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_INIT_TYPE = 269492227,
		/// <summary>
		/// ch: 圆拟合权重函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_WEIGHT_FUN = 269492228,
		/// <summary>
		/// ch: 圆拟合最大迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_MAX_ITERS = 269492229,
		/// <summary>
		/// ch: 圆拟合剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_REJECT_NUM = 269492230,
		/// <summary>
		/// ch: 圆拟合剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_REJECT_DIST = 269492231,
		/// <summary>
		/// ch: 圆拟合期望半径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_EXPECT_RADIUS = 269492232,
		/// <summary>
		/// ch: 圆拟合半径容忍参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_CIRCLE_FIT_RADIUS_TOLERANCE = 269492233,
		/// <summary>
		/// ch: 直线拟合初始化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_LINE_FIT_INIT_TYPE = 269492234,
		/// <summary>
		/// ch: 直线拟合拟合函数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_LINE_FIT_WEIGHT_FUN = 269492235,
		/// <summary>
		/// ch: 直线拟合最大迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_LINE_FIT_MAX_ITERS = 269492236,
		/// <summary>
		/// ch: 直线拟合剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_LINE_FIT_REJECT_NUM = 269492237,
		/// <summary>
		/// ch: 直线拟合剔除距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_LINE_FIT_REJECT_DIST = 269492238,
		/// <summary>
		/// ch: 像素统计阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_PIXEL_COUNT_THRESH = 269492239,
		/// <summary>
		/// ch: 测量算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_ABILITY_ARG_MODU_ENABLE = 269492240,
		/// <summary>
		/// ch: 测量算法能力集最大图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_ABILITY_ARG_MAX_IMAGE_SIZE = 269492241,
		/// <summary>
		/// ch: 测量算法能力集最大点个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_ABILITY_ARG_MAX_POINT_NUM = 269492242,
		/// <summary>
		/// ch: 测量算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MMVBMEASURE_ABILITY_ARG_THREAD_NUM = 269492243,
		/// <summary>
		/// ch: 测量算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_ABILITY_ARG_THREAD_HANDLE = 269492244,
		/// <summary>
		/// ch: 测量算法椭圆拟合失败
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_ELLIPSE_FIT_FAIL = 269492245,
		/// <summary>
		/// ch: 椭圆拟合 误差容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_ELLIPSE_FIT_ERROR_TOLERANCE = 269492246,
		/// <summary>
		/// ch: 像素统计阈值参数指针为空
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_PIXEL_COUNT_POINTER_NULL = 269492247,
		/// <summary>
		/// ch: 像素统计V3 MASK使能输入错误
		/// </summary>
		MVALG_STS_ERR_MVBMEASURE_PARAM_PIXEL_COUNT_USE_MASK = 269492248,
		/// <summary>
		/// ch: 图像增强模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_ENHANCE_DISABLE = 269508608,
		/// <summary>
		/// ch: 图像滤波模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_FILTER_DISABLE = 269508609,
		/// <summary>
		/// ch: 图像几何变换模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_TRAN_DISABLE = 269508610,
		/// <summary>
		/// ch: 图像二值化模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_BINA_DISABLE = 269508611,
		/// <summary>
		/// ch: 图像灰度形态学模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_GRAY_MORP_DISABLE = 269508612,
		/// <summary>
		/// ch: 图像二值形态学模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_BINARY_MORP_DISABLE = 269508613,
		/// <summary>
		/// ch: 图像算术运算模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_MATH_DISABLE = 269508614,
		/// <summary>
		/// ch: 图像统计模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_STATISTICS_DISABLE = 269508615,
		/// <summary>
		/// ch: 图像清晰度评估模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_SHARPNESS_DISABLE = 269508616,
		/// <summary>
		/// ch: 图像归一化处理模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_NORMLIZE_DISABLE = 269508617,
		/// <summary>
		/// ch: 帧平均处理模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_FRAME_MEAN_DISABLE = 269508618,
		/// <summary>
		/// ch: 图像矫正处理模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_UNDO_DISTORT_DISABLE = 269508619,
		/// <summary>
		/// ch: 图像掩膜生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_MASK_GENERATE_DISABLE = 269508620,
		/// <summary>
		/// ch: 图像区域拷贝模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_COPY_REGION_DISABLE = 269508621,
		/// <summary>
		/// ch: 图像旋转模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_IMG_ROTATE_DISABLE = 269508622,
		/// <summary>
		/// ch: 图像镜像模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_IMG_MIRROR_DISABLE = 269508623,
		/// <summary>
		/// ch: 圆环展开模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_POLAR_UNWARP_DISABLE = 269508624,
		/// <summary>
		/// ch: 仿射变换模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_AFFINE_TRANS_DISABLE = 269508625,
		/// <summary>
		/// ch: 图像几何变换模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_TRAN_ALL_DISABLE = 269508626,
		/// <summary>
		/// ch: 增强类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_TYPE = 269508627,
		/// <summary>
		/// ch: 对比度调节因子参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_CONTRAST_FACTOR = 269508628,
		/// <summary>
		/// ch: 锐度调节因子参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_SHARPEN_FACTOR = 269508629,
		/// <summary>
		/// ch: gamma值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_GAMMA = 269508630,
		/// <summary>
		/// ch: 锐化平滑核大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_SHARPEN_KERNELSIZE = 269508631,
		/// <summary>
		/// ch: LBCE算法作用强度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_LBCE_STRENGTH = 269508632,
		/// <summary>
		/// ch: LBCE算法最大增强比率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_LBCE_CEIL_SLOPE = 269508633,
		/// <summary>
		/// ch: LBCE算法最小增强比率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_LBCE_FLOOR_SLOPE = 269508634,
		/// <summary>
		/// ch: LBCE算法暗区范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_LBCE_SHADOW = 269508635,
		/// <summary>
		/// ch: LBCE算法亮区范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_LBCE_BRIGHT = 269508636,
		/// <summary>
		/// ch: LBCE算法滤波截止频率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ENHANCE_LBCE_CUT_OFF = 269508637,
		/// <summary>
		/// ch: 阴影校正方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHADE_CORRECT_DIR = 269508638,
		/// <summary>
		/// ch: 阴影校正比率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHADE_CORRECT_RATIO = 269508639,
		/// <summary>
		/// ch: 阴影校正增益参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHADE_CORRECT_GAIN = 269508640,
		/// <summary>
		/// ch: 阴影校正补偿值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHADE_CORRECT_OFFSET = 269508641,
		/// <summary>
		/// ch: 阴影校正噪声值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHADE_CORRECT_NOISE = 269508642,
		/// <summary>
		/// ch: 亮度校正增益参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BRIGHT_ADJUST_GAIN = 269508643,
		/// <summary>
		/// ch: 亮度校正补偿值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BRIGHT_ADJUST_OFFSET = 269508644,
		/// <summary>
		/// ch: 形态学类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_TYPE = 269508645,
		/// <summary>
		/// ch: 灰度形态学形状参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_SHAPE = 269508646,
		/// <summary>
		/// ch: 迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_ITERATIONS = 269508647,
		/// <summary>
		/// ch: 模板宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_KERNELSIZE_X = 269508648,
		/// <summary>
		/// ch: 模板高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_KERNELSIZE_Y = 269508649,
		/// <summary>
		/// ch: 锚点x参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_ANCHOR_X = 269508650,
		/// <summary>
		/// ch: 锚点y参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_GRAY_MORP_ANCHOR_Y = 269508651,
		/// <summary>
		/// ch: 形态学类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_TYPE = 269508652,
		/// <summary>
		/// ch: 二值形态学形状参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_SHAPE = 269508653,
		/// <summary>
		/// ch: 迭代次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_ITERATIONS = 269508654,
		/// <summary>
		/// ch: 模板宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_KERNELSIZE_X = 269508655,
		/// <summary>
		/// ch: 模板高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_KERNELSIZE_Y = 269508656,
		/// <summary>
		/// ch: 锚点x参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_ANCHOR_X = 269508657,
		/// <summary>
		/// ch: 锚点y参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINARY_MORP_ANCHOR_Y = 269508658,
		/// <summary>
		/// ch: 几何变换类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_TYPE = 269508659,
		/// <summary>
		/// ch: 旋转角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_ANGLE = 269508660,
		/// <summary>
		/// ch: 插值类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_INTERPOLATION = 269508661,
		/// <summary>
		/// ch: 目标图像高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SCALE_SIZE_HEIGHT = 269508662,
		/// <summary>
		/// ch: 目标图像宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SCALE_SIZE_WIDTH = 269508663,
		/// <summary>
		/// ch: 展开起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_START_ANGLE = 269508664,
		/// <summary>
		/// ch: 展开终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_ANGLE_SPAN = 269508665,
		/// <summary>
		/// ch: 展开内径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_IN_RADIUS = 269508666,
		/// <summary>
		/// ch: 展开外径参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_OUT_RADIUS = 269508667,
		/// <summary>
		/// ch: 展开圆心点坐标x参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_CENTER_X = 269508668,
		/// <summary>
		/// ch: 展开圆心点坐标y参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_CENTER_Y = 269508669,
		/// <summary>
		/// ch: 镜像类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_MIRROR_TYPE = 269508670,
		/// <summary>
		/// ch: 区域拷贝类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_REGION_COPY_TYPE = 269508671,
		/// <summary>
		/// ch: 区域填充值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_REGION_COPY_FILL_VAL = 269508672,
		/// <summary>
		/// ch: 区域外填充值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_REGION_COPY_EXT_FILL_VAL = 269508673,
		/// <summary>
		/// ch: 圆环展开角度方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_POLAR_UNWARP_ANGLE_DIR = 269508674,
		/// <summary>
		/// ch: 圆环展开半径方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_TRAN_POLAR_UNWARP_RADIUS_DIR = 269508675,
		/// <summary>
		/// ch: 插值类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_AFFINE_TRAN_INTERPOLATION = 269508676,
		/// <summary>
		/// ch: 扩充类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_AFFINE_TRAN_EXTENT_TYPE = 269508677,
		/// <summary>
		/// ch: 填充值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_AFFINE_TRAN_EXTENT_VAL = 269508678,
		/// <summary>
		/// ch: 滤波类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_TYPE = 269508679,
		/// <summary>
		/// ch: 均值/中值滤波核宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_KERNELSIZE_X = 269508680,
		/// <summary>
		/// ch: 均值/中值均值滤波核高度参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_KERNELSIZE_Y = 269508681,
		/// <summary>
		/// ch: 高斯滤波核参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_GAUSS_KERNELSIZE = 269508682,
		/// <summary>
		/// ch: 高斯滤波方差参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_SIGMA = 269508683,
		/// <summary>
		/// ch: 高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_HIGH_THRE = 269508684,
		/// <summary>
		/// ch: 低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FILT_LOW_THRE = 269508685,
		/// <summary>
		/// ch: 二值化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_TYPE = 269508686,
		/// <summary>
		/// ch: 二值化低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_LOW_THRESH = 269508687,
		/// <summary>
		/// ch: 二值化高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_HIGH_THRESH = 269508688,
		/// <summary>
		/// ch: 滤波核宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_KERNELSIZE_X = 269508689,
		/// <summary>
		/// ch: 滤波核高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_KERNELSIZE_Y = 269508690,
		/// <summary>
		/// ch: 高斯滤波核大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_GAUSS_KERNELSIZE = 269508691,
		/// <summary>
		/// ch: 高斯滤波方差参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_SIGMA = 269508692,
		/// <summary>
		/// ch: 线扫描方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_LINE_SCAN_MODE = 269508693,
		/// <summary>
		/// ch: 线扫描长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_LINE_SCAN_LEN = 269508694,
		/// <summary>
		/// ch: 线扫描差异阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_LINE_SCAN_THRESH = 269508695,
		/// <summary>
		/// ch: 线扫描分割类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_LINE_SCAN_SEG_TYPE = 269508696,
		/// <summary>
		/// ch: 比较类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_COMPARE_TYPE = 269508697,
		/// <summary>
		/// ch: 阈值偏移量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_DYN_OFFSET = 269508698,
		/// <summary>
		/// ch: 算术运算类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_ARITHMETIC_TYPE = 269508699,
		/// <summary>
		/// ch: 统计类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_STATISTICS_TYPE = 269508700,
		/// <summary>
		/// ch: 清晰度评价方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHARPNESS_MODE = 269508701,
		/// <summary>
		/// ch: 噪声等级参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_SHARPNESS_NOISE_LEVEL = 269508702,
		/// <summary>
		/// ch: 归一化类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_TYPE = 269508703,
		/// <summary>
		/// ch: 目标均值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_MEAN_VAL = 269508704,
		/// <summary>
		/// ch: 目标标准差值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_STD_VAL = 269508705,
		/// <summary>
		/// ch: 左去尾比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_LEFT_CLIP_PERCENT = 269508706,
		/// <summary>
		/// ch: 右去尾比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_RIGHT_CLIP_PERCENT = 269508707,
		/// <summary>
		/// ch: 目标灰度值下限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_DST_LEFT_POS = 269508708,
		/// <summary>
		/// ch: 目标灰度值上限参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_NORMLIZE_DST_RIGHT_POS = 269508709,
		/// <summary>
		/// ch: 低阈值比例系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FRAME_MEAN_LOW_SCALE = 269508710,
		/// <summary>
		/// ch: 低阈值偏移系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FRAME_MEAN_LOW_OFFSET = 269508711,
		/// <summary>
		/// ch: 高阈值比例系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FRAME_MEAN_HIGH_SCALE = 269508712,
		/// <summary>
		/// ch: 高阈值偏移系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_FRAME_MEAN_HIGH_OFFSET = 269508713,
		/// <summary>
		/// ch: 缩放量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_UNDO_DISTORT_ZOOM = 269508714,
		/// <summary>
		/// ch: 曲张量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_UNDO_DISTORT_WARP = 269508715,
		/// <summary>
		/// ch: 畸变矫正模型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_WARP_DISCORRECT_MODEL = 269508716,
		/// <summary>
		/// ch: 畸变矫正标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_WARP_DISCORRECT_FLAG = 269508717,
		/// <summary>
		/// ch: 基于标定的畸变校正模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_CALIB_DISTORT_DISABLE = 269508718,
		/// <summary>
		/// ch: 图像二值化模块非ROI区域处理方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_NON_ROI_PRC_TYPE = 269508719,
		/// <summary>
		/// ch: 基于阵列匹配结果修正校正模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_2DARRAY_CORRECT_DISABLE = 269508720,
		/// <summary>
		/// ch: 基于阵列匹配结果修正校正模块参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_2DARRAY_CORRECT_PARAM_EEROR = 269508721,
		/// <summary>
		/// ch: Box融合模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_BOX_MERGE_DISABLE = 269508722,
		/// <summary>
		/// ch: Box融合模块参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_BOX_MERGE_PARAM_EEROR = 269508723,
		/// <summary>
		/// ch: Box重叠率计算未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_BOX_OVERLAP_DISABLE = 269508724,
		/// <summary>
		/// ch: 实例分割掩膜图二值化模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_SEGMASKIMG_SEGMENT_DISABLE = 269508725,
		/// <summary>
		/// ch: 掩膜图外box过滤
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_MODU_BOXFILTER_OUTMASKIMG_DISABLE = 269508726,
		/// <summary>
		/// ch: 图像缩放插值类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_IMAGE_RESIZE_INTERPOLATION = 269508727,
		/// <summary>
		/// ch: 图像缩放是否抗混叠参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_IMAGE_RESIZE_ANTIANLIASING = 269508728,
		/// <summary>
		/// ch: 图像二值化模块sauvola校正系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_SAUVOLA_ADJ_COEF = 269508729,
		/// <summary>
		/// ch: 图像二值化模块sauvola动态范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_SAUVOLA_RANGE = 269508730,
		/// <summary>
		/// ch: 图像二值化模块sauvola分割类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_SAUVOLA_SEG_TYPE = 269508731,
		/// <summary>
		/// ch: 图像二值化模块sauvola滤波核宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_SAUVOLA_WIN_WITDTH = 269508732,
		/// <summary>
		/// ch: 图像二值化模块sauvola滤波核高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_BINA_SAUVOLA_WIN_HEIGHT = 269508733,
		/// <summary>
		/// ch: 图像预处理算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_MODU_ENABLE = 269508734,
		/// <summary>
		/// ch: 图像预处理算法能力集最大输入图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_MAX_SRC_IMG_SIZE = 269508735,
		/// <summary>
		/// ch: 图像预处理算法能力集最大输出图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_MAX_DST_IMG_SIZE = 269508736,
		/// <summary>
		/// ch: 图像预处理算法能力集ROI使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_ROI_SIZE_ENABLE = 269508737,
		/// <summary>
		/// ch: 图像预处理算法能力集ROI类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_ROI_TYPE = 269508738,
		/// <summary>
		/// ch: 图像预处理算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_THREAD_NUM = 269508739,
		/// <summary>
		/// ch: 图像预处理算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_THREAD_HANDLE = 269508740,
		/// <summary>
		/// ch: 图像增强子模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_ENHANCE_ENABLE = 269508741,
		/// <summary>
		/// ch: 图像二值化子模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_ABILITY_ARG_BINARY_ENABLE = 269508742,
		/// <summary>
		/// ch: 拷贝填充算子填充方式错误
		/// </summary>
		MVALG_STS_ERR_MVBPREPRO_PARAM_REGION_COPY_FILL_TYPE = 269508743,
		/// <summary>
		///   ch: 图像投影模块未使能
		/// </summary>
		MVD_ALG_STS_ERR_MVBPREPRO_MODU_IMAGE_PROJ_DISABLE = 269508744,
		/// <summary>
		///  ch: 图像投影模块参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBPREPRO_MODU_IMAGE_PROJ_PARAM_EEROR = 269508745,
		/// <summary>
		///  ch: box去重模块未使能
		/// </summary>
		MVD_ALG_STS_ERR_MVBPREPRO_MODU_BOX_REMOVE_REPEAT_DISABLE = 269508746,
		/// <summary>
		///   ch: box去重模块参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBPREPRO_MODU_BOX_REMOVE_REPEAT_PARAM_EEROR = 269508747,
		/// <summary>
		///  ch: 图像预处理算法能力集最大输入box数参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBPREPRO_ABILITY_ARG_BOX_REMOVE_REPEAT_MAX_NUM = 269508748,
		/// <summary>
		/// ch: 特征不足：特征模板生成失败 | en: Failed to generate the feature modle
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_FEATURES_NOT_ENOUGH = 269484032,
		/// <summary>
		/// ch: 算法版本号出错：模板版本号与匹配版本不符 | en: Algorithm version error. The version of the feature modle does not match the version of Matching.
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MATCH_VERSION = 269484033,
		/// <summary>
		/// ch: 模型版本号出错：模板版本号与当前版本不符 | en: Modle version error. The version of modle input does match the current version.
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODEL_VERSION = 269484034,
		/// <summary>
		/// ch: 建模时，模型内存申请失败 | en: Model memory allocation failed while modeling.
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MALLOC_MODEL_BUFF_FAIL = 269484035,
		/// <summary>
		/// ch: 模型中没有图像 | en: Image not included in model
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODEL_NO_IMG = 269484036,
		/// <summary>
		/// ch: 模型与平台不匹配 | en: Model does not match platform
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODEL_PLATFORM = 269484037,
		/// <summary>
		/// ch: 匹配时内存申请失败 | en: Match malloc buff failed
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MALLOC_MATCH_BUFF_FAIL = 269484038,
		/// <summary>
		/// ch: 灰度模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_GRAY_MODEL_DISABLE = 269484039,
		/// <summary>
		/// ch: 灰度模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_GRAY_MATCH_DISABLE = 269484040,
		/// <summary>
		/// ch: 特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FEATURE_MODEL_DISABLE = 269484041,
		/// <summary>
		/// ch: 特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FEATURE_MATCH_DISABLE = 269484042,
		/// <summary>
		/// ch: 高精度特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_KEYPNT_MODEL_DISABLE = 269484043,
		/// <summary>
		/// ch: 高精度特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_KEYPNT_MATCH_DISABLE = 269484044,
		/// <summary>
		/// ch: 精修特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_REFINE_MODEL_DISABLE = 269484045,
		/// <summary>
		/// ch: 精修特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_REFINE_MATCH_DISABLE = 269484046,
		/// <summary>
		/// ch: 快速特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FAST_KEYPNT_MODEL_DISABLE = 269484047,
		/// <summary>
		/// ch: 快速特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FAST_KEYPNT_MATCH_DISABLE = 269484048,
		/// <summary>
		/// ch: 快速特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FAST_FEATURE_MODEL_DISABLE = 269484049,
		/// <summary>
		/// ch: 快速特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FAST_FEATURE_MATCH_DISABLE = 269484050,
		/// <summary>
		/// ch: S16特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FEATURE_S16_MODEL_DISABLE = 269484051,
		/// <summary>
		/// ch: S16特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_FEATURE_S16_MATCH_DISABLE = 269484052,
		/// <summary>
		/// ch: 灰度模板建模金字塔层数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MODEL_NUM_LEVEL = 269484309,
		/// <summary>
		/// ch: 灰度模板建模起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MODEL_ANGLE_START = 269484310,
		/// <summary>
		/// ch: 灰度模板建模终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MODEL_ANGLE_END = 269484311,
		/// <summary>
		/// ch: 灰度模板建模角度步长参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MODEL_ANGLE_STEP = 269484312,
		/// <summary>
		/// ch: 灰度模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_MIN_SCORE = 269484313,
		/// <summary>
		/// ch: 灰度模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_MAX_MATCH_NUM = 269484314,
		/// <summary>
		/// ch: 灰度模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_MAX_OVERLAP = 269484320,
		/// <summary>
		/// ch: 灰度模板匹配角度步长参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_ANGLE_STEP = 269484321,
		/// <summary>
		/// ch: 灰度模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_ANGLE_START = 269484322,
		/// <summary>
		/// ch: 灰度模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_ANGLE_END = 269484323,
		/// <summary>
		/// ch: 灰度模板匹配终止金字塔层数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_END_PYD_LVL = 269484324,
		/// <summary>
		/// ch: 灰度模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_POLARITY = 269484325,
		/// <summary>
		/// ch: 灰度模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_SORT_TYPE = 269484326,
		/// <summary>
		/// ch: 灰度模板匹配超时控制参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_TIME_OUT = 269484327,
		/// <summary>
		/// ch: 灰度模板匹配ROI尺寸不得小于模板尺寸
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MATCH_ROI_SIZE = 269484328,
		/// <summary>
		/// ch: 灰度模板建模启用自动确定金字塔层数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_GRAY_MODEL_NUM_LEVEL_AUTO = 269484329,
		/// <summary>
		/// ch: 特征模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MODEL_PYRAMIDSCALE_FLAG = 269484063,
		/// <summary>
		/// ch: 特征模板模型精度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MODEL_PYRAMIDSCALE_FINE = 269484064,
		/// <summary>
		/// ch: 特征模板模型速度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MODEL_PYRAMIDSCALE_ROUGH = 269484065,
		/// <summary>
		/// ch: 特征模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MODEL_EDGE_THRESHOLD_FLAG = 269484066,
		/// <summary>
		/// ch: 特征模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MODEL_EDGE_THRESHOLD = 269484067,
		/// <summary>
		/// ch: 特征模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_MIN_SCORE = 269484068,
		/// <summary>
		/// ch: 特征模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_MAX_MATCH_NUM = 269484069,
		/// <summary>
		/// ch: 特征模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_POLARITY = 269484070,
		/// <summary>
		/// ch: 特征模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_ANGLE_START = 269484071,
		/// <summary>
		/// ch: 特征模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_ANGLE_END = 269484072,
		/// <summary>
		/// ch: 特征模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_MAX_OVERLAP = 269484073,
		/// <summary>
		/// ch: 特征模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SCALE_START = 269484074,
		/// <summary>
		/// ch: 特征模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SCALE_END = 269484075,
		/// <summary>
		/// ch: 特征模板匹配X起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SCALE_X_START = 269484076,
		/// <summary>
		/// ch: 特征模板匹配X终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SCALE_X_END = 269484077,
		/// <summary>
		/// ch: 特征模板匹配Y起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SCALE_Y_START = 269484078,
		/// <summary>
		/// ch: 特征模板匹配Y终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SCALE_Y_END = 269484079,
		/// <summary>
		/// ch: 特征模板匹配延拓阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_EXTENT_RATE = 269484080,
		/// <summary>
		/// ch: 特征模板匹配速度阈值开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_ROUGH_FLAG = 269484081,
		/// <summary>
		/// ch: 特征模板匹配速度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_ROUGH_THD = 269484082,
		/// <summary>
		/// ch: 特征模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_SORT_TYPE = 269484083,
		/// <summary>
		/// ch: 特征模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MATCH_TIME_OUT = 269484084,
		/// <summary>
		/// ch: 高精度模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_PYRAMIDSCALE_FLAG = 269484085,
		/// <summary>
		/// ch: 高精度模板模型精度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_PYRAMIDSCALE_FINE = 269484086,
		/// <summary>
		/// ch: 高精度模板模型速度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_PYRAMIDSCALE_ROUGH = 269484087,
		/// <summary>
		/// ch: 高精度模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_EDGE_THRESHOLD_FLAG = 269484088,
		/// <summary>
		/// ch: 高精度模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_EDGE_THRESHOLD = 269484089,
		/// <summary>
		/// ch: 高精度模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_MIN_SCORE = 269484090,
		/// <summary>
		/// ch: 高精度模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_MAX_MATCH_NUM = 269484091,
		/// <summary>
		/// ch: 高精度模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_POLARITY = 269484092,
		/// <summary>
		/// ch: 高精度模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_ANGLE_START = 269484093,
		/// <summary>
		/// ch: 高精度模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_ANGLE_END = 269484094,
		/// <summary>
		/// ch: 高精度模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SCALE_START = 269484095,
		/// <summary>
		/// ch: 高精度模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SCALE_END = 269484096,
		/// <summary>
		/// ch: 高精度模板匹配噪点标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SPOTTER_FLAG = 269484097,
		/// <summary>
		/// ch: 高精度模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_MAX_OVERLAP = 269484098,
		/// <summary>
		/// ch: 高精度模板匹配X起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SCALE_X_START = 269484099,
		/// <summary>
		/// ch: 高精度模板匹配X终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SCALE_X_END = 269484100,
		/// <summary>
		/// ch: 高精度模板匹配Y起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SCALE_Y_START = 269484101,
		/// <summary>
		/// ch: 高精度模板匹配Y终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SCALE_Y_END = 269484102,
		/// <summary>
		/// ch: 高精度模板匹配延拓阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_EXTENT_RATE = 269484103,
		/// <summary>
		/// ch: 高精度模板匹配速度阈值开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_ROUGH_FLAG = 269484104,
		/// <summary>
		/// ch: 高精度模板匹配速度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_ROUGH_THD = 269484105,
		/// <summary>
		/// ch: 高精度模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_SORT_TYPE = 269484106,
		/// <summary>
		/// ch: 高精度模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_TIME_OUT = 269484107,
		/// <summary>
		/// ch: 高精度模板匹配边缘阈值标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_THD_FLAG = 269484108,
		/// <summary>
		/// ch: 高精度模板匹边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_THD = 269484109,
		/// <summary>
		/// ch: 关键点模板匹配重要性得分阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MATCH_IMPORTANCE_SCORE_TH = 269484195,
		/// <summary>
		/// ch: 关键点模板建模手动设置最小链长标识参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_MANUAL_CHAIN_LEN_FLAG = 269484196,
		/// <summary>
		/// ch: 建模最短链长阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_MIN_CHAIN_LEN = 269484197,
		/// <summary>
		/// ch: 精修模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MODEL_PYRAMIDSCALE_FLAG = 269484110,
		/// <summary>
		/// ch: 精修模板模型精度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MODEL_PYRAMIDSCALE_FINE = 269484111,
		/// <summary>
		/// ch: 精修模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MODEL_EDGE_THRESHOLD_FLAG = 269484112,
		/// <summary>
		/// ch: 精修模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MODEL_EDGE_THRESHOLD = 269484113,
		/// <summary>
		/// ch: 精修模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_MIN_SCORE = 269484114,
		/// <summary>
		/// ch: 精修模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_POLARITY = 269484115,
		/// <summary>
		/// ch: 精修模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_ANGLE_START = 269484116,
		/// <summary>
		/// ch: 精修模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_ANGLE_END = 269484117,
		/// <summary>
		/// ch: 精修模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SCALE_START = 269484118,
		/// <summary>
		/// ch: 精修模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SCALE_END = 269484119,
		/// <summary>
		/// ch: 精修模板匹配噪点标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SPOTTER_FLAG = 269484120,
		/// <summary>
		/// ch: 精修模板匹配X起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SCALE_X_START = 269484121,
		/// <summary>
		/// ch: 精修模板匹配X终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SCALE_X_END = 269484122,
		/// <summary>
		/// ch: 精修模板匹配Y起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SCALE_Y_START = 269484123,
		/// <summary>
		/// ch: 精修模板匹配Y终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_SCALE_Y_END = 269484124,
		/// <summary>
		/// ch: 精修模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_TIME_OUT = 269484125,
		/// <summary>
		/// ch: 精修模板匹配边缘阈值标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_THD_FLAG = 269484126,
		/// <summary>
		/// ch: 精修模板匹边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_REFINE_MATCH_THD = 269484127,
		/// <summary>
		/// ch: 快速模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MODEL_PYRAMIDSCALE_FLAG = 269484128,
		/// <summary>
		/// ch: 快速模板模型速度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MODEL_PYRAMIDSCALE_ROUGH = 269484129,
		/// <summary>
		/// ch: 快速模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MODEL_EDGE_THRESHOLD_FLAG = 269484130,
		/// <summary>
		/// ch: 快速模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MODEL_EDGE_THRESHOLD = 269484131,
		/// <summary>
		/// ch: 快速模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_MIN_SCORE = 269484132,
		/// <summary>
		/// ch: 快速模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_MAX_MATCH_NUM = 269484133,
		/// <summary>
		/// ch: 快速模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_POLARITY = 269484134,
		/// <summary>
		/// ch: 快速模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_ANGLE_START = 269484135,
		/// <summary>
		/// ch: 快速模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_ANGLE_END = 269484136,
		/// <summary>
		/// ch: 快速模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_SCALE_START = 269484137,
		/// <summary>
		/// ch: 快速模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_SCALE_END = 269484138,
		/// <summary>
		/// ch: 快速模板匹配噪点标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_SPOTTER_FLAG = 269484139,
		/// <summary>
		/// ch: 快速模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_MAX_OVERLAP = 269484140,
		/// <summary>
		/// ch: 快速模板匹配延拓阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_EXTENT_RATE = 269484141,
		/// <summary>
		/// ch: 快速模板匹配速度阈值开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_ROUGH_FLAG = 269484142,
		/// <summary>
		/// ch: 快速模板匹配速度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_ROUGH_THD = 269484143,
		/// <summary>
		/// ch: 快速模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_SORT_TYPE = 269484144,
		/// <summary>
		/// ch: 快速模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_TIME_OUT = 269484145,
		/// <summary>
		/// ch: 快速模板匹配边缘阈值标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_THD_FLAG = 269484146,
		/// <summary>
		/// ch: 快速模板匹边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MATCH_THD = 269484147,
		/// <summary>
		/// ch: 快速特征模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MODEL_PYRAMIDSCALE_FLAG = 269484148,
		/// <summary>
		/// ch: 快速特征模板模型速度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MODEL_PYRAMIDSCALE_ROUGH = 269484149,
		/// <summary>
		/// ch: 快速特征模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATUREMODEL_EDGE_THRESHOLD_FLAG = 269484150,
		/// <summary>
		/// ch: 快速特征模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MODEL_EDGE_THRESHOLD = 269484151,
		/// <summary>
		/// ch: 快速特征模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_MIN_SCORE = 269484152,
		/// <summary>
		/// ch: 快速特征模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_MAX_MATCH_NUM = 269484153,
		/// <summary>
		/// ch: 快速特征模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_POLARITY = 269484154,
		/// <summary>
		/// ch: 快速特征模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_ANGLE_START = 269484155,
		/// <summary>
		/// ch: 快速特征模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_ANGLE_END = 269484156,
		/// <summary>
		/// ch: 快速特征模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_MAX_OVERLAP = 269484157,
		/// <summary>
		/// ch: 快速特征模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_SCALE_START = 269484158,
		/// <summary>
		/// ch: 快速特征模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_SCALE_END = 269484159,
		/// <summary>
		/// ch: 快速特征模板匹配延拓阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_EXTENT_RATE = 269484160,
		/// <summary>
		/// ch: 快速特征模板匹配速度阈值开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_ROUGH_FLAG = 269484161,
		/// <summary>
		/// ch: 快速特征模板匹配速度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_ROUGH_THD = 269484162,
		/// <summary>
		/// ch: 快速特征模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_SORT_TYPE = 269484163,
		/// <summary>
		/// ch: 快速特征模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_FEATURE_MATCH_TIME_OUT = 269484164,
		/// <summary>
		/// ch: S16特征模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MODEL_PYRAMIDSCALE_FLAG = 269484165,
		/// <summary>
		/// ch: S16特征模板模型精度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MODEL_PYRAMIDSCALE_FINE = 269484166,
		/// <summary>
		/// ch: S16特征模板模型速度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MODEL_PYRAMIDSCALE_ROUGH = 269484167,
		/// <summary>
		/// ch: S16特征模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MODEL_EDGE_THRESHOLD_FLAG = 269484168,
		/// <summary>
		/// ch: S16特征模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MODEL_EDGE_THRESHOLD_HIGH = 269484169,
		/// <summary>
		/// ch: S16特征模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MODEL_EDGE_THRESHOLD_LOW = 269484170,
		/// <summary>
		/// ch: S16特征模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_MIN_SCORE = 269484171,
		/// <summary>
		/// ch: S16特征模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_MAX_MATCH_NUM = 269484172,
		/// <summary>
		/// ch: S16特征模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_POLARITY = 269484173,
		/// <summary>
		/// ch: S16特征模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_ANGLE_START = 269484174,
		/// <summary>
		/// ch: S16特征模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_ANGLE_END = 269484175,
		/// <summary>
		/// ch: S16特征模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_MAX_OVERLAP = 269484176,
		/// <summary>
		/// ch: S16特征模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SCALE_START = 269484177,
		/// <summary>
		/// ch: S16特征模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SCALE_END = 269484178,
		/// <summary>
		/// ch: S16特征模板匹配X起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SCALE_X_START = 269484179,
		/// <summary>
		/// ch: S16特征模板匹配X终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SCALE_X_END = 269484180,
		/// <summary>
		/// ch: S16特征模板匹配Y起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SCALE_Y_START = 269484181,
		/// <summary>
		/// ch: S16特征模板匹配Y终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SCALE_Y_END = 269484182,
		/// <summary>
		/// ch: S16特征模板匹配延拓查找标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_EXTENT_FLAG = 269484183,
		/// <summary>
		/// ch: S16特征模板匹配延拓阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_EXTENT_THD = 269484184,
		/// <summary>
		/// ch: S16特征模板匹配速度阈值开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_ROUGH_FLAG = 269484185,
		/// <summary>
		/// ch: S16特征模板匹配速度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_ROUGH_THD = 269484186,
		/// <summary>
		/// ch: S16特征模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_SORT_TYPE = 269484187,
		/// <summary>
		/// ch: S16特征模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_S16_MATCH_TIME_OUT_CTL = 269484188,
		/// <summary>
		/// ch: 处理器主频参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PROCESSOR_FREQUENCE = 269484189,
		/// <summary>
		/// ch: 高精度模板是否包含图像标记错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_MODEL_IMAGE_FLAG = 269484190,
		/// <summary>
		/// ch: 快速模板是否包含图像标记错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FAST_KEYPNT_MODEL_IMAGE_FLAG = 269484191,
		/// <summary>
		/// ch: 特征模板模型特征灵敏度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_MODEL_SENSITIVITY = 269484192,
		/// <summary>
		/// ch: 特征模板模型特征灵敏度数值错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_FEATURE_VALUE_SENSITIVITY = 269484193,
		/// <summary>
		/// ch: 权重掩膜标记错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_KEYPNT_WEIGHT_MASK_FLAG = 269484194,
		/// <summary>
		/// ch: 动态内存异常
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_DYMANIC_MEM_ERROR = 269484198,
		/// <summary>
		/// ch: 模板匹配算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_ABILITY_ARG_MODU_ENABLE = 269484199,
		/// <summary>
		/// ch: 模板匹配算法能力集最大图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_ABILITY_ARG_MAX_IMAGE_SIZE = 269484200,
		/// <summary>
		/// ch: 模板匹配算法能力集最大模板宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_ABILITY_ARG_MAX_TEMP_SIZE = 269484201,
		/// <summary>
		/// ch: 模板匹配算法能力集ROI使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_ABILITY_ARG_ROI_ENABLE = 269484202,
		/// <summary>
		/// ch: 模板匹配算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_ABILITY_ARG_THREAD_NUM = 269484203,
		/// <summary>
		/// ch: 模板匹配算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_ABILITY_ARG_THREAD_HANDLE = 269484204,
		/// <summary>
		/// ch: 关键点特征模型生成模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_PYRAMID_MODEL_DISABLE = 269484208,
		/// <summary>
		/// ch: 关键点特征模板匹配查找模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_MODU_PYRAMID_MATCH_DISABLE = 269484209,
		/// <summary>
		/// ch: 关键点模板模型特征尺度标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_PYRAMIDSCALE_FLAG = 269484210,
		/// <summary>
		/// ch: 关键点模板模型精度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_PYRAMIDSCALE_FINE = 269484211,
		/// <summary>
		/// ch: 关键点模板模型速度特征尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_PYRAMIDSCALE_ROUGH = 269484212,
		/// <summary>
		/// ch: 关键点模板模型边缘阈值标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_EDGE_THRESHOLD_FLAG = 269484213,
		/// <summary>
		/// ch: 关键点模板模型边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_EDGE_THRESHOLD = 269484214,
		/// <summary>
		/// ch: 关键点模板匹配最小匹配分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_MIN_SCORE = 269484215,
		/// <summary>
		/// ch: 关键点模板匹配最大匹配个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_MAX_MATCH_NUM = 269484216,
		/// <summary>
		/// ch: 关键点模板匹配匹配极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_POLARITY = 269484217,
		/// <summary>
		/// ch: 关键点模板匹配起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_ANGLE_START = 269484218,
		/// <summary>
		/// ch: 关键点模板匹配终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_ANGLE_END = 269484219,
		/// <summary>
		/// ch: 关键点模板匹配起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SCALE_START = 269484220,
		/// <summary>
		/// ch: 关键点模板匹配终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SCALE_END = 269484221,
		/// <summary>
		/// ch: 关键点模板匹配噪点标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SPOTTER_FLAG = 269484222,
		/// <summary>
		/// ch: 关键点模板匹配最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_MAX_OVERLAP = 269484223,
		/// <summary>
		/// ch: 关键点模板匹配X起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SCALE_X_START = 269484224,
		/// <summary>
		/// ch: 关键点模板匹配X终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SCALE_X_END = 269484225,
		/// <summary>
		/// ch: 关键点模板匹配Y起始尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SCALE_Y_START = 269484226,
		/// <summary>
		/// ch: 关键点模板匹配Y终止尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SCALE_Y_END = 269484227,
		/// <summary>
		/// ch: 关键点模板匹配延拓阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_EXTENT_RATE = 269484228,
		/// <summary>
		/// ch: 关键点模板匹配速度阈值开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_ROUGH_FLAG = 269484229,
		/// <summary>
		/// ch: 关键点模板匹配速度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_ROUGH_THD = 269484230,
		/// <summary>
		/// ch: 关键点模板匹配排序类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_SORT_TYPE = 269484231,
		/// <summary>
		/// ch: 关键点模板匹超时控制配参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_TIME_OUT = 269484232,
		/// <summary>
		/// ch: 关键点模板匹配边缘阈值标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_THD_FLAG = 269484233,
		/// <summary>
		/// ch: 关键点模板匹边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_THD = 269484234,
		/// <summary>
		/// ch: 关键点模板匹配重要性得分阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MATCH_IMPORTANCE_SCORE_TH = 269484235,
		/// <summary>
		/// ch: 关键点模板建模手动设置最小链长标识参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_MANUAL_CHAIN_LEN_FLAG = 269484236,
		/// <summary>
		/// ch: 建模最短链长阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_PYRAMID_MODEL_MIN_CHAIN_LEN = 269484237,
		/// <summary>
		///  ch: 轮廓模板匹配超时控制参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBPATMATCH_PARAM_PROCESS_CANCEL_FLAG = 269484238,
		/// <summary>
		///  ch: 轮廓模板匹配掩膜建模使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBPATMATCH_PARAM_MASK_MODEL_ENABLE = 269484239,
		/// <summary>
		/// ch: ROI区域比算法要求的最小宽高、模块要求的最小参数值小 | en: The ROI area is smaller than the minimum threshold required by the algorithm and the module.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_ROI_TOO_SMALL = 269500416,
		/// <summary>
		/// ch: 图像能力集太小 | en: Beyond the limits of image ability.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_IMG_ABILITY_TOO_SMALL = 269500417,
		/// <summary>
		/// ch: 字符能力集太小 | en: Beyond the limits of character ability.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_CHARACTER_ABILITY_TOO_SMALL = 269500418,
		/// <summary>
		/// ch: 字库增加处理样本数量错误 | en: Font library failed to increase the number of samples.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_GENERATE_DATA_NUM = 269500419,
		/// <summary>
		/// ch: 字符识别处理缺失字库文件 | en: Character recognition processing failed because of missing font files.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_NO_FONT_FILE = 269500420,
		/// <summary>
		/// ch: 字库训练处理缺失训练样本 | en: Font training failed because of missing training samples.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_NO_TRAIN_DATA = 269500421,
		/// <summary>
		/// ch: 文本检测区域的中心点不在图像内部 | en: The center point of the area to detect text is not inside the image.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_TEXT_DETECT_OUT = 269500422,
		/// <summary>
		/// ch: 文本检测区域的高、宽小于算法支持的最小字符大小 | en: The width or height of the area to detect text is smaller than the minimum character size supported by the algorithm.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_TEXT_BOX_INVALID = 269500423,
		/// <summary>
		/// ch: 字库类型不匹配 | en: Font type does not match.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_FONT_MISMATCH = 269500424,
		/// <summary>
		/// ch: 字库没有与待识别字符类型一致的样本 | en: The font does not have a sample that matches the type of character to be recognized.
		/// </summary>
		MVALG_STS_ERR_MVBOCR_FONT_NO_MATCHING_TYPE = 269500425,
		/// <summary>
		/// ch: 训练字符标签种类数大于上限
		/// </summary>
		MVALG_STS_ERR_MVBOCR_EXCEED_MAX_LABEL_NUM = 269500426,
		/// <summary>
		/// ch: 字符过滤参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_CHARACTER_FIELD = 269500427,
		/// <summary>
		/// ch: 字符分割模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODU_CHARACTER_SEGMENT = 269500428,
		/// <summary>
		/// ch: 字符识别模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODU_CHARACTER_PREDICT = 269500429,
		/// <summary>
		/// ch: 字库训练模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODU_FONT_TRAIN = 269500430,
		/// <summary>
		/// ch: 字库增加模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODU_CHARACTER_GENERATE = 269500431,
		/// <summary>
		/// ch: 字库提取模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODU_FONT_EXTRACT = 269500432,
		/// <summary>
		/// ch: 字符识别模块或者字库训练模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODU_CHARACTER_PREDICT_FONT_TRAIN = 269500433,
		/// <summary>
		/// ch: 字符极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_POLARITY = 269500434,
		/// <summary>
		/// ch: 二值化模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_BINARY_MODE = 269500435,
		/// <summary>
		/// ch: 二值化系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_BINARY_COEF = 269500436,
		/// <summary>
		/// ch: 字符片段最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_FRAGMENT_MIN_AREA = 269500437,
		/// <summary>
		/// ch: 字符片段最大面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_FRAGMENT_MAX_AREA = 269500438,
		/// <summary>
		/// ch: 字符笔画宽度滤波使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_STROKE_WIDTH_FILTER_ENABLE = 269500439,
		/// <summary>
		/// ch: 字符最小笔画宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_MIN_STROKE_WIDTH = 269500440,
		/// <summary>
		/// ch: 字符最大笔画宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_MAX_STROKE_WIDTH = 269500441,
		/// <summary>
		/// ch: 是否忽略边框片段参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_IGNORE_BORDER_FRAGMENT_ENABLE = 269500442,
		/// <summary>
		/// ch: 团块到基线距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_MAIN_LINE_DIST_THRE = 269500443,
		/// <summary>
		/// ch: 主方向搜索范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_ORIENT_HALF_RANGE = 269500444,
		/// <summary>
		/// ch: 倾斜角搜索范围参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_SLANT_HALF_RANGE = 269500445,
		/// <summary>
		/// ch: 投影阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_PROJECT_THRE = 269500446,
		/// <summary>
		/// ch: 是否合并字符间较小间隙参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_GAP_MERGE_ENABLE = 269500447,
		/// <summary>
		/// ch: 字符间最小间隙参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_MIN_INTER_CHARACTER_GAP = 269500448,
		/// <summary>
		/// ch: 字符最小宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_MIN_WIDTH = 269500449,
		/// <summary>
		/// ch: 字符最大宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_MAX_WIDTH = 269500450,
		/// <summary>
		/// ch: 字符最小高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_MIN_HEIGHT = 269500451,
		/// <summary>
		/// ch: 字符最大高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_MAX_HEIGHT = 269500452,
		/// <summary>
		/// ch: 字符box框长宽比最大阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_MAX_LENTH_WIDTH_RATIO = 269500453,
		/// <summary>
		/// ch: 字符的宽度类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_WIDTH_TYPE = 269500454,
		/// <summary>
		/// ch: 文本行间最小间隙参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_MIN_INTER_TEXT_GAP = 269500455,
		/// <summary>
		/// ch: 字符特征参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CHARACTER_FEATURE = 269500456,
		/// <summary>
		/// ch: 字符分类方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_CLASSIFICATION = 269500457,
		/// <summary>
		/// ch: KNN分类算法的k值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_KNN_K_VALUE = 269500458,
		/// <summary>
		/// ch: KNN分类算法的相似度类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_KNN_DISTANCE = 269500459,
		/// <summary>
		/// ch: KNN分类方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_KNN_CLASSES_METHOD = 269500460,
		/// <summary>
		/// ch: SVM分类算法的惩罚因子参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_SVM_C_VALUE = 269500461,
		/// <summary>
		/// ch: 字符识别合格阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_ACCEPT_THRESHOLD = 269500462,
		/// <summary>
		/// ch: 字库增加类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_PARAM_GENERATE_TYPE = 269500463,
		/// <summary>
		/// ch: 字符识别模型版本错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_MODEL_VERSION_ERROR = 269500464,
		/// <summary>
		/// ch: 字符识别算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_ABILITY_ARG_MODU_ENABLE = 269500465,
		/// <summary>
		/// ch: 字符识别算法能力集最大图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_ABILITY_ARG_MAX_IMAGE_SIZE = 269500466,
		/// <summary>
		/// ch: 字符识别算法能力集最大字符宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_ABILITY_ARG_MAX_CHAR_SIZE = 269500467,
		/// <summary>
		/// ch: 字符识别算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_ABILITY_ARG_THREAD_NUM = 269500468,
		/// <summary>
		/// ch: 字符识别算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCR_ABILITY_ARG_THREAD_HANDLE = 269500469,
		/// <summary>
		/// ch: 字符识别算法文本框超出图像范围，非法值
		/// </summary>
		MVALG_STS_ERR_MVBOCR_TEXT_ROI_PARAM_INVALID = 269500470,
		/// <summary>
		/// ch: 输入处理类型与模块使能不一致 | en: Input processing type is inconsistent with the status of ModuleEnable
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PRC_TYPE_OR_MODU_ENABLE = 269516800,
		/// <summary>
		/// ch: 输入box个数小于MVBMKINSP_MARK_NUM_MIN或大于设置的最大个数 | en: The number of boxes input less than MVBMKINSP_MARK_NUM_MIN or greater than the maximum number set
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MARK_NUM = 269516801,
		/// <summary>
		/// ch: 输入字符box大小范围超过图像范围或与之前生成模型使用的mark_box大小不一样 | en: The input character box size range is larger than the image range or different from the mark_box size used in the previous generation model.
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MARK_BOX_SCALE = 269516802,
		/// <summary>
		/// ch: proc_order输入异常 | en: Input exception
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PROC_ORDER = 269516803,
		/// <summary>
		/// ch: 模型输入类型不一致 | en: Model input type is inconsistent.
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODEL_TYPE = 269516804,
		/// <summary>
		/// ch: 模型输入版本不一致 | en: Model input version is inconsistent.
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODEL_VERSION = 269516805,
		/// <summary>
		/// ch: 模型保存的参数异常 | en: Abnormal parameter saved by the model.
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODEL_CONFIG = 269516806,
		/// <summary>
		/// ch: 模型训练个数异常
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODEL_TRAIN_NUM = 269516807,
		/// <summary>
		/// ch: 基于逐点灰度相关计算的模型建立模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_COAR_CORRE_MODEL_CREATE_DISABLE = 269516808,
		/// <summary>
		/// ch: 均值标准差法精细检测模型创建模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_FINE_MEAN_STD_MODEL_CREATE_DISABLE = 269516809,
		/// <summary>
		/// ch: 投影均值标准差法精细检测模型创建模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_PROJ_MEAN_STD_MODEL_CREATE_DISABLE = 269516810,
		/// <summary>
		/// ch: 投影均值标准差法精细检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_PROJ_MEAN_STD_INSP_DISABLE = 269516811,
		/// <summary>
		/// ch: 字宽法精细检测模型创建模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_STROKE_WIDTH_MODEL_CREATE_DISABLE = 269516812,
		/// <summary>
		/// ch: 字宽法精细检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_STROKE_WIDTH_INSP_DISABLE = 269516813,
		/// <summary>
		/// ch: 基于投影灰度相关计算的模型建立模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_PROJCORRE_MOD_DISABLE = 269516814,
		/// <summary>
		/// ch: 基于投影灰度相关计算的缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_PROJCORRE_DISABLE = 269516815,
		/// <summary>
		/// ch: 相关系数法缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_COAR_CORRE_INSP_DISABLE = 269516816,
		/// <summary>
		/// ch: 均值标准差法缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_FINE_MEAN_STD_INSP_DISABLE = 269516817,
		/// <summary>
		/// ch: 模型训练模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_TRAIN_MODEL_DISABLE = 269516818,
		/// <summary>
		/// ch: 字符自动分割模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_MODU_MARK_SEGMENT_DISABLE = 269516819,
		/// <summary>
		/// ch: 投影相关分值阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_PROJCORRE_SCORE_THRESH = 269516820,
		/// <summary>
		/// ch: 比例系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_PROJ_MEANSTD_HIGH_SCALE = 269516821,
		/// <summary>
		/// ch: 偏移系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_PROJ_MEANSTD_HIGH_OFFSET = 269516822,
		/// <summary>
		/// ch: 比例系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_PROJ_MEANSTD_LOW_SCALE = 269516823,
		/// <summary>
		/// ch:  偏移系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_PROJ_MEANSTD_LOW_OFFSET = 269516824,
		/// <summary>
		/// ch: 缺陷类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_PROJ_MEANSTD_FLAW_TYPE = 269516825,
		/// <summary>
		/// ch: 缺陷最小长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_PROJ_MEANSTD_FLAW_MIN_LEN = 269516826,
		/// <summary>
		/// ch: 行方向上的膨胀尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_TEMPL_DILATE_WIDTH = 269516827,
		/// <summary>
		/// ch: 列方向上的膨胀尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_TEMPL_DILATE_HEIGHT = 269516828,
		/// <summary>
		/// ch: 字符进行3x3膨胀处理的次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_DILATE_NUM = 269516829,
		/// <summary>
		/// ch: 缺失检测使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_LACK_FLAW_ENABLE = 269516830,
		/// <summary>
		/// ch: 缺失检测类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_LACK_FLAW_TYPE = 269516831,
		/// <summary>
		/// ch: 缺失检测尺度，参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_LACK_FLAW_SCALE = 269516832,
		/// <summary>
		/// ch: 缺失笔画最小宽度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_LACK_FLAW_MIN_STROKE_WIDTH = 269516833,
		/// <summary>
		/// ch: 冗余检测使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_EXCESS_FLAW_ENABLE = 269516834,
		/// <summary>
		/// ch: 冗余检测尺度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_EXCESS_FLAW_SCALE = 269516835,
		/// <summary>
		/// ch: 冗余笔画最小宽度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_EXCESS_FLAW_MIN_STROKE_WIDTH = 269516836,
		/// <summary>
		/// ch: 是否输出缺陷二值大图参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_STROKE_WIDTH_IS_LARGE_RES = 269516837,
		/// <summary>
		/// ch: 高度方向分块个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_COARCORRE_BLOCK_NUM_HEIGHT = 269516838,
		/// <summary>
		/// ch: 宽度方向分块个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_COARCORRE_BLOCK_NUM_WIDTH = 269516839,
		/// <summary>
		/// ch: 逐点灰度投影分值阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_COARCORRE_SCORE_THRESH = 269516840,
		/// <summary>
		/// ch: 高阈值比例系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_HIGH_SCALE = 269516841,
		/// <summary>
		/// ch: 高阈值偏移系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_HIGH_OFFSET = 269516842,
		/// <summary>
		/// ch: 低阈值比例系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_LOW_SCALE = 269516843,
		/// <summary>
		/// ch: 低阈值偏移系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_LOW_OFFSET = 269516844,
		/// <summary>
		/// ch: 缺陷类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_FLAW_TYPE = 269516845,
		/// <summary>
		/// ch: 是否输出缺陷二值大图参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_IS_LARGE_RES = 269516846,
		/// <summary>
		/// ch: 精细比对弹性开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_TOLERANCE_FLAG = 269516847,
		/// <summary>
		/// ch: 精细比对弹性度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_INSPECT_FINE_MEANSTD_TOLERANCE_VAL = 269516848,
		/// <summary>
		/// ch: 直方图左端比例值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_NORMLIZE_LEFT_CLIP_PERCENT = 269516849,
		/// <summary>
		/// ch: 直方图右端比例值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_NORMLIZE_RIGHT_CLIP_PERCENT = 269516850,
		/// <summary>
		/// ch: 灰度归一化方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_NORMLIZE_TYPE = 269516851,
		/// <summary>
		/// ch: 字符分割最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MIN_AREA = 269516852,
		/// <summary>
		/// ch: 字符分割最小宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MIN_MARK_WIDTH = 269516853,
		/// <summary>
		/// ch: 字符分割最小高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MIN_MARK_HEIGHT = 269516854,
		/// <summary>
		/// ch: 字符分割最大宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MAX_MARK_WIDTH = 269516855,
		/// <summary>
		/// ch: 字符分割最大高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MAX_MARK_HEIGHT = 269516856,
		/// <summary>
		/// ch: 是否合并参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_IS_MERGE = 269516857,
		/// <summary>
		/// ch: 字符分割水平合并距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MERGE_DIST_X = 269516858,
		/// <summary>
		/// ch: 字符分割竖直合并距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_MERGE_DIST_Y = 269516859,
		/// <summary>
		/// ch: 字符分割灰度极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_GRAY_POLARITY = 269516860,
		/// <summary>
		/// ch: 字符分割灰度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_SEGMENT_GRAY_THRESH = 269516861,
		/// <summary>
		/// ch: 字符精定位位置纠正标识参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_CORRECT_FLAG = 269516862,
		/// <summary>
		/// ch: 字符精定位水平容忍大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_TOLERANCE_X = 269516863,
		/// <summary>
		/// ch: 字符精定位竖直容忍大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_TOLERANCE_Y = 269516864,
		/// <summary>
		/// ch: 字符精定位分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_SCORE = 269516865,
		/// <summary>
		/// ch: 字符精定位角度范围起始值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_ANGLE_START = 269516866,
		/// <summary>
		/// ch: 字符精定位角度范围终止值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_ANGLE_END = 269516867,
		/// <summary>
		/// ch: 字符精定位粗搜分数使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_COARSE_FLAG = 269516868,
		/// <summary>
		/// ch: 字符精定位粗搜分数值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_COARSE_VAL = 269516869,
		/// <summary>
		/// ch: 字符精定位阈值模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_THD_FLAG = 269516870,
		/// <summary>
		/// ch: 字符精定位阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBMKINSP_PARAM_MARK_FINE_MATCH_THD_VAL = 269516871,
		/// <summary>
		/// ch: 输入处理类型与模块使能不一致
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_PRC_TYPE_OR_MODU_ENABLE = 269516880,
		/// <summary>
		/// ch: 输入box个数小于MVB2D_MKINSP_MARK_NUM_MIN或不等于设置的最大个数
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MARK_NUM = 269516881,
		/// <summary>
		/// ch: 算法模块输入错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODU_INPUT_ERR = 269516882,
		/// <summary>
		/// ch: 算法模块参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODU_PARAM_ERR = 269516883,
		/// <summary>
		/// ch: 算法模块输出错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODU_OUTPUT_ERR = 269516884,
		/// <summary>
		/// ch: 不支持的模型版本
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODEL_VERSION = 269516885,
		/// <summary>
		/// ch: 错误的模型数据
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODEL_DATA = 269516886,
		/// <summary>
		/// ch: 检测模型未初始化
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODEL_UNINITIALIZED = 269516887,
		/// <summary>
		/// ch: 模型训练个数异常
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODEL_TRAIN_NUM = 269516888,
		/// <summary>
		/// ch: box定位失败导致无法训练
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_MODEL_BOX_LOC_FAIL = 269516889,
		/// <summary>
		/// ch: 掩模图大小与模板图大小不一致
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_INSP_MASK_SIZE_TEMP_SIZE_DIFFERENT = 269516890,
		/// <summary>
		/// ch: 字符分割roi太小或太大
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_TEXT_SEG_ROI_SIZE_ERR = 269516891,
		/// <summary>
		/// ch: 算法句柄错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_ALG_HANDLE_ERR = 269516892,
		/// <summary>
		/// ch: 字符框位置修正超出图像边界
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_CHAR_BOX_FIXPOINT_OUT_IMAGE = 269516893,
		/// <summary>
		/// ch: 字符缺陷检测算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_ABILITY_ARG_MODU_ENABLE = 269516894,
		/// <summary>
		/// ch: 字符缺陷检测算法能力集最大字符宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_ABILITY_ARG_MAX_IMAGE_SIZE = 269516895,
		/// <summary>
		/// ch: 字符缺陷检测算法能力集最大图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_ABILITY_ARG_MAX_MARK_SIZE = 269516896,
		/// <summary>
		/// ch: 字符缺陷检测算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_ABILITY_ARG_THREAD_NUM = 269516897,
		/// <summary>
		/// ch: 字符缺陷检测算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_MKINSP_ABILITY_ARG_THREAD_HANDLE = 269516898,
		/// <summary>
		/// ch: 输入处理类型与模块使能不一致
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PRC_TYPE_OR_MODU_ENABLE = 269520896,
		/// <summary>
		/// ch: 输入轨迹几何元素个数超过范围
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_TRAJ_ELEM_NUM = 269520897,
		/// <summary>
		/// ch: 输入轨迹几何元素类型错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_TRAJ_GEO_ELEM_TYPE = 269520898,
		/// <summary>
		/// ch: 输入轨迹几何元素坐标点错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_TRAJ_GEO_ELEM_PT = 269520899,
		/// <summary>
		/// ch: 输入的基准点和运行点角度越界（弧度制，-2pi~2pi）
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_BASIS_ANGEL_OUT = 269520900,
		/// <summary>
		/// ch: 输入的最大轨迹长度能力集不够
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MAX_TRAJ_LEN = 269520901,
		/// <summary>
		/// ch: 输入的轨迹条数超过最大能力集
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MAX_TRAJ_NUM = 269520902,
		/// <summary>
		/// ch: 输入的模型与当前要检测的类型不一致
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_VISION_CHECKOUT = 269520903,
		/// <summary>
		/// ch: 起始输入分析区域越界或无效
		/// </summary>
		MVALG_STS_ERR_START_INDICATOR_OUT = 269520904,
		/// <summary>
		/// ch: 直线边缘缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_LINE_DEFECT_INSP_DISABLE = 269520905,
		/// <summary>
		/// ch: 圆弧边缘缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_CIRCLE_DEFECT_INSP_DISABLE = 269520906,
		/// <summary>
		/// ch: 边缘位置趋势分析模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_TREND_EDGE_POS_DISABLE = 269520907,
		/// <summary>
		/// ch: 边缘对模型缺陷检测：模型建立模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_EDGE_PAIR_MOD_DISABLE = 269520908,
		/// <summary>
		/// ch: 边缘对模型缺陷检测：执行检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_EDGE_PAIR_FLAW_DISABLE = 269520909,
		/// <summary>
		/// ch: 边缘  模型缺陷检测：模型建立模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_EDGE_POINT_MOD_DISABLE = 269520910,
		/// <summary>
		/// ch: 边缘  模型缺陷检测：执行检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_EDGE_POINT_FLAW_DISABLE = 269520911,
		/// <summary>
		/// ch: 边缘对缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_EDGE_PAIR_FLAW_NO_MOD_DISABLE = 269520912,
		/// <summary>
		/// ch: 直线边缘对缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_LINE_PAIR_DEFECT_INSP_DISABLE = 269520913,
		/// <summary>
		/// ch: 圆弧边缘对缺陷检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_CIRCLE_PAIR_DEFECT_INSP_DISABLE = 269520914,
		/// <summary>
		/// ch: 边缘对位置趋势分析模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_TREND_PAIR_POS_DISABLE = 269520915,
		/// <summary>
		/// ch: 轨迹自动获取模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_AUTO_GET_TRAJ_DISABLE = 269520916,
		/// <summary>
		/// ch: 轨迹引导模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_MODU_TRAJ_POS_GUIDE_DISABLE = 269520917,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_FIND_MODE = 269520918,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_EDGE_POLARITY = 269520919,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_HALF_KERNEL_SIZE = 269520920,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_EDGE_STRENGTH = 269520921,
		/// <summary>
		/// ch: 卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_CALIPER_DIST_TRAJ = 269520922,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_CALIPER_HEIGHT = 269520923,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_CALIPER_WIDTH = 269520924,
		/// <summary>
		/// ch: 缺陷极性判断参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_FLAW_POLARITY_ENABLE = 269520925,
		/// <summary>
		/// ch: 缺陷尺寸使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_SIZE_ENABLE = 269520927,
		/// <summary>
		/// ch: 缺陷面积使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_AREA_ENABLE = 269520928,
		/// <summary>
		/// ch: 凹凸缺陷最小距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_ROUGH_MIN_DIS = 269520929,
		/// <summary>
		/// ch: 凹凸缺陷最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_ROUGH_MIN_SIZE = 269520930,
		/// <summary>
		/// ch: 凹凸缺陷最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_ROUGH_MIN_AREA = 269520931,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_LINE_CALIPER_COUNT = 269520932,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_LINE_FIT_REJECT_NUM = 269520933,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_LINE_FIT_REJECT_DIS = 269520934,
		/// <summary>
		/// ch: 边缘趋势追踪容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_TRACK_DIS_TOL = 269520935,
		/// <summary>
		/// ch: 凹凸缺陷最大距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_ROUGH_MAX_DIS = 269520936,
		/// <summary>
		/// ch: 凹凸缺陷最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_ROUGH_MAX_SIZE = 269520937,
		/// <summary>
		/// ch: 凹凸缺陷最大面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_ROUGH_MAX_AREA = 269520938,
		/// <summary>
		/// ch: 断裂尺寸最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_CRACK_MIN_SIZE = 269520939,
		/// <summary>
		/// ch: 断裂尺寸最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_DEFECT_INSP_CRACK_MAX_SIZE = 269520940,
		/// <summary>
		/// ch: 边缘类型参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_FIND_MODE = 269520941,
		/// <summary>
		/// ch: 边缘极性参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_EDGE_POLARITY = 269520942,
		/// <summary>
		/// ch: 滤波尺寸参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_HALF_KERNEL_SIZE = 269520943,
		/// <summary>
		/// ch: 边缘阈值参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_EDGE_STRENGTH = 269520944,
		/// <summary>
		/// ch: 卡尺间距参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_CALIPER_DIST_TRAJ = 269520945,
		/// <summary>
		/// ch: 卡尺高度参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_CALIPER_HEIGHT = 269520946,
		/// <summary>
		/// ch: 卡尺宽度参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_CALIPER_WIDTH = 269520947,
		/// <summary>
		/// ch: 缺陷极性判断参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_FLAW_POLARITY_ENABLE = 269520948,
		/// <summary>
		/// ch: 缺陷尺寸使能参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_SIZE_ENABLE = 269520949,
		/// <summary>
		/// ch: 缺陷面积使能参数错误参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_AREA_ENABLE = 269520950,
		/// <summary>
		/// ch: 凹凸缺陷最小距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_ROUGH_MIN_DIS = 269520951,
		/// <summary>
		/// ch: 凹凸缺陷最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_ROUGH_MIN_SIZE = 269520952,
		/// <summary>
		/// ch:  凹凸缺陷最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_ROUGH_MIN_AREA = 269520953,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_CIRCLE_CALIPER_NUM = 269520954,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_FIT_REJECT_NUM = 269520955,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_FIT_REJECT_DIST = 269520956,
		/// <summary>
		/// ch: 边缘趋势追踪容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_TRACK_DIS_TOL = 269520957,
		/// <summary>
		/// ch: 凹凸缺陷最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_ROUGH_MAX_SIZE = 269520958,
		/// <summary>
		/// ch: 凹凸缺陷最大距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_ROUGH_MAX_DIS = 269520959,
		/// <summary>
		/// ch: 凹凸缺陷最大面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_ROUGH_MAX_AREA = 269520960,
		/// <summary>
		/// ch: 断裂尺寸最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_CRACK_MIN_SIZE = 269520961,
		/// <summary>
		/// ch: 断裂尺寸最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_DEFECT_INSP_CRACK_MAX_SIZE = 269520962,
		/// <summary>
		/// ch: 边缘对类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_FIND_MODE = 269520963,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_EDGE0_POLARITY = 269520964,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_EDGE1_POLARITY = 269520965,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_HALF_KERNEL_SIZE = 269520966,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_EDGE_STRENGTH = 269520967,
		/// <summary>
		/// ch: 理想边缘宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_IDEA_WIDTH = 269520968,
		/// <summary>
		/// ch: 卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_CALIPER_DIST_TRAJ = 269520969,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_CALIPER_HEIGHT = 269520970,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_CALIPER_WIDTH = 269520971,
		/// <summary>
		/// ch: 缺陷尺寸使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_SIZE_ENABLE = 269520972,
		/// <summary>
		/// ch: 缺陷面积使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_AREA_ENABLE = 269520973,
		/// <summary>
		/// ch: 凹凸缺陷最小距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_ROUGH_MIN_WID = 269520974,
		/// <summary>
		/// ch: 凹凸缺陷最大距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_ROUGH_MAX_WID = 269520975,
		/// <summary>
		/// ch: 凹凸缺陷最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_ROUGH_MIN_SIZE = 269520976,
		/// <summary>
		/// ch:  凹凸缺陷最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_ROUGH_MIN_AREA = 269520977,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_LINE_CALIPER_COUNT = 269520978,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_LINE_FIT_REJECT_NUM = 269520979,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_LINE_FIT_REJECT_DIS = 269520980,
		/// <summary>
		/// ch: 角度容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_LINE_ANGLE_TOL = 269520981,
		/// <summary>
		/// ch: 边缘趋势追踪容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_TRACK_DIS_TOL = 269520982,
		/// <summary>
		/// ch: 凹凸缺陷最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_ROUGH_MAX_SIZE = 269520983,
		/// <summary>
		/// ch: 凹凸缺陷最大面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_ROUGH_MAX_AREA = 269520984,
		/// <summary>
		/// ch: 断裂尺寸最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_CRACK_MIN_SIZE = 269520985,
		/// <summary>
		/// ch: 断裂尺寸最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_LINE_PAIR_DEFECT_INSP_CRACK_MAX_SIZE = 269520986,
		/// <summary>
		/// ch: 边缘对类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_FIND_MODE = 269520987,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_EDGE0_POLARITY = 269520988,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_EDGE1_POLARITY = 269520989,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_HALF_KERNEL_SIZE = 269520990,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_EDGE_STRENGTH = 269520991,
		/// <summary>
		/// ch: 理想边缘宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_IDEA_WIDTH = 269520992,
		/// <summary>
		/// ch: 卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_CALIPER_DIST_TRAJ = 269520993,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_CALIPER_HEIGHT = 269520994,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_CALIPER_WIDTH = 269520995,
		/// <summary>
		/// ch: 缺陷尺寸使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_SIZE_ENABLE = 269520996,
		/// <summary>
		/// ch: 缺陷面积使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_AREA_ENABLE = 269520997,
		/// <summary>
		/// ch: 凹凸缺陷最小距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_ROUGH_MIN_WID = 269520998,
		/// <summary>
		/// ch: 凹凸缺陷最大距离参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_ROUGH_MAX_WID = 269520999,
		/// <summary>
		/// ch: 凹凸缺陷最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_ROUGH_MIN_SIZE = 269521000,
		/// <summary>
		/// ch: 凹凸缺陷最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_ROUGH_MIN_AREA = 269521001,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_FIT_CALIPER_COUNT = 269521002,
		/// <summary>
		/// ch: 剔除点数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_FIT_REJECT_NUM = 269521003,
		/// <summary>
		/// ch: 剔除阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_FIT_REJECT_DIS = 269521004,
		/// <summary>
		/// ch: 角度容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_FIT_CONCENTRIC_TOL = 269521005,
		/// <summary>
		/// ch: 边缘趋势追踪容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_TRACK_DIS_TOL = 269521006,
		/// <summary>
		/// ch: 凹凸缺陷最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_ROUGH_MAX_SIZE = 269521007,
		/// <summary>
		/// ch: 凹凸缺陷最大面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_ROUGH_MAX_AREA = 269521008,
		/// <summary>
		/// ch: 断裂尺寸最小尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_CRACK_MIN_SIZE = 269521009,
		/// <summary>
		/// ch: 断裂尺寸最大尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCLE_PAIR_DEFECT_INSP_CRACK_MAX_SIZE = 269521010,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_FIND_MODE = 269521011,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_EDGE_POLARITY = 269521012,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_HALF_KERNEL_SIZE = 269521013,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_EDGE_STRENGTH = 269521014,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_CALIPER_COUNT = 269521015,
		/// <summary>
		/// ch: 边缘查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_DETECT_ORIENT = 269521016,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_PROJECTION_LENGTH = 269521017,
		/// <summary>
		/// ch: 距离低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_DIST_LOW = 269521018,
		/// <summary>
		/// ch: 距离高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_EDGE_DIST_HIGH = 269521019,
		/// <summary>
		/// ch: 边缘对类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_FIND_MODE = 269521020,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_PAIR_POLARITY0 = 269521021,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_PAIR_POLARITY1 = 269521022,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_HALF_KERNEL_SIZE = 269521023,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_PAIR_STRENGTH = 269521024,
		/// <summary>
		/// ch: 理想边缘宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_PAIR_IDEA_WID = 269521025,
		/// <summary>
		/// ch: 卡尺数量参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_CALIPER_COUNT = 269521026,
		/// <summary>
		/// ch: 边缘查找方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_DETECT_ORIENT = 269521027,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_PROJECTION_LENGTH = 269521028,
		/// <summary>
		/// ch: 距离低阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_DIST_LOW = 269521029,
		/// <summary>
		/// ch: 距离高阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TREND_PAIR_DIST_HIGH = 269521030,
		/// <summary>
		/// ch: 边缘对理想宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_EDGE_PAIR_IDEAL_WIDTH = 269521031,
		/// <summary>
		/// ch: 是否进行边缘精细检测参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_FINE_EDGE_ENABLE = 269521032,
		/// <summary>
		/// ch: 卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_CALIPER_DIST_COURSE = 269521033,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_CALIPER_WIDTH = 269521034,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_CALIPER_HEIGHT = 269521035,
		/// <summary>
		/// ch: 边缘对类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_EDGE_PAIR_FIND_MODE = 269521036,
		/// <summary>
		/// ch: 边缘对边缘点0边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_EDGE_PAIR_EDGE0_POLARITY = 269521037,
		/// <summary>
		/// ch: 边缘对边缘点1边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_EDGE_PAIR_EDGE1_POLARITY = 269521038,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_EDGE_STRENGTH = 269521039,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_HALF_KERNENL_SIZE = 269521040,
		/// <summary>
		/// ch: 最大卡尺个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_MAX_CALIPER_NUM = 269521041,
		/// <summary>
		/// ch: 卡尺检测方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_CALIPER_DETECT_ORIENT = 269521042,
		/// <summary>
		/// ch: 精细检测胶体边缘时放置卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_CALIPER_DIST_FINE = 269521043,
		/// <summary>
		/// ch: 是否修正错误的边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_MODIFY_WRONG_PTS = 269521044,
		/// <summary>
		/// ch: 边缘点修正时连续拟合边缘点个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_MODIFY_PTS_NUM = 269521045,
		/// <summary>
		/// ch: 边缘点修正距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_MODIFY_THRESH = 269521046,
		/// <summary>
		/// ch: 边缘点修正时拐点距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_MODIFY_BREAK_PT_DIST = 269521047,
		/// <summary>
		/// ch: 轨迹多边形折弯处拟合长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_POLY_CURVE_LEN = 269521048,
		/// <summary>
		/// ch: 是否追踪断裂处边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_MOD_TRACK_ENABLE = 269521049,
		/// <summary>
		/// ch: 最大缺陷个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_MAX_FLAW_NUM = 269521050,
		/// <summary>
		/// ch: 边缘对理想宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_EDGE_PAIR_IDEAL_WIDTH = 269521051,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_CALIPER_WIDTH = 269521052,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_CALIPER_HEIGHT = 269521053,
		/// <summary>
		/// ch: 边缘对类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_EDGE_PAIR_FIND_MODE = 269521054,
		/// <summary>
		/// ch: 边缘对边缘点0边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_EDGE_PAIR_EDGE0_POLARITY = 269521055,
		/// <summary>
		/// ch: 边缘对边缘点1边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_EDGE_PAIR_EDGE1_POLARITY = 269521056,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_EDGE_STRENGTH = 269521057,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_HALF_KERNENL_SIZE = 269521058,
		/// <summary>
		/// ch: 是否检测宽度缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_WIDTH_ENABLE = 269521059,
		/// <summary>
		/// ch: 最小胶体宽度百分比参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_WIDTH_LOW_OFFSET = 269521060,
		/// <summary>
		/// ch: 最大胶体宽度百分比参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_WIDTH_HIGH_OFFSET = 269521061,
		/// <summary>
		/// ch: 位置偏移使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_OFFSET_ENABLE = 269521062,
		/// <summary>
		/// ch: 位置偏移阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_OFFSET_THRESH = 269521063,
		/// <summary>
		/// ch: 断裂缺陷使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_CRACK_ENABLE = 269521064,
		/// <summary>
		/// ch: 缺陷长度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_LEN_THRESH = 269521065,
		/// <summary>
		/// ch: 是否追踪断裂处边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_TRACK_ENABLE = 269521066,
		/// <summary>
		/// ch: 阶梯缺陷使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_GRAD_ENABLE = 269521067,
		/// <summary>
		/// ch: 阶梯偏离高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_GRAD_THRESH = 269521068,
		/// <summary>
		/// ch: 最小阶梯长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_GRAD_LEN = 269521069,
		/// <summary>
		/// ch: 灰度辅助检测参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_GRAY_TRACK_ENABLE = 269521070,
		/// <summary>
		/// ch: 气泡缺陷检测使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_BUBBLE_ENABLE = 269521071,
		/// <summary>
		/// ch: 气泡灰度值下限比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_BUBBLE_LOW_OFFSET = 269521072,
		/// <summary>
		/// ch: 气泡灰度值上限比例参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_BUBBLE_HIGH_OFFSET = 269521073,
		/// <summary>
		/// ch: 气泡突变阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_BUBBLE_CHANGE_THRESH = 269521074,
		/// <summary>
		/// ch: 气泡缺陷长度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_BUBBLE_LEN = 269521075,
		/// <summary>
		/// ch: 突变最大次数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_BUBBLE_GRAY_CHANGE_NUM = 269521076,
		/// <summary>
		/// ch: 偏离容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_GRAY_TRACK_DIS_TOL = 269521077,
		/// <summary>
		/// ch: 是否修正错误的边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_MODIFY_WRONG_PTS = 269521078,
		/// <summary>
		/// ch: 边缘点修正时连续拟合边缘点个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_MODIFY_PTS_NUM = 269521079,
		/// <summary>
		/// ch: 边缘点修正距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_MODIFY_THRESH = 269521080,
		/// <summary>
		/// ch: 边缘点修正时拐点距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_MODIFY_BREAK_PT_DIST = 269521081,
		/// <summary>
		/// ch: 最小胶体宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_WIDTH_LOW = 269521082,
		/// <summary>
		/// ch: 最大胶体宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_WIDTH_HIGH = 269521083,
		/// <summary>
		/// ch: 是否检测凹凸缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_ROUGH_ENABLE = 269521084,
		/// <summary>
		/// ch: 缺陷面积阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_AREA_THRESH = 269521085,
		/// <summary>
		/// ch: 断裂缺陷阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_CRACK_THRESH = 269521086,
		/// <summary>
		/// ch: 最大卡尺个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_MAX_CALIPER_NUM = 269521087,
		/// <summary>
		/// ch: 放置卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_CALIPER_DIST = 269521088,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_CALIPER_WIDTH = 269521089,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_CALIPER_HEIGHT = 269521090,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_EDGE_POINT_FIND_MODE = 269521091,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_EDGE_POINT_POLARITY = 269521092,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_EDGE_STRENGTH = 269521093,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_HALF_KERNENL_SIZE = 269521094,
		/// <summary>
		/// ch: 是否进行边缘精细检测参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_FINE_EDGE_ENABLE = 269521095,
		/// <summary>
		/// ch: 卡尺检测方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_CALIPER_DETECT_ORIENT = 269521096,
		/// <summary>
		/// ch: 是否修正错误的边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_MODIFY_WRONG_PTS = 269521097,
		/// <summary>
		/// ch: 边缘点修正时连续拟合边缘点个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_MODIFY_PTS_NUM = 269521098,
		/// <summary>
		/// ch: 边缘点修正距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_MODIFY_THRESH = 269521099,
		/// <summary>
		/// ch: 边缘点修正时拐点距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_MODIFY_BREAK_PT_DIST = 269521100,
		/// <summary>
		/// ch: 轨迹多边形折弯处拟合长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_MOD_POLY_CURVE_LEN = 269521101,
		/// <summary>
		/// ch: 最大缺陷个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_MAX_FLAW_NUM = 269521102,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_CALIPER_WIDTH = 269521103,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_CALIPER_HEIGHT = 269521104,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_EDGE_FIND_MODE = 269521105,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_EDGE_POINT_POLARITY = 269521106,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_EDGE_STRENGTH = 269521107,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_HALF_KERNENL_SIZE = 269521108,
		/// <summary>
		/// ch: 是否检测位置缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_OFFSET_ENABLE = 269521109,
		/// <summary>
		/// ch: 是否检测断裂缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_CRACK_ENABLE = 269521110,
		/// <summary>
		/// ch: 是否检测边缘阶梯差缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_GRAD_ENABLE = 269521111,
		/// <summary>
		/// ch: 缺陷长度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_LEN_THRESH = 269521112,
		/// <summary>
		/// ch: 位置偏移阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_OFFSET_THRESH = 269521113,
		/// <summary>
		/// ch: 阶梯偏离高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_GRAD_THRESH = 269521114,
		/// <summary>
		/// ch: 最小阶梯长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_GRAD_LEN = 269521115,
		/// <summary>
		/// ch: 灰度辅助检测参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_GRAY_TRACK_ENABLE = 269521116,
		/// <summary>
		/// ch: 偏离容忍度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_GRAY_TRACK_DIS_TOL = 269521117,
		/// <summary>
		/// ch: 是否修正错误的边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_MODIFY_WRONG_PTS = 269521118,
		/// <summary>
		/// ch: 边缘点修正时连续拟合边缘点个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_MODIFY_PTS_NUM = 269521119,
		/// <summary>
		/// ch: 边缘点修正距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_MODIFY_THRESH = 269521120,
		/// <summary>
		/// ch: 边缘点修正时拐点距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_MODIFY_BREAK_PT_DIST = 269521121,
		/// <summary>
		/// ch: 是否检测凹凸缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_ROUGH_ENABLE = 269521122,
		/// <summary>
		/// ch: 断裂长度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_CRACK_THRESH = 269521123,
		/// <summary>
		/// ch: 缺陷面积阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_AREA_THRESH = 269521124,
		/// <summary>
		/// ch: 边缘缺陷位置参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_POS = 269521125,
		/// <summary>
		/// ch: 最大卡尺个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_MAX_CALIPER_NUM = 269521126,
		/// <summary>
		/// ch: 卡尺检测方向参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_DETECT_ORIENT = 269521127,
		/// <summary>
		/// ch: 边缘对理想宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_EDGE_PAIR_IDEAL_WIDTH = 269521128,
		/// <summary>
		/// ch: 是否进行边缘精细检测参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_FINE_EDGE_ENABLE = 269521129,
		/// <summary>
		/// ch: 粗略检测胶体边缘时放置卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_DIST_COURSE = 269521130,
		/// <summary>
		/// ch: 精细检测胶体边缘时放置卡尺间距参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_DIST_FINE = 269521131,
		/// <summary>
		/// ch: 粗略检测胶体边缘时卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_WIDTH = 269521132,
		/// <summary>
		/// ch: 粗略检测胶体边缘时卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_HEIGHT = 269521133,
		/// <summary>
		/// ch: 精细检测胶体边缘时卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_WIDTH_FINE = 269521134,
		/// <summary>
		/// ch: 精细检测胶体边缘时卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CALIPER_HEIGHT_FINE = 269521135,
		/// <summary>
		/// ch: 边缘对类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_EDGE_PAIR_FIND_MODE = 269521136,
		/// <summary>
		/// ch: 边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_EDGE_PAIR_EDGE0_POLARITY = 269521137,
		/// <summary>
		/// ch: 边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_EDGE_PAIR_EDGE1_POLARITY = 269521138,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_EDGE_STRENGTH = 269521139,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_HALF_KERNENL_SIZE = 269521140,
		/// <summary>
		/// ch: 是否修正错误的边缘点参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_MODIFY_WRONG_PTS = 269521141,
		/// <summary>
		/// ch: 边缘点修正时连续拟合边缘点个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_MODIFY_PTS_NUM = 269521142,
		/// <summary>
		/// ch: 边缘点修正距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_MODIFY_THRESH = 269521143,
		/// <summary>
		/// ch: 边缘点修正时拐点距离阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_MODIFY_BREAK_PT_DIST = 269521144,
		/// <summary>
		/// ch: 轨迹多边形折弯处拟合长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_POLY_CURVE_LEN = 269521145,
		/// <summary>
		/// ch: 最大缺陷个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_MAX_FLAW_NUM = 269521146,
		/// <summary>
		/// ch: 是否检测宽度缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_WIDTH_ENABLE = 269521147,
		/// <summary>
		/// ch: 最小胶体宽度（无范围限制）参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_WIDTH_LOW = 269521148,
		/// <summary>
		/// ch: 最大胶体宽度（无范围限制）参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_WIDTH_HIGH = 269521149,
		/// <summary>
		/// ch: 是否检测断裂缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CRACK_ENABLE = 269521150,
		/// <summary>
		/// ch: 是否检测凹凸缺陷参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_ROUGH_ENABLE = 269521151,
		/// <summary>
		/// ch: 缺陷长度阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_LEN_THRESH = 269521152,
		/// <summary>
		/// ch: 缺陷面积阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_AREA_THRESH = 269521153,
		/// <summary>
		/// ch: 断裂缺陷阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_PAIR_FLAW_NO_MOD_CRACK_THRESH = 269521154,
		/// <summary>
		/// ch: 参数自动化设置使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_AUTO_CFG_ENABLE = 269521155,
		/// <summary>
		/// ch: 起始点边缘0极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_START_EDGE0_POLARITY = 269521156,
		/// <summary>
		/// ch: 起始点边缘1极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_START_EDGE1_POLARITY = 269521157,
		/// <summary>
		/// ch: 起始点搜索边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_START_EDGE_STRENGTH = 269521158,
		/// <summary>
		/// ch: 起始点搜索宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_START_IDEA_WIDHT = 269521159,
		/// <summary>
		/// ch: 最大偏差尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_MAX_WIDTH_BIAS = 269521160,
		/// <summary>
		/// ch: 平滑系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_AUTO_GET_TRAJ_SMO_COEF = 269521161,
		/// <summary>
		/// ch: 卡尺宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TRAJ_POS_GUIDE_CALIPER_WIDTH = 269521162,
		/// <summary>
		/// ch: 卡尺高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TRAJ_POS_GUIDE_CALIPER_HEIGHT = 269521163,
		/// <summary>
		/// ch: 边缘类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TRAJ_POS_GUIDE_EDGE_FIND_MODE = 269521164,
		/// <summary>
		/// ch: 边缘极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TRAJ_POS_GUIDE_EDGE_POINT_POLARITY = 269521165,
		/// <summary>
		/// ch: 边缘阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TRAJ_POS_GUIDE_EDGE_STRENGTH = 269521166,
		/// <summary>
		/// ch: 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_TRAJ_POS_GUIDE_HALF_KERNENL_SIZE = 269521167,
		/// <summary>
		/// ch: 边缘点模型缺陷极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_EDGE_POINT_FLAW_POLARITY_TYPE = 269521168,
		/// <summary>
		/// ch: 圆拟合失败
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_PARAM_CIRCEL_FIT_FAIL = 269521169,
		/// <summary>
		/// ch: 算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_MODU_ENABLE = 269521170,
		/// <summary>
		/// ch: 算法能力集最大图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_MAX_IMAGE_SIZE = 269521171,
		/// <summary>
		/// ch: 算法能力集最大卡尺大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_MAX_CALIPER_SIZE = 269521172,
		/// <summary>
		/// ch: 算法能力集最大轨迹长度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_MAX_TRAJ_LEN = 269521173,
		/// <summary>
		/// ch: 算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_THREAD_NUM = 269521174,
		/// <summary>
		/// ch: 算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_THREAD_HANDLE = 269521175,
		/// <summary>
		/// ch: 输入的模型异常
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_INPUT_MODEL_ERROR = 269521176,
		/// <summary>
		/// ch: 边缘缺陷检测算法能力集roi使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_ABILITY_ARG_ROI_ENABLE = 269521177,
		/// <summary>
		/// ch: 边缘缺陷检测算法算法库标识错误
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_SPEC_ID_ERROR = 269521178,
		/// <summary>
		/// ch: 卡尺宽高参数不合理
		/// </summary>
		MVALG_STS_ERR_MVBEDGEINSP_CALIPER_ROI_SIZE = 269521179,
		/// <summary>
		/// ch: 无有效轨迹点输出（MVBEDGEINSP_MODU_TRAJ_POS_GUIDE）
		/// </summary>
		MVD_ALG_STS_ERR_MVBEDGEINSP_TRAJ_POS_GUIDE_WRONG_POINTS = 269521180,
		/// <summary>
		/// ch: 基准圆异常（输入圆不合理或拟合参数不合理或拟合失败）
		/// </summary>
		MVD_ALG_STS_ERR_MVBEDGEINSP_BASIC_CIRCLE_ERROR = 269521181,
		/// <summary>
		/// ch: 基准轮廓异常（输入轮廓不合理或拟合参数不合理或拟合失败）
		/// </summary>
		MVD_ALG_STS_ERR_MVBEDGEINSP_BASIC_CONTOUR_ERROR = 269521182,
		/// <summary>
		/// ch: 角点提取结果异常（角点数较少，需调节参数）
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PAT_SIZE_NOT_ENOUGH = 269529088,
		/// <summary>
		/// ch: 标定矩阵计算异常（角点数较少，需调节参数）
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_HOMO_CAL_WRONG = 269529089,
		/// <summary>
		/// ch: 拼接模型创建失败（生成的矩阵异常）
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODEL_CREATE_FAILED = 269529090,
		/// <summary>
		/// ch: 图像拼接失败(可能为图像有较大倾斜或其他原因)
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_IMG_STITCH_FAILED = 269529091,
		/// <summary>
		/// ch: 图像模型不匹配
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODEL_NOT_MATCH = 269529092,
		/// <summary>
		/// ch: 矩阵计算异常，需要修改当前模块参数
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_HOMO_WRONG = 269529344,
		/// <summary>
		/// ch: 提取自研标定板角点信息模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODU_CALIBBOARD_LOC_DISABLE = 269529093,
		/// <summary>
		/// ch: 图像拼接建模模块未使能数
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODU_STITCH_MODEL_CREATE_CALIBBOARD_DISABLE = 269529094,
		/// <summary>
		/// ch: 图像拼接建模模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODU_STITCH_MODEL_CREATE_CORNERS_DISABLE = 269529095,
		/// <summary>
		/// ch: 图像拼接模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODU_IMG_STITCH_DISABLE = 269529096,
		/// <summary>
		/// ch: 图像拼接建模模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODU_STITCH_MODEL_CREATE_DISABLE = 269529097,
		/// <summary>
		/// ch: 提取自研标定板角点信息模块或者图像拼接建模模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_MODU_STITCH_CALIBBOARD_LOC_MODEL_CREATE_DISABLE = 269529098,
		/// <summary>
		/// ch: 拼接类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_IMG_STITCH_MODEL_TYPE = 269529099,
		/// <summary>
		/// ch: 亚像素窗口大小自适应标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_CHECKERBOARD_SUBPIXEL_WIN_AUTO = 269529100,
		/// <summary>
		/// ch: 亚像素窗口大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_CHECKERBOARD_SUBPIXEL_WIN = 269529101,
		/// <summary>
		/// ch: 黑白模块灰度对比度最小值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_CHECKERBOARD_GRAY_CONTRAST = 269529102,
		/// <summary>
		/// ch: 中值滤波状态参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_CHECKERBOARD_FILT_STS = 269529103,
		/// <summary>
		/// ch: 融合参数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_IMG_STITCH_MERGE_TYPE = 269529104,
		/// <summary>
		/// ch: 拼接图像裁剪参数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_PARAM_IMG_STITCH_CUT_RATIO = 269529105,
		/// <summary>
		/// ch: 图像拼接算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_ABILITY_ARG_MODU_ENABLE = 269529106,
		/// <summary>
		/// ch: 图像拼接算法能力集图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_ABILITY_ARG_MAX_IMAGE_SIZE = 269529107,
		/// <summary>
		/// ch: 图像拼接算法能力集最大图像个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_ABILITY_ARG_MAX_IMAGE_NUM = 269529108,
		/// <summary>
		/// ch: 图像拼接算法能力集最大标定板宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_ABILITY_ARG_MAX_PAT_SIZE = 269529109,
		/// <summary>
		/// ch: 图像拼接算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_ABILITY_ARG_THREAD_NUM = 269529110,
		/// <summary>
		/// ch: 图像拼接算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSTITCH_ABILITY_ARG_THREAD_HANDLE = 269529111,
		/// <summary>
		/// ch: 图像拼接算法模型优化方法错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBSTITCH_PARAM_STITCH_MODEL_OPT_METHOD = 269529112,
		/// <summary>
		/// ch: 图像拼接算法模型优化采样数据错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBSTITCH_PARAM_STITCH_MODEL_OPT_SAMPLE_NUM = 269529113,
		/// <summary>
		/// ch: 图像拼接算法能力优化偏移错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBSTITCH_PARAM_STITCH_MODEL_OPT_OFFSET = 269529114,
		/// <summary>
		/// ch: 特征模型训练模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_MODU_FEATURE_MODEL_TRAIN_DISABLE = 269549568,
		/// <summary>
		/// ch: 基于模型的检验模块未使能
		/// </summary> 
		MVALG_STS_ERR_MVBINSPECT_MODU_FEATURE_MODEL_INSPECT_DISABLE = 269549569,
		/// <summary>
		/// ch: 自动计算单元尺寸的标记参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_AUTO_BLOCK = 269549570,
		/// <summary>
		/// ch: 单元尺寸宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_BLOCK_NUM = 269549571,
		/// <summary>
		/// ch: 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_SCORE = 269549572,
		/// <summary>
		/// ch: 特征使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_FEATURE_ENABLE = 269549573,
		/// <summary>
		/// ch: 是否需要模型精简参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_MODEL_REFINE = 269549574,
		/// <summary>
		/// ch: 特征模型错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_FEATURE_MODEL = 269549575,
		/// <summary>
		/// ch: 标签越界
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_LABELS_OUT_RANGE = 269549576,
		/// <summary>
		/// ch: 模型类型错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_MODEL_TYPE = 269549577,
		/// <summary>
		/// ch: 模型大小错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_MODEL_SIZE = 269549578,
		/// <summary>
		/// ch: ROI参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_BOX_PARAM_WRONG = 269549579,
		/// <summary>
		/// ch: 注册的样本数量越界
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_REGED_SMP_OUT_RANGE = 269549580,
		/// <summary>
		/// ch: 模型版本错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_MODEL_VERSION = 269549581,
		/// <summary>
		/// ch: 降采样参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_DOWNSAMPLE_RATE = 269549582,
		/// <summary>
		/// ch: 训练与检测的降采样倍率不匹配
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_DOWNSAMPLE_RATE_MISMATCH = 269549583,
		/// <summary>
		/// ch: 训练的图像尺寸超出能力集
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_INPUT_PIC_SIZE_MISMATCH = 269549584,
		/// <summary>
		/// ch: 比对缺陷检测算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_ABILITY_ARG_MODU_ENABLE = 269549585,
		/// <summary>
		/// ch: 比对缺陷检测算法能力集最大图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_ABILITY_ARG_MAX_IMAGE_SIZE = 269549586,
		/// <summary>
		/// ch: 比对缺陷检测算法能力集最大模板大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_ABILITY_ARG_MAX_TEMP_SIZE = 269549587,
		/// <summary>
		/// ch: 比对缺陷检测算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_ABILITY_ARG_THREAD_NUM = 269549588,
		/// <summary>
		/// ch: 比对缺陷检测算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_ABILITY_ARG_THREAD_HANDLE = 269549589,
		/// <summary>
		/// ch: 比对缺陷检测算法能力集训练样本个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBINSPECT_ABILITY_ARG_SAMPLE_NUM = 269549590,
		/// <summary>
		/// ch: 边缘过滤使能参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_OUT_FILTER_ENABLE = 269549591,
		/// <summary>
		/// ch: 边缘过滤比例参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_OUT_FILTER_RATE = 269549592,
		/// <summary>
		/// ch: 背景噪声抑制阈值参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_RESTRAIN_THOLD = 269549593,
		/// <summary>
		/// ch: 背景噪声抑制系数参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_RESTRAIN_RATIO = 269549594,
		/// <summary>
		/// ch: 背景噪声抑制类型参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBINSPECT_PARAM_FEATURE_INSPECT_RESTRAIN_TTYPE = 269549595,
		/// <summary>
		/// ch: 直线检测失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_LINE_DETECT_ERR = 269545473,
		/// <summary>
		/// ch: 矩阵计算失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_HOMO_WRONG = 269545474,
		/// <summary>
		/// ch: 露白检测CNN分类错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_WB_CNN_CLASSIFY = 269545475,
		/// <summary>
		/// ch: 边缘检测结果太少
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_EDGE_COUNT_NOT_ENOUGH = 269545476,
		/// <summary>
		/// ch: 定位出的水平栅数量过少
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_COUNT_NOT_ENOUGH = 269545477,
		/// <summary>
		/// ch: 定位出的竖直边缘数量过少
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_VER_COUNT_NOT_ENOUGH = 269545478,
		/// <summary>
		/// ch: 竖直边缘或边缘对定位失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_VER_EDGE_DETECT_ERR = 269545479,
		/// <summary>
		/// ch: 定位到的拟合点过少
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_POINT_LOC_NUM_ERR = 269545480,
		/// <summary>
		/// ch: 多边形建模回调函数类型错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CMP_FUNC_TYPE = 269545481,
		/// <summary>
		/// ch: 多边形模板匹配建模错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CMP_CREATE_MODEL = 269545482,
		/// <summary>
		/// ch: 多边形建模使能标志位错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CMP_ENABLE = 269545483,
		/// <summary>
		/// ch: 主栅模板匹配建模错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_IMG_CREATE_MODEL = 269545484,
		/// <summary>
		/// ch: 建模模块回调函数个数错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CREATE_MODEL_CFG_NUM = 269545485,
		/// <summary>
		/// ch: 建模模块回调函数类型错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CREATE_MODEL_FUNC_TYPE = 269545486,
		/// <summary>
		/// ch: 建模模块使能标志位错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CREATE_MODEL_ENABLE = 269545487,
		/// <summary>
		/// ch: 建模模块基准点错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CREATE_MODEL_FIX_POINT = 269545488,
		/// <summary>
		/// ch: 建模模块ROI设置错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CREATE_MODEL_ROI_RECT = 269545489,
		/// <summary>
		/// ch: 定位模块回调函数类型错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_FUNC_TYPE = 269545490,
		/// <summary>
		/// ch: 定位模块使能标志位错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_ENABLE = 269545491,
		/// <summary>
		/// ch: 定位模块输入模型数据为空
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_MODEL_NULL = 269545492,
		/// <summary>
		/// ch: 定位模块模型大小错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_MODE_SIZE_ERR = 269545493,
		/// <summary>
		/// ch: 定位模块深度学习定位失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_DP_ERR = 269545494,
		/// <summary>
		/// ch: 定位模块片模板匹配失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_CELL_MATCH_ERR = 269545495,
		/// <summary>
		/// ch: 定位模块栅模板匹配失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_GRID_MATCH_ERR = 269545496,
		/// <summary>
		/// ch: 正面目标检测CNN错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_DETECT_CNN = 269545497,
		/// <summary>
		/// ch: 定位模块片模板匹配没匹配到
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_TEMPLET_LOC_CELL_MATCH_NONE = 269545498,
		/// <summary>
		/// ch: 正面崩边CNN分类错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CRACK_CNN_CLASSIFY = 269545499,
		/// <summary>
		/// ch: 串定位模块输入ROI错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_STRING_LOC_ROI = 269545500,
		/// <summary>
		/// ch: 串定位模块输入ROI使能输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_STRING_LOC_ROI_ENABLE = 269545501,
		/// <summary>
		/// ch: 片定位模块输入ROI错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_PIECE_LOC_ROI = 269545502,
		/// <summary>
		/// ch: 栅定位模块输入ROI错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_LOC_ROI = 269545503,
		/// <summary>
		/// ch: 栅定位模块电池片数输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_LOC_CELL_NUM = 269545504,
		/// <summary>
		/// ch: 栅定位模块主栅数输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_LOC_GRID_NUM = 269545505,
		/// <summary>
		/// ch: 片、栅定位模块并片类型输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_LOC_CELL_MERGE_TYPE = 269545506,
		/// <summary>
		/// ch: 片、栅定位模块检测类型(正面或反面)输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_LOC_INSPECT_TYPE = 269545507,
		/// <summary>
		/// ch: 串定位模块边缘查找输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_LOC_STRING_EDGE_STRENGTH = 269545508,
		/// <summary>
		/// ch: 串定位模块边缘查找理想宽度输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_LOC_STRING_IDEA_WIDTH = 269545509,
		/// <summary>
		/// ch: 正面片定位模块电池片数输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_PI_LOC_CELL_NUM = 269545510,
		/// <summary>
		/// ch: 背面片定位模块电池片数输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_NI_LOC_CELL_NUM = 269545511,
		/// <summary>
		/// ch: 正面栅定位模块栅个数输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_PI_LOC_GRID_NUM = 269545512,
		/// <summary>
		/// ch: 背面栅定位模块栅个数输入错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_NI_LOC_GRID_NUM = 269545513,
		/// <summary>
		/// ch: 直线拟合失败
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_LINE_FIT_FAIL = 269545514,
		/// <summary>
		/// ch: 光伏行业算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_ABILITY_ARG_MODU_ENABLE = 269545515,
		/// <summary>
		/// ch: 光伏行业算法能力集图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_ABILITY_ARG_MAX_IMAGE_SIZE = 269545516,
		/// <summary>
		/// ch: 光伏行业算法能力集最大图像个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_ABILITY_ARG_MAX_IMAGE_NUM = 269545517,
		/// <summary>
		/// ch: 光伏行业算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_ABILITY_ARG_THREAD_NUM = 269545518,
		/// <summary>
		/// ch: 光伏行业算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_ABILITY_ARG_THREAD_HANDLE = 269545519,
		/// <summary>
		/// ch: 主栅定位错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_MAIN_GRID_LOC = 269545520,
		/// <summary>
		/// ch: 片尺寸测量错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_CELL_SIZE_MEASURE = MVALG_STS_ERR_MVBSOLARCELL_ABILITY_ARG_THREAD_HANDLE,
		/// <summary>
		/// ch: 片初定位错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_COARSE_CELL_LOC = 269545522,
		/// <summary>
		/// ch: 间距检测直线拟合错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GAP_LINE_FIT_LOC = 269545523,
		/// <summary>
		/// ch: 间距检测直线检测错误
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GAP_LINE_DETECT_LOC = 269545524,
		/// <summary>
		/// ch: 传统焊带偏移输入存在空指针
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_BIAS_NULL_PTR = 269545536,
		/// <summary>
		/// ch: 传统焊带偏移输入片box参数异常
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_BIAS_CELL_BOX = 269545537,
		/// <summary>
		/// ch: 传统焊带偏移输入栅直线异常
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_BIAS_GRID_LINE = 269545538,
		/// <summary>
		/// ch: 传统焊带偏移输入栅数量异常
		/// </summary>
		MVALG_STS_ERR_MVBSOLARCELL_GRID_BIAS_GRID_NUM = 269545539,
		/// <summary>
		/// ch: 二维码评级模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_MODU_VERIFY_ISO_15415_DISABLE = 269537280,
		/// <summary>
		/// ch: 一维码评级模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_MODU_VERIFY_ISO_15416_DISABLE = 269537281,
		/// <summary>
		/// ch: 码极性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_POLARITY = 269537282,
		/// <summary>
		/// ch: 码列数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_SYMBOL_COLS = 269537283,
		/// <summary>
		/// ch: 码行数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_SYMBOL_ROWS = 269537284,
		/// <summary>
		/// ch: 最小模块大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_MIN_CELL_SIZE = 269537285,
		/// <summary>
		/// ch: aperture大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_APERTURE = 269537286,
		/// <summary>
		/// ch: 镜像模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_MIRROR_MODE = 269537287,
		/// <summary>
		/// ch: 标准aperture大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_STD_APERTURE = 269537288,
		/// <summary>
		/// ch: 成像系统真实放大率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_SYS_MAG = 269537289,
		/// <summary>
		/// ch: 码边缘特性参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_ISO_15415_EDGE_FEATURE = 269537290,
		/// <summary>
		/// ch: 计算总体等级是否考虑dec等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DEC_FLAG = 269537291,
		/// <summary>
		/// ch: 计算总体等级是否考虑EDGE等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_EDGE_FLAG = 269537292,
		/// <summary>
		/// ch: 计算总体等级是否考虑SC等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_SC_FLAG = 269537293,
		/// <summary>
		/// ch: 计算总体等级是否考虑MINR等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINR_FLAG = 269537294,
		/// <summary>
		/// ch: 计算总体等级是否考虑MINE等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINE_FLAG = 269537295,
		/// <summary>
		/// ch: 计算总体等级是否考虑MOD等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MOD_FLAG = 269537296,
		/// <summary>
		/// ch: 计算总体等级是否考虑DCD等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DCD_FLAG = 269537297,
		/// <summary>
		/// ch: 计算总体等级是否考虑DEF等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DEF_FLAG = 269537298,
		/// <summary>
		/// ch: 计算总体等级是否考虑QZ等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_QZ_FLAG = 269537299,
		/// <summary>
		/// ch: decodability 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DCD_THR_A = 269537300,
		/// <summary>
		/// ch: decodability 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DCD_THR_B = 269537301,
		/// <summary>
		/// ch: decodability 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DCD_THR_C = 269537302,
		/// <summary>
		/// ch: decodability 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DCD_THR_D = 269537303,
		/// <summary>
		/// ch: defects 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DEF_THR_A = 269537304,
		/// <summary>
		/// ch: defects 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DEF_THR_B = 269537305,
		/// <summary>
		/// ch: defects 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DEF_THR_C = 269537306,
		/// <summary>
		/// ch: defects 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_DEF_THR_D = 269537307,
		/// <summary>
		/// ch: minimum reflectance分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINR_THR_A = 269537308,
		/// <summary>
		/// ch: minimum reflectance分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINR_THR_B = 269537309,
		/// <summary>
		/// ch: minimum reflectance分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINR_THR_C = 269537310,
		/// <summary>
		/// ch: minimum reflectance分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINR_THR_D = 269537311,
		/// <summary>
		/// ch: minimum edge contrst分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINE_THR_A = 269537312,
		/// <summary>
		/// ch: minimum edge contrst分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINE_THR_B = 269537313,
		/// <summary>
		/// ch: minimum edge contrst分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINE_THR_C = 269537314,
		/// <summary>
		/// ch: minimum edge contrst分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MINE_THR_D = 269537315,
		/// <summary>
		/// ch: modulation 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MOD_THR_A = 269537316,
		/// <summary>
		/// ch: modulation 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MOD_THR_B = 269537317,
		/// <summary>
		/// ch: modulation 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MOD_THR_C = 269537318,
		/// <summary>
		/// ch: modulation 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_MOD_THR_D = 269537319,
		/// <summary>
		/// ch: symbol contrast 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_SC_THR_A = 269537320,
		/// <summary>
		/// ch: symbol contrast 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_SC_THR_B = 269537321,
		/// <summary>
		/// ch: symbol contrast 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_SC_THR_C = 269537322,
		/// <summary>
		/// ch: symbol contrast 分数阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_SC_THR_D = 269537323,
		/// <summary>
		/// ch: aperture支持外部输入使能标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_APERTURE_ENABLE = 269537324,
		/// <summary>
		/// ch: apeture值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_BARCODE_ISO_15416_APERTURE = 269537325,
		/// <summary>
		/// ch: 计算总体等级是否考虑SC等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_SC_FLAG = 269537326,
		/// <summary>
		/// ch: 计算总体等级是否考虑AN等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_AN_FLAG = 269537327,
		/// <summary>
		/// ch: 计算总体等级是否考虑PG等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_PG_FLAG = 269537328,
		/// <summary>
		/// ch: 计算总体等级是否考虑DEC等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_DEC_FLAG = 269537329,
		/// <summary>
		/// ch: 计算总体等级是否考虑UEC等级标志参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_UEC_FLAG = 269537330,
		/// <summary>
		/// ch: 计算总体等级是否考虑GN等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_GN_FLAG = 269537331,
		/// <summary>
		/// ch: 计算总体等级是否考虑FPD等级标志参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_FPD_FLAG = 269537332,
		/// <summary>
		/// ch: 计算总体等级是否考虑MOD等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_MOD_FLAG = 269537333,
		/// <summary>
		/// ch: 计算总体等级是否考虑RM等级标志参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_RM_FLAG = 269537334,
		/// <summary>
		/// ch: SC分数阈值A参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_SC_THR_A = 269537335,
		/// <summary>
		/// ch: SC分数阈值B参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_SC_THR_B = 269537336,
		/// <summary>
		/// ch: SC分数阈值C参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_SC_THR_C = 269537337,
		/// <summary>
		/// ch: SC分数阈值D参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_SC_THR_D = 269537338,
		/// <summary>
		/// ch: AN分数阈值A参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_AN_THR_A = 269537339,
		/// <summary>
		/// ch: AN分数阈值B参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_AN_THR_B = 269537340,
		/// <summary>
		/// ch: AN分数阈值C参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_AN_THR_C = 269537341,
		/// <summary>
		/// ch: AN分数阈值D参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_AN_THR_D = 269537342,
		/// <summary>
		/// ch: PG分数阈值A参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_PG_THR_A = 269537343,
		/// <summary>
		/// ch: PG分数阈值B参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_PG_THR_B = 269537344,
		/// <summary>
		/// ch: PG分数阈值C参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_PG_THR_C = 269537345,
		/// <summary>
		/// ch: PG分数阈值D参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_PG_THR_D = 269537346,
		/// <summary>
		/// ch: UEC分数阈值A参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_UEC_THR_A = 269537347,
		/// <summary>
		/// ch: UEC分数阈值B参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_UEC_THR_B = 269537348,
		/// <summary>
		/// ch: UEC分数阈值C参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_UEC_THR_C = 269537349,
		/// <summary>
		/// ch: UEC分数阈值D参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_UEC_THR_D = 269537350,
		/// <summary>
		/// ch: GN分数阈值A参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_GN_THR_A = 269537351,
		/// <summary>
		/// ch: GN分数阈值B参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_GN_THR_B = 269537352,
		/// <summary>
		/// ch: GN分数阈值C参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_GN_THR_C = 269537353,
		/// <summary>
		/// ch: GN分数阈值D参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_GN_THR_D = 269537360,
		/// <summary>
		/// ch: FPD分数阈值A参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_FPD_THR_A = 269537370,
		/// <summary>
		/// ch: FPD分数阈值B参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_FPD_THR_B = 269537371,
		/// <summary>
		/// ch: FPD分数阈值C参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_FPD_THR_C = 269537372,
		/// <summary>
		/// ch: FPD分数阈值D参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_FPD_THR_D = 269537373,
		/// <summary>
		/// ch: MOD分数阈值A参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_MOD_THR_A = 269537374,
		/// <summary>
		/// ch: MOD分数阈值B参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_MOD_THR_B = 269537375,
		/// <summary>
		/// ch: MOD分数阈值C参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_MOD_THR_C = 269537376,
		/// <summary>
		/// ch: MOD分数阈值D参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_MOD_THR_D = 269537377,
		/// <summary>
		/// ch: RM分数阈值A参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_RM_THR_A = 269537378,
		/// <summary>
		/// ch: RM分数阈值B参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_RM_THR_B = 269537379,
		/// <summary>
		/// ch: RM分数阈值C参数错误
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_RM_THR_C = 269537380,
		/// <summary>
		/// ch: RM分数阈值D参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ISO_15415_RM_THR_D = 269537381,
		/// <summary>
		/// ch: 是否使用模板匹配进行校正参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBSYMBOLVERIFY_PARAM_2DCODE_ENABLE_TPL_MATCH = 269537382,
		/// <summary>
		/// ch: 目标数目超出限制
		/// </summary>
		MVALG_STS_ERR_MVB2D_TRACK_PKG_NUM_OVERFLOW = 269557761,
		/// <summary>
		/// ch: 跟踪计数RECT区域设置超出图像范围
		/// </summary>
		MVALG_STS_ERR_MVB2D_TRACK_ROI_RECT_OVERFLOW = 269557762,
		/// <summary>
		/// ch: 跟踪计数算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_TRACK_ABILITY_ARG_MODU_ENABLE = 269557763,
		/// <summary>
		/// ch: 跟踪计数算法能力集最大目标个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_TRACK_ABILITY_ARG_MAX_OBJ_NUM = 269557764,
		/// <summary>
		/// ch: 跟踪计数算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_TRACK_ABILITY_ARG_THREAD_NUM = 269557765,
		/// <summary>
		/// ch: 跟踪计数算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_TRACK_ABILITY_ARG_THREAD_HANDLE = 269557766,
		/// <summary>
		/// ch: 总边缘点数太少
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_FEATURE_POINTS_TOO_FEW = 272633857,
		/// <summary>
		/// ch: box内边缘点数太少
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_FEATURE_POINTS_TOO_FEW_IN_BOX = 272633858,
		/// <summary>
		/// ch: 顶点越界
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_BOX_VERTEX_OUT_OF_RANGE = 272633859,
		/// <summary>
		/// ch: 拟合的直线平行
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_CENTER_NOT_IN_ROI = 272633860,
		/// <summary>
		/// ch: 建模内存不够
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_NOT_ENOUGH_MEMORY = 272633861,
		/// <summary>
		/// ch: 模板匹配内没有结果
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_NO_RESULT_IN_PATMATCH = 272633862,
		/// <summary>
		/// ch: 中心点拟合错误
		/// </summary>
		MVALG_STS_ERR_MVBFINDMARK_FIT_CROSS_ERR = 272633863,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_KER_PARAM_OVERFLOW = 269561857,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_KER_NUM_OVERFLOW = 269561858,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_FILTER_RESPOND_OFFSET_OVERFLOW = 269561859,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_SCRATCH_LINK_PARAM_OVERFLOW = 269561860,
		/// <summary>
		/// ch: 参数范围错误 
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_SCREEN_OUT_PARAM_OVERFLOW = 269561861,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_PARAM_ADJUST_FAILED = 269561862,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ROI_OVERFLOW = 269561863,
		/// <summary>
		/// ch: 参数范围错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_MODEL_PARAM_OVERFLOW = 269561864,
		/// <summary>
		/// ch: 模型版本错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_MODEL_VERSION_ERR = 269561865,
		/// <summary>
		/// ch: 训练集样本小于最小值
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_CLS_NUMBER_ERR = 269561866,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_MODU_ENABLE = 269561867,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_MAX_IMAGE_SIZE = 269561868,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集滤波核宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_MAX_KERNEL_SIZE = 269561869,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集最大滤波核个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_MAX_KERNEL_NUM = 269561870,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集最大样本个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_MAX_SAMPLE_NUM = 269561871,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集最大类别个数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_MAX_CLS_NUM = 269561872,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_THREAD_NUM = 269561873,
		/// <summary>
		/// ch: 表面缺陷检测算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVB2D_FLAW_ABILITY_ARG_THREAD_HANDLE = 269561874,
		/// <summary>
		/// 多角度光图像融合模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_MODU_MULTIANGLE_LIGHT_IMAGE_FUSION_DISABLE = 269541376,
		/// <summary>
		/// 图像类型参数错误
		/// </summary>                                                                                 
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_RESULT_TYPE = 269541377,
		/// <summary>
		/// 滤波尺寸参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_KERNEL_SIZE = 269541378,
		/// <summary>
		/// 增强开关参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_ENHANCE_ENABLE = 269541379,
		/// <summary>
		/// 背景亮度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_BRIGHTNESS = 269541380,
		/// <summary>
		/// 对比系数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_CONTRAST_COEF = 269541381,
		/// <summary>
		/// 方向增强类型参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_DIR_ENHANCE_TYPE = 269541382,
		/// <summary>
		/// 方向增强等级参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_DIR_ENHANCE_LEVEL = 269541383,
		/// <summary>
		/// 光晕去除等级参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_PARAM_MALIF_HALATION_REMOVE_LEVEL = 269541384,
		/// <summary>
		/// 图像融合算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_ABILITY_ARG_MODU_ENABLE = 269541385,
		/// <summary>
		/// 图像融合算法能力集最大图像大小参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_ABILITY_ARG_MAX_IMAGE_SIZE = 269541386,
		/// <summary>
		/// 图像融合算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_ABILITY_ARG_THREAD_NUM = 269541387,
		/// <summary>
		/// 图像融合算法能力集线程句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_ABILITY_ARG_THREAD_HANDLE = 269541388,
		/// <summary>
		/// 图像融合算法能力集ROI使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBFUSION_ABILITY_ARG_ROI_SIZE_ENABLE = 269541389,
		/// <summary>
		/// ROI设置超出图像范围
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ROI_OVERFLOW = 269570049,
		/// <summary>
		/// 模型参数范围错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_MODEL_PARAM_OVERFLOW = 269570050,
		/// <summary>
		/// 模型参数范围错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_MODEL_VERSION_ERR = 269570051,
		/// <summary>
		/// 训练集样本小于最小值
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_NUMBER_ERR = 269570052,
		/// <summary>
		/// 图像分类算法能力集模块使能参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_MODU_ENABLE = 269570053,
		/// <summary>
		/// 图像分类算法能力集图像宽高参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_MAX_IMAGE_SIZE = 269570054,
		/// <summary>
		/// 图像分类算法能力集最大样本个数参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_MAX_SAMPLE_NUM = 269570055,
		/// <summary>
		/// 图像分类算法能力集最大类别个数参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_MAX_CLS_NUM = 269570056,
		/// <summary>
		/// 图像分类算法能力集线程数参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_THREAD_NUM = 269570057,
		/// <summary>
		/// 图像分类算法能力集线程句柄参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_THREAD_HANDLE = 269570058,
		/// <summary>
		/// 图像分类算法能力集分类模型大小错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_MODEL_SIZE = 269570059,
		/// <summary>
		/// 图像分类算法能力集分类模型空指针错误
		/// </summary>
		MVD_ALG_STS_ERR_MVB2D_CLS_ABILITY_ARG_MODEL_NULL_PTR = 269570060,
		/// <summary>
		/// ch: cuda参数错误
		/// </summary>
		MVALG_STS_ERR_CUDA_INVALID_VALUE = 270532608,
		/// <summary>
		/// ch: cuda分配内存失败
		/// </summary>
		MVALG_STS_ERR_CUDA_MEMORY_ALLOC = 270532609,
		/// <summary>
		/// ch: cuda初始化错误
		/// </summary>
		MVALG_STS_ERR_CUDA_INITIAL = 270532610,
		/// <summary>
		/// ch: cuda runtime库未加载
		/// </summary>
		MVALG_STS_ERR_CUDA_RT_UNLOAD = 270532611,
		/// <summary>
		/// ch: 不存在支持CUDA的显卡
		/// </summary>
		MVALG_STS_ERR_CUDA_NO_DEVICE = 270532612,
		/// <summary>
		/// ch: 无效CUDA的显卡
		/// </summary>
		MVALG_STS_ERR_CUDA_INVALID_DEVICE = 270532613,
		/// <summary>
		/// ch: CUDA其他错误
		/// </summary>
		MVALG_STS_ERR_CUDA_OTHER = 270532614,
		/// <summary>
		/// ch: CUBLAS未初始化
		/// </summary>
		MVALG_STS_ERR_CUBLAS_NOT_INITIALIZED = 270532615,
		/// <summary>
		/// ch: CUBLAS分配失败
		/// </summary>
		MVALG_STS_ERR_CUBLAS_ALLOC_FAILED = 270532616,
		/// <summary>
		/// ch: CUBLAS参数错误
		/// </summary>
		MVALG_STS_ERR_CUBLAS_INVALID_VALUE = 270532617,
		/// <summary>
		/// ch: CUBLAS架构不匹配
		/// </summary>
		MVALG_STS_ERR_CUBLAS_ARCH_MISMATCH = 270532618,
		/// <summary>
		/// ch: CUBLAS映射错误
		/// </summary>
		MVALG_STS_ERR_CUBLAS_MAPPING_ERROR = 270532619,
		/// <summary>
		/// ch: CUBLAS执行错误
		/// </summary>
		MVALG_STS_ERR_CUBLAS_EXECUTION_FAILED = 270532620,
		/// <summary>
		/// ch: CUBLAS内部错误
		/// </summary>
		MVALG_STS_ERR_CUBLAS_INTERNAL_ERROR = 270532621,
		/// <summary>
		/// ch: CUBLAS功能不支持
		/// </summary>
		MVALG_STS_ERR_CUBLAS_NOT_SUPPORTED = 270532622,
		/// <summary>
		/// ch: CUBLAS的LICENSE不符
		/// </summary>
		MVALG_STS_ERR_CUBLAS_LICENSE_ERROR = 270532623,
		/// <summary>
		/// ch: CUBLAS其他错误
		/// </summary>
		MVALG_STS_ERR_CUBLAS_OTHER = 270532624,
		/// <summary>
		/// ch: CUDNN未初始化
		/// </summary>
		MVALG_STS_ERR_CUDNN_NOT_INITIALIZED = 270532625,
		/// <summary>
		/// ch: CUDNN分配失败
		/// </summary>
		MVALG_STS_ERR_CUDNN_ALLOC_FAILED = 270532626,
		/// <summary>
		/// ch: CUDNN参数错误
		/// </summary>
		MVALG_STS_ERR_CUDNN_BAD_PARAM = 270532627,
		/// <summary>
		/// ch: CUDNN内部错误
		/// </summary>
		MVALG_STS_ERR_CUDNN_INTERNAL_ERROR = 270532628,
		/// <summary>
		/// ch: CUDNN无效值
		/// </summary>
		MVALG_STS_ERR_CUDNN_INVALID_VALUE = 270532629,
		/// <summary>
		/// ch: CUDNN架构不匹配
		/// </summary>
		MVALG_STS_ERR_CUDNN_ARCH_MISMATCH = 270532630,
		/// <summary>
		/// ch: CUDNN映射错误
		/// </summary>
		MVALG_STS_ERR_CUDNN_MAPPING_ERROR = 270532631,
		/// <summary>
		/// ch: CUDNN执行错误
		/// </summary>
		MVALG_STS_ERR_CUDNN_EXECUTION_FAILED = 270532632,
		/// <summary>
		/// ch: CUDNN功能不支持
		/// </summary>
		MVALG_STS_ERR_CUDNN_NOT_SUPPORTED = 270532633,
		/// <summary>
		/// ch: CUDNN的LICENSE不符
		/// </summary>
		MVALG_STS_ERR_CUDNN_LICENSE_ERROR = 270532634,
		/// <summary>
		/// ch: CUDNN其他错误
		/// </summary>
		MVALG_STS_ERR_CUDNN_OTHER = 270532635,
		/// <summary>
		/// ch: HCNN库错误
		/// </summary>
		MVALG_STS_ERR_HCNN_ERR = 270536704,
		/// <summary>
		/// ch: 模型解析label_map出错
		/// </summary>
		MVALG_STS_ERR_LABEL_MAP_ERR = 270536705,
		/// <summary>
		/// ch: 模型类别数出错
		/// </summary>
		MVALG_STS_ERR_CLASS_NUM_ERR = 270536706,
		/// <summary>
		/// ch: 深度学习模型解析错误
		/// </summary>
		MVALG_STS_ERR_CNN_MODEL_PARSE = 270536707,
		/// <summary>
		/// ch: 深度学习模型头信息错误
		/// </summary>
		MVALG_STS_ERR_CNN_MODEL_HEAD_INFO = 270536708,
		/// <summary>
		/// ch: 深度学习模型类型错误
		/// </summary>
		MVALG_STS_ERR_CNN_MODEL_TYPE = 270536709,
		/// <summary>
		/// ch: 深度学习模型完整性错误
		/// </summary>
		MVALG_STS_ERR_CNN_MODEL_IMCOMPLETE = 270536710,
		/// <summary>
		/// ch: 深度学习模型自定义层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_CUSTOM_LAYER_PARAM = 270536711,
		/// <summary>
		/// ch: 深度学习模型优化失败，优化后层数溢出
		/// </summary>
		MVALG_STS_ERR_CNN_INSERT_REORDER_FAILED = 270536712,
		/// <summary>
		/// ch: 数据转换格式错误
		/// </summary>
		MVALG_STS_ERR_CNN_REORDER_FORMAT_ERR = 270536713,
		/// <summary>
		/// ch: 读取参数个数错误
		/// </summary>
		MVALG_STS_ERR_CNN_SCANF_PARAM_NUM_ERR = 270536714,
		/// <summary>
		/// ch: FP16不支持
		/// </summary>
		MVALG_STS_ERR_CNN_FP16_UNSUPPORTED = 270536715,
		/// <summary>
		/// ch: 输入Blob个数错误
		/// </summary>
		MVALG_STS_ERR_CNN_INPUT_BLOB_NUM_ERR = 270536716,
		/// <summary>
		/// ch: 输入Blob维数错误
		/// </summary>
		MVALG_STS_ERR_CNN_INPUT_BLOB_NDIMS_ERR = 270536717,
		/// <summary>
		/// ch: 输入Blob形状错误
		/// </summary>
		MVALG_STS_ERR_CNN_INPUT_BLOB_SHAPE_ERR = 270536718,
		/// <summary>
		/// ch: 输入Blob参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_INPUT_BLOB_PARAMS_ERR = 270536719,
		/// <summary>
		/// ch: 输出Blob个数错误
		/// </summary>
		MVALG_STS_ERR_CNN_OUTPUT_BLOB_NUM_ERR = 270536720,
		/// <summary>
		/// ch: 网络层数错误
		/// </summary>
		MVALG_STS_ERR_CNN_LAYER_NUM_ERR = 270536721,
		/// <summary>
		/// ch: 网络层类型错误
		/// </summary>
		MVALG_STS_ERR_CNN_LAYER_TYPE_ERR = 270536722,
		/// <summary>
		/// ch: 网络层ID错误
		/// </summary>
		MVALG_STS_ERR_CNN_LAYER_ID_ERR = 270536723,
		/// <summary>
		/// ch: 网络层ID错误
		/// </summary>
		MVALG_STS_ERR_CNN_LAYER_IDX_ERR = 270536724,
		/// <summary>
		/// ch: 网络层类型错误
		/// </summary>
		MVALG_STS_ERR_CNN_LAYER_PIN_ERR = 270536725,
		/// <summary>
		/// ch: Blob索引错误
		/// </summary>
		MVALG_STS_ERR_CNN_BLOB_IDX_ERR = 270536726,
		/// <summary>
		/// ch: 字符串长度错误
		/// </summary>
		MVALG_STS_ERR_CNN_STRING_LEN_ERR = 270536727,
		/// <summary>
		/// ch: 超参指针错误
		/// </summary>
		MVALG_STS_ERR_CNN_HYPERPARAM_PTR_ERR = 270536728,
		/// <summary>
		/// ch: 数学运算错误
		/// </summary>
		MVALG_STS_ERR_CNN_CUBLAS_ERR = 270536729,
		/// <summary>
		/// ch: 数据格式不支持
		/// </summary>
		MVALG_STS_ERR_CNN_DATA_FORMAT_UNSUPPORTED = 270536730,
		/// <summary>
		/// ch: men_tab个数错误
		/// </summary>
		MVALG_STS_ERR_CNN_MEM_TAB_NUM = 270536731,
		/// <summary>
		/// ch: 归一化层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_NORMALIZE_LAYER_PARAM_ERR = 270536732,
		/// <summary>
		/// ch: 目标检测层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_OBJECT_DETECTION_LAYER_PARAM_ERR = 270536733,
		/// <summary>
		/// ch: proposal层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_PROPOSAL_LAYER_PARAM_ERR = 270536734,
		/// <summary>
		/// ch: relu层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_REORG_LAYER_PARAM_ERR = 270536735,
		/// <summary>
		/// ch: reshape层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_RESHAPE_LAYER_PARAM_ERR = 270536736,
		/// <summary>
		/// ch: reverse层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_REVERSE_LAYER_PARAM_ERR = 270536737,
		/// <summary>
		/// ch: roi align层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_ROI_ALIGN_LAYER_PARAM_ERR = 270536738,
		/// <summary>
		/// ch: roi pooling层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_ROI_POOLING_LAYER_PARAM_ERR = 270536739,
		/// <summary>
		/// ch: seglink detection层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_SEGLINK_DETECTION_LAYER_PARAM_ERR = 270536740,
		/// <summary>
		/// ch: solo output层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_SOLO_OUTPUT_LAYER_PARAM_ERR = 270536741,
		/// <summary>
		/// ch: upsample层参数错误
		/// </summary>
		MVALG_STS_ERR_CNN_UPSAMPLE_LAYER_PARAM_ERR = 270536742,
		/// <summary>
		/// ch: 内存优化出错
		/// </summary>
		MVD_ALG_STS_ERR_CNN_MEM_OPT_CHECK_ERR = 270536743,
		/// <summary>
		/// ch: eltwise层参数错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_ELTWISE_LAYER_PARAM_ERR = 270536744,
		/// <summary>
		/// ch: 虚拟地址转物理地址报错
		/// </summary>
		MVD_ALG_STS_ERR_CNN_VIR2PHY_ERR = 270536745,
		/// <summary>
		/// ch: infer version参数错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_INFER_VERSION_PARAM_ERR = 270536746,
		/// <summary>
		/// ch: nesting层参数错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_NESTING_PARAM_ERR = 270536747,
		/// <summary>
		/// ch: nesting层模型错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_NESTING_MODEL_ERR = 270536748,
		/// <summary>
		/// ch: model framework参数错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_MODEL_FRAMEWORK_PARAM_ERR = 270536749,
		/// <summary>
		/// ch: ov没有执行
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_NOT_IMPLEMENTED_ERR = 270536750,
		/// <summary>
		/// ch: ov网络没有加载
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_NETWORK_NOT_LOADED_ERR = 270536751,
		/// <summary>
		/// ch: ov参数错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_PARAMETER_MISMATCH_ERR = 270536752,
		/// <summary>
		/// ch: ov没有被发现
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_NOT_FOUND_ERR = 270536753,
		/// <summary>
		/// ch: ov执行超出边界
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_OUT_OF_BOUNDS_ERR = 270536754,
		/// <summary>
		/// ch: ov请求忙碌
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_REQUEST_BUSY_ERR = 270536755,
		/// <summary>
		/// ch: ov输出结果没有准备好
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_RESULT_NOT_READY_ERR = 270536756,
		/// <summary>
		/// ch: ov没有分配
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_NOT_ALLOCATED_ERR = 270536757,
		/// <summary>
		/// ch: ov没有启动推理
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_INFER_NOT_STARTED_ERR = 270536758,
		/// <summary>
		/// ch: ov没有读取网络
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_NETWORK_NOT_READ_ERR = 270536759,
		/// <summary>
		/// ch: ov推理取消
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_INFER_CANCELLED_ERR = 270536760,
		/// <summary>
		/// ch: ov未知错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_OV_UNEXPECTED_ERR = 270536761,
		/// <summary>
		/// ch: trt离线模型反序列错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_OFFLINE_MODEL_DESERIALIZE_ERR = 270536768,
		/// <summary>
		/// ch: trt在线模型解析错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_MODEL_PARSE_ERR = 270536769,
		/// <summary>
		/// ch: trt序列化模型错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_MODEL_SERIALIZE_ERR = 270536770,
		/// <summary>
		/// ch: trt引擎反序列错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_ENGINE_DESERIALIZE_ERR = 270536771,
		/// <summary>
		/// ch: trt上下文创建错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_CONTEXT_CREATE_ERR = MVD_ALG_STS_ERR_CNN_TRT_ENGINE_DESERIALIZE_ERR,
		/// <summary>
		/// ch: trt推理blob个数错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_INFER_BLOB_NUM_ERR = 270536772,
		/// <summary>
		/// ch: trt profile id设置错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_PROFILE_ID_SET_ERR = 270536773,
		/// <summary>
		/// ch: trt推理执行错误
		/// </summary>
		MVD_ALG_STS_ERR_CNN_TRT_EXECUTE_ERR = 270536774,
		/// <summary>
		/// ch: 缩放时填充的数据值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CLASSIFY_PARAM_RESIZE_FILL_VAL = 270540800,
		/// <summary>
		/// ch: 缩放方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CLASSIFY_PARAM_RESIZE_TYPE = 270540801,
		/// <summary>
		/// ch: 前向分类模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CLASSIFY_MODU_CLS_DISABLE = 270540802,
		/// <summary>
		/// ch: 分类模型性能差，无法正常输出结果，CPU/GPU可能存在不一致现象，建议重新训练
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CLASSIFY_BAD_MODEL = 270540803,
		/// <summary>
		/// ch: 深度学习分类算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_MODU_ENABLE = 270540804,
		/// <summary>                                               
		/// ch: 深度学习分类算法能力集最大图像宽高参数错误                            
		/// </summary>                                                                       
		MVALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_MAX_IMAGE_SIZE = 270540805,
		/// <summary>                                               
		/// ch: 深度学习分类算法能力集模型句柄参数错误                               
		/// </summary>                                              
		MVALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_MODEL_HANDEL = 270540806,
		/// <summary>                                               
		/// ch: 深度学习分类算法能力集CUDA句柄参数错误                              
		/// </summary>                                              
		MVALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_CUDA_HANDLE = 270540807,
		/// <summary>                                               
		/// ch: 深度学习分类算法能力集线程数参数错误                                 
		/// </summary>                                              
		MVALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_THREAD_NUM = 270540808,
		/// <summary>                                               
		/// ch: 深度学习分类算法能力集线程句柄参数错误                               
		/// </summary>                                              
		MVALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_THREAD_HANDLE = 270540809,
		/// <summary>
		/// ch: 深度学习分类算法能力集批处理使能错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_BATCH_SIZE_ENABLE = 270540810,
		/// <summary>
		/// ch: 深度学习分类算法能力集批处理等级错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_BATCH_SIZE_NUM = 270540811,
		/// <summary>
		/// ch: 深度学习分类算法能力集能力集变化使能错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_CLASSIFY_ABILITY_ARG_CHANGE_FLAG = 270540812,
		/// <summary>
		/// ch: 内存空间错误，不支持的内存空间
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_VCA_MEM_SPACE = 270544896,
		/// <summary>
		/// ch: 开发平台错误，只可用于海思平台
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_VCA_PLAT = 270544897,
		/// <summary>
		/// ch: 内部HKANN库处理异常
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_VCA = 270544898,
		/// <summary>
		/// ch: 最大目标数目参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_OBJ_NUM = 270544899,
		/// <summary>
		/// ch: 最小置信度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MIN_SCORE = 270544900,
		/// <summary>
		/// ch: 最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_OVERLAP = 270544901,
		/// <summary>
		/// ch: 缩放时填充的数据值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_RESIZE_FILL_VAL = 270544902,
		/// <summary>
		/// ch: 缩放方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_RESIZE_TYPE = 270544903,
		/// <summary>
		/// ch: 起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_START_ANGLE = 270544904,
		/// <summary>
		/// ch: 终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_END_ANGLE = 270544905,
		/// <summary>
		/// ch: 最小宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MIN_WIDTH = 270544906,
		/// <summary>
		/// ch: 最大宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_WIDTH = 270544907,
		/// <summary>
		/// ch: 最小高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MIN_HEIGHT = 270544908,
		/// <summary>
		/// ch: 最大高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_HEIGHT = 270544909,
		/// <summary>
		/// ch: 角度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_ANGLE_ENABLE = 270544910,
		/// <summary>
		/// ch: 宽度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_WIDTH_ENABLE = 270544911,
		/// <summary>
		/// ch: 高度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_HEIGHT_ENABLE = 270544912,
		/// <summary>
		/// ch: 目标框合并使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_OBJ_COMBINE_ENABLE = 270544913,
		/// <summary>
		/// ch: 排序方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_ORDER_TYPE = 270544914,
		/// <summary>
		/// ch: 出界是否过滤参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_OUT_FILTER_ENABLE = 270544915,
		/// <summary>
		/// ch: 最大覆盖率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_COVERAGE = 270544916,
		/// <summary>
		/// ch: 目标保留在ROI内的面积比参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_OUT_FILTER_SCORE = 270544917,
		/// <summary>
		/// ch: 前向预测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_MODU_DETECT_DISABLE = 270544918,
		/// <summary>
		/// ch: 不同类别非极大抑制使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_DIFFCLASSNMS_ENABLE = 270544919,
		/// <summary>
		/// ch: 最小面积参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MIN_AREA = 270544920,
		/// <summary>
		/// ch: 最大面积参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_AREA = 270544921,
		/// <summary>
		/// ch: 面积筛选使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_PARAM_AREA_ENABLE = 270544922,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集模块使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_MODU_ENABLE = 270544923,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集最大图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_MAX_IMAGE_SIZE = 270544924,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集模型句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_MODEL_HANDEL = 270544925,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集CUDA句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_CUDA_HANDLE = 270544926,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集线程数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_THREAD_NUM = 270544927,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_THREAD_HANDLE = 270544928,
		/// <summary>
		/// ch: 深度学习目标检测SOD尺寸错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_SOD_SIZE = 270544929,
		/// <summary>
		/// ch: 深度学习目标检测SOD重叠率错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_SOD_OVERLAP = 270544930,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集批处理使能错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_BATCH_SIZE_ENABLE = 270544931,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集批处理等级错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_BATCH_SIZE_NUM = 270544932,
		/// <summary>
		/// ch: 深度学习目标检测算法能力集能力集变化使能错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_ABILITY_ARG_CHANGE_FLAG = 270544933,
		/// <summary>
		/// ch: 最小宽高比参数错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_PARAM_MIN_WH_RATIO = 270544934,
		/// <summary>
		/// ch: 最大宽高比参数错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_PARAM_MAX_WH_RATIO = 270544935,
		/// <summary>
		/// ch: 宽高比筛选使能参数错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_DETECT_PARAM_WH_RATIO_ENABLE = 270544936,
		/// <summary>
		/// ch: 文本定位模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_MODU_DETECT_DISABLE = 270548992,
		/// <summary>
		/// ch: 单字符检测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_MODU_RECOG_DISABLE = 270548993,
		/// <summary>
		/// ch: 字符识别模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_OCR_MODU_RECOG_DISABLE = 270548994,
		/// <summary>
		/// ch: 最大目标数目参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MAX_OBJ_NUM = 270548995,
		/// <summary>
		/// ch: 最小置信度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MIN_SCORE = 270548996,
		/// <summary>
		/// ch: 框最小平均分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_BOX_SCORE = 270548997,
		/// <summary>
		/// ch: 最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MAX_OVERLAP = 270548998,
		/// <summary>
		/// ch: 链接阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_LINK_THRESHOLD = 270548999,
		/// <summary>
		/// ch: 起始角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_START_ANGLE = 270549000,
		/// <summary>
		/// ch: 终止角度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_END_ANGLE = 270549001,
		/// <summary>
		/// ch: 最小宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MIN_WIDTH = 270549002,
		/// <summary>
		/// ch: 最大宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MAX_WIDTH = 270549003,
		/// <summary>
		/// ch: 最小高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MIN_HEIGHT = 270549004,
		/// <summary>
		/// ch: 最大高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_MAX_HEIGHT = 270549005,
		/// <summary>
		/// ch: 角度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_ANGLE_ENABLE = 270549006,
		/// <summary>
		/// ch: 宽度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_WIDTH_ENABLE = 270549007,
		/// <summary>
		/// ch: 高度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_HEIGHT_ENABLE = 270549008,
		/// <summary>
		/// ch: 缩放时填充的数据值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_RESIZE_FILL_VAL = 270549009,
		/// <summary>
		/// ch: 缩放方法参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_RESIZE_TYPE = 270549010,
		/// <summary>
		/// ch: 排序方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_ORDER_TYPE = 270549011,
		/// <summary>
		/// ch: 出界是否过滤参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_OUT_FILTER_ENABLE = 270549012,
		/// <summary>
		/// ch: 出界是否过滤参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_OUT_FILTER_SCORE = 270549013,
		/// <summary>
		/// ch: 最大目标数目参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MAX_OBJ_NUM = 270549014,
		/// <summary>
		/// ch: 最小置信度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MIN_SCORE = 270549015,
		/// <summary>
		/// ch: 最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MAX_OVERLAP = 270549016,
		/// <summary>
		/// ch: 最小宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MIN_WIDTH = 270549017,
		/// <summary>
		/// ch: 最大宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MAX_WIDTH = 270549018,
		/// <summary>
		/// ch: 最小高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MIN_HEIGHT = 270549019,
		/// <summary>
		/// ch: 最大高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_MAX_HEIGHT = 270549020,
		/// <summary>
		/// ch: 宽度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_WIDTH_ENABLE = 270549021,
		/// <summary>
		/// ch: 高度筛选使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_HEIGHT_ENABLE = 270549022,
		/// <summary>
		/// ch: 输出结果的top_n参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_TOP_N = 270549023,
		/// <summary>
		/// ch: 排序方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_ORDER_TYPE = 270549024,
		/// <summary>
		/// ch: 出界是否过滤参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_OUT_FILTER_ENABLE = 270549025,
		/// <summary>
		/// ch: 出界是否过滤参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_RECOG_PARAM_OUT_FILTER_SCORE = 270549026,
		/// <summary>
		/// ch: 最小置信度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNNOCR_PARAM_MIN_SCORE = 270549027,
		/// <summary>
		/// ch: 输出结果的top_n参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNNOCR_PARAM_TOP_N = 270549028,
		/// <summary>
		/// ch: 文本框回复缩放值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_SHRINK_BACK = 270549029,
		/// <summary>
		/// ch: 热力图输出设置错误(该模型不该设置热力图输出)
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_PARAM_USE_RESPONSE = 270549030,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集模块使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_MODU_ENABLE = 270549031,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集最大图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_MAX_IMAGE_SIZE = 270549032,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集模型句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_MODEL_HANDEL = 270549033,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集CUDA句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_CUDA_HANDLE = 270549034,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集线程数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_THREAD_NUM = 270549035,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_THREAD_HANDLE = 270549036,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集处理类型与模型类型不匹配
		/// </summary>		
		MVALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_PRC_TYPE = 270549037,
		/// <summary>
		/// ch: 字符识别输出维度异常
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_OCR_DIM_ERR = 270549038,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集批处理使能错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_BATCH_SIZE_ENABLE = 270549039,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集批处理等级错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_BATCH_SIZE_NUM = 270549040,
		/// <summary>
		/// ch: 翻转最小置信度使能参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNNOCR_PARAM_FLIP_ENABLE = 270549041,
		/// <summary>
		/// ch: 翻转最小置信度参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNNOCR_PARAM_FLIP_MIN_SCORE = 270549042,
		/// <summary>
		/// ch: 深度学习文本定位识别算法能力集能力集变化使能错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_CHAR_DETECT_ABILITY_ARG_CHANGE_FLAG = 270549043,
		/// <summary>
		/// ch: 字符识别失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_FAIL = 270553088,
		/// <summary>
		/// ch: 分辨率不满足
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_RESOLUTION_UNSUPPORT = 270553089,
		/// <summary>
		/// ch: 内存分配失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MEM_ALLOC_FAIL = 270553090,
		/// <summary>
		/// ch: 接口内存溢出
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MEM_OVER = 270553091,
		/// <summary>
		/// ch: 内存校验失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MEM_CHECK_FAIL = 270553092,
		/// <summary>
		/// ch: 接口或流程函数参数指针为空
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_PTR_NULL = 270553093,
		/// <summary>
		/// ch: 模型内存错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MODEL_MEM_ERROR = 270553094,
		/// <summary>
		/// ch: 外部传入的cuda handle为空
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_CUDA_HANDLE_ERROR = 270553095,
		/// <summary>
		/// ch: 模型参数指针为空
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MODEL_PARAM_PTR_NULL = 270553096,
		/// <summary>
		/// ch: 模型接口指针为空
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MODEL_ITF_PTR_NULL = 270553097,
		/// <summary>
		/// ch: 模型数量错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_LOAD_MODEL_SIZE_ERR = 270553098,
		/// <summary>
		/// ch: 模型类型错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_LOAD_MODEL_TYPE_ERR = 270553099,
		/// <summary>
		/// ch: 获取调试信息失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_POS_INFO_FAIL = 270553100,
		/// <summary>
		/// ch: 获取模型内存失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_GET_MODEL_SIZE = 270553101,
		/// <summary>
		/// ch: 创建模型失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_CREATE_MODEL = 270553102,
		/// <summary>
		/// ch: 获取文件头信息失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MODEL_INFO_ERR = 270553103,
		/// <summary>
		/// ch: cnnlib读取模型失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_MODEL_CNNLIB_MODEL_ERR = 270553104,
		/// <summary>
		/// ch: 模型加载失败
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_LOAD_MODEL_ERR = 270553105,
		/// <summary>
		/// ch: 字符识别类算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_ABILITY_ARG_MODU_ENABLE = 270553106,
		/// <summary>
		/// ch: 字符识别类算法能力集最大图像宽高参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_ABILITY_ARG_MAX_IMAGE_SIZE = 270553107,
		/// <summary>
		/// ch: 字符识别类算法能力集模型句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_ABILITY_ARG_MODEL_HANDEL = 270553108,
		/// <summary>
		/// ch: 字符识别类算法能力集CUDA句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_ABILITY_ARG_CUDA_HANDLE = 270553109,
		/// <summary>
		/// ch: 字符识别类算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_ABILITY_ARG_THREAD_NUM = 270553110,
		/// <summary>
		/// ch: 字符识别类算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBOCRDL_ABILITY_ARG_THREAD_HANDLE = 270553111,
		/// <summary>
		/// ch: 模型错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_MODEL = 270557184,
		/// <summary>
		/// ch: 模型大小错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_MODEL_SIZE = 270557185,
		/// <summary>
		/// ch: 模型版本错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_MODEL_VERSIONE = 270557186,
		/// <summary>
		/// ch: 该功能未实现
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_UNREALIZE = 270557187,
		/// <summary>
		/// ch: ASI获取模型内存失败
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ASI_GET_MODEL_SIZE = 270557188,
		/// <summary>
		/// ch: ASI创建模型失败
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_CREATE_MODEL = 270557189,
		/// <summary>
		/// ch: ASI获取模型类型失败
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_GET_MODEL_TYPE = 270557190,
		/// <summary>
		/// ch: 前向预测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_MODU_FORWARD_DISABLE = 270557191,
		/// <summary>
		/// ch: 检测像素间隔参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_PARAM_SAMPLE_INTERVAL = 270557192,
		/// <summary>
		/// ch: 图片padding方式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_PARAM_IMG_PAD_TYPE = 270557193,
		/// <summary>
		/// ch: 算法库运行模式参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_PARAM_APP_MODE = 270557194,
		/// <summary>
		/// ch: 最小分数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_PARAM_MIN_SCORE = 270557195,
		/// <summary>
		/// ch: 运行平台错误，只支持cpu/gpu
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_PLATFORM = 270557196,
		/// <summary>
		/// ch: 当前运行平台不支持该模式
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_MODEL_PLATFORM = 270557197,
		/// <summary>
		/// ch: 算法能力集模块使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_MODU_ENABLE = 270557198,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_MAX_IMAGE_SIZE = 270557199,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集通道个数参数错误 
		/// </summary> 
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_CHANNLES = 270557200,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集模型句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_MODEL_HANDLE = 270557201,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集CUDA句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_CUDA_HANDLE = 270557202,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集线程数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_THREAD_NUM = 270557203,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_THREAD_HANDLE = 270557204,
		/// <summary>
		/// ch: 深度学习缺陷检测深度学习缺陷检测算法概率图类别参数错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_PARAM_SCORE_NUM = 270557205,
		/// <summary>
		/// ch: 深度学习缺陷检测深度学习缺陷检测算法轻量化SOD尺寸错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_SEMANTIC_SOD_SIZE = 270557206,
		/// <summary>
		/// ch: 深度学习缺陷检测算法轻量化SOD重叠率错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_SEMANTIC_SOD_OVERLAP = 270557207,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集批处理使能错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_BATCH_SIZE_ENABLE = 270557208,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集批处理等级错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_BATCH_SIZE_NUM = 270557209,
		/// <summary>
		/// ch: 深度学习缺陷检测算法能力集能力集变化使能错误
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_ABILITY_ARG_CHANGE_FLAG = 270557210,
		/// <summary>
		/// ch: 深度学习缺陷检测算法旧版本模型不支持该模式（SOD和ROI裁剪）
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_FLAW_NO_SUPPORT_MODE_TYPE = 270557211,
		/// <summary>
		/// ch: 图像检索模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_MODU_RETRIEVAL_DISABLE = 270581760,
		/// <summary>
		/// ch: 图像校验模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_MODU_COMPARE_DISABLE = 270581761,
		/// <summary>
		/// ch: gallery管理模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_MODU_GALLERY_DISABLE = 270581762,
		/// <summary>
		/// ch: 最小相似度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_PARAM_MIN_SIMILARITY = 270581763,
		/// <summary>
		/// ch: 输出top n参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_PARAM_II_TOP_NUM = 270581764,
		/// <summary>
		/// ch: 不合理的注册图像数 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_INVALID_REGISTER_NUM = 270581765,
		/// <summary>
		/// ch: 不合理的注册类别ID 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_INVALID_REGISTER_ID = 270581766,
		/// <summary>
		/// ch: 注册新类必须输入类名名
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_MISSING_REGISTER_LABEL_NAME = 270581767,
		/// <summary>
		/// ch: 超过单类别最大注册图像数 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_OVER_MAX_SINGLE_REGISTER_NUM = 270581768,
		/// <summary>
		/// ch: 超过最大注册类别数 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_OVER_MAX_REGISTER_CLASS_NUM = 270581769,
		/// <summary>
		/// ch: 不合理的删除类别ID 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_INVALID_DELETE_ID = 270581770,
		/// <summary>
		/// ch: 不合理的删除类别名
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_INVALID_DELETE_CLASS_NAME = 270581771,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集模块使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_MODU_ENABLE = 270581772,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集最大图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_MAX_IMAGE_SIZE = 270581773,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集模型句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_MODEL_HANDEL = 270581774,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集CUDA句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_CUDA_HANDLE = 270581775,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集线程数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_THREAD_NUM = 270581776,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_THREAD_HANDLE = 270581777,
		/// <summary>
		/// ch: 特征类型组合参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_PARAM_FEAT_TYPE = 270581778,
		/// <summary>
		/// ch: gallery_size错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_GALLERY_SIZE = 270581780,
		/// <summary>
		/// ch: 注册文件与模型不匹配 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_REGISTER_MISMATCH_BIN = 270581781,
		/// <summary>
		/// ch: 不合理的查询类别ID 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_INVALID_QUERY_ID = 270581782,
		/// <summary>
		/// ch: 不合理的查询类别名 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_INVALID_QUERY_CLASS_NAME = 270581783,
		/// <summary>
		/// ch: 注册文件为空 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_GALLERY_IS_NULL = 270581784,
		/// <summary>
		/// ch: 不合理的最大类别数设置 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_PARAM_CLASS_NUM_MAX = 270581785,
		/// <summary>
		/// ch: 不合理的最大单类样本数设置 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_PARAM_REGI_NUM_MAX = 270581786,
		/// <summary>
		/// ch: 类别个数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_RETRIEVAL_CLASS_NUM_ERROR = 270581787,
		/// <summary>
		/// ch: 不合理的batchsize 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_RETRIEVAL_PARAM_IMG_NUM = 270581788,
		/// <summary>
		/// ch: 不合理的批处理使能参数 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_BATCH_SIZE_ENABLE = 270581789,
		/// <summary>
		/// ch: 不合理的多NPU使能参数 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_MULTI_NPUS_ENABLE = 270581790,
		/// <summary>
		/// ch: 深度学习图像检索算法能力集能力集变化标志位参数错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_RETRIEVAL_ABILITY_ARG_ABILITY_CHANGE_FLAG = 270581791,
		/// <summary>
		/// ch: 深度学习异常检测算法能力集模块使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_MODU_ENABLE = 270589952,
		/// <summary>
		/// ch: 深度学习异常检测算法能力集最大图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_MAX_IMAGE_SIZE = 270589953,
		/// <summary>
		/// ch: 深度学习异常检测算法能力集模型句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_MODEL_HANDEL = 270589954,
		/// <summary>
		/// ch: 深度学习异常检测算法能力集CUDA句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_CUDA_HANDLE = 270589955,
		/// <summary>
		/// ch: 深度学习异常检测算法能力集线程数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_THREAD_NUM = 270589956,
		/// <summary>
		/// ch:深度学习异常检测算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_THREAD_HANDLE = 270589957,
		/// <summary>
		/// ch: 模型类型错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_INSPECT_MODEL_VER_ERROR = 270589958,
		/// <summary>
		/// ch: 模型大小错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_INSPECT_MODEL_SIZE_ERROR = 270589959,
		/// <summary>
		/// ch: 该功能未实现错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_INSPECT_NOT_IMPLEMENTED_ERROR = 270589960,
		/// <summary>
		/// ch: 运行平台错误，只支持cpu/gpu 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_INSPECT_PLATFORM_ERROR = 270589961,
		/// <summary>
		/// ch: 当前运行平台不支持该模式 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_INSPECT_MODEL_PLATFORM_ERROR = 270589962,
		/// <summary>
		/// ch: 深度学习异常检测算法能力集能力集变化使能错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_INSPECT_ABILITY_ARG_ABILITY_CHANGE_FLAG = 270589968,
		/// <summary>
		/// ch: 内存空间错误，不支持的内存空间
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_VCA_MEM_SPACE = 270585856,
		/// <summary>
		/// ch: 开发平台错误，只可用于海思平台
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_VCA_PLAT = 270585857,
		/// <summary>
		/// ch: 内部HKANN库处理异常
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_VCA = 270585858,
		/// <summary>
		/// ch: 掩模图blob轮廓点数异常
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_MASK_PTS_NUM = 270585859,
		/// <summary>
		/// ch: 深度学习分割算法能力集模块使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_MODU_ENABLE = 270585860,
		/// <summary>
		/// ch: 深度学习分割算法能力集最大图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_MAX_IMAGE_SIZE = 270585861,
		/// <summary>
		/// ch: 深度学习分割算法能力集模型句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_MODEL_HANDEL = 270585862,
		/// <summary>
		/// ch: 深度学习分割算法能力集CUDA句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_CUDA_HANDLE = 270585863,
		/// <summary>
		/// ch: 深度学习分割算法能力集线程数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_THREAD_NUM = 270585864,
		/// <summary>
		/// ch: 深度学习分割算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_THREAD_HANDLE = 270585865,
		/// <summary>
		/// ch: 深度学习分割算法能力集全景分割质量参数错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_PANOPTIC_SEG_QUALITY = 270585866,
		/// <summary>
		/// ch: 输入的点提示数量错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_BAD_ARG_POINT_PROMT_NUM = 270585867,
		/// <summary>
		/// ch: 输入的检测框提示数量错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_BAD_ARG_BOX_PROMT_NUM = 270585868,
		/// <summary>
		/// ch: 输入的掩膜提示数量错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_BAD_ARG_MASK_PROMT_NUM = 270585869,
		/// <summary>
		/// ch: 编码器类型错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_ENCODER_MODEL_TYPE_ERR = 270585870,
		/// <summary>
		/// ch: 深度学习分割算法能力集批处理使能错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_BATCH_SIZE_ENABLE = 270585871,
		/// <summary>
		/// ch: 深度学习分割算法能力集批处理等级错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_BATCH_SIZE_NUM = 270585872,
		/// <summary>
		/// ch: 深度学习分割算法能力集能力集变化使能错误 
		/// </summary>
		MVD_ALG_STS_ERR_MVBCNN_SEG_ABILITY_ARG_CHANGE_FLAG = 270585873,
		/// <summary>
		/// ch: 内存空间错误，不支持的内存空间
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_VCA_MEM_SPACE = 18034688,
		/// <summary>
		/// ch: 开发平台错误，只可用于海思平台 
		/// </summary> 
		MVALG_STS_ERR_MVBCNN_FEWSHOT_VCA_PLAT = 18034689,
		/// <summary>
		/// ch: 内部HKANN库处理异常 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_VCA = 18034690,
		/// <summary>
		/// ch: 最大查找个数参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_OBJ_NUM = 18034691,
		/// <summary>
		/// ch: 最小置信度参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MIN_SCORE = 18034692,
		/// <summary>
		/// ch: 最大重叠率参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_OVERLAP = 18034693,
		/// <summary>
		/// ch: 缩放时填充的数据值参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_RESIZE_FILL_VAL = 18034694,
		/// <summary>
		/// ch: 缩放方法参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_RESIZE_TYPE = 18034695,
		/// <summary>
		/// ch: 起始角度参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_START_ANGLE = 18034696,
		/// <summary>
		/// ch: 终止角度参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_END_ANGLE = 18034697,
		/// <summary>
		/// ch: 最小宽度参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MIN_WIDTH = 18034698,
		/// <summary>
		/// ch: 最大宽度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_WIDTH = 18034699,
		/// <summary>
		/// ch: 最小高度参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MIN_HEIGHT = 18034700,
		/// <summary>
		/// ch: 最大高度参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_HEIGHT = 18034701,
		/// <summary>
		/// ch: 角度筛选使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_ANGLE_ENABLE = 18034702,
		/// <summary>
		/// ch: 宽度筛选使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_WIDTH_ENABLE = 18034703,
		/// <summary>
		/// ch: 高度筛选使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_HEIGHT_ENABLE = 18034704,
		/// <summary>
		/// ch: 目标框合并使能参数错误	 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_OBJ_COMBINE_ENABLE = 18034705,
		/// <summary>
		/// ch: 排序方式参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_ORDER_TYPE = 18034706,
		/// <summary>
		/// ch: 出界是否过滤参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_OUT_FILTER_ENABLE = 18034707,
		/// <summary>
		/// ch: 最大覆盖率参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_COVERAGE = 18034708,
		/// <summary>
		/// ch: 目标保留在ROI内的面积比参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_OUT_FILTER_SCORE = 18034709,
		/// <summary>
		/// ch: 前向预测模块未使能
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_MODU_DETECT_DISABLE = 18034710,
		/// <summary>
		/// ch: 不同类别非极大抑制使能错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_DIFFCLASSNMS_ENABLE = 18034711,
		/// <summary>
		/// ch: 最小面积参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MIN_AREA = 18034712,
		/// <summary>
		/// ch: 最大面积参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_AREA = 18034713,
		/// <summary>
		/// ch: 面积筛选使能参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_AREA_ENABLE = 18034714,
		/// <summary>
		/// ch: 掩模阈值参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_INSSEG_MASK_THRESH = 18034715,
		/// <summary>
		/// ch: NMS阈值参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_NMS_THRESH = 18034716,
		/// <summary>
		/// ch: NMS之前最小面积参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MIN_NMS_AREA = 18034717,
		/// <summary>
		/// ch: NMS之前最大前面积参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_MAX_NMS_AREA = 18034718,
		/// <summary>
		/// ch: gallery最优分辨率设置使能错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_OPTIMAL_SIZE_ENABLE = 18034719,
		/// <summary>
		/// ch: gallery最优分辨率参数设置错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_PARAM_OPTIMAL_SIZE = 18034720,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集模块使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_MODU_ENABLE = 18034721,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集最大图像宽高参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_MAX_IMAGE_SIZE = 18034722,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集模型句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_MODEL_HANDEL = 18034723,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集CUDA句柄参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_CUDA_HANDLE = 18034724,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集线程数参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_THREAD_NUM = 18034725,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集线程句柄参数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_THREAD_HANDLE = 18034726,
		/// <summary>
		/// ch: 模板注册类别ID错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_INVALID_REGISTER_ID = 18034727,
		/// <summary>
		/// ch: 模板注册类别名错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_MISSING_REGISTER_LABEL_NAME = 18034728,
		/// <summary>
		/// ch: 模板注册最大图像数目错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_OVER_MAX_SINGLE_REGISTER_NUM = 18034729,
		/// <summary>
		/// ch: 模板注册最大类别数错误 
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_OVER_MAX_REGISTER_CLASS_NUM = 18034730,
		/// <summary>
		/// ch: 模板删除ID错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_INVALID_DELETE_ID = 18034731,
		/// <summary>
		/// ch: 模板删除类别名错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_INVALID_DELETE_CLASS_NAME = 18034732,
		/// <summary>
		/// ch: 模板的版本错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_INVALID_GALLERY = 18034733,
		/// <summary>
		/// ch: 模板的对应的建模子类号错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_INVALID_GALLERY_TYPE = 18034734,
		/// <summary>
		/// ch: 模板注册图像数目错误	
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_INVALID_REGISTER_NUM = 18034735,
		/// <summary>
		/// ch: 模型与处理类型不匹配
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_WRONG_MODEL_TYPE = 18034736,
		/// <summary>
		/// ch: 模板ROI位于图像外
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_SHOT_RECT_SIZE = 18034737,
		/// <summary>
		/// ch: 模板内类别数目错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_GALLERY_CLASS_NUM = 18034738,
		/// <summary>
		/// ch: 模板内类别实例错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_GALLERY_SHOT_NUM = 18034739,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集多NPU使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_MULTI_NPUS_ENABLE = 18034740,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集批处理使能参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_BATCH_SIZE_ENABLE = 18034741,
		/// <summary>
		/// ch: 深度学习fewshot检测/分割算法能力集最大批处理参数错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_ABILITY_ARG_MAX_BATCH_SIZE = 18034742,
		/// <summary>
		/// ch: 模板类别错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_GALLERY_INVALID_LABEL_NAME = 18034944,
		/// <summary>
		/// ch: 模板ROI宽度错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_GALLERY_SHOT_WIDTH = 18034945,
		/// <summary>
		/// ch: 模板ROI高度错误
		/// </summary>
		MVALG_STS_ERR_MVBCNN_FEWSHOT_GALLERY_SHOT_HEIGHT = 18034946,
		/// <summary>
		/// ch: 错误或无效的句柄 | en:Error or invalid handle
		/// </summary>
		MVD_CAM_E_HANDLE = int.MinValue,
		/// <summary>
		/// ch: 不支持的功能 | en:Not supported function
		/// </summary>
		MVD_CAM_E_SUPPORT = -2147483647,
		/// <summary>
		/// ch: 缓存已满 | en: Cache is full
		/// </summary>
		MVD_CAM_E_BUFOVER = -2147483646,
		/// <summary>
		/// ch: 函数调用顺序错误 | en:Function calling order error
		/// </summary>
		MVD_CAM_E_CALLORDER = -2147483645,
		/// <summary>
		/// ch: 错误的参数 | en:Incorrect parameter
		/// </summary>
		MVD_CAM_E_PARAMETER = -2147483644,
		/// <summary>
		/// ch: 资源申请失败 | en:Applying resource failed
		/// </summary>
		MVD_CAM_E_RESOURCE = -2147483642,
		/// <summary>
		/// ch: 无数据 | en:No data
		/// </summary>
		MVD_CAM_E_NODATA = -2147483641,
		/// <summary>
		/// ch: 前置条件有误，或运行环境已发生变化 | en:Precondition error, or running environment changed
		/// </summary>
		MVD_CAM_E_PRECONDITION = -2147483640,
		/// <summary>
		/// ch: 版本不匹配 | en:Version mismatches
		/// </summary> 
		MVD_CAM_E_VERSION = -2147483639,
		/// <summary>
		/// ch: 传入的内存空间不足 | en:Insufficient memory
		/// </summary> 
		MVD_CAM_E_NOENOUGH_BUF = -2147483638,
		/// <summary>
		/// ch: 异常图像，可能是丢包导致图像不完整 | en:Abnormal image, maybe incomplete image because of lost packet
		/// </summary>
		MVD_CAM_E_ABNORMAL_IMAGE = -2147483637,
		/// <summary>
		/// ch: 动态导入DLL失败 | en:Load library failed
		/// </summary>
		MVD_CAM_E_LOAD_LIBRARY = -2147483636,
		/// <summary>
		/// ch: 没有可输出的缓存 | en:No Avaliable Buffer
		/// </summary> 
		MVD_CAM_E_NOOUTBUF = -2147483635,
		/// <summary>
		/// ch: 未知的错误 | en:Unknown error
		/// </summary> 
		MVD_CAM_E_UNKNOW = -2147483393,
		/// <summary>
		/// ch: 通用错误 | en:General error
		/// </summary> 
		MVD_CAM_E_GC_GENERIC = -2147483392,
		/// <summary>
		/// ch: 参数非法 | en:Illegal parameters
		/// </summary>
		MVD_CAM_E_GC_ARGUMENT = -2147483391,
		/// <summary>
		/// ch: 值超出范围 | en:The value is out of range
		/// </summary> 
		MVD_CAM_E_GC_RANGE = -2147483390,
		/// <summary>
		/// ch: 属性 | en:Property
		/// </summary> 
		MVD_CAM_E_GC_PROPERTY = -2147483389,
		/// <summary>
		/// ch: 运行环境有问题 | en:Running environment error
		/// </summary> 
		MVD_CAM_E_GC_RUNTIME = -2147483388,
		/// <summary>
		/// ch: 逻辑错误 | en:Logical error
		/// </summary> 
		MVD_CAM_E_GC_LOGICAL = -2147483387,
		/// <summary>
		/// ch: 节点访问条件有误 | en:Node accessing condition error
		/// </summary>
		MVD_CAM_E_GC_ACCESS = -2147483386,
		/// <summary>
		/// ch: 超时 | en:Timeout
		/// </summary> 
		MVD_CAM_E_GC_TIMEOUT = -2147483385,
		/// <summary>
		/// ch: 转换异常 | en:Transformation exception
		/// </summary> 
		MVD_CAM_E_GC_DYNAMICCAST = -2147483384,
		/// <summary>
		/// ch: GenICam未知错误 | en:GenICam unknown error
		/// </summary> 
		MVD_CAM_E_GC_UNKNOW = -2147483137,
		/// <summary>
		/// ch: 命令不被设备支持 | en:The command is not supported by device
		/// </summary> 
		MVD_CAM_E_NOT_IMPLEMENTED = -2147483136,
		/// <summary>
		/// ch: 访问的目标地址不存在 | en:The target address being accessed does not exist
		/// </summary>
		MVD_CAM_E_INVALID_ADDRESS = -2147483135,
		/// <summary>
		/// ch: 目标地址不可写 | en:The target address is not writable
		/// </summary> 
		MVD_CAM_E_WRITE_PROTECT = -2147483134,
		/// <summary>
		/// ch: 设备无访问权限 | en:No permission
		/// </summary> 
		MVD_CAM_E_ACCESS_DENIED = -2147483133,
		/// <summary>
		/// ch: 设备忙，或网络断开 | en:Device is busy, or network disconnected
		/// </summary> 
		MVD_CAM_E_BUSY = -2147483132,
		/// <summary>
		/// ch: 网络包数据错误 | en:Network data packet error
		/// </summary> 
		MVD_CAM_E_PACKET = -2147483131,
		/// <summary>
		/// ch: 网络相关错误 | en:Network error
		/// </summary>
		MVD_CAM_E_NETER = -2147483130,
		/// <summary>
		/// ch: 设备IP冲突 | en:Device IP conflict
		/// </summary> 
		MVD_CAM_E_IP_CONFLICT = -2147483103,
		/// <summary>
		/// ch: 读usb出错 | en:Reading USB error
		/// </summary> 
		MVD_CAM_E_USB_READ = -2147482880,
		/// <summary>
		/// ch: 写usb出错 | en:Writing USB error
		/// </summary> 
		MVD_CAM_E_USB_WRITE = -2147482879,
		/// <summary>
		/// ch: 设备异常 | en:Device exception
		/// </summary> 
		MVD_CAM_E_USB_DEVICE = -2147482878,
		/// <summary>
		/// ch: GenICam相关错误 | en:GenICam error
		/// </summary> 
		MVD_CAM_E_USB_GENICAM = -2147482877,
		/// <summary>
		/// ch: 带宽不足  该错误码新增 | en:Insufficient bandwidth, this error code is newly added
		/// </summary> 
		MVD_CAM_E_USB_BANDWIDTH = -2147482876,
		/// <summary>
		/// ch: 驱动不匹配或者未装驱动 | en:Driver mismatch or unmounted drive
		/// </summary> 
		MVD_CAM_E_USB_DRIVER = -2147482875,
		/// <summary>
		/// ch: USB未知的错误 | en:USB unknown error
		/// </summary> 
		MVD_CAM_E_USB_UNKNOW = -2147482625,
		/// <summary>
		/// ch: 升级固件不匹配 | en:Firmware mismatches
		/// </summary> 
		MVD_CAM_E_UPG_FILE_MISMATCH = -2147482624,
		/// <summary>
		/// ch: 升级固件语言不匹配 | en:Firmware language mismatches
		/// </summary> 
		MVD_CAM_E_UPG_LANGUSGE_MISMATCH = -2147482623,
		/// <summary>
		/// ch: 升级冲突（设备已经在升级了再次请求升级即返回此错误） | en:Upgrading conflicted (repeated upgrading requests during device upgrade)
		/// </summary> 
		MVD_CAM_E_UPG_CONFLICT = -2147482622,
		/// <summary>
		/// ch: 升级时相机内部出现错误 | en:Camera internal error during upgrade
		/// </summary> 
		MVD_CAM_E_UPG_INNER_ERR = -2147482621,
		/// <summary>
		/// ch: 升级时未知错误 | en:Unknown error during upgrade
		/// </summary> 
		MVD_CAM_E_UPG_UNKNOW = -2147482369,
		/// <summary>
		///  ch: cpu不支持优化代码中的指令集 | en: CPU does not support optimizing instruction sets in code
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_CPUID = -2146434816,
		/// <summary>
		/// ch: 数据step不正确（除HKA_IMAGE结构体之外） | en: Wrong with the 'step' of data
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_STEP = -2146434815,
		/// <summary>
		/// ch: 数据大小不正确（一维数据len，二维数据的HKA_SIZE） | en: Incorrect length or size of data
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_DATA_SIZE = -2146434814,
		/// <summary>
		/// ch: 参数范围不正确 | en: Incorrect parameter range
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_BAD_ARG = -2146434813,
		/// <summary>
		/// ch: 算法库使用期限错误 | en: Error in limit of use of the Algorithmic Library
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_EXPIRE = -2146434812,
		/// <summary>
		/// ch: 加密错误 | en: Encryption error
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_ENCRYPT = -2146434811,
		/// <summary>
		/// ch: 回调函数出错 | en: Wrong with the callback function
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_CALL_BACK = -2146434810,
		/// <summary>
		/// ch: 超过HKA限定最大内存 | en: Exceeding the maximum memory limit
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_OVER_MAX_MEM = -2146434809,
		/// <summary>
		/// ch: 函数参数指针为空（共用） | en: Empty function pointer is not allowed
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_NULL_PTR = -2146434808,
		/// <summary>
		/// ch: param_num参数不正确 | en: Incorrect name of the parameter
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_PARAM_NUM = -2146434807,
		/// <summary>
		/// ch: value参数不正确或者超出范围 | en: Incorrect value of the parameter
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_PARAM_VALUE = -2146434806,
		/// <summary>
		/// ch: index参数不正确 | en: Incorrect index of the parameter
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_PARAM_INDEX = -2146434805,
		/// <summary>
		/// ch: 子处理时输入、输出参数大小不正确 | en: Incorrect size of I/O parameter for sub-processing
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_FUNC_SIZE = -2146434804,
		/// <summary>
		/// ch: 子处理类型不正确 | en: Incorrect sub-processing type
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_FUNC_TYPE = -2146434803,
		/// <summary>
		/// ch: 处理时输入、输出参数大小不正确 | en: Incorrect size of I/O parameter for processing
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_PRC_SIZE = -2146434802,
		/// <summary>
		/// ch: 处理类型不正确 | en: Incorrect processing type
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_PRC_TYPE = -2146434801,
		/// <summary>
		/// ch: 设置、获取参数输入、输出结构体大小不正确 | en: Incorrect size of the I/O structure for setting or getting parameter
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_CFG_SIZE = -2146434800,
		/// <summary>
		/// ch: 设置、获取参数类型不正确 | en: Incorrect type of the I/O structure for setting or getting parameter
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_CFG_TYPE = -2146434799,
		/// <summary>
		/// ch: 图像数据存储地址为空（某个分量） | en: Empty address of the image data channel
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_IMG_DATA_NULL = -2146434798,
		/// <summary>
		/// ch: 图像宽高与step参数不匹配（检测宽高和像素深度） | en: Mismatch between image width and height and 'step' parameters
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_IMG_STEP = -2146434797,
		/// <summary>
		/// ch: 图像宽高不正确或者超出范围 | en: Incorrect or out-of-range image width or height
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_IMG_SIZE = -2146434796,
		/// <summary>
		/// ch: 图像格式不正确或者不支持 | en: Incorrect or out-of-range pixel format
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_IMG_FORMAT = -2146434795,
		/// <summary>
		/// ch: 内存地址不满足对齐要求 | en: Memory address does not meet alignment requirements
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_MEM_ADDR_ALIGN = -2146434794,
		/// <summary>
		/// ch: 内存空间大小不满足对齐要求 | en: Memory space size does not meet alignment requirements
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_MEM_SIZE_ALIGN = -2146434793,
		/// <summary>
		/// ch: 内存空间大小不够 | en: Insufficient memory space
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_MEM_LACK = -2146434792,
		/// <summary>
		/// ch: 内存对齐不满足要求 | en: Memory alignment does not meet requirements
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_MEM_ALIGN = -2146434791,
		/// <summary>
		/// ch: 内存地址为空 | en: Empty address of the memory
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_MEM_NULL = -2146434790,
		/// <summary>
		/// ch: ABILITY存在无效参数(检查Init时的图像最大宽高) | en: Wrong with the algorithm ability(e.g. not supported)
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_ABILITY_ARG = -2146434789,
		/// <summary>
		/// ch: 算法库未知错误 | en: Unknown Error in Algorithmic Library
		/// </summary>
		MVD_ALGORITHM_E_ALGORITHM_UNKNOW = -2146434788,
		/// <summary>
		/// ch: 算法警告值 | en: Warning in Algorithmic Library
		/// </summary> 
		MVD_ALGORITHM_E_ALGORITHM_WARNING = -2146434787,
		/// <summary>
		/// ch: ROI区域比算法要求的最小宽高、模块要求的最小参数值小 | en: The ROI area is smaller than the minimum threshold required by the algorithm and the module.
		/// </summary>
		MVD_ALGORITHM_E_OCR_ROI_TOO_SMALL = MVD_E_RENDER_IMG_FORMAT,
		/// <summary>
		/// ch: 图像能力集太小 | en: Beyond the limits of image ability.
		/// </summary>
		MVD_ALGORITHM_E_OCR_IMG_ABILITY_TOO_SMALL = MVD_E_RENDER_IMG_TYPE,
		/// <summary>
		/// ch: 字符能力集太小 | en: Beyond the limits of character ability.
		/// </summary>
		MVD_ALGORITHM_E_OCR_CHARACTER_ABILITY_TOO_SMALL = MVD_E_RENDER_IMG_SIZE,
		/// <summary>
		/// ch: 字库增加处理样本数量错误 | en: Font library failed to increase the number of samples.
		/// </summary>
		MVD_ALGORITHM_E_OCR_GENERATE_DATA_NUM = MVD_E_RENDER_IMG_ENCOD,
		/// <summary>
		/// ch: 字符识别处理缺失字库文件 | en: Character recognition processing failed because of missing font files.
		/// </summary>
		MVD_ALGORITHM_E_OCR_NO_FONT_FILE = MVD_E_RENDER_CANVAS_ZOOM,
		/// <summary>
		/// ch: 字库训练处理缺失训练样本 | en: Font training failed because of missing training samples.
		/// </summary>
		MVD_ALGORITHM_E_OCR_NO_TRAIN_DATA = MVD_E_RENDER_CANVAS_PARAM,
		/// <summary>
		/// ch: 文本检测区域的中心点不在图像内部 | en: The center point of the area to detect text is not inside the image.
		/// </summary>
		MVD_ALGORITHM_E_OCR_TEXT_DETECT_OUT = MVD_E_RENDER_GENERIC,
		/// <summary>
		/// ch: 文本检测区域的高、宽小于算法支持的最小字符大小 | en: The width or height of the area to detect text is smaller than the minimum character size supported by the algorithm.
		/// </summary>
		MVD_ALGORITHM_E_OCR_TEXT_BOX_INVALID = MVD_E_RENDER_ILLEGAL_SHAPE,
		/// <summary>
		/// ch: 字库类型不匹配 | en: Font type does not match.
		/// </summary>
		MVD_ALGORITHM_E_OCR_FONT_MISMATCH = MVD_E_RENDER_MEMORY,
		/// <summary>
		/// ch: 字库没有与待识别字符类型一致的样本 | en: The font does not have a sample that matches the type of character to be recognized.
		/// </summary>
		MVD_ALGORITHM_E_OCR_FONT_NO_MATCHING_TYPE = -2146434288,
		/// <summary>
		/// ch: 输入处理类型与模块使能不一致 | en: Input processing type is inconsistent with the status of ModuleEnable
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_PRC_TYPE_OR_MODU_ENABLE = -2146434047,
		/// <summary>
		/// ch: 输入box个数小于MVBMKINSP_MARK_NUM_MIN或大于设置的最大个数 | en: The number of boxes input less than MVBMKINSP_MARK_NUM_MIN or greater than the maximum number set
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_MARK_NUM = -2146434046,
		/// <summary>
		/// ch: 输入字符box大小范围超过图像范围或与之前生成模型使用的mark_box大小不一样 | en: The input character box size range is larger than the image range or different from the mark_box size used in the previous generation model.
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_MARK_BOX_SCALE = -2146434045,
		/// <summary>
		/// ch: proc_order输入异常 | en: Input exception
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_PROC_ORDER = -2146434044,
		/// <summary>
		/// ch: 模型输入类型不一致 | en: Model input type is inconsistent.
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_MODEL_TYPE = -2146434043,
		/// <summary>
		/// ch: 模型输入版本不一致 | en: Model input version is inconsistent.
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_MODEL_VERSION = -2146434042,
		/// <summary>
		/// ch: 模型保存的参数异常 | en: Abnormal parameter saved by the model.
		/// </summary>
		MVD_ALGORITHM_E_MKINSP_MODEL_CONFIG = -2146434041,
		/// <summary>
		/// ch: 特征不足：特征模板生成失败 | en: Failed to generate the feature modle
		/// </summary>
		MVD_ALGORITHM_E_PATMATCH_FEATURES_NOT_ENOUGH = -2146433791,
		/// <summary>
		/// ch: 算法超时：匹配时超出预定处理时间 | en: Exceeded the scheduled processing time when matching
		/// </summary>
		MVD_ALGORITHM_E_PATMATCH_MATCH_TIME_OUT = -2146433790,
		/// <summary>
		/// ch: 算法版本号出错：模板版本号与匹配版本不符 | en: Algorithm version error. The version of the feature modle does not match the version of Matching.
		/// </summary>
		MVD_ALGORITHM_E_PATMATCH_MATCH_VERSION_ERROR = -2146433789,
		/// <summary>
		/// ch: 模型版本号出错：模板版本号与当前版本不符 | en: Modle version error. The version of modle input does match the current version.
		/// </summary>
		MVD_ALGORITHM_E_PATMATCH_MODEL_VERSION_ERROR = -2146433788,
		/// <summary>
		/// ch: 建模时，模型内存申请失败 | en: Model memory allocation failed while modeling.
		/// </summary>
		MVD_ALGORITHM_E_PATMATCH_MALLOC_MODEL_BUFF_FAIL = -2146433787
	}

	/// <summary>
	/// CH: 输入图像数据信息结构 | EN: Input image data information structure
	/// </summary>
	[Serializable]
	public struct IMVS_PF_IMAGE_DATA_3D_INFO
	{
		/// <param name="pImgData">CH: 图像数据 | EN: Image data</param>
		public IntPtr pImgData;

		/// <param name="iImgDataLen">CH: 图像数据长度 | EN: Image data length</param>
		public int iImgDataLen;

		/// <param name="iWidth">CH: 图像宽度 | EN: Image width</param>
		public int iWidth;

		/// <param name="iHeight">CH: 图像高度 | EN: Image height</param>
		public int iHeight;

		/// <param name="pIntensityImgData">CH: 亮度图图像数据 | EN: Image data</param>
		public IntPtr pIntensityImgData;

		/// <param name="iIntensityImgDataLen">CH: 亮度图图像数据长度 | EN: Image data length</param>
		public int iIntensityImgDataLen;

		/// <param name="iImgFormat">CH: 图像像素格式 | EN: Image pixel format </param>
		public int iImgFormat;

		/// <param name="iImgMode">CH: 图像类型（1：原始图 4：轮廓图 7：深度图） | EN: Image pixel format (1:Origin 4:CloudPoint 7:Range)</param>
		public int iImgMode;

		/// <param name="fScaleX">CH: 尺度X | EN: Scale X</param>
		public float fScaleX;

		/// <param name="fScaleY">CH: 尺度Y | EN: Scale Y</param>
		public float fScaleY;

		/// <param name="fScaleZ">CH: 尺度Z | EN: Scale Z</param>
		public float fScaleZ;

		/// <param name="iOffsetX">CH: 偏移X | EN: Offset X</param>
		public int iOffsetX;

		/// <param name="iOffsetY">CH: 偏移Y | EN: Offset Y</param>
		public int iOffsetY;

		/// <param name="iOffsetZ">CH: 偏移Z | EN: Offset Z</param>
		public int iOffsetZ;
	}

	/// <summary>
	/// CH: 用户输入3D图像信息结构 | EN: User input image information structure
	/// </summary>
	[Serializable]
	public struct IMVS_PF_INPUT_IMAGE_3D_INFO
	{
		/// <param name="nModuleID">CH: 模块ID | EN: Module ID</param>
		public uint nModuleID;

		/// <param name="nDataType">CH: 图像数据类型(0:二进制数据, 1:路径) | EN: Image pixel format(0:Binary data, 1:Path)</param>
		public int nDataType;

		/// <param name="strImagePath">CH: 图像文件路径 | EN: Image file path</param>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string strImagePath;

		/// <param name="stImageDataInfo">CH: 图像数据 | EN: Image data</param>
		public IMVS_PF_IMAGE_DATA_3D_INFO stImageDataInfo;

		public int nIsP2PSend;

		/// <param name="nReserved">CH: 保留字段 | EN: Reserved</param>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public uint[] nReserved;
	}

	/// <summary>
	/// CH: 无错误 | EN: No error
	/// </summary>
	public const int IMVS_EC_OK = 0;

	/// <summary>
	/// CH: 版本错误 | EN: Incorrect version No.
	/// </summary>
	public const int IMVS_EC_VERSION = -536870912;

	/// <summary>
	/// CH: 参数错误 | EN: Incorrect parameter
	/// </summary>
	public const int IMVS_EC_PARAM = -536870911;

	/// <summary>
	/// CH: 资源创建失败 | EN: Applying for resource failed
	/// </summary>
	public const int IMVS_EC_RESOURCE_CREATE = -536870910;

	/// <summary>
	/// CH: 内存不足 | EN: Insufficient memory
	/// </summary>
	public const int IMVS_EC_OUTOFMEMORY = -536870909;

	/// <summary>
	/// CH: 指针转换错误 | EN: Pointer conversion failed
	/// </summary>
	public const int IMVS_EC_POINTER_CAST = -536870908;

	/// <summary>
	/// CH: 系统内存使用率超过阈值 | EN: The CPU usage is too high
	/// </summary>
	public const int IMVS_EC_MEMORY_BEYOND_THRESHOLD = -536870907;

	/// <summary>
	/// CH: 句柄无效 | EN: Invalid or incorrect handle
	/// </summary>
	public const int IMVS_EC_INVALID_HANDLE = -536870906;

	/// <summary>
	/// CH: 操作不支持 | EN: This operation is not supported
	/// </summary>
	public const int IMVS_EC_NOT_SUPPORT = -536870905;

	/// <summary>
	/// CH: 资源未初始化或未准备好 | EN: Initializing resource failed
	/// </summary>
	public const int IMVS_EC_NOT_READY = -536870904;

	/// <summary>
	/// CH: CH: 等待超时 | EN: Waiting timed out
	/// </summary>
	public const int IMVS_EC_WAIT_TIMEOUT = -536870903;

	/// <summary>
	/// CH: 指针为空 | EN: Null pointer
	/// </summary>
	public const int IMVS_EC_NULL_PTR = -536870902;

	/// <summary>
	/// CH: 进程启动失败 | EN: Starting up process failed
	/// </summary>
	public const int IMVS_EC_PROCESS_START_FAIL = -536870901;

	/// <summary>
	/// CH: 客户端程序已启动 | EN: The client is already started up
	/// </summary>
	public const int IMVS_EC_PROCESS_ALREADY_START = -536870900;

	/// <summary>
	/// CH: 正在加载方案 | EN: Solution is loading
	/// </summary>
	public const int IMVS_EC_SOLUTION_LOADING = -536870899;

	/// <summary>
	/// CH: 正在保存方案 | EN: Solution is saving
	/// </summary>
	public const int IMVS_EC_SOLUTION_SAVING = -536870898;

	/// <summary>
	/// CH: 接口调用顺序错误 | EN: Incorrect API call sequence
	/// </summary>
	public const int IMVS_EC_CALL_ORDER = -536870897;

	/// <summary>
	/// CH: 动态库加载失败 | EN: Loading dynamic library failed
	/// </summary>
	public const int IMVS_EC_LOAD_LIBRARY = -536870896;

	/// <summary>
	/// CH: 获取函数地址失败 | EN: Getting function address failed
	/// </summary>
	public const int IMVS_EC_GET_FUN_ADDRESS = -536870895;

	/// <summary>
	/// CH: 参数缓冲区长度不足 | EN: Insufficient buffer size
	/// </summary>
	public const int IMVS_EC_PARAM_BUF_LEN = -536870894;

	/// <summary>
	/// CH: 正在获取底层信息 | EN: Getting underlying information
	/// </summary>
	public const int IMVS_EC_GETTING_PLAT_INFO = -536870893;

	/// <summary>
	/// CH: 索引值越界 | EN: The index No. is out of limit
	/// </summary>
	public const int IMVS_EC_INDEX_OUT_OF_BOUNDARY = -536870892;

	/// <summary>
	/// CH: 打开共享内存失败 | EN: Opening sharing memory failed
	/// </summary>
	public const int IMVS_EC_OPEN_FILEMAPPING = -536870891;

	/// <summary>
	/// CH: 开启线程失败 | EN: Starting thread failed
	/// </summary>
	public const int IMVS_EC_THREAD_START_FAIL = -536870890;

	/// <summary>
	/// CH: 协议解析错误 | EN: Parsing protocol failed
	/// </summary>
	public const int IMVS_EC_PROTOCOL = -536870889;

	/// <summary>
	/// CH: 数据错误 | EN: Incorrect data
	/// </summary>
	public const int IMVS_EC_DATA_ERROR = -536870888;

	/// <summary>
	/// CH: 操作未实现 | EN: No operation is done
	/// </summary>
	public const int IMVS_EC_NOT_IMPLEMENTED = -536870887;

	/// <summary>
	/// CH: 数据大小超过上限 | EN: No more data is allowed
	/// </summary>
	public const int IMVS_EC_DATA_OVER_SIZE = -536870886;

	/// <summary>
	/// CH: 前置条件有误 | EN: Pre-condition is wrong
	/// </summary>
	public const int IMVS_EC_PRECONDITION = -536870885;

	/// <summary>
	/// CH: 运行环境错误 | EN: Running environment error
	/// </summary>
	public const int IMVS_EC_RUNTIME = -536870884;

	/// <summary>
	/// CH: 正在关闭方案 | EN: Closing solution
	/// </summary>
	public const int IMVS_EC_SOLUTION_CLOSING = -536870883;

	/// <summary>
	/// CH: 静默执行中   | EN: In silent execution
	/// </summary>
	public const int IMVS_EC_SILENT_EXEC_STATUS = -536870882;

	/// <summary>
	/// CH: 动态IO参数无法找到 | EN: No dynamicIO param found
	/// </summary>
	public const int IMVS_EC_DYNAMICIO_PARAM_NOT_FOUND = -536870881;

	/// <summary>
	/// CH: 关闭共享内存失败 | EN: Close sharing memory failed
	/// </summary>
	public const int IMVS_EC_CLOSE_FILEMAPPING = -536870880;

	/// <summary>
	/// CH: 名称不合法 | EN: Name invalid
	/// </summary>
	public const int IMVS_EC_NAME_INVALID = -536870879;

	/// <summary>
	/// CH: 缓存队列满 | EN: Cache queue full
	/// </summary>
	public const int IMVS_EC_MODULE_CACHE_FULL = -536870878;

	/// <summary>
	/// CH: 存图大文件 | EN: Save image to big file
	/// </summary>
	public const int IMVS_EC_SAVE_BIG_FILE = -536870877;

	/// <summary>
	/// CH: 数据队列为空 | EN: Data queue is empty
	/// </summary>
	public const int IMVS_EC_DATA_QUEUE_EMPTY = -536870876;

	/// <summary>
	/// CH: 未知错误 | EN: Unknown error
	/// </summary>
	public const int IMVS_EC_UNKNOWN = -536870657;

	/// <summary>
	/// CH: socket创建失败 | EN: Creating socket failed
	/// </summary>
	public const int IMVS_EC_COMMU_SOCKET_CREAT = -536870656;

	/// <summary>
	/// CH: socket无效 | EN: Invalid socket
	/// </summary>
	public const int IMVS_EC_COMMU_SOCKET_INVALID = -536870655;

	/// <summary>
	/// CH: 打开串口失败 | EN: Connecting to serial port failed
	/// </summary>
	public const int IMVS_EC_COMMU_SERIAL_OPEN = -536870654;

	/// <summary>
	/// CH: 地址无效 | EN: Invalid address
	/// </summary>
	public const int IMVS_EC_COMMU_INVALID_ADDRESS = -536870653;

	/// <summary>
	/// CH: 地址已被使用 | EN: Unavailable address
	/// </summary>
	public const int IMVS_EC_COMMU_ADDRESS_INUSE = -536870652;

	/// <summary>
	/// CH: 连接失败 | EN: Connection failed
	/// </summary>
	public const int IMVS_EC_COMMU_CONNECT = -536870651;

	/// <summary>
	/// CH: 断开连接失败 | EN: Disconnection failed
	/// </summary>
	public const int IMVS_EC_COMMU_DISCONNECT = -536870650;

	/// <summary>
	/// CH: 发送失败 | EN: Sending data failed
	/// </summary>
	public const int IMVS_EC_COMMU_SEND_FAIL = -536870649;

	/// <summary>
	/// CH: 接收超时 | EN: Response timed out
	/// </summary>
	public const int IMVS_EC_COMMU_RECV_TIMEOUT = -536870648;

	/// <summary>
	/// CH: 消息格式错误 | EN: Incorrect protocol format
	/// </summary>
	public const int IMVS_EC_COMMU_MESSAGE_FORMAT = -536870647;

	/// <summary>
	/// CH: 报文长度超出限制 | EN: Maximum message length reached
	/// </summary>
	public const int IMVS_EC_COMMU_MSG_TOO_LONG = -536870646;

	/// <summary>
	/// CH: 心跳异常 | EN: Heartbeat exception
	/// </summary>
	public const int IMVS_EC_COMMU_HEARTBEAT = -536870645;

	/// <summary>
	/// CH: 端口无效 | EN: Invalid port
	/// </summary>
	public const int IMVS_EC_COMMU_INVALID_PORT = -536870644;

	/// <summary>
	/// CH: 创建路径错误 | EN: Incorrect file path
	/// </summary>
	public const int IMVS_EC_FILE_MKDIR = -536870400;

	/// <summary>
	/// CH: 文件无法打开 | EN: Opening file failed
	/// </summary>
	public const int IMVS_EC_FILE_OPEN = -536870399;

	/// <summary>
	/// CH: 保存文件数据失败 | EN: Saving file failed
	/// </summary>
	public const int IMVS_EC_FILE_SAVE = -536870398;

	/// <summary>
	/// CH: 文件不存在 | EN: The file does not exist
	/// </summary>
	public const int IMVS_EC_FILE_NOT_FOUND = -536870397;

	/// <summary>
	/// CH: 文件格式错误 | EN: Incorrect file format
	/// </summary>
	public const int IMVS_EC_FILE_FORMAT = -536870396;

	/// <summary>
	/// CH: 文件压缩失败 | EN: Compressing file failed
	/// </summary>
	public const int IMVS_EC_FILE_COMPRESS = -536870395;

	/// <summary>
	/// CH: 文件解压失败 | EN: Decompressing file failed
	/// </summary>
	public const int IMVS_EC_FILE_DECOMPRESS = -536870394;

	/// <summary>
	/// CH: xml中element不存在 | EN: The "element" does not exist in the XML file
	/// </summary>
	public const int IMVS_EC_FILE_XML_ELEMENT = -536870393;

	/// <summary>
	/// CH: xml中Attribute不存在 | EN: The "attribute" does not exist in the XML file
	/// </summary>
	public const int IMVS_EC_FILE_XML_ATTRIBUTE = -536870392;

	/// <summary>
	/// CH: 文件路径长度超过系统最大值 | EN: The file path is too long
	/// </summary>
	public const int IMVS_EC_FILE_PATH_TOO_LONG = -536870391;

	/// <summary>
	/// CH: 文件被占用 | EN: The file is already occupied
	/// </summary>
	public const int IMVS_EC_FILE_BE_OCCUPIED = -536870390;

	/// <summary>
	/// CH: 目标路径为空 | EN: The target path is empty
	/// </summary>
	public const int IMVS_EC_FILE_PATH_EMPTY = -536870389;

	/// <summary>
	/// CH: 磁盘空间不足 | EN: Insufficient disk space
	/// </summary>
	public const int IMVS_EC_FILE_INSUFFICIENT_DISK_SPACE = -536870388;

	/// <summary>
	/// CH: 界面文件不存在 | EN: Interface file does not exist
	/// </summary>
	public const int IMVS_EC_FILE_UI_NOT_FOUND = -536870387;

	/// <summary>
	/// CH: 文件备份失败 | EN: Backup file failed
	/// </summary>
	public const int IMVS_EC_FILE_BACKUP = -536870386;

	/// <summary>
	/// CH: 目标路径无效 | EN: The target path is invalid
	/// </summary>
	public const int IMVS_EC_FILE_PATH_INVALID = -536870385;

	/// <summary>
	/// CH: 删除文件失败 | EN: Delete file failed
	/// </summary>
	public const int IMVS_EC_FILE_DELETE = -536870384;

	/// <summary>
	/// CH: 文件是只读的 | EN: File is read-only
	/// </summary>
	public const int IMVS_EC_FILE_READ_ONLY = -536870383;

	/// <summary>
	/// CH: 流程处于忙碌状态 | EN: The procedure is busy
	/// </summary>
	public const int IMVS_EC_MODULE_BUSY = -536870144;

	/// <summary>
	/// CH: 模块个数超出限制 | EN: Maximum number of modules reached
	/// </summary>
	public const int IMVS_EC_MODULE_COUNT_BEYOND = -536870143;

	/// <summary>
	/// CH: 模块不存在 | EN: The module does not exist
	/// </summary>
	public const int IMVS_EC_MODULE_NOT_FOUND = -536870142;

	/// <summary>
	/// CH: 模块已存在 | EN: The module already exists
	/// </summary>
	public const int IMVS_EC_MODULE_ALREADY_EXIST = -536870141;

	/// <summary>
	/// CH: 模块数量为0 | EN: No module
	/// </summary>
	public const int IMVS_EC_MODULE_EMPTY = -536870140;

	/// <summary>
	/// CH: 模块未注册 | EN: The module is not registered
	/// </summary>
	public const int IMVS_EC_MODULE_NOT_REGISTER = -536870139;

	/// <summary>
	/// CH: 模块订阅失败 | EN: Subscribing module failed
	/// </summary>
	public const int IMVS_EC_MODULE_SUBSCRIBE_FAIL = -536870138;

	/// <summary>
	/// CH: 流程控制模块出现异常（崩溃） | EN: Procedure control module exception
	/// </summary>
	public const int IMVS_EC_MODULE_PROC_CTRL_EXCEPT = -536870137;

	/// <summary>
	/// CH: 模块输入未配置完成 | EN: The module input parameter is not configured
	/// </summary>
	public const int IMVS_EC_MODULE_INPUT_CFG_UNDONE = -536870136;

	/// <summary>
	/// CH: 模块输入无法找到 | EN: No module input found
	/// </summary>
	public const int IMVS_EC_MODULE_INPUT_NOT_FOUND = -536870135;

	/// <summary>
	/// CH: 模块输入状态错误 | EN: Module input status error
	/// </summary>
	public const int IMVS_EC_MODULE_INPUT_STATUS = -536870134;

	/// <summary>
	/// CH: 模块输入个数错误 | EN: Incorrect number of module inputs
	/// </summary>
	public const int IMVS_EC_MODULE_INPUT_COUNT = -536870133;

	/// <summary>
	/// CH: 模块输入缓冲区长度太小 | EN: The buffer size of module input is too small
	/// </summary>
	public const int IMVS_EC_MODULE_INPUT_BUF_LEN = -536870132;

	/// <summary>
	/// CH: 参数不支持 | EN: This parameter is not supported
	/// </summary>
	public const int IMVS_EC_MODULE_PARAM_NOT_SUPPORT = -536870131;

	/// <summary>
	/// CH: 参数值无效 | EN: Invalid parameter value
	/// </summary>
	public const int IMVS_EC_MODULE_PARAM_VALUE_ILLEGAL = -536870130;

	/// <summary>
	/// CH: 参数类型错误 | EN: Incorrect parameter type
	/// </summary>
	public const int IMVS_EC_MODULE_PARAM_TYPE = -536870129;

	/// <summary>
	/// CH: 导入数据格式错误 | EN: The imported data format is incorrect
	/// </summary>
	public const int IMVS_EC_MODULE_IMPORT_DATA = -536870128;

	/// <summary>
	/// CH: 正在连续执行 | EN: Continuously executing
	/// </summary>
	public const int IMVS_EC_MODULE_CONTINUE_EXECUTE = -536870127;

	/// <summary>
	/// CH: 流程内的模块数量为0 | EN: No module in the procedure
	/// </summary>
	public const int IMVS_EC_MODULE_PROCEDURE_EMPTY = -536870126;

	/// <summary>
	/// CH: 模块心跳出现异常 | EN: Module heartbeat exception
	/// </summary>
	public const int IMVS_EC_MODULE_HEARTBEAT_EXCEPTION = -536870125;

	/// <summary>
	/// CH: 未找到订阅结果值 | EN: No subscription result found
	/// </summary>
	public const int IMVS_EC_MODULE_SUB_RST_NOT_FOUND = -536870124;

	/// <summary>
	/// CH: 模块输出无法找到 | EN: No module output found
	/// </summary>
	public const int IMVS_EC_MODULE_OUT_NOT_FOUND = -536870123;

	/// <summary>
	/// CH: 流程不存在 | EN: The procedure does not exist
	/// </summary>
	public const int IMVS_EC_MODULE_PROCEDURE_NOT_FOUND = -536870122;

	/// <summary>
	/// CH: 流程已存在 | EN: The procedure already exists
	/// </summary>
	public const int IMVS_EC_MODULE_PROCEDURE_ALREADY_EXIST = -536870121;

	/// <summary>
	/// CH: 创建算法模块失败 | EN: Creating algorithm module failed
	/// </summary>
	public const int IMVS_EC_MODULE_CREATE_ALG_MODULE_FAILED = -536870120;

	/// <summary>
	/// CH: 循环已存在 | EN: The loop already exists
	/// </summary>
	public const int IMVS_EC_MODULE_LOOP_ALREADY_EXIST = -536870119;

	/// <summary>
	/// CH: 循环不存在 | EN: The loop does not exist
	/// </summary>
	public const int IMVS_EC_MODULE_LOOP_NOT_FOUND = -536870118;

	/// <summary>
	/// CH: 未找到订阅记录 | EN: No subscription record found
	/// </summary>
	public const int IMVS_EC_MODULE_SUB_RECORD_NOT_FOUND = -536870117;

	/// <summary>
	/// CH: 订阅参数有误 | EN: Incorrect subscription parameter
	/// </summary>
	public const int IMVS_EC_MODULE_SUB_PARAM_ERROR = -536870116;

	/// <summary>
	/// CH: 流程处于禁用状态 | EN: Process is disabled
	/// </summary>
	public const int IMVS_EC_MODULE_PROCEDURE_DISABLED = -536870115;

	/// <summary>
	/// CH: 触发字符不匹配 | EN: Trigger string mismatch
	/// </summary>
	public const int IMVS_EC_MODULE_TRIGGERSTRING_NOT_MATCH = -536870114;

	/// <summary>
	/// CH: 全局脚本流程正在执行中 | EN: Global scripting process is executing
	/// </summary>
	public const int IMVS_EC_MODULE_GLOBALSCRIPT_PROCESSING = -536870113;

	/// <summary>
	/// CH: 全局脚本编译失败 | EN: Global script compilation failed
	/// </summary>
	public const int IMVS_EC_MODULE_GLOBALSCRIPT_COMPILE_FAIL = -536870112;

	/// <summary>
	/// CH: 分组已存在 | EN: Group already exists
	/// </summary>
	public const int IMVS_EC_MODULE_GROUP_ALREADY_EXIST = -536870111;

	/// <summary>
	/// CH: 分组不存在 | EN: Group does not exist
	/// </summary>
	public const int IMVS_EC_MODULE_GROUP_NOT_FOUND = -536870110;

	/// <summary>
	/// CH: 正在导出Group | EN: Exporting group
	/// </summary>
	public const int IMVS_EC_MODULE_GROUP_EXPORTING = -536870109;

	/// <summary>
	/// CH: 正在导入Group | EN: Importing group
	/// </summary>
	public const int IMVS_EC_MODULE_GROUP_IMPORTING = -536870108;

	/// <summary>
	/// CH: 超出嵌套层级 | EN: Beyond module nested layer
	/// </summary>
	public const int IMVS_EC_MODULE_BEYOND_NESTED_LAYER = -536870107;

	/// <summary>
	/// CH: 流程数量超出限制  | EN: Maximum number of process reached
	/// </summary>
	public const int IMVS_EC_PROCESS_COUNT_BEYOND = -536870106;

	/// <summary>
	/// CH: 名称重复  | EN: Name duplicated
	/// </summary>
	public const int IMVS_EC_MODULE_NAME_DUPLICATED = -536870105;

	/// <summary>
	/// CH: 流程包含在方案的界面文件中  | EN: The procedure is contained within interface file of solution
	/// </summary>
	public const int IMVS_EC_PROCESS_IN_VMFILE_SOL = -536870104;

	/// <summary>
	/// CH: 正在导出流程 | EN: Exporting procedure
	/// </summary>
	public const int IMVS_EC_MODULE_PROCEDURE_EXPORTING = -536870103;

	/// <summary>
	/// CH: 正在导入流程 | EN: Importing procedure
	/// </summary>
	public const int IMVS_EC_MODULE_PROCEDURE_IMPORTING = -536870102;

	/// <summary>
	/// CH: 字符缺陷粗定位失败 | EN: Character defect coarse positioning failed
	/// </summary>
	public const int IMVS_EC_MODULE_ROUGH_MATCH_FAIL = -536870101;

	/// <summary>
	/// CH: 粗糙度阈值大于最小匹配分数 | EN: The roughness threshold is greater than the minimum match score
	/// </summary>
	public const int IMVS_EC_MODULE_COARSE_OVER_SCORE = -536870100;

	/// <summary>
	/// CH: 两模块存在订阅关系但连线不可达 | EN: Subscribe one module output but connection does not exist
	/// </summary>
	public const int IMVS_EC_MODULE_SUB_CONNECTION_ERROR = -536870099;

	/// <summary>
	/// CH: 位置修正XY尺度值超出范围(0~10000) | EN： The value of scale X or Y is too big
	/// </summary>
	public const int IMVS_EC_MODULE_SCALE_VALUE_OVERFLOW = -536870098;

	/// <summary>
	/// CH: 字符缺陷精定位失败 | EN： Character defect fine positioning failed
	/// </summary>
	public const int IMVS_EC_MODULE_EXACT_MATCH_FAIL = -536870097;

	/// <summary>
	/// CH: 精定位信息改变 重新进行统计训练 | EN: Resume statistical training due to fine position change
	/// </summary>
	public const int IMVS_EC_MODULE_EXACT_MATCH_CHANGED = -536870096;

	/// <summary>
	/// CH: 模块输入IO个数为0 | EN： No input
	/// </summary>
	public const int IMVS_EC_MODULE_PARAM_INPUT = -536870095;

	/// <summary>
	/// CH: 订阅了全局变量 | EN： Subscribe global variable
	/// </summary>
	public const int IMVS_EC_MODULE_SUB_GLOBAL_VARIABLE = -536870094;

	/// <summary>
	/// CH: 存在加载失败模块 | EN： There are modules that failed to load
	/// </summary>
	public const int IMVS_EC_MODULE_HAS_LOAD_FAIL = -536870093;

	/// <summary>
	/// CH: 单一图像源存图数量大于120 | EN： The number of saved imaged of the imagesource is greater than 120 
	/// </summary>
	public const int IMVS_EC_MODULE_OVERRUN_IMAGECOUNT = -536870092;

	/// <summary>
	/// CH: 未匹配到目标或匹配个数异常 | EN： The target is not matched or the number of matches is abnormal
	/// </summary>
	public const int IMVS_EC_MODULE_INPECT_MATCH_FAIL = -536870091;

	/// <summary>
	/// CH: group输出关联模块执行失败 | EN：
	/// </summary>
	public const int IMVS_EC_MODULE_GROUP_OUTPUT_MODULE_FAILED = -536870090;

	public const int IMVS_EC_MODULE_DUPLICATED_BINDING = -536870089;

	public const int IMVS_EC_MODULE_NO_BINDING_INFO = -536870088;

	/// <summary>
	/// CH: 运行环境有问题 | EN: Running environment exception
	/// </summary>
	public const int IMVS_EC_CAMERA_RUNTIME = -536869888;

	/// <summary>
	/// CH: 命令不被设备支持 | EN: This command is not supported by the device
	/// </summary>
	public const int IMVS_EC_CAMERA_NOT_IMPLEMENTED = -536869887;

	/// <summary>
	/// CH: 设备无访问权限 | EN: No permission for this device to access to
	/// </summary>
	public const int IMVS_EC_CAMERA_ACCESS_DENIED = -536869886;

	/// <summary>
	/// CH: 设备忙，或网络断开 | EN: Device is busy or network is disconnected
	/// </summary>
	public const int IMVS_EC_CAMERA_BUSY = -536869885;

	/// <summary>
	/// CH: 网络包数据错误 | EN: Network data packet error
	/// </summary>
	public const int IMVS_EC_CAMERA_PACKET = -536869884;

	/// <summary>
	/// CH: 读USB出错 | EN: Reading from USB failed
	/// </summary>
	public const int IMVS_EC_CAMERA_USB_READ = -536869883;

	/// <summary>
	/// CH: 写USB出错 | EN: Writing to USB failed
	/// </summary>
	public const int IMVS_EC_CAMERA_USB_WRITE = -536869882;

	/// <summary>
	/// CH: 设备异常 | EN: USB device exception
	/// </summary>
	public const int IMVS_EC_CAMERA_USB_DEVICE = -536869881;

	/// <summary>
	/// CH: USB带宽不足 | EN: Insufficient USB bandwidth
	/// </summary>
	public const int IMVS_EC_CAMERA_USB_BANDWIDTH = -536869880;

	/// <summary>
	/// CH: 相机无数据 | EN: No data in the camera
	/// </summary>
	public const int IMVS_EC_CAMERA_NODATA = -536869879;

	/// <summary>
	/// CH: 未连接相机 | EN: Camera not connected
	/// </summary>
	public const int IMVS_EC_CAMERA_EMPTY = -536869872;

	/// <summary>
	/// CH: 相机类型不匹配 | EN: Camera type mismatch
	/// </summary>
	public const int IMVS_EC_CAMERA_TYPE_NOT_MATCH = -536869871;

	/// <summary>
	/// CH: 图像数据存储地址为空（某个分量） | EN: Image data storage path is empty (a component)
	/// </summary>
	public const int IMVS_EC_ALGORITHM_IMG_DATA_NULL = -536869632;

	/// <summary>
	/// CH: 图像宽高与step参数不匹配 | EN: The image width/height and step parameter mismatches
	/// </summary>
	public const int IMVS_EC_ALGORITHM_IMG_STEP = -536869631;

	/// <summary>
	/// CH: 图像宽高不正确或者超出范围 | EN: The image width/height is incorrect or out of limit
	/// </summary>
	public const int IMVS_EC_ALGORITHM_IMG_SIZE = -536869630;

	/// <summary>
	/// CH: 图像格式不正确或者不支持 | EN: The image format is incorrect or not supported
	/// </summary>
	public const int IMVS_EC_ALGORITHM_IMG_FORMAT = -536869629;

	/// <summary>
	/// CH: 内存空间大小不满足对齐要求 | EN: The memory size does not meet with the alignment requirement
	/// </summary>
	public const int IMVS_EC_ALGORITHM_MEM_SIZE_ALIGN = -536869628;

	/// <summary>
	/// CH: 内存空间大小不够 | EN: Insufficient memory size
	/// </summary>
	public const int IMVS_EC_ALGORITHM_MEM_LACK = -536869627;

	/// <summary>
	/// CH: 内存对齐不满足要求 | EN: The memory alignment does not meet with the requirement
	/// </summary>
	public const int IMVS_EC_ALGORITHM_MEM_ALIGN = -536869626;

	/// <summary>
	/// CH: ABILITY存在无效成员变量 | EN: Invalid variable member exists in the ABILITY
	/// </summary>
	public const int IMVS_EC_ALGORITHM_ABILITY_ARG = -536869625;

	/// <summary>
	/// CH: cpu不支持优化代码中的指令集 | EN: Optimizing command set of code is not supported by CPU
	/// </summary>
	public const int IMVS_EC_ALGORITHM_CPUID = -536869624;

	/// <summary>
	/// CH: 数据大小不正确 | EN: Incorrect data size
	/// </summary>
	public const int IMVS_EC_ALGORITHM_DATA_SIZE = -536869623;

	/// <summary>
	/// CH: 回调函数出错 | EN: Callback function error
	/// </summary>
	public const int IMVS_EC_ALGORITHM_CALL_BACK = -536869622;

	/// <summary>
	/// CH: 超过HKA限定最大内存 | EN: Maximum HKA memory reached
	/// </summary>
	public const int IMVS_EC_ALGORITHM_OVER_MAX_MEM = -536869621;

	/// <summary>
	/// CH: 数据STEP错误 | EN: Data STEP error
	/// </summary>
	public const int IMVS_EC_ALGORITHM_STEP_ERR = -536869620;

	/// <summary>
	/// CH: 参数index错误 | EN: Parameter "index" error
	/// </summary>
	public const int IMVS_EC_ALGORITHM_PARAM_INDEX = -536869619;

	/// <summary>
	/// CH: 参数个数错误 | EN: Incorrect number of parameters
	/// </summary>
	public const int IMVS_EC_ALGORITHM_PARAM_NUM = -536869618;

	/// <summary>
	/// CH: 算法库未初始化 | EN: Algorithm library is not initialized
	/// </summary>
	public const int IMVS_EC_ALGORITHM_NOT_INIT = -536869617;

	/// <summary>
	/// CH: 获取输入图像失败 | EN: Failed to get input image
	/// </summary>
	public const int IMVS_EC_ALGORITHM_INPUT_IMAGE_ERROR = -536869616;

	/// <summary>
	/// CH: 获取输入ROI失败 | EN: Failed to get input ROI
	/// </summary>
	public const int IMVS_EC_ALGORITHM_INPUT_ROI_ERROR = -536869615;

	/// <summary>
	/// CH: 获取位置修正信息失败 | EN: Failed to get position correction information
	/// </summary>
	public const int IMVS_EC_ALGORITHM_INPUT_FIXTUREINFO_ERROR = -536869614;

	/// <summary>
	/// CH: 模型数据为空 | EN: Model data is empty
	/// </summary>
	public const int IMVS_EC_ALGORITHM_MODEL_EMPTY = -536869613;

	/// <summary>
	/// CH: 未定义的ROI类型 | EN: Undefined ROI type
	/// </summary>
	public const int IMVS_EC_ALGORITHM_ROITYPE_UNKNOWN = -536869612;

	/// <summary>
	/// CH: 不支持含中文的路径 | EN: Paths with Chinese are not supported
	/// </summary>
	public const int IMVS_EC_ALGORITHM_IMG_PATH = -536869611;

	/// <summary>
	/// CH: 未发送示教信息 | EN: 
	/// </summary>
	public const int IMVS_EC_ALGORITHM_TEACH_NOREADY = -536869610;

	/// <summary>
	/// CH: 标定流程未完成 | EN: 
	/// </summary>
	public const int IMVS_EC_ALGORITHM_CALIB_NOREADY = -536869609;

	/// <summary>
	/// CH: 最小值和最大值相等，不支持反选 | EN: Inversion failed. The Max. value is the same with the Min. value.
	/// </summary>
	public const int IMVS_EC_ALGORITHM_NOT_SUPPORT_INVERT_SELECT = -536869608;

	/// <summary>
	/// CH: 创建服务失败 | EN: Creating service failed
	/// </summary>
	public const int IMVS_EC_SERVICE_CREATE = -536869376;

	/// <summary>
	/// CH: 删除服务失败 | EN: Deleting service failed
	/// </summary>
	public const int IMVS_EC_SERVICE_DELETE = -536869375;

	/// <summary>
	/// CH: 打开服务失败 | EN: Opening service failed
	/// </summary>
	public const int IMVS_EC_SERVICE_OPEN = -536869374;

	/// <summary>
	/// CH: 服务启动失败 | EN: Starting service failed
	/// </summary>
	public const int IMVS_EC_SERVICE_START = -536869373;

	/// <summary>
	/// CH: 服务停止失败 | EN: Stopping service failed
	/// </summary>
	public const int IMVS_EC_SERVICE_STOP = -536869372;

	/// <summary>
	/// CH: 服务路径获取失败 | EN: Getting service path failed
	/// </summary>
	public const int IMVS_EC_SERVICE_GET_PATH = -536869371;

	/// <summary>
	/// CH: 服务文件查找失败 | EN: Finding service file failed
	/// </summary>
	public const int IMVS_EC_SERVICE_FIND_FILE = -536869370;

	/// <summary>
	/// CH: 服务库加载失败 | EN: Loading service library failed
	/// </summary>
	public const int IMVS_EC_SERVICE_LOAD_LIBRARY = -536869369;

	/// <summary>
	/// CH: 服务函数检索失败 | EN: Retrieving service function failed
	/// </summary>
	public const int IMVS_EC_SERVICE_RETRIEVES_FUNC = -536869368;

	/// <summary>
	/// CH: 加密狗未检测到或检测异常 | EN: No dongle detected or detection exception
	/// </summary>
	public const int IMVS_EC_ENCRYPT_DONGLE_OUTDATE = -536869120;

	/// <summary>
	/// CH: 算法平台老版本狗试用时间过期 | EN: The trial license of dongle in the algorithm platform with old version is expired
	/// </summary>
	public const int IMVS_EC_ENCRYPT_DONGLE_OLD_EXPIRE = -536869119;

	/// <summary>
	/// CH: 算法库检测授权失败 | EN: Detecting algorithm library license failed
	/// </summary>
	public const int IMVS_EC_ENCRYPT_ALGORITHM_CHECK_FAIL = -536869118;

	/// <summary>
	/// CH: 算法库使用期已过 | EN: The algorithm library license is expired
	/// </summary>
	public const int IMVS_EC_ENCRYPT_ALGORITHM_EXPIRE = -536869117;

	/// <summary>
	/// CH: 软锁未检测到或检测异常 | EN: Soft lock not detected or detected exception
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_OUTDATE = -536869116;

	/// <summary>
	/// CH: 产品授权未激活 | EN: Product authorization not activated
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_NOT_ACTIVATED = -536869115;

	/// <summary>
	/// CH: 软锁不支持的功能ID | EN: Feature ID not supported by soft lock
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_NOT_SUPPORT = -536869114;

	/// <summary>
	/// CH: 授权的功能已过期 | EN: The authorized feature has expired
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_FEATURE_EXPIRE = -536869113;

	/// <summary>
	/// CH: 访问被拒绝 | EN: Access denied
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_ACCESS_DENIED = -536869112;

	/// <summary>
	/// CH: 时钟不可用 | EN: Clock is not available
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_NO_TIME = -536869111;

	/// <summary>
	/// CH: 未安装软加密RTE | EN: Soft encryption RTE not installed
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_NO_DRIVER = -536869110;

	/// <summary>
	/// CH: 程序在终端运行 | EN: The program is running at the terminal
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_TS_DETECTED = -536869109;

	/// <summary>
	/// CH: 程序在远程端运行 | EN: The program is running on the remote side
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_RDP_DETECTED = -536869108;

	/// <summary>
	/// CH: 程序在虚拟机运行 | EN: The program is running in a virtual machine
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_VM_DETECTED = -536869107;

	/// <summary>
	/// CH: 功能未找到 | EN: Feature not found
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_FEATURE_NOT_FOUND = -536869106;

	/// <summary>
	/// CH: 内部实现错误 | EN: Internal implementation errors
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_INNERIMPL_ERROR = -536869105;

	/// <summary>
	/// CH: 产品未找到 | EN: The product was not found
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_PRODUCT_NOT_FOUND = -536869104;

	/// <summary>
	/// CH: 查询结果为空 | EN: The query result is empty
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_RESULTS_EMPTY = -536869103;

	/// <summary>
	/// CH: 因超出能力集或可用内存资源不足导致获取许可信息失败 | EN: Get license information failure because of beyond the limits of capacity or applying resource failed
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_ABILITY_OR_RESOURCE = -536869102;

	/// <summary>
	/// CH: 机器不满足授权要求而被禁用 | EN: The feature is forbiddened for the machine not satisfied with authorization requirement
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_FEATURE_FORBIDDEN = -536869101;

	/// <summary>
	/// CH: 未找到可用的保护锁 | EN: No available production key were found
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_HASP_NOT_FOUND = -536869100;

	/// <summary>
	/// CH: 会话已断开，如超时 | EN: Session is disconnected, such as timeout
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_BROKEN_SESSION = -536869099;

	/// <summary>
	/// CH: 通信错误 | EN: Communication error
	/// </summary>
	public const int IMVS_EC_ENCRYPT_SOFT_COMM_ERR = -536869098;

	/// <summary>
	/// CH: 没有开发狗权限 | EN: Without development authority  
	/// </summary>
	public const int IMVS_EC_ENCRYPT_WITHOUT_DEVELOP_AUTH = -536869097;

	/// <summary>
	/// CH: 相机模块数量超出限制 | EN: The number of camera modules is out of limit  
	/// </summary>
	public const int IMVS_EC_ENCRYPT_CAMERA_COUNT_BEYOND = -536869096;

	/// <summary>
	/// CH: 校验加密狗中密码失败  
	/// </summary>
	public const int IMVS_EC_ENCRYPT_KEY_INDOG_WRONG = -536869095;

	/// <summary>
	/// CH: 使用加密狗解密文件失败
	/// </summary>
	public const int IMVS_EC_ENCRYPT_DECRYPT_FAIL_WITH_DOG = -536869094;

	/// <summary>
	/// CH: 网络锁校验失败
	/// </summary>
	public const int IMVS_EC_ENCRYPT_REMOTE_DONGLE_CHECK = -536869093;

	/// <summary>
	/// CH: 网络锁已达到最大并发授权
	/// </summary>
	public const int IMVS_EC_ENCRYPT_REMOTE_DONGLE_MAX_CONNECTION = -536869092;

	/// <summary>
	/// CH: 该加密狗系列不支持
	/// </summary>
	public const int IMVS_EC_ENCRYPT_DONGLE_SERIES_NOT_SUPPORT = -536869091;

	/// <summary>
	/// CH: 使用加密狗加密数据失败
	/// </summary>
	public const int IMVS_EC_ENCRYPT_DONGLE_ENCRYPT_FAIL = -536869090;

	/// <summary>
	/// CH: 解密后的数据校验失败
	/// </summary>
	public const int IMVS_EC_ENCRYPT_CHECK_DECRYPTED_DATA_FAIL = -536869089;

	/// <summary>
	/// CH: 有两个及以上加密狗设置了密码 | EN：Two or more dongles have set passwords
	/// </summary>
	public const int IMVS_EC_ENCRYPT_TWO_MORE_DONGLE_SET_PWD = -536869088;

	/// <summary>
	/// CH: 文件解密失败，有两个及以上加密狗设置了密码 | EN：File decryption failed, two or more dongles have set passwords
	/// </summary>
	public const int IMVS_EC_DECRYPT_FAIL_TWO_MORE_DONGLE_SET_PWD = -536869087;

	/// <summary>
	/// CH: 方案密码错误 | EN: Incorrect solution password
	/// </summary>
	public const int IMVS_EC_AUTH_SOLU_PASSWORD = -536868864;

	/// <summary>
	/// CH: 用户或密码错误(预留) | EN: Incorrect user name or password (reserved)
	/// </summary>
	public const int IMVS_EC_AUTH_USER_PASSWORD = -536868863;

	/// <summary>
	/// CH: 远程连接需设置密码 | EN: 
	/// </summary>
	public const int IMVS_EC_AUTH_REMOTE_NO_PASSWORD = -536868862;

	/// <summary>
	/// CH: 远程连接密码错误 | EN: 
	/// </summary>
	public const int IMVS_EC_AUTH_REMOTE_PASSWORD_INVALID = -536868861;

	/// <summary>
	/// CH: 原密码错误 | EN: 
	/// </summary>
	public const int IMVS_EC_AUTH_ORIGINAL_PASSWORD_INVALID = -536868860;

	/// <summary>
	/// CH: 密码已存在 | EN: 
	/// </summary>
	public const int IMVS_EC_AUTH_ASSWORD_IS_EXIST = -536868859;

	/// <summary>
	/// CH: 用户锁定 | EN: 
	/// </summary>
	public const int IMVS_EC_AUTH_USER_LOCKOUT = -536868858;

	/// <summary>
	/// CH: 没有登录用户数据文件 | EN: 
	/// </summary>
	public const int IMVS_EC_AUTH_NO_LOGINUSER_DATABASE_FILE = -536868857;

	/// <summary>
	/// CH: 模块输入未订阅 | EN: The module input is not subscribed
	/// </summary>
	public const int IMVS_EC_WARN_MODULE_INPUT_SUB = -536868608;

	/// <summary>
	/// CH: 算法库中出现警告 | EN: Warning occurred in algorithm library
	/// </summary>
	public const int IMVS_EC_WARN_ALGORITHM = -536868607;

	/// <summary>
	/// CH: 模块算法类初始化结果 | EN: Algorithmic initialization result of module
	/// </summary>
	public const int IMVS_EC_MODULE_ALGORITHM_INIT = -536866816;

	/// <summary>
	/// CH: 模块错误码上报 | EN: Report Error Code
	/// </summary>
	public const int IMVS_EC_MODULE_REPORT_CODE = -536866815;

	/// <summary>
	/// CH: 模型无法打开 | EN: Model can't be opened
	/// </summary>
	public const int IMVS_EC_MODEL_OPEN = -536866560;

	/// <summary>
	/// CH: 模型不存在 | EN: Model does not exist
	/// </summary>
	public const int IMVS_EC_MODEL_NOT_FOUND = -536866559;

	/// <summary>
	/// CH: 模型格式错误 | EN: Model format is wrong
	/// </summary>
	public const int IMVS_EC_MODEL_FORMAT = -536866558;

	/// <summary>
	/// CH: 模型文件被占用 | EN: Model file is occupied
	/// </summary>
	public const int IMVS_EC_MODEL_BE_OCCUPIED = -536866557;

	/// <summary>
	/// CH: 模型数据异常 | EN: Model data anomalies
	/// </summary>
	public const int IMVS_EC_MODEL_DATA = -536866556;

	/// <summary>
	/// CH: 模型数据长度异常 | EN: The length of model data is abnormal
	/// </summary>
	public const int IMVS_EC_MODEL_DATA_LENGTH = -536866555;

	/// <summary>
	/// CH: 从模型中获取ROI失败 | EN: Failed to get ROI from the model file
	/// </summary>
	public const int IMVS_EC_GET_ROI_FROM_MODEL_FAIL = -536866554;

	/// <summary>
	/// CH: 缺少二进制数据(需要外部输入数据) | EN: Lack the binary data
	/// </summary>
	public const int IMVS_EC_PROXY_LACK_BINARYDATA = -536866304;

	/// <summary>
	/// CH: 缺少二进制内存(需要外部申请内存) | EN: Lack the binary memory
	/// </summary>
	public const int IMVS_EC_PROXY_LACK_BINARYMEMORY = -536866303;

	/// <summary>
	/// CH: FTP连接失败 | EN: Ftp connect failed
	/// </summary>
	public const int IMVS_EC_FTP_CONNECT_FAIL = -536866041;

	/// <summary>
	/// CH: FTP拒绝访问 | EN: Ftp access failed
	/// </summary>
	public const int IMVS_EC_FTP_ACCESS_DENIED = -536866039;

	/// <summary>
	/// CH: FTP连接超时 | EN: Ftp connect timeout
	/// </summary>
	public const int IMVS_EC_FTP_CONNECT_TIMEOUT = -536866008;

	/// <summary>
	/// CH: FTP上传数据失败 | EN: Send data failed
	/// </summary>
	public const int IMVS_EC_FTP_UPLOAD_FAIL = -536865963;

	/// <summary>
	/// CH: FTP登录失败 | EN: Ftp login failed
	/// </summary>
	public const int IMVS_EC_FTP_LOGIN_DENIED = -536865945;

	public const int IMVS_E_MAX_PROCESSID = 10999;

	public const int IMVS_MODULE_GLOBAL_VAR = 13000;

	public const uint IMVS_LIGHT_CONTROL_MODULE = 14000u;

	public const uint IMVS_COMMU_CONTROL_MODULE = 11000u;

	public const uint IMVS_GLOBAL_TRRIGER_MODULE = 14200u;

	public const uint MODULE_MIN_ID_PROC_LOCAL_VARIABLE = 24000u;

	public const uint MODULE_MAX_ID_PROC_LOCAL_VARIABLE = 25999u;

	public const int MODULE_MIN_ID_GROUP_LOCAL_VARIABLE = 31000;

	public const int MODULE_MAX_ID_GROUP_LOCAL_VARIABLE = 33999;

	public const int INDEPENDENT_GROUP_LOCAL_VARIABLE_MIN_ID = 105000;

	public const int INDEPENDENT_GROUP_LOCAL_VARIABLE_MAX_ID = 108999;

	public const int IMVS_MAX_GROUP_NAME_LENGTH = 200;

	public const int IMVS_MAX_MODULE_NAME_LENGTH = 64;

	public const int IMVS_MAX_MODULE_NUM = 2064;

	public const int IMVS_MAX_INPUT_NUM = 64;

	public const int IMVS_MAX_OUTPUT_NUM = 64;

	public const int IMVS_MAX_SUBSCRIBE_CONTENT_LENGTH = 256;

	public const int IMVS_MAX_PLATFORM_NUM = 16;

	public const int IMVS_MAX_USERNAME_LENGTH = 16;

	public const int IMVS_MAX_PASSWORD_LENGTH = 32;

	public const int IMVS_MAX_VENDORNAME_LENGTH = 16;

	public const int IMVS_MAX_PATH_LENGTH = 260;

	public const int IMVS_DEFINE_CUSTOM_OUTPUT_PAGE_SIZE = 10;

	public const int IMVS_MAX_PROCESS_NAME_LENGTH = 64;

	public const int IMVS_MAX_PROCESS_NUM = 1000;

	public const int IMVS_MAX_ADD_MODULE_NUM = 2060;

	public const int IMVS_MAX_DEL_MODULE_NUM = 2060;

	public const int IMVS_MAX_TRANS_DATA_LENGTH = 1024;

	public const int IMVS_PF_MAX_PROCESS_NAME_LENGTH = 64;

	public const int IMVS_AUTO_LOAD_SOLU_STATE_STOP_EXECUTE = 0;

	public const int IMVS_AUTO_LOAD_SOLU_STATE_EXECUTE_ONCE = 1;

	public const int IMVS_AUTO_LOAD_SOLU_STATE_CONTINUE_EXECUTE = 2;

	public const int IMVS_CLIENT_TAKE_OVER_STATE_NOT_NEED = 0;

	public const int IMVS_CLIENT_TAKE_OVER_STATE_NEED = 1;

	public const int IMVS_CLIENT_TAKE_OVER_STATE_HB_EXCEPTION = 2;

	public const int IMVS_CAMPICINFO_LIST_NUM = 256;

	public const int IMVS_MAX_CORE_NUM = 1024;

	public const int IMVS_MAX_RUN_POLICY_NUM = 32;

	public const int IMVS_DEFAULT_GUID_LEN = 256;

	public const int IMVS_MAX_MODULE_PARAM_NUM = 128;

	public const int IMVS_MAX_PARAM_NAME_LENGTH = 128;

	public const int IMVS_MAX_PARAM_VALUE_LENGTH = 512;

	public const int IMVS_DEFINE_MODULE_RES_STRING_LENGTH = 4096;

	public const int IMVS_MAX_SHARE_MEMORY_ADDR_LENGTH = 260;

	public const int IMVS_MAX_PATH_UTF8_LENGTH = 780;

	public const int IMVS_DEFINE_PUB_ADDRESS_LENGTH = 64;

	public const int IMVS_MAX_RESULT_PARAM_NAME_LENGTH = 128;

	public const int IMVS_SOLUTION_VERSION_LENGTH = 32;

	public const int IMVS_BINARY_DATA_PARAM_LENGTH = 256;

	public const int IMVS_MAX_MODULE_PARAMVALUE_LENGTH = 512;

	public const int IMVS_MAX_MODULE_DISPLAY_NAME_LENGTH = 256;

	public const int IMVS_PF_MAX_IMPORT_NUM = 16;

	public const int IMVS_MAX_THIRD_SOFTWARE_NUM = 16;

	public const int IMVS_MAX_GLOBAL_TYPE_NUM = 5;

	public const int IMVS_MAX_MODULE_IO_NUM = 128;

	public const int NORMAL_MIN_ID_GROUP = 21000;

	public const int NORMAL_MAX_ID_GROUP = 21999;

	public const int INDEPENDENT_GROUP_MIN_ID = 101000;

	public const int INDEPENDENT_GROUP_MAX_ID = 105999;

	public const int INDEPENDENT_MODULE_MIN_ID = 110000;

	public const int INDEPENDENT_MODULE_MAX_ID = 119999;

	public const int MODULE_MIN_ID_PROCESS_CONTROL = 10000;

	public const int MODULE_MAX_ID_PROCESS_CONTROL = 10999;

	public const int IMVS_MAX_OBJECT_IO_NUM = 1024;

	public const int IMVS_MAX_MODULE_INPUT_IO_NUM = 128;

	public const int IMVS_DEFAULT_DATA_NUM = 512;

	public const int IMVS_DEFAULT_STYLE_NUM = 32;

	public const int IMVS_REENTRANT_MAX_CMD_LEN = 16;

	public const int IMVS_CAMERA_MAX_SN_LEN = 64;

	public const int IMVS_CAMERA_MAX_NAME_LEN = 64;

	public const int IMVS_CAMERA_MAX_NUM = 256;

	public const int IMVS_MAX_POLYGON_VERTEX_NUM = 32;

	public const int IMVS_ALGORITHM_VERSION_LENGTH = 64;

	public const int IMVS_MVDP_KEY_LENGTH = 16;

	public const int IMVS_MVDP_ENC_DEC_DATA_LEN_DIFFERENCE = 84;

	public const int MODULE_MIN_ID_PROC_PROCEDURECOMM = 26000;

	public const int MODULE_MAX_ID_PROC_PROCEDURECOMM = 27999;

	public const int IMVS_MODULE_RUN_ERROR_INFO_MAX_NUM = 16;

	public const int IMVS_MODULE_FILTER_IO_INFO_MAX_NUM = 10;

	public const int MODULE_TYPE_NORMAL = 0;

	public const int MODULE_TYPE_PROCESS_CONTROL = 1;

	public const int MODULE_TYPE_LOOP_CONTROL = 2;

	public const int MODULE_TYPE_GLOBAL_COMMUNICAT = 3;

	public const int MODULE_TYPE_GLOBAL_QUEUE = 4;

	public const int MODULE_TYPE_GLOBAL_VARIABLE = 5;

	public const int MODULE_TYPE_GLOBAL_CAMERA = 6;

	public const int MODULE_TYPE_GLOBAL_LIGHT = 7;

	public const int MODULE_TYPE_GROUP = 20;

	public const int MODULE_TYPE_LOCAL_VARIABLE = 25;

	public const int MODULE_TYPE_PROCEDURE_COMMUNICAT = 26;

	public const int MODULE_TYPE_SOLUTION = 100;

	public const int GLOBAL_BASE_ID = 12000;

	public const int GLOBAL_MODULE_PARENT_ID = 10999;

	/// <summary>
	/// CH: 整型数据 | EN: Integer data
	/// </summary>
	public const int IMVS_MODURES_TYPE_INT = 1;

	/// <summary>
	/// CH: 浮点型数据 | EN: Float data
	/// </summary>
	public const int IMVS_MODURES_TYPE_FLOAT = 2;

	/// <summary>
	/// CH: 字符串型数据 | EN: String data
	/// </summary>
	public const int IMVS_MODURES_TYPE_STRING = 3;

	/// <summary>
	/// CH: 图像型数据 | EN: Image data
	/// </summary>
	public const int IMVS_MODURES_TYPE_IMAGE = 4;

	/// <summary>
	/// CH: 大数据类型数据 | EN: Data used for big data
	/// </summary>
	public const int IMVS_MODURES_TYPE_CHUNK = 5;

	/// <summary>
	/// CH: 二进制数据类型数据 | EN: Binary data
	/// </summary>
	public const int IMVS_PF_MODURES_TYPE_BYTEDATA = 6;

	/// <summary>
	/// double类型数据
	/// </summary>
	public const int IMVS_MODURES_TYPE_DOUBLE = 7;

	/// <summary>
	/// CH: 像素格式 MONO8 | EN: MONO8 pixel format
	/// </summary>
	public const int IMVS_IMG_FORMAT_MONO8 = 17301505;

	/// <summary>
	/// CH: 像素格式 RGB24 C3 | EN: RGB24 C3 pixel format
	/// </summary>
	public const int IMVS_IMG_FORMAT_RGB24 = 35127316;

	/// <summary>
	/// CH: 像素格式 RGB24 P3 | EN: RGB24 P3 pixel format
	/// </summary>
	public const int IMVS_IMG_FORMAT_RGB24_P3 = 35127329;

	public const int FTP_MAX_CHARACTER_NUM = 255;

	public const int FTP_MAX_FILE_NUM = 100;

	public const int SERVER_EC_OK = 0;

	public const int SERVER_EC_AUTH_NORMAL = 0;

	public const int SERVER_EC_AUTH_ABNORMAL = 10001;

	public const int SERVER_EC_AUTH_DONGLE_OUTDATE = 11000;

	public const int SERVER_EC_AUTH_DONGLE_OLDVER_EXPIRE = 11001;

	public const int SERVER_EC_AUTH_TRIAL_OUTDATE = 12000;

	public const int AUTH_TRUE = 1;

	public const int AUTH_FALSE = 0;

	public const int AUTH_REMIND_DEADTIME = 3;

	public const int AUTH_TIME_INTERVAL = 300;

	private const int sdkMemoryOffset = 16;

	/// <summary>
	/// CH: 本地图像模块 | EN: Local image module
	/// </summary>
	public const string MODU_NAME_LOCALIMAGEVIEW = "LocalImageView";

	/// <summary>
	/// CH: 相机图像模块 | EN: Camera module
	/// </summary>
	public const string MODU_NAME_CAMERAMODULE = "CameraModule";

	/// <summary>
	/// CH: 存储图像模块 | EN: Image storage module
	/// </summary>
	public const string MODU_NAME_SAVEIMAGE = "SaveImage";

	/// <summary>
	/// CH: 高精度特征匹配模块 | EN: Feature high-precising matching module
	/// </summary>
	public const string MODU_NAME_HPFEATUREMATCHMODU = "IMVSHPFeatureMatchModu";

	/// <summary>
	/// CH: 快速特征匹配模块 | EN: Feature fast matching module
	/// </summary>
	public const string MODU_NAME_FASTFEATUREMATCHMODU = "IMVSFastFeatureMatchModu";

	/// <summary>
	/// CH: 圆查找模块 | EN: Circle search module
	/// </summary>
	public const string MODU_NAME_CIRCLEFINDMODU = "IMVSCircleFindModu";

	/// <summary>
	/// CH: 直线查找模块 | EN: Line search module
	/// </summary>
	public const string MODU_NAME_LINEFINDMODU = "IMVSLineFindModu";

	/// <summary>
	/// CH: Blob分析模块 | EN: BLOB (Binary Large Object) analysis module
	/// </summary>
	public const string MODU_NAME_BLOBFINDMODU = "IMVSBlobFindModu";

	/// <summary>
	/// CH: 卡尺工具模块 | EN: Caliper tool module
	/// </summary>
	public const string MODU_NAME_CALIPERMODU = "IMVSCaliperModu";

	/// <summary>
	/// CH: 边缘查找模块 | EN: Edge search module
	/// </summary>
	public const string MODU_NAME_CALIPEREDGEMODU = "IMVSCaliperEdgeModu";

	/// <summary>
	/// CH: 间距检测模块 | EN: Interval detection module
	/// </summary>
	public const string MODU_NAME_EDGEWIDTHFINDMODU = "IMVSEdgeWidthFindModu";

	/// <summary>
	/// CH: 位置修正模块 | EN: Position correction module
	/// </summary>
	public const string MODU_NAME_FIXTUREMODU = "IMVSFixtureModu";

	/// <summary>
	/// CH: 矩形检测模块 | EN: Rectangle detection module
	/// </summary>
	public const string MODU_NAME_RECTFINDMODU = "IMVSRectFindModu";

	/// <summary>
	/// CH: 顶点检测模块 | EN: Vertex detection module
	/// </summary>
	public const string MODU_NAME_PEAKFINDMODU = "IMVSPeakFindModu";

	/// <summary>
	/// CH: 边缘交点模块 | EN: Edge intersection search module
	/// </summary>
	public const string MODU_NAME_CALIPERCORNERMODU = "IMVSCaliperCornerModu";

	/// <summary>
	/// CH: 平行线查找模块 | EN: Parallel line search module
	/// </summary>
	public const string MODU_NAME_PAIRLINEMODU = "IMVSPairLineModu";

	/// <summary>
	/// CH: 线圆测量模块 | EN: Line and circle distance measurement module
	/// </summary>
	public const string MODU_NAME_L2CMEASUREMODU = "IMVSL2CMeasureModu";

	/// <summary>
	/// CH: 圆圆测量模块 | EN: Circle and circle distance measurement module
	/// </summary>
	public const string MODU_NAME_C2CMEASUREMODU = "IMVSC2CMeasureModu";

	/// <summary>
	/// CH: 点圆测量模块 | EN: Point and circle distance measurement module
	/// </summary>
	public const string MODU_NAME_P2CMEASUREMODU = "IMVSP2CMeasureModu";

	/// <summary>
	/// CH: 点线测量模块 | EN: Point and line distance measurement module
	/// </summary>
	public const string MODU_NAME_P2LMEASUREMODU = "IMVSP2LMeasureModu";

	/// <summary>
	/// CH: 线线测量模块 | EN: Line and line distance measurement module
	/// </summary>
	public const string MODU_NAME_L2LMEASUREMODU = "IMVSL2LMeasureModu";

	/// <summary>
	/// CH: 点点测量模块 | EN: Point and point distance measurement module
	/// </summary>
	public const string MODU_NAME_P2PMEASUREMODU = "IMVSP2PMeasureModu";

	/// <summary>
	/// CH: 圆拟合模块 | EN: Circle fitting module
	/// </summary>
	public const string MODU_NAME_CIRCLEFITMODU = "IMVSCircleFitModu";

	/// <summary>
	/// CH: 线拟合模块 | EN: Line fitting module
	/// </summary>
	public const string MODU_NAME_LINEFITMODU = "IMVSLineFitModu";

	/// <summary>
	/// CH: 亮度测量模块 | EN: Brightness detection module
	/// </summary>
	public const string MODU_NAME_INTENSITYMEASUREMODU = "IMVSIntensityMeasureModu";

	/// <summary>
	/// CH: 像素统计模块 | EN: Pixel statistics module
	/// </summary>
	public const string MODU_NAME_PIXELCOUNTMODU = "IMVSPixelCountModu";

	/// <summary>
	/// CH: 直方图工具模块 | EN: Histogram tool module
	/// </summary>
	public const string MODU_NAME_HISTTOOLMODU = "IMVSHistToolModu";

	/// <summary>
	/// CH: 几何创建模块 | EN: Geometry creation module
	/// </summary>
	public const string MODU_NAME_GEOMETRYCREATE = "GeometryCreate";

	/// <summary>
	/// CH: 二维码识别模块 | EN: QR code recognition module
	/// </summary>
	public const string MODU_NAME_2DBCRMODU = "IMVS2dBcrModu";

	/// <summary>
	/// CH: 条码识别模块 | EN: Bar code recognition module
	/// </summary>
	public const string MODU_NAME_BCRMODU = "IMVSBcrModu";

	/// <summary>
	/// CH: 字符识别模块 | EN: OCR (Optical Character Recognition) module
	/// </summary>
	public const string MODU_NAME_OCRMODU = "IMVSOcrModu";

	/// <summary>
	/// CH: VeriCode模块 | EN: Verification code module
	/// </summary>
	public const string MODU_NAME_VERICODEMODU = "IMVSVericodeModu";

	/// <summary>
	/// CH: 标定板标定模块 | EN: Calibration module
	/// </summary>
	public const string MODU_NAME_CALIBBOARDCALIBMODU = "IMVSCalibBoardCalibModu";

	/// <summary>
	/// CH: N点标定模块 | EN: N-point calibration module
	/// </summary>
	public const string MODU_NAME_ALIGNCALIBMODU = "IMVSAlignCalibModu";

	/// <summary>
	/// CH: 标定转换模块 | EN: Calibration conversion module
	/// </summary>
	public const string MODU_NAME_CALIBTRANSFORMMODU = "IMVSCalibTransformModu";

	/// <summary>
	/// CH: 单位转换模块 | EN: Unit conversion module
	/// </summary>
	public const string MODU_NAME_SCALETRANSFORMMODU = "IMVSScaleTransformModu";

	/// <summary>
	/// CH: 畸变校正模块 | EN: Distortion calibration module
	/// </summary>
	public const string MODU_NAME_IMAGECORRECTMODU = "IMVSImageCorrectModu";

	/// <summary>
	/// CH: 形态学处理模块 | EN: Image morphological processing module
	/// </summary>
	public const string MODU_NAME_IMAGEMORPHMODU = "IMVSImageMorphModu";

	/// <summary>
	/// CH: 图像二值化模块 | EN: Image binarization module
	/// </summary>
	public const string MODU_NAME_BINARYMODU = "IMVSBinaryModu";

	/// <summary>
	/// CH: 图像滤波模块 | EN: Image filtering module
	/// </summary>
	public const string MODU_NAME_IMAGEFILTERMODU = "IMVSImageFilterModu";

	/// <summary>
	/// CH: 图像增强模块 | EN: Image enhancement module
	/// </summary>
	public const string MODU_NAME_IMAGEENHANCEMODU = "IMVSImageEnhanceModu";

	/// <summary>
	/// CH: 图像运算模块 | EN: Image arithmetic module
	/// </summary>
	public const string MODU_NAME_IMAGEMATHMODU = "IMVSImageMathModu";

	/// <summary>
	/// CH: 清晰度评估模块 | EN: Image resolution assessment module
	/// </summary>
	public const string MODU_NAME_IMAGESHARPNESSMODU = "IMVSImageSharpnessModu";

	/// <summary>
	/// CH: 图像修正模块 | EN: Image correction module
	/// </summary>
	public const string MODU_NAME_IMAGEFIXTUREMODU = "IMVSImageFixtureModu";

	/// <summary>
	/// CH: 阴影校正模块 | EN: Shadow correction module
	/// </summary>
	public const string MODU_NAME_SHADECORRECTMODU = "IMVSShadeCorrectModu";

	/// <summary>
	/// CH: 仿射变换模块 | EN: Affine transformation module
	/// </summary>
	public const string MODU_NAME_AFFINETRANSFORMMODU = "IMVSAffineTransformModu";

	/// <summary>
	/// CH: 圆环展开模块 | EN: Circular expansion module
	/// </summary>
	public const string MODU_NAME_POLARUNWARPMODU = "IMVSPolarUnwarpModu";

	/// <summary>
	/// CH: 颜色抽取模块 | EN: Color extraction module
	/// </summary>
	public const string MODU_NAME_COLOREXTRACTMODU = "IMVSColorExtractModu";

	/// <summary>
	/// CH: 颜色测量模块 | EN: Color detection module
	/// </summary>
	public const string MODU_NAME_COLORMEASUREMODU = "IMVSColorMeasureModu";

	/// <summary>
	/// CH: 颜色转换模块 | EN: Color conversion module
	/// </summary>
	public const string MODU_NAME_COLORTRANSFORMMODU = "IMVSColorTransformModu";

	/// <summary>
	/// CH: 条件检测模块 | EN: Condition detection module
	/// </summary>
	public const string MODU_NAME_IFMODULE = "IfModule";

	/// <summary>
	/// CH: 分支模块 | EN: Branching module
	/// </summary>
	public const string MODU_NAME_BRANCHMODULE = "BranchModule_STD";

	/// <summary>
	/// CH: 逻辑模块 | EN: Logic module
	/// </summary>
	public const string MODU_NAME_ANDMODULE = "AndModule";

	/// <summary>
	/// CH: 格式化模块 | EN: Formatting module
	/// </summary>
	public const string MODU_NAME_FORMATMODULE = "FormatModule";

	/// <summary>
	/// CH: 变量计算模块 | EN: Variable calculation module
	/// </summary>
	public const string MODU_NAME_CALCULATORMODULE = "CalculatorModule";

	/// <summary>
	/// CH: 字符比较模块 | EN: Character comparison module
	/// </summary>
	public const string MODU_NAME_STRINGCOMPAREMODULE = "StringCompareModule";

	/// <summary>
	/// CH: 脚本模块 | EN: Script module
	/// </summary>
	public const string MODU_NAME_SHELLMODULE = "ShellModule";

	/// <summary>
	/// CH: 点集模块 | EN: Point set module
	/// </summary>
	public const string MODU_NAME_POINTSETMODU = "PointSetMODU_STD";

	/// <summary>
	/// CH: 深度学习字符识别模块 | EN: OCR (Optical Character Recognition) module (for deep learning)
	/// </summary>
	public const string MODU_NAME_OCRDLMODU = "IMVSOcrDlModu";

	/// <summary>
	/// CH: 深度学习图像分割模块 | EN: Image segmentation module (for deep learning)
	/// </summary>
	public const string MODU_NAME_CNNFLAWMODU = "IMVSCnnFlawModu";

	/// <summary>
	/// CH: 深度学习字符定位模块 （GPU版本） | EN: Character location module (for deep learning), GPU version
	/// </summary>
	public const string MODU_NAME_CNNCHARDETECTMODU = "IMVSCnnCharDetectModu";

	/// <summary>
	/// CH: 深度学习分类模块 （GPU版本） | EN: Deep learning classification module, GPU version
	/// </summary>
	public const string MODU_NAME_CNNCLASSIFYMODU = "IMVSCnnClassifyModu";

	/// <summary>
	/// CH: 深度学习目标检测模块 （GPU版本） | EN: Target detection module (for deep learning), GPU version
	/// </summary>
	public const string MODU_NAME_CNNDETECTMODU = "IMVSCnnDetectModu";

	/// <summary>
	/// CH: 深度学习训练工具模块 | EN: Training tool module
	/// </summary>
	public const string MODU_NAME_CNNTRAINTOOLMODU = "IMVSCnnTrainToolModu";

	/// <summary>
	/// CH: 深度学习字符定位模块 （CPU版本） | EN: Character location module (for deep learning), CPU version
	/// </summary>
	public const string MODU_NAME_CNNCHARDETECTMODUC = "IMVSCnnCharDetectModuC";

	/// <summary>
	/// CH: 深度学习分类模块 （CPU版本） | EN: Deep learning classification module, CPU version
	/// </summary>
	public const string MODU_NAME_CNNCLASSIFYMODUC = "IMVSCnnClassifyModuC";

	/// <summary>
	/// CH: 深度学习目标检测模块 （CPU版本） | EN: Target detection module (for deep learning), CPU version
	/// </summary>
	public const string MODU_NAME_CNNDETECTMODUC = "IMVSCnnDetectModuC";

	/// <summary>
	/// CH: 分支字符模块 | EN: Branch character module
	/// </summary>
	public const string MODU_NAME_BRANCHSTRINGCPMOLD = "BranchStringCpm";

	/// <summary>
	/// CH: 帧平均模块 | EN: Frame averaging module
	/// </summary>
	public const string MODU_NAME_FRAMEMEANMODU = "IMVSFrameMeanModu";

	/// <summary>
	/// CH: 畸变标定模块 | EN: Distortion calibration module
	/// </summary>
	public const string MODU_NAME_IMAGECALIBMODU = "IMVSImageCalibModu";

	/// <summary>
	/// CH: 图像组合模块 | EN: Image combination module
	/// </summary>
	public const string MODU_NAME_IMAGECOMBINEPROCESSMODU = "IMVSImageCombineProcessModu";

	/// <summary>
	/// CH: 图像归一化模块 | EN: Image normalization module
	/// </summary>
	public const string MODU_NAME_IMAGENORMLIZEMODU = "IMVSImageNormlizeModu";

	/// <summary>
	/// CH: 字符缺陷检测模块 | EN: Character defect detection module
	/// </summary>
	public const string MODU_NAME_MARKINSPMODU = "IMVSMarkInspModu";

	/// <summary>
	/// CH: 拷贝填充模块 | EN: Area copying and filling module
	/// </summary>
	public const string MODU_NAME_REGIONCOPYMODU = "IMVSRegionCopyModu";

	/// <summary>
	/// CH: 接收数据模块 | EN: Data receiving module
	/// </summary>
	public const string MODU_NAME_READDATASMODULE = "ReadDatasModule";

	/// <summary>
	/// CH: 文本保存模块 | EN: Text storage module
	/// </summary>
	public const string MODU_NAME_SAVETEXTMODULE = "SaveTextModule";

	/// <summary>
	/// CH: 发送数据模块 | EN: Data sending module
	/// </summary>
	public const string MODU_NAME_SENDDATASMODULE = "SendDatasModule";

	/// <summary>
	/// CH: 耗时统计模块 | EN: Time-consuming statistics module
	/// </summary>
	public const string MODU_NAME_TIMESTATISTICMODULE = "TimeStatisticModule";

	/// <summary>
	/// CH: 相机映射模块 | EN: Camera mapping module
	/// </summary>
	public const string MODU_NAME_CAMERAMAPMODU = "IMVSCameraMapModu";

	/// <summary>
	/// CH: 单点对位模块 | EN: Single point alignment module
	/// </summary>
	public const string MODU_NAME_SINGLEPOINTALIGNMODU = "IMVSSinglePointAlignModu";

	/// <summary>
	/// CH: 点集对位模块 | EN: Point set alignment module
	/// </summary>
	public const string MODU_NAME_MULTIPOINTALIGNMODU = "IMVSMultiPointAlignModu";

	/// <summary>
	/// CH: 线对位模块 | EN: Line alignment module
	/// </summary>
	public const string MODU_NAME_LINEALIGNMODU = "IMVSLineAlignModu";

	/// <summary>
	/// CH: 圆弧边缘缺陷检测模块 | EN: Arc edge defect detection module
	/// </summary>
	public const string MODU_NAME_CIRCLEEDGEINSPMODU = "IMVSCircleEdgeInspModu";

	/// <summary>
	/// CH: 直线边缘缺陷检测模块 | EN: Line edge defect detection module
	/// </summary>
	public const string MODU_NAME_LINEEDGEINSPMODU = "IMVSLineEdgeInspModu";

	/// <summary>
	/// CH: 圆弧边缘对缺陷检测模块 | EN: Arc edge pair defect detection module
	/// </summary>
	public const string MODU_NAME_CIRCLEEDGEPAIRINSPMODU = "IMVSCircleEdgePairInspModu";

	/// <summary>
	/// CH: 直线边缘对缺陷检测模块 | EN: Line edge pair defect detection module
	/// </summary>
	public const string MODU_NAME_LINEEDGEPAIRINSPMODU = "IMVSLineEdgePairInspModu";

	/// <summary>
	/// CH: 边缘组合缺陷检测模块 | EN: Edge combination defect detection module
	/// </summary>
	public const string MODU_NAME_EDGEINSPGROUPMODU = "IMVSEdgeInspGroupModu";

	/// <summary>
	/// CH: 边缘对组合缺陷检测模块 | EN: Edge pair combination defect detection module
	/// </summary>
	public const string MODU_NAME_EDGEPAIRINSPGROUPMODU = "IMVSEdgePairInspGroupModu";

	/// <summary>
	/// CH: 边缘模型缺陷检测模块 | EN: Edge model defect detection module
	/// </summary>
	public const string MODU_NAME_EDGEFLAWINSPMODU = "IMVSEdgeFlawInspModu";

	/// <summary>
	/// CH: 边缘对模型缺陷检测模块 | EN: Edge pair model defect detection module
	/// </summary>
	public const string MODU_NAME_EDGEPAIRFLAWINSPMODU = "IMVSEdgePairFlawInspModu";

	/// <summary>
	/// CH: 分支字符模块（单进程） | EN: Character comparison module (for single process)
	/// </summary>
	public const string MODU_NAME_BRANCHSTRINGCPML = "BranchStringCpmL";

	/// <summary>
	/// CH: 缓存图像模块 | EN: Cache image module
	/// </summary>
	public const string MODU_NAME_IMAGEBUFFERMODULE = "ImageBufferModule";

	/// <summary>
	/// CH: 灰度模型匹配模块 | EN: Grayscale model matching module
	/// </summary>
	public const string MODU_NAME_GRAYMATCHMODU = "IMVSGrayMatchModu";

	/// <summary>
	/// CH: 颜色识别模块 | EN: Color distinguish module
	/// </summary>
	public const string MODU_NAME_COLORDISTINGUISHMODU = "IMVSColorDistinguishModu";

	/// <summary>
	/// CH: 图像矫正模块 | EN: Image correction module
	/// </summary>
	public const string MODU_NAME_IMAGEUNDODISTORTMODU = "IMVSImageUndoDistortModu";

	/// <summary>
	/// CH: 相机IO通信模块 | EN: Camera IO communication module
	/// </summary>
	public const string MODU_NAME_CAMERAIOMODULE = "CameraIOModule";

	/// <summary>
	/// CH: 图像源模块 | EN: Image source module
	/// </summary>
	public const string MODU_NAME_IMAGESOURCEMODULE = "ImageSourceModule";

	/// <summary>
	/// CH: 光源模块 | EN: Light module
	/// </summary>
	public const string MODU_NAME_LIGHTMODULE = "LightModule";

	/// <summary>
	/// CH: 四边形查找模块 | EN: Quadrangle find module
	/// </summary>
	public const string MODU_NAME_QUADRANGEFINDMODULE = "IMVSQuadrangleFindModu";

	/// <summary>
	/// CH: 直线查找组合模块 | EN: Line find group module
	/// </summary>
	public const string MODU_NAME_LINEFINDGROUPMODULE = "IMVSLineFindGroupModu";

	/// <summary>
	/// CH: 多直线查找模块 | EN: Multi-line find module
	/// </summary>
	public const string MODU_NAME_MULTILINEFINDMODULE = "IMVSMultiLineFindModu";

	/// <summary>
	/// CH: 映射标定模块 | EN: Mapping calibration module
	/// </summary>
	public const string MODU_NAME_TWOIMGCALIBMODULE = "IMVSTwoImgCalibModu";

	/// <summary>
	/// CH: N图像标定模块 | EN: N-Image calibration module
	/// </summary>
	public const string MODU_NAME_NIMGCALIBMODULE = "IMVSNImgCalibModu";

	/// <summary>
	/// CH: 图像拼接模块 | EN: Image stitching module
	/// </summary>
	public const string MODU_NAME_IMGSTITCHMODULE = "IMVSImgStitchCalibModu";

	/// <summary>
	/// CH: 几何变换模块 | EN: Geometric transform module
	/// </summary>
	public const string MODU_NAME_GEOMETRICTRANSFORMMODULE = "IMVSGeometricTransformModu";

	/// <summary>
	/// CH: 数据集合模块 | EN: Data set module
	/// </summary>
	public const string MODU_NAME_DATASETMODULE = "DataSetModule";

	/// <summary>
	/// CH: 协议组装模块 | EN: Protocol assemble module
	/// </summary>
	public const string MODU_NAME_DATAASSEMBLEMODULE = "DataAssembleModule";

	/// <summary>
	/// CH: 协议解析模块 | EN: Protocol analysis module
	/// </summary>
	public const string MODU_NAME_DATAANALYSISMODULE = "DataAnalysisModule";

	/// <summary>
	/// CH: 深度学习单字符检测模块 （GPU版本） | EN: Single character detection module (for deep learning), GPU version
	/// </summary>
	public const string MODU_NAME_CNNSINGLECHARDETECTMODU = "IMVSCnnSingleCharDetectModu";

	/// <summary>
	/// CH: 深度学习单字符检测模块 （CPU版本） | EN: Single character detection module (for deep learning), CPU version
	/// </summary>
	public const string MODU_NAME_CNNSINGLECHARDETECTMODUC = "IMVSCnnSingleCharDetectModuC";

	/// <summary>
	/// CH: Group模块 | EN: Group module
	/// </summary>
	public const string MODU_NAME_GROUPMODULE = "IMVSGroup";

	public const uint IMVS_IMG_FORMAT_3D_ABC16 = 36700345u;

	public const uint IMVS_IMG_FORMAT_3D_C16 = 17825976u;

	public static bool IsGlobalModule(uint moduleType)
	{
		if (moduleType == 3 || moduleType == 4 || moduleType == 5 || moduleType == 7 || moduleType == 6)
		{
			return true;
		}
		return false;
	}

	internal static IntPtr GetByteFromMemoryAddress(string memoryAddress, out int lenth)
	{
		byte[] byteData = null;
		return MemoryHelper.VMReadFromMemory(memoryAddress, 16, out lenth, ref byteData);
	}

	internal static IntPtr GetByteFromMemoryAddress(string memoryAddress, out int lenth, ref byte[] byteData, bool bUseMemPool = true)
	{
		return MemoryHelper.VMReadFromMemory(memoryAddress, 16, out lenth, ref byteData, bUseMemPool);
	}

	internal static IntPtr GetByteFromMemoryIntptr(IntPtr memoryPtr, int size, ref bool isExcOccur, bool bUseMemPool = true)
	{
		return MemoryHelper.VMReadFromMemoryPtr(memoryPtr, size, ref isExcOccur, bUseMemPool);
	}
}
