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
