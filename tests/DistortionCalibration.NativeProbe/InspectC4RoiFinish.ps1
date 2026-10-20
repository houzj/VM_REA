$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'));Set-Location $root
$code=@"
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Reflection.Emit;
public static class DistortionC4Metadata {
 public static object Read(string path,string[] selected) {
  using var f=File.OpenRead(path);using var pe=new PEReader(f);var md=pe.GetMetadataReader();var rows=new List<object>();
  string TN(EntityHandle h){if(h.Kind==HandleKind.TypeDefinition){var t=md.GetTypeDefinition((TypeDefinitionHandle)h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name);}if(h.Kind==HandleKind.TypeReference){var t=md.GetTypeReference((TypeReferenceHandle)h);return md.GetString(t.Namespace)+"."+md.GetString(t.Name);}return h.Kind.ToString();}
  string Member(int token){var h=MetadataTokens.EntityHandle(token);if(h.Kind==HandleKind.MemberReference){var m=md.GetMemberReference((MemberReferenceHandle)h);return TN(m.Parent)+"::"+md.GetString(m.Name);}if(h.Kind==HandleKind.MethodDefinition){var m=md.GetMethodDefinition((MethodDefinitionHandle)h);return TN(m.GetDeclaringType())+"::"+md.GetString(m.Name);}if(h.Kind==HandleKind.FieldDefinition){var m=md.GetFieldDefinition((FieldDefinitionHandle)h);return TN(m.GetDeclaringType())+"::"+md.GetString(m.Name);}return h.Kind.ToString();}
  var opmap=typeof(OpCodes).GetFields().Where(x=>x.FieldType==typeof(OpCode)).Select(x=>(OpCode)x.GetValue(null)).ToDictionary(x=>(ushort)x.Value);
  foreach(var th in md.TypeDefinitions){var t=md.GetTypeDefinition(th);string name=TN(th);if(!selected.Contains(name))continue;foreach(var mh in t.GetMethods()){var m=md.GetMethodDefinition(mh);var anchors=new List<object>();string hash=null;int bytes=0;object[] eh=Array.Empty<object>();if(m.RelativeVirtualAddress!=0){var body=pe.GetMethodBody(m.RelativeVirtualAddress);var il=body.GetILBytes();bytes=il.Length;hash=Convert.ToHexString(SHA256.HashData(il));eh=body.ExceptionRegions.Select(e=>(object)new{kind=e.Kind.ToString(),tryOffset=e.TryOffset,tryLength=e.TryLength,handlerOffset=e.HandlerOffset,handlerLength=e.HandlerLength}).ToArray();for(int i=0;i<il.Length;){int pos=i;ushort id=il[i++];if(id==254)id=(ushort)(0xfe00|il[i++]);var op=opmap[id];int n=0;switch(op.OperandType){case OperandType.InlineNone:break;case OperandType.ShortInlineBrTarget:case OperandType.ShortInlineI:case OperandType.ShortInlineVar:n=1;break;case OperandType.InlineVar:n=2;break;case OperandType.InlineI8:case OperandType.InlineR:n=8;break;case OperandType.InlineSwitch:n=4+4*BitConverter.ToInt32(il,i);break;default:n=4;break;}if(n==4&&(op.OperandType==OperandType.InlineMethod||op.OperandType==OperandType.InlineField||op.OperandType==OperandType.InlineString)){int tok=BitConverter.ToInt32(il,i);string value=op.OperandType==OperandType.InlineString?md.GetUserString(MetadataTokens.UserStringHandle(tok&0xffffff)):Member(tok);anchors.Add(new{offset=pos,opcode=op.Name,token="0x"+tok.ToString("x8"),value});}i+=n;}}
   rows.Add(new{type=name,typeToken="0x"+MetadataTokens.GetToken(th).ToString("x8"),method=md.GetString(m.Name),token="0x"+MetadataTokens.GetToken(mh).ToString("x8"),rva=m.RelativeVirtualAddress,ilBytes=bytes,ilSha256=hash,exceptionRegions=eh,anchors});}}
  return new{path,mvid=md.GetGuid(md.GetModuleDefinition().Mvid).ToString(),members=rows};
 }
}
"@
Add-Type -TypeDefinition $code
$specs=@(
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll';types=@('VMControls.WPF.ParamsTabControlBase','VMControls.WPF.ParamsConfigControlBase','VMControls.WPF.TabWinBase','VMControls.WPF.ProcedureConfigControl','VMControls.WPF.ModuleResultView','VMControls.WPF.RenderControlBase','VMControls.WPF.DataForUI.DisplayImageListDecorator','VMControls.WPF.DataForUI.ModuleDisplayDecorator','VMControls.WPF.DataForUI.ImageViewItem','VMControls.WPF.Base.ROISelecterHolder')},
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlParser.dll';types=@('Apps.XmlParser.ObjectBase','Apps.XmlParser.FeatureBase','Apps.XmlParser.ParamTab.ROISelecter','Apps.XmlParser.ParamTab.AlgorithmTabRoot','Apps.XmlParser.ParamTab.Tab','Apps.XmlParser.ParamTab.Category','Apps.XmlParser.ParamTab.NameValueItem','Apps.XmlParser.ParamTab.ROIDataStore','Apps.XmlParser.ParamTab.GetROIDataOperation','Apps.XmlParser.ParamTab.SetROIDataOperation')}
)
$specs+=@{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlUI.dll';types=@('Apps.XmlUI.ParamTabControl.ROIStackControl','Apps.XmlUI.ParamTabControl.RoiSelectControl','Apps.XmlUI.ParamsTabUserControl')}
$result=@(foreach($v in $specs){$m=[DistortionC4Metadata]::Read($v.path,[string[]]$v.types);[ordered]@{path=$v.path;sha256=(Get-FileHash $v.path).Hash;provider='Selected-type PEReader; no target loading';mvid=$m.mvid;members=$m.members}})
$result|ConvertTo-Json -Depth 16|Set-Content analysis/distortion-calibration/c4-source/managed/roi-finish-metadata.json -Encoding utf8
$result|ForEach-Object{@{path=$_.path;sha256=$_.sha256;mvid=$_.mvid;methods=$_.members.Count}}|ConvertTo-Json



