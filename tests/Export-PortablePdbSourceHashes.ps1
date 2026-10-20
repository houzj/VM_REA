#Requires -Version 7.0
param([Parameter(Mandatory)][string[]]$Pdb,[Parameter(Mandatory)][string]$Output)
$ErrorActionPreference='Stop'
$rows=@()
foreach($item in $Pdb){
 $path=(Resolve-Path -LiteralPath $item).Path
 $stream=[IO.File]::OpenRead($path)
 $provider=$null
 try{
  $provider=[System.Reflection.Metadata.MetadataReaderProvider]::FromPortablePdbStream($stream)
  $reader=$provider.GetMetadataReader()
  foreach($handle in $reader.Documents){
   $document=$reader.GetDocument($handle)
   $rows+=[pscustomobject]@{pdb=$path;path=$reader.GetString($document.Name);compiledSha256=[Convert]::ToHexString($reader.GetBlobBytes($document.Hash))}
  }
 }finally{if($null -ne $provider){$provider.Dispose()};$stream.Dispose()}
}
$target=[IO.Path]::GetFullPath($Output)
$parent=[IO.Path]::GetDirectoryName($target)
if(-not [IO.Directory]::Exists($parent)){[IO.Directory]::CreateDirectory($parent)|Out-Null}
[IO.File]::WriteAllText($target,(ConvertTo-Json -InputObject @($rows) -Depth 4),[Text.UTF8Encoding]::new($false))
Write-Output "Documents: $($rows.Count)"
Get-FileHash -LiteralPath $target -Algorithm SHA256
