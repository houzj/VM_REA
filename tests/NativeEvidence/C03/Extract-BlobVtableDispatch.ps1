[CmdletBinding()]
param(
  [string]$DllPath = 'D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSBlobFindModu\IMVSBlobFindModu.dll',
  [string]$OutputPath = 'analysis/full-audit/batch1-20261107/C03/original-blob-vftable-region-dispatch.json',
  [UInt32]$VtableRva = 0x302C0,
  [int]$SlotCount = 15
)
Set-StrictMode -Version Latest
$bytes=[IO.File]::ReadAllBytes($DllPath)
$e=[BitConverter]::ToInt32($bytes,0x3c)
if([Text.Encoding]::ASCII.GetString($bytes,$e,4) -ne 'PE'+[char]0+[char]0){throw 'Invalid PE signature'}
$sectionCount=[BitConverter]::ToUInt16($bytes,$e+6)
$optionalSize=[BitConverter]::ToUInt16($bytes,$e+20)
$sectionTable=$e+24+$optionalSize
$sections=@()
for($i=0;$i -lt $sectionCount;$i++){
  $o=$sectionTable+40*$i
  $sections += [pscustomobject]@{
    Name=[Text.Encoding]::ASCII.GetString($bytes,$o,8).Trim([char]0)
    VirtualSize=[BitConverter]::ToUInt32($bytes,$o+8)
    VirtualAddress=[BitConverter]::ToUInt32($bytes,$o+12)
    RawSize=[BitConverter]::ToUInt32($bytes,$o+16)
    RawPointer=[BitConverter]::ToUInt32($bytes,$o+20)
  }
}
$section=$sections | Where-Object { $VtableRva -ge $_.VirtualAddress -and $VtableRva -lt ($_.VirtualAddress + [Math]::Max($_.VirtualSize,$_.RawSize)) } | Select-Object -First 1
if($null -eq $section){throw ('RVA 0x{0:X} is not in a section' -f $VtableRva)}
$fileOffset=[int]$section.RawPointer + ([int]$VtableRva-[int]$section.VirtualAddress)
$imageBase=[BitConverter]::ToUInt64($bytes,$e+24+24)
$entries=@()
for($i=0;$i -lt $SlotCount;$i++){
  $target=[BitConverter]::ToUInt64($bytes,$fileOffset+($i*8))
  $entries += [ordered]@{slot=$i;vtableOffset=('0x{0:X}' -f ($i*8));target=('0x{0:X}' -f $target);targetRva=('0x{0:X}' -f ($target-$imageBase))}
}
$sha=(Get-FileHash -LiteralPath $DllPath -Algorithm SHA256).Hash.ToLower()
$doc=[ordered]@{
 schemaVersion=1
 scope='C03 original Blob module vtable region dispatch'
 evidenceType='source-confirmed shipped PE vtable bytes; caller/export correlations are recorded separately'
 artifact=[ordered]@{path=$DllPath;sha256=$sha;architecture='x86_64';vtableRva=('0x{0:X}' -f $VtableRva);section=$section.Name;fileOffset=('0x{0:X}' -f $fileOffset);imageBase=('0x{0:X}' -f $imageBase)}
 entries=$entries
 correlations=@(
  [ordered]@{slot=2;offset='0x10';targetRva='0x1E3C0';role='GetParam';evidence='native export table'},
  [ordered]@{slot=3;offset='0x18';targetRva='0x22500';role='SetParam';evidence='native export table and stored decompilation'},
  [ordered]@{slot=7;offset='0x38';targetRva='0x1FA80';role='Process';evidence='native export table and stored decompilation'},
  [ordered]@{slot=9;offset='0x48';targetRva='0x9C80';role='GenerateMaskImage candidate';evidence='ProcessAlg call site passes module input/ROI-mask arguments'}
 )
 unknowns=@('RVA 0x9C80 function body and generated region-image pixel layout remain to be decompiled or captured.','The vtable includes inherited/base methods; SDK Blob vtable offsets must not be substituted.')
}
$parent=Split-Path -Parent $OutputPath
if($parent -and -not(Test-Path $parent)){New-Item -ItemType Directory -Path $parent -Force|Out-Null}
$doc|ConvertTo-Json -Depth 8|Set-Content -LiteralPath $OutputPath -Encoding UTF8
Get-FileHash -LiteralPath $OutputPath -Algorithm SHA256
