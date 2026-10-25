using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Apps.Localization;
using Apps.PcUI.Controls;
using Apps.UIHelper.Extern;
using DesignerModifyDataSource;
using FrontendUI.Design.Convert;
using FrontendUI.Design.Guide;
using FrontendUI.Design.Helper;
using FrontendUI.Design.TestDesigner;
using Mvvm.Commands;
using VM.Controls;
using VM.Controls.Utility;
using VM.Core;
using VM.PlatformSDKCS;
using VM.Utility;
using VM.Utility.SingleInstance;

namespace FrontendUI.Design;

public class EditFrontendWindow : BaseWindow, IComponentConnector
{
	private bool _isCodeCreating;

	public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(EditFrontendWindow), new PropertyMetadata(false, OnIsBusyPropertyChanged));

	public static readonly DependencyProperty IsShowProgressMaskProperty = DependencyProperty.Register("IsShowProgressMask", typeof(bool), typeof(EditFrontendWindow), new PropertyMetadata(false, OnIsShowProgressMaskChanged));

	public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(EditFrontendWindow), new PropertyMetadata(0.0));

	public static readonly DependencyProperty MaskMessageProperty = DependencyProperty.Register("MaskMessage", typeof(string), typeof(EditFrontendWindow), new PropertyMetadata(null));

	private ProgressMask progressMask;

	private DelegateCommand<object> maxBtnCommand = null;

	private DelegateCommand _openHelpCommand;

	internal EditFrontendWindow editFrontendWindow;

	internal Image img;

	internal Grid PART_View;

	internal FrontendEdit mainEditView;

	private bool _contentLoaded;

	public bool IsCodeCreating
	{
		get
		{
			return _isCodeCreating;
		}
		set
		{
			if (_isCodeCreating != value)
			{
				_isCodeCreating = value;
				OnPropertyChanged("IsCodeCreating");
			}
		}
	}

	public string EditFrontTitle
	{
		get
		{
			if (IsBusy)
			{
				return AppLocalizationService.GetLang("Lang_FrontTitle") + AppLocalizationService.GetLang("Lang_FrontendLocked");
			}
			return AppLocalizationService.GetLang("Lang_FrontTitle");
		}
	}

	public bool IsBusy
	{
		get
		{
			return (bool)GetValue(IsBusyProperty);
		}
		set
		{
			SetValue(IsBusyProperty, value);
		}
	}

	public bool IsShowProgressMask
	{
		get
		{
			return (bool)GetValue(IsShowProgressMaskProperty);
		}
		set
		{
			SetValue(IsShowProgressMaskProperty, value);
		}
	}

	public double Progress
	{
		get
		{
			return (double)GetValue(ProgressProperty);
		}
		set
		{
			SetValue(ProgressProperty, value);
		}
	}

	public string MaskMessage
	{
		get
		{
			return (string)GetValue(MaskMessageProperty);
		}
		set
		{
			SetValue(MaskMessageProperty, value);
		}
	}

	public ICommand MaxBtnCommand => (ICommand)(maxBtnCommand ?? (maxBtnCommand = new DelegateCommand<object>((Action<object>)MaxBtnExcute)));

	public DelegateCommand OpenHelpCommand
	{
		get
		{
			if (_openHelpCommand == null)
			{
				_openHelpCommand = new DelegateCommand(ExecuteOpenHelp);
			}
			return _openHelpCommand;
		}
	}

	public EditFrontendWindow()
	{
		InitializeComponent();
		FrontendWindowData.Instance.DisplayMode = DisplayModeEnum.Editing;
		mainEditView.InitFrontendWindow(SingletonBase<DesignerContext>.Instance.DisplayStringA);
		RemoveHandlerVM();
		AddHandlerVM();
		MultiBinding multiBinding = new MultiBinding();
		Binding item = new Binding
		{
			Source = VmSolution.Instance,
			Path = new PropertyPath("IsRunning"),
			Mode = BindingMode.OneWay
		};
		Binding item2 = new Binding
		{
			Source = VmSolution.Instance,
			Path = new PropertyPath("ContinuousRunEnable"),
			Mode = BindingMode.OneWay
		};
		multiBinding.Bindings.Add(item);
		multiBinding.Bindings.Add(item2);
		multiBinding.Converter = new MulitBoolOrConvert();
		multiBinding.Mode = BindingMode.OneWay;
		SetBinding(IsBusyProperty, multiBinding);
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		if (IsCodeCreating)
		{
			e.Cancel = true;
		}
		else if (mainEditView.Close())
		{
			base.OnClosing(e);
			RemoveHandlerVM();
		}
		else
		{
			e.Cancel = true;
		}
	}

	private static void OnIsBusyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((EditFrontendWindow)d)?.OnPropertyChanged("EditFrontTitle");
	}

	private static void OnIsShowProgressMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		EditFrontendWindow editFrontendWindow = (EditFrontendWindow)d;
		if ((bool)e.NewValue)
		{
			if (editFrontendWindow.progressMask != null)
			{
				editFrontendWindow.RemoveMask();
			}
			editFrontendWindow.AddMask();
		}
		else
		{
			editFrontendWindow.RemoveMask();
		}
	}

	private void RemoveMask()
	{
		PART_View.Children.Remove(progressMask);
		progressMask = null;
	}

	private void AddMask()
	{
		progressMask = new ProgressMask();
		progressMask.SetBinding(ProgressMask.MessageProperty, new Binding("MaskMessage")
		{
			Source = this
		});
		progressMask.SetBinding(ProgressMask.ValueProperty, new Binding("Progress")
		{
			Source = this
		});
		progressMask.SetValue(Grid.ColumnSpanProperty, 2);
		progressMask.SetValue(Grid.RowSpanProperty, 2);
		PART_View.Children.Add(progressMask);
	}

	private void AddHandlerVM()
	{
		FrontendWindowData.CreateNewDesignEvent += FrontendWindowData_CreateNewDesign;
		VmSolution.OnSolutionLoadBeginCallBack += VmSolution_OnSolutionLoadBeginCallBack;
		VmSolution.OnSolutionLoadProgressCallBack += VmSolution_OnSolutionLoadProgressCallBack;
		VmSolution.OnSolutionLoadEndCallBack += VmSolution_OnSolutionLoadEndCallBack;
		VmSolution.OnSolutionSaveBeginCallBack += VmSolution_OnSolutionSaveBeginCallBack;
		VmSolution.OnSolutionSaveProgressCallBack += VmSolution_OnSolutionSaveProgressCallBack;
		VmSolution.OnSolutionSaveEndCallBack += VmSolution_OnSolutionSaveEndCallBack;
	}

	private void RemoveHandlerVM()
	{
		FrontendWindowData.CreateNewDesignEvent -= FrontendWindowData_CreateNewDesign;
		VmSolution.OnSolutionLoadBeginCallBack -= VmSolution_OnSolutionLoadBeginCallBack;
		VmSolution.OnSolutionLoadProgressCallBack -= VmSolution_OnSolutionLoadProgressCallBack;
		VmSolution.OnSolutionLoadEndCallBack -= VmSolution_OnSolutionLoadEndCallBack;
		VmSolution.OnSolutionSaveBeginCallBack -= VmSolution_OnSolutionSaveBeginCallBack;
		VmSolution.OnSolutionSaveEndCallBack -= VmSolution_OnSolutionSaveEndCallBack;
		VmSolution.OnSolutionSaveProgressCallBack -= VmSolution_OnSolutionSaveProgressCallBack;
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		if (!CVmObjectBase.GetIsSDKMode() && System.Convert.ToBoolean(ConfigurationHelper.GetConfig("IsLoadFrontendGuide")))
		{
			GuidePages guidePages = new GuidePages();
			guidePages.Owner = this;
			guidePages.Left = Left + (Width - guidePages.Width) / 2.0;
			guidePages.Top = Top + (Height - guidePages.Height) / 2.0;
			guidePages.Show();
		}
	}

	private void FrontendWindowData_CreateNewDesign(object sender, EventArgs e)
	{
		Dispatcher.Invoke(() =>
		{
			mainEditView.InitDesignerSurface(string.Empty);
		});
	}

	private void VmSolution_InstanceChanged(object sender, EventArgs e)
	{
	}

	private void VmSolution_OnSolutionSaveBeginCallBack(object sender, ObjectEventArgs args)
	{
		if (!IsCodeCreating)
		{
			Dispatcher.Invoke(() =>
			{
				MaskMessage = FrontendUI.Design.Helper.ResourceBinding.FindResoure("方案保存中");
				IsShowProgressMask = true;
			});
		}
	}

	private void VmSolution_OnSolutionSaveProgressCallBack(object sender, ObjectEventArgs args)
	{
		if (!IsCodeCreating)
		{
			uint progress = (uint)args.Param;
			Dispatcher.Invoke(() =>
			{
				Progress = progress;
			});
		}
	}

	private void VmSolution_OnSolutionSaveEndCallBack(object sender, EndEventArgs args)
	{
		if (IsCodeCreating)
		{
			return;
		}
		Dispatcher.Invoke(() =>
		{
			if (IsShowProgressMask)
			{
				IsShowProgressMask = false;
			}
		});
	}

	private void VmSolution_OnSolutionLoadBeginCallBack(object sender, ObjectEventArgs args)
	{
		Dispatcher.Invoke(() =>
		{
			IsShowProgressMask = true;
			MaskMessage = FrontendUI.Design.Helper.ResourceBinding.FindResoure("方案加载中");
		});
	}

	private void VmSolution_OnSolutionLoadProgressCallBack(object sender, ObjectEventArgs args)
	{
		uint progress = (uint)args.Param;
		Dispatcher.Invoke(() =>
		{
			Progress = progress;
		});
	}

	private void VmSolution_OnSolutionLoadEndCallBack(object sender, EndEventArgs args)
	{
		DocumentManager.Instance().isSolutionDesignerBuffer_Loaded = false;
		Dispatcher.Invoke(() =>
		{
			mainEditView.InitDesignerSurface(string.Empty);
			if (IsShowProgressMask)
			{
				IsShowProgressMask = false;
			}
		});
	}

	protected override void Header_DoubleClick(object sender, MouseButtonEventArgs e)
	{
		switch (WindowState)
		{
		case WindowState.Maximized:
			WindowState = WindowState.Normal;
			break;
		case WindowState.Normal:
			WindowState = WindowState.Maximized;
			break;
		}
		e.Handled = true;
	}

	private void MaxBtnExcute(object ischeckedObj)
	{
	}

	private void ExecuteOpenHelp()
	{
		string toolItemInfoPath = $"{CurrentPathHelper.Instance.GetCurrentPath()}\\\\Resources\\Frontend\\ToolItemInfo.xml";
		HelpDocumentUtil.OpenHelpDocument(toolItemInfoPath);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/FrontendUI.Design;component/editfrontendwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			editFrontendWindow = (EditFrontendWindow)target;
			break;
		case 2:
			img = (Image)target;
			break;
		case 3:
			PART_View = (Grid)target;
			break;
		case 4:
			mainEditView = (FrontendEdit)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
