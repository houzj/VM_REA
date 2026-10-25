# Current scope retirement probe

This console probe references the previously built GroupDataProbe copies of VisionFramework.dll and VisionData.dll through HintPath. It has no ProjectReference and does not build product projects. Twelve checks use synthetic disposable resources and a throwing cancellation callback to observe partial service replacement, failed node removal and batch retention, closing with an outstanding lease, and once-only root cleanup. Read-only reflection inspects ownership and failed-resource holders.

The report binds loaded DLL/PDB hashes, MVID and CodeView identity, and verifies all PortablePDB source checksums. The audit deliberately keeps object references; it proves neither GC timing nor final native/pixel release. No vendor program, real tool/camera resource, GUI, device, concurrency race or original-program parity is tested.

Build only this project with `dotnet build .../ScopeRetirementProbe.csproj --artifacts-path .../ScopeRetirementProbe/artifacts --ignore-failed-sources -p:RestoreSources= -p:NuGetAudit=false -p:UseSharedCompilation=false`. Run its resulting DLL with workspace and report paths. Preserve the build/runtime logs beside this project; the resulting evidence is `docs/architecture-recovery/current-scope-retirement-probe.json`.
