$ErrorActionPreference='Stop'
$taskRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$taskFiles=@(& rg --files (Join-Path $taskRoot 'analysis/decompiled') -g '*.xaml' -g '!**/obj/**' -g '!**/bin/**' | Sort-Object)
if($LASTEXITCODE -ne 0){throw 'XAML source enumeration failed'}
$taskRows=[Collections.Generic.List[object]]::new()
$taskManifest=[Collections.Generic.List[object]]::new()
$taskIgnored=@{}
function Get-EntryKind([string]$name){
 if($name -eq 'KeyTime'){return 'animation-key-time-property'}
 if($name -eq 'EventName'){return 'event-trigger'}
 if($name -match '(^|[.:])(Command|Executed|CanExecute)$' -or $name -match 'Command$'){return 'command-binding'}
 if($name -match '^(Gesture|Modifiers|Key|MouseAction)$'){return 'input-binding'}
 if($name -match '^(Preview)?(Mouse\w+|Key\w+|TextInput\w*|Drag\w+|Drop|GiveFeedback|QueryContinueDrag)$'){return 'input-event'}
 if($name -match '^(Click|Checked|Unchecked|Loaded|Unloaded|Closing|Closed|SelectionChanged|ValueChanged|SizeChanged|DataContextChanged|SourceUpdated|TargetUpdated|ScrollChanged|GotFocus|LostFocus|Initialized|Expanded|Collapsed)$'){return 'ui-lifecycle-event'}
 return $null
}
foreach($taskPath in $taskFiles){
 $taskRelative=[IO.Path]::GetRelativePath($taskRoot,$taskPath).Replace('\','/')
 $taskBytes=[IO.File]::ReadAllBytes($taskPath)
 $taskDigest=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($taskBytes)).ToLowerInvariant()
 $taskStatus='parsed';$taskError=$null;$taskElementCount=0;$taskAttributeCount=0;$taskBefore=$taskRows.Count
 $taskSettings=[Xml.XmlReaderSettings]::new();$taskSettings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$taskSettings.XmlResolver=$null
 $taskStream=[IO.MemoryStream]::new($taskBytes,$false)
 $taskReader=$null
 try{
  $taskReader=[Xml.XmlReader]::Create($taskStream,$taskSettings)
  $taskDocument=[Xml.Linq.XDocument]::Load($taskReader,[Xml.Linq.LoadOptions]::SetLineInfo)
  $taskClass=$taskDocument.Root.Attribute([Xml.Linq.XName]::Get('Class','http://schemas.microsoft.com/winfx/2006/xaml'))
  foreach($taskElement in $taskDocument.Descendants()){
   $taskElementCount++
   $taskElementLine=[Xml.IXmlLineInfo]$taskElement
   $taskContext=[ordered]@{}
   foreach($taskAttribute in $taskElement.Attributes()){
    $taskAttributeCount++
    if($taskAttribute.Name.LocalName -in @('Name','Key','EventName','Command','CommandParameter','Gesture','Modifiers','MouseAction','Property','Value','Event','Handler','TargetName','TargetType')){$taskContext[$taskAttribute.Name.ToString()]=$taskAttribute.Value}
   }
   $taskAncestors=@($taskElement.Ancestors() | ForEach-Object {$_.Name.ToString()});[Array]::Reverse($taskAncestors)
   $taskContainers=@($taskElement.Ancestors() | Where-Object {$_.Name.LocalName -in @('Style','ControlTemplate','DataTemplate','HierarchicalDataTemplate','Trigger','DataTrigger','MultiTrigger','MultiDataTrigger','Condition')} | ForEach-Object {
    $taskContainerAttributes=[ordered]@{};foreach($taskContainerAttribute in $_.Attributes()){if(!$taskContainerAttribute.IsNamespaceDeclaration){$taskContainerAttributes[$taskContainerAttribute.Name.ToString()]=$taskContainerAttribute.Value}}
    [pscustomobject]@{element=$_.Name.ToString();line=([Xml.IXmlLineInfo]$_).LineNumber;attributes=$taskContainerAttributes}
   })
   $taskElementKind=$null;$taskElementTarget=$null;$taskElementValue=$null
   if($taskElement.Name.NamespaceName -eq 'http://schemas.microsoft.com/winfx/2006/xaml/presentation' -and $taskElement.Name.LocalName -eq 'EventSetter'){
    $taskEvent=$taskElement.Attribute('Event');$taskHandler=$taskElement.Attribute('Handler')
    if($taskEvent -and $taskHandler){$taskElementKind='event-setter';$taskElementTarget=$taskEvent.Value;$taskElementValue=$taskHandler.Value}
   }
   if($taskElement.Name.NamespaceName -eq 'http://schemas.microsoft.com/winfx/2006/xaml/presentation' -and $taskElement.Name.LocalName -eq 'Setter'){
    $taskProperty=$taskElement.Attribute('Property')
    if($taskProperty -and $taskProperty.Value -match '(^|[.:])(\w*Command)$'){
     $taskElementKind='setter-command-binding';$taskElementTarget=$taskProperty.Value;$taskValue=$taskElement.Attribute('Value')
     $taskElementValue=$(if($taskValue){$taskValue.Value}else{($taskElement.Elements() | ForEach-Object {$_.ToString([Xml.Linq.SaveOptions]::DisableFormatting)}) -join ''})
    }
   }
   if($taskElementKind){
    [void]$taskRows.Add([pscustomobject][ordered]@{
     id="xaml-element:$taskRelative`:$($taskElementLine.LineNumber):$($taskElementLine.LinePosition)";path=$taskRelative;sha256=$taskDigest
     line=$taskElementLine.LineNumber;column=$taskElementLine.LinePosition;elementLine=$taskElementLine.LineNumber;elementColumn=$taskElementLine.LinePosition
     kind=$taskElementKind;target=$taskElementTarget;namespace=$taskElement.Name.NamespaceName;value=$taskElementValue
     element=$taskElement.Name.ToString();ancestors=$taskAncestors;containers=$taskContainers;rootClass=$(if($taskClass){$taskClass.Value}else{$null});context=$taskContext
     evidence='xml-element-declaration';bindingConfirmed=$false;fullBehaviorClosed=$false
    })
   }
   foreach($taskAttribute in $taskElement.Attributes()){
    if($taskAttribute.IsNamespaceDeclaration){continue}
    $taskName=$taskAttribute.Name.LocalName
    # x:Key declares a resource identity; it is not KeyBinding.Key.
    if($taskName -eq 'Key' -and $taskAttribute.Name.NamespaceName -eq 'http://schemas.microsoft.com/winfx/2006/xaml'){
     $taskIgnored['{http://schemas.microsoft.com/winfx/2006/xaml}Key']=1+[int]$taskIgnored['{http://schemas.microsoft.com/winfx/2006/xaml}Key'];continue
    }
    $taskKind=Get-EntryKind $taskName
    if(!$taskKind){$taskIgnored[$taskName]=1+[int]$taskIgnored[$taskName];continue}
    $taskLine=[Xml.IXmlLineInfo]$taskAttribute
    [void]$taskRows.Add([pscustomobject][ordered]@{
     id="xaml:$taskRelative`:$($taskLine.LineNumber):$($taskLine.LinePosition)";path=$taskRelative;sha256=$taskDigest
     line=$taskLine.LineNumber;column=$taskLine.LinePosition;elementLine=$taskElementLine.LineNumber;elementColumn=$taskElementLine.LinePosition
     kind=$taskKind;target=$taskName;namespace=$taskAttribute.Name.NamespaceName;value=$taskAttribute.Value
     element=$taskElement.Name.ToString();ancestors=$taskAncestors;containers=$taskContainers;rootClass=$(if($taskClass){$taskClass.Value}else{$null});context=$taskContext
     evidence='xml-attribute-declaration';bindingConfirmed=$false;fullBehaviorClosed=$false
    })
   }
  }
 }catch{$taskStatus='parse-failed';$taskError=$_.Exception.Message}
 finally{if($taskReader){$taskReader.Dispose()};$taskStream.Dispose()}
 [void]$taskManifest.Add([pscustomobject]@{path=$taskRelative;sha256=$taskDigest;status=$taskStatus;error=$taskError;elements=$taskElementCount;attributes=$taskAttributeCount;candidates=$taskRows.Count-$taskBefore})
}
$taskOutput=[pscustomobject][ordered]@{
 schemaVersion=1;generatedUtc=[DateTime]::UtcNow.ToString('o');complete=$false
 parser='System.Xml.Linq text-only XDocument with line information; DTD prohibited, XmlResolver null; no XAML object construction'
 scope='Existing analysis/decompiled .xaml only; excludes obj/bin; no BAML extraction, target execution, native or dynamic registration'
 counts=[pscustomobject]@{files=$taskFiles.Count;parsed=@($taskManifest | Where-Object status -eq 'parsed').Count;failed=@($taskManifest | Where-Object status -eq 'parse-failed').Count;candidates=$taskRows.Count}
 limitations=@('Recognized attribute names are a candidate heuristic, not a complete WPF event/command registry.','Markup extensions and templates are text; actual binding type, DataContext, resource lookup and execution remain unverified.','XDocument preserves XML attribute order, but XML text order does not replace original BAML order evidence.','Unrecognized attribute names and parse failures are retained for follow-up.')
 files=$taskManifest.ToArray();candidates=$taskRows.ToArray();unrecognizedAttributeNames=$taskIgnored
}
$taskOutputPath=Join-Path $taskRoot 'docs/architecture-recovery/xaml-entry-inventory.json'
$taskTemporaryPath=$taskOutputPath+'.'+[Guid]::NewGuid().ToString('N')+'.tmp'
[IO.File]::WriteAllText($taskTemporaryPath,($taskOutput | ConvertTo-Json -Depth 16),[Text.UTF8Encoding]::new($false))
[IO.File]::Move($taskTemporaryPath,$taskOutputPath,$true)
$taskOutput.counts | ConvertTo-Json -Compress
