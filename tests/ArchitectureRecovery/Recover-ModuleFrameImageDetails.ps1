$ErrorActionPreference='Stop'
$taskRoot=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$taskDir=Join-Path $taskRoot 'analysis/native-moduleframe-image-20261021'
$evidenceDir=Join-Path $taskRoot 'docs/architecture-recovery/moduleframe-image.resources'
$projectDir=Join-Path $taskDir 'projects'
$javaHome=Join-Path $taskDir 'java-home'
$tempDir=Join-Path $taskDir 'temp'
foreach($dir in @($taskDir,$evidenceDir,$projectDir,$javaHome,$tempDir)){New-Item -ItemType Directory -Path $dir -Force | Out-Null}
$runtime=Join-Path $taskRoot '.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe'
$utility=Join-Path $taskRoot '.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar'
$target='D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'
$digest=(Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash.ToLowerInvariant()
if($digest -ne '45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b'){throw 'Original buffer DLL identity differs'}
$arguments=@('-Xmx2G','-XX:ParallelGCThreads=2','-XX:CICompilerCount=2',"-Duser.home=$javaHome","-Djava.io.tmpdir=$tempDir",'-cp',$utility,'ghidra.Ghidra','ghidra.app.util.headless.AnalyzeHeadless',$projectDir,'ModuleFrameImage','-process','ModuleFrame.dll','-noanalysis','-max-cpu','2','-scriptPath',(Join-Path $taskRoot 'tests/NativeEvidence'),'-postScript','VmNativeCapability.java',$evidenceDir,'1801312f0','180131190','180130530','1801303f0','180147230','180122a40','180121720','18008e690','1801e8810','1800c99f0','1800eee30','1800f49a0','1800f41e0','18009ae50','-log',(Join-Path $taskDir 'application.log'),'-scriptlog',(Join-Path $taskDir 'script.log'))
$arguments=@('-Djava.system.class.loader=ghidra.GhidraClassLoader','-Dfile.encoding=UTF8',"-Dapplication.settingsdir=$javaHome","-Dapplication.cachedir=$javaHome","-Dapplication.tempdir=$tempDir",'--enable-native-access=ALL-UNNAMED','-Xshare:off')+$arguments
@{target=$target;sha256=$digest;runtime=$runtime;arguments=$arguments;mode='offline static Ghidra CLI; not REA Evidence';vendorExecuted=$false} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidenceDir 'invocation.json') -Encoding utf8
& $runtime @arguments
if($LASTEXITCODE -ne 0){throw "Offline analysis failed: $LASTEXITCODE"}
