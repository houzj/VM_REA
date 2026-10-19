using System.Reflection;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;

internal static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread] static int Main()
 {
  try
  {
   var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};
   var window=new MainWindow();
   var doc=(GraphEditorDocument)typeof(MainWindow).GetField("doc",Private)!.GetValue(window)!;
   object? Call(string name,params object[] args)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,args);
   int checks=0;
   void Check(bool condition,string label){if(!condition)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
   doc.NewSolution();
   var root=doc.CurrentProcedureId;
   var groupNode=doc.AddOwnedGroup(100,100);
   var group=doc.EnterOwnedGroup(groupNode);
   var nestedNode=doc.AddOwnedGroup(200,200);
   var nested=doc.EnterOwnedGroup(nestedNode);
   Check((string)Call("ResolveDisplayedWorkflow",group)! == group,"Explicit Group target is preserved");
   var other=doc.AddProcedure("Other");
   Call("SelectWorkflow",root);
   Check(doc.CurrentProcedureId==nested,"Root tab restores its nested displayed Group");
   Call("SelectWorkflow",other);
   Check(doc.CurrentProcedureId==other,"Other tab selects its own root");
   Call("SelectWorkflow",root);
   Check(doc.CurrentProcedureId==nested,"A to B to A retains nested Group identity");
   var rootGroupNode=doc.Document.Procedures.Single(p=>p.Id==root).Nodes.Single(n=>n.Id==groupNode);
   Call("SelectWorkflowNode",root,rootGroupNode);
   Check(doc.CurrentProcedureId==root,"Explicit workflow-list node navigates to its owning root instead of remembered nested Group");
   Check((string?)typeof(MainWindow).GetField("selected",Private)!.GetValue(window)==groupNode,"Explicit workflow-list node becomes the selected node");
   doc.EnterOwnedGroup(groupNode);doc.EnterOwnedGroup(nestedNode);
   Call("ReturnFromGroup");
   Check(doc.CurrentProcedureId==group,"Back navigates to actual immediate parent");
   Call("SelectWorkflow",other);Call("SelectWorkflow",root);
   Check(doc.CurrentProcedureId==group,"Back updates the remembered display instead of restoring deeper history");
   doc.SelectProcedure(root);doc.DeleteNode(groupNode);
   Check((string)Call("ResolveDisplayedWorkflow",root)! == root,"Deleting Group discards its remembered container");
   Call("SelectWorkflow",other);Call("SelectWorkflow",root);
   Check(doc.CurrentProcedureId==root,"Deleted Group falls back to surviving root");
   groupNode=doc.AddOwnedGroup(100,100);doc.EnterOwnedGroup(groupNode);
   doc.NewSolution();
   Check((string)Call("ResolveDisplayedWorkflow","main")! == "main","Same root ID in new solution does not restore old Group");
   var state=(Dictionary<string,string>)typeof(MainWindow).GetField("displayedWorkflowContainers",Private)!.GetValue(window)!;
   Check(state.Values.All(id=>doc.Document.Procedures.Any(p=>p.Id==id)),"No previous solution container identity survives replacement");
   window.Close();app.Shutdown();Console.WriteLine($"PASS {checks}/{checks} session navigation checks; no original runtime or manual acceptance claimed");return 0;
  }
  catch(Exception e){Console.Error.WriteLine(e);return 1;}
 }
}
