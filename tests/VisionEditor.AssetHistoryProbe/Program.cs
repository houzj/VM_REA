using System.IO;
using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ImageSource;

internal static class Program {
    [STAThread] static int Main(string[] args){
        if(args.Length!=1)throw new ArgumentException("Supply an audit output directory.");
        string root=Path.Combine(Path.GetFullPath(args[0]),"asset-history-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
        string oldDir=Path.Combine(root,"original"),newDir=Path.Combine(root,"saved");Directory.CreateDirectory(Path.Combine(oldDir,"assets"));Directory.CreateDirectory(newDir);
        string image=Path.Combine(oldDir,"assets","rows.bmp");WriteImage(image);byte[] originalBytes=File.ReadAllBytes(image);
        var plugin=new ImageSourcePlugin();var registry=new ModuleRegistry();var host=new ToolPluginRegistry();host.Register(plugin,registry);
        var values=plugin.Descriptor.Parameters.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
        values["ImagePaths"]=JsonSerializer.SerializeToElement(JsonSerializer.Serialize(new[]{"assets/rows.bmp"}));values["SolSaveImageData"]=JsonSerializer.SerializeToElement(true);
        var node=new NodeDocument("source",plugin.Manifest.Id,plugin.Manifest.Version,"source",values,20,30);
        var document=new SolutionDocument(SolutionStore.CurrentVersion,"asset-history","asset-history",[new("p","original",[node],[])],[]);
        string oldFile=Path.Combine(oldDir,"original.vmx.json"),newFile=Path.Combine(newDir,"saved.vmx.json");SolutionStore.Save(oldFile,document);
        var editor=new GraphEditorDocument(registry){PrepareSave=host.PrepareSave};editor.Load(oldFile);
        var observations=new List<object>();var results=new List<bool>();
        void Capture(string phase){
            var current=editor.CurrentProcedure.Nodes.Single();string path=JsonSerializer.Deserialize<string[]>(current.Parameters["ImagePaths"].GetString()!)!.Single();
            bool bytesMatch=false;string? error=null;
            try{using var input=plugin.OpenEncodedImage(current,path,editor.BaseDirectory,new ResourceCatalog(editor.BaseDirectory,editor.Document.Resources));using var memory=new MemoryStream();input.CopyTo(memory);bytesMatch=memory.ToArray().SequenceEqual(originalBytes);}
            catch(Exception e){error=e.GetType().FullName+": "+e.Message;}
            var map=JsonSerializer.Deserialize<Dictionary<string,string>>(current.Parameters["EmbeddedImageResources"].GetString()!)!;
            bool identity=Path.GetFullPath(path,editor.BaseDirectory)==image;results.Add(identity&&bytesMatch);
            observations.Add(new{phase,baseDirectory=editor.BaseDirectory,editor.FilePath,procedureName=editor.CurrentProcedure.Name,path,resolvedPath=Path.GetFullPath(path,editor.BaseDirectory),expectedPath=image,externalExists=File.Exists(image),resourceCount=editor.Document.Resources.Count,embeddedMapCount=map.Count,identityPreserved=identity,encodedBytesMatch=bytesMatch,error});
            Console.WriteLine($"{phase}: identity={identity}, bytes={bytesMatch}, resources={editor.Document.Resources.Count}, sourceExists={File.Exists(image)}, error={error}");
        }
        Capture("loaded-unprepared");editor.RenameProcedure("p","edited");editor.Save(newFile);Capture("prepared-save-as");
        editor.Undo();Capture("undo-before-prepare-snapshot");editor.Redo();Capture("redo-prepared-snapshot");
        File.Delete(image);editor.Load(newFile);Capture("reopen-without-external-file");
        bool passed=results.All(x=>x);
        File.WriteAllText(Path.Combine(root,"observations.json"),JsonSerializer.Serialize(new{passed,root,authority="current main plugin PrepareSave/history characterization; not native VM",observations},new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine($"passed={passed}; evidence={root}");return passed?0:1;
    }
    static void WriteImage(string path){using var writer=new BinaryWriter(File.Create(path));int stride=4,height=2,length=stride*height;writer.Write((ushort)0x4d42);writer.Write(1078+length);writer.Write(0);writer.Write(1078);writer.Write(40);writer.Write(4);writer.Write(height);writer.Write((ushort)1);writer.Write((ushort)8);writer.Write(0);writer.Write(length);writer.Write(0);writer.Write(0);writer.Write(256);writer.Write(0);for(int i=0;i<256;i++){writer.Write((byte)i);writer.Write((byte)i);writer.Write((byte)i);writer.Write((byte)0);}for(int y=0;y<height;y++)for(int x=0;x<stride;x++)writer.Write((byte)(20+y*10+x));}
}
