using System.IO;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;

static class Program
{
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 [STAThread] static int Main(string[] args)
 {
  try
  {
   var output=Path.GetFullPath(args.FirstOrDefault()??"analysis/full-audit/batch12-20261019/A/checks");Directory.CreateDirectory(output);
   var app=new Application {ShutdownMode=ShutdownMode.OnExplicitShutdown};var window=new MainWindow();
   var doc=(GraphEditorDocument)typeof(MainWindow).GetField("doc",Private)!.GetValue(window)!;
   object? Call(string name,params object[] values)=>typeof(MainWindow).GetMethod(name,Private)!.Invoke(window,values);
   int checks=0;void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;Console.WriteLine("PASS "+label);}
   int Zoom(string id)=>doc.Document.Procedures.Single(p=>p.Id==id).CanvasZoomIndex;
   double Scale()=>(double)typeof(MainWindow).GetField("scale",Private)!.GetValue(window)!;
   doc.NewSolution();var root=doc.CurrentProcedureId;
   Check(Zoom(root)==9,"Missing view data defaults to original 100% tick");
   doc.SetContainerZoomIndex(root,4);Check(Zoom(root)==4,"Root stores its discrete tick");
   Check(!doc.CanUndo,"A zoom write does not invent a root undo operation");
   Call("RenderGraph");Check(Math.Abs(Scale()-.5)<1e-8,"Displayed root reads model tick");
   Call("SetCanvasZoom",1.3,new Point(0,0));Check(Zoom(root)==12,"Actual canvas zoom writes container model");
   var node=doc.AddOwnedGroup(100,100);var group=doc.EnterOwnedGroup(node);doc.SetContainerZoomIndex(group,2);
   var nestedNode=doc.AddOwnedGroup(200,200);var nested=doc.EnterOwnedGroup(nestedNode);doc.SetContainerZoomIndex(nested,17);
   var other=doc.AddProcedure("Other");doc.SetContainerZoomIndex(other,7);
   Check(Zoom(root)==12&&Zoom(group)==2&&Zoom(nested)==17&&Zoom(other)==7,"Root, nested Groups and other tab own independent values");
   var path=Path.Combine(output,"roundtrip.vmx.json");doc.Save(path);doc.Load(path);
   Check(Zoom(root)==12&&Zoom(group)==2&&Zoom(nested)==17&&Zoom(other)==7,"Save/reopen preserves every container tick");
   doc.SetContainerZoomIndex(root,12);Check(!doc.IsDirty,"Writing an unchanged persisted tick is a no-op");
   doc.SelectProcedure(group);Call("RenderGraph");Check(Math.Abs(Scale()-.3)<1e-8,"Switching to Group reads its persisted tick");
   doc.SelectProcedure(other);doc.RenameProcedure(other,"Other renamed");doc.SetContainerZoomIndex(group,11);
   Check(doc.CurrentProcedureId==other,"Nonactive container writes do not steal tab selection");
   doc.Undo();Check(Zoom(group)==2,"Undo from another tab restores snapshot of inactive Group view data");
   long revision=doc.Revision;doc.SetContainerZoomIndex(other,8);Check(doc.CanRedo&&doc.Revision==revision,"View writes preserve redo and structural gesture revision");
   doc.SelectProcedure(group);Call("RenderGraph");Check(Math.Abs(Scale()-.3)<1e-8,"Restored inactive Group tick appears when selected");
   doc.Redo();Check(Zoom(group)==11,"Redo restores inactive Group view data from document snapshot");
   doc.SelectProcedure(group);Call("RenderGraph");Check(Math.Abs(Scale()-1.2)<1e-8,"Same displayed Group re-reads model after history restore");
   var ticks=doc.Document.Procedures.ToDictionary(p=>p.Id,p=>p.CanvasZoomIndex);
   Call("WorkflowViewClick",window,new RoutedEventArgs());Call("SetCanvasZoom",.4,new Point(0,0));
   Check(doc.Document.Procedures.All(p=>ticks[p.Id]==p.CanvasZoomIndex),"Overview zoom cannot overwrite any workflow container");
   Call("WorkflowViewClick",window,new RoutedEventArgs());Check(Math.Abs(Scale()-1.2)<1e-8,"Leaving overview restores displayed Group model tick");
   int old=Zoom(group);foreach(var invalid in new[]{-1,20}){bool rejected=false;try{doc.SetContainerZoomIndex(group,invalid);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected&&Zoom(group)==old,"Invalid tick rejected without mutation: "+invalid);}
   var json=JsonNode.Parse(File.ReadAllText(path))!;var procedures=json["Procedures"]!.AsArray();foreach(var p in procedures)p!.AsObject().Remove("CanvasZoomIndex");var legacy=Path.Combine(output,"legacy.vmx.json");File.WriteAllText(legacy,json.ToJsonString());doc.Load(legacy);
   Check(doc.Document.Procedures.All(p=>p.CanvasZoomIndex==9),"Existing files without zoom property default to 100%");
   Call("SetCanvasZoom",.1,new Point(0,0));Check(Zoom(doc.CurrentProcedureId)==0,"Lowest UI tick maps to zero");Call("SetCanvasZoom",2d,new Point(0,0));Check(Zoom(doc.CurrentProcedureId)==19,"Highest UI tick maps to nineteen");
   Call("Fit");Check(Zoom(doc.CurrentProcedureId)>=0&&Zoom(doc.CurrentProcedureId)<=19,"Fit updates a valid discrete container tick");
   doc.Load(path);var copy=doc.DuplicateProcedure(root,"Copy");
   var copied=doc.Document.Procedures.Where(p=>doc.RootProcedureId(p.Id)==copy).ToDictionary(p=>p.Id,p=>p.CanvasZoomIndex);
   Check(copied.Count==3&&copied.Values.Order().SequenceEqual(new[]{2,12,17}),"Copy remaps owned container identities and preserves each tick");
   doc.DeleteProcedure(copy);Check(copied.Keys.All(id=>doc.Document.Procedures.All(p=>p.Id!=id)),"Deleting copied root removes only its owned view identities");
   Check(Zoom(root)==12&&Zoom(group)==2&&Zoom(nested)==17,"Deleting copy cannot change source container ticks");
   doc.Undo();Check(copied.All(pair=>Zoom(pair.Key)==pair.Value),"Undo deletion restores exact copied identities and their zoom values");
   doc.NewSolution();Check(Zoom(doc.CurrentProcedureId)==9,"New solution cannot inherit old model zoom");
   Call("SetCanvasZoom",.6,new Point(0,0));Call("Fit");Check(Zoom(doc.CurrentProcedureId)==9,"Fitting an empty canvas persists its 100% default");
   window.Close();app.Shutdown();Console.WriteLine($"PASS {checks}/{checks}; independent document/WPF self-review, not original SDK or desktop parity");return 0;
  }
  catch(Exception e){Console.Error.WriteLine(e);return 1;}
 }
}
