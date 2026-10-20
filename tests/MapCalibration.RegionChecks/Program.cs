using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionCore;
using VisionFramework;
using VisionPlugins.MapCalibration;
try {
 int checks=0;void Check(bool ok,string message){if(!ok)throw new Exception(message);checks++;}
 var plugin=new MapCalibrationPlugin();var runtime=plugin.CreateRuntime();var directory=Directory.GetCurrentDirectory();
 var parameters=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());parameters["WeightFun"]=JsonSerializer.SerializeToElement(1);
 var rect=new RoiDocument("a",RoiKind.Rectangle,1000,1000,1600,1600);var other=rect with{Id="b",Width=1400};
 var resolve=typeof(MapCalibrationPlugin).Assembly.GetType("VisionPlugins.MapCalibration.MapCalibrationRegionInput")!.GetMethod("Resolve",BindingFlags.NonPublic|BindingFlags.Static)!;
 IReadOnlyList<RoiDocument> Resolve(IReadOnlyList<RoiDocument> configured,RoiRegionValue? input)=>(IReadOnlyList<RoiDocument>)resolve.Invoke(null,[configured,input])!;
 void Reject(IReadOnlyList<RoiDocument> c,RoiRegionValue r,string code){try{Resolve(c,r);throw new Exception("Expected rejection");}catch(TargetInvocationException e){Check(e.InnerException is ArgumentException&&e.InnerException.Message.StartsWith(code),code);}}
 Check(Resolve([rect],null)[0]==rect,"Absent port retains configuration");
 Check(Resolve([],new([rect]))[0]==rect,"Default full image consumes Region");
 Check(Resolve([rect],new([rect with{Id="other-id"}]))[0].Id=="other-id","Geometry equality ignores identity");
 Check(Resolve([],new([])).Count==0,"Empty Region means independent full-image contract");
 Reject([rect],new([other]),"RegionConfigurationConflict");Reject([rect],new([]),"RegionConfigurationConflict");
 Reject([],new([rect,other]),"UnsupportedRegion");Reject([],new([rect with{Exclusion=true}]),"UnsupportedRegion");Reject([],new([rect with{CoordinateSpace=RoiCoordinateSpace.Fixture}]),"UnsupportedRegion");
 GrayImage Load(string path){var decoder=BitmapDecoder.Create(new Uri(Path.GetFullPath(path)),BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);var bitmap=new FormatConvertedBitmap(decoder.Frames[0],PixelFormats.Gray8,null,0);var pixels=new byte[bitmap.PixelWidth*bitmap.PixelHeight];bitmap.CopyPixels(pixels,bitmap.PixelWidth,0);return new(bitmap.PixelWidth,bitmap.PixelHeight,pixels);}
 var a=Load("analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp");var b=Load("analysis/full-audit/n-image-calibration/solver-fixture/frame-4.bmp");
 Dictionary<string,PortValue> Pair()=>new(){{"InputImage1",PortValue.Image(a)},{"InputImage2",PortValue.Image(b)}};
 var configured=new Dictionary<string,IReadOnlyList<RoiDocument>>{{"RoiType_0",[rect]}};
 var baseline=await runtime.ExecuteAsync(new("baseline",directory,Pair(),parameters,roiGroups:configured),CancellationToken.None);Check(baseline.Status==ModuleStatus.Success,"Configured ROI baseline");
 var waiting=await runtime.ExecuteAsync(new("paired",directory,new Dictionary<string,PortValue>{{"InputImage1",PortValue.Image(a)},{"InputROI1",PortValue.Region([other])}},parameters,roiGroups:configured),CancellationToken.None);
 Check(waiting.Status==ModuleStatus.Success&&waiting.Outputs["CalibStatus"].As<long>()==0,"First frame waits before resolving conflicting Region");
 var final=new Dictionary<string,PortValue>{{"InputImage2",PortValue.Image(b)},{"InputROI1",PortValue.Region([rect])},{"InputROI2",PortValue.Region([])}};
 var result=await runtime.ExecuteAsync(new("paired",directory,final,parameters),CancellationToken.None);Check(result.Status==ModuleStatus.Success,"Final-pair Region is used; first-frame Region is not latched");
 foreach(string name in new[]{"CornersCurPointX","CornersCurPointY","CornersObjPointX","CornersObjPointY","CornersProPointX","CornersProPointY"})Check(result.Outputs[name].As<IReadOnlyList<double>>().SequenceEqual(baseline.Outputs[name].As<IReadOnlyList<double>>()),"Region source rectangle/target full independently matches configured baseline: "+name);
 foreach(string name in new[]{"DetectCenterX","DetectCenterY","DetectWidth","DetectHeight","DetectAngle"})Check(result.Outputs[name].As<IReadOnlyList<double>>().SequenceEqual(new double[]{0,0}),"Original normal-path Detect fields stay zero: "+name);
 Check(result.Outputs["ROI"].As<RoiRegionValue>().Regions.Count==0,"Detect output does not fabricate configured ROI geometry");
 var conflictInputs=Pair();conflictInputs["InputROI2"]=PortValue.Region([other]);
 var targetConfigured=new Dictionary<string,IReadOnlyList<RoiDocument>>{{"RoiType_1",[rect]}};
 var conflict=await runtime.ExecuteAsync(new("conflict",directory,conflictInputs,parameters,roiGroups:targetConfigured),CancellationToken.None);
 Check(conflict.Status==ModuleStatus.Failed&&conflict.ErrorCode=="RegionConfigurationConflict","Second-side conflict is explicit before detection");
 Console.WriteLine($"PASS {checks}: production Region adapter/pair lifecycle/side independence and original zero Detect fields");return 0;
}catch(Exception error){Console.Error.WriteLine(error);return 1;}
