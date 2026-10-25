using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apps.Data;
using Apps.ErrorCode;
using Apps.Localization;
using Apps.Log;
using Apps.UIHelper.Extern;
using Hik.Util;
using UIVersionControlManager;
using VM.Controls;
using VM.PlatformSDKCS;
using VM.Utility;
using VM.Utility.SingleInstance;
using VMControls.Interface;

namespace VM.Core;

public class VmSolution : VmModuleSet, IVmSolution, IChildren, IZoomIndex, IVMRun
{
	public enum ContinuousRunStatus
	{
		EndOnce,
		StartOnce
	}

	public delegate void ContinuousRunStatusCallBack(ContinuousRunStatus stat, uint procID);

	public delegate void ModuleInfoCallBack(object sender, ModuInfo cameraInfo);

	internal struct WaitNotifyPage
	{
		internal bool IsRunning;

		internal uint FunID;

		internal int OperID;
	}

	private const uint DEFAULT_PROCESS_ID = 100000u;

	private const uint GLOBAL_ID = 70000u;

	private const string GLOBAL_KEY = "VMSOLUTION";

	private bool isDisplayProgress;

	private bool isGlobalRuning = false;

	private bool _IsContinuousSelfExecute = false;

	private bool _isSaving = false;

	private bool gloalScriptContinusRunEnable = false;

	private Dictionary<uint, uint> dictProcessIDStopStatus = new Dictionary<uint, uint>();

	private static bool isLoading;

	internal static bool _failReflashFlag;

	internal static object _failFlagObject;

	private static bool isTakeOver;

	private static object SolObj;

	private static uint nGroupId;

	private static string strGroupName;

	internal static Thread refreshParamthread;

	private static bool dataNumForReceiveStore;

	private static uint nProcessId;

	private static string strProcessName;

	/// <summary>
	/// 流程模块加载完成标记
	/// </summary>
	private static bool mbImportProcedureEndFlag;

	/// <summary>
	/// 组模块加载完成标记
	/// </summary>
	private static bool mbImportGroupEndFlag;

	/// <summary>
	/// 正在加载的Group的上级
	/// </summary>
	private IVmModuleSet importGroupModuleSet;

	private VmModule importedGroupModule;

	private VmProcedure importedProcedure;

	internal static bool mbExportProcedureEndFlag;

	internal static int MaxModuleCount;

	/// <summary>
	/// 流程升级标志
	/// </summary>
	internal static bool mbUpgradeProcedureEndFlag;

	/// <summary>
	/// Group升级标志
	/// </summary>
	internal static bool mbUpgradeGroupEndFlag;

	internal bool IsDispose = false;

	/// <summary>
	/// 所有生成的模块，包含流程和组模块
	/// </summary>
	internal ConcurrentDictionary<uint, VmModule> AllModules = new ConcurrentDictionary<uint, VmModule>();

	public static VmIO ExternIO;

	private int proceZoomIndex = 9;

	/// <summary>
	/// 标识导入流程是否忽略全局模块
	/// </summary>
	internal static bool IsIgnoreGlobalModule;

	public static bool IsParamModifyExecute;

	public static bool IsModelModifyExecute;

	private static VmSolution instance;

	private static Dictionary<uint, WaitNotifyPage> _waitNotifyPages;

	private static Thread refreshNotifyPageThread;

	private static object waitNotifyLock;

	private static Dictionary<uint, int> _waitDoneProcedureCache;

	private static Thread refreshProcessTimeThread;

	private static object currentObj;

	internal static ConcurrentDictionary<uint, bool> _updateContinueStatus;

	/// <summary>
	/// 方案执行事件标志
	/// </summary>
	private static bool mbSolRunFlag;

	private static int mnSolRunErrorde;

	private static string Import_path;

	private static string Import_password;

	private bool isOnExecuteAfterLoad;

	private static DogStateEnum _DogState;

	private static string _strDogState;

	private static string _strDongleSerialNo;

	private static int _softLanguage;

	/// <summary>
	/// 相机数量
	/// </summary>
	private static int _cameraNum;

	/// <summary>
	/// 加密狗剩余时间(小时数)
	/// </summary>
	private static int _RemainTime;

	private static int _MemeryUsed;

	private static int _CPUUsed;

	private static int _NetUsed;

	private static Thread callbackthread;

	private static bool isCallBackThreadAlive;

	private static object threadLock;

	private static int callbackNumForReceiveStore;

	private static int callbackNumForSendStore;

	private static int maxCallBackStoreLen;

	private static object[] callbackReceiveStore;

	private static object[] callbackSendStore;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool <IsCallBackCacheEnable>k__BackingField;

	/// <summary>
	/// 一键标定相关数据，导入一键标定流程时使用
	/// </summary>
	internal object importCalibData = null;

	private object customModule;

	private static bool isClearLastExecuteResult;

	private static bool isOpenSetValueToDisplay;

	private bool? isSubscribeInWindow = null;

	internal static VmSolution VirtualSol { get; }

	public static ConcurrentDictionary<string, string> ModulePathList { get; }

	/// <summary>
	/// 虚拟流程，用于存放独立group
	/// </summary>
	internal static VmIndependentProcedure VirtualPro { get; }

	/// <summary>
	/// 全局运行标志位
	/// </summary>
	private bool IsGlobalRuning
	{
		get
		{
			return isGlobalRuning;
		}
		set
		{
			if (isGlobalRuning != value)
			{
				this.SetProperty(ref isGlobalRuning, value, "IsGlobalRuning");
			}
		}
	}

	/// <summary>
	/// 辅助标志，由单独运行流程导致
	/// </summary>
	internal bool InternalContinousRunEnable => AllModules.Values.OfType<VmProcedure>().Any((VmProcedure p) => p.ContinuousRunEnable);

	/// <summary>
	/// 是否正在保存方案
	/// </summary>
	private bool IsSaving => _isSaving;

	/// <summary>
	/// 是否正在加载方案
	/// </summary>
	internal static bool IsLoading => isLoading;

	/// <summary>
	/// 全流程百分比
	/// </summary>
	internal int ProceZoomIndex
	{
		get
		{
			byte[] array = ((IVmModule)this).UserData["ProceZoomIndex"];
			if (array == null)
			{
				return proceZoomIndex;
			}
			return int.Parse(Hik.Util.StringUtil.UTF8GetString(array));
		}
		set
		{
			if (proceZoomIndex != value)
			{
				proceZoomIndex = value;
				try
				{
					((IVmModule)this).UserData["ProceZoomIndex"] = Hik.Util.StringUtil.UTF8GetBytesPadZero(proceZoomIndex.ToString());
				}
				catch (Exception ex)
				{
					HikMessageBox.Show(ex.Message);
				}
				OnPropertyChanged("PartentZoomIndex");
			}
		}
	}

	internal static bool FailReflashFlag => _failReflashFlag;

	/// <summary>
	/// 是否接管
	/// </summary>
	internal static bool IsTakeOver => isTakeOver;

	/// <summary>
	/// 方案密码(内部使用)
	/// </summary>
	private string SolutionPassword
	{
		get
		{
			return ModuleFilePassword;
		}
		set
		{
			ModuleFilePassword = value;
		}
	}

	/// <summary>
	/// 流程ID
	/// </summary>
	public override uint ID
	{
		get
		{
			return 70000u;
		}
		set
		{
			throw new Exception("预留,未实现");
		}
	}

	/// <summary>
	/// 运行标志位（所有流程标志位的或）
	/// </summary>
	public override bool IsRunning
	{
		get
		{
			bool flag = Instance.AllModules.Values.Where((VmModule p) => p != null && p.Parent == this).OfType<VmProcedure>().Any((VmProcedure p) => p.IsRunning);
			Ioc.Get<IUnRedoService>()?.SetRunningStatus(flag);
			return flag;
		}
	}

	/// <summary>
	/// 连续运行 | 停止执行标志
	/// </summary>
	public override bool ContinuousRunEnable
	{
		get
		{
			return GloalScriptContinusRunEnable || InternalContinousRunEnable || _IsContinuousSelfExecute;
		}
		set
		{
			if (GloalScriptContinusRunEnable != value)
			{
				GloalScriptContinusRunEnable = value;
			}
			if (!value)
			{
				((IVmSolution)this).IsContinuousSelfExecute = value;
			}
			if (!value)
			{
				Modules.OfType<VmProcedure>().ToList().ForEach((VmProcedure p) =>
				{
					p.ContinuousRunEnable = value;
				});
			}
		}
	}

	/// <summary>
	/// 全局脚本连续运行|停止运行标志位
	/// </summary>
	public bool GloalScriptContinusRunEnable
	{
		get
		{
			return gloalScriptContinusRunEnable;
		}
		set
		{
			if (gloalScriptContinusRunEnable == value)
			{
				return;
			}
			if (value)
			{
				ServerSDKManager.serverSDKManager.mGlobalModuleManager.GlobalScriptStartContinueRun();
				foreach (VmProcedure item in Modules.OfType<VmProcedure>())
				{
					if (dictProcessIDStopStatus.ContainsKey(item.ID))
					{
						dictProcessIDStopStatus[item.ID] = 0u;
					}
					else
					{
						dictProcessIDStopStatus.Add(item.ID, 0u);
					}
				}
			}
			else
			{
				ServerSDKManager.serverSDKManager.mGlobalModuleManager.GlobalScriptStopRun();
				dictProcessIDStopStatus.Clear();
			}
			gloalScriptContinusRunEnable = value;
			Modules.OfType<VmProcedure>().ToList().ForEach((VmProcedure x) =>
			{
				x.IsGlobalRuning = value;
			});
			OnPropertyChanged("IsRunning");
			OnPropertyChanged("ContinuousRunEnable");
			OnPropertyChanged("GloalScriptContinusRunEnable");
		}
	}

	/// <summary>
	/// 方案路径
	/// </summary>
	public string SolutionPath
	{
		get
		{
			return ModuleFilePath;
		}
		private set
		{
			ModuleFilePath = value;
		}
	}

	/// <summary>
	/// 当前方案路径是否包含密码
	/// </summary>
	public bool IsHasPassword
	{
		get
		{
			if (!string.IsNullOrEmpty(SolutionPassword))
			{
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// 路径获取对象
	/// </summary>
	public override object this[string sub]
	{
		get
		{
			if (sub == null)
			{
				return null;
			}
			if (sub.Length == 0)
			{
				return this;
			}
			if (sub.Contains("[Params]"))
			{
				return sub;
			}
			string[] array = sub.Split('.');
			string procInfo = array[0];
			VmModule vmModule = Modules.FirstOrDefault((VmModule p) => p.Name == procInfo);
			if (vmModule == null && procInfo == VirtualPro.Name)
			{
				vmModule = VirtualPro;
			}
			if (procInfo == "[UserVar]")
			{
				vmModule = AllModules.Values?.FirstOrDefault((VmModule p) => p != null && ((IVmModule)p).ModuleType == 5);
			}
			if (sub.Contains('.'))
			{
				return vmModule?[sub.Remove(0, array[0].Length + 1)];
			}
			return vmModule;
		}
	}

	/// <summary>
	/// 方案模块名称
	/// </summary>
	public override string StrModuleName
	{
		get
		{
			if (_StrModuleName == null)
			{
				_StrModuleName = "SolutionModule";
			}
			return _StrModuleName;
		}
		set
		{
		}
	}

	public static VmSolution Instance
	{
		get
		{
			return instance;
		}
		private set
		{
			if (instance == value)
			{
				return;
			}
			instance.AllModules.Values.ToList().ForEach((VmModule p) =>
			{
				if (p != null)
				{
					p.IsActive = false;
				}
			});
			VmSolution param = instance;
			instance = value;
			RestoreDefaultDisplay?.Invoke(SolObj, new ObjectEventArgs
			{
				Param = param
			});
		}
	}

	object IVmSolution.this[string sub] => this[sub];

	IList IChildren.Children => Modules;

	bool IVMRun.IsRunning => IsRunning || IsGlobalRuning || gloalScriptContinusRunEnable || _IsContinuousSelfExecute;

	bool IVmSolution.IsContinuousSelfExecute
	{
		get
		{
			return _IsContinuousSelfExecute;
		}
		set
		{
			if (_IsContinuousSelfExecute != value)
			{
				_IsContinuousSelfExecute = value;
				Modules.OfType<VmProcedure>().ToList().ForEach((VmProcedure x) =>
				{
					x.IsGlobalRuning = value;
				});
				OnPropertyChanged("IsRunning");
				OnPropertyChanged("ContinuousRunEnable");
				OnPropertyChanged("IsContinuousSelfExecute");
			}
		}
	}

	int IZoomIndex.PartentZoomIndex
	{
		get
		{
			return ProceZoomIndex;
		}
		set
		{
			ProceZoomIndex = value;
		}
	}

	bool IVmSolution.IsLoading => IsLoading;

	bool IVmSolution.IsSaving => IsSaving;

	bool IVmSolution.IsTakeOver => IsTakeOver;

	bool IVmSolution.IsIgnoreGlobalModule
	{
		get
		{
			return IsIgnoreGlobalModule;
		}
		set
		{
			IsIgnoreGlobalModule = value;
		}
	}

	bool IVmSolution.IsSolutionModified => IsSolModifyInfo();

	/// <summary>
	/// 获取句柄（内部使用）
	/// </summary>
	public IntPtr BaseHandle => CVmObjectBase.mhandle;

	/// <summary>
	/// 是否正在静默执行
	/// </summary>
	public bool IsOnExecuteAfterLoad
	{
		get
		{
			return isOnExecuteAfterLoad;
		}
		set
		{
			if (isOnExecuteAfterLoad != value)
			{
				isOnExecuteAfterLoad = value;
				OnPropertyChanged("IsOnExecuteAfterLoad");
			}
		}
	}

	public static DogStateEnum DogState
	{
		get
		{
			return _DogState;
		}
		set
		{
			if (_DogState != value)
			{
				_DogState = value;
				instance.OnPropertyChanged("DogState");
			}
		}
	}

	public static string StrDogState
	{
		get
		{
			return _strDogState;
		}
		set
		{
			if (_strDogState != value)
			{
				_strDogState = value;
				instance.OnPropertyChanged("StrDogState");
			}
		}
	}

	public static string StrDongleSerialNo
	{
		get
		{
			return _strDongleSerialNo;
		}
		set
		{
			if (_strDongleSerialNo != value)
			{
				_strDongleSerialNo = value;
				instance.OnPropertyChanged("StrDongleSerialNo");
			}
		}
	}

	public static int SoftLanguage
	{
		get
		{
			return _softLanguage;
		}
		set
		{
			if (_softLanguage != value)
			{
				_softLanguage = value;
				instance.OnPropertyChanged("SoftLanguage");
			}
		}
	}

	public static int CameraNum
	{
		get
		{
			return _cameraNum;
		}
		set
		{
			if (_cameraNum != value)
			{
				_cameraNum = value;
				instance.OnPropertyChanged("CameraNum");
			}
		}
	}

	public static int RemainTime
	{
		get
		{
			return _RemainTime;
		}
		set
		{
			if (_RemainTime != value)
			{
				_RemainTime = value;
				instance.OnPropertyChanged("RemainTime");
			}
		}
	}

	/// <summary>
	/// 内存使用率
	/// </summary>
	public static int MemeryUsed
	{
		get
		{
			return _MemeryUsed;
		}
		set
		{
			if (_MemeryUsed != value)
			{
				_MemeryUsed = value;
				instance.OnPropertyChanged("MemeryUsed");
			}
		}
	}

	/// <summary>
	/// CPU使用率
	/// </summary>
	public static int CPUUsed
	{
		get
		{
			return _CPUUsed;
		}
		set
		{
			if (_CPUUsed != value)
			{
				_CPUUsed = value;
				instance.OnPropertyChanged("CPUUsed");
			}
		}
	}

	/// <summary>
	/// 网络使用率
	/// </summary>
	public static int NetUsed
	{
		get
		{
			return _NetUsed;
		}
		set
		{
			if (_NetUsed != value)
			{
				_NetUsed = value;
				instance.OnPropertyChanged("NetUsed");
			}
		}
	}

	/// <summary>
	/// 是否开启回调数据缓存
	/// </summary>
	public static bool IsCallBackCacheEnable
	{
		[CompilerGenerated]
		get
		{
			return <IsCallBackCacheEnable>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			<IsCallBackCacheEnable>k__BackingField = value;
		}
	}

	[ParsePropertyVisible]
	object IVmSolution.CustomModule
	{
		get
		{
			return customModule;
		}
		set
		{
			customModule = value;
		}
	}

	/// <summary>
	/// 流程执行结束后是否清空上一次执行结果
	/// </summary>
	bool IVmSolution.IsClearLastExecuteResult
	{
		get
		{
			return isClearLastExecuteResult;
		}
		set
		{
			isClearLastExecuteResult = value;
		}
	}

	/// <summary>
	/// VmIO值改变时是否通知界面
	/// </summary>
	bool IVmSolution.IsOpenSetValueToDisplay
	{
		get
		{
			return isOpenSetValueToDisplay;
		}
		set
		{
			isOpenSetValueToDisplay = value;
		}
	}

	/// <summary>
	/// 是否默认开启悬浮订阅窗口
	/// </summary>
	bool IVmSolution.IsSubscribeInWindow
	{
		get
		{
			if (!isSubscribeInWindow.HasValue)
			{
				byte[] array = null;
				bool result = false;
				try
				{
					array = Instance.UserData["IsSubscribeInWindow"];
					if (array != null)
					{
						bool.TryParse(ByteUtil.GetStringFromByte(array), out result);
					}
					isSubscribeInWindow = result;
				}
				catch (Exception)
				{
				}
			}
			return isSubscribeInWindow ?? false;
		}
		set
		{
			isSubscribeInWindow = value;
			OnPropertyChanged("IsSubscribeInWindow");
			try
			{
				byte[] value2 = null;
				if (isSubscribeInWindow.HasValue)
				{
					value2 = ByteUtil.GetBytes(isSubscribeInWindow.ToString());
				}
				Instance.UserData["IsSubscribeInWindow"] = value2;
			}
			catch (Exception)
			{
			}
		}
	}

	public event ContinuousRunStatusCallBack OnContinuousRunStatusCallBack;

	public static event EventHandler InstanceChanged;

	public static event EventHandler RestoreDefaultDisplay;

	public static event ObjectEventHandler OnSolutionLoadBeginCallBack;

	public static event EndEventHandler OnSolutionLoadEndCallBack;

	public static event ObjectEventHandler OnModelLoadWarn;

	public static event ObjectEventHandler OnSolutionSaveBeginCallBack;

	public static event EndEventHandler OnSolutionSaveEndCallBack;

	public static event ObjectEventHandler OnGlobalScriptReportCallBack;

	public static event EventHandler OnExecuteAfterLoadStart;

	public static event ObjectEventHandler OnExecuteAfterLoad;

	public static event EventHandler OnExecuteAfterLoadFinally;

	public static event ObjectEventHandler OnReportModuleData;

	/// <summary>
	/// 模块方案加载进度
	/// </summary>
	public static event ObjectEventHandler OnSolutionLoadProgressCallBack;

	public static event ObjectEventHandler OnSolutionSaveProgressCallBack;

	public static event ObjectEventHandler OnProcessImportBeginCallBack;

	public static event EndEventHandler OnProcessImportEndCallBack;

	public static event ObjectEventHandler OnProcessImportProgressCallBack;

	public static event ObjectEventHandler OnProcessExportBeginCallBack;

	public static event ObjectEventHandler OnProcessExportProgressCallBack;

	public static event EndEventHandler OnProcessExportEndCallBack;

	public static event ObjectEventHandler OnGroupImportBeginCallBack;

	public static event EndEventHandler OnGroupImportEndCallBack;

	public static event ObjectEventHandler OnGroupImportProgressCallBack;

	public static event ObjectEventHandler OnGroupExportBeginCallBack;

	public static event EndEventHandler OnGroupExportEndCallBack;

	public static event ObjectEventHandler OnGroupExportProgressCallBack;

	public static event ObjectEventHandler OnProxyCrashNoticeCallBack;

	public static event ObjectEventHandler OnServerStatusCallBack;

	public static event ObjectEventHandler OnProcedureUnRegister;

	public static event ObjectEventHandler OnProcedureUpgradeBeginCallBack;

	public static event ObjectEventHandler OnProcedureUpgradeEndCallBack;

	public static event ObjectEventHandler OnGroupUpgradeBeginCallBack;

	public static event ObjectEventHandler OnGroupUpgradeEndCallBack;

	/// <summary>
	/// 流程工作状态
	/// </summary>
	public static event WorkStatusCallBack OnWorkStatusEvent;

	/// <summary>
	/// 加密狗状态
	/// </summary>
	public static event DongleCallBack OnDongleEvent;

	/// <summary>
	/// 开始加载方案
	/// </summary>
	public static event ModuleSolutionLoadBeginCallBack OnSolutionLoadBeginEvent;

	/// <summary>
	/// 方案加载结束
	/// </summary>
	public static event ModuleSolutionLoadEndCallBack OnSolutionLoadEndEvent;

	/// <summary>
	/// 模块方案加载进度
	/// </summary>
	public static event ModuleSolutionLoadProcessCallBack OnSolutionLoadProgressEvent;

	/// <summary>
	/// 服务状态回调 (目前服务异常时触发，状态对应错误码，VM单进程版本不触发此事件)
	/// </summary>
	public static event ServerStatusCallBack OnServerStatusEvent;

	/// <summary>
	/// 代理崩溃 (VM单进程版本不触发此事件)
	/// </summary>
	public static event ProxyCrashNotice OnProxyCrashEvent;

	/// <summary>
	/// 流程解注册状态信息
	/// </summary>
	public static event ProcedureUnRegisterCallBack OnProcedureUnRegisterEvent;

	/// <summary>
	/// 方案加载时模块错误警告信息
	/// </summary>
	public static event ModelLoadWarn OnModelLoadWarnEvent;

	/// <summary>
	/// 连续执行开始状态信息
	/// </summary>
	public static event ModuleStartContinuouslyProcessStatusCallBack OnProcessStatusStartEvent;

	/// <summary>
	/// 连续执行结束状态信息
	/// </summary>
	public static event ModuleStopRunProcessStatusCallBack OnProcessStatusStopEvent;

	/// <summary>
	/// 模块结果信息回调
	/// </summary>
	public static event ModuleResultExDataCallBack OnModuleResultCallbackEvent;

	/// <summary>
	/// 接收数据回调 (设备ID(1字节) AddressID(1字节) 数据)
	/// </summary>
	public static event CommunicationDataCallBack OnCommunicationRecvCallBackEvent;

	/// <summary>
	/// 通信状态回调 (状态 设备ID(1字节))
	/// </summary>
	public static event CommunicationDataCallBack OnCommunicationStatusCallBackEvent;

	/// <summary>
	/// 相机取图开始信息回调 需要在MVS中开启相机Event中的Frame Start
	/// </summary>
	public static event CameraCollectCallBack OnCameraCollectStartCallBackEvent;

	/// <summary>
	/// 相机取图结束信息回调
	/// </summary>
	public static event CameraCollectCallBack OnCameraCollectCallBackEvent;

	/// <summary>
	/// 相机连接状态回调（相机断线后，打开全局相机模块配置界面需要较长时间，请耐心等待！）
	/// </summary>
	public static event CameraConnectStatusCallBack OnCameraConnectStatusCallBackEvent;

	/// <summary>
	/// 相机模块添加事件
	/// </summary>
	public static event ModuleInfoCallBack OnGlobalCameraModuleAddedEvent;

	/// <summary>
	/// 相机模块删除事件
	/// </summary>
	public static event ModuleInfoCallBack OnGlobalCameraModuleDeletedEvent;

	/// <summary>
	/// 相机连接状态回调
	/// </summary>
	private event EventHandler<EventArgs> OnCameraConnectStatusCallBackEventF;

	event EventHandler<EventArgs> IVmSolution.OnCameraConnectStatusCallBackEvent
	{
		add
		{
			OnCameraConnectStatusCallBackEventF += value;
		}
		remove
		{
			OnCameraConnectStatusCallBackEventF -= value;
		}
	}

	/// <summary>
	/// 通信状态回调 (状态 设备ID(1字节))
	/// </summary>
	private event EventHandler<EventArgs> OnCommunicationStatusCallBackEventF;

	event EventHandler<EventArgs> IVmSolution.OnCommunicationStatusCallBackEvent
	{
		add
		{
			OnCommunicationStatusCallBackEventF += value;
		}
		remove
		{
			OnCommunicationStatusCallBackEventF -= value;
		}
	}

	/// <summary>
	/// 通信连接状态回调
	/// </summary>
	public static event CommuConnectCallBack OnCommuConnectCallBackEvent;

	internal event ReportModuleStatusCallBack reportModuleStatusCallBack;

	/// <summary>
	/// 相机模块重命名事件
	/// </summary>
	private static event EventHandler OnGlobalCameraModuleRenamedEvent;

	event EventHandler IVmSolution.OnGlobalCameraModuleRenamedEvent
	{
		add
		{
			OnGlobalCameraModuleRenamedEvent += value;
		}
		remove
		{
			OnGlobalCameraModuleRenamedEvent -= value;
		}
	}

	/// <summary>
	/// 新增模块事件
	/// </summary>
	private static event EventHandler OnModuleAddedEvent;

	event EventHandler IVmSolution.OnModuleAddedEvent
	{
		add
		{
			OnModuleAddedEvent += value;
		}
		remove
		{
			OnModuleAddedEvent -= value;
		}
	}

	private static event EventHandler ModuleActionEvnetChangedCallBack;

	event EventHandler IVmSolution.ModuleActionEvnetChangedCallBack
	{
		add
		{
			ModuleActionEvnetChangedCallBack += value;
		}
		remove
		{
			ModuleActionEvnetChangedCallBack -= value;
		}
	}

	/// <summary>
	/// 通过GUID路径获取对象,IO还是名字
	/// </summary>
	/// <returns></returns>
	public override object GetObjectByGUIDPath(string sub)
	{
		if (sub == null)
		{
			return null;
		}
		if (sub.Length == 0)
		{
			return this;
		}
		if (sub.Contains("[Params]"))
		{
			return sub;
		}
		string[] array = sub.Split('.');
		string procInfo = array[0];
		VmModule vmModule = Modules.FirstOrDefault((VmModule p) => p.GUID == procInfo);
		if (vmModule == null && procInfo == VirtualPro.Name)
		{
			vmModule = VirtualPro;
		}
		if (sub.Contains('.'))
		{
			return vmModule?.GetObjectByGUIDPath(sub.Remove(0, array[0].Length + 1));
		}
		return vmModule;
	}

	private void ResetInstance()
	{
		instance.AllModules.Values.ToList().ForEach((VmModule p) =>
		{
			if (p != null)
			{
				p.IsActive = false;
			}
			if (p is IVmProcedure vmProcedure)
			{
				vmProcedure.DeleteRefrence();
			}
		});
		RestoreDefaultDisplay?.Invoke(SolObj, new ObjectEventArgs());
		instance.AllModules = new ConcurrentDictionary<uint, VmModule>();
		instance.ModuleFilePassword = null;
		instance.ModuleFilePath = null;
		instance.gloalScriptContinusRunEnable = false;
		ResetLazyBuilder();
		instance.isSubscribeInWindow = null;
	}

	static VmSolution()
	{
		_failReflashFlag = true;
		_failFlagObject = new object();
		SolObj = new object();
		nGroupId = 0u;
		strGroupName = "";
		refreshParamthread = null;
		dataNumForReceiveStore = false;
		nProcessId = 0u;
		strProcessName = "";
		mbImportProcedureEndFlag = true;
		mbImportGroupEndFlag = true;
		mbExportProcedureEndFlag = true;
		MaxModuleCount = 2048;
		mbUpgradeProcedureEndFlag = true;
		mbUpgradeGroupEndFlag = true;
		VirtualSol = new VmSolution();
		ExternIO = new VmIO
		{
			Name = "TRIGGER_STRING",
			TypeName = "string"
		};
		ModulePathList = new ConcurrentDictionary<string, string>();
		IsIgnoreGlobalModule = false;
		IsParamModifyExecute = false;
		IsModelModifyExecute = true;
		instance = new VmSolution();
		_waitNotifyPages = new Dictionary<uint, WaitNotifyPage>();
		refreshNotifyPageThread = null;
		waitNotifyLock = new object();
		_waitDoneProcedureCache = new Dictionary<uint, int>();
		refreshProcessTimeThread = null;
		currentObj = new object();
		_updateContinueStatus = new ConcurrentDictionary<uint, bool>();
		mbSolRunFlag = false;
		mnSolRunErrorde = 0;
		_DogState = DogStateEnum.Default;
		_softLanguage = -1;
		_RemainTime = int.MaxValue;
		_MemeryUsed = -1;
		_CPUUsed = -1;
		_NetUsed = -1;
		callbackthread = null;
		isCallBackThreadAlive = true;
		threadLock = new object();
		callbackNumForReceiveStore = 0;
		callbackNumForSendStore = 0;
		maxCallBackStoreLen = 10000;
		callbackReceiveStore = new object[maxCallBackStoreLen];
		callbackSendStore = new object[maxCallBackStoreLen];
		<IsCallBackCacheEnable>k__BackingField = false;
		isClearLastExecuteResult = false;
		isOpenSetValueToDisplay = true;
		string text = "";
		string currentPath = CurrentPathHelper.Instance.GetCurrentPath();
		using (StreamReader streamReader = new StreamReader($"{currentPath}ModulePathGenerate.txt"))
		{
			while ((text = streamReader.ReadLine()) != null)
			{
				string[] array = text.Split(new string[1] { " # " }, StringSplitOptions.None);
				ModulePathList.TryAdd(array[0], currentPath + array[1]);
			}
		}
		VirtualPro = new VmIndependentProcedure(100000u, "流程100000");
		VirtualPro.Parent = VirtualSol;
		refreshParamthread = new Thread(() =>
		{
			while (true)
			{
				if (dataNumForReceiveStore)
				{
					lock (refreshParamthread)
					{
						dataNumForReceiveStore = false;
					}
					instance.reportModuleStatusCallBack?.Invoke(default);
					VirtualPro.VmSolution_reportModuleStatusCallBack(default);
				}
				Thread.Sleep(15);
			}
		})
		{
			IsBackground = true,
			Priority = ThreadPriority.BelowNormal
		};
		refreshParamthread.Start();
		CallBackExecutAndProcessTime();
		CreateCallBackQueue();
		CallBackWaitNotify();
	}

	private static void CallBackWaitNotify()
	{
		refreshNotifyPageThread = new Thread(() =>
		{
			while (true)
			{
				try
				{
					if (_waitNotifyPages.Count > 0)
					{
						Queue<WaitNotifyPage> queue = new Queue<WaitNotifyPage>();
						lock (waitNotifyLock)
						{
							foreach (WaitNotifyPage value2 in _waitNotifyPages.Values)
							{
								queue.Enqueue(value2);
							}
							_waitNotifyPages = new Dictionary<uint, WaitNotifyPage>();
						}
						while (queue.Count > 0)
						{
							WaitNotifyPage waitNotifyPage = queue.Dequeue();
							instance.AllModules.TryGetValue(waitNotifyPage.FunID, out var value);
							if (value == null)
							{
								LogHelper.Error($"未从instance.AllModules捕获到流程{waitNotifyPage.FunID}");
							}
							else if (waitNotifyPage.OperID == 0)
							{
								(value as VmProcedure).UpdateRunStatus(waitNotifyPage.IsRunning);
							}
							else
							{
								(value as VmProcedure).UpdateContinusRunStatusSync(waitNotifyPage.IsRunning);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex.Message + ex.StackTrace);
				}
				SpinWait.SpinUntil(() => false, 30);
			}
		})
		{
			IsBackground = true,
			Priority = ThreadPriority.BelowNormal
		};
		refreshNotifyPageThread.Start();
	}

	private static void CallBackExecutAndProcessTime()
	{
		refreshProcessTimeThread = new Thread(() =>
		{
			while (true)
			{
				if (_waitDoneProcedureCache.Count > 0)
				{
					Dictionary<uint, int> dictionary = null;
					lock (currentObj)
					{
						dictionary = _waitDoneProcedureCache;
						_waitDoneProcedureCache = new Dictionary<uint, int>();
					}
					dictionary.Keys.ToList().ForEach((uint x) =>
					{
						try
						{
							instance.AllModules.TryGetValue(x, out var value);
							value?.OnPropertyChanged("ProcessTime");
							value?.OnPropertyChanged("ExecuteCount");
						}
						catch (Exception)
						{
							LogHelper.Error("发送PopertyChanged--ProcessTime和ExecuteCount出现异常");
						}
					});
				}
				Thread.Sleep(150);
			}
		})
		{
			IsBackground = true,
			Priority = ThreadPriority.BelowNormal
		};
		refreshProcessTimeThread.Start();
	}

	private VmSolution()
	{
	}

	/// <summary>
	/// 该类型模块是否加载
	/// </summary>
	/// <param name="moduleType"></param>
	/// <returns></returns>
	bool IVmSolution.IsModuleTypeLoaded(string moduleType)
	{
		return AllModules.Values.Any((VmModule p) => p.StrModuleName == moduleType);
	}

	/// <summary>
	/// 返回路径列表(内部使用)
	/// </summary>
	/// <returns></returns>
	List<string> IVmSolution.GetAllModulePathList()
	{
		List<string> list = new List<string>();
		ICollection<VmModule> values = instance.AllModules.Values;
		return (from p in values.OfType<IVmModule>()
			select p.ModuleDir).Distinct().ToList();
	}

	List<IVmModule> IVmSolution.GetAllModule()
	{
		ICollection<VmModule> values = instance.AllModules.Values;
		return values.OfType<IVmModule>().ToList();
	}

	/// <summary>
	/// 关闭全部模块回调(已弃用)
	/// </summary>
	[Obsolete("CH: 弃用,推荐(DisableModulesCallback()) | EN: Obsolete,recommend(DisableModulesCallback())", false)]
	void IVmSolution.CloseAllModulesCallBack()
	{
		List<VmModule> list = new List<VmModule>();
		GetAllModule(list);
		List<VmModule> list2 = list.Where((VmModule p) => p.ReportResultEnable)?.ToList();
		foreach (VmModule item in list2)
		{
			((IVmModule)item).SubscribeCount = 0;
			item.ReportResultEnable = false;
		}
	}

	/// <summary>
	/// 重置所有模块回调状态
	/// </summary>
	void IVmSolution.ResetAllModulesCallBackStatus()
	{
		try
		{
			List<VmModule> list = Instance.AllModules.Values.ToList();
			foreach (VmProcedure item in list.OfType<VmProcedure>())
			{
				IVmProcedure vmProcedure;
				if ((vmProcedure = item) != null && vmProcedure.NeedGetAllModulesCallBack)
				{
					ServerSDKManager.serverSDKManager.mSolutionManager.SetAllReportModuleResult();
					return;
				}
			}
			List<uint> list2 = list?.Where((VmModule p) => p.ReportResultEnable)?.Select((VmModule p) => p.ID)?.ToList();
			if (list2 == null || list2.Count == 0)
			{
				ServerSDKManager.serverSDKManager.mSolutionManager.SetNoneReportModuleResult();
				return;
			}
			ImvsSdkDefine.IMVS_REPORT_MODULE_RES_INFO_LIST reportModuleResult = new ImvsSdkDefine.IMVS_REPORT_MODULE_RES_INFO_LIST
			{
				nNum = list2.Count,
				anReportModuIds = new uint[2064]
			};
			for (int num = 0; num < list2.Count; num++)
			{
				reportModuleResult.anReportModuIds[num] = list2[num];
			}
			ServerSDKManager.serverSDKManager.mSolutionManager.SetReportModuleResult(reportModuleResult);
		}
		catch (Exception ex)
		{
			if (ex is VmException ex2)
			{
				LogHelper.Error(ErrorCodeInfo.GetErrorInfo(ex2.errorCode) + "Error Code = " + ex2.errorCode);
			}
		}
	}

	void IVmSolution.Import(string path)
	{
		Import(path);
	}

	void IVmSolution.Import(string path, string password)
	{
		Import(path, password);
	}

	void IVmSolution.SetServerReadyEvent()
	{
		TempVmSolution.Init();
		try
		{
			ServerSDKManager.serverSDKManager.mSolutionManager.SetServerReadyEvent();
		}
		catch (Exception)
		{
		}
	}

	void IVmSolution.Export(string path, string password, bool bDisplayProgress)
	{
		Export(path, password, bDisplayProgress);
	}

	void IVmSolution.ExportAs(string path, string password, bool bDisplayProgress)
	{
		ExportAs(path, password, bDisplayProgress);
	}

	IVmProcedure IVmSolution.CreateProcedure()
	{
		ImvsSdkDefine.IMVS_PROCESS_INFO data = CreateToProcessInfo();
		return ChangToProcess(data);
	}

	/// <summary>
	/// 新建方案(内部使用)
	/// </summary>
	void IVmSolution.CreatNewInstance()
	{
		TempVmSolution.Init();
		SolutionPassword = "";
		ServerSDKManager.serverSDKManager.mSolutionManager.CloseSolution();
		Instance = new VmSolution();
		Instance.CreatNewProcedure();
	}

	bool IVmSolution.CheckBeforeRun(ref string message)
	{
		return CheckBeforeRun(ref message);
	}

	/// <summary>
	/// 显示进度条(内部使用)
	/// </summary>
	void IVmSolution.DisplayProgress(string message)
	{
		Instance.DisplayProgress(message);
	}

	/// <summary>
	/// 关闭进度条(内部使用)
	/// </summary>
	void IVmSolution.CloseProgress()
	{
		Instance.CloseProgress();
	}

	private static void CheckSol(string path, string password)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new VmException(-536870397, string.Format("(VmSolution.Load(string path)) path:{0}", AppLocalizationService.GetLang("Lang_ParamErr")));
		}
		uint nHasPwd = 0u;
		uint nValidFormat = 0u;
		ServerSDKManager.serverSDKManager.mSolutionManager.CheckSolution(path, ref nHasPwd, ref nValidFormat);
		if (nHasPwd == 1 && string.IsNullOrEmpty(password))
		{
			throw new VmException(-536868864, string.Format("(VmSolution.Load(string path)) {0}", AppLocalizationService.GetLang("请输入密码")));
		}
	}

	private static bool InternalExport(string path = null, string password = null, bool bDisplayProgress = true, bool isExportAs = false, string initDir = null)
	{
		try
		{
			TempVmSolution.Init();
			if (Instance._isSaving)
			{
				return false;
			}
			Instance._isSaving = true;
			LogHelper.Debug($"InternalExport_isSaving_{Instance._isSaving}");
			string text = "";
			Instance.isDisplayProgress = bDisplayProgress;
			if (password == null)
			{
				if (Instance.SolutionPassword != null)
				{
					text = Instance.SolutionPassword;
				}
			}
			else
			{
				text = password;
			}
			if (path == null)
			{
				path = ((isExportAs || Instance.ModuleFilePath == null) ? ServerSDKManager.serverSDKManager.mCollectionManager.OpenExportDialog("Lang_SolutionFile", $"|*.{AppFileExtension.Solution}", initDir) : Instance.ModuleFilePath);
			}
			if (string.IsNullOrEmpty(path))
			{
				Instance._isSaving = false;
				return false;
			}
			ServerSDKManager.serverSDKManager.mSolutionManager.SyncSaveSolution(path, text, IntPtr.Zero, 0u);
			Instance.SolutionPassword = text;
			Instance.SolutionPath = path;
			return true;
		}
		catch (Exception ex)
		{
			Instance._isSaving = false;
			LogHelper.Debug($"InternalExport0_isSaving_{Instance._isSaving}");
			Instance.SolutionPath = string.Empty;
			throw ex;
		}
	}

	private string BaseSave()
	{
		return base.Save();
	}

	private string BaseSaveAs(string path, string passWord)
	{
		return base.SaveAs(path, passWord);
	}

	internal static IMVSGroup internalImportIndependentGroup(string path, string password = null)
	{
		try
		{
			TempVmSolution.Init();
			ImvsSdkDefine.IMVS_MODULE_INFO proInfo = ServerSDKManager.serverSDKManager.mCollectionManager.ImportIndependentGroup(100000u, path, password, 0u);
			string str = "IMVSGroup";
			proInfo.strModuleName = Hik.Util.StringUtil.UTF8GetFixLenBytes(str, 64);
			IMVSGroup iMVSGroup = VmModuleSet.CreatModuleByInfo(proInfo) as IMVSGroup;
			iMVSGroup.Parent = VirtualPro;
			iMVSGroup.ModuleFilePath = path;
			iMVSGroup.ModuleFilePassword = password;
			VirtualPro.Modules.Add(iMVSGroup);
			return iMVSGroup;
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	internal static VmProcedure InternalImportProcedure(string strPath, string strPassword, bool isForceIgnoreGlobal = true, uint nIdAllocateWay = 0u, object calibData = null)
	{
		TempVmSolution.Init();
		string strPath2 = strPath;
		return ImportProFile(ref strPath2, strPassword, isForceIgnoreGlobal, nIdAllocateWay, isFromCopy: false, isFromCalibration: true, calibData) as VmProcedure;
	}

	private static void Delete_Process(UnRedo_ProcessData data, IUnRedoParams unRedoParams)
	{
		object obj = Instance[data.FullName];
		if (data.Path == null)
		{
			data.Path = (string)(obj as VmProcedure).Export();
		}
		if (obj != null)
		{
			(obj as VmProcedure).Dispose();
		}
	}

	private static void Import_Process(UnRedo_ProcessData data, IUnRedoParams unRedoParams)
	{
		if (data.Path != null)
		{
			ImportProFile(ref data.Path, null, isForceIgnoreGlobal: true, 1u);
		}
		else
		{
			((IVmSolution)instance).CreateProcedure();
		}
	}

	internal static object Export_Procedure(VmProcedure vmProcedure)
	{
		vmProcedure.UserData["CalibData"] = null;
		string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		if (!Directory.Exists(baseDirectory + "\\TempFiles"))
		{
			Directory.CreateDirectory(baseDirectory + "\\TempFiles");
		}
		string text = baseDirectory + $"TempFiles\\0.{AppFileExtension.Process}";
		string text2 = $".{AppFileExtension.Process}";
		string directoryName = Path.GetDirectoryName(text);
		string text3 = text;
		string text4 = null;
		if (File.Exists(text3))
		{
			for (uint num = 1u; num <= 800; num++)
			{
				text3 = directoryName + "\\" + num + text2;
				if (!File.Exists(text3))
				{
					text4 = text3;
					break;
				}
			}
		}
		else
		{
			text4 = text3;
		}
		string text5 = text4;
		LogHelper.Info("Export proPath: " + text5);
		try
		{
			mbExportProcedureEndFlag = false;
			ServerSDKManager.serverSDKManager.mCollectionManager.ExportProcess(vmProcedure.ID, text5, "");
			SyncWaitor.Wait(ref mbExportProcedureEndFlag);
		}
		catch (VmException ex)
		{
			if (ex.errorCode != 0 && ex.errorCode == -536870093)
			{
				string text6 = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
				text6 = text6 + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
				HikMessageBox.Show(text6, MessageIcons.Error);
			}
			text5 = null;
		}
		return text5;
	}

	internal static IMVSGroup internalImportGroup(IVmModuleSet moduleSet, string strPath = null, string strPassword = null, uint nIdAllocateWay = 0u, bool isFromCopy = false)
	{
		TempVmSolution.Init();
		if (!mbImportGroupEndFlag)
		{
			return null;
		}
		VmModule obj = moduleSet as VmModule;
		if (obj == null || obj.disposedValue)
		{
			return null;
		}
		string text = null;
		bool flag = true;
		if (strPath == null)
		{
			string initPath = ConfigurationHelper.INIGetStringValue("OpenGroup");
			text = ServerSDKManager.serverSDKManager.mCollectionManager.OpenImportDialog("Group文件", $"|*.{AppFileExtension.Group}", initPath);
			if (File.Exists(text))
			{
				ConfigurationHelper.INISetStringValue("OpenGroup", Path.GetDirectoryName(text));
			}
		}
		else
		{
			text = strPath;
			flag = false;
		}
		if (!string.IsNullOrEmpty(text))
		{
			ImvsSdkDefine.IMVS_FILE_VERSION_INFO pstVersionInfo = default;
			if (ServerSDKAPI.IMVS_GetGroVersion(CVmObjectBase.mhandle, text, "", ref pstVersionInfo) == 0)
			{
				string lang = AppLocalizationService.GetLang("Lang_version");
				if (string.Compare(pstVersionInfo.strCurrentVesionInfo, "V4.0.0") < 0)
				{
					HikMessageBox.Show(string.Format("{0}\r\n{1}{2}", AppLocalizationService.GetLang("Lang_SolVersion_OutOfDate"), AppLocalizationService.GetLang("版本信息"), pstVersionInfo.strCurrentVesionInfo), MessageIcons.Error);
					return null;
				}
				if (string.Compare(pstVersionInfo.strCurrentVesionInfo, lang) < 0)
				{
					if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "CompatibleTool.exe") || !File.Exists(AppDomain.CurrentDomain.BaseDirectory + "CompatibleTool.exe.config"))
					{
						HikMessageBox.Show("CompatibleTool.exe" + AppLocalizationService.GetLang("Lang_FileNotEixt"), MessageIcons.Ok, MessageButtons.NoButton, 3000u);
						return null;
					}
					mbUpgradeGroupEndFlag = false;
					OnGroupUpgradeBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
					{
						Param = string.Format("{0}", AppLocalizationService.GetLang("group_upgrade"))
					});
					string text2 = $"{Path.GetDirectoryName(text)}\\{Path.GetFileNameWithoutExtension(text)}_";
					int? num = null;
					string text3;
					do
					{
						text3 = string.Format("{0}{1}{2}{3}", text2, lang, (!num.HasValue) ? "" : $"_{num}", Path.GetExtension(text));
						num = (num ?? 0) + 1;
					}
					while (File.Exists(text3));
					Process[] processesByName = Process.GetProcessesByName("CompatibleTool");
					if (processesByName.Length != 0 && processesByName[0].MainModule.FileName != AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CompatibleTool.exe")
					{
						processesByName[0].Kill();
					}
					string text4 = text3;
					Process process = new Process();
					process.StartInfo.FileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CompatibleTool.exe";
					process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
					process.StartInfo.Arguments = $"VMGroup;\"{text}\";;\"{text3}\";{lang}";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardError = true;
					process.Start();
					process.WaitForExit();
					string strMsg = process.StandardOutput.ReadToEnd();
					OnGroupUpgradeEndCallBack?.Invoke(SolObj, null);
					if (!File.Exists(text4))
					{
						HikMessageBox.Show(string.Format("{0}", AppLocalizationService.GetLang("group_upgrade_fail")), strMsg, MessageIcons.Error);
						return null;
					}
					text = text4;
					HikMessageBox.Show(string.Format("{0}\r\n{1}", AppLocalizationService.GetLang("group_upgraded"), Path.GetFileName(text)), MessageIcons.Ok);
				}
			}
		}
		if (text != null)
		{
			if (!isFromCopy)
			{
				Ioc.Get<IUnRedoService>()?.Register(new RootOperation(), string.Format("{0}: {1}", AppLocalizationService.GetLang("Lang_ImportGroup"), text));
			}
			VmModuleSet vmModuleSet = moduleSet as VmModuleSet;
			try
			{
				LogHelper.Info("Import group path: " + text);
				instance.importGroupModuleSet = moduleSet;
				if (moduleSet is IMVSGroup)
				{
					ServerSDKManager.serverSDKManager.mCollectionManager.ImportGroup(vmModuleSet.Root.ID, vmModuleSet.ID, text, strPassword ?? "", nIdAllocateWay);
				}
				else
				{
					ServerSDKManager.serverSDKManager.mCollectionManager.ImportGroup(vmModuleSet.ID, 0u, text, strPassword ?? "", nIdAllocateWay);
				}
				mbImportGroupEndFlag = false;
				SyncWaitor.Wait(ref mbImportGroupEndFlag);
				instance.importGroupModuleSet = null;
				if (instance.importedGroupModule != null)
				{
					IMVSGroup iMVSGroup = instance.importedGroupModule as IMVSGroup;
					if (iMVSGroup != null)
					{
						iMVSGroup.Parent = moduleSet;
						IUnRedoService unRedoService = Ioc.Get<IUnRedoService>();
						if (unRedoService != null && unRedoService.IsNeedExport())
						{
							object obj2 = new VmModuleDataInfo
							{
								strModuleFullName = iMVSGroup.FullName
							};
							string displayName = string.Format("{0}{1}:{2}", AppLocalizationService.GetLang("增加"), AppLocalizationService.GetLang("Lang_Modu"), iMVSGroup.FullName.Substring(iMVSGroup.FullName.LastIndexOf('.') + 1));
							Ioc.Get<IUnRedoService>()?.Register(new ModuleUnRedoOperation(iMVSGroup.Import_Module, iMVSGroup.Delete_Module, obj2 as VmModuleDataInfo, iMVSGroup.Update_Module), displayName);
						}
						LogHelper.Info("Add module:" + iMVSGroup.Name);
					}
					instance.importedGroupModule = null;
					iMVSGroup.ModuleFilePath = text;
					iMVSGroup.ModuleFilePassword = strPassword;
					return iMVSGroup;
				}
			}
			catch (VmException ex)
			{
				string text5 = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
				text5 = text5 + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
				HikMessageBox.Show(AppLocalizationService.GetLang("Lang_ImportGroupFail"), text5, MessageIcons.Error);
				LogHelper.Error("Import group error:" + text);
			}
		}
		else
		{
			LogHelper.Error("Import group path is empty.");
		}
		return null;
	}

	/// <summary>
	/// 导入Group时特殊处理
	/// </summary>
	private void ModifyImportGroup(IMVSGroup Group)
	{
		foreach (VmModule module in Group.Modules)
		{
			if (module is IMVSGroup iMVSGroup)
			{
				ModifyImportGroup(iMVSGroup);
			}
			else if (module.IsImportProcess)
			{
				module.ImportProcess();
			}
		}
	}

	/// <summary>
	/// 方案另存为
	/// </summary>
	private static void ExportAs(string path, string password = null, bool bDisplayProgress = true)
	{
		try
		{
			TempVmSolution.Init();
			if (!Instance._isSaving)
			{
				Instance._isSaving = true;
				LogHelper.Debug($"ExportAs_isSaving_{Instance._isSaving}");
				Instance.isDisplayProgress = bDisplayProgress;
				string text = password;
				if (text == null)
				{
					text = "";
				}
				ServerSDKManager.serverSDKManager.mSolutionManager.SyncSaveSolution(path, text, IntPtr.Zero, 0u);
			}
		}
		catch (VmException ex)
		{
			Instance._isSaving = false;
			LogHelper.Debug($"ExportAs1_isSaving_{Instance._isSaving}");
			string text2 = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
			text2 = text2 + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
			if (bDisplayProgress)
			{
				HikMessageBox.Show(AppLocalizationService.GetLang("Lang_SolSaveFail"), text2, MessageIcons.Error);
			}
			LogHelper.Error("ExportAs error:" + text2);
		}
		catch (Exception ex2)
		{
			Instance._isSaving = false;
			LogHelper.Debug($"ExportAs2_isSaving_{Instance._isSaving}");
			if (bDisplayProgress)
			{
				HikMessageBox.Show(AppLocalizationService.GetLang("Lang_SolSaveFail"), ex2.Message, MessageIcons.Error);
			}
			LogHelper.Error("ExportAs error:" + ex2.Message);
		}
	}

	private void CreatNewProcedure()
	{
		new VmProcedure(this);
	}

	internal override void UpdateContinusRunStatus(bool isRun)
	{
		gloalScriptContinusRunEnable = isRun;
		AllModules.Values.OfType<VmProcedure>().ToList().ForEach((VmProcedure x) =>
		{
			x.UpdateGlobalRunStatus(isRun);
		});
		OnPropertyChanged("IsRunning");
		OnPropertyChanged("ContinuousRunEnable");
		OnPropertyChanged("GloalScriptContinusRunEnable");
	}

	/// <summary>
	/// 加载流程
	/// </summary>
	private VmProcedure ChangToProcess(ImvsSdkDefine.IMVS_PROCESS_INFO data, object calibData = null)
	{
		VmModule vmModule = VmModuleSet.CreatModuleByInfo(data);
		if (calibData != null)
		{
			if (calibData is IParent)
			{
				(calibData as IParent).Parent = vmModule;
			}
			(vmModule as IVmProcedure).NeedGetAllModulesCallBack = true;
			(vmModule as IVmProcedure).CalibrationData = calibData;
		}
		(vmModule as IVmProcedure).IsBootMode = calibData != null;
		Modules.Add(vmModule);
		return vmModule as VmProcedure;
	}

	/// <summary>
	/// 运行前检查
	/// </summary>
	private bool CheckBeforeRun(ref string message)
	{
		IEnumerable<VmProcedure> source = instance.Modules.OfType<VmProcedure>();
		IVmProcedure vmProcedure;
		if ((vmProcedure = source.FirstOrDefault((VmProcedure x) => ((IChildren)x).Children.Count == 0)) != null)
		{
			message = string.Format("{0}:{1}", vmProcedure.Name, AppLocalizationService.GetLang("等待添加组件"));
			return false;
		}
		string vmModuleName = "";
		IVmProcedure vmProcedure2;
		if ((vmProcedure2 = source.FirstOrDefault((VmProcedure x) =>
		{
			IVmModule vmModule;
			if ((vmModule = (from p in ((IChildren)x).Children.OfType<IVmModule>()
				where !p.IsForbidden
				select p).FirstOrDefault((IVmModule p) => (!p.IsReady) ?? false)) != null)
			{
				vmModuleName = vmModule.ID + " " + vmModule.Name;
				return true;
			}
			return false;
		})) != null)
		{
			message = string.Format("{0}:{1} {2}", vmProcedure2.Name, vmModuleName, AppLocalizationService.GetLang("Param not Complete"));
			return false;
		}
		return true;
	}

	private static string UnRedoMaskMessage()
	{
		return string.IsNullOrEmpty(Ioc.Get<IUnRedoService>()?.GetExportMessage()) ? "流程导出中" : Ioc.Get<IUnRedoService>()?.GetExportMessage();
	}

	/// <summary>
	/// 加载流程(内部使用)
	/// </summary>
	protected override void LoadModules()
	{
		VmModuleCollection<VmModule> vmModuleCollection = new VmModuleCollection<VmModule>();
		try
		{
			LoadProcedures(vmModuleCollection);
			LoadGlobleModules(vmModuleCollection);
		}
		catch
		{
		}
		finally
		{
			Modules = vmModuleCollection;
		}
	}

	/// <summary>
	/// 加载流程
	/// </summary>
	private void LoadProcedures(VmModuleCollection<VmModule> moduleList)
	{
		try
		{
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST? allProcessList = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllProcessList();
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST? iMVS_PROCESS_INFO_LIST = allProcessList;
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST valueOrDefault = iMVS_PROCESS_INFO_LIST.GetValueOrDefault();
			if (!iMVS_PROCESS_INFO_LIST.HasValue)
			{
				return;
			}
			for (int i = 0; i < valueOrDefault.nNum; i++)
			{
				ImvsSdkDefine.IMVS_PROCESS_INFO proInfo = valueOrDefault.astProcessInfo[i];
				VmModule vmModule = VmModuleSet.CreatModuleByInfo(proInfo);
				vmModule.PreAddModuleTo(this);
				moduleList.Add(vmModule);
				if (!_updateContinueStatus.TryGetValue(vmModule.ID, out var value))
				{
					continue;
				}
				vmModule.UpdateContinusRunStatus(value);
				lock (waitNotifyLock)
				{
					if (_waitNotifyPages.ContainsKey(vmModule.ID))
					{
						_waitNotifyPages[vmModule.ID] = new WaitNotifyPage
						{
							OperID = 1,
							FunID = vmModule.ID,
							IsRunning = value
						};
					}
					else
					{
						_waitNotifyPages.Add(vmModule.ID, new WaitNotifyPage
						{
							OperID = 1,
							FunID = vmModule.ID,
							IsRunning = value
						});
					}
				}
			}
		}
		catch (VmException ex)
		{
			if (ex.errorCode != -536869120)
			{
				throw ex;
			}
		}
	}

	/// <summary>
	/// 加载全局模块(内部使用)
	/// </summary>
	internal void LoadGlobleModules(VmModuleCollection<VmModule> moduleList, bool coverGlobalmodu = false)
	{
		try
		{
			ImvsSdkDefine.IMVS_MODULE_INFO_LIST? allGlobalModule = ServerSDKManager.serverSDKManager.mGlobalModuleManager.GetAllGlobalModule();
			ImvsSdkDefine.IMVS_MODULE_INFO_LIST? iMVS_MODULE_INFO_LIST = allGlobalModule;
			ImvsSdkDefine.IMVS_MODULE_INFO_LIST valueOrDefault = iMVS_MODULE_INFO_LIST.GetValueOrDefault();
			if (!iMVS_MODULE_INFO_LIST.HasValue)
			{
				return;
			}
			for (int i = 0; i < valueOrDefault.nNum; i++)
			{
				ImvsSdkDefine.IMVS_MODULE_INFO proInfo = valueOrDefault.astModuleInfo[i];
				if (moduleList.Count((VmModule p) => p.ID == proInfo.nNodeID) > 0)
				{
					if (coverGlobalmodu)
					{
						moduleList.FirstOrDefault((VmModule p) => p.ID == proInfo.nNodeID)?.ReloadModule();
					}
				}
				else
				{
					VmModule vmModule = VmModuleSet.CreatModuleByInfo(proInfo);
					vmModule.PreAddModuleTo(this);
					moduleList.Add(vmModule);
				}
			}
		}
		catch (VmException ex)
		{
			if (ex.errorCode != -536869120)
			{
				throw ex;
			}
		}
	}

	private void DisplayProgress(string message)
	{
		OnSolutionLoadBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = message
		});
	}

	private void CloseProgress()
	{
		EndEventArgs e = new EndEventArgs();
		e.ErrorCode = 0;
		OnSolutionLoadEndCallBack?.Invoke(SolObj, e);
	}

	protected override List<LinkModuleInfo> GetLinkStack(string type, bool isCurrentLevel)
	{
		if (isCurrentLevel)
		{
			return null;
		}
		List<LinkModuleInfo> linkInfos = new List<LinkModuleInfo>();
		ImvsSdkDefine.IMVS_PROCESS_INFO_LIST? allProcessList = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllProcessList();
		if (allProcessList.HasValue && allProcessList.HasValue)
		{
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST? iMVS_PROCESS_INFO_LIST = allProcessList;
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST proList = iMVS_PROCESS_INFO_LIST.GetValueOrDefault();
			if (iMVS_PROCESS_INFO_LIST.HasValue)
			{
				int i;
				for (i = 0; i < proList.nNum; i++)
				{
					List<LinkModuleInfo> linkStackInner = GetLinkStackInner(type, proList.astProcessInfo[i].nProcID, 1u, isCurrentLevel: false);
					linkStackInner.ForEach((LinkModuleInfo x) =>
					{
						x.RootID = proList.astProcessInfo[i].nProcID;
						linkInfos.Add(x);
					});
				}
			}
		}
		return linkInfos;
	}

	protected override VmModule CreateLinkModuleTrans(LinkModuleInfo moduleInfo, int index)
	{
		VmModuleSet vmModuleSet = Instance.Modules.FirstOrDefault((VmModule x) => x.ID == moduleInfo.RootID) as VmModuleSet;
		if (vmModuleSet == null && VirtualPro.ID == moduleInfo.RootID)
		{
			vmModuleSet = VirtualPro;
			return (vmModuleSet.Modules.FirstOrDefault((VmModule x) => x.ID == moduleInfo.ParentID) is VmModuleSet vmModuleSet2) ? vmModuleSet2.CreateLinkModule(moduleInfo, index) : null;
		}
		return vmModuleSet?.CreateLinkModule(moduleInfo, index);
	}

	/// <summary>
	/// CH: 获取加密狗信息 目前仅用于校验加密狗 | EN: Get information of dongle. Currently only used to check dongle.
	/// </summary>
	public static void GetDongleInfo()
	{
		int num = ServerSDKAPI.IMVS_GetDongleInfo();
		if (num != 0)
		{
			throw new VmException(num);
		}
	}

	/// <summary>
	/// CH: 导入方案,功能与Import一致 | EN: Load solution
	/// </summary>
	/// <param name="path">方案路径</param>
	/// <param name="password">方案密码</param>
	/// <param name="isSilentExecute">是否静默执行</param>
	public static void Load(string path, string password = null, bool isSilentExecute = false)
	{
		try
		{
			TempVmSolution.Init();
			CheckSol(path, password);
			Import(path, password);
			if (CVmObjectBase.GetIsSDKMode())
			{
				if (isSilentExecute)
				{
					ServerSDKManager.serverSDKManager.mSolutionManager.SilentExecute(1);
				}
				else
				{
					ServerSDKManager.serverSDKManager.mSolutionManager.SetServerReadyEvent();
				}
			}
		}
		catch (Exception ex)
		{
			throw VmExceptionTool.GetVmException(ex);
		}
	}

	/// <summary>
	/// CH: 保存方案 | EN: Save Solution
	/// </summary>
	/// <returns>CH: 文件路径 | EN:File Path</returns>
	public new static string Save()
	{
		return instance.BaseSave();
	}

	/// <summary>
	///  CH: 另存方案 | EN: Save as Solution
	/// </summary>
	/// <returns></returns>
	public new static string SaveAs(string path, string passWord = null)
	{
		return instance.BaseSaveAs(path, passWord);
	}

	/// <summary>
	/// CH:异步执行一次 | EN:Async Run once
	/// </summary>
	public override void Run()
	{
		IsGlobalRuning = true;
		LogHelper.Info("全局点击Run设置IsGlobalRuning为true");
		try
		{
			ServerSDKManager.serverSDKManager.mGlobalModuleManager.GlobalScriptStartRunOnce();
		}
		catch (Exception ex)
		{
			IsGlobalRuning = false;
			throw ex;
		}
		IsGlobalRuning = false;
		LogHelper.Info("全局点击Run设置IsGlobalRuning为false");
	}

	/// <summary>
	/// CH:开启全部模块结果回调 | EN: Enable all module result callback
	/// </summary>
	public void EnableModulesCallback()
	{
		TempVmSolution.Init();
		ServerSDKManager.serverSDKManager.mSolutionManager.SetReportModuleResult_V2(ImvsSdkDefine.IMVS_REPORT_RESULT_TYPE.REPORT_RESULT_TYPE_ALL);
	}

	/// <summary>
	/// CH:关闭全部模块结果回调 | EN: Unenbale all module result callback
	/// </summary>
	public void DisableModulesCallback()
	{
		TempVmSolution.Init();
		ServerSDKManager.serverSDKManager.mSolutionManager.SetReportModuleResult_V2(ImvsSdkDefine.IMVS_REPORT_RESULT_TYPE.REPORT_RESULT_TYPE_NONE);
		Instance.AllModules.Values?.ToList().ForEach((VmModule x) =>
		{
			x.isSetCallBack = 0;
		});
	}

	/// <summary>
	///  CH: 加载*.prc文件 | EN:Load *.prc File (内部使用)
	/// </summary>
	/// <param name="strPath">CH: 文件路径 | EN:File Path</param>
	/// <param name="strPassword">CH: 密码 | EN:Password</param>
	/// <returns></returns>
	[Obsolete("CH: 弃用,推荐(VmProcedure.Load(string strPath, string strPassword = \"\")) | EN: Obsolete,recommend(VmProcedure.Load(string strPath, string strPassword = \"\"))", false)]
	public static VmProcedure LoadProcedure(string strPath, string strPassword = null, object calibData = null)
	{
		return InternalImportProcedure(strPath, strPassword, isForceIgnoreGlobal: true, 0u, calibData);
	}

	IVmProcedure IVmSolution.LoadProcedure(string strPath, string strPassword, object calibData)
	{
		return InternalImportProcedure(strPath, strPassword, isForceIgnoreGlobal: true, 0u, calibData);
	}

	/// <summary>
	///  CH: 加载*.gro文件(独立Group) | EN:Load *.gro File(Independent Group)
	/// </summary>
	/// <param name="strPath">CH: 文件路径 | EN:File Path</param>
	/// <param name="strPassword">CH: 密码 | EN:Password</param>
	/// <returns></returns>  
	public static IMVSGroup LoadIndependentGroup(string strPath, string strPassword = null)
	{
		return internalImportIndependentGroup(strPath, strPassword);
	}

	/// <summary>
	///  CH: 加载*.gro文件 | EN:Load *.gro File
	/// </summary>
	/// <param name="moduleSet">CH: 父模块 | EN:Parent Module</param>
	/// <param name="strPath">CH: 文件路径 | EN:File Path</param>
	/// <param name="strPassword">CH: 密码 | EN:Password</param>
	/// <returns></returns>
	public static IMVSGroup LoadGroup(IVmModuleSet moduleSet, string strPath = null, string strPassword = null, bool isFromCopy = false)
	{
		return internalImportGroup(moduleSet, strPath, strPassword, 0u, isFromCopy);
	}

	/// <summary>
	///  CH: 导出*.sol文件 | EN:Save *.sol File
	/// </summary>
	/// <param name="strPath">CH: 文件路径 | EN:File Path</param>
	/// <param name="strPassword">CH: 密码 | EN:Password</param>
	/// <param name="initDir">CH: 文件夹路径 | EN:Folder Path</param>
	/// <returns>CH: 文件路径 | EN:File Path</returns>
	public override string ExportFile(string path = null, string passWord = null, bool isExportAs = false, string initDir = null)
	{
		InternalExport(path, passWord, bDisplayProgress: true, isExportAs, initDir);
		return Instance.ModuleFilePath;
	}

	/// <summary>
	/// 获取VM内部异常，用于规范抛出去的异常
	/// </summary>
	/// <returns></returns>
	public static VmException GetVmException(Exception ex)
	{
		VmException result = null;
		while (ex != null)
		{
			if (ex is VmException)
			{
				result = (VmException)ex;
				break;
			}
			ex = ex.InnerException;
		}
		return result;
	}

	/// <summary>
	/// 启动新建方案(内部使用)
	/// </summary>
	public static void StartNewInstance()
	{
		TempVmSolution.Init();
		Instance = new VmSolution();
		ServerSDKManager.serverSDKManager.mProcessManager.SyncAddOneProcess(AppLocalizationService.GetLang("Lang_Process"));
	}

	/// <summary>
	/// (内部使用)
	/// </summary>
	public static bool GetSolutionTakeState()
	{
		TempVmSolution.Init();
		ImvsSdkDefine.IMVS_STATUS_TAKEOVER_INFO iMVS_STATUS_TAKEOVER_INFO = default;
		int nRet = 0;
		iMVS_STATUS_TAKEOVER_INFO = ServerSDKManager.serverSDKManager.mVmInterfaceManager.GetTakeoverStatus(ref nRet, isThrow: false).Value;
		while (true)
		{
			switch (nRet)
			{
			case -536870899:
				break;
			default:
				return false;
			case 0:
				switch (iMVS_STATUS_TAKEOVER_INFO.nTakeoverStatus)
				{
				case 1u:
				{
					ImvsSdkDefine.IMVS_PLATFORM_DETAILED_INFO iMVS_PLATFORM_DETAILED_INFO = default;
					iMVS_PLATFORM_DETAILED_INFO = ServerSDKManager.serverSDKManager.mVmInterfaceManager.GetPlatformDetailedInfo().Value;
					string solutionPath = Hik.Util.StringUtil.UTF8GetString(iMVS_PLATFORM_DETAILED_INFO.strSolutionPath);
					string solutionPassword = Hik.Util.StringUtil.UTF8GetString(iMVS_PLATFORM_DETAILED_INFO.strPassWord);
					instance.SolutionPath = solutionPath;
					instance.SolutionPassword = solutionPassword;
					break;
				}
				}
				isTakeOver = iMVS_STATUS_TAKEOVER_INFO.nTakeoverStatus == 1;
				return isTakeOver;
			}
			iMVS_STATUS_TAKEOVER_INFO = ServerSDKManager.serverSDKManager.mVmInterfaceManager.GetTakeoverStatus(ref nRet, isThrow: false).Value;
			Thread.Sleep(200);
		}
	}

	/// <summary>
	/// 创建空方案(内部使用)
	/// </summary>
	public static void CreatSolInstance()
	{
		try
		{
			TempVmSolution.Init();
			ServerSDKManager.serverSDKManager.mSolutionManager.CloseSolution();
			Instance = new VmSolution();
			ServerSDKManager.serverSDKManager.mProcessManager.SyncAddOneProcess(AppLocalizationService.GetLang("Lang_Process"));
		}
		catch (VmException ex)
		{
			string text = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
			text = text + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
			LogHelper.Error("CreatSolInstance error: " + text);
		}
		catch (Exception ex2)
		{
			LogHelper.Error("CreatSolInstance error: \r\n" + ex2.StackTrace);
		}
	}

	/// <summary>
	/// 创建空方案For独立Group(内部使用)
	/// </summary>
	public static void CreatSolInstanceForIndependentGroup()
	{
		try
		{
			TempVmSolution.Init();
			Instance = new VmSolution();
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// 释放所有资源，仅在退出程序前执行一次，避免在析构函数中调用
	/// </summary>
	public override void Dispose()
	{
		if (CVmObjectBase.GetIsSDKMode())
		{
			LogHelper.Info("DestoryThirdHandle.");
			bool isDisposeEnd = false;
			Task.Run(() =>
			{
				ServerSDKManager.serverSDKManager.mVmInterfaceManager.DestoryThirdHandle();
				isDisposeEnd = true;
			});
			SyncWaitor.Wait(ref isDisposeEnd);
		}
		else
		{
			base.Dispose();
			Dispose(isTakeOver: false);
			IsDispose = true;
			isTakeOver = false;
		}
		customModule = null;
	}

	/// <summary>
	/// Dispose(内部使用)
	/// </summary>
	/// <param name="isTakeOver"></param>
	public void Dispose(bool isTakeOver)
	{
		LogHelper.Info("DestoryHandle.");
		ServerSDKManager.serverSDKManager.mVmInterfaceManager.DestoryHandle(isTakeOver ? 1 : 0);
	}

	/// <summary>
	/// CH:设置默认打开方案 | EN: Set Default Open Solution
	/// <param name="solInfo">CH: 方案信息 | EN:Solution Info</param>
	/// </summary>
	public void SetAutoLoadSolution(ImvsSdkDefine.IMVS_AUTO_LOAD_SOLUTION_INFO solInfo)
	{
		solInfo.strPassWord = Hik.Util.StringUtil.UTF8GetFixLenBytes(SolutionPassword, 32);
		solInfo.strSolutionPath = Hik.Util.StringUtil.UTF8GetFixLenBytes(SolutionPath, 780);
		ServerSDKManager.serverSDKManager.mVmInterfaceManager.SetAutoLoadSolution(solInfo);
	}

	/// <summary>
	/// CH:方案是否含有密码 | EN: Soution has password
	/// </summary>
	/// <param name="path">CH:方案路径 | EN:solution path</param>
	/// <returns> 有密码，返回true;无密码,返回false</returns>
	public bool HasPassword(string path)
	{
		try
		{
			uint nHasPwd = 0u;
			uint nValidFormat = 0u;
			ServerSDKManager.serverSDKManager.mSolutionManager.CheckSolution(path, ref nHasPwd, ref nValidFormat);
			if (1 == nHasPwd)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// CH:获取当前方案版本 |EN:Get solution version
	/// </summary>
	/// <returns> 方案版本号</returns>
	public string GetSolutionVersion(string strPath, string strPassWord)
	{
		try
		{
			TempVmSolution.Init();
			string text = ServerSDKManager.serverSDKManager.mSolutionManager.GetSolutionVersion(strPath, strPassWord);
			if ("V4.2.1" == text)
			{
				text = "V4.2.0";
			}
			return text;
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// CH:关闭方案 |EN:Close solution
	/// </summary>
	public void CloseSolution()
	{
		TempVmSolution.Init();
		ServerSDKManager.serverSDKManager.mSolutionManager.CloseSolution();
	}

	/// <summary>
	/// CH:删除流程 |EN:Delete procedure
	/// </summary>
	/// <param name="strProcedureName">CH:流程名 |EN:procedure name</param>
	public void DeleteOneProcedure(string strProcedureName)
	{
		try
		{
			TempVmSolution.Init();
			if (strProcedureName == null || "" == strProcedureName)
			{
				VmException ex = new VmException(-536870911);
				throw ex;
			}
			VmProcedure vmProcedure = (VmProcedure)Instance[strProcedureName];
			if (vmProcedure == null)
			{
				VmException ex2 = new VmException(-536870122);
				throw ex2;
			}
			vmProcedure.Dispose();
		}
		catch (Exception ex3)
		{
			throw GetVmException(ex3);
		}
	}

	/// <summary>
	/// CH:判断方案是否修改|EN:Solution is modify
	/// </summary>
	/// <returns> 方案修改，返回true;方案未修改,返回false</returns>
	public bool IsSolModifyInfo()
	{
		return ServerSDKManager.serverSDKManager.mSolutionManager.IsSolModifyInfo();
	}

	/// <summary>
	/// CH:禁用一个流程 |EN:Disable procedure
	/// </summary>
	/// <param name="strProcessName">CH:流程名称|EN:Procedure name</param>
	public void DisableProcedure(string strProcessName)
	{
		TempVmSolution.Init();
		if (strProcessName == null || "" == strProcessName)
		{
			VmException ex = new VmException(-536870911);
			throw ex;
		}
		VmProcedure vmProcedure = (VmProcedure)Instance[strProcessName];
		if (vmProcedure == null)
		{
			VmException ex2 = new VmException(-536870122);
			throw ex2;
		}
		vmProcedure.IsForbidden = true;
	}

	/// <summary>
	/// CH:启用一个流程 |EN:Enable procedure
	/// </summary>
	/// <param name="strProcessName">CH:流程名称|EN:Procedure name</param>
	public void EnableProcedure(string strProcessName)
	{
		try
		{
			TempVmSolution.Init();
			if (strProcessName == null || "" == strProcessName)
			{
				VmException ex = new VmException(-536870911);
				throw ex;
			}
			VmProcedure vmProcedure = (VmProcedure)Instance[strProcessName];
			if (vmProcedure == null)
			{
				VmException ex2 = new VmException(-536870122);
				throw ex2;
			}
			vmProcedure.IsForbidden = false;
		}
		catch (Exception ex3)
		{
			throw GetVmException(ex3);
		}
	}

	/// <summary>
	/// CH:流程是否禁用|EN:Procedure is disabled
	/// </summary>
	/// <param name="strProcessName">流程名称</param>
	/// <returns> 流程禁用，返回true;流程启用,返回false </returns>
	public bool IsProcedureDisable(string strProcessName)
	{
		try
		{
			TempVmSolution.Init();
			if (strProcessName == null || "" == strProcessName)
			{
				VmException ex = new VmException(-536870911);
				throw ex;
			}
			VmProcedure vmProcedure = (VmProcedure)Instance[strProcessName];
			if (vmProcedure == null)
			{
				VmException ex2 = new VmException(-536870122);
				throw ex2;
			}
			return !vmProcedure.IsForbidden;
		}
		catch (Exception ex3)
		{
			throw GetVmException(ex3);
		}
	}

	/// <summary>
	/// CH:获取当前方案中所有流程列表|EN:Get procedure list from solution
	/// </summary>
	/// <returns> 流程列表</returns>
	public ProcessInfoList GetAllProcedureList()
	{
		try
		{
			TempVmSolution.Init();
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST value = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllProcessList().Value;
			ProcessInfoList result = new ProcessInfoList
			{
				nNum = value.nNum,
				astProcessInfo = new ProcessInfo[1000]
			};
			for (int i = 0; i < value.nNum; i++)
			{
				result.astProcessInfo[i].nProcessID = value.astProcessInfo[i].nProcID;
				result.astProcessInfo[i].pProcessName = value.astProcessInfo[i].strProcName;
			}
			return result;
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// CH:获取当前方案中所有流程对象|EN:Get procedure objects from solution
	/// </summary>
	/// <returns> 流程对象</returns>
	public void GetAllProcedureObjects(ref List<VmProcedure> procedures)
	{
		if (procedures == null)
		{
			procedures = new List<VmProcedure>();
		}
		try
		{
			TempVmSolution.Init();
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST value = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllProcessList().Value;
			procedures.Clear();
			for (int i = 0; i < value.nNum; i++)
			{
				string sub = Encoding.UTF8.GetString(value.astProcessInfo[i].strProcName).TrimEnd(default(char));
				VmProcedure item = (VmProcedure)this[sub];
				procedures.Add(item);
			}
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// 获取模块全路径信息列表
	/// </summary>
	/// <returns></returns>
	public List<FullMoudleInfo> GetFullModuleList(string ID = null)
	{
		List<FullMoudleInfo> list = new List<FullMoudleInfo>();
		ImvsSdkDefine.IMVS_PROCESS_INFO_LIST? allProcessList = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllProcessList();
		if (allProcessList.HasValue && allProcessList.HasValue)
		{
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST? iMVS_PROCESS_INFO_LIST = allProcessList;
			ImvsSdkDefine.IMVS_PROCESS_INFO_LIST valueOrDefault = iMVS_PROCESS_INFO_LIST.GetValueOrDefault();
			if (iMVS_PROCESS_INFO_LIST.HasValue && valueOrDefault.nNum != 0)
			{
				if (!string.IsNullOrEmpty(ID))
				{
					ImvsSdkDefine.IMVS_PROCESS_INFO iMVS_PROCESS_INFO = valueOrDefault.astProcessInfo.FirstOrDefault((ImvsSdkDefine.IMVS_PROCESS_INFO x) => x.nProcID.ToString() == ID);
					GetInnerFullModuleList(list, iMVS_PROCESS_INFO.nProcID, 1u, iMVS_PROCESS_INFO.nProcID, Hik.Util.StringUtil.UTF8GetString(iMVS_PROCESS_INFO.strProcName));
					return list;
				}
				for (int num = 0; num < valueOrDefault.nNum; num++)
				{
					ImvsSdkDefine.IMVS_PROCESS_INFO iMVS_PROCESS_INFO2 = valueOrDefault.astProcessInfo[num];
					GetInnerFullModuleList(list, iMVS_PROCESS_INFO2.nProcID, 1u, iMVS_PROCESS_INFO2.nProcID, Hik.Util.StringUtil.UTF8GetString(iMVS_PROCESS_INFO2.strProcName));
				}
			}
		}
		return list;
	}

	private void GetInnerFullModuleList(List<FullMoudleInfo> resultMoudleInfo, uint nodeID, uint nObjectType, uint processID, string path = null)
	{
		ImvsSdkDefine.IMVS_NODE_TREE_INFO? firstLayerModuleTree = ServerSDKManager.serverSDKManager.mProcessManager.GetFirstLayerModuleTree(nodeID, nObjectType);
		if (!firstLayerModuleTree.HasValue || !firstLayerModuleTree.HasValue)
		{
			return;
		}
		ImvsSdkDefine.IMVS_NODE_TREE_INFO? iMVS_NODE_TREE_INFO = firstLayerModuleTree;
		ImvsSdkDefine.IMVS_NODE_TREE_INFO valueOrDefault = iMVS_NODE_TREE_INFO.GetValueOrDefault();
		if (!iMVS_NODE_TREE_INFO.HasValue)
		{
			return;
		}
		for (int i = 0; i < valueOrDefault.nChildNodeNum; i++)
		{
			ImvsSdkDefine.IMVS_MODULE_INFO iMVS_MODULE_INFO = valueOrDefault.astChildNodeInfo[i];
			FullMoudleInfo item = default;
			string text = Hik.Util.StringUtil.UTF8GetString(iMVS_MODULE_INFO.strModuleName);
			item.ID = iMVS_MODULE_INFO.nNodeID;
			item.Name = Hik.Util.StringUtil.UTF8GetString(iMVS_MODULE_INFO.strDisplayName);
			item.ParentID = nodeID;
			item.ProcessID = processID;
			item.FullName = path;
			if (text == "IMVSGroup")
			{
				GetInnerFullModuleList(resultMoudleInfo, item.ID, 20u, processID, path + "." + item.Name);
			}
			resultMoudleInfo.Add(item);
		}
	}

	public ModuleInfoList GetAllModuleList()
	{
		try
		{
			TempVmSolution.Init();
			ImvsSdkDefine.IMVS_MODULE_INFO_LIST value = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllModuleList().Value;
			ModuleInfoList result = new ModuleInfoList
			{
				nNum = value.nNum,
				astModuleInfo = new ModuInfo[2064]
			};
			for (int i = 0; i < value.nNum; i++)
			{
				result.astModuleInfo[i].nModuleID = value.astModuleInfo[i].nNodeID;
				result.astModuleInfo[i].nProcessID = value.astModuleInfo[i].nProcessID;
				result.astModuleInfo[i].pDisplayName = value.astModuleInfo[i].strDisplayName;
				result.astModuleInfo[i].pModuleName = value.astModuleInfo[i].strModuleName;
			}
			return result;
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// CH:设置模块结果缓存配置|EN:Set module result chcahe num
	/// </summary>
	/// <param name="bBufferEnable">CH:缓存使能|EN:Cache enable</param>
	/// <param name="nBufferNum">CH:缓存结果数量|EN:Cache num</param>
	public void SetModuleResultBuffer(bool bBufferEnable, uint nBufferNum)
	{
		try
		{
			TempVmSolution.Init();
			ServerSDKManager.serverSDKManager.mVmInterfaceManager.SetModuleResultBuffer(bBufferEnable, nBufferNum);
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// CH:获取模块结果缓存配置|EN:Get module result chcahe num
	/// </summary>
	/// <param name="bBufferEnable">CH:缓存使能|EN:Cache enable</param>
	/// <param name="nBufferNum">CH:缓存结果数量|EN:Cache num</param>
	public void GetModuleResultBuffer(ref bool bBufferEnable, ref uint nBufferNum)
	{
		try
		{
			TempVmSolution.Init();
			ServerSDKManager.serverSDKManager.mVmInterfaceManager.GetModuleResultBuffer(ref bBufferEnable, ref nBufferNum);
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// CH:设置连续运行时间间隔|EN:Set continue run interval
	/// </summary>
	/// <param name="nMillSecond">CH:时间间隔|EN:Time interval</param>
	public void SetRunInterval(uint nMillSecond)
	{
		TempVmSolution.Init();
		ServerSDKManager.serverSDKManager.mSolutionManager.SetcontinousRunInterval(nMillSecond);
	}

	public uint GetModuleCount()
	{
		uint result = 0u;
		try
		{
			TempVmSolution.Init();
			result = ServerSDKManager.serverSDKManager.mSolutionManager.GetAllModuleList().Value.nNum;
		}
		catch (VmException ex)
		{
			string text = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
			text = text + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
			LogHelper.Error("GetModuleCount error: " + text);
		}
		catch (Exception ex2)
		{
			LogHelper.Error("GetModuleCount error: " + ex2.Message);
			throw GetVmException(ex2);
		}
		return result;
	}

	/// <summary>
	/// CH:同步执行一次 | EN:Synchronize run once
	/// </summary>
	public void SyncRun()
	{
		IsGlobalRuning = true;
		try
		{
			TempVmSolution.Init();
			mbSolRunFlag = false;
			mnSolRunErrorde = 0;
			ServerSDKManager.serverSDKManager.mGlobalModuleManager.GlobalScriptStartRunOnce();
		}
		catch (Exception ex)
		{
			IsGlobalRuning = false;
			throw GetVmException(ex);
		}
		uint nShieldFlag = 0u;
		ServerSDKManager.serverSDKManager.mProcessManager.GetAllProcedureShieldState(ref nShieldFlag);
		if (1 == nShieldFlag)
		{
			VmException ex2 = new VmException(-536870115);
			throw ex2;
		}
		IsGlobalRuning = false;
		SyncWaitor.Wait(ref mbSolRunFlag);
		if (mnSolRunErrorde != 0)
		{
			throw new VmException(mnSolRunErrorde);
		}
	}

	void IVmSolution.InvokeGlobalCameraModuleAddedOrDeleted(IVmModule module, int type, object arsg)
	{
		ModuInfo cameraInfo = new ModuInfo
		{
			nModuleID = module.ID,
			nProcessID = 0u,
			pDisplayName = Hik.Util.StringUtil.UTF8GetBytesPadZero(module.Name),
			pModuleName = Hik.Util.StringUtil.UTF8GetBytesPadZero(module.ModuleTypeName)
		};
		switch (type)
		{
		case 1:
			OnGlobalCameraModuleAddedEvent?.Invoke(module, cameraInfo);
			break;
		case 2:
			OnGlobalCameraModuleDeletedEvent?.Invoke(module, cameraInfo);
			break;
		case 3:
			OnGlobalCameraModuleRenamedEvent?.Invoke(module, arsg as NameChangedEventArgs);
			break;
		}
	}

	/// <summary>
	/// 导入方案
	/// </summary>
	[Obsolete("CH: 弃用,推荐(Load(string path, string password = null)) | EN: Obsolete,recommend(Load(string path, string password = null))", false)]
	public static void Import(string path, string password)
	{
		TempVmSolution.Init();
		Import_path = path;
		Import_password = password;
		OnSolutionLoadEndCallBack += VmSolution_Import_OnEndCallBack;
		bool isReportSilent = !(Ioc.Get<IUnRedoService>()?.IsInUnRedo() ?? false);
		try
		{
			ServerSDKManager.serverSDKManager.mSolutionManager.SyncLoadSolution(path, password ?? "", isReportSilent);
		}
		catch (VmException ex)
		{
			if (ex.errorCode != 0 && ex.errorCode != -536870093)
			{
				throw ex;
			}
			if (CVmObjectBase.GetIsSDKMode())
			{
				throw ex;
			}
		}
		catch (Exception ex2)
		{
			throw ex2;
		}
		finally
		{
			OnSolutionLoadEndCallBack -= VmSolution_Import_OnEndCallBack;
		}
	}

	private static void VmSolution_Import_OnEndCallBack(object sender, EndEventArgs e)
	{
		if (e != null)
		{
			if (e.ErrorCode == 0 || e.ErrorCode == -536870093)
			{
				Instance.SolutionPath = Import_path;
				Instance.SolutionPassword = Import_password;
			}
			else
			{
				Instance.SolutionPath = null;
				Instance.SolutionPassword = null;
			}
		}
	}

	[Obsolete("CH: 弃用,推荐(Load(string path, string password = null))| EN: Obsolete,recommend(Load(string path, string password = null))", false)]
	public static void Import(string path)
	{
		TempVmSolution.Init();
		CheckSol(path, null);
		Import(path, null);
	}

	/// <summary>
	/// 保存方案
	/// </summary>
	[Obsolete("CH: 弃用,推荐(SaveAs(string path, string password = null)) | EN: Obsolete,recommend(SaveAs(string path, string password = null))", false)]
	public static bool Export(string path = null, string password = null, bool bDisplayProgress = true, bool isExportAs = false, string initDir = null)
	{
		try
		{
			return InternalExport(path, password, bDisplayProgress, isExportAs, initDir);
		}
		catch (VmException ex)
		{
			Instance._isSaving = false;
			LogHelper.Debug($"Export_isSaving_{Instance._isSaving}");
			Instance.SolutionPath = string.Empty;
			string text = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
			text = text + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
			if (bDisplayProgress)
			{
				HikMessageBox.Show(AppLocalizationService.GetLang("Lang_SolSaveFail"), text, MessageIcons.Error);
			}
			LogHelper.Error("Export error:" + text);
			return false;
		}
		catch (Exception ex2)
		{
			Instance._isSaving = false;
			LogHelper.Debug($"Export_isSaving_{Instance._isSaving}");
			Instance.SolutionPath = string.Empty;
			if (bDisplayProgress)
			{
				HikMessageBox.Show(AppLocalizationService.GetLang("Lang_SolSaveFail"), ex2.Message, MessageIcons.Error);
			}
			LogHelper.Error("Export error:" + ex2.Message);
			return false;
		}
	}

	/// <summary>
	/// 导入流程
	/// </summary>   
	[Obsolete("CH: 弃用,推荐(VmProcedure.Load(string strPath, string strPassword = \"\")) | EN: Obsolete,recommend(VmProcedure.Load(string strPath, string strPassword = \"\"))", false)]
	public static object ImportProFile(string strPath, string strPassword = null, bool isForceIgnoreGlobal = true, uint nIdAllocateWay = 0u)
	{
		return InternalImportProcedure(strPath, strPassword, isForceIgnoreGlobal, nIdAllocateWay);
	}

	/// <summary>
	/// CH: 应调用本类ImportProcedure方法 | EN:Should Use ImportProcedure Method Replace in this Class 
	/// </summary>
	/// <param name="strPath"></param>
	/// <param name="isForceIgnoreGlobal">默认忽略全局变量</param>
	/// <param name="nIdAllocateWay"></param>
	/// <param name="isFromCalib">是否来源于copy</param>
	/// <param name="isFromCalibration">是否来源于一键标定</param>
	/// <param name="calibData">一键标定引导数据</param>
	/// <returns></returns>
	[Obsolete("CH: 弃用,推荐(VmProcedure.Load(string strPath, string strPassword = \"\")) | EN: Obsolete,recommend(VmProcedure.Load(string strPath, string strPassword = \"\"))", false)]
	public static object ImportProFile(ref string strPath, string strPassword = null, bool isForceIgnoreGlobal = true, uint nIdAllocateWay = 0u, bool isFromCopy = false, bool isFromCalibration = false, object calibData = null)
	{
		TempVmSolution.Init();
		if (!mbImportProcedureEndFlag)
		{
			return null;
		}
		bool flag = IsIgnoreGlobalModule & isForceIgnoreGlobal;
		bool flag2 = (string.IsNullOrEmpty(strPath) ? true : false);
		bool flag3 = false;
		if (flag2)
		{
			string initPath = ConfigurationHelper.INIGetStringValue("OpenProcess");
			strPath = ServerSDKManager.serverSDKManager.mCollectionManager.OpenImportDialog("Lang_ProcessFile", $"|*.{AppFileExtension.Process}", initPath);
			if (File.Exists(strPath))
			{
				ConfigurationHelper.INISetStringValue("OpenProcess", Path.GetDirectoryName(strPath));
			}
		}
		if (strPath != null)
		{
			if (flag2)
			{
				Ioc.Get<IUnRedoService>()?.Register(new RootOperation(), string.Format("{0}", AppLocalizationService.GetLang("Lang_ImportProcess")));
			}
			try
			{
				LogHelper.Info("Import procedure path: " + strPath);
				if (!flag2 & isFromCopy)
				{
					SyncWaitor.Wait(ref mbExportProcedureEndFlag);
				}
				bool flag4 = false;
				if (!string.IsNullOrEmpty(strPath))
				{
					string lang = AppLocalizationService.GetLang("Lang_version");
					string prcVersion = ServerSDKManager.serverSDKManager.mProcessManager.GetPrcVersion(strPath, string.Empty);
					flag3 = true;
					if (string.Compare(prcVersion, "V4.0.0") < 0)
					{
						HikMessageBox.Show(string.Format("{0}\r\n{1}{2}", AppLocalizationService.GetLang("Lang_SolVersion_OutOfDate"), AppLocalizationService.GetLang("版本信息"), prcVersion), MessageIcons.Error);
						return null;
					}
					if (string.Compare(prcVersion, lang) < 0)
					{
						if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "CompatibleTool.exe") || !File.Exists(AppDomain.CurrentDomain.BaseDirectory + "CompatibleTool.exe.config"))
						{
							HikMessageBox.Show("CompatibleTool.exe" + AppLocalizationService.GetLang("Lang_FileNotEixt"), MessageIcons.Ok, MessageButtons.NoButton, 3000u);
							return null;
						}
						mbUpgradeProcedureEndFlag = false;
						OnProcedureUpgradeBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
						{
							Param = string.Format("{0}", AppLocalizationService.GetLang("procedure_upgrade"))
						});
						string text = $"{Path.GetDirectoryName(strPath)}\\{Path.GetFileNameWithoutExtension(strPath)}_";
						int? num = null;
						string text2;
						do
						{
							text2 = string.Format("{0}{1}{2}{3}", text, lang, (!num.HasValue) ? "" : $"_{num}", Path.GetExtension(strPath));
							num = (num ?? 0) + 1;
						}
						while (File.Exists(text2));
						Process[] processesByName = Process.GetProcessesByName("CompatibleTool");
						if (processesByName.Length != 0 && processesByName[0].MainModule.FileName != AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CompatibleTool.exe")
						{
							processesByName[0].Kill();
						}
						string text3 = text2;
						Process process = new Process();
						process.StartInfo.FileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "CompatibleTool.exe";
						process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
						process.StartInfo.Arguments = $"VMProcedure;\"{strPath}\";;\"{text2}\";{lang}";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						process.StartInfo.CreateNoWindow = true;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.RedirectStandardError = true;
						process.Start();
						process.WaitForExit();
						string strMsg = process.StandardOutput.ReadToEnd();
						OnProcedureUpgradeEndCallBack?.Invoke(SolObj, null);
						if (!File.Exists(text3))
						{
							flag4 = false;
							HikMessageBox.Show(string.Format("{0}", AppLocalizationService.GetLang("procedure_upgrade_fail")), strMsg, MessageIcons.Error);
							return null;
						}
						strPath = text3;
						flag4 = true;
					}
				}
				ServerSDKManager.serverSDKManager.mCollectionManager.ImportProcess(strPath, "", isFromCopy ? 2u : (flag ? 1u : 0u), nIdAllocateWay);
				mbImportProcedureEndFlag = false;
				if (calibData != null)
				{
					instance.importCalibData = calibData;
				}
				SyncWaitor.Wait(ref mbImportProcedureEndFlag);
				if (flag4)
				{
					HikMessageBox.Show(string.Format("{0}\r\n{1}", AppLocalizationService.GetLang("procedure_upgraded"), Path.GetFileName(strPath)), MessageIcons.Ok);
				}
				if (instance.importedProcedure != null)
				{
					VmProcedure vmProcedure = instance.importedProcedure;
					if (vmProcedure != null)
					{
						if (!isFromCalibration)
						{
							IUnRedoService unRedoService = Ioc.Get<IUnRedoService>();
							if (unRedoService != null && unRedoService.IsNeedExport())
							{
								try
								{
									string displayName = string.Format("{0}:{1}", AppLocalizationService.GetLang("增加"), vmProcedure.FullName);
									Ioc.Get<IUnRedoService>()?.Register(new ProcessUnRedoOperation(Import_Process, Delete_Process, new UnRedo_ProcessData
									{
										Path = null,
										FullName = vmProcedure.FullName,
										Point = null,
										Calib = null
									}), displayName);
									Ioc.Get<IUnRedoService>()?.ModifyDisplayInfo(strPath);
								}
								finally
								{
									Ioc.Get<IUnRedoService>()?.GetExportMessage("");
								}
							}
						}
						LogHelper.Info("Add module:" + vmProcedure.Name);
					}
					instance.importedProcedure = null;
					vmProcedure.ModuleFilePath = strPath;
					vmProcedure.ModuleFilePassword = strPassword;
					return vmProcedure;
				}
			}
			catch (VmException ex)
			{
				string text4 = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
				text4 = text4 + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
				if (flag2 || ex.errorCode == -536869094 || ex.errorCode == -536870396 || !flag3)
				{
					HikMessageBox.Show(AppLocalizationService.GetLang("Lang_ImportProcessFail"), text4, MessageIcons.Error);
				}
				LogHelper.Error("Import procedure error:" + strPath);
			}
			catch (Exception ex2)
			{
				LogHelper.Error("Import procedure error:" + ex2.Message + "\r\n" + ex2.StackTrace);
				return null;
			}
		}
		else
		{
			LogHelper.Error("Import procedure path is empty.");
			Ioc.Get<IUnRedoService>()?.RemoveEmptyOperation();
		}
		return null;
	}

	/// <summary>
	/// Group导入
	/// </summary>
	/// <param name="moduleSet"></param>
	/// <param name="strPath"></param>
	/// <param name="strPassword"></param>
	/// <param name="nIdAllocateWay"></param>
	/// <returns></returns>
	[Obsolete("CH: 弃用,推荐(LoadGroup(IVmModuleSet moduleSet, string strPath = null, string strPassword = null, bool isFromCopy = false)) | EN: Obsolete,recommend(ImportGroup(IVmModuleSet moduleSet, string strPath = null, string strPassword = null, bool isFromCopy = false))", false)]
	public static IVMGroup ImportFile(IVmModuleSet moduleSet, string strPath = null, string strPassword = null, uint nIdAllocateWay = 0u)
	{
		return internalImportGroup(moduleSet, strPath, strPassword, nIdAllocateWay);
	}

	/// <summary>
	/// 停止上报所有模块结果
	/// </summary>
	[Obsolete("CH: 弃用,推荐(DisableModulesCallback()) | EN: Obsolete,recommend(DisableModulesCallback())", false)]
	public void StopReportModuleResults()
	{
		TempVmSolution.Init();
		ServerSDKManager.serverSDKManager.mSolutionManager.SetNoneReportModuleResult();
	}

	/// <summary>
	/// 上报所有模块结果
	/// </summary>
	[Obsolete("CH: 弃用,推荐(EnableModulesCallback()) | EN: Obsolete,recommend(EnableModulesCallback())", false)]
	public void ReportAllModuleResults()
	{
		TempVmSolution.Init();
		ServerSDKManager.serverSDKManager.mSolutionManager.SetAllReportModuleResult();
	}

	/// <summary>
	/// 上报指定模块结果
	/// </summary>
	[Obsolete("CH: 弃用,推荐(XXmodule.EnableResultCallback()) | EN: Obsolete,recommend(XXmodule.EnableResultCallback())", false)]
	public void ReportModuleResults(List<uint> moduleIds)
	{
		try
		{
			TempVmSolution.Init();
			ImvsSdkDefine.IMVS_REPORT_MODULE_RES_INFO_LIST reportModuleResult = new ImvsSdkDefine.IMVS_REPORT_MODULE_RES_INFO_LIST
			{
				nNum = moduleIds.Count,
				anReportModuIds = new uint[2064]
			};
			for (int i = 0; i < moduleIds.Count; i++)
			{
				reportModuleResult.anReportModuIds[i] = moduleIds[i];
			}
			ServerSDKManager.serverSDKManager.mSolutionManager.SetReportModuleResult(reportModuleResult);
		}
		catch (Exception ex)
		{
			throw GetVmException(ex);
		}
	}

	/// <summary>
	/// 创建流程（内部使用）
	/// </summary>
	[Obsolete("CH: 弃用 | EN: Obsolet", false)]
	public VmProcedure CreateProcedure(ImvsSdkDefine.IMVS_PROCESS_INFO info, object calibData = null)
	{
		if (Instance.Modules.Count((VmModule p) => p.ID == info.nProcID) == 0)
		{
			Ioc.Get<IUnRedoService>()?.SetIsLaziLoading(isLaziLoading: true);
			VmProcedure result = ChangToProcess(info, calibData);
			Ioc.Get<IUnRedoService>()?.SetIsLaziLoading(isLaziLoading: false);
			return result;
		}
		return null;
	}

	/// <summary>
	/// 获取VM内部异常，用于规范抛出去的异常
	/// </summary>
	/// <returns></returns>
	[Obsolete("CH: 弃用,推荐(GetVmException(Exception ex)) | EN: Obsolete,recommend(GetVmException(Exception ex))", false)]
	public static VmException GetVmException1(Exception ex)
	{
		VmException result = null;
		while (ex != null)
		{
			if (ex is VmException)
			{
				result = (VmException)ex;
				break;
			}
			ex = ex.InnerException;
		}
		return result;
	}

	/// <summary>
	/// 载入独立Group
	/// </summary>
	/// <param name="strPath"></param>
	/// <param name="strPassword"></param>
	/// <returns></returns>
	[Obsolete("CH: 弃用,推荐(LoadIndependentGroup(string strPath, string strPassword = null)) | EN: Obsolete,recommend(LoadIndependentGroup(string strPath, string strPassword = null))", false)]
	public static IMVSGroup ImportIndependentGroup(string strPath, string strPassword = null)
	{
		return internalImportIndependentGroup(strPath, strPassword);
	}

	/// <summary>
	/// 静默执行，在OnExecuteAfterLoad中需要调用，否则出错
	/// </summary>
	public void SilentExecute(int bIfAllModu = 0)
	{
		LogHelper.Info("sileentExecute ------begin");
		OnExecuteAfterLoadStart?.Invoke(instance, null);
		LogHelper.Info("OnExecuteAfterLoadStart ------end");
		IsOnExecuteAfterLoad = true;
		try
		{
			LogHelper.Info("ServerSDKManager.serverSDKManager.mSolutionManager.SilentExecute ------beigin");
			ServerSDKManager.serverSDKManager.mSolutionManager.SilentExecute(bIfAllModu);
			LogHelper.Info("ServerSDKManager.serverSDKManager.mSolutionManager.SilentExecute ------end");
		}
		catch (VmException vmException)
		{
			LogHelper.Error("(VmSolution.SilentExecute)");
			ShowMessage.ShowVmExceptionMessage(vmException, 3000u);
		}
		catch (Exception ex)
		{
			LogHelper.Error("(VmSolution.SilentExecute)" + ex.Message);
			HikMessageBox.Show(ex.Message, MessageIcons.Error);
		}
		IsOnExecuteAfterLoad = false;
		LogHelper.Info("OnExecuteAfterLoadFinally ------beigin");
		OnExecuteAfterLoadFinally?.Invoke(instance, null);
		LogHelper.Info("OnExecuteAfterLoadFinally ------end");
	}

	internal static void MSolutionManager_OnExecuteAfterLoad(ImvsSdkDefine.IMVS_EXECUTE_STATE_AFTER_LOAD_INFO stExecuteState)
	{
		OnExecuteAfterLoad?.Invoke(null, new ObjectEventArgs
		{
			Param = (stExecuteState.nStatus == 0)
		});
	}

	internal static void MSolutionManager_OnDongleCallBack(ImvsSdkDefine.IMVS_DONGLE_INFO moduleInfo)
	{
		OnDongleEvent?.Invoke(moduleInfo);
		CameraNum = moduleInfo.nCameraNum;
		RemainTime = moduleInfo.nRemainTime;
		switch (moduleInfo.nDongleStatus)
		{
		case 0:
		{
			DogState = DogStateEnum.NORMAL;
			string arg = Hik.Util.StringUtil.UTF8GetString(moduleInfo.strDongleType);
			UIVersionControlManagerCS.DongleSeriesEnum nDongleSeries = (UIVersionControlManagerCS.DongleSeriesEnum)moduleInfo.nDongleSeries;
			StrDogState = $"{arg}({nDongleSeries.ToString()})";
			StrDongleSerialNo = $"{Hik.Util.StringUtil.UTF8GetString(moduleInfo.strDongleSerialNo)}";
			SoftLanguage = moduleInfo.nSoftLanguage;
			break;
		}
		case -536869120:
		case -536869116:
		case -536869115:
		case -536869114:
		case -536869093:
			DogState = DogStateEnum.UNDETECTED;
			break;
		case -536869119:
		case -536869118:
		case -536869117:
		case -536869113:
			DogState = DogStateEnum.OVERDUE;
			break;
		}
	}

	internal static void MSolutionManager_OnProxyCrashNotice(ImvsSdkDefine.IMVS_PROXY_CRASH_SP_INFO proxyCrashInfo)
	{
		LogHelper.Error("底层代理崩溃回调通知");
		if (proxyCrashInfo.nHasSolStatu == 0)
		{
			instance.GloalScriptContinusRunEnable = false;
			instance.Modules.OfType<VmProcedure>().ToList().ForEach((VmProcedure x) =>
			{
				x.UpdateContinusRunStatus(isRun: false);
			});
			instance.AllModules.Clear();
		}
		OnProxyCrashNoticeCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = proxyCrashInfo.nHasSolStatu
		});
		OnProxyCrashEvent?.Invoke(proxyCrashInfo);
	}

	internal static void MSolutionManager_OnServerStatusCallBack(ImvsSdkDefine.IMVS_SERVER_INFO serverInfo)
	{
		OnServerStatusCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = serverInfo.nServerStatus
		});
		OnServerStatusEvent?.Invoke(serverInfo);
	}

	internal static void MSolutionManager_OnResourceUsagePushCallBack(ImvsSdkDefine.IMVS_RESOURCE_USAGE_INFO resourceInfo)
	{
		MemeryUsed = resourceInfo.nMemUsage;
		CPUUsed = resourceInfo.nCPUUsage;
		NetUsed = resourceInfo.nNetWorkUsage;
	}

	/// <summary>
	///             保存方案开始
	/// </summary>
	internal static void MSolutionManager_OnSolutionSaveBeginCallBack(ImvsSdkDefine.IMVS_SOLUTION_SAVE_BEGEIN_INFO solutionSaveBeginInfo)
	{
		if (Instance.isDisplayProgress)
		{
			OnSolutionSaveBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
			{
				Param = "方案保存中"
			});
		}
	}

	/// <summary>
	/// 保存方案结束
	/// </summary>
	internal static void MSolutionManager_OnSolutionSaveEndCallBack(ImvsSdkDefine.IMVS_SOLUTION_SAVE_END_INFO solutionSaveEndInfo)
	{
		if (Instance.isDisplayProgress)
		{
			EndEventArgs e = new EndEventArgs();
			e.ClientType = solutionSaveEndInfo.nClientType;
			e.ErrorCode = (int)solutionSaveEndInfo.nStatus;
			if (solutionSaveEndInfo.nStatus != 0)
			{
				e.BoxTitle = AppLocalizationService.GetLang("Lang_SolSaveFail");
			}
			OnSolutionSaveEndCallBack?.Invoke(SolObj, e);
		}
		Instance._isSaving = false;
		LogHelper.Debug($"MSolutionManager_OnSolutionSaveEndCallBack_{Instance._isSaving}");
	}

	/// <summary>
	/// 保存方案进度
	/// </summary>
	internal static void MSolutionManager_OnSolutionSaveProgressCallBack(ImvsSdkDefine.IMVS_SOLUTION_SAVE_PROCESS_INFO solutionSaveProcessInfo)
	{
		if (Instance.isDisplayProgress)
		{
			OnSolutionSaveProgressCallBack?.Invoke(SolObj, new ObjectEventArgs
			{
				Param = solutionSaveProcessInfo.nProcess
			});
		}
	}

	/// <summary>
	/// 加载方案进度
	/// </summary>
	internal static void MSolutionManager_OnSolutionLoadProgressCallBack(ImvsSdkDefine.IMVS_SOLUTION_LOAD_PROCESS_INFO solutionLoadProcessInfo)
	{
		OnSolutionLoadProgressCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = solutionLoadProcessInfo.nProcess
		});
		OnSolutionLoadProgressEvent?.Invoke(solutionLoadProcessInfo);
	}

	/// <summary>
	/// 方案加载开始
	/// </summary>
	internal static void MSolutionManager_OnSolutionLoadBeginCallBack(ImvsSdkDefine.IMVS_SOLUTION_LOAD_BEGEIN_INFO solutionLoadBeginInfo)
	{
		isLoading = true;
		OnSolutionLoadBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = "方案加载中"
		});
		OnSolutionLoadBeginEvent?.Invoke(solutionLoadBeginInfo);
	}

	/// <summary>
	/// 方案加载结束
	/// </summary>
	internal static void MSolutionManager_OnSolutionLoadEndCallBack(ImvsSdkDefine.IMVS_SOLUTION_LOAD_END_INFO solutionLoadEndInfo)
	{
		isLoading = false;
		_failReflashFlag = true;
		EndEventArgs e = new EndEventArgs();
		e.ClientType = solutionLoadEndInfo.nClientType;
		e.ErrorCode = (int)solutionLoadEndInfo.nStatus;
		if (e.ErrorCode != -536870093)
		{
			_failReflashFlag = false;
		}
		if (instance.SolutionPath == null)
		{
			instance.SolutionPath = Hik.Util.StringUtil.UTF8GetString(solutionLoadEndInfo.strSolPath);
		}
		if (solutionLoadEndInfo.nStatus != 0 && !_failReflashFlag)
		{
			e.BoxTitle = AppLocalizationService.GetLang("Lang_SolLoadErr");
			Instance = new VmSolution();
		}
		try
		{
			if (!CVmObjectBase.GetIsSDKMode())
			{
				((IVmSolution)instance).ResetAllModulesCallBackStatus();
			}
		}
		catch (Exception ex)
		{
			if (ex is VmException ex2)
			{
				LogHelper.Error(ErrorCodeInfo.GetErrorInfo(ex2.errorCode) + "Error Code = " + ex2.errorCode);
			}
		}
		LogHelper.Info("加载方案后失败状态 ------begin");
		if (_failReflashFlag)
		{
			ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId = new ImvsSdkDefine.IMVS_MODULE_ID_LIST
			{
				anModuleId = new uint[2064],
				nReserved = new uint[4]
			};
			int num = 0;
			foreach (VmModule value2 in Instance.AllModules.Values)
			{
				stModuleId.anModuleId[num++] = value2.ID;
			}
			stModuleId.nNum = num;
			if (num > 0)
			{
				lock (_failFlagObject)
				{
					ImvsSdkDefine.IMVS_MODULE_STATE_LIST value = ServerSDKManager.serverSDKManager.mModuleManager.GetMultiObjectLoadState(stModuleId).Value;
					for (int i = 0; i < value.nNum; i++)
					{
						ImvsSdkDefine.IMVS_MODULE_STATE_INFO stateInfo = value.astModuState[i];
						VmModule vmModule = Instance.AllModules.Values.FirstOrDefault((VmModule p) => p.ID == stateInfo.nModuleId);
						if (vmModule != null)
						{
							switch (stateInfo.nEnable)
							{
							case 1u:
								vmModule.isLoadFailed = false;
								vmModule.OnPropertyChanged("IsLoadFailed");
								break;
							case 0u:
								vmModule.isLoadFailed = true;
								vmModule.OnPropertyChanged("IsLoadFailed");
								break;
							case 2u:
								vmModule.isLoadFailed = null;
								vmModule.OnPropertyChanged("IsLoadFailed");
								break;
							}
							vmModule.initLoadState = true;
						}
					}
				}
			}
		}
		LogHelper.Info("加载方案后失败状态 ------end");
		OnSolutionLoadEndCallBack?.Invoke(SolObj, e);
		OnSolutionLoadEndEvent?.Invoke(solutionLoadEndInfo);
	}

	/// <summary>
	/// 方案加载警告信息
	/// </summary>
	internal static void MSolutionManager_OnModelLoadWarn(ImvsSdkDefine.IMVS_LOAD_MODULE_ERROR_INFO_LIST moduleInfoList)
	{
		if (moduleInfoList.nModuleNum != 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < moduleInfoList.nModuleNum; i++)
			{
				ImvsSdkDefine.IMVS_LOAD_MODULE_ERROR_INFO iMVS_LOAD_MODULE_ERROR_INFO = moduleInfoList.astLoadModuErrInfo[i];
				stringBuilder.Append(iMVS_LOAD_MODULE_ERROR_INFO.nModuleID).Append(" ").Append(Hik.Util.StringUtil.UTF8GetString(iMVS_LOAD_MODULE_ERROR_INFO.strModuleName))
					.Append(" ")
					.Append(AppLocalizationService.GetLang("errInfo:"))
					.Append(ErrorCodeInfo.GetErrorInfo(iMVS_LOAD_MODULE_ERROR_INFO.nErrorCode))
					.Append("\r\n");
			}
			OnModelLoadWarn?.Invoke(SolObj, new ObjectEventArgs
			{
				Param = stringBuilder.ToString()
			});
		}
		OnModelLoadWarnEvent?.Invoke(moduleInfoList);
	}

	/// <summary>
	/// 加载方案通知界面获取
	/// </summary>
	internal static void MSolutionManager_OnUIGlobalProcessObjectIDCallBack(ImvsSdkDefine.IMVS_GLOBAL_PROCESS_UI_OBJECT_INFO uiGlobalProcessObjectInfo)
	{
		LogHelper.Info("通知获取界面数据回调开始");
		_updateContinueStatus = new ConcurrentDictionary<uint, bool>();
		LogHelper.Info("开始获取界面数据");
		Instance = new VmSolution();
		LogHelper.Info("结束获取界面数据");
		LogHelper.Info("通知获取界面数据回调结束");
	}

	/// <summary>
	/// 全局脚本回调
	/// </summary>
	internal static void MSolutionManager_OnGlobalScriptRecvDataCallBack(ImvsSdkDefine.IMVS_GLOBALSCRIPT_REPORTDATA_INFO globalScrptReportInfo)
	{
		object obj = UTF8Marshaler.GetInstance("cookie").MarshalNativeToManaged(globalScrptReportInfo.pData);
		if (obj == null)
		{
			return;
		}
		if (obj is string text && text.Contains("execute error"))
		{
			if (mbSolRunFlag)
			{
				return;
			}
			string[] array = text.Split(':');
			if (array.Length > 1)
			{
				uint result = 0u;
				if (uint.TryParse(array[1], out result))
				{
					mnSolRunErrorde = (int)result;
				}
				else
				{
					int.TryParse(array[1], out mnSolRunErrorde);
				}
			}
			mbSolRunFlag = true;
		}
		else
		{
			OnGlobalScriptReportCallBack?.Invoke(null, new ObjectEventArgs
			{
				Param = obj
			});
		}
	}

	/// <summary>
	/// 全局/局部变量上报事件
	/// </summary>
	internal static void MSolutionManager_OnVarValueChangeCallBack(ImvsSdkDefine.IMVS_VAR_REPORT_VALUE_INFO varChangeInfo)
	{
		if (!instance.AllModules.TryGetValue(varChangeInfo.nModuleID, out var value) || varChangeInfo.pData == IntPtr.Zero)
		{
			return;
		}
		try
		{
			ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX resultInfo = (ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX)Marshal.PtrToStructure(varChangeInfo.pData, typeof(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX));
			ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data iMVS_MODULE_RESULT_INFO_LIST_EX_Data = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(resultInfo);
			Dictionary<int, VmIO> dictionary = value.Outputs.leafIOs;
			Dictionary<int, VmIO> dictionary2 = value.Outputs.leafIOsWithoutTimeStamp;
			List<VmIO> list = new List<VmIO>();
			for (int i = 0; i < iMVS_MODULE_RESULT_INFO_LIST_EX_Data.nResultNum; i++)
			{
				int hashCode = iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].strParamName.GetHashCode();
				if (!dictionary2.ContainsKey(hashCode))
				{
					continue;
				}
				VmIO vmIO = dictionary2[hashCode];
				vmIO._value = null;
				vmIO._valueList.Clear();
				vmIO.isNull = true;
				if (iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i].nValueNum > 0)
				{
					try
					{
						vmIO.SetValueFromServer(iMVS_MODULE_RESULT_INFO_LIST_EX_Data.pInfo[i]);
					}
					catch (Exception ex)
					{
						LogHelper.Error("SetValueFromServer exception:" + ex.ToString());
					}
				}
				while (vmIO.CombinRoot != null)
				{
					vmIO = vmIO.CombinRoot;
				}
				if (!list.Contains(vmIO))
				{
					list.Add(vmIO);
				}
			}
			(value as IVarModule).VarValueReport(list);
		}
		catch (Exception ex2)
		{
			LogHelper.Error("OnVarValueChangeCallBack is error:" + ex2.ToString());
		}
	}

	/// <summary>
	/// 上报事件
	/// </summary>
	internal static void MSolutionManager_OnReportDataToVM(ImvsSdkDefine.IMVS_ALGORITHM_INIT_RESULT_INFO algResultInfou)
	{
		OnReportModuleData?.Invoke(null, new ObjectEventArgs
		{
			Param = algResultInfou
		});
	}

	/// <summary>
	/// 关闭方案
	/// </summary>
	internal static void MSolutionManager_OnCloseSolutionCallBack(ImvsSdkDefine.IMVS_CLOSE_SOLUTION_STATUS_INFO closeSolInfo)
	{
		if (closeSolInfo.nClientType != 0)
		{
			Instance = new VmSolution();
		}
	}

	/// <summary>
	/// 流程解注册
	/// </summary>
	internal static void MSolutionManager_OnProcedureUnRegister(ImvsSdkDefine.IMVS_PROCEDURE_UNREGISTER_INFO procUnregisterInfo)
	{
		if (procUnregisterInfo.nClientType != 0)
		{
			OnProcedureUnRegister?.Invoke(SolObj, new ObjectEventArgs
			{
				Param = procUnregisterInfo.nProcessID
			});
		}
		OnProcedureUnRegisterEvent?.Invoke(procUnregisterInfo);
	}

	/// <summary>
	/// 数据队列上报
	/// </summary>
	internal static void MSolutionManager_DataQueueReportData(ImvsSdkDefine.IMVS_REPORT_DATAQUEUE_INFO dataqueueInfo)
	{
	}

	/// <summary>
	/// 通信数据接收
	/// </summary>
	internal static void MSolutionManager_OnCommuRecvData(ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO reportData)
	{
		OnCommunicationRecvCallBackEvent?.Invoke(reportData);
	}

	/// <summary>
	/// 通信设备状态
	/// </summary>
	internal static void MSolutionManager_OnCommuStatus(ImvsSdkDefine.IMVS_COMMU_REPORT_DATA_INFO reportData)
	{
		OnCommunicationStatusCallBackEvent?.Invoke(reportData);
		instance.OnCommunicationStatusCallBackEventF?.Invoke(instance, new ObjectEventArgs
		{
			Param = reportData
		});
	}

	/// <summary>
	/// 相机取图结束信息
	/// </summary>
	internal static void MSolutionManager_OnCameraCollect(ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO cameraCollectInfo)
	{
		OnCameraCollectCallBackEvent?.Invoke(cameraCollectInfo);
	}

	/// <summary>
	/// 相机取图开始信息
	/// </summary>
	internal static void MSolutionManager_OnCameraCollectStart(ImvsSdkDefine.IMVS_CAMERA_COLLECT_INFO cameraCollectStartInfo)
	{
		OnCameraCollectStartCallBackEvent?.Invoke(cameraCollectStartInfo);
	}

	/// <summary>
	/// 相机连接状态
	/// </summary>
	internal static void MSolutionManager_OnCameraConnectStatus(ImvsSdkDefine.IMVS_CAMERA_CONNECT_STATUS_INFO cameraConnectStatusInfo)
	{
		OnCameraConnectStatusCallBackEvent?.Invoke(cameraConnectStatusInfo);
		instance.OnCameraConnectStatusCallBackEventF?.Invoke(instance, new ObjectEventArgs
		{
			Param = cameraConnectStatusInfo
		});
	}

	/// <summary>
	/// 通信连接状态
	/// </summary>
	internal static void MSolutionManager_OnCommuConnect(ImvsSdkDefine.IMVS_COMMUNICATION_CONNECT_INFO connectCollectInfo)
	{
		OnCommuConnectCallBackEvent?.Invoke(connectCollectInfo);
	}

	/// <summary>
	/// 接管方案时，运行状态回调
	/// </summary>
	internal static void MProcessManager_OnProcessExecuteStatusCallBack(ImvsSdkDefine.IMVS_PROC_CONTIEXECUTE_INFO_LIST statusInfo)
	{
		uint statusNum = statusInfo.nNum;
		if (statusNum < 1)
		{
			return;
		}
		Task.Run(() =>
		{
			try
			{
				bool flag = false;
				if (statusInfo.stProcExecuteInfo[statusNum - 1].nProcessID == 10999)
				{
					statusNum--;
					while (statusNum != instance.AllModules.Values.OfType<VmProcedure>().Count() && !instance.IsDispose)
					{
						Thread.Sleep(1);
					}
					if (statusInfo.stProcExecuteInfo[statusNum].nProcRunStatus == 1)
					{
						flag = true;
						instance.UpdateContinusRunStatus(isRun: true);
					}
				}
				else
				{
					while (statusNum != instance.AllModules.Values.OfType<VmProcedure>().Count() && !instance.IsDispose)
					{
						Thread.Sleep(1);
					}
				}
				for (int i = 0; i < statusNum; i++)
				{
					if (statusInfo.stProcExecuteInfo[i].nProcRunStatus == 1 && !flag && instance.AllModules.ContainsKey(statusInfo.stProcExecuteInfo[i].nProcessID))
					{
						(instance.AllModules[statusInfo.stProcExecuteInfo[i].nProcessID] as VmProcedure)?.UpdateContinusRunStatus(isRun: true);
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("OnProcessExecuteStatusCallBack异常信息：" + ex.Message);
				LogHelper.Error("OnProcessExecuteStatusCallBack堆栈信息：" + ex.StackTrace);
			}
		});
	}

	/// <summary>
	/// 流程停止回调
	/// </summary>
	internal static void MProcessManager_OnModuleStopRunProcessStatusCallBack(ImvsSdkDefine.IMVS_STATUS_PROCESS_STOP_INFO statusInfo)
	{
		OnProcessStatusStopEvent?.Invoke(statusInfo);
		_updateContinueStatus.AddOrUpdate(statusInfo.nProcessID, addValue: false, (uint oldKey, bool oldValue) => false);
		try
		{
			if (instance.dictProcessIDStopStatus.ContainsKey(statusInfo.nProcessID))
			{
				instance.dictProcessIDStopStatus[statusInfo.nProcessID] = statusInfo.nStopAction;
			}
			if ((statusInfo.nStopAction == 2 || statusInfo.nStopAction == 3 || statusInfo.nStopAction == 1) && instance.AllModules.ContainsKey(statusInfo.nProcessID))
			{
				(instance.AllModules[statusInfo.nProcessID] as VmProcedure)?.UpdateContinusRunStatus(isRun: false);
				lock (waitNotifyLock)
				{
					if (_waitNotifyPages.ContainsKey(statusInfo.nProcessID))
					{
						LogHelper.Info($"入队MProcessManager_OnModuleStopRunProcessStatusCallBack流程停止回调流程ID为{statusInfo.nProcessID}空闲状态为0");
						_waitNotifyPages[statusInfo.nProcessID] = new WaitNotifyPage
						{
							OperID = 1,
							FunID = statusInfo.nProcessID,
							IsRunning = false
						};
					}
					else
					{
						LogHelper.Info($"入队MProcessManager_OnModuleStopRunProcessStatusCallBack流程停止回调流程ID为{statusInfo.nProcessID}空闲状态为0");
						_waitNotifyPages.Add(statusInfo.nProcessID, new WaitNotifyPage
						{
							OperID = 1,
							FunID = statusInfo.nProcessID,
							IsRunning = false
						});
					}
				}
			}
			if (instance.dictProcessIDStopStatus.Count > 0 && instance.dictProcessIDStopStatus.FirstOrDefault((KeyValuePair<uint, uint> x) => x.Value == 0 || x.Value == 1).Key == 0)
			{
				instance.GloalScriptContinusRunEnable = false;
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("OnModuleStopRunProcessStatusCallBack exception：" + ex.ToString());
		}
	}

	/// <summary>
	/// 单个流程连续运行开始回调
	/// </summary>
	internal static void MProcessManager_OnProcessStartContinuousStatusCallBack(ImvsSdkDefine.IMVS_STATUS_PROCESS_START_CONTINUOUSLY_INFO statusInfo)
	{
		try
		{
			OnProcessStatusStartEvent?.Invoke(statusInfo);
			_updateContinueStatus.AddOrUpdate(statusInfo.nProcessID, addValue: true, (uint oldKey, bool oldValue) => true);
			if (!instance.AllModules.TryGetValue(statusInfo.nProcessID, out var value))
			{
				return;
			}
			LogHelper.Info($"连续执行开始回调MProcessManager_OnProcessStartContinuousStatusCallBack，给对应流程{statusInfo.nProcessID}的执行状态设置为true");
			(value as VmProcedure)?.UpdateContinusRunStatus(isRun: true);
			lock (waitNotifyLock)
			{
				if (_waitNotifyPages.ContainsKey(value.ID))
				{
					LogHelper.Info($"入队连续执行开始回调{value.ID}isRuning为true");
					_waitNotifyPages[value.ID] = new WaitNotifyPage
					{
						OperID = 1,
						FunID = value.ID,
						IsRunning = true
					};
				}
				else
				{
					LogHelper.Info($"入队连续执行开始回调{value.ID}isRuning为true");
					_waitNotifyPages.Add(value.ID, new WaitNotifyPage
					{
						OperID = 1,
						FunID = value.ID,
						IsRunning = true
					});
				}
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("OnProcessStartContinuousStatusCallBack exception：" + ex.ToString());
		}
	}

	/// <summary>
	/// 所有流程连续运行回调 调用所有流程连续运行会进入该回调
	/// </summary>
	internal static void MSolutionManager_OnModuleContinousRunStatusCallBack(ImvsSdkDefine.IMVS_STATUS_START_RUN_INFO statusInfo)
	{
		try
		{
			if (!Instance.GloalScriptContinusRunEnable)
			{
				Instance.AllModules.Values.OfType<VmProcedure>().ToList().ForEach((VmProcedure x) =>
				{
					if (!x.IsForbidden)
					{
						x.UpdateContinusRunStatus(isRun: true);
						lock (waitNotifyLock)
						{
							if (_waitNotifyPages.ContainsKey(x.ID))
							{
								_waitNotifyPages[x.ID] = new WaitNotifyPage
								{
									OperID = 1,
									FunID = x.ID,
									IsRunning = true
								};
							}
							else
							{
								_waitNotifyPages.Add(x.ID, new WaitNotifyPage
								{
									OperID = 1,
									FunID = x.ID,
									IsRunning = true
								});
							}
						}
					}
				});
				return;
			}
			Instance.AllModules.Values.OfType<VmProcedure>().ToList().ForEach((VmProcedure x) =>
			{
				if (!x.IsForbidden)
				{
					lock (waitNotifyLock)
					{
						if (_waitNotifyPages.ContainsKey(x.ID))
						{
							_waitNotifyPages[x.ID] = new WaitNotifyPage
							{
								OperID = 0,
								FunID = x.ID,
								IsRunning = true
							};
						}
						else
						{
							_waitNotifyPages.Add(x.ID, new WaitNotifyPage
							{
								OperID = 0,
								FunID = x.ID,
								IsRunning = true
							});
						}
					}
				}
			});
		}
		catch (Exception ex)
		{
			LogHelper.Error("OnModuleContinousRunStatusCallBack exception：" + ex.ToString());
		}
	}

	/// <summary>
	/// 模块状态 OK或者NG
	/// </summary>
	internal static void MModuleManager_OnReportModuleStatusCallBack(ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO_LIST reportModuleStatusInfo)
	{
		lock (refreshParamthread)
		{
			VmModule value = null;
			instance.AllModules.TryGetValue(reportModuleStatusInfo.nProcID, out value);
			if (value == null && reportModuleStatusInfo.nProcID == VirtualPro.ID)
			{
				value = VirtualPro;
			}
			if (value == null)
			{
				return;
			}
			(value as VmProcedure).forReceiveStore = reportModuleStatusInfo;
			if (reportModuleStatusInfo.nNum > 0 && reportModuleStatusInfo.pstModuleStatuInfo[0].bIsSelfRun == 1)
			{
				(value as VmProcedure).forReceiveStore = reportModuleStatusInfo;
				if (reportModuleStatusInfo.nNum > 0 && reportModuleStatusInfo.pstModuleStatuInfo[0].bIsSelfRun == 1)
				{
					(value as VmProcedure).AddSelfExcuteReceiveData(reportModuleStatusInfo.pstModuleStatuInfo.Where((ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO p) => p.bIsSelfRun == 1).ToList());
				}
				(value as VmProcedure).dataNumForSelfStore = true;
			}
			(value as VmProcedure).dataNumForReceiveStore = true;
			dataNumForReceiveStore = true;
		}
	}

	/// <summary>
	/// 流程工作状态空闲忙碌回调
	/// </summary>
	internal static void MProcessManager_OnWorkStatusCallBack(ImvsSdkDefine.IMVS_MODULE_WORK_STAUS workStatusInfo)
	{
		try
		{
			OnWorkStatusEvent?.Invoke(workStatusInfo);
			if (instance.AllModules.Values.OfType<VmProcedure>().Count() <= 0)
			{
				return;
			}
			instance.OnContinuousRunStatusCallBack?.Invoke((ContinuousRunStatus)workStatusInfo.nWorkStatus, workStatusInfo.nProcessID);
			if (!instance.AllModules.ContainsKey(workStatusInfo.nProcessID))
			{
				return;
			}
			VmModule proc = instance.AllModules[workStatusInfo.nProcessID];
			if (proc == null)
			{
				return;
			}
			if (workStatusInfo.nExecuteCount != 0)
			{
				if (workStatusInfo.nWorkStatus == 0)
				{
					proc.ExecuteCount = workStatusInfo.nExecuteCount;
					(proc as VmProcedure).ProcessTime = workStatusInfo.fProcessTime;
					lock (currentObj)
					{
						if (!_waitDoneProcedureCache.ContainsKey(workStatusInfo.nProcessID))
						{
							_waitDoneProcedureCache.Add(workStatusInfo.nProcessID, 0);
						}
					}
					(proc as VmProcedure).canModuleSelfExecute = true;
					List<VmModule> list = instance.AllModules.Values?.Where((VmModule p) => p.Parent == proc)?.ToList();
					if (list != null)
					{
						foreach (VmModule item in list)
						{
							if (item is IMVSGroup { ReportResultEnable: false } iMVSGroup)
							{
								iMVSGroup.RaiseGroupSubModuleValueChanged();
							}
						}
					}
					(proc as VmProcedure)?.InvokeOnWorkEndStatusCallBack(workStatusInfo, workStatusInfo.nExecuteCount);
				}
				else
				{
					if (isClearLastExecuteResult && workStatusInfo.nExecuteCount > 1)
					{
						List<VmModule> list2 = instance.AllModules.Values?.Where((VmModule p) => p.Root == proc)?.ToList();
						if (list2 != null)
						{
							foreach (VmModule item2 in list2)
							{
								List<VmIO> list3 = item2.Outputs.leafIOsWithoutTimeStamp.Values.ToList();
								foreach (VmIO item3 in list3)
								{
									item3.SetNoneValueWithoutRaiseEvent();
								}
								item2.IsEmpty = true;
							}
						}
					}
					(proc as VmProcedure)?.InvokeOnWorkBeginStatusCallBack(workStatusInfo, workStatusInfo.nExecuteCount);
				}
				Tuple<VmModule, ValueEventArgs> data = new Tuple<VmModule, ValueEventArgs>(proc, new ValueEventArgs
				{
					Value = workStatusInfo,
					ExecuteCount = workStatusInfo.nExecuteCount
				});
				instance.EnqueueForCallBack(data);
			}
			proc.OnModuleResultCallBack(workStatusInfo, workStatusInfo.nExecuteCount);
			LogHelper.Info($"---MProcessManager_OnWorkStatusCallBack回调入队前判断---流程ID为{workStatusInfo.nProcessID}空闲状态为{workStatusInfo.nWorkStatus}");
			if (proc.ContinuousRunEnable || Instance.ContinuousRunEnable)
			{
				return;
			}
			lock (waitNotifyLock)
			{
				if (_waitNotifyPages.ContainsKey(workStatusInfo.nProcessID))
				{
					int operID = ((_waitNotifyPages[workStatusInfo.nProcessID].OperID != 0) ? 2 : 0);
					_waitNotifyPages[workStatusInfo.nProcessID] = new WaitNotifyPage
					{
						OperID = operID,
						FunID = workStatusInfo.nProcessID,
						IsRunning = (workStatusInfo.nWorkStatus == 1)
					};
				}
				else
				{
					_waitNotifyPages.Add(workStatusInfo.nProcessID, new WaitNotifyPage
					{
						OperID = 0,
						FunID = workStatusInfo.nProcessID,
						IsRunning = (workStatusInfo.nWorkStatus == 1)
					});
				}
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("ErrorOnWorkStatusCallBack" + ex.Message + ex.StackTrace);
		}
	}

	/// <summary>
	/// 方案执行结束
	/// </summary>
	internal static void MSolutionManager_OnSolutionRunEndCallBack()
	{
		mbSolRunFlag = true;
	}

	/// <summary>
	/// 设置流程回调延时
	/// </summary>
	public static void SetProcedureDelayTime(uint millseconds)
	{
		ServerSDKManager.serverSDKManager.mSolutionManager.SetExecuteDelayTime(millseconds);
	}

	internal static void MModuleManager_OnModuleResultCallBack(ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data result, bool isCallBack)
	{
		try
		{
			if (!instance.AllModules.TryGetValue(result.nModuleID, out var value))
			{
				return;
			}
			if (isCallBack && value is IMVSGroup module && result.pInfo.Any((ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data p) => p.strParamName == "StartIndex"))
			{
				int num = result.pInfo.Single((ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data p) => p.strParamName == "StartIndex").pIntValue[0];
				int num2 = result.pInfo.Single((ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data p) => p.strParamName == "EndIndex").pIntValue[0];
				int num3 = result.pInfo.Single((ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data p) => p.strParamName == "LoopIndex").pIntValue[0];
				uint nStatus = result.nStatus;
				int num4 = num3 - num + 1;
				if (num4 > 0)
				{
					bool value2 = num3 + 1 == num2 || nStatus == 5 || nStatus == 2;
					SetValue(module, ref result, value2);
					return;
				}
			}
			SetValue(value, ref result, null, isCallBack);
		}
		catch (Exception ex)
		{
			LogHelper.Error("异常信息：" + ex.Message);
			LogHelper.Error("堆栈信息：" + ex.StackTrace);
		}
	}

	/// <summary>
	/// 模块结果回调
	/// </summary>
	internal static void MModuleManager_OnModuleResultCallBack(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data result)
	{
		OnModuleResultCallbackEvent?.Invoke(result);
		MModuleManager_OnModuleResultCallBack(ref result, isCallBack: true);
		FunctionCameraManager.Execute("GetFrameMeanCount");
	}

	/// <summary>
	/// 设置模块结果回调的值到VMIO
	/// </summary>
	/// <param name="module">回调模块</param>
	/// <param name="result">回调结果结构体</param>
	/// <param name="isLoopEnd">是否循环结束，null-非循环结果，true-循环结束，false-循环未结束</param>
	/// <param name="isCallBack">是否回调结果</param>
	private static void SetValue(VmModule module, ref ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data result, bool? isLoopEnd = null, bool isCallBack = true)
	{
		bool flag = module is IVmModuleSet;
		module.InitCurrentIO();
		Dictionary<int, VmIO> dictionary = (flag ? module.leafIOs : module.Outputs.leafIOs);
		Dictionary<int, VmIO> dictionary2 = (flag ? module.leafIOsWithoutTimeStamp : module.Outputs.leafIOsWithoutTimeStamp);
		if (module.ExecuteCount != result.nExecuteCount || result.bIsSelfExecute == 1 || !module.NeedMergeValueInLoop || module is VmProcedure)
		{
			foreach (VmIO value in dictionary2.Values)
			{
				value._ExecuteCount = result.nExecuteCount;
				value._value = null;
				value._valueList.Clear();
				value.isNull = true;
				value.RaiseValueChanged = true;
			}
		}
		else if (isCallBack)
		{
			foreach (VmIO value2 in dictionary.Values)
			{
				value2.RaiseValueChanged = false;
			}
		}
		module.executeCount = result.nExecuteCount;
		List<int> list = new List<int>();
		for (int i = 0; i < result.nResultNum; i++)
		{
			int hashCode = result.pInfo[i].strParamName.GetHashCode();
			if (dictionary2.ContainsKey(hashCode))
			{
				VmIO.SetValueFromServer(dictionary2[hashCode], ref result.pInfo[i], isCallBack);
				dictionary2[hashCode].isNull = false;
			}
			list.Add(hashCode);
		}
		foreach (int key in dictionary2.Keys)
		{
			if (dictionary2[key] != null && dictionary2[key].TypeName != null)
			{
				if (isLoopEnd == false)
				{
					dictionary2[key].RaiseValueChanged = false;
				}
				else if (!list.Contains(key))
				{
					dictionary2[key].SetValueFromDefault();
				}
			}
		}
		if (isLoopEnd == true)
		{
			if (!(module.Parent is IMVSGroup))
			{
				if (module.ReportResultEnable)
				{
					foreach (VmIO value3 in dictionary.Values)
					{
						if (!value3.RaiseValueChanged)
						{
							value3.RaiseValueChanged = true;
							if (value3.GetCurValueCount() > 0)
							{
								value3.InvokeValueChangedEvent();
							}
						}
					}
				}
				IMVSGroup iMVSGroup = module as IMVSGroup;
				iMVSGroup.RaiseGroupSubModuleValueChanged();
			}
			module.IsEmpty = false;
			if (isCallBack)
			{
				module.OnModuleResultCallBack(result, result.nExecuteCount);
				Tuple<VmModule, ValueEventArgs> data = new Tuple<VmModule, ValueEventArgs>(module, new ValueEventArgs
				{
					Value = result,
					ExecuteCount = result.nExecuteCount
				});
				instance.EnqueueForCallBack(data);
			}
		}
		else if (!isLoopEnd.HasValue)
		{
			module.Inputs.SetExecuteCount(result.nExecuteCount);
			module.Outputs.SetExecuteCount(result.nExecuteCount);
			module.IsEmpty = false;
			if (isCallBack)
			{
				module.OnModuleResultCallBack(result, result.nExecuteCount);
				Tuple<VmModule, ValueEventArgs> data2 = new Tuple<VmModule, ValueEventArgs>(module, new ValueEventArgs
				{
					Value = result,
					ExecuteCount = result.nExecuteCount
				});
				instance.EnqueueForCallBack(data2);
			}
			if (result.bIsSelfExecute == 1 && !((IVmSolution)Instance).IsContinuousSelfExecute)
			{
				((IVmProcedure)module.Root)?.SetInnerModuleIsSelfRuning(false);
				module.IsRunning = false;
			}
		}
	}

	private static void CreateCallBackQueue()
	{
		callbackthread = new Thread(() =>
		{
			while (isCallBackThreadAlive)
			{
				if (callbackNumForReceiveStore > 0)
				{
					object[] array = callbackSendStore;
					lock (threadLock)
					{
						callbackSendStore = callbackReceiveStore;
						callbackNumForSendStore = callbackNumForReceiveStore;
						callbackNumForReceiveStore = 0;
						callbackReceiveStore = array;
					}
					for (int i = 0; i < callbackNumForSendStore; i++)
					{
						DequeueForCallBack(callbackSendStore[i]);
						callbackSendStore[i] = null;
					}
					callbackNumForSendStore = 0;
				}
				SpinWait.SpinUntil(() => false, 1);
			}
		})
		{
			IsBackground = true,
			Priority = ThreadPriority.BelowNormal
		};
		callbackthread.Start();
	}

	private static void DequeueForCallBack(object data)
	{
		if (data is Tuple<VmIO, ValueEventArgs> tuple)
		{
			tuple.Item1.OnValueChanged(tuple.Item2);
		}
		else if (data is Tuple<VmModule, ValueEventArgs> tuple2)
		{
			if (tuple2.Item2.Value is ImvsSdkDefine.IMVS_MODULE_WORK_STAUS)
			{
				(tuple2.Item1 as VmProcedure)?.OnAsyncWorkStatusCallBack(tuple2.Item2);
			}
			else
			{
				tuple2.Item1.OnAsyncModuleResultCallBackArrived(tuple2.Item2);
			}
		}
	}

	internal void EnqueueForCallBack(object data)
	{
		if (IsCallBackCacheEnable)
		{
			lock (threadLock)
			{
				if (callbackNumForReceiveStore < maxCallBackStoreLen)
				{
					callbackReceiveStore[callbackNumForReceiveStore] = data;
					callbackNumForReceiveStore++;
				}
				else
				{
					LogHelper.Error("CallBackStore overflow!!!");
				}
				return;
			}
		}
		DequeueForCallBack(data);
	}

	/// <summary>
	/// 扩大回调缓存队列长度（必须在程序未加载任何方案时调用）
	/// </summary>
	/// <param name="newLength"></param>
	public static void ExpandCallBackQueueLength(int newLength)
	{
		lock (threadLock)
		{
			if (newLength > maxCallBackStoreLen)
			{
				maxCallBackStoreLen = newLength;
				callbackNumForReceiveStore = 0;
				callbackNumForSendStore = 0;
				callbackReceiveStore = new object[maxCallBackStoreLen];
				callbackSendStore = new object[maxCallBackStoreLen];
			}
		}
	}

	/// <summary>
	/// 关闭回调缓存队列（必须在程序未加载任何方案时调用）
	/// </summary>
	public static void ShutDownCallBackQueue()
	{
		if (callbackthread != null)
		{
			isCallBackThreadAlive = false;
			callbackthread = null;
		}
	}

	/// <summary>
	/// 流程导入开始
	/// </summary>
	internal static void MProcessManager_OnProcessImportBeginCallBack(ImvsSdkDefine.IMVS_IMPORT_PROCESS_BEGIN_INFO processImportBeginInfo)
	{
		OnProcessImportBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = "Lang_ProcessImporting"
		});
	}

	/// <summary>
	/// 流程导入进度
	/// </summary>
	internal static void MProcessManager_OnProcessImportProgressCallBack(ImvsSdkDefine.IMVS_IMPORT_PROCESS_PROGRESS_INFO processImportProgressInfo)
	{
		OnProcessImportProgressCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = processImportProgressInfo.nProgress
		});
	}

	/// <summary>
	/// 流程导入UI数据
	/// </summary>
	internal static void MProcessManager_OnProcessImportUiDataCallBack(ImvsSdkDefine.IMVS_IMPORT_PROCESS_GET_UIDATA_INFO processImportUiDataInfo)
	{
		nProcessId = processImportUiDataInfo.nProcessId;
		strProcessName = Hik.Util.StringUtil.UTF8GetString(processImportUiDataInfo.strProcessName);
	}

	/// <summary>
	/// 流程导入结束
	/// </summary>
	internal static void MProcessManager_OnProcessImportEndCallBack(ImvsSdkDefine.IMVS_IMPORT_PROCESS_END_INFO processImportEndInfo)
	{
		EndEventArgs e = new EndEventArgs();
		e.ClientType = processImportEndInfo.nClientType;
		e.ErrorCode = processImportEndInfo.nErrorCode;
		if (processImportEndInfo.nErrorCode != 0)
		{
			e.BoxTitle = AppLocalizationService.GetLang("Lang_LoopAddErr");
		}
		OnProcessImportEndCallBack?.Invoke(SolObj, e);
		if (processImportEndInfo.nErrorCode == 0)
		{
			Instance.LoadGlobleModules(Instance.Modules);
			ImvsSdkDefine.IMVS_PROCESS_INFO proInfo = new ImvsSdkDefine.IMVS_PROCESS_INFO
			{
				nProcID = processImportEndInfo.nProcessId,
				strProcName = processImportEndInfo.strProcessName,
				nModuleType = 1u
			};
			Instance.importedProcedure = instance.AddModuleByInfo(proInfo, Instance.importCalibData);
			if (Instance.importCalibData != null)
			{
				Instance.importCalibData = null;
			}
		}
		mbImportProcedureEndFlag = true;
	}

	/// <summary>
	/// 流程导出开始
	/// </summary>
	internal static void MProcessManager_OnProcessExportBeginCallBack(ImvsSdkDefine.IMVS_EXPORT_PROCESS_BEGIN_INFO processExportBeginInfo)
	{
		OnProcessExportBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = UnRedoMaskMessage()
		});
	}

	/// <summary>
	/// 流程导出进度
	/// </summary>
	internal static void MProcessManager_OnProcessExportProgressCallBack(ImvsSdkDefine.IMVS_EXPORT_PROCESS_PROGRESS_INFO processExportProgressInfo)
	{
		OnProcessExportProgressCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = processExportProgressInfo.nProgress
		});
	}

	/// <summary>
	/// 流程导出结束
	/// </summary>
	internal static void MProcessManager_OnProcessExportEndCallBack(ImvsSdkDefine.IMVS_EXPORT_PROCESS_END_INFO processExportEndInfo)
	{
		EndEventArgs e = new EndEventArgs();
		e.ClientType = processExportEndInfo.nClientType;
		e.ErrorCode = processExportEndInfo.nErrorCode;
		if (processExportEndInfo.nErrorCode != 0)
		{
			e.BoxTitle = AppLocalizationService.GetLang("Lang_ProcessExportFail");
		}
		OnProcessExportEndCallBack?.Invoke(SolObj, e);
		mbExportProcedureEndFlag = true;
	}

	/// <summary>
	/// Group导入开始
	/// </summary>
	internal static void MModuleManager_OnImportGroupBeginCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_BEGIN_INFO groupImportBeginInfo)
	{
		OnGroupImportBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = AppLocalizationService.GetLang("Lang_GroupImporting")
		});
	}

	/// <summary>
	/// Group导入进度
	/// </summary>
	internal static void MModuleManager_OnImportGroupProcessCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_PROGRESS_INFO groupImportProcessInfo)
	{
		OnGroupImportProgressCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = groupImportProcessInfo.nProgress
		});
	}

	/// <summary>
	/// Group导入UI数据
	/// </summary>
	internal static void MModuleManager_OnImportGroupUiDataCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_GET_UIDATA_INFO groupImportUiDataInfo)
	{
		nGroupId = groupImportUiDataInfo.nGroupId;
		strGroupName = Hik.Util.StringUtil.UTF8GetString(groupImportUiDataInfo.strGroupName);
	}

	/// <summary>
	/// Group导入结束
	/// </summary>
	internal static void MModuleManager_OnImportGroupEndCallBack(ImvsSdkDefine.IMVS_IMPORT_GROUP_END_INFO groupImportEndInfo)
	{
		EndEventArgs e = new EndEventArgs();
		e.ClientType = groupImportEndInfo.nClientType;
		e.ErrorCode = groupImportEndInfo.nErrorCode;
		if (groupImportEndInfo.nErrorCode != 0)
		{
			e.BoxTitle = AppLocalizationService.GetLang("Lang_GroupImportError");
		}
		OnGroupImportEndCallBack?.Invoke(SolObj, e);
		if (groupImportEndInfo.nErrorCode == 0)
		{
			ImvsSdkDefine.IMVS_MODULE_INFO proInfo = new ImvsSdkDefine.IMVS_MODULE_INFO
			{
				nNodeID = groupImportEndInfo.nGroupNewId,
				nProcessID = groupImportEndInfo.nProcessId,
				strDisplayName = groupImportEndInfo.strGroupNewName,
				strModuleName = Hik.Util.StringUtil.UTF8GetBytesPadZero("IMVSGroup")
			};
			if (Instance != null && Instance.importGroupModuleSet != null)
			{
				Instance.importedGroupModule = (Instance.importGroupModuleSet as VmModuleSet).AddModuleByInfo(proInfo);
				if (Instance.importedGroupModule != null)
				{
					Instance.importedGroupModule.Parent = Instance.importGroupModuleSet as VmModuleSet;
					Instance.importedGroupModule.NotifyReadyStateToParent();
				}
			}
		}
		mbImportGroupEndFlag = true;
	}

	/// <summary>
	/// Gorup导出开始
	/// </summary>
	internal static void MModuleManager_OnExportGroupBeginCallBack(ImvsSdkDefine.IMVS_EXPORT_GROUP_BEGIN_INFO groupExportBeginInfo)
	{
		OnGroupExportBeginCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = "Group导出中"
		});
	}

	/// <summary>
	/// Group导出进度
	/// </summary>
	internal static void MModuleManager_OnExportGroupProcessCallBack(ImvsSdkDefine.IMVS_EXPORT_GROUP_PROGRESS_INFO groupExportProcessInfo)
	{
		OnGroupExportProgressCallBack?.Invoke(SolObj, new ObjectEventArgs
		{
			Param = groupExportProcessInfo.nProgress
		});
	}

	/// <summary>
	/// Group导出进度
	/// </summary>
	internal static void MModuleManager_OnExportGroupEndCallBack(ImvsSdkDefine.IMVS_EXPORT_GROUP_END_INFO groupExportEndInfo)
	{
		EndEventArgs e = new EndEventArgs();
		e.ClientType = groupExportEndInfo.nClientType;
		e.ErrorCode = groupExportEndInfo.nErrorCode;
		if (groupExportEndInfo.nErrorCode != 0)
		{
			e.BoxTitle = AppLocalizationService.GetLang("Lang_GroupExportFail");
		}
		OnGroupExportEndCallBack?.Invoke(SolObj, e);
	}

	void IVmSolution.InvokeModuleAddedEvent(IVmModule module)
	{
		OnModuleAddedEvent?.Invoke(module, null);
	}

	void IVmSolution.OnModuleActionChanged(string actionName, string moduleName, string newModuleName)
	{
		ModuleActionEvnetChangedCallBack?.Invoke(this, new ModuleActionEventArgs
		{
			ActionName = actionName,
			ModuleName = moduleName,
			NewModuleName = newModuleName
		});
	}
}
