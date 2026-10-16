using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.ImageSource;

public sealed partial class ImageSourcePlugin:IToolAssetPersistence {
 static Dictionary<string,string> EmbeddedMap(NodeDocument node)=>node.Parameters.TryGetValue("EmbeddedImageResources",out var value)?JsonSerializer.Deserialize<Dictionary<string,string>>(value.GetString()??"{}")??new():new();
 public Stream OpenEncodedImage(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null){
  if(node.Parameters.TryGetValue("SolSaveImageData",out var save)&&save.GetBoolean()&&resources is not null&&EmbeddedMap(node).TryGetValue(path,out var id))return resources.OpenVerified(id);
  return File.OpenRead(Path.GetFullPath(path,baseDirectory));
 }
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources)=>PrepareSave(node,baseDirectory,resources,null);
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources,ModuleExecutionScope? scope){
  var values=Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());foreach(var pair in node.Parameters)values[pair.Key]=pair.Value.Clone();var paths=ReadPaths(values["ImagePaths"].GetString()??"[]");
  if(values["ImageSourceType"].GetInt32()==1&&values["IsSubscribeFolderMode"].GetBoolean()){
   var state=scope?.State(Manifest.Id,node.Id,()=>new State());string configured=values["SubscribeFolderPath"].GetString()??"";string? folder=string.IsNullOrWhiteSpace(configured)?null:Path.GetFullPath(configured,baseDirectory);
   paths=folder is null?[]:state?.Folder?.DirectoryPath==folder?state.FolderPaths:Directory.Exists(folder)?LocalFolderCatalog.Read(folder):[];
  }
  var embedded=new Dictionary<string,string>(StringComparer.Ordinal);var assets=new Dictionary<string,ResourceDocument>(StringComparer.Ordinal);
  if(values["SolSaveImageData"].GetBoolean())foreach(string path in paths.Take(120)){
   string full=Path.GetFullPath(path,baseDirectory);using var stream=File.Exists(full)?File.OpenRead(full):OpenEncodedImage(node,path,baseDirectory,resources);if(stream.Length>256L*1024*1024)throw new InvalidDataException("Embedded image exceeds encoded resource limit.");using var copy=new MemoryStream();stream.CopyTo(copy);byte[] bytes=copy.ToArray();string hash=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),id="plugin:"+Manifest.Id+":"+node.Id+":"+hash;
   assets[id]=new(id,full,hash){EmbeddedBase64=Convert.ToBase64String(bytes)};embedded[full]=id;
  }
  values["ImagePaths"]=J(JsonSerializer.Serialize(paths.Select(path=>Path.GetFullPath(path,baseDirectory))));
  values["SelectedPaths"]=J(JsonSerializer.Serialize(ReadPaths(values["SelectedPaths"].GetString()??"[]").Select(path=>Path.GetFullPath(path,baseDirectory))));
  if(values["CurrentImagePath"].GetString() is {Length:>0} current)values["CurrentImagePath"]=J(Path.GetFullPath(current,baseDirectory));
  if(values["SubscribeFolderPath"].GetString() is {Length:>0} folderPath)values["SubscribeFolderPath"]=J(Path.GetFullPath(folderPath,baseDirectory));
  values["EmbeddedImageResources"]=J(JsonSerializer.Serialize(embedded));return new(node with{Parameters=values},assets.Values.ToArray());
 }
}
