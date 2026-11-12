#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Image,
    [string]$SdkRoot = 'C:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK',
    [string]$OutputRoot = (Join-Path $PSScriptRoot '../../../analysis/full-audit/batch1-20261107/C03')
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false

function Get-Sha256([string]$File) {
    (Get-FileHash -LiteralPath $File -Algorithm SHA256).Hash
}

function Assert-Identity([string]$File, [string]$Expected) {
    $actual = Get-Sha256 $File
    if ($actual -ne $Expected) {
        throw "Identity mismatch for $File. Expected $Expected, got $actual."
    }
}

$baselinePath = Join-Path $PSScriptRoot 'sdk-region-baseline.json'
$baseline = Get-Content -Raw -LiteralPath $baselinePath | ConvertFrom-Json -AsHashtable
$imagePath = (Resolve-Path -LiteralPath $Image).Path
$sdk = (Resolve-Path -LiteralPath $SdkRoot).Path
$runtime = Join-Path $sdk 'Runtime/x64'
$windowsPrefix = $env:SystemRoot.TrimEnd('\') + '\'
Assert-Identity $imagePath $baseline.inputSha256
foreach ($dll in $baseline.runtimeDlls.Keys) {
    Assert-Identity (Join-Path $runtime $dll) $baseline.runtimeDlls[$dll]
}
$sdkDllNames = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($file in Get-ChildItem -LiteralPath $runtime -Filter '*.dll' -File) {
    [void]$sdkDllNames.Add($file.Name)
}
$compiler = Get-Command cl.exe -CommandType Application -ErrorAction SilentlyContinue
if (!$compiler) { throw 'Run this script from an x64 Visual Studio Developer PowerShell.' }

$out = New-Item -ItemType Directory -Path (Join-Path $OutputRoot "sdk-region-matrix-$([Guid]::NewGuid().ToString('N'))")
$source = Join-Path $PSScriptRoot 'OriginalSdkCppProbe.cpp'
$exe = Join-Path $out.FullName 'probe.exe'
$buildLog = Join-Path $out.FullName 'compile.log'
$input = Join-Path $out.FullName 'input.jpg'
Copy-Item -LiteralPath $imagePath -Destination $input
Assert-Identity $input $baseline.inputSha256
# The probe uses its fixed default parameters when the override file is empty.
[IO.File]::WriteAllText((Join-Path $out.FullName 'params.txt'), '')
$libs = @(
    'Libraries/x64/Algorithms/MVDBlobFindCpp.lib'
    'Libraries/x64/Algorithms/MVDGlobalInitCpp.lib'
    'Libraries/x64/Algorithms/MVDPreproMaskCpp.lib'
    'Libraries/x64/Common/MVDImageCpp.lib'
    'Libraries/x64/Common/MVDShapeCpp.lib'
) | ForEach-Object { Join-Path $sdk $_ }
$buildArgs = @(
    '/nologo', '/EHsc', '/O2', '/MD'
    "/I$(Join-Path $sdk 'Includes')"
    "/I$(Join-Path $sdk 'Includes/Common/VisionDesigner')"
    "/Fo$(Join-Path $out.FullName 'probe.obj')"
    "/Fe$exe"
    $source
) + $libs + @('/link', '/MACHINE:X64')
& $compiler.Source @buildArgs *> $buildLog
if ($LASTEXITCODE -ne 0) { throw "Probe compilation failed. See $buildLog" }

$failures = [Collections.Generic.List[string]]::new()
$memoryReference = [ordered]@{
    'default-memory' = 'default'
    'full-roi-memory' = 'full-roi'
}
$modes = @($baseline.modes.Keys) + @($memoryReference.Keys)
$cases = foreach ($mode in $modes) {
    @{ mode = $mode; flags = '11' }
    if ($mode -in 'default', 'full-roi') {
        foreach ($flags in '10', '01', '00') { @{ mode = $mode; flags = $flags } }
    }
}
$runs = foreach ($case in $cases) {
    $mode = $case.mode
    $flags = $case.flags
    $referenceMode = $memoryReference[$mode]
    $expected = $baseline.modes[($referenceMode ?? $mode)]
    $expectedBuffers = $expected.buffers.Clone()
    if ($flags[0] -eq '0') { $expectedBuffers.Remove('binary') }
    if ($flags[1] -eq '0') { $expectedBuffers.Remove('blob') }
    foreach ($attempt in 1, 2) {
        $runName = "$mode-$attempt"
        if ($flags -ne '11') { $runName = "$mode-flags$flags-$attempt" }
        $runDir = New-Item -ItemType Directory -Path (Join-Path $out.FullName $runName)
        $start = [Diagnostics.ProcessStartInfo]::new($exe)
        $start.WorkingDirectory = $out.FullName
        $start.UseShellExecute = $false
        $start.CreateNoWindow = $true
        $start.RedirectStandardOutput = $true
        $start.RedirectStandardError = $true
        $start.Environment['PATH'] = "$runtime;$env:PATH"
        $inputArgument = 'input.jpg'
        $initialization = 'file'
        if ($referenceMode) {
            $inputArgument = "$referenceMode-$attempt/input.bin"
            $initialization = 'memory'
            $rawInput = Join-Path $out.FullName $inputArgument
            if (!(Test-Path -LiteralPath $rawInput -PathType Leaf) -or
                (Get-Sha256 $rawInput) -ne $baseline.decodedInputSha256) {
                $failures.Add("$runName reference pixels are missing or differ from the archive")
                continue
            }
        }
        foreach ($arg in $inputArgument, $runName, 'params.txt', $mode, $flags) {
            $start.ArgumentList.Add($arg)
        }
        $process = [Diagnostics.Process]::Start($start)
        try {
            $stdout = $process.StandardOutput.ReadToEndAsync()
            $stderr = $process.StandardError.ReadToEndAsync()
            $timedOut = !$process.WaitForExit(30000)
            if ($timedOut) { $process.Kill(); $process.WaitForExit() }
            $log = $stdout.GetAwaiter().GetResult()
            $logPath = Join-Path $runDir.FullName 'probe.log'
            [IO.File]::WriteAllText($logPath, $log)
            [IO.File]::WriteAllText((Join-Path $runDir.FullName 'stderr.log'), $stderr.GetAwaiter().GetResult())
            $exitCode = $process.ExitCode
        } finally {
            $process.Dispose()
        }

        $lines = $log -split '\r?\n'
        $result = @($lines | Where-Object { $_ -match '^result count=' })
        $vector = ($lines | Where-Object { $_ -match '^(blob|projected-angle)\[' }) -join "`n"
        $vectorHash = [Convert]::ToHexString(
            [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($vector)))
        if ($timedOut -or $exitCode -ne $expected.exitCode) {
            $failures.Add("$runName exit $exitCode, expected $($expected.exitCode), timedOut=$timedOut")
        }
        if ($lines -notcontains "inputInitialization=$initialization" -or
            $lines -notcontains "outputFlags=$flags" -or
            $lines -notcontains 'image=1280 x 960 fmt=1' -or
            $lines -notcontains 'inputDataSize=1228800 inputRowStep=1280' -or
            $log -notmatch '\{"global":0,"create":0,') {
            $failures.Add("$runName input or initialization differs")
        }
        if ($null -eq $expected.result) {
            if ($result.Count -ne 0 -or $log -notmatch '(?m)^exception stage=run-blob ') {
                $failures.Add("$runName expected a Run-stage exception and no result")
            }
        } elseif ($result.Count -ne 1 -or
            $result[0] -ne "result count=$($expected.result[0]) total=$($expected.result[1])") {
            $failures.Add("$runName Blob count or total area differs")
        }
        if ($vectorHash -ne $expected.vectorSha256) {
            $failures.Add("$runName result vector or angle projection differs")
        }

        $moduleManifest = Join-Path $runDir.FullName 'loaded-modules.txt'
        $loadedSdk = [ordered]@{}
        $moduleManifestHash = $null
        if ($lines -notcontains 'loadedModulesCaptured=1' -or !(Test-Path -LiteralPath $moduleManifest -PathType Leaf)) {
            $failures.Add("$runName loaded module snapshot is missing or incomplete")
        } else {
            $moduleManifestHash = Get-Sha256 $moduleManifest
            foreach ($path in Get-Content -LiteralPath $moduleManifest -Encoding utf8) {
                $name = [IO.Path]::GetFileName($path)
                $fromRuntime = [IO.Path]::GetDirectoryName($path) -eq $runtime
                if (!$fromRuntime -and !$sdkDllNames.Contains($name) -and $name -notlike 'MVD*.dll') { continue }
                if ($path.StartsWith($windowsPrefix, [StringComparison]::OrdinalIgnoreCase) -and
                    $name -match '^(?:msvc[pr]|vcruntime|vcomp)\d+(?:_\d+)?\.dll$') { continue }
                try {
                    $loadedSdk[$path] = Get-Sha256 $path
                } catch {
                    $failures.Add("$runName cannot hash loaded SDK file $path")
                    continue
                }
                if (!$fromRuntime) {
                    $failures.Add("$runName loaded SDK file outside the selected runtime: $path")
                }
            }
            foreach ($dll in $baseline.runtimeDlls.Keys) {
                $expectedPath = Join-Path $runtime $dll
                if ($loadedSdk[$expectedPath] -ne $baseline.runtimeDlls[$dll]) {
                    $failures.Add("$runName loaded identity differs or is missing: $expectedPath")
                }
            }
        }

        $buffers = [ordered]@{}
        foreach ($file in Get-ChildItem -LiteralPath $runDir.FullName -Filter '*.bin' -File) {
            $key = $file.BaseName
            $buffers[$key] = @{ bytes = $file.Length; sha256 = Get-Sha256 $file.FullName }
            if ($key -eq 'input') { continue }
            $reference = $expectedBuffers[$key]
            if (!$reference -or $file.Length -ne $reference.bytes -or $buffers[$key].sha256 -ne $reference.sha256) {
                $failures.Add("$runName buffer $key differs")
            }
        }
        foreach ($key in $expectedBuffers.Keys) {
            if (!$buffers.Contains($key)) { $failures.Add("$runName buffer $key is missing") }
        }
        if ($buffers.input.bytes -ne 1228800 -or $buffers.input.sha256 -ne $baseline.decodedInputSha256) {
            $failures.Add("$runName SDK input buffer differs from the archived decoded pixels")
        }
        [ordered]@{
            mode = $mode
            attempt = $attempt
            outputFlags = $flags
            initialization = $initialization
            input = $inputArgument
            exitCode = $exitCode
            timedOut = $timedOut
            log = "$runName/probe.log"
            logSha256 = Get-Sha256 $logPath
            result = $result
            vectorSha256 = $vectorHash
            loadedModuleManifest = "$runName/loaded-modules.txt"
            loadedModuleManifestSha256 = $moduleManifestHash
            loadedSdkDlls = $loadedSdk
            buffers = $buffers
        }
    }
}

$report = [ordered]@{
    scope = 'Headless direct SDK. No VisionMaster host execution.'
    baselineSha256 = Get-Sha256 $baselinePath
    sourceSha256 = Get-Sha256 $source
    executableSha256 = Get-Sha256 $exe
    inputPath = $imagePath
    inputSha256 = Get-Sha256 $input
    decodedInputSha256 = $baseline.decodedInputSha256
    sdkRoot = $sdk
    verifiedRuntimeDlls = $baseline.runtimeDlls
    parameters = $baseline.parameters
    runs = @($runs)
    failures = @($failures.ToArray())
}
$summary = Join-Path $out.FullName 'summary.json'
$report | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $summary -Encoding utf8
if ($failures.Count -ne 0) { throw "$($failures.Count) discrepancies. See $summary" }
"All $($report.runs.Count) SDK runs match the baseline. $summary"
