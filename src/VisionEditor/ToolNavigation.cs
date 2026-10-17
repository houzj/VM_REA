using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Button=System.Windows.Controls.Button;
using Image=System.Windows.Controls.Image;
using Color=System.Windows.Media.Color;
using Brushes=System.Windows.Media.Brushes;
using Orientation=System.Windows.Controls.Orientation;
using Cursors=System.Windows.Input.Cursors;
using DragEventArgs=System.Windows.DragEventArgs;
using DataObject=System.Windows.DataObject;
using DragDropEffects=System.Windows.DragDropEffects;
using HorizontalAlignment=System.Windows.HorizontalAlignment;
using Panel=System.Windows.Controls.Panel;
using Point=System.Windows.Point;

namespace VisionEditor;

public partial class MainWindow
{
    sealed record CatalogTool(string Id,string Name,double Priority,string DisplayType,string? Icon,string? ModuleId);
    sealed record CatalogGroup(string Id,string Name,double Priority,string? Icon,string? HoverIcon,CatalogTool[] Items);
    sealed record CatalogFile(int Schema,CatalogGroup[] Groups);
    CatalogGroup[]? toolGroups;
    Border? toolDragGhost;
    bool? toolRailOverview;
    bool EditableOverview=>workflowOverview&&!windowClosed&&!parameterEditorOpen&&!isLocked&&activeRun is null;
    CatalogGroup[] ToolGroups => toolGroups??=JsonSerializer.Deserialize<CatalogFile>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"tool-catalog.json")))!.Groups;

    ImageSource? CatalogIcon(string? file) => string.IsNullOrEmpty(file)?null:Asset(file);

    void BuildRail()
    {
        toolRailOverview=workflowOverview;
        Rail.Children.Clear();
        Rail.Margin=new Thickness(0,toolboxExpanded?4:12,0,0);
        if(workflowOverview)
        {
            // ToolBoxData.GlobalToolName explicitly filters these three global tools.
            foreach(var entry in new[]{("ProcedureModule","流程"),("GlobalCameraModule","全局相机"),("DataQueueModule","数据队列")})
            {
                string id=entry.Item1;bool available=id=="ProcedureModule";
                var picture=new Image {Source=Asset(id+"ImageEnable.png"),Width=52,Height=50,Stretch=Stretch.Fill};
                var button=new Button {Width=52,Height=50,Content=picture,ToolTip=available?entry.Item2:entry.Item2+"：独立插件尚未实现",IsEnabled=available,Opacity=available?1:.45,Tag=id,Template=PlainTemplate(),Cursor=Cursors.SizeAll};
                ToolTipService.SetShowOnDisabled(button,true);
                button.MouseEnter+=(_,_)=>picture.Source=Asset(id+"ImageHover.png");button.MouseLeave+=(_,_)=>picture.Source=Asset(id+"ImageEnable.png");
                AttachToolDrag(button,"__procedure",()=>available&&EditableOverview);
                Rail.Children.Add(button);
            }
            return;
        }
        foreach(var group in ToolGroups)
        {
            if(toolboxExpanded)
            {
                var expander=new Expander { Header=group.Name, Width=230, Style=(Style)FindResource("ToolboxCategory") };
                // Source groups tools by GroupName, preserving priority order within each group.
                expander.Content=CreateToolGrid(group);
                expander.Expanded+=(_,_)=>activeToolCategory=group.Id;
                Rail.Children.Add(expander);
                continue;
            }
            var icon=new Image { Source=CatalogIcon(group.Icon), Width=52, Height=50, Stretch=Stretch.Uniform };
            var button=new Button { Width=52, Height=50, Background=Brushes.Transparent, BorderThickness=new Thickness(0), ToolTip=group.Name, Content=icon, Tag=group.Id, Padding=new Thickness(0) };
            button.Style=(Style)FindResource("ToolboxCategoryButton");
            button.MouseEnter+=(_,_)=> { icon.Source=CatalogIcon(group.HoverIcon??group.Icon);ShowToolPicker(group.Id,button); };
            button.MouseLeave+=(_,_)=>CloseToolPopupSoon();
            Rail.Children.Add(button);
        }
    }

    UniformGrid CreateToolGrid(CatalogGroup group)
    {
        var grid=new UniformGrid { Columns=3, Margin=new Thickness(5,2,5,0) };
        foreach(var tool in group.Items)grid.Children.Add(CreateCatalogTile(tool));
        return grid;
    }

    Button CreateCatalogTile(CatalogTool tool)
    {
        bool available=tool.ModuleId is not null&&modules.Descriptors.Any(d=>d.Id==tool.ModuleId);
        var tile=new Button { Width=68, Height=90, Background=Brushes.Transparent, BorderThickness=new Thickness(0), Padding=new Thickness(0), IsEnabled=available, ToolTip=available?tool.Name:tool.Name+"：独立插件尚未实现", Cursor=Cursors.SizeAll, Tag=tool.Id };
        var content=new Grid { Margin=new Thickness(0,7,0,0),VerticalAlignment=VerticalAlignment.Top };
        content.RowDefinitions.Add(new RowDefinition {Height=GridLength.Auto});content.RowDefinitions.Add(new RowDefinition {Height=new GridLength(4)});content.RowDefinitions.Add(new RowDefinition());
        var hover=new Border {Width=64,CornerRadius=new CornerRadius(2),BorderThickness=new Thickness(1),BorderBrush=new SolidColorBrush(Color.FromRgb(23,23,22)),Background=new LinearGradientBrush(Color.FromArgb(77,255,255,255),Color.FromArgb(77,168,168,168),90),Visibility=Visibility.Collapsed,IsHitTestVisible=false,Effect=new System.Windows.Media.Effects.DropShadowEffect {BlurRadius=2,Direction=270,ShadowDepth=3,Opacity=.35}};
        Grid.SetRowSpan(hover,3);content.Children.Add(hover);
        content.Children.Add(new Image { Source=CatalogIcon(tool.Icon), Width=36, Height=36, Margin=new Thickness(0,5,0,0) });
        var label=new TextBlock { Text=tool.Name, Height=35, FontSize=12, Foreground=new SolidColorBrush(Color.FromRgb(190,190,190)), TextAlignment=TextAlignment.Center, TextWrapping=TextWrapping.Wrap, TextTrimming=TextTrimming.CharacterEllipsis };Grid.SetRow(label,2);content.Children.Add(label);
        tile.Content=content;
        tile.Opacity=available?1:.45;
        ToolTipService.SetShowOnDisabled(tile,true);
        tile.Template=PlainTemplate();
        tile.MouseEnter+=(_,_)=>hover.Visibility=Visibility.Visible;tile.MouseLeave+=(_,_)=>hover.Visibility=Visibility.Collapsed;
        AttachToolDrag(tile,tool.ModuleId??"",()=>available&&EditableCanvas);
        // ToolItemGrid initiates a drag on pointer movement; a click alone does not add a node.
        return tile;
    }

    void AttachToolDrag(Button tile,string id,Func<bool> enabled)
    {
        bool armed=false;
        tile.PreviewMouseLeftButtonDown+=(_,e)=> { armed=true;toolDragStarted=false; };
        tile.PreviewMouseLeftButtonUp+=(_,_)=>armed=false;
        tile.LostMouseCapture+=(_,_)=>armed=false;
        tile.PreviewMouseMove+=(_,e)=>
        {
            if(!armed)return;
            armed=false; // ToolItemGrid consumes the first movement after mouse-down.
            if(!enabled()||e.LeftButton!=MouseButtonState.Pressed||toolDragStarted)return;
            toolDragStarted=true;
            if(toolPopup is not null)toolPopup.IsOpen=false;
            var data=CreateToolDragData(id);
            System.Windows.QueryContinueDragEventHandler query=(_,args)=>{if(ReadDragPayload(data,ToolDragFormat) is not ToolDragPayload payload||!ValidToolDrag(payload))args.Action=System.Windows.DragAction.Cancel;};
            tile.QueryContinueDrag+=query;
            try { DragDrop.DoDragDrop(tile,data,DragDropEffects.Move); }
            catch(Exception error){if(!windowClosed)StatusText.Text="工具拖动已取消："+error.Message;}
            finally { tile.QueryContinueDrag-=query;ClearToolGhost();ReleaseToolDragData();toolDragStarted=false; }
        };
    }

    void ShowToolPicker(string category="all",FrameworkElement? anchor=null)
    {
        if(toolboxExpanded||workflowOverview||!EditableCanvas)return;
        var group=ToolGroups.FirstOrDefault(g=>g.Id==category)??ToolGroups.First();
        if(toolPopup is {IsOpen:true}&&toolPopupAnchor==anchor)return;
        if(toolPopup is not null)toolPopup.IsOpen=false;
        activeToolCategory=group.Id;
        toolPopupPointerInside=false;
        var content=new Grid { Margin=new Thickness(0,10,0,10) };
        content.RowDefinitions.Add(new RowDefinition {Height=GridLength.Auto});content.RowDefinitions.Add(new RowDefinition());
        var header=new Grid {Margin=new Thickness(12,0,0,0)};
        header.Children.Add(new Border {Width=2,Height=15,Background=Brushes.LightGray,HorizontalAlignment=HorizontalAlignment.Left,Margin=new Thickness(0,2,0,0)});
        header.Children.Add(new TextBlock {Text=group.Name,FontSize=16,Foreground=Brushes.LightGray,Margin=new Thickness(10,0,0,0),VerticalAlignment=VerticalAlignment.Center});
        content.Children.Add(header);
        var scroll=new ScrollViewer { Content=CreateToolGrid(group), MaxHeight=Math.Max(0,ToolRail.ActualHeight-80), Style=(Style)FindResource("RecScrollViewer"), VerticalScrollBarVisibility=ScrollBarVisibility.Auto, HorizontalScrollBarVisibility=ScrollBarVisibility.Disabled };
        Grid.SetRow(scroll,1);content.Children.Add(scroll);
        var border=new Border { Child=content, Margin=new Thickness(6,8,8,8), Background=new LinearGradientBrush(Color.FromRgb(37,37,37),Color.FromRgb(50,50,50),90), BorderBrush=new SolidColorBrush(Color.FromRgb(23,23,22)), BorderThickness=new Thickness(2), CornerRadius=new CornerRadius(4), Effect=new System.Windows.Media.Effects.DropShadowEffect {BlurRadius=7,Direction=305,ShadowDepth=3,Opacity=.39} };
        var popupRoot=new Grid {Width=235,Background=new SolidColorBrush(Color.FromArgb(2,255,255,255))};popupRoot.Children.Add(border);
        toolPopupAnchor=anchor;
        var popup=new Popup { Child=popupRoot, PlacementTarget=anchor??Rail, Placement=PlacementMode.Right, StaysOpen=true, AllowsTransparency=true, MaxHeight=ToolRail.ActualHeight };
        toolPopup=popup;
        popupRoot.MouseEnter+=(_,_)=>{if(ReferenceEquals(toolPopup,popup))toolPopupPointerInside=true;};
        popupRoot.MouseLeave+=(_,_)=> {if(!ReferenceEquals(toolPopup,popup))return;toolPopupPointerInside=false;CloseToolPopupSoon();};
        popup.Closed+=(_,_)=>{if(ReferenceEquals(toolPopup,popup))toolPopupPointerInside=false;UpdateRailHighlights();};
        toolPopup.IsOpen=true;
        UpdateRailHighlights();
    }

    void UpdateRailHighlights()
    {
        if(workflowOverview)return;
        foreach(var button in Rail.Children.OfType<Button>())
        {
            var group=ToolGroups.First(g=>g.Id==(string)button.Tag);
            bool active=button.IsMouseOver||(toolPopup is {IsOpen:true}&&(string)button.Tag==activeToolCategory);
            if(button.Content is Image image)image.Source=CatalogIcon(active?group.HoverIcon??group.Icon:group.Icon);
            button.SetValue(Selector.IsSelectedProperty,active);
        }
    }

    void CloseToolPopupSoon()
    {
        var popup=toolPopup;var anchor=toolPopupAnchor;
        // ToolBox's IsMouseLeave/IsMouseOver trigger has no timed grace period.
        // Defer only to let WPF deliver the corresponding Popup MouseEnter event.
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input,new Action(()=>{
            if(popup!=toolPopup||toolPopupPointerInside||anchor?.IsMouseOver==true)return;
            if(popup is not null)popup.IsOpen=false;
            UpdateRailHighlights();
        }));
    }

    bool ValidToolDrag(ToolDragPayload payload)=>ReferenceEquals(payload.Owner,this)&&payload.Solution==doc.SolutionId&&payload.Procedure==doc.CurrentProcedureId&&payload.Revision==doc.Revision&&payload.Overview==workflowOverview&&(payload.Module=="__procedure"?EditableOverview:EditableCanvas);

    string? DragModule(DragEventArgs e)
    {
        if(ReadDragPayload(e.Data,ToolDragFormat) is not ToolDragPayload payload||!ValidToolDrag(payload))return null;
        if(payload.Module=="__procedure")return EditableOverview?payload.Module:null;
        if(!EditableCanvas)return null;
        return modules.Descriptors.Any(d=>d.Id==payload.Module)?payload.Module:null;
    }

    void ToolDropOver(object sender,DragEventArgs e)
    {
        string? id=DragModule(e);
        if(!IsCanvasDropPoint(e.GetPosition(CanvasViewport)))id=null;
        e.Effects=id is null||!e.AllowedEffects.HasFlag(DragDropEffects.Move)?DragDropEffects.None:DragDropEffects.Move;
        if(e.Effects==DragDropEffects.None){ClearToolGhost();e.Handled=true;return;}
        if(id=="__procedure")
        {
            if(toolDragGhost is null){var card=CreateWorkflowOverviewCard(new VisionFramework.ProcedureDocument("tool-preview",NextProcedureName(),[],[],[]),true);toolDragGhost=new Border {Width=140,Height=120,Opacity=.7,IsHitTestVisible=false,Child=card};Panel.SetZIndex(toolDragGhost,int.MaxValue);GraphLayer.Children.Add(toolDragGhost);}
            var p=e.GetPosition(GraphLayer);Canvas.SetLeft(toolDragGhost,Math.Clamp(Snap(p.X-70),0,DiagramPageSize-140));Canvas.SetTop(toolDragGhost,Math.Clamp(Snap(p.Y-60),0,DiagramPageSize-120));e.Handled=true;return;
        }
        if(id is not null)
        {
            if(toolDragGhost is null)
            {
                var descriptor=modules.Descriptors.First(d=>d.Id==id);
                var preview=CreateNodeCard(new VisionFramework.NodeDocument("tool-preview",id,descriptor.Version,descriptor.DisplayName,Defaults(descriptor),0,0),doc.CurrentProcedure.Nodes.Count,false);
                var body=(Border)preview.Children[0];preview.Children.Remove(body);
                toolDragGhost=new Border {Width=body.Width,Height=body.Height,Opacity=.7,IsHitTestVisible=false,Child=body};
                Panel.SetZIndex(toolDragGhost,int.MaxValue);GraphLayer.Children.Add(toolDragGhost);
            }
            var point=e.GetPosition(GraphLayer);
            Canvas.SetLeft(toolDragGhost,Math.Clamp(Math.Round((point.X-toolDragGhost.Width/2)/5)*5,0,DiagramPageSize-toolDragGhost.Width));
            Canvas.SetTop(toolDragGhost,Math.Clamp(Math.Round((point.Y-toolDragGhost.Height/2)/5)*5,0,DiagramPageSize-toolDragGhost.Height));
            DrawAlignmentGuides(Canvas.GetLeft(toolDragGhost),Canvas.GetTop(toolDragGhost));
            UpdateNodeHotRegion(Canvas.GetLeft(toolDragGhost),Canvas.GetTop(toolDragGhost),[]);
            UpdateInsertionWire(point,[]);
        }
        e.Handled=true;
    }

    void ToolDrop(object sender,DragEventArgs e)
    {
        string? id=DragModule(e);if(id is null||!IsCanvasDropPoint(e.GetPosition(CanvasViewport))||!e.AllowedEffects.HasFlag(DragDropEffects.Move)){e.Effects=DragDropEffects.None;ClearToolGhost();e.Handled=true;return;}
        var point=e.GetPosition(GraphLayer);
        if(id=="__procedure")
        {
            double px=toolDragGhost is null?Snap(point.X-70):Canvas.GetLeft(toolDragGhost),py=toolDragGhost is null?Snap(point.Y-60):Canvas.GetTop(toolDragGhost);ClearToolGhost();
            string name=NextProcedureName();doc.AddProcedure(name,Math.Clamp(px,0,DiagramPageSize-140),Math.Clamp(py,0,DiagramPageSize-120));e.Effects=DragDropEffects.Move;e.Handled=true;return;
        }
        double x=toolDragGhost is null?Math.Max(0,Math.Round((point.X-55)/5)*5):Canvas.GetLeft(toolDragGhost);
        double height=id==VisionFramework.WorkflowCalls.ModuleId?40:NodeHeight;
        double y=toolDragGhost is null?Math.Max(0,Math.Round((point.Y-height/2)/5)*5):Canvas.GetTop(toolDragGhost);
        var target=ActiveNodeHotPoint??new Point(x,y);string? predecessor=AvailableHotPredecessor();
        var insertion=FindInsertionWire(point,[]);
        ClearToolGhost();Safe(()=>DropToolModule(id,Math.Clamp(target.X,0,DiagramPageSize-NodeWidth),Math.Clamp(target.Y,0,DiagramPageSize-height),insertion,predecessor));e.Handled=true;
    }

    void ClearToolGhost(){ClearAlignmentGuides();ClearNodeHotRegion();ClearInsertionWire();if(toolDragGhost is not null)GraphLayer.Children.Remove(toolDragGhost);toolDragGhost=null;}
}
