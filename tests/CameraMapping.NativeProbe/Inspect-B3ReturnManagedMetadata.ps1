$ErrorActionPreference='Stop'
$code=@'
using System;using System.IO;using System.Linq;using System.Collections.Generic;using System.Reflection.Metadata;using System.Reflection.Metadata.Ecma335;using System.Reflection.PortableExecutable;
public static class B3ReturnManaged {
 public static object[] Read(string path,string name){using var pe=new PEReader(File.OpenRead(path));var r=pe.GetMetadataReader();var rows=new List<object>();foreach(var h in r.TypeDefinitions){var t=r.GetTypeDefinition(h);if(r.GetString(t.Name)!=name)continue;foreach(var mh in t.GetMethods()){var m=r.GetMethodDefinition(mh);rows.Add(new{type=r.GetString(t.Namespace)+"."+name,typeToken="0x"+MetadataTokens.GetToken(h).ToString("x8"),method=r.GetString(m.Name),token="0x"+MetadataTokens.GetToken(mh).ToString("x8"),rva="0x"+m.RelativeVirtualAddress.ToString("x"),signature=Convert.ToHexString(r.GetBlobBytes(m.Signature))});}}return rows.ToArray();}
}
'@
Add-Type -TypeDefinition $code
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$specs=@(@{path='D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.ErrorCode.dll';name='ErrorCodeInfo'},@{path='D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/VM.PlatformSDKCS.dll';name='VmException'})
$rows=@(foreach($s in $specs){[ordered]@{path=$s.path;sha256=(Get-FileHash -LiteralPath $s.path).Hash;provider='PEReader passive metadata, no target load';members=@([B3ReturnManaged]::Read($s.path,$s.name))}})
$rows|ConvertTo-Json -Depth 10|Set-Content "$root/analysis/camera-mapping/b3-return-source-20261019/managed-metadata.json" -Encoding utf8

