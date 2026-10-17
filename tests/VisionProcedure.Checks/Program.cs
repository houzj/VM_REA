using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionModules;
var registry=new ModuleRegistry(); BuiltinModules.RegisterAll(registry);
var editor=new GraphEditorDocument(registry); int checks=0;
void Check(bool condition,string message){if(!condition)throw new Exception(message);checks++;}
void Reject(Action action){try{action();}catch(Exception e) when(e is ArgumentException or InvalidOperationException or KeyNotFoundException){checks++;return;}throw new Exception("Expected rejection");}
// The editor supports the source-confirmed empty-workflow state, also covered
// by actual WPF checks. Last-procedure deletion is undoable, not rejected.
string main=editor.CurrentProcedureId;editor.DeleteProcedure(main);Check(!editor.HasCurrentProcedure&&editor.Document.Procedures.Count==0,"Deleting last procedure enters empty state");editor.Undo();Check(editor.CurrentProcedureId==main,"Undo restores last procedure selection");
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
// Independent .vmx.json save-as must keep resource identity while changing the base directory.
string migrationRoot=Path.Combine(Path.GetTempPath(),"procedure-resource-"+Guid.NewGuid().ToString("N"));
try {
    string oldDir=Path.Combine(migrationRoot,"old"),newDir=Path.Combine(migrationRoot,"new"),assetDir=Path.Combine(oldDir,"assets");
    Directory.CreateDirectory(assetDir);Directory.CreateDirectory(newDir);
    byte[] payload=[1,3,5,7,9];File.WriteAllBytes(Path.Combine(assetDir,"sample.bin"),payload);
    string oldPath=Path.Combine(oldDir,"original.vmx.json"),newPath=Path.Combine(newDir,"copy.vmx.json");
    var solution=new SolutionDocument(SolutionStore.CurrentVersion,"resource-case","resource-case",[new ProcedureDocument("p","p",[],[])],[new ResourceDocument("sample","assets/sample.bin")]);
    SolutionStore.Save(oldPath,solution);
    var migrated=new GraphEditorDocument(registry);migrated.Load(oldPath);migrated.Save(newPath);
    var reopened=SolutionStore.OpenWorkspace(newPath);
    using(var stream=new ResourceCatalog(reopened.BaseDirectory,reopened.Document.Resources).OpenVerified("sample"))
        Check(File.Exists(newPath)&&stream.ReadByte()==1,"Save-as rebases relative resource without losing bytes");
    Check(SolutionStore.Load(oldPath).Resources.Single().Path=="assets/sample.bin"&&migrated.Document.Resources.Single().Path==Path.GetRelativePath(newDir,Path.Combine(assetDir,"sample.bin")),"Save-as preserves original solution and updates editor resource base");
    var priorFile=migrated.FilePath;var priorBase=migrated.BaseDirectory;var priorRevision=migrated.Revision;
    migrated.PrepareSave=(_,_)=>throw new InvalidDataException("prepare failure");
    try{migrated.Save(Path.Combine(newDir,"failed.vmx.json"));throw new Exception("Expected prepare failure");}catch(InvalidDataException){}
    Check(migrated.FilePath==priorFile&&migrated.BaseDirectory==priorBase&&migrated.Revision==priorRevision&&!File.Exists(Path.Combine(newDir,"failed.vmx.json")),"Failed preparation leaves editor and output unchanged");
}finally{if(Directory.Exists(migrationRoot))Directory.Delete(migrationRoot,true);}
Console.WriteLine($"PASS {checks} procedure management checks");
