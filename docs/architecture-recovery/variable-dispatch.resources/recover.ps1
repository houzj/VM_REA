param([ValidateSet('global','frame')][string]$Target='global',[string[]]$Seeds,[switch]$Details)
$ErrorActionPreference='Stop'
$taskRoot=(Resolve-Path '.').Path
$work=Join-Path $taskRoot ('analysis/native-variable-dispatch-20261021/'+$Target)
$out=Join-Path $taskRoot ('docs/architecture-recovery/variable-dispatch.resources/'+$Target)
foreach($p in @($work,$out,"$work/projects","$work/java-home","$work/temp")){New-Item -ItemType Directory -Path $p -Force|Out-Null}
$original=if($Target -eq 'global'){'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/GlobalVariableModule/GlobalVariableModule.dll'}else{'D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'}
$expected=if($Target -eq 'global'){'20cd716e9471a2687c0cc777548dedb469bcd0356b9da90aa97c42e301e16ff0'}else{'45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b'}
if((Get-FileHash -LiteralPath $original -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected){throw 'identity mismatch'}
$java=Join-Path $taskRoot '.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe'
$utility=Join-Path $taskRoot '.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar'
$a=@('-Djava.system.class.loader=ghidra.GhidraClassLoader','-Dfile.encoding=UTF8',"-Dapplication.settingsdir=$work/java-home","-Dapplication.cachedir=$work/java-home","-Dapplication.tempdir=$work/temp",'--enable-native-access=ALL-UNNAMED','-Xshare:off','-Xmx2G','-XX:ParallelGCThreads=2','-XX:CICompilerCount=2',"-Duser.home=$work/java-home","-Djava.io.tmpdir=$work/temp",'-cp',$utility,'ghidra.Ghidra','ghidra.app.util.headless.AnalyzeHeadless',"$work/projects",'VariableDispatch')
if($Details){$a+=@('-process',[IO.Path]::GetFileName($original),'-noanalysis')}else{$a+=@('-import',$original,'-analysisTimeoutPerFile','70')}
$a+=@('-max-cpu','2','-scriptPath',(Join-Path $taskRoot 'tests/NativeEvidence'),'-postScript','VmNativeCapability.java',$out)+$Seeds+@('-log',"$work/application.log",'-scriptlog',"$work/script.log")
$tag=($Seeds -join '-')
@{target=$original;sha256=$expected;arguments=$a;mode='offline static CLI; not REA';vendorExecuted=$false}|ConvertTo-Json -Depth 5|Set-Content -LiteralPath "$out/invocation-$tag.json" -Encoding utf8
& $java @a
$code=$LASTEXITCODE
@{exitCode=$code;finishedUtc=[DateTime]::UtcNow.ToString('o');vendorExecuted=$false}|ConvertTo-Json|Set-Content -LiteralPath "$out/completion-$tag.json" -Encoding utf8
if($code -ne 0){throw "static extraction exit $code"}
