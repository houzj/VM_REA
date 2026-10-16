# Integration handoff

Initial baseline: existing independent reconstruction, including owned Group navigation and Tab return fix. Not complete VM parity.

For each handoff record: branch/commit, source evidence, changed tool or framework behavior, real UI/run verification, remaining gaps, and shared contract requirements.

## 2026-10-16 / tool-plugins: inventory first

- User direction: establish the full reverse-engineering checklist first, then proceed against it; ImageSource is P0. No framework files or other worktree modified.
- Inventory: docs/plugins/CHECKLIST.md (162 plugin folders / 14 categories); inventory.json contains parameters, typed IO declarations, UI controls, binary/XML identities, resource hashes, declared libraries and shared dependencies. tests/Export-PluginInventory.ps1 regenerates and validates totals and unique identities from local original evidence. Transitive dependencies and native runtime semantics remain unverified.
- Evidence: D:/MyWork/REA逆向测试/analysis/full-audit/plugin-coverage.json and image-source-audit; exact ImageSource entries and native method addresses in docs/plugins/IMAGE-SOURCE.md.
- Small pre-inventory ImageSource fixes retained: XML numeric bounds and mode visibility; selected/folder empty list cannot fall back to unrelated current image; blank list entries report an error. These are partial changes, not full ImageSource delivery.
- Verification: existing ImageSource.PluginChecks passed 18 checks; inventory validated 162 unique identities and 173 native / 415 managed plugin DLL records. No original runtime equivalence claimed; new boundary checks and full native comparison remain pending.
- Shared needs: IS-001 RGB image, IS-002 camera/SDK provider lifecycle, IS-003 asset persistence/trigger/preview/scheduling in CONTRACT-REQUESTS.md. Framework acceptance pending.
