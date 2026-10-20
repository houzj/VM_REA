using System.Text;
namespace VisionPlugins.BranchString;
public sealed record BranchStringRow(string NodeId,string Text,int DebugFlag);
public sealed record BranchStringSuccessor(string NodeId,int? NumericId=null);
public sealed record BranchStringSelection(IReadOnlyList<string> NodeIds,string BranchMatch,int ModuleStatus,bool UsesNodeIdAdapter);
// Source Process 17890 decision slice. Caller owns the source GetString error/
// Count gate and its single capacity256 read. No SDK calls occur here.
public static class BranchStringProcess {
 public static BranchStringSelection Select(string input,IReadOnlyList<BranchStringRow> rows,IReadOnlyList<BranchStringSuccessor> successors,int? reportedBytes=null){
  input=input.Split('\0')[0];if(successors.Count==0)return new(Array.Empty<string>(),"",0,false);
  if((reportedBytes??checked(Encoding.UTF8.GetByteCount(input)+1))<1)return new(Array.Empty<string>(),"No Module",0,false);
  var selected=new List<string>();var text=new StringBuilder();bool adapter=false;
  foreach(var row in rows){bool debug=row.DebugFlag==1;if(!debug&&!string.Equals(row.Text,input,StringComparison.Ordinal))continue;if(debug)text.Append("(D)");foreach(var next in successors)if(next.NodeId==row.NodeId){selected.Add(next.NodeId);if(next.NumericId is int id)text.Append("ID").Append(id.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(';');else{adapter=true;text.Append("NodeId:").Append(next.NodeId).Append(';');}}}
  bool matched=text.Length>0;return new(selected.AsReadOnly(),matched?text.ToString():"No Module",matched?1:0,adapter);
 }
}

