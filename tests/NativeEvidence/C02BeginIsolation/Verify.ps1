param([Parameter(Mandatory=$true)][string]$EvidenceDirectory)
$ErrorActionPreference='Stop'
$core=Get-Content (Join-Path $EvidenceDirectory 'VM.Core.dll.cil-methods.json') -Raw|ConvertFrom-Json
$sdk=Get-Content (Join-Path $EvidenceDirectory 'VM.PlatformSDKCS.dll.cil-methods.json') -Raw|ConvertFrom-Json
$cil=Get-Content (Join-Path $EvidenceDirectory 'VM.Core.dll.cil-disassembly.txt') -Raw
$sdkcil=Get-Content (Join-Path $EvidenceDirectory 'VM.PlatformSDKCS.dll.cil-disassembly.txt') -Raw
$gates=@()
function Gate($name,$ok){if(!$ok){throw "Static prerequisite changed: $name"}; $script:gates+=@{name=$name;matched=$true}}
Gate 'Core identity' ($core.sha256 -eq '936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d')
Gate 'SDK identity' ($sdk.sha256 -eq '34c0c90b00cb75a701685b96670b6490f61376f62eacc0ab29d240157db401c1')
Gate 'Solution explicit cctor' (@($core.typeInitialization|Where-Object {$_.type -eq 'VM.Core.VmSolution' -and !$_.beforeFieldInit -and $_.typeInitializerToken -eq '0x060000AC'}).Count -eq 1)
Gate 'Procedure all three constructors load Solution.MaxModuleCount' ([regex]::Matches($cil,'IL_001D: ldsfld 0x04000020 VM.Core.VmSolution::Int32 MaxModuleCount').Count -eq 3)
Gate 'Solution resolves external current path' ($cil.Contains('VM.Utility.SingleInstance.CurrentPathHelper::System.String GetCurrentPath()'))
Gate 'Solution starts thread' ($cil.Contains('IL_0255: callvirt 0x0A00020E System.Threading.Thread::Void Start()'))
Gate 'SDK handle creation boundary' ($sdkcil.Contains('IL_0166: call 0x060002A4 VM.PlatformSDKCS.ServerSDKAPI::Int32 IMVS_CreateHandle'))
Gate 'SDK global script boundary' ($sdkcil.Contains('IL_0267: call 0x06000335 VM.PlatformSDKCS.ServerSDKAPI::Int32 IMVS_StartGlobalScript'))
@{authority='static metadata and CIL only';targetExecution=$false;checks=$gates;actualBeginPermitted=$false;reason='Explicit initialization requires non-isolated path, threads and SDK service; no runtime attempt authorized by this verifier'}|ConvertTo-Json -Depth 5|Set-Content (Join-Path $EvidenceDirectory 'isolation-gates.json')
Write-Output "Matched $($gates.Count) static prerequisite checks; no target code executed; actual Begin remains blocked"

