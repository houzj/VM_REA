using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using VisionEditor.Core;
using VisionFramework;
using VisionModules;

// Current independent format only. No native VM or plugin-worktree execution.
if(args.Length!=1)throw new ArgumentException("Supply audit directory.");
string root=Path.Combine(Path.GetFullPath(args[0]),"storage-boundary-"+Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);const int limit=32*1024*1024;
var labels=new List<string>();var failures=new List<object>();bool passed=false;
void Check(bool value,string label){if(!value)throw new Exception(label);labels.Add(label);Console.WriteLine("PASS "+label);}
try{
    var node=new NodeDocument("n","vision.image-file","1.0.0","source",new(){["Path"]=JsonSerializer.SerializeToElement("image.bmp"),["Opaque"]=JsonSerializer.SerializeToElement(new{FutureVendorField=12})},10,20);
    var doc=new SolutionDocument(1,"boundary","boundary",[new("p","initial",[node],[])],[]);
    string original=Path.Combine(root,"original.vmx.json");SolutionStore.Save(original,doc);string valid=File.ReadAllText(original);
    var registry=new ModuleRegistry();BuiltinModules.RegisterAll(registry);var editor=new GraphEditorDocument(registry);editor.Load(original);
    editor.RenameProcedure("p","first");editor.RenameProcedure("p","second");editor.Undo();
    string State()=>JsonSerializer.Serialize(new{editor.Document,editor.BaseDirectory,editor.FilePath,editor.Revision,editor.IsDirty,editor.CanUndo,editor.CanRedo,editor.CurrentProcedureId});
    string before=State();int changed=0,replaced=0;editor.Changed+=()=>changed++;editor.SolutionReplaced+=()=>replaced++;
    void Reject(string name,string text,Type expected){
        string path=Path.Combine(root,name+".json");File.WriteAllText(path,text,new UTF8Encoding(false));Exception? error=null;
        try{editor.Load(path);}catch(Exception e){error=e;}
        failures.Add(new{name,type=error?.GetType().FullName,error=error?.Message});
        Check(error is not null&&expected.IsInstanceOfType(error),name+": expected rejection type");
        Check(State()==before&&changed==0&&replaced==0,name+": full editor/history/selection state and events unchanged");
    }
    JsonObject Root()=>JsonNode.Parse(valid)!.AsObject();
    var version=Root();version["FormatVersion"]=2;Reject("future-version",version.ToJsonString(),typeof(InvalidDataException));
    var unknown=Root();unknown["Procedures"]![0]!["FutureField"]=3;Reject("unknown-procedure-field",unknown.ToJsonString(),typeof(JsonException));
    var missing=Root();missing["Procedures"]![0]!["Nodes"]![0]!["ModuleVersion"]=null;Reject("missing-module-version",missing.ToJsonString(),typeof(InvalidDataException));
    string duplicate=valid.Replace("\"FutureVendorField\": 12","\"FutureVendorField\": 12, \"FutureVendorField\": 13");
    Check(duplicate!=valid,"duplicate parameter fixture actually differs");Reject("duplicate-private-parameter-field",duplicate,typeof(InvalidDataException));
    string deep="0";for(int i=0;i<70;i++)deep="{\"x\":"+deep+"}";
    Reject("deep-private-parameter",valid.Replace("\"FutureVendorField\": 12","\"FutureVendorField\": "+deep),typeof(JsonException));
    // File-length limit is checked before JSON parsing; preserve both exact-boundary files.
    string atLimit=Path.Combine(root,"exact-limit.json");byte[] prefix=Encoding.UTF8.GetBytes(valid);
    using(var stream=File.Create(atLimit)){stream.Write(prefix);byte[] spaces=Enumerable.Repeat((byte)' ',65536).ToArray();while(stream.Length<limit)stream.Write(spaces,0,(int)Math.Min(spaces.Length,limit-stream.Length));}
    Check(SolutionStore.Load(atLimit).Id==doc.Id,"exact 32MiB input including whitespace accepted");
    string overLimit=Path.Combine(root,"over-limit.json");File.Copy(atLimit,overLimit);using(var stream=new FileStream(overLimit,FileMode.Append))stream.WriteByte((byte)' ');
    Exception? sizeError=null;try{editor.Load(overLimit);}catch(Exception e){sizeError=e;}failures.Add(new{name="over-limit",type=sizeError?.GetType().FullName,error=sizeError?.Message});
    Check(sizeError is InvalidDataException,"32MiB plus one input rejected");Check(State()==before&&changed==0&&replaced==0,"oversize load preserves dirty undo/redo state without notification");
    // Arbitrary JSON inside the parameter value is an opaque payload, unlike typed document fields.
    var opaque=SolutionStore.Load(original).Procedures[0].Nodes[0].Parameters["Opaque"];
    Check(opaque.GetProperty("FutureVendorField").GetInt32()==12,"unknown vendor payload remains opaque and roundtrips");
    string exactSave=Path.Combine(root,"exact-save.json");
    SolutionDocument Padded(int length)=>doc with{Name=new string('a',length)};
    SolutionStore.Save(exactSave,Padded(1));int fixedBytes=(int)new FileInfo(exactSave).Length-1;
    SolutionStore.Save(exactSave,Padded(limit-fixedBytes));
    Check(new FileInfo(exactSave).Length==limit,"serialized output exactly 32MiB accepted");
    string exactHash=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(exactSave)));
    Exception? saveError=null;try{SolutionStore.Save(exactSave,Padded(limit-fixedBytes+1));}catch(Exception e){saveError=e;}
    failures.Add(new{name="oversize-save",type=saveError?.GetType().FullName,error=saveError?.Message});
    Check(saveError is InvalidDataException,"serialized output 32MiB plus one rejected");
    Check(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(exactSave)))==exactHash&&!Directory.EnumerateFiles(root,".exact-save.json.*.tmp").Any(),"oversize save preserves target bytes and creates no temporary file");
    int prepared=0;editor.PrepareSave=(document,baseDirectory)=>{prepared++;return document with{Name=new string('a',limit)};};
    Exception? preparedError=null;try{editor.Save(original);}catch(Exception e){preparedError=e;}
    failures.Add(new{name="oversize-prepared-save",type=preparedError?.GetType().FullName,error=preparedError?.Message});
    Check(prepared==1&&preparedError is InvalidDataException,"oversize prepared document rejected after preparation");
    Check(State()==before&&changed==0&&replaced==0&&File.ReadAllText(original)==valid,"failed prepared save keeps editor/history/events and original target unchanged");
    editor.PrepareSave=null;editor.Redo();Check(editor.CurrentProcedure.Name=="second","redo remains operational after all rejected loads and save");
    editor.Undo();editor.Undo();Check(editor.CurrentProcedure.Name=="initial","both undo entries remain operational after rejections");
    passed=true;
}finally{
    File.WriteAllText(Path.Combine(root,"result.json"),JsonSerializer.Serialize(new{passed,checks=labels.Count,labels,failures,authority="independent main format/editor boundary operations; not native VM",root},new JsonSerializerOptions{WriteIndented=true}));
    Console.WriteLine($"passed={passed}; checks={labels.Count}; evidence={root}");
}
