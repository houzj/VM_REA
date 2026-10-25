param([string[]]$Seeds=@('180028250','180025c40','180027080'),[string]$Capture='composite')
$ErrorActionPreference='Stop'
$storageRoot=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$storageTask=Join-Path $storageRoot 'analysis/native-variable-composite-image-source-flow-20261021'
$storageEvidence=Join-Path $storageRoot 'docs/architecture-recovery/variable-composite.resources'
$storageProjects=Join-Path $storageRoot 'analysis/native-global-variable-receiver-image-source-flow-20261021/projects'
$storageHome=Join-Path $storageTask 'java-home'
$storageTemp=Join-Path $storageTask 'temp'
foreach($storagePath in @($storageTask,$storageEvidence,$storageHome,$storageTemp)){New-Item -ItemType Directory -Path $storagePath -Force|Out-Null}
$storageRuntime=Join-Path $storageRoot '.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe'
$storageUtility=Join-Path $storageRoot '.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar'
$storageTarget='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/GlobalVariableModule/GlobalVariableModule.dll'
$storageDigest=(Get-FileHash -LiteralPath $storageTarget -Algorithm SHA256).Hash.ToLowerInvariant()
if($storageDigest -ne '20cd716e9471a2687c0cc777548dedb469bcd0356b9da90aa97c42e301e16ff0'){throw 'Original variable DLL identity differs'}
$storageArgs=@('-Djava.system.class.loader=ghidra.GhidraClassLoader','-Dfile.encoding=UTF8',"-Dapplication.settingsdir=$storageHome","-Dapplication.cachedir=$storageHome","-Dapplication.tempdir=$storageTemp",'--enable-native-access=ALL-UNNAMED','-Xshare:off','-Xmx2G','-XX:ParallelGCThreads=2','-XX:CICompilerCount=2',"-Duser.home=$storageHome","-Djava.io.tmpdir=$storageTemp",'-cp',$storageUtility,'ghidra.Ghidra','ghidra.app.util.headless.AnalyzeHeadless',$storageProjects,'GlobalVariableReceiver','-process','GlobalVariableModule.dll','-noanalysis','-max-cpu','2','-scriptPath',(Join-Path $storageRoot 'tests/NativeEvidence'),'-postScript','VmNativeCapability.java',$storageEvidence)+$Seeds+@('-log',(Join-Path $storageTask ('application-'+$Capture+'.log')),'-scriptlog',(Join-Path $storageTask ('script-'+$Capture+'.log')))
@{target=$storageTarget;sha256=$storageDigest;runtime=$storageRuntime;arguments=$storageArgs;mode='offline static Ghidra CLI, existing owned project -noanalysis; not REA output';vendorExecuted=$false}|ConvertTo-Json -Depth 5|Set-Content -LiteralPath (Join-Path $storageEvidence ('invocation-'+$Capture+'.json')) -Encoding utf8
& $storageRuntime @storageArgs
$storageExit=$LASTEXITCODE
@{exitCode=$storageExit;finishedUtc=[DateTime]::UtcNow.ToString('o');liveProcessHandle=$null;vendorExecuted=$false}|ConvertTo-Json|Set-Content -LiteralPath (Join-Path $storageEvidence ('completion-'+$Capture+'.json')) -Encoding utf8
if($storageExit -ne 0){throw "Offline static capture failed: $storageExit"}

