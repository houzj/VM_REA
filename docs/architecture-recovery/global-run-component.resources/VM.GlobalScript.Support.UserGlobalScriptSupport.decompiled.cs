using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using VM.GlobalScript.Methods;

namespace VM.GlobalScript.Support;

public class UserGlobalScriptSupport
{
	private AppDomainSetup _objSetup;

	private AppDomain _objAppDomain;

	private object _objectClass;

	public ArrayList ResultInfo;

	private RemoteLoader objRemoteLoader = null;

	private DateTime lastDateTime;

	private bool _isExecuteInit = false;

	private bool _isCompileOK = false;

	private bool isUsePlatformSDK = true;

	private static UserGlobalScriptSupport _userGlobalScriptSupportInstance = null;

	private string appBaseDirectory;

	private string VMBaseDllPath = "D:\\Program\\VM4.3.0\\Public_Release\\myLibs\\";

	public string VMRegisterPath = null;

	public ArrayList ArrayRefrences = null;

	public bool IsUsePlatformSDK
	{
		get
		{
			return isUsePlatformSDK;
		}
		set
		{
			isUsePlatformSDK = value;
		}
	}

	public string AppBaseDirectory
	{
		get
		{
			return appBaseDirectory;
		}
		set
		{
			try
			{
				if (value != null)
				{
					appBaseDirectory = value;
					DirectoryInfo directoryInfo = new DirectoryInfo(appBaseDirectory);
					VMBaseDllPath = directoryInfo.Parent.FullName;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("Set app base directory error: " + ex.Message);
			}
		}
	}

	public event Action<string, int, string> UpdateUIScriptEvent = null;

	public static UserGlobalScriptSupport GetScriptInstance()
	{
		if (_userGlobalScriptSupportInstance == null)
		{
			_userGlobalScriptSupportInstance = new UserGlobalScriptSupport();
		}
		return _userGlobalScriptSupportInstance;
	}

	private UserGlobalScriptSupport()
	{
		_objectClass = null;
		_objSetup = null;
		_objAppDomain = null;
		ResultInfo = null;
		AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
	}

	private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	{
		Assembly assembly = null;
		AssemblyName assemblyName = new AssemblyName(args.Name);
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		if (assemblies == null)
		{
			return assembly;
		}
		Assembly[] array = assemblies;
		foreach (Assembly assembly2 in array)
		{
			if (assembly2.FullName.Equals(args.Name, StringComparison.CurrentCultureIgnoreCase))
			{
				assembly = assembly2;
				break;
			}
		}
		if (assembly == null)
		{
			string path = AppBaseDirectory + "DLL\\" + assemblyName.Name + ".dll";
			if (!File.Exists(path))
			{
				path = AppBaseDirectory + assemblyName.Name + ".dll";
			}
			if (File.Exists(path))
			{
				byte[] array2 = null;
				using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					int num = (int)fileStream.Length;
					array2 = new byte[num];
					int num2 = fileStream.Read(array2, 0, array2.Length);
				}
				if (array2 != null)
				{
					assembly = Assembly.Load(array2);
				}
			}
		}
		return assembly;
	}

	private bool InitSource()
	{
		_objSetup = new AppDomainSetup();
		if (_objSetup == null)
		{
			return false;
		}
		_objSetup.ApplicationBase = AppBaseDirectory;
		_objAppDomain = AppDomain.CreateDomain("MyAppDomain", null, _objSetup);
		if (_objAppDomain == null)
		{
			return false;
		}
		return true;
	}

	private void unLoad()
	{
		try
		{
			if (objRemoteLoader != null)
			{
				objRemoteLoader.Unload();
				objRemoteLoader = null;
			}
			if (_objAppDomain != null)
			{
				AppDomain.Unload(_objAppDomain);
				_objAppDomain = null;
				_objSetup = null;
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("unLoad is error," + ex.ToString());
		}
	}

	public void DisposeAndUnload()
	{
		DisposeObjectClass();
		_objectClass = null;
		unLoad();
	}

	private bool StartAppdomain(string shellContent, bool isCompile = true)
	{
		try
		{
			if (!IsUsePlatformSDK)
			{
				LogHelper.Debug("StartAppdomain start");
				_objSetup = new AppDomainSetup();
				_objSetup.ApplicationBase = AppBaseDirectory;
				_objSetup.ShadowCopyFiles = "false";
				_objAppDomain = AppDomain.CreateDomain("myAppdomain", null, _objSetup);
				objRemoteLoader = (RemoteLoader)_objAppDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().FullName, typeof(RemoteLoader).FullName);
				if (objRemoteLoader != null)
				{
					bool flag = true;
					if (isCompile)
					{
						flag = objRemoteLoader.Compile(shellContent, GetRefrences(), compileDebug: false, out ResultInfo, ref lastDateTime);
					}
					if (flag)
					{
						_objAppDomain.SetData("sdkfunction", PlatformSdkFunction.GetInstance().m_operateHandle);
						objRemoteLoader.SetObjData();
						flag = objRemoteLoader.CreateShellInstance(ref ResultInfo);
					}
					LogHelper.Debug("StartAppdomain end");
					return flag;
				}
				return false;
			}
			objRemoteLoader = new RemoteLoader(AppBaseDirectory);
			bool result = (result = objRemoteLoader.CreateShellInstance(shellContent, GetRefrences(), compileDebug: false, out ResultInfo, ref lastDateTime, isCompile));
			LogHelper.Debug("StartAppdomain end");
			return result;
		}
		catch (Exception ex)
		{
			LogHelper.Error("start appdomain is error." + ex.ToString());
			return false;
		}
	}

	public string CompileCode(string shellContent, out bool compileOK)
	{
		string compileError = "";
		compileOK = false;
		try
		{
			DisposeObjectClass();
			unLoad();
			StartAppdomain(shellContent);
			GetErrorInfo(ResultInfo, out compileError, out compileOK);
			LogHelper.Info(compileError);
		}
		catch (Exception ex)
		{
			LogHelper.Error("complie code error," + ex.Message);
		}
		return compileError;
	}

	public void DisposeObjectClass()
	{
		try
		{
			if (objRemoteLoader != null)
			{
				bool bReturn = false;
				objRemoteLoader.ExecuteMethod("Dispose", null, ref bReturn);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("DisposeObjectClass Error " + ex.Message);
		}
	}

	public void InitProcessMsg()
	{
		try
		{
			if (_objectClass != null)
			{
				_objectClass.GetType().InvokeMember("InitProcessID", BindingFlags.InvokeMethod, null, _objectClass, null);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("init processid error，" + ex.Message);
		}
	}

	public void LoadSolutionInit()
	{
		try
		{
			if (AppDomain.CurrentDomain == null)
			{
				LogHelper.Error("LoadSolutionInit AppDomain is null ");
			}
			else if (objRemoteLoader != null)
			{
				bool bReturn = false;
				objRemoteLoader?.ExecuteMethod("InitAfterLoadSol", null, ref bReturn);
				objRemoteLoader?.ExecuteMethod("StartTryGlobalCommunicate", null, ref bReturn);
			}
		}
		catch (Exception ex)
		{
			LogHelper.Error("LoadSolutionInit Error " + ex.Message);
		}
	}

	public void GetErrorInfo(ArrayList compileResult, out string compileError, out bool compileOk)
	{
		if (compileResult == null)
		{
			compileError = string.Format("{0} : Compile complete -- no compile result \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
			compileOk = false;
			return;
		}
		int num = 0;
		int num2 = 0;
		string text = "";
		string text2 = "";
		foreach (object item in compileResult)
		{
			if (item != null)
			{
				HikCompileMessage hikCompileMessage = (HikCompileMessage)item;
				if (hikCompileMessage.IsError)
				{
					num2++;
					text2 += $"Line:{hikCompileMessage.Line} -- Error:{hikCompileMessage.Text}\r\n";
				}
				else
				{
					num++;
					text2 += $"Line:{hikCompileMessage.Line} -- Warnings:{hikCompileMessage.Text}\r\n";
				}
			}
		}
		text = string.Format("{0} : Compile complete -- {1} Errors, {2} Warnings \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), num2, num);
		compileError = text + text2;
		_isCompileOK = (compileOk = num2 == 0);
	}

	private ArrayList GetRefrences()
	{
		if (ArrayRefrences != null && ArrayRefrences.Count > 0)
		{
			return ArrayRefrences;
		}
		ArrayRefrences = new ArrayList();
		ArrayRefrences.Add("System.dll");
		ArrayRefrences.Add("System.Core.dll");
		ArrayRefrences.Add("System.Windows.dll");
		ArrayRefrences.Add("System.Windows.Forms.dll");
		ArrayRefrences.Add("System.Drawing.dll");
		ArrayRefrences.Add(AppBaseDirectory + "iMVS-6000PlatformSDKCS.dll");
		ArrayRefrences.Add(AppBaseDirectory + "VM.GlobalScript.Methods.dll");
		ArrayRefrences.Add(AppBaseDirectory + "Apps.Json.dll");
		if (IsUsePlatformSDK)
		{
			ArrayRefrences.Add(GetVMAssemblyPath(VMRegisterPath + "VM.Core.dll", "VM.Core"));
			ArrayRefrences.Add(GetVMAssemblyPath(VMRegisterPath + "VM.PlatformSDKCS.dll", "VM.PlatformSDKCS"));
			ArrayRefrences.Add(GetVMAssemblyPath(VMRegisterPath + "VMControls.BaseInterface.dll", "VMControls.BaseInterface"));
			ArrayRefrences.Add(GetVMAssemblyPath(VMRegisterPath + "VMControls.Interface.dll", "VMControls.Interface"));
			ArrayRefrences.Add(GetVMAssemblyPath(VMRegisterPath + "VMControls.RenderInterface.dll", "VMControls.RenderInterface"));
			string vMAssemblyPath = GetVMAssemblyPath(VMRegisterPath + "ImageSourceModuleCs.dll", "ImageSourceModuleCs");
			if (!File.Exists(vMAssemblyPath))
			{
				LogHelper.Error($"{vMAssemblyPath} is not exit");
			}
			else
			{
				ArrayRefrences.Add(vMAssemblyPath);
			}
			vMAssemblyPath = GetVMAssemblyPath(VMRegisterPath + "IMVSFastFeatureMatchModuCs.dll", "IMVSFastFeatureMatchModuCs");
			if (!File.Exists(vMAssemblyPath))
			{
				LogHelper.Error($"{vMAssemblyPath} is not exit");
			}
			else
			{
				ArrayRefrences.Add(vMAssemblyPath);
			}
		}
		return ArrayRefrences;
	}

	private string GetVMAssemblyPath(string assemblyPath, string assemblyName)
	{
		if (!string.IsNullOrEmpty(assemblyName))
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly assembly = assemblies.FirstOrDefault((Assembly x) => x.GetName().Name == assemblyName);
			if (assembly != null)
			{
				return assembly.Location;
			}
		}
		return assemblyPath;
	}

	public void SetRefrences(string[] refresces)
	{
		if (refresces != null && refresces.Length != 0)
		{
			if (ArrayRefrences == null)
			{
				ArrayRefrences = new ArrayList();
			}
			ArrayRefrences.Clear();
			Array.ForEach(refresces, (string x) =>
			{
				ArrayRefrences.Add(x);
			});
		}
	}

	public bool CodeInitFunction(bool bExecuteContinues, bool isCrash)
	{
		if (objRemoteLoader == null)
		{
			return false;
		}
		if (!_isCompileOK)
		{
			return false;
		}
		try
		{
			bool bReturn = false;
			objRemoteLoader.ExecuteMethod("DefaultInitProcess", new object[2] { bExecuteContinues, isCrash }, ref bReturn);
			return bReturn;
		}
		catch (Exception ex)
		{
			LogHelper.Error("Global Script Execute CodeInitFunction " + ex.StackTrace);
			return false;
		}
	}

	public bool LoadExternAssembly(bool bExecuteContinues, bool isCrash)
	{
		try
		{
			if (!objRemoteLoader.GetIsReLoadAssembly(ref lastDateTime))
			{
				return true;
			}
			DisposeObjectClass();
			_objectClass = null;
			unLoad();
			GC.Collect();
			if (!StartAppdomain("", isCompile: false))
			{
				return false;
			}
			if (UpdateUIScriptEvent != null)
			{
				UpdateUIScriptEvent("updateScript", 0, AppBaseDirectory + "\\GlobalUserScript\\UserGlobalScript.cs");
			}
			return CodeInitFunction(bExecuteContinues, isCrash);
		}
		catch (Exception ex)
		{
			LogHelper.Error("LoadExternAssembly is error," + ex.ToString());
			return false;
		}
	}

	public bool CodeRun(ref int errorCode)
	{
		errorCode = 0;
		if (objRemoteLoader != null)
		{
			bool flag = objRemoteLoader.ExecuteProcessMethod(ref errorCode);
		}
		if ((long)errorCode != 0)
		{
			return false;
		}
		return true;
	}

	public uint GetScriptContinusExecuteInterval()
	{
		if (objRemoteLoader != null)
		{
			return objRemoteLoader.GetScriptContinusExecuteInterval();
		}
		return 100u;
	}
}
