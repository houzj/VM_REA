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
    public async Task<ExecutionSessionResult> RunAsync(SolutionWorkspace workspace,string procedureId,IProgress<ExecutionEvent>? progress=null,CancellationToken cancellationToken=default,IReadOnlyDictionary<string,PortValue>? inputs=null,ModuleExecutionScope? scope=null,ModuleRunControl? control=null){
        SolutionStore.ValidateStructure(workspace.Document);
        var document=workspace.Document with{Resources=workspace.Document.Resources.ToArray(),Procedures=workspace.Document.Procedures.Select(p=>p with{Configuration=p.Configuration.Snapshot(),Connections=p.Connections.ToArray(),ExecutionLinks=p.ExecutionLinks?.ToArray(),Nodes=p.Nodes.Select(n=>n with{Call=n.Call?.Snapshot(),Parameters=n.Parameters.ToDictionary(v=>v.Key,v=>v.Value.Clone()),Rois=n.Rois?.Select(r=>r with{Vertices=r.Vertices?.ToArray()}).ToArray()}).ToArray()}).ToArray()};
        WorkflowCalls.Validate(document,procedureId);
        var resources=document.Resources.ToArray();var catalog=new ResourceCatalog(workspace.BaseDirectory,resources);
        foreach(var resource in resources){if(cancellationToken.IsCancellationRequested)break;using var stream=catalog.OpenVerified(resource.Id);}
        async Task<ExecutionSessionResult> Execute(string id,IReadOnlyDictionary<string,PortValue>? arguments,CancellationToken token,int depth){
            if(depth>32)throw new InvalidDataException("Independent subprogram depth limit is 32.");
            var procedure=document.Procedures.SingleOrDefault(p=>p.Id==id)??throw new InvalidDataException("Unknown procedure: "+id);
            return await new GraphExecutor(registry).RunAsync(procedure,workspace.BaseDirectory,depth==0?progress:null,token,arguments,(node,values,childToken)=>Execute(node.Call!.ProcedureId,values,childToken,depth+1),scope,control,catalog);
        }
        return await Execute(procedureId,inputs,cancellationToken,0);
    }
}