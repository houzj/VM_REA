using System.Text.Json;
using VisionFramework;
using VisionEditor.Core;
try {
int count=0;
void Check(bool valid,string name){if(!valid)throw new Exception(name);count++;}
void Reject(Action action,string name){try{action();}catch(InvalidOperationException){count++;return;}throw new Exception(name);}
var registry=new ModuleRegistry();
void Register(string id,DataKind kind,bool input){var d=new ModuleDescriptor(id,"1",id,input?[new("In",kind)]:[],[new("Out",kind)],[new("Value",ParameterKind.Text,false)]);registry.Register(d,()=>throw new NotSupportedException());}
Register("number",DataKind.Number,false);Register("pipe",DataKind.Number,true);Register("text",DataKind.Text,true);Register("vision.image-file",DataKind.GrayImage,false);
var editor=new GraphEditorDocument(registry);Check(editor.IsDirty&&editor.CurrentProcedure.Id=="main","new");
var a=editor.AddNode("number","1",0,0);var b=editor.AddNode("pipe","1",100,0);var c=editor.AddNode("pipe","1",200,0);var t=editor.AddNode("text","1",0,100);
editor.Connect(a,"Out",b,"In");Check(editor.CurrentProcedure.Connections.Count==1,"connect unfinished graph");
editor.Undo();Check(editor.CurrentProcedure.Connections.Count==0,"undo connection");editor.Redo();Check(editor.CurrentProcedure.Connections.Count==1,"redo connection");
Reject(()=>editor.Connect(a,"Out",t,"In"),"type mismatch");Reject(()=>editor.Connect(c,"Out",b,"In"),"single binding");Reject(()=>editor.Connect(a,"In",c,"In"),"port direction");
editor.Connect(b,"Out",c,"In");editor.Disconnect(editor.CurrentProcedure.Connections.Single(x=>x.ToNode==b));Reject(()=>editor.Connect(c,"Out",b,"In"),"cycle");
editor.Connect(a,"Out",b,"In");editor.DeleteNode(b);Check(editor.CurrentProcedure.Connections.Count==0,"delete incident edges");editor.Undo();Check(editor.CurrentProcedure.Connections.Count==2,"undo delete restores edges");
using(var json=JsonDocument.Parse("\"hello\"")){editor.SetParameter(a,"Value",json.RootElement);}Check(editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Parameters["Value"].GetString()=="hello","json clone disposed source");
var snapshot=editor.Document;snapshot.Procedures[0].Nodes.Single(n=>n.Id==a).Parameters.Clear();Check(editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Parameters.Count==1,"defensive snapshot");
editor.SetParameter(a,"Value",JsonSerializer.SerializeToElement("second"));editor.Undo();Check(editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Parameters["Value"].GetString()=="hello","undo parameter deep snapshot");
editor.UpdateNode(a,"renamed",false,100,new(){["Value"]=JsonSerializer.SerializeToElement("batch")});
editor.Undo();Check(editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Name=="number"&&editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Parameters["Value"].GetString()=="hello","batch edit one history item");
try{editor.SetParameters(a,new(){["Value"]=JsonSerializer.SerializeToElement(123)});throw new Exception("accepted wrong type");}catch(ArgumentException){count++;}
try{editor.SetParameters(a,new(){["Unknown"]=JsonSerializer.SerializeToElement("x")});throw new Exception("accepted unknown key");}catch(ArgumentException){count++;}
string temp=Path.Combine(Path.GetTempPath(),"vision-editor-check-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(temp);
try{
 string first=Path.Combine(temp,"first","plan.json"),second=Path.Combine(temp,"second","plan.json");
 var imageDescriptor=new ModuleDescriptor("vision.image-file","2","image",[],[new("Image",DataKind.GrayImage)],[new("Path",ParameterKind.Text,true)]);registry.Register(imageDescriptor,()=>throw new NotSupportedException());
 string image=Path.Combine(temp,"image.bmp");File.WriteAllText(image,"placeholder");
 var n=editor.AddNode("vision.image-file","2",0,0,new(){["Path"]=JsonSerializer.SerializeToElement(Path.GetRelativePath(editor.BaseDirectory,image))});
 editor.Save(first);Check(!editor.IsDirty&&editor.FilePath==first,"save dirty");editor.MoveNode(n,10,10);editor.Save(second);
 string Resolved()=>Path.GetFullPath(editor.CurrentProcedure.Nodes.Single(x=>x.Id==n).Parameters["Path"].GetString()!,editor.BaseDirectory);
 Check(Resolved()==image,"saveas source rebasing");editor.Undo();Check(Resolved()==image&&editor.IsDirty,"undo after saveas");editor.Redo();Check(Resolved()==image,"redo after saveas");
 var before=editor.Document;long revision=editor.Revision;try{editor.Save(temp);}catch(Exception ex) when(ex is IOException or UnauthorizedAccessException){}Check(editor.Revision==revision&&editor.FilePath==second,"failed save leaves state");
 var procedure=editor.AddProcedure("Second");Check(editor.CurrentProcedureId==procedure&&editor.Document.Procedures.Count==2,"add procedure selection");editor.SelectProcedure("main");editor.Save(second);Check(!editor.IsDirty,"save clear dirty");editor.SelectProcedure(procedure);Check(!editor.IsDirty,"selection does not dirty");editor.Load(second);Check(!editor.IsDirty&&!editor.CanUndo&&editor.Document.Procedures.Count==2,"load resets history");
  var selected=editor.CurrentProcedureId; editor.AddProcedure("Third");editor.Undo();Check(editor.CurrentProcedureId==selected&&editor.Document.Procedures.Count==2,"undo add procedure selection");editor.Redo();Check(editor.Document.Procedures.Count==3&&editor.CurrentProcedure.Name=="Third","redo add procedure selection");editor.SelectProcedure("main");
 // Deep-copy polygon vertices, including history snapshots.
 var roiDoc=editor.Document;
 var roiProc=roiDoc.Procedures.Single(p=>p.Id=="main");
 var roiNodes=roiProc.Nodes.Select(node=>node.Id==a?node with {Rois=[new RoiDocument("polygon",RoiKind.Polygon,0,0,Vertices:[new(1,1),new(2,1),new(2,2)])]}:node).ToArray();
 roiDoc=roiDoc with {Procedures=roiDoc.Procedures.Select(p=>p.Id=="main"?p with {Nodes=roiNodes}:p).ToArray()};SolutionStore.Save(second,roiDoc);editor.Load(second);
 var roiSnapshot=editor.Document;((VisionCore.Point2[])roiSnapshot.Procedures.Single(p=>p.Id=="main").Nodes.Single(n=>n.Id==a).Rois![0].Vertices!)[0]=new(999,999);
 Check(editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Rois![0].Vertices![0].X==1,"ROI snapshot isolated");editor.SetNodeName(a,"ROI edit");editor.Undo();Check(editor.CurrentProcedure.Nodes.Single(n=>n.Id==a).Rois![0].Vertices![0].X==1,"ROI preserved through undo");
 // Resource path preservation is checked through a loaded file as resources have no edit command yet.
 var resourcePlan=editor.Document with {Resources=[new("image",Path.GetRelativePath(editor.BaseDirectory,image))]};SolutionStore.Save(second,resourcePlan);editor.Load(second);editor.Save(first);Check(Path.GetFullPath(editor.Document.Resources[0].Path,editor.BaseDirectory)==image,"resource saveas rebasing");
}finally{Directory.Delete(temp,true);}
var separated=new GraphEditorDocument(registry);var sa=separated.AddNode("number","1",0,0);var sb=separated.AddNode("pipe","1",0,100);var sc=separated.AddNode("pipe","1",0,200);
separated.ConnectExecution(sa,sb);separated.ConnectExecution(sb,sc);Check(separated.CurrentProcedure.Connections.Count==0,"execution edit does not create subscription");
separated.BindInput(sa,"Out",sb,"In");separated.BindInput(sa,"Out",sc,"In");Check(separated.CurrentProcedure.ExecutionLinks!.Count==2&&separated.Validate().IsValid,"data subscription can use nonadjacent execution ancestor");
separated.DisconnectExecution(new(sb,sc));Check(separated.CurrentProcedure.Connections.Count==2&&separated.Validate().Issues.Any(i=>i.Code=="DataExecutionOrder"),"execution removal retains subscriptions and exposes ordering error");separated.Undo();Check(separated.Validate().IsValid,"undo control disconnect restores schedule");
separated.Disconnect(separated.CurrentProcedure.Connections.Single(c=>c.ToNode==sc));Check(separated.CurrentProcedure.ExecutionLinks!.Count==2,"subscription removal preserves execution relations");separated.Undo();
Reject(()=>separated.ConnectExecution(sc,sa),"control cycle transaction rejected");separated.DeleteNode(sb);Check(separated.CurrentProcedure.ExecutionLinks!.Count==0&&separated.CurrentProcedure.Connections.Count==1,"delete cleans both relation sets");separated.Undo();Check(separated.CurrentProcedure.ExecutionLinks!.Count==2&&separated.CurrentProcedure.Connections.Count==2,"undo restores both relation sets");
var workflows=new GraphEditorDocument(registry);string initial=workflows.CurrentProcedureId;
workflows.MoveProcedure(initial,150,300);Check(workflows.CurrentProcedure.OverviewX==150&&workflows.CurrentProcedure.OverviewY==300,"overview position is document state");workflows.Undo();Check(workflows.CurrentProcedure.OverviewX is null,"overview move undo");
workflows.DeleteProcedure(initial);Check(!workflows.HasCurrentProcedure&&workflows.Document.Procedures.Count==0,"last workflow deletion allowed");
Reject(()=>workflows.AddNode("number","1",0,0),"cannot add node without workflow");
workflows.Undo();Check(workflows.HasCurrentProcedure&&workflows.CurrentProcedureId==initial,"last workflow deletion undo restores selection");workflows.Redo();Check(!workflows.HasCurrentProcedure,"last workflow deletion redo");
string restored=workflows.AddProcedure("流程1");Check(workflows.CurrentProcedureId==restored&&workflows.HasCurrentProcedure,"add workflow from empty state");
string workflowFile=Path.Combine(Path.GetTempPath(),"workflow-roundtrip-"+Guid.NewGuid().ToString("N")+".json");
try{workflows.MoveProcedure(restored,300,450);workflows.Save(workflowFile);workflows.Load(workflowFile);Check(workflows.CurrentProcedure.OverviewX==300&&workflows.CurrentProcedure.OverviewY==450,"overview position persistence");workflows.DeleteProcedure(restored);workflows.Save(workflowFile);workflows.Load(workflowFile);Check(!workflows.HasCurrentProcedure&&workflows.CurrentProcedureId=="","empty workflow solution roundtrip");}finally{File.Delete(workflowFile);}
var clipboardDoc=new GraphEditorDocument(registry);string ca=clipboardDoc.AddNode("number","1",10,20),cb=clipboardDoc.AddNode("pipe","1",130,80),cc=clipboardDoc.AddNode("pipe","1",250,140);
clipboardDoc.SetParameter(ca,"Value",JsonSerializer.SerializeToElement("snapshot"));clipboardDoc.ConnectExecution(ca,cb,2,0);clipboardDoc.ConnectExecution(cb,cc);clipboardDoc.BindInput(ca,"Out",cb,"In");clipboardDoc.BindInput(cb,"Out",cc,"In");
long copyRevision=clipboardDoc.Revision;var fragment=clipboardDoc.CopyNodes([ca,cb]);Check(clipboardDoc.Revision==copyRevision&&fragment.Nodes.Count==2,"copy captures without modifying document");
Check(fragment.Connections.Count==1&&fragment.ExecutionLinks.Count==1,"fragment retains internal bindings and execution links only");clipboardDoc.SetParameter(ca,"Value",JsonSerializer.SerializeToElement("changed"));copyRevision=clipboardDoc.Revision;
var pasted=clipboardDoc.PasteNodes(fragment,210,320);Check(clipboardDoc.Revision==copyRevision+1&&pasted.Count==2,"group paste is one atomic undo transaction");
Check(pasted.All(id=>id!=ca&&id!=cb)&&pasted.Distinct().Count()==2,"paste generates fresh node identities");
var pastedA=clipboardDoc.CurrentProcedure.Nodes.Single(n=>n.Id==pasted[0]);var pastedB=clipboardDoc.CurrentProcedure.Nodes.Single(n=>n.Id==pasted[1]);Check(pastedA.X==210&&pastedA.Y==320&&pastedB.X==330&&pastedB.Y==380,"group paste preserves relative positions");
Check(pastedA.Parameters["Value"].GetString()=="changed"&&clipboardDoc.CurrentProcedure.Nodes.Single(n=>n.Id==ca).Parameters["Value"].GetString()=="changed","paste copies current source parameters per native CopyModuleParam path");
Check(clipboardDoc.CurrentProcedure.ExecutionLinks!.Any(e=>e.FromNode==pasted[0]&&e.ToNode==pasted[1]&&e.SourceIndex==2&&e.TargetIndex==0),"copy remaps internal execution ports");
Check(clipboardDoc.CurrentProcedure.Connections.Any(c=>c.FromNode==pasted[0]&&c.ToNode==pasted[1]),"copy remaps internal data references");
Check(pasted.Select(id=>clipboardDoc.CurrentProcedure.Nodes.Single(n=>n.Id==id).Name).Distinct().Count()==2&&clipboardDoc.CurrentProcedure.Nodes.Where(n=>pasted.Contains(n.Id)).All(n=>!fragment.Nodes.Any(old=>old.Name==n.Name)),"copy gives fresh unique display names");
clipboardDoc.Undo();Check(clipboardDoc.CurrentProcedure.Nodes.Count==3&&clipboardDoc.CurrentProcedure.ExecutionLinks!.Count==2,"undo paste removes copied nodes and links together");clipboardDoc.Redo();Check(pasted.All(id=>clipboardDoc.CurrentProcedure.Nodes.Any(n=>n.Id==id)),"redo restores same copied identities");
copyRevision=clipboardDoc.Revision;clipboardDoc.SetNodesEnabled(pasted,false);Check(clipboardDoc.Revision==copyRevision+1&&clipboardDoc.CurrentProcedure.Nodes.Where(n=>pasted.Contains(n.Id)).All(n=>!n.Enabled),"group disable is atomic");clipboardDoc.Undo();Check(clipboardDoc.CurrentProcedure.Nodes.All(n=>n.Enabled),"group disable undo");
string destinationFlow=clipboardDoc.AddProcedure("目标流程");var transferred=clipboardDoc.PasteNodes(fragment,100,100);Check(clipboardDoc.CurrentProcedureId==destinationFlow&&transferred.Count==2&&clipboardDoc.Validate().IsValid,"cross-workflow paste retains internal data and control relations");clipboardDoc.Undo();Check(clipboardDoc.CurrentProcedure.Nodes.Count==0,"cross-workflow paste undo does not affect source workflow");
var hotDoc=new GraphEditorDocument(registry);string hotSource=hotDoc.AddNode("number","1",100,100);long hotRevision=hotDoc.Revision;
string hotNew=hotDoc.AddNode("pipe","1",100,170,executionPredecessor:hotSource);
Check(hotDoc.Revision==hotRevision+1&&hotDoc.CurrentProcedure.Nodes.Count==2&&hotDoc.CurrentProcedure.ExecutionLinks!.Single() is {SourceIndex:3,TargetIndex:1},"hot drop node and control link are atomic");
long occupiedRevision=hotDoc.Revision;bool occupiedRejected=false;try{hotDoc.AddNode("pipe","1",100,240,executionPredecessor:hotSource);}catch(InvalidOperationException){occupiedRejected=true;}
Check(occupiedRejected&&hotDoc.Revision==occupiedRevision&&hotDoc.CurrentProcedure.Nodes.Count==2,"occupied hot connector rejection leaves no orphan node");
hotDoc.Undo();Check(hotDoc.CurrentProcedure.Nodes.Count==1&&ProcedureGraph.ExecutionEdges(hotDoc.CurrentProcedure).Count==0,"undo hot drop removes node and link together");hotDoc.Redo();Check(hotDoc.CurrentProcedure.Nodes.Any(n=>n.Id==hotNew),"redo hot drop restores identity");
var wireDoc=new GraphEditorDocument(registry);string ws=wireDoc.AddNode("number","1",100,100),wt=wireDoc.AddNode("pipe","1",400,300),wi=wireDoc.AddNode("pipe","1",200,200);wireDoc.ConnectExecution(ws,wt,2,0);var originalWire=wireDoc.CurrentProcedure.ExecutionLinks!.Single();long wireRevision=wireDoc.Revision;
wireDoc.InsertExecutionNode(originalWire,wi,250,200);Check(wireDoc.Revision==wireRevision+1&&wireDoc.CurrentProcedure.ExecutionLinks!.Count==2&&wireDoc.CurrentProcedure.ExecutionLinks.All(e=>e.SourceIndex==2&&e.TargetIndex==0),"insertion preserves both original connector indices in atomic split");
wireDoc.Undo();Check(wireDoc.CurrentProcedure.ExecutionLinks!.Single().FromNode==ws&&wireDoc.CurrentProcedure.Nodes.Single(n=>n.Id==wi).X==200,"undo restores original wire and inserted node position together");
wireRevision=wireDoc.Revision;Reject(()=>wireDoc.InsertExecutionNode(originalWire,ws,0,0),"reject insertion of its own endpoint");Check(wireDoc.Revision==wireRevision,"endpoint insertion rejection is transactional");
string addedInsert=wireDoc.AddNode("pipe","1",300,200,insertInto:originalWire);Check(wireDoc.Revision==wireRevision+1&&wireDoc.CurrentProcedure.Nodes.Count==4&&wireDoc.CurrentProcedure.ExecutionLinks!.Count==2,"toolbox insertion creates and splits in one transaction");wireDoc.Undo();Check(wireDoc.CurrentProcedure.Nodes.Count==3&&wireDoc.CurrentProcedure.ExecutionLinks!.Count==1,"toolbox insertion undo leaves no orphan module");
wireDoc.ConnectExecution(wt,wi);wireRevision=wireDoc.Revision;Reject(()=>wireDoc.InsertExecutionNode(originalWire,wi,200,200),"insertion rejects resulting cycle");Check(wireDoc.Revision==wireRevision&&wireDoc.CurrentProcedure.ExecutionLinks!.Count==2,"cycle insertion rolls back all edges and position");wireDoc.Undo();
wireDoc.ConnectExecution(ws,wi,3,1);wireDoc.InsertExecutionNode(originalWire,wi,200,200);Check(wireDoc.CurrentProcedure.ExecutionLinks!.Count==2&&wireDoc.CurrentProcedure.ExecutionLinks.All(e=>e.SourceIndex==2&&e.TargetIndex==0),"existing insertion edge receives original connector indices as VMAddConnection does");wireDoc.Undo();wireDoc.Undo();
var manual=new[]{new WirePointDocument(210,115),new WirePointDocument(300,115),new WirePointDocument(300,315),new WirePointDocument(400,315)};wireRevision=wireDoc.Revision;wireDoc.SetExecutionPath(originalWire,manual);Check(wireDoc.Revision==wireRevision+1&&wireDoc.CurrentProcedure.ExecutionLinks!.Single().PathPoints!.SequenceEqual(manual),"manual route is persisted atomically");
manual[1]=new(999,999);Check(wireDoc.CurrentProcedure.ExecutionLinks!.Single().PathPoints![1]==new WirePointDocument(300,115),"manual route snapshot is isolated from caller");
wireRevision=wireDoc.Revision;try{wireDoc.SetExecutionPath(originalWire,[new(0,0),new(1,1)]);throw new Exception("diagonal accepted");}catch(ArgumentException){count++;}Check(wireDoc.Revision==wireRevision,"invalid manual route does not edit document");
wireDoc.MoveNode(ws,105,100);Check(wireDoc.CurrentProcedure.ExecutionLinks!.Single().PathPoints is null,"endpoint movement invalidates saved route as original position handler does");wireDoc.Undo();Check(wireDoc.CurrentProcedure.ExecutionLinks!.Single().PathPoints!.Count==4,"undo endpoint movement restores custom path");
var pathCopy=wireDoc.CopyNodes([ws,wt]);var pathIds=wireDoc.PasteNodes(pathCopy,200,200);Check(wireDoc.CurrentProcedure.ExecutionLinks!.Single(e=>e.FromNode==pathIds[0]&&e.ToNode==pathIds[1]).PathPoints![0]==new WirePointDocument(310,215),"copy translates manual route with pasted subgraph");wireDoc.Undo();
var clippedIds=wireDoc.PasteNodes(pathCopy,4840,4840);Check(wireDoc.CurrentProcedure.ExecutionLinks!.Single(e=>e.FromNode==clippedIds[0]&&e.ToNode==clippedIds[1]).PathPoints is null,"non-rigid boundary paste invalidates stale custom route");wireDoc.Undo();
string routeTemp=Path.Combine(Path.GetTempPath(),"vm-route-"+Guid.NewGuid().ToString("N")+".json");try{wireDoc.Save(routeTemp);var loadedWire=new GraphEditorDocument(registry);loadedWire.Load(routeTemp);Check(loadedWire.CurrentProcedure.ExecutionLinks!.Single().PathPoints![1]==new WirePointDocument(300,115),"custom path survives save and reopen");}finally{File.Delete(routeTemp);}
var nameDoc=new GraphEditorDocument(registry);string nameId=nameDoc.CurrentProcedureId;long nameRevision=nameDoc.Revision;
foreach(char invalid in "?;.[]<>()`") {try{nameDoc.RenameProcedure(nameId,"流程"+invalid);throw new Exception("invalid VM name accepted");}catch(ArgumentException){count++;}}
Check(nameDoc.Revision==nameRevision,"VM-invalid workflow names leave document and history unchanged");
nameDoc.RenameProcedure(nameId,"检测流程_1");nameDoc.Undo();Check(nameDoc.CurrentProcedure.Name=="Main","workflow rename remains undoable after validation");
var policyDoc=new GraphEditorDocument(registry);string policyId=policyDoc.CurrentProcedureId;long policyRevision=policyDoc.Revision;
policyDoc.SetProcedureRunPolicy(policyId,250,true);Check(policyDoc.Revision==policyRevision+1&&policyDoc.CurrentProcedure.ContinuousRunIntervalMs==250&&policyDoc.CurrentProcedure.StopOnNg,"workflow run policy is one real document edit");
policyDoc.Undo();Check(policyDoc.CurrentProcedure.ContinuousRunIntervalMs==80&&!policyDoc.CurrentProcedure.StopOnNg,"undo restores original workflow run policy");policyDoc.Redo();
foreach(int value in new[]{-1,100001}){try{policyDoc.SetProcedureRunPolicy(policyId,value,true);throw new Exception("invalid interval accepted");}catch(ArgumentOutOfRangeException){count++;}}
string policyCopy=policyDoc.DuplicateProcedure(policyId,overviewX:150,overviewY:200);Check(policyDoc.CurrentProcedure.StopOnNg&&policyDoc.CurrentProcedure.ContinuousRunIntervalMs==250&&policyDoc.CurrentProcedure.OverviewX==150,"drag copy preserves policy and sets position in one transaction");policyDoc.Undo();Check(policyDoc.Document.Procedures.Count==1,"undo removes overview copy without a stranded position transaction");
string policyFile=Path.Combine(Path.GetTempPath(),"vm-policy-"+Guid.NewGuid().ToString("N")+".json");try{policyDoc.Save(policyFile);var policyLoaded=new GraphEditorDocument(registry);policyLoaded.Load(policyFile);Check(policyLoaded.CurrentProcedure.StopOnNg&&policyLoaded.CurrentProcedure.ContinuousRunIntervalMs==250,"run policy survives save and reopen");}finally{File.Delete(policyFile);}
policyRevision=policyDoc.Revision;policyDoc.AddProcedure("拖放流程",300,400);Check(policyDoc.Revision==policyRevision+1&&policyDoc.CurrentProcedure.OverviewY==400,"overview tool drop creates workflow and position atomically");policyDoc.Undo();Check(policyDoc.Document.Procedures.Count==1,"one undo removes dropped workflow");
var configurationDoc=new GraphEditorDocument(registry);string configurationId=configurationDoc.CurrentProcedureId;
string configurationNode=configurationDoc.AddNode("vision.image-file","1",10,10,new(){["Value"]=JsonSerializer.SerializeToElement("test.bmp")});
var interfaceRows=new[]{new WorkflowInterfaceItem("output-image","out0",DataKind.GrayImage,configurationNode,"Out")};
var configuration=new WorkflowConfiguration {Outputs=interfaceRows,Groups=[new("group0","分组0")],Variables=[new("var0","group0","%var0%","int","12")],RunParameters=[new("run0",configurationNode,"Value","Value")]};
configurationDoc.SetWorkflowConfiguration(configurationId,configuration);interfaceRows[0]=interfaceRows[0] with{Name="mutated"};Check(configurationDoc.CurrentProcedure.Configuration.Outputs[0].Name=="out0","configuration isolates caller-owned interface arrays");
var exposed=configurationDoc.Document.Procedures[0].Configuration;((WorkflowInterfaceItem[])exposed.Outputs)[0]=exposed.Outputs[0] with{Name="leaked"};Check(configurationDoc.CurrentProcedure.Configuration.Outputs[0].Name=="out0","document snapshot isolates workflow configuration arrays");
string configurationCopy=configurationDoc.DuplicateProcedure(configurationId);Check(configurationDoc.CurrentProcedure.Configuration.Outputs[0].NodeId==configurationDoc.CurrentProcedure.Nodes[0].Id&&configurationDoc.CurrentProcedure.Configuration.RunParameters[0].NodeId==configurationDoc.CurrentProcedure.Nodes[0].Id,"workflow copy remaps interface and runtime parameter references");
configurationDoc.SetProcedureNodeParameters(configurationId,configurationNode,new(){["Value"]=JsonSerializer.SerializeToElement("edited in background flow")});Check(configurationDoc.CurrentProcedureId==configurationCopy&&configurationDoc.Document.Procedures.Single(p=>p.Id==configurationId).Nodes[0].Parameters["Value"].GetString()=="edited in background flow"&&configurationDoc.CurrentProcedure.Nodes[0].Parameters["Value"].GetString()=="test.bmp","modeless configuration edits its own workflow without changing selected tab or copied node");
configurationDoc.DeleteNodes([configurationDoc.CurrentProcedure.Nodes[0].Id]);Check(configurationDoc.CurrentProcedure.Configuration.Outputs[0].NodeId is null&&configurationDoc.CurrentProcedure.Configuration.RunParameters.Count==0,"node deletion clears subscribed interface and exposed parameters atomically");configurationDoc.Undo();Check(configurationDoc.CurrentProcedure.Configuration.Outputs[0].NodeId is not null,"undo restores cleared workflow configuration references");
var validConfig=configurationDoc.CurrentProcedure.Configuration;long configRevision=configurationDoc.Revision;
try{configurationDoc.SetWorkflowConfiguration(configurationCopy,validConfig with{Variables=[new("bad","group0","%bad%","int","not an integer")]});throw new Exception("Invalid variable accepted");}catch(ArgumentException){count++;}
Check(configurationDoc.Revision==configRevision,"invalid variable value leaves document and undo history unchanged");
try{configurationDoc.SetWorkflowConfiguration(configurationCopy,validConfig with{Outputs=[new("wrong","wrong",DataKind.Integer,configurationDoc.CurrentProcedure.Nodes[0].Id,"Out")]});throw new Exception("Wrong interface source kind accepted");}catch(InvalidDataException){count++;}
string configurationFile=Path.Combine(Path.GetTempPath(),"workflow-config-"+Guid.NewGuid().ToString("N")+".json");try{configurationDoc.Save(configurationFile);var loadedConfiguration=new GraphEditorDocument(registry);loadedConfiguration.Load(configurationFile);Check(loadedConfiguration.Document.Procedures.Single(p=>p.Id==configurationCopy).Configuration.Variables.Single().Value=="12","local variables and configuration survive save/reload");}finally{File.Delete(configurationFile);}
var resetDoc=new GraphEditorDocument(registry);var resetId=resetDoc.AddNode("number","1",0,0,new(){["Value"]=JsonSerializer.SerializeToElement("accepted")});
var resetInput=resetDoc.AddNode("pipe","1",100,0);
resetDoc.SetWorkflowConfiguration(resetDoc.CurrentProcedureId,new WorkflowConfiguration{Subscriptions=[new(resetId,"Value",true,new(WorkflowSourceKind.TextConstant,"\"bound\"")),new(resetInput,"In",false,new(WorkflowSourceKind.NumberConstant,"3"))]});
long resetRevision=resetDoc.Revision;
resetDoc.ResetProcedureNodeParameters(resetDoc.CurrentProcedureId,resetId,new(){["Value"]=JsonSerializer.SerializeToElement("default")});
Check(resetDoc.Revision==resetRevision+1&&resetDoc.CurrentProcedure.Nodes.Single(n=>n.Id==resetId).Parameters["Value"].GetString()=="default","plugin reset is one document transaction");
Check(resetDoc.CurrentProcedure.Configuration.Subscriptions.Count==1&&!resetDoc.CurrentProcedure.Configuration.Subscriptions[0].IsParameter,"reset removes parameter subscriptions and preserves input bindings");
resetDoc.Undo();Check(resetDoc.CurrentProcedure.Configuration.Subscriptions.Count==2,"one undo restores removed parameter subscription");Check(resetDoc.CurrentProcedure.Nodes.Single(n=>n.Id==resetId).Parameters["Value"].GetString()=="accepted","one undo restores pre-reset parameter values");
resetDoc.Redo();Check(resetDoc.CurrentProcedure.Nodes.Single(n=>n.Id==resetId).Parameters["Value"].GetString()=="default","redo restores accepted reset");
resetRevision=resetDoc.Revision;
try{resetDoc.ResetProcedureNodeParameters(resetDoc.CurrentProcedureId,resetId,new(){["Value"]=JsonSerializer.SerializeToElement(123)});throw new Exception("Invalid reset accepted");}catch(ArgumentException){count++;}
Check(resetDoc.Revision==resetRevision&&resetDoc.CurrentProcedure.Nodes.Single(n=>n.Id==resetId).Parameters["Value"].GetString()=="default","invalid reset leaves document and undo history untouched");
Console.WriteLine($"PASS {count} editor semantic checks");



} catch(Exception checkFailure) { Console.Error.WriteLine(checkFailure); Environment.ExitCode=1; }
