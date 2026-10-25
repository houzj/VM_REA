using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Apps.Data;
using Apps.Log;
using VM.Controls;
using VM.Utility;

namespace VM.User;

public class CodeManager : NotifyPropertyChangedBase
{
	public delegate bool AccessEnableCallBack(bool bEnable, AccessUser curLoginUser);

	public delegate bool IsAllAccessCallBack(bool bEnable);

	public AccessEnableCallBack OnAccessEnableCallBack;

	public IsAllAccessCallBack OnAllAccessCallBack;

	public AccessUserInfo _accessCurUserInfo;

	public string accessInfoNodeName = "AccessInfo";

	private string SoftEncryptNodeName = "IsSoftEncrypt";

	private static object syncObj = new object();

	public static CodeManager instance = null;

	private bool _isAdmin = true;

	public static CodeManager Instance => GetInstance();

	public bool IsAccessEnable => _accessCurUserInfo.curLoginUser != AccessUser.None;

	public bool IsAdmin
	{
		get
		{
			return _isAdmin;
		}
		set
		{
			if (_isAdmin != value)
			{
				_isAdmin = value;
				OnPropertyChanged("IsAdmin");
			}
		}
	}

	public string UserName
	{
		get
		{
			string result = string.Empty;
			switch (_accessCurUserInfo.curLoginUser)
			{
			case AccessUser.None:
				result = string.Empty;
				break;
			case AccessUser.Admin:
				result = "Lang_Role_Admin";
				break;
			case AccessUser.Tech:
				result = "Lang_Role_Tech";
				break;
			case AccessUser.Operator:
				result = "Lang_Role_Operator";
				break;
			}
			return result;
		}
	}

	private CodeManager()
	{
	}

	~CodeManager()
	{
	}

	public static CodeManager GetInstance()
	{
		lock (syncObj)
		{
			if (instance == null)
			{
				instance = new CodeManager();
			}
			return instance;
		}
	}

	public bool IsUseSoftEncrypt()
	{
		try
		{
			return Convert.ToBoolean(ConfigurationHelper.GetConfig(SoftEncryptNodeName));
		}
		catch (Exception ex)
		{
			LogHelper.Info("读取软加密信息失败:" + ex.Message);
			throw ex;
		}
	}

	public bool IsAllAccessUser()
	{
		if (_accessCurUserInfo.curLoginUser == AccessUser.Admin || _accessCurUserInfo.curLoginUser == AccessUser.None)
		{
			return true;
		}
		return false;
	}

	public bool SaveAccessInfo()
	{
		try
		{
			string defaultValue = Convert.ToBase64String(StructTOBytes(_accessCurUserInfo));
			ConfigurationHelper.SaveConfig(accessInfoNodeName, defaultValue);
			OnPropertyChanged("IsAccessEnable");
			OnPropertyChanged("IsAdmin");
		}
		catch (Exception ex)
		{
			HikMessageBox.Show("Lang_CfgSaveErr", ex.Message, MessageIcons.Warning);
			return false;
		}
		return true;
	}

	public AccessUser GetLoginUser()
	{
		return _accessCurUserInfo.curLoginUser;
	}

	public static bool CheckCodeFormat(string tempCode)
	{
		if (tempCode.Length < 3 || tempCode.Length > 15)
		{
			return false;
		}
		return true;
	}

	public bool ResetAccess()
	{
		_accessCurUserInfo.curLoginUser = AccessUser.None;
		_accessCurUserInfo.adminEnable = false;
		_accessCurUserInfo.adminCode = null;
		_accessCurUserInfo.techEnable = false;
		_accessCurUserInfo.techCode = null;
		_accessCurUserInfo.operatorEnable = false;
		_accessCurUserInfo.operatorCode = null;
		OnPropertyChanged("LoginUser");
		IsAdmin = true;
		OnAllAccessCallBack?.Invoke(IsAdmin);
		OnAccessEnableCallBack?.Invoke(bEnable: false, AccessUser.None);
		return SaveAccessInfo();
	}

	public bool SetAdminEnable(bool bEnable, string adminCode)
	{
		_accessCurUserInfo.adminEnable = bEnable;
		_accessCurUserInfo.adminCode = adminCode;
		if (ConfigurationHelper.GetConfig("AccessCheckDisk") == "1")
		{
			_accessCurUserInfo.IDInfo = GetDiskID();
		}
		else
		{
			_accessCurUserInfo.IDInfo = "";
		}
		return SaveAccessInfo();
	}

	public bool SetTechEnable(bool bEnable, string techCode)
	{
		_accessCurUserInfo.techEnable = bEnable;
		_accessCurUserInfo.techCode = techCode;
		return SaveAccessInfo();
	}

	public bool SetOperatorEnable(bool bEnable, string operatorCode)
	{
		_accessCurUserInfo.operatorEnable = bEnable;
		_accessCurUserInfo.operatorCode = operatorCode;
		return SaveAccessInfo();
	}

	public bool GetAdminCode(out bool bEnable, out string adminCode)
	{
		bEnable = _accessCurUserInfo.adminEnable;
		adminCode = _accessCurUserInfo.adminCode;
		return true;
	}

	public bool GetTechCode(out bool bEnable, out string techCode)
	{
		bEnable = _accessCurUserInfo.techEnable;
		techCode = _accessCurUserInfo.techCode;
		return true;
	}

	public bool GetOperatorCode(out bool bEnable, out string operatorCode)
	{
		bEnable = _accessCurUserInfo.operatorEnable;
		operatorCode = _accessCurUserInfo.operatorCode;
		return true;
	}

	public void RefreshTitleShowUserName()
	{
		AccessUser curLoginUser = GetInstance()._accessCurUserInfo.curLoginUser;
		if (curLoginUser != AccessUser.None)
		{
			GetInstance().OnAccessEnableCallBack?.Invoke(bEnable: true, curLoginUser);
		}
	}

	public static string GetDiskID()
	{
		string text = "";
		try
		{
			string text2 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Substring(0, 1);
			foreach (ManagementObject item in new ManagementObjectSearcher(new SelectQuery("Win32_Volume")).Get())
			{
				string text3 = item.Properties["Caption"].Value.ToString();
				string text4 = item.Properties["DeviceID"].Value.ToString();
				if (text3.Substring(0, 1) == text2)
				{
					text += text3;
					text += text4;
				}
			}
			LogHelper.Info("获取本机硬盘标识:" + text);
			if (text.Length > 63)
			{
				text = text.Substring(0, 63);
			}
			LogHelper.Info("截断本机硬盘标识:" + text);
			return text;
		}
		catch (Exception ex)
		{
			LogHelper.Info("获取本机硬盘标识失败:" + ex.Message);
			return "unknow";
		}
	}

	private static string GetUserMd5(string originCode)
	{
		string text = "";
		byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(originCode));
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("X");
		}
		return text;
	}

	private XElement GetElement(XDocument xDoc, string nodename)
	{
		if (xDoc.Root == null)
		{
			return null;
		}
		return xDoc.Root.Elements().SingleOrDefault((XElement p) => p.Name == nodename);
	}

	public object BytesToStruct(byte[] bytes, Type type)
	{
		int num = Marshal.SizeOf(type);
		Math.Log(num, 1.0);
		if (num > bytes.Length)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(bytes, 0, intPtr, num);
		object result = Marshal.PtrToStructure(intPtr, type);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public byte[] StructTOBytes(object obj)
	{
		int num = Marshal.SizeOf(obj);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(obj, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}
}
