using System.IO;
using System.Reflection;
using System.Text.Json;
using VisionCore;
using VisionPlugins.Calibration;
try {
var rows=new List<object>();
foreach(string name in new[]{"source","target"}) {
var image=new GrayImage(1000,1000,File.ReadAllBytes("analysis/map-calibration/image-input/native-"+name+".gray"));
var lattice=NImageBoardDetector.FindLattice(image)!;
foreach(int boardType in new[]{7,9}) {
var tags=new List<NImageDetectedTag>();
foreach(var quad in lattice.CodeQuadrilaterals){var rings=NImageFiducials.Find(image,quad);if(rings==null||!NImageCodeDecoder.TryDecode(image,rings,out var code)||code.Orientation!=0)continue;var identity=NImageBoardIdentity.FromDecodedBytes(code.Bytes.ToArray(),boardType);if(identity.GridPitch>0&&float.IsFinite(identity.GridPitch))tags.Add(new(identity,quad,rings));}
var grid=NPointSolver.Solve(lattice.Points.Select(p=>new NPointPair(p.X,p.Y,p.Column,p.Row)).ToArray(),[],new(1,1));
(int X,int Y) Nearest(Point2 point){var p=NPointSolver.Transform(grid.Matrix,point.X,point.Y);return((int)Math.Round(p.X),(int)Math.Round(p.Y));}
var first=tags[0];var anchor=Nearest(first.Opening[0]);var checks=tags.Select(tag=>{var p=Nearest(tag.Opening[0]);var expected=first.Identity.WorldIndex(p.X,p.Y,anchor.X,anchor.Y,1);return new{tag.Identity,expectedX=expected.X,expectedY=expected.Y,actualX=tag.Identity.OriginX,actualY=tag.Identity.OriginY,gridPitchMatches=tag.Identity.GridPitch==first.Identity.GridPitch,originMatches=expected==(tag.Identity.OriginX,tag.Identity.OriginY)};}).ToArray();
string? rejection=checks.Any(p=>!p.gridPitchMatches)?"NImageBoardRecognition.Recognize: GridPitch mismatch":checks.Any(p=>!p.originMatches)?"NImageBoardRecognition.Recognize: WorldIndex/Origin mismatch":null;
var method=typeof(NImageBoardRecognition).GetMethod("Recognize",BindingFlags.NonPublic|BindingFlags.Static)!;var normal=(NImageDetectedBoard?)method.Invoke(null,[image,boardType,lattice,CancellationToken.None]);var publicResult=NImageBoardRecognition.RecognizeLattice(image,boardType,lattice);if(normal is null||publicResult is null||normal.GridPitch!=publicResult.GridPitch||!normal.Points.SequenceEqual(publicResult.Points)||normal.Tags[0].Identity!=publicResult.Tags[0].Identity)throw new Exception("Public recognition phase changed lattice result");var reverseLattice=lattice with {CodeQuadrilaterals=lattice.CodeQuadrilaterals.Reverse().ToArray()};var reversed=(NImageDetectedBoard?)method.Invoke(null,[image,boardType,reverseLattice,CancellationToken.None]);if(normal is null||reversed is null||normal.Tags.Count!=1||reversed.Tags.Count!=1)throw new Exception("Native first-success tag selection boundary failed");if(normal.Tags[0].Identity!=first.Identity)throw new Exception("First decoded tag identity changed");rows.Add(new{name,boardType,afterFixNormal=normal.Tags[0].Identity,afterFixReversed=reversed.Tags[0].Identity,afterFixCount=normal.Points.Count,latticeCount=lattice.Points.Count,decodedTags=tags.Count,gridFitError=grid.ErrorCode,rejection,checks});Console.WriteLine($"{name} HIK{boardType}: tags={tags.Count}, rejection={rejection??"none"}");
}
}
Directory.CreateDirectory("analysis/map-calibration/detector-boundary");File.WriteAllText("analysis/map-calibration/detector-boundary/rejection.json",JsonSerializer.Serialize(new{independentManagedBoundary=true,originalDetectorAlgorithmVerified=false,rows},new JsonSerializerOptions{WriteIndented=true,IncludeFields=true}));return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}



