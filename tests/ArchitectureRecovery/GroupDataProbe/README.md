# Current owned-Group data probe

This non-GUI console references the real current `VisionFramework` and `VisionData` projects. It executes `SolutionExecutor` with root → owned Group body → ordinary helper workflows. Two tiny synthetic leaf modules produce/echo typed values, create 1×1 images, expose disposal observations and intentionally fail one finite case. They do not replace the Group runner, break logic, accumulation or graph executor.

```powershell
dotnet build tests/ArchitectureRecovery/GroupDataProbe/GroupDataProbe.csproj --artifacts-path tests/ArchitectureRecovery/GroupDataProbe/artifacts --ignore-failed-sources -p:NuGetAudit=false -p:RestoreSources= -p:UseSharedCompilation=false -v:minimal
dotnet tests/ArchitectureRecovery/GroupDataProbe/artifacts/bin/GroupDataProbe/debug/GroupDataProbe.dll D:/MyWork/REA逆向测试 D:/MyWork/REA逆向测试/docs/architecture-recovery/current-group-data-probe.json
```

Only three scenarios run: two successful iterations then self-output break before the next iteration; second-iteration failure; zero iteration range. The report keeps actual ordered enter/return/dispose events, runtime Group metadata, returned Nodes/Calls, exports/displays and reference checks. There is no invented event for an unexecuted third iteration. It distinguishes attempted iteration count, prior successful accumulation, failed caller output filtering and required outputs missing after a zero range.

Build outputs stay in this probe's own artifacts directory. Each loaded DLL's hash/MVID and PDB ID/stamp are recorded; every portable-PDB document checksum is checked against its current source. The tracker deliberately holds values to compare identity, so this does **not** measure last-owner release or GC. No original/native program, real tool algorithm, WPF, device, memory-peak or parity test runs.
