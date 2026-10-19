using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using VisionEditor.Core.VmRouting;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program
{
    [STAThread]
    static int Main()
    {
        try { Run(); return 0; }
        catch(Exception error) { Console.Error.WriteLine(error); return 1; }
    }

    static void Run()
    {
        var application=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
        var window=new MainWindow {ShowActivated=false,ShowInTaskbar=false,WindowStartupLocation=WindowStartupLocation.Manual,Left=-20000,Top=-20000};
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
        void Pump() {window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
        void Check(bool condition,string description) {if(!condition)throw new Exception(description);Console.WriteLine("PASS: "+description);}
        void Capture(string name)
        {
            window.UpdateLayout();
            var bitmap=new RenderTargetBitmap((int)Math.Ceiling(window.ActualWidth),(int)Math.Ceiling(window.ActualHeight),96,96,PixelFormats.Pbgra32);
            bitmap.Render(window);
            var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));
            string directory=Path.Combine("analysis","full-audit");Directory.CreateDirectory(directory);
            using var stream=File.Create(Path.Combine(directory,name));encoder.Save(stream);
        }
        object? Invoke(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(window,args);
        void ClickLayout(string section,string command)
        {
            var menu=(ContextMenu)Invoke("GroupSelectionMenu")!;
            var parent=menu.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,section));
            var entry=parent.Items.OfType<MenuItem>().Single(item=>Equals(item.Header,command));
            if(!entry.IsEnabled)throw new Exception("Disabled layout command "+command+"; editable="+typeof(MainWindow).GetProperty("EditableCanvas",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window));
            entry.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Pump();
        }
        var doc=(GraphEditorDocument)typeof(MainWindow).GetField("doc",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
        var selected=(HashSet<string>)typeof(MainWindow).GetField("selectedNodes",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
        try
        {
            window.Show();Pump();doc.NewSolution();
            string ordinary=doc.AddNode("vision.image-source","1.0.0",200,100);
            string group=doc.AddNode(WorkflowCalls.ModuleId,WorkflowCalls.Version,400,180);
            selected.Clear();selected.Add(ordinary);selected.Add(group);Pump();
            long revision=doc.Revision;
            ClickLayout("对齐","底部对齐");
            var bottomNodes=doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).ToArray();
            Check(bottomNodes.Single(n=>n.Id==ordinary).Y+30==bottomNodes.Single(n=>n.Id==group).Y+40,$"mixed-height nodes share a bottom edge: {bottomNodes.Single(n=>n.Id==ordinary).Y+30} / {bottomNodes.Single(n=>n.Id==group).Y+40}, revision {doc.Revision}");
            Capture("canvas-mixed-height-bottom-align.png");
            Check(doc.Revision==revision+1,"bottom alignment is one undo transaction");
            doc.Undo();Pump();
            Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==ordinary).Y==100&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==group).Y==180,"undo restores both nodes");
            ClickLayout("对齐","垂直居中");
            var centered=doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).ToArray();
            Check(centered.Single(n=>n.Id==ordinary).Y+15==centered.Single(n=>n.Id==group).Y+20,"mixed-height nodes share a vertical center");
            doc.Undo();Pump();
            ClickLayout("分布","水平分布");
            var horizontal=doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).ToArray();
            Check(horizontal.Single(n=>n.Id==ordinary).Y+15==horizontal.Single(n=>n.Id==group).Y+20,"horizontal distribution centers actual item heights");
            doc.Undo();Pump();
            string third=doc.AddNode("vision.image-source","1.0.0",600,440);selected.Add(third);Pump();
            ClickLayout("分布","垂直分布");
            var vertical=doc.CurrentProcedure.Nodes.Where(n=>selected.Contains(n.Id)).OrderBy(n=>n.Y).ToArray();
            double Height(NodeDocument node)=>node.ModuleId==WorkflowCalls.ModuleId?40:30;
            var gaps=vertical.Zip(vertical.Skip(1),(a,b)=>b.Y-a.Y-Height(a)).ToArray();
            Check(gaps.All(g=>g>=0)&&Math.Abs(gaps[0]-gaps[1])<=5,"vertical distribution leaves equal non-overlapping gaps around mixed heights");
            string file=Path.Combine(Path.GetTempPath(),"vision-canvas-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(file);var positions=vertical.ToDictionary(n=>n.Id,n=>(n.X,n.Y));doc.Load(file);Check(doc.CurrentProcedure.Nodes.Where(n=>positions.ContainsKey(n.Id)).All(n=>positions[n.Id]==(n.X,n.Y)),"distributed positions survive save and reload");}
            finally {if(File.Exists(file))File.Delete(file);}

            doc.NewSolution();
            string source=doc.AddNode("vision.image-source","1.0.0",200,100);
            string target=doc.AddNode("vision.image-source","1.0.0",450,280);
            // Native Connector.AttachedDiagramItem_PreviewMouseMove calls
            // PathFinder.GetConnectionLine(start item, pointer in ItemsPage, port index)
            // before HitTesting/SetNearestConnector. Exercise the local WPF gesture
            // entry and its visible preview geometry for the same legal bottom-to-top
            // port pair; this checks the canvas adapter as well as the router.
            revision=doc.Revision;
            Invoke("BeginConnection",source,3);
            Invoke("AdvanceConnectionPreview",new Point(505,280),MouseButtonState.Pressed);Pump();
            var preview=(System.Windows.Shapes.Path)typeof(MainWindow).GetField("connectionPreview",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            var firstPreview=preview;
            var expectedPreview=VmOrthogonalRouter.Preview(new RoutePort(new RoutePoint(255,130),PortDirection.Down,new RouteRect(200,100,110,30)),new RoutePoint(505,280));
            Check(preview.Data.ToString()==typeof(MainWindow).GetMethod("LineGeometry",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,[expectedPreview])!.ToString(),"connector drag preview draws the native GetConnectionLine route for bottom port 3 at ItemsPage point 505,280");
            var previewTarget=typeof(MainWindow).GetField("connectionTarget",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
            Check(previewTarget is ValueTuple<string,int> hit&&hit.Item1==target&&hit.Item2==1,"connector drag preview hit-tests the target top port without committing the link");
            Check(doc.Revision==revision&&doc.CurrentProcedure.ExecutionLinks!.Count==0,"connector preview is transient and does not write workflow data");
            Capture("canvas-connector-drag-preview.png");
            Invoke("FinishConnection",new Point(505,280));Pump();
            var connected=doc.CurrentProcedure.ExecutionLinks!.Single();
            Check(doc.Revision==revision+1&&connected.FromNode==source&&connected.ToNode==target&&connected.SourceIndex==3&&connected.TargetIndex==1,"connector release commits the previewed bottom-to-top port pair as one document edit");
            Check(typeof(MainWindow).GetField("connectionPreview",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window) is null,"connector release removes and clears its temporary adorner state");
            doc.Undo();Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Count==0,"undo removes the committed connector drag link");
            Invoke("BeginConnection",source,3);
            Invoke("AdvanceConnectionPreview",new Point(505,280),MouseButtonState.Pressed);Pump();
            var secondPreview=(System.Windows.Shapes.Path)typeof(MainWindow).GetField("connectionPreview",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Check(!ReferenceEquals(firstPreview,secondPreview)&&ReferenceEquals(secondPreview.Parent,window.FindName("GraphLayer")),"a second connector drag creates and attaches a fresh preview after first-drag cleanup");
            Invoke("CancelCanvasGesture");Pump();

            doc.NewSolution();
            source=doc.AddNode("vision.image-source","1.0.0",200,100);
            target=doc.AddNode("vision.image-source","1.0.0",450,280);
            doc.ConnectExecution(source,target,3,1);
            var link=doc.CurrentProcedure.ExecutionLinks!.Single();
            doc.SetExecutionPath(link,[new(255,130),new(255,205),new(505,205),new(505,280)]);Pump();
            string key=source+">"+target;
            var canvas=(Canvas)window.FindName("GraphLayer");
            var arrows=(Dictionary<string,System.Windows.Shapes.Polygon>)typeof(MainWindow).GetField("wireArrows",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            var hitPaths=(Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            string before=hitPaths[key].Data.ToString();
            revision=doc.Revision;
            Invoke("BeginWireDrag",link,1);
            Invoke("PreviewWireDrag",new Point(350,235));Pump();
            var visible=canvas.Children.OfType<System.Windows.Shapes.Path>().Single(p=>!p.IsHitTestVisible&&p.StrokeThickness==2);
            Check(ReferenceEquals(hitPaths[key].Data,visible.Data)&&hitPaths[key].Data.ToString()!=before,"midpoint preview updates visible route and its interactive hit geometry together");
            var drag=typeof(MainWindow).GetField("wireDrag",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            var dragPoints=(IReadOnlyList<Point>)drag.GetType().GetProperty("PathPoints")!.GetValue(drag)!;
            var displayed=VmOrthogonalRouter.Passivate(dragPoints.Select(p=>new RoutePoint(p.X,p.Y)).ToArray());
            var expected=(PointCollection)typeof(MainWindow).GetMethod("OriginalWireArrow",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,[displayed])!;
            Check(arrows[key].Points.SequenceEqual(expected),"midpoint preview redraws arrow from displayed route as native SetPath does");
            Check(doc.Revision==revision,"preview leaves saved route and undo history untouched");
            Capture("canvas-wire-midpoint-preview.png");
            Invoke("FinishWireDrag",false);Pump();
            Check(doc.Revision==revision&&hitPaths[key].Data.ToString()==before,"cancel restores path, arrow and hit geometry from saved route");
            Invoke("BeginWireDrag",link,1);Invoke("PreviewWireDrag",new Point(350,235));Invoke("FinishWireDrag",true);Pump();
            Check(doc.Revision==revision+1&&doc.CurrentProcedure.ExecutionLinks!.Single().PathPoints![1].Y==235,"commit records moved midpoint in one undo transaction");
            doc.Undo();Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Single().PathPoints![1].Y==205,"undo restores saved midpoint");
            revision=doc.Revision;

            var mini=(Canvas)window.FindName("MiniMapCanvas");
            var region=(System.Windows.Shapes.Rectangle)window.FindName("MiniMapViewport");
            var scroll=(ScrollViewer)window.FindName("CanvasScrollViewer");
            mini.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.MouseDownEvent});Pump();
            Check((bool)typeof(MainWindow).GetField("eyeDragging",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!,"actual mini-map mouse-down routed event starts dragging");
            mini.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.MouseUpEvent});Pump();
            Check(!(bool)typeof(MainWindow).GetField("eyeDragging",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!,"actual mini-map mouse-up routed event ends dragging");
            scroll.ScrollToHorizontalOffset(0);scroll.ScrollToVerticalOffset(0);Pump();
            var edge=new Point(186,149);
            double regionWidth=region.Width,regionHeight=region.Height;
            Invoke("BeginMiniMapDrag",edge);Pump();
            double requestedX=edge.X-regionWidth/2,requestedY=edge.Y-regionHeight/2;
            var recorded=(Point)typeof(MainWindow).GetField("eyeRegionStart",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Check(Math.Abs(recorded.X-requestedX)<.001&&Math.Abs(recorded.Y-requestedY)<.001,$"edge click remembers original requested view location before scroll clamping: {recorded} expected {requestedX},{requestedY}");
            double clampedX=Canvas.GetLeft(region),clampedY=Canvas.GetTop(region);
            Check(clampedX<requestedX&&clampedY<requestedY,"scroll viewer clamps the clicked viewport at bottom-right extent");
            Invoke("MoveMiniMapDrag",new Point(edge.X-5,edge.Y-5));Pump();
            Check(Math.Abs(Canvas.GetLeft(region)-clampedX)<.001&&Math.Abs(Canvas.GetTop(region)-clampedY)<.001,"short reverse drag preserves native requested-position hysteresis at edge");
            Invoke("MoveMiniMapDrag",new Point(edge.X-35,edge.Y-35));Pump();
            Check(Canvas.GetLeft(region)<clampedX&&Canvas.GetTop(region)<clampedY,"long reverse drag moves the viewport after leaving clamped edge");
            Capture("canvas-minimap-edge-drag.png");
            mini.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonUpEvent});Pump();
            Check(doc.Revision==revision,"mini-map navigation does not edit workflow model or undo history");
            string mapFile=Path.Combine(Path.GetTempPath(),"vision-canvas-map-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(mapFile);doc.Load(mapFile);Check(doc.CurrentProcedure.Nodes.Count==2&&doc.CurrentProcedure.ExecutionLinks!.Count==1,"mini-map navigation leaves workflow data intact through save and reopen");}
            finally {if(File.Exists(mapFile))File.Delete(mapFile);}

            doc.NewSolution();
            string focusSource=doc.AddNode("vision.image-source","1.0.0",1000,1000);
            string focusTarget=doc.AddNode("vision.image-source","1.0.0",1350,1250);
            doc.ConnectExecution(focusSource,focusTarget,3,1);Pump();
            scroll.ScrollToHorizontalOffset(900);scroll.ScrollToVerticalOffset(900);Pump();
            double focusX=scroll.HorizontalOffset,focusY=scroll.VerticalOffset;
            Check(focusX>0&&focusY>0,"wire focus scenario starts with nonzero canvas scroll offsets");
            string focusKey=focusSource+">"+focusTarget;
            var focusHit=((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[focusKey];
            focusHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
            var handles=(List<System.Windows.Shapes.Rectangle>)typeof(MainWindow).GetField("wireHandles",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Check(focusHit.IsKeyboardFocused&&handles.Count>0,"actual wire mouse-down focuses the connection and exposes midpoint handles");
            Check(Math.Abs(scroll.HorizontalOffset-focusX)<.001&&Math.Abs(scroll.VerticalOffset-focusY)<.001,"focusing the connection does not auto-scroll the viewport");
            Capture("canvas-wire-focus-no-scroll.png");
            ((TabControl)window.FindName("ProcedureTabs")).Focus();Pump();
            Check(handles.Count==0,"connection lost focus removes midpoint handles");
            focusHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
            revision=doc.Revision;
            var delete=new KeyEventArgs(Keyboard.PrimaryDevice,PresentationSource.FromVisual(window)!,0,Key.Delete){RoutedEvent=Keyboard.PreviewKeyDownEvent};
            window.RaiseEvent(delete);Pump();
            Check(delete.Handled&&doc.CurrentProcedure.ExecutionLinks!.Count==0&&doc.Revision==revision+1,"focused wire Delete removes only its link in one undo transaction");
            doc.Undo();Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Count==1,"undo restores selected execution connection");
            string focusFile=Path.Combine(Path.GetTempPath(),"vision-canvas-focus-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(focusFile);doc.Load(focusFile);Check(doc.CurrentProcedure.ExecutionLinks!.Single().FromNode==focusSource&&doc.CurrentProcedure.ExecutionLinks!.Single().ToNode==focusTarget,"restored execution link survives save and reload");}
            finally {if(File.Exists(focusFile))File.Delete(focusFile);}

            // ZoomArea.ScrollViewer_MouseMove updates lastDragPoint for each move, even
            // when ViewArea has been clamped at the canvas edge.
            doc.NewSolution();Pump();
            scroll.ScrollToHorizontalOffset(100);scroll.ScrollToVerticalOffset(100);Pump();
            Check(Math.Abs(scroll.HorizontalOffset-100)<1,"pan check begins one hundred pixels from the canvas edge");
            typeof(MainWindow).GetField("pointerStart",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(window,new Point(100,100));
            typeof(MainWindow).GetField("panning",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(window,true);
            revision=doc.Revision;
            Invoke("MoveCanvasPan",new Point(250,100));Pump();
            Check(Math.Abs(scroll.HorizontalOffset)<.001,"Ctrl drag clamps at the left canvas boundary");
            Invoke("MoveCanvasPan",new Point(230,100));Pump();
            Check(Math.Abs(scroll.HorizontalOffset-20)<1,"reverse drag immediately moves twenty pixels from clamped edge using last pointer position");
            typeof(MainWindow).GetField("panning",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(window,false);
            Capture("canvas-pan-edge-reversal.png");
            double beforeZoom=(double)typeof(MainWindow).GetField("scale",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Invoke("MoveCanvasZoomByWheel",0,new Point(180,140));Pump();
            Check((double)typeof(MainWindow).GetField("scale",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)! == beforeZoom,"zero-delta Ctrl wheel preserves native zoom tick");
            Invoke("MoveCanvasZoomByWheel",120,new Point(180,140));Pump();
            Check(Math.Abs((double)typeof(MainWindow).GetField("scale",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!-beforeZoom-.1)<.001,"positive Ctrl wheel advances exactly one native zoom tick");
            Invoke("MoveCanvasZoomByWheel",-120,new Point(180,140));Pump();
            Check(Math.Abs((double)typeof(MainWindow).GetField("scale",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!-beforeZoom)<.001,"reverse Ctrl wheel restores the original tick");
            Check(doc.Revision==revision,"canvas pan and zoom do not edit workflow data or undo history");
            string viewportFile=Path.Combine(Path.GetTempPath(),"vision-canvas-pan-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(viewportFile);doc.Load(viewportFile);Check(doc.CurrentProcedure.Nodes.Count==0,"pan and zoom leave solution data valid after save and reopen");}
            finally {if(File.Exists(viewportFile))File.Delete(viewportFile);}

            doc.NewSolution();Pump();
            string originalFlow=doc.CurrentProcedureId;
            string searchFlow=doc.AddProcedure("定位验证");
            doc.SelectProcedure(searchFlow);
            string searchNode=doc.AddNode("vision.image-source","1.0.0",2500,2100);Pump();
            Invoke("SetCanvasZoom",1.6,new Point(180,140));Pump();
            doc.SelectProcedure(originalFlow);Pump();
            revision=doc.Revision;
            Invoke("ProcedureListClick",window.FindName("ProcedureListButton")!,new RoutedEventArgs());Pump();
            var listPopup=(Popup)typeof(MainWindow).GetField("workflowListPopup",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            var listTree=(TreeView)((Border)listPopup.Child).Child;
            var flowItem=listTree.Items.OfType<TreeViewItem>().Single(item=>(string)item.Tag==searchFlow);
            var nodeItem=flowItem.Items.OfType<TreeViewItem>().Single(item=>(string)item.Tag==searchNode);
            nodeItem.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.PreviewMouseLeftButtonDownEvent});Pump();
            Check(!listPopup.IsOpen&&doc.CurrentProcedureId==searchFlow&&selected.SetEquals([searchNode]),"actual workflow list leaf click opens target flow and selects its node");
            var located=doc.CurrentProcedure.Nodes.Single(n=>n.Id==searchNode);
            double actualZoom=(double)typeof(MainWindow).GetField("scale",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Check(Math.Abs(actualZoom-1.6)<.001,"target workflow restores its own zoom before node search");
            var view=(FrameworkElement)window.FindName("CanvasViewport")!;
            double expectedX=Math.Max(0,located.X-view.ActualWidth*.5+50)*actualZoom;
            double expectedY=Math.Max(0,located.Y-view.ActualHeight*.5+15)*actualZoom;
            Check(Math.Abs(scroll.HorizontalOffset-expectedX)<1&&Math.Abs(scroll.VerticalOffset-expectedY)<1,$"native SearchModel scroll formula positions selected node at {expectedX:0.##},{expectedY:0.##}; actual {scroll.HorizontalOffset:0.##},{scroll.VerticalOffset:0.##}");
            Check(doc.Revision==revision,"workflow list search changes view and selection without editing document");
            Capture("canvas-workflow-list-search.png");
            doc.Undo();Pump();
            Check(doc.CurrentProcedure.Nodes.All(n=>n.Id!=searchNode),"undo after view-only search still targets preceding node-add transaction");
            doc.Redo();Pump();
            string searchFile=Path.Combine(Path.GetTempPath(),"vision-canvas-search-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(searchFile);doc.Load(searchFile);Check(doc.Document.Procedures.Single(p=>p.Id==searchFlow).Nodes.Single(n=>n.Id==searchNode).X==2500,"searched node persists through save and reopen");}
            finally {if(File.Exists(searchFile))File.Delete(searchFile);}

            doc.NewSolution();
            string cornerSource=doc.AddNode("vision.image-source","1.0.0",4300,4200);
            string cornerTarget=doc.AddNode("vision.image-source","1.0.0",4600,4550);
            string cornerObstacle=doc.AddNode("vision.image-source","1.0.0",4450,4380);
            doc.ConnectExecution(cornerSource,cornerTarget,3,1);Pump();
            string cornerKey=cornerSource+">"+cornerTarget;
            var cornerHit=((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[cornerKey];
            var cornerArrow=((Dictionary<string,System.Windows.Shapes.Polygon>)typeof(MainWindow).GetField("wireArrows",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[cornerKey];
            Check(cornerHit.Data is not null&&cornerArrow.Points.Count==3,"lower-right 5000 canvas route renders interactive path and directed arrow beyond former 250k-cell cap");
            scroll.ScrollToHorizontalOffset(4200);scroll.ScrollToVerticalOffset(4150);Pump();
            cornerHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
            Check((string?)typeof(MainWindow).GetField("selectedWire",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)==cornerKey,"lower-right connection remains hover/select hit-testable");
            Capture("canvas-corner-route-selected.png");
            revision=doc.Revision;
            doc.MoveNode(cornerTarget,4700,4700);Pump();
            var movedHit=((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[cornerKey];
            Check(movedHit.Data is not null&&doc.CurrentProcedure.ExecutionLinks!.Single().PathPoints is null&&doc.Revision==revision+1,"moving destination recalculates extreme route in one undo transaction");
            doc.Undo();Pump();
            Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==cornerTarget).Y==4550&&((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!).ContainsKey(cornerKey),"undo restores corner node and visible connection");
            string cornerFile=Path.Combine(Path.GetTempPath(),"vision-canvas-corner-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(cornerFile);doc.Load(cornerFile);Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Single().ToNode==cornerTarget&&((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!).ContainsKey(cornerKey),"extreme route survives save/reopen and redraw");}
            finally {if(File.Exists(cornerFile))File.Delete(cornerFile);}

            doc.NewSolution();
            string routeSource=doc.AddNode("vision.image-source","1.0.0",200,100);
            string routeTarget=doc.AddNode("vision.image-source","1.0.0",500,350);
            doc.AddNode("vision.image-source","1.0.0",320,180);
            doc.ConnectExecution(routeSource,routeTarget,3,1);Pump();
            var routeSourceBox=new RouteRect(200,100,110,30);var routeTargetBox=new RouteRect(500,350,110,30);
            var expectedRoute=VmOrthogonalRouter.Route(new(new(255,130),PortDirection.Down,routeSourceBox),new(new(555,350),PortDirection.Up,routeTargetBox),[new RouteRect(320,180,110,30)],5000,5000,1_100_000);
            var routeGrid=(Array)typeof(VmOrthogonalRouter).GetField("cachedMap",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;
            Check(routeGrid.GetLength(0)==1032&&routeGrid.GetLength(1)==1032,"actual canvas route retains one native 1032-by-1032 cached APoint grid");
            var routeKey=routeSource+">"+routeTarget;
            var routeHit=((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[routeKey];
            var expectedGeometry=new PathGeometry();var expectedFigure=new PathFigure{StartPoint=new(expectedRoute.Points[0].X,expectedRoute.Points[0].Y)};
            foreach(var point in expectedRoute.Points.Skip(1))expectedFigure.Segments.Add(new LineSegment(new(point.X,point.Y),true));expectedGeometry.Figures.Add(expectedFigure);
            Check(routeHit.Data.ToString()==expectedGeometry.ToString(),"real WPF edge display uses the native fixed 5000x5000 DiagramPage routing grid");
            Capture("canvas-fixed-native-route-grid.png");

            doc.NewSolution();
            string stableSource=doc.AddNode("vision.image-source","1.0.0",100,100);
            string stableTarget=doc.AddNode("vision.image-source","1.0.0",500,350);
            string movingObstacle=doc.AddNode("vision.image-source","1.0.0",900,900);
            doc.ConnectExecution(stableSource,stableTarget,3,1);Pump();
            scroll.ScrollToHorizontalOffset(0);scroll.ScrollToVerticalOffset(0);Pump();
            string stableKey=stableSource+">"+stableTarget;
            var automatic=(System.Collections.IDictionary)typeof(MainWindow).GetField("automaticRoutes",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            var cached=automatic[(doc.CurrentProcedureId,stableSource,stableTarget)]!;
            var originalPoints=(IReadOnlyList<RoutePoint>)cached.GetType().GetField("Item3")!.GetValue(cached)!;
            var longest=originalPoints.Zip(originalPoints.Skip(1),(a,b)=>(A:a,B:b,Length:Math.Abs(a.X-b.X)+Math.Abs(a.Y-b.Y))).OrderByDescending(p=>p.Length).First();
            double obstacleX=Math.Clamp(Math.Round(((longest.A.X+longest.B.X)/2-55)/5)*5,0,4800);
            double obstacleY=Math.Clamp(Math.Round(((longest.A.Y+longest.B.Y)/2-15)/5)*5,0,4800);
            string BeforePath()=>((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[stableKey].Data.ToString();
            string originalPath=BeforePath();revision=doc.Revision;
            doc.MoveNode(movingObstacle,obstacleX,obstacleY);Pump();
            var sourceBox=new RouteRect(100,100,110,30);var targetBox=new RouteRect(500,350,110,30);
            // Match the native DiagramPage's 1032x1032 map (1,065,024 cells);
            // the router's general-purpose default budget is intentionally smaller.
            var wouldReroute=VmOrthogonalRouter.Route(new(new(155,130),PortDirection.Down,sourceBox),new(new(555,350),PortDirection.Up,targetBox),[new RouteRect(obstacleX,obstacleY,110,30)],5000,5000,1_100_000);
            bool intersectsLongest=longest.A.X==longest.B.X
                ? longest.A.X>=obstacleX&&longest.A.X<=obstacleX+110&&Math.Max(Math.Min(longest.A.Y,longest.B.Y),obstacleY)<=Math.Min(Math.Max(longest.A.Y,longest.B.Y),obstacleY+30)
                : longest.A.Y>=obstacleY&&longest.A.Y<=obstacleY+30&&Math.Max(Math.Min(longest.A.X,longest.B.X),obstacleX)<=Math.Min(Math.Max(longest.A.X,longest.B.X),obstacleX+110);
            static string FormatRoute(IEnumerable<RoutePoint> points)=>string.Join(" -> ",points.Select(p=>$"({p.X:0.##},{p.Y:0.##})"));
            Console.WriteLine($"DIAG obstacle={obstacleX:0.##},{obstacleY:0.##}..{obstacleX+110:0.##},{obstacleY+30:0.##}; longest=({longest.A.X:0.##},{longest.A.Y:0.##})->({longest.B.X:0.##},{longest.B.Y:0.##}), intersects={intersectsLongest}; oldRoute=[{FormatRoute(originalPoints)}]; newStatus={wouldReroute.Status}; newRoute=[{FormatRoute(wouldReroute.Points)}]");
            Capture("canvas-obstacle-diagnostic-window.png");
            Check(intersectsLongest&&wouldReroute.Points.Count>1&&!wouldReroute.Points.SequenceEqual(originalPoints),"moved obstacle intersects prior route and a fresh native routing call would choose a different geometry");
            Check(BeforePath()==originalPath&&doc.Revision==revision+1,"unrelated obstacle move preserves existing Connection path until its own endpoints change");
            var retainedArrow=((Dictionary<string,System.Windows.Shapes.Polygon>)typeof(MainWindow).GetField("wireArrows",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[stableKey];
            Check(retainedArrow.Points.Count==3,"retained route keeps its directed arrow");
            var retainedHit=((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[stableKey];
            retainedHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=Mouse.PreviewMouseDownEvent});Pump();
            Check((string?)typeof(MainWindow).GetField("selectedWire",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)==stableKey,"retained route remains selectable through real WPF hit path");
            Capture("canvas-obstacle-move-retains-route.png");
            doc.MoveNode(stableTarget,500,400);Pump();
            Check(BeforePath()!=originalPath&&doc.Revision==revision+2,"moving this connection's endpoint invalidates and recomputes its path");
            doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==stableTarget).Y==350,"undo restores endpoint position");
            doc.Undo();Pump();Check(doc.CurrentProcedure.Nodes.Single(n=>n.Id==movingObstacle).X==900,"second undo restores independent obstacle position");
            string stableFile=Path.Combine(Path.GetTempPath(),"vision-canvas-obstacle-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(stableFile);doc.Load(stableFile);Pump();Check(doc.CurrentProcedure.ExecutionLinks!.Single().ToNode==stableTarget&&BeforePath().Length>0,"connection survives save/reopen and rebuilds route on solution replacement");}
            finally {if(File.Exists(stableFile))File.Delete(stableFile);}

            // End-to-end overlap case also covered by VmRouting.Checks against the untouched AStar oracle.
            // Native DiagramPage.GetAllRect includes every DiagramItem, while GetPath seeds both endpoints
            // separately; retain that overlapping-node geometry through the actual WPF GraphLayer renderer.
            doc.NewSolution();
            string overlapSource=doc.AddNode("vision.image-source","1.0.0",180,150);
            string overlapTarget=doc.AddNode("vision.image-source","1.0.0",220,160);
            doc.AddNode("vision.image-source","1.0.0",210,145);
            doc.ConnectExecution(overlapSource,overlapTarget,3,1);Pump();
            var overlapSourceBox=new RouteRect(180,150,110,30);var overlapTargetBox=new RouteRect(220,160,110,30);
            var overlapExpected=VmOrthogonalRouter.Route(new(new(235,180),PortDirection.Down,overlapSourceBox),new(new(275,160),PortDirection.Up,overlapTargetBox),[new RouteRect(210,145,110,30)],5000,5000,1_100_000);
            string overlapKey=overlapSource+">"+overlapTarget;
            var overlapHit=((Dictionary<string,System.Windows.Shapes.Path>)typeof(MainWindow).GetField("wireHitPaths",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!)[overlapKey];
            var overlapGeometry=new PathGeometry();var overlapFigure=new PathFigure{StartPoint=new(overlapExpected.Points[0].X,overlapExpected.Points[0].Y)};
            foreach(var point in overlapExpected.Points.Skip(1))overlapFigure.Segments.Add(new LineSegment(new(point.X,point.Y),true));overlapGeometry.Figures.Add(overlapFigure);
            Check(overlapExpected.Points.Count>1&&overlapHit.Data.ToString()==overlapGeometry.ToString(),"actual WPF canvas renders the native-oracle-tested overlapping endpoint route");
            Capture("canvas-overlapping-endpoint-route.png");

            // Connection.Shape_Drop forwards the dragged DiagramItem.DataContext;
            // VmConnectionDecorator.InsertModule therefore rewires the source item
            // even when its drag effect was Copy. The ghost position is not committed.
            doc.NewSolution();
            string copySource=doc.AddNode("vision.image-source","1.0.0",100,100);
            string copyTarget=doc.AddNode("vision.image-source","1.0.0",500,100);
            string copyCandidate=doc.AddNode("vision.image-source","1.0.0",800,700);
            doc.ConnectExecution(copySource,copyTarget,2,0);Pump();
            selected.Clear();selected.Add(copyCandidate);
            revision=doc.Revision;
            Invoke("BeginNodeGesture",new Point(800,700),true);
            Invoke("PreviewNodeMove",new Point(350,115));Pump();
            var insertionWire=(ExecutionLinkDocument?)typeof(MainWindow).GetField("insertWire",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window);
            Check(insertionWire is {FromNode:var from,ToNode:var to}&&from==copySource&&to==copyTarget,"Ctrl-copy drag over the rendered execution wire resolves the original insertion target");
            Capture("canvas-wire-copy-insertion-preview.png");
            Invoke("CommitNodeMove");Pump();
            var copyInsertedEdges=doc.CurrentProcedure.ExecutionLinks!;
            Check(doc.Revision==revision+1&&doc.CurrentProcedure.Nodes.Count==3&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==copyCandidate).X==800&&copyInsertedEdges.Any(e=>e.FromNode==copySource&&e.ToNode==copyCandidate&&e.SourceIndex==2&&e.TargetIndex==0)&&copyInsertedEdges.Any(e=>e.FromNode==copyCandidate&&e.ToNode==copyTarget&&e.SourceIndex==2&&e.TargetIndex==0),"Ctrl-copy wire drop follows native InsertModule: rewires the existing module, preserves its position and both port indexes in one transaction");
            doc.Undo();Pump();
            Check(doc.CurrentProcedure.Nodes.Count==3&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==copyCandidate).X==800&&doc.CurrentProcedure.ExecutionLinks!.Single().FromNode==copySource&&doc.CurrentProcedure.ExecutionLinks.Single().ToNode==copyTarget,"undo restores original node position and original wire after copy-effect insertion");
            doc.Redo();Pump();
            string copyInsertFile=Path.Combine(Path.GetTempPath(),"vision-canvas-copy-insert-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try {doc.Save(copyInsertFile);doc.Load(copyInsertFile);Pump();var reopened=doc.CurrentProcedure.ExecutionLinks!;Check(doc.CurrentProcedure.Nodes.Count==3&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==copyCandidate).X==800&&reopened.Any(e=>e.FromNode==copySource&&e.ToNode==copyCandidate)&&reopened.Any(e=>e.FromNode==copyCandidate&&e.ToNode==copyTarget),"copy-effect insertion topology and original node position survive save/reopen");}
            finally {if(File.Exists(copyInsertFile))File.Delete(copyInsertFile);}

            // ItemsPage.GetDropItems clamps each ghost independently, while the
            // native copy path anchors all clones at the first item's drop point.
            doc.NewSolution();
            string firstCopy=doc.AddNode("vision.image-source","1.0.0",4800,100);
            string secondCopy=doc.AddNode("vision.image-source","1.0.0",100,250);Pump();
            selected.Clear();selected.Add(firstCopy);selected.Add(secondCopy);
            revision=doc.Revision;
            Invoke("BeginNodeGesture",new Point(0,0),true);
            Invoke("PreviewNodeMove",new Point(500,0));Pump();
            var copyGhosts=(Dictionary<string,Canvas>)typeof(MainWindow).GetField("movingGhosts",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Check(Math.Abs(Canvas.GetLeft(copyGhosts[firstCopy])+10-4890)<.001&&Math.Abs(Canvas.GetLeft(copyGhosts[secondCopy])+10-600)<.001,"multi-copy preview clamps each source item at its own right-edge position");
            Capture("canvas-multicopy-right-edge-preview-near.png");
            scroll.ScrollToHorizontalOffset(4300);Pump();
            Capture("canvas-multicopy-right-edge-preview-far.png");
            Invoke("CommitNodeMove");Pump();
            var edgeCopies=doc.CurrentProcedure.Nodes.Where(n=>n.Id!=firstCopy&&n.Id!=secondCopy).ToArray();
            Check(edgeCopies.Length==2&&edgeCopies.Single(n=>Math.Abs(n.X-4890)<.001).Y==100&&edgeCopies.Single(n=>Math.Abs(n.X-190)<.001).Y==250&&doc.Revision==revision+1,"multi-copy uses first source item as native paste anchor and preserves original relative offset after per-item source clamp");
            doc.Undo();Pump();
            Check(doc.CurrentProcedure.Nodes.Count==2&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==firstCopy).X==4800&&doc.CurrentProcedure.Nodes.Single(n=>n.Id==secondCopy).X==100,"undo removes both edge-copies and preserves original nodes");
            doc.Redo();Pump();
            string edgeCopyFile=Path.Combine(Path.GetTempPath(),"vision-canvas-edge-copy-"+Guid.NewGuid().ToString("N")+".vmx.json");
            try{doc.Save(edgeCopyFile);doc.Load(edgeCopyFile);Pump();edgeCopies=doc.CurrentProcedure.Nodes.Where(n=>n.Id!=firstCopy&&n.Id!=secondCopy).ToArray();Check(edgeCopies.Length==2&&edgeCopies.Single(n=>Math.Abs(n.X-4890)<.001).Y==100&&edgeCopies.Single(n=>Math.Abs(n.X-190)<.001).Y==250,"first-item anchored multi-copy coordinates survive save/reopen");}
            finally{if(File.Exists(edgeCopyFile))File.Delete(edgeCopyFile);}
        }
        finally {window.Close();application.Shutdown();}
    }
}
