using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string root=Path.Combine(Path.GetFullPath(args[0]),"transaction-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
int checks=0;var labels=new List<string>();var failures=new List<object>();
void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;labels.Add(label);Console.WriteLine("PASS "+label);}
string Json(object value)=>JsonSerializer.Serialize(value);
string Digest(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
// The actual representation now contains roots/parts and both payloads, rather than two snapshot stacks.
string History(GraphEditorDocument doc,string name){var engine=typeof(GraphEditorDocument).GetField("history",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(doc)!;var stack=(IEnumerable)engine.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(engine)!;return string.Join("\n",stack.Cast<object>().Select(s=>JsonSerializer.Serialize(s,s.GetType())));}
string State(GraphEditorDocument doc)=>Json(new{Document=Json(doc.Document),doc.CurrentProcedureId,doc.FilePath,doc.BaseDirectory,doc.IsDirty,doc.Revision,doc.CanUndo,doc.CanRedo,Undo=History(doc,"undo"),Redo=History(doc,"redo")});
void Record(string phase,Exception error)=>failures.Add(new{phase,type=error.GetType().FullName,hresult=$"0x{error.HResult:X8}",error.Message,error.StackTrace});
try
{
 var doc=new GraphEditorDocument(new ModuleRegistry());
 string originalDir=Path.Combine(root,"original"),alternateDir=Path.Combine(root,"alternate");Directory.CreateDirectory(originalDir);Directory.CreateDirectory(alternateDir);
 string original=Path.Combine(originalDir,"current.vmx.json"),alternate=Path.Combine(alternateDir,"alternate.vmx.json");
 doc.Save(original);SolutionStore.Save(alternate,doc.Document);string originalHash=Digest(original),alternateHash=Digest(alternate);
 int changed=0;doc.Changed+=()=>changed++;
 void LockedFailure(string target,string phase)
 {
  string state=State(doc);int previousEvents=changed;bool denied=false;
  using(var locked=new FileStream(target,FileMode.Open,FileAccess.ReadWrite,FileShare.None))
  {
   try{doc.Save(target);}catch(Exception ex)when(ex is IOException or UnauthorizedAccessException){denied=true;Record(phase,ex);}
   Check(denied,phase+": explicit exclusive destination lock rejects save");
   Check(State(doc)==state,phase+": document, path, base, dirty, selection, revision and both history stacks unchanged");
   Check(changed==previousEvents,phase+": no commit Changed event on precommit failure");
  }
  string dir=Path.GetDirectoryName(target)!;Check(!Directory.EnumerateFiles(dir,"."+Path.GetFileName(target)+".*.tmp").Any(),phase+": own temp output cleaned after rejected move");
 }
 LockedFailure(original,"clean-current-save");Check(Digest(original)==originalHash&&!doc.IsDirty,"Clean failed save preserves current bytes and clean flag");
 string id=doc.CurrentProcedureId;doc.RenameProcedure(id,"first");doc.RenameProcedure(id,"second");doc.Undo();
 Check(doc.CanUndo&&doc.CanRedo&&doc.IsDirty,"Dirty fixture has distinct undo and redo entries before denial");
 int prepares=0;doc.PrepareSave=(draft,basePath)=>{prepares++;Check(basePath==originalDir,"PrepareSave receives current base before attempted relocation");return draft with{Name="prepared-only",Procedures=draft.Procedures.Select(p=>p with{Name="prepared-only"}).ToArray()};};
 LockedFailure(alternate,"dirty-alternate-save-as");Check(prepares==1,"Preparation executes once without retry");
 Check(Digest(original)==originalHash&&Digest(alternate)==alternateHash,"Denied SaveAs leaves current and alternate file bytes intact");
 Check(doc.Document.Name!="prepared-only"&&doc.CurrentProcedure.Name=="first","Prepared draft cannot leak into current model after move failure");
 doc.PrepareSave=null;doc.Redo();Check(doc.CurrentProcedure.Name=="second"&&doc.BaseDirectory==originalDir&&doc.FilePath==original,"Redo after rejected SaveAs still restores original structural edit and base identity");
 doc.Undo();Check(doc.CurrentProcedure.Name=="first","Undo remains usable after rejected SaveAs and subsequent redo");
 string beforePrepareFailure=State(doc);int eventsBefore=changed;doc.PrepareSave=(_,_)=>throw new InvalidDataException("owned-prepare-failure");bool prepareFailed=false;
 try{doc.Save(alternate);}catch(InvalidDataException ex){prepareFailed=ex.Message=="owned-prepare-failure";Record("prepare-delegate-failure",ex);}
 Check(prepareFailed&&State(doc)==beforePrepareFailure&&changed==eventsBefore,"PrepareSave exception preserves entire editor state before store invocation");
 Check(Digest(alternate)==alternateHash&&!Directory.EnumerateFiles(alternateDir,".*.tmp").Any(),"PrepareSave failure leaves destination and temp set unchanged");
 doc.PrepareSave=null;
 // A notification failure is a different phase: source commits before raising Changed.
 string committed=Path.Combine(alternateDir,"committed-before-notification.vmx.json");
 Action throwing=()=>throw new InvalidOperationException("owned-changed-failure");doc.Changed+=throwing;long beforeCommitRevision=doc.Revision;bool notificationFailed=false;
 try{doc.Save(committed);}catch(InvalidOperationException ex){notificationFailed=ex.Message=="owned-changed-failure";Record("postcommit-notification-failure",ex);}finally{doc.Changed-=throwing;}
 Check(notificationFailed,"Postcommit notification exception is observable to save caller");
 Check(File.Exists(committed)&&doc.FilePath==committed&&doc.BaseDirectory==alternateDir&&!doc.IsDirty&&doc.Revision==beforeCommitRevision+1,"Notification failure occurs after successful disk and editor identity commit");
 Check(Json(SolutionStore.Load(committed))==Json(doc.Document),"Committed disk document matches editor despite notification exception");
 Check(doc.CanUndo&&doc.CanRedo,"Successful commit preceding failed notification retains both history stacks");
 File.WriteAllText(Path.Combine(root,"summary.json"),JsonSerializer.Serialize(new{checks,labels,failures,root,scope="Independent editor transaction and explicit own-file locks; not original VM or a repeat of the long stability run"},new JsonSerializerOptions{WriteIndented=true}));
 Console.WriteLine($"PASS {checks}/{checks}; phase-specific failure evidence: {root}");return 0;
}
catch(Exception ex){Record("fixture-failure",ex);File.WriteAllText(Path.Combine(root,"failed.json"),JsonSerializer.Serialize(new{checks,labels,failures,root},new JsonSerializerOptions{WriteIndented=true}));Console.Error.WriteLine(ex);return 1;}
