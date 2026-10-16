# Orthogonal router evidence and implementation boundary

Original evidence: `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`, GetPath at line 438 and GetPathMap at line 488; `VMControls.WPF.Base.Astar/AStar.cs`.

Observed VM pipeline: anchor and direction -> 5-unit margin offset -> grid at 5-unit resolution -> AStar.FindPath -> PathFinder.OptimizeLinePoints -> GetTurnPoints -> RechargeLinePoints. All occupied node/group rectangles are collected recursively. On initial failure VM retries with source/target rectangles only, then falls back to an elbow path. AStar also has search termination heuristics. These are observed decompiled statements, not an independently confirmed live runtime trace.

Independent implementation: `src/VisionEditor.Core/OrthogonalRouter.cs`. Public Route accepts source/target RoutePort (anchor, outward direction, owner bounds), extra obstacles, margin, state budget and bend penalty. Owners are always obstacles except their own outward anchor stubs. Rectangle boundaries form a compressed visibility grid, searched using Dijkstra with heading state, Manhattan segment length and bend penalty. This avoids full canvas area allocation and supports negative coordinates. Success has orthogonal non-crossing segments; invalid input, blocked route or budget exhaustion has an empty point set so the caller must omit/flag the connection rather than draw through obstacles.

Differences: route vertices and tie-breaking do not reproduce the VM 5-unit AStar/PathFinder sequence; margin and obstacle avoidance are restored as independent behavior. VM fallback ignoring other nodes is deliberately not reproduced. Boundary following is allowed. Existing connectors are not obstacles. No exact VM routing equivalence claim is justified.

Validation: `dotnet run --project tests/VisionRouting.Checks` passes 20 checks covering obstructed detour, orthogonality, margin collision, outward/inward port direction, negative coordinates, blocked stubs, budget exhaustion, invalid input, simplification, owner-side validation. Canvas integration and original-VM path comparisons remain outside this isolated implementation.
