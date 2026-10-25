using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Apps.Data;
using Apps.ErrorCode;
using Apps.Localization;
using Apps.Log;
using Hik.Util;
using HikExternalCall.Common;
using VM.PlatformSDKCS;
using VM.Utility;
using VM.Utility.SingleInstance;
using VMControls.BaseInterface;
using VMControls.Interface;
using VMControls.Interface.CustomAttribute;

namespace VM.Core;

/// <summary>
/// VmModule3D
/// </summary>
public class VmModule : VmFrameTreeBase, IVmModule, IVmFrameBase, IParent, IDisposable, IVmExternPath, IUserData, IVMRun, IParamsConfig, IUserStringData, IUserBytesData, ICatchCoreInfo, IParamsOperator
{
	private struct IMVS_PATMATCH_POINT_INFO
	{
		public float MatchOutlineX;

		public float MatchOutlineY;

		public float MatchOutlineScore;

		public int MatchOutlineIndex;
	}

	public delegate void RenameDelegate(string oldName, string newName);

	private const string DYNAMIC_OUT_STR = "DynamicOutputParam";

	private const string DYNAMIC_IN_STR = "DynamicInputParam";

	private const string DYNAMIC_DISPLAY_STR = "DynamicDisplayParam";

	public static readonly string[] fTimeStamp = new string[4] { "fArrivalTimeStampLow", "fArrivalTimeStampHigh", "fLeaveTimeStampLow", "fLeaveTimeStampHigh" };

	internal string _StrModuleName;

	private string _ModuleDir;

	protected bool? _IsReady;

	private bool? isRunOK;

	private ImvsSdkDefine.IMVS_MODULE_INFO moduleInfo;

	internal Dictionary<int, VmIO> allIos = new Dictionary<int, VmIO>();

	internal Dictionary<int, VmIO> leafIOsWithoutTimeStamp = new Dictionary<int, VmIO>();

	internal Dictionary<int, VmIO> leafIOs = new Dictionary<int, VmIO>();

	private bool nextAndPreModules_IsLoaded = false;

	internal bool relateIO_IsLoaded = false;

	private int SubscribeCount = 0;

	internal bool isForbidden = false;

	internal bool bInitForbidden = true;

	private VmProcedure root = null;

	internal bool? isLoadFailed = false;

	internal bool initLoadState = false;

	internal int isSetCallBack = 0;

	protected internal bool disposedValue = false;

	internal bool IsOnDispose = false;

	private Dictionary<string, object> ExternPathStartNodes = new Dictionary<string, object>();

	/// <summary>
	/// 用户数据，UserData[KeyName] = null时，删除该key
	/// </summary>
	private UserData _userData = new UserData();

	/// <summary>
	/// CH: 内部参数 | EN: Internal Param
	/// </summary>
	private ModuleParam _params = new ModuleParam();

	private bool outputs_IsLoaded = false;

	private bool inputs_IsLoaded = false;

	protected bool IsParastic = false;

	protected VmModule HostModule = null;

	internal uint executeCount;

	private float moduleTime;

	private float algorithmTime;

	private bool configAvailable = true;

	internal VmModuleCollection<VmModule> _NextModules = new VmModuleCollection<VmModule>();

	private ObservableCollection<IndexData> _SourceIndexList = new ObservableCollection<IndexData>();

	private ObservableCollection<IndexData> _TargetIndexList = new ObservableCollection<IndexData>();

	private bool reportResultEnable = false;

	private object position;

	internal bool isRunning = false;

	private static Dictionary<uint, uint> HasInserted = new Dictionary<uint, uint>();

	private const string GUIDKey = "GUID";

	/// <summary>
	/// @~chinese 模块结果内存管理类型 @~english Module Result Memory Type
	/// </summary>
	public ModuResultMemoryTypeEnum ModuResultMemType = ModuResultMemoryTypeEnum.Memory_Deep_Copy;

	private int SyncBindingFlag = 0;

	private int MaxConnecion { get; set; } = int.MaxValue;

	internal bool IsActive { get; set; }

	public VmProcedure Root
	{
		get
		{
			if (root == null)
			{
				object obj = Parent;
				while (obj != null && !(obj is VmProcedure))
				{
					obj = (obj as VmFrameTreeBase)?.Parent;
				}
				root = obj as VmProcedure;
			}
			return root;
		}
	}

	/// <summary>
	/// 模块id
	/// </summary>
	public virtual uint ID
	{
		get
		{
			return moduleInfo.nNodeID;
		}
		set
		{
			if (value != moduleInfo.nNodeID)
			{
				moduleInfo.nNodeID = value;
			}
		}
	}

	public override string GUID => guid;

	/// <summary>
	/// 模块显示名称
	/// </summary>
	public override string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (!(name != value))
			{
				return;
			}
			ServerSDKManager.serverSDKManager.mModuleManager.SetModuDisplayName(ID, value);
			string fullName = FullName;
			string text = name;
			name = value;
			foreach (VmModule preModule in PreModules)
			{
				byte[] value2 = preModule.UserData["SourceIndex" + text];
				byte[] value3 = preModule.UserData["TargetIndex" + text];
				preModule.UserData["SourceIndex" + text] = null;
				preModule.UserData["TargetIndex" + text] = null;
				preModule.UserData["SourceIndex" + name] = value2;
				preModule.UserData["TargetIndex" + name] = value3;
			}
			NotifyNameChangedToParent(text);
			OnPropertyChanged("Name");
			((IVmSolution)VmSolution.Instance).OnModuleActionChanged("ReName", fullName, FullName);
		}
	}

	public override string FullName => base.FullName;

	public override string FullGUID => base.FullGUID;

	/// <summary>
	/// 模块名称
	/// </summary>
	public virtual string StrModuleName
	{
		get
		{
			if (_StrModuleName == null)
			{
				_StrModuleName = ((moduleInfo.strModuleName == null) ? null : Hik.Util.StringUtil.UTF8GetString(moduleInfo.strModuleName));
			}
			return _StrModuleName;
		}
		set
		{
			this.SetProperty(ref _StrModuleName, value, "StrModuleName");
		}
	}

	/// <summary>
	/// 模块订阅是否已完成
	/// </summary>
	public virtual bool? IsReady
	{
		get
		{
			if (!_IsReady.HasValue)
			{
				byte[] array = UserData["IsReady"];
				if (array != null)
				{
					_IsReady = array[0] == 1;
				}
			}
			return _IsReady;
		}
		set
		{
			if (_IsReady != value)
			{
				_IsReady = value;
				if (!value.HasValue)
				{
					UserData["IsReady"] = null;
				}
				else
				{
					UserData["IsReady"] = ((!_IsReady.Value) ? new byte[1] : new byte[1] { 1 });
				}
				NotifyReadyStateToParent();
				OnPropertyChanged("IsReady");
			}
		}
	}

	/// <summary>
	/// 执行是否ok
	/// </summary>
	public bool? IsRunOK
	{
		get
		{
			return isRunOK;
		}
		set
		{
			this.SetProperty(ref isRunOK, value, "IsRunOK");
		}
	}

	public virtual UserData UserData => _userData;

	private UserData InnerUserData { get; } = new UserData
	{
		isUsedForDynamicUIParam = true
	};

	IUserData IVmModule.InnerUserData => InnerUserData;

	public ModuleParam Params => _params;

	/// <summary>
	/// 模块运行参数管理
	/// </summary>
	public VmParamSubManager ParamSubManager { get; private set; }

	private VmIOCollection _Inputs { get; set; } = new VmIOCollection();

	private VmIOCollection _Outputs { get; set; } = new VmIOCollection();

	public virtual VmIOCollection Outputs
	{
		get
		{
			if (!outputs_IsLoaded)
			{
				if (!VmSolution.IsLoading)
				{
					outputs_IsLoaded = true;
					InitOutputDynamicIO();
				}
				else
				{
					LogHelper.Error("未加载完成方案，禁止访问模块输入输出" + new StackTrace().ToString());
				}
			}
			return _Outputs;
		}
		set
		{
			_Outputs = value;
		}
	}

	public virtual VmIOCollection Inputs
	{
		get
		{
			if (!inputs_IsLoaded)
			{
				if (!VmSolution.IsLoading)
				{
					inputs_IsLoaded = true;
					InitInputDynamicIO();
				}
				else
				{
					LogHelper.Error("未加载完成方案，禁止访问模块输入输出" + new StackTrace().ToString());
				}
			}
			return _Inputs;
		}
		set
		{
			_Inputs = value;
		}
	}

	public virtual bool IsContainsModel => false;

	public virtual bool IsGlobalModu => false;

	public virtual bool IsDynamicModu => false;

	public virtual bool IsConfigTriggerSaveModel => false;

	public virtual bool NeedConfirm { get; set; } = false;

	/// <summary>
	/// 是否叠加循环内的值
	/// </summary>
	public virtual bool NeedMergeValueInLoop => true;

	public virtual bool IsImportProcess => false;

	/// <summary>
	/// 流程执行次数
	/// </summary>
	public virtual uint ExecuteCount
	{
		get
		{
			return executeCount;
		}
		set
		{
			this.SetProperty(ref executeCount, value, "ExecuteCount");
		}
	}

	/// <summary>
	/// 模块耗时
	/// </summary>
	public float ModuleTime
	{
		get
		{
			return moduleTime;
		}
		set
		{
			this.SetProperty(ref moduleTime, value, "ModuleTime");
		}
	}

	/// <summary>
	/// 流程执行时间
	/// </summary>
	public virtual float ProcessTime
	{
		get
		{
			if (((IVmFrameBase)this).GetParent<VmProcedure>() != null)
			{
				return ((IVmFrameBase)this).GetParent<VmProcedure>().ProcessTime;
			}
			return 0f;
		}
		internal set
		{
			OnPropertyChanged("ProcessTime");
		}
	}

	/// <summary>
	/// 算法耗时
	/// </summary>
	public float AlgorithmTime
	{
		get
		{
			return algorithmTime;
		}
		set
		{
			this.SetProperty(ref algorithmTime, value, "AlgorithmTime");
		}
	}

	/// <summary>
	/// 权限
	/// 打开配置界面
	/// </summary>
	public bool ConfigAvailable
	{
		get
		{
			try
			{
				byte[] array = UserData["ConfigAvailable"];
				if (array != null)
				{
					configAvailable = ExternalCallHelper.BytesToStruct<bool>(array, 0, array.Length);
				}
			}
			catch (VmException ex)
			{
				string text = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
				text = text + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
				LogHelper.Error("ConfigAvailable get error:" + text);
				return false;
			}
			return configAvailable;
		}
		set
		{
			if (value != configAvailable)
			{
				this.SetProperty(ref configAvailable, value, "ConfigAvailable");
				try
				{
					UserData["ConfigAvailable"] = ExternalCallHelper.StructToBytes(value);
				}
				catch (VmException ex)
				{
					string text = AppLocalizationService.GetLang("errCode:") + $"{ex.errorCode:X}" + "\r\n";
					text = text + AppLocalizationService.GetLang("errInfo:") + ErrorCodeInfo.GetErrorInfo(ex.errorCode);
					LogHelper.Error("ConfigAvailable set error:" + text);
				}
			}
		}
	}

	/// <summary>
	/// 配置界面打开前检查，密码校验或者模块需要打开前初始化的内容
	/// </summary>
	public virtual bool ConfigCheck => true;

	internal VmModuleCollection<VmModule> NextModules
	{
		get
		{
			if (!nextAndPreModules_IsLoaded)
			{
				nextAndPreModules_IsLoaded = true;
				LoadNextAndPreModules();
			}
			return _NextModules;
		}
		set
		{
			this.SetProperty(ref _NextModules, value, "NextModules");
		}
	}

	private VmModuleCollection<VmModule> _PreModules { get; set; } = new VmModuleCollection<VmModule>();

	internal VmModuleCollection<VmModule> PreModules
	{
		get
		{
			if (!nextAndPreModules_IsLoaded)
			{
				nextAndPreModules_IsLoaded = true;
				LoadNextAndPreModules();
			}
			return _PreModules;
		}
		set
		{
			this.SetProperty(ref _NextModules, value, "PreModules");
		}
	}

	internal ObservableCollection<IndexData> SourceIndexList => _SourceIndexList;

	internal ObservableCollection<IndexData> TargetIndexList => _TargetIndexList;

	/// <summary>
	/// 结果回调使能
	/// </summary>
	public virtual bool ReportResultEnable
	{
		get
		{
			return reportResultEnable;
		}
		internal set
		{
			if (IsActive && reportResultEnable != value)
			{
				if (value)
				{
					RefreshResult();
				}
				reportResultEnable = value;
				((IVmSolution)VmSolution.Instance).ResetAllModulesCallBackStatus();
			}
		}
	}

	/// <summary>
	/// 是否加载失败
	/// </summary>
	public virtual bool? IsLoadFailed
	{
		get
		{
			if (!initLoadState)
			{
				try
				{
					uint nEnableFlag = 1u;
					ServerSDKManager.serverSDKManager.mModuleManager.GetModuleFailedState(ID, ref nEnableFlag);
					switch (nEnableFlag)
					{
					case 1u:
						isLoadFailed = false;
						break;
					case 0u:
						isLoadFailed = true;
						break;
					case 2u:
						isLoadFailed = null;
						break;
					}
				}
				catch (VmException ex)
				{
					LogHelper.Error(FullName + " Get IsForbidden Error: " + ex.errorMessage);
				}
				catch (Exception ex2)
				{
					LogHelper.Error(FullName + " Get IsForbidden Error: " + ex2.Message);
				}
				initLoadState = true;
			}
			return isLoadFailed;
		}
		set
		{
			if (!IsActive)
			{
				OnPropertyChanged("IsLoadFailed");
			}
			else if (isLoadFailed != value)
			{
				isLoadFailed = value;
				NotifyReadyStateToParent();
				OnPropertyChanged("IsLoadFailed");
			}
		}
	}

	/// <summary>
	/// 是否禁用
	/// </summary>
	public virtual bool IsForbidden
	{
		get
		{
			if (bInitForbidden)
			{
				try
				{
					uint nEnableFlag = 1u;
					ServerSDKManager.serverSDKManager.mModuleManager.GetModuleState(ID, ref nEnableFlag);
					isForbidden = nEnableFlag == 0;
				}
				catch (VmException ex)
				{
					LogHelper.Error(FullName + " Get IsForbidden Error: " + ex.errorMessage);
				}
				catch (Exception ex2)
				{
					LogHelper.Error(FullName + " Get IsForbidden Error: " + ex2.Message);
				}
				bInitForbidden = false;
			}
			return isForbidden;
		}
		set
		{
			if (!IsActive)
			{
				OnPropertyChanged("IsForbidden");
			}
			else if (isForbidden != value)
			{
				try
				{
					ServerSDKManager.serverSDKManager.mModuleManager.EnableModule(ID, (!value) ? 1u : 0u);
					isForbidden = value;
					NotifyReadyStateToParent();
					OnPropertyChanged("IsForbidden");
				}
				catch (VmException ex)
				{
					LogHelper.Error(FullName + " Set IsForbidden Error: " + ex.errorMessage);
				}
				catch (Exception ex2)
				{
					LogHelper.Error(FullName + " Set IsForbidden Error: " + ex2.Message);
				}
			}
		}
	}

	/// <summary>
	/// 模块位置
	/// </summary>
	public virtual object Position
	{
		get
		{
			return position;
		}
		set
		{
			if (!GetEqualPoint(position, value))
			{
				position = value;
				OnPropertyChanged("Position");
			}
		}
	}

	/// <summary>
	/// 执行标志位
	/// </summary>
	public virtual bool IsRunning
	{
		get
		{
			return isRunning;
		}
		internal set
		{
			if (this.SetProperty(ref isRunning, value, "IsRunning"))
			{
				(this as VmModuleSet)?.SetGlobalRunState(value);
				(Parent as ModelBase)?.OnPropertyChanged("IsRunning");
				if (!(Parent is VmSolution))
				{
					VmSolution.Instance.OnPropertyChanged("IsRunning");
				}
			}
		}
	}

	/// <summary>
	/// 连续运行 | 停止执行标志
	/// </summary>
	public virtual bool ContinuousRunEnable
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public override string UniqueName => Name;

	public override string UniqueGUID => GUID;

	uint IVmModule.ModuleType => moduleInfo.nModuleType;

	bool IVMRun.RunEnable
	{
		get
		{
			if (((IVmFrameBase)this).GetParent<IVmProcedure>() is VmIndependentProcedure)
			{
				return false;
			}
			return true;
		}
	}

	bool IVMRun.IsRunning
	{
		get
		{
			int result;
			if (!isRunning)
			{
				VmProcedure vmProcedure = Root;
				if (vmProcedure == null || !vmProcedure.IsRunning)
				{
					result = (((IVmSolution)VmSolution.Instance).IsContinuousSelfExecute ? 1 : 0);
					goto IL_002a;
				}
			}
			result = 1;
			goto IL_002a;
			IL_002a:
			return (byte)result != 0;
		}
	}

	/// <summary>
	/// 模块所在路径
	/// </summary>
	string IVmModule.ModuleDir
	{
		get
		{
			if (_ModuleDir == null)
			{
				if (VmSolution.ModulePathList != null && VmSolution.ModulePathList.ContainsKey(StrModuleName))
				{
					_ModuleDir = VmSolution.ModulePathList[StrModuleName];
				}
				else
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(CurrentPathHelper.Instance.GetCurrentPath() + "\\Module(sp)\\x64");
					DirectoryInfo[] directories = directoryInfo.GetDirectories();
					foreach (DirectoryInfo directoryInfo2 in directories)
					{
						if ((from p in directoryInfo2.GetDirectories()
							select p.Name).Contains(StrModuleName))
						{
							_ModuleDir = $"{directoryInfo2.FullName}\\{StrModuleName}";
							VmSolution.ModulePathList?.TryAdd(StrModuleName, _ModuleDir);
							break;
						}
					}
				}
			}
			return _ModuleDir;
		}
	}

	IUserData IVmModule.UserData => UserData;

	IParamsConfig IVmModule.Params => Params;

	bool IVmModule.IsActive
	{
		get
		{
			return IsActive;
		}
		set
		{
			IsActive = value;
		}
	}

	/// <summary>
	/// 最大连接数
	/// </summary>
	int IVmModule.MaxConnecion => MaxConnecion;

	IVmModuleSet IVmModule.Root => Root;

	IList IVmModule.Inputs => Inputs;

	IList IVmModule.Outputs => Outputs;

	IList IVmModule.SourceIndexs => SourceIndexList;

	IList IVmModule.TargetIndexs => TargetIndexList;

	IList IVmModule.PreModules => PreModules;

	IList IVmModule.NextModules => NextModules;

	string IVmModule.ModuleTypeName => StrModuleName ?? (GetType().Name.EndsWith("Tool") ? GetType().Name.Remove(GetType().Name.Length - 4) : GetType().Name);

	int IVmModule.SubscribeCount
	{
		get
		{
			return SubscribeCount;
		}
		set
		{
			if (SubscribeCount != value)
			{
				int subscribeCount = SubscribeCount;
				SubscribeCount = value;
				if (subscribeCount == 0 && SubscribeCount == 1)
				{
					ReportResultEnable = true;
				}
				if (subscribeCount == 1 && SubscribeCount == 0)
				{
					ReportResultEnable = false;
				}
			}
		}
	}

	bool IVmModule.IsOnDispose => IsOnDispose;

	public IList FrontModules => GetFrontParamItems();

	byte[] IUserData.this[string key]
	{
		get
		{
			return UserData?[key];
		}
		set
		{
			UserData[key] = value;
		}
	}

	public bool IsEmpty { get; set; } = true;

	protected internal virtual bool IsGression => false;

	/// <summary>
	/// 模块保存加载路径
	/// </summary>
	public string ModuleFilePath { get; internal set; }

	/// <summary>
	/// 模块密码(内部使用)
	/// </summary>
	internal string ModuleFilePassword { get; set; }

	Dictionary<string, byte[]> IParamsOperator.ParamsDictionary { get; set; } = new Dictionary<string, byte[]>();

	string IParamsConfig.XmlPath
	{
		get
		{
			return Params.XmlPath;
		}
		set
		{
			Params.XmlPath = value;
		}
	}

	uint IParamsConfig.TimeOut
	{
		get
		{
			return Params.TimeOut;
		}
		set
		{
			Params.TimeOut = value;
		}
	}

	byte[] IUserBytesData.this[string key]
	{
		get
		{
			return ((IUserBytesData)Params)[key];
		}
		set
		{
			((IUserBytesData)Params)[key] = value;
		}
	}

	string IUserStringData.this[string key]
	{
		get
		{
			return ((IUserStringData)Params)[key];
		}
		set
		{
			((IUserStringData)Params)[key] = value;
		}
	}

	/// <summary>
	/// 模块结果回调（EventArgs为VMControls.Interface.ValueEventArgs类型，其中Value属性为模块结果结构体ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data）
	/// </summary>
	public event EventHandler ModuleResultCallBackArrived;

	private event EventHandler asyncModuleResultCallBackArrived;

	/// <summary>
	/// 异步回调，VM内部渲染使用
	/// </summary>
	event EventHandler IVmModule.AsyncModuleResultCallBackArrived
	{
		add
		{
			asyncModuleResultCallBackArrived += value;
		}
		remove
		{
			asyncModuleResultCallBackArrived -= value;
		}
	}

	public event EventHandler ModuleParamChanged;

	public IVmRangeImageData GetVmIORangeImageValue(VmIO io)
	{
		if (io.TypeName == "IMAGE")
		{
			VmDataType.VmRangeImageData vmRangeImageData = new VmDataType.VmRangeImageData();
			object obj = (io.Childs[0].Value as Array)?.GetValue(0);
			if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
			{
				vmRangeImageData.rangeData = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(out var lenth);
				vmRangeImageData.rangeDataLenth = lenth;
				string intensityImageIoName = GetIntensityImageIoName(io.Parent as VmIO);
				vmRangeImageData.intensityImageData = GetVmIOImageValue(intensityImageIoName) as VmDataType.VmImageData;
				vmRangeImageData.rangeWidth = (int)(io.Childs[1].Value as Array).GetValue(0);
				vmRangeImageData.rangeHeight = (int)(io.Childs[2].Value as Array).GetValue(0);
				int rangeImageFormat = (int)(io.Childs[3].Value as Array).GetValue(0);
				vmRangeImageData.rangeImageFormat = rangeImageFormat;
				VmIO childIoByName;
				if ((childIoByName = GetChildIoByName(io.Parent as VmIO, ImageSource3DModuleType.RangeAttribute.ToString())) != null)
				{
					vmRangeImageData.xoffset = (int)(childIoByName.Childs[0].Value as Array).GetValue(0);
					vmRangeImageData.yoffset = (int)(childIoByName.Childs[1].Value as Array).GetValue(0);
					vmRangeImageData.zoffset = (int)(childIoByName.Childs[2].Value as Array).GetValue(0);
					vmRangeImageData.xscale = (float)(childIoByName.Childs[3].Value as Array).GetValue(0);
					vmRangeImageData.yscale = (float)(childIoByName.Childs[4].Value as Array).GetValue(0);
					vmRangeImageData.zscale = (float)(childIoByName.Childs[5].Value as Array).GetValue(0);
				}
				return vmRangeImageData;
			}
			return null;
		}
		throw ThrowTypeOfIOError();
	}

	/// <summary>
	/// 获取IO3D图像值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVm3DImageData GetVmIO3DImageValue(string vmIOName)
	{
		if (GetIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "IMAGE3D")
			{
				VmDataType.Vm3DImageData vm3DImageData = new VmDataType.Vm3DImageData();
				int num = (int)(io.Childs[0].Value as Array)?.GetValue(0);
				int num2 = num;
				if (num2 == 7)
				{
					vm3DImageData.rangeImageData = GetVmIORangeImageValue(io.Childs[1].Name) as VmDataType.VmRangeImageData;
					return vm3DImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 获取IO深度图像值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmRangeImageData GetVmIORangeImageValue(string vmIOName)
	{
		if (GetIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "IMAGE")
			{
				VmDataType.VmRangeImageData vmRangeImageData = new VmDataType.VmRangeImageData();
				object obj = (io.Childs[0].Value as Array)?.GetValue(0);
				if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
				{
					if (!string.IsNullOrEmpty(iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryAddress))
					{
						iMVS_MODULE_CHUNK_VALUE_EX_Data.IsMemory = true;
					}
					vmRangeImageData.rangeData = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(out var lenth);
					vmRangeImageData.rangeDataLenth = lenth;
					string intensityImageIoName = GetIntensityImageIoName(io.Parent as VmIO);
					vmRangeImageData.intensityImageData = GetVmIOImageValue(intensityImageIoName) as VmDataType.VmImageData;
					vmRangeImageData.rangeWidth = (int)(io.Childs[1].Value as Array).GetValue(0);
					vmRangeImageData.rangeHeight = (int)(io.Childs[2].Value as Array).GetValue(0);
					int rangeImageFormat = (int)(io.Childs[3].Value as Array).GetValue(0);
					vmRangeImageData.rangeImageFormat = rangeImageFormat;
					VmIO childIoByName;
					if ((childIoByName = GetChildIoByName(io.Parent as VmIO, ImageSource3DModuleType.RangeAttribute.ToString())) != null)
					{
						vmRangeImageData.xoffset = (int)(childIoByName.Childs[0].Value as Array).GetValue(0);
						vmRangeImageData.yoffset = (int)(childIoByName.Childs[1].Value as Array).GetValue(0);
						vmRangeImageData.zoffset = (int)(childIoByName.Childs[2].Value as Array).GetValue(0);
						vmRangeImageData.xscale = (float)(childIoByName.Childs[3].Value as Array).GetValue(0);
						vmRangeImageData.yscale = (float)(childIoByName.Childs[4].Value as Array).GetValue(0);
						vmRangeImageData.zscale = (float)(childIoByName.Childs[5].Value as Array).GetValue(0);
					}
					return vmRangeImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 获取IO RGB图像值
	/// </summary>
	/// <param name="vmIOName"></param>
	/// <returns></returns>
	public VmDataType.RgbdImageData GetVmIORgbdImageValue(string vmIOName)
	{
		if (GetIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "IMAGE")
			{
				VmDataType.RgbdImageData rgbdImageData = new VmDataType.RgbdImageData();
				object obj = (io.Childs[0].Value as Array)?.GetValue(0);
				if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
				{
					rgbdImageData.RangeData = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(out var lenth);
					rgbdImageData.RangeDataLenth = lenth;
					rgbdImageData.RangeWidth = (int)(io.Childs[1].Value as Array).GetValue(0);
					rgbdImageData.RangeHeight = (int)(io.Childs[2].Value as Array).GetValue(0);
					int rangeImageFormat = (int)(io.Childs[3].Value as Array).GetValue(0);
					rgbdImageData.RangeImageFormat = rangeImageFormat;
					VmIO childIoByName;
					if ((childIoByName = GetChildIoByName(io.Parent as VmIO, ImageSource3DModuleType.RangeAttribute.ToString())) != null)
					{
						rgbdImageData.Xoffset = (int)(childIoByName.Childs[0].Value as Array).GetValue(0);
						rgbdImageData.Yoffset = (int)(childIoByName.Childs[1].Value as Array).GetValue(0);
						rgbdImageData.Zoffset = (int)(childIoByName.Childs[2].Value as Array).GetValue(0);
						rgbdImageData.Xscale = (float)(childIoByName.Childs[3].Value as Array).GetValue(0);
						rgbdImageData.Yscale = (float)(childIoByName.Childs[4].Value as Array).GetValue(0);
						rgbdImageData.Zscale = (float)(childIoByName.Childs[5].Value as Array).GetValue(0);
					}
					rgbdImageData.RgbImageData = GetVmIOImageValue(ImageSource3DModuleType.RGBDRgbImageData.ToString()) as VmDataType.VmImageData;
					rgbdImageData.CameraIntrinsicParam = new VmDataType.CameraIntrinsicParam
					{
						Depth2rgbMatrix = new double[16],
						RgbCamMatrix = new double[9],
						DepthDisCoeffs = new double[5],
						RgbDisCoeffs = new double[5]
					};
					if (GetIOByName("Depth2Rgb", out var io2))
					{
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[0] = (double)(io2.Childs[0].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[1] = (double)(io2.Childs[1].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[2] = (double)(io2.Childs[2].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[3] = (double)(io2.Childs[3].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[4] = (double)(io2.Childs[4].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[5] = (double)(io2.Childs[5].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[6] = (double)(io2.Childs[6].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[7] = (double)(io2.Childs[7].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[8] = (double)(io2.Childs[8].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[9] = (double)(io2.Childs[9].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[10] = (double)(io2.Childs[10].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[11] = (double)(io2.Childs[11].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[12] = (double)(io2.Childs[12].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[13] = (double)(io2.Childs[13].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[14] = (double)(io2.Childs[14].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.Depth2rgbMatrix[15] = (double)(io2.Childs[15].Value as Array).GetValue(0);
					}
					if (GetIOByName("RgbCamMatrix", out var io3))
					{
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[0] = (double)(io3.Childs[0].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[1] = (double)(io3.Childs[1].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[2] = (double)(io3.Childs[2].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[3] = (double)(io3.Childs[3].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[4] = (double)(io3.Childs[4].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[5] = (double)(io3.Childs[5].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[6] = (double)(io3.Childs[6].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[7] = (double)(io3.Childs[7].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbCamMatrix[8] = (double)(io3.Childs[8].Value as Array).GetValue(0);
					}
					if (GetIOByName("RgbDisCoeffs", out var io4))
					{
						rgbdImageData.CameraIntrinsicParam.RgbDisCoeffs[0] = (double)(io4.Childs[0].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbDisCoeffs[1] = (double)(io4.Childs[1].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbDisCoeffs[2] = (double)(io4.Childs[2].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbDisCoeffs[3] = (double)(io4.Childs[3].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.RgbDisCoeffs[4] = (double)(io4.Childs[4].Value as Array).GetValue(0);
					}
					if (GetIOByName("DepthDisCoeffs", out var io5))
					{
						rgbdImageData.CameraIntrinsicParam.DepthDisCoeffs[0] = (double)(io5.Childs[0].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.DepthDisCoeffs[1] = (double)(io5.Childs[1].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.DepthDisCoeffs[2] = (double)(io5.Childs[2].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.DepthDisCoeffs[3] = (double)(io5.Childs[3].Value as Array).GetValue(0);
						rgbdImageData.CameraIntrinsicParam.DepthDisCoeffs[4] = (double)(io5.Childs[4].Value as Array).GetValue(0);
					}
					return rgbdImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 获取IO 点云图数据
	/// </summary>
	/// <param name="PointcloudParam"></param>
	/// <returns></returns>
	public PointCloudImageData GetVmIOPointCloudImageValue(GetIOPointCloudDataParam PointcloudParam)
	{
		ImageData imageData = new PointCloudImageData();
		ImageData imageData2 = new ImageData();
		if (GetIOByName(PointcloudParam.PcdInputImageData, out var io))
		{
			if (!(io.TypeName == "IMAGE"))
			{
				throw ThrowTypeOfIOError();
			}
			object obj = (io.Childs[0].Value as Array)?.GetValue(0);
			if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
			{
				PointcloudParam.PointMemoryPtr = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(out var lenth);
				if (!string.IsNullOrEmpty(iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryAddress))
				{
					imageData2.MemoryAddress = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetRealMemoryAddress();
				}
				imageData2.ImageBuffer = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData();
				imageData2.Size = lenth;
				imageData2.NeedConvertColor = true;
				imageData2.Width = (int)(io.Childs[1].Value as Array).GetValue(0);
				imageData2.Height = (int)(io.Childs[2].Value as Array).GetValue(0);
				int num = (int)(io.Childs[3].Value as Array).GetValue(0);
				imageData2.PixelFormat = ImageData.GetPixelFormatByProtocol((uint)num);
				imageData2.DetailedPixelFormat = (uint)num;
			}
		}
		ImageData imageData3 = new ImageData();
		if (GetIOByName(PointcloudParam.NormalsInputImageData, out var io2))
		{
			if (!(io2.TypeName == "IMAGE"))
			{
				throw ThrowTypeOfIOError();
			}
			object obj2 = (io2.Childs[0].Value as Array)?.GetValue(0);
			if (obj2 is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data2)
			{
				PointcloudParam.NormalMemoryPtr = iMVS_MODULE_CHUNK_VALUE_EX_Data2.GetData(out var lenth2);
				if (!string.IsNullOrEmpty(iMVS_MODULE_CHUNK_VALUE_EX_Data2.MemoryAddress))
				{
					imageData3.MemoryAddress = iMVS_MODULE_CHUNK_VALUE_EX_Data2.GetRealMemoryAddress();
				}
				imageData3.ImageBuffer = iMVS_MODULE_CHUNK_VALUE_EX_Data2.GetData();
				imageData3.Size = lenth2;
				imageData3.NeedConvertColor = true;
				imageData3.Width = (int)(io2.Childs[1].Value as Array).GetValue(0);
				imageData3.Height = (int)(io2.Childs[2].Value as Array).GetValue(0);
				int num2 = (int)(io2.Childs[3].Value as Array).GetValue(0);
				imageData3.PixelFormat = ImageData.GetPixelFormatByProtocol((uint)num2);
				imageData3.DetailedPixelFormat = (uint)num2;
			}
		}
		PointcloudParam.PcdExtInfo = new PcdExtinfoF32();
		if (GetIOByName(PointcloudParam.ExtInfo, out var io3))
		{
			if (!(io3.TypeName == "PCDEXTINFO"))
			{
				throw ThrowTypeOfIOError();
			}
			foreach (VmIO child in io3.Childs)
			{
				if (child.Name.Equals("ExtInfoData"))
				{
					PointcloudParam.PcdExtInfo.CoorType = Convert.ToInt32(GetValue(child.Childs[0].Value));
					PointcloudParam.PcdExtInfo.flag_size = Convert.ToInt32(GetValue(child.Childs[1].Value));
					PointcloudParam.PcdExtInfo.length = (float)GetValue(child.Childs[2].Value);
					PointcloudParam.PcdExtInfo.width = (float)GetValue(child.Childs[3].Value);
					PointcloudParam.PcdExtInfo.flag_height = Convert.ToInt32(GetValue(child.Childs[4].Value));
					PointcloudParam.PcdExtInfo.height = (float)GetValue(child.Childs[5].Value);
					PointcloudParam.PcdExtInfo.flag_area = Convert.ToInt32(GetValue(child.Childs[6].Value));
					PointcloudParam.PcdExtInfo.area = (float)GetValue(child.Childs[7].Value);
					PointcloudParam.PcdExtInfo.flag_density = Convert.ToInt32(GetValue(child.Childs[8].Value));
					PointcloudParam.PcdExtInfo.density = (float)GetValue(child.Childs[9].Value);
					PointcloudParam.PcdExtInfo.flag_volume = Convert.ToInt32(GetValue(child.Childs[10].Value));
					PointcloudParam.PcdExtInfo.volume = (float)GetValue(child.Childs[11].Value);
				}
				if (child.Name.Equals("FlagCenter"))
				{
					PointcloudParam.PcdExtInfo.flag_center = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("Center"))
				{
					PointcloudParam.PcdExtInfo.center.X = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.center.Y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.center.Z = (float)GetValue(child.Childs[2].Value);
				}
				if (child.Name.Equals("FlagCvx"))
				{
					PointcloudParam.PcdExtInfo.flag_cvx = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("CvxHull"))
				{
					PointcloudParam.PcdExtInfo.cvx_hull[0].X = (float)GetValue(child.Childs[0].Childs[0].Value);
					PointcloudParam.PcdExtInfo.cvx_hull[0].Y = (float)GetValue(child.Childs[0].Childs[1].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[0].Z = (float)GetValue(child.Childs[0].Childs[2].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[1].X = (float)GetValue(child.Childs[0].Childs[3].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[1].Y = (float)GetValue(child.Childs[0].Childs[4].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[1].Z = (float)GetValue(child.Childs[0].Childs[5].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[2].X = (float)GetValue(child.Childs[0].Childs[6].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[2].Y = (float)GetValue(child.Childs[0].Childs[7].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[2].Z = (float)GetValue(child.Childs[0].Childs[8].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[3].X = (float)GetValue(child.Childs[0].Childs[9].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[3].Y = (float)GetValue(child.Childs[0].Childs[10].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[3].Z = (float)GetValue(child.Childs[0].Childs[11].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[4].X = (float)GetValue(child.Childs[0].Childs[12].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[4].Y = (float)GetValue(child.Childs[0].Childs[13].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[4].Z = (float)GetValue(child.Childs[0].Childs[14].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[5].X = (float)GetValue(child.Childs[0].Childs[15].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[5].Y = (float)GetValue(child.Childs[0].Childs[16].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[5].Z = (float)GetValue(child.Childs[0].Childs[17].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[6].X = (float)GetValue(child.Childs[0].Childs[18].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[6].Y = (float)GetValue(child.Childs[0].Childs[19].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[6].Z = (float)GetValue(child.Childs[0].Childs[20].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[7].X = (float)GetValue(child.Childs[0].Childs[21].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[7].Y = (float)GetValue(child.Childs[0].Childs[22].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[7].Z = (float)GetValue(child.Childs[0].Childs[23].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[8].X = (float)GetValue(child.Childs[1].Childs[0].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[8].Y = (float)GetValue(child.Childs[1].Childs[1].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[8].Z = (float)GetValue(child.Childs[1].Childs[2].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[9].X = (float)GetValue(child.Childs[1].Childs[3].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[9].Y = (float)GetValue(child.Childs[1].Childs[4].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[9].Z = (float)GetValue(child.Childs[1].Childs[5].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[10].X = (float)GetValue(child.Childs[1].Childs[6].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[10].Y = (float)GetValue(child.Childs[1].Childs[7].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[10].Z = (float)GetValue(child.Childs[1].Childs[8].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[11].X = (float)GetValue(child.Childs[1].Childs[9].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[11].Y = (float)GetValue(child.Childs[1].Childs[10].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[11].Z = (float)GetValue(child.Childs[1].Childs[11].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[12].X = (float)GetValue(child.Childs[1].Childs[12].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[12].Y = (float)GetValue(child.Childs[1].Childs[13].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[12].Z = (float)GetValue(child.Childs[1].Childs[14].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[13].X = (float)GetValue(child.Childs[1].Childs[15].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[13].Y = (float)GetValue(child.Childs[1].Childs[16].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[13].Z = (float)GetValue(child.Childs[1].Childs[17].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[14].X = (float)GetValue(child.Childs[1].Childs[18].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[14].Y = (float)GetValue(child.Childs[1].Childs[19].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[14].Z = (float)GetValue(child.Childs[1].Childs[20].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[15].X = (float)GetValue(child.Childs[1].Childs[21].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[15].Y = (float)GetValue(child.Childs[1].Childs[22].Value as Array);
					PointcloudParam.PcdExtInfo.cvx_hull[15].Z = (float)GetValue(child.Childs[1].Childs[23].Value as Array);
				}
				if (child.Name.Equals("CvxPntNum"))
				{
					PointcloudParam.PcdExtInfo.cvx_pnt_num = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("FlagRect"))
				{
					PointcloudParam.PcdExtInfo.flag_rect = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("RectPt"))
				{
					PointcloudParam.PcdExtInfo.rect_pt[0].X = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.rect_pt[0].Y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.rect_pt[0].Z = (float)GetValue(child.Childs[2].Value);
					PointcloudParam.PcdExtInfo.rect_pt[1].X = (float)GetValue(child.Childs[3].Value);
					PointcloudParam.PcdExtInfo.rect_pt[1].Y = (float)GetValue(child.Childs[4].Value);
					PointcloudParam.PcdExtInfo.rect_pt[1].Z = (float)GetValue(child.Childs[5].Value);
					PointcloudParam.PcdExtInfo.rect_pt[2].X = (float)GetValue(child.Childs[6].Value);
					PointcloudParam.PcdExtInfo.rect_pt[2].Y = (float)GetValue(child.Childs[7].Value);
					PointcloudParam.PcdExtInfo.rect_pt[2].Z = (float)GetValue(child.Childs[8].Value);
					PointcloudParam.PcdExtInfo.rect_pt[3].X = (float)GetValue(child.Childs[9].Value);
					PointcloudParam.PcdExtInfo.rect_pt[3].Y = (float)GetValue(child.Childs[10].Value);
					PointcloudParam.PcdExtInfo.rect_pt[3].Z = (float)GetValue(child.Childs[11].Value);
				}
				if (child.Name.Equals("FlagRectRat"))
				{
					PointcloudParam.PcdExtInfo.flag_rect_rat = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("RectRat"))
				{
					PointcloudParam.PcdExtInfo.rect_rat = (float)GetValue(child.Value);
				}
				if (child.Name.Equals("FlagPlane"))
				{
					PointcloudParam.PcdExtInfo.flag_plane = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("PlaneCoe"))
				{
					PointcloudParam.PcdExtInfo.plane_coe.a = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.plane_coe.b = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.plane_coe.c = (float)GetValue(child.Childs[2].Value);
					PointcloudParam.PcdExtInfo.plane_coe.d = (float)GetValue(child.Childs[3].Value);
				}
				if (child.Name.Equals("FlagNormal"))
				{
					PointcloudParam.PcdExtInfo.flag_normal = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("PcdNormal"))
				{
					PointcloudParam.PcdExtInfo.pcd_normal.X = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.pcd_normal.Y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.pcd_normal.Z = (float)GetValue(child.Childs[2].Value);
				}
				if (child.Name.Equals("FlagRange"))
				{
					PointcloudParam.PcdExtInfo.flag_range = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("Range"))
				{
					PointcloudParam.PcdExtInfo.range.min_x = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.range.min_y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.range.min_z = (float)GetValue(child.Childs[2].Value);
					PointcloudParam.PcdExtInfo.range.max_x = (float)GetValue(child.Childs[3].Value);
					PointcloudParam.PcdExtInfo.range.max_y = (float)GetValue(child.Childs[4].Value);
					PointcloudParam.PcdExtInfo.range.max_z = (float)GetValue(child.Childs[5].Value);
				}
				if (child.Name.Equals("FlagBbox"))
				{
					PointcloudParam.PcdExtInfo.flag_bbox = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("Bbox"))
				{
					PointcloudParam.PcdExtInfo.bbox[0].X = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.bbox[0].Y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.bbox[0].Z = (float)GetValue(child.Childs[2].Value);
					PointcloudParam.PcdExtInfo.bbox[1].X = (float)GetValue(child.Childs[3].Value);
					PointcloudParam.PcdExtInfo.bbox[1].Y = (float)GetValue(child.Childs[4].Value);
					PointcloudParam.PcdExtInfo.bbox[1].Z = (float)GetValue(child.Childs[5].Value);
					PointcloudParam.PcdExtInfo.bbox[2].X = (float)GetValue(child.Childs[6].Value);
					PointcloudParam.PcdExtInfo.bbox[2].Y = (float)GetValue(child.Childs[7].Value);
					PointcloudParam.PcdExtInfo.bbox[2].Z = (float)GetValue(child.Childs[8].Value);
					PointcloudParam.PcdExtInfo.bbox[3].X = (float)GetValue(child.Childs[9].Value);
					PointcloudParam.PcdExtInfo.bbox[3].Y = (float)GetValue(child.Childs[10].Value);
					PointcloudParam.PcdExtInfo.bbox[3].Z = (float)GetValue(child.Childs[11].Value);
					PointcloudParam.PcdExtInfo.bbox[4].X = (float)GetValue(child.Childs[12].Value);
					PointcloudParam.PcdExtInfo.bbox[4].Y = (float)GetValue(child.Childs[13].Value);
					PointcloudParam.PcdExtInfo.bbox[4].Z = (float)GetValue(child.Childs[14].Value);
					PointcloudParam.PcdExtInfo.bbox[5].X = (float)GetValue(child.Childs[15].Value);
					PointcloudParam.PcdExtInfo.bbox[5].Y = (float)GetValue(child.Childs[16].Value);
					PointcloudParam.PcdExtInfo.bbox[5].Z = (float)GetValue(child.Childs[17].Value);
					PointcloudParam.PcdExtInfo.bbox[6].X = (float)GetValue(child.Childs[18].Value);
					PointcloudParam.PcdExtInfo.bbox[6].Y = (float)GetValue(child.Childs[19].Value);
					PointcloudParam.PcdExtInfo.bbox[6].Z = (float)GetValue(child.Childs[20].Value);
					PointcloudParam.PcdExtInfo.bbox[7].X = (float)GetValue(child.Childs[21].Value);
					PointcloudParam.PcdExtInfo.bbox[7].Y = (float)GetValue(child.Childs[22].Value);
					PointcloudParam.PcdExtInfo.bbox[7].Z = (float)GetValue(child.Childs[23].Value);
				}
				if (child.Name.Equals("FlagIncircle"))
				{
					PointcloudParam.PcdExtInfo.flag_incircle = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("IncircleCenter"))
				{
					PointcloudParam.PcdExtInfo.incircle_center.X = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.incircle_center.Y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.incircle_center.Z = (float)GetValue(child.Childs[2].Value);
				}
				if (child.Name.Equals("LongAxis"))
				{
					PointcloudParam.PcdExtInfo.long_axis = (float)GetValue(child.Value);
				}
				if (child.Name.Equals("ShortAxis"))
				{
					PointcloudParam.PcdExtInfo.short_axis = (float)GetValue(child.Value);
				}
				if (child.Name.Equals("FlagOutcircle"))
				{
					PointcloudParam.PcdExtInfo.flag_outcircle = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("OutcircleCenter"))
				{
					PointcloudParam.PcdExtInfo.outcircle_center.X = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.outcircle_center.Y = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.outcircle_center.Z = (float)GetValue(child.Childs[2].Value);
				}
				if (child.Name.Equals("OutcircleRadius"))
				{
					PointcloudParam.PcdExtInfo.outcircle_radius = (float)GetValue(child.Value);
				}
				if (child.Name.Equals("FlagPose"))
				{
					PointcloudParam.PcdExtInfo.flag_pose = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("Pose"))
				{
					PointcloudParam.PcdExtInfo.pose[0].RtmatX[0] = (float)GetValue(child.Childs[0].Value);
					PointcloudParam.PcdExtInfo.pose[0].RtmatX[1] = (float)GetValue(child.Childs[1].Value);
					PointcloudParam.PcdExtInfo.pose[0].RtmatX[2] = (float)GetValue(child.Childs[2].Value);
					PointcloudParam.PcdExtInfo.pose[0].RtmatX[3] = (float)GetValue(child.Childs[3].Value);
					PointcloudParam.PcdExtInfo.pose[1].RtmatX[0] = (float)GetValue(child.Childs[4].Value);
					PointcloudParam.PcdExtInfo.pose[1].RtmatX[1] = (float)GetValue(child.Childs[5].Value);
					PointcloudParam.PcdExtInfo.pose[1].RtmatX[2] = (float)GetValue(child.Childs[6].Value);
					PointcloudParam.PcdExtInfo.pose[1].RtmatX[3] = (float)GetValue(child.Childs[7].Value);
					PointcloudParam.PcdExtInfo.pose[2].RtmatX[0] = (float)GetValue(child.Childs[8].Value);
					PointcloudParam.PcdExtInfo.pose[2].RtmatX[1] = (float)GetValue(child.Childs[9].Value);
					PointcloudParam.PcdExtInfo.pose[2].RtmatX[2] = (float)GetValue(child.Childs[10].Value);
					PointcloudParam.PcdExtInfo.pose[2].RtmatX[3] = (float)GetValue(child.Childs[11].Value);
					PointcloudParam.PcdExtInfo.pose[3].RtmatX[0] = (float)GetValue(child.Childs[12].Value);
					PointcloudParam.PcdExtInfo.pose[3].RtmatX[1] = (float)GetValue(child.Childs[13].Value);
					PointcloudParam.PcdExtInfo.pose[3].RtmatX[2] = (float)GetValue(child.Childs[14].Value);
					PointcloudParam.PcdExtInfo.pose[3].RtmatX[3] = (float)GetValue(child.Childs[15].Value);
				}
				if (child.Name.Equals("FlagClass"))
				{
					PointcloudParam.PcdExtInfo.flag_class = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("ClassType"))
				{
					PointcloudParam.PcdExtInfo.class_type = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("FlagDimension"))
				{
					PointcloudParam.PcdExtInfo.flag_dimension = Convert.ToInt32(GetValue(child.Value));
				}
				if (child.Name.Equals("DimensionScale"))
				{
					PointcloudParam.PcdExtInfo.dimension_scale = Convert.ToInt32(GetValue(child.Value));
				}
			}
		}
		(imageData as PointCloudImageData).SubDatas = new List<ImageData>();
		(imageData as PointCloudImageData).SubDatas.Add(imageData2);
		(imageData as PointCloudImageData).NormalDatas = new List<ImageData>();
		(imageData as PointCloudImageData).NormalDatas.Add(imageData3);
		return (PointCloudImageData)imageData;
	}

	/// <summary>
	/// 获取子节点
	/// Name 获取
	/// </summary>
	/// <param name="parentIo"></param>
	/// <param name="childName"></param>
	/// <returns></returns>
	private VmIO GetChildIoByName(VmIO parentIo, string childName)
	{
		return parentIo?.Children.FirstOrDefault((VmIO a) => a.Name.Contains(childName));
	}

	/// <summary>
	/// 获取图像
	/// Name 获取
	/// </summary>
	/// <param name="parentIo"></param>
	/// <returns></returns>
	private string GetIntensityImageIoName(VmIO parentIo)
	{
		if (parentIo != null)
		{
			foreach (VmIO child in parentIo.Children)
			{
				if (child.Name.Contains(ImageSource3DModuleType.ProfileIntensityImageData.ToString()))
				{
					return child.Name;
				}
			}
		}
		return ImageSource3DModuleType.ProfileIntensityImageData.ToString();
	}

	private object GetValue(object value)
	{
		object result = 0f;
		if (value == null)
		{
			return result;
		}
		IEnumerator enumerator = (value as Array).GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				if (current != null)
				{
					result = current;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return result;
	}

	protected VmIOCollection GetIntputs()
	{
		return _Inputs;
	}

	protected VmIOCollection GetOutputs()
	{
		return _Outputs;
	}

	private bool GetEqualPoint(object oldPoint, object newPoint)
	{
		if (oldPoint != newPoint)
		{
			if (oldPoint is Point point && newPoint is Point point2 && true && point.X == point2.X && point.Y == point2.Y)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	void IVmModule.ModulePaste(object moduMap)
	{
		ModulePaste(moduMap);
	}

	/// <summary>
	/// 自动订阅
	/// </summary>
	/// <param name="target"></param>
	void IVmModule.AutoLinkData(IVmModule source)
	{
		HasInserted = new Dictionary<uint, uint>();
		ReAutoLinkData(source);
	}

	private void ReAutoLinkData(IVmModule source)
	{
		if (!HasInserted.ContainsKey(source.ID))
		{
			HasInserted.Add(source.ID, 0u);
			AutoLinkData(source);
			RefreshNextRelation(source);
		}
	}

	bool IVmModule.IsExistName(string name)
	{
		return allIos.ContainsKey(name.GetHashCode());
	}

	object IVmExternPath.GetExternNode(string sub)
	{
		return GetSelfExternNode(sub);
	}

	protected virtual object GetSelfExternNode(string sub)
	{
		if (sub == null || sub.Length == 0)
		{
			return this;
		}
		string[] array = sub.Split('.');
		object obj = null;
		string key = array[0];
		if (ExternPathStartNodes.ContainsKey(key))
		{
			obj = ExternPathStartNodes[key];
		}
		else
		{
			Hashtable hashtable = ActivatorWrap.TypeTable.Clone() as Hashtable;
			Type type = (from p in hashtable.Values.OfType<Type>()
				where p.IsDefined(typeof(VmExternPathAttribute), inherit: false) && (p.GetCustomAttribute(typeof(VmExternPathAttribute), inherit: false) as VmExternPathAttribute).KeyName == key
				select p).Single();
			hashtable = null;
			obj = ActivatorWrap.CreateInstance(type);
			(obj as IVmExternPath)?.InitNode(this);
			ExternPathStartNodes.Add(key, obj);
		}
		int num = sub.IndexOf('.') + 1;
		if (num != 0)
		{
			string key2 = sub.Substring(num);
			obj = (obj as IVmExternPath).GetExternNode(key2);
			(obj as IVmExternPath)?.InitNode();
		}
		return obj;
	}

	void IVmExternPath.InitNode(object datas)
	{
	}

	void IVmExternPath.PasteNode(object datas)
	{
	}

	/// <summary>
	/// 特殊模块的操作，例如分支，分支字符模块
	/// </summary>
	/// <param name="objParam"></param>
	bool IVmModule.SpModuleCheck(object objParam)
	{
		return SpModuleCheck(objParam);
	}

	private void BindingSynchronization()
	{
	}

	public VmModule()
	{
		_Inputs.Parent = this;
		_Outputs.Parent = this;
		BindingSynchronization();
		InitBase();
	}

	public VmModule(object param, string typeName = null)
	{
		_Inputs.Parent = this;
		_Outputs.Parent = this;
		BindingSynchronization();
		if (param is VmProcedure vmProcedure)
		{
			CreateToModuleSet(vmProcedure, typeName, 1u, vmProcedure.ID);
			ReFlashReadyState();
		}
		else if (param is IMVSGroup iMVSGroup)
		{
			CreateToModuleSet(iMVSGroup, typeName, 20u, iMVSGroup.ID);
			ReFlashReadyState();
		}
		else if (param is VmSolution vmModuleSet)
		{
			CreateToModuleSet(vmModuleSet, typeName, 1u, 10999u);
			ReFlashReadyState();
		}
		else if (param is ImvsSdkDefine.IMVS_MODULE_INFO module)
		{
			Init(module);
		}
		else if (param is ImvsSdkDefine.IMVS_PROCESS_INFO pro)
		{
			Init(pro);
		}
		VmSolution.Instance.AllModules.AddOrUpdate(ID, this, (uint oldKey, VmModule oldValue) => this);
		((IVmSolution)VmSolution.Instance)?.InvokeModuleAddedEvent((IVmModule)this);
	}

	internal VmModule(ImvsSdkDefine.IMVS_MODULE_INFO module)
	{
		BindingSynchronization();
		Init(module);
	}

	internal VmModule(ImvsSdkDefine.IMVS_MODULE_INFO module, VmModuleSet vmModuleSet)
	{
		BindingSynchronization();
		Init(module, vmModuleSet);
	}

	public VmModule(ISubjectModel proxyModule)
	{
		HostModule = proxyModule as VmModule;
		InitParasticValue();
	}

	private void InitParasticValue()
	{
		IsParastic = true;
		IsActive = false;
		Parent = HostModule.Parent;
		_params = HostModule.Params;
		_userData = HostModule.UserData;
	}

	private void DeInitParasticValue()
	{
		IsParastic = false;
		HostModule = null;
		Parent = null;
		_params = null;
		_userData = null;
	}

	private void Init(ModelBase vmModuleSet, uint Id, string strDisplayName)
	{
		Inputs.Parent = this;
		Outputs.Parent = this;
		ID = Id;
		name = strDisplayName;
		StrModuleName = "IMVSGroup";
		InitBase();
		InitIO();
		AddModuleTo(vmModuleSet);
	}

	private void Init(ImvsSdkDefine.IMVS_PROCESS_INFO pro)
	{
		if (this is VmProcedure vmProcedure)
		{
			InitBase();
			vmProcedure.Init(pro);
			InitIO();
		}
	}

	private void Init(ImvsSdkDefine.IMVS_MODULE_INFO module, ModelBase vmModuleSet = null)
	{
		moduleInfo = module;
		name = Hik.Util.StringUtil.UTF8GetString(module.strDisplayName);
		InitBase();
		InitIO();
		if (vmModuleSet != null)
		{
			AddModuleTo(vmModuleSet);
		}
		if (ServerSDKManager.serverSDKManager.mModuleManager.CheckUIParamExist(ID, "GUID"))
		{
			GUID = new Guid(UserData["GUID"]).ToString();
			return;
		}
		Guid guid = Guid.NewGuid();
		UserData["GUID"] = guid.ToByteArray();
		GUID = guid.ToString();
	}

	private void AddModuleTo(ModelBase set)
	{
		PreAddModuleTo(set);
		(set as IChildren).Children.Add(this);
	}

	internal void PreAddModuleTo(ModelBase set)
	{
		Parent = set;
		if (!IsGlobalModu)
		{
			IUnRedoService unRedoService = Ioc.Get<IUnRedoService>();
			if (unRedoService != null && unRedoService.IsNeedExport())
			{
				object obj = new VmModuleDataInfo
				{
					strModuleFullName = FullName
				};
				string displayName = string.Format("{0}{1}:{2}", AppLocalizationService.GetLang("增加"), AppLocalizationService.GetLang("Lang_Modu"), FullName.Substring(name.LastIndexOf('.') + 1));
				Ioc.Get<IUnRedoService>()?.Register(new ModuleUnRedoOperation(Import_Module, Delete_Module, obj as VmModuleDataInfo, Update_Module), displayName);
			}
		}
		LogHelper.Info("Add module:" + Name);
	}

	protected void InitBase()
	{
		IsActive = true;
		UserData.Parent = this;
		InnerUserData.Parent = this;
		Params.Parent = this;
		Params.TimeOut = 3000u;
		string strModuleName = StrModuleName;
		if (strModuleName != null)
		{
			Params.XmlPath = ((IVmModule)this).ModuleDir + $"\\{strModuleName}AlgorithmTab.xml";
		}
		ParamSubManager = new VmParamSubManager(this);
	}

	internal virtual void UpdateContinusRunStatus(bool isRun)
	{
	}

	/// <summary>
	/// 订阅
	/// </summary>
	/// <param name="source">订阅方</param>
	/// <param name="target">被订阅方</param>
	internal virtual void Subscribe(VmIO source, VmIO target)
	{
		if (!IsSuitable(target.TypeName, source.TypeName))
		{
			return;
		}
		if (target.IsLeafNode && source.IsLeafNode)
		{
			ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO subInfo = GetSubInfo(source);
			ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO pubInfo = GetPubInfo(target, source.RelateIndex);
			ServerSDKManager.serverSDKManager.mModuleManager.SubscribeResult(subInfo, pubInfo);
			return;
		}
		if (source.Childs.Count == target.Childs.Count)
		{
			for (int i = 0; i < target.Childs.Count; i++)
			{
				source.Childs[i].RelateIO = target.Childs[i];
			}
			return;
		}
		string lang = AppLocalizationService.GetLang("SubscriptionsNotAligned");
		throw new Exception(lang);
	}

	/// <summary>
	/// 解订阅
	/// </summary>
	/// <param name="source">订阅方</param>
	/// <param name="target">被订阅方</param>
	internal virtual void UnSubscribe(VmIO source, VmIO target)
	{
		try
		{
			if (!IsSuitable(target.TypeName, source.TypeName))
			{
				return;
			}
			if (target.IsLeafNode && source.IsLeafNode)
			{
				ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO subInfo = GetSubInfo(source);
				ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO pubInfo = GetPubInfo(target, source.RelateIndex);
				ServerSDKManager.serverSDKManager.mModuleManager.UnSubscribeResult(subInfo, pubInfo);
			}
			else
			{
				if (source.Childs.Count != target.Childs.Count)
				{
					return;
				}
				for (int i = 0; i < target.Childs.Count; i++)
				{
					if (source.Childs[i].RelateIO == target.Childs[i])
					{
						source.Childs[i].RelateIO = null;
					}
				}
			}
		}
		catch (Exception)
		{
			LogHelper.Error("UnSubscribe error:" + source.Name + ":" + target.Name);
		}
	}

	/// <summary>
	/// 获取能够订阅该模块的所有模块，一个模块IO只可能被本层和下一层和父节点订阅
	/// 增加参数isGlobalModule全局变量模块可被所有模块订阅 2021-05-08 zrw
	/// </summary>
	/// <returns></returns>
	internal List<VmModule> GetSubScribeVisible(bool isGlobalModule = false)
	{
		if (!isGlobalModule)
		{
			IEnumerable<VmModule> enumerable = (Parent as VmModuleSet).Modules.Concat(new VmModule[1] { Parent as VmModule }).OfType<VmModule>();
			if (this is VmModuleSet vmModuleSet)
			{
				enumerable = enumerable.Concat(vmModuleSet.Modules);
			}
			return enumerable.ToList();
		}
		List<VmModule> list = new List<VmModule>();
		(Parent as VmModuleSet).GetAllModule(list);
		return list;
	}

	/// <summary>
	/// 获取被订阅io信息
	/// </summary>
	/// <returns></returns>
	private static ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO GetPubInfo(VmIO target, VmIO relateIndex)
	{
		ImvsSdkDefine.IMVS_PUBLISH_BASIC_INFO result = default;
		if (target.Root == null)
		{
			result.nPubModuleID = 990000u;
		}
		else
		{
			result.nPubModuleID = target.Root.ID;
		}
		result.strParamName = target.Name;
		if (relateIndex != null)
		{
			result.strIndexContent = relateIndex.Name;
			if (relateIndex is VmConstantIO)
			{
				result.nIsIndexConstant = 1u;
			}
			else
			{
				result.nIndexModuleID = relateIndex.Root.ID;
			}
		}
		else
		{
			result.nIsIndexConstant = 1u;
			result.strIndexContent = "All";
		}
		if (target is VmConstantIO)
		{
			result.nIsPubConstant = 1u;
			result.nPubModuleID = 0u;
		}
		return result;
	}

	/// <summary>
	/// 获取订阅IO信息
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	private static ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO GetSubInfo(VmIO source)
	{
		return new ImvsSdkDefine.IMVS_SUBSCRIBE_BASIC_INFO
		{
			nSubModuleID = source.Root.ID,
			strParamName = source.Name
		};
	}

	/// <summary>
	/// 是否可订阅，如果主动订阅IO类型为string或者两者类型相同，返回true,否则返回false// 增加允许情况，主动订阅IO类型为float,被动订阅IO类型为int---20211214，暂时只有结果显示位置XY使用
	/// </summary>
	/// <param name="targetType">被动订阅IO类型</param>
	/// <param name="sourceType">主动订阅IO类型</param>
	/// <returns></returns>
	internal virtual bool IsSuitable(string targetType, string sourceType)
	{
		if (sourceType == "string" || targetType == sourceType || (sourceType == "float" && targetType == "int"))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 获取所有前序模块，不包括父模块
	/// </summary>
	private List<VmModule> GetAllPreModule(List<VmModule> ls = null)
	{
		if (ls == null)
		{
			ls = new List<VmModule>();
		}
		PreModules.ToList().ForEach((VmModule preModule) =>
		{
			if (!ls.Contains(preModule))
			{
				ls.Add(preModule);
				preModule.GetAllPreModule(ls);
			}
		});
		return ls;
	}

	void IVmModule.InitDynamicIO()
	{
		InitCurrentIO();
	}

	internal void InitCurrentIO()
	{
		if (!inputs_IsLoaded || !outputs_IsLoaded)
		{
			VmIOCollection inputs = Inputs;
			VmIOCollection outputs = Outputs;
		}
	}

	/// <summary>
	/// 解订阅前项
	/// </summary>
	internal void UnSubPreModule(List<VmModule> checkedModules)
	{
		if (checkedModules != null)
		{
			if (checkedModules.Contains(this))
			{
				return;
			}
			checkedModules.Add(this);
		}
		List<VmModule> preModules = GetAllPreModule();
		InitCurrentIO();
		if (this is VmModuleSet)
		{
			Inputs.ToList().ForEach((VmIO io) =>
			{
				if (io.RelateIO != null && "TRIGGER_STRING" != io.RelateIO.Name && !(io.RelateIO is VmConstantIO) && !preModules.Contains(io.RelateIO.Root))
				{
					VmModule vmModule = io.RelateIO.Root;
					if ((vmModule == null || vmModule.ID != 13000) && Parent != io.RelateIO.Root && !(io.RelateIO.Root is IVarModule) && this != io.RelateIO.Root)
					{
						io.RelateIO = null;
					}
				}
			});
			return;
		}
		allIos.Values.ToList().ForEach((VmIO io) =>
		{
			if (io.RelateIO != null && "TRIGGER_STRING" != io.RelateIO.Name && !(io.RelateIO is VmConstantIO) && !preModules.Contains(io.RelateIO.Root))
			{
				VmModule vmModule = io.RelateIO.Root;
				if (vmModule == null || vmModule.ID != 13000)
				{
					VmModule vmModule2 = io.RelateIO.Root;
					if (vmModule2 == null || vmModule2.ID >= 24000)
					{
						VmModule vmModule3 = io.RelateIO.Root;
						if (vmModule3 == null || vmModule3.ID <= 25999)
						{
							return;
						}
					}
					if (Parent != io.RelateIO.Root && this != io.RelateIO.Root)
					{
						io.RelateIO = null;
					}
				}
			}
		});
	}

	/// <summary>
	/// 解订阅后项
	/// </summary>
	/// <param name="module"></param>
	internal void UnSubNextModule(List<VmModule> checkedModules)
	{
		foreach (VmModule nextModule in NextModules)
		{
			if (!checkedModules.Contains(nextModule))
			{
				nextModule.UnSubModule(checkedModules);
			}
		}
	}

	/// <summary>
	/// 解订阅
	/// </summary>
	internal void UnSubModule(List<VmModule> checkedModules = null)
	{
		if (checkedModules == null)
		{
			checkedModules = new List<VmModule>();
		}
		UnSubPreModule(checkedModules);
		UnSubNextModule(checkedModules);
	}

	/// <summary>
	/// 模块已经存在的情况下,重新获取io参数，用于导入流程覆盖全局模块使用
	/// add by zrw 20210513
	/// </summary>
	protected virtual void ReloadModuleData()
	{
	}

	internal void ReloadModule()
	{
		ReloadModuleData();
	}

	/// <summary>
	/// 通知父节点，名称已修改
	/// </summary>
	private void NotifyNameChangedToParent(string oldName)
	{
		(Parent as VmModuleSet)?.ReFlashReadyInfoOfName(this, oldName);
	}

	/// <summary>
	/// 加载后序或者前序模块
	/// </summary>
	protected virtual void LoadNextAndPreModules()
	{
		List<VmModule> list = (Parent as VmModuleSet).Modules.Where((VmModule x) => !x.nextAndPreModules_IsLoaded).ToList();
		list.Add(this);
		ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId = new ImvsSdkDefine.IMVS_MODULE_ID_LIST
		{
			nNum = list.Count(),
			anModuleId = new uint[2064]
		};
		int num = 0;
		foreach (VmModule item in list)
		{
			stModuleId.anModuleId[num] = item.ID;
			num++;
			item.nextAndPreModules_IsLoaded = true;
		}
		ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO_LIST multiModuleConnect = ServerSDKManager.serverSDKManager.mModuleManager.GetMultiModuleConnect(stModuleId);
		ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO_LIST iMVS_MODULE_CONNECT_INFO_LIST = multiModuleConnect;
		if (1 == 0)
		{
			return;
		}
		ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO[] pstModuleConnectInfo = iMVS_MODULE_CONNECT_INFO_LIST.pstModuleConnectInfo;
		for (int num2 = 0; num2 < iMVS_MODULE_CONNECT_INFO_LIST.nNum; num2++)
		{
			ImvsSdkDefine.IMVS_MODULE_CONNECT_INFO connectionInfo = pstModuleConnectInfo[num2];
			VmModule vmModule = list[num2];
			int j;
			for (j = 0; j < connectionInfo.nNextModuNum; j++)
			{
				VmModule vmModule2 = (Parent as VmModuleSet).Modules.FirstOrDefault((VmModule p) => p.ID == connectionInfo.anNextModuleId[j]);
				if (!vmModule._NextModules.Contains(vmModule2))
				{
					vmModule._NextModules.Add(vmModule2);
				}
				if (!vmModule2._PreModules.Contains(vmModule))
				{
					vmModule2._PreModules.Add(vmModule);
				}
			}
			vmModule._NextModules.CollectionChanged += vmModule.NextModules_CollectionChanged;
		}
	}

	/// <summary>
	/// 自动连接
	/// </summary>
	private void RefreshNextRelation(IVmModule source)
	{
		foreach (VmModule nextModule in NextModules)
		{
			nextModule.ReAutoLinkData(source);
		}
	}

	/// <summary>
	/// 自动订阅
	/// </summary>
	/// <param name="source"></param>
	/// <param name="needCheckPre">是否需要往前查</param>
	/// <param name="lowerlevel">降低要求的等级</param>
	private void AutoLinkData(IVmModule source, int lowerlevel = 0)
	{
		if (!(source is VmModule vmModule))
		{
			return;
		}
		bool flag = Parent == vmModule;
		VmIOCollection vmIOCollection = (flag ? vmModule.Inputs : vmModule.Outputs);
		Func<VmIO, bool> predicate = (VmIO p) => !p.IsLeafNode && (p.IsForce || p.IsPrefer) && !p.GetAllLeafIO().Any((VmIO pt) => pt.RelateIO != null) && p.AutoSubscribe;
		List<VmIO> list = (from p in vmIOCollection.GetCombinationIO()
			where p.CanSubscribeA && p.Visible && p.AutoSubscribeByOther
			select p).ToList();
		IEnumerable<VmIO> enumerable = list.Where((VmIO p) => p.IsPrefer);
		IEnumerable<VmIO> enumerable2 = Inputs.Where(predicate);
		List<VmIO> list2 = enumerable2.ToList();
		if (enumerable.Count() > 0)
		{
			foreach (VmIO outIO in enumerable)
			{
				list2.AddRange(Inputs.Where((VmIO p) => p.TypeName == outIO.TypeName && p.RelateIO == null));
			}
		}
		foreach (VmIO io in list2)
		{
			if (io.AutoSubscribe)
			{
				VmIO vmIO;
				VmIO vmIO2;
				if (!string.IsNullOrEmpty(io.autoSubscribeName) && (vmIO = list.FirstOrDefault((VmIO p) => p.Name == io.autoSubscribeName)) != null)
				{
					list.Remove(vmIO);
					io.RelateIO = vmIO;
				}
				else if ((vmIO2 = list.FirstOrDefault((VmIO p) => p.TypeName == io.TypeName)) != null)
				{
					list.Remove(vmIO2);
					io.RelateIO = vmIO2;
				}
			}
		}
		Func<VmIO, bool> predicate2 = (VmIO p) => p.IsLeafNode && (p.IsForce || p.IsPrefer) && p.AutoSubscribe && p.RelateIO == null;
		IEnumerable<VmIO> enumerable3 = Inputs.Where(predicate2);
		list2 = enumerable3.ToList();
		if (list2.Count > 0)
		{
			list = vmIOCollection.Where((VmIO p) => p.IsLeafNode && p.CanSubscribeA && p.Visible && p.AutoSubscribeByOther).ToList();
		}
		foreach (VmIO io2 in list2)
		{
			List<VmIO> list3 = list.Where((VmIO p) => p.TypeName == io2.TypeName).ToList();
			VmIO vmIO3;
			if ((vmIO3 = list3.FirstOrDefault((VmIO p) => p.Name == io2.Name || (lowerlevel == 1 && io2.IsForce))) != null)
			{
				list.Remove(vmIO3);
				list3.Remove(vmIO3);
				io2.RelateIO = vmIO3;
			}
		}
		Func<VmIO, bool> predicate3 = (VmIO p) => p.IsLeafNode && (p.IsForce || p.IsPrefer) && IsAngle(p) && p.AutoSubscribe && p.RelateIO == null;
		IEnumerable<VmIO> enumerable4 = Inputs.Where(predicate3);
		list2 = enumerable4.ToList();
		if (list2.Count > 0)
		{
			list = (from p in vmIOCollection.GetAllLeafIO()
				where p.IsLeafNode && IsAngle(p) && p.CanSubscribeA && p.Visible && p.AutoSubscribeByOther
				select p).ToList();
		}
		foreach (VmIO item in list2)
		{
			VmIO vmIO4;
			if (item.AutoSubscribe && (vmIO4 = list.FirstOrDefault()) != null)
			{
				list.Remove(vmIO4);
				item.RelateIO = vmIO4;
			}
		}
		if (flag)
		{
			return;
		}
		list2 = enumerable2.Concat(enumerable3).Concat(enumerable4).ToList();
		if (list2.Count() > 0)
		{
			foreach (VmModule preModule in vmModule.PreModules)
			{
				AutoLinkData(preModule, lowerlevel);
				list2 = enumerable2.Concat(enumerable3).Concat(enumerable4).ToList();
				if (list2.Count() == 0)
				{
					break;
				}
			}
			if (Parent is IVmModuleSet vmModuleSet && list2.Count() != 0)
			{
				AutoLinkData(vmModuleSet as IVmModule, lowerlevel);
			}
		}
		if (lowerlevel == 0)
		{
			list2 = enumerable2.Concat(enumerable3).Concat(enumerable4).ToList();
			if (list2.Count() > 0)
			{
				AutoLinkData(vmModule, 1);
			}
		}
	}

	/// <summary>
	/// 判断参数是否为角度意义
	/// </summary>
	/// <returns></returns>
	private bool IsAngle(VmIO param)
	{
		if (param == null)
		{
			return false;
		}
		if (param.Name == "DetectAngle" || param.Name.Contains("Start") || param.Name.Contains("End") || param.Name.Contains("Extend") || param.Name.Contains("Line"))
		{
			return false;
		}
		if (param.Name.Contains("Angle"))
		{
			return true;
		}
		if (param.Name.Contains("angle"))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 创建模块
	/// </summary>
	/// <param name="vmModuleSet"></param>
	/// <param name="typeName"></param>
	/// <param name="SetNodeType"></param>
	/// <param name="mID"></param>
	protected virtual void CreateToModuleSet(ModelBase vmModuleSet, string typeName, uint SetNodeType, uint mID)
	{
		ImvsSdkDefine.IMVS_SYNC_ADD_ONE_MODULE_INFO stSyncAddModuInput = new ImvsSdkDefine.IMVS_SYNC_ADD_ONE_MODULE_INFO
		{
			nParentNodeType = SetNodeType,
			nParentNodeID = mID
		};
		_StrModuleName = typeName ?? GetType().Name;
		string strModuleName = _StrModuleName;
		stSyncAddModuInput.strModuleDllName = Hik.Util.StringUtil.UTF8GetFixLenBytes(strModuleName, 64);
		string lang = AppLocalizationService.GetLang(strModuleName);
		stSyncAddModuInput.strDisplayName = Hik.Util.StringUtil.UTF8GetFixLenBytes(lang, 256);
		stSyncAddModuInput.nWaitTime = 5000u;
		Init(ServerSDKManager.serverSDKManager.mModuleManager.SyncAddOneModule(stSyncAddModuInput), vmModuleSet);
	}

	private bool InnerParseToBool(string value, string defaultValue = "true")
	{
		return bool.Parse(string.IsNullOrWhiteSpace(value) ? defaultValue : value);
	}

	private bool InnerAnalysisIOInfo(string subinfo, VmIO dynamicVmIO, string value)
	{
		switch (subinfo)
		{
		case "CanSubscribe":
			dynamicVmIO.CanSubscribeA = InnerParseToBool(value);
			break;
		case "IsPrefer":
			dynamicVmIO.IsPrefer = InnerParseToBool(value, "false");
			break;
		case "Visible":
			dynamicVmIO.Visible = InnerParseToBool(value);
			break;
		case "VisibleInResultTree":
			dynamicVmIO.VisibleInResultTree = InnerParseToBool(value);
			break;
		case "AutoSubscribeName":
			dynamicVmIO.autoSubscribeName = value;
			break;
		default:
			return false;
		}
		return true;
	}

	/// <summary>
	/// 解析IO信息
	/// </summary>
	/// <param name="info">sdk提供的IO信息相关结构体</param>
	/// <param name="vmIOCollection">父节点</param>
	protected void AnalysisIOInfo(ImvsSdkDefine.IMVS_MODULE_IO_INFO info, VmIOCollection vmIOCollection)
	{
		string name = Encoding.UTF8.GetString(info.szParamName).TrimEnd(default(char));
		VmIO vmIO = null;
		vmIO = ((!name.StartsWith("%") || !name.EndsWith("%")) ? new VmIO
		{
			Root = this,
			Parent = vmIOCollection.Parent
		} : new DynamicVmIO
		{
			Root = this,
			Parent = vmIOCollection.Parent,
			IsLoading = true
		});
		if (vmIOCollection.FirstOrDefault((VmIO p) => p.Name == name) != null)
		{
			return;
		}
		vmIO.Name = name;
		vmIOCollection.Add(vmIO);
		vmIO.typeName = Encoding.UTF8.GetString(info.szParamType).TrimEnd(default(char));
		char[] array = new char[0];
		string text = Encoding.UTF8.GetString(info.szExtraData).TrimEnd(default(char)).Trim();
		array = ((text.IndexOf(';') <= -1) ? new char[1] { ' ' } : new char[1] { ';' });
		string[] array2 = text.Trim(array).Split(array);
		if (info.nDataType == 1)
		{
			string[] array3 = array2;
			foreach (string text2 in array3)
			{
				int num2 = -1;
				if ((num2 = text2.IndexOf("=")) <= 0)
				{
					continue;
				}
				string text3 = text2.Substring(0, num2).Trim();
				string value = ((text2.Split('=').Count() == 2) ? text2.Split('=')[1] : string.Empty).Trim();
				if (!InnerAnalysisIOInfo(text3, vmIO, value))
				{
					switch (text3)
					{
					case "CustomVisible":
						vmIO.CustomVisibleA = InnerParseToBool(value);
						break;
					case "IsForce":
						vmIO.IsForce = InnerParseToBool(value, "false");
						break;
					case "IsPrivate":
						vmIO.IsPrivate = InnerParseToBool(value, "false");
						break;
					case "IsReturnRelateValue":
						vmIO.IsReturnRelateValue = InnerParseToBool(value);
						break;
					case "AllowAutoSubscribe":
						vmIO.AutoSubscribe = InnerParseToBool(value);
						break;
					case "AllowAutoSubscribeByOther":
						vmIO.AutoSubscribeByOther = InnerParseToBool(value);
						break;
					case "IsResultShow":
						vmIO.IsResultShow = InnerParseToBool(value, "false");
						break;
					}
				}
			}
		}
		else if (info.nDataType == 2)
		{
			string[] array4 = array2;
			foreach (string text4 in array4)
			{
				int num4 = -1;
				if ((num4 = text4.IndexOf("=")) > 0)
				{
					string subinfo = text4.Substring(0, num4).Trim();
					string value2 = ((text4.Split('=').Count() == 2) ? text4.Split('=')[1] : string.Empty).Trim();
					InnerAnalysisIOInfo(subinfo, vmIO, value2);
				}
			}
			int nNum = info.nNum;
			if (nNum > 0 && IntPtr.Zero != info.pstCombInfo)
			{
				for (int num5 = 0; num5 < nNum; num5++)
				{
					ImvsSdkDefine.IMVS_MODULE_IO_INFO info2 = (ImvsSdkDefine.IMVS_MODULE_IO_INFO)Marshal.PtrToStructure(info.pstCombInfo + num5 * Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_IO_INFO)), typeof(ImvsSdkDefine.IMVS_MODULE_IO_INFO));
					AnalysisIOInfo(info2, vmIO.Children);
				}
			}
		}
		if (vmIO is DynamicVmIO dynamicVmIO)
		{
			dynamicVmIO.IsLoading = false;
		}
	}

	/// <summary>
	/// 后继模块改变事件
	/// </summary>
	protected void NextModules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		try
		{
			if (e.OldItems != null)
			{
				foreach (object oldItem in e.OldItems)
				{
					ServerSDKManager.serverSDKManager.mModuleManager.DisConnectOneModule(ID, (oldItem as VmModule).ID);
					if (!IsGlobalModu && !(oldItem as VmModule).IsGlobalModu)
					{
						string displayName = string.Format("{0}{1}:{2}, {3}", AppLocalizationService.GetLang("Lang_Delet"), AppLocalizationService.GetLang("Lang_Connect"), FullName.Substring(name.LastIndexOf('.') + 1), (oldItem as VmModule).FullName.Substring(name.LastIndexOf('.') + 1));
						Ioc.Get<IUnRedoService>()?.Register(new ConnectionUnRedoAction(DeleteConnection, AddConnection, new UnRedo_ConnectionData
						{
							SourceModuleFullName = FullName,
							TargetModuleFullName = (oldItem as VmModule).FullName
						}), displayName);
					}
					(oldItem as VmModule).PreModules.Remove(this);
					(oldItem as VmModule).UnSubModule();
					ModuleOperate(null);
				}
			}
			if (e.NewItems != null)
			{
				foreach (object newItem in e.NewItems)
				{
					ServerSDKManager.serverSDKManager.mModuleManager.ConnectOneModule(ID, (newItem as VmModule).ID);
					if (!(newItem as VmModule).PreModules.Contains(this))
					{
						(newItem as VmModule).PreModules.Add(this);
					}
					ModuleOperate(null);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				VmModule vmModule = (VmModule)e.NewItems[e.NewItems.Count - 1];
				if (!IsGlobalModu && !vmModule.IsGlobalModu)
				{
					string displayName2 = string.Format("{0}{1}:{2}, {3}", AppLocalizationService.GetLang("Lang_Edit"), AppLocalizationService.GetLang("Lang_Connect"), FullName.Substring(name.LastIndexOf('.') + 1), vmModule.FullName.Substring(name.LastIndexOf('.') + 1));
					Ioc.Get<IUnRedoService>()?.Register(new ConnectionUnRedoAction(AddConnection, DeleteConnection, new UnRedo_ConnectionData
					{
						SourceModuleFullName = FullName,
						TargetModuleFullName = vmModule.FullName
					}), displayName2);
				}
			}
		}
		catch (VmException ex)
		{
			LogHelper.Error(ex.errorMessage);
			throw ex;
		}
		catch (Exception ex2)
		{
			LogHelper.Error(ex2.Message);
		}
	}

	/// <summary>
	/// 移除所有模块
	/// </summary>
	protected void RemoveFromAllModules()
	{
		VmSolution.Instance.AllModules.TryRemove(ID, out var _);
		if (!(this is VmModuleSet vmModuleSet))
		{
			return;
		}
		foreach (VmModule module in vmModuleSet.Modules)
		{
			module.RemoveFromAllModules();
		}
	}

	protected void GetAllModulesIDFroVar(ref Dictionary<uint, List<uint?>> nModuleIDList)
	{
		if (nModuleIDList == null)
		{
			nModuleIDList = new Dictionary<uint, List<uint?>>();
		}
		if (this is VmModuleSet vmModuleSet)
		{
			if (Parent != null && Parent is VmModuleSet vmModuleSet2 && nModuleIDList.ContainsKey(vmModuleSet2.ID))
			{
				nModuleIDList[vmModuleSet2.ID].Add(ID);
			}
			if (!nModuleIDList.ContainsKey(ID))
			{
				nModuleIDList.Add(ID, new List<uint?>());
			}
			nModuleIDList[ID].Add(((IVmModuleSet)vmModuleSet).LocalVariablModule?.ID);
			{
				foreach (VmModule module in vmModuleSet.Modules)
				{
					module.GetAllModulesIDFroVar(ref nModuleIDList);
				}
				return;
			}
		}
		if (Parent != null && Parent is IVmModule vmModule && nModuleIDList.ContainsKey(vmModule.ID))
		{
			nModuleIDList[vmModule.ID].Add(ID);
		}
	}

	/// <summary>
	/// 向上汇报模块状态
	/// </summary>
	internal void NotifyReadyStateToParent()
	{
		(Parent as VmModuleSet)?.ReFlashReadyState(this);
	}

	/// <summary>
	/// 同步创建流程
	/// </summary>
	/// <returns></returns>
	internal ImvsSdkDefine.IMVS_PROCESS_INFO CreateToProcessInfo()
	{
		return ServerSDKManager.serverSDKManager.mProcessManager.SyncAddOneProcess(AppLocalizationService.GetLang("Lang_Process"));
	}

	/// <summary>
	/// 主动获取模块结果
	/// </summary>
	internal virtual void RefreshResult()
	{
		if (Root == null || (ReportResultEnable && Root.isRunning) || IsForbidden || ExecuteCount == Root?.ExecuteCount)
		{
			return;
		}
		ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST_Class iMVS_QUERY_MODULE_RES_INFO_LIST_Class = null;
		try
		{
			iMVS_QUERY_MODULE_RES_INFO_LIST_Class = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleRecentlyResult(ID, Root.ExecuteCount);
			if (iMVS_QUERY_MODULE_RES_INFO_LIST_Class == null)
			{
				return;
			}
			ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX[] array = iMVS_QUERY_MODULE_RES_INFO_LIST_Class.pastModuResultList();
			uint nExecuteCount = Root.ExecuteCount;
			if (!((IVmSolution)VmSolution.Instance).IsClearLastExecuteResult && iMVS_QUERY_MODULE_RES_INFO_LIST_Class.nNum > 0)
			{
				nExecuteCount = array[0].nExecuteCount;
			}
			if (nExecuteCount != ExecuteCount)
			{
				int num = 0;
				for (int num2 = iMVS_QUERY_MODULE_RES_INFO_LIST_Class.nNum - 1; num2 >= 0; num2--)
				{
					if (array[num2].nExecuteCount == nExecuteCount && (array[num2].bIsSelfExecute != 1 || num2 == 0))
					{
						ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data result = ServerSDKManager.serverSDKManager.mModuleManager.PreProData(array[num2]);
						VmSolution.MModuleManager_OnModuleResultCallBack(ref result, isCallBack: false);
						num++;
					}
				}
				if (num > 1)
				{
					List<VmIO> list = Outputs.leafIOsWithoutTimeStamp.Values.ToList();
					foreach (VmIO item in list)
					{
						if (!item.RaiseValueChanged)
						{
							item.RaiseValueChanged = true;
							if (item.GetCurValueCount() > 0)
							{
								item.InvokeValueChangedEvent();
							}
						}
					}
				}
			}
			ExecuteCount = Root.ExecuteCount;
			Inputs.SetExecuteCount(Root.ExecuteCount);
			Outputs.SetExecuteCount(Root.ExecuteCount);
		}
		catch (VmException ex)
		{
			LogHelper.Error(ErrorCodeInfo.GetErrorInfo(ex.errorCode));
		}
		finally
		{
			iMVS_QUERY_MODULE_RES_INFO_LIST_Class?.Release();
		}
	}

	private void DeleteModuleParamBind(VmModule delModu, VmModule curModu)
	{
		if (delModu == null || curModu == null || curModu.NextModules == null || curModu.NextModules.Count <= 0)
		{
			return;
		}
		foreach (VmModule modu in curModu.NextModules)
		{
			if (modu.Params.Data is IUpdateItemRoot updateItemRoot)
			{
				Expression<Action<object>> expression = (object obj) => modu.RegisterSubParamFunc(obj);
				updateItemRoot.DeleteRunParamBind(delModu, modu, expression);
			}
			DeleteModuleParamBind(delModu, modu);
		}
	}

	/// <summary>
	/// 删除模块
	/// </summary>
	protected virtual void CloseModuleInterface()
	{
		ServerSDKManager.serverSDKManager.mModuleManager.CloseOneModule(ID);
	}

	/// <summary>
	/// 通用释放
	/// </summary>
	protected void CommonDispose()
	{
		for (int num = NextModules.Count - 1; num >= 0; num--)
		{
			NextModules.RemoveAt(num);
		}
		if (Parent is VmModuleSet vmModuleSet)
		{
			vmModuleSet.UnSubInnerModule(this);
		}
		for (int num2 = Inputs.Count - 1; num2 >= 0; num2--)
		{
			Inputs[num2].Dispose();
		}
		for (int num3 = Outputs.Count - 1; num3 >= 0; num3--)
		{
			Outputs[num3].Dispose();
		}
	}

	/// <summary>
	/// 复制粘贴后操作
	/// </summary>
	protected virtual void PasteFinish(VmModule oldMode, object moduMap)
	{
	}

	internal void ToPasteFinish(VmModule oldMode, object moduMap)
	{
		PasteFinish(oldMode, moduMap);
	}

	internal void ModulePasteFinish(object moduMap)
	{
		if (moduMap != null)
		{
			Dictionary<object, object> source = moduMap as Dictionary<object, object>;
			object key = source.Where((KeyValuePair<object, object> p) => p.Value == this).FirstOrDefault().Key;
			if (key != null)
			{
				PasteFinish((VmModule)key, moduMap);
			}
		}
	}

	/// <summary>
	/// 复制粘贴
	/// </summary>
	protected virtual void Paste(VmModule oldMode, object moduMap)
	{
	}

	internal void ToPaste(VmModule oldMode, object moduMap)
	{
		Paste(oldMode, moduMap);
	}

	/// <summary>
	/// 模块粘贴
	/// </summary>
	internal void ModulePaste(object moduMap)
	{
		if (moduMap == null)
		{
			return;
		}
		Dictionary<object, object> dictionary = moduMap as Dictionary<object, object>;
		object key = dictionary.Where((KeyValuePair<object, object> p) => p.Value == this).FirstOrDefault().Key;
		if (key == null)
		{
			return;
		}
		VmModule vmModule = key as VmModule;
		foreach (VmModule oldSource in vmModule.PreModules)
		{
			object value = dictionary.Where((KeyValuePair<object, object> p) => p.Key == oldSource).FirstOrDefault().Value;
			if (value == null)
			{
				if (oldSource.Parent == Parent && oldSource.SpModuleCheck(false))
				{
					int num = 2;
					byte[] array = oldSource.UserData["SourceIndex" + vmModule.Name];
					if (array != null)
					{
						num = ExternalCallHelper.BytesToStruct<int>(array, 0, array.Length);
					}
					oldSource.UserData["SourceIndex" + Name] = ExternalCallHelper.StructToBytes(num);
					int num2 = 0;
					byte[] array2 = oldSource.UserData["TargetIndex" + vmModule.Name];
					if (array2 != null)
					{
						num2 = ExternalCallHelper.BytesToStruct<int>(array2, 0, array2.Length);
					}
					oldSource.UserData["TargetIndex" + Name] = ExternalCallHelper.StructToBytes(num2);
					if (!oldSource.NextModules.Contains(this))
					{
						oldSource.NextModules.Add(this);
					}
				}
			}
			else
			{
				int num3 = 2;
				byte[] array3 = oldSource.UserData["SourceIndex" + vmModule.Name];
				if (array3 != null)
				{
					num3 = ExternalCallHelper.BytesToStruct<int>(array3, 0, array3.Length);
				}
				(value as VmModule).UserData["SourceIndex" + Name] = ExternalCallHelper.StructToBytes(num3);
				int num4 = 0;
				byte[] array4 = oldSource.UserData["TargetIndex" + vmModule.Name];
				if (array4 != null)
				{
					num4 = ExternalCallHelper.BytesToStruct<int>(array4, 0, array4.Length);
				}
				(value as VmModule).UserData["TargetIndex" + Name] = ExternalCallHelper.StructToBytes(num4);
				if (!(value as VmModule).NextModules.Contains(this))
				{
					(value as VmModule).NextModules.Add(this);
				}
			}
		}
		Expression<Func<object>> oldValue = () => GetAllUserData();
		string displayName = string.Format("{0}{1}:{2}ALL_USERDATA", AppLocalizationService.GetLang("Lang_Edit"), AppLocalizationService.GetLang("界面数据"), FullName.Substring(FullName.LastIndexOf(',') + 1));
		Ioc.Get<IUnRedoService>()?.Register(new UserDataUnRedoOperation(UserData.UserData_Redo, UserData.UserData_Undo, new UnRedo_UserData
		{
			ModuleFullName = FullName,
			IsAllUserData = true,
			PropertyKey = "ALL_USERDATA",
			OldValue = oldValue,
			NewValue = null
		}, UserData.UserData_Update), displayName, (IBuilder builder, IUnRedoOperation operation) => UserData.CanRegisterUserData(builder, operation));
		object moduleAllParams = GetModuleAllParams();
		string displayName2 = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), FullName, AppLocalizationService.GetLang("Tab_Params"));
		Ioc.Get<IUnRedoService>()?.Register(new ParamUnRedoOperation(Param_Redo, Param_Undo, Param_Update, new UnRedo_ParamsData
		{
			ModuleFullName = FullName,
			OldParamList = moduleAllParams,
			NewParamList = null
		}, IsGression), displayName2);
		if (IsContainsModel)
		{
			IUnRedoService unRedoService = Ioc.Get<IUnRedoService>();
			if (unRedoService != null && unRedoService.IsNeedCache())
			{
				byte[] oldModel = null;
				try
				{
					oldModel = GetModel();
				}
				catch
				{
				}
				string displayName3 = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), FullName, AppLocalizationService.GetLang("模型"));
				Ioc.Get<IUnRedoService>()?.Register(new ModelUnRedoOperation(Model_Redo, Model_Undo, Model_Update, new UnRedo_ModelData
				{
					ModuleFullName = FullName,
					OldModel = oldModel,
					NewModel = null
				}), displayName3);
			}
		}
		if (IsForbidden)
		{
			ServerSDKManager.serverSDKManager.mModuleManager.EnableModule(ID, 0u);
			string text = string.Format("{0}:{1}, {2}", AppLocalizationService.GetLang("Lang_Forbidden"), Name, true);
			Ioc.Get<IUnRedoService>()?.Register(new ForbidSetUnRedoOperation(SetNewForbid, SetOldForbid, new UnRedo_ForbidData(FullName, oldValue: false, newValue: true)), displayName2);
		}
		IList<VmIO> list = new List<VmIO>();
		List<VmModule> frontParamItems = GetFrontParamItems();
		foreach (VmIO input in vmModule.Inputs)
		{
			if (input.RelateIO?.Root?.IsGlobalModu ?? false)
			{
				list.Add(input);
				continue;
			}
			VmModule vmModule2 = input.RelateIO?.Root;
			if (vmModule2 == null)
			{
				list.Add(input);
				continue;
			}
			if ((MathHelper.IsNumber(input.SubscriptionInfo) || MathHelper.IsConst(input.SubscriptionInfo)) && input.SubscriptionInfo != "TRIGGER_STRING" && input.SubscriptionInfo != "TRIGGER_STRING[]")
			{
				list.Add(input);
				continue;
			}
			if (!frontParamItems.Contains(vmModule2))
			{
				Dictionary<object, object>.KeyCollection keys = dictionary.Keys;
				if (keys == null || !keys.Contains(vmModule2) || !frontParamItems.Contains(dictionary[vmModule2]))
				{
					list.Add(input);
					continue;
				}
			}
			if (input.RelateIO?.Root != Parent || Parent == vmModule.Parent)
			{
				list.Add(input);
			}
		}
		PasteIO(list, Inputs, dictionary);
		PasteIO(vmModule.Outputs, Outputs, dictionary);
		if (!(this is IMVSGroup))
		{
			ReflashReadyStateToServer();
		}
		if (vmModule.Params.Data != null)
		{
			(vmModule.Params.Data as IUpdateItemRoot)?.UpdateRunParamBind(FullName, dictionary);
		}
		Paste(vmModule, dictionary);
		if (VmSolution.Instance[((IVmFullPath)this).FullPath + ".[Display]"] is IVmExternPath vmExternPath)
		{
			vmExternPath.PasteNode(dictionary);
		}
	}

	private void SetOldForbid(UnRedo_ForbidData data, IUnRedoParams unRedoParams)
	{
		object obj = VmSolution.Instance[data.ModuleName];
		if (obj != null)
		{
			(obj as IVmModule).IsForbidden = data.OldValue;
		}
	}

	private void SetNewForbid(UnRedo_ForbidData data, IUnRedoParams unRedoParams)
	{
		object obj = VmSolution.Instance[data.ModuleName];
		if (obj != null)
		{
			(obj as IVmModule).IsForbidden = data.NewValue;
		}
	}

	/// <summary>
	/// 模块导出
	/// </summary>
	protected internal virtual object Export()
	{
		ImvsSdkDefine.IMVS_EXPORT_MODULE_DATA_INFO? moduleExportData = ServerSDKManager.serverSDKManager.mModuleManager.GetModuleExportData(ID, Name);
		if (moduleExportData.HasValue)
		{
			VmModuleDataInfo vmModuleDataInfo = new VmModuleDataInfo();
			if (Parent is VmSolution)
			{
				vmModuleDataInfo.nParentNodeID = 10999u;
				vmModuleDataInfo.strParentName = "VmSolution";
			}
			else
			{
				vmModuleDataInfo.nParentNodeID = (Parent as VmModule).ID;
				vmModuleDataInfo.strParentName = (Parent as VmModule).Name;
			}
			vmModuleDataInfo.nModuleID = ID;
			vmModuleDataInfo.strModuleFullName = FullName;
			vmModuleDataInfo.strModuDisplayName = Name;
			vmModuleDataInfo.strModuTypeName = StrModuleName;
			vmModuleDataInfo.nWaitTime = 5000u;
			vmModuleDataInfo.pData = moduleExportData.Value.pData;
			vmModuleDataInfo.nDataLen = moduleExportData.Value.nDataLen;
			return vmModuleDataInfo;
		}
		return null;
	}

	/// <summary>
	/// 设置模块状态
	/// </summary>
	/// <param name="info"></param>
	internal void SetModuleState(ImvsSdkDefine.IMVS_REPORT_MODULE_STATUS_INFO info)
	{
		switch (info.nModuProcessStatus)
		{
		case 0u:
		case 5u:
			IsRunOK = info.nModuleStatus == 1;
			break;
		case 1u:
			IsRunOK = false;
			break;
		case 2u:
		case 3u:
			IsRunOK = null;
			break;
		}
		AlgorithmTime = info.fAlgorithmTime;
		ModuleTime = info.fModuleTime;
		ProcessTime = info.fProcessTime;
	}

	/// <summary>
	/// 连线更改时操作
	/// </summary>
	protected internal virtual void ModuleOperate(object objPram)
	{
	}

	/// <summary>
	/// 复制粘贴前模块检验
	/// </summary>
	/// <param name="objParam"></param>
	/// <returns></returns>
	protected internal virtual bool SpModuleCheck(object objParam)
	{
		return true;
	}

	/// <summary>
	/// 解除模块输出与全局变量的绑定关系
	/// </summary>
	protected void ClearVarBindResultRelation(Dictionary<uint, List<uint?>> nModuleIdList)
	{
		VmModule vmModule = VmSolution.Instance.Modules.FirstOrDefault((VmModule x) => x.ID == 13000);
		if (vmModule != null)
		{
			List<uint?> list = new List<uint?>();
			foreach (KeyValuePair<uint, List<uint?>> nModuleId in nModuleIdList)
			{
				list.Add(nModuleId.Key);
				list.AddRange(nModuleId.Value);
			}
			ServerSDKManager.serverSDKManager.mGlobalModuleManager.UnBindAllVarParam(13000u, list);
		}
		foreach (KeyValuePair<uint, List<uint?>> nModuleId2 in nModuleIdList)
		{
			uint num = 0u;
			if (nModuleId2.Value.Count > 0 && nModuleId2.Value[0].HasValue)
			{
				num = nModuleId2.Value[0].Value;
				if (ServerSDKManager.serverSDKManager.mModuleManager.CheckLocalVariableModule((int)num))
				{
					List<uint?> list2 = new List<uint?>();
					list2.Add(nModuleId2.Key);
					list2.AddRange(nModuleId2.Value);
					ServerSDKManager.serverSDKManager.mGlobalModuleManager.UnBindAllVarParam(num, list2);
				}
			}
		}
	}

	internal object GetModuleAllSubscribe()
	{
		ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId = new ImvsSdkDefine.IMVS_MODULE_ID_LIST
		{
			nNum = 1,
			anModuleId = new uint[2064]
		};
		stModuleId.anModuleId[0] = ID;
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST multiModuleSubscribe = ServerSDKManager.serverSDKManager.mModuleManager.GetMultiModuleSubscribe(stModuleId);
		return multiModuleSubscribe.Clone();
	}

	internal object GetModuleAllParams()
	{
		ImvsSdkDefine.IMVS_MODULE_PARAM_LIST value = ServerSDKManager.serverSDKManager.mModuleManager.GetParamList(ID).Value;
		return value;
	}

	private void ClearAllSubscribe()
	{
		foreach (VmIO input in Inputs)
		{
			input.ClearSubscribe();
		}
	}

	internal void SetAllSubscribe(object subList)
	{
		if (this is VmProcedure vmProcedure)
		{
			ClearAllSubscribe();
			if (subList is ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST procedureRelateIO)
			{
				vmProcedure.SetProcedureRelateIO(procedureRelateIO);
			}
			return;
		}
		ClearAllSubscribe();
		if (!(subList is ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST))
		{
			return;
		}
		Dictionary<VmIO, VmIO> dictionary = new Dictionary<VmIO, VmIO>();
		ImvsSdkDefine.IMVS_SUBSCRIBE_INFO[] array = iMVS_MODULE_SUBSCRIBE_INFO_LIST.pstSubscribeInfo();
		for (int i = 0; i < iMVS_MODULE_SUBSCRIBE_INFO_LIST.nNum; i++)
		{
			ImvsSdkDefine.IMVS_SUBSCRIBE_INFO subScribeInfo = array[i];
			string text = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strSubInput);
			string text2 = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strPubOutput);
			VmModule vmModule = (Parent as VmModuleSet).Modules.FirstOrDefault((VmModule p) => p.ID == subScribeInfo.nPubModuleID);
			if (vmModule == null && Parent is IMVSGroup iMVSGroup && iMVSGroup.ID == subScribeInfo.nPubModuleID)
			{
				vmModule = iMVSGroup;
			}
			if (vmModule == null && this is IMVSGroup)
			{
				vmModule = (this as VmModuleSet).Modules.FirstOrDefault((VmModule p) => p.ID == subScribeInfo.nPubModuleID);
			}
			if (vmModule == null && subScribeInfo.nPubModuleID >= 10000)
			{
				vmModule = VmSolution.Instance.Modules.FirstOrDefault((VmModule p) => p.ID == subScribeInfo.nPubModuleID);
			}
			VmIO leafIO = GetLeafIO(text);
			VmIO vmIO = null;
			if (subScribeInfo.nIsPubConstant == 1)
			{
				VmConstantIO relateIO = new VmConstantIO
				{
					Name = text2,
					TypeName = leafIO.TypeName,
					Root = this
				};
				leafIO.RelateIO = relateIO;
			}
			else if (vmModule == null && text2.Contains("TRIGGER_STRING"))
			{
				VmIO relateIO2 = new VmIO
				{
					Name = "TRIGGER_STRING",
					TypeName = "string"
				};
				leafIO.RelateIO = relateIO2;
			}
			else
			{
				vmIO = vmModule.GetLeafIO(text2);
				VmIO combinRoot = leafIO.CombinRoot;
				VmIO combinRoot2 = vmIO.CombinRoot;
				leafIO.RelateIO = vmIO;
			}
		}
		foreach (KeyValuePair<VmIO, VmIO> item in dictionary)
		{
			item.Key.RelateIO = item.Value;
		}
	}

	internal void SetAllParams(object paramList)
	{
		if (paramList is ImvsSdkDefine.IMVS_MODULE_PARAM_LIST stParamList && true && stParamList.nParamNum != 0)
		{
			ServerSDKManager.serverSDKManager.mModuleManager.SetParamList(ID, stParamList);
			if (this is IMVSGroup)
			{
				OnPropertyChanged("IsEnableLoop");
			}
		}
	}

	internal void SetUserData(string property, object value, bool isEnd)
	{
		if (property == "ALL_USERDATA")
		{
			if (this is VmProcedure)
			{
				ServerSDKManager.serverSDKManager.mModuleManager.DeleteAllUIParamValue(ID);
				ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST iMVS_UI_OBJECT_PARAM_INFO_LIST = (ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST)value;
				if (iMVS_UI_OBJECT_PARAM_INFO_LIST == null)
				{
					return;
				}
				ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO[] array = iMVS_UI_OBJECT_PARAM_INFO_LIST.pUIObjParamInfo();
				for (int i = 0; i < iMVS_UI_OBJECT_PARAM_INFO_LIST.nParamNum; i++)
				{
					string text = Hik.Util.StringUtil.UTF8GetString(array[i].strParamName);
					if (!(text == "IsForbidden"))
					{
						if (array[i].pData == null)
						{
							UserData[text] = null;
						}
						else
						{
							UserData[text] = array[i].pData;
						}
					}
				}
				try
				{
					ReloadFuncChild(this)?.UpdateRootItem(property);
				}
				catch
				{
				}
				OnPropertyChanged("Position");
				return;
			}
			ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleIOBeSubscribed(ID, "");
			ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST2 = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)iMVS_MODULE_SUBSCRIBE_INFO_LIST.Clone();
			ServerSDKManager.serverSDKManager.mModuleManager.DeleteAllUIParamValue(ID);
			ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST iMVS_UI_OBJECT_PARAM_INFO_LIST2 = (ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO_LIST)value;
			if (iMVS_UI_OBJECT_PARAM_INFO_LIST2 == null)
			{
				return;
			}
			ImvsSdkDefine.IMVS_UI_OBJECT_PARAM_INFO[] array2 = iMVS_UI_OBJECT_PARAM_INFO_LIST2.pUIObjParamInfo();
			for (int j = 0; j < iMVS_UI_OBJECT_PARAM_INFO_LIST2.nParamNum; j++)
			{
				string key = Hik.Util.StringUtil.UTF8GetString(array2[j].strParamName);
				if (array2[j].pData == null)
				{
					UserData[key] = null;
				}
				else
				{
					UserData[key] = array2[j].pData;
				}
			}
			try
			{
				ReloadFunc(this)?.UpdateRootItem(property);
			}
			catch
			{
			}
			Position = null;
			OnPropertyChanged("Position");
			return;
		}
		UserData[property] = (byte[])value;
		if (property.Contains("TargetIndex"))
		{
			string text2 = property.Substring("TargetIndex".Length);
			string sub = FullName.Substring(0, FullName.LastIndexOf('.') + 1) + text2;
			((VmModule)VmSolution.Instance[sub])?.OnPropertyChanged("TargetIndex");
		}
		else if (property.Contains("SourceIndex"))
		{
			string text3 = property.Substring("SourceIndex".Length);
			string sub2 = FullName.Substring(0, FullName.LastIndexOf('.') + 1) + text3;
			((VmModule)VmSolution.Instance[sub2])?.OnPropertyChanged("SourceIndex");
		}
		else if (property.Contains("[ParamRoot]"))
		{
			if (isEnd && ExternPathStartNodes.ContainsKey("[Display]"))
			{
				ReloadFunc(this)?.UpdateRootItem(property);
			}
		}
		else if (property.Contains("Position"))
		{
			Position = null;
			OnPropertyChanged("Position");
		}
		else if (property.Contains("IsForbidden"))
		{
			if (value != null)
			{
				byte[] array3 = (byte[])value;
				IsForbidden = ExternalCallHelper.BytesToStruct<bool>(array3, 0, array3.Length);
			}
		}
		else
		{
			OnPropertyChanged(property);
		}
	}

	internal void RecoverSubscribe(ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST m_list)
	{
		ImvsSdkDefine.IMVS_SUBSCRIBE_INFO[] array = m_list.pstSubscribeInfo();
		for (int i = 0; i < m_list.nNum; i++)
		{
			ImvsSdkDefine.IMVS_SUBSCRIBE_INFO subScribeInfo = array[i];
			List<VmModule> subScribeVisible = GetSubScribeVisible();
			VmModule vmModule = subScribeVisible.Single((VmModule p) => p.ID == subScribeInfo.nSubModuleID);
			string text = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strSubInput);
			string text2 = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strPubOutput);
			string text3 = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strIndex);
			uint nIndexModuleID = subScribeInfo.nIndexModuleID;
			uint nIsIndexConstant = subScribeInfo.nIsIndexConstant;
			VmIO leafIO = GetLeafIO(text2);
			VmModule vmModule2 = VmSolution.Instance.Modules.FirstOrDefault((VmModule p) => p.ID == 13000);
			if (vmModule2 != null)
			{
				subScribeVisible.Add(vmModule2);
			}
			VmIO vmIO = null;
			vmIO = vmModule.GetLeafIO(text);
			if (vmIO != null)
			{
				if (vmIO.RelateIO != null)
				{
					continue;
				}
				vmIO.RelateIO = leafIO;
			}
			if (string.IsNullOrEmpty(text3) || !(text3 != "All"))
			{
				continue;
			}
			if (nIsIndexConstant == 1)
			{
				VmConstantIO vmIO2 = new VmConstantIO
				{
					Name = text3
				};
				vmIO.InitRelateIndexIO(vmIO2);
				continue;
			}
			VmModule vmModule3 = subScribeVisible.FirstOrDefault((VmModule p) => p.ID == nIndexModuleID);
			VmIO leafIO2 = vmModule3.GetLeafIO(text3);
			if (leafIO2 == null && vmModule3.ID == 13000)
			{
				vmModule3.InitGlobalIo();
				leafIO2 = vmModule3.GetLeafIO(text3);
			}
			vmIO.InitRelateIndexIO(leafIO2);
		}
	}

	/// <summary>
	/// io初始化
	/// </summary>
	protected internal virtual void InitIO()
	{
		string[] array = fTimeStamp;
		foreach (string text in array)
		{
			if (_Outputs.GetIO(text) == null)
			{
				VmIO item = new VmIO
				{
					Name = text,
					Root = this,
					Parent = this,
					TypeName = "float",
					CustomVisibleA = false,
					CanSubscribeA = false,
					Visible = false,
					VisibleInResultTree = false
				};
				_Outputs.Add(item);
			}
		}
	}

	protected internal virtual void InitOutputDynamicIO()
	{
		try
		{
			ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST moduOutputIOInfo = ServerSDKManager.serverSDKManager.mModuleManager.GetModuOutputIOInfo(ID);
			if (moduOutputIOInfo != null)
			{
				InitDynamicIO(moduOutputIOInfo, _Outputs);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error($"Get output IO fail:{ex.Message},ID={ID}.");
		}
		Outputs.OfType<DynamicVmIO>().ToList().ForEach((DynamicVmIO p) =>
		{
			p.SetIsReturnRelateValueProperty(value: false);
		});
	}

	protected internal virtual void InitInputDynamicIO()
	{
		try
		{
			if (!(this is VmSolution))
			{
				ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST moduInputIOInfo = ServerSDKManager.serverSDKManager.mModuleManager.GetModuInputIOInfo(ID);
				if (moduInputIOInfo != null)
				{
					InitDynamicIO(moduInputIOInfo, _Inputs);
				}
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error($"Get input IO fail:{ex.Message},ID={ID}.");
		}
	}

	/// <summary>
	/// 动态io初始化
	/// </summary>
	protected internal virtual void InitDynamicIO()
	{
		try
		{
			ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST moduInputIOInfo = ServerSDKManager.serverSDKManager.mModuleManager.GetModuInputIOInfo(ID);
			if (moduInputIOInfo != null)
			{
				InitDynamicIO(moduInputIOInfo, _Inputs);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error($"Get input IO fail:{ex.Message},ID={ID}.");
		}
		try
		{
			ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST moduOutputIOInfo = ServerSDKManager.serverSDKManager.mModuleManager.GetModuOutputIOInfo(ID);
			if (moduOutputIOInfo != null)
			{
				InitDynamicIO(moduOutputIOInfo, _Outputs);
			}
		}
		catch (Exception ex2)
		{
			LogHelper.Error($"Get output IO fail:{ex2.Message},ID={ID}.");
		}
		_Outputs.OfType<DynamicVmIO>().ToList().ForEach((DynamicVmIO p) =>
		{
			p.SetIsReturnRelateValueProperty(value: false);
		});
	}

	/// <summary>
	/// 重建叶子IO字典（内部使用）
	/// </summary>
	void IVmModule.RebuildLeafIODictionary()
	{
		if (this is IVmModuleSet)
		{
			leafIOs = leafIOs.ToList().ToDictionary((KeyValuePair<int, VmIO> p) => p.Key, (KeyValuePair<int, VmIO> p) => p.Value);
			leafIOsWithoutTimeStamp = leafIOsWithoutTimeStamp.ToList().ToDictionary((KeyValuePair<int, VmIO> p) => p.Key, (KeyValuePair<int, VmIO> p) => p.Value);
		}
		else
		{
			Outputs.leafIOs = Outputs.leafIOs.ToList().ToDictionary((KeyValuePair<int, VmIO> p) => p.Key, (KeyValuePair<int, VmIO> p) => p.Value);
			Outputs.leafIOsWithoutTimeStamp = Outputs.leafIOsWithoutTimeStamp.ToList().ToDictionary((KeyValuePair<int, VmIO> p) => p.Key, (KeyValuePair<int, VmIO> p) => p.Value);
		}
	}

	/// <summary>
	/// 添加io
	/// </summary>
	/// <param name="allIO"></param>
	internal void AddAllIO(VmIO allIO)
	{
		allIos.Add(allIO.Name.GetHashCode(), allIO);
		allIO.PropertyChanged += AllIO_PropertyChanged;
	}

	/// <summary>
	/// 移除io
	/// </summary>
	/// <param name="allIO"></param>
	internal void RemoveAllIO(VmIO allIO)
	{
		allIos.Remove(allIO.Name.GetHashCode());
		allIO.PropertyChanged -= AllIO_PropertyChanged;
	}

	/// <summary>
	/// io变化事件
	/// </summary>
	private void AllIO_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "Name")
		{
			KeyValuePair<int, VmIO> keyValuePair = allIos.First((KeyValuePair<int, VmIO> p) => p.Value == sender);
			int key = keyValuePair.Key;
			string text = keyValuePair.Value.Name;
			int hashCode = text.GetHashCode();
			allIos.Remove(key);
			allIos.Add(hashCode, keyValuePair.Value);
			if (leafIOsWithoutTimeStamp.TryGetValue(key, out var _))
			{
				leafIOsWithoutTimeStamp.Remove(key);
				leafIOsWithoutTimeStamp.Add(hashCode, keyValuePair.Value);
			}
			if (leafIOs.TryGetValue(key, out var _))
			{
				leafIOs.Remove(key);
				leafIOs.Add(hashCode, keyValuePair.Value);
			}
		}
	}

	internal void AddLeafIO(VmIO leafIO)
	{
		if (!leafIOs.ContainsKey(leafIO.Name.GetHashCode()))
		{
			leafIOs.Add(leafIO.Name.GetHashCode(), leafIO);
			if (!fTimeStamp.Contains(leafIO.Name))
			{
				leafIOsWithoutTimeStamp.Add(leafIO.Name.GetHashCode(), leafIO);
			}
		}
	}

	internal void RemoveLeafIO(VmIO leafIO)
	{
		leafIOs.Remove(leafIO.Name.GetHashCode());
		if (!fTimeStamp.Contains(leafIO.Name))
		{
			leafIOsWithoutTimeStamp.Remove(leafIO.Name.GetHashCode());
		}
	}

	internal virtual void InitDynamicIO(ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST list, VmIOCollection vmIOCollection)
	{
		if (list != null && list.nNum != 0)
		{
			for (int i = 0; i < list.nNum; i++)
			{
				ImvsSdkDefine.IMVS_MODULE_IO_INFO info = (ImvsSdkDefine.IMVS_MODULE_IO_INFO)Marshal.PtrToStructure(list.pstModuIoInfo + i * Marshal.SizeOf(typeof(ImvsSdkDefine.IMVS_MODULE_IO_INFO)), typeof(ImvsSdkDefine.IMVS_MODULE_IO_INFO));
				AnalysisIOInfo(info, vmIOCollection);
			}
		}
	}

	private void RemoveDynamicIO(VmIOCollection vmIOCollection)
	{
		for (int num = vmIOCollection.Count; num > 0; num--)
		{
			if (vmIOCollection[num - 1] is DynamicVmIO dynamicVmIO)
			{
				dynamicVmIO.RemoveSelf();
			}
		}
	}

	internal void RemoveDynamicIn()
	{
		RemoveDynamicIO(Inputs);
	}

	internal void RemoveDynamicOut()
	{
		RemoveDynamicIO(Outputs);
	}

	private VmIO GetInput(string name)
	{
		return Inputs[name];
	}

	private VmIO GetOutput(string name)
	{
		return Outputs[name];
	}

	/// <summary>
	/// 复制粘贴io数据
	/// </summary>
	internal void PasteIO(IList<VmIO> oldIo, IList<VmIO> newIo, Dictionary<object, object> modu_map)
	{
		foreach (VmIO oldIoChild in oldIo)
		{
			VmIO vmIO = newIo.FirstOrDefault((VmIO p) => p.Name == oldIoChild.Name);
			if (vmIO == null)
			{
				continue;
			}
			if (oldIoChild.RelateIO != null)
			{
				if (oldIoChild.RelateIO is VmConstantIO)
				{
					VmConstantIO relateIO = new VmConstantIO
					{
						Name = oldIoChild.RelateIO.Name,
						TypeName = oldIoChild.RelateIO.TypeName,
						Root = vmIO.Root
					};
					vmIO.RelateIO = relateIO;
					continue;
				}
				VmModule vmModule = oldIoChild.RelateIO.Root;
				if (vmModule != null && vmModule.ID == 13000)
				{
					vmIO.RelateIO = oldIoChild.RelateIO;
					continue;
				}
				object value = modu_map.Where((KeyValuePair<object, object> p) => p.Key == oldIoChild.RelateIO.Root).FirstOrDefault().Value;
				if (value == null)
				{
					if (oldIoChild.RelateIO.Name.Contains("TRIGGER_STRING"))
					{
						VmIO relateIO2 = new VmIO
						{
							Name = "TRIGGER_STRING",
							TypeName = "string"
						};
						vmIO.RelateIO = relateIO2;
					}
					else if (oldIoChild.RelateIO.Root.Parent == oldIoChild.Root)
					{
						string fullName = oldIoChild.RelateIO.FullName;
						string text = fullName.Substring(fullName.LastIndexOf(oldIoChild.RelateIO.Root.Name));
						string sub = FullName + "." + text;
						VmIO vmIO2 = (VmIO)VmSolution.Instance[sub];
						if (vmIO2 != null)
						{
							vmIO.RelateIO = vmIO2;
						}
					}
					else if (oldIoChild.RelateIO.Root.Parent == vmIO.Root.Parent)
					{
						if (oldIoChild.RelateIO.Root != oldIoChild.Root.Parent)
						{
							vmIO.RelateIO = oldIoChild.RelateIO;
							vmIO.RelateIndex = oldIoChild.RelateIndex;
						}
					}
					else if (oldIoChild.RelateIO.Root == vmIO.Root.Parent)
					{
						vmIO.RelateIO = oldIoChild.RelateIO;
						vmIO.RelateIndex = oldIoChild.RelateIndex;
					}
				}
				else
				{
					vmIO.RelateIO = (value as VmModule).GetIO(oldIoChild.RelateIO.Name);
					if (oldIoChild.RelateIndex == null)
					{
						vmIO.RelateIndex = null;
					}
					else if (oldIoChild.RelateIndex is VmConstantIO)
					{
						VmConstantIO relateIndex = new VmConstantIO
						{
							Name = oldIoChild.RelateIndex.Name,
							TypeName = oldIoChild.RelateIndex.TypeName
						};
						vmIO.RelateIndex = relateIndex;
					}
					else
					{
						vmIO.RelateIndex = (value as VmModule).GetIO(oldIoChild.RelateIndex.Name);
					}
				}
			}
			else
			{
				PasteIO(oldIoChild.Childs, vmIO.Childs, modu_map);
			}
		}
		relateIO_IsLoaded = true;
	}

	internal VmIO GetLeafIO(string name)
	{
		VmIO leafIO = Outputs.GetLeafIO(name);
		if (leafIO == null)
		{
			leafIO = Inputs.GetLeafIO(name);
		}
		return leafIO;
	}

	private VmIO GetIoByPath(string str)
	{
		if (string.IsNullOrEmpty(str) || !str.Contains('.'))
		{
			return null;
		}
		int num = str.IndexOf(' ', 0);
		if (num < 0)
		{
			return null;
		}
		string s = str.Substring(0, num);
		int result = -1;
		if (!int.TryParse(s, out result))
		{
			return null;
		}
		VmModule aroundModule = GetAroundModule(result);
		if (aroundModule == null)
		{
			return null;
		}
		string[] source = str.Split('.');
		string sub = source.Last();
		return aroundModule[sub] as VmIO;
	}

	private VmModule GetAroundModule(int id)
	{
		if (id == 13000)
		{
			return VmSolution.Instance.Modules.FirstOrDefault((VmModule p) => p.ID == id);
		}
		if (Parent is VmModuleSet vmModuleSet)
		{
			if (vmModuleSet.ID == id)
			{
				return vmModuleSet;
			}
			return vmModuleSet.Modules.FirstOrDefault((VmModule p) => p.ID == id);
		}
		return null;
	}

	/// <summary>
	/// 根据名称获取io
	/// </summary>
	private VmIO GetInputIOWithLeafByName(IList<VmIO> inputs, string inputName)
	{
		foreach (VmIO item in inputs.Where((VmIO p) => !p.IsLeafNode))
		{
			if (item.Name == inputName)
			{
				return item;
			}
			VmIO inputIOWithLeafByName = GetInputIOWithLeafByName(item.Childs, inputName);
			if (inputIOWithLeafByName != null)
			{
				return inputIOWithLeafByName;
			}
		}
		return null;
	}

	/// <summary>
	/// 获得本模块指定类型的组合输出
	/// </summary>
	private IEnumerable<VmIO> GetAllCombinationByType(string typeName, VmIOCollection vmIOCollection)
	{
		List<VmIO> list = new List<VmIO>();
		GetAllCombinationByType(vmIOCollection, typeName, list);
		return list;
	}

	/// <summary>
	/// 获得本模块指定类型的组合输入
	/// </summary>
	private void GetAllCombinationByType(IList<VmIO> inputs, string typeName, IList<VmIO> vmIOs)
	{
		foreach (VmIO item in inputs.Where((VmIO p) => !p.IsLeafNode))
		{
			if (item.TypeName == typeName)
			{
				vmIOs.Add(item);
			}
			GetAllCombinationByType(item.Childs, typeName, vmIOs);
		}
	}

	internal void OnModuleResultCallBack(object result, uint excuteCount)
	{
		ModuleResultCallBackArrived?.Invoke(this, new ValueEventArgs
		{
			Value = result,
			ExecuteCount = excuteCount
		});
	}

	internal void OnAsyncModuleResultCallBackArrived(ValueEventArgs args)
	{
		asyncModuleResultCallBackArrived?.Invoke(this, args);
	}

	/// <summary>
	/// 参数变化
	/// </summary>
	/// <param name="key"></param>
	internal virtual void OnModuleParamChanged(string key)
	{
		ModuleParamChanged?.Invoke(this, new ValueEventArgs
		{
			Value = key
		});
	}

	/// <summary>
	/// 模块执行
	/// </summary>
	public virtual void Run()
	{
		if (!((IVmSolution)VmSolution.Instance).IsContinuousSelfExecute)
		{
			((IVmProcedure)Root)?.SetInnerModuleIsSelfRuning(true);
			IsRunning = true;
		}
		try
		{
			ServerSDKManager.serverSDKManager.mModuleManager.SelfRun(moduleInfo.nNodeID);
		}
		catch (VmException ex)
		{
			((IVmProcedure)Root)?.SetInnerModuleIsSelfRuning(false);
			IsRunning = false;
			throw ex;
		}
	}

	/// <summary>
	/// 刷新配置状态
	/// </summary>
	public virtual void ReFlashReadyState()
	{
		IsReady = Inputs.IsReady;
	}

	/// <summary>
	/// 强刷状态用于userdata与isready不同步的情况
	/// </summary>
	private void ReflashReadyStateToServer()
	{
		ReFlashReadyState();
		UserData["IsReady"] = ((!IsReady.Value) ? new byte[1] : new byte[1] { 1 });
	}

	/// <summary>
	/// 获取io节点
	/// </summary>
	public override object GetNode(string key)
	{
		if (!(key == "Outputs"))
		{
			if (key == "Inputs")
			{
				return Inputs;
			}
			return GetIO(key);
		}
		return Outputs;
	}

	/// <summary>
	/// 移除输入输出io
	/// </summary>
	public void RemoveIo(string ioName, VmIO io)
	{
		if (ioName == "Input")
		{
			io.RelateIO = null;
			Inputs.Remove(io);
		}
		else if (ioName == "Output")
		{
			Outputs.Remove(io);
		}
	}

	/// <summary>
	/// 强制刷新结果
	/// </summary>
	public void RefreshResultForce()
	{
		if (Root != null && !ReportResultEnable)
		{
			RefreshResult();
		}
	}

	/// <summary>
	/// 初始化全局变量模块的io
	/// </summary>
	public virtual void InitGlobalIo()
	{
	}

	/// <summary>
	/// 获取可订阅模块信息
	/// </summary>
	/// <returns></returns>
	public IEnumerable<VmModule> GetSubscribleModules(bool isOutput)
	{
		IEnumerable<VmModule> enumerable = null;
		if (isOutput)
		{
			enumerable = ((!(this is VmModuleSet)) ? new object[1] { Parent }.Concat((Parent as VmModuleSet).Modules).OfType<VmModule>() : (this as VmModuleSet).Modules);
		}
		else
		{
			enumerable = new object[1] { Parent }.Concat((Parent as VmModuleSet).Modules).OfType<VmModule>();
			if (this is IMVSGroup { IsEnableLoop: not false } iMVSGroup)
			{
				enumerable = enumerable.Concat(iMVSGroup.Modules);
			}
		}
		return enumerable.Concat(VmSolution.Instance.Modules.Where((VmModule p) => p.ID == 13000));
	}

	/// <summary>
	/// 获取所有userdata
	/// </summary>
	/// <returns></returns>
	public object GetAllUserData()
	{
		return ServerSDKManager.serverSDKManager.mModuleManager.GetUIParamValueByObject(ID)?.Clone();
	}

	/// <summary>
	/// 获取模型
	/// </summary>
	/// <returns></returns>
	public virtual byte[] GetModel()
	{
		return null;
	}

	/// <summary>
	/// 导入模型
	/// </summary>
	/// <param name="model"></param>
	public virtual void ImportModel(object model)
	{
	}

	/// <summary>
	/// 导入流程或Group时做特殊处理
	/// </summary>
	public virtual void ImportProcess()
	{
	}

	/// <summary>
	/// 获取订阅模块列表
	/// </summary>
	/// <returns></returns>
	public List<VmModule> GetRelatedModules()
	{
		List<VmModule> list = new List<VmModule>();
		foreach (VmIO input in Inputs)
		{
			VmModule vmModule = input.RelateIO?.Root;
			if (vmModule != null && !list.Contains(vmModule))
			{
				list.Add(vmModule);
			}
		}
		return list;
	}

	/// <summary>
	/// 从输入和输入中查找一个名为name的IO对象，输入输出默认不同名
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public VmIO GetIO(string name)
	{
		if (!string.IsNullOrEmpty(name) && name[0] == '(' && name[name.Length - 1] == ')')
		{
			name = name.Substring(1, name.Length - 2);
		}
		VmIO output = GetOutput(name);
		if (output != null)
		{
			return output;
		}
		return GetInput(name);
	}

	/// <summary>
	/// 根据名称获得组合输入
	/// </summary>
	/// <returns></returns>
	public VmIO GetInputIOWithLeafByName(string inputName)
	{
		return GetInputIOWithLeafByName(Inputs, inputName);
	}

	/// <summary>
	/// 根据名称获得组合输出
	/// </summary>
	/// <returns></returns>
	public VmIO GetOutputIOWithLeafByName(string inputName)
	{
		return GetInputIOWithLeafByName(Outputs, inputName);
	}

	/// <summary>
	/// 获取连接到本模块的连线
	/// </summary>
	/// <returns></returns>
	public IList GetPreModules()
	{
		return PreModules;
	}

	/// <summary>
	/// 获取前置模块
	/// </summary>
	public void DeGetFrontOutputItems(IList allInputDiagramItem)
	{
		IList preModules = GetPreModules();
		foreach (VmModule item in preModules)
		{
			if (!allInputDiagramItem.Contains(item))
			{
				allInputDiagramItem.Add(item);
				item.DeGetFrontOutputItems(allInputDiagramItem);
			}
		}
	}

	/// <summary>
	/// 获取前置模块
	/// </summary>
	public void GetFrontOutputItems(IList allInputDiagramItem)
	{
		DeGetFrontOutputItems(allInputDiagramItem);
		if (Parent != null && !(Parent is VmSolution))
		{
			allInputDiagramItem.Add(Parent);
		}
	}

	/// <summary>
	/// 获取前置模块
	/// </summary>
	/// <param name="predicate"></param>
	/// <returns></returns>
	public List<VmModule> GetFrontParamItems()
	{
		List<VmModule> list = new List<VmModule>();
		GetFrontOutputItems(list);
		return list;
	}

	/// <summary>
	/// 获取本模块后续的连线
	/// </summary>
	/// <returns></returns>
	public IList GetNextModules()
	{
		return NextModules;
	}

	/// <summary>
	/// 获取后置模块
	/// </summary>
	public void DeGetBackOutputItems(IList allInputDiagramItem)
	{
		IList nextModules = GetNextModules();
		foreach (VmModule item in nextModules)
		{
			if (!allInputDiagramItem.Contains(item))
			{
				allInputDiagramItem.Add(item);
				item.DeGetBackOutputItems(allInputDiagramItem);
			}
		}
	}

	/// <summary>
	/// 获取前模块与当前模块之间的所有路径上模块(包括起始自身)
	/// </summary>
	/// <param name="startModule"> 起始模块</param>
	/// <returns></returns>
	public List<VmModule> GetFrontToCurParamItems(VmModule startModule)
	{
		List<VmModule> list = new List<VmModule>();
		List<VmModule> list2 = new List<VmModule>();
		startModule.DeGetBackOutputItems(list2);
		list2.Add(startModule);
		foreach (object frontModule in FrontModules)
		{
			if (list2.Contains(frontModule) && !list.Contains(frontModule) && frontModule is VmModule item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	/// <summary>
	/// 获得前项模块中对应类型的组合输出
	/// </summary>
	/// <param name="typeName"></param>
	/// <returns></returns>
	public IList GetFrontIOWithLeafItemsByName(string typeName)
	{
		List<VmIO> list = new List<VmIO>();
		List<VmModule> frontParamItems = GetFrontParamItems();
		if (VmSolution.Instance.AllModules.ContainsKey(13000u))
		{
			frontParamItems.Add(VmSolution.Instance.AllModules[13000u]);
		}
		IVmModule vmModule = ((this is IVmModuleSet) ? (this as IVmModuleSet).LocalVariablModule : (Parent as IVmModuleSet).LocalVariablModule);
		if (vmModule != null)
		{
			frontParamItems.Add(vmModule as VmModule);
		}
		foreach (VmModule item in frontParamItems)
		{
			if (item == Parent)
			{
				list.AddRange(item.GetAllCombinationByType(typeName, item.Inputs));
			}
			else
			{
				list.AddRange(item.GetAllCombinationByType(typeName, item.Outputs));
			}
		}
		return (from p in list
			where p.CanSubscribeA && p.Visible
			select p.ModulePathName.ToString()).ToList();
	}

	/// <summary>
	/// 导入模块
	/// </summary>
	/// <returns></returns>
	public virtual object Import(ImvsSdkDefine.IMVS_IMPORT_MODULE_DATA_INFO info, ModelBase moduset)
	{
		IList children = (moduset as IChildren).Children;
		Init(ServerSDKManager.serverSDKManager.mModuleManager.ImportModuleDataEx(info), moduset);
		VmSolution.Instance.AllModules.AddOrUpdate(ID, this, (uint oldKey, VmModule oldValue) => this);
		return null;
	}

	/// <summary>
	/// 复制参数
	/// </summary>
	/// <returns></returns>
	public bool PasteParam(object copyInfo)
	{
		try
		{
			string sub = (string)copyInfo;
			object obj = VmSolution.Instance[sub];
			if (obj is VmModule vmModule)
			{
				if (vmModule.StrModuleName != StrModuleName)
				{
					return false;
				}
				if (vmModule == this)
				{
					return false;
				}
				object moduleAllParams = GetModuleAllParams();
				string displayName = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), FullName, AppLocalizationService.GetLang("Tab_Params"));
				Ioc.Get<IUnRedoService>()?.Register(new ParamUnRedoOperation(Param_Redo, Param_Undo, Param_Update, new UnRedo_ParamsData
				{
					ModuleFullName = FullName,
					OldParamList = moduleAllParams,
					NewParamList = null
				}), displayName);
				ServerSDKManager.serverSDKManager.mModuleManager.CopyModuleAlgParam(vmModule.ID, ID);
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add(vmModule, this);
				if (vmModule.Params.Data != null)
				{
					(vmModule.Params.Data as IUpdateItemRoot)?.UpdateSingleParamBind(vmModule, this);
				}
				return true;
			}
		}
		catch (VmException)
		{
		}
		return false;
	}

	/// <summary>
	/// 重置算法参数
	/// </summary>
	/// <returns></returns>
	public void ResetParam()
	{
		try
		{
			ServerSDKManager.serverSDKManager.mModuleManager.SetDefaultParamList(ID);
		}
		catch (VmException ex)
		{
			throw new Exception(ErrorCodeInfo.GetErrorInfo(ex.errorCode));
		}
	}

	/// <summary>
	/// 释放 dispose
	/// </summary>
	public virtual void Dispose()
	{
		InnerDispose();
	}

	private void InnerDispose()
	{
		try
		{
			IsOnDispose = true;
			if (disposedValue)
			{
				return;
			}
			Dictionary<uint, List<uint?>> nModuleIDList = new Dictionary<uint, List<uint?>>();
			IVmModule vmModule;
			if (this is IVMGroup)
			{
				GetAllModulesIDFroVar(ref nModuleIDList);
			}
			else if ((vmModule = this) != null && vmModule.Parent is VmModuleSet vmModuleSet)
			{
				nModuleIDList.Add(vmModuleSet.ID, new List<uint?>
				{
					(vmModule.Parent as IVmModuleSet).LocalVariablModule?.ID,
					ID
				});
			}
			ClearVarBindResultRelation(nModuleIDList);
			object obj = null;
			if (!IsGlobalModu)
			{
				IUnRedoService unRedoService = Ioc.Get<IUnRedoService>();
				if (unRedoService != null && unRedoService.IsNeedExport())
				{
					obj = Export();
				}
			}
			if (this is VmModuleSet vmModuleSet2)
			{
				vmModuleSet2.DeleteInnerModules();
			}
			VmModuleSet vmModuleSet3 = Parent as VmModuleSet;
			for (int num = PreModules.Count - 1; num >= 0; num--)
			{
				PreModules[num].NextModules.Remove(this);
			}
			DeleteModuleParamBind(this, this);
			if (Params.Data is IUpdateItemRoot)
			{
				RegisterSubParamFunc(this);
			}
			CommonDispose();
			vmModuleSet3.Modules.Remove(this);
			VmSolution.Instance.AllModules.TryRemove(ID, out var _);
			CloseModuleInterface();
			IsReady = null;
			disposedValue = true;
			if (obj != null)
			{
				string displayName = string.Format("{0}{1}:{2}", AppLocalizationService.GetLang("Lang_Delet"), AppLocalizationService.GetLang("Lang_Modu"), FullName.Substring(FullName.LastIndexOf('.') + 1));
				Ioc.Get<IUnRedoService>()?.Register(new ModuleUnRedoOperation(Delete_Module, Import_Module, obj as VmModuleDataInfo), displayName);
			}
			LogHelper.Info("Delete module:" + Name);
			((IVmSolution)VmSolution.Instance).OnModuleActionChanged("DeleteModule", FullName, string.Empty);
			if (isLoadFailed != false)
			{
				vmModuleSet3.ReFlashLoadFailed();
			}
		}
		catch (Exception ex)
		{
			IsOnDispose = false;
			throw VmSolution.GetVmException(ex);
		}
	}

	/// <summary>
	/// 重写tostring
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return UniqueName;
	}

	/// <summary>
	/// 懒加载订阅关系，如果一个模块的其中一个输入需要加载，则加载整个模块的订阅，因为只有以模块为单位的订阅加载
	/// </summary>
	public void LoadModuleRelateIO()
	{
		ImvsSdkDefine.IMVS_MODULE_ID_LIST stModuleId = new ImvsSdkDefine.IMVS_MODULE_ID_LIST
		{
			nNum = 1,
			anModuleId = new uint[2064]
		};
		stModuleId.anModuleId[0] = ID;
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST multiModuleSubscribe = ServerSDKManager.serverSDKManager.mModuleManager.GetMultiModuleSubscribe(stModuleId);
		ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST;
		if ((iMVS_MODULE_SUBSCRIBE_INFO_LIST = multiModuleSubscribe) == null)
		{
			return;
		}
		ImvsSdkDefine.IMVS_SUBSCRIBE_INFO[] array = iMVS_MODULE_SUBSCRIBE_INFO_LIST.pstSubscribeInfo();
		for (int i = 0; i < iMVS_MODULE_SUBSCRIBE_INFO_LIST.nNum; i++)
		{
			ImvsSdkDefine.IMVS_SUBSCRIBE_INFO subScribeInfo = array[i];
			string text = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strSubInput);
			string text2 = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strPubOutput);
			string text3 = Hik.Util.StringUtil.UTF8GetString(subScribeInfo.strIndex);
			uint nIndexModuleID = subScribeInfo.nIndexModuleID;
			uint nIsIndexConstant = subScribeInfo.nIsIndexConstant;
			VmIO leafIO = GetLeafIO(text);
			if (leafIO == null)
			{
				continue;
			}
			IEnumerable<VmModule> subscribleModules = GetSubscribleModules(leafIO.IsOutput);
			VmModule vmModule = subscribleModules.FirstOrDefault((VmModule p) => p.ID == subScribeInfo.nPubModuleID);
			VmIO vmIO = null;
			if (subScribeInfo.nIsPubConstant == 1)
			{
				VmConstantIO vmIO2 = new VmConstantIO
				{
					Name = text2,
					TypeName = leafIO.TypeName,
					Root = this
				};
				leafIO.InitRelateIO(vmIO2);
				continue;
			}
			if (subScribeInfo.nPubModuleID == 990000)
			{
				vmIO = new VmIO
				{
					Name = "TRIGGER_STRING",
					TypeName = "string"
				};
			}
			else if (subScribeInfo.nPubModuleID == 13000)
			{
				vmIO = vmModule.GetLeafIO(text2);
				if (vmIO == null)
				{
					vmModule.InitGlobalIo();
					vmIO = vmModule.GetLeafIO(text2);
				}
			}
			else if (ServerSDKManager.serverSDKManager.mModuleManager.CheckLocalVariableModule((int)subScribeInfo.nPubModuleID))
			{
				IVmModule vmModule2 = null;
				if (!(this is IVMGroup) || leafIO.IsOutput)
				{
					vmModule2 = ((this is IVmModuleSet) ? (this as IVmModuleSet).LocalVariablModule : (Parent as IVmModuleSet).LocalVariablModule);
				}
				else
				{
					vmModule2 = (Parent as IVmModuleSet)?.LocalVariablModule;
				}
				if (vmModule2 != null && vmModule2 is VmModule vmModule3 && vmModule3.ID == subScribeInfo.nPubModuleID)
				{
					vmModule = vmModule3;
					vmModule.InitGlobalIo();
					vmIO = vmModule.GetLeafIO(text2);
				}
			}
			else if (subScribeInfo.nPubModuleID >= 26000 && subScribeInfo.nPubModuleID <= 27999)
			{
				object obj = ((this is VmModuleSet) ? (this as VmModuleSet).ProcedureCommunicaitonModule : (Parent as VmModuleSet).ProcedureCommunicaitonModule);
				if (obj != null && obj is VmModule vmModule4 && vmModule4.ID == subScribeInfo.nPubModuleID)
				{
					vmModule = vmModule4;
					vmIO = vmModule.GetLeafIO(text2);
				}
			}
			else
			{
				vmIO = vmModule?.GetLeafIO(text2);
			}
			if (leafIO.RelateIO != null)
			{
				continue;
			}
			leafIO.InitRelateIO(vmIO);
			if (string.IsNullOrEmpty(text3) || !(text3 != "All"))
			{
				continue;
			}
			if (nIsIndexConstant == 1)
			{
				VmConstantIO vmIO3 = new VmConstantIO
				{
					Name = text3
				};
				leafIO.InitRelateIndexIO(vmIO3);
				continue;
			}
			VmModule vmModule5 = subscribleModules.FirstOrDefault((VmModule p) => p.ID == nIndexModuleID);
			VmIO leafIO2 = vmModule5.GetLeafIO(text3);
			if (leafIO2 == null && vmModule5.ID == 13000)
			{
				vmModule5.InitGlobalIo();
				leafIO2 = vmModule5.GetLeafIO(text3);
			}
			leafIO.InitRelateIndexIO(leafIO2);
		}
	}

	/// <summary>
	/// 设置图像缓存节点数量
	/// </summary>
	/// <param name="nBufferNodeNum">节点数量</param>
	public void SetBufferNodeNum(uint nBufferNodeNum)
	{
		ServerSDKManager.serverSDKManager.mModuleManager.SetImageBufferNodeNum(ID, nBufferNodeNum);
	}

	/// <summary>
	/// 保持模块上次结果
	/// </summary>
	/// <param name="bIsUnchanged">是否保持</param>
	public void KeepModuleLastResult(bool bIsUnchanged)
	{
		ServerSDKManager.serverSDKManager.mModuleManager.KeepModuleLastResult(ID, bIsUnchanged);
	}

	/// <summary>
	/// 二次开发使用，开启模块结果回调
	/// </summary>
	public void EnableResultCallback()
	{
		ServerSDKManager.serverSDKManager.mSolutionManager.SetReportModuleResult_V2(ImvsSdkDefine.IMVS_REPORT_RESULT_TYPE.REPORT_RESULT_TYPE_PART, ID, bIsEnable: true);
		isSetCallBack = 1;
	}

	private VmException ThrowTypeOfIOError()
	{
		return new VmException(-536870911, "The IO With UnMatched Type");
	}

	private VmException ThrowNotFindIOError()
	{
		return new VmException(-536870911, "Cannot Find IO From Name");
	}

	/// <summary>
	/// 获取IO整型值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public int?[] GetVmIOIntValue(string vmIOName)
	{
		InitCurrentIO();
		if (leafIOs.TryGetValue(vmIOName.GetHashCode(), out var value))
		{
			if (value.TypeName.ToLower() == "INT".ToLower())
			{
				return value.Value as int?[];
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 获取IO单精度值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public float?[] GetVmIOFloatValue(string vmIOName)
	{
		InitCurrentIO();
		if (leafIOs.TryGetValue(vmIOName.GetHashCode(), out var value))
		{
			if (value.TypeName.ToLower() == "FLOAT".ToLower())
			{
				return value.Value as float?[];
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 获取IO字符串值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public string[] GetVmIOStringValue(string vmIOName)
	{
		InitCurrentIO();
		if (leafIOs.TryGetValue(vmIOName.GetHashCode(), out var value))
		{
			if (value.TypeName.ToLower() == "STRING".ToLower())
			{
				if (value.Value is ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[] array)
				{
					string[] array2 = new string[array.Length];
					int num = 0;
					ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[] array3 = array;
					for (int i = 0; i < array3.Length; i++)
					{
						ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX iMVS_MODULE_STRING_VALUE_EX = array3[i];
						array2[num] = iMVS_MODULE_STRING_VALUE_EX.strValue;
						num++;
					}
					return array2;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	private unsafe VmDataType.VmLevelPoint[] GetLevelPointFromData(ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data strs)
	{
		int num = Marshal.SizeOf<IMVS_PATMATCH_POINT_INFO>();
		byte[] data = strs.GetData();
		fixed (byte* ptr = data)
		{
			IMVS_PATMATCH_POINT_INFO* tempExternPoint_Bufferptr = (IMVS_PATMATCH_POINT_INFO*)ptr;
			int num2 = data.Length / num;
			VmDataType.VmLevelPoint[] res = new VmDataType.VmLevelPoint[num2];
			Parallel.For(0, num2, (int i) =>
			{
				IMVS_PATMATCH_POINT_INFO iMVS_PATMATCH_POINT_INFO = tempExternPoint_Bufferptr[i];
				VmDataType.VmLevelPoint vmLevelPoint = new VmDataType.VmLevelPoint
				{
					point = new VmDataType.VmPoint
					{
						x = iMVS_PATMATCH_POINT_INFO.MatchOutlineX,
						y = iMVS_PATMATCH_POINT_INFO.MatchOutlineY
					},
					vmLevelEnum = ((iMVS_PATMATCH_POINT_INFO.MatchOutlineScore == 0f) ? VmLevelEnum.NORMAL : ((iMVS_PATMATCH_POINT_INFO.MatchOutlineScore == 1f) ? VmLevelEnum.BELOWNORMAL : VmLevelEnum.BED))
				};
				res[i] = vmLevelPoint;
			});
			return res;
		}
	}

	/// <summary>
	/// 获取IO点集值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmLevelPoint[][] GetVmIOPointSetValue(string vmIOName)
	{
		InitCurrentIO();
		if (leafIOs.TryGetValue(vmIOName.GetHashCode(), out var value))
		{
			if (value.TypeName.ToLower() == "pointset".ToLower())
			{
				if (value.Value is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data[] array)
				{
					VmDataType.VmLevelPoint[][] array2 = new VmDataType.VmLevelPoint[array.Length][];
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null)
						{
							array2[i] = GetLevelPointFromData(array[i]);
						}
					}
					return array2;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 获取IO图像缓存
	/// </summary>
	/// <param name="vmIOName"></param>
	/// <returns></returns>
	public byte[] GetVmIOImageBufferValue(string vmIOName)
	{
		InitCurrentIO();
		if (leafIOs.TryGetValue(vmIOName.GetHashCode(), out var value))
		{
			if (value.TypeName.ToLower() == "image".ToLower())
			{
				if (value.Value is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data[] source)
				{
					return source.First().GetData();
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw ThrowNotFindIOError();
	}

	/// <summary>
	/// 通过名称获取对应IO(动态IO的名称存在%号)
	/// </summary>
	/// <param name="vmIOName"></param>
	public bool GetIOByName(string vmIOName, out VmIO io)
	{
		InitCurrentIO();
		if (allIos.TryGetValue(vmIOName.GetHashCode(), out io))
		{
			return true;
		}
		vmIOName = $"%{vmIOName}%";
		return allIos.TryGetValue(vmIOName.GetHashCode(), out io);
	}

	/// <summary>
	/// 通过名称获取对应输出IO(动态IO的名称存在%号)
	/// </summary>
	/// <param name="vmIOName"></param>
	public bool GetOutputIOByName(string vmIOName, out VmIO io)
	{
		InitCurrentIO();
		if (allIos.TryGetValue(vmIOName.GetHashCode(), out io))
		{
			VmIO obj = io;
			if (obj != null && obj.IsOutput)
			{
				return true;
			}
		}
		else
		{
			vmIOName = $"%{vmIOName}%";
			if (allIos.TryGetValue(vmIOName.GetHashCode(), out io))
			{
				VmIO obj2 = io;
				if (obj2 != null && obj2.IsOutput)
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 获取IO图像值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmImageData GetVmIOImageValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "IMAGE")
			{
				VmDataType.VmImageData vmImageData = new VmDataType.VmImageData();
				object obj = (io.Childs[0].Value as Array)?.GetValue(0);
				if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
				{
					vmImageData.data = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(out var lenth);
					vmImageData.dataLenth = lenth;
					vmImageData.width = (int)(io.Childs[1].Value as Array).GetValue(0);
					vmImageData.height = (int)(io.Childs[2].Value as Array).GetValue(0);
					vmImageData.imageFormat = (int)(io.Childs[3].Value as Array).GetValue(0);
					return vmImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	public IVmImageData GetImageByteData(string vmIOName, ref byte[] imageByteData, ref string sharedMemAddress)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "IMAGE")
			{
				VmDataType.VmImageData vmImageData = new VmDataType.VmImageData();
				object obj = (io.Childs[0].Value as Array)?.GetValue(0);
				if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
				{
					if (ModuResultMemoryTypeEnum.Memory_Shallow_Copy == ModuResultMemType)
					{
						if (iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryType == 0 || 2 == iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryType)
						{
							sharedMemAddress = iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryAddress;
						}
					}
					else
					{
						imageByteData = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(bUseMemPool: false);
					}
					vmImageData.data = IntPtr.Zero;
					vmImageData.dataLenth = 0;
					vmImageData.width = (int)(io.Childs[1].Value as Array).GetValue(0);
					vmImageData.height = (int)(io.Childs[2].Value as Array).GetValue(0);
					vmImageData.imageFormat = (int)(io.Childs[3].Value as Array).GetValue(0);
					return vmImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO像素图像值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmPixelImageData GetVmIOPixelImageValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "PIXELIMAGE")
			{
				VmDataType.VmPixelImageData vmPixelImageData = new VmDataType.VmPixelImageData();
				object obj = (io.Childs[0].Children[0].Value as Array)?.GetValue(0);
				int?[] array = io.Childs[1].Childs[1].Value as int?[];
				if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
				{
					vmPixelImageData.imageData = new VmDataType.VmImageData();
					vmPixelImageData.imageData.data = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(out var lenth);
					vmPixelImageData.imageData.dataLenth = lenth;
					vmPixelImageData.imageData.width = (int)(io.Childs[0].Childs[1].Value as Array).GetValue(0);
					vmPixelImageData.imageData.height = (int)(io.Childs[0].Childs[2].Value as Array).GetValue(0);
					vmPixelImageData.imageData.imageFormat = (int)(io.Childs[0].Childs[3].Value as Array).GetValue(0);
					if (array != null)
					{
						vmPixelImageData.classInfo = new VmDataType.VmClassInfo[array.Length];
						for (int i = 0; i < vmPixelImageData.classInfo.Length; i++)
						{
							VmDataType.VmClassInfo vmClassInfo = new VmDataType.VmClassInfo();
							vmClassInfo.className = ((ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX)(io.Childs[1].Childs[0].Value as Array).GetValue(i)).strValue;
							vmClassInfo.grayValue = (int)(io.Childs[1].Childs[1].Value as Array).GetValue(i);
							vmPixelImageData.classInfo[i] = vmClassInfo;
						}
					}
					return vmPixelImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	public IVmPixelImageData GetPixelImageByteData(string vmIOName, ref byte[] imageByteData, ref string sharedMemAddress)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "PIXELIMAGE")
			{
				VmDataType.VmPixelImageData vmPixelImageData = new VmDataType.VmPixelImageData();
				object obj = (io.Childs[0].Children[0].Value as Array)?.GetValue(0);
				int?[] array = io.Childs[1].Childs[1].Value as int?[];
				if (obj is ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data iMVS_MODULE_CHUNK_VALUE_EX_Data)
				{
					vmPixelImageData.imageData = new VmDataType.VmImageData();
					if (ModuResultMemoryTypeEnum.Memory_Shallow_Copy == ModuResultMemType)
					{
						if (iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryType == 0 || 2 == iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryType)
						{
							sharedMemAddress = iMVS_MODULE_CHUNK_VALUE_EX_Data.MemoryAddress;
						}
					}
					else
					{
						imageByteData = iMVS_MODULE_CHUNK_VALUE_EX_Data.GetData(bUseMemPool: false);
					}
					vmPixelImageData.imageData.data = IntPtr.Zero;
					vmPixelImageData.imageData.dataLenth = 0;
					vmPixelImageData.imageData.width = (int)(io.Childs[0].Childs[1].Value as Array).GetValue(0);
					vmPixelImageData.imageData.height = (int)(io.Childs[0].Childs[2].Value as Array).GetValue(0);
					vmPixelImageData.imageData.imageFormat = (int)(io.Childs[0].Childs[3].Value as Array).GetValue(0);
					if (array != null)
					{
						vmPixelImageData.classInfo = new VmDataType.VmClassInfo[array.Length];
						for (int i = 0; i < vmPixelImageData.classInfo.Length; i++)
						{
							VmDataType.VmClassInfo vmClassInfo = new VmDataType.VmClassInfo();
							vmClassInfo.className = ((ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX)(io.Childs[1].Childs[0].Value as Array).GetValue(i)).strValue;
							vmClassInfo.grayValue = (int)(io.Childs[1].Childs[1].Value as Array).GetValue(i);
							vmPixelImageData.classInfo[i] = vmClassInfo;
						}
					}
					return vmPixelImageData;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的ROI值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmROIBox[] GetVmIORoiBoxValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "ROIBOX")
			{
				if (io.Childs[0].Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[0].Childs[1].Value as float?[];
					float?[] array3 = io.Childs[1].Value as float?[];
					float?[] array4 = io.Childs[2].Value as float?[];
					float?[] array5 = io.Childs[3].Value as float?[];
					VmDataType.VmROIBox[] array6 = new VmDataType.VmROIBox[array.Length];
					for (int i = 0; i < array6.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmROIBox vmROIBox = new VmDataType.VmROIBox();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							vmROIBox.centerPoint = vmPoint;
							vmROIBox.width = array3[i].Value;
							vmROIBox.height = array4[i].Value;
							vmROIBox.angle = array5[i].Value;
							array6[i] = vmROIBox;
						}
					}
					return array6;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的Point值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmPoint[] GetVmIOPointValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "POINT")
			{
				if (io.Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[1].Value as float?[];
					VmDataType.VmPoint[] array3 = new VmDataType.VmPoint[array.Length];
					for (int i = 0; i < array3.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							array3[i] = vmPoint;
						}
					}
					return array3;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的Line值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmLine[] GetVmIOLineValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "LINE")
			{
				if (io.Childs[0].Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[0].Childs[1].Value as float?[];
					float?[] array3 = io.Childs[1].Childs[0].Value as float?[];
					float?[] array4 = io.Childs[1].Childs[1].Value as float?[];
					VmDataType.VmLine[] array5 = new VmDataType.VmLine[array.Length];
					for (int i = 0; i < array5.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmLine vmLine = new VmDataType.VmLine();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							VmDataType.VmPoint vmPoint2 = new VmDataType.VmPoint();
							vmPoint2.x = array3[i].Value;
							vmPoint2.y = array4[i].Value;
							vmLine.startPoint = vmPoint;
							vmLine.endPoint = vmPoint2;
							array5[i] = vmLine;
						}
					}
					return array5;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的Line值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmCircle[] GetVmIOCircleValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "CIRCLE")
			{
				if (io.Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[1].Childs[0].Value as float?[];
					float?[] array3 = io.Childs[1].Childs[1].Value as float?[];
					VmDataType.VmCircle[] array4 = new VmDataType.VmCircle[array.Length];
					for (int i = 0; i < array4.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmCircle vmCircle = new VmDataType.VmCircle();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array2[i].Value;
							vmPoint.y = array3[i].Value;
							vmCircle.centerPoint = vmPoint;
							vmCircle.radius = array[i].Value;
							array4[i] = vmCircle;
						}
					}
					return array4;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的Ellipse值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmEllipse[] GetVmIOEllipseValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "ELLIPSE")
			{
				if (io.Childs[0].Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[0].Childs[1].Value as float?[];
					float?[] array3 = io.Childs[1].Value as float?[];
					float?[] array4 = io.Childs[2].Value as float?[];
					float?[] array5 = io.Childs[3].Value as float?[];
					VmDataType.VmEllipse[] array6 = new VmDataType.VmEllipse[array.Length];
					for (int i = 0; i < array6.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmEllipse vmEllipse = new VmDataType.VmEllipse();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							vmEllipse.centerPoint = vmPoint;
							vmEllipse.majorradius = array3[i].Value;
							vmEllipse.minorradius = array4[i].Value;
							vmEllipse.angle = array5[i].Value;
							array6[i] = vmEllipse;
						}
					}
					return array6;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的位置修正值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmFixture[] GetVmIOFixtureValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "FIXTURE")
			{
				if (io.Childs[0].Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[0].Childs[1].Value as float?[];
					float?[] array3 = io.Childs[1].Value as float?[];
					float?[] array4 = io.Childs[2].Value as float?[];
					float?[] array5 = io.Childs[3].Value as float?[];
					float?[] array6 = io.Childs[4].Childs[0].Value as float?[];
					float?[] array7 = io.Childs[4].Childs[1].Value as float?[];
					float?[] array8 = io.Childs[5].Value as float?[];
					float?[] array9 = io.Childs[6].Value as float?[];
					float?[] array10 = io.Childs[7].Value as float?[];
					VmDataType.VmFixture[] array11 = new VmDataType.VmFixture[array.Length];
					for (int i = 0; i < array11.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmFixture vmFixture = new VmDataType.VmFixture();
							VmDataType.VmFixturePointInfo vmFixturePointInfo = new VmDataType.VmFixturePointInfo();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							vmFixturePointInfo.point = vmPoint;
							vmFixturePointInfo.angle = array3[i].Value;
							vmFixturePointInfo.scaleX = array4[i].Value;
							vmFixturePointInfo.scaleY = array5[i].Value;
							vmFixture.fixtureInfo = vmFixturePointInfo;
							VmDataType.VmFixturePointInfo vmFixturePointInfo2 = new VmDataType.VmFixturePointInfo();
							VmDataType.VmPoint vmPoint2 = new VmDataType.VmPoint();
							vmPoint2.x = array6[i].Value;
							vmPoint2.y = array7[i].Value;
							vmFixturePointInfo2.point = vmPoint2;
							vmFixturePointInfo2.angle = array8[i].Value;
							vmFixturePointInfo2.scaleX = array9[i].Value;
							vmFixturePointInfo2.scaleY = array10[i].Value;
							vmFixture.unFixtureInfo = vmFixturePointInfo2;
							array11[i] = vmFixture;
						}
					}
					return array11;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的ROIAnnulus值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmROIAnnulus[] GetVmIOROIAnnulusValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "ROIANNULUS")
			{
				if (io.Childs[0].Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[0].Childs[1].Value as float?[];
					float?[] array3 = io.Childs[1].Value as float?[];
					float?[] array4 = io.Childs[2].Value as float?[];
					float?[] array5 = io.Childs[3].Value as float?[];
					float?[] array6 = io.Childs[4].Value as float?[];
					VmDataType.VmROIAnnulus[] array7 = new VmDataType.VmROIAnnulus[array.Length];
					for (int i = 0; i < array7.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmROIAnnulus vmROIAnnulus = new VmDataType.VmROIAnnulus();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							vmROIAnnulus.centerPoint = vmPoint;
							vmROIAnnulus.innerRadius = array3[i].Value;
							vmROIAnnulus.outerRadius = array4[i].Value;
							vmROIAnnulus.startAngle = array5[i].Value;
							vmROIAnnulus.angleExtend = array6[i].Value;
							array7[i] = vmROIAnnulus;
						}
					}
					return array7;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的ContourPoints值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmContourPoints[] GetVmIOContourPointsValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "CONTOURPOINTS")
			{
				if (io.Childs[1].Value is int?[] array)
				{
					ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data[] array2 = io.Childs[0].Value as ImvsSdkDefine.IMVS_MODULE_CHUNK_VALUE_EX_Data[];
					VmDataType.VmContourPoints[] array3 = new VmDataType.VmContourPoints[array.Length];
					for (int i = 0; i < array3.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmLevelPoint[] levelPointFromData = GetLevelPointFromData(array2[i]);
							VmDataType.VmContourPoints vmContourPoints = new VmDataType.VmContourPoints();
							IVmLevelPoint[] levelPoint = levelPointFromData;
							vmContourPoints.levelPoint = levelPoint;
							vmContourPoints.num = array[i].Value;
							array3[i] = vmContourPoints;
						}
					}
					return array3;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的Rect值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmRect[] GetVmIORectValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "Rect")
			{
				if (io.Childs[0].Childs[0].Value is float?[] array)
				{
					float?[] array2 = io.Childs[0].Childs[1].Value as float?[];
					float?[] array3 = io.Childs[1].Value as float?[];
					float?[] array4 = io.Childs[2].Value as float?[];
					VmDataType.VmRect[] array5 = new VmDataType.VmRect[array.Length];
					for (int i = 0; i < array5.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmRect vmRect = new VmDataType.VmRect();
							VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
							vmPoint.x = array[i].Value;
							vmPoint.y = array2[i].Value;
							vmRect.centerPoint = vmPoint;
							vmRect.width = array3[i].Value;
							vmRect.height = array4[i].Value;
							array5[i] = vmRect;
						}
					}
					return array5;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的ROIAnnulus值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmClassInfo[] GetVmIOClassInfoValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "CLASSINFO")
			{
				if (io.Childs[1].Value is int?[] array)
				{
					ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[] array2 = io.Childs[0].Value as ImvsSdkDefine.IMVS_MODULE_STRING_VALUE_EX[];
					VmDataType.VmClassInfo[] array3 = new VmDataType.VmClassInfo[array.Length];
					for (int i = 0; i < array3.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmClassInfo vmClassInfo = new VmDataType.VmClassInfo();
							vmClassInfo.className = array2[i].strValue;
							vmClassInfo.grayValue = array[i].Value;
							array3[i] = vmClassInfo;
						}
					}
					return array3;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的ROIPolygon值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmROIPolygon[] GetVmIOROIPolygonValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "ROIPOLYGON")
			{
				if (io.Childs[0].Value is int?[] array)
				{
					float?[] array2 = io.Childs[1].Childs[0].Value as float?[];
					float?[] array3 = io.Childs[1].Childs[1].Value as float?[];
					VmDataType.VmROIPolygon[] array4 = new VmDataType.VmROIPolygon[array.Length];
					int num = 0;
					for (int i = 0; i < array4.Length; i++)
					{
						if (array[i].HasValue)
						{
							VmDataType.VmROIPolygon vmROIPolygon = new VmDataType.VmROIPolygon();
							vmROIPolygon.num = array[i].Value;
							IVmPoint[] points = new VmDataType.VmPoint[vmROIPolygon.num];
							vmROIPolygon.points = points;
							int num2 = vmROIPolygon.num + num;
							int num3 = 0;
							for (int j = num; j < num2; j++)
							{
								VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
								vmPoint.x = array2[j].Value;
								vmPoint.y = array3[j].Value;
								vmROIPolygon.points[num3] = vmPoint;
								num3++;
							}
							array4[i] = vmROIPolygon;
						}
					}
					return array4;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// 获取IO的Posture值
	/// </summary>
	/// <param name="vmIOName">输出名称</param>
	/// <returns></returns>
	public IVmPosture[] GetVmIOPostureValue(string vmIOName)
	{
		if (GetOutputIOByName(vmIOName, out var io))
		{
			if (io.TypeName == "POSTURE")
			{
				float?[] array = io.Childs[1].Value as float?[];
				float?[] array2 = io.Childs[0].Childs[0].Value as float?[];
				float?[] array3 = io.Childs[0].Childs[1].Value as float?[];
				if (array != null && array2 != null && array3 != null)
				{
					int num = array2.Length;
					int num2 = array3.Length;
					int num3 = array.Length;
					int num4 = ((num > num2) ? num : num2);
					num4 = ((num4 > num3) ? num4 : num3);
					VmDataType.VmPosture[] array4 = new VmDataType.VmPosture[num4];
					VmDataType.VmPosture vmPosture = new VmDataType.VmPosture();
					VmDataType.VmPoint vmPoint = new VmDataType.VmPoint();
					for (int i = 0; i < num4; i++)
					{
						if ((i < num && array2[i].HasValue) || (i < num2 && array3[i].HasValue) || (i < num3 && array[i].HasValue))
						{
							vmPosture = new VmDataType.VmPosture();
							vmPoint = new VmDataType.VmPoint();
							if (i < num && array2[i].HasValue)
							{
								vmPoint.x = array2[i].Value;
							}
							if (i < num2 && array3[i].HasValue)
							{
								vmPoint.y = array3[i].Value;
							}
							vmPosture.point = vmPoint;
							if (i < num3 && array[i].HasValue)
							{
								vmPosture.r = array[i].Value;
							}
							array4[i] = vmPosture;
						}
					}
					return array4;
				}
				return null;
			}
			throw ThrowTypeOfIOError();
		}
		throw new VmException(-536870881, "Cannot Find IO " + vmIOName.ToString() + " From Name");
	}

	/// <summary>
	/// io名称修改回调处理方法
	/// </summary>
	public virtual void InvokeOnModuleIONameChanged(string oldName, string newName)
	{
	}

	/// <summary>
	/// 显示名称修改回调处理方法
	/// </summary>
	public virtual void InvokeOnModuleDisplayParamNameChanged(string oldName, string newName)
	{
	}

	internal void DeleteConnection(UnRedo_ConnectionData data, IUnRedoParams unRedoParams)
	{
		(unRedoParams.Dispatcher as Dispatcher).Invoke(() =>
		{
			string sourceModuleFullName = data.SourceModuleFullName;
			string targetModuleFullName = data.TargetModuleFullName;
			VmModule item = VmSolution.Instance[targetModuleFullName] as VmModule;
			(VmSolution.Instance[sourceModuleFullName] as VmModule)?.NextModules.Remove(item);
		});
	}

	internal void AddConnection(UnRedo_ConnectionData data, IUnRedoParams unRedoParams)
	{
		(unRedoParams.Dispatcher as Dispatcher).Invoke(() =>
		{
			string sourceModuleFullName = data.SourceModuleFullName;
			string targetModuleFullName = data.TargetModuleFullName;
			VmModule item = VmSolution.Instance[targetModuleFullName] as VmModule;
			(VmSolution.Instance[sourceModuleFullName] as VmModule)?.NextModules.Add(item);
		});
	}

	internal void Import_Module(VmModuleDataInfo data, IUnRedoParams unRedoParams)
	{
		string strModuleFullName = data.strModuleFullName;
		string sub = strModuleFullName.Substring(0, strModuleFullName.LastIndexOf('.'));
		object obj = VmSolution.Instance[sub];
		if (obj != null)
		{
			(obj as VmModuleSet).ImportModule(data, unRedoParams.Dispatcher);
		}
	}

	internal void Delete_Module(VmModuleDataInfo data, IUnRedoParams unRedoParams)
	{
		string strModuleFullName = data.strModuleFullName;
		VmProcedure parent = (VmSolution.Instance[strModuleFullName] as IVmFrameBase).GetParent<VmProcedure>();
		(VmSolution.Instance[strModuleFullName] as VmModule)?.Dispose();
		parent?.OnPropertyChanged("WrokState");
	}

	internal void Update_Module(VmModuleDataInfo data)
	{
		string strModuleFullName = data.strModuleFullName;
		object obj = VmSolution.Instance[strModuleFullName];
		if (obj != null)
		{
			VmModuleDataInfo vmModuleDataInfo = (obj as VmModule).Export() as VmModuleDataInfo;
			data.strModuleFullName = vmModuleDataInfo.strModuleFullName;
			data.nDataLen = vmModuleDataInfo.nDataLen;
			data.nModuleID = vmModuleDataInfo.nModuleID;
			data.nParentNodeID = vmModuleDataInfo.nParentNodeID;
			data.nWaitTime = vmModuleDataInfo.nWaitTime;
			data.pData = vmModuleDataInfo.pData;
			data.strModuDisplayName = vmModuleDataInfo.strModuDisplayName;
			data.strModuTypeName = vmModuleDataInfo.strModuTypeName;
			data.strParentName = vmModuleDataInfo.strParentName;
			data.strPath = vmModuleDataInfo.strPath;
		}
	}

	internal void RegisterInputIOToIoc()
	{
		Expression<Func<object>> oldValue = () => GetModuInputIOInfoClone(ID);
		string displayName = string.Format("{0}{1}:{2}ALL_IO", AppLocalizationService.GetLang("Lang_Edit"), AppLocalizationService.GetLang("界面数据"), FullName.Substring(FullName.LastIndexOf(',') + 1));
		Ioc.Get<IUnRedoService>()?.Register(new DynamicIOUnRedoOperation(IOData_Redo, IOData_Undo, new UnRedo_DynamicIOData
		{
			ModuleID = ID,
			IsAllIO = true,
			IsInput = true,
			FullName = FullName,
			OldValue = oldValue,
			NewValue = null
		}, IOData_Update), displayName, (IBuilder builder, IUnRedoOperation operation) => CanRegisterDynamicIOData(builder, operation));
	}

	internal void RegisterOutputIOToIoc()
	{
		Expression<Func<object>> oldValue = () => GetModuOutputIOInfoClone(ID);
		string displayName = string.Format("{0}{1}:{2}ALL_IO", AppLocalizationService.GetLang("Lang_Edit"), AppLocalizationService.GetLang("界面数据"), FullName.Substring(FullName.LastIndexOf(',') + 1));
		Ioc.Get<IUnRedoService>()?.Register(new DynamicIOUnRedoOperation(IOData_Redo, IOData_Undo, new UnRedo_DynamicIOData
		{
			ModuleID = ID,
			IsAllIO = true,
			IsInput = false,
			FullName = FullName,
			OldValue = oldValue,
			NewValue = null
		}, IOData_Update), displayName, (IBuilder builder, IUnRedoOperation operation) => CanRegisterDynamicIOData(builder, operation));
	}

	private object GetModuOutputIOInfoClone(uint id)
	{
		return ServerSDKManager.serverSDKManager.mModuleManager.GetModuOutputIOInfo(id);
	}

	private object GetModuInputIOInfoClone(uint id)
	{
		return ServerSDKManager.serverSDKManager.mModuleManager.GetModuInputIOInfo(id);
	}

	private ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST ConvertIODataToStruct(object obj)
	{
		if (obj == null)
		{
			return new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST();
		}
		ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST_EX iMVS_MODULE_IO_INFO_LIST_EX = obj as ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST_EX;
		return iMVS_MODULE_IO_INFO_LIST_EX.GetSourceValue();
	}

	private bool CanRegisterDynamicIOData(IBuilder builder, IUnRedoOperation unRedoOperation)
	{
		IUnRedoDataDone<UnRedo_DynamicIOData> unRedoDone = unRedoOperation as IUnRedoDataDone<UnRedo_DynamicIOData>;
		if (builder.Actions.FirstOrDefault((IUnRedoActionOperation p) => p is IUnRedoDataDone<UnRedo_DynamicIOData> unRedoDataDone && unRedoDataDone.Data.IsAllIO && unRedoDataDone.Data.ModuleID == unRedoDone.Data.ModuleID && unRedoDataDone.Data.IsInput == unRedoDone.Data.IsInput) != null)
		{
			return false;
		}
		IRootParam rootParam = builder as IRootParam;
		if (rootParam.RootOperType != "ModifyConfig" && rootParam.RootOperType != "DeleteModule" && rootParam.RootOperType != "CopyModule")
		{
			return false;
		}
		if (unRedoDone.Data.OldValue is Expression<Func<object>> expression)
		{
			unRedoDone.Data.OldValue = expression.Compile()();
		}
		return true;
	}

	private void IOData_Undo(UnRedo_DynamicIOData data, IUnRedoParams unRedoParams)
	{
		(unRedoParams.Dispatcher as Dispatcher).Invoke(() =>
		{
			if (VmSolution.Instance[data.FullName] is VmModule vmModule)
			{
				if (data.IsInput)
				{
					ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleIOBeSubscribed(vmModule.ID, "");
					ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST cloneList = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)iMVS_MODULE_SUBSCRIBE_INFO_LIST.Clone();
					vmModule.RemoveDynamicIn();
					ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST iMVS_MODULE_IO_INFO_LIST = ((data.OldValue is ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST) ? ((ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST)data.OldValue) : new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST());
					iMVS_MODULE_IO_INFO_LIST.nIoClass = 1;
					ServerSDKManager.serverSDKManager.mModuleManager.SetModuInputIOInfo(data.ModuleID, iMVS_MODULE_IO_INFO_LIST);
					vmModule.InitDynamicIO(iMVS_MODULE_IO_INFO_LIST, vmModule.Inputs);
					Done_IOData(vmModule, isInput: true, cloneList, data.IsReloadEnd);
				}
				else
				{
					ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST2 = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleIOBeSubscribed(vmModule.ID, "");
					ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST cloneList2 = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)iMVS_MODULE_SUBSCRIBE_INFO_LIST2.Clone();
					vmModule.RemoveDynamicOut();
					ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST iMVS_MODULE_IO_INFO_LIST2 = ((data.OldValue is ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST) ? ((ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST)data.OldValue) : new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST());
					iMVS_MODULE_IO_INFO_LIST2.nIoClass = 2;
					ServerSDKManager.serverSDKManager.mModuleManager.SetModuOutputIOInfo(data.ModuleID, iMVS_MODULE_IO_INFO_LIST2);
					vmModule.InitDynamicIO(iMVS_MODULE_IO_INFO_LIST2, vmModule.Outputs);
					Done_IOData(vmModule, isInput: false, cloneList2, data.IsReloadEnd);
				}
			}
		});
	}

	private void IOData_Redo(UnRedo_DynamicIOData data, IUnRedoParams unRedoParams)
	{
		VmModule module = VmSolution.Instance[data.FullName] as VmModule;
		if (module == null)
		{
			return;
		}
		(unRedoParams.Dispatcher as Dispatcher).Invoke(() =>
		{
			if (data.IsInput)
			{
				ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleIOBeSubscribed(module.ID, "");
				ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST cloneList = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)iMVS_MODULE_SUBSCRIBE_INFO_LIST.Clone();
				module.RemoveDynamicIn();
				ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST iMVS_MODULE_IO_INFO_LIST = ((data.NewValue is ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST) ? ((ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST)data.NewValue) : new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST());
				iMVS_MODULE_IO_INFO_LIST.nIoClass = 1;
				ServerSDKManager.serverSDKManager.mModuleManager.SetModuInputIOInfo(data.ModuleID, iMVS_MODULE_IO_INFO_LIST);
				module.InitDynamicIO(iMVS_MODULE_IO_INFO_LIST, module.Inputs);
				Done_IOData(module, isInput: true, cloneList, data.IsReloadEnd);
			}
			else
			{
				ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST iMVS_MODULE_SUBSCRIBE_INFO_LIST2 = ServerSDKManager.serverSDKManager.mModuleManager.QueryModuleIOBeSubscribed(module.ID, "");
				ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST cloneList2 = (ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST)iMVS_MODULE_SUBSCRIBE_INFO_LIST2.Clone();
				module.RemoveDynamicOut();
				ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST iMVS_MODULE_IO_INFO_LIST2 = ((data.NewValue is ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST) ? ((ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST)data.NewValue) : new ImvsSdkDefine.IMVS_MODULE_IO_INFO_LIST());
				iMVS_MODULE_IO_INFO_LIST2.nIoClass = 2;
				ServerSDKManager.serverSDKManager.mModuleManager.SetModuOutputIOInfo(data.ModuleID, iMVS_MODULE_IO_INFO_LIST2);
				module.InitDynamicIO(iMVS_MODULE_IO_INFO_LIST2, module.Outputs);
				Done_IOData(module, isInput: false, cloneList2, data.IsReloadEnd);
			}
		});
	}

	private void IOData_Update(UnRedo_DynamicIOData data)
	{
		if (VmSolution.Instance[data.FullName] is VmModule)
		{
			if (data.IsInput)
			{
				data.NewValue = GetModuInputIOInfoClone(data.ModuleID);
			}
			else
			{
				data.NewValue = GetModuOutputIOInfoClone(data.ModuleID);
			}
		}
	}

	internal void Set_SubParams(SubscribeParamData data, IUnRedoParams unRedoParams)
	{
		try
		{
			string moduleName = data.ModuleName;
			VmModule module = VmSolution.Instance[moduleName] as VmModule;
			(data.ParamData as IList<ISubParam>)?.ToList().ForEach((ISubParam x) =>
			{
				module.SetRunParamBind(x);
			});
			(module.Params.Data as IUpdateItemRoot)?.UpdateRunParamStatus((data.ParamData as IList<ISubParam>)?.Select((ISubParam x) => x.ParamName).ToList());
		}
		catch (Exception)
		{
		}
	}

	internal void Delete_SubParams(SubscribeParamData data, IUnRedoParams unRedoParams)
	{
		try
		{
			string moduleName = data.ModuleName;
			object obj = VmSolution.Instance[moduleName];
		}
		catch (Exception)
		{
		}
	}

	internal bool CanRegisterSubParam(IBuilder builder, IUnRedoOperation unRedoOperation)
	{
		IUnRedoDataDone<SubscribeParamData> subData = unRedoOperation as IUnRedoDataDone<SubscribeParamData>;
		IEnumerable<IUnRedoActionOperation> source = builder.Actions.Where((IUnRedoActionOperation p) => p is IUnRedoDataDone<SubscribeParamData> { Data: var data } && data?.ModuleName == subData.Data?.ModuleName);
		if (source.Count() != 0)
		{
			return false;
		}
		if (subData.Data.ParamData is Expression<Func<object>> expression)
		{
			subData.Data.ParamData = expression.Compile()();
			if (subData.Data.ParamData is IList<ISubParam> { Count: 0 })
			{
				return false;
			}
		}
		return true;
	}

	private void Param_Undo(UnRedo_ParamsData data, IUnRedoParams unRedoParams)
	{
		try
		{
			object obj = VmSolution.Instance[data.ModuleFullName];
			(obj as VmModule)?.SetAllParams(data.OldParamList);
		}
		catch
		{
		}
	}

	private void Param_Redo(UnRedo_ParamsData data, IUnRedoParams unRedoParams)
	{
		try
		{
			object obj = VmSolution.Instance[data.ModuleFullName];
			(obj as VmModule)?.SetAllParams(data.NewParamList);
		}
		catch
		{
		}
	}

	private void Param_Update(UnRedo_ParamsData data)
	{
		object newParamList = ((VmModule)VmSolution.Instance[data.ModuleFullName])?.GetModuleAllParams();
		data.NewParamList = newParamList;
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

	private void Done_IOData(VmModule module, bool isInput, ImvsSdkDefine.IMVS_MODULE_SUBSCRIBE_INFO_LIST cloneList = null, bool isReloadEnd = true)
	{
		if (module is IVMGroup iVMGroup)
		{
			iVMGroup.SetIoParams();
		}
		if (module is IVmProcedure vmProcedure)
		{
			vmProcedure.SetIoParams();
		}
		if (cloneList != null)
		{
			module.RecoverSubscribe(cloneList);
		}
		IUpdateItemRoot updateItemRoot = ReloadFunc(module, isReloadEnd);
		string text = (isInput ? "DynamicInputParam" : "DynamicOutputParam");
		updateItemRoot?.UpdatDynamicIO(text);
	}

	private IUpdateItemRoot ReloadFunc(VmModule module, bool isReloadEnd = true)
	{
		IUpdateItemRoot updateItemRoot = ReloadFuncChild(module, isReloadEnd);
		updateItemRoot?.UpdatDynamicIO("DynamicDisplayParam");
		return updateItemRoot;
	}

	private IUpdateItemRoot ReloadFuncChild(VmModule module, bool isReloadEnd = true)
	{
		if (isReloadEnd && module.ExternPathStartNodes.ContainsKey("[Display]"))
		{
			object obj = module.ExternPathStartNodes["[Display]"];
			(obj as IReload)?.Reload();
		}
		return module.Params.Data as IUpdateItemRoot;
	}

	private void RegisterSubParamFunc(object module)
	{
		VmModule curModule = module as VmModule;
		Expression<Func<object>> paramData = () => (curModule.Params.Data as IUpdateItemRoot).GetAllRunParamBind(curModule.FullName);
		string displayName = string.Format("{0}{1}{2}", AppLocalizationService.GetLang("Lang_Edit"), curModule.FullName, AppLocalizationService.GetLang("Tab_Params"));
		Ioc.Get<IUnRedoService>()?.Register(new SubParamUnRedoOperation(curModule.Delete_SubParams, curModule.Set_SubParams, new SubscribeParamData
		{
			ModuleName = curModule.FullName,
			ParamData = paramData
		}), displayName, (IBuilder builder, IUnRedoOperation operation) => curModule.CanRegisterSubParam(builder, operation));
	}

	private void SetRunParamBind(ISubParam subParam)
	{
		if (subParam.ParamName.Length <= 0 || subParam.CurValue == null)
		{
			return;
		}
		if ((MathHelper.IsNumber(subParam.CurValue) || MathHelper.IsConst(subParam.CurValue)) && subParam.CurValue != "TRIGGER_STRING" && subParam.CurValue != "TRIGGER_STRING[]")
		{
			ParamSubManager.SetModuleParamBinding(subParam.ParamName, subParam.Type, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_CONSTANT, subParam.CurValue);
			return;
		}
		if (subParam.CurValue.Equals(VmSolution.ExternIO.Name + "[]") || subParam.CurValue == VmSolution.ExternIO.Name)
		{
			ParamSubManager.SetModuleParamBinding(subParam.ParamName, subParam.Type, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_GLOBAL, "", 0u, VmSolution.ExternIO.Name);
			return;
		}
		string text = "";
		int num = subParam.CurValue.IndexOf('[', 0);
		if (num > 0)
		{
			text = subParam.CurValue.Substring(num);
			text = text.Replace("[", "").Replace("]", "");
			subParam.CurValue = subParam.CurValue.Remove(subParam.CurValue.IndexOf('[', 0));
		}
		VmIO vmIO = GetIoByPath(subParam.CurValue);
		if (vmIO == null)
		{
			vmIO = VmSolution.Instance[subParam.CurValue] as VmIO;
		}
		if (vmIO == null)
		{
			return;
		}
		if (text.Contains('.'))
		{
			VmIO ioByPath = GetIoByPath(text);
			if (ioByPath != null)
			{
				ParamSubManager.SetModuleParamBinding(subParam.ParamName, subParam.Type, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_VARIABLE, "", vmIO.Root.ID, vmIO.Name, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_VARIABLE, "", ioByPath.Root.ID, ioByPath.Name);
			}
		}
		else if (MathHelper.IsNumber(text))
		{
			ParamSubManager.SetModuleParamBinding(subParam.ParamName, subParam.Type, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_VARIABLE, "", vmIO.Root.ID, vmIO.Name, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_CONSTANT, text);
		}
		else
		{
			ParamSubManager.SetModuleParamBinding(subParam.ParamName, subParam.Type, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_VAR_TYPE.SUBSCRIBE_VAR_VARIABLE, "", vmIO.Root.ID, vmIO.Name, ImvsSdkDefine.IMVS_ENUM_SUBSCRIBE_INDEX_TYPE.SUBSCRIBE_INDEX_ALL, "All");
		}
	}

	/// <summary>
	/// 导出数据到文件
	/// </summary>
	[Obsolete("CH: 弃用,推荐(SaveAs(string path, string passWord = null)) | EN: Obsolete,recommend(SaveAs(string path, string password = null))", false)]
	public virtual string ExportFile(string path = null, string passWord = null, bool isExportAs = false, string initDir = null)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// 保存方案，与ExportFile功能一致
	/// </summary>
	/// <returns></returns>
	public virtual string Save()
	{
		return ExportFile();
	}

	/// <summary>
	/// 数据另存为
	/// </summary>
	/// <param name="path">存储路径</param>
	/// <param name="passWord">存储密码</param>
	/// <returns></returns>
	public virtual string SaveAs(string path, string passWord = null)
	{
		return ExportFile(path, (passWord == null) ? ModuleFilePassword : passWord);
	}

	bool ICatchCoreInfo.IsModelTrigger()
	{
		return VmSolution.IsParamModifyExecute && VmSolution.IsModelModifyExecute;
	}

	void IParamsOperator.GetParamsForConfig()
	{
		((IParamsOperator)this).ParamsDictionary = new Dictionary<string, byte[]>();
		ImvsSdkDefine.IMVS_MODULE_PARAM_LIST value = ServerSDKManager.serverSDKManager.mModuleManager.GetParamList(ID).Value;
		for (int i = 0; i < value.nParamNum; i++)
		{
			ImvsSdkDefine.IMVS_MODULE_PARAM iMVS_MODULE_PARAM = value.stModuleParamList[i];
			((IParamsOperator)this).ParamsDictionary.Add(Hik.Util.StringUtil.UTF8GetString(iMVS_MODULE_PARAM.strParamName), iMVS_MODULE_PARAM.strParamValue);
		}
	}

	void IParamsOperator.DisposeParamsDict()
	{
		((IParamsOperator)this).ParamsDictionary = null;
	}
}
