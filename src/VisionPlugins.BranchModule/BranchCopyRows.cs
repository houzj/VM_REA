using System.Text;
namespace VisionPlugins.BranchModule;
// Candidate: normal source Paste consumes terminated rows and retains raw value
// text (including N), not the runtime's parsed Int32. Null means source did not
// assign ModuleInfoList because no mapped target survived.
public static class BranchCopyRows {
 public static string? Remap(string? source,IReadOnlyDictionary<string,string> nodeMap){
  if(string.IsNullOrEmpty(source))return null;var parts=source.Split('$');var result=new StringBuilder();
  for(int i=0;i<parts.Length-1;i++){var fields=parts[i].Split('#');if(fields.Length<2)throw new FormatException("Invalid source Branch copy row.");if(nodeMap.TryGetValue(fields[0],out var mapped))result.Append(mapped).Append('#').Append(fields[1]).Append('$');}
  return result.Length==0?null:result.ToString();
 }
}
