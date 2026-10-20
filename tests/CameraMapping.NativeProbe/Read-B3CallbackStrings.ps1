param([switch]$Manager,[switch]$Proxy)
$ErrorActionPreference='Stop'
$code=@'
using System;using System.IO;using System.Reflection.PortableExecutable;using System.Text;
public static class B3Strings { public static string Read(string path,int rva,bool wide){using var pe=new PEReader(File.OpenRead(path));var b=pe.GetSectionData(rva).GetContent(0,160);int n=0;for(;n+(wide?1:0)<b.Length;n+=wide?2:1){if(b[n]==0&&(!wide||b[n+1]==0))break;}return (wide?Encoding.Unicode:Encoding.ASCII).GetString(b.AsSpan(0,n));}}
'@
Add-Type -TypeDefinition $code
$path=if($Proxy){'D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleProxy.dll'}elseif($Manager){'D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll'}else{'D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll'}
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$addresses=if($Manager){@(0x7dd648,0x7e8c60,0x7e3ba8,0x7dabc8,0x7d7508,0x7d7650,0x7e8c38,0x7e72b0,0x7e72c0,0x7e71c4,0x7e7128,0x7d7690,0x7e70f8,0x7e1e90,0x7e1fb0,0x7d7774,0x7d7c04,0x7ddeb8,0x7db848,0x7db828,0x7db8d0,0x7db8a8,0x7db808,0x7db7e8)}else{@(0xab4c10,0xac0aa8,0xac40d0,0xac4020,0xac4130,0xac41c8,0xac4000,0xac4218)}
if($Proxy){$addresses=@(0x146690,0x1466d8,0x1472e8,0x14f268,0x14f0dc)}
$rows=@(foreach($rva in $addresses){
 [ordered]@{rva=('0x{0:x}' -f $rva);wide=(!$Manager -and $rva -in @(0xab4c10,0xac0aa8,0xac40d0));value=[B3Strings]::Read($path,$rva,(!$Manager -and $rva -in @(0xab4c10,0xac0aa8,0xac40d0)))}
})
$name=if($Proxy){'proxy-reset-strings'}elseif($Manager){'manager-reset-strings'}else{'callback-strings'}
$rows|ConvertTo-Json|Set-Content "$root/analysis/camera-mapping/b3-reset-source-20261019/$name.json" -Encoding utf8
$rows|Format-Table



