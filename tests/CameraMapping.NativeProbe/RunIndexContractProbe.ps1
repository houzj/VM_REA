param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$output=Join-Path $root 'analysis/camera-mapping/native/index-contract'
New-Item -ItemType Directory -Force $output | Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
if(-not $vs){throw 'MSVC x64 tools were not found.'}
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('camera-map-index-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias | Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root | Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target $SdkRoot | Out-Null
$cmd=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/native/index-contract/IndexContractProbe.obj" /Fe"$alias/work/analysis/camera-mapping/native/index-contract/IndexContractProbe.exe" "$alias/work/tests/CameraMapping.NativeProbe/IndexContractProbe.cpp"
"@
[IO.File]::WriteAllText((Join-Path $output 'build.cmd'),$cmd,[Text.Encoding]::ASCII)
& cmd /c (Join-Path $output 'build.cmd') 2>&1 | Tee-Object (Join-Path $output 'build.log')
if($LASTEXITCODE -ne 0){throw "Probe build failed: $LASTEXITCODE"}

$dll=Join-Path $SdkRoot 'Runtime/x64/MVDCameraMapCpp.dll'
$exe=Join-Path $output 'IndexContractProbe.exe'
$results=[System.Collections.Generic.List[object]]::new()
function Invoke-IndexCase($case,[int]$base){
  $stdout=Join-Path $output ("{0}-{1}.stdout.jsonl" -f $case.op,$case.index)
  $stderr=Join-Path $output ("{0}-{1}.stderr.txt" -f $case.op,$case.index)
  $psi=[System.Diagnostics.ProcessStartInfo]::new()
  $psi.FileName=$exe
  $psi.UseShellExecute=$false
  $psi.CreateNoWindow=$true
  $psi.RedirectStandardOutput=$true
  $psi.RedirectStandardError=$true
  $psi.Environment['PATH']="$SdkRoot/Runtime/x64;$($psi.Environment['PATH'])"
  $psi.Arguments='"{0}" "{1}" "{2}" "{3}"' -f $dll,$case.op,$case.index,$base
  $process=[System.Diagnostics.Process]::new()
  $process.StartInfo=$psi
  [void]$process.Start()
  $outTask=$process.StandardOutput.ReadToEndAsync()
  $errTask=$process.StandardError.ReadToEndAsync()
  $completed=$process.WaitForExit(5000)
  if(-not $completed){try{$process.Kill()}catch{};[void]$process.WaitForExit(2000)}
  $outText=if($outTask.Wait(1500)){$outTask.GetAwaiter().GetResult()}else{''}
  $errText=if($errTask.Wait(1500)){$errTask.GetAwaiter().GetResult()}else{''}
  [IO.File]::WriteAllText($stdout,$outText,[Text.Encoding]::UTF8)
  [IO.File]::WriteAllText($stderr,$errText,[Text.Encoding]::UTF8)
  $exit=if($completed){$process.ExitCode}else{$null}
  $record=[pscustomobject]@{operation=$case.op;index=$case.index;getterBaseArgument=$base;completed=$completed;exitCode=$exit;stdout=$outText;stderr=$errText}
  $process.Dispose()
  return $record
}
$getterIndexes=@(0,1,2,3,-1)
foreach($op in @('get-object','get-target')){foreach($index in $getterIndexes){$results.Add((Invoke-IndexCase ([pscustomobject]@{op=$op;index=$index}) -1))}}

# Derive a getter base only from the known distinctive first stored point. If
# evidence is ambiguous, removal snapshots are omitted rather than guessed.
$getterBase=$null
$objectZero=$null
$objectOne=$null
foreach($record in $results | Where-Object operation -eq 'get-object'){
  if(-not $record.completed -or -not $record.stdout){continue}
  foreach($line in $record.stdout -split "`r?`n"){
    if(-not $line){continue}
    try{$event=$line|ConvertFrom-Json}catch{continue}
    if($event.event -eq 'get-object' -and $event.repeat -eq 0){
      if($event.index -eq 0){$objectZero=$event}
      if($event.index -eq 1){$objectOne=$event}
    }
  }
}
if($objectZero -and $objectZero.x -eq 10 -and $objectZero.y -eq 1){$getterBase=0}
elseif($objectOne -and $objectOne.x -eq 10 -and $objectOne.y -eq 1){$getterBase=1}
foreach($index in $getterIndexes){$results.Add((Invoke-IndexCase ([pscustomobject]@{op='remove';index=$index}) $(if($null -eq $getterBase){-1}else{$getterBase})))}
$report=[pscustomobject]@{
  generatedUtc=[DateTime]::UtcNow.ToString('o')
  sdkDll=$dll
  sdkDllSha256=(Get-FileHash $dll -Algorithm SHA256).Hash.ToLowerInvariant()
  sdkVersion='V4.4.0_build_Apr29_2025'
  hardwareUsed=$false
  runtimeScope='direct SDK ICameraMapBasicParam only; no platform, module, Run(), or device invocation'
  processIsolation='one fresh child process per operation/index; 5 second timeout'
  initialPairs=3
  operations=@('GetObjectPoint','GetTargetPoint','RemoveAlignPairAt')
  indexes=@(0,1,2,3,-1)
  repeatedCalls=2
  getterIndexBaseInferred=$getterBase
  baseInference='GetObjectPoint result matching initial first point (10,1); unambiguous pattern required to enable removal snapshots'
  cases=$results
}
$report|ConvertTo-Json -Depth 8|Set-Content (Join-Path $output 'report.json') -Encoding utf8
Write-Host "Wrote SDK index contract probe report: $(Join-Path $output 'report.json')"
