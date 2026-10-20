param(
 [string]$Target='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSBinaryModu',
 [string]$ProgramName='IMVSBinaryModu',
 [string]$AddressesCsv='18001e750,18001e8d0,18001ec00,18001f100,18001f9f0',
 [string]$OutputDirectory='analysis/source-audit-regression/20261018/ghidra-binary'
)
$Addresses=@($AddressesCsv.Split(',') | ForEach-Object {$_.Trim()})
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$tools='D:/MyWork/REA逆向测试/.tools/native-analysis'
$run=$OutputDirectory
New-Item -ItemType Directory -Force "$root/$run/output","$root/$run/project","$root/$run/settings","$root/$run/cache","$root/$run/temp"|Out-Null
$alias=Join-Path $env:TEMP ('binary-ghidra-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $alias|Out-Null
New-Item -ItemType Junction -Path "$alias/tools" -Target $tools|Out-Null
New-Item -ItemType Junction -Path "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction -Path "$alias/target" -Target $target|Out-Null
$oldJavaHome=$env:JAVA_HOME;$oldJavaOptions=$env:JAVA_TOOL_OPTIONS
try{
 $env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
 $env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/$run/settings -Dapplication.cachedir=$alias/work/$run/cache -Dapplication.tempdir=$alias/work/$run/temp"
 $binary=Get-ChildItem -LiteralPath $target -Filter '*.dll' | Where-Object Name -EQ ($ProgramName+'.dll') | Select-Object -First 1
 if(!$binary){throw "Target DLL not found: $ProgramName"}
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/$run/project" ($ProgramName+'Functions') -import (Join-Path "$alias/target" $binary.Name) -noanalysis -scriptPath "$alias/work/tests/NativeEvidence" -postScript VmNativeCapability.java "$alias/work/$run/output" @Addresses -analysisTimeoutPerFile 120 -deleteProject *> "$root/$run/functions-headless.log"
 if($LASTEXITCODE -ne 0 -or !(Test-Path "$root/$run/output/functions-*.tsv")){throw 'Ghidra function decompilation failed; inspect functions-headless.log.'}
}finally{
 $env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions
 foreach($name in @('tools','work','target')){$link=Join-Path $alias $name;if(Test-Path -LiteralPath $link){Remove-Item -LiteralPath $link -Force}}
 if(Test-Path -LiteralPath $alias){Remove-Item -LiteralPath $alias -Force}
}
Get-Content "$root/$run/output/functions-*.tsv"
