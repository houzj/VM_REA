using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisionEditor.Core;
using VisionFramework;
using VisionPlugins.ImageSource;

static class Program
{
 [STAThread] static int Main(string[] args)
 {
  string output=Path.GetFullPath(args.FirstOrDefault()??Path.Combine("analysis/full-audit/image-source-host",Guid.NewGuid().ToString("N")));
  Directory.CreateDirectory(output);var checks=new List<string>();
  void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);checks.Add(message);}
  try
  {
   string folder=Directory.CreateDirectory(Path.Combine(output,"images")).FullName;
   string child=Directory.CreateDirectory(Path.Combine(folder,"child")).FullName;
   string deep=Directory.CreateDirectory(Path.Combine(child,"deep")).FullName;
   var bitmap=BitmapSource.Create(8,8,96,96,PixelFormats.Gray8,null,Enumerable.Range(0,64).Select(i=>(byte)i).ToArray(),8);
   var files=new List<string>();
   foreach(string ext in new[]{"bmp","jpg","jpeg","png","tif","tiff"})
   {
    string file=Path.Combine(folder,"image."+ext);
    BitmapEncoder encoder=ext switch{"bmp"=>new BmpBitmapEncoder(),"jpg" or "jpeg"=>new JpegBitmapEncoder(),"png"=>new PngBitmapEncoder(),_=>new TiffBitmapEncoder()};
    encoder.Frames.Add(BitmapFrame.Create(bitmap));using(var stream=File.Create(file))encoder.Save(stream);files.Add(file);
    Check(ImageSourceListOperations.SupportsFile(file),"Accept source image extension "+ext);
   }
   string sub=Path.Combine(child,"sub.PNG");File.Copy(files[3],sub);File.Copy(files[3],Path.Combine(deep,"excluded.png"));File.WriteAllText(Path.Combine(folder,"ignore.txt"),"not image");
   var enumerated=ImageSourceListOperations.EnumerateFolder(folder).ToArray();
   Check(enumerated.Length==7&&enumerated.Contains(sub),"Folder import includes all six formats and immediate subdirectories");
   Check(!enumerated.Any(p=>p.Contains(Path.DirectorySeparatorChar+"deep"+Path.DirectorySeparatorChar))&&!enumerated.Any(p=>p.EndsWith(".txt")),"Folder import excludes deeper levels and non-image extensions");
   var expected=Directory.GetFileSystemEntries(folder).OrderBy(p=>p.Length).ThenBy(p=>p).SelectMany(p=>File.Exists(p)?ImageSourceListOperations.SupportsFile(p)?new[]{p}:[]:Directory.GetFiles(p).Where(ImageSourceListOperations.SupportsFile)).ToArray();
   Check(enumerated.SequenceEqual(expected),"Folder order follows original length/name traversal rather than format grouping");
   Check(!ImageSourceListOperations.EnumerateFolder(Path.Combine(output,"absent")).Any(),"Missing directory produces no imported paths");
   Check(ImageSourceListOperations.CanDrop(files)&&!ImageSourceListOperations.CanDrop([]),"File drop accepts a nonempty all-image selection");
   Check(!ImageSourceListOperations.CanDrop([files[0],Path.Combine(folder,"ignore.txt")])&&!ImageSourceListOperations.CanDrop([folder])&&!ImageSourceListOperations.CanDrop([Path.Combine(folder,"missing.jpg")]),"File drop rejects a whole mixed, directory or missing-file selection");
   var plugin=new ImageSourcePlugin();var modules=new ModuleRegistry();modules.Register(plugin.Descriptor,plugin.CreateRuntime);
   var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
   var empty=new NodeDocument("source",plugin.Descriptor.Id,plugin.Descriptor.Version,"Image source",values);
   var node=ImageSourceListOperations.Append(empty,files);
   Check(ImageSourceListOperations.Paths(node).SequenceEqual(files)&&node.Parameters["CurrentImagePath"].GetString()=="","Import preserves declared order and does not pre-advance AutoPlay");
   Check(ImageSourceListOperations.Paths(empty).Count==0,"Host list operations do not mutate previous document snapshots");
   Check(ImageSourceListOperations.Paths(ImageSourceListOperations.Append(node,[files[0],files[0].ToUpperInvariant()])).Count==6,"Managed Windows list policy suppresses duplicate path spellings");
   async Task<ExecutionSessionResult> Run(NodeDocument n)=>await new GraphExecutor(modules).RunAsync(new("p","P",[n],[]),output);
   var first=Run(node).GetAwaiter().GetResult();
   Check(first.Status==SessionStatus.Success&&first.Nodes.Single().Outputs["CurImagePath"].As<string>()==files[0],"Actual image-source first Run uses first imported image instead of skipping it");
   foreach(var file in files)
   {
    var single=ImageSourceListOperations.Append(empty,[file]);
    var result=Run(single).GetAwaiter().GetResult();
    Check(result.Status==SessionStatus.Success&&result.Nodes.Single().Outputs["Width"].As<long>()==8&&result.Nodes.Single().Outputs["Height"].As<long>()==8,"Actual decoder and graph execute imported "+Path.GetExtension(file));
   }
   foreach(bool auto in new[]{false,true})foreach(int mode in new[]{1,2})
   {
    var configured=node with{Parameters=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone())};
    configured.Parameters["AutoPlay"]=JsonSerializer.SerializeToElement(auto);configured.Parameters["ExecuteType"]=JsonSerializer.SerializeToElement(mode);
    configured.Parameters["SelectedPaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{files[2],files[4]}));
    var reset=ImageSourceListOperations.ReturnToFirst(configured);string expectedPath=mode==1?auto?files[^1]:files[0]:auto?files[4]:files[2];
    Check(reset.Parameters["CurrentImagePath"].GetString()==expectedPath,"Return-to-first seeds correct prior path for AutoPlay="+auto+" mode="+mode);
    var result=Run(reset).GetAwaiter().GetResult();string displayed=mode==1?files[0]:files[2];
    Check(result.Nodes.Single().Outputs["CurImagePath"].As<string>()==displayed,"Next actual Run displays first eligible image for AutoPlay="+auto+" mode="+mode);
   }
   var selected=node with{Parameters=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone())};
   selected.Parameters["CurrentImagePath"]=JsonSerializer.SerializeToElement(files[1]);selected.Parameters["SelectedPaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{files[1],files[2]}));
   var removed=ImageSourceListOperations.Remove(selected,files[1]);
   Check(!ImageSourceListOperations.Paths(removed).Contains(files[1])&&ImageSourceListOperations.Paths(removed,"SelectedPaths").SequenceEqual(new[]{files[2]})&&removed.Parameters["CurrentImagePath"].GetString()=="","Individual delete updates paths, marked subset and current path together");
   Check(File.Exists(files[1]),"Individual delete does not delete the original image file");
   var cleared=ImageSourceListOperations.Clear(removed);
   Check(ImageSourceListOperations.Paths(cleared).Count==0&&ImageSourceListOperations.Paths(cleared,"SelectedPaths").Count==0,"Clear removes list and marks without filesystem deletion");
   var failed=Run(cleared).GetAwaiter().GetResult();
   Check(failed.Status==SessionStatus.Failed&&failed.Nodes.Single().Outputs.Count==0,"Run after clear fails without publishing a stale image");
   string saved=Path.Combine(output,"list.vmx.json");SolutionStore.Save(saved,new(SolutionStore.CurrentVersion,"s","S",[new("p","P",[selected],[])],[]));
   using(var doc=new GraphEditorDocument(modules))
   {
    doc.Load(saved);doc.SetParameters(selected.Id,removed.Parameters);Check(ImageSourceListOperations.Paths(doc.CurrentProcedure.Nodes.Single()).Count==5,"Real document commits list deletion");
    doc.Undo();Check(ImageSourceListOperations.Paths(doc.CurrentProcedure.Nodes.Single()).Count==6,"One Undo restores complete pre-delete list");
    doc.Redo();Check(ImageSourceListOperations.Paths(doc.CurrentProcedure.Nodes.Single()).Count==5,"Redo reapplies list deletion");doc.Save(saved);
   }
   Check(ImageSourceListOperations.Paths(SolutionStore.Load(saved).Procedures.Single().Nodes.Single()).Count==5,"Saved solution reopens with the edited list");
   File.WriteAllText(Path.Combine(output,"summary.json"),JsonSerializer.Serialize(new{passed=checks.Count,checks,windowsCreated=0,vendorSdkExecuted=false,scope="Source-guided main image-list operations, real decoders and graph, document undo/redo and persistence"},new JsonSerializerOptions{WriteIndented=true}));
   Console.WriteLine($"PASS {checks.Count} image-source host operation checks");return 0;
  }
  catch(Exception error){File.WriteAllText(Path.Combine(output,"failed.txt"),error.ToString());Console.Error.WriteLine(error);return 1;}
 }
}
