using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Apps.Data;
using Apps.Localization;
using Apps.Log;
using Apps.UIHelper.Extern;
using Apps.XmlParser.ParamTab;
using Apps.XmlParser.Variable;
using Apps.XmlUI.ParamTab;
using DesignerModifyDataSource;
using Frontend.Base;
using Frontend.Base.Interface;
using FrontendUI.Design.Common;
using FrontendUI.Design.Controls;
using FrontendUI.Design.Helper;
using Hik.Util;
using Mvvm.Commands;
using VM.Controls;
using VM.Core;
using VM.PlatformSDKCS;
using VM.User;
using VM.Utility;
using VM.Utility.SingleInstance;
using VMControls.Interface;
using VMControls.WPF;

namespace FrontendUI.Design;

public class FrontendWindowData : NotifyPropertyChangedBase, IInitDesignerSurfaceEndHelper
{
	private static FrontendWindowData _instance;

	private static readonly object syncObj = new object();

	private readonly Func<bool?> _userLogin;

	private readonly INotifyPropertyChanged _codemanager;

	private readonly Action _doClose;

	private readonly Func<List<SolutionFileInfo>> _getRecentSolution;

	private readonly object[] _solutionActions;

	private readonly Action _solutionManager;

	private readonly Func<int> _getlogincode;

	private readonly Action _hideMain;

	private readonly Action _showMain;

	private readonly Func<string, int> _saveAction;

	private readonly Func<string, int> _saveRemoteAction;

	private readonly Func<string, int> _saveLocalAction;

	private readonly Action _saveAsSolution;

	public static Func<string, string> _globalvariableselectdlgFunction;

	public ParamsConfigWithResultViewControl _paramsTabControl;

	private DisplayModeEnum _displaymode = DisplayModeEnum.Running;

	private string _username;

	private bool _isSDKMode = false;

	private string _titleName;

	private bool _isInitDesigner;

	private int curSubDesignDepth = 0;

	private DelegateCommand<object> switchMainWindowCommand = null;

	private DelegateCommand _logoutcommand;

	private DelegateCommand<object> topmostCommand = null;

	private DelegateCommand<IconMenuItem> iconMenuItem_SubmenuOpenedCommand = null;

	private DelegateCommand<object> loginCommand = null;

	private DelegateCommand openSolCommand = null;

	private DelegateCommand openSolRemoteCommand = null;

	private DelegateCommand openSolLocalCommand = null;

	private DelegateCommand saveSolutionCommand = null;

	private DelegateCommand saveSolutionRemoteCommand = null;

	private DelegateCommand saveSolutionLocalCommand = null;

	private DelegateCommand solManagerCommand = null;

	private const string strFrontendDesignPath = "myLibs\\FrontendUI.Design.dll";

	private const string strGACFrontendDesignPath = "FrontendUI.Design.dll";

	public static FrontendWindowData Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (syncObj)
				{
					if (_instance == null)
					{
						_instance = new FrontendWindowData();
					}
				}
			}
			return _instance;
		}
	}

	public DisplayModeEnum DisplayMode
	{
		get
		{
			return _displaymode;
		}
		set
		{
			_displaymode = value;
			OnPropertyChanged("DisplayMode");
		}
	}

	public int UserType => (int)CodeManager.GetInstance().GetLoginUser();

	public string UserName
	{
		get
		{
			switch (UserType)
			{
			case 0:
				_username = string.Empty;
				break;
			case 1:
				_username = AppLocalizationService.GetLang("Lang_Role_Admin");
				break;
			case 2:
				_username = AppLocalizationService.GetLang("Lang_Role_Tech");
				break;
			case 3:
				_username = AppLocalizationService.GetLang("Lang_Role_Operator");
				break;
			}
			return _username;
		}
		set
		{
			if (!(_username == value))
			{
				_username = value;
				OnPropertyChanged("UserName");
			}
		}
	}

	public bool IsRemoteMode => RemoteOperate.IsRemoteMode;

	public bool IsSDKMode
	{
		get
		{
			return _isSDKMode;
		}
		set
		{
			if (_isSDKMode != value)
			{
				_isSDKMode = value;
				OnPropertyChanged("IsSDKMode");
			}
		}
	}

	public string TitleName
	{
		get
		{
			return _titleName;
		}
		set
		{
			if (!(_titleName == value))
			{
				_titleName = value;
				OnPropertyChanged("TitleName");
			}
		}
	}

	public bool IsInitDesigner
	{
		get
		{
			return _isInitDesigner;
		}
		set
		{
			if (_isInitDesigner != value)
			{
				_isInitDesigner = value;
				OnPropertyChanged("IsInitDesigner");
			}
		}
	}

	public int CurSubDesignDepth
	{
		get
		{
			return curSubDesignDepth;
		}
		set
		{
			curSubDesignDepth = value;
		}
	}

	public ICommand SwitchMainWindowCommand => (ICommand)(switchMainWindowCommand ?? (switchMainWindowCommand = new DelegateCommand<object>((Action<object>)SwitchMainWindowExcute)));

	public DelegateCommand LogoutCommand
	{
		get
		{
			return _logoutcommand;
		}
		set
		{
			if (_logoutcommand == null || _logoutcommand != value)
			{
				_logoutcommand = value;
				OnPropertyChanged("LogoutCommand");
			}
		}
	}

	public ICommand TopmostCommand => (ICommand)(topmostCommand ?? (topmostCommand = new DelegateCommand<object>((Action<object>)TopmostExcute)));

	public ICommand IconMenuItem_SubmenuOpenedCommand => (ICommand)(iconMenuItem_SubmenuOpenedCommand ?? (iconMenuItem_SubmenuOpenedCommand = new DelegateCommand<IconMenuItem>((Action<IconMenuItem>)IconMenuItem_SubmenuOpenedExcute)));

	public ICommand LoginCommand => (ICommand)(loginCommand ?? (loginCommand = new DelegateCommand<object>((Action<object>)LoginExcute)));

	public ICommand OpenSolCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = openSolCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)OpenSolExcute);
				DelegateCommand val3 = val2;
				openSolCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public ICommand OpenSolRemoteCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = openSolRemoteCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)OpenSolRemoteExcute);
				DelegateCommand val3 = val2;
				openSolRemoteCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public ICommand OpenSolLocalCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = openSolLocalCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)OpenSolLocalExcute);
				DelegateCommand val3 = val2;
				openSolLocalCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public ICommand SaveSolutionCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = saveSolutionCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)SaveSolutionExcute);
				DelegateCommand val3 = val2;
				saveSolutionCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public ICommand SaveSolutionRemoteCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = saveSolutionRemoteCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)SaveSolutionRemoteExcute);
				DelegateCommand val3 = val2;
				saveSolutionRemoteCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public ICommand SaveSolutionLocalCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = saveSolutionLocalCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)SaveSolutionLocalExcute);
				DelegateCommand val3 = val2;
				saveSolutionLocalCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public ICommand SolManagerCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = solManagerCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)SolManagerExcute);
				DelegateCommand val3 = val2;
				solManagerCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public static event EventHandler CreateNewDesignEvent;

	private static event EventHandler InitDesignerSurfaceEndEvnet;

	public event EventHandler InitDesignerSurfaceEndEvnetChangedCallBack
	{
		add
		{
			InitDesignerSurfaceEndEvnet += value;
		}
		remove
		{
			InitDesignerSurfaceEndEvnet -= value;
		}
	}

	public FrontendWindowData()
	{
		ToolFunctionSourceModle[] toolFunctionSources = ServerManager.Instance.GetToolFunctionSources();
		if (toolFunctionSources.Length != 0)
		{
			ToolFunctionSourceModle toolFunctionSourceModle = toolFunctionSources.Single((ToolFunctionSourceModle p) => p.strPubAddress == "tool");
			if (toolFunctionSourceModle != null)
			{
				foreach (FunctionInfo functionInfo in toolFunctionSourceModle.FunctionInfos)
				{
					switch (functionInfo.FilterName)
					{
					case "usermanagercommand":
						LogoutCommand = functionInfo.Value as DelegateCommand;
						break;
					case "userlogincommand":
						_userLogin = functionInfo.Value as Func<bool?>;
						break;
					case "codemanager":
						_codemanager = functionInfo.Value as INotifyPropertyChanged;
						break;
					case "doclose":
						_doClose = functionInfo.Value as Action;
						break;
					case "getrecentlysolution":
						_getRecentSolution = functionInfo.Value as Func<List<SolutionFileInfo>>;
						break;
					case "solutionActions":
						_solutionActions = functionInfo.Value as object[];
						break;
					case "solutionManager":
						_solutionManager = functionInfo.Value as Action;
						break;
					case "getloginuser":
						_getlogincode = functionInfo.Value as Func<int>;
						break;
					case "hidewindow":
						_hideMain = functionInfo.Value as Action;
						break;
					case "showwindow":
						_showMain = functionInfo.Value as Action;
						break;
					case "savesolution":
						_saveAction = functionInfo.Value as Func<string, int>;
						break;
					case "saveremotesolution":
						_saveRemoteAction = functionInfo.Value as Func<string, int>;
						break;
					case "savelocalsolution":
						_saveLocalAction = functionInfo.Value as Func<string, int>;
						break;
					case "SaveAsSolution":
						_saveAsSolution = functionInfo.Value as Action;
						break;
					}
				}
			}
		}
		_globalvariableselectdlgFunction = GetFuncGlobalVariableSelectDlg();
		Init();
	}

	~FrontendWindowData()
	{
		RemoveHandlerVM();
	}

	private void SwitchMainWindowExcute(object Obj)
	{
		FrontendWindow frontendWindow;
		if (_userLogin == null || (UserType != 0 && _userLogin() != true) || (frontendWindow = Obj as FrontendWindow) == null)
		{
			return;
		}
		int userType = UserType;
		if ((uint)userType > 2u)
		{
			return;
		}
		Ioc.Get<IApplicationOperation>()?.UnRegisterMainWindow(frontendWindow);
		_showMain?.Invoke();
		Task.Run(() =>
		{
			Application.Current.Dispatcher?.BeginInvoke((Action)(() =>
			{
				frontendWindow.BDisposeMainWindow = false;
				frontendWindow.Visibility = Visibility.Hidden;
				frontendWindow.Close();
			}));
		});
	}

	private void TopmostExcute(object ischeckedObj)
	{
	}

	private void IconMenuItem_SubmenuOpenedExcute(IconMenuItem inputIconMenuItem)
	{
		if (inputIconMenuItem == null || _getRecentSolution == null)
		{
			return;
		}
		List<SolutionFileInfo> list = _getRecentSolution().ToList();
		while (inputIconMenuItem.Items.Count > 0)
		{
			if (inputIconMenuItem.Items[0] is IconMenuItem iconMenuItem)
			{
				iconMenuItem.Click -= OpenLastSolMenuItem_OnClick;
			}
			inputIconMenuItem.Items.RemoveAt(0);
		}
		foreach (SolutionFileInfo item in list)
		{
			IconMenuItem iconMenuItem2 = new IconMenuItem
			{
				Tag = item.FilePath,
				ToolTip = item.FilePath,
				Header = ShortString(item.FileName)
			};
			iconMenuItem2.Click += OpenLastSolMenuItem_OnClick;
			inputIconMenuItem.Items.Add(iconMenuItem2);
		}
	}

	private void OpenLastSolMenuItem_OnClick(object sender, RoutedEventArgs e)
	{
		if (_solutionActions != null && _solutionActions.Count() >= 3 && sender is IconMenuItem iconMenuItem && _solutionActions[1] is Func<string, bool> func && !func(iconMenuItem.Tag.ToString()))
		{
		}
	}

	private void LoginExcute(object obj)
	{
		Login(obj);
	}

	private void OpenSolExcute()
	{
		if (_solutionActions != null)
		{
			((Action)_solutionActions[0])();
		}
	}

	private void OpenSolRemoteExcute()
	{
		if (_solutionActions != null && _solutionActions.Count() == 6)
		{
			((Action)_solutionActions[4])();
		}
	}

	private void OpenSolLocalExcute()
	{
		if (_solutionActions != null && _solutionActions.Count() == 6)
		{
			((Action)_solutionActions[5])();
		}
	}

	public void SaveSolutionExcute()
	{
		if (_saveAction != null)
		{
			_saveAction(null);
		}
	}

	public bool IsCanSaveSolution()
	{
		return _saveAction != null;
	}

	private void SaveSolutionRemoteExcute()
	{
		if (_saveRemoteAction != null)
		{
			_saveRemoteAction(null);
		}
	}

	private void SaveSolutionLocalExcute()
	{
		if (_saveLocalAction != null)
		{
			_saveLocalAction(null);
		}
	}

	private void SolManagerExcute()
	{
		if (_solutionManager != null)
		{
			_solutionManager();
		}
	}

	private void Init()
	{
		if (_codemanager != null)
		{
			_codemanager.PropertyChanged += OnLoinUserChanged;
		}
		AddHandlerVM();
	}

	private void AddHandlerVM()
	{
		VmSolution.OnSolutionLoadEndCallBack += VmSolution_OnSolutionLoadEndCallBack;
	}

	private void VmSolution_OnSolutionLoadEndCallBack(object sender, EndEventArgs args)
	{
		DocumentManager.Instance().ResetBuffer();
	}

	private void RemoveHandlerVM()
	{
		VmSolution.OnSolutionLoadEndCallBack -= VmSolution_OnSolutionLoadEndCallBack;
	}

	private string ShortString(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		MeasureText measureText = new MeasureText(input, 16.0, "Microsoft YaHei");
		int num = input.Length;
		while (measureText.Width > 156.0)
		{
			num--;
			string text = input.Substring(0, num) + "...";
			measureText = new MeasureText(text, 16.0, "Microsoft YaHei");
		}
		return (num == input.Length) ? input : (input.Substring(0, num) + "...");
	}

	private void Login(object obj, bool needClose = true)
	{
		SwitchMainWindowExcute(obj);
	}

	private void ChangeUser()
	{
		OnPropertyChanged("UserType");
		OnPropertyChanged("UserName");
		NotifyPropertyChanged("IsOperatorEnabled");
	}

	private void NotifyPropertyChanged(string propertyName)
	{
	}

	public void CreateNewDesign()
	{
		DocumentManager.Instance().Init();
		CreateNewDesignEvent?.Invoke(null, new EventArgs());
	}

	public static EditFrontendWindow GetParentFrontendWindow(FrameworkElement fe)
	{
		if (fe is EditFrontendWindow result)
		{
			return result;
		}
		return CommonMethod.FindVisualParent<EditFrontendWindow>(fe);
	}

	public static Window GetParentWindow(FrameworkElement fe)
	{
		if (fe is Window result)
		{
			return result;
		}
		return CommonMethod.FindVisualParent<Window>(fe);
	}

	public static BaseWindow GetParentBaseWindow(FrameworkElement fe)
	{
		if (fe is BaseWindow result)
		{
			return result;
		}
		return CommonMethod.FindVisualParent<BaseWindow>(fe);
	}

	public static void EditFrontendUI(object parent = null)
	{
		EditFrontendWindow editFrontendWindow = new EditFrontendWindow();
		Instance.DisplayMode = DisplayModeEnum.Editing;
		Instance.IsSDKMode = true;
		if (parent is Window owner)
		{
			editFrontendWindow.Owner = owner;
		}
		else if (parent is IntPtr owner2)
		{
			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(editFrontendWindow)
			{
				Owner = owner2
			};
		}
		editFrontendWindow.ShowInTaskbar = true;
		editFrontendWindow.ShowDialog();
		Instance.IsSDKMode = false;
	}

	private static void LoadFrontendDesignDllAssembly()
	{
		string currentPath = CurrentPathHelper.Instance.GetCurrentPath();
		string text = Path.Combine(currentPath, "FrontendUI.Design.dll");
		if (!File.Exists(text))
		{
			text = Path.Combine(currentPath, "myLibs\\FrontendUI.Design.dll");
		}
		if (File.Exists(text))
		{
			Assembly assemblyByFileName = ActivatorWrap.GetAssemblyByFileName(text);
			if (null == assemblyByFileName)
			{
				ActivatorWrap.LoadAssembly(text);
			}
		}
	}

	public static void UpdateAssemblyInfo()
	{
		LoadFrontendDesignDllAssembly();
		SingletonBase<DesignerDataSourceHelper>.Instance.UpdateSourceInfo();
	}

	public void OpenGlobaleVarReport()
	{
		VmModule globalVarModule = VmModuleExtern.GetGlobalVarModule();
		if (globalVarModule != null)
		{
			(globalVarModule as IVarModule).VarValueReportEnable = true;
			VariableManager variableManager = new VariableManager(globalVarModule);
			variableManager.SetReportState(isEnable: true);
		}
	}

	public void CloseGlobaleVarReport()
	{
		VmModule globalVarModule = VmModuleExtern.GetGlobalVarModule();
		if (globalVarModule != null)
		{
			(globalVarModule as IVarModule).VarValueReportEnable = false;
			VariableManager variableManager = new VariableManager(globalVarModule);
			variableManager.SetReportState(isEnable: false);
		}
	}

	public void OpenLocalVarReport(IVmModule localVarModule)
	{
		if (localVarModule != null)
		{
			(localVarModule as IVarModule).VarValueReportEnable = true;
			VariableManager variableManager = new VariableManager(localVarModule);
			variableManager.SetReportState(isEnable: true);
		}
	}

	public void CloseLocalVarReport(IVmModule localVarModule)
	{
		if (localVarModule != null)
		{
			(localVarModule as IVarModule).VarValueReportEnable = false;
			VariableManager variableManager = new VariableManager(localVarModule);
			variableManager.SetReportState(isEnable: false);
		}
	}

	public bool IsCanSaveAsSolution()
	{
		return _saveAsSolution != null;
	}

	public void SaveAsSolution()
	{
		_saveAsSolution?.Invoke();
	}

	private void ShowMainWindow()
	{
		_showMain?.Invoke();
	}

	public static Func<string, string> GetFuncGlobalVariableSelectDlg()
	{
		return (string type) =>
		{
			GlobalVariableSelectDlg globalVariableSelectDlg = new GlobalVariableSelectDlg(isAllowRepeatedSelection: true);
			globalVariableSelectDlg.Type = new string[1] { type };
			globalVariableSelectDlg.ShowDialog();
			return globalVariableSelectDlg.IsConfirm ? globalVariableSelectDlg.SelectedGlobalVar : "";
		};
	}

	public static string GetLocalVariableSelectDlg(object localVarModule, string valueType)
	{
		if (localVarModule == null || string.IsNullOrWhiteSpace(valueType))
		{
			return string.Empty;
		}
		string empty = string.Empty;
		GlobalVariableSelectDlg globalVariableSelectDlg = new GlobalVariableSelectDlg(isAllowRepeatedSelection: true);
		globalVariableSelectDlg.IsEnumGlobalVarModule = false;
		globalVariableSelectDlg.Type = new string[1] { valueType };
		globalVariableSelectDlg.DecorateSource = localVarModule;
		GlobalVariableSelectDlg globalVariableSelectDlg2 = globalVariableSelectDlg;
		globalVariableSelectDlg2.ShowDialog();
		if (globalVariableSelectDlg2.IsConfirm)
		{
			return globalVariableSelectDlg2.SelectedGlobalVar;
		}
		return empty;
	}

	public static IVmModule GetLoaclVariableFromFullNamePath(string localVariableModuleFullName)
	{
		if (string.IsNullOrWhiteSpace(localVariableModuleFullName))
		{
			return null;
		}
		try
		{
			return VmSolution.Instance[localVariableModuleFullName] as IVmModule;
		}
		catch (Exception ex)
		{
			LogHelper.frontedLog.Error(ex.Message + $"未找到模块{localVariableModuleFullName}");
			return null;
		}
	}

	public Window InitRunWindowFromDesignData()
	{
		DisplayMode = DisplayModeEnum.Running;
		Window result = null;
		try
		{
			result = CommonMethod.ParseXaml(DocumentManager.InstanceP.SolutionDesignerBuffer) as Window;
			Instance.OnInitDesignerSurfaceEndChanged();
		}
		catch (Exception message)
		{
			LogHelper.frontedLog.Error(message);
		}
		return result;
	}

	private void OnLoinUserChanged(object sender, PropertyChangedEventArgs e)
	{
		if (!(e.PropertyName != "LoginUser"))
		{
			ChangeUser();
		}
	}

	public void OnInitDesignerSurfaceEndChanged()
	{
		InitDesignerSurfaceEndEvnet?.Invoke(this, new EventArgs());
	}
}
