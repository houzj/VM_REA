using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

// Metadata-only: never invokes target constructors, properties, or methods.
public static class ExportIdentityCil
{
    static string root;
    static string Hash(byte[] b) { using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant(); }
    static Assembly Resolve(object sender,ResolveEventArgs e)
    {
        try{return Assembly.ReflectionOnlyLoad(e.Name);}catch{}
        string name=new AssemblyName(e.Name).Name+".dll";
        foreach(var d in new[]{root,Path.GetFullPath(Path.Combine(root,"../3rdLib/MVVM")),Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),"WPF")})
        {string p=Path.Combine(d,name);if(File.Exists(p))return Assembly.ReflectionOnlyLoadFrom(p);}return null;
    }
    static string Operand(Module mod,OpCode op,byte[] il,ref int i)
    {
        int start=i,n=0;switch(op.OperandType){case OperandType.InlineNone:return "";case OperandType.ShortInlineBrTarget:n=1;break;case OperandType.ShortInlineI:n=1;break;case OperandType.ShortInlineVar:n=1;break;case OperandType.InlineVar:n=2;break;case OperandType.InlineI8:case OperandType.InlineR:n=8;break;case OperandType.InlineSwitch:n=4+4*BitConverter.ToInt32(il,i);break;default:n=op.OperandType==OperandType.ShortInlineR?4:4;break;}
        i+=n;byte[] bytes=il.Skip(start).Take(n).ToArray();
        if(op.OperandType==OperandType.InlineMethod||op.OperandType==OperandType.InlineField||op.OperandType==OperandType.InlineType||op.OperandType==OperandType.InlineTok||op.OperandType==OperandType.InlineString)
        {int token=BitConverter.ToInt32(il,start);try{if(op.OperandType==OperandType.InlineString)return "0x"+token.ToString("X8")+" \""+mod.ResolveString(token)+"\"";var m=mod.ResolveMember(token);return "0x"+token.ToString("X8")+" "+m.DeclaringType+"::"+m;}catch(Exception e){return "0x"+token.ToString("X8")+" unresolved:"+e.GetType().Name;}}
        if(op.OperandType==OperandType.ShortInlineBrTarget)return "IL_"+(i+(sbyte)il[start]).ToString("X4");
        if(op.OperandType==OperandType.InlineBrTarget)return "IL_"+(i+BitConverter.ToInt32(il,start)).ToString("X4");
        return BitConverter.ToString(bytes).Replace("-","");
    }
    public static int Main(string[] args)
    {
        root=Path.GetFullPath(args[0]);string output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve+=Resolve;
        string path=Path.Combine(root,args[2]);var assembly=Assembly.ReflectionOnlyLoadFrom(path);var rows=new List<object>();var text=new List<string>();
        var ops=typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static).Where(f=>f.FieldType==typeof(OpCode)).Select(f=>(OpCode)f.GetValue(null)).ToDictionary(x=>unchecked((ushort)x.Value));
        var selections=new Dictionary<string,string[]>();
        if(args[2]=="VMControls.WPF.dll") {
          selections.Add("VMControls.WPF.DataForUI.VmModuleBaseDecorator",new[]{"CopyItemsCommandExecute","PasteItemsCommandExecute"});
          selections.Add("VMControls.WPF.DataForUI.Clipboard",new[]{"AddCopyInfo","ClearCopyInfo"});
        }
        if(args[2]=="VM.Core.dll") {
          selections.Add("VM.Core.VmModuleSet",new[]{"ProcedurePaste","AddModuleByInfo","CreatModuleByInfo"});
          selections.Add("VM.Core.VmModule",new[]{"ModulePaste","ModulePasteFinish","PasteIO","Init","PreAddModuleTo","get_UniqueName","get_UniqueGUID","set_Name","get_ID"});
          selections.Add("VM.Core.IMVSGroup",new[]{"Export"});
          selections.Add("VM.Core.VmFrameTreeBase",new[]{"get_FullName","get_FullGUID","set_Parent"});
          selections.Add("VM.Core.VmSolution",new[]{"LoadGroup","internalImportGroup","ModifyImportGroup","MModuleManager_OnImportGroupEndCallBack"});
        }
        foreach(var pair in selections)
        {
            var type=assembly.GetType(pair.Key,true);var methods=type.GetMethods(BindingFlags.DeclaredOnly|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static).Cast<MethodBase>().Concat(type.GetConstructors(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static));
            foreach(var m in methods.Where(m=>pair.Value.Contains(m.Name)))
            {var body=m.GetMethodBody();if(body==null)continue;byte[] il=body.GetILAsByteArray();text.Add("\n"+type.FullName+"::"+m.Name+" token=0x"+m.MetadataToken.ToString("X8")+" SHA256="+Hash(il));
                int i=0;while(i<il.Length){int offset=i;ushort code=il[i++];if(code==254)code=(ushort)(0xfe00|il[i++]);var op=ops[code];text.Add("IL_"+offset.ToString("X4")+": "+op.Name+" "+Operand(m.Module,op,il,ref i));}
                rows.Add(new{type=type.FullName,name=m.Name,token="0x"+m.MetadataToken.ToString("X8"),ilSize=il.Length,ilSha256=Hash(il),exceptionRegions=body.ExceptionHandlingClauses.Select(e=>new{flags=e.Flags.ToString(),tryOffset=e.TryOffset,tryLength=e.TryLength,handlerOffset=e.HandlerOffset,handlerLength=e.HandlerLength}).ToArray()});
            }
        }
        var result=new{authority="shipped-artifact",evidenceType="metadata-only ReflectionOnlyLoadFrom; no target execution",artifact=path,sha256=Hash(File.ReadAllBytes(path)),mvid=assembly.ManifestModule.ModuleVersionId,methods=rows,limitations=new[]{"Focused methods only; resolved operands are descriptive metadata", "No native SDK, WPF desktop, Undo or persistence replay"}};
        File.WriteAllText(Path.Combine(output,args[2]+".cil-methods.json"),new JavaScriptSerializer().Serialize(result));File.WriteAllLines(Path.Combine(output,args[2]+".cil-disassembly.txt"),text);Console.WriteLine("Exported "+rows.Count+" focused method bodies; target code not executed");return 0;
    }
}





