#Requires -Version 7.0
[CmdletBinding()]
param(
    [string]$OutputRoot = (Join-Path $PSScriptRoot '../../../analysis/full-audit/batch1-20261107/C02')
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$dotnet = (Get-Command dotnet -CommandType Application).Source
$batch = New-Item -ItemType Directory -Path (Join-Path $OutputRoot "lifecycle-regression-$([Guid]::NewGuid().ToString('N'))") -Force
$artifacts = Join-Path $batch.FullName 'artifacts'
$suites = @(
    @{ name = 'VisionFramework.ExecutionChecks'; minimum = 91; countPattern = '^(\d+) execution checks passed' }
    @{ name = 'VisionFramework.DebugChecks'; minimum = 35; countPattern = '^PASS (\d+) debug runtime checks' }
    @{ name = 'VisionFramework.PersistenceChecks'; minimum = 30; countPattern = '^(\d+) persistence checks passed' }
    @{ name = 'VisionFramework.IntegrationChecks'; minimum = 10; countPattern = '^(\d+) real module graph integration checks passed' }
    @{ name = 'VisionFramework.GroupBreakChecks'; minimum = 57; countPattern = '^PASS (\d+) Group break and accumulation checks' }
    @{ name = 'GroupRoundLifecycleChecks20'; minimum = 58; audit = $true }
    @{ name = 'CallbackQueueChecks22'; minimum = 15; audit = $true }
    @{ name = 'ExecutionOwnerChecks17'; minimum = 10; audit = $true }
    @{ name = 'VisionWorkflow.CallChecks'; minimum = 93; countPattern = '^PASS (\d+) canvas subprogram checks' }
    @{ name = 'ExecutionPolicyChecks11'; minimum = 24; audit = $true; countPattern = '^PASS (\d+)/\1; owned audit:' }
    @{ name = 'VisionConfiguration.ExecutionChecks'; minimum = 39; countPattern = '^(\d+) configuration execution checks passed' }
)

$results = foreach ($suite in $suites) {
    $name = $suite.name
    $directory = New-Item -ItemType Directory -Path (Join-Path $batch.FullName $name)
    $buildLog = Join-Path $directory.FullName 'build.log'
    $runLog = Join-Path $directory.FullName 'run.log'
    $record = [ordered]@{
        project = $name
        buildLog = "$name/build.log"
        runLog = $null
        buildExit = $null
        testExit = $null
        assemblySha256 = $null
        checks = $null
        error = $null
    }
    try {
        $project = Join-Path $repo "tests/$name/$name.csproj"
        & $dotnet build $project -c Release --artifacts-path $artifacts --disable-build-servers -p:UseSharedCompilation=false -m:1 *> $buildLog
        $record.buildExit = $LASTEXITCODE
        if ($record.buildExit -ne 0) { throw "Build exited with $($record.buildExit)." }

        $assembly = @(Get-ChildItem -LiteralPath (Join-Path $artifacts "bin/$name") -Recurse -File -Filter "$name.dll")
        if ($assembly.Count -ne 1) { throw "Expected a single Release assembly for $name." }
        $record.assemblySha256 = (Get-FileHash -LiteralPath $assembly[0].FullName).Hash
        $runArgs = @()
        if ($suite.audit) { $runArgs += $directory.FullName }
        if ($name -eq 'VisionFramework.IntegrationChecks') {
            $runArgs += (Join-Path $repo 'examples/architecture-demo.vmx.json')
        }
        if ($name -eq 'VisionWorkflow.CallChecks') {
            $imageDirectory = New-Item -ItemType Directory -Path (Join-Path $directory.FullName 'analysis/images') -Force
            Copy-Item -LiteralPath (Join-Path $repo 'analysis/images/circle-find-sample.bmp') -Destination $imageDirectory.FullName
        }

        # Several suites write relative to cwd, including a generated example solution.
        Push-Location $directory.FullName
        try {
            $record.runLog = "$name/run.log"
            & $dotnet $assembly[0].FullName @runArgs *> $runLog
            $record.testExit = $LASTEXITCODE
        } finally {
            Pop-Location
        }
        if ($record.testExit -ne 0) { throw "Tests exited with $($record.testExit)." }

        if ($suite.countPattern) {
            $match = [regex]::Match((Get-Content -Raw -LiteralPath $runLog), "(?m)$($suite.countPattern)")
            if (!$match.Success) { throw 'Test log lacks its completion count.' }
            $record.checks = [int]$match.Groups[1].Value
        } else {
            $audit = @(Get-ChildItem -LiteralPath $directory.FullName -Recurse -File -Filter summary.json)
            if ($audit.Count -ne 1) { throw 'Expected a single suite audit summary.' }
            $record.checks = (Get-Content -Raw -LiteralPath $audit[0].FullName | ConvertFrom-Json).passed
        }
        if ($record.checks -lt $suite.minimum) { throw "Observed $($record.checks) checks, expected at least $($suite.minimum)." }
        Write-Host "PASS $name ($($record.checks) checks)"
    } catch {
        $record.error = $_.Exception.Message
        Write-Host "FAIL $name $($record.error)"
    }
    [pscustomobject]$record
}

$passed = @($results | Where-Object { !$_.error })
$summary = Join-Path $batch.FullName 'summary.json'
[ordered]@{
    scope = 'C02 managed Release lifecycle regression, headless'
    runnerSha256 = (Get-FileHash -LiteralPath $PSCommandPath).Hash
    passedSuites = $passed.Count
    passedChecks = ($passed | Measure-Object -Property checks -Sum).Sum
    suites = @($results)
} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summary -Encoding utf8
if ($passed.Count -ne $suites.Count) { throw "C02 regression failed. See $summary" }
Write-Output $summary
