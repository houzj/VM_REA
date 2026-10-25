using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using Apps.Log;

namespace VM.Utility;

public class ConfigurationHelper
{
	private const string DefaultSectionName = "DefaultSection";

	private const string DefaultFileName = "App";

	private const string DefaultOpenPathKey = "ModuleOpenPath";

	private const string DefaultOpenRemotePathKey = "ModuleOpenRemotePath";

	private static bool _isLazyLoadCfg = false;

	private static XDocument _cfgDocument = null;

	public static string GetConfig(string key)
	{
		string text = string.Empty;
		try
		{
			text = ConfigurationManager.AppSettings[key];
			if (text == null)
			{
				LogHelper.Error($"The result is null, key:{key}");
			}
		}
		catch (ConfigurationErrorsException ex)
		{
			LogHelper.Error($"Error reading app settings, key:{key}, error message: {ex.Message}");
		}
		return text;
	}

	public static void SaveConfig(string key, string defaultValue)
	{
		if (defaultValue == GetConfig(key))
		{
			return;
		}
		LogHelper.Info($"Save Config {key}");
		try
		{
			LogHelper.Info("Save config begin");
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetConfig("ApplicationName") + ".exe.config");
			using (TransactionScope transactionScope = new TransactionScope())
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(text);
				XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("appSettings");
				if (elementsByTagName != null && elementsByTagName.Count >= 1)
				{
					XmlNode xmlNode = elementsByTagName[0];
					foreach (XmlNode item in xmlNode)
					{
						if (item != null && item.NodeType != XmlNodeType.Comment)
						{
							XmlAttribute xmlAttribute = item.Attributes["key"];
							XmlAttribute xmlAttribute2 = item.Attributes["value"];
							if (xmlAttribute != null && xmlAttribute2 != null && xmlAttribute.Value.Equals(key))
							{
								xmlAttribute2.Value = defaultValue;
							}
						}
					}
				}
				xmlDocument.Save(text);
				ConfigurationManager.RefreshSection("appSettings");
				transactionScope.Complete();
			}
			LogHelper.Info("Save config end");
			LogHelper.Info("Bak config begin");
			BackupFile(text);
			LogHelper.Info("Bak config end");
		}
		catch (TransactionAbortedException ex)
		{
			LogHelper.Error($"TransactionAbortedException Message: {ex.Message}");
		}
		catch (Exception ex2)
		{
			LogHelper.Error($"Error saving config file, Message: {ex2.Message}");
		}
	}

	private static void Save(string filePath)
	{
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, [In][Out] char[] lpReturnedString, uint nSize, string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnedString, uint nSize, string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

	private static string[] INIGetAllSectionNames(string iniFile)
	{
		uint num = 32767u;
		string[] result = new string[0];
		IntPtr intPtr = Marshal.AllocCoTaskMem((int)(num * 2));
		uint privateProfileSectionNames = GetPrivateProfileSectionNames(intPtr, num, iniFile);
		if (privateProfileSectionNames != 0)
		{
			string text = Marshal.PtrToStringAuto(intPtr, (int)privateProfileSectionNames).ToString();
			result = text.Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}
		Marshal.FreeCoTaskMem(intPtr);
		return result;
	}

	private static string[] INIGetAllItems(string iniFile, string section)
	{
		uint num = 32767u;
		string[] result = new string[0];
		IntPtr intPtr = Marshal.AllocCoTaskMem((int)(num * 2));
		uint privateProfileSection = GetPrivateProfileSection(section, intPtr, num, iniFile);
		if (privateProfileSection != num - 2 || privateProfileSection == 0)
		{
			string text = Marshal.PtrToStringAuto(intPtr, (int)privateProfileSection);
			result = text.Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}
		Marshal.FreeCoTaskMem(intPtr);
		return result;
	}

	private static string[] INIGetAllItemKeys(string iniFile, string section)
	{
		string[] result = new string[0];
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		char[] array = new char[10240];
		uint privateProfileString = GetPrivateProfileString(section, null, null, array, 10240u, iniFile);
		if (privateProfileString != 0)
		{
			result = new string(array).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}
		array = null;
		return result;
	}

	private static string INIGetStringValue(string iniFile, string section, string key, string defaultValue)
	{
		string result = defaultValue;
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("必须指定键名称(key)", "key");
		}
		StringBuilder stringBuilder = new StringBuilder(10240);
		uint privateProfileString = GetPrivateProfileString(section, key, defaultValue, stringBuilder, 10240u, iniFile);
		if (privateProfileString != 0)
		{
			result = stringBuilder.ToString();
		}
		stringBuilder = null;
		return result;
	}

	private static bool INIWriteItems(string iniFile, string section, string items)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		if (string.IsNullOrEmpty(items))
		{
			throw new ArgumentException("必须指定键值对", "items");
		}
		return WritePrivateProfileSection(section, items, iniFile);
	}

	private static bool INIWriteValue(string iniFile, string section, string key, string value)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("必须指定键名称", "key");
		}
		if (value == null)
		{
			throw new ArgumentException("值不能为null", "value");
		}
		return WritePrivateProfileString(section, key, value, iniFile);
	}

	private static bool INIDeleteKey(string iniFile, string section, string key)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("必须指定键名称", "key");
		}
		return WritePrivateProfileString(section, key, null, iniFile);
	}

	private static bool INIDeleteSection(string iniFile, string section)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		return WritePrivateProfileString(section, null, null, iniFile);
	}

	private static bool INIEmptySection(string iniFile, string section)
	{
		if (string.IsNullOrEmpty(section))
		{
			throw new ArgumentException("必须指定节点名称", "section");
		}
		return WritePrivateProfileSection(section, string.Empty, iniFile);
	}

	public static string INIGetStringValue(string keyName, string dirPath = null)
	{
		dirPath = ((dirPath != null) ? (dirPath + "/App.ini") : (AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "App.ini"));
		if (File.Exists(dirPath))
		{
			return INIGetStringValue(dirPath, "DefaultSection", keyName, null);
		}
		return null;
	}

	public static void INISetStringValue(string keyName, string value, string dirPath = null)
	{
		dirPath = ((dirPath != null) ? (dirPath + "/App.ini") : (AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "App.ini"));
		INIWriteValue(dirPath, "DefaultSection", keyName, value);
	}

	public static string INIGetDefaultOpenPath(string dirPath)
	{
		string text = INIGetStringValue("ModuleOpenPath", dirPath);
		if (Directory.Exists(text))
		{
			return text;
		}
		return null;
	}

	public static string INIGetDefaultOpenRemotePath(string dirPath)
	{
		return INIGetStringValue("ModuleOpenRemotePath", dirPath);
	}

	public static void INISetDefaultOpenPath(string value, string dirPath)
	{
		if (value != null && Directory.Exists(value))
		{
			INISetStringValue("ModuleOpenPath", value, dirPath);
		}
	}

	public static void INISetDefaultOpenRemotePath(string value, string dirPath)
	{
		if (value != null)
		{
			INISetStringValue("ModuleOpenRemotePath", value, dirPath);
		}
	}

	public static string GetConfigValue(string strkey)
	{
		string uri = GetConfig("ApplicationName") + ".cfg";
		string text = null;
		try
		{
			if (!_isLazyLoadCfg)
			{
				_cfgDocument = XDocument.Load(uri);
				_isLazyLoadCfg = true;
				if (_cfgDocument == null)
				{
					return null;
				}
			}
			return _cfgDocument.Root.Elements().SingleOrDefault((XElement p) => p.Name == strkey)?.Elements().SingleOrDefault((XElement p) => p.Name == "CurValue")?.Value;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static void SaveConfig(XDocument doc)
	{
		try
		{
			LogHelper.Info("Save cfg begin");
			string text = GetConfig("ApplicationName") + ".cfg";
			using (TransactionScope transactionScope = new TransactionScope())
			{
				using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
				{
					using XmlWriter writer = XmlWriter.Create(fileStream, new XmlWriterSettings
					{
						Encoding = Encoding.UTF8,
						Indent = true
					});
					doc.Save(writer);
					fileStream.Flush();
				}
				transactionScope.Complete();
			}
			LogHelper.Info("Save cfg end");
			LogHelper.Info("Bak cfg begin");
			BackupFile(text);
			LogHelper.Info("Bak cfg end");
		}
		catch (TransactionAbortedException ex)
		{
			LogHelper.Error($"TransactionAbortedException Message: {ex.Message}");
		}
		catch (Exception ex2)
		{
			LogHelper.Error($"Error saving config file, Message: {ex2.Message}");
		}
	}

	public static void SaveConfig(XmlDocument doc)
	{
		try
		{
			LogHelper.Info("Save cfg begin");
			string text = GetConfig("ApplicationName") + ".cfg";
			using (TransactionScope transactionScope = new TransactionScope())
			{
				using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
				{
					using XmlWriter w = XmlWriter.Create(fileStream, new XmlWriterSettings
					{
						Encoding = Encoding.UTF8,
						Indent = true
					});
					doc.Save(w);
					fileStream.Flush();
				}
				transactionScope.Complete();
			}
			LogHelper.Info("Save cfg end");
			LogHelper.Info("Bak cfg begin");
			BackupFile(text);
			LogHelper.Info("Bak cfg end");
		}
		catch (TransactionAbortedException ex)
		{
			LogHelper.Error($"TransactionAbortedException Message: {ex.Message}");
		}
		catch (Exception ex2)
		{
			LogHelper.Error($"Error saving config file, Message: {ex2.Message}");
		}
	}

	private static void BackupFile(string sourceFilePath)
	{
		string directoryName = Path.GetDirectoryName(sourceFilePath);
		string extension = Path.GetExtension(sourceFilePath);
		string path = Path.GetFileNameWithoutExtension(sourceFilePath) + "_temp" + extension;
		string destFileName = Path.Combine(directoryName, path);
		File.Copy(sourceFilePath, destFileName, overwrite: true);
	}
}
