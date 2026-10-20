param([string]$Image="platform",[int[]]$Targets=@(0x6b7970,0x64a2e0))
$ErrorActionPreference='Stop'
$code=@'
using System;using System.Linq;using System.IO;using System.Reflection.PortableExecutable;using System.Collections.Generic;
public static class B3ReturnEh {
 public static object[] Read(string path,int[] targets){using var pe=new PEReader(File.OpenRead(path));var rows=new List<object>();var dir=pe.PEHeaders.PEHeader.ExceptionTableDirectory;var tab=pe.GetSectionData(dir.RelativeVirtualAddress).GetContent(0,dir.Size).ToArray();
  byte[] Data(int rva,int n)=>pe.GetSectionData(rva).GetContent(0,n).ToArray();
  foreach(int target in targets)for(int i=0;i+12<=tab.Length;i+=12){int start=BitConverter.ToInt32(tab,i),end=BitConverter.ToInt32(tab,i+4);if(target<start||target>=end)continue;int uw=BitConverter.ToInt32(tab,i+8);var b=Data(uw,64);int flags=b[0]>>3,off=4+((b[2]+1)&~1)*2;int handler=(flags&3)!=0?BitConverter.ToInt32(b,off):0,fi=(flags&3)!=0?BitConverter.ToInt32(b,off+4):0;var catches=new List<object>();string info="";if(fi>0&&fi<pe.PEHeaders.PEHeader.SizeOfImage){var f=Data(fi,40);info=Convert.ToHexString(f);if((BitConverter.ToUInt32(f,0)&0x1fffffff)==0x19930522){int count=BitConverter.ToInt32(f,12),map=BitConverter.ToInt32(f,16);if(count>=0&&count<100)for(int t=0;t<count;t++){var tr=Data(map+t*20,20);int nc=BitConverter.ToInt32(tr,12),ha=BitConverter.ToInt32(tr,16);if(nc>=0&&nc<100)for(int c=0;c<nc;c++){var h=Data(ha+c*20,20);catches.Add(new{tryLow=BitConverter.ToInt32(tr,0),tryHigh=BitConverter.ToInt32(tr,4),catchHigh=BitConverter.ToInt32(tr,8),adjectives=BitConverter.ToInt32(h,0),typeRva="0x"+BitConverter.ToInt32(h,4).ToString("x"),catchObjectOffset=BitConverter.ToInt32(h,8),handlerRva="0x"+BitConverter.ToInt32(h,12).ToString("x"),frameOffset=BitConverter.ToInt32(h,16)});}}}}
   rows.Add(new{target="0x"+target.ToString("x"),start="0x"+start.ToString("x"),end="0x"+end.ToString("x"),unwindRva="0x"+uw.ToString("x"),flags,handlerRva="0x"+handler.ToString("x"),funcInfoRva="0x"+fi.ToString("x"),funcInfoHex=info,catches});break;}
  return rows.ToArray();
 }
}
'@
Add-Type -TypeDefinition $code
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$path=if($Image -eq 'proxy'){'D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleProxy.dll'}else{'D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll'}
[B3ReturnEh]::Read($path,$Targets)|ConvertTo-Json -Depth 12|Set-Content "$root/analysis/camera-mapping/b3-return-source-20261019/$Image-eh.json" -Encoding utf8


