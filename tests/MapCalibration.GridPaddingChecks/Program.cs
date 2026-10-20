using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
try {
var bytes=File.ReadAllBytes("analysis/map-calibration/image-grid-trace/candidates-002.bin");var candidates=new List<NImageSourceCandidate>();for(int i=0;i<bytes.Length;i+=28)candidates.Add(new(BitConverter.ToSingle(bytes,i),BitConverter.ToSingle(bytes,i+4),BitConverter.ToInt32(bytes,i+8),BitConverter.ToSingle(bytes,i+12),BitConverter.ToSingle(bytes,i+16),BitConverter.ToSingle(bytes,i+20),BitConverter.ToSingle(bytes,i+24)));
const string directory="analysis/map-calibration/grid-padding";var report=new List<object>();long fields=0;
foreach(string line in File.ReadLines(directory+"/image-results.jsonl")){
 using var document=JsonDocument.Parse(line);var row=document.RootElement;int kind=row.GetProperty("kind").GetInt32();int sequence=row.GetProperty("sequence").GetInt32(),reference=row.GetProperty("reference").GetInt32(),mode=row.GetProperty("mode").GetInt32();var used=new int[90000];if(mode!=0)Array.Fill(used,1);
 var state=new NImageSourceGridState{Count=mode==2?17:0,Width=mode==2?19:0,Height=mode==2?23:0};bool result=kind==0?NImageSourceGridSeeds.Create2x2(candidates,reference,used,state):NImageSourceGridSeeds.Create3x3(candidates,reference,used,state);
 int direction=row.GetProperty("direction").GetInt32();bool padded=NImageSourceGridPadding.AddBorder(state,direction),repeated=NImageSourceGridPadding.AddBorder(state,direction);if((padded?1:0)!=row.GetProperty("padStatus").GetInt32()||(repeated?1:0)!=row.GetProperty("repeatStatus").GetInt32())throw new Exception($"Native padding status mismatch {sequence}");if((result?1:0)!=row.GetProperty("status").GetInt32()||state.Count!=row.GetProperty("count").GetInt32()||state.Width!=row.GetProperty("dimensions")[0].GetInt32()||state.Height!=row.GetProperty("dimensions")[1].GetInt32())throw new Exception($"Native seed status mismatch {sequence}");
 var expectedIndices=File.ReadAllBytes($"{directory}/seed-{sequence:D3}-indices.bin");var expectedUsed=File.ReadAllBytes($"{directory}/seed-{sequence:D3}-used.bin");var expectedScores=File.ReadAllBytes($"{directory}/seed-{sequence:D3}-scores.float");
 int exact=0;for(int i=0;i<90000;i++){if(state.Indices[i]==BitConverter.ToInt32(expectedIndices,i*4))exact++;if(used[i]==BitConverter.ToInt32(expectedUsed,i*4))exact++;}for(int y=0;y<3;y++)for(int x=0;x<9;x++)if(BitConverter.SingleToInt32Bits(state.Scores[y*900+x])==BitConverter.ToInt32(expectedScores,(y*9+x)*4))exact++;
 if(exact!=180027)throw new Exception($"Native seed workspace mismatch {sequence}: {exact}/180027");var paddedScores=File.ReadAllBytes($"{directory}/seed-{sequence:D3}-padded.float");int paddedExact=0;for(int i=0;i<state.Scores.Length;i++)if(BitConverter.SingleToInt32Bits(state.Scores[i])==BitConverter.ToInt32(paddedScores,i*4))paddedExact++;if(paddedExact!=state.Scores.Length)throw new Exception($"Native padding score mismatch {sequence}");fields+=paddedExact+2;fields+=exact+4;report.Add(new{sequence,result,state.Count,state.Width,state.Height,exact});
}
if(report.Count!=32)throw new Exception("Expected32 original directional padding cases");File.WriteAllText(directory+"/comparison.json",JsonSerializer.Serialize(new{cases=report.Count,fields,exact=fields,report},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"Native grid padding: {report.Count} cases/{fields} fields exact");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}


