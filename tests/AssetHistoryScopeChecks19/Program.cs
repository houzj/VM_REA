using System.IO;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.Calibration;

string? auditOutput=null;
try{
if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"asset-history-scope-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
auditOutput=output;
AppContext.SetSwitch("VisionEditor.DisableAutomaticCameraDiscovery",true);
var checks=new List<string>();void Check(bool condition,string label){if(!condition)throw new Exception(label);checks.Add(label);Console.WriteLine("PASS characterization: "+label);}
var fixture=new StatefulAssetProvider();var modules=new ModuleRegistry();var plugins=new ToolPluginRegistry();plugins.Register(fixture,modules);
using var scope=new ModuleExecutionScope();
var node=new NodeDocument("asset",fixture.Manifest.Id,"1","asset",new(){["OpaquePoint"]=JsonSerializer.SerializeToElement(11d)},10,10);
var document=new SolutionDocument(SolutionStore.CurrentVersion,"same-solution","fixture",[new("p","Main",[node],[])],[]);
fixture.LiveValue=11;var captured=plugins.PrepareSave(document,output,scope);
Check(captured.Procedures.Single().Nodes.Single().Parameters["OpaquePoint"].GetDouble()==11,"Public provider freezes first live value before later state changes");
fixture.LiveValue=99;
var historical=plugins.PrepareSave(captured,output,scope);
Check(historical.Procedures.Single().Nodes.Single().Parameters["OpaquePoint"].GetDouble()==99,"Historical PrepareSave with future state overwrites captured opaque value");
Check(captured.Procedures.Single().Nodes.Single().Parameters["OpaquePoint"].GetDouble()==11,"Original captured document remains immutable while re-preparation produces wrong replacement");
var withoutScope=plugins.PrepareSave(captured,output);
Check(withoutScope.Procedures.Single().Nodes.Single().Parameters["OpaquePoint"].GetDouble()==99,"Three-argument public interface alone does not guarantee state-free preparation");

var calibration=new NPointPlugin();var values=calibration.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
values["ImagePointX0"]=JsonSerializer.SerializeToElement(11d);
var calibrationNode=new NodeDocument("actual-calibration",calibration.Manifest.Id,calibration.Manifest.Version,"calibration",values,10,10);
var calibrationScope=scope.ForProcedure("calibration-procedure");
ToolEditorContext Context(NodeDocument n)=>new(n,output,_=>{},_=>throw new InvalidOperationException("No execution permitted in static-state fixture")){Scope=calibrationScope};
calibration.UpdateConfiguration(Context(calibrationNode),calibrationNode.Parameters);
var resources=new ResourceCatalog(output,[]);
var first=calibration.PrepareSave(calibrationNode,output,resources,calibrationScope);
Check(first.Node.Parameters["ImagePointX0"].GetDouble()==11,"Actual main calibration public configuration and preparation store old value");
var future=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());future["ImagePointX0"]=JsonSerializer.SerializeToElement(99d);
calibration.UpdateConfiguration(Context(calibrationNode with{Parameters=future}),future);
var oldSnapshot=calibrationNode with{Parameters=values.ToDictionary(p=>p.Key,p=>p.Value.Clone())};
var repeated=calibration.PrepareSave(oldSnapshot,output,resources,calibrationScope);
Check(repeated.Node.Parameters["ImagePointX0"].GetDouble()==99,"Actual main calibration current scope injects future point into old node during historical preparation");
Check(oldSnapshot.Parameters["ImagePointX0"].GetDouble()==11,"Actual old calibration parameters were not edited by the fixture");
Check(calibration.Snapshot(Context(oldSnapshot))[0,0]==99,"Public runtime snapshot confirms scope remains future while document is old");
var noScope=calibration.PrepareSave(oldSnapshot,output,resources);
Check(noScope.Node.Parameters["ImagePointX0"].GetDouble()==11,"Actual calibration no-scope overload currently preserves old node but is not a generic interface purity guarantee");
var calibrationModules=new ModuleRegistry();var calibrationPlugins=new ToolPluginRegistry();calibrationPlugins.Register(calibration,calibrationModules);
string calFile=Path.Combine(output,"calibration-old.vmx.json");
SolutionStore.Save(calFile,new(SolutionStore.CurrentVersion,"calibration-history","calibration-history",[new("calibration-procedure","Main",[oldSnapshot],[])],[]));
using var editor=new GraphEditorDocument(calibrationModules){PrepareSave=(current,baseDirectory)=>calibrationPlugins.PrepareSave(current,baseDirectory,scope)};
editor.Load(calFile);editor.RenameProcedure("calibration-procedure","changed");editor.Undo();
Check(editor.CurrentProcedure.Nodes.Single().Parameters["ImagePointX0"].GetDouble()==11,"Actual main document Undo restores old persisted calibration point");
string savedCal=Path.Combine(output,"later","calibration-saved.vmx.json");editor.Save(savedCal);
Check(editor.CurrentProcedure.Nodes.Single().Parameters["ImagePointX0"].GetDouble()==99,"Actual main Save after Undo takes future scope state through unchanged public provider chain");
var evidence=new{passedCharacterization=checks.Count,checks,expectedUnsafeBehaviorObserved=true,
  fixture=new{capturedValue=11,futurePreparedValue=historical.Procedures.Single().Nodes.Single().Parameters["OpaquePoint"].GetDouble()},
  actualCalibration=new{oldDocument=oldSnapshot.Parameters["ImagePointX0"].GetDouble(),futureScope=calibration.Snapshot(Context(oldSnapshot))[0,0],repreparedOld=repeated.Node.Parameters["ImagePointX0"].GetDouble(),actualDocumentSavedAfterUndo=editor.CurrentProcedure.Nodes.Single().Parameters["ImagePointX0"].GetDouble()},
  scope="Public provider counterexample and actual main NPoint public managed state only. No tool execution, SDK, camera, file format export, plugin edits or claim of fixed D02.2."};
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(evidence,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("OBSERVED "+checks.Count+" scope/history counterexample checks; D02.2 remains blocked; audit="+output);
return 0;
}catch(Exception error){
 try{if(auditOutput is not null)File.WriteAllText(Path.Combine(auditOutput,"failed.json"),JsonSerializer.Serialize(new{failed=true,error=error.ToString(),scope="characterization fixture failure, not fixed or accepted D02.2"},new JsonSerializerOptions{WriteIndented=true}));}catch(Exception diagnostic){Console.Error.WriteLine("Failure diagnostic also failed: "+diagnostic);}
 Console.Error.WriteLine(error);return 1;
}

sealed class StatefulAssetProvider:IVisionToolPlugin,IToolAssetPersistence
{
 public double LiveValue{get;set;}
 public ToolManifest Manifest{get;}=new("test.stateful-asset","1","stateful-asset","test","");
 public ModuleDescriptor Descriptor{get;}=new("test.stateful-asset","1","stateful-asset",[],[],[new("OpaquePoint",ParameterKind.Number,true,JsonSerializer.SerializeToElement(11d))]);
 public IModule CreateRuntime()=>throw new InvalidOperationException("Fixture runtime must not execute");
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Fixture editor must not open");
 public void Reset(string nodeId){}
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources){var parameters=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());parameters["OpaquePoint"]=JsonSerializer.SerializeToElement(LiveValue);return new(node with{Parameters=parameters},[]);}
}
