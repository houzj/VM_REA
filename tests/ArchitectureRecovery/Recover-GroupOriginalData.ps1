param([string[]]$Seeds=@('1800a0930','1800a55b0','1800a7830','1800a7190'),[string]$Capture='loop-output')
$ErrorActionPreference='Stop'
$Seeds=@($Seeds | ForEach-Object { $_ -split ',' })
foreach($seed in $Seeds){if($seed -notmatch '^[0-9a-fA-F]+$'){throw 'Invalid native seed'}}
$taskRoot=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$taskDir=Join-Path $taskRoot 'analysis/native-group-data-20261021'
$evidenceDir=Join-Path $taskRoot 'docs/architecture-recovery/group-original-data.resources'
$projectDir=Join-Path $taskDir 'projects'
$javaHome=Join-Path $taskDir 'java-home'
$tempDir=Join-Path $taskDir 'temp'
foreach($dir in @($taskDir,$evidenceDir,$projectDir,$javaHome,$tempDir)){New-Item -ItemType Directory -Path $dir -Force | Out-Null}
$runtime=Join-Path $taskRoot '.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe'
$utility=Join-Path $taskRoot '.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar'
$target='D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'
$digest=(Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash.ToLowerInvariant()
if($digest -ne '45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b'){throw 'Original buffer DLL identity differs'}
$arguments=@('-Xmx2G','-XX:ParallelGCThreads=2','-XX:CICompilerCount=2',"-Duser.home=$javaHome","-Djava.io.tmpdir=$tempDir",'-cp',$utility,'ghidra.Ghidra','ghidra.app.util.headless.AnalyzeHeadless',$projectDir,'ModuleFrameImage','-process','ModuleFrame.dll','-noanalysis','-max-cpu','2','-scriptPath',(Join-Path $taskRoot 'tests/NativeEvidence'),'-postScript','VmNativeCapability.java',$evidenceDir)+$Seeds+@('-log',(Join-Path $taskDir 'application.log'),'-scriptlog',(Join-Path $taskDir 'script.log'))
$arguments=@('-Djava.system.class.loader=ghidra.GhidraClassLoader','-Dfile.encoding=UTF8',"-Dapplication.settingsdir=$javaHome","-Dapplication.cachedir=$javaHome","-Dapplication.tempdir=$tempDir",'--enable-native-access=ALL-UNNAMED','-Xshare:off')+$arguments
@{target=$target;sha256=$digest;runtime=$runtime;arguments=$arguments;mode='offline static Ghidra CLI; not REA Evidence';vendorExecuted=$false} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidenceDir ('invocation-'+$Capture+'.json')) -Encoding utf8
& $runtime @arguments
$storageExit=$LASTEXITCODE
@{exitCode=$storageExit;liveProcessHandle=$null;vendorExecuted=$false;finishedUtc=[DateTime]::UtcNow.ToString('o')}|ConvertTo-Json|Set-Content -LiteralPath (Join-Path $evidenceDir ('completion-'+$Capture+'.json')) -Encoding utf8
if($storageExit -ne 0){throw "Offline analysis failed: $storageExit"}
