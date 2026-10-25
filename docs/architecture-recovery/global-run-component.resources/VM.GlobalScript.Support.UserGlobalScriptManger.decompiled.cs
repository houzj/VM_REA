using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using Apps.Json;
using Microsoft.Win32;
using VM.GlobalScript.Methods;
using VM.Utility;
using VMGlobalScript;

namespace VM.GlobalScript.Support;

public class UserGlobalScriptManger
{
	public GlobalScriptDataContext globalScriptDataContext = null;

	public string ClientCommAddr = string.Empty;

	public string ServerRepAddr = string.Empty;

	public int ServerPid = 0;

	public bool bCrashFlag = false;

	public bool bSmGlobalProfix = false;

	private bool bRunOnceProcess = false;

	private bool bContinueRunProcess = false;

	private bool bContinueRunWhile = false;

	private string strDefaultGlobalScript = string.Empty;

	private string strBackGlobalScript = string.Empty;

	private Mutex compileMutex = null;

	private PlatFormSDKManager objPlatFormSdkManager = null;

	private const string strSaveGlobalScriptFileName = "Global_0.txt";

	private const string strDefaultGlobalScriptFileName = "GlobalScript.txt";

	private const string strTempGlobalScriptCompileFileName = "GlobalComplie.txt";

	private const string strTempGlobalScriptFileName = "GlobalScript.temp";

	private BaseZmqCommunicate objZmqToServer = null;

	private object lockObject = new object();

	private bool bSeliceExecuteProcess = false;

	private System.Timers.Timer m_CheckScriptTime = null;

	private DateTime objLastWriteTime;

	private bool bStartCheck = false;

	private AutoResetEvent lodResetEvent = new AutoResetEvent(initialState: false);

	private AutoResetEvent runResetEvent = new AutoResetEvent(initialState: false);

	public GlobalScriptReportMsg ReportMsgAction = null;

	private int nModuSlientExecuteMode = 1;

	private int nCompileWaitTime = 60;

	private bool isLoadSol = false;

	private List<ShellRefrences> _defaultRefrences = null;

	private bool IsSingleProcessMode = false;

	private IntPtr SdkBaseHandle = IntPtr.Zero;

	private string VMBaseDllPath = "";

	private string appBaseDirectory = "";

	private string vmregistrPath = "";

	private readonly string _registrykeyPathName = "SOFTWARE\\WOW6432Node\\Microsoft\\.NETFramework\\v4.0.30319\\AssemblyFoldersEx\\VisionMaster";

	private const int INVALID_HANDLE_VALUE = -1;

	private IntPtr hShareMemoryHandle = IntPtr.Zero;

	private IntPtr hBufferView = IntPtr.Zero;

	private const int PAGE_READWRITE = 4;

	private const int FILE_MAP_ALL_ACCESS = 2;

	private object loadlock = new object();

	private int nLoadInitCount = 0;

	private int nLoadSolCount = 0;

	private const int LOADCOUNT = 65535;

	private List<DLLNameMap> mapNameInfo = new List<DLLNameMap>();

	private bool IsLoadNeuFile = false;

	public List<ShellRefrences> DefaultRefrences
	{
		get
		{
			if (_defaultRefrences == null)
			{
				_defaultRefrences = new List<ShellRefrences>();
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "mscorlib.dll",
					refrencesType = 0
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "System.dll",
					refrencesType = 0
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "System.Core.dll",
					refrencesType = 0
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "System.Drawing.dll",
					refrencesType = 0
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "System.Windows.Forms.dll",
					refrencesType = 0
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "VM.GlobalScript.Methods.dll",
					refrencesType = 1
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "iMVS-6000PlatformSDKCS.dll",
					refrencesType = 1
				});
				_defaultRefrences.Add(new ShellRefrences
				{
					Name = "Apps.Json.dll",
					refrencesType = 1
				});
				if (IsSingleProcessMode)
				{
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "VM.Core.dll",
						refrencesType = 6
					});
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "VM.PlatformSDKCS.dll",
						refrencesType = 7
					});
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "VMControls.BaseInterface.dll",
						refrencesType = 6
					});
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "VMControls.Interface.dll",
						refrencesType = 6
					});
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "VMControls.RenderInterface.dll",
						refrencesType = 6
					});
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "ImageSourceModuleCs.dll",
						refrencesType = 6
					});
					_defaultRefrences.Add(new ShellRefrences
					{
						Name = "IMVSFastFeatureMatchModuCs.dll",
						refrencesType = 6
					});
				}
			}
			return _defaultRefrences;
		}
	}

	public string AppBaseDirectory
	{
		get
		{
			if (string.IsNullOrEmpty(appBaseDirectory))
			{
				if (IsSingleProcessMode)
				{
					string location = Assembly.GetExecutingAssembly().Location;
					location = location.Substring(0, location.LastIndexOf('\\') + 1);
					DirectoryInfo directoryInfo = new DirectoryInfo(location);
					VMBaseDllPath = directoryInfo.Parent.FullName;
					appBaseDirectory = location;
				}
				else
				{
					appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				}
			}
			return appBaseDirectory;
		}
	}

	private string VMRegisterPath
	{
		get
		{
			if (string.IsNullOrEmpty(vmregistrPath))
			{
				vmregistrPath = GetRegistrykeyPath() + "\\";
			}
			return vmregistrPath;
		}
	}

	public UserGlobalScriptManger(bool isSingleMode = false)
	{
		IsSingleProcessMode = isSingleMode;
		globalScriptDataContext = new GlobalScriptDataContext();
		strDefaultGlobalScript = AppBaseDirectory + "GlobalScript.txt";
		strBackGlobalScript = AppBaseDirectory + "GlobalScript.temp";
		compileMutex = new Mutex();
		objPlatFormSdkManager = new PlatFormSDKManager();
		objPlatFormSdkManager.SetRunMode(IsSingleProcessMode ? 1 : 0);
		UserGlobalScriptSupport.GetScriptInstance().IsUsePlatformSDK = IsSingleProcessMode;
		UserGlobalScriptSupport.GetScriptInstance().AppBaseDirectory = AppBaseDirectory;
		UserGlobalScriptSupport.GetScriptInstance().UpdateUIScriptEvent += SendDataToServer;
		UserGlobalScriptSupport.GetScriptInstance().VMRegisterPath = VMRegisterPath;
		if (m_CheckScriptTime == null)
		{
			m_CheckScriptTime = new System.Timers.Timer(2000.0);
			m_CheckScriptTime.AutoReset = true;
			m_CheckScriptTime.Elapsed += m_CheckScriptTime_Elapsed;
			m_CheckScriptTime.Enabled = false;
		}
		if (IsSingleProcessMode)
		{
			string text = ConfigurationHelper.INIGetStringValue("ModuSlientExecuteMode", AppBaseDirectory);
			if (!string.IsNullOrEmpty(text))
			{
				int.TryParse(text, out nModuSlientExecuteMode);
			}
			string text2 = ConfigurationHelper.INIGetStringValue("CompileWaitTime", AppBaseDirectory);
			if (!string.IsNullOrEmpty(text2))
			{
				int.TryParse(text2, out nCompileWaitTime);
			}
		}
	}

	private string GetRegistrykeyPath()
	{
		string result = string.Empty;
		try
		{
			RegistryKey localMachine = Registry.LocalMachine;
			if (localMachine != null)
			{
				RegistryKey registryKey = localMachine.OpenSubKey(_registrykeyPathName);
				if (registryKey != null)
				{
					object value = registryKey.GetValue("");
					if (value != null && value is string text)
					{
						if (string.IsNullOrWhiteSpace(text))
						{
							return result;
						}
						if (Directory.Exists(text))
						{
							result = text;
						}
					}
					registryKey.Close();
				}
				localMachine.Close();
			}
		}
		catch
		{
		}
		return result;
	}

	public void SetSdkHandel(IntPtr handle)
	{
		SdkBaseHandle = handle;
	}

	public void InitZmqToServer(string ServerPairAddr, int receiveTime, int writeTime, bool bzmqType)
	{
		if (string.IsNullOrEmpty(ServerPairAddr) || !ServerPairAddr.Contains(":"))
		{
			LogHelper.Error("ReportPairAddr is null");
			return;
		}
		bool flag = false;
		ZmqDataContext zmqDataContext = new ZmqDataContext
		{
			ConnectionString = ServerPairAddr,
			RcvTimout = receiveTime,
			Encod = Encoding.UTF8,
			ServerOrClient = true,
			WriteTimeOut = writeTime,
			StartReceiveTask = false
		};
		if (bzmqType)
		{
			zmqDataContext.ZmqType = 1;
			objZmqToServer = new HkrMqCommunicate(zmqDataContext);
		}
		else
		{
			zmqDataContext.ZmqType = 0;
			objZmqToServer = new ZmqCommunicate(zmqDataContext);
		}
		if (objZmqToServer.InitCommuncate())
		{
			LogHelper.Info("Create Zmqserver Succeed:" + ServerPairAddr);
		}
		else
		{
			LogHelper.Error("Create Zmqserver Faild:" + ServerPairAddr);
		}
	}

	private void SendDataToServer(string commandType, int errorCode, string path = "")
	{
		try
		{
			lock (lockObject)
			{
				dynamic val = new
				{
					command = CMDStatusWithUI.ReportData,
					type = commandType,
					msg = path,
					errorCode = errorCode
				};
				if (val != null)
				{
					dynamic val2 = new
					{
						head = (object)val
					};
					string text = JsonConvert.SerializeObject(val2);
					if (objZmqToServer != null)
					{
						if (!objZmqToServer.SendData(text))
						{
							LogHelper.Error("Global script report error failed");
						}
						else
						{
							LogHelper.Info("Global script report error msg " + text);
						}
					}
					else
					{
						objPlatFormSdkManager.ReportData(text);
					}
				}
				if (commandType == "updateScript" && !string.IsNullOrEmpty(path) && File.Exists(path))
				{
					globalScriptDataContext.GlobalScriptContent = File.ReadAllText(path, Encoding.UTF8);
					updateBackFile(globalScriptDataContext.GlobalScriptPassword, globalScriptDataContext.GlobalScriptContent);
				}
				LogHelper.Error($"Global script report command:{commandType},errorcode:{errorCode} ");
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("Global script send report data error " + ex.Message);
		}
	}

	public uint LoadSolution(string filePath, bool isCrash)
	{
		uint result = 0u;
		if (string.IsNullOrEmpty(filePath))
		{
			return 3758096899u;
		}
		try
		{
			if (!isCrash && filePath != "null")
			{
				result = readScriptDataFromMapFile(filePath);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("load solution falid,error:" + ex.Message);
			return 3758096900u;
		}
		return result;
	}

	public uint SaveSolution(out string fileSavePath)
	{
		uint result = 0u;
		fileSavePath = AppBaseDirectory + "Global_0.txt";
		try
		{
			SaveInfo saveInfo = new SaveInfo();
			saveInfo.ScriptPassword = globalScriptDataContext.GlobalScriptPassword;
			saveInfo.ScriptContent = globalScriptDataContext.GlobalScriptContent;
			saveInfo.ScriptRefences = globalScriptDataContext.GlobalScriptRefences;
			string value = JsonConvert.SerializeObject(saveInfo);
			using StreamWriter streamWriter = new StreamWriter(fileSavePath, append: false, Encoding.UTF8);
			streamWriter.Write(value);
		}
		catch (Exception ex)
		{
			result = 3758096898u;
			LogHelper.Error("save solution falid,error:" + ex.Message);
		}
		return result;
	}

	public uint SaveSolutionByMap(out string fileSavePath)
	{
		uint result = 0u;
		SaveInfo saveInfo = new SaveInfo();
		saveInfo.ScriptPassword = globalScriptDataContext.GlobalScriptPassword;
		saveInfo.ScriptContent = globalScriptDataContext.GlobalScriptContent;
		saveInfo.ScriptRefences = globalScriptDataContext.GlobalScriptRefences;
		string text = JsonConvert.SerializeObject(saveInfo);
		if (IsSingleProcessMode)
		{
			fileSavePath = text;
			LogHelper.Info("save solution byte len:" + Encoding.UTF8.GetByteCount(fileSavePath));
		}
		else
		{
			if (bSmGlobalProfix)
			{
				fileSavePath = $"Global\\GlobalScriptSaveSol_{Process.GetCurrentProcess().Id}";
			}
			else
			{
				fileSavePath = $"GlobalScriptSaveSol_{Process.GetCurrentProcess().Id}";
			}
			try
			{
				result = WriteToMemory(Encoding.UTF8.GetBytes(text), fileSavePath);
			}
			catch (Exception ex)
			{
				result = 3758096898u;
				LogHelper.Error("save solution failed,error:" + ex.Message);
			}
		}
		return result;
	}

	public uint ReleaseShaleMap(string shareMemory)
	{
		try
		{
			ReleaseMapFileHandle();
		}
		catch (Exception ex)
		{
			LogHelper.Error("release map memory falid,error:" + ex.Message);
			return 3758096639u;
		}
		return 0u;
	}

	private void ReleaseMapFileHandle()
	{
		if (hBufferView != IntPtr.Zero)
		{
			MemoryHelper.UnmapViewOfFile(hBufferView);
			hBufferView = IntPtr.Zero;
		}
		if (hShareMemoryHandle != IntPtr.Zero)
		{
			MemoryHelper.CloseHandle(hShareMemoryHandle);
			hShareMemoryHandle = IntPtr.Zero;
		}
	}

	private uint WriteToMemory(byte[] byteData, string fileName)
	{
		IntPtr hFile = new IntPtr(-1);
		int num = byteData.Length + 16 + 64;
		try
		{
			ReleaseMapFileHandle();
			hShareMemoryHandle = MemoryHelper.CreateFileMapping(hFile, IntPtr.Zero, 4, 0, num, fileName);
			if (hShareMemoryHandle == IntPtr.Zero)
			{
				LogHelper.Error("create filemap error");
				return 3758096405u;
			}
			hBufferView = MemoryHelper.MapViewOfFile(hShareMemoryHandle, 2, 0, 0, new IntPtr(num));
			if (hBufferView == IntPtr.Zero)
			{
				MemoryHelper.CloseHandle(hShareMemoryHandle);
				LogHelper.Error("create MapViewOfFile error");
				return 3758096405u;
			}
			byte[] bytes = BitConverter.GetBytes(num);
			Marshal.Copy(bytes, 0, hBufferView, bytes.Length);
			Marshal.Copy(byteData, 0, hBufferView + 16, byteData.Length);
		}
		catch (Exception ex)
		{
			LogHelper.Error("create MapViewOfFile error," + ex.Message);
			return 3758096639u;
		}
		return 0u;
	}

	private bool WaiteLoadCompileEnd()
	{
		if (isLoadSol)
		{
			lock (loadlock)
			{
				if (!isLoadSol)
				{
					return true;
				}
				if (!lodResetEvent.WaitOne(nCompileWaitTime * 1000))
				{
					return false;
				}
				isLoadSol = false;
			}
		}
		return true;
	}

	public uint StartOnce()
	{
		if (bContinueRunProcess || bSeliceExecuteProcess)
		{
			objPlatFormSdkManager.ReportData("execute error:" + -536870113);
			return 0u;
		}
		Task.Run(() =>
		{
			bRunOnceProcess = true;
			try
			{
				if (!WaiteLoadCompileEnd())
				{
					LogHelper.Error("StartOnce is time out, waite for compile end");
				}
				initSourceBeforeRunScript();
				if (!globalScriptDataContext.IsComplieOK)
				{
					SendDataToServer("report", -536870112);
					bRunOnceProcess = false;
					return;
				}
				if (UserGlobalScriptSupport.GetScriptInstance().CodeInitFunction(bExecuteContinues: false, isCrash: false))
				{
					if (!UserGlobalScriptSupport.GetScriptInstance().LoadExternAssembly(bExecuteContinues: false, isCrash: false))
					{
						SendDataToServer("report", -536870112);
						bRunOnceProcess = false;
						return;
					}
					int errorCode = 0;
					LogHelper.Info("GlobalScript execute once start");
					if (!UserGlobalScriptSupport.GetScriptInstance().CodeRun(ref errorCode))
					{
						LogHelper.Error("GlobalScript execute faild " + errorCode);
						if ((long)errorCode != 0)
						{
							objPlatFormSdkManager.ReportData("execute error:" + errorCode);
						}
					}
					else
					{
						LogHelper.Info("GlobalScript execute once end");
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("GlobalScript execute once error " + ex.Message);
			}
			bRunOnceProcess = false;
		});
		return 0u;
	}

	public uint ExecuteLoadInit()
	{
		uint result = 0u;
		nLoadInitCount++;
		if (nLoadInitCount > 65535)
		{
			nLoadInitCount = 1;
		}
		Task.Run(() =>
		{
			loadInitAction(nLoadInitCount);
		});
		return result;
	}

	private void loadInitAction(int nLoadCount)
	{
		if (nLoadCount <= nLoadSolCount + 1)
		{
			if (WaiteLoadCompileEnd())
			{
				LogHelper.Info("ExecuteLoadInit start");
				UserGlobalScriptSupport.GetScriptInstance().LoadSolutionInit();
			}
			else
			{
				LogHelper.Error("ExecuteLoadInit is time out");
			}
		}
	}

	private string GetConfig(string key)
	{
		string empty = string.Empty;
		Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		AppSettingsSection appSettings = configuration.AppSettings;
		if (appSettings == null || appSettings.Settings == null || appSettings.Settings.Count == 0 || appSettings.Settings[key] == null)
		{
			return null;
		}
		return appSettings.Settings[key].Value;
	}

	public uint SilentlyExecuteOnce()
	{
		uint ret = 0u;
		Task.Run(() =>
		{
			try
			{
				bSeliceExecuteProcess = true;
				initSourceAndStartScript(bCompile: false, bSclienceRunonce: true);
				ret = (uint)objPlatFormSdkManager.SilentlyExecuteOnce(nModuSlientExecuteMode);
			}
			catch (Exception ex)
			{
				ret = 3758096408u;
				LogHelper.Error("SilentlyExecuteOnce is exception:" + ex.ToString());
			}
			finally
			{
				bSeliceExecuteProcess = false;
			}
		});
		return ret;
	}

	public uint StopExcute()
	{
		if (bContinueRunProcess)
		{
			bContinueRunWhile = false;
			runResetEvent.Set();
		}
		StopAllProcess();
		return 0u;
	}

	public void StopAllProcess()
	{
		Task.Run(() =>
		{
			int num = objPlatFormSdkManager.StopRunAllProcess();
			LogHelper.Info("StopRunAllProcess return " + num);
		});
	}

	public uint StartContinueRun()
	{
		if (bContinueRunProcess || bRunOnceProcess || bSeliceExecuteProcess)
		{
			LogHelper.Info("Continue Task is Run");
			objPlatFormSdkManager.ReportData("execute error:" + -536870113);
			return 0u;
		}
		bContinueRunProcess = true;
		runResetEvent.Reset();
		Task.Run(() => ContinueRun());
		return 0u;
	}

	private async Task ContinueRun()
	{
		try
		{
			bool bError = false;
			if (!WaiteLoadCompileEnd())
			{
				LogHelper.Error("ContinueRun is time out, waite for compile end");
			}
			initSourceBeforeRunScript();
			if (!globalScriptDataContext.IsComplieOK)
			{
				SendDataToServer("report", -536870112);
				bContinueRunProcess = false;
			}
			else
			{
				if (!UserGlobalScriptSupport.GetScriptInstance().CodeInitFunction(bExecuteContinues: true, bCrashFlag))
				{
					return;
				}
				if (bCrashFlag)
				{
					bCrashFlag = false;
				}
				LogHelper.Info("GlobalScript continueExecute begin");
				int errorInfo = 0;
				bContinueRunWhile = true;
				uint executeInterval = UserGlobalScriptSupport.GetScriptInstance().GetScriptContinusExecuteInterval();
				if (!UserGlobalScriptSupport.GetScriptInstance().LoadExternAssembly(bExecuteContinues: false, isCrash: false))
				{
					SendDataToServer("report", -536870112);
					return;
				}
				int nRunCount = 0;
				while (bContinueRunWhile)
				{
					if (!UserGlobalScriptSupport.GetScriptInstance().CodeRun(ref errorInfo) && !bError)
					{
						bError = true;
						LogHelper.Error("Script continueExecute faild " + errorInfo);
						if ((long)errorInfo != 0)
						{
							objPlatFormSdkManager.ReportData("execute error:" + errorInfo);
							break;
						}
					}
					if (nRunCount == 0)
					{
						nRunCount++;
					}
					runResetEvent.WaitOne((int)executeInterval);
				}
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			if (ex2 is TaskCanceledException)
			{
				LogHelper.Info("Global script cancel continue task");
			}
			else
			{
				LogHelper.Error("Global script continueRun faild " + ex2.Message);
			}
		}
		finally
		{
			bContinueRunProcess = false;
		}
	}

	private void DisposeScriptObject()
	{
		try
		{
			UserGlobalScriptSupport.GetScriptInstance().DisposeAndUnload();
		}
		catch (Exception ex)
		{
			LogHelper.Error("DisposeScriptObject is error:" + ex.ToString());
		}
		finally
		{
		}
	}

	public uint CloseScript(bool bUnsdk = false)
	{
		bContinueRunWhile = false;
		if (bContinueRunProcess)
		{
			bContinueRunProcess = false;
			runResetEvent.Set();
		}
		bRunOnceProcess = false;
		bSeliceExecuteProcess = false;
		globalScriptDataContext.GlobalScriptContent = string.Empty;
		globalScriptDataContext.GlobalScriptPassword = string.Empty;
		globalScriptDataContext.GlobalScriptComplieResult = string.Empty;
		globalScriptDataContext.IsComplieFinish = false;
		globalScriptDataContext.GlobalScriptRefences.Clear();
		DisposeScriptObject();
		objPlatFormSdkManager.Dispose();
		globalScriptDataContext.IsComplieOK = false;
		bCrashFlag = false;
		if (m_CheckScriptTime != null)
		{
			m_CheckScriptTime.Enabled = false;
		}
		if (bUnsdk)
		{
			if (!IsSingleProcessMode)
			{
				objPlatFormSdkManager.UinitSDK();
				LogHelper.Info("Global script cancel UinitSDK end");
				if (objZmqToServer != null)
				{
					objZmqToServer.Dispose();
				}
			}
			if (m_CheckScriptTime != null)
			{
				bStartCheck = false;
				m_CheckScriptTime.Enabled = false;
				m_CheckScriptTime.Dispose();
				m_CheckScriptTime = null;
			}
		}
		ReleaseMapFileHandle();
		return 0u;
	}

	public uint SetVMZmqPair(string pairAddress)
	{
		LogHelper.Info("SetVMZmqPair pairAddress:" + pairAddress);
		if (string.IsNullOrEmpty(pairAddress))
		{
			pairAddress = "null";
		}
		return 0u;
	}

	public void SetCommunicateData(IntPtr ptrData)
	{
		objPlatFormSdkManager.Enqueue(ptrData);
	}

	public void LoadDefaultSolution()
	{
		globalScriptDataContext.IsComplieFinish = false;
		initSourceAndStartScript(bCompile: true);
	}

	public void LoadRecoverSolution()
	{
		globalScriptDataContext.IsComplieFinish = false;
		Task.Run(() =>
		{
			LogHelper.Info("Get data from back file");
			if (!readScriptDataFromFile(strBackGlobalScript))
			{
				globalScriptDataContext.GlobalScriptContent = string.Empty;
				globalScriptDataContext.GlobalScriptPassword = string.Empty;
			}
			initSourceAndStartScript();
		});
	}

	private void compileGlobalScript(string scriptContent, bool bCompile = false)
	{
		if (!string.IsNullOrEmpty(scriptContent) && (!globalScriptDataContext.IsComplieOK | bCompile))
		{
			LogHelper.Info("Compile begin");
			bool compileOK = false;
			globalScriptDataContext.GlobalScriptComplieResult = UserGlobalScriptSupport.GetScriptInstance().CompileCode(scriptContent, out compileOK);
			globalScriptDataContext.IsComplieOK = compileOK;
			if (globalScriptDataContext.IsComplieOK)
			{
				updateBackFile(globalScriptDataContext.GlobalScriptPassword, scriptContent);
			}
			LogHelper.Info("Compile end");
		}
	}

	private void initSourceAndStartScript(bool bCompile = false, bool bSclienceRunonce = false, bool bLoadSol = false)
	{
		compileMutex.WaitOne();
		try
		{
			objPlatFormSdkManager.InitPlatformSDKEx(ClientCommAddr, ServerRepAddr, ServerPid, SdkBaseHandle);
			if (bSclienceRunonce)
			{
				return;
			}
			if (globalScriptDataContext.IsComplieFinish && !bLoadSol)
			{
				LogHelper.Info("InitSourceEndStartScript already init");
			}
			else if (readDefaultScript())
			{
				compileGlobalScript(globalScriptDataContext.GlobalScriptContent, bCompile);
				if (bLoadSol)
				{
					lodResetEvent.Set();
				}
				globalScriptDataContext.IsComplieFinish = true;
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("InitSourceEndStartScript Error," + ex.Message);
		}
		finally
		{
			compileMutex.ReleaseMutex();
		}
	}

	private void initSourceBeforeRunScript()
	{
		initSourceAndStartScript();
	}

	private bool readDefaultScript()
	{
		if (string.IsNullOrEmpty(globalScriptDataContext.GlobalScriptContent))
		{
			if (!string.IsNullOrEmpty(globalScriptDataContext.GlobalScriptDefault))
			{
				globalScriptDataContext.GlobalScriptContent = globalScriptDataContext.GlobalScriptDefault;
				globalScriptDataContext.GlobalScriptRefences = DefaultRefrences;
				setScriptCodeAndRefrences(globalScriptDataContext.GlobalScriptContent, DefaultRefrences);
				return true;
			}
			if (!File.Exists(strDefaultGlobalScript))
			{
				LogHelper.Error("Default Script File Is Not Exit");
				return false;
			}
			try
			{
				globalScriptDataContext.GlobalScriptContent = File.ReadAllText(strDefaultGlobalScript, Encoding.UTF8);
				globalScriptDataContext.GlobalScriptDefault = globalScriptDataContext.GlobalScriptContent;
				globalScriptDataContext.GlobalScriptRefences = DefaultRefrences;
				setScriptCodeAndRefrences(globalScriptDataContext.GlobalScriptContent, DefaultRefrences);
				return true;
			}
			catch (Exception ex)
			{
				LogHelper.Error("Read Script File Error," + ex.Message);
				return false;
			}
		}
		return true;
	}

	public uint SetMsgFromUI(string strCommandMsg)
	{
		if (string.IsNullOrEmpty(strCommandMsg))
		{
			LogHelper.Error("SetMsgFromUI Error,strCommandMsg is null");
			return 3758096385u;
		}
		Task.Run(() =>
		{
			compileMutex.WaitOne();
			string text = string.Empty;
			try
			{
				LogHelper.Error(strCommandMsg);
				dynamic val = JsonConvert.DeserializeObject(strCommandMsg);
				if (val != null)
				{
					string value = val.head.command;
					switch ((CMDStatusWithUI)Convert.ToInt32(value))
					{
					case CMDStatusWithUI.SetRefenceAssembly:
					{
						string value2 = val.body.refrences.ToString();
						globalScriptDataContext.GlobalScriptRefences = JsonConvert.DeserializeObject<List<ShellRefrences>>(value2);
						if (globalScriptDataContext.GlobalScriptRefences != null)
						{
							UserGlobalScriptSupport.GetScriptInstance().SetRefrences(GetRefrences());
						}
						setScriptToSolutionRefrence(globalScriptDataContext.GlobalScriptRefences);
						break;
					}
					case CMDStatusWithUI.SetGlobalScript:
						globalScriptDataContext.GlobalScriptPassword = val.body.password;
						text = val.body.filePath;
						if (!string.IsNullOrEmpty(text) && File.Exists(text))
						{
							globalScriptDataContext.GlobalScriptComplieResult = "";
							globalScriptDataContext.GlobalScriptContent = File.ReadAllText(text, Encoding.UTF8);
							setScriptToSolutionFile(globalScriptDataContext.GlobalScriptContent);
							bool compileOK = false;
							globalScriptDataContext.GlobalScriptComplieResult = UserGlobalScriptSupport.GetScriptInstance().CompileCode(globalScriptDataContext.GlobalScriptContent, out compileOK);
							globalScriptDataContext.IsComplieOK = compileOK;
							if (globalScriptDataContext.IsComplieOK)
							{
								updateBackFile(globalScriptDataContext.GlobalScriptPassword, globalScriptDataContext.GlobalScriptContent);
							}
						}
						break;
					case CMDStatusWithUI.SetGlobalScriptForSave:
						globalScriptDataContext.GlobalScriptPassword = val.body.password;
						text = val.body.filePath;
						if (!string.IsNullOrEmpty(text) && File.Exists(text))
						{
							globalScriptDataContext.GlobalScriptComplieResult = "";
							globalScriptDataContext.GlobalScriptContent = File.ReadAllText(text, Encoding.UTF8);
							setScriptToSolutionFile(globalScriptDataContext.GlobalScriptContent);
						}
						break;
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("SetMsgFromUI Error," + ex.Message);
				globalScriptDataContext.IsComplieOK = false;
			}
			finally
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				compileMutex.ReleaseMutex();
			}
		});
		return 0u;
	}

	private string[] GetRefrences()
	{
		List<string> arrayRefrences = new List<string>();
		globalScriptDataContext.GlobalScriptRefences.ForEach((ShellRefrences x) =>
		{
			if (x.refrencesType == 1)
			{
				string text = AppBaseDirectory + x.Name;
				if (!File.Exists(text))
				{
					DLLNameMap dLLNameMap = mapNameInfo.FirstOrDefault((DLLNameMap p) => p.StdDLLName == x.Name);
					if (dLLNameMap != null)
					{
						string text2 = AppBaseDirectory + dLLNameMap.NeuDLLName;
						if (File.Exists(text2))
						{
							text = text2;
							x.Name = dLLNameMap.NeuDLLName;
						}
					}
				}
				arrayRefrences.Add(text);
			}
			else if (x.refrencesType == 2)
			{
				arrayRefrences.Add(AppBaseDirectory + "DLL\\" + x.Name);
			}
			else if (x.refrencesType == 6)
			{
				string text3 = VMRegisterPath + x.Name;
				if (!File.Exists(text3))
				{
					DLLNameMap dLLNameMap2 = mapNameInfo.FirstOrDefault((DLLNameMap p) => p.StdDLLName == x.Name);
					if (dLLNameMap2 != null)
					{
						string text4 = VMRegisterPath + dLLNameMap2.NeuDLLName;
						if (File.Exists(text4))
						{
							text3 = text4;
							x.Name = dLLNameMap2.NeuDLLName;
						}
					}
				}
				arrayRefrences.Add(text3);
			}
			else if (x.refrencesType == 7)
			{
				string text5 = VMBaseDllPath + "\\PublicFile\\x64\\VM.PlatformSDKCS.dll";
				if (!File.Exists(text5))
				{
					DLLNameMap dLLNameMap3 = mapNameInfo.FirstOrDefault((DLLNameMap p) => p.StdDLLName == x.Name);
					if (dLLNameMap3 != null)
					{
						string text6 = $"{VMBaseDllPath}\\PublicFile\\x64\\{dLLNameMap3.NeuDLLName}";
						if (File.Exists(text6))
						{
							text5 = text6;
							x.Name = dLLNameMap3.NeuDLLName;
						}
					}
				}
				arrayRefrences.Add(text5);
			}
			else
			{
				arrayRefrences.Add(x.Name);
			}
		});
		return arrayRefrences.ToArray();
	}

	public uint GetMsgToUI(string strCommandMsg, ref string returnStr)
	{
		uint num = 0u;
		dynamic val = null;
		dynamic returnBody = null;
		dynamic val2 = JsonConvert.DeserializeObject(strCommandMsg);
		if (val2 == null)
		{
			num = 3758096649u;
		}
		else
		{
			try
			{
				string value = val2.head.command;
				switch ((CMDStatusWithUI)Convert.ToInt32(value))
				{
				case CMDStatusWithUI.GetGlobalScript:
					num = getScriptInfo(ref returnBody);
					break;
				case CMDStatusWithUI.Getcomplie:
					num = getScriptCompileInfo(ref returnBody);
					break;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("GetMsgToUI Error," + ex.Message);
				num = 3758096639u;
			}
		}
		object head = new
		{
			command = (object)val2.head.command,
			type = "response",
			seqId = (object)val2.head.seqId,
			errorCode = num,
			errorDesc = ErrorCode.GetErrorInfo(num)
		};
		val = ((!((returnBody != null) ? true : false)) ? ((object)new { head }) : ((object)new
		{
			head = head,
			body = (object)returnBody
		}));
		returnStr = JsonConvert.SerializeObject(val);
		return num;
	}

	private bool setScriptCodeAndRefrences(string content, List<ShellRefrences> shellRefrences)
	{
		return setScriptToSolutionFile(content) && setScriptToSolutionRefrence(shellRefrences);
	}

	private bool setScriptToSolutionFile(string content)
	{
		string text = AppBaseDirectory + "\\GlobalUserScript\\UserGlobalScript.cs";
		if (!Directory.Exists(AppBaseDirectory + "\\GlobalUserScript"))
		{
			return false;
		}
		bool result = WriteStringToFile(text, content);
		FileInfo fileInfo = new FileInfo(text);
		objLastWriteTime = fileInfo.LastWriteTime;
		if (m_CheckScriptTime != null && !m_CheckScriptTime.Enabled)
		{
			m_CheckScriptTime.Enabled = true;
		}
		bStartCheck = true;
		return result;
	}

	private void setDefaultRefrence()
	{
		try
		{
			List<ShellRefrences> list = new List<ShellRefrences>();
			list.Add(new ShellRefrences
			{
				Name = "mscorlib.dll",
				refrencesType = 0
			});
			list.Add(new ShellRefrences
			{
				Name = "System.dll",
				refrencesType = 0
			});
			list.Add(new ShellRefrences
			{
				Name = "System.Core.dll",
				refrencesType = 0
			});
			list.Add(new ShellRefrences
			{
				Name = "System.Windows.Forms.dll",
				refrencesType = 0
			});
			list.Add(new ShellRefrences
			{
				Name = "iMVS-6000PlatformSDKCS.dll",
				refrencesType = 1
			});
			list.Add(new ShellRefrences
			{
				Name = "VM.GlobalScript.Methods.dll",
				refrencesType = 1
			});
			list.Add(new ShellRefrences
			{
				Name = "Apps.Json.dll",
				refrencesType = 1
			});
			setScriptToSolutionRefrence(list);
		}
		catch (Exception ex)
		{
			LogHelper.Error("setDefaultRefrence exception:" + ex.ToString());
		}
	}

	private bool setScriptToSolutionRefrence(List<ShellRefrences> shellRefrences)
	{
		try
		{
			string filename = AppBaseDirectory + "\\GlobalUserScript\\GlobalUserScript.csproj";
			if (!Directory.Exists(AppBaseDirectory + "\\GlobalUserScript"))
			{
				return false;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			XmlElement documentElement = xmlDocument.DocumentElement;
			XmlElement xmlElement = null;
			foreach (XmlNode childNode in documentElement.ChildNodes)
			{
				if (childNode.Name == "ItemGroup")
				{
					xmlElement = (XmlElement)childNode;
					break;
				}
			}
			if (xmlElement == null)
			{
				return false;
			}
			xmlElement.RemoveAll();
			foreach (ShellRefrences shellRefrence in shellRefrences)
			{
				if (shellRefrence.refrencesType == 0)
				{
					XmlElement xmlElement2 = xmlDocument.CreateElement("Reference", documentElement.NamespaceURI);
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Include");
					xmlAttribute.InnerText = shellRefrence.Name.Substring(0, shellRefrence.Name.LastIndexOf('.'));
					xmlElement2.SetAttributeNode(xmlAttribute);
					xmlElement.AppendChild(xmlElement2);
					continue;
				}
				string innerText = "";
				if (shellRefrence.refrencesType == 1)
				{
					innerText = "..\\" + shellRefrence.Name;
				}
				else if (shellRefrence.refrencesType == 2)
				{
					innerText = "..\\DLL\\" + shellRefrence.Name;
				}
				XmlElement xmlElement3 = xmlDocument.CreateElement("Reference", documentElement.NamespaceURI);
				XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("Include");
				xmlAttribute2.InnerText = shellRefrence.Name.Substring(0, shellRefrence.Name.LastIndexOf('.'));
				xmlElement3.SetAttributeNode(xmlAttribute2);
				XmlElement xmlElement4 = xmlDocument.CreateElement("Reference", documentElement.NamespaceURI);
				xmlElement3.AppendChild(CreateNewLelment(xmlDocument, "HintPath", innerText, documentElement.NamespaceURI));
				xmlElement3.AppendChild(CreateNewLelment(xmlDocument, "Private", "False", documentElement.NamespaceURI));
				xmlElement.AppendChild(xmlElement3);
			}
			xmlDocument.Save(filename);
		}
		catch (Exception ex)
		{
			LogHelper.Error("setScriptToSolutionRefrence exception:" + ex.ToString());
		}
		return true;
	}

	private XmlElement CreateNewLelment(XmlDocument doc, string nodeName, string innerText, string namespaceUrl)
	{
		XmlElement xmlElement = doc.CreateElement(nodeName, namespaceUrl);
		xmlElement.InnerText = innerText;
		return xmlElement;
	}

	private string GetOutDllRefrenceNode(string filepath)
	{
		try
		{
			if (!File.Exists(filepath))
			{
				return null;
			}
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(filepath);
			return $"{assemblyName.FullName},processorArchitecture={assemblyName.ProcessorArchitecture.ToString()}";
		}
		catch (Exception ex)
		{
			LogHelper.Error("GetOutDllRefrenceNode Error," + ex.Message);
			return null;
		}
	}

	private void m_CheckScriptTime_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (!bStartCheck)
		{
			return;
		}
		string text = AppBaseDirectory + "\\GlobalUserScript\\UserGlobalScript.cs";
		if (!File.Exists(text))
		{
			return;
		}
		FileInfo fileInfo = new FileInfo(text);
		DateTime lastWriteTime = fileInfo.LastWriteTime;
		if (lastWriteTime.Subtract(objLastWriteTime).TotalSeconds > 2.0)
		{
			objLastWriteTime = lastWriteTime;
			string text2 = File.ReadAllText(text, Encoding.UTF8);
			if (text2 != globalScriptDataContext.GlobalScriptContent)
			{
				globalScriptDataContext.GlobalScriptContent = text2;
				SendDataToServer("updateScript", 0, text);
			}
		}
	}

	private uint getScriptInfo(ref dynamic returnBody)
	{
		uint result = 0u;
		string text = "";
		if (string.IsNullOrEmpty(globalScriptDataContext.GlobalScriptContent))
		{
			if (!string.IsNullOrEmpty(globalScriptDataContext.GlobalScriptDefault))
			{
				globalScriptDataContext.GlobalScriptContent = globalScriptDataContext.GlobalScriptDefault;
				text = AppBaseDirectory + Guid.NewGuid().ToString("N") + "GlobalScript.txt";
				if (!WriteStringToFile(text, globalScriptDataContext.GlobalScriptContent))
				{
					result = 3758096896u;
					text = "";
				}
			}
			else if (File.Exists(strDefaultGlobalScript))
			{
				globalScriptDataContext.GlobalScriptContent = File.ReadAllText(strDefaultGlobalScript, Encoding.UTF8);
				text = AppBaseDirectory + Guid.NewGuid().ToString("N") + "GlobalScript.txt";
				File.Copy(strDefaultGlobalScript, text);
			}
			else
			{
				result = 3758096896u;
			}
		}
		else
		{
			text = AppBaseDirectory + Guid.NewGuid().ToString("N") + "GlobalScript.txt";
			if (!WriteStringToFile(text, globalScriptDataContext.GlobalScriptContent))
			{
				result = 3758096896u;
				text = "";
			}
		}
		returnBody = new
		{
			password = globalScriptDataContext.GlobalScriptPassword,
			filePath = text,
			refrences = JsonConvert.SerializeObject(globalScriptDataContext.GlobalScriptRefences)
		};
		return result;
	}

	private uint getScriptCompileInfo(ref dynamic returnBody)
	{
		uint result = 0u;
		string text = "";
		if (!string.IsNullOrEmpty(globalScriptDataContext.GlobalScriptComplieResult))
		{
			text = AppBaseDirectory + Guid.NewGuid().ToString("N") + "GlobalComplie.txt";
			if (!WriteStringToFile(text, globalScriptDataContext.GlobalScriptComplieResult))
			{
				result = 3758096896u;
				text = "";
			}
		}
		returnBody = new
		{
			filePath = text
		};
		return result;
	}

	private bool WriteStringToFile(string path, string info)
	{
		try
		{
			using (StreamWriter streamWriter = new StreamWriter(path, append: false, Encoding.UTF8))
			{
				streamWriter.Write(info);
			}
			return true;
		}
		catch (Exception ex)
		{
			LogHelper.Error("StreamWriter Write File Error," + ex.Message);
			return false;
		}
	}

	private void updateBackFile(string password, string content)
	{
		try
		{
			SaveInfo saveInfo = new SaveInfo();
			saveInfo.ScriptPassword = password;
			saveInfo.ScriptContent = content;
			string value = JsonConvert.SerializeObject(saveInfo);
			using (StreamWriter streamWriter = new StreamWriter(strBackGlobalScript, append: false, Encoding.UTF8))
			{
				streamWriter.Write(value);
			}
			LogHelper.Info("Update BackFile end");
		}
		catch (Exception ex)
		{
			LogHelper.Error("Update BackFile is error," + ex.Message);
		}
	}

	private bool readScriptDataFromFile(string filepath)
	{
		if (!File.Exists(filepath))
		{
			LogHelper.Error("GlobalScript File is not exit,file name:" + filepath);
			return false;
		}
		try
		{
			string value = File.ReadAllText(filepath, Encoding.UTF8);
			SaveInfo saveInfo = JsonConvert.DeserializeObject<SaveInfo>(value);
			globalScriptDataContext.GlobalScriptContent = saveInfo.ScriptContent;
			globalScriptDataContext.GlobalScriptPassword = saveInfo.ScriptPassword;
			globalScriptDataContext.GlobalScriptRefences = saveInfo.ScriptRefences;
			return true;
		}
		catch (Exception ex)
		{
			LogHelper.Error("Read script file is error," + ex.Message);
			return false;
		}
	}

	private uint readScriptDataFromMapFile(string filepath)
	{
		if (string.IsNullOrEmpty(filepath))
		{
			LogHelper.Error("GlobalScript map file is error:" + filepath);
			return 3758096899u;
		}
		try
		{
			string text = "";
			if (IsSingleProcessMode)
			{
				text = filepath;
			}
			else
			{
				byte[] array = MemoryHelper.VMReadFromMemory(filepath, 16);
				if (array == null)
				{
					LogHelper.Error("VMReadFromMemory return bytes is null");
					return 3758096899u;
				}
				text = Encoding.UTF8.GetString(array);
			}
			int num = text.IndexOf("{");
			int num2 = text.LastIndexOf("}");
			if (num == num2)
			{
				LogHelper.Error("startIndex is error");
				return 3758096899u;
			}
			string value = text.Substring(num, num2 - num + 1);
			SaveInfo saveInfo = JsonConvert.DeserializeObject<SaveInfo>(value);
			globalScriptDataContext.GlobalScriptContent = saveInfo.ScriptContent;
			globalScriptDataContext.GlobalScriptPassword = saveInfo.ScriptPassword;
			globalScriptDataContext.GlobalScriptRefences = saveInfo.ScriptRefences;
			globalScriptDataContext.IsComplieFinish = false;
			isLoadSol = true;
			Task.Run(() =>
			{
				if (globalScriptDataContext.GlobalScriptRefences != null && globalScriptDataContext.GlobalScriptRefences.Count > 0)
				{
					if (!IsLoadNeuFile)
					{
						LoadNeufileName();
						IsLoadNeuFile = true;
					}
					UserGlobalScriptSupport.GetScriptInstance().SetRefrences(GetRefrences());
					setScriptToSolutionRefrence(globalScriptDataContext.GlobalScriptRefences);
				}
				else
				{
					setScriptToSolutionRefrence(DefaultRefrences);
				}
				setScriptToSolutionFile(globalScriptDataContext.GlobalScriptContent);
				initSourceAndStartScript(bCompile: true, bSclienceRunonce: false, bLoadSol: true);
				nLoadSolCount++;
				if (nLoadSolCount >= 65535)
				{
					nLoadSolCount = 0;
				}
			});
			return 0u;
		}
		catch (Exception ex)
		{
			LogHelper.Error("Read script file is error," + ex.ToString());
			return 3758096899u;
		}
	}

	public void Dispose()
	{
		try
		{
			if (IsSingleProcessMode)
			{
				CloseScript(bUnsdk: true);
				AssemblyManager.ClearAeembly();
			}
			else
			{
				CloseScript(bUnsdk: true);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("UserGlobalScriptManger Dispose Error" + ex.Message);
		}
	}

	private void LoadNeufileName()
	{
		string text = VMBaseDllPath + "\\Tools\\NEUFILE\\NeuFileNameMap.xml";
		DirectoryInfo directoryInfo = new DirectoryInfo(VMRegisterPath);
		if (directoryInfo == null || directoryInfo.Parent == null)
		{
			return;
		}
		string fullName = directoryInfo.Parent.FullName;
		string text2 = fullName + "\\Tool\\NeuFileNameMap.xml";
		string text3 = "";
		if (File.Exists(text))
		{
			text3 = text;
		}
		else if (File.Exists(text2))
		{
			text3 = text2;
		}
		if (string.IsNullOrEmpty(text3))
		{
			return;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(text3);
		XmlElement documentElement = xmlDocument.DocumentElement;
		XmlElement xmlElement = null;
		foreach (XmlNode childNode in documentElement.ChildNodes)
		{
			if (childNode.Name == "filenames")
			{
				xmlElement = (XmlElement)childNode;
				break;
			}
		}
		foreach (XmlNode childNode2 in xmlElement.ChildNodes)
		{
			if (childNode2.Name == "filename")
			{
				string stdDLLName = childNode2.Attributes["name"].Value + ".dll";
				string neuDLLName = childNode2.InnerText + ".dll";
				DLLNameMap item = new DLLNameMap
				{
					StdDLLName = stdDLLName,
					NeuDLLName = neuDLLName
				};
				mapNameInfo.Add(item);
			}
		}
	}
}
