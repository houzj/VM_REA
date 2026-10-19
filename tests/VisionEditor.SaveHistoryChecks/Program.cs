using System.Security.Cryptography;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionModules;

// Independent vmx.json/editor transaction checks, not native .sol evidence.
if(args.Length!=1)throw new ArgumentException("Supply an audit output directory.");
string root=Path.Combine(Path.GetFullPath(args[0]),"save-history-"+Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);int checks=0;var labels=new List<string>();
void Check(bool value,string label){if(!value)throw new Exception(label);checks++;labels.Add(label);Console.WriteLine("PASS "+label);}
string oldDir=Path.Combine(root,"original"),firstDir=Path.Combine(root,"first"),secondDir=Path.Combine(root,"second");
Directory.CreateDirectory(Path.Combine(oldDir,"assets"));Directory.CreateDirectory(firstDir);Directory.CreateDirectory(secondDir);
byte[] payload=[1,17,33,49,65];string asset=Path.Combine(oldDir,"assets","sample.bin");File.WriteAllBytes(asset,payload);
string digest=Convert.ToHexString(SHA256.HashData(payload));
var node=new NodeDocument("source","vision.image-file","1.0.0","source",new(){["Path"]=JsonSerializer.SerializeToElement("assets/sample.bin")},20,30);
var original=new SolutionDocument(SolutionStore.CurrentVersion,"history-assets","history-assets",[new("p","original",[node],[])],
    [new("disk","assets/sample.bin",digest),new ResourceDocument("embedded","virtual.bin",digest){EmbeddedBase64=Convert.ToBase64String(payload)}]);
string oldFile=Path.Combine(oldDir,"original.vmx.json"),firstFile=Path.Combine(firstDir,"first.vmx.json"),secondFile=Path.Combine(secondDir,"second.vmx.json");
SolutionStore.Save(oldFile,original);byte[] originalBytes=File.ReadAllBytes(oldFile);
var registry=new ModuleRegistry();BuiltinModules.RegisterAll(registry);var editor=new GraphEditorDocument(registry);editor.Load(oldFile);
void VerifyAssets(string phase){
    var document=editor.Document;var catalog=new ResourceCatalog(editor.BaseDirectory,document.Resources);
    foreach(string id in new[]{"disk","embedded"}){using var stream=catalog.OpenVerified(id);using var bytes=new MemoryStream();stream.CopyTo(bytes);Check(bytes.ToArray().SequenceEqual(payload),phase+": "+id+" resource bytes");}
    var path=document.Procedures.Single().Nodes.Single().Parameters["Path"].GetString()!;
    Check(Path.GetFullPath(path,editor.BaseDirectory)==asset,phase+": built-in image path retains original asset identity");
}
editor.RenameProcedure("p","edited");editor.Save(firstFile);VerifyAssets("first save-as");
editor.Undo();
Check(editor.FilePath==firstFile&&editor.BaseDirectory==firstDir&&editor.CurrentProcedure.Name=="original"&&editor.IsDirty&&editor.CanRedo,"undo after save-as keeps current file identity and restores edit");
VerifyAssets("undo across old base");
editor.Save(secondFile);
Check(editor.CanRedo&&!editor.IsDirty,"second save-as retains redo history");
editor.Redo();
Check(editor.FilePath==secondFile&&editor.BaseDirectory==secondDir&&editor.CurrentProcedure.Name=="edited"&&editor.IsDirty,"redo across second save-as uses current file identity");
VerifyAssets("redo across first base");
editor.Undo();long revision=editor.Revision;string state=JsonSerializer.Serialize(editor.Document);byte[] firstBefore=File.ReadAllBytes(firstFile);
using(var locked=new FileStream(firstFile,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){
    bool failed=false;try{editor.Save(firstFile);}catch(IOException){failed=true;}catch(UnauthorizedAccessException){failed=true;}
    Check(failed,"locked alternate save-as fails");
    Check(editor.FilePath==secondFile&&editor.BaseDirectory==secondDir&&editor.Revision==revision&&editor.CanRedo&&editor.IsDirty&&JsonSerializer.Serialize(editor.Document)==state,"failed alternate save-as preserves document, base and redo");
}
Check(File.ReadAllBytes(firstFile).SequenceEqual(firstBefore),"failed alternate save-as preserves target bytes");
Check(!Directory.EnumerateFiles(firstDir,".first.vmx.json.*.tmp").Any(),"failed alternate save-as leaves no temporary file");
editor.Redo();VerifyAssets("redo after failed save-as");
Check(editor.CurrentProcedure.Name=="edited"&&editor.FilePath==secondFile,"redo still works after failed alternate save-as");
editor.Save(secondFile);var reopened=SolutionStore.OpenWorkspace(secondFile);
using(var stream=new ResourceCatalog(reopened.BaseDirectory,reopened.Document.Resources).OpenVerified("disk"))Check(stream.ReadByte()==payload[0],"final saved resource opens from reopened base");
Check(reopened.Document.Procedures.Single().Name=="edited"&&File.ReadAllBytes(oldFile).SequenceEqual(originalBytes),"final save preserves original file and stores edited name");
Check(File.ReadAllBytes(asset).SequenceEqual(payload),"save/history operations do not rewrite source asset");
File.WriteAllText(Path.Combine(root,"result.json"),JsonSerializer.Serialize(new{checks,labels,root,authority="independent vmx/editor runtime; not VM native parity"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"PASS {checks} save/history/resource checks. Evidence: {root}");
