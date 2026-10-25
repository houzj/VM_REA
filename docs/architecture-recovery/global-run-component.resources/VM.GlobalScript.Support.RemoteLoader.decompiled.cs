using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using VM.GlobalScript.Methods;

namespace VM.GlobalScript.Support;

public class RemoteLoader : MarshalByRefObject
{
	private Type _Type = null;

	private Assembly _objAssembly = null;

	private object _objectClass = null;

	private IScriptMethods _iProcessClass = null;

	private const string OUT_ASSEMBLY_NAME = "GlobalUserScript.dll";

	private const string OUT_ASSEMBLY_PDB_NAME = "GlobalUserScript.pdb";

	private CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

	private string ApplicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

	public RemoteLoader(string applicatPath)
	{
		ApplicationPath = applicatPath;
	}

	public RemoteLoader()
	{
	}

	public void LoadAssembly(string assemblyFile, string typeName)
	{
		try
		{
			if (!string.IsNullOrEmpty(assemblyFile) && !string.IsNullOrEmpty(typeName))
			{
				_objAssembly = Assembly.LoadFrom(assemblyFile);
				_Type = _objAssembly.GetType(typeName);
				if (!(_Type == null))
				{
					_objectClass = Activator.CreateInstance(_Type);
				}
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public void Unload()
	{
		_iProcessClass = null;
		_objectClass = null;
		_objAssembly = null;
		objCSharpCodePrivoder.Dispose();
		objCSharpCodePrivoder = null;
	}

	public override object InitializeLifetimeService()
	{
		return null;
	}

	public void CreateInstance(string assemblyFile, string typeName, object[] constructArgs)
	{
		if (_Type == null)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyFile);
			AppDomain.CurrentDomain.Load(fileNameWithoutExtension);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				if (!(assembly == null) && Path.GetFileName(assembly.Location).Equals(assemblyFile, StringComparison.CurrentCultureIgnoreCase))
				{
					_objAssembly = assembly;
				}
			}
			if (_objAssembly == null)
			{
				return;
			}
			Type[] types = _objAssembly.GetTypes();
			Type type = null;
			Type[] array2 = types;
			foreach (Type type2 in array2)
			{
				if (!(type2 == null))
				{
					if (type2.FullName == typeName)
					{
						type = type2;
						break;
					}
					if (type2.BaseType != null && type2.BaseType.FullName == typeName)
					{
						type = type2;
						break;
					}
					if (type2.GetInterface(typeName) != null)
					{
						type = type2;
						break;
					}
				}
			}
			if (type == null)
			{
				return;
			}
			_Type = type;
		}
		_objectClass = Activator.CreateInstance(_Type);
	}

	public bool CreateInstanceFromByte(string assemblyFile, string typeName, object[] constructArgs)
	{
		try
		{
			if (_Type == null)
			{
				if (_objAssembly == null)
				{
					byte[] array = null;
					using (FileStream fileStream = new FileStream(assemblyFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						int num = (int)fileStream.Length;
						array = new byte[num];
						int num2 = fileStream.Read(array, 0, array.Length);
					}
					if (array != null)
					{
						_objAssembly = Assembly.Load(array);
					}
				}
				if (_objAssembly == null)
				{
					return false;
				}
				Type[] types = _objAssembly.GetTypes();
				if (types == null)
				{
					return false;
				}
				Type type = null;
				Type[] array2 = types;
				foreach (Type type2 in array2)
				{
					if (!(type2 == null))
					{
						if (type2.FullName == typeName)
						{
							type = type2;
							break;
						}
						if (type2.BaseType != null && type2.BaseType.FullName == typeName)
						{
							type = type2;
							break;
						}
						if (type2.GetInterface(typeName) != null)
						{
							type = type2;
							break;
						}
					}
				}
				if (type == null)
				{
					return false;
				}
				_Type = type;
			}
			_objectClass = Activator.CreateInstance(_Type);
			return _objectClass != null;
		}
		catch (Exception ex)
		{
			LogHelper.Error("CreateInstance " + ex.ToString());
			return false;
		}
	}

	public bool Compile(string source, ArrayList references, bool compileDebug, out ArrayList ResultInfo, ref DateTime lastDateTime)
	{
		LogHelper.Debug("user compile start");
		ResultInfo = new ArrayList();
		if (references == null)
		{
			ResultInfo.Add(new HikCompileMessage("LanguageCompileExeception", 0, 0, warning: false));
			return false;
		}
		try
		{
			GC.Collect();
			if (File.Exists(ApplicationPath + "GlobalUserScript.dll"))
			{
				File.Delete(ApplicationPath + "GlobalUserScript.dll");
			}
			if (File.Exists(ApplicationPath + "GlobalUserScript.pdb"))
			{
				File.Delete(ApplicationPath + "GlobalUserScript.pdb");
			}
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.GenerateExecutable = false;
			compilerParameters.GenerateInMemory = false;
			compilerParameters.IncludeDebugInformation = false;
			compilerParameters.OutputAssembly = ApplicationPath + "GlobalUserScript.dll";
			compilerParameters.WarningLevel = 4;
			string location = typeof(HikCompileMessage).Assembly.Location;
			if (!references.Contains(location))
			{
				references.Add(typeof(HikCompileMessage).Assembly.Location);
			}
			foreach (object reference in references)
			{
				string value = (string)reference;
				compilerParameters.ReferencedAssemblies.Add(value);
			}
			string text = "/lib:";
			string text2 = ApplicationPath + "DLL";
			string compilerOptions = compilerParameters.CompilerOptions;
			compilerParameters.CompilerOptions = compilerOptions + " \"" + text + text2 + "\"";
			compilerParameters.CompilerOptions += " /unsafe";
			DateTime now = DateTime.Now;
			CompilerResults compilerResults = null;
			try
			{
				LogHelper.Debug("CompileAssemblyFromSource start");
				compilerResults = objCSharpCodePrivoder.CompileAssemblyFromSource(compilerParameters, source);
				LogHelper.Debug("CompileAssemblyFromSource end");
				if (File.Exists(ApplicationPath + "GlobalUserScript.dll"))
				{
					FileInfo fileInfo = new FileInfo(ApplicationPath + "GlobalUserScript.dll");
					lastDateTime = fileInfo.LastWriteTime;
				}
				else
				{
					lastDateTime = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("compile is exception:" + ex.ToString());
				ResultInfo.Add(new HikCompileMessage("编译错误：可能未正确添加引用集", 0, 0, warning: false));
				return false;
			}
			DateTime now2 = DateTime.Now;
			bool flag = false;
			for (int i = 0; i < compilerResults.Errors.Count; i++)
			{
				if (!compilerResults.Errors[i].IsWarning)
				{
					flag = true;
				}
				HikCompileMessage value2 = new HikCompileMessage(compilerResults.Errors[i].ErrorText, compilerResults.Errors[i].Line, compilerResults.Errors[i].Column, compilerResults.Errors[i].IsWarning);
				ResultInfo.Add(value2);
			}
			if (flag)
			{
				return false;
			}
			LogHelper.Debug("user compile end");
		}
		catch (Exception ex2)
		{
			ResultInfo.Add(new HikCompileMessage("LanguageCompileExeception，" + ex2.Message, 0, 0, warning: false));
			LogHelper.Error("编译异常信息:" + ex2.ToString());
			_objAssembly = null;
			_objectClass = null;
			return false;
		}
		return true;
	}

	public bool CreateShellInstance(ref ArrayList ResultInfo)
	{
		if (!CreateInstanceFromByte(ApplicationPath + "GlobalUserScript.dll", "UserGlobalScript", null))
		{
			return false;
		}
		return CheckClass(out ResultInfo);
	}

	public bool CreateShellInstance(string source, ArrayList references, bool compileDebug, out ArrayList ResultInfo, ref DateTime lastDateTime, bool isCompile)
	{
		bool flag = false;
		ResultInfo = new ArrayList();
		if (isCompile)
		{
			Assembly exitAssembly = AssemblyManager.GetExitAssembly(source);
			if (exitAssembly != null)
			{
				_objAssembly = exitAssembly;
				flag = CreateShellInstance(ref ResultInfo);
			}
			else
			{
				flag = Compile(source, references, compileDebug, out ResultInfo, ref lastDateTime);
				if (flag)
				{
					flag = CreateShellInstance(ref ResultInfo);
					if (flag)
					{
						AssemblyManager.AddAssembly(source, _objAssembly);
					}
				}
			}
		}
		else
		{
			flag = CreateShellInstance(ref ResultInfo);
		}
		return flag;
	}

	public bool GetIsReLoadAssembly(ref DateTime ldatetime)
	{
		if (!File.Exists(ApplicationPath + "GlobalUserScript.dll"))
		{
			return false;
		}
		FileInfo fileInfo = new FileInfo(ApplicationPath + "GlobalUserScript.dll");
		DateTime lastWriteTime = fileInfo.LastWriteTime;
		if (lastWriteTime.Subtract(ldatetime).TotalSeconds > 5.0)
		{
			ldatetime = lastWriteTime;
			return true;
		}
		return false;
	}

	public bool CheckClass(out ArrayList myResut)
	{
		myResut = new ArrayList();
		if (_objectClass == null)
		{
			myResut.Add(new HikCompileMessage("LanguageCompileCreateObjectError UserGlobalScript", 0, 0, warning: false));
			_objAssembly = null;
			return false;
		}
		if (null == _objectClass.GetType().GetMethod("Init"))
		{
			myResut.Add(new HikCompileMessage("LanguageCompileMissFunctionError Init()", 0, 0, warning: false));
			_objAssembly = null;
			_objectClass = null;
			return false;
		}
		if (null == _objectClass.GetType().GetMethod("Process"))
		{
			myResut.Add(new HikCompileMessage("LanguageCompileMissFunctionError Process()", 0, 0, warning: false));
			_objAssembly = null;
			_objectClass = null;
			return false;
		}
		_iProcessClass = (IScriptMethods)_objectClass;
		try
		{
			if (_iProcessClass != null)
			{
				int num = _iProcessClass.Init();
				if (num != 0)
				{
					myResut.Add(new HikCompileMessage("LanguageCompileExecuteError Init()，" + num, 0, 0, warning: false));
					_objAssembly = null;
					_objectClass = null;
					return false;
				}
			}
			else
			{
				_objectClass.GetType().InvokeMember("Init", BindingFlags.InvokeMethod, null, _objectClass, null);
			}
		}
		catch (Exception ex)
		{
			myResut.Add(new HikCompileMessage("LanguageCompileExecuteError Init()，" + ex.Message, 0, 0, warning: false));
			_objAssembly = null;
			_objectClass = null;
			return false;
		}
		return true;
	}

	public void SetObjData()
	{
		PlatformSdkFunction.GetInstance().m_operateHandle = (IntPtr)AppDomain.CurrentDomain.GetData("sdkfunction");
	}

	public uint GetScriptContinusExecuteInterval()
	{
		return PlatformSdkFunction.GetInstance().ScriptContinusExecuteInterval;
	}

	public object ExecuteMethod(string methodName, object[] args, ref bool bReturn)
	{
		bReturn = false;
		if (_objectClass == null)
		{
			bReturn = false;
			return null;
		}
		try
		{
			bReturn = true;
			return _objectClass.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, _objectClass, args);
		}
		catch (Exception)
		{
			bReturn = false;
			return null;
		}
	}

	public bool ExecuteProcessMethod(ref int errorCode)
	{
		errorCode = 0;
		if (_objectClass == null || _objAssembly == null)
		{
			errorCode = -536870911;
			return false;
		}
		try
		{
			if (_iProcessClass != null)
			{
				errorCode = _iProcessClass.Process();
			}
			else
			{
				object obj = _objectClass.GetType().InvokeMember("Process", BindingFlags.InvokeMethod, null, _objectClass, null);
				if (obj != null)
				{
					int.TryParse(obj.ToString(), out errorCode);
				}
			}
			if ((long)errorCode != 0)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			errorCode = -536870657;
			LogHelper.Error("process is exception:" + ex.ToString());
			Debugger.Log(0, null, "process is exception:" + ex.ToString());
			return false;
		}
	}
}
