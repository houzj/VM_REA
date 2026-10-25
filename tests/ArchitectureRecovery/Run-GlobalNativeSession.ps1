param(
 [ValidateSet('ModuleFrame','ModuleProxy','PlatformManager','ProcedureComm')][string]$ProgramName='ModuleFrame',
 [Parameter(Mandatory=$true)][string]$AddressesCsv,
 [Parameter(Mandatory=$true)][string]$OutputDirectory,
 [ValidateSet('VmNativeCapability.java','VmGlobalReferences.java')][string]$ScriptName='VmNativeCapability.java'
)
# Owned, persistent offline databases. Never attach another task's project or process.
$ErrorActionPreference='Stop'
$taskRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$target=if($ProgramName -eq 'PlatformManager'){'D:/Program Files/VisionMaster4.4.0/Applications/Server'}elseif($ProgramName -eq 'ProcedureComm'){'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/ProcedureComm'}else{'D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64'}
$sessionDir=Join-Path $taskRoot ('analysis/global-variable-followup/persistent-'+$ProgramName)
$runDir=[IO.Path]::GetFullPath((Join-Path $taskRoot $OutputDirectory))
if(!$runDir.StartsWith((Join-Path $taskRoot 'analysis/global-variable-followup/'),[StringComparison]::OrdinalIgnoreCase)){throw 'Output must remain in this task analysis directory'}
New-Item -ItemType Directory -Force "$sessionDir/project","$sessionDir/settings","$sessionDir/cache","$sessionDir/temp","$runDir/output"|Out-Null
$alias=Join-Path $env:TEMP ('global-native-session-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $alias|Out-Null
New-Item -ItemType Junction -Path "$alias/tools" -Target (Join-Path $taskRoot '.tools/native-analysis')|Out-Null
New-Item -ItemType Junction -Path "$alias/work" -Target $taskRoot|Out-Null
New-Item -ItemType Junction -Path "$alias/target" -Target $target|Out-Null
$priorJavaHome=$env:JAVA_HOME;$priorJavaOptions=$env:JAVA_TOOL_OPTIONS
try{
 $env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
 $env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/analysis/global-variable-followup/persistent-$ProgramName/settings -Dapplication.cachedir=$alias/work/analysis/global-variable-followup/persistent-$ProgramName/cache -Dapplication.tempdir=$alias/work/analysis/global-variable-followup/persistent-$ProgramName/temp"
 $sessionProject=$ProgramName+'GlobalVariables'
 $project="$alias/work/analysis/global-variable-followup/persistent-$ProgramName/project"
 $action=if(Test-Path (Join-Path $sessionDir ('project/'+$sessionProject+'.gpr'))){@('-process',($ProgramName+'.dll'))}else{@('-import',"$alias/target/$ProgramName.dll")}
 $addresses=@($AddressesCsv.Split(',')|ForEach-Object {$_.Trim()})
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" $project $sessionProject @action -noanalysis -scriptPath "$alias/work/tests/NativeEvidence" -postScript $ScriptName "$alias/work/$OutputDirectory/output" @addresses *> "$runDir/functions-headless.log"
 $outputPattern=if($ScriptName -eq 'VmGlobalReferences.java'){'references-*.tsv'}else{'functions-*.tsv'}
 if($LASTEXITCODE -ne 0 -or !(Test-Path "$runDir/output/$outputPattern")){throw 'Native extraction failed; inspect owned log'}
}finally{
 $env:JAVA_HOME=$priorJavaHome;$env:JAVA_TOOL_OPTIONS=$priorJavaOptions
 foreach($name in @('tools','work','target')){$link=Join-Path $alias $name;if(Test-Path -LiteralPath $link){Remove-Item -LiteralPath $link -Force}}
 if(Test-Path -LiteralPath $alias){Remove-Item -LiteralPath $alias -Force}
}
Get-Content "$runDir/output/$outputPattern"
