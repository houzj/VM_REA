using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

if(args.Length!=1)throw new ArgumentException("Supply an owned audit directory.");
string output=Path.Combine(Path.GetFullPath(args[0]),"configuration-generation-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(output);
var checks=new List<string>();void Check(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);}
var doc=new GraphEditorDocument(new ModuleRegistry());string procedure=doc.CurrentProcedureId;
string file=Path.Combine(output,"same-id.vmx.json");doc.Save(file);string solution=doc.SolutionId;
doc.ExecuteConfigurationEdit(outer=>{
  outer.RenameProcedure(procedure,"old-scope-first");
  outer.Load(file);Check(outer.SolutionId==solution&&outer.CurrentProcedureId==procedure,"Load inside configuration preserves same IDs but replaces document instance");
  outer.ExecuteConfigurationEdit(inner=>{inner.RenameProcedure(procedure,"new-inner-first");inner.RenameProcedure(procedure,"new-inner-last");});
  Check(outer.CurrentProcedure.Name=="new-inner-last","New-generation configuration writes commit normally");
});
doc.RenameProcedure(procedure,"after-old-finally");doc.Undo();
Check(doc.CurrentProcedure.Name=="new-inner-last","Old-generation finally cannot suppress subsequent normal history");
doc.Undo();Check(doc.CurrentProcedure.Name=="Main"&&!doc.CanUndo,"New-generation inner writes share one root and never resurrect pre-load old root");
doc.Redo();Check(doc.CurrentProcedure.Name=="new-inner-last","New-generation grouped redo restores final inner value");
doc.Redo();Check(doc.CurrentProcedure.Name=="after-old-finally","Later ordinary action remains separately redoable");

doc.Load(file);bool escaped=false;
try{doc.ExecuteConfigurationEdit(outer=>{
  outer.RenameProcedure(procedure,"old-failing-scope");outer.Load(file);
  outer.ExecuteConfigurationEdit(inner=>{inner.RenameProcedure(procedure,"kept-new");inner.RenameProcedure(procedure,"kept-final");});
  throw new InvalidOperationException("old-generation-fixture");
});}catch(InvalidOperationException error)when(error.Message=="old-generation-fixture"){escaped=true;}
Check(escaped&&doc.CurrentProcedure.Name=="kept-final","Old scope error propagates without rolling back new-generation writes");
doc.RenameProcedure(procedure,"normal-after-error");doc.Undo();Check(doc.CurrentProcedure.Name=="kept-final","Old failed scope finally does not leak forbidden state");
doc.Undo();Check(doc.CurrentProcedure.Name=="Main"&&!doc.CanUndo,"Old failed scope history cannot cross same-ID Load boundary");
doc.Load(file);
doc.ExecuteConfigurationEdit(outer=>{outer.RenameProcedure(procedure,"before-new");outer.NewSolution();outer.ExecuteConfigurationEdit(inner=>{inner.RenameProcedure(inner.CurrentProcedureId,"fresh-a");inner.RenameProcedure(inner.CurrentProcedureId,"fresh-b");});});
doc.Undo();Check(doc.CurrentProcedure.Name=="Main"&&!doc.CanUndo,"NewSolution inside old configuration also isolates grouped history generation");
File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,scope="actual Core document and same-ID configuration history generation; independent main adapter, no SDK"},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("PASS "+checks.Count+" configuration-generation checks; audit="+output);
