using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Apps.Log;
using VM.Core;
using VM.PlatformSDKCS;
using VM.Utility;
using VM.Utility.SingleInstance;

namespace FrontendUI.Design.Helper;

public class DocumentManager : INotifyPropertyChanged
{
	private string _dmd5;

	public const string SolutionDesignerKey = "DefaultDesign.xaml";

	private string _smd5;

	private string _tmd5;

	private static DocumentManager _instance;

	private MemoryStream _tempxml;

	private MemoryStream _solutionxml;

	private MemoryStream _designxml;

	private byte[] _defaultSubBuffer;

	private static readonly object syncObj = new object();

	private const string DESIGNERBUFFER_KEY = "DESIGNERBUFFER";

	public bool isSolutionDesignerBuffer_Loaded = false;

	public byte[] DefaultSubBuffer => _defaultSubBuffer ?? (_defaultSubBuffer = File.ReadAllBytes(GetSubDefaultDesignerPath()));

	public static DocumentManager InstanceP
	{
		get
		{
			if (_instance == null)
			{
				lock (syncObj)
				{
					if (_instance == null)
					{
						_instance = new DocumentManager();
					}
				}
			}
			return _instance;
		}
	}

	public bool IsLoadDefaultDesignerBuffer => VmSolution.Instance.UserData["DESIGNERBUFFER"] == null;

	public MemoryStream SolutionDesignerBuffer
	{
		get
		{
			if (!isSolutionDesignerBuffer_Loaded)
			{
				isSolutionDesignerBuffer_Loaded = true;
				try
				{
					byte[] array = VmSolution.Instance.UserData["DESIGNERBUFFER"];
					if (array != null)
					{
						MemoryStream input = new MemoryStream(array, 0, array.Length - 1, writable: true, publiclyVisible: true);
						XmlDocument xmlDocument = new XmlDocument();
						MemoryStream solutionxml = new MemoryStream();
						using (XmlReader reader = XmlReader.Create(input))
						{
							input = null;
							xmlDocument.Load(reader);
							string s = xmlDocument.OuterXml.ToString();
							byte[] bytes = Encoding.UTF8.GetBytes(s);
							solutionxml = new MemoryStream(bytes, 0, bytes.Length, writable: true, publiclyVisible: true);
						}
						_solutionxml = solutionxml;
						_smd5 = _solutionxml.GetMd5();
						_dmd5 = _smd5;
					}
					else
					{
						Init();
					}
				}
				catch (Exception message)
				{
					LogHelper.frontedLog.Info(message);
				}
			}
			if (_solutionxml != null)
			{
				_solutionxml.Seek(0L, SeekOrigin.Begin);
			}
			return _solutionxml;
		}
		set
		{
			if (_solutionxml != null)
			{
				_solutionxml.Dispose();
			}
			_solutionxml = value;
			_smd5 = value.GetMd5();
			try
			{
				VmSolution.Instance.UserData["DESIGNERBUFFER"] = StringUtil.UTF8GetBytesPadZero(value.ToArray());
			}
			catch (Exception message)
			{
				LogHelper.frontedLog.Info(message);
			}
		}
	}

	public MemoryStream TempDesignerBuffer
	{
		get
		{
			if (_tempxml != null)
			{
				_tempxml.Seek(0L, SeekOrigin.Begin);
			}
			return _tempxml;
		}
		set
		{
			if (_tempxml != null)
			{
				_tempxml.Dispose();
			}
			_tempxml = value;
			_tmd5 = value.GetMd5();
		}
	}

	internal MemoryStream DesignerBuffer
	{
		get
		{
			if (_designxml != null)
			{
				_designxml.Seek(0L, SeekOrigin.Begin);
			}
			return _designxml;
		}
		set
		{
			if (_designxml != null)
			{
				_designxml.Dispose();
			}
			_designxml = value;
			_dmd5 = value.GetMd5();
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	private DocumentManager()
	{
		LoadSolutionDesigner();
	}

	public void LoadSolutionDesigner()
	{
		string config = ConfigurationHelper.GetConfig("IsRunBackground");
		if (!string.IsNullOrWhiteSpace(config) && int.Parse(config) == 1)
		{
			if (SolutionDesignerBuffer == null)
			{
				Init();
			}
			return;
		}
		string config2 = ConfigurationHelper.GetConfig("IsAutoLoadPath");
		if (!string.IsNullOrWhiteSpace(config2) && bool.Parse(config2))
		{
			if (SolutionDesignerBuffer == null)
			{
				Init();
			}
			return;
		}
		try
		{
			Init();
		}
		catch (Exception message)
		{
			LogHelper.frontedLog.Info(message);
		}
		LogHelper.frontedLog.Info("LoadSolutionDesigner运行界面加载设计器方案完成");
	}

	private string GetSubDefaultDesignerPath()
	{
		string empty = string.Empty;
		if (CurrentPathHelper.Instance.IsSDKMode)
		{
			return $"{CurrentPathHelper.Instance.GetCurrentPath()}/Config/DefaultSubDesign.xaml";
		}
		return $"{CurrentPathHelper.Instance.GetCurrentPath()}/myLibs/Config/DefaultSubDesign.xaml";
	}

	private string GetDefaultDesignerPath()
	{
		string empty = string.Empty;
		if (CurrentPathHelper.Instance.IsSDKMode)
		{
			return string.Format("{0}/Config/{1}", CurrentPathHelper.Instance.GetCurrentPath(), "DefaultDesign.xaml");
		}
		return string.Format("{0}/myLibs/Config/{1}", CurrentPathHelper.Instance.GetCurrentPath(), "DefaultDesign.xaml");
	}

	public void Init()
	{
		LogHelper.frontedLog.Info("从配置的设计器默认文件里读取buffer开始");
		byte[] array = File.ReadAllBytes(GetDefaultDesignerPath());
		MemoryStream input = new MemoryStream(array, 0, array.Length, writable: true, publiclyVisible: true);
		XmlDocument xmlDocument = new XmlDocument();
		MemoryStream ms = new MemoryStream();
		using (XmlReader reader = XmlReader.Create(input))
		{
			input = null;
			xmlDocument.Load(reader);
			string s = xmlDocument.OuterXml.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			ms = new MemoryStream(bytes, 0, bytes.Length, writable: true, publiclyVisible: true);
		}
		InitXMLAndMd5(ms);
		LogHelper.frontedLog.Info("初始化SolutionDesignerBuffer、解决方案md5(_smd5)、设计器保存文件md5值(_dmd5)");
	}

	public static DocumentManager Instance()
	{
		if (_instance == null)
		{
			lock (syncObj)
			{
				if (_instance == null)
				{
					_instance = new DocumentManager();
				}
			}
		}
		return _instance;
	}

	public bool NeedSave()
	{
		if (string.IsNullOrEmpty(_tmd5))
		{
			return false;
		}
		return _tmd5 != _smd5;
	}

	public bool NeedSaveForDesigner()
	{
		if (string.IsNullOrEmpty(_tmd5))
		{
			return false;
		}
		return _tmd5 != _dmd5;
	}

	private void InitXMLAndMd5(MemoryStream ms)
	{
		TempDesignerBuffer = null;
		if (_solutionxml != null)
		{
			_solutionxml.Dispose();
		}
		_solutionxml = ms;
		_smd5 = ms.GetMd5();
		_dmd5 = _smd5;
	}

	internal void ResetBuffer()
	{
		TempDesignerBuffer = null;
		if (_solutionxml != null)
		{
			_solutionxml.Dispose();
		}
		_solutionxml = null;
		isSolutionDesignerBuffer_Loaded = false;
	}

	public virtual void OnPropertyChanged(string prop)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}
}
