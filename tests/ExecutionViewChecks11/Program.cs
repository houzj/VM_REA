using System.Reflection;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;

internal static class Program
{
 [STAThread] static int Main()
 {
  try
  {
   var app=new Application{ShutdownMode=ShutdownMode.OnExplicitShutdown};var window=new MainWindow();
   const BindingFlags flags=BindingFlags.Instance|BindingFlags.NonPublic;
   T Field<T>(string name)=>(T)typeof(MainWindow).GetField(name,flags)!.GetValue(window)!;
   void Call(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,flags)!.Invoke(window,args);
   int count=0;void Check(bool ok,string name){if(!ok)throw new Exception(name);count++;Console.WriteLine("PASS "+name);}
   var doc=Field<GraphEditorDocument>("doc");doc.NewSolution();string id=doc.CurrentProcedureId;
   var sessions=Field<Dictionary<string,ExecutionSessionResult>>("workflowSessions");
   var results=Field<Dictionary<string,NodeRunRecord>>("results");
   var preferences=Field<FrameworkExecutionPreferences>("frameworkExecutionPreferences");
   var record=new NodeRunRecord("fixture",NodeRunStatus.Success,new Dictionary<string,PortValue>(),1);
   var old=new ExecutionSessionResult(SessionStatus.Success,[record]);sessions[id]=old;results[record.NodeId]=record;
   preferences.IsClearLastExecuteResult=true;Call("BeginWorkflowResults",id);
   Check(results.Count==1,"first Begin retains old view even with clearing enabled");
   Call("BeginWorkflowResults",id);
   Check(results.Count==0,"second Begin clears the displayed result view");
   Check(ReferenceEquals(sessions[id],old),"clear view leaves completed session identity intact");
   Call("RestoreWorkflowResults");
   Check(results.Count==0,"tab refresh cannot resurrect hidden pre-Begin view");
   var next=new ExecutionSessionResult(SessionStatus.Success,[record with{ElapsedMilliseconds=2}]);
   Call("StoreWorkflowCallResults",id,next);Call("RestoreWorkflowResults");
   Check(results[record.NodeId].ElapsedMilliseconds==2,"new completed results replace the hidden view");
   Check(!Field<HashSet<string>>("clearedWorkflowResultViews").Contains(id),"completed results release the view marker");
   preferences.IsClearLastExecuteResult=false;Call("BeginWorkflowResults",id);
   Check(results.Count==1,"disabled policy retains preceding results at Begin");
   string other=doc.AddProcedure("Other");preferences.IsClearLastExecuteResult=true;
   results[record.NodeId]=record;Call("BeginWorkflowResults",other);
   Check(results.Count==1,"another procedure has its own first-Begin counter");
   Call("BeginWorkflowResults",other);doc.DeleteProcedure(other);
   Check(!Field<Dictionary<string,uint>>("workflowExecuteCounts").ContainsKey(other),"deleting a procedure releases its execution counter");
   Check(!Field<HashSet<string>>("clearedWorkflowResultViews").Contains(other),"deleting a procedure releases its hidden-view marker");
   doc.NewSolution();
   Check(Field<Dictionary<string,uint>>("workflowExecuteCounts").Count==0,"solution replacement resets main execution counters");
   Check(Field<HashSet<string>>("clearedWorkflowResultViews").Count==0,"solution replacement clears view markers");
   window.Close();app.Shutdown();Console.WriteLine($"PASS {count} main execution-view checks; independent adapter, not SDK leaf clearing");return 0;
  }
  catch(Exception e){Console.Error.WriteLine(e);return 1;}
 }
}
