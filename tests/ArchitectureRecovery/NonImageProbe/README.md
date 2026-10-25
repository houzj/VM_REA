# Current nonimage transport probe

This console references the real current `VisionFramework` project and its `VisionData` dependency. It does not include a substitute transport implementation, vendor assemblies, GUI or device calls.

From the repository root (tested with .NET SDK 10.0.401):

```powershell
dotnet build tests/ArchitectureRecovery/NonImageProbe/NonImageProbe.csproj --artifacts-path tests/ArchitectureRecovery/NonImageProbe/artifacts --ignore-failed-sources -p:NuGetAudit=false -p:RestoreSources= -p:UseSharedCompilation=false -v:minimal
dotnet tests/ArchitectureRecovery/NonImageProbe/artifacts/bin/NonImageProbe/debug/NonImageProbe.dll D:/MyWork/REA逆向测试 D:/MyWork/REA逆向测试/docs/architecture-recovery/current-nonimage-data-probe.json
```

All build outputs are isolated under this probe's `artifacts` directory. The report records each loaded assembly's path/SHA-256/MVID, matching portable PDB identity and SHA-256, and checks every PDB document checksum against its current source file before reporting success.

Thirteen finite checks cover distinct `NumberArray/Scalar` and `Number/Array` contracts; scalar element/reference retention; integer-array declaration acceptance followed by actual scalar-coercion exception; scalar integer promotion via float; copy versus reference behavior of numbers, text arrays and value-struct geometry; compound leaf representations and dictionary expansion; and `JoinArray` container copying. `GraphExecutor` failure-code routing, full workflow execution, GC/memory peaks and original-program equivalence are not executed or established.
