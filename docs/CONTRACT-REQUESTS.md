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
