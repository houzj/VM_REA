# Shared contract requests

Framework line owns acceptance and implementation. Plugin line does not edit shared contracts.

## IS-001: typed RGB image and optional Mono8 output (pending)
- Caller: VisionPlugins.ImageSource, PixelFormat=2 and OutMono8.
- Evidence: analysis/full-audit/image-source-audit/ImageSourceModuleAlgorithmTab.xml, PixelFormat/OutMono8; image-source-native-all-methods.json, CMyAlgorithmManager::SetImageOutput (0x18001d720), SetImageOutputMono8 (0x18001da20), ConvertRgbToMono8 (0x18001dd20). Evidence root is D:/MyWork/REA逆向测试.
- Payload: image width, height, pixel format, stride and owned bytes; RGB24 main Image plus optional ImageMono8 and matching dimensions/format outputs. Existing GrayImage cannot carry RGB24 honestly.
- Proposed API: shared image value type and DataKind/PortValue support, plus preview service accepting that type. Define ownership, disposal, serialization and downstream compatibility before plugin adopts it. Existing gray consumers must keep working.

## IS-002: camera/SDK provider and session lifecycle (pending)
- Caller: ImageSourceType=2/3, camera selector, exposure/gain subscriptions, reset and cancellation.
- Evidence: decompiled/ImageSourceModuleControl/Apps.XmlParser.ParamTab/EnumerationCameraForImage.cs; AlgorithmTab CameraID/ExposureTimeInput/GainInput; native CMyAlgorithmManager::AddShareMemoryImgToList (0x18001af00), ConvertDataImage (0x18001c250), NotifyEvent (0x180023620).
- Payload: provider identity, camera IDs, typed image buffers, frame metadata, exposure/gain inputs, provider error and acquisition timeout.
- Proposed API: per-execution-session acquisition/SDK submission service; specify buffer ownership, thread/cancel/timeout behavior and shutdown/reset hooks. NodeId alone is not a safe global session key. No vendor provider is currently wired; preserve explicit unavailable errors until integrated.

## IS-003: persistence assets, triggers, preview and execution control (pending)
- Caller: SolSaveImageData, ShowImageName, AutoStop, TriggerStringEnable, subscribed-folder TriggerProcessRun.
- Evidence: AlgorithmTab SolSaveImageData tips (maximum 120 saved images), TriggerFilter/TriggerString; native SaveModuleData (0x1800171a0), SaveImageFileToSol (0x180018210), MonitoringSubFolderChanges (0x180022110); decompiled LocalImageList.Init distinguishes desktop and SDK context.
- Proposed API: solution asset save/load hooks with relative references; preview filename overlay service; typed subscription inputs and scheduler stop/request-run actions. Folder watching must be disposed with the owning session. ToolEditorContext needs an explicit desktop/SDK context before SDK-only image list is embedded.
- Compatibility: generic ModuleStatus.NoMatch currently cannot communicate native workflow stop semantics; do not silently equate the two. File path persistence alone cannot implement embedded image saving. Native serialization layout and exact trigger semantics remain to be mapped.

## 2026-10-16 evidence updates to IS-001/002/003 (still pending)

Original SDK probe and source ledger: docs/plugins/IMAGE-SOURCE.md and image-source-evidence.json. Probe loads a sample with partial failures in other modules, then runs the present ImageSource module independently; this is not whole-solution acceptance.

- IS-001: native SDK SetImagePath succeeds with RGB24 (`0x02180014`), 1296×1024, 3981312 bytes even when local PixelFormat had been MONO8. SDK mode must honor the submitted/native buffer format rather than applying local-file enum semantics. Local MONO8 sample output hash has been independently matched; no general RGB conversion parity established.
- IS-002: SDK Process consumes a fresh-submission flag (`this+0x25d40`) and can wait 3000ms for copy completion; no submission returns `0xe000000f`. Unbound camera returns `0xe0000309`. Observed successful SDK path submission has no SN/name/path output (SDK result object defaults to 0/empty); do not synthesize local metadata. Camera source additionally publishes FrameNum, LostFrameCount, LostPacketCount and GetCameraFailCount; acquisition error increments fail count. Exposure/Gain subscriptions are floats, formatted to three decimals with ExposureTimeAbs/GainRaw fallbacks and per-write wait time. Provider contracts must define these lifetimes and optional results.
- IS-003: local SN starts with `InitialSN+1`, not InitialSN. AutoStop calls `VM_M_StopOwnerProcedure` only when owner running parameters indicate the relevant running mode and AutoPlay is on, while still producing the last frame. Editor self-run versus continuous owner execution must be available in ModuleExecutionContext; an unconditional permanent NoMatch is incorrect. Plugin now explicitly reports ProcedureStopUnavailable when this service is needed.
- Editor lifecycle: `ToolEditorContext` lacks global camera enumeration (module type 6), typed subscription pickers (float/string/int), and the desktop/SDK distinction. Source confirmed controls now exist in the plugin view, but service selectors stay disabled. Please expose editor services through the shared context rather than having plugin code reach into WPF shell state.
- Error output: original self-run can return normally while result ErrorCode is nonzero and frame is empty. Current GraphExecutor discards Failed result outputs; a policy is needed for native numeric ErrorCode/ModuStatus and stale-output clearing. NoMatch currently requires all required outputs and still permits execution successors; it cannot represent the original trigger-rejection control flow.

## IS-004: subscribed folder events and session-owned disposal (pending)

- Caller: ImageSource IsSubscribeFolderMode, RefreshImage and TriggerProcessRun.
- Evidence: CMyAlgorithmManager::MonitoringSubFolderChanges `0x180022110`, UpdateImageFilesListFromDir `0x180023190`, GetImageFilesPathNameFromDir `0x180022750`; Process `native-0.c:350–418`. Four binary extension literals at `0x180031964/68/6c/70` are jpg/png/bmp/tif. SortImageNameInFolder calls comparator FUN_180023550, whose semantics still need mapping.
- Payload/lifecycle: session/node identity, folder path, change notification queue, refresh/report notification and scheduler request-run action. Native monitor uses completion-port state, Process consumes dirty flags and preserves the active-list cursor on rebuild; RefreshImage affects UI/report behavior, not a simple enable/disable directory scan. Native ordering/debounce semantics must remain explicitly unverified until mapped.
- Proposed API: session-owned folder event subscription returning a disposable handle and events; host guarantees disposal on node removal, reset, solution close/reload and cancellation. Runtime modules are currently created/disposed per graph run, so a watcher in each runtime instance is not a valid persistent owner.
- Compatibility: plugin no longer silently substitutes `EnumerateFiles("*.bmp")` per execution. Until the session service and sorting semantics are defined, folder mode returns FolderWatchUnavailable. Do not require a plugin-specific watcher in framework assemblies.

## 2026-10-16 original SDK UI clarification (IS-002/003, pending)

- Original SDK-hosted controls now render in all three modes, using a byte-identical XML copy in the plugin worktree because vendor loadXml changes file attributes. Evidence and screenshot hashes: docs/plugins/image-source-evidence.json. The earlier blank model was a probe hosting failure, not absence of mode UI. Desktop capture remains unavailable.
- IS-002/003 editor payload still needs a real camera enumeration/action and typed float/string/int subscription pickers. Original camera exposure/gain and trigger inputs are text fields with link actions; InitialSN and stitching heights have separate numeric/subscription switches. A generic disabled ComboBox does not represent these behaviors. Plugin now displays the respective fields/icons, but host actions remain pending.
- IS-003 must expose desktop versus SDK context explicitly: original SDK local mode embeds image-list add/add-folder/delete, all/selected execution and thumbnails; camera/SDK modes hide the list. Empty folder-category headings remain visible. No new shared API or framework source has been changed by this audit.

## 2026-10-16 MONO8 stitching clarification (IS-001/002/003, pending)

- Plugin-local MONO8 stitching no longer requires a new shared image contract and is now implemented. Evidence: docs/plugins/image-source-stitch-evidence.json; 11 original SDK operations compare frame bytes, geometry errors and SN behavior. RGB stitching still depends on IS-001; camera/SDK inputs still depend on IS-002.
- IS-003 clear-trigger payload is the native integer input `ClearTriggerValue` (UI selector name `ClearTriggerCondition`), gated by `ClearTrigger`. Process reads it before acquisition/stitching; nonzero sets the clear flag. StitchImageOutput zeroes its retained buffer and clears recorded width before validating the incoming geometry. A visual toggle alone cannot supply this input. Typed subscription binding and missing-input/error policy remain required; plugin reports StitchTriggerUnavailable until integrated.
- IS-002 session identity/reset/disposal also owns cross-frame stitch history. A changed width fails with native 0xe0000502; changing crop origin or toggling stitching off does not clear history. Output-size changes reallocate/zero the buffer. Do not reset the entire plugin state for each parameter edit or ephemeral runtime factory creation. Existing plugin fallback key is base directory + NodeId, which is not a complete session identity.

## 2026-10-16 local decoding and folder selection clarification (pending)

- IS-001: decoding color files to existing MONO8 does not require RGB PortValue changes. Plugin-local BMP/PNG/TIFF decoding and observed integer grayscale conversion are now implemented with exact lossless-corpus pixel comparisons. RGB24 output and preview remain blocked by IS-001. JPEG decoder mismatch is an algorithm-line item, not a request for a new shared API: plugin explicitly reports ImageCodecUnavailable until compatible decoding is completed.
- IS-003: codec observations add original non-image error 0x80100001. Truncated BMP returns success with unstable missing pixels in repeated original runs; plugin deliberately rejects it. Please define numeric native-code mapping versus plugin policy errors and clearing stale outputs without forcing the plugin to synthesize frames from malformed data.
- IS-003/004: local folder picker/path checks now live in the plugin and do not need a framework file change. Original OpenFolderDialog selects a separate remote DirectoryBrowser when RemoteOperate.IsRemoteMode, then SetParamUnit reports and PreExecuteRun can request execution. ToolEditorContext still needs explicit local/remote editor context, remote directory selection and parameter-change execution/report services. Local selection must not silently start a private folder watcher or imitate TriggerProcessRun; those remain IS-004 session services.
