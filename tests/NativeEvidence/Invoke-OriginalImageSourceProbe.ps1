#Requires -Version 7.0
param(
    [string]$Applications = 'D:/Program Files/VisionMaster4.4.0/Applications',
    [string]$Solution = 'D:/Program Files/VisionMaster4.4.0/Development/V4.x/Samples/C#/PlatformSDKSampleCS/CircleFind/CircleFind.sol',
    [switch]$CaptureSdkUi,
    [switch]$ProbeStitch,
    [switch]$ProbeCodecs,
    [switch]$ProbeFolder
)
$ErrorActionPreference = 'Stop'
$workspace = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$runId = [Guid]::NewGuid().ToString('N')
$relativeOutput = 'analysis/full-audit/image-source-runtime/run-' + $runId
$output = Join-Path $workspace $relativeOutput
New-Item -ItemType Directory -Path $output | Out-Null
$binary = Join-Path $Applications 'Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModule.dll'
if ((Get-FileHash -LiteralPath $binary).Hash -ne '91EA3659BE97E51EDEAB0C913779D4555D51DCD0D6288086FBF9916D35B942DB') { throw 'Original DLL identity differs; audit before execution' }
# Legacy csc misinterprets this workspace's Unicode path. A unique alias points
# only at this worktree; all generated files still land in this worktree.
$aliasRoot = Join-Path $env:TEMP ('vision-image-probe-' + $runId)
New-Item -ItemType Directory -Path $aliasRoot | Out-Null
$alias = Join-Path $aliasRoot 'workspace'
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$exe = Join-Path $alias ($relativeOutput + '/OriginalImageSourceProbe.exe')
$wpf = 'C:/Windows/Microsoft.NET/Framework64/v4.0.30319/WPF'
$compiler = 'C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe'
& $compiler /nologo /target:exe /platform:x64 /r:System.Web.Extensions.dll /r:System.Core.dll "/r:$wpf/PresentationCore.dll" "/r:$wpf/PresentationFramework.dll" "/r:$wpf/WindowsBase.dll" /r:System.Xaml.dll "/out:$exe" (Join-Path $alias 'tests/NativeEvidence/OriginalImageSourceProbe.cs') (Join-Path $alias 'tests/NativeEvidence/ImageSourceCodecFixtures.cs')
if ($LASTEXITCODE -ne 0) { throw 'Probe compile failed' }
'<configuration><appSettings><add key="StartServerByExe" value="0"/><add key="ServerPath" value=""/><add key="StartClientMode" value="1"/></appSettings><startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1"/></startup></configuration>' | Set-Content -LiteralPath ($exe + '.config') -Encoding utf8
$result = Join-Path $output 'probe.json'
$arguments = @($Applications, $Solution, (Join-Path $workspace 'analysis/images/circle-find-sample.bmp'), $result)
if (@($CaptureSdkUi,$ProbeStitch,$ProbeCodecs,$ProbeFolder).Where({$_}).Count -gt 1) { throw 'UI, stitch, codec and folder probes use separate fixture runs' }
if ($CaptureSdkUi) { $arguments += 'ui' }
if ($ProbeStitch) { $arguments += 'stitch' }
if ($ProbeCodecs) { $arguments += 'codecs' }
if ($ProbeFolder) { $arguments += 'folder' }
$quotedArguments = $arguments | ForEach-Object {
    if ($_.Contains('"')) { throw 'Probe paths cannot contain a quote' }
    '"' + $_ + '"'
}
$process = Start-Process -FilePath $exe -ArgumentList $quotedArguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output 'stdout.log') -RedirectStandardError (Join-Path $output 'stderr.log')
# Exit code and even successful SDK method return are not behavioral success;
# inspect each snapshot's nativeErrorCode, dimensions and pixel hash.
if (!$process.WaitForExit(60000)) {
    # This retained Process object belongs solely to the just-launched probe.
    # Never terminate desktop VM, native server or unrelated task processes.
    $process.Kill()
    @{ phase='process-timeout'; resultFileExists=(Test-Path -LiteralPath $result); originalRuntimeEquivalence=$false } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output 'timeout.json')
}
if (!(Test-Path -LiteralPath $result)) { throw "No probe evidence produced; see $output" }
Write-Output $result
Get-Content -LiteralPath $result -Raw -Encoding utf8 | ConvertFrom-Json | Select-Object phase,nativeErrorCode,sn,name,width,height,length,accepted,scenarioError,error
