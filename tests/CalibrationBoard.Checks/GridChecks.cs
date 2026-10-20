using System.IO;
using System.IO.Compression;
using System.Text.Json;
using VisionPlugins.CalibrationBoard;
static class GridChecks
{
 public static void Run(Action<bool,string> check)
 {
  using var fixture=JsonDocument.Parse(File.ReadAllText("docs/plugins/calibration-board-circle-grid-fixtures.json"));var grids=fixture.RootElement.GetProperty("rows").EnumerateArray().ToDictionary(r=>r.GetProperty("index").GetInt32());var differences=new List<object>();
  foreach(var phase in fixture.RootElement.GetProperty("adjacency").EnumerateArray().Select(r=>(Row:r,Pruned:false)).Concat(fixture.RootElement.GetProperty("pruned").EnumerateArray().Select(r=>(Row:r,Pruned:true))))
  {
   var row=phase.Row;int index=row.GetProperty("index").GetInt32();float orientation=row.GetProperty("orientation").GetSingle();var points=grids[index].GetProperty("inputs").EnumerateArray().Select(p=>new CalibrationBoardCircleCandidate(p[0].GetSingle(),p[1].GetSingle(),true,p[2].GetSingle())).ToArray();var actual=phase.Pruned?CalibrationBoardCircleGrid.PrunedNeighborWords(points,orientation):CalibrationBoardCircleGrid.NeighborWords(points,orientation);
   using var input=new MemoryStream(Convert.FromBase64String(row.GetProperty("zlibBase64").GetString()!));using var stream=new ZLibStream(input,CompressionMode.Decompress);using var output=new MemoryStream();stream.CopyTo(output);var bytes=output.ToArray();int ids=0,fields=0;var details=new List<object>();
   for(int i=0;i<actual.Length;i++){for(int slot=0;slot<4;slot++){int offset=slot*5,native=BitConverter.ToInt32(bytes,(i*21+offset)*4);if(actual[i][offset]!=native){ids++;details.Add(new{node=i,slot,originalNeighbor=native,managedNeighbor=actual[i][offset],input=points[i],originalVector=new[]{BitConverter.Int32BitsToSingle(BitConverter.ToInt32(bytes,(i*21+offset+2)*4)),BitConverter.Int32BitsToSingle(BitConverter.ToInt32(bytes,(i*21+offset+3)*4))},managedVector=new[]{BitConverter.Int32BitsToSingle(actual[i][offset+2]),BitConverter.Int32BitsToSingle(actual[i][offset+3])}});}foreach(int field in native>=0?new[]{1,2,3,4}:new[]{4})if(actual[i][offset+field]!=BitConverter.ToInt32(bytes,(i*21+offset+field)*4))fields++;}if(actual[i][20]!=BitConverter.ToInt32(bytes,(i*21+20)*4))fields++;}
   differences.Add(new{index,orientation,pruned=phase.Pruned,idDifferences=ids,fieldDifferences=fields,details});
  }
  File.WriteAllText("analysis/calibration-board/circle-grid-native/adjacency-differences.json",JsonSerializer.Serialize(differences,new JsonSerializerOptions{WriteIndented=true}));
  var componentDifferences=new List<object>();foreach(var phase in fixture.RootElement.GetProperty("components").EnumerateArray().Select(r=>(Row:r,Canonical:false)).Concat(fixture.RootElement.GetProperty("canonical").EnumerateArray().Select(r=>(Row:r,Canonical:true))))
  {
   var row=phase.Row;int index=row.GetProperty("index").GetInt32();var points=grids[index].GetProperty("inputs").EnumerateArray().Select(p=>new CalibrationBoardCircleCandidate(p[0].GetSingle(),p[1].GetSingle(),true,p[2].GetSingle())).ToArray();var component=phase.Canonical?CalibrationBoardCircleGrid.CanonicalComponent(points,row.GetProperty("orientation").GetSingle()):CalibrationBoardCircleGrid.LargestComponent(points,row.GetProperty("orientation").GetSingle());var expected=row.GetProperty("points").EnumerateArray().ToArray();
   int differencesCount=Math.Abs(component.Points.Count-expected.Length);for(int i=0;i<Math.Min(component.Points.Count,expected.Length);i++){var a=component.Points[i];var b=expected[i];if(a.CandidateId!=b[0].GetInt32()||a.IndexX!=b[1].GetInt32()||a.IndexY!=b[2].GetInt32()||a.X!=b[3].GetSingle()||a.Y!=b[4].GetSingle()||a.Size!=b[5].GetSingle())differencesCount++;}
   componentDifferences.Add(new{index,canonical=phase.Canonical,originalCount=expected.Length,managedCount=component.Points.Count,differencesCount});
  }
  File.WriteAllText("analysis/calibration-board/circle-grid-native/component-differences.json",JsonSerializer.Serialize(componentDifferences,new JsonSerializerOptions{WriteIndented=true}));
  foreach(var item in componentDifferences){using var row=JsonDocument.Parse(JsonSerializer.Serialize(item));check(row.RootElement.GetProperty("differencesCount").GetInt32()==0,"native largest consistent component, order and normalized coordinates "+row.RootElement.GetProperty("index"));}
  var finalDifferences=new List<object>();foreach(var row in grids.Values)
  {
   int index=row.GetProperty("index").GetInt32();var points=row.GetProperty("inputs").EnumerateArray().Select(p=>new CalibrationBoardCircleCandidate(p[0].GetSingle(),p[1].GetSingle(),true,p[2].GetSingle())).ToArray();var actual=CalibrationBoardCircleGrid.Match(points);var expected=row.GetProperty("outputs").EnumerateArray().ToArray();int difference=Math.Abs(actual.Points.Count-expected.Length);
   for(int i=0;i<Math.Min(actual.Points.Count,expected.Length);i++){var a=actual.Points[i];var b=expected[i];if(a.IndexX!=b[2].GetInt32()||a.IndexY!=b[1].GetInt32()||a.X!=b[4].GetSingle()||a.Y!=b[5].GetSingle())difference++;}finalDifferences.Add(new{index,originalCount=expected.Length,managedCount=actual.Points.Count,difference});
  }
  File.WriteAllText("analysis/calibration-board/circle-grid-native/complete-grid-differences.json",JsonSerializer.Serialize(finalDifferences,new JsonSerializerOptions{WriteIndented=true}));
  foreach(var item in finalDifferences){using var row=JsonDocument.Parse(JsonSerializer.Serialize(item));check(row.RootElement.GetProperty("difference").GetInt32()==0,"native complete grid matching "+row.RootElement.GetProperty("index"));}
  foreach(var item in differences){using var row=JsonDocument.Parse(JsonSerializer.Serialize(item));check(row.RootElement.GetProperty("idDifferences").GetInt32()==0,"native grid neighbor ids "+row.RootElement.GetProperty("index"));check(row.RootElement.GetProperty("fieldDifferences").GetInt32()==0,"native grid neighbor ratio/direction/distance float fields");}
 }
}
