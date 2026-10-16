using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionModules;
var registry=new ModuleRegistry(); BuiltinModules.RegisterAll(registry);
var editor=new GraphEditorDocument(registry); int checks=0;
void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
void Reject(Action action){try{action();}catch(Exception e) when(e is ArgumentException or InvalidOperationException or KeyNotFoundException){checks++;return;}throw new Exception("Expected rejection");}
string main=editor.CurrentProcedureId; Reject(()=>editor.DeleteProcedure(main));
string source=editor.AddNode("vision.image-file","1.0.0",40,50,new(){["Path"]=JsonSerializer.SerializeToElement("assets/source.bmp")});
string threshold=editor.AddNode("vision.threshold","1.0.0",300,150);
editor.Connect(source,"Image",threshold,"Image");
var old=editor.CurrentProcedure;
string copy=editor.DuplicateProcedure(main,"流程副本");
Check(copy!=main && editor.CurrentProcedureId==copy,"Copy identity and selection");
var duplicated=editor.CurrentProcedure;
Check(duplicated.Nodes.Count==2 && duplicated.Nodes.All(n=>old.Nodes.All(o=>o.Id!=n.Id)),"Fresh node identities");
Check(duplicated.Connections[0].FromNode==duplicated.Nodes[0].Id && duplicated.Connections[0].ToNode==duplicated.Nodes[1].Id,"Subscription remap");
Check(duplicated.ExecutionLinks![0].FromNode==duplicated.Nodes[0].Id && duplicated.ExecutionLinks[0].ToNode==duplicated.Nodes[1].Id,"Execution remap");
Check(duplicated.Nodes[0].Parameters["Path"].GetString()=="assets/source.bmp" && duplicated.Nodes[0].X==40,"Parameters and coordinates");
editor.SetParameter(duplicated.Nodes[0].Id,"Path",JsonSerializer.SerializeToElement("another.bmp"));
editor.SelectProcedure(main); Check(editor.CurrentProcedure.Nodes[0].Parameters["Path"].GetString()=="assets/source.bmp","Copy parameter isolation");
editor.SelectProcedure(copy); editor.RenameProcedure(copy,"重命名");
Check(editor.CurrentProcedure.Name=="重命名","Rename"); editor.Undo(); Check(editor.CurrentProcedure.Name=="流程副本" && editor.CurrentProcedureId==copy,"Rename undo selection");
editor.Redo(); Check(editor.CurrentProcedure.Name=="重命名","Rename redo");
editor.DeleteProcedure(copy); Check(editor.CurrentProcedureId==main && editor.Document.Procedures.Count==1,"Delete chooses surviving neighbour");
editor.Undo(); Check(editor.CurrentProcedureId==copy && editor.Document.Procedures.Count==2,"Delete undo restores selection");
editor.Redo(); Check(editor.CurrentProcedureId==main,"Delete redo restores selection");
string other=editor.AddProcedure("Other"); editor.SelectProcedure(main); editor.DeleteProcedure(other); Check(editor.CurrentProcedureId==main,"Deleting unselected preserves selection");
long revision=editor.Revision; Reject(()=>editor.RenameProcedure(main," ")); Reject(()=>editor.DuplicateProcedure("missing")); Reject(()=>editor.DeleteProcedure("missing")); Check(editor.Revision==revision,"Rejected edits are atomic");
var path=Path.Combine(Path.GetTempPath(),"procedure-check-"+Guid.NewGuid().ToString("N")+".json");
try{editor.DuplicateProcedure(main);editor.Save(path);var loaded=new GraphEditorDocument(registry);loaded.Load(path);Check(loaded.Document.Procedures.Count==2 && loaded.Document.Procedures[1].Connections.Count==1,"Persistence");}finally{File.Delete(path);}
Console.WriteLine($"PASS {checks} procedure management checks");
