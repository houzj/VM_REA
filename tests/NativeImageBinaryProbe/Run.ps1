param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$repoRoot=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$out=Join-Path $repoRoot 'analysis/full-audit/roi-binary-native-probe'
Push-Location -LiteralPath $PSScriptRoot
try {
 $refs=Get-ChildItem (Join-Path $SdkRoot 'ReferencedAssemblies/Common') -Filter '*.dll' | Where-Object Name -in 'MVDImage.Net.dll','MVDShape.Net.dll','MVDCore.Net.dll','MVDRegion.Net.dll'
 foreach($ref in $refs){Copy-Item -LiteralPath $ref.FullName -Destination $PSScriptRoot}
 Copy-Item -LiteralPath (Join-Path $SdkRoot 'ReferencedAssemblies/Algorithms/MVDImageBinary.Net.dll') -Destination $PSScriptRoot
 $compilerRefs=Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.dll' | ForEach-Object {'/r:'+ $_.FullName}
 & "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo /r:System.Drawing.dll /out:Probe.exe @compilerRefs Probe.cs
 if($LASTEXITCODE -ne 0){throw 'Native probe compilation failed.'}
 & .\Probe.exe $SdkRoot $out
 if($LASTEXITCODE -ne 0){throw 'Native probe failed. Inspect retained failure.txt.'}
 & "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo /r:System.Drawing.dll /out:SectorProbe.exe @compilerRefs SectorProbe.cs
 if($LASTEXITCODE -ne 0){throw 'Sector probe compilation failed.'}
 & .\SectorProbe.exe $SdkRoot (Join-Path $repoRoot 'analysis/full-audit/sector-raster-probe')
 if($LASTEXITCODE -ne 0){throw 'Sector probe failed.'}
 @{Success=$true;SdkRoot=$SdkRoot;Time=(Get-Date).ToString('o');AssemblyHashes=@(Get-ChildItem -Filter '*.dll' | Get-FileHash -Algorithm SHA256 | Select-Object Path,Hash)} | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $out 'latest-run.json')
} finally {Pop-Location}
