using System.Text.Json;
using VisionPlugins.Calibration;
using VisionPlugins.MapCalibration;
try {var report=new List<object>();int checkedFields=0,equalBits=0;double max=0;int intermediateFields=0,intermediateEqualBits=0;var intermediates=new List<object>();
foreach(int weight in new[]{3,1,2}){string traceDirectory=weight==3?"analysis/map-calibration/image-fit-trace":weight==1?"analysis/map-calibration/image-fit-weight1-trace":"analysis/map-calibration/image-fit-weight2-trace"; foreach(string line in File.ReadLines(traceDirectory+"/image-results.jsonl")){
 using var document=JsonDocument.Parse(line);var row=document.RootElement;if(!row.TryGetProperty("source",out var source))continue;int board=row.GetProperty("board").GetInt32(),model=row.GetProperty("model").GetInt32();
 // Physical pitch decoded from the installed HIK fixture identity, deliberately tool-specific.
 float pitch=board==7?10f:1000f;
 (float,float,float,float)[] Points(JsonElement array)=>array.EnumerateArray().Select(p=>((float)p[0].GetDouble(),(float)p[1].GetDouble(),(float)p[2].GetDouble()*pitch,(float)p[3].GetDouble()*pitch)).ToArray();
 var a=NPointSolver.FitNativePoints(Points(source),model,weight,2f);var b=NPointSolver.FitNativePoints(Points(row.GetProperty("target")),model,weight,2f);
 if(a.ErrorCode!=0||b.ErrorCode!=0){report.Add(new{board,model,weight,sourceError=a.ErrorCode,targetError=b.ErrorCode});continue;}
 var fit=MapCalibrationSolver.ComposeImageFits(a.Matrix!,b.Matrix!);var expected=row.GetProperty("matrix").EnumerateArray().Select(p=>p.GetSingle()).ToArray();double delta=0;int bits=0;
 for(int i=0;i<9;i++){checkedFields++;if(BitConverter.SingleToInt32Bits(fit.Matrix[i])==BitConverter.SingleToInt32Bits(expected[i])){equalBits++;bits++;}delta=Math.Max(delta,Math.Abs((double)fit.Matrix[i]-expected[i]));}max=Math.Max(max,delta);if(weight==1&&bits!=9)throw new Exception("Map original constant-weight entry mismatch");
 report.Add(new{board,model,weight,delta,bits,actual=fit.Matrix,expected,sourceScale=MapCalibrationSolver.ImagePixelScale(a.Matrix!),targetScale=MapCalibrationSolver.ImagePixelScale(b.Matrix!)});
 string trace=traceDirectory+"/fixed-trace.jsonl";
 if(File.Exists(trace)){
  var records=File.ReadLines(trace).Select(l=>JsonDocument.Parse(l).RootElement.Clone()).ToArray();var selected=records.Where(x=>x.GetProperty("phase").GetString()=="fixed-fit-input"&&x.GetProperty("board").GetInt32()==board&&x.GetProperty("model").GetInt32()==model&&x.GetProperty("options")[3].GetInt32()==50).ToArray();
  if(selected.Length==2){float[] Output(JsonElement input)=>records.Single(x=>x.GetProperty("phase").GetString()=="fixed-fit-output"&&x.GetProperty("sequence").GetInt32()==input.GetProperty("sequence").GetInt32()).GetProperty("matrix").EnumerateArray().Select(v=>v.GetSingle()).ToArray();var ah=Output(selected[0]);var bh=Output(selected[1]);var exact=MapCalibrationSolver.ComposeImageFits(ah,bh);int exactBits=0;for(int i=0;i<9;i++){intermediateFields++;if(BitConverter.SingleToInt32Bits(exact.Matrix[i])==BitConverter.SingleToInt32Bits(expected[i])){intermediateEqualBits++;exactBits++;}}MapCalibrationImagePoint[] ReadPoints(JsonElement array)=>array.EnumerateArray().Select(v=>new MapCalibrationImagePoint(v[0].GetSingle(),v[1].GetSingle(),v[2].GetInt32(),v[3].GetInt32())).ToArray();var evaluation=MapCalibrationImageEvaluator.Evaluate(exact.Matrix,ReadPoints(source),ReadPoints(row.GetProperty("target")));var originalProjection=ReadPoints(row.GetProperty("projection"));int projectionBits=evaluation.Projection.Zip(originalProjection).Sum(pair=>(pair.First.X==pair.Second.X?1:0)+(pair.First.Y==pair.Second.Y?1:0));intermediates.Add(new{board,model,weight,exactBits,mapError=evaluation.MapError,originalMapError=row.GetProperty("error").GetSingle(),projectionBits,projectionFields=originalProjection.Length*2,actual=exact.Matrix,expected,sourceFitEqualBits=a.Matrix!.Zip(ah).Count(x=>BitConverter.SingleToInt32Bits(x.First)==BitConverter.SingleToInt32Bits(x.Second)),targetFitEqualBits=b.Matrix!.Zip(bh).Count(x=>BitConverter.SingleToInt32Bits(x.First)==BitConverter.SingleToInt32Bits(x.Second)),sourceFit=ah,targetFit=bh,sourceScale=MapCalibrationSolver.ImagePixelScale(ah),targetScale=MapCalibrationSolver.ImagePixelScale(bh)});}
 }
}
}
Directory.CreateDirectory("analysis/map-calibration/image-fit");File.WriteAllText("analysis/map-calibration/image-fit/diagnostics.json",JsonSerializer.Serialize(new{checkedFields,equalBits,max,intermediateFields,intermediateEqualBits,complete=checkedFields==162,sharedFitStillDifferential=equalBits!=checkedFields,report,intermediates},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"Diagnostic: {equalBits}/{checkedFields} float bits; max={max:R}; native intermediate replay {intermediateEqualBits}/{intermediateFields}; no parity claim");




if(checkedFields!=162||equalBits!=checkedFields||intermediateEqualBits!=intermediateFields)throw new Exception("Original fixed Map fit matrix mismatch");return 0;}catch(Exception error){Console.Error.WriteLine(error);return 1;}

