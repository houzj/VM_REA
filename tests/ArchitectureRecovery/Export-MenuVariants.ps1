$ErrorActionPreference='Stop'
$taskRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$taskResourceRoot='D:/Program Files/VisionMaster4.4.0/Applications/Resources'
$taskSpecs=@(
 @{Name='MenuItems.xml';Condition='appData.IsNeutral=false; RemoteOperate.IsRemoteMode=false'},
 @{Name='MenuItemsLinux.xml';Condition='appData.IsNeutral=false; RemoteOperate.IsRemoteMode=true'},
 @{Name='NeutralMenuItems.xml';Condition='appData.IsNeutral=true; RemoteOperate.IsRemoteMode=false'},
 @{Name='NeutralMenuItemsLinux.xml';Condition='appData.IsNeutral=true; RemoteOperate.IsRemoteMode=true'}
)
$taskVariants=[Collections.Generic.List[object]]::new()
foreach($taskSpec in $taskSpecs){
 $taskPath=Join-Path $taskResourceRoot $taskSpec.Name
 $taskReaderSettings=[Xml.XmlReaderSettings]::new()
 $taskReaderSettings.DtdProcessing=[Xml.DtdProcessing]::Prohibit
 $taskReaderSettings.XmlResolver=$null
 $taskReader=[Xml.XmlReader]::Create($taskPath,$taskReaderSettings)
 try{$taskDocument=[Xml.XmlDocument]::new();$taskDocument.XmlResolver=$null;$taskDocument.Load($taskReader)}finally{$taskReader.Dispose()}
 $taskRows=[Collections.Generic.List[object]]::new()
 function Read-MenuNode([Xml.XmlElement]$node,[string]$identity,[string[]]$parents,[bool]$inheritedHidden){
  $fields=[ordered]@{}
  foreach($child in $node.ChildNodes){if($child -is [Xml.XmlElement] -and $child.Name -notin @('items','toolGroupItemInfo')){$fields[$child.Name]=$child.InnerText.Trim()}}
  $nodeName=$fields['name'];$hidden=$inheritedHidden -or $fields['isDisplayInMenu'] -eq 'false'
  $location=@($parents);if($nodeName){$location+=@($nodeName)}elseif($hidden){$location+=@('[hidden-toolbar]')}
  if($nodeName){
   $resolvers=[ordered]@{};foreach($key in @('methodName','dynamicGetItemsMethodName','getCustomItemDataContextMethodName','initMethodName')){if($fields[$key]){$resolvers[$key]=$fields[$key]}}
   $toolbar=[ordered]@{};foreach($child in $node.SelectNodes('toolGroupItemInfo/*')){$toolbar[$child.Name]=$child.InnerText.Trim()}
   [void]$taskRows.Add([pscustomobject][ordered]@{id=$identity;name=$nodeName;sourceType=$node.LocalName;hierarchy=$location;hidden=$hidden;fields=$fields;resolvers=$resolvers;toolbar=$toolbar;traceStatus='open'})
  }
  $index=0;foreach($child in $node.SelectNodes('items/*')){Read-MenuNode $child $(if($identity -eq 'root'){[string]$index}else{"$identity.$index"}) $location $hidden;$index++}
 }
 Read-MenuNode $taskDocument.DocumentElement 'root' @() $false
 [void]$taskVariants.Add([ordered]@{name=$taskSpec.Name;condition=$taskSpec.Condition;path=[IO.Path]::GetFullPath($taskPath);sha256=(Get-FileHash -LiteralPath $taskPath -Algorithm SHA256).Hash.ToLowerInvariant();namedDeclarations=$taskRows.Count;toolbarDeclarations=@($taskRows|Where-Object {$_.toolbar.Count -gt 0}).Count;hiddenDeclarations=@($taskRows|Where-Object {$_.hidden}).Count;entries=@($taskRows)})
}
$taskDefault=$taskVariants[0]
foreach($variant in $taskVariants){$variant['namesAbsentFromDefault']=@($variant.entries|Where-Object {$_.name -notin $taskDefault.entries.name}|Select-Object -ExpandProperty name);$variant['defaultNamesAbsentHere']=@($taskDefault.entries|Where-Object {$_.name -notin $variant.entries.name}|Select-Object -ExpandProperty name)}
$taskData=[ordered]@{schemaVersion=1;generatedUtc=[DateTime]::UtcNow.ToString('o');evidenceType='shipped XML resources parsed with DTD prohibited and no vendor execution';selectionEvidence='analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs::CheckIsNeutral';complete=$false;scope='Four menu resource filenames referenced by CheckIsNeutral; original executable identity, all dynamic entries and effect chains not closed';variants=@($taskVariants)}
$taskOutput=Join-Path $taskRoot 'docs/architecture-recovery/menu-variants.json'
[IO.File]::WriteAllText($taskOutput,($taskData|ConvertTo-Json -Depth 30)+[Environment]::NewLine,[Text.UTF8Encoding]::new($false))
$taskVariants|ForEach-Object {[pscustomobject]$_}|Select-Object name,namedDeclarations,toolbarDeclarations,hiddenDeclarations|ConvertTo-Json
