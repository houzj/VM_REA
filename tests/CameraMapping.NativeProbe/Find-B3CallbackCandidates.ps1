param([int]$Displacement=0x2398,[string]$Image='D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll',[string]$OutputName='callback-displacement-candidates.tsv')
$ErrorActionPreference='Stop'
# Passive raw-byte candidates, not decoded instructions or proof of references.
# PE exception-table ranges identify function entries for subsequent Ghidra work.
$code=@'
using System;using System.IO;using System.Reflection.PortableExecutable;using System.Collections.Generic;
public static class B3CallbackCandidates {
 public static string[] Find(string path,int displacement) {
  var bytes=File.ReadAllBytes(path);using var pe=new PEReader(new MemoryStream(bytes));var lines=new List<string>();
  var d=pe.PEHeaders.PEHeader.ExceptionTableDirectory;var table=pe.GetSectionData(d.RelativeVirtualAddress).GetContent(0,d.Size);
  foreach(var s in pe.PEHeaders.SectionHeaders) {
   if((s.SectionCharacteristics&SectionCharacteristics.MemExecute)==0)continue;
   for(int i=s.PointerToRawData+8;i<s.PointerToRawData+s.SizeOfRawData-8;i++) {
    if(BitConverter.ToInt32(bytes,i)!=displacement)continue;
    int rva=s.VirtualAddress+i-s.PointerToRawData;string function="unknown";
    for(int j=0;j+12<=table.Length;j+=12) {
     int a=table[j]|table[j+1]<<8|table[j+2]<<16|table[j+3]<<24;
     int e=table[j+4]|table[j+5]<<8|table[j+6]<<16|table[j+7]<<24;
     if(rva>=a&&rva<e){function=(pe.PEHeaders.PEHeader.ImageBase+(ulong)a).ToString("x")+"-"+(pe.PEHeaders.PEHeader.ImageBase+(ulong)e).ToString("x");break;}
    }
    lines.Add((pe.PEHeaders.PEHeader.ImageBase+(ulong)rva).ToString("x")+"\t"+function+"\t"+Convert.ToHexString(bytes.AsSpan(i-8,20)));
   }
  }
  return lines.ToArray();
 }
}
'@
Add-Type -TypeDefinition $code
$root=(Resolve-Path "$PSScriptRoot/../..").Path
[B3CallbackCandidates]::Find($Image,$Displacement) | Set-Content "$root/analysis/camera-mapping/b3-reset-source-20261019/$OutputName" -Encoding utf8
