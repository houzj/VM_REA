$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b3-input-lifecycle-source-20261019'
# Passive PE metadata only: no target assembly loading or target initialization.
$code=@'
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
public static class B3InputMetadata {
 public static object[] Read(string path,string[] types) {
  using var stream=File.OpenRead(path);using var pe=new PEReader(stream);var reader=pe.GetMetadataReader();
  var rows=new List<object>();
  string TypeName(EntityHandle h){if(h.IsNil)return "<none>";if(h.Kind==HandleKind.TypeDefinition){var t=reader.GetTypeDefinition((TypeDefinitionHandle)h);return reader.GetString(t.Namespace)+"."+reader.GetString(t.Name);}if(h.Kind==HandleKind.TypeReference){var t=reader.GetTypeReference((TypeReferenceHandle)h);return reader.GetString(t.Namespace)+"."+reader.GetString(t.Name);}return h.Kind.ToString();}
  foreach(var th in reader.TypeDefinitions){var t=reader.GetTypeDefinition(th);var full=reader.GetString(t.Namespace)+"."+reader.GetString(t.Name);if(!types.Contains(full))continue;
   rows.Add(new {type=full,method="<type-metadata>",baseType=TypeName(t.BaseType),interfaces=t.GetInterfaceImplementations().Select(h=>TypeName(reader.GetInterfaceImplementation(h).Interface)).ToArray()});
   foreach(var fh in t.GetFields()){var f=reader.GetFieldDefinition(fh);rows.Add(new {type=full,typeToken="0x"+MetadataTokens.GetToken(th).ToString("x8"),field=reader.GetString(f.Name),fieldToken="0x"+MetadataTokens.GetToken(fh).ToString("x8"),signature=Convert.ToHexString(reader.GetBlobBytes(f.Signature))});}
   foreach(var mh in t.GetMethods()){var m=reader.GetMethodDefinition(mh);rows.Add(new {type=full,typeToken="0x"+MetadataTokens.GetToken(th).ToString("x8"),method=reader.GetString(m.Name),methodToken="0x"+MetadataTokens.GetToken(mh).ToString("x8"),rva="0x"+m.RelativeVirtualAddress.ToString("x"),signature=Convert.ToHexString(reader.GetBlobBytes(m.Signature))});}
  }
  return rows.ToArray();
 }
}
'@
Add-Type -TypeDefinition $code
$specs=@(
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlParser.dll';types=@('Apps.XmlParser.ParamTab.ValueBase','Apps.XmlParser.ParamTab.OpenFileForCalibDialog','Apps.XmlParser.ParamTab.SetModuleParamBindOperation','Apps.XmlParser.ParamTab.AlgorithmTabRoot','Apps.XmlParser.ParamTab.Button_ResetAlgoParams','Apps.XmlParser.ParamTab.PointPair','Apps.XmlParser.ParamTab.ButtonSelecter','Apps.XmlParser.ParamTab.AddButton_PointPair','Apps.XmlParser.ParamTab.SetCombinationSourceOperation','Apps.XmlParser.ParamTab.SetFilterSourceOperation','Apps.XmlParser.ParamTab.GetSelectedCombinationOperation','Apps.XmlParser.ParamTab.GetSelectedFilterOperation','Apps.XmlParser.ParamTab.RadioSelecter','Apps.XmlParser.ParamTab.Setter','Apps.XmlParser.ParamTab.Trigger','Apps.XmlParser.ParamTab.NameValueItem')},
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll';types=@('VM.Core.VmIO','VM.Core.VmModule')},
 @{path='D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/VM.PlatformSDKCS.dll';types=@('VM.PlatformSDKCS.ModuleManager','VM.PlatformSDKCS.ServerSDKAPI','VM.PlatformSDKCS.NativeMethods')}
)
$reports=@(foreach($s in $specs){[ordered]@{path=$s.path;sha256=(Get-FileHash $s.path).Hash;provider='System.Reflection.Metadata PEReader, no target execution';members=@([B3InputMetadata]::Read($s.path,[string[]]$s.types))}})
$reports|ConvertTo-Json -Depth 12|Set-Content "$out/managed-metadata.json" -Encoding utf8
foreach($r in $reports){$r.members|Where-Object{$_.method -match 'Reset|DefaultParam'}|Select-Object type,method,methodToken,rva}



