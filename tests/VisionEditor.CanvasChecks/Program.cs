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
            mini.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent});Pump();
            Check((bool)typeof(MainWindow).GetField("eyeDragging",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!,"actual mini-map mouse-down routed event starts dragging");
            mini.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonUpEvent});Pump();
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
            focusHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent});Pump();
            var handles=(List<System.Windows.Shapes.Rectangle>)typeof(MainWindow).GetField("wireHandles",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
            Check(focusHit.IsKeyboardFocused&&handles.Count>0,"actual wire mouse-down focuses the connection and exposes midpoint handles");
            Check(Math.Abs(scroll.HorizontalOffset-focusX)<.001&&Math.Abs(scroll.VerticalOffset-focusY)<.001,"focusing the connection does not auto-scroll the viewport");
            Capture("canvas-wire-focus-no-scroll.png");
            ((TabControl)window.FindName("ProcedureTabs")).Focus();Pump();
            Check(handles.Count==0,"connection lost focus removes midpoint handles");
            focusHit.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,0,MouseButton.Left){RoutedEvent=UIElement.MouseLeftButtonDownEvent});Pump();
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
        }
        finally {window.Close();application.Shutdown();}
    }
}
