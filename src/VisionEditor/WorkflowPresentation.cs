using System.IO;
using System.Text.Json;
namespace VisionEditor;
internal static class WorkflowPresentation
{
 sealed record Labels(string Module,Dictionary<string,string> Parameters,Dictionary<string,string> Outputs);
 static readonly Labels labels=JsonSerializer.Deserialize<Labels>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"workflow-labels.json")))!;
 internal static string Parameter(string module,string name)=>module==labels.Module?labels.Parameters.GetValueOrDefault(name)??name:name;
 internal static string Output(string module,string name)=>module==labels.Module?labels.Outputs.GetValueOrDefault(name)??name:name;
}
