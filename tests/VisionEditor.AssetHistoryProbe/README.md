# Asset history diagnostic

This is a deliberately failing characterization of current main, not a passing regression suite or a native VM probe. See docs/reverse/D02-asset-history-batch3.md and FCR-024. Exit 1 and `passed=false` preserve the unresolved SaveAs/Undo asset identity failure.

Run from the repository root:

```powershell
dotnet run --project tests/VisionEditor.AssetHistoryProbe/VisionEditor.AssetHistoryProbe.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false -- analysis/full-audit/batch3-20261019/D02
```

Each run creates a GUID directory and owns its generated BMP; only that fixture is deleted after Redo. It does not access tool-plugins or the installed VM. The first recorded run used the former AssetHistoryChecks project name; renaming the diagnostic does not change its logic or rerun its assertions.
