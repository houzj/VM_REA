# Shared contract requests

User-assigned ownership: the plugin line implements ImageSource-required shared changes in its own worktree. The other worktree is not modified. Earlier pending sections below are historical requests; this status and the latest implementation notes take precedence.

| Contract | Current implementation | Remaining boundary |
|---|---|---|
| IS-001 | Typed RGB/MONO, actual-kind validation, WPF previews and optional MONO output implemented | Cross-CPU conversion paths and documented unsafe-format policy differences |
| IS-002 | Scoped lifecycle, SDK submission and installed MVS provider; real MV-CS050-10GC acquisition/settings/failure recovery verified | Other models and positive physical loss counters unverified |
| IS-003 | Typed subscription bridge, trigger suppression, stop control, embedded assets and partial numeric diagnostics implemented | Original desktop camera/subscription/folder popup UI and remote directory service need evidence/environment |
| IS-004 | Natural catalog order, scoped events, refresh/run callbacks, reset/reload/close disposal implemented | Original ANSI-path differences and uncontrolled event-burst timing unverified |

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

- IS-001: decoding color files to existing MONO8 does not require RGB PortValue changes. Plugin-local BMP/PNG/TIFF decoding and observed integer grayscale conversion are now implemented with exact lossless-corpus pixel comparisons. RGB24 output and preview remain blocked by IS-001. JPEG now uses an independently built libjpeg-turbo 2.1.3 backend. RGB corpus pixels and successful native MONO8 outputs match; small-color native conversion errors remain an algorithm-line gap, requiring no shared API. Integration must preserve the plugin-local runtimes/win-x64/native/turbojpeg.dll and ThirdParty/libjpeg-turbo license content in deployment. Missing backend or unsupported architecture reports ImageCodecUnavailable; shared RGB output remains blocked by IS-001.
- IS-003: codec observations add original non-image error 0x80100001. Truncated BMP returns success with unstable missing pixels in repeated original runs; plugin deliberately rejects it. Please define numeric native-code mapping versus plugin policy errors and clearing stale outputs without forcing the plugin to synthesize frames from malformed data.
- IS-003/004: local folder picker/path checks now live in the plugin and do not need a framework file change. Original OpenFolderDialog selects a separate remote DirectoryBrowser when RemoteOperate.IsRemoteMode, then SetParamUnit reports and PreExecuteRun can request execution. ToolEditorContext still needs explicit local/remote editor context, remote directory selection and parameter-change execution/report services. Local selection must not silently start a private folder watcher or imitate TriggerProcessRun; those remain IS-004 session services.

## 2026-10-16 implementation ownership update
User explicitly assigned required framework adjustments to the plugin line in this worktree. Pending IS-001/002/003/004 are now implementation work, not stop conditions. Other worktree remains untouched.
- JPEG minimum geometry is source-confirmed and implemented: color width/height >=4, gray bypass; modern SIMD coefficients 76/150/30 >>8 confirmed. 35 native fixtures now compare valid RGB/MONO and native failures.
- Existing integer inputs suffice for ClearTriggerValue. Plugin declares the optional port; missing input continues, every nonzero clears history/width before geometry validation. Eight native pixel/error scenarios pass. A typed picker and session lifecycle still need integration.
- LocalFolderCatalog implements original StrCmpLogicalW<0 ordering and top-level lowercase jpg/png/bmp/tif filtering; 121 comparisons and a complete native snapshot match. This is not yet session watching. Tie order and original ANSI-path differences remain explicit.
- Extended format observations cover Gray16 high-byte conversion, low-bit rejection, alpha, first TIFF frame, LZW and indexed bytes. Packed-palette PNG is explicitly unavailable: original packed-row indexing lacks unpacking and its MONO output differs from converting RGB for the same fixture. Do not fabricate safe canonical pixels from buffer-dependent behavior.

## 2026-10-16 ImageSource integration implemented in plugin worktree

The following supersedes earlier unavailable/pending implementation descriptions; validation is not a claim of complete native parity.

- IS-001: `IImageData`, immutable `RgbImage`, appended `DataKind.RgbImage/Image`, and generic ImageSource output. Existing enum ordinals and GrayImage factory remain compatible. Graph inputs and workflow exports check the actual concrete image kind; MONO consumers reject RGB. WPF main/workflow previews use the actual format/stride. Deploy the plugin JPEG native runtime and licenses as before.
- IS-002: `ModuleExecutionScope` owns per-node state and registered services for an opened solution, with cancellation and execution leases. Reset/removal/reload/close release state; runtime factory disposal does not erase frame history. `IImageAcquisitionService` accepts camera ID, optional exposure/gain and wait time and returns an owned frame plus counters. `SubmitImage(scope,node,image)` consumes each SDK submission once. MVS adapter uses installed binaries, not redistributed vendor DLLs. Real MV-CS050-10GC acquisition, exposure/gain readback, rejected negative writes, timeout/recovery and camera stitch failure were compared. Other models and positive physical loss counters remain unverified.
- IS-003: `ToolEditorContext` supplies mode, scope, typed subscription selection/labels and camera catalog/action. WPF connects these to existing workflow subscriptions; integer clear, numeric camera settings and text filter have real links. `ModuleRunControl` requests continuous-owner stop after the final successful local frame. `ModuleResult.Continuation` suppresses successors for filtered triggers and `NativeErrorCode` reaches node records. Failed results may retain validated partial outputs; stale records are not reused.
- IS-004: plugin-owned watcher is scoped to the solution/node. Create/delete dirty the snapshot and optionally request execution; modify dirties without requesting execution; rename is not subscribed, matching observed native behavior. Scope refresh/run events bridge to the WPF dispatcher and ignore callbacks from replaced scopes. Native comparator/filtering evidence remains in the catalog ledger.
- Persistence: optional `ResourceDocument.EmbeddedBase64`, digest-verified resource streams, plugin save preparation and `IToolAssetPersistence` embed up to 120 configured images. Old external-resource JSON omits the new field. This is independent VMX storage, not vendor SOL compatibility: vendor restore prefixes filenames with a numeric module ID; VMX retains configured filenames. Folder snapshots are now included; native subscribed-folder reload rescans the folder and returns NoImage if empty, without falling back to embedded assets. Remote selection and full desktop popup parity remain open.
- Evidence ledgers: `image-source-rgb-evidence.json`, `image-source-control-evidence.json`, `image-source-watch-evidence.json`, `image-source-embed-evidence.json`, `image-source-folder-embed-evidence.json` and `image-source-camera-evidence.json` record actual original SDK operations. Camera evidence includes real-device observations and full-frame RGB/extra-MONO comparisons. Synthetic provider checks remain separate independent integration tests.

## 2026-10-16 camera/persistence contract completion and migration

- `CameraInfo.PersistentId` is optional and additive. The MVS provider supplies device serials; ImageSource persists `CameraIdentity` and resolves it before the transient enumeration ID. Old nodes with an empty identity continue using CameraID. A missing saved identity returns CameraNotBound rather than selecting another indexed camera. This is an independent host binding, not serialization of the vendor global camera module.
- Native camera SN increments before stitch validation. A stitch failure retains already-produced SNCode/FrameNum/GetCameraFailCount while publishing no stale image. A no-trigger timeout is Failed with ModuStatus=0 and incremented failure count even when its observed native ErrorCode is 0. The raw SDK error stays in the message; unobserved SDK failures are not asserted equivalent.
- Exposure/gain inputs use float32 and three decimal places. Both native parameter wait slots apply; rejected writes try the fallback name and do not abort acquisition. Software-trigger mode issues one TriggerSoftware command per requested frame without changing device trigger configuration.
- `IToolAssetPersistence` adds a scope-aware save overload with a default implementation forwarding to the old method. Existing two-argument registry save callers remain valid. WPF supplies its opened-solution scope so folder images come from the active catalog; an unstarted folder uses a fresh catalog. SaveAs normalizes configured image and folder paths against the old base directory.
- New embedded-resource fields and CameraIdentity require the updated host/plugin. Reading older documents remains supported; compatibility with older binaries reading newly extended documents is not claimed. Contracts are still under reconstruction.
# 多图采集 IA-001～005（当前插件 worktree 内实施）

用户已将插件必需框架调整授权给插件线。本批只改当前 worktree，不动框架线目录。

- IA-001：CameraRequest 新增可选 ConvertParametersToFloatBeforeFormatting，默认true保持图像源行为；多图采集为false，double直接按三位小数格式化（原生1800111e0的%.3f路径）。MVS提供器据此格式化写入，不改默认触发源。
- IA-002：IAcquisitionCameraSettingsService 提供曝光/增益当前值和最小/最大范围；MVS已实现读取。供新增逐图配置和实机验证使用，不把模拟相机值当设备能力。
- IA-003：IAcquisitionLightService 提供设备列表、逐图 TriggerAsync 和 CompleteAsync；请求携带设备ID、通道、亮度、触发时间。CompleteAsync对应原生重置本地暂存结构，不自动发送物理OFF；触发/结束错误保留诊断并继续采集。取消清理属于独立宿主生命周期支持。设备实测按用户要求排除。
- 宿主：注册 vision.image-acquisition、映射采集工具箱项目。IToolImageListProvider.SupportsSingleImageExecution 默认true，多图采集为false，避免单图列表操作写入多图插件没有的CurrentImagePath/AutoPlay等参数。

- IA-004：IToolImageOutputProvider 返回多图输出端口和显示名；宿主分别预览OutImage0Image～OutImage7Image，未产生的槽位显示为空，不能复用上一槽帧。

- IA-005：IAcquisitionLightModuleTransport提供全局光源资源目录和模块二进制命令传输，AcquisitionLightProtocolService已实现LightDeviceParamSetTrigger完整748字节编码。设备驱动/传输属于全局资源提供器，不把原厂托管744字节结构误用为多图采集748字节载荷；设备测试不再作为代码交付阻塞项。

# 缓存图像 IB-001～004（当前插件 worktree 内实施）

- IB-001：原厂 `ButtonSelecter/SetFilterSourceOperation` 支持整数常量输入。追加 `WorkflowSourceKind.IntegerConstant`，`Id` 保存有符号32位整数文本，`Port` 必须为空；接通配置校验、输入解析、标签及持久化，保留原枚举值。`ToolEditorContext.InputInteger/SetInputInteger` 提供读写和清空；宿主通过已有文档编辑事务维护撤销并移除被替代的绑定。新常量文档需更新宿主读取，旧文档不受影响。
- IB-002：`IToolImageOutputProvider` 作为独立预览发现接口，输出提供器无需实现图像列表接口；缓存图像暴露15路。单图列表、加图及轮播仍仅由图像列表接口启用。未产生槽位必须清空预览。
- IB-003：原生 `CopyImage` 只复制载荷，不校验图像格式/长度。新增不可变 `RawImage` 与 `PortValue.Image(RawImage)` 的通用图像类型，保留尺寸、格式标记和原始字节。已知格式仅在 `ImagePreview.Packed` 中转换预览；未知格式不伪造灰度预览。独立框架仍拒绝非法指针、非正尺寸、空载荷。
- IB-004：原厂底部 `SelfExcuteModule` 仅调用当前模块 `Run`。WPF配置宿主改为从当前结果及订阅读取输入、执行当前运行器、更新结果与预览，避免每次缓存追加都重跑前序采集工具。状态仍归属当前 `ModuleExecutionScope`，撤销配置不清除缓存，节点移除/会话结束清理。调度与共享选择器是独立宿主实现，原厂全部弹层及线程时序仍不宣称相同。

依据、运行序列、来源身份与边界详见 `docs/plugins/IMAGE-BUFFER.md`。用户授权插件线实施这些必要调整，并在闭合节点使用中文说明提交、推送当前分支；不修改或合并另一 worktree。

# 光源 LT-001～004（2026-10-17，当前插件 worktree 内实施）

- LT-001：原厂算法读写8路，旧XML仅声明6路亮度输入/输出。独立光源模块声明9输入、42输出，补足第7/8路及可选状态/沿/持续时间字段；真实原厂SDK设置第7/8路及结果读取已对照，不伪造未发布输出。46项配置区分32算法参数与UI/原生补充字段。
- LT-002：复用 `IAcquisitionLightModuleTransport`，载荷为原生748字节 `LightDeviceParamSetTrigger`，保留+24触发时长和+28首通道、8通道44字节布局。传输提供器把下发失败作为IOException报告；模块记录诊断、输出NG并保持原生可为0的错误码。订阅越界先发送后验证，不能由编码器提前钳位或拦截。未匹配触发不发送。
- LT-003：追加 `WorkflowSourceKind.TextConstant`，Id存储JSON字符串，Port必须为空；类型验证、执行、标签及VMX保存/读取已接通。`ToolEditorContext.InputText/SetInputText`提供直接输入和清空，沿用文档编辑事务与撤销。整数、已有枚举值及旧文档不变；新字符串常量文档需新版宿主。
- LT-004：`ILightResourceConfiguration`及`ToolEditorContext.ConfigureLights`提供全局资源管理入口，UI从已注册传输服务读取目录，按原厂旧全局资源类型映射形成选择字符串，换有效设备复位通道配置并保留手动亮度。宿主有管理服务时启用按钮；全局LightControl/CommManager的目录、设备连接和驱动由各自清单项承接，不在工具中虚构物理协议。模块自执行及连续调度复用已有作用域、取消和生命周期；资源管理共享弹层未宣称完整等价。

原厂算法通过自有SDK进程和明确标注的测试传输边界对照，不做设备测试；源码/资源/运行证据与未知边界见 `docs/plugins/LIGHT.md`。用户授权本线闭合后使用中文说明提交并推送当前分支，不操作另一worktree。
