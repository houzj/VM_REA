using System.IO;
using System.Text.Json;

try
{
    const string folder="analysis/map-calibration/image-candidate-descriptor-trace";
    var reports=new List<object>();bool failed=false;int totalFields=0,totalExact=0;
    foreach(string line in File.ReadLines(folder+"/candidate-trace.jsonl"))
    {
        using var document=JsonDocument.Parse(line);var row=document.RootElement;
        if(row.GetProperty("phase").GetString()!="descriptorConfig")continue;
        int sequence=row.GetProperty("sequence").GetInt32();
        var words=row.GetProperty("words").EnumerateArray().Select(p=>p.GetUInt32()).ToArray();
        float factor=BitConverter.Int32BitsToSingle((int)words[1]),low=BitConverter.Int32BitsToSingle((int)words[2]),high=BitConverter.Int32BitsToSingle((int)words[3]);
        var image=File.ReadAllBytes($"{folder}/image-{sequence:D3}.gray");
        int width=(int)Math.Sqrt(image.Length);if(width*width!=image.Length)throw new Exception("Expected square source patch");
        var input=File.ReadAllBytes($"{folder}/candidate-{sequence:D3}-entry.bin");
        var native=File.ReadAllBytes($"{folder}/candidate-{sequence:D3}-return.bin");
        var sourceImage=new VisionCore.GrayImage(width,width,image);int count=input.Length/28;var output=new List<float[]>();
        for(int offset=0;offset<input.Length;offset+=28)
        {
            var p=VisionPlugins.Calibration.NImageSourceCandidateDescriptor.Describe(sourceImage,new(BitConverter.ToSingle(input,offset),BitConverter.ToSingle(input,offset+4),BitConverter.ToInt32(input,offset+8)),count,factor,low,high);
            if(p is { } point)output.Add([point.X,point.Y,BitConverter.Int32BitsToSingle(point.Radius),point.DirectionAX,point.DirectionAY,point.DirectionBX,point.DirectionBY]);
        }
        int exact=0;double maximum=0;
        for(int i=0;i<Math.Min(output.Count,native.Length/28);i++)for(int field=0;field<7;field++)
        {
            int expected=BitConverter.ToInt32(native,i*28+field*4),actual=BitConverter.SingleToInt32Bits(output[i][field]);
            if(expected==actual)exact++;
            if(field!=2)maximum=Math.Max(maximum,Math.Abs((double)BitConverter.Int32BitsToSingle(expected)-output[i][field]));
        }
        if(output.Count!=native.Length/28||exact!=native.Length/4)failed=true;totalFields+=native.Length/4;totalExact+=exact;reports.Add(new{sequence,count,accepted=output.Count,expected=native.Length/28,exact,fields=native.Length/4,maximum});
        Console.WriteLine($"Descriptor {sequence}: {output.Count}/{native.Length/28}, {exact}/{native.Length/4} fields, max {maximum:R}");
        if(args.Contains("--first"))break;
    }
    Directory.CreateDirectory("analysis/map-calibration/descriptor-kernel");
    File.WriteAllText("analysis/map-calibration/descriptor-kernel/diagnostic.json",JsonSerializer.Serialize(new{productionIntegrated=true,productEntranceChanged=false,totalFields,totalExact,failed,reports},new JsonSerializerOptions{WriteIndented=true}));
    if(failed||(!args.Contains("--first")&&(reports.Count!=24||totalFields!=31458)))throw new Exception("Original descriptor candidate fields differ");return 0;
}
catch(Exception error){Console.Error.WriteLine(error);return 1;}


