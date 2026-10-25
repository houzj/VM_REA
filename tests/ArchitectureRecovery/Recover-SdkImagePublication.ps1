$ErrorActionPreference='Stop'
$taskRoot=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$taskDir=Join-Path $taskRoot 'analysis/native-sdk-image-publication-20261021'
$evidenceDir=Join-Path $taskRoot 'docs/architecture-recovery/sdk-image-publication.resources'
$projectDir=Join-Path $taskDir 'projects'
$javaHome=Join-Path $taskDir 'java-home'
$tempDir=Join-Path $taskDir 'temp'
foreach($dir in @($taskDir,$evidenceDir,$projectDir,$javaHome,$tempDir)){New-Item -ItemType Directory -Path $dir -Force | Out-Null}
$runtime=Join-Path $taskRoot '.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe'
$utility=Join-Path $taskRoot '.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar'
$target='D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll'
$digest=(Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash.ToLowerInvariant()
if($digest -ne 'b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1'){throw 'Original buffer DLL identity differs'}
$arguments=@('-Xmx2G','-XX:ParallelGCThreads=2','-XX:CICompilerCount=2',"-Duser.home=$javaHome","-Djava.io.tmpdir=$tempDir",'-cp',$utility,'ghidra.Ghidra','ghidra.app.util.headless.AnalyzeHeadless',$projectDir,'SdkImagePublication','-import',$target,'-overwrite','-analysisTimeoutPerFile','90','-max-cpu','2','-scriptPath',(Join-Path $taskRoot 'tests/NativeEvidence'),'-postScript','VmNativeCapability.java',$evidenceDir,'180645630','18064c650','180650730','-log',(Join-Path $taskDir 'application.log'),'-scriptlog',(Join-Path $taskDir 'script.log'))
$arguments=@('-Djava.system.class.loader=ghidra.GhidraClassLoader','-Dfile.encoding=UTF8',"-Dapplication.settingsdir=$javaHome","-Dapplication.cachedir=$javaHome","-Dapplication.tempdir=$tempDir",'--enable-native-access=ALL-UNNAMED','-Xshare:off')+$arguments
@{target=$target;sha256=$digest;runtime=$runtime;arguments=$arguments;mode='offline static Ghidra CLI; not REA Evidence';vendorExecuted=$false} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidenceDir 'invocation.json') -Encoding utf8
& $runtime @arguments
if($LASTEXITCODE -ne 0){throw "Offline analysis failed: $LASTEXITCODE"}
