using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VisionFramework;
using Button=System.Windows.Controls.Button;
using TextBox=System.Windows.Controls.TextBox;
using MenuItem=System.Windows.Controls.MenuItem;
using ContextMenu=System.Windows.Controls.ContextMenu;
using Brushes=System.Windows.Media.Brushes;
using Image=System.Windows.Controls.Image;
using Color=System.Windows.Media.Color;
using Orientation=System.Windows.Controls.Orientation;
using Cursors=System.Windows.Input.Cursors;
using TreeView=System.Windows.Controls.TreeView;

namespace VisionEditor;

public partial class MainWindow
{
    readonly Dictionary<string, Border> procedureHeaders = new();
    readonly Dictionary<string,string> workflowHeaderStates=new();
    bool refreshingWorkflowTabs,workflowOverview;
    string? focusedProcedureId;
    System.Windows.Controls.Primitives.Popup? workflowListPopup;

    WorkflowImageButton WorkflowIcon(string stem,string tip,Action? action,bool enabled=true)
    {
        var button=new WorkflowImageButton {Style=(Style)FindResource("WorkflowImageButton"),ToolTip=tip,IsEnabled=enabled};
        SetWorkflowIcon(button,stem);
        if(action is not null)button.Click+=(_,e)=>{e.Handled=true;Safe(action);};
        return button;
    }
    void SetWorkflowIcon(WorkflowImageButton button,string stem)
    {
        button.NormalSource=Asset(stem+"_normal.png");
        System.Windows.Media.ImageSource Variant(string state)=>System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory,"ui-assets","wpf",stem+"_"+state+".png"))?Asset(stem+"_"+state+".png"):button.NormalSource;
        button.HoverSource=Variant("hover");button.PressedSource=Variant("click");button.DisabledSource=Variant("disable");
    }
    void WorkflowSelectionChanged(object sender,SelectionChangedEventArgs e)
    {
        if(refreshingWorkflowTabs||!ReferenceEquals(e.Source,ProcedureTabs))return;
        if(ProcedureTabs.SelectedItem is TabItem {Tag:string id} tab)
        {
            // ProcedureConfigControl.m_ProcessTabControl_SelectionChanged focuses
            // the selected item first; the ScrollViewer then applies its edge allowance.
            tab.Focus();
            Keyboard.ClearFocus();
            focusedProcedureId=id;
            SelectWorkflow(id);
        }
    }
    void SelectWorkflow(string id)
    {
        if(!doc.Document.Procedures.Any(p=>p.Id==id))return;
        ResetEditingGesture();ClearToolGhost();
        if(toolPopup is not null)toolPopup.IsOpen=false;
        selected=null;selectedNodes.Clear();selectedWire=null;
        if(doc.CurrentProcedureId!=id)doc.SelectProcedure(id);else {RenderGraph();RefreshRight();}
    }

    // ProcedureConfigControl.TopTabItemStyle: config, variables, status, name, run buttons.
    void RefreshAll()
    {
        if (!IsInitialized || doc is null || windowClosed) return;
        workflowConfigurationWindow?.SetExecutionState(activeRun is not null,runningProcedureId==configurationProcedureId,isLocked);
        if(toolRailOverview!=workflowOverview)
        {
            if(toolPopup is not null)toolPopup.IsOpen=false;
            if(workflowOverview){toolboxExpanded=false;ToggleToolboxButton.Tag="folded";if(ToolRail.Parent is Grid host)host.ColumnDefinitions[0].Width=new GridLength(53);}
            BuildRail();
        }
        ToggleToolboxButton.Visibility=workflowOverview?Visibility.Collapsed:Visibility.Visible;
        ToolRail.RowDefinitions[1].Height=new GridLength(workflowOverview?0:22);
        RestoreWorkflowResults();
        EmptyWorkflowPanel.Visibility=doc.HasCurrentProcedure?Visibility.Collapsed:Visibility.Visible;
        WorkflowHeaderRow.Height=new GridLength(doc.HasCurrentProcedure?40:0);
        ProcedureTabScroll.Visibility=workflowOverview?Visibility.Collapsed:Visibility.Visible;
        WorkflowActionsRight.Visibility=workflowOverview?Visibility.Collapsed:Visibility.Visible;
        WorkflowViewButton.ToolTip=workflowOverview?"子流程":"全部流程";
        Breadcrumb.Visibility=workflowOverview||!doc.HasCurrentProcedure?Visibility.Collapsed:Visibility.Visible;
        CanvasViewport.Visibility=doc.HasCurrentProcedure?Visibility.Visible:Visibility.Collapsed;
        MiniMapPanel.Visibility=doc.HasCurrentProcedure&&eyeExpanded?Visibility.Visible:Visibility.Collapsed;
        MiniMapToggle.Visibility=doc.HasCurrentProcedure?Visibility.Visible:Visibility.Collapsed;
        EmptyWorkflowImage.Source=Asset("pic_流程.png");EmptyAddImage.Source=Asset("btn_add_normal.png");
        WindowProjectName.Text = doc.Document.Name;
        SolutionTitle.Visibility = Visibility.Collapsed;
        Breadcrumb.Text = GroupBreadcrumb();Breadcrumb.IsHitTestVisible=doc.CurrentProcedure.GroupOwner is not null;Breadcrumb.ToolTip=doc.CurrentProcedure.GroupOwner is null?null:"返回上一级 Group / 流程";Breadcrumb.MouseLeftButtonDown-=GroupBreadcrumbBack;Breadcrumb.MouseLeftButtonDown+=GroupBreadcrumbBack;
        refreshingWorkflowTabs=true;
        var procedureIds=doc.Document.Procedures.Where(p=>p.GroupOwner is null).Select(p=>p.Id).ToHashSet();
        foreach(var removed in ProcedureTabs.Items.OfType<TabItem>().Where(t=>!procedureIds.Contains((string)t.Tag)).ToArray())
        {ProcedureTabs.Items.Remove(removed);procedureHeaders.Remove((string)removed.Tag);workflowHeaderStates.Remove((string)removed.Tag);}
        int tabIndex=0;
        foreach (var procedure in doc.Document.Procedures.Where(p=>p.GroupOwner is null))
        {
            bool active = procedure.Id == doc.RootProcedureId(doc.CurrentProcedureId);
            var displayed=active?doc.CurrentProcedure:procedure;bool ready = GraphValidator.Validate(displayed, modules).IsValid;
            var tab=ProcedureTabs.Items.OfType<TabItem>().FirstOrDefault(t=>(string)t.Tag==procedure.Id);
            if(tab is null){tab=new TabItem {Tag=procedure.Id};ProcedureTabs.Items.Insert(tabIndex,tab);}
            else if(ProcedureTabs.Items.IndexOf(tab)!=tabIndex){ProcedureTabs.Items.Remove(tab);ProcedureTabs.Items.Insert(tabIndex,tab);}
            tabIndex++;
            tab.IsSelected=active;
            string state=System.Text.Json.JsonSerializer.Serialize(new {procedure.Name,displayId=displayed.Id,displayName=displayed.Name,hasParent=displayed.GroupOwner is not null,active,ready,isLocked,running=activeRun is not null,continuousRun,runningProcedureId,procedure.StopOnNg,parameterEditorOpen,empty=procedure.Nodes.Count==0});
            if(workflowHeaderStates.GetValueOrDefault(procedure.Id)==state)continue;
            workflowHeaderStates[procedure.Id]=state;
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 8, 0), VerticalAlignment = VerticalAlignment.Center };
            if (active)
            {
                if(displayed.GroupOwner is not null){var back=WorkflowIcon("tab_ic_back","返回上一级",ReturnFromGroup,!parameterEditorOpen&&activeRun is null);back.Tag="GroupTabBack";back.Margin=new Thickness(3,0,4,0);back.HoverSource=Asset("tab_ic_back_disable.png");back.DisabledSource=Asset("tab_ic_back_hover.png");row.Children.Add(back);row.Children.Add(new Border{Width=1,Height=30,Background=new SolidColorBrush(Color.FromRgb(48,50,52))});}
                row.Children.Add(WorkflowIcon("ic_tab_group_configuration", "流程配置", OpenDisplayedConfiguration, !isLocked&&activeRun is null&&!parameterEditorOpen));
                row.Children.Add(WorkflowIcon("ic_local-variable", "局部变量", ()=>OpenWorkflowLocalVariables(doc.CurrentProcedureId), !isLocked&&activeRun is null&&!parameterEditorOpen));
                row.Children.Add(new Border { Width = 1, Height = 30, Background = new SolidColorBrush(Color.FromRgb(48, 50, 52)) });
            }
            row.Children.Add(new Ellipse { Width = 8, Height = 8, Fill = new SolidColorBrush(Color.FromRgb(236,0,0)), Margin = new Thickness(8, 0, 0, 0), Visibility = ready ? Visibility.Collapsed : Visibility.Visible });
            var heading = new Border { Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            heading.Child = new TextBlock { Text = displayed.Name, MaxWidth = 240, FontSize = 14, Foreground = new SolidColorBrush(Color.FromRgb(217,217,217)), TextTrimming = TextTrimming.CharacterEllipsis };
            procedureHeaders[procedure.Id] = heading;
            row.Children.Add(heading);
            if (active)
            {
                row.Children.Add(WorkflowIcon("ic_play_单流程", "单次运行", () => { _ = RunProcedure(procedure.Id); }, activeRun is null&&procedure.Nodes.Count>0));
                bool ownsRun=continuousRun&&runningProcedureId==procedure.Id;
                var repeat=WorkflowIcon(ownsRun ? "ic_停止运行" : "ic_多次运行", ownsRun ? "停止运行" : "连续运行", () => StartProcedureLoop(procedure.Id), (activeRun is null || ownsRun)&&procedure.Nodes.Count>0);repeat.Margin=new Thickness(4,0,0,0);row.Children.Add(repeat);
            }
            tab.Header=row;tab.ToolTip=displayed.Name;
            heading.MouseLeftButtonDown+=(_,e)=>{if(e.OriginalSource is TextBox)return; if(active&&doc.CurrentProcedure.GroupOwner is null)SelectWorkflow(procedure.Id);};
            // OperateMenu is assigned to TopTabItemStyle when IsProcedure=True.
            tab.ContextMenu=displayed.GroupOwner is null&&!isLocked&&activeRun is null&&!parameterEditorOpen?ProcedureMenu(procedure.Id):null;
        }
        refreshingWorkflowTabs=false;
        SetWorkflowIcon(AddProcedureButton,"ic_hu_add");SetWorkflowIcon(PinnedAddProcedureButton,"ic_hu_add");SetWorkflowIcon(ProcedureListButton,"ic_tab_downarrow");SetWorkflowIcon(WorkflowViewButton,workflowOverview?"ic_全局":"ic_单个");
        AddProcedureButton.IsEnabled = !isLocked && activeRun is null && !parameterEditorOpen;
        PinnedAddProcedureButton.IsEnabled = AddProcedureButton.IsEnabled;
        AddProcedureButton.Visibility = isLocked ? Visibility.Collapsed : Visibility.Visible;
        Rail.IsEnabled = !isLocked && activeRun is null && !parameterEditorOpen && (workflowOverview||doc.HasCurrentProcedure);
        if(!EditableCanvas&&toolPopup is not null)toolPopup.IsOpen=false;
        EmptyAddWorkflowButton.IsEnabled=!isLocked&&activeRun is null&&!parameterEditorOpen;
        ToggleToolboxButton.IsEnabled = !isLocked;
        undoToolButton!.IsEnabled = !isLocked && activeRun is null && doc.CanUndo;
        redoToolButton!.IsEnabled = !isLocked && activeRun is null && doc.CanRedo;
        stopToolButton!.IsEnabled = activeRun is not null;
        continuousToolButton!.IsEnabled = activeRun is null || continuousRun;
        if (continuousToolButton.Content is Image loopImage) loopImage.Source = Asset(continuousRun ? "ic_停止运行_normal.png" : "toolbar_toolbarRunRepeat_normal.png");
        RenderGraph();
        RefreshRight();
        Dispatcher.BeginInvoke(new Action(UpdateProcedureOverflow));
        if(ProcedureTabs.SelectedItem is TabItem {Tag:string selectedId} selectedTab&&focusedProcedureId!=selectedId)
        {
            focusedProcedureId=selectedId;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if(!ReferenceEquals(ProcedureTabs.SelectedItem,selectedTab)||!selectedTab.IsVisible)return;
                // Unlike the original template's ScrollViewer, our outer
                // ScrollViewer does not reveal an off-screen item on Focus alone.
                selectedTab.BringIntoView();
                selectedTab.Focus();
                Keyboard.ClearFocus();
                if(ProcedureTabs.Items.IndexOf(selectedTab)==0)
                    ProcedureTabScroll.ScrollToHorizontalOffset(0);
                EnsureSelectedProcedureVisible();
            }));
        }
    }

    // ProcedureConfigControl.m_ProcessTabControl_SelectionChanged / Sv_ScrollChanged:
    // adjust only a selected tab already inside the viewport and within 30
    // units of an edge. Focus handles a tab that starts entirely off-screen.
    void EnsureSelectedProcedureVisible()
    {
        if(ProcedureTabs.SelectedItem is not TabItem tab||!tab.IsVisible||ProcedureTabScroll.ViewportWidth<=0)return;
        var position=tab.TransformToAncestor(ProcedureTabScroll).Transform(new System.Windows.Point());
        if(position.X<0||position.X>ProcedureTabScroll.ViewportWidth)return;
        double offset=ProcedureTabScroll.HorizontalOffset;
        if(Math.Abs(position.X)<30)offset-=30;
        if(Math.Abs(position.X+tab.ActualWidth-ProcedureTabScroll.ViewportWidth)<30)offset+=30;
        ProcedureTabScroll.ScrollToHorizontalOffset(Math.Max(0,offset));
    }

    void UpdateProcedureOverflow()
    {
        bool overflow = ProcedureTabs.ActualWidth + 40 > ProcedureTabScroll.ViewportWidth;
        AddProcedureButton.Visibility = isLocked || overflow ? Visibility.Collapsed : Visibility.Visible;
        PinnedAddProcedureButton.Visibility = !isLocked && overflow ? Visibility.Visible : Visibility.Collapsed;
    }

    void ProcedureBarSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (IsLoaded) UpdateProcedureOverflow();
    }

    void AddProcedureClick(object sender, RoutedEventArgs e)
    {
        if (windowClosed||parameterEditorOpen||isLocked || activeRun is not null) return;
        selected = null;
        Safe(() => doc.AddProcedure(NextProcedureName()));
    }

    string NextProcedureName()
    {
        int number = 1;
        while (doc.Document.Procedures.Any(p => p.Name == "流程" + number)) number++;
        return "流程"+number;
    }

    void ProcedureListClick(object sender, RoutedEventArgs e)
    {
        if(workflowListPopup is {IsOpen:true}){workflowListPopup.IsOpen=false;return;}
        var tree=new TreeView {BorderThickness=new Thickness(0),Background=Brushes.Transparent,MaxHeight=320,FontSize=14,ItemContainerStyle=(Style)FindResource("WorkflowTreeItem")};
        foreach (var procedure in doc.Document.Procedures.Where(p=>p.GroupOwner is null))
        {
            var item=new TreeViewItem {Header=procedure.Name,Tag=procedure.Id,Style=(Style)FindResource("WorkflowTreeItem")};
            item.PreviewMouseLeftButtonDown+=(_,e)=> {if(e.OriginalSource is DependencyObject child&&(IsTreeExpander(child)||NearestTreeItem(child)!=item))return;workflowListPopup!.IsOpen=false;workflowOverview=false;SelectWorkflow(procedure.Id);RefreshAll();e.Handled=true;};
            foreach(var node in procedure.Nodes)
            {
                var leaf=new TreeViewItem {Header=node.Name,Tag=node.Id,Style=(Style)FindResource("WorkflowTreeItem")};
                leaf.PreviewMouseLeftButtonDown+=(_,e)=> {workflowListPopup!.IsOpen=false;workflowOverview=false;SelectWorkflow(procedure.Id);selected=node.Id;selectedNodes.Clear();selectedNodes.Add(node.Id);RefreshAll();NavigateToWorkflowNode(node);e.Handled=true;};
                // Bubble selection to the leaf, rather than swallowing it in the parent preview event.
                item.Items.Add(leaf);
            }
            tree.Items.Add(item);
        }
        var background=new LinearGradientBrush(Color.FromRgb(37,37,37),Color.FromRgb(50,50,50),90);
        var border=new Border {Child=tree,CornerRadius=new CornerRadius(5),BorderThickness=new Thickness(1),BorderBrush=new SolidColorBrush(Color.FromRgb(22,22,23)),Background=background,Effect=new System.Windows.Media.Effects.DropShadowEffect {BlurRadius=7,Direction=305,Opacity=.45,ShadowDepth=3}};
        workflowListPopup=new System.Windows.Controls.Primitives.Popup {Child=border,PlacementTarget=ProcedureListButton,Placement=System.Windows.Controls.Primitives.PlacementMode.Bottom,AllowsTransparency=true,StaysOpen=false,IsOpen=true};
    }

    void NavigateToWorkflowNode(NodeDocument node)
    {
        // VmProcedureDecorator.SearchModuleCommandExecute supplies the module Position;
        // ZoomArea.OnSearchModuleInfoChanged uses the unscaled viewport dimensions
        // before multiplying the complete target offset by Scale.
        double x=Math.Max(0,node.X-CanvasViewport.ActualWidth*.5+50)*scale;
        double y=Math.Max(0,node.Y-CanvasViewport.ActualHeight*.5+15)*scale;
        CanvasScrollViewer.ScrollToHorizontalOffset(x);
        CanvasScrollViewer.ScrollToVerticalOffset(y);
    }

    static bool IsTreeExpander(DependencyObject child)
    {
        for(var current=child;current is not null;current=VisualTreeHelper.GetParent(current))
        {if(current is System.Windows.Controls.Primitives.ToggleButton)return true;if(current is TreeViewItem)return false;}
        return false;
    }
    static TreeViewItem? NearestTreeItem(DependencyObject child)
    {
        for(var current=child;current is not null;current=VisualTreeHelper.GetParent(current))if(current is TreeViewItem item)return item;
        return null;
    }

    ContextMenu ProcedureMenu(string id)
    {
        var menu = new ContextMenu();
        void Item(string name, Action? action, string? gap = null)
        {
            var item = new MenuItem { Header = name, IsEnabled = action is not null, ToolTip = gap };
            if (action is not null) item.Click += (_, _) => {if(!CanOperateProcedureMenu(id))return;Safe(action);};
            menu.Items.Add(item);
        }
        Item("导出流程", null, "VM 流程格式导出尚未接入");
        Item("复制流程", () => { selected = null; doc.DuplicateProcedure(id); });
        Item("导出算法代码", null, "代码导出尚未接入");
        Item("设置运行间隔", () => SetProcedureInterval(id));
        Item("删除", () => DeleteProcedure(id, true));
        var ng=new MenuItem {Header="NG时停止流程",StaysOpenOnClick=true,Template=(ControlTemplate)FindResource("WorkflowNgMenuItem")};
        var policy=doc.Document.Procedures.First(p=>p.Id==id);
        var toggle=new System.Windows.Controls.Primitives.ToggleButton {IsChecked=policy.StopOnNg,Margin=new Thickness(5,0,0,0),Focusable=false};
        toggle.Template=(ControlTemplate)FindResource("WorkflowNgSwitch");
        ng.Icon=toggle;
        void SetStopOnNg(bool value){if(!CanOperateProcedureMenu(id)){toggle.IsChecked=doc.Document.Procedures.FirstOrDefault(p=>p.Id==id)?.StopOnNg??false;return;}var p=doc.Document.Procedures.First(p=>p.Id==id);doc.SetProcedureRunPolicy(id,p.ContinuousRunIntervalMs,value);}
        toggle.Checked+=(_,_)=>SetStopOnNg(true);toggle.Unchecked+=(_,_)=>SetStopOnNg(false);
        ng.Click+=(_,e)=>{if(!ReferenceEquals(e.OriginalSource,ng))return;if(CanOperateProcedureMenu(id))toggle.IsChecked=toggle.IsChecked!=true;};
        menu.Items.Add(ng);
        Item("重命名", () => RenameProcedure(id));
        Item("流程超时设置", () => SetProcedureWaitTime(id));
        return menu;
    }

    bool CanEditProcedure(string id)=>!windowClosed&&!parameterEditorOpen&&!isLocked&&activeRun is null&&doc.Document.Procedures.Any(p=>p.Id==id);
    bool CanOperateProcedureMenu(string id)=>CanEditProcedure(id)&&(workflowOverview||doc.CurrentProcedure.GroupOwner is null||doc.RootProcedureId(doc.CurrentProcedureId)!=id);

    void SetProcedureInterval(string id)
    {
        if(!CanEditProcedure(id))return;
        var p=doc.Document.Procedures.First(p=>p.Id==id);
        var dialog=new WorkflowRunIntervalWindow(p.ContinuousRunIntervalMs){Owner=this};
        ResetEditingGesture();parameterEditorOpen=true;
        bool applied;try{applied=dialog.ShowDialog()==true;}finally{parameterEditorOpen=false;}
        if(applied&&CanEditProcedure(id))doc.SetProcedureRunPolicy(id,dialog.IntervalMs,p.StopOnNg);
    }

    void SetProcedureWaitTime(string id)
    {
        if(!CanEditProcedure(id))return;
        var p=doc.Document.Procedures.First(p=>p.Id==id);
        var dialog=new WorkflowWaitTimeWindow(p.WaitTimeMs){Owner=this};
        ResetEditingGesture();parameterEditorOpen=true;
        bool applied;try{applied=dialog.ShowDialog()==true;}finally{parameterEditorOpen=false;}
        if(applied&&CanEditProcedure(id))doc.SetProcedureWaitTime(id,dialog.WaitTimeMs);
    }

    // VmModuleBaseDecorator.DeleteItemCommandExecute(bool isShowWarn).
    void DeleteProcedure(string id, bool isShowWarn)
    {
        if (!CanEditProcedure(id)) return;
        if (isShowWarn && System.Windows.MessageBox.Show(this,"确认删除？","",MessageBoxButton.YesNo,MessageBoxImage.Question)!=MessageBoxResult.Yes) return;
        selected = null;
        doc.DeleteProcedure(id);
    }

    // HeadReNameTextBox commits on Enter or lost focus, using an explicit source update.
    void RenameProcedure(string id)
    {
        if (!CanEditProcedure(id) || !procedureHeaders.TryGetValue(id, out var holder)) return;
        var procedure = doc.Document.Procedures.First(p => p.Id == id);
        var editor = new TextBox { Text = procedure.Name, MaxLength = 64, MinWidth = 65, MaxWidth = 240, FontSize = 14, Foreground = Brushes.White, Background = new SolidColorBrush(Color.FromRgb(40, 42, 44)), BorderThickness = new Thickness(0), ContextMenu = null };
        bool finished = false;
        void Commit()
        {
            if (finished) return;
            finished = true;
            string name = editor.Text.Trim();
            workflowHeaderStates.Remove(id);
            if (CanEditProcedure(id)) Safe(() => doc.RenameProcedure(id, name));
            RefreshAll();
        }
        editor.LostKeyboardFocus += (_, _) => Commit();
        editor.KeyDown += (_, e) => { if (e.Key == Key.Return) { e.Handled = true; Commit(); } };
        holder.Child = editor;
        Dispatcher.BeginInvoke(new Action(() => { editor.Focus(); editor.SelectAll(); }));
    }
}
