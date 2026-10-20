using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

static class Program {
[STAThread] public static int Main(){
AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
var window=new MainWindow();
object Field(string name)=>typeof(MainWindow).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(window)!;
var doc=(GraphEditorDocument)Field("doc");
var checks=new List<string>();
void Check(bool condition,string label){if(!condition)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS "+label);}
doc.NewSolution();
var scope=(ModuleExecutionScope)Field("executionScope");
string procedure=doc.CurrentProcedureId;
string node=doc.AddNode("vision.calibration-board","1.0.0",100,100);
int disposed=0;
var state=scope.ForProcedure(procedure).State("vision.calibration-board",node,()=>new Probe(()=>disposed++));
doc.DeleteNode(node);
Check(disposed==1,"Deleting a real calibration-board graph node disposes its owned execution state");
Check(!doc.CurrentProcedure.Nodes.Any(n=>n.Id==node),"Deleted board node leaves the real graph");
doc.Undo();
Check(doc.CurrentProcedure.Nodes.Any(n=>n.Id==node),"Undo restores the board node identity");
var recreated=scope.ForProcedure(procedure).State("vision.calibration-board",node,()=>new Probe(()=>disposed++));
Check(!ReferenceEquals(state,recreated),"Undo creates a fresh board execution state");
doc.NewSolution();
Check(disposed==2,"Replacing the solution disposes the restored board state exactly once");
scope.Dispose();
Check(disposed==2,"Old scope disposal does not double-release board state");
window.Close();app.Shutdown();
Console.WriteLine("PASS "+checks.Count+" calibration lifecycle ownership checks");
Environment.Exit(0);
return 0;
}
}

sealed class Probe(Action release):IDisposable {public void Dispose()=>release();}
