namespace VisionFramework;
public sealed class SolutionExecutor(ModuleRegistry registry){
    // Explicit independent host call: route only successful current caller exports.
    public Task<ExecutionSessionResult> CallAsync(SolutionWorkspace workspace,string procedureId,ExecutionSessionResult caller,IReadOnlyDictionary<string,string> inputToOutput,CancellationToken cancellationToken=default){
        if(caller.Status!=SessionStatus.Success)throw new InvalidDataException("Cannot call from unsuccessful workflow.");
        var inputs=new Dictionary<string,PortValue>(StringComparer.Ordinal);
        foreach(var map in inputToOutput)inputs.Add(map.Key,caller.WorkflowOutputs.TryGetValue(map.Value,out var value)?value:throw new InvalidDataException("Caller output unavailable: "+map.Value));
        return RunAsync(workspace,procedureId,cancellationToken:cancellationToken,inputs:inputs);
    }
    // Initial policy: all declared resources must be available and match their optional hash.
    public async Task<ExecutionSessionResult> RunAsync(SolutionWorkspace workspace,string procedureId,IProgress<ExecutionEvent>? progress=null,CancellationToken cancellationToken=default,IReadOnlyDictionary<string,PortValue>? inputs=null){
        SolutionStore.ValidateStructure(workspace.Document);
        var document=workspace.Document with{Resources=workspace.Document.Resources.ToArray(),Procedures=workspace.Document.Procedures.Select(p=>p with{Configuration=p.Configuration.Snapshot(),Connections=p.Connections.ToArray(),ExecutionLinks=p.ExecutionLinks?.ToArray(),Nodes=p.Nodes.Select(n=>n with{Call=n.Call?.Snapshot(),Parameters=n.Parameters.ToDictionary(v=>v.Key,v=>v.Value.Clone()),Rois=n.Rois?.Select(r=>r with{Vertices=r.Vertices?.ToArray()}).ToArray()}).ToArray()}).ToArray()};
        WorkflowCalls.Validate(document,procedureId);
        var resources=document.Resources.ToArray();var catalog=new ResourceCatalog(workspace.BaseDirectory,resources);
        foreach(var resource in resources){if(cancellationToken.IsCancellationRequested)break;using var stream=catalog.OpenVerified(resource.Id);}
        async Task<ExecutionSessionResult> Execute(string id,IReadOnlyDictionary<string,PortValue>? arguments,CancellationToken token,int depth,int? groupLoopIndex=null){
            if(depth>32)throw new InvalidDataException("Independent subprogram depth limit is 32.");
            var procedure=document.Procedures.SingleOrDefault(p=>p.Id==id)??throw new InvalidDataException("Unknown procedure: "+id);
            async Task<ExecutionSessionResult> Call(NodeDocument node,IReadOnlyDictionary<string,PortValue> values,CancellationToken childToken){
                var body=document.Procedures.Single(p=>p.Id==node.Call!.ProcedureId);
                // Legacy cross-flow calls retain one-shot behavior. Native Group loops own their body.
                if(body.GroupOwner is null)return await Execute(body.Id,values,childToken,depth+1);
                var loop=GroupLoop.Read(node);ExecutionSessionResult? last=null;int completed=0;
                for(long index=loop.Start;index<loop.End;index++){
                    childToken.ThrowIfCancellationRequested();
                    // Independent resource guard; this is not a recovered VM iteration limit.
                    if(completed>=10000)throw new InvalidOperationException("Independent Group iteration limit is 10000.");
                    last=await Execute(body.Id,values,childToken,depth+1,(int)index);completed++;
                    if(last.Status!=SessionStatus.Success)break;
                    if(loop.IntervalMs>0)await Task.Delay(loop.IntervalMs,childToken);
                }
                return (last??new ExecutionSessionResult(SessionStatus.Success,[])) with{CompletedGroupIterations=completed};
            }
            return (await new GraphExecutor(registry).RunAsync(procedure,workspace.BaseDirectory,depth==0?progress:null,token,arguments,Call,groupLoopIndex)) with{GroupLoopIndex=groupLoopIndex};
        }
        return await Execute(procedureId,inputs,cancellationToken,0);
    }
}
