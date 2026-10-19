using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program {
    [STAThread] static int Main(string[] args){try{Run(args);return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
    static void Run(string[] args){
        if(args.Length!=1)throw new ArgumentException("Supply an audit directory.");
        string root=Path.Combine(Path.GetFullPath(args[0]),"canvas-replacement-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
        var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};
        var window=new MainWindow{ShowActivated=false,ShowInTaskbar=false,Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual};
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(window.Dispatcher));
        const BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
        object Get(string name)=>typeof(MainWindow).GetField(name,flags)!.GetValue(window)!;
        void Set(string name,object? value)=>typeof(MainWindow).GetField(name,flags)!.SetValue(window,value);
        void Call(string name,params object?[] values)=>typeof(MainWindow).GetMethod(name,flags)!.Invoke(window,values);
        void Pump(){window.UpdateLayout();window.Dispatcher.Invoke(()=>{},DispatcherPriority.ApplicationIdle);}
        int checks=0;void Check(bool value,string text){if(!value)throw new Exception(text);checks++;Console.WriteLine("PASS "+text);}
        var doc=(GraphEditorDocument)Get("doc");
        void Seed(bool overview){Set("workflowOverview",overview);Set("canvasProcedure",doc.CurrentProcedureId);Set("scale",1.8d);Set("panX",-300d);Set("panY",-200d);
            var cache=(IDictionary)Get("canvasViews");cache["main"]=(1.8d,-300d,-200d);cache["__solution"]=(1.7d,-90d,-70d);cache["old-only"]=(.6d,-20d,-30d);Call("ApplyCanvasTransform");Pump();}
        void DefaultView(string phase){Check(Math.Abs((double)Get("scale")-1)<.0001&&Math.Abs((double)Get("panX"))<.001&&Math.Abs((double)Get("panY"))<.001,phase+": scale and offsets reset");
            Check(!((IDictionary)Get("canvasViews")).Contains("old-only"),phase+": old solution cache removed");
            var transform=((FrameworkElement)Get("GraphSurface")).LayoutTransform as ScaleTransform;var scroll=(ScrollViewer)Get("CanvasScrollViewer");
            Check(transform is not null&&Math.Abs(transform.ScaleX-1)<.0001&&Math.Abs(transform.ScaleY-1)<.0001&&Math.Abs(scroll.HorizontalOffset)<.001&&Math.Abs(scroll.VerticalOffset)<.001&&((TextBlock)Get("ZoomValue")).Text=="100%",phase+": rendered transform, scroll and zoom label reset");}
        try{
            window.Show();Pump();doc.NewSolution();Pump();Seed(false);doc.NewSolution();Pump();DefaultView("new solution reusing main id");
            string first=Path.Combine(root,"first.vmx.json"),second=Path.Combine(root,"second.vmx.json");
            SolutionStore.Save(first,doc.Document);SolutionStore.Save(second,doc.Document with{Id="other-solution",Name="other"});
            doc.Load(first);Pump();Seed(false);doc.Load(second);Pump();DefaultView("load other solution with same procedure id");
            Seed(true);doc.Load(first);Pump();DefaultView("replace while overview visible");
            Call("WorkflowViewClick",window,new RoutedEventArgs());Pump();DefaultView("leave overview after replacement");
            Call("WorkflowViewClick",window,new RoutedEventArgs());Pump();DefaultView("return to overview after replacement");
            Set("workflowOverview",false);doc.Load(first);Pump();Seed(false);doc.Load(first);Pump();DefaultView("reload same solution and procedure identity");
            string empty=Path.Combine(root,"empty.vmx.json");SolutionStore.Save(empty,doc.Document with{Procedures=[]});
            Seed(false);doc.Load(empty);Pump();DefaultView("load empty solution");
            doc.AddProcedure("first after empty");Pump();DefaultView("add procedure after empty load");
            Set("workflowOverview",false);Call("RenderGraph");Pump();string main=doc.CurrentProcedureId;string other=doc.AddProcedure("other");Pump();
            Call("SelectWorkflow",main);Call("SetCanvasZoom",1.4d,new Point(0,0));Pump();
            Call("SelectWorkflow",other);Call("SetCanvasZoom",1.7d,new Point(0,0));Pump();
            Call("SelectWorkflow",main);Pump();Check(Math.Abs((double)Get("scale")-1.4)<.0001,"same solution retains first procedure viewport");
            Call("SelectWorkflow",other);Pump();Check(Math.Abs((double)Get("scale")-1.7)<.0001,"same solution retains second procedure viewport");
            Console.WriteLine($"PASS {checks} actual WPF canvas replacement checks. Evidence: {root}");
        }finally{window.Close();app.Shutdown();}
    }
}
