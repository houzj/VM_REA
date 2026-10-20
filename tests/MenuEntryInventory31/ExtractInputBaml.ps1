param([string]$OutputDirectory = 'analysis/full-audit/batch31-20261020/A05.1/baml')
$ErrorActionPreference = 'Stop'
$source = 'D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll'
$before = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
$assembly = [Reflection.Assembly]::ReflectionOnlyLoadFrom($source)
$stream = $assembly.GetManifestResourceStream('VMControls.WPF.g.resources')
if ($null -eq $stream) { throw 'Original resource table missing' }
$reader = New-Object System.Resources.ResourceReader($stream)
$directory = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($directory) | Out-Null
$entries = @()
try {
    $keys = @('globalui/globalcommunication.baml', 'base/commentboxstyle.baml')
    $enumerator = $reader.GetEnumerator()
    $available = New-Object System.Collections.Generic.List[string]
    while ($enumerator.MoveNext()) { $available.Add([string]$enumerator.Key) }
    $matches = @($available | Where-Object { $_ -match 'globalcommunication|commentboxstyle' })
    $matches | Set-Content -Encoding UTF8 (Join-Path $directory 'matching-resource-keys.txt')
    foreach ($match in $matches) { if ($keys -notcontains $match) { $keys += $match } }
    foreach ($key in $keys) {
        if (-not $available.Contains($key)) {
            $entries += [pscustomobject]@{Key=$key;Status='not-found'}
            continue
        }
        [string]$kind = $null
        [byte[]]$bytes = $null
        $reader.GetResourceData($key, [ref]$kind, [ref]$bytes)
        $file = Join-Path $directory (($key -replace '/', '_') + '.resource-data.bin')
        [IO.File]::WriteAllBytes($file, $bytes)
        $entries += [pscustomobject]@{Key=$key;Status='extracted';ResourceType=$kind;Length=$bytes.Length;SHA256=(Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash;Path=$file}
    }
} finally {
    $reader.Dispose()
    $stream.Dispose()
}
$after = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
if ($before -ne $after) { throw 'Original assembly hash changed' }
[pscustomobject]@{OriginalPath=$source;OriginalSHA256=$before;MVID=$assembly.ManifestModule.ModuleVersionId;Entries=$entries;Method='ReflectionOnly resource extraction; no vendor code executed'} |
    ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 (Join-Path $directory 'manifest.json')
$entries | Format-Table Key,Status,Length,SHA256 -AutoSize
