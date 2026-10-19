using System.Text.Json;
using System.Text.Json.Nodes;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"viewport-"+Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(output);
var checks=new List<string>();
void Check(bool value,string name){if(!value)throw new Exception(name);checks.Add(name);Console.WriteLine("PASS "+name);}
void Reject<T>(Action action,string name)where T:Exception{try{action();}catch(T){Check(true,name);return;}throw new Exception("Missing rejection: "+name);}
GraphEditorDocument Fresh(){var registry=new ModuleRegistry();WorkflowCalls.Register(registry);return new(registry);}
int Zoom(GraphEditorDocument d,string id)=>d.Document.Procedures.Single(p=>p.Id==id).CanvasZoomIndex;
var doc=Fresh();string first=doc.CurrentProcedureId;
Check(Zoom(doc,first)==9,"New container defaults to index 9");
string saved=Path.Combine(output,"initial.vmx.json");doc.Save(saved);
long version=doc.Revision;int events=0;doc.Changed+=()=>events++;
doc.SetContainerZoomIndex(first,9);
Check(!doc.IsDirty&&events==0&&doc.Revision==version,"Equal zoom does not dirty notify or increment revision");
Reject<ArgumentOutOfRangeException>(()=>doc.SetContainerZoomIndex(first,-1),"Negative zoom rejected");
Reject<ArgumentOutOfRangeException>(()=>doc.SetContainerZoomIndex(first,20),"Index above 19 rejected");
Reject<KeyNotFoundException>(()=>doc.SetContainerZoomIndex("missing",1),"Missing container rejected");
Check(!doc.IsDirty&&events==0&&!doc.CanUndo&&!doc.CanRedo,"Rejected values preserve state and history");
doc.SetContainerZoomIndex(first,0);
Check(doc.IsDirty&&events==1&&doc.Revision==version,"Zoom dirties and notifies without structural revision");
Check(!doc.CanUndo&&!doc.CanRedo,"Zoom alone creates no structural history");
doc.RenameProcedure(first,"renamed");doc.SetContainerZoomIndex(first,19);
doc.Undo();
Check(Zoom(doc,first)==0&&doc.CurrentProcedure.Name=="Main","Undo restores prior document zoom");
Check(doc.CanRedo,"Undo creates redo");
doc.SetContainerZoomIndex(first,4);
Check(doc.CanRedo,"Zoom after undo retains redo");
doc.Redo();
Check(Zoom(doc,first)==19&&doc.CurrentProcedure.Name=="renamed","Redo restores captured destination zoom");
doc.Undo();Check(Zoom(doc,first)==4,"Redo captures interim zoom as reverse destination");

var inactive=Fresh();string p=inactive.CurrentProcedureId;string other=inactive.AddProcedure("other");
inactive.SetContainerZoomIndex(p,3);
Check(inactive.CurrentProcedureId==other&&Zoom(inactive,p)==3&&Zoom(inactive,other)==9,"Nonactive target updates without selecting or polluting active page");
inactive.RenameProcedure(other,"other-edited");inactive.SetContainerZoomIndex(p,18);
inactive.Undo();
Check(inactive.CurrentProcedureId==other&&Zoom(inactive,p)==3,"Undo of active structural edit restores inactive container zoom");
inactive.Redo();Check(Zoom(inactive,p)==18,"Redo restores inactive container zoom");
inactive.SelectProcedure(p);Check(Zoom(inactive,p)==18,"Selection retains restored zoom");
string duplicate=inactive.DuplicateProcedure(p,"copy");
Check(Zoom(inactive,duplicate)==18,"Procedure duplication preserves zoom");
inactive.SetContainerZoomIndex(duplicate,1);
Check(Zoom(inactive,p)==18&&Zoom(inactive,duplicate)==1,"Duplicate zoom is independent");

var group=Fresh();string parent=group.CurrentProcedureId;group.SetContainerZoomIndex(parent,6);
string node=group.AddOwnedGroup(100,100);string body=group.EnterOwnedGroup(node);group.SetContainerZoomIndex(body,17);
group.SelectProcedure(parent);string parentCopy=group.DuplicateProcedure(parent,"parent-copy");
var clonedBody=group.Document.Procedures.Single(x=>x.GroupOwner?.ProcedureId==parentCopy);
Check(Zoom(group,parentCopy)==6&&clonedBody.CanvasZoomIndex==17,"Owned Group subtree duplication preserves both container fields");
group.SetContainerZoomIndex(clonedBody.Id,2);
Check(Zoom(group,body)==17&&Zoom(group,clonedBody.Id)==2,"Cloned Group field is independent from source Group");

string file=Path.Combine(output,"first","zoom.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(file)!);
inactive.Save(file);var loaded=Fresh();loaded.Load(file);
Check(Zoom(loaded,p)==18&&Zoom(loaded,duplicate)==1,"Save and Load preserve multiple zoom values");
Check(!loaded.IsDirty&&!loaded.CanUndo&&!loaded.CanRedo,"Load resets dirty and history");
string second=Path.Combine(output,"second","zoom.vmx.json");Directory.CreateDirectory(Path.GetDirectoryName(second)!);
loaded.PrepareSave=(value,_)=>value with{Name="prepared"};loaded.Save(second);
var savedAs=Fresh();savedAs.Load(second);
Check(Zoom(savedAs,p)==18&&Zoom(savedAs,duplicate)==1&&savedAs.Document.Name=="prepared","PrepareSave and cross-directory SaveAs retain zoom");
var json=JsonNode.Parse(File.ReadAllText(second))!;
foreach(var item in json["Procedures"]!.AsArray())item!.AsObject().Remove("CanvasZoomIndex");
string legacy=Path.Combine(output,"legacy.vmx.json");File.WriteAllText(legacy,json.ToJsonString());savedAs.Load(legacy);
Check(savedAs.Document.Procedures.All(x=>x.CanvasZoomIndex==9),"Legacy JSON missing property receives default 9");
foreach(int invalid in new[]{-1,20}){
  json["Procedures"]![0]!["CanvasZoomIndex"]=invalid;
  string bad=Path.Combine(output,"bad-"+invalid+".vmx.json");File.WriteAllText(bad,json.ToJsonString());
  string before=JsonSerializer.Serialize(savedAs.Document);string? identity=savedAs.FilePath;
  Reject<InvalidDataException>(()=>savedAs.Load(bad),"Load rejects out of range index "+invalid);
  Check(JsonSerializer.Serialize(savedAs.Document)==before&&savedAs.FilePath==identity,"Failed Load preserves current document "+invalid);
}
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="independent document viewport adapter; no native SDK or manual acceptance"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("PASS "+checks.Count+" checks; audit="+output);
