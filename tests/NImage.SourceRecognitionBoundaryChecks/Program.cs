using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.Calibration;

// A managed boundary experiment on existing fixtures, never an original-runtime oracle.
if(args.Length!=1) throw new ArgumentException("Expected a fresh output JSON path");
if(File.Exists(args[0])) throw new IOException("Preserve the prior diagnostic output");
var rows=new List<object>();
var ordered=new List<NImageSourceGridPoint>{new(-2,3,10,20),new(-1,4,30,40),new(-2,3,11,21)};
var synthetic=NImageSourceGridContext.FromResult(new(ordered,2,2,0))??throw new Exception("Missing dense context");
if(synthetic.MinColumn!=-2||synthetic.MinRow!=3||synthetic.Cells[0]!=new NImageSourceGridCell(-2,3,11,21)||synthetic.Cells[1]!=new NImageSourceGridCell(int.MinValue,int.MinValue,null,null)||synthetic.Cells[3]!=new NImageSourceGridCell(-1,4,30,40))throw new Exception("Source cell offsets, last-write wins or unknown missing XY changed");
ordered.Clear();if(synthetic.OrderedPoints.Count!=3)throw new Exception("Context did not snapshot source order");
// Native wrapper filters and float-adds the ROI origin before recomputing minima;
// it preserves the original grid allocation dimensions even if the lowest ID vanished.
var projected=synthetic.RebuildForProjectedPoints(new[]{new NImageSourceGridPoint(-1,4,130,240)})??throw new Exception("Projected context missing");
if(projected.Width!=2||projected.Height!=2||projected.MinColumn!=-1||projected.MinRow!=4||projected.Cells[0]!=new NImageSourceGridCell(-1,4,130,240)||projected.Cells[1].HasIdentity)throw new Exception("Filtered minima, original grid dimensions or global XY lost");
if(synthetic.MinColumn!=-2||synthetic.Cells[0].X!=11)throw new Exception("Projection mutated original local context");
if(synthetic.RebuildForProjectedPoints(Array.Empty<NImageSourceGridPoint>()) is not null)throw new Exception("Empty projected context fabricated identities");
bool rejected=false;try{NImageSourceGridContext.FromResult(new(new[]{new NImageSourceGridPoint(0,0,1,2),new NImageSourceGridPoint(2,0,3,4)},2,1,0));}catch(ArgumentException){rejected=true;}
if(!rejected)throw new Exception("Malformed external grid must not write beyond the dense grid");
foreach(var name in new[]{"source","target"}) {
 var bytes=File.ReadAllBytes($"analysis/map-calibration/image-input/native-{name}.gray");
 if(bytes.Length!=1000000) throw new InvalidDataException("Expected the existing 1000x1000 fixture");
 var image=new GrayImage(1000,1000,bytes);
 var lattice=NImageBoardDetector.FindLattice(image) ?? throw new Exception("Existing lattice unavailable");
  var sourceLattice=NImageSourceLatticeAdapter.Find(image)??throw new Exception("Existing source adapter unavailable");
  var context=sourceLattice.SourceGrid??throw new Exception("Source adapter lost its grid context");
  if(context.OrderedPoints.Count!=sourceLattice.Points.Count||!context.OrderedPoints.Select(p=>new NImageLatticePoint(p.Column,p.Row,p.X,p.Y,double.NaN)).SequenceEqual(sourceLattice.Points))throw new Exception("Source context and published point order differ");
  foreach(var p in context.OrderedPoints){var cell=context.Cells[(p.Row-context.MinRow)*context.Width+p.Column-context.MinColumn];if(!cell.HasIdentity||cell.Column!=p.Column||cell.Row!=p.Row||cell.X!=p.X||cell.Y!=p.Y)throw new Exception("Dense context differs from source producer");}
 foreach(var boardType in new[]{7,9}) foreach(var reverse in new[]{false,true}) {
  var quads=(reverse?lattice.CodeQuadrilaterals.Reverse():lattice.CodeQuadrilaterals).ToArray();
  int firstSuccess=-1, decodedCount=0;
  for(int i=0;i<quads.Length;i++) {
   var rings=NImageFiducials.Find(image,quads[i]);
   if(rings is null || !NImageCodeDecoder.TryDecode(image,rings,out var code) || code.Orientation!=0) continue;
   var identity=NImageBoardIdentity.FromDecodedBytes(code.Bytes.ToArray(),boardType);
   if(!(identity.GridPitch>0 && float.IsFinite(identity.GridPitch))) continue;
   if(firstSuccess<0) firstSuccess=i;
   decodedCount++;
  }
  if(firstSuccess<0) throw new Exception("Existing fixture has no selected identity");
   var contextual=NImageBoardRecognition.RecognizeLattice(image,boardType,sourceLattice);
   var withoutContext=NImageBoardRecognition.RecognizeLattice(image,boardType,sourceLattice with{SourceGrid=null});
   if((contextual is null)!=(withoutContext is null)||contextual is not null&&!contextual.Points.SequenceEqual(withoutContext!.Points))throw new Exception("Transport-only source context changed current recognition");
  var all=NImageBoardRecognition.RecognizeLattice(image,boardType,lattice with{CodeQuadrilaterals=quads}) ?? throw new Exception("All-proposal recognition failed");
  var prefix=NImageBoardRecognition.RecognizeLattice(image,boardType,lattice with{CodeQuadrilaterals=quads.Take(firstSuccess+1).ToArray()}) ?? throw new Exception("Prefix recognition failed");
  bool sameIdentity=all.Tags[0].Identity==prefix.Tags[0].Identity;
  if(!sameIdentity) throw new Exception("Changing only the post-success suffix changed the selected identity");
  var added=prefix.Points.Except(all.Points).ToArray();
  var removed=all.Points.Except(prefix.Points).ToArray();
  rows.Add(new{name,boardType,reverse,proposalCount=quads.Length,decodedCount,prefixProposalCount=firstSuccess+1,sameIdentity,allPointCount=all.Points.Count,prefixPointCount=prefix.Points.Count,added,removed});
  Console.WriteLine($"{name} HIK{boardType} reverse={reverse}: proposals {quads.Length}->{firstSuccess+1}, points {all.Points.Count}->{prefix.Points.Count}");
 }
}
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[0]))!);
File.WriteAllText(args[0],JsonSerializer.Serialize(new{managedBoundaryExperiment=true,originalRuntimeExecuted=false,sourceExactProposalOrDecoder=false,productModified=true,change="Source-grid context transport; identity, legacy geometry and mask unchanged",denseContextFixtureChecks=true,rows},new JsonSerializerOptions{WriteIndented=true,IncludeFields=true}));
