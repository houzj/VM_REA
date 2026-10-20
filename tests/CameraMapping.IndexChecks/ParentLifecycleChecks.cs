using System.IO;
using System.Reflection;
using System.Windows;
using VisionEditor;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.CameraMapping;

internal static class ParentLifecycleChecks {
 const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static T Field<T>(object target,string name)=>(T)target.GetType().GetField(name,Private)!.GetValue(target)!;
 static object? Call(object target,string name,params object[] args)=>target.GetType().GetMethod(name,Private)!.Invoke(target,args);
 internal static void Run(Action<bool,string> check,string output){
  var shell=new MainWindow{Left=-20000,Top=-20000,ShowActivated=false,ShowInTaskbar=false};shell.Show();
  try{
   var doc=Field<GraphEditorDocument>(shell,"doc");var registry=Field<ModuleRegistry>(shell,"modules");doc.NewSolution();var producer=new Source();registry.Register(producer.Descriptor,()=>producer);string source=doc.AddNode(producer.Descriptor.Id,"1",0,0),target=doc.AddNode(CameraMappingPlugin.Id,"1.0.0",200,0),procedure=doc.CurrentProcedureId;
   doc.ConnectExecution(source,target);
   ToolEditorContext Context(string id){var node=doc.CurrentProcedure.Nodes.Single(n=>n.Id==id);return (ToolEditorContext)Call(shell,"ConnectToolEditorServices",new ToolEditorContext(node,output,next=>doc.SetParameters(id,next),_=>throw new InvalidOperationException()),id)!;}
   ConfigurationSubscriptionDestination Combination()=>new(ConfigurationSubscriptionDomain.Combination,"ObjectPointInput1","POINT");ConfigurationSubscriptionDestination Axis(string axis)=>new(ConfigurationSubscriptionDomain.InputIo,"ObjectPoint"+axis+"1","float");
   var context=Context(target);var service=context.ConfigurationSubscriptions!;var reference=new ConfigurationOutputReference(procedure,source,"Points","POINT");
   check(service.Bind(Combination(),new(reference,ConfigurationSubscriptionIndex.At(1))).Status==ConfigurationSubscriptionStatus.Available,"parent lifecycle combination sets parent and X/Y index1");
   foreach(string axis in new[]{"X","Y"}){var binding=service.Read(Axis(axis)).Binding!;check(service.Bind(Axis(axis),binding with{Index=ConfigurationSubscriptionIndex.All}).Status==ConfigurationSubscriptionStatus.Available,"parent lifecycle independently clears "+axis);}
   check(doc.CurrentProcedure.Configuration.Subscriptions.All(b=>b.Source.ArrayIndex is null),"parent lifecycle leaf runtime selectors really All");check(service.Read(Combination()).Binding?.Index==ConfigurationSubscriptionIndex.At(1),"both leaf All fall back to parent local index1");
   var reopened=Context(target).ConfigurationSubscriptions!;check(reopened.Read(Combination()).Binding?.Index==ConfigurationSubscriptionIndex.At(1),"new configuration provider reuses same live module parent cache");
   // Paste reads live parent summary at paste time, not stale clipboard capture.
   var fragment=doc.CopyNodes(new[]{target});var copied=doc.PasteNodes(fragment,400,0).Single();var cloned=doc.CurrentProcedure.Configuration.Subscriptions.Where(b=>b.NodeId==copied).ToArray();check(cloned.Length==2&&cloned.All(b=>b.Source.ArrayIndex?.LiteralText=="1"),"actual host paste reconstructs coherent parent and synchronizes clone leaf index1");var cloneView=Context(copied).ConfigurationSubscriptions!;foreach(string axis in new[]{"X","Y"}){var binding=cloneView.Read(Axis(axis)).Binding!;check(cloneView.Bind(Axis(axis),binding with{Index=ConfigurationSubscriptionIndex.All}).Status==ConfigurationSubscriptionStatus.Available,"clear cloned leaf "+axis);}check(cloneView.Read(Combination()).Binding?.Index==ConfigurationSubscriptionIndex.At(1),"clone receives its own nonpersistent parent cache");
   // Combination All explicitly clears the parent cache; leaf-only clears did not.
   check(reopened.Bind(Combination(),new(reference,ConfigurationSubscriptionIndex.All)).Status==ConfigurationSubscriptionStatus.Available,"explicit combination All updates parent cache");check(reopened.Read(Combination()).Binding?.Index.Kind==ConfigurationSubscriptionIndexKind.All,"parent cache cleared only by combination index setter");
   // The cached module field is deliberately outside document Undo. Leaf Undo is
   // the product's safe typed-document adaptation, not native SetAllSubscribe.
   var localStates=Field<Dictionary<(string Procedure,string Module,string Node),PointAliasPresentationState>>(shell,"pointAliasPresentation");doc.Undo();check(localStates[(procedure,CameraMappingPlugin.Id,target)].Read("ObjectPointInput1").Kind==ConfigurationSubscriptionIndexKind.All,"Undo does not fabricate a parent-field snapshot outside document data");doc.Redo();
   // Save/load produces new module objects: restoring All leaves does not import
   // old private parent cache. The live clone's cache is intentionally discarded.
   string path=Path.Combine(output,"parent-cache-reload.solution.json");doc.Save(path);doc.Load(path);check(Context(copied).ConfigurationSubscriptions!.Read(Combination()).Binding?.Index.Kind==ConfigurationSubscriptionIndexKind.All,"solution load restores only persisted leaf state and clears private parent cache");
   var restored=Context(target).ConfigurationSubscriptions!;check(restored.Bind(Combination(),new(reference,ConfigurationSubscriptionIndex.At(1))).Status==ConfigurationSubscriptionStatus.Available,"seed parent before remove/undo");foreach(string axis in new[]{"X","Y"}){var binding=restored.Read(Axis(axis)).Binding!;restored.Bind(Axis(axis),binding with{Index=ConfigurationSubscriptionIndex.All});}doc.DeleteNode(target);doc.Undo();check(Context(target).ConfigurationSubscriptions!.Read(Combination()).Binding?.Index.Kind==ConfigurationSubscriptionIndexKind.All,"node removal prunes parent cache; recreated node Undo starts with leaf-only All");
   var swapped=Context(target).ConfigurationSubscriptions!;check(swapped.Bind(Axis("X"),new(new ConfigurationOutputReference(procedure,source,"Points.Y","float"),ConfigurationSubscriptionIndex.At(0))).Status==ConfigurationSubscriptionStatus.Available&&swapped.Bind(Axis("Y"),new(new ConfigurationOutputReference(procedure,source,"Points.X","float"),ConfigurationSubscriptionIndex.At(1))).Status==ConfigurationSubscriptionStatus.Available,"host swapped distinct same-parent source leaves");var swappedCopy=doc.PasteNodes(doc.CopyNodes(new[]{target}),600,0).Single();var swappedLeaves=doc.CurrentProcedure.Configuration.Subscriptions.Where(b=>b.NodeId==swappedCopy).ToDictionary(b=>b.Target,b=>b.Source);check(swappedLeaves["ObjectPointX1"].Port=="Points.X"&&swappedLeaves["ObjectPointY1"].Port=="Points.Y"&&swappedLeaves.Values.All(v=>v.ArrayIndex?.LiteralText=="0"),"native parent reconstruction clone repositions X/Y and applies first target-child index to both");
  }finally{Call(shell,"CloseToolConfiguration");shell.Close();}
 }
 sealed class Source:IModule {
  public ModuleDescriptor Descriptor{get;}=new("camera-parent-source","1","Parent source",[],[new PortDescriptor("Points",DataKind.Point){Cardinality=ValueCardinality.Array},new("Index",DataKind.Integer,false)],[]);
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken token)=>throw new InvalidOperationException("Only source binding identity is required here.");
 }
}
