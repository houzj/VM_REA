namespace VisionPlugins.Calibration;
public sealed record NImageCornerTrack(int IndexX,int IndexY,IReadOnlyList<NImageBoardPoint> Pixels);
/// <summary>Source 7cb190: first-frame ordering, exact world-ID intersection across the batch.
/// Module ProcessOutput exports only the first track, while CornerPairsNum counts all tracks.</summary>
public static class NImageTrackedCorners{
 public static IReadOnlyList<NImageCornerTrack> Build(IReadOnlyList<NImageDetectedBoard> boards){
  if(boards.Count==0)return [];var maps=boards.Select(b=>b.Points.ToDictionary(p=>(p.IndexX,p.IndexY))).ToArray();var result=new List<NImageCornerTrack>();
  foreach(var first in boards[0].Points){var key=(first.IndexX,first.IndexY);if(maps.Any(m=>!m.ContainsKey(key)))continue;result.Add(new(first.IndexX,first.IndexY,Array.AsReadOnly(maps.Select(m=>m[key]).ToArray())));}
  return result.AsReadOnly();
 }
}
