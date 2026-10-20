param([ValidateSet('manager','proxy','frame')][string]$Image='manager',[int[]]$Rva=@(0x7d7728,0x7d7738))
$ErrorActionPreference='Stop'
$code=@'
using System;using System.IO;using System.Reflection.PortableExecutable;using System.Text;
public static class B3ReturnStrings {public static string Read(string path,int rva,bool wide){using var pe=new PEReader(File.OpenRead(path));var b=pe.GetSectionData(rva).GetContent(0,256);int n=0;for(;n+(wide?1:0)<b.Length;n+=wide?2:1)if(b[n]==0&&(!wide||b[n+1]==0))break;return(wide?Encoding.Unicode:Encoding.ASCII).GetString(b.AsSpan(0,n));}}
'@
Add-Type -TypeDefinition $code
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$path=@{manager='D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll';proxy='D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleProxy.dll';frame='D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'}[$Image]
$output=Join-Path $root 'analysis/camera-mapping/b3-return-source-20261019'
New-Item -ItemType Directory $output -Force|Out-Null
$rows=@(foreach($address in $Rva){[ordered]@{image=$path;sha256=(Get-FileHash -LiteralPath $path).Hash;rva=('0x{0:x}' -f $address);ascii=[B3ReturnStrings]::Read($path,$address,$false);utf16=[B3ReturnStrings]::Read($path,$address,$true)}})
$rows|ConvertTo-Json -Depth 5|Set-Content "$output/$Image-strings.json" -Encoding utf8
$rows|Select-Object rva,ascii,utf16
