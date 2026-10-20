using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
try {
var bytes=File.ReadAllBytes(args.Length>1?args[1]:"analysis/map-calibration/image-grid-trace/candidates-002.bin");var candidates=new List<NImageSourceCandidate>();
for(int i=0;i<bytes.Length;i+=28)candidates.Add(new(BitConverter.ToSingle(bytes,i),BitConverter.ToSingle(bytes,i+4),BitConverter.ToInt32(bytes,i+8),BitConverter.ToSingle(bytes,i+12),BitConverter.ToSingle(bytes,i+16),BitConverter.ToSingle(bytes,i+20),BitConverter.ToSingle(bytes,i+24)));
var report=new List<object>();long fields=0;string directory="analysis/map-calibration/"+(args.FirstOrDefault()??"grid-neighbor");
foreach(string line in File.ReadLines(directory+"/image-results.jsonl")){
 using var document=JsonDocument.Parse(line);var row=document.RootElement;int sequence=row.GetProperty("sequence").GetInt32(),reference=row.GetProperty("reference").GetInt32(),mode=row.GetProperty("mode").GetInt32();
 var used=new int[candidates.Count];used[reference]=1;if(mode==1){used[(reference+1)%used.Length]=1;used[^1]=1;}if(mode==2)Array.Fill(used,1);
 var direction=row.GetProperty("directionBits").EnumerateArray().Select(p=>BitConverter.Int32BitsToSingle((int)p.GetUInt32())).ToArray();var costs=new float[candidates.Count];
 int selected=NImageSourceGridNeighbor.Select(candidates,reference,used,direction[0],direction[1],costs,out float score);var expected=File.ReadAllBytes($"{directory}/neighbor-{sequence:D3}-costs.float");
 int exact=0;for(int i=0;i<costs.Length;i++)if(BitConverter.SingleToInt32Bits(costs[i])==BitConverter.ToInt32(expected,i*4))exact++;
 if(selected!=row.GetProperty("selected").GetInt32()||(uint)BitConverter.SingleToInt32Bits(score)!=row.GetProperty("scoreBits").GetUInt32()||exact!=costs.Length)throw new Exception($"Native neighbor mismatch {sequence}: index {selected}, score {score:R}, costs {exact}/{costs.Length}");
 fields+=exact+2;report.Add(new{sequence,selected,exact,fields=costs.Length,scoreBits=(uint)BitConverter.SingleToInt32Bits(score)});
}
if(report.Count!=48||fields!=48L*(candidates.Count+2))throw new Exception("Expected 48 original neighbor cases");File.WriteAllText(directory+"/comparison.json",JsonSerializer.Serialize(new{fields,exact=fields,cases=report.Count,report},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"Native neighbors: {report.Count} cases/{fields} fields exact");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}

