using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
namespace VisionPlugins.ContourMatch;

// Model list and opaque UI sidecars only; training data is supplied by the
// not-yet-ported CreateDipolePyramid consumer, never synthesized here.
public sealed class ContourModelCollection
{
    public const string ParameterName="ContourCandidateStorage";
    public sealed record SavedModel(byte[] Name,byte[] Data,bool Selected);
    public sealed record SavedState(SavedModel[] Models,Dictionary<string,byte[]?> UserData);
    static readonly string[] movedSuffixes={"_BasicData","_CurImageData","_OtherMask","_OtherMask2","_PointsData"};
    readonly Dictionary<string,byte[]?> userData=new(StringComparer.Ordinal);
    public ContourModelStorage Storage { get; }=new();
    static string Key(int index,string suffix)=>"SolModelFile_"+index+suffix;
    public IReadOnlyDictionary<string,byte[]?> UserDataSnapshot()=>userData.ToDictionary(x=>x.Key,x=>x.Value?.ToArray());
    // Independent solution transport for owned opaque buffers, not a vendor
    // .contourmxml export/import or a native object serialization ABI.
    public string Save()=>JsonSerializer.Serialize(new SavedState(Storage.Snapshot().Select(x=>new SavedModel(x.Value.Name,x.Value.Data,x.Value.Selected)).ToArray(),userData));
    public static ContourModelCollection Load(string json)
    {
        var result=new ContourModelCollection();if(string.IsNullOrEmpty(json))return result;
        var saved=JsonSerializer.Deserialize<SavedState>(json)??throw new InvalidOperationException("Missing saved contour model state.");
        if(saved.Models is null||saved.Models.Length>100||saved.UserData is null)throw new InvalidOperationException("Invalid saved contour model collection.");
        foreach(var model in saved.Models){
            if(model.Name is null||model.Name.Length>260||model.Data is null||model.Data.Length==0)throw new InvalidOperationException("Invalid saved contour model record.");
            result.Storage.SetPublicName(model.Name);uint error=result.Storage.Add(model.Data);if(error!=0)throw new InvalidOperationException($"Model restore failed: {error:X8}");
            if(!model.Selected)result.Storage.Select(result.Storage.Count-1,0);
        }
        foreach(var item in saved.UserData)result.userData[item.Key]=item.Value?.ToArray();
        return result;
    }
    public uint Add(ReadOnlySpan<byte> name,ReadOnlySpan<byte> model,
        IReadOnlyDictionary<string,byte[]?> sidecars,Action persist)
    {
        int index=Storage.Count;
        Storage.SetPublicName(name);
        uint error=Storage.Add(model);if(error!=0)return error;
        try{
            foreach(var item in sidecars){
                if(!movedSuffixes.Contains(item.Key)&&item.Key!="_PointsNums")throw new ArgumentException("Unknown original model sidecar suffix.");
                userData[Key(index,item.Key)]=item.Value?.ToArray();
            }
            persist();return 0;
        }catch{
            // FeaturepatMatchConfigPanalDataBase.AddModelToModule catches
            // AddUserData failure and deletes native candidate; it does not
            // pretend already-written UI userdata was atomically rolled back.
            Storage.Delete(index);throw;
        }
    }
    public uint Delete(int index,Action persist)
    {
        uint error=Storage.Delete(index);if(error!=0)return error;
        for(int i=index;i<Storage.Count;i++)foreach(string suffix in movedSuffixes)
            userData[Key(i,suffix)]=userData.GetValueOrDefault(Key(i+1,suffix))?.ToArray();
        foreach(string suffix in movedSuffixes)userData[Key(Storage.Count,suffix)]=null;
        persist();return 0;
    }
    public uint Move(int index,bool up,Action persist)
    {
        // Source UI first/last guards avoid calling native boundary cases.
        if(up?index<=0:index>=Storage.Count-1)return 0;
        uint error=Storage.Move(index,up);if(error!=0)return error;
        int other=index+(up?-1:1);
        foreach(string suffix in movedSuffixes){
            string a=Key(index,suffix),b=Key(other,suffix);
            (userData[a],userData[b])=(userData.GetValueOrDefault(b),userData.GetValueOrDefault(a));
        }
        persist();return 0;
    }
    public void Clear(Action persist)
    {
        var prior=Storage.Snapshot();Storage.Clear();
        foreach(var item in prior)foreach(string suffix in movedSuffixes)userData[Key(item.Slot,suffix)]=null;
        // Original helpers omit _PointsNums from move/delete/clear; do not
        // silently "repair" that field without a new source consumer trace.
        persist();
    }
}
