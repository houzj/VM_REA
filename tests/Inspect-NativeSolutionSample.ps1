param(
    [Parameter(Mandatory = $true)][string[]]$SamplePath,
    [Parameter(Mandatory = $true)][string]$OutputPath
)

$ErrorActionPreference = 'Stop'
# Audit only: never load native modules, execute XAML, or import into vmx.json.
function Read-U32BigEndian([IO.BinaryReader]$Reader) {
    $bytes = $Reader.ReadBytes(4)
    if ($bytes.Length -ne 4) { throw 'Truncated uint32 field.' }
    return [uint32]([uint64]$bytes[0] * 16777216 + [uint64]$bytes[1] * 65536 + [uint64]$bytes[2] * 256 + $bytes[3])
}

function Read-CandidateUiFields([byte[]]$Bytes) {
    $stream = [IO.MemoryStream]::new($Bytes, $false)
    $reader = [IO.BinaryReader]::new($stream)
    try {
        $marker = [Convert]::ToHexString($reader.ReadBytes(4))
        if ($marker -ne '66553322') { throw 'Unrecognized sample framing marker.' }
        $framingValue = Read-U32BigEndian $reader
        $count = Read-U32BigEndian $reader
        if ($count -gt 65536) { throw 'Field count exceeds audit limit.' }
        $fields = [Collections.Generic.List[object]]::new()
        for ($index = 0; $index -lt $count; $index++) {
            $offset = $stream.Position
            $keyLength = Read-U32BigEndian $reader
            if ($keyLength -gt $stream.Length - $stream.Position) { throw 'Key length exceeds remaining data.' }
            $key = $reader.ReadBytes([int]$keyLength)
            $valueLength = Read-U32BigEndian $reader
            if ($valueLength -gt $stream.Length - $stream.Position) { throw 'Value length exceeds remaining data.' }
            $value = $reader.ReadBytes([int]$valueLength)
            $fields.Add([pscustomobject]@{
                Offset = $offset; KeyHex = [Convert]::ToHexString($key)
                KeyText = [Text.Encoding]::UTF8.GetString($key)
                ValueLength = $valueLength; ValueHex = [Convert]::ToHexString($value)
                ValueSha256 = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($value))
            })
        }
        return [pscustomobject]@{
            EvidenceType = 'independent sample-framing decoder; not a native format contract'
            Marker = $marker; UninterpretedFramingValue = $framingValue
            FieldCount = $count; Fields = $fields.ToArray()
            ConsumedBytes = $stream.Position; TrailingBytes = $stream.Length - $stream.Position
        }
    }
    finally { $reader.Dispose(); $stream.Dispose() }
}

$samples = [Collections.Generic.List[object]]::new()
foreach ($inputPath in $SamplePath) {
    $resolved = (Resolve-Path -LiteralPath $inputPath).Path
    $before = (Get-FileHash -LiteralPath $resolved -Algorithm SHA256).Hash
    $zip = [IO.Compression.ZipFile]::OpenRead($resolved)
    try {
        $entries = [Collections.Generic.List[object]]::new()
        $documentInfo = $null
        foreach ($entry in $zip.Entries) {
            if ($entry.Length -gt 67108864) { throw "Entry exceeds the 64 MiB audit limit: $($entry.FullName)" }
            $stream = $entry.Open()
            try {
                $hash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($stream))
            }
            finally { $stream.Dispose() }
            $item = [ordered]@{
                Name = $entry.FullName; Length = $entry.Length
                CompressedLength = $entry.CompressedLength; Sha256 = $hash
            }
            $normalized = $entry.FullName.Replace('\', '/')
            if ($normalized.EndsWith('/VmServer.xml', [StringComparison]::Ordinal)) {
                $stream = $entry.Open()
                $settings = [Xml.XmlReaderSettings]::new()
                $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
                $settings.XmlResolver = $null
                $settings.MaxCharactersInDocument = 16777216
                $reader = [Xml.XmlReader]::Create($stream, $settings)
                try {
                    $xml = [Xml.XmlDocument]::new(); $xml.XmlResolver = $null; $xml.Load($reader)
                    $versions = @($xml.SelectNodes('//*[@CurrentVersion]') | ForEach-Object { $_.GetAttribute('CurrentVersion') })
                    $documentInfo = [pscustomobject]@{ Versions = $versions; XmlText = $xml.OuterXml }
                }
                finally { $reader.Dispose(); $stream.Dispose() }
            }
            if ($normalized.Contains('/UiParamData/')) {
                if ($entry.Length -gt 16777216) { throw 'UI data exceeds audit limit.' }
                $stream = $entry.Open(); $buffer = [IO.MemoryStream]::new()
                try { $stream.CopyTo($buffer); $bytes = $buffer.ToArray() }
                finally { $buffer.Dispose(); $stream.Dispose() }
                try { $item.UiCandidate = Read-CandidateUiFields $bytes }
                catch { $item.UiCandidate = [pscustomobject]@{ Unsupported = $true; Reason = $_.Exception.Message } }
            }
            $entries.Add([pscustomobject]$item)
        }
        $samples.Add([pscustomobject]@{
            Path = $resolved; Sha256 = $before; EvidenceType = 'read-only shipped sample observation'
            EntryCount = $entries.Count; Entries = $entries.ToArray(); VmServer = $documentInfo
        })
    }
    finally { $zip.Dispose() }
    if ((Get-FileHash -LiteralPath $resolved -Algorithm SHA256).Hash -ne $before) { throw 'Sample changed during audit.' }
}

$destination = [IO.Path]::GetFullPath($OutputPath)
foreach ($sample in $samples) {
    if ($sample.Path.Equals($destination, [StringComparison]::OrdinalIgnoreCase)) { throw 'Report must not overwrite a sample.' }
}
if (!(Test-Path -LiteralPath (Split-Path $destination))) { New-Item -ItemType Directory -Path (Split-Path $destination) -Force | Out-Null }
[IO.File]::WriteAllText($destination, (ConvertTo-Json -InputObject $samples.ToArray() -Depth 24), [Text.UTF8Encoding]::new($false))
Write-Output "Inspected $($samples.Count) samples; original SHA-256 values unchanged. Report: $destination"
