using System.Text.Json;
using VisionFramework;
namespace VisionEditor.Core;

/// <summary>UI-independent graph edits, history, persistence and connection invariants.</summary>
public sealed partial class GraphEditorDocument
{
    readonly ModuleRegistry registry;
    SolutionDocument document = null!;
    readonly Stack<Snapshot> undo = new(), redo = new();
    sealed record Snapshot(SolutionDocument Document, string ProcedureId, string BaseDirectory);
    public GraphEditorDocument(ModuleRegistry registry) { this.registry = registry; NewSolution(); }
    public SolutionDocument Document => Copy(document);
    public string SolutionId=>document.Id;
    public bool HasCurrentProcedure => document.Procedures.Count>0;
    public ProcedureDocument CurrentProcedure => HasCurrentProcedure?Document.Procedures.Single(p => p.Id == CurrentProcedureId):new("","",[],[],[]);
    public string CurrentProcedureId { get; private set; } = "";
    public string BaseDirectory { get; private set; } = Path.GetFullPath(Environment.CurrentDirectory);
    public string? FilePath { get; private set; }
    public bool IsDirty { get; private set; }
    public long Revision { get; private set; }
    public bool CanUndo => undo.Count != 0;
    public bool CanRedo => redo.Count != 0;
    public event Action? Changed;
    public void NewSolution(bool createProcedure=true) {
        document = new(SolutionStore.CurrentVersion, Guid.NewGuid().ToString("N"), "New solution", createProcedure?[new("main", "Main", [], [], [])]:[], []);
        CurrentProcedureId = createProcedure?"main":""; BaseDirectory = Path.GetFullPath(Environment.CurrentDirectory); FilePath = null;
        undo.Clear(); redo.Clear(); IsDirty = true; Revision++; Changed?.Invoke();
    }
    public void Load(string path) {
        string full = Path.GetFullPath(path); var loaded = SolutionStore.Load(full);
        document = Copy(loaded with{Procedures=loaded.Procedures.Select(ProcedureGraph.Separate).ToArray()}); CurrentProcedureId = loaded.Procedures.FirstOrDefault()?.Id??"";
        FilePath = full; BaseDirectory = Path.GetDirectoryName(full)!; undo.Clear(); redo.Clear(); IsDirty = false; Revision++; Changed?.Invoke();
    }
    public void Save(string path) {
        string full = Path.GetFullPath(path), directory = Path.GetDirectoryName(full)!;
        var rebased = Rebase(document, BaseDirectory, directory);
        SolutionStore.Save(full, rebased); // Commit editor state only after an atomic successful write.
        document = rebased; BaseDirectory = directory; FilePath = full; IsDirty = false; Revision++; Changed?.Invoke();
    }
    public void SelectProcedure(string id) { if (!document.Procedures.Any(p => p.Id == id)) throw new KeyNotFoundException(id); if(CurrentProcedureId == id)return; CurrentProcedureId = id; Changed?.Invoke(); }
    public string AddProcedure(string name,double? overviewX=null,double? overviewY=null) {
        RequireName(name); string id = Guid.NewGuid().ToString("N");
        if(overviewX.HasValue||overviewY.HasValue){if(!overviewX.HasValue||!overviewY.HasValue)throw new ArgumentException("Both overview coordinates are required.");Position(overviewX.Value,overviewY.Value);}
        Edit(d => d with { Procedures = d.Procedures.Append(new ProcedureDocument(id, name, [], [], []){OverviewX=overviewX,OverviewY=overviewY}).ToArray() },id);
        return id;
    }
    public void SetProcedureRunPolicy(string id,int intervalMs,bool stopOnNg) {
        if(intervalMs is <0 or >100000)throw new ArgumentOutOfRangeException(nameof(intervalMs));
        var p=Procedure(id);if(p.ContinuousRunIntervalMs==intervalMs&&p.StopOnNg==stopOnNg)return;
        Edit(d=>d with{Procedures=d.Procedures.Select(p=>p.Id==id?p with{ContinuousRunIntervalMs=intervalMs,StopOnNg=stopOnNg}:p).ToArray()});
    }
    public void SetWorkflowConfiguration(string id,WorkflowConfiguration configuration) {
        var p=Procedure(id);configuration.Validate(p,registry);var snapshot=configuration.Snapshot();
        if(JsonSerializer.Serialize(p.Configuration)==JsonSerializer.Serialize(snapshot))return;
        Edit(d=>{var changed=p with{Configuration=snapshot};var signature=WorkflowCalls.From(changed);return d with{Procedures=d.Procedures.Select(item=>item.Id==id?changed:changed.GroupOwner is {} owner&&item.Id==owner.ProcedureId?item with{Nodes=item.Nodes.Select(n=>n.Id==owner.NodeId?n with{Call=signature}:n).ToArray()}:item).ToArray()};});
    }
    public void SetProcedureNodeParameters(string procedureId,string nodeId,Dictionary<string,JsonElement> values) {
        var p=Procedure(procedureId);var node=p.Nodes.FirstOrDefault(n=>n.Id==nodeId)??throw new KeyNotFoundException(nodeId);var parameters=CheckedParameters(node,values);
        Edit(d=>d with{Procedures=d.Procedures.Select(p=>p.Id==procedureId?p with{Nodes=p.Nodes.Select(n=>n.Id==nodeId?n with{Parameters=parameters}:n).ToArray()}:p).ToArray()});
    }
    public void RenameProcedure(string id,string name) {
        RequireName(name);
        // VmModuleBaseDecorator.Name rejects these characters before invoking the SDK.
        if(name.IndexOfAny("?;.[]<>()`".ToCharArray())>=0)throw new ArgumentException("名称不能包含 ? ; . [ ] < > ( ) `。",nameof(name));
        var procedure=Procedure(id); if(procedure.Name==name)return;
        Edit(d=>d with {Procedures=d.Procedures.Select(p=>p.Id==id?p with {Name=name}:p).ToArray()});
    }
    public void MoveProcedure(string id,double x,double y) {
        Position(x,y);var procedure=Procedure(id);if(procedure.OverviewX==x&&procedure.OverviewY==y)return;
        Edit(d=>d with {Procedures=d.Procedures.Select(p=>p.Id==id?p with {OverviewX=x,OverviewY=y}:p).ToArray()});
    }
    public void DeleteProcedure(string id) {
        _=Procedure(id);if(document.Procedures.Where(p=>p.Id!=id).Any(p=>p.Nodes.Any(n=>n.Call?.ProcedureId==id)))throw new InvalidOperationException("流程仍被子程序节点引用，请先移除调用。");
        int index=document.Procedures.ToList().FindIndex(p=>p.Id==id);
        var remaining=document.Procedures.Where(p=>p.Id!=id).ToArray();
        string selection=CurrentProcedureId==id?(remaining.Length==0?"":remaining[Math.Min(index,remaining.Length-1)].Id):CurrentProcedureId;
        Edit(d=>d with {Procedures=remaining},selection);
    }
    public string DuplicateProcedure(string id,string? name=null,double? overviewX=null,double? overviewY=null) {
        var original=Procedure(id); string copyName=name??original.Name+" 副本"; RequireName(copyName);
        if(overviewX.HasValue||overviewY.HasValue){if(!overviewX.HasValue||!overviewY.HasValue)throw new ArgumentException("Both overview coordinates are required.");Position(overviewX.Value,overviewY.Value);}
        string newId=Guid.NewGuid().ToString("N");
        var ids=original.Nodes.ToDictionary(n=>n.Id,_=>Guid.NewGuid().ToString("N"));
        var duplicate=original with {
            Id=newId,Name=copyName,Configuration=original.Configuration.RemapNodes(ids),OverviewX=overviewX??original.OverviewX,OverviewY=overviewY??original.OverviewY,
            Nodes=original.Nodes.Select(n=>n with {Id=ids[n.Id]}).ToArray(),
            Connections=original.Connections.Select(c=>c with {FromNode=ids[c.FromNode],ToNode=ids[c.ToNode]}).ToArray(),
            ExecutionLinks=original.ExecutionLinks?.Select(e=>e with {FromNode=ids[e.FromNode],ToNode=ids[e.ToNode]}).ToArray()
        };
        Edit(d=>d with {Procedures=d.Procedures.SelectMany(p=>p.Id==id?new[]{p,duplicate}:new[]{p}).ToArray()},newId);
        return newId;
    }
    ProcedureDocument Procedure(string id)=>document.Procedures.SingleOrDefault(p=>p.Id==id)??throw new KeyNotFoundException(id);
    public string AddNode(string moduleId, string version, double x, double y, Dictionary<string, JsonElement>? parameters = null, string? executionPredecessor = null, ExecutionLinkDocument? insertInto=null) {
        if(!HasCurrentProcedure)throw new InvalidOperationException("请先添加流程。");
        Position(x,y); var descriptor = registry.GetDescriptor(moduleId, version); string id = Guid.NewGuid().ToString("N");
        var values = descriptor.Parameters.Where(p => p.DefaultValue.HasValue).ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
        if(parameters is not null)foreach(var p in parameters)values[p.Key] = p.Value.Clone();
        if(executionPredecessor is not null){_=Node(executionPredecessor);if(ProcedureGraph.ExecutionEdges(CurrentProcedure).Any(e=>(e.FromNode==executionPredecessor&&e.SourceIndex==3)||(e.ToNode==executionPredecessor&&e.TargetIndex==3)))throw new InvalidOperationException("Bottom connector is occupied.");}
        if(insertInto is not null&&executionPredecessor is not null)throw new ArgumentException("Insertion and automatic predecessor are exclusive.");
        ChangeProcedure(p => {var next=p with { Nodes = p.Nodes.Append(new NodeDocument(id,moduleId,version,descriptor.DisplayName,values,x,y)).ToArray(), ExecutionLinks=executionPredecessor is null?p.ExecutionLinks:ProcedureGraph.ExecutionEdges(p).Append(new ExecutionLinkDocument(executionPredecessor,id){SourceIndex=3,TargetIndex=1}).ToArray() };return insertInto is null?next:InsertExecution(next,insertInto,id);}); return id;
    }
    public void ConfigureSubprogram(string id,string targetProcedureId){
        var node=Node(id);if(node.ModuleId!=WorkflowCalls.ModuleId)throw new InvalidOperationException("Not a subprogram node.");var target=Procedure(targetProcedureId);if(target.GroupOwner is {} targetOwner&&targetOwner!=new GroupOwnership(CurrentProcedureId,id))throw new InvalidOperationException("不能引用其他节点的内部 Group。");if(node.Call is {} oldCall&&Procedure(oldCall.ProcedureId).GroupOwner is not null&&oldCall.ProcedureId!=targetProcedureId)throw new InvalidOperationException("内部 Group 不能替换为跨流程引用。");var call=WorkflowCalls.From(target);
        if(node.Call is not null&&(CurrentProcedure.Connections.Any(c=>c.FromNode==id||c.ToNode==id)||CurrentProcedure.Configuration.Subscriptions.Any(b=>b.NodeId==id||b.Source.Kind==WorkflowSourceKind.NodeOutput&&b.Source.Id==id)||CurrentProcedure.Configuration.Outputs.Concat(CurrentProcedure.Configuration.Displays).Any(o=>o.NodeId==id)||CurrentProcedure.Configuration.RunParameters.Any(o=>o.NodeId==id))&&(!node.Call.Inputs.SequenceEqual(call.Inputs)||!node.Call.Outputs.SequenceEqual(call.Outputs)))throw new InvalidOperationException("接口已连接；请先解除订阅后更新不同的调用签名。");
        var candidate=document with{Procedures=document.Procedures.Select(p=>p.Id==CurrentProcedureId?p with{Nodes=p.Nodes.Select(n=>n.Id==id?n with{Call=call}:n).ToArray()}:p).ToArray()};WorkflowCalls.Validate(candidate,CurrentProcedureId,allowDrafts:true);Edit(_=>candidate);
    }
    public void SetSubprogramInput(string id,string port,WorkflowValueReference? source){var node=Node(id);if(node.ModuleId!=WorkflowCalls.ModuleId||node.Call is null)throw new InvalidOperationException("Invalid subprogram.");SetProcedureInputSubscription(CurrentProcedureId,id,port,source);}
    public void SetProcedureInputSubscription(string procedureId,string id,string port,WorkflowValueReference? source){
        var p=Procedure(procedureId);var node=p.Nodes.SingleOrDefault(n=>n.Id==id)??throw new KeyNotFoundException(id);if(!registry.GetDescriptor(node).Inputs.Any(i=>i.Name==port))throw new InvalidOperationException("Invalid input.");
        var subscriptions=p.Configuration.Subscriptions.Where(b=>b.NodeId!=id||b.Target!=port||b.IsParameter);
        var next=p with{Connections=p.Connections.Where(c=>c.ToNode!=id||c.ToPort!=port).ToArray(),Configuration=p.Configuration with{Subscriptions=(source is null?subscriptions:subscriptions.Append(new(id,port,false,source))).ToArray()}};
        next.Configuration.Validate(next,registry);var invalid=GraphValidator.Validate(next,registry).Issues.FirstOrDefault(i=>i.Code is "DataExecutionOrder" or "Cycle" or "WorkflowConfiguration");if(invalid is not null)throw new InvalidOperationException(invalid.Message);Edit(d=>d with{Procedures=d.Procedures.Select(item=>item.Id==procedureId?next:item).ToArray()});
    }
    public void MoveNode(string id,double x,double y) =>MoveNodes(new Dictionary<string,(double X,double Y)>{{id,(x,y)}});
    public void MoveNodes(IReadOnlyDictionary<string,(double X,double Y)> positions) {
        foreach(var item in positions){_=Node(item.Key);Position(item.Value.X,item.Value.Y);}
        if(!positions.Any(item=>{var n=Node(item.Key);return n.X!=item.Value.X||n.Y!=item.Value.Y;}))return;
        var moved=positions.Where(item=>{var n=Node(item.Key);return n.X!=item.Value.X||n.Y!=item.Value.Y;}).Select(item=>item.Key).ToHashSet();
        ChangeProcedure(p=>p with {Nodes=p.Nodes.Select(n=>positions.TryGetValue(n.Id,out var point)?n with {X=point.X,Y=point.Y}:n).ToArray(),ExecutionLinks=p.ExecutionLinks?.Select(e=>moved.Contains(e.FromNode)||moved.Contains(e.ToNode)?e with {PathPoints=null}:e).ToArray()});
    }
    public void DeleteNodes(IEnumerable<string> nodeIds) {
        var ids=nodeIds.ToHashSet(StringComparer.Ordinal);foreach(var id in ids)_=Node(id);if(ids.Count==0)return;
        ChangeProcedure(p=>p with {Configuration=p.Configuration.RemoveNodes(ids),Nodes=p.Nodes.Where(n=>!ids.Contains(n.Id)).ToArray(),Connections=p.Connections.Where(c=>!ids.Contains(c.FromNode)&&!ids.Contains(c.ToNode)).ToArray(),ExecutionLinks=p.ExecutionLinks?.Where(e=>!ids.Contains(e.FromNode)&&!ids.Contains(e.ToNode)).ToArray()});
    }
    public GraphFragment CopyNodes(IEnumerable<string> nodeIds) {
        var ids=nodeIds.ToHashSet(StringComparer.Ordinal);foreach(var id in ids)_=Node(id);
        var procedure=CurrentProcedure;
        return new(procedure.Nodes.Where(n=>ids.Contains(n.Id)).ToArray(),procedure.Connections.Where(c=>ids.Contains(c.FromNode)&&ids.Contains(c.ToNode)).ToArray(),ProcedureGraph.ExecutionEdges(procedure).Where(c=>ids.Contains(c.FromNode)&&ids.Contains(c.ToNode)).ToArray());
    }
    public IReadOnlyList<string> PasteNodes(GraphFragment fragment,double x,double y) {
        if(!HasCurrentProcedure)throw new InvalidOperationException("请先添加流程。");Position(x,y);
        if(fragment.Nodes.Count==0)return [];
        foreach(var node in fragment.Nodes)_=registry.GetDescriptor(node);
        var ids=fragment.Nodes.ToDictionary(n=>n.Id,_=>Guid.NewGuid().ToString("N"));
        var names=CurrentProcedure.Nodes.Select(n=>n.Name).ToHashSet(StringComparer.Ordinal);
        string UniqueName(string name){if(names.Add(name))return name;var stem=System.Text.RegularExpressions.Regex.Replace(name,@"\d+$","");int index=1;while(!names.Add(stem+index))index++;return stem+index;}
        double left=fragment.Nodes.Min(n=>n.X),top=fragment.Nodes.Min(n=>n.Y);
        double Coordinate(double value)=>Math.Round(Math.Clamp(value,0,4850)/5)*5;
        var nodes=fragment.Nodes.Select(n=>n with {Id=ids[n.Id],Name=UniqueName(n.Name),X=Coordinate(x+n.X-left),Y=Coordinate(y+n.Y-top),Call=n.Call?.Snapshot(),Parameters=n.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone()),Rois=n.Rois?.Select(r=>r with {Vertices=r.Vertices?.ToArray()}).ToArray()}).ToArray();
        var connections=fragment.Connections.Select(c=>c with {FromNode=ids[c.FromNode],ToNode=ids[c.ToNode]}).ToArray();
        IReadOnlyList<WirePointDocument>? TranslatedPath(ExecutionLinkDocument edge){
            if(edge.PathPoints is null)return null;var source=fragment.Nodes.Single(n=>n.Id==edge.FromNode);var target=fragment.Nodes.Single(n=>n.Id==edge.ToNode);var copiedSource=nodes.Single(n=>n.Id==ids[source.Id]);var copiedTarget=nodes.Single(n=>n.Id==ids[target.Id]);
            double dx=copiedSource.X-source.X,dy=copiedSource.Y-source.Y;if(copiedTarget.X-target.X!=dx||copiedTarget.Y-target.Y!=dy)return null;
            var translated=edge.PathPoints.Select(p=>new WirePointDocument(p.X+dx,p.Y+dy)).ToArray();return translated.Any(p=>p.X<0||p.Y<0||p.X>5000||p.Y>5000)?null:translated;
        }
        var links=fragment.ExecutionLinks.Select(c=>c with {FromNode=ids[c.FromNode],ToNode=ids[c.ToNode],PathPoints=TranslatedPath(c)}).ToArray();
        ChangeProcedure(p=>p with {Nodes=p.Nodes.Concat(nodes).ToArray(),Connections=p.Connections.Concat(connections).ToArray(),ExecutionLinks=ProcedureGraph.ExecutionEdges(p).Concat(links).ToArray()});
        return nodes.Select(n=>n.Id).ToArray();
    }
    public void SetNodesEnabled(IEnumerable<string> nodeIds,bool enabled) {
        var ids=nodeIds.ToHashSet(StringComparer.Ordinal);foreach(var id in ids)_=Node(id);
        if(!CurrentProcedure.Nodes.Any(n=>ids.Contains(n.Id)&&n.Enabled!=enabled))return;
        ChangeProcedure(p=>p with {Nodes=p.Nodes.Select(n=>ids.Contains(n.Id)?n with {Enabled=enabled}:n).ToArray()});
    }
    public void SetExecutionPath(ExecutionLinkDocument link,IReadOnlyList<WirePointDocument>? points){
        var current=ProcedureGraph.ExecutionEdges(CurrentProcedure).Single(e=>e.FromNode==link.FromNode&&e.ToNode==link.ToNode);
        if(points is not null){if(points.Count<2||points.Count>10000)throw new ArgumentException("Invalid path length.");foreach(var point in points)Position(point.X,point.Y);for(int i=1;i<points.Count;i++)if(points[i].X!=points[i-1].X&&points[i].Y!=points[i-1].Y)throw new ArgumentException("Path must be orthogonal.");}
        if((current.PathPoints is null&&points is null)||(current.PathPoints is not null&&points is not null&&current.PathPoints.SequenceEqual(points)))return;
        ChangeProcedure(p=>p with{ExecutionLinks=ProcedureGraph.ExecutionEdges(p).Select(e=>e.FromNode==link.FromNode&&e.ToNode==link.ToNode?e with{PathPoints=points?.ToArray()}:e).ToArray()});
    }
    public void InsertExecutionNode(ExecutionLinkDocument link,string nodeId,double x,double y){
        Position(x,y);_=Node(nodeId);var p=CurrentProcedure;var candidate=InsertExecution(p,link,nodeId);
        candidate=candidate with{Nodes=candidate.Nodes.Select(n=>n.Id==nodeId?n with{X=x,Y=y}:n).ToArray(),ExecutionLinks=candidate.ExecutionLinks!.Select(e=>e.FromNode==nodeId||e.ToNode==nodeId?e with{PathPoints=null}:e).ToArray()};
        ChangeProcedure(_=>candidate);
    }
    ProcedureDocument InsertExecution(ProcedureDocument p,ExecutionLinkDocument link,string nodeId){
        if(nodeId==link.FromNode||nodeId==link.ToNode)throw new InvalidOperationException("Cannot insert an endpoint into its own connection.");
        var edges=ProcedureGraph.ExecutionEdges(p);if(!edges.Any(e=>e.FromNode==link.FromNode&&e.ToNode==link.ToNode))throw new InvalidOperationException("Connection no longer exists.");
        var newEdges=edges.Where(e=>e.FromNode!=link.FromNode||e.ToNode!=link.ToNode).ToList();
        foreach(var edge in new[]{new ExecutionLinkDocument(link.FromNode,nodeId){SourceIndex=link.SourceIndex,TargetIndex=link.TargetIndex},new ExecutionLinkDocument(nodeId,link.ToNode){SourceIndex=link.SourceIndex,TargetIndex=link.TargetIndex}}){int existing=newEdges.FindIndex(e=>e.FromNode==edge.FromNode&&e.ToNode==edge.ToNode);if(existing<0)newEdges.Add(edge);else newEdges[existing]=edge;}
        var candidate=p with{ExecutionLinks=newEdges.ToArray()};if(GraphValidator.Validate(candidate,registry).Issues.Any(i=>i.Code=="Cycle"))throw new InvalidOperationException("Insertion creates a cycle.");return candidate;
    }
    public void SetParameter(string id,string name,JsonElement value) {
        var node=Node(id); if(!registry.GetDescriptor(node).Parameters.Any(p=>p.Name==name))throw new ArgumentException("Unknown parameter: "+name);
        if(value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)throw new ArgumentException("A parameter value is required.");
        var cloned=value.Clone(); ChangeNode(id,n=> { var parameters=n.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone()); parameters[name]=cloned; return n with {Parameters=parameters}; });
    }
    public void SetParameters(string nodeId,Dictionary<string,JsonElement> values) {
        var n=Node(nodeId); var parameters=CheckedParameters(n,values); ChangeNode(nodeId,n=>n with {Parameters=parameters});
    }
    public void UpdateNode(string nodeId,string name,bool enabled,int timeoutMs,Dictionary<string,JsonElement> parameters) {
        RequireName(name); if(timeoutMs<0)throw new ArgumentOutOfRangeException(nameof(timeoutMs));
        var values=CheckedParameters(Node(nodeId),parameters);
        ChangeNode(nodeId,n=>n with {Name=name,Enabled=enabled,TimeoutMs=timeoutMs,Parameters=values});
    }
    Dictionary<string,JsonElement> CheckedParameters(NodeDocument node,Dictionary<string,JsonElement> values) {
        ArgumentNullException.ThrowIfNull(values);
        var copied=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());
        var test=node with {Parameters=copied};
        var validation=GraphValidator.Validate(new("validation","validation",[test],[]),registry);
        var errors=validation.Issues.Where(i=>i.Code is "UnknownParameter" or "ParameterValue" or "MissingParameter").ToArray();
        if(errors.Length>0)throw new ArgumentException(string.Join("; ",errors.Select(i=>i.Code+": "+i.Message)));
        return copied;
    }
    public void SetNodeName(string id,string name) { RequireName(name); ChangeNode(id,n=>n with {Name=name}); }
    public void SetNodeEnabled(string id,bool enabled) => ChangeNode(id,n=>n with {Enabled=enabled});
    public void SetTimeout(string id,int timeoutMs) { if(timeoutMs<0)throw new ArgumentOutOfRangeException(nameof(timeoutMs)); ChangeNode(id,n=>n with {TimeoutMs=timeoutMs}); }
    public void Connect(string fromNode,string fromPort,string toNode,string toPort)=>BindInput(fromNode,fromPort,toNode,toPort,true);
    public void BindInput(string fromNode,string fromPort,string toNode,string toPort,bool addExecutionLink=false) {
        var p=CurrentProcedure; var a=Node(fromNode); var b=Node(toNode);
        var output=registry.GetDescriptor(a).Outputs.SingleOrDefault(p=>p.Name==fromPort);
        var input=registry.GetDescriptor(b).Inputs.SingleOrDefault(p=>p.Name==toPort);
        if(output is null||input is null)throw new InvalidOperationException("Invalid port direction or name.");
        if(output.Kind!=input.Kind||output.Cardinality!=input.Cardinality)throw new InvalidOperationException("Port type mismatch.");
        if(p.Connections.Any(c=>c.ToNode==toNode&&c.ToPort==toPort))throw new InvalidOperationException("Input already connected.");
        var pending=new Stack<string>(); var visited=new HashSet<string>(); pending.Push(toNode);
        while(pending.TryPop(out var id)) { if(id==fromNode)throw new InvalidOperationException("Connection creates a cycle."); if(!visited.Add(id))continue; foreach(var c in p.Connections.Where(c=>c.FromNode==id))pending.Push(c.ToNode);if(addExecutionLink)foreach(var link in ProcedureGraph.ExecutionEdges(p).Where(e=>e.FromNode==id))pending.Push(link.ToNode); }
        ChangeProcedure(p=>p with { Connections=p.Connections.Append(new ConnectionDocument(fromNode,fromPort,toNode,toPort)).ToArray(),ExecutionLinks=addExecutionLink?ProcedureGraph.ExecutionEdges(p).Append(new ExecutionLinkDocument(fromNode,toNode)).Distinct().ToArray():p.ExecutionLinks });
    }
    public void ConnectExecution(string fromNode,string toNode,int sourceIndex=3,int targetIndex=1){if(sourceIndex is <0 or >3||targetIndex is <0 or >3)throw new ArgumentOutOfRangeException(nameof(sourceIndex));_=Node(fromNode);_=Node(toNode);var p=CurrentProcedure;var edge=new ExecutionLinkDocument(fromNode,toNode){SourceIndex=sourceIndex,TargetIndex=targetIndex};var links=ProcedureGraph.ExecutionEdges(p);if(links.Any(e=>e.FromNode==fromNode&&e.ToNode==toNode))throw new InvalidOperationException("Execution link already exists.");var candidate=p with{ExecutionLinks=links.Append(edge).ToArray()};if(GraphValidator.Validate(candidate,registry).Issues.Any(i=>i.Code=="Cycle"))throw new InvalidOperationException("Execution link creates a cycle.");ChangeProcedure(_=>candidate);}
    public void DisconnectExecution(ExecutionLinkDocument link){var p=CurrentProcedure;var links=ProcedureGraph.ExecutionEdges(p);if(!links.Any(e=>e.FromNode==link.FromNode&&e.ToNode==link.ToNode))throw new KeyNotFoundException("Execution link not found.");ChangeProcedure(p=>p with{ExecutionLinks=links.Where(e=>e.FromNode!=link.FromNode||e.ToNode!=link.ToNode).ToArray()});}
    public void Disconnect(ConnectionDocument connection) { if(!CurrentProcedure.Connections.Contains(connection))throw new KeyNotFoundException("Connection not found."); ChangeProcedure(p=>p with {Connections=p.Connections.Where(c=>c!=connection).ToArray()}); }
    public void DeleteNode(string id)=>DeleteNodes([id]);
    public void Undo() => Restore(undo,redo);
    public void Redo() => Restore(redo,undo);
    public ProcedureValidationResult Validate() => GraphValidator.Validate(CurrentProcedure,registry);
    NodeDocument Node(string id)=>CurrentProcedure.Nodes.SingleOrDefault(n=>n.Id==id)??throw new KeyNotFoundException(id);
    void ChangeNode(string id,Func<NodeDocument,NodeDocument> action) { _=Node(id); ChangeProcedure(p=>p with {Nodes=p.Nodes.Select(n=>n.Id==id?action(n):n).ToArray()}); }
    void ChangeProcedure(Func<ProcedureDocument,ProcedureDocument> action)=>Edit(d=>d with {Procedures=d.Procedures.Select(p=>p.Id==CurrentProcedureId?action(p):p).ToArray()});
    void Edit(Func<SolutionDocument,SolutionDocument> action,string? selectedProcedure=null) { var next=Copy(PruneOwnedGroups(CloneOwnedCopies(action(Copy(document))))); SolutionStore.ValidateStructure(next); undo.Push(Capture());redo.Clear();document=next;if(selectedProcedure is not null)CurrentProcedureId=selectedProcedure;if(!document.Procedures.Any(p=>p.Id==CurrentProcedureId))CurrentProcedureId=document.Procedures.FirstOrDefault(p=>p.GroupOwner is null)?.Id??"";IsDirty=true;Revision++;Changed?.Invoke(); }
    Snapshot Capture()=>new(Copy(document),CurrentProcedureId,BaseDirectory);
    void Restore(Stack<Snapshot> source,Stack<Snapshot> destination) { if(source.Count==0)return; var s=source.Peek();var next=Rebase(s.Document,s.BaseDirectory,BaseDirectory);destination.Push(Capture());source.Pop();document=next;CurrentProcedureId=s.ProcedureId;IsDirty=true;Revision++;Changed?.Invoke(); }
    static void Position(double x,double y) { if(!double.IsFinite(x)||!double.IsFinite(y))throw new ArgumentOutOfRangeException(nameof(x)); }
    static void RequireName(string name) { if(string.IsNullOrWhiteSpace(name))throw new ArgumentException("Name is required."); }
    static SolutionDocument Copy(SolutionDocument d)=>d with {Resources=d.Resources.ToArray(),Procedures=d.Procedures.Select(p=>p with {Configuration=p.Configuration.Snapshot(),Connections=p.Connections.ToArray(),ExecutionLinks=p.ExecutionLinks?.ToArray(),Nodes=p.Nodes.Select(n=>n with {Call=n.Call?.Snapshot(),Parameters=n.Parameters.ToDictionary(k=>k.Key,k=>k.Value.Clone()),Rois=n.Rois?.Select(r=>r with {Vertices=r.Vertices?.ToArray()}).ToArray()}).ToArray()}).ToArray()};
    static SolutionDocument Rebase(SolutionDocument d,string oldBase,string newBase) {
        var copy=Copy(d); string PathValue(string path)=>Path.IsPathFullyQualified(path)?path:Path.GetRelativePath(newBase,Path.GetFullPath(path,oldBase));
        return copy with { Resources=copy.Resources.Select(r=>r with {Path=PathValue(r.Path)}).ToArray(),Procedures=copy.Procedures.Select(p=>p with {Nodes=p.Nodes.Select(n=> {if(n.ModuleId!="vision.image-file"||!n.Parameters.TryGetValue("Path",out var v)||v.ValueKind!=JsonValueKind.String)return n; var values=n.Parameters.ToDictionary(k=>k.Key,k=>k.Value.Clone());values["Path"]=JsonSerializer.SerializeToElement(PathValue(v.GetString()!));return n with {Parameters=values};}).ToArray()}).ToArray() };
    }
}



