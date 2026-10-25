# Finite current GraphExecutor transport observations

This test-only console runs synthetic source/consumer modules through the actual current `VisionFramework.GraphExecutor`. It does not use substitute framework transport or execute vendor code, tool algorithms, UI or devices.

```powershell
dotnet build tests/ArchitectureRecovery/NonImageGraphProbe/NonImageGraphProbe.csproj --artifacts-path tests/ArchitectureRecovery/NonImageGraphProbe/artifacts --ignore-failed-sources -p:NuGetAudit=false -p:RestoreSources= -p:UseSharedCompilation=false -v:minimal
dotnet tests/ArchitectureRecovery/NonImageGraphProbe/artifacts/bin/NonImageGraphProbe/debug/NonImageGraphProbe.dll D:/MyWork/REA逆向测试 D:/MyWork/REA逆向测试/docs/architecture-recovery/current-nonimage-graph-probe.json
```

Nine finite checks observe shared TextArray PortValue references across node records, inputs and workflow output/display projections; dictionary copying; source parameter snapshot isolation; module disposal before downstream execution; actual InputTypeMismatch before consumer creation for Integer arrays; scalar float promotion; bound variable retention; and throwing-source disposal/consumer suppression. WorkflowDisplays is a returned data structure, not evidence of renderer or history execution. Module Dispose counters do not prove payload or native-resource release.

Every loaded assembly is bound to its PDB and every compiled source checksum. `verify-current-nonimage-graph-probe.cjs` independently verifies the retained report and raw artifact identities. Outputs stay in this probe's exclusive artifacts directory. The older 13-method probe remains separate.
