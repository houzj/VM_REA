using System.Text.Json;
namespace VisionFramework;

public sealed partial class ModuleExecutionScope {
 sealed class EffectiveParameters {public readonly object Gate=new();public Dictionary<string,JsonElement>? Seed,Values;}
 readonly Dictionary<(string Procedure,string Module,string Node),EffectiveParameters> effectiveParameters=new();
 EffectiveParameters Parameters(string module,string node){var root=Root;lock(root.gate){ObjectDisposedException.ThrowIf(root.closing,this);var key=(ProcedureId,module,node);if(!root.effectiveParameters.TryGetValue(key,out var value))root.effectiveParameters[key]=value=new();return value;}}
 /// <summary>Managed module-entry parameters, after binding resolution. Not an acknowledgement of native SDK setters.</summary>
 public void RememberExecutionParameters(string module,string node,IReadOnlyDictionary<string,JsonElement> seed,IReadOnlyDictionary<string,JsonElement> applied){var state=Parameters(module,node);lock(state.Gate){state.Seed=seed.ToDictionary(p=>p.Key,p=>p.Value.Clone());state.Values=applied.ToDictionary(p=>p.Key,p=>p.Value.Clone());}}
 public IReadOnlyDictionary<string,JsonElement>? ReadEffectiveParameters(string module,string node,IReadOnlyDictionary<string,JsonElement> current){var state=Parameters(module,node);lock(state.Gate){if(state.Values is null)return null;var result=state.Values.ToDictionary(p=>p.Key,p=>p.Value.Clone());foreach(var pair in current)if(state.Seed is null||!state.Seed.TryGetValue(pair.Key,out var old)||old.GetRawText()!=pair.Value.GetRawText())result[pair.Key]=pair.Value.Clone();return result;}}
 public void RememberConfigurationParameters(string module,string node,IReadOnlyDictionary<string,JsonElement> before,IReadOnlyDictionary<string,JsonElement> after,bool reset=false){var state=Parameters(module,node);lock(state.Gate){if(state.Values is null&&!reset)return;state.Seed??=before.ToDictionary(p=>p.Key,p=>p.Value.Clone());state.Values??=new();foreach(var pair in after)if(reset||!before.TryGetValue(pair.Key,out var old)||old.GetRawText()!=pair.Value.GetRawText()){state.Seed[pair.Key]=pair.Value.Clone();state.Values[pair.Key]=pair.Value.Clone();}}}
}
