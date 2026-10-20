using System.IO;
using System.Text.Json;
using VisionPlugins.Calibration;
try {
const string folder="analysis/map-calibration/image-grid-prediction-trace";var report=new List<object>();long fields=0;bool failed=false;
foreach(string line in File.ReadLines(folder+"/prediction-trace.jsonl")){
 using var document=JsonDocument.Parse(line);var row=document.RootElement;int sequence=row.GetProperty("sequence").GetInt32(),length=row.GetProperty("lengths")[2].GetInt32();string prefix=$"{folder}/prediction-{sequence:D3}-";
 byte[] bytes=File.ReadAllBytes(prefix+"candidates.bin");var candidates=new List<NImageSourceCandidate>();for(int i=0;i<bytes.Length;i+=28)candidates.Add(new(BitConverter.ToSingle(bytes,i),BitConverter.ToSingle(bytes,i+4),BitConverter.ToInt32(bytes,i+8),BitConverter.ToSingle(bytes,i+12),BitConverter.ToSingle(bytes,i+16),BitConverter.ToSingle(bytes,i+20),BitConverter.ToSingle(bytes,i+24)));
 int[] Ints(string suffix){var b=File.ReadAllBytes(prefix+suffix+".bin");return Enumerable.Range(0,b.Length/4).Select(i=>BitConverter.ToInt32(b,i*4)).ToArray();}
 var first=Ints("first");var second=Ints("second");var third=Ints("third");var triples=Enumerable.Range(0,length).Select(i=>new NImageSourcePredictionTriple(first[i],second[i],third[i])).ToArray();var used=Ints("used-entry");var assigned=Ints("assigned-entry");var predicted=new float[length*2];
 int changed=NImageSourceGridAssignment.Assign(candidates,triples,used,assigned,predicted);var expectedUsed=Ints("used-return");var expectedAssigned=Ints("assigned-return");var expectedPoints=File.ReadAllBytes(prefix+"points-return.bin");int xy=0;for(int i=0;i<predicted.Length;i++)if(BitConverter.SingleToInt32Bits(predicted[i])==BitConverter.ToInt32(expectedPoints,i*4))xy++;
 int flags=used.Zip(expectedUsed).Count(p=>p.First==p.Second),indices=assigned.Zip(expectedAssigned).Count(p=>p.First==p.Second);bool success=changed==row.GetProperty("assignedCount").GetInt32()&&xy==predicted.Length&&flags==used.Length&&indices==assigned.Length;failed|=!success;fields+=xy+flags+indices+(changed==row.GetProperty("assignedCount").GetInt32()?1:0);report.Add(new{sequence,length,changed,expectedChanged=row.GetProperty("assignedCount").GetInt32(),xy,xyFields=predicted.Length,flags,indices,success});if(!success)Console.WriteLine($"Assignment {sequence}: {changed}/{row.GetProperty("assignedCount")}, XY{xy}/{predicted.Length}, flags{flags}/{used.Length}, indices{indices}/{assigned.Length}");
}
Directory.CreateDirectory("analysis/map-calibration/grid-assignment");File.WriteAllText("analysis/map-calibration/grid-assignment/comparison.json",JsonSerializer.Serialize(new{cases=report.Count,fields,failed,report},new JsonSerializerOptions{WriteIndented=true}));if(failed||report.Count!=64)throw new Exception("Original grid assignment differs");Console.WriteLine($"Native assignment: {report.Count} cases/{fields} fields exact");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
