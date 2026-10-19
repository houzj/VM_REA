param([Parameter(Mandatory = $true)][string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$directory = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($directory) | Out-Null

# Synthetic ZIPs exercise the audit reader only. No vendor assembly is loaded.
$valid = [byte[]]::new(271)
$valid[3] = 5; $valid[7] = 1
[Text.Encoding]::Unicode.GetBytes('fixture.mdata').CopyTo($valid, 8)
$valid[267] = 3; $valid[268] = 1; $valid[269] = 2; $valid[270] = 3
$oversized = [byte[]]$valid.Clone(); $oversized[267] = 4
$excessCount = [byte[]]$valid.Clone(); $excessCount[5] = 1; $excessCount[7] = 1
$trailing = [byte[]]::new(272); $valid.CopyTo($trailing, 0); $trailing[271] = 255
$fixtures = [ordered]@{
    Valid = $valid; TruncatedHeader = [byte[]]@(0, 0, 0)
    TruncatedName = $valid[0..266]; OversizedPayload = $oversized
    ExcessCount = $excessCount; Trailing = $trailing
}
$paths = @()
foreach ($case in $fixtures.GetEnumerator()) {
    $path = Join-Path $directory ($case.Key + '-' + [Guid]::NewGuid().ToString('N') + '.zip')
    $file = [IO.File]::Open($path, [IO.FileMode]::CreateNew)
    $zip = [IO.Compression.ZipArchive]::new($file, [IO.Compression.ZipArchiveMode]::Create)
    try {
        $entry = $zip.CreateEntry('SolutionFile/MoudleFrame'); $stream = $entry.Open()
        try { $stream.Write($case.Value, 0, $case.Value.Length) } finally { $stream.Dispose() }
    } finally { $zip.Dispose(); $file.Dispose() }
    $paths += $path
}
$report = Join-Path $directory 'synthetic-report.json'
& (Join-Path $PSScriptRoot 'Inspect-NativeSolutionSample.ps1') -SamplePath $paths -OutputPath $report
$rows = @(Get-Content -LiteralPath $report -Raw | ConvertFrom-Json)
$frames = @($rows | ForEach-Object { $_.Entries[0].ModuleFrameCandidate })
if ($frames[0].Records[0].Name -ne 'fixture.mdata' -or $frames[0].Records[0].PayloadLength -ne 3 -or
    $frames[0].ConsumedBytes -ne 271 -or $frames[0].TrailingBytes -ne 0) { throw 'Valid fixture mismatch.' }
foreach ($index in 1..4) {
    if (!$frames[$index].Unsupported) { throw "Malformed fixture $index was not rejected." }
}
if ($frames[5].TrailingBytes -ne 1) { throw 'Trailing byte was not reported.' }
Write-Output 'PASS: valid frame, four malformed boundaries, and explicit trailing-byte reporting.'
