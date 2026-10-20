$ErrorActionPreference='Stop'
$code=@'
using System;using System.IO;using System.Reflection.PortableExecutable;using System.Collections.Generic;
public static class B3SetParamPointer {
 public static string[] Read(string path) {var bytes=File.ReadAllBytes(path);using var pe=new PEReader(new MemoryStream(bytes));var rows=new List<string>();foreach(var s in pe.PEHeaders.SectionHeaders){if((s.SectionCharacteristics&SectionCharacteristics.MemExecute)!=0)continue;for(int i=s.PointerToRawData+24;i<s.PointerToRawData+s.SizeOfRawData-8;i+=8){if(BitConverter.ToUInt64(bytes,i)!=0x18000ce30UL)continue;var va=pe.PEHeaders.PEHeader.ImageBase+(ulong)(s.VirtualAddress+i-s.PointerToRawData);rows.Add(va.ToString("x")+"\tprevious24="+Convert.ToHexString(bytes.AsSpan(i-24,24))+"\tnext32="+Convert.ToHexString(bytes.AsSpan(i,32)));}}return rows.ToArray();}
}
'@
Add-Type -TypeDefinition $code
$root=(Resolve-Path "$PSScriptRoot/../..").Path
[B3SetParamPointer]::Read('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModu.dll')|Set-Content "$root/analysis/camera-mapping/b3-reset-source-20261019/camera-setparam-pointer-candidates.tsv" -Encoding utf8
