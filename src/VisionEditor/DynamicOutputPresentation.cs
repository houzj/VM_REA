using VisionFramework;
using VisionEditor.Core;

namespace VisionEditor;

public partial class MainWindow
{
    readonly Dictionary<string,Dictionary<string,ModuleDescriptor>> liveDynamicDescriptors=new(StringComparer.Ordinal);

    ModuleDescriptor? RuntimeDescriptor(NodeDocument node)
    {
        if(liveDynamicDescriptors.TryGetValue(doc.CurrentProcedureId,out var current)&&current.TryGetValue(node.Id,out var live))return live;
        return workflowSessions.TryGetValue(doc.CurrentProcedureId,out var session)&&session.NodeDescriptors.TryGetValue(node.Id,out var descriptor)&&descriptor.SupportsDynamicOutputs?descriptor:null;
    }

    void ClearLiveDynamicDescriptors(string procedureId)=>liveDynamicDescriptors.Remove(procedureId);

    IProgress<ExecutionEvent> CreateExecutionProgress(string procedureId, ExecutionOwnerStamp owner, uint runSequence)=>new Progress<ExecutionEvent>(executionEvent=>
    {
        if(!IsCurrentExecutionOwner(owner)||workflowExecuteCounts.GetValueOrDefault(procedureId)!=runSequence)return;
        if(executionEvent.UpdatedDescriptor is not {SupportsDynamicOutputs:true} descriptor)return;
        if(!liveDynamicDescriptors.TryGetValue(procedureId,out var descriptors))liveDynamicDescriptors[procedureId]=descriptors=new(StringComparer.Ordinal);
        descriptors[executionEvent.NodeId]=descriptor;
        if(!windowClosed&&doc.HasCurrentProcedure&&doc.CurrentProcedureId==procedureId&&!workflowOverview)RenderGraph();
    });

    object[] ModuleResultRows(NodeDocument node,NodeRunRecord record)
    {
        var rows=record.Outputs.Select(pair=>(object)new ModuleResultViewRow(pair.Key,pair.Value.Kind+(pair.Value.Cardinality==ValueCardinality.Array?"[]":""),FormatOutput(pair.Value))).ToList();
        if(RuntimeDescriptor(node) is {SupportsDynamicOutputs:true} descriptor)
        {
            foreach(var output in descriptor.Outputs)
            {
                var kind=output.Kind+(output.Cardinality==ValueCardinality.Array?"[]":"")+" · "+(output.Cardinality==ValueCardinality.Array?"数组":"单值")+" · "+(output.Required?"必需":"可选");
                var value=record.Outputs.TryGetValue(output.Name,out var actual)?FormatOutput(actual):"本次运行未输出";
                rows.Insert(0,new ModuleResultViewRow("运行端口 · "+output.Name,kind,value));
            }
        }
        return rows.ToArray();
    }
}

sealed record ModuleResultViewRow(string Port,string Kind,string Value);
