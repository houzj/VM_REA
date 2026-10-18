param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$output=Join-Path $root 'analysis/camera-mapping/parser'
New-Item -ItemType Directory -Force $output | Out-Null
$alias=Join-Path $env:TEMP ('camera-parser-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
$wpf='C:/Windows/Microsoft.NET/Framework64/v4.0.30319/WPF'
foreach($name in @('OriginalParserOperationProbe','OriginalUiProbe')){
 $exe=Join-Path $alias "analysis\camera-mapping\parser\$name.exe"
 $source=Join-Path $alias "tests\CameraMapping.NativeProbe\$name.cs"
 & C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /platform:x64 /r:System.Core.dll /r:System.Web.Extensions.dll /r:System.Xml.dll "/r:$Applications/myLibs/VMControls.BaseInterface.dll" "/r:$wpf/PresentationCore.dll" "/r:$wpf/PresentationFramework.dll" "/r:$wpf/WindowsBase.dll" /r:System.Xaml.dll "/out:$exe" $source
 if($LASTEXITCODE -ne 0){throw 'Original parser probe build failed.'}
 $destination=if($name -eq 'OriginalUiProbe'){Join-Path $alias 'analysis\camera-mapping\parser\model'}else{Join-Path $alias 'analysis\camera-mapping\parser\operation-factory.json'}
 $arguments=@($Applications,$destination) | ForEach-Object {'"'+$_+'"'}
 $process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput "$output/$name.stdout.log" -RedirectStandardError "$output/$name.stderr.log"
 if(!$process.WaitForExit(30000)){$process.Kill();throw 'Owned parser probe timeout'}
 if($process.ExitCode -ne 0){throw "Original parser probe failed: $($process.ExitCode)"}
}
