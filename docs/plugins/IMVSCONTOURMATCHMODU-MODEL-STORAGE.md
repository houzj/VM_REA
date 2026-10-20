# Contour normal model storage — source slice

Target is Contour module SHA256 `56156A8707FA92CE758485772BEB18A5289CB97599533D86897A9391817FCF64`. Private retained Ghidra export `ExportContourModelStorage.java` completed exit0/readOnly/noanalysis, without importing another copy or running the DLL. Eleven direct bodies and original assembly/call listings are in `analysis/contour-match/native-source/output`; `model-storage-completed.txt` records the finite set.

| Actual method RVA | Confirmed operation |
|---|---|
|8040|zero 260-byte pending public-name buffer d7e8 then memcpy_s; lock d7c0|
|6c50|reject null/zero data E0000001; first empty pointer among100; malloc/copy, actual/allocated lengths, selected1, Count++; copy pending name; full E0000326, allocation failure E0000002|
|7270|copy selected-index text into260-byte buffer, atoi→selected index at offsetd9f0 (`param1+367c*4`); rejects unsigned index>100 **after writing**; accepts100, not a safe100-record access contract|
|70b0|rename selected record, rejects negative selected index; lacks matching upper check in this body; zero/copy260 name bytes|
|6e70|replace selected model buffer; frees previous before allocating replacement; rejects negative selection/zero length, allocation fail leaves old buffer gone; requires exact read of body for error path|
|73a0|free each of100 buffers then memset(this+8,d7b8), Count0; pending public name/selection fields after this range survive|
|7440|index unsigned<100; free target, shift every following record left through99, clear last; Count-- even if target was empty|
|80d0|index1..99; scan preceding slots until nearest nonempty, call8250 swap helper; no neighbor yields success|
|7600|index0..98; scan following slots until nearest nonempty, call8250; no neighbor yields success|
|7da0|split comma into exactly2 strings, atoi index/flag; index>=0 && index<Count and flag<2; writes selected flag|
|7980|SetModelData is an XML **file** loader; load document, root→ModelData text; first empty slot; decode via1140, store filename-derived name/path and two distinct lengths; no Add-like Count++/selected1 assignment visible in this body, so import cannot be replaced by Add|

Important boundaries: `7270` accepting index100 does not prove renaming slot100 is safe. Raw names are ANSI byte buffers; no encoding equivalence is inferred. `7440` deleting an empty slot can decrement Count; normal UI admission versus malformed/sparse states must be explicit. Decode1140 and swap8250 are required direct helper gaps before those two operations can be ported completely. Shared XML parser/file-IO calls remain evidence dependencies; no generic base64/hex format is guessed for ModelData.

`analysis/contour-match/proposals/implementation/ContourModelStorage.cs.candidate` implements only source-confirmed normal Add/public-name/Select/Delete/Clear and copied snapshots, with explicit safe admission for excessive names, sparse selection and empty-slot deletion. It is not yet product code, built, a model decoder, or a complete model pane. Source malformed pointer/allocator behavior is not simulated. The RangeScaleType plan is separately covered in PARAMETER-PROVIDER.

Next finite implementation work is exact swap8250 and decoder1140, then connect normal model collection and current CreateDipolePyramid parameters to the dedicated training control. Matching/training numerical kernels remain genuine unimplemented scope; no tool-complete claim follows from the separate24 result/configuration checks.

## Follow-up: direct helpers and production list integration

Both remaining direct helpers were acquired from the same hash-matched private retained project:1140 C SHA256 `6724F16A998678DEA13C4333C3863E96F8687C67689A4EE7A134CDAA0B80C12F`;8250 C SHA256 `E8ACB5640751E80FA7D752D0B3D3B056C0EEFCF720428D2C51165FB76E9D02EF`. Original alphabet pointer3f000→VA1800306b0 and its65 bytes including nul are recorded in `model-decoder-alphabet.json`. The decoder consumes groups4, maps unknown bytes toFF, stops on padding/incomplete tail, and does not skip arbitrary whitespace. It is ported explicitly rather than using Convert.FromBase64String. The XML loader alone skips one initial newline before calling this helper.8250 swaps exactly0x228 bytes; managed entries exchange full name/data/selection records without native pointers.

Production `src/VisionPlugins.ContourMatch/ContourModel{Storage,Collection,Decoder,Controls}.cs` now implements normal-domain model ownership and source5-key sidecar shifts/swaps/deletion, Add failure's candidate removal, immutable copied snapshots, and actual configuration model list. `ContourConfiguration` evaluates the original runtime DisplayType (`BaseType.Name`) and registers the actual FeaturepatMatchConfigPanalData key; root supplied its source-confirmed shared template catalog entry through the generator. The list preserves row command names, selection, ordering, deletion, confirmation, name's LostFocus32-character/blank checks and original MaxLength63. Current actual FeatureMatchCommonControl classes were passively IL-exported for branch confirmation, rather than trusting the old readable source alone.

Owned model buffers and opaque UI sidecars use a separate `ContourCandidateStorage` JSON parameter for this implementation's solution transport. This is explicitly not vendor .contourmxml file compatibility. Training/Load/Edit callbacks deliberately notify unimplemented operations; no fake patterns or empty-success matcher is supplied. Source Rename's IUnRedoService.SetForbidRecord try/finally remains an explicitly reported shared history-contract boundary; no shared changes were made by this line.

Real production ProjectReference ModelChecks passed131:123 storage/decoder/sidecar checks,2 JSON checks,6 WPF checks opening original secondTab and exercising persisted CheckBox click, training rejection and confirmed Clear. Separate ResultChecks passed24 on the same production dependency set. Both final builds exited0 with0 warnings/0 errors. First model test build's CS0542 helper class/method name collision is preserved in model-build.log; only the test helper name was corrected. Final logs: model-build02.log/model-run.log/model-result-build.log/model-result-run.log.

Before build436 production source/project/XML hashes were recorded. After managed release435 remained the same; shared ConfigurationTemplateCatalog had the expected separately owned metadata-label change fromBDF902F0730209E7EB91A7452322631D161D38D32E56FCDD948DE4A1A9403BDB toCBCA2A719E4B567AF16D9F7107858498E46024CBFABBB06E10D0B99574CB6BAA. Actual compiled Contracts PDB confirmsBDF902..., so this test is not misreported as the later current catalog snapshot. Exported68 PDB source documents are in model-compiled-source-manifest.json SHA256 `4696A8FEA73D569E2E88C4E532B8331301D3CAA3CCF1E17F21D14F7D476FA2CD`; Contour12 documents (including generated files) match current hashes0 differences.

Still required: actual numerical single/multiple matching kernels, training kernel/dialog, real vendor file import, complete run-parameter side effects/formatting/error paths, result publication and full production host integration, shared outline/variable rectangles, and the above source history gate. This finite success is not complete-tool or native parity evidence.

