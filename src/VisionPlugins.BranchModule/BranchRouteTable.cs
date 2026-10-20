#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace VisionPlugins.BranchModule;
public sealed record BranchStoredRoute(string NodeId,int Value);
public sealed record BranchConnectedNode(string NodeId,int? NumericId=null);
public sealed record BranchTopologyUpdate(IReadOnlyList<BranchStoredRoute> Rows,bool UsesNodeIdIdentityAdapter);
public static class BranchRouteTable {
 // Only the host's real connection add/remove path calls this method. Load,
 // opening an editor and algorithm Reset deliberately do not call it.
 public static BranchTopologyUpdate ApplyTopologyChange(IReadOnlyList<BranchStoredRoute> rows,IReadOnlyList<BranchConnectedNode> previous,IReadOnlyList<BranchConnectedNode> current){
  if(previous.Select(x=>x.NodeId).SequenceEqual(current.Select(x=>x.NodeId)))return new(Array.AsReadOnly(rows.ToArray()),false);
  var eligible=current.Where(x=>x.NumericId is not int id||id<10000||unchecked((uint)(id-21000))<1000).ToArray();
  var result=rows.Where(row=>eligible.Any(next=>next.NodeId==row.NodeId)).ToList();
  foreach(var next in eligible)if(!result.Any(row=>row.NodeId==next.NodeId))result.Add(new(next.NodeId,0));
  return new(result.AsReadOnly(),eligible.Any(x=>x.NumericId is null));
 }
 public static IReadOnlyList<BranchStoredRoute> Parse(string? serialized){
  var result=new List<BranchStoredRoute>();if(string.IsNullOrWhiteSpace(serialized))return result.AsReadOnly();
  // The normal UI supplies terminated pairs. Unlike the original native atoi,
  // host NodeId text is preserved verbatim as an explicitly independent identity.
  var parts=serialized.Split('$');foreach(string part in parts.Where(x=>x.Length>0)){var fields=part.Split('#');if(fields.Length<2)throw new FormatException("Invalid ModuleInfoList row.");int value=int.TryParse(fields[1],NumberStyles.Integer,CultureInfo.CurrentCulture,out var n)?n:0;result.Add(new(fields[0],value));}return result.AsReadOnly();
 }
 public static string Serialize(IReadOnlyList<BranchStoredRoute> rows)=>string.Concat(rows.Select(row=>row.NodeId+"#"+row.Value.ToString(CultureInfo.InvariantCulture)+"$"));
}
