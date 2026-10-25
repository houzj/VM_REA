param([string]$Root = (Join-Path $PSScriptRoot '../..'), [string]$Source = 'D:/Program Files/VisionMaster4.4.0/Applications/CompatibleTool.xml')
$ErrorActionPreference = 'Stop'
$rootPath = [IO.Path]::GetFullPath($Root)
$sourcePath = [IO.Path]::GetFullPath($Source)
$bytes = [IO.File]::ReadAllBytes($sourcePath)
$settings = [Xml.XmlReaderSettings]::new()
$settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
$settings.XmlResolver = $null
$reader = [Xml.XmlReader]::Create($sourcePath, $settings)
try { $document = [Xml.Linq.XDocument]::Load($reader, [Xml.Linq.LoadOptions]::SetLineInfo) } finally { $reader.Dispose() }
if ($document.Root.Name.ToString() -ne 'InfoList') { throw 'Unexpected compatibility resource root' }
$list = $document.Root.Element([Xml.Linq.XName]::Get('ModuleInfoList'))
if ($null -eq $list) { throw 'Missing ModuleInfoList' }
$modules = @($list.Elements([Xml.Linq.XName]::Get('ModuleInfo')) | ForEach-Object {
    $node = $_
    $type = $node.Element([Xml.Linq.XName]::Get('TypeName'))
    if ($null -eq $type) { throw 'ModuleInfo without TypeName' }
    $io = $node.Element([Xml.Linq.XName]::Get('IOItems'))
    $items = @()
    if ($null -ne $io) { $items = @($io.Elements([Xml.Linq.XName]::Get('IOInfo')) | ForEach-Object {
        $srcAttribute = $_.Attribute([Xml.Linq.XName]::Get('SrcName'))
        $dstAttribute = $_.Attribute([Xml.Linq.XName]::Get('DstName'))
        [ordered]@{ srcName = $(if ($null -eq $srcAttribute) { $null } else { $srcAttribute.Value }); dstName = $(if ($null -eq $dstAttribute) { $null } else { $dstAttribute.Value }); line = ([Xml.IXmlLineInfo]$_).LineNumber }
    }) }
    [ordered]@{ typeName = $type.Value; line = ([Xml.IXmlLineInfo]$node).LineNumber; renames = $items }
})
$renameCount = ($modules | ForEach-Object { $_.renames.Count } | Measure-Object -Sum).Sum
$model = [ordered]@{ schemaVersion = 1; complete = $false; source = [ordered]@{ path = $sourcePath; sha256 = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant(); root = $document.Root.Name.ToString() }; parser = 'XMLReader: DTD prohibited, resolver null; XDocument line info; no vendor code execution'; scope = 'Ordered ModuleInfo/IOInfo name mappings only; no runtime reachability or full schema equivalence'; counts = [ordered]@{ modules = $modules.Count; renames = [int]$renameCount }; modules = $modules }
$out = Join-Path $rootPath 'docs/architecture-recovery/variable-migration-resource.json'
[IO.File]::WriteAllText($out, ($model | ConvertTo-Json -Depth 15) + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
$model.counts | ConvertTo-Json -Compress
