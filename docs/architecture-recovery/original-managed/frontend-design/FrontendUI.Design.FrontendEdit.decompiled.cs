using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Apps.Localization;
using Apps.Log;
using Apps.UIHelper.Extern;
using DesignerModifyDataSource;
using Frontend.Controls;
using Frontend.Interface;
using FrontendUI.Design.Coding;
using FrontendUI.Design.Common;
using FrontendUI.Design.ConfigWin;
using FrontendUI.Design.Controls;
using FrontendUI.Design.DesignTemplate;
using FrontendUI.Design.Editors;
using FrontendUI.Design.Helper;
using FrontendUI.Design.TestDesigner;
using FrontendUI.Design.Tool;
using FrontendUI.Design.UnReDo;
using FrontendUI.Design.View;
using HandyControl.Controls;
using HandyControl.EventArgs;
using Hik.Util;
using HikPcUI.Commom;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.Interfaces;
using Mvvm.Commands;
using UIVersionControlManager;
using VM.Controls;
using VM.Core;
using VM.Utility;
using VM.Utility.SingleInstance;

namespace FrontendUI.Design;

public class FrontendEdit : System.Windows.Controls.UserControl, INotifyPropertyChanged, IComponentConnector
{
	private DesignSurface _mainDesignSurface;

	private DeleteRowOrColHelper _deleteRowOrColHelper;

	private bool? _IsProLevel;

	private bool? _IsShowLayoutSetBtn;

	private bool _IsSubDesign = false;

	private string rootStr;

	private string subRootStr;

	public static readonly DependencyProperty ExtenContentProperty;

	private Thickness _beforeMoveMargin = default;

	private string CFGPath = ConfigurationHelper.GetConfig("ApplicationName") + ".cfg";

	private const string strFrontendDesignPath = "myLibs\\FrontendUI.Design.dll";

	private const string strGACFrontendDesignPath = "FrontendUI.Design.dll";

	private const string strGACFrontendControlsPath = "Frontend.Controls.dll";

	private static Size _viewSize;

	private DelegateCommand createCodeCommand = null;

	internal FrontendEdit us;

	internal Grid Grid;

	internal Border TitleBar;

	internal ScrollViewer m_HorScroll;

	internal System.Windows.Controls.Menu Menu;

	internal Slider Slider;

	internal System.Windows.Controls.Button PreViewBtn;

	internal System.Windows.Controls.Button CreateCodeBtn;

	internal Grid toolGrid;

	internal System.Windows.Controls.CheckBox toolCheckBox;

	internal ToolBox toolBox;

	internal Grid Designer;

	public DesignSurface ds;

	internal ProgressMask progressMask;

	internal System.Windows.Controls.TabControl settingTab;

	internal TabItem PropertyTabItem;

	internal HandyControl.Controls.PropertyGrid propertyGrid2;

	internal HandyControl.Controls.PropertyGrid propertyGrid;

	private bool _contentLoaded;

	public DesignSurface MainSurface => _mainDesignSurface;

	public bool IsHasSaveDesign { get; set; } = false;

	public object ValveOwner { get; set; }

	public string ValveNames { get; set; }

	public bool? ProLevel
	{
		get
		{
			if (!_IsProLevel.HasValue)
			{
				_IsProLevel = !UIVersionControlManagerCS.IsDongleLowerThanConfig(UIVersionControlManagerCS.ProLevel);
			}
			return _IsProLevel.Value;
		}
	}

	public bool IsShowLayoutSetBtn
	{
		get
		{
			_IsShowLayoutSetBtn = ProLevel.Value && !IsSubDesign;
			return _IsShowLayoutSetBtn.Value;
		}
	}

	public bool IsSubDesign
	{
		get
		{
			return _IsSubDesign;
		}
		set
		{
			if (_IsSubDesign != value)
			{
				_IsSubDesign = value;
				OnPropertyChanged("IsSubDesign");
				OnPropertyChanged("IsShowinSubDesign");
				OnPropertyChanged("IsShowLayoutSetBtn");
				OnPropertyChanged("IsShowDesignTemplate");
			}
		}
	}

	public bool IsShowDesignTemplate => IsShowLayoutSetBtn;

	public bool IsShowinSubDesign => !IsSubDesign;

	public string RootStr => rootStr;

	public string SubRootStr
	{
		get
		{
			return subRootStr;
		}
		set
		{
			subRootStr = value;
		}
	}

	public FrameworkElement ExtenContent
	{
		get
		{
			return (FrameworkElement)GetValue(ExtenContentProperty);
		}
		set
		{
			SetValue(ExtenContentProperty, value);
		}
	}

	public FrontendEditData EditData { get; } = new FrontendEditData();

	public ICommand CreateCodeCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0023: Expected Obj, but got Unknown
			DelegateCommand val = createCodeCommand;
			if (val == null)
			{
				DelegateCommand val2 = new DelegateCommand((Action)CreateCodeExcute);
				DelegateCommand val3 = val2;
				createCodeCommand = val2;
				val = val3;
			}
			return (ICommand)val;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public event Func<bool> OnSaveTextDesign;

	public virtual void OnPropertyChanged(string prop)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}

	static FrontendEdit()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected Obj, but got Unknown
		ExtenContentProperty = DependencyProperty.Register("ExtenContent", typeof(FrameworkElement), typeof(FrontendEdit), new PropertyMetadata(null));
		ModelTools.AfterDeleteComponentsEvent += ModelTools_AfterDeleteComponentsEvent;
		SetIViewSize(new Size
		{
			Width = SystemParameters.WorkArea.Width,
			Height = SystemParameters.WorkArea.Height
		});
	}

	private static void deleteControl(FrameworkElement fe)
	{
		Grid grid;
		if ((grid = fe as Grid) != null || fe is System.Windows.Controls.GroupBox || fe is System.Windows.Controls.TabControl)
		{
			List<FrameworkElement> lstT = new List<FrameworkElement>();
			CommonMethod.FindAllChild(fe, ref lstT);
			{
				foreach (FrameworkElement item in lstT)
				{
					deleteControl(item);
				}
				return;
			}
		}
		if (fe is ExpendChartControl expendChartControl)
		{
			EVBLDataManager.Instance.RemoveDataFromDic(expendChartControl.ViewLayoutData as IViewLayoutData);
			EVBLDuringUIDesignManager.Instance.RemoveDataFromDic(expendChartControl.ViewLayoutData as IViewLayoutData);
			DelEVBLDuringUIDesignManager.Instance.AddDataToDic(expendChartControl.ViewLayoutData as IViewLayoutData);
		}
	}

	private static void ModelTools_AfterDeleteComponentsEvent(ICollection<DesignItem> deleteItems, DesignItem parentDesignItem)
	{
		if (deleteItems == null)
		{
			return;
		}
		foreach (DesignItem deleteItem in deleteItems)
		{
			deleteControl(deleteItem.Component as FrameworkElement);
			if (deleteItem.Component is TabColumnItem item && parentDesignItem.Component is Tab tab)
			{
				tab.SetTabItemCount();
				tab.TabColumns.Remove(item);
			}
		}
	}

	public FrontendEdit()
	{
		InitializeComponent();
		LoadAssembly();
		InitFrontendWindow();
	}

	public FrontendEdit(string rootStr)
	{
		InitializeComponent();
		InitFrontendWindow(rootStr);
	}

	public FrontendEdit(object control, bool bSubDesign = false)
	{
		InitializeComponent();
		InitFrontendEdit(control, bSubDesign);
	}

	private void InitFrontendWindow()
	{
		HikPcUI.Commom.CurrentUIHelper.Instance.Dispatcher.Invoke(() =>
		{
		});
		EVBLDuringUIDesignManager.Instance.ClearDicData();
		DelEVBLDuringUIDesignManager.Instance.ClearDicData();
		_mainDesignSurface = ds;
		OnPropertyChanged("MainSurface");
		ExtensionMethods.AddCommandHandler((UIElement)(object)_mainDesignSurface, CustomCommands.DeleteColumnCommand, (Action<object>)((object s) =>
		{
			_deleteRowOrColHelper.DeleteRowOrCol(bRow: false, s as FrameworkElement);
		}), (Func<object, bool>)((object s) => _deleteRowOrColHelper.CanDeleteRowOrCol(bRow: false, s as FrameworkElement)));
		ExtensionMethods.AddCommandHandler((UIElement)(object)_mainDesignSurface, CustomCommands.DeleteRowCommand, (Action<object>)((object s) =>
		{
			_deleteRowOrColHelper.DeleteRowOrCol(bRow: true, s as FrameworkElement);
		}), (Func<object, bool>)((object s) => _deleteRowOrColHelper.CanDeleteRowOrCol(bRow: true, s as FrameworkElement)));
		ExtensionMethods.AddCommandHandler((UIElement)(object)_mainDesignSurface, Commands.ChooseParentCommand, (Action)(() =>
		{
			DesignItem primarySelection = _mainDesignSurface.DesignContext.Services.Selection.PrimarySelection;
			DesignItem iContainerDesignItem = ModelTools.GetIContainerDesignItem(primarySelection);
			if (iContainerDesignItem != null)
			{
				primarySelection.Services.Selection.SetSelectedComponents((ICollection<DesignItem>)new DesignItem[1] { iContainerDesignItem });
			}
		}), (Func<bool>)(() =>
		{
			DesignContext designContext = _mainDesignSurface.DesignContext;
			return designContext != null && designContext.Services.Selection.SelectedItems.Count == 1 && !(_mainDesignSurface.DesignContext.Services.Selection.PrimarySelection.View is Grid);
		}));
		_mainDesignSurface.DesignerLoaded += DesignerLoaded;
		_mainDesignSurface.DesignerUnLoading += DesignerUnLoaded;
		_mainDesignSurface.ComponentsDeleting += OnComponentsDeleting;
		InitPropertyGrid();
	}

	public void InitDesignerSurface(DesignItem designItem)
	{
		progressMask.Visibility = Visibility.Visible;
		Task.Run(() =>
		{
			Thread.Sleep(100);
			Dispatcher.Invoke(() =>
			{
				try
				{
					_mainDesignSurface.InitializeDesigner(designItem.Context, designItem.Properties["Child"].ValueOnInstance as UIElement);
				}
				finally
				{
					progressMask.Visibility = Visibility.Collapsed;
					FrontendWindowData.Instance.IsInitDesigner = false;
				}
			});
		});
	}

	private void LoadTempDesignerBufferFromVM()
	{
		if (SolutionFileHelper.GetInstance().IsConvertFromVMDoing)
		{
			HikMessageBox.Show(FrontendUI.Design.Helper.ResourceBinding.FindResoure("正在加载数据"), VM.Controls.MessageIcons.Warning, VM.Controls.MessageButtons.NoButton, 1000u);
		}
		lock (SolutionFileHelper.GetInstance()._Lock)
		{
		}
		if (SolutionFileHelper.GetInstance().IsModifyFromVM && HikMessageBox.Show("Lang_LogTips", "检测到修改:是否同步主界面修改?", VM.Controls.MessageButtons.YesOrNo, VM.Controls.MessageIcons.Question, isTranslate: true) == VM.Controls.DialogResult.Yes)
		{
			SolutionFileHelper.GetInstance().SaveDesingerBuffer();
			DocumentManager.Instance().isSolutionDesignerBuffer_Loaded = false;
			DocumentManager.Instance().TempDesignerBuffer = DocumentManager.Instance().SolutionDesignerBuffer;
			DocumentManager.Instance().isSolutionDesignerBuffer_Loaded = false;
		}
	}

	public void InitDesignerSurface(string rootStr, bool isSubDesign = false, bool whetherToUseTheRootStr = false)
	{
		DestoryUIDesign();
		if (!isSubDesign)
		{
			if (string.IsNullOrEmpty(rootStr))
			{
				rootStr = DefaultDesignerTool.GetDefaultDesign("DefaultDesign");
			}
			this.rootStr = rootStr;
		}
		EVBLDuringUIDesignManager.Instance.ClearDicData();
		DelEVBLDuringUIDesignManager.Instance.ClearDicData();
		progressMask.Visibility = Visibility.Visible;
		FrontendWindowData.Instance.IsInitDesigner = true;
		Task.Run(() =>
		{
			Thread.Sleep(100);
			Dispatcher.Invoke(() =>
			{
				try
				{
					if (InitDesignerSurface(isSubDesign, whetherToUseTheRootStr))
					{
						ControlSelection_SelectionChanged(null, null);
						_mainDesignSurface.DesignContext.Services.Selection.SelectionChanged += ControlSelection_SelectionChanged;
						_mainDesignSurface.DesignContext.Services.GridPositionChanged.DesignItemsPositionChanged += GridPositionChanged_DesignItemsPositionChanged;
						_mainDesignSurface.DesignContext.Services.GridPositionChanged.DesignItemsMoveBeigin += GridPositionChanged_DesignItemsMoveBeigin;
						_deleteRowOrColHelper = new DeleteRowOrColHelper(_mainDesignSurface.DesignContext);
					}
				}
				finally
				{
					progressMask.Visibility = Visibility.Collapsed;
					FrontendWindowData.Instance.IsInitDesigner = false;
					FrontendWindowData.Instance.OnInitDesignerSurfaceEndChanged();
				}
			});
		});
	}

	public void InitFrontendWindow(string rootStr = "")
	{
		InitFrontendWindow();
		InitDesignerSurface(rootStr);
	}

	public void InitFrontendEdit(object control, bool bSubDesign = true)
	{
		if (bSubDesign && control is SubWindow subWindow)
		{
			IsSubDesign = bSubDesign;
			InitFrontendWindow();
			if (string.IsNullOrWhiteSpace(subWindow.Document))
			{
				subRootStr = DefaultDesignerTool.GetDefaultDesign("DefaultSubDesign");
			}
			else
			{
				subRootStr = subWindow.Document.Base64StringToString();
			}
			InitDesignerSurface(subRootStr, isSubDesign: true);
			HideSplitterMergePannel();
		}
	}

	public void HideSplitterMergePannel()
	{
		settingTab.SelectedIndex = 0;
	}

	private void GridPositionChanged_DesignItemsMoveBeigin(object sender, GridPosEventArgs e)
	{
		DesignItem item = e.Item;
		if (item != null && item.View is FrameworkElement frameworkElement)
		{
			_beforeMoveMargin = frameworkElement.Margin;
		}
	}

	private void GridPositionChanged_DesignItemsPositionChanged(object sender, GridPosChangedEventArgs e)
	{
		DesignItem item = e.Item;
		GridPos oldPos = e.OldPos;
		GridPos newPos = e.NewPos;
		if (item == null)
		{
			return;
		}
		UIElement view = item.View;
		IAutoStretchBehavior val;
		if ((val = (IAutoStretchBehavior)(object)((view is IAutoStretchBehavior) ? view : null)) != null && val.AutoStrechA)
		{
			if (item.Parent != null)
			{
				SubRoot.Strech(item.Parent.View as FrameworkElement, item);
			}
			if (item.View is ISetPropertyBehaviorObj setPropertyBehaviorObj)
			{
				setPropertyBehaviorObj.BhObjA.oldMagin = new Thickness(0.0);
			}
		}
		IsHasSaveDesign = false;
	}

	private bool isSelectSameItems()
	{
		ISelectionService selection = _mainDesignSurface.DesignContext.Services.Selection;
		if (selection == null)
		{
			return false;
		}
		if (selection.PrimarySelection == null)
		{
			return false;
		}
		if (selection.SelectionCount == 1)
		{
			return true;
		}
		Type componentType = selection.PrimarySelection.ComponentType;
		foreach (DesignItem selectedItem in selection.SelectedItems)
		{
			if (selectedItem.ComponentType != componentType)
			{
				return false;
			}
		}
		return true;
	}

	private void OnComponentsDeleting(object sender, ComponentsDeletingArgs e)
	{
		DesignSurface val = (DesignSurface)((sender is DesignSurface) ? sender : null);
		if (val != null)
		{
			IsHasSaveDesign = false;
		}
	}

	private void DesignerLoaded(object sender, EventArgs e)
	{
		toolBox.InitToolBox(_mainDesignSurface.DesignContext.Services.Tool);
	}

	private void DesignerUnLoaded(object sender, EventArgs e)
	{
		toolBox.RemoveToolService(_mainDesignSurface.DesignContext.Services.Tool);
	}

	internal HandyControl.Controls.PropertyGrid GetPropertyGrid()
	{
		return propertyGrid;
	}

	public void DesignerUIVisibleChanged(bool bIsVisible)
	{
		List<DesignItem> allDesignItems = ModelTools.GetAllDesignItems(_mainDesignSurface.DesignContext);
		if (allDesignItems == null)
		{
			return;
		}
		foreach (DesignItem item in allDesignItems)
		{
			if (item.View is IDesignerCustomControl designerCustomControl)
			{
				designerCustomControl.IsVisibleChanged(bIsVisible);
			}
		}
	}

	private void DisposeUI()
	{
		List<DesignItem> allDesignItems = ModelTools.GetAllDesignItems(_mainDesignSurface.DesignContext);
		if (allDesignItems == null)
		{
			return;
		}
		foreach (DesignItem item in allDesignItems)
		{
			if (item.View is IDisposable disposable && item.View.GetType() == typeof(SubRoot))
			{
				disposable.Dispose();
			}
			else if (item.View is IDesignerCustomControl designerCustomControl)
			{
				designerCustomControl.Dispose();
			}
		}
	}

	private void CleanControlBindingInfo()
	{
		List<DesignItem> allDesignItems = ModelTools.GetAllDesignItems(_mainDesignSurface.DesignContext);
		if (allDesignItems == null)
		{
			return;
		}
		foreach (DesignItem item in allDesignItems)
		{
			if (item.View is ICleanBindingInfo cleanBindingInfo)
			{
				if (!(item.View is SubRoot))
				{
					cleanBindingInfo.CleanBindingInfo();
				}
			}
			else if (item.View is DockControl { BottomBar: ICleanBindingInfo bottomBar })
			{
				bottomBar.CleanBindingInfo();
			}
		}
	}

	public string GetNoBindingInfoRootStr()
	{
		string empty = string.Empty;
		string text = GetRootStr();
		CleanControlBindingInfo();
		UpdateRootStr();
		empty = GetRootStr();
		rootStr = text;
		InitDesignerSurface(rootStr, isSubDesign: false, whetherToUseTheRootStr: true);
		return empty;
	}

	public string GetRootStr()
	{
		if (string.IsNullOrEmpty(RootStr))
		{
			return "";
		}
		DesignSurface mainDesignSurface = _mainDesignSurface;
		return (mainDesignSurface != null) ? mainDesignSurface.SaveDesignerToXamlCode() : null;
	}

	public void UpdateRootStr()
	{
		DesignSurface mainDesignSurface = _mainDesignSurface;
		rootStr = ((mainDesignSurface != null) ? mainDesignSurface.SaveDesignerToXamlCode() : null);
	}

	public void UpdateSubRootStr()
	{
		DesignSurface mainDesignSurface = _mainDesignSurface;
		subRootStr = ((mainDesignSurface != null) ? mainDesignSurface.SaveDesignerToXamlCode() : null);
	}

	public List<T> GetControls<T>() where T : class
	{
		IEnumerable<IGrouping<Type, object>> enumerable = ModelTools.GetAllDesignItems(GetDesignContext())?.Select((DesignItem x) => x.Component).GroupBy((object x) => x.GetType());
		if (enumerable == null || enumerable.Count() == 0)
		{
			return null;
		}
		return enumerable.FirstOrDefault((IGrouping<Type, object> t) => t.Key == typeof(T))?.Select((object t) => t as T)?.ToList();
	}

	public void DestoryUIDesign()
	{
		DisposeUI();
		ds.UnloadDesigner();
		EVBLDuringUIDesignManager.Instance.ClearDicData();
		DelEVBLDuringUIDesignManager.Instance.ClearDicData();
	}

	public bool Close(bool isShowCancelBtn = true)
	{
		VM.Controls.DialogResult dialogResult = DoSave(isShowCancelBtn);
		if (dialogResult == VM.Controls.DialogResult.Cancel)
		{
			return false;
		}
		if (IsInitialized)
		{
			toolBox.Dispose();
			EditData.SelectedObject = null;
			if (_mainDesignSurface.DesignContext != null)
			{
				_mainDesignSurface.DesignContext.Services.Selection.SelectionChanged -= ControlSelection_SelectionChanged;
				_mainDesignSurface.DesignContext.Services.GridPositionChanged.DesignItemsPositionChanged -= GridPositionChanged_DesignItemsPositionChanged;
				_mainDesignSurface.DesignContext.Services.GridPositionChanged.DesignItemsMoveBeigin -= GridPositionChanged_DesignItemsMoveBeigin;
			}
			_mainDesignSurface.ComponentsDeleting -= OnComponentsDeleting;
			_mainDesignSurface.DesignerLoaded -= DesignerLoaded;
			_mainDesignSurface.DesignerUnLoading -= DesignerUnLoaded;
			propertyGrid.PropertyValueChanged -= PropertyValueChanged;
			VM.Utility.ReleaseHelper.RemoveEvents(propertyGrid);
			propertyGrid.Dispose();
			DestoryUIDesign();
			EVBLDuringUIDesignManager.Instance.ClearDicData();
			DelEVBLDuringUIDesignManager.Instance.ClearDicData();
		}
		return true;
	}

	private VM.Controls.DialogResult DoSave(bool isShowCancelBtn = true)
	{
		Save(save2Solution: false);
		if (IsSubDesign)
		{
			return VM.Controls.DialogResult.Yes;
		}
		if (!DocumentManager.Instance().NeedSave())
		{
			return VM.Controls.DialogResult.Yes;
		}
		bool flag = true;
		VM.Controls.DialogResult dialogResult = HikMessageBox.Show("Lang_Notice", (!flag) ? "Lang_IfDesignSave" : "当前设计与新方案不一致，是否将当前设计文档替换到新方案中？", (!isShowCancelBtn) ? VM.Controls.MessageButtons.YesOrNo : VM.Controls.MessageButtons.YesOrNoOrCancel, VM.Controls.MessageIcons.Warning, isTranslate: true);
		flag = false;
		switch (dialogResult)
		{
		case VM.Controls.DialogResult.Yes:
			Save(save2Solution: true, null, isExcuteAction: false);
			SolutionFileHelper.GetInstance().IsSolutionDesignerModified = true;
			return VM.Controls.DialogResult.Yes;
		case VM.Controls.DialogResult.No:
			ResetBuffer();
			return VM.Controls.DialogResult.No;
		default:
			return VM.Controls.DialogResult.Cancel;
		}
	}

	public void Save(bool save2Solution, string path = null, bool isExcuteAction = true, bool save2Design = false)
	{
		try
		{
			if (_mainDesignSurface == null || _mainDesignSurface.DesignContext == null)
			{
				return;
			}
			string s = _mainDesignSurface.SaveDesignerToXamlCode();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			MemoryStream tempDesignerBuffer = new MemoryStream(bytes, 0, bytes.Length, writable: true, publiclyVisible: true);
			DocumentManager.Instance().TempDesignerBuffer = tempDesignerBuffer;
			if (save2Design)
			{
				DocumentManager.Instance().DesignerBuffer = DocumentManager.Instance().TempDesignerBuffer;
			}
			if (save2Solution)
			{
				if (DocumentManager.Instance().TempDesignerBuffer != null)
				{
					byte[] buffer = DocumentManager.Instance().TempDesignerBuffer.GetBuffer();
					long length = DocumentManager.Instance().TempDesignerBuffer.Length;
					DocumentManager.Instance().SolutionDesignerBuffer = new MemoryStream(buffer, 0, (int)length, writable: true, publiclyVisible: true);
				}
				DocumentManager.Instance().TempDesignerBuffer = null;
			}
		}
		catch (Exception ex)
		{
			LogHelper.frontedLog.Error(ex);
			HikMessageBox.Show(AppLocalizationService.GetLang("Lang_DsgnErr"), string.Format("{0}! {1}", AppLocalizationService.GetLang("Lang_FileSaveFail"), ex.Message), VM.Controls.MessageIcons.Error);
		}
	}

	private VM.Controls.DialogResult DoSave2Design()
	{
		Save(save2Solution: false);
		if (!DocumentManager.Instance().NeedSaveForDesigner())
		{
			return VM.Controls.DialogResult.Yes;
		}
		switch (HikMessageBox.Show("Lang_Notice", "Lang_IfDesignSave2DesignBuffer", VM.Controls.MessageButtons.YesOrNoOrCancel, VM.Controls.MessageIcons.Warning, isTranslate: true))
		{
		case VM.Controls.DialogResult.Yes:
			Save(save2Solution: false, null, isExcuteAction: true, save2Design: true);
			return VM.Controls.DialogResult.Yes;
		case VM.Controls.DialogResult.No:
			return VM.Controls.DialogResult.No;
		default:
			return VM.Controls.DialogResult.Cancel;
		}
	}

	private void ResetBuffer()
	{
		DocumentManager.Instance().TempDesignerBuffer = null;
		DocumentManager.Instance().isSolutionDesignerBuffer_Loaded = false;
	}

	private string getDesignerDefaultFolder()
	{
		string startupPath = System.Windows.Forms.Application.StartupPath;
		string text = startupPath + "\\ProjectParam\\Designer";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public void ImportDesign()
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = AppLocalizationService.GetLang("打开设计");
		openFileDialog.Filter = string.Format("{0}{1}|*{2}", AppLocalizationService.GetLang("设计文件"), DesignerSaveHelper.DesignerFileExtension, DesignerSaveHelper.DesignerFileExtension);
		string designerDefaultFolder = getDesignerDefaultFolder();
		if (designerDefaultFolder != null)
		{
			openFileDialog.InitialDirectory = designerDefaultFolder;
		}
		if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			if (!File.Exists(fileName))
			{
				HikMessageBox.Show("Lang_OpenFileErr", "Lang_FileNotEixt", VM.Controls.MessageIcons.Error);
				return;
			}
			string text = DesignerSaveHelper.ReadFromFile(fileName);
			InitDesignerSurface(text);
		}
	}

	private string GetInitPath(string strkey)
	{
		string text = null;
		try
		{
			if (!File.Exists(CFGPath))
			{
				return null;
			}
			XDocument xDocument = XDocument.Load(CFGPath);
			if (xDocument == null || xDocument.Root == null)
			{
				return null;
			}
			XElement xElement = xDocument.Root.Elements().SingleOrDefault((XElement p) => p.Name == "OpenDialogPath");
			if (xElement == null)
			{
				return null;
			}
			xElement = xElement.Elements().SingleOrDefault((XElement p) => p.Name == strkey);
			if (xElement == null)
			{
				return null;
			}
			text = xElement.Value;
			if (!Directory.Exists(text))
			{
				return null;
			}
			return text;
		}
		catch (Exception message)
		{
			LogHelper.frontedLog.Error(message);
			return null;
		}
	}

	private void SaveInitPath(string strkey, string path)
	{
		if (string.IsNullOrEmpty(strkey) || string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			return;
		}
		try
		{
			string directoryName = Path.GetDirectoryName(path);
			if (directoryName == null || !File.Exists(CFGPath))
			{
				return;
			}
			XDocument xDocument = XDocument.Load(CFGPath);
			if (xDocument == null || xDocument.Root == null)
			{
				return;
			}
			XElement xElement = xDocument.Root.Elements().SingleOrDefault((XElement p) => p.Name == "OpenDialogPath");
			if (xElement != null)
			{
				xElement = xElement.Elements().SingleOrDefault((XElement p) => p.Name == strkey);
				if (xElement != null)
				{
					xElement.Value = directoryName;
					ConfigurationHelper.SaveConfig(xDocument);
				}
			}
		}
		catch (Exception message)
		{
			LogHelper.frontedLog.Error(message);
		}
	}

	private void LoadAssembly()
	{
		string currentPath = CurrentPathHelper.Instance.GetCurrentPath();
		string text = Path.Combine(currentPath, "FrontendUI.Design.dll");
		if (!File.Exists(text))
		{
			text = Path.Combine(currentPath, "myLibs\\FrontendUI.Design.dll");
		}
		if (File.Exists(text))
		{
			ActivatorWrap.LoadAssembly(text);
		}
		text = Path.Combine(currentPath, "Frontend.Controls.dll");
		if (!File.Exists(text))
		{
			text = Path.Combine(currentPath, "myLibs\\FrontendUI.Design.dll");
		}
		if (File.Exists(text))
		{
			ActivatorWrap.LoadAssembly(text);
		}
	}

	public ToolBox GeToolBox()
	{
		return toolBox;
	}

	private bool InitDesignerSurface(bool isSubDesign = false, bool whetherToUseTheRootStr = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected Obj, but got Unknown
		try
		{
			XamlLoadSettings val = new XamlLoadSettings();
			val.DesignerAssemblies.Add(GetType().Assembly);
			if (whetherToUseTheRootStr)
			{
				if (!string.IsNullOrEmpty(rootStr))
				{
					using XmlReader xmlReader = XmlReader.Create(new StringReader(rootStr));
					_mainDesignSurface.LoadDesigner(xmlReader, val);
					OnPropertyChanged("MainSurface");
				}
			}
			else if (isSubDesign)
			{
				if (!string.IsNullOrEmpty(subRootStr))
				{
					using XmlReader xmlReader2 = XmlReader.Create(new StringReader(subRootStr));
					_mainDesignSurface.LoadDesigner(xmlReader2, val);
					OnPropertyChanged("MainSurface");
				}
			}
			else
			{
				LoadTempDesignerBufferFromVM();
				MemoryStream memoryStream = DocumentManager.Instance().TempDesignerBuffer ?? DocumentManager.Instance().SolutionDesignerBuffer;
				if (memoryStream != null)
				{
					using XmlReader xmlReader3 = XmlReader.Create(memoryStream);
					_mainDesignSurface.LoadDesigner(xmlReader3, val);
					if (DocumentManager.Instance().IsLoadDefaultDesignerBuffer)
					{
						InitAdjuctRes();
					}
					OnPropertyChanged("MainSurface");
				}
			}
			List<string> errorsMessage = _mainDesignSurface.GetErrorsMessage(false);
			if (errorsMessage != null && errorsMessage.Count > 0)
			{
				LogHelper.frontedLog.Error(errorsMessage[0]);
			}
			if (_mainDesignSurface.LoadDesignerFailed(false) && HikMessageBox.Show("界面设计不兼容", "显示异常:界面设计版本不兼容!是否将界面初始化?", VM.Controls.MessageButtons.YesOrNo, VM.Controls.MessageIcons.Error, isTranslate: true) == VM.Controls.DialogResult.Yes)
			{
				string defaultDesign = DefaultDesignerTool.GetDefaultDesign("DefaultDesign");
				if (!string.IsNullOrEmpty(defaultDesign))
				{
					using XmlReader xmlReader4 = XmlReader.Create(new StringReader(defaultDesign));
					_mainDesignSurface.LoadDesigner(xmlReader4, val);
					InitAdjuctRes();
					OnPropertyChanged("MainSurface");
				}
			}
			if (_mainDesignSurface.DesignContext != null)
			{
				_mainDesignSurface.AddService(typeof(HandyControl.Controls.PropertyGrid), (object)propertyGrid);
			}
		}
		catch (Exception ex)
		{
			LogHelper.frontedLog.Error(ex);
			throw ex;
		}
		return true;
	}

	public DesignContext GetDesignContext()
	{
		DesignSurface mainDesignSurface = _mainDesignSurface;
		return (mainDesignSurface != null) ? mainDesignSurface.DesignContext : null;
	}

	private void InitPropertyGrid()
	{
		propertyGrid.PropertyValueChanged -= PropertyValueChanged;
		propertyGrid.PropertyValueChanged += PropertyValueChanged;
	}

	public void TabItemVisibilityChanged()
	{
		if (PropertyTabItem.Visibility == Visibility.Collapsed)
		{
			settingTab.SelectedIndex = 1;
		}
		else
		{
			settingTab.SelectedIndex = 0;
		}
	}

	public void ChangedTabItemVisibility()
	{
		if (EditData.SelectedObject is ITemplateSelectable)
		{
			PropertyTabItem.Visibility = Visibility.Collapsed;
		}
	}

	private bool AttributeContainsType(AttributeCollection ac, Type attriType)
	{
		if (ac == null)
		{
			return false;
		}
		foreach (object item in ac)
		{
			if (item.GetType() == attriType)
			{
				return true;
			}
		}
		return false;
	}

	private void changeDesignItemProptery(DesignItem di, HandyControl.EventArgs.PropertyValueChangedEventArgs e)
	{
		DesignItemProperty property = di.Properties.GetProperty(e.ChangedProptertyA.Name);
		if (property == null)
		{
			return;
		}
		DesignSurface val = ds;
		ISelectionService val2 = ((val != null) ? val.DesignContext.Services.Selection : null);
		if (val2 == null)
		{
			return;
		}
		DesignItem primarySelection = val2.PrimarySelection;
		object newValueA = e.NewValueA;
		if (di != primarySelection && di.Component is IPropertyCopy propertyCopy)
		{
			propertyCopy.SetValue(e.ChangedProptertyA.Name, newValueA);
		}
		if (AttributeContainsType(e.ChangedProptertyA.Attributes, typeof(PropertyValueChangedIgnoreAttribute)))
		{
			return;
		}
		if (AttributeContainsType(e.ChangedProptertyA.Attributes, typeof(XMALIgnoreAttribute)))
		{
			DesignContext designContext = GetDesignContext();
			object obj;
			if (designContext == null)
			{
				obj = null;
			}
			else
			{
				ServiceContainer services = designContext.Services;
				obj = ((services != null) ? services.GetService<UndoService>() : null);
			}
			UndoService val3 = (UndoService)obj;
			if (val3 != null)
			{
				val3.Execute((ITransactionItem)(object)new PropertyChangeAction(di, e.ChangedProptertyA.Name, e.OldValueA, e.NewValueA));
			}
		}
		else
		{
			if (e.OldValueA != null && property.TextValue == null)
			{
				property.CreatePropertyValue(e.OldValueA);
			}
			if (newValueA != null)
			{
				property.SetValue(newValueA);
			}
			else
			{
				property.Reset();
			}
		}
	}

	private void PropertyValueChanged(object sender, HandyControl.EventArgs.PropertyValueChangedEventArgs e)
	{
		if (e.ChangedProptertyA == null)
		{
			return;
		}
		DesignSurface val = ds;
		ISelectionService val2 = ((val != null) ? val.DesignContext.Services.Selection : null);
		if (val2 == null)
		{
			return;
		}
		DesignItem primarySelection = val2.PrimarySelection;
		if (primarySelection == null)
		{
			return;
		}
		ChangeGroup val3 = null;
		if (!AttributeContainsType(e.ChangedProptertyA.Attributes, typeof(ChangeGroupCommitIgnoreAttribute)))
		{
			val3 = primarySelection.OpenGroup("ModifyProperty");
		}
		try
		{
			foreach (DesignItem selectedItem in val2.SelectedItems)
			{
				changeDesignItemProptery(selectedItem, e);
			}
		}
		catch
		{
			if (val3 != null)
			{
				val3.Abort();
			}
		}
		finally
		{
			if (val3 != null)
			{
				val3.Commit();
			}
			val3 = null;
		}
		IsHasSaveDesign = false;
	}

	public void PerformAction(ICommand command)
	{
		((UIElement)(object)ds).Focus();
		Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
		{
			if (command != null)
			{
				command.Execute(null);
			}
			IsHasSaveDesign = false;
		}));
	}

	private void ControlSelection_SelectionChanged(object sender, DesignItemCollectionEventArgs e)
	{
		ISelectionService selection = _mainDesignSurface.DesignContext.Services.Selection;
		if (selection != null)
		{
			if (isSelectSameItems())
			{
				EditData.SelectedObject = selection.PrimarySelection.Component;
			}
			else
			{
				EditData.SelectedObject = null;
			}
		}
	}

	private void OpenBtn_OnClick(object sender, RoutedEventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = FrontendUI.Design.Helper.ResourceBinding.FindResoure("打开设计");
		openFileDialog.Filter = string.Format("{0}.vmd|*.vmd", FrontendUI.Design.Helper.ResourceBinding.FindResoure("设计文件"));
		string initPath = GetInitPath("OpenSol");
		if (initPath != null)
		{
			openFileDialog.InitialDirectory = initPath;
		}
		if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
		{
			return;
		}
		string path = openFileDialog.FileName;
		if (!File.Exists(path))
		{
			HikMessageBox.Show("Lang_OpenFileErr", "Lang_FileNotEixt", VM.Controls.MessageIcons.Error, isTranslate: true);
		}
		else
		{
			if (DoSave2Design() == VM.Controls.DialogResult.Cancel)
			{
				return;
			}
			progressMask.Visibility = Visibility.Visible;
			SaveInitPath("OpenSol", path);
			Task.Run(() =>
			{
				Dispatcher.Invoke(() =>
				{
					try
					{
						string vMInstallPath = CurrentPathHelper.Instance.GetVMInstallPath();
						string text = Path.Combine(vMInstallPath, "CompatibleTool.exe");
						string path2 = Path.Combine(vMInstallPath, "CompatibleTool.exe.config");
						if (!File.Exists(text) || !File.Exists(path2))
						{
							HikMessageBox.Show("CompatibleTool.exe" + AppLocalizationService.GetLang("Lang_FileNotEixt"), VM.Controls.MessageIcons.Ok, VM.Controls.MessageButtons.NoButton, 3000u);
						}
						else
						{
							bool flag = false;
							string lang = AppLocalizationService.GetLang("Lang_version");
							int length = path.Length;
							string text2 = path.Insert(length - 4, "_" + lang);
							string text3 = text2;
							Process process = new Process();
							process.StartInfo.FileName = text;
							process.StartInfo.WorkingDirectory = vMInstallPath;
							process.StartInfo.Arguments = $"VMDesign;\"{path}\";;\"{text2}\";{lang}";
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
							process.StartInfo.CreateNoWindow = true;
							process.StartInfo.RedirectStandardOutput = true;
							process.StartInfo.RedirectStandardError = true;
							process.Start();
							process.WaitForExit();
							string text4 = process.StandardOutput.ReadToEnd();
							if (File.Exists(text3))
							{
								flag = true;
								path = text3;
							}
							else if (!text4.Contains(path))
							{
								HikMessageBox.Show(AppLocalizationService.GetLang("升级失败"), AppLocalizationService.GetLang(text4), VM.Controls.MessageIcons.Warning);
								return;
							}
							string s = File.ReadAllText(path);
							byte[] bytes = Encoding.UTF8.GetBytes(s);
							DocumentManager.Instance().TempDesignerBuffer = new MemoryStream(bytes, 0, bytes.Length, writable: true, publiclyVisible: true);
							InitDesignerSurface(s, isSubDesign: false, whetherToUseTheRootStr: true);
						}
					}
					catch (Exception message)
					{
						LogHelper.frontedLog.Error(message);
					}
					finally
					{
						progressMask.Visibility = Visibility.Collapsed;
					}
				});
			});
		}
	}

	private void SaveAsBtn_Click(object sender, RoutedEventArgs e)
	{
		FileStream fileStream = null;
		try
		{
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Title = FrontendUI.Design.Helper.ResourceBinding.FindResoure("Lang_DesignSave");
				saveFileDialog.OverwritePrompt = true;
				saveFileDialog.Filter = string.Format("{0}.vmd|*.vmd", FrontendUI.Design.Helper.ResourceBinding.FindResoure("设计文件"));
				string initPath = GetInitPath("OpenSol");
				if (initPath != null)
				{
					saveFileDialog.InitialDirectory = initPath;
				}
				if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK || string.IsNullOrEmpty(saveFileDialog.FileName) || saveFileDialog.FileName.Length > 260)
				{
					return;
				}
				SaveInitPath("OpenSol", saveFileDialog.FileName);
				Save(save2Solution: false, null, isExcuteAction: true, save2Design: true);
				if (DocumentManager.Instance().TempDesignerBuffer != null)
				{
					using (fileStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
					{
						fileStream.SetLength(0L);
						long length = DocumentManager.Instance().TempDesignerBuffer.Length;
						byte[] buffer = DocumentManager.Instance().TempDesignerBuffer.GetBuffer();
						fileStream.Write(buffer, 0, (int)length);
						fileStream.Flush(flushToDisk: true);
						fileStream.Seek(0L, SeekOrigin.Begin);
					}
					fileStream = null;
				}
			}
			HikMessageBox.Show("Lang_DesignSave", "Lang_DesignSaveOK", VM.Controls.MessageIcons.Ok, isTranslate: true);
		}
		catch (Exception ex)
		{
			LogHelper.frontedLog.Error(ex);
			HikMessageBox.Show(FrontendUI.Design.Helper.ResourceBinding.FindResoure("Lang_DesignSave"), string.Format("{0} {1}", FrontendUI.Design.Helper.ResourceBinding.FindResoure("Savefailed"), ex.Message), VM.Controls.MessageIcons.Error);
		}
		finally
		{
			fileStream?.Dispose();
		}
	}

	private void SetFrontBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.ForwardCommand);
	}

	private void SetBackBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.BackwardCommand);
	}

	private void SetTopBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.BringToFrontCommand);
	}

	private void SetBottomBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.SendToBackCommand);
	}

	private void UndoBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(ApplicationCommands.Undo);
	}

	private void RedoBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(ApplicationCommands.Redo);
	}

	private void AlignLeftBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.AlignLeftCommand);
	}

	private void AlignRightBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.AlignRightCommand);
	}

	private void AlignUptBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.AlignTopCommand);
	}

	private void AlignCentertBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.AlignCenterCommand);
	}

	private void AlignVCentertBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.AlignMiddleCommand);
	}

	private void AlignDowntBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.AlignBottomCommand);
	}

	private void HCenter_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.HorizSpaceMakeEqual);
	}

	private void VCenter_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(Commands.VertSpaceMakeEqual);
	}

	private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
	{
		PerformAction(ApplicationCommands.Delete);
	}

	public static void SetIViewSize(Size viewSize)
	{
		_viewSize = viewSize;
	}

	private void InitAdjuctRes()
	{
		AdjuctRes_OnClick(null, null);
	}

	private void AdjuctRes_OnClick(object sender, RoutedEventArgs e)
	{
		if (_mainDesignSurface == null || _mainDesignSurface.DesignContext == null || _mainDesignSurface.DesignContext.RootItem == null)
		{
			return;
		}
		try
		{
			ChangeGroup val = _mainDesignSurface.DesignContext.RootItem.OpenGroup("Adjust Resolution");
			_ = _viewSize;
			if (true)
			{
				if (_viewSize.Width > 0.0)
				{
					_mainDesignSurface.DesignContext.RootItem.Properties["Width"].SetValue((object)((int)_viewSize.Width * 2 / 3));
				}
				if (_viewSize.Height > 0.0)
				{
					_mainDesignSurface.DesignContext.RootItem.Properties["Height"].SetValue((object)((int)_viewSize.Height * 2 / 3));
				}
				val.Commit();
				_mainDesignSurface.DesignerAutoScale();
			}
		}
		catch (Exception message)
		{
			LogHelper.frontedLog.Error(message);
		}
	}

	private void ToolBoxItem_DragOver(object sender, System.Windows.DragEventArgs e)
	{
		toolBox.UpdateAdornerLocation();
	}

	private void ToolBoxItem_DragLeave(object sender, System.Windows.DragEventArgs e)
	{
		toolBox.HideAdorner();
	}

	private void ToolBoxItem_DragEnter(object sender, System.Windows.DragEventArgs e)
	{
		toolBox.ShowAdorner();
		IsHasSaveDesign = false;
	}

	private void btnSave_Click(object sender, RoutedEventArgs e)
	{
		OnSaveTextDesign?.Invoke();
	}

	private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		bool? flag = !toolCheckBox.IsChecked;
		if (System.Convert.ToBoolean(flag))
		{
			toolCheckBox.IsChecked = flag;
		}
	}

	private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (sender is ScrollViewer scrollViewer)
		{
			double num = Math.Abs(e.Delta / 2);
			double num2 = 0.0;
			num2 = ((e.Delta <= 0) ? Math.Min(scrollViewer.ScrollableWidth, scrollViewer.HorizontalOffset + num) : Math.Max(0.0, scrollViewer.HorizontalOffset - num));
			if (num2 != scrollViewer.HorizontalOffset)
			{
				scrollViewer.ScrollToHorizontalOffset(num2);
				e.Handled = true;
			}
		}
	}

	private void PreViewBtn_Click(object sender, RoutedEventArgs e)
	{
		UpdateRootStr();
		SingletonBase<DesignerContext>.Instance.DisplayStringA = RootStr;
		FrameworkElement frameworkElement = (FrameworkElement)XamlReader.Parse(RootStr);
		if (frameworkElement is Window window)
		{
			window.Resources.MergedDictionaries.Add((ResourceDictionary)(object)LocalizationResourceDictionary.Instance);
			FrontendWindowData.Instance.DisplayMode = DisplayModeEnum.PreView;
			EditFrontendWindow parentFrontendWindow = FrontendWindowData.GetParentFrontendWindow(this);
			if (parentFrontendWindow != null)
			{
				window.Owner = parentFrontendWindow;
			}
			FrontendWindowData.Instance.OnInitDesignerSurfaceEndChanged();
			window.Loaded += Win_Loaded;
			if (window.ShowDialog().Value)
			{
				window.Loaded -= Win_Loaded;
			}
			FrontendWindowData.Instance.DisplayMode = DisplayModeEnum.Editing;
		}
	}

	private void Win_Loaded(object sender, RoutedEventArgs e)
	{
		FrontendTabControl firstChild;
		if ((firstChild = FrontendWindowClone.GetFirstChild<FrontendTabControl>(MainSurface.DesignPanel.Context.RootItem)) != null)
		{
			int selectedIndex = firstChild.SelectedIndex;
			firstChild = Apps.UIHelper.Extern.UIHelper.GetChild<FrontendTabControl>(sender as FrameworkElement);
			firstChild.SelectedIndex = selectedIndex;
		}
	}

	private void CreateCodeExcute()
	{
		DoSave(isShowCancelBtn: false);
		CodeCreateBuilder codeCreateBuilder = new CodeCreateBuilder
		{
			Solution = VmSolution.Instance
		};
		EditFrontendWindow parentFrontendWindow = FrontendWindowData.GetParentFrontendWindow(this);
		if (parentFrontendWindow != null)
		{
			parentFrontendWindow.SetBinding(EditFrontendWindow.IsShowProgressMaskProperty, new System.Windows.Data.Binding("IsShowProgressMask")
			{
				Source = codeCreateBuilder
			});
			parentFrontendWindow.SetBinding(EditFrontendWindow.MaskMessageProperty, new System.Windows.Data.Binding("ProgressMessage")
			{
				Source = codeCreateBuilder
			});
			codeCreateBuilder.FrontCreateCode(parentFrontendWindow);
		}
	}

	private void btnLayoutSet_Click(object sender, RoutedEventArgs e)
	{
		LayoutSetWin layoutSetWin = new LayoutSetWin(_mainDesignSurface);
		if (layoutSetWin.ShowDialog() == true)
		{
			ChangeGroup val = _mainDesignSurface.DesignContext.RootItem.OpenGroup("Chang Layout");
			layoutSetWin.ChangView(_mainDesignSurface);
			val.Commit();
		}
	}

	private void DesignTemplateManaggement_Click(object sender, RoutedEventArgs e)
	{
		DesignTemplateManagement designTemplateManagement = new DesignTemplateManagement
		{
			ParentObj = this
		};
		designTemplateManagement.ShowDialog();
	}

	private void SaveDesignTemplate_OnClick(object sender, RoutedEventArgs e)
	{
		if (DesignTemplateHelper.Instance.TemplateNoDesignDataList.Count() >= 50)
		{
			HikMessageBox.Show(string.Format("{0} [{1}]", AppLocalizationService.GetLang("Lang_TemplateLimit"), 50), VM.Controls.MessageIcons.Ok, VM.Controls.MessageButtons.NoButton, 1000u);
			return;
		}
		DesignerLayoutTemplateWin designerLayoutTemplateWin = new DesignerLayoutTemplateWin(this);
		EditFrontendWindow parentFrontendWindow = FrontendWindowData.GetParentFrontendWindow(this);
		if (parentFrontendWindow != null)
		{
			designerLayoutTemplateWin.Owner = parentFrontendWindow;
		}
		designerLayoutTemplateWin.ShowDialog();
	}

	public void ApplyDesignTemplate(string rootStr)
	{
		InitDesignerSurface(rootStr, isSubDesign: false, whetherToUseTheRootStr: true);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/FrontendUI.Design;component/frontendedit.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Expected Obj, but got Unknown
		switch (connectionId)
		{
		case 1:
			us = (FrontendEdit)target;
			break;
		case 2:
			Grid = (Grid)target;
			break;
		case 3:
			TitleBar = (Border)target;
			break;
		case 4:
			m_HorScroll = (ScrollViewer)target;
			m_HorScroll.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
			break;
		case 5:
			Menu = (System.Windows.Controls.Menu)target;
			break;
		case 6:
			((VM.Controls.IconMenuItem)target).Click += OpenBtn_OnClick;
			break;
		case 7:
			((VM.Controls.IconMenuItem)target).Click += SaveAsBtn_Click;
			break;
		case 8:
			((VM.Controls.IconMenuItem)target).Click += DesignTemplateManaggement_Click;
			break;
		case 9:
			((VM.Controls.IconMenuItem)target).Click += SaveDesignTemplate_OnClick;
			break;
		case 10:
			((VM.Controls.IconMenuItem)target).Click += SetTopBtn_OnClick;
			break;
		case 11:
			((VM.Controls.IconMenuItem)target).Click += SetFrontBtn_OnClick;
			break;
		case 12:
			((VM.Controls.IconMenuItem)target).Click += SetBottomBtn_OnClick;
			break;
		case 13:
			((VM.Controls.IconMenuItem)target).Click += SetBackBtn_OnClick;
			break;
		case 14:
			((VM.Controls.IconMenuItem)target).Click += AlignLeftBtn_OnClick;
			break;
		case 15:
			((VM.Controls.IconMenuItem)target).Click += AlignCentertBtn_OnClick;
			break;
		case 16:
			((VM.Controls.IconMenuItem)target).Click += AlignRightBtn_OnClick;
			break;
		case 17:
			((VM.Controls.IconMenuItem)target).Click += AlignUptBtn_OnClick;
			break;
		case 18:
			((VM.Controls.IconMenuItem)target).Click += AlignVCentertBtn_OnClick;
			break;
		case 19:
			((VM.Controls.IconMenuItem)target).Click += AlignDowntBtn_OnClick;
			break;
		case 20:
			((VM.Controls.IconMenuItem)target).Click += HCenter_OnClick;
			break;
		case 21:
			((VM.Controls.IconMenuItem)target).Click += VCenter_OnClick;
			break;
		case 22:
			((VM.Controls.IconMenuItem)target).Click += UndoBtn_OnClick;
			break;
		case 23:
			((VM.Controls.IconMenuItem)target).Click += RedoBtn_OnClick;
			break;
		case 24:
			((VM.Controls.IconMenuItem)target).Click += DeleteBtn_OnClick;
			break;
		case 25:
			((VM.Controls.IconMenuItem)target).Click += AdjuctRes_OnClick;
			break;
		case 26:
			Slider = (Slider)target;
			break;
		case 27:
			((System.Windows.Controls.Button)target).Click += btnLayoutSet_Click;
			break;
		case 28:
			PreViewBtn = (System.Windows.Controls.Button)target;
			PreViewBtn.Click += PreViewBtn_Click;
			break;
		case 29:
			CreateCodeBtn = (System.Windows.Controls.Button)target;
			break;
		case 30:
			toolGrid = (Grid)target;
			break;
		case 31:
			toolCheckBox = (System.Windows.Controls.CheckBox)target;
			break;
		case 32:
			((Grid)target).PreviewMouseDown += Grid_PreviewMouseDown;
			break;
		case 33:
			toolBox = (ToolBox)target;
			break;
		case 34:
			Designer = (Grid)target;
			break;
		case 35:
			ds = (DesignSurface)target;
			break;
		case 36:
			progressMask = (ProgressMask)target;
			break;
		case 37:
			settingTab = (System.Windows.Controls.TabControl)target;
			break;
		case 38:
			PropertyTabItem = (TabItem)target;
			break;
		case 39:
			propertyGrid2 = (HandyControl.Controls.PropertyGrid)target;
			break;
		case 40:
			propertyGrid = (HandyControl.Controls.PropertyGrid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
