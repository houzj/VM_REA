# Contour parameter/provider implementation slice

Original module SHA256 `56156A8707FA92CE758485772BEB18A5289CB97599533D86897A9391817FCF64`; original current Cs SHA256 `AEE906888A4C9CF6DF5917828665DEE83B1EA06C02F3A02E214CD9787FB96B80`. Exact 58 constructor/accessor mappings including each getter/setter RVA are in `analysis/contour-match/proposals/implementation/parameter-accessor-map.json`, produced by the adjacent script. Every entry has both accessors. Original `ParamItem` current source confirms constructor `(module,name,max,min)`, SetInt/Double checks bounds; SetEnum writes text without those numeric bounds. No 0..0 enum restriction is inferred.

Production `ContourParameterDescriptors` declares these 58 types; only original Algorithm's 56 defaults are supplied. NMSMaxOverlap/NMSSortType have no reset default and remain missing until a real value is supplied. `ContourConfiguration` uses existing parameter adapter, source-derived subscription queries/typed trees, shared reset helper with original Algorithm list, ROI rectangle/polygon/full-screen registration, standard/object controls, and disposal of registrations/run subscriptions. No Editor/catalog registration has been made. Custom `PyramidFeaturepatMatchConfigPanalData HPPyramidMatchPat` remains unimplemented; the shared standard renderer is not claimed to reproduce the training pane. The source-backed plugin runtime explicitly fails with `ContourMatchingKernelNotImplemented`; this is an implementation diagnostic, not a vendor error code or successful empty result.

## Native writing routes and remaining side effects

`SetParam` RVA251b0: model commands dispatch to CFeaturePattern `7780`; OKWhenNumIsZero/UseMatchAllMode and count/float limit fields update module storage; normal run fields go through `26790`→tool vtable70 (`15630`→core SetValue). Scale/Skew setters update module caches only after the call. `OutLineEnable`/`OutLinePointEnable` update separate bytes and BasicParam enable is their OR; they also set module dirty field15d90. SortType also updates module15eec and exact string `Score` flag15e51. NMS names fall through to base185b0 rather than a literal contour handler; concrete base/schema handling remains a gap. Not every string anchor proves a closed native route; per-entry JSON explicitly preserves this distinction.

### RangeScaleType actual assembly

Source C omits sscanf/sprintf extra arguments. Original instructions restore them:

* `26196..261a7`: `%u` destination `this+15db0`.
* `261e7..26214`: format local strings `%d` with integer1 and0.
* `26225..2622c`: compare stored mode with zero, nonzero jumps2635a.
* mode0 calls26239/26250/26267/2627e with ScaleXStart/End and ScaleYStart/End =1;26295/262ac SkewXStart/End=0;262c6/26318 load cached float15db4/15db8, promote to double, `%0.2f`, calls262fe/26350 for ScaleStart/End.
* nonzero calls26361/26378 ScaleStart/End=1;26392/263e4/26436/26488 load float15dbc/15dc0/15dc4/15dc8, promote/format `%0.2f`, calls263ca/2641c/2646e/264c0 ScaleX/YStart/End;264da/26522 load int15dcc/15dd0 and `%d`, calls26508/26550 SkewXStart/End.

The command list candidate is `ContourScaleModePlan.cs.candidate`; original address strings are independently extracted with strict module hash into `run-parameter-strings.json`. This is not yet a backend implementation: CRT format rounding/locale and actual SDK setters/error exceptions are not replaced by assumed .NET formatting/success. Provider currently stores the mode; backend side effects remain required before normal execution can succeed.

## Validation status

Earlier production result/resource slice built0 warnings/0 errors and passed17 checks; its immutable manifest is `production-validation.json` (SHA256 `3C15E26807506CFF5AA575D501F7FBB7B33CA4908B68562CD7FB1563F7C7FA27`). That snapshot predates the plugin/provider/descriptor files and must not be used as their build evidence. Seven new checks are prepared (total24), not yet built while root/B own the managed slot. Complete tool is still open: matching arithmetic, training/model ownership, full custom pane, native parameter side effects, result publication/actual host, shared outline and variable rectangles.

## Follow-up: concrete base and model dispatch bodies acquired

Read-only retained private Ghidra export `ExportContourModelCommands.java` completed with process exit0; no import, target execution, active REA target change or managed build. Exact module bodies are `output/180007780.c` SHA256 `6B9C966D4BFF8662E8A867736BC29B52DAC9B888D94261DBDB1C8D193B693E30` and `output/1800185b0.c` SHA256 `2A909E3E5FE0FAECA09F85EFD8A9D1034EEA993DA546C854FD2EC2FB78EC5410`, with adjacent original instruction/call listings.

This closes the earlier base-route uncertainty: base185b0 handles RoiType/ExternRoiType/DrawImageSize/FixtureInfo/ROIAreaType/OutputMaskImage/DynamicInData/DynamicOutData/LogLevel; unknown names take the explicit `Param not supported` branch and returnE000030D (C lines160–172). Consequently NMSMaxOverlap/NMSSortType are exposed Cs wrappers without a working module SetParam route, and are absent from original Algorithm and Tab. They are extra API, not a missing active normal UI operation. The current partial provider must reject explicit writes to these two names at the next authorized product window; generic successful local persistence would be a known gap until then.

Model dispatcher7780 maps SetModelData→7980, SpareModelName→8040, ChooseSpareModelIndex→7270, ChangeSpareModelName→70b0, ChangeSpareModel→6e70, AddSpareModel→6c50, ClearCandidate→73a0, DeleteCandidateByIndex→7440, UpCandidate→80d0, DownCandidate→7600, SelectedModule→7da0. It also accepts ModifyModel as return0, while caller251b0's concrete name gates must be respected before calling it. These downstream normal model-operation bodies are the next finite required set; neither capacity/index validation nor file format is inferred from names.

Current installed `.CommonControl/PatCreateCSharp.dll` SHA256 `C4BDD68167FCCE67BE3DF81C2A7FD9DB07FE3B7BEE8DF395DB6BCEA4EBBE96B5` was passively IL-exported to `PatCreateManagerCSharp.current.il` SHA256 `00EDD8B481BDC53F9BC3DB9783E6E4EF9331355EAAE75E63F436144BE83AD372`. Its CreateDipolePyramid method is lines8633–12066. Existing decompiled source at full-audit/renderer-shared-projects/PatCreateCSharp/PatCreateCSharp/PatCreateManagerCSharp.cs:991 is a reading aid; current IL, not old source alone, must support implementation. It creates CContourPattern, sets InputImage/weight-mask/RegionImage and training options, sets FixPoint, trains, reads automatic threshold/pyramid/chain outputs and ModelData, requires an outline, copies origin points, and disposes the native object in finally. Training kernel itself is not yet ported.

## Production provider build follow-up

After root's authorized short edit window, both unsupported NMS names were removed from writable descriptors; all58 Cs constructor/accessor records remain evidence. Production now has56 active parameters plus separate configuration/display persistence fields. New real ProjectReference build finished exit0,0 warnings,0 errors; actual executable passed24 assertions (13 result policy,4 resource parses,7 parameter/provider declaration checks). Logs are `provider-build.log` and `provider-run.log`; snapshot `production-provider-validation.json` supersedes the older17-check snapshot only for this finite slice. The checks instantiate the production plugin and inspect its provider contract; they do not open an actual configuration session or validate a real host/training/matching execution. Those remain required, and no complete-tool claim is made.
