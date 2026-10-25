using System;
using System.IO;
using Microsoft.Win32;

namespace VM.Utility.SingleInstance;

public class CurrentPathHelper
{
	private readonly string _registrykeyPathName = "SOFTWARE\\WOW6432Node\\Microsoft\\.NETFramework\\v4.0.30319\\AssemblyFoldersEx\\VisionMaster";

	private readonly string _vmInstallPath = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

	public bool IsSDKMode = false;

	public bool IsRpMode = false;

	private static readonly CurrentPathHelper instance;

	public static CurrentPathHelper Instance => instance;

	static CurrentPathHelper()
	{
		instance = new CurrentPathHelper();
		string config = ConfigurationHelper.GetConfig("StartClientMode");
		if (string.IsNullOrWhiteSpace(config) || config.Equals("1"))
		{
			instance.IsSDKMode = true;
		}
		else if (config.Equals("2"))
		{
			Instance.IsRpMode = true;
		}
	}

	private CurrentPathHelper()
	{
	}

	public string GetCurrentPath()
	{
		string result = AppDomain.CurrentDomain.BaseDirectory;
		if (IsSDKMode)
		{
			string text = GetRegisterValueByName(_registrykeyPathName, "");
			if (!text.EndsWith("\\"))
			{
				text += "\\";
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				result = text;
			}
		}
		else if (!IsRpMode)
		{
		}
		return result;
	}

	public string GetRegistryPath()
	{
		return GetRegisterValueByName(_registrykeyPathName, "");
	}

	public string GetVMInstallPath()
	{
		string result = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
		if (IsSDKMode)
		{
			string registerValueByName = GetRegisterValueByName(_registrykeyPathName, "CurrentVersion");
			string path = Path.Combine(_vmInstallPath, "VisionMaster_" + registerValueByName);
			string registerValueByName2 = GetRegisterValueByName(path, "InstallPath");
			if (!string.IsNullOrWhiteSpace(registerValueByName2))
			{
				result = registerValueByName2 + "\\Applications\\";
			}
		}
		return result;
	}

	private string GetRegisterValueByName(string path, string name)
	{
		string result = string.Empty;
		try
		{
			RegistryKey localMachine = Registry.LocalMachine;
			if (localMachine != null)
			{
				RegistryKey registryKey = localMachine.OpenSubKey(path);
				if (registryKey != null)
				{
					object value = registryKey.GetValue(name);
					if (value != null && value is string text && !string.IsNullOrWhiteSpace(text))
					{
						result = text;
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
}
