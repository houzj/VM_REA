using System.IO;
using System.Text.Json;
using VisionCore;
using VisionPlugins.Calibration;
try {
var report=new List<object>();var chainReports=new List<object>();long candidateFields=0;long fields=0,exact=0;
for(int frame=0;frame<12;frame++){
 int sequence=frame*5+1,width=new[]{1000,500,250}[frame%3];
 var raw=File.ReadAllBytes($"analysis/map-calibration/image-preprocess-trace/raw-{frame+1:D3}.gray");
 var expected=File.ReadAllBytes($"analysis/map-calibration/image-candidate-trace/image-{sequence:D3}.gray");
 var source=new GrayImage(width,width,raw);var actual=NImageMedian.Apply(source);int matching=0;
 for(int i=0;i<expected.Length;i++)if(actual.Pixels[i]==expected[i])matching++;
 fields+=expected.Length;exact+=matching;report.Add(new{frame,width,matching,fields=expected.Length});
 Console.WriteLine($"Median {frame}: {matching}/{expected.Length} bytes");var described=NImageSourceCandidateDetector.FindCandidates(source);for(int pass=0;pass<2;pass++){if(pass==1)described=described.Select(p=>NImageSourceCandidateDescriptor.Describe(source,p,described.Count,1.75f,65,110)).Where(p=>p.HasValue).Select(p=>p!.Value).ToArray();var expectedCandidates=File.ReadAllBytes($"analysis/map-calibration/image-candidate-trace/candidate-{sequence+3+pass:D3}-return.bin");int candidateBits=0;for(int i=0;i<Math.Min(described.Count,expectedCandidates.Length/28);i++){var p=described[i];int[] words=[BitConverter.SingleToInt32Bits(p.X),BitConverter.SingleToInt32Bits(p.Y),p.Radius,BitConverter.SingleToInt32Bits(p.DirectionAX),BitConverter.SingleToInt32Bits(p.DirectionAY),BitConverter.SingleToInt32Bits(p.DirectionBX),BitConverter.SingleToInt32Bits(p.DirectionBY)];for(int j=0;j<7;j++)if(words[j]==BitConverter.ToInt32(expectedCandidates,i*28+j*4))candidateBits++;}candidateFields+=candidateBits;chainReports.Add(new{frame,pass,count=described.Count,expected=expectedCandidates.Length/28,fields=expectedCandidates.Length/4,exact=candidateBits});Console.WriteLine($"Full candidate {frame} pass{pass}: {described.Count}/{expectedCandidates.Length/28}, {candidateBits}/{expectedCandidates.Length/4} fields");if(described.Count!=expectedCandidates.Length/28||candidateBits!=expectedCandidates.Length/4)throw new Exception("Original full candidate chain differs");}
}
Directory.CreateDirectory("analysis/map-calibration/candidate-chain");File.WriteAllText("analysis/map-calibration/candidate-chain/median.json",JsonSerializer.Serialize(new{fields,exact,candidateFields,productEntranceChanged=false,report,chainReports},new JsonSerializerOptions{WriteIndented=true}));
if(fields!=5250000||exact!=fields||candidateFields!=31458||chainReports.Count!=24)throw new Exception("Native median preprocessing differs");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}


