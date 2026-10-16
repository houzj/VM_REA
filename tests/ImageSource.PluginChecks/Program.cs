using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;
using VisionEditor.Core;
using Window=System.Windows.Window;
using WCombo=System.Windows.Controls.ComboBox;
using WGrid=System.Windows.Controls.Grid;

static class Program {
 static int checks;
 static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
 static void Set<T>(Dictionary<string,JsonElement> p,string key,T value)=>p[key]=JsonSerializer.SerializeToElement(value);
 static void List(Dictionary<string,JsonElement> p,string key,params string[] paths)=>Set(p,key,JsonSerializer.Serialize(paths));
 static void WriteRows(string path,byte[] rows,int width=4){using var writer=new BinaryWriter(File.Create(path));int stride=(width+3)&~3,length=rows.Length*stride;writer.Write((ushort)0x4d42);writer.Write(1078+length);writer.Write(0);writer.Write(1078);writer.Write(40);writer.Write(width);writer.Write(rows.Length);writer.Write((ushort)1);writer.Write((ushort)8);writer.Write(0);writer.Write(length);writer.Write(0);writer.Write(0);writer.Write(256);writer.Write(0);for(int i=0;i<256;i++){writer.Write((byte)i);writer.Write((byte)i);writer.Write((byte)i);writer.Write((byte)0);}for(int y=rows.Length-1;y>=0;y--)for(int x=0;x<stride;x++)writer.Write(x<width?rows[y]:(byte)0);}
 static IEnumerable<DependencyObject> Descendants(DependencyObject parent){for(int i=0;i<VisualTreeHelper.GetChildrenCount(parent);i++){var child=VisualTreeHelper.GetChild(parent,i);yield return child;foreach(var inner in Descendants(child))yield return inner;}}
 [STAThread] static int Main(){try{Run().GetAwaiter().GetResult();return 0;}catch(Exception ex){Console.Error.WriteLine(ex);return 1;}}
 static async Task Run(){
  ApplicationConfiguration.Initialize();
  string output=Path.GetFullPath("analysis/full-audit/image-source-checks");Directory.CreateDirectory(output);
  string scratch=Path.Combine(output,Guid.NewGuid().ToString("N"));Directory.CreateDirectory(scratch);
  string sample=Path.GetFullPath("analysis/images/circle-find-sample.bmp");
  string a=Path.Combine(scratch,"a.bmp"),b=Path.Combine(scratch,"b.bmp"),c=Path.Combine(scratch,"c.bmp");
  foreach(var file in new[]{a,b,c})File.Copy(sample,file);
  var plugin=new ImageSourcePlugin();var registry=new ModuleRegistry();var host=new ToolPluginRegistry();host.Register(plugin,registry);
  Check(host.Find(plugin.Manifest.Id)==plugin,"plugin registration");
  Dictionary<string,JsonElement> Defaults(){var p=plugin.Descriptor.Parameters.ToDictionary(d=>d.Name,d=>d.DefaultValue!.Value.Clone());List(p,"ImagePaths",a,b,c);Set(p,"InitialSN",7);return p;}
  ValueTask<ModuleResult> Execute(string node,Dictionary<string,JsonElement> p,CancellationToken token=default,string? root=null)=>registry.Create(plugin.Manifest.Id,plugin.Manifest.Version).ExecuteAsync(new(node,root??Environment.CurrentDirectory,new Dictionary<string,PortValue>(),p),token);
  long SN(ModuleResult r)=>r.Outputs["SNCode"].As<long>();
  string FileName(ModuleResult r)=>r.Outputs["CurImageName"].As<string>();
  var values=Defaults();var first=await Execute("sequence",values);
  Check(first.Status==ModuleStatus.Success&&FileName(first)=="a.bmp","first advance from -1");
  Check(SN(first)==8,"native pre-increment SN");
  Check(first.Outputs["Width"].As<long>()>0&&first.Outputs["Image"].As<VisionCore.GrayImage>().Pixels.Length>0,"real decoded data");
  Check(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(first.Outputs["Image"].As<VisionCore.GrayImage>().Pixels)).ToLowerInvariant()=="96187178e18c6e0104e79909116de929b6dceab22a595c6ab8abeefdd57d2098","sample pixels match original SDK capture");
  Check(FileName(await Execute("sequence",values))=="b.bmp","next runtime factory retains cursor");
  Set(values,"ShowImageName",false);Check(SN(await Execute("sequence",values))==10,"display parameter does not reset SN");
  Check(FileName(await Execute("sequence",values))=="a.bmp","wrap cursor");
  Set(values,"InitialSN",42);Check(SN(await Execute("sequence",values))==43,"only InitialSN updates sequence");
  plugin.Reset("sequence");Check(SN(await Execute("sequence",values))==43,"explicit reset");
  Check(SN(await Execute("sequence",values,root:scratch))==43,"different solution base directory isolates state");
  values=Defaults();Set(values,"CurrentImagePath",a);Check(FileName(await Execute("current",values))=="b.bmp","manual current followed by native auto-advance");
  Set(values,"AutoPlay",false);Set(values,"CurrentImagePath",c);Check(FileName(await Execute("current",values))=="c.bmp","manual current without autoplay");
  Check(FileName(await Execute("current",values))=="c.bmp","manual mode holds current");
  values=Defaults();Set(values,"ExecuteType",2);List(values,"SelectedPaths",c,a);Check(FileName(await Execute("selected",values))=="a.bmp","selected execution follows original list order");
  Check(FileName(await Execute("selected",values))=="c.bmp","selected skips unselected");
  List(values,"SelectedPaths");Set(values,"CurrentImagePath",a);Check((await Execute("selected",values)).ErrorCode=="NoImage","empty selection never falls back");
  List(values,"SelectedPaths",sample);Check((await Execute("selected",values)).ErrorCode=="NoImage","selection must belong to full list");
  values=Defaults();List(values,"ImagePaths");Set(values,"CurrentImagePath",a);Check((await Execute("empty",values)).ErrorCode=="NoImage","current selector does not submit one-shot image");
  values=Defaults();List(values,"ImagePaths",a,a,b);Check(FileName(await Execute("duplicate",values))=="a.bmp"&&FileName(await Execute("duplicate",values))=="b.bmp","AddImageCheck ignores literal duplicates");
  values=Defaults();List(values,"ImagePaths","missing.bmp",a);Check((await Execute("failure",values)).ErrorCode=="ImageReadFailed","missing file error");
  var recovered=await Execute("failure",values);Check(FileName(recovered)=="a.bmp"&&SN(recovered)==8,"failed read advances position but does not increment SN");
  values=Defaults();List(values,"ImagePaths","");Check((await Execute("blank",values)).Status==ModuleStatus.Failed,"blank path rejected");
  Set(values,"ImagePaths","[");Check((await Execute("json",values)).Status==ModuleStatus.Failed,"invalid JSON rejected");
  foreach(var pair in new[]{("InitialSN",100001),("PathCache",6),("Interval",1001),("StitchHeight",20001)}){values=Defaults();Set(values,pair.Item1,pair.Item2);Check((await Execute("invalid",values)).ErrorCode=="InvalidParameter","parameter bound "+pair.Item1);}
  values=Defaults();List(values,"ImagePaths",a);Set(values,"PathCache",1);Check((await Execute("cache",values)).Status==ModuleStatus.Success,"cache admission");File.Delete(a);
  Check((await Execute("cache",values)).Status==ModuleStatus.Success,"cached bytes survive source removal");plugin.Reset("cache");Check((await Execute("cache",values)).Status==ModuleStatus.Failed,"reset releases cache");File.Copy(sample,a);
  values=Defaults();Set(values,"Interval",100);var wall=Stopwatch.StartNew();var delayed=await Execute("interval",values);wall.Stop();Check(wall.ElapsedMilliseconds>=90,"local interval delays execution");Check(delayed.Outputs["ModuRunTime"].As<double>()<wall.Elapsed.TotalMilliseconds-60,"interval excluded from module runtime");
  values=Defaults();Set(values,"Interval",1000);using(var cancel=new CancellationTokenSource(30)){bool cancelled=false;try{await Execute("cancel",values,cancel.Token);}catch(OperationCanceledException){cancelled=true;}Check(cancelled,"interval cooperatively cancels");}
  values=Defaults();var concurrent=await Task.WhenAll(Enumerable.Range(0,12).Select(_=>Execute("concurrent",values).AsTask()));Check(concurrent.Select(SN).Order().SequenceEqual(Enumerable.Range(8,12).Select(n=>(long)n)),"same-node executions serialize state");
  values=Defaults();Set(values,"OutMono8",true);Check(!(await Execute("mono",values)).Outputs.ContainsKey("ImageMono8"),"no duplicate Mono8 output for Mono8 source");
  foreach(var mode in new[]{2,3}){values=Defaults();Set(values,"ImageSourceType",mode);Check((await Execute("provider",values)).ErrorCode=="InputProviderUnavailable","provider boundary "+mode);}
  values=Defaults();Set(values,"PixelFormat",2);Check((await Execute("rgb",values)).ErrorCode=="PixelFormatUnavailable","typed RGB boundary");
  foreach(var pair in new[]{("AutoStop","ProcedureStopUnavailable"),("IsSubscribeFolderMode","FolderWatchUnavailable"),("SolSaveImageData","ModelEmbeddingUnavailable"),("TriggerStringEnable","TriggerUnavailable")}){values=Defaults();Set(values,pair.Item1,true);Check((await Execute("boundary",values)).ErrorCode==pair.Item2,"explicit service boundary "+pair.Item1);}
  string rowA=Path.Combine(scratch,"rows-a.bmp"),rowB=Path.Combine(scratch,"rows-b.bmp");WriteRows(rowA,[10,20,30,40]);WriteRows(rowB,[50,60,70,80]);
  values=Defaults();List(values,"ImagePaths",rowA,rowB);Set(values,"StitchEnable",true);Set(values,"StitchHeight",2);Set(values,"StitchStartHeight",1);
  VisionCore.GrayImage? retainedFirstStitch=null;
  using(var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/image-source-stitch-evidence.json"))){int observed=0;foreach(var record in evidence.RootElement.GetProperty("observations").EnumerateArray()){
   string phase=record.GetProperty("phase").GetString()!;
   switch(phase){case "stitch-start-change":Set(values,"StitchStartHeight",0);break;case "stitch-height-change":Set(values,"StitchHeight",1);break;case "stitch-invalid-crop":Set(values,"StitchStartHeight",4);break;case "stitch-after-error":Set(values,"StitchStartHeight",0);break;case "stitch-disabled":Set(values,"StitchEnable",false);break;case "stitch-reenabled":Set(values,"StitchEnable",true);break;case "stitch-zero-height":Set(values,"StitchHeight",0);break;case "stitch-width-change":Set(values,"StitchHeight",1);Set(values,"AutoPlay",false);Set(values,"CurrentImagePath",rowA);WriteRows(rowA,[10,20,30,40],5);break;case "stitch-width-restored":WriteRows(rowA,[10,20,30,40]);break;}
   var result=await Execute("stitch-native",values);observed++;
   if(record.GetProperty("nativeErrorCode").GetUInt32()!=0){Check(result.ErrorCode=="StitchGeometryInvalid",phase+" original geometry failure");continue;}
   Check(result.Status==ModuleStatus.Success,phase+" success");var image=result.Outputs["Image"].As<VisionCore.GrayImage>();if(retainedFirstStitch is null)retainedFirstStitch=image;Check(image.Width==record.GetProperty("width").GetInt32()&&image.Height==record.GetProperty("height").GetInt32(),phase+" original dimensions");Check(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(image.Pixels)).ToLowerInvariant()==record.GetProperty("pixelSha256").GetString(),phase+" original pixel hash");Check(SN(result)==record.GetProperty("sn").GetInt64()-10,phase+" original SN transition");
  }Check(observed==11,"all original stitch scenarios compared");}
  plugin.Reset("stitch-native");var resetStitch=await Execute("stitch-native",values);Check(resetStitch.Outputs["Image"].As<VisionCore.GrayImage>()[0,0]==0,"explicit reset releases stitch history");
  Check(retainedFirstStitch![0,0]==0&&retainedFirstStitch[0,5]==40,"later stitching does not mutate previously published frame");
  Set(values,"ClearTrigger",true);Check((await Execute("stitch-native",values)).ErrorCode=="StitchTriggerUnavailable","clear trigger remains explicit subscription boundary");
  string codecs=Path.Combine(scratch,"codecs");ImageSourceCodecFixtures.Create(codecs);
  using(var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/image-source-codec-evidence.json"))){Check(evidence.RootElement.GetProperty("observations").GetArrayLength()==13,"complete original codec corpus retained");foreach(var record in evidence.RootElement.GetProperty("observations").EnumerateArray()){
   string phase=record.GetProperty("phase").GetString()!,file=Path.Combine(codecs,phase[6..]);
   Check(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(file))).ToLowerInvariant()==record.GetProperty("fixtureSha256").GetString(),phase+" exact original fixture bytes");
   values=Defaults();List(values,"ImagePaths",file);var result=await Execute(phase,values);
   if(phase is "codec-truncated.bmp" or "codec-not-image.bmp"){Check(result.ErrorCode=="ImageReadFailed",phase+" strict invalid-data policy");continue;}
   if(phase=="codec-color.jpg"){
    Check(result.ErrorCode=="ImageCodecUnavailable","JPEG mismatch is explicit, not silently emitted");
    using var input=File.OpenRead(file);var frame=BitmapDecoder.Create(input,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad).Frames[0];var color=new FormatConvertedBitmap(frame,PixelFormats.Bgr24,null,0);byte[] bgr=new byte[frame.PixelWidth*frame.PixelHeight*3],candidate=new byte[frame.PixelWidth*frame.PixelHeight];color.CopyPixels(bgr,frame.PixelWidth*3,0);for(int i=0;i<candidate.Length;i++)candidate[i]=(byte)((76*bgr[i*3+2]+150*bgr[i*3+1]+30*bgr[i*3])>>8);
    byte[] original=Convert.FromHexString(record.GetProperty("pixelsHex").GetString()!);int differences=Enumerable.Range(0,candidate.Length).Count(i=>candidate[i]!=original[i]),maxError=Enumerable.Range(0,candidate.Length).Max(i=>Math.Abs(candidate[i]-original[i]));
    File.WriteAllText(Path.Combine(output,"jpeg-decoder-gap.json"),JsonSerializer.Serialize(new{nativeSha256=record.GetProperty("pixelSha256").GetString(),wicSha256=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(candidate)).ToLowerInvariant(),differences,maxError,completeParity=false},new JsonSerializerOptions{WriteIndented=true}));continue;
   }
   Check(result.Status==ModuleStatus.Success,phase+" decode success");var image=result.Outputs["Image"].As<VisionCore.GrayImage>();
   Check(image.Width==record.GetProperty("width").GetInt32()&&image.Height==record.GetProperty("height").GetInt32(),phase+" original dimensions");
   string actualHash=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(image.Pixels)).ToLowerInvariant();
   Check(actualHash==record.GetProperty("pixelSha256").GetString(),phase+" original MONO8 pixel hash");
  }}
  using(var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/image-source-folder-evidence.json"))){foreach(var record in evidence.RootElement.GetProperty("observations").EnumerateArray().Where(r=>r.GetProperty("phase").GetString()=="folder-syntax")){
   string name=record.GetProperty("name").GetString()!,value=name switch{"local"=>scratch,"relative"=>"images","unc"=>@"\\server\images","forward-slash"=>"D:/images","control"=>"D:\\im\tages","unicode"=>"D:\\图像",_=>throw new Exception(name)};
   Check(LocalFolderSelection.HasLocalSyntax(value)==record.GetProperty("accepted").GetBoolean(),"original folder syntax "+name);
  }}
  string folder=Path.Combine(scratch,"folder-select"),sentinel=Path.Combine(folder,"test.txt");Directory.CreateDirectory(folder);File.WriteAllText(sentinel,"keep");
  Check(LocalFolderSelection.Prepare(folder)==folder,"folder selection applies local directory");Check(File.ReadAllText(sentinel)=="keep"&&Directory.GetFiles(folder).Length==1,"folder write probe preserves existing test.txt and leaves no temp file");
  string created=Path.Combine(folder,"新建目录");Check(LocalFolderSelection.Prepare(created)==created&&Directory.Exists(created),"manual folder selection creates missing local directory");
  foreach(string invalid in new[]{"","relative",@"\\server\images",scratch+"\\"+new string('x',201)}){bool rejected=false;try{LocalFolderSelection.Prepare(invalid);}catch(ArgumentException){rejected=true;}Check(rejected,"invalid folder selection rejected");}
  values=Defaults();Set(values,"AutoPlay",false);Set(values,"AutoStop",true);Check((await Execute("autostop-hidden",values)).Status==ModuleStatus.Success,"AutoStop ignored without autoplay");
  values=Defaults();var document=new GraphEditorDocument(registry);document.NewSolution();string id=document.AddNode(plugin.Manifest.Id,plugin.Manifest.Version,100,100,values);string path=Path.Combine(output,"restored.vmx.json");document.Save(path);document.Load(path);var session=await new SolutionExecutor(registry).RunAsync(new SolutionWorkspace(document.Document,document.BaseDirectory),document.CurrentProcedureId);Check(session.Status==SessionStatus.Success&&session.Nodes.Single().Outputs.ContainsKey("Image"),"persisted plugin executes through host");
  // WPF checks must run on their own STA after async runtime checks resume on a worker.
  Exception? uiError=null;var uiThread=new Thread(()=>{try{CheckUi(plugin,document.CurrentProcedure.Nodes.Single(),output);}catch(Exception ex){uiError=ex;}});uiThread.SetApartmentState(ApartmentState.STA);uiThread.Start();uiThread.Join();if(uiError is not null)throw uiError;
  File.WriteAllText(Path.Combine(output,"results.json"),JsonSerializer.Serialize(new{checks,passed=true,originalRuntimeEquivalence=false,nativeBasis="original SDK state, sample/lossless codec/stitch pixel hashes, folder helper and three-mode visibility comparisons; JPEG, desktop capture and service parity remain open"},new JsonSerializerOptions{WriteIndented=true}));
  Console.WriteLine($"PASS {checks} image-source checks; original runtime equivalence remains unverified");
 }
 static void CheckUi(ImageSourcePlugin plugin,NodeDocument node,string output){
  var editor=plugin.CreateWpfEditor(new(node,Environment.CurrentDirectory,_=>{},_=>Task.FromResult(new NodeRunRecord(node.Id,NodeRunStatus.Success,new Dictionary<string,PortValue>(),0))));
  editor.WindowStartupLocation=WindowStartupLocation.Manual;editor.Left=-30000;editor.Top=-30000;editor.Show();editor.UpdateLayout();
  try{
   T Control<T>(string key)where T:FrameworkElement=>Descendants(editor).OfType<T>().Single(e=>e.Name==key);
   bool Visible(string key)=>Descendants(editor).OfType<WGrid>().Single(g=>Equals(g.Tag,key)).Visibility==Visibility.Visible;
   void Capture(string name){editor.UpdateLayout();var dpi=VisualTreeHelper.GetDpi(editor);var bitmap=new RenderTargetBitmap((int)Math.Round(editor.ActualWidth*dpi.DpiScaleX),(int)Math.Round(editor.ActualHeight*dpi.DpiScaleY),dpi.PixelsPerInchX,dpi.PixelsPerInchY,PixelFormats.Pbgra32);bitmap.Render(editor);var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));using var stream=File.Create(Path.Combine(output,name+".png"));encoder.Save(stream);}
   Check(editor.Width==420&&editor.Height==500,"source logical dimensions");Check(Visible("PixelFormat")&&!Visible("CameraID"),"local UI mode");Capture("local-mono8");
   Control<ToggleButton>("AutoPlay").IsChecked=false;Check(!Visible("AutoStop"),"autoplay false hides AutoStop");
   using(var evidence=JsonDocument.Parse(File.ReadAllText("docs/plugins/image-source-evidence.json"))) {
    var models=evidence.RootElement.GetProperty("runtimeResults").EnumerateArray().Where(r=>r.GetProperty("phase").GetString()=="sdk-ui-model"&&r.GetProperty("variant").GetString()!.StartsWith("mode-"));
    int modelCount=0;
    foreach(var model in models){modelCount++;Control<WCombo>("ImageSourceType").SelectedIndex=model.GetProperty("mode").GetInt32()-1;foreach(var item in model.GetProperty("items").EnumerateArray()){var fields=item.GetProperty("fields");string key=fields.GetProperty("Name").GetString()!;if(key=="CurrentImagePath")continue;Check(Visible(key)==bool.Parse(fields.GetProperty("CustomVisible").GetString()!),"original SDK model visibility: "+model.GetProperty("variant").GetString()+"/"+key);}}
    Check(modelCount==3,"all three original SDK model captures used");
   }
   Control<WCombo>("ImageSourceType").SelectedIndex=0;Control<ToggleButton>("AutoPlay").IsChecked=true;
   var number=Control<WpfNumericControl>("InitialSN");number.Text="100001";Check(number.Text!="100001","SN UI maximum");number.Text="-1";Check(number.Text!="-1","SN UI minimum");
   var cache=Control<WpfNumericControl>("PathCache");cache.Text="6";Check(cache.Text!="6","cache UI maximum");
   Control<WCombo>("PixelFormat").SelectedIndex=1;Check(Visible("OutMono8"),"RGB UI enables extra Mono8 row");Capture("local-rgb24");
   Control<WCombo>("ImageSourceType").SelectedIndex=1;Check(Visible("CameraID")&&Visible("ExposureTimeInput")&&Visible("GainInput")&&!Visible("PixelFormat")&&!Visible("SolSaveImageData"),"camera UI rows");Check(!Control<WCombo>("CameraID").IsEnabled,"camera selector exposes unavailable service honestly");Capture("camera");
   Check(Control<System.Windows.Controls.TextBox>("ExposureTimeInput").IsReadOnly,"exposure uses subscription text field");Check(Control<System.Windows.Controls.TextBox>("SubscribeFolderPath").MaxLength==200,"folder input source length bound");
   Check(Control<System.Windows.Controls.Button>("BrowseImageFolder").IsEnabled,"local folder picker works independently of watcher service");
   editor.UpdateLayout();Check(Control<WCombo>("CameraID").ActualWidth>200&&Control<System.Windows.Controls.TextBox>("ExposureTimeInput").ActualWidth>200&&number.ActualWidth>200,"decorated editors retain usable width");
   Control<WCombo>("ImageSourceType").SelectedIndex=2;Check(!Visible("CameraID")&&!Visible("AutoPlay")&&Visible("InitialSN"),"SDK UI rows");Capture("sdk");
   Control<ToggleButton>("StitchEnable").IsChecked=true;Check(Visible("StitchHeight")&&Visible("ClearTrigger"),"persisted stitching shows dependent controls");Control<ToggleButton>("ClearTrigger").IsChecked=true;Check(Visible("ClearTriggerCondition"),"clear trigger subscription row");
   Check(Control<ToggleButton>("StitchEnable").IsEnabled&&!Control<ToggleButton>("ClearTrigger").IsEnabled,"local stitch enabled independently of trigger service");
   Control<ToggleButton>("TriggerStringEnable").IsChecked=true;Check(Visible("TriggerString")&&Visible("TriggerFilter"),"trigger subscription rows");
   Control<WCombo>("ImageSourceType").SelectedIndex=0;Control<ToggleButton>("IsSubscribeFolderMode").IsChecked=true;Check(Visible("SubscribeFolderPath")&&Visible("RefreshImage")&&Visible("TriggerProcessRun"),"folder dependent rows");
   Descendants(editor).OfType<ScrollViewer>().First().ScrollToEnd();editor.UpdateLayout();Capture("local-folder-expanded");
  }finally{editor.Close();}
 }
}
