using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using VisionCore;
using VisionIO;
if(args.Length != 6) { Console.Error.WriteLine("Usage: VisionRunner <image.bmp> <output-prefix> <center-x> <center-y> <radius> <search-half-width>"); return 2; }
try {
    var imagePath=Path.GetFullPath(args[0]); var prefix=Path.GetFullPath(args[1]);
    if(new[]{prefix+".json",prefix+".png"}.Any(p => string.Equals(p,imagePath,StringComparison.OrdinalIgnoreCase))) throw new ArgumentException("Output must differ from input.");
    double Number(int i) => double.Parse(args[i],CultureInfo.InvariantCulture);
    var options=new RadialCircleOptions(Number(2),Number(3),Number(4),SearchHalfWidth:Number(5),SampleCount:64,GradientThreshold:15,Polarity:EdgePolarity.Both);
    using var source=File.OpenRead(imagePath);
    var sha=Convert.ToHexString(SHA256.HashData(source)).ToLowerInvariant(); source.Position=0;
    var image=BmpReader.Read(source); var started=System.Diagnostics.Stopwatch.StartNew();
    var result=RadialCircleDetector.Detect(image,options); started.Stop();
    var report=new { schema="vision-independent.circle.v1", imagePath, imageSha256=sha, image.Width, image.Height, options, result.Status, result.Circle, result.EdgePoints, result.OutOfBoundsSamples, elapsedMilliseconds=started.Elapsed.TotalMilliseconds,
        compatibility="unverified", parameterSource="explicit caller seed; not automatically recovered original ROI", limitations=new[]{"Independent strongest-edge radial detector; vendor numeric equivalence unverified.","No vendor SDK or assemblies loaded.","No full .sol workflow execution, runtime scheduler, or camera integration.","No outlier rejection or angular coverage constraint."} };
    Directory.CreateDirectory(Path.GetDirectoryName(prefix)!);
    File.WriteAllText(prefix+".json",JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true,Converters={new System.Text.Json.Serialization.JsonStringEnumConverter()}}));
    OverlayPng.Write(prefix+".png",image,result);
    Console.WriteLine(JsonSerializer.Serialize(new { result.Status, result.Circle, edgeCount=result.EdgePoints.Count, json=prefix+".json", overlay=prefix+".png" },new JsonSerializerOptions{Converters={new System.Text.Json.Serialization.JsonStringEnumConverter()}}));
    return result.Status==CircleDetectionStatus.Success ? 0 : 3;
} catch(Exception ex) when(ex is IOException or ArgumentException or InvalidDataException or OverflowException) {Console.Error.WriteLine(ex.Message);return 1;}
