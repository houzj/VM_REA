using System;
using System.Collections.Generic;
using System.Linq;
namespace VisionPlugins.ContourMatch;

// Finite normal-domain reconstruction of module-owned candidate storage.
// Bytes are owned copies, not a vendor pattern decoder or pointer structure.
// Raw 260-byte names deliberately avoid guessing native ANSI encoding.
public sealed class ContourModelStorage
{
    public sealed record Entry(byte[] Name, byte[] Data, bool Selected);
    readonly object gate = new();
    readonly Entry?[] slots = new Entry?[100];
    byte[] publicName = new byte[260];
    public int Count { get; private set; }
    public void SetPublicName(ReadOnlySpan<byte> name)
    {
        // Independent safe admission replacing memcpy_s invalid-parameter paths.
        if(name.Length>260)throw new NotSupportedException("Native name buffer length exceeds 260.");
        lock(gate){publicName=new byte[260];name.CopyTo(publicName);}
    }
    public uint Add(ReadOnlySpan<byte> data)
    {
        if(data.Length==0)return 0xe0000001;
        lock(gate){
            for(int i=0;i<slots.Length;i++)if(slots[i] is null){
                // RVA6c50: first empty, selected1, Count++, copy pending name.
                slots[i]=new((byte[])publicName.Clone(),data.ToArray(),true);Count++;return 0;
            }
            return 0xe0000326;
        }
    }
    public uint Select(int index,int selected)
    {
        lock(gate){
            // RVA7da0: index>=0 && index<Count, flag0/1.
            if(index<0||index>=Count||(uint)selected>=2)return 0xe0000001;
            if(slots[index] is not {} entry)throw new NotSupportedException("Sparse imported storage requires its exact import-state mapping.");
            slots[index]=entry with{Selected=selected==1};return 0;
        }
    }
    public uint Delete(int index)
    {
        lock(gate){
            if((uint)index>=100)return 0xe0000001;
            // Native also decrements Count for an empty slot. This safety
            // boundary does not claim that malformed state is equivalent.
            if(slots[index] is null)throw new NotSupportedException("Native empty-slot Count underflow path is outside normal UI admission.");
            for(int i=index;i<99;i++)slots[i]=slots[i+1];slots[99]=null;Count--;return 0;
        }
    }
    public uint Rename(int index,ReadOnlySpan<byte> name)
    {
        if(name.Length>260)throw new NotSupportedException("Native name buffer exceeds 260.");
        lock(gate){
            if((uint)index>=100)return 0xe0000001;
            if(slots[index] is not {} entry)throw new NotSupportedException("Rename requires a normal populated UI model.");
            byte[] bytes=new byte[260];name.CopyTo(bytes);slots[index]=entry with{Name=bytes};return 0;
        }
    }
    public uint Move(int index,bool up)
    {
        lock(gate){
            if(up?(index<1||index>99):(index<0||index>98))return 0xe0000001;
            for(int other=index+(up?-1:1);other>=0&&other<100;other+=up?-1:1)
                if(slots[other] is not null){
                    // RVA8250 exchanges the entire 0x228 record, including
                    // name/path, pointer, sizes, and selected flag.
                    (slots[index],slots[other])=(slots[other],slots[index]);break;
                }
            return 0;
        }
    }
    public void Clear()
    {
        // RVA73a0 clears exactly 100 records/count, retains pending name and
        // selected-index storage located after the cleared d7b8 region.
        lock(gate){Array.Clear(slots);Count=0;}
    }
    public IReadOnlyList<(int Slot,Entry Value)> Snapshot()
    {
        lock(gate)return Array.AsReadOnly(slots.Select((value,index)=>(value,index))
            .Where(x=>x.value is not null).Select(x=>(x.index,new Entry(
                (byte[])x.value!.Name.Clone(),(byte[])x.value.Data.Clone(),x.value.Selected))).ToArray());
    }
}
