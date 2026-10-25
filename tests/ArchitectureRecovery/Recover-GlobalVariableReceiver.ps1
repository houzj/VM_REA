param([switch]$Details,[string[]]$Seeds=@('18001f8c0'))
$ErrorActionPreference='Stop'
$receiverRoot=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$receiverTask=Join-Path $receiverRoot 'analysis/native-global-variable-receiver-image-source-flow-20261021'
$receiverEvidence=Join-Path $receiverRoot 'docs/architecture-recovery/variable-native-receiver.resources'
$receiverProjects=Join-Path $receiverTask 'projects'
$receiverJavaHome=Join-Path $receiverTask 'java-home'
$receiverTemp=Join-Path $receiverTask 'temp'
foreach($receiverPath in @($receiverTask,$receiverEvidence,$receiverProjects,$receiverJavaHome,$receiverTemp)){New-Item -ItemType Directory -Path $receiverPath -Force|Out-Null}
$receiverRuntime=Join-Path $receiverRoot '.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe'
$receiverUtility=Join-Path $receiverRoot '.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar'
$receiverTarget='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/GlobalVariableModule/GlobalVariableModule.dll'
$receiverDigest=(Get-FileHash -LiteralPath $receiverTarget -Algorithm SHA256).Hash.ToLowerInvariant()
if($receiverDigest -ne '20cd716e9471a2687c0cc777548dedb469bcd0356b9da90aa97c42e301e16ff0'){throw 'Original variable DLL identity differs'}
$receiverArgs=@('-Djava.system.class.loader=ghidra.GhidraClassLoader','-Dfile.encoding=UTF8',"-Dapplication.settingsdir=$receiverJavaHome","-Dapplication.cachedir=$receiverJavaHome","-Dapplication.tempdir=$receiverTemp",'--enable-native-access=ALL-UNNAMED','-Xshare:off','-Xmx2G','-XX:ParallelGCThreads=2','-XX:CICompilerCount=2',"-Duser.home=$receiverJavaHome","-Djava.io.tmpdir=$receiverTemp",'-cp',$receiverUtility,'ghidra.Ghidra','ghidra.app.util.headless.AnalyzeHeadless',$receiverProjects,'GlobalVariableReceiver')
if($Details){$receiverArgs+=@('-process','GlobalVariableModule.dll','-noanalysis')}else{$receiverArgs+=@('-import',$receiverTarget,'-analysisTimeoutPerFile','90')}
$receiverArgs+=@('-max-cpu','2','-scriptPath',(Join-Path $receiverRoot 'tests/NativeEvidence'),'-postScript','VmNativeCapability.java',$receiverEvidence)+$Seeds+@('-log',(Join-Path $receiverTask 'application.log'),'-scriptlog',(Join-Path $receiverTask 'script.log'))
$receiverLabel=if($Details){'details'}else{'initial'}
@{target=$receiverTarget;sha256=$receiverDigest;runtime=$receiverRuntime;arguments=$receiverArgs;mode='offline static Ghidra CLI; not REA output';vendorExecuted=$false;details=[bool]$Details}|ConvertTo-Json -Depth 5|Set-Content -LiteralPath (Join-Path $receiverEvidence ('invocation-'+$receiverLabel+'.json')) -Encoding utf8
& $receiverRuntime @receiverArgs
$receiverExit=$LASTEXITCODE
@{exitCode=$receiverExit;finishedUtc=[DateTime]::UtcNow.ToString('o');liveProcessHandle=$null;vendorExecuted=$false}|ConvertTo-Json|Set-Content -LiteralPath (Join-Path $receiverEvidence ('completion-'+$receiverLabel+'.json')) -Encoding utf8
if($receiverExit -ne 0){throw "Offline static capture failed: $receiverExit"}
