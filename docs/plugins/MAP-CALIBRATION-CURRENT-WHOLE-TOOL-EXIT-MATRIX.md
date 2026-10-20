> 2026-10-20 最终自审提交：根线已接原typed tree初始化结果运输，真实MainWindow Host26与Path30实际通过；450个src快照0漂移。全工具源码→实施→验证矩阵已复核，交根线最终审计，未自行提高工具计数。原Save80100203与忙门失败日志保留，缓存所有权产品修复及真实宿主等待测试修正分列；最新manifest SHA cc86e1612d925bac0e8b1d9f169d2885714fd5aae74e51e96bf2af23dd8425ac，完整结果见MAP-CALIBRATION-PATH-HOST-FINAL-VALIDATION。下文未应用/未运行/仍缺枚举的段落均是历史阶段，不能覆盖本段。人工验收单列，不阻挡源码+实施自审完成标准。
# Map当前完整退出矩阵与待跑两入口

2026-10-20。当前产品冻结，NI由根线独占managed；本表不提升工具完成数。

| 必需活动 | 原证据 / 当前产品 | 已有自动证据 | 当前退出条件 |
|---|---|---|---|
| 图像识别、18/24纠错、真实格网方向/ID/匹配 | R03/R04实际caller已接生产；不回退legacy几何 | 生产2046、ROI7361、6/8 axis12/mask4/full4及原bit/order对照 | 算法未改，不重复泛化样本；本轮真实主壳两图168生产再确认接缝 |
| 六typed输入及正常POINT组合 | typed服务按原读序/index0/容量512/模式门 | MapTyped121，InputState48；原10例oracle | 本轮公共path未改变读核；主壳POINT组合订阅保存重开必须通过 |
| Region、Detect、显示、Reset、配对生命周期 | 窄Region互操作已批准；Detect原正常三路径保持0；显示原图元；Reset有效名单 | Region25、Display15、Reset27、半帧48 | 旧证据保留；当前新生产DLL真实Host显示/close/RemoveNode验证 |
| 两输入文件base控件 | 原OpenFileDialog先留CurValue；SetParam失败保原native path、可能按reader替换matrix；picker无条件history | 旧File reader/runtime证据；本轮原case8复用 | **阻塞**：base-input-path.patch待根线应用，PathChecks双侧真实控件失败尝试值/成功通知/history跑过 |
| 输出路径derived控件 | Filter/目录前门，拒绝不触native保UI尝试值；picker无条件history | 通用消费者Path70不能替代Map单独接线 | MapPathChecks真实Map完成，不套base路径 |
| Save | 原SaveFileDialog本身Filter双字段/current-culture suffix门→导出→GetParam读回→history→Success；错误不history | 已落Map专属Save门及通知，尚未运行 | MapHost有有效真实两图seed后.XML拒绝和.xml实际导出/参数读回/事件序列 |
| 真实主壳配置与注册 | 根线已加MainWindow Register、csproj引用、catalog ModuleId | 新Host源码仅准备，旧Graph不能证明 | **阻塞**：MapHost catalog入口/原模板/两模式/实际调度/显示/save-load/释放通过 |
| 配置文件/缓存与全工具自审 | 独立JSON运输与原ABI分列；未执行Platform Init/设备；手验单列 | 历史MANIFEST保留 | 最终两入口退出0后核当前源/DLL/artifact hash，根线整表审计再计工具 |

## 精确候选与测试身份

- 待应用共享根线审阅件（只改Map专属源）：`analysis/map-calibration/proposals/base-input-path/base-input-path.patch`，SHA256 `E2E5220E6A2280E3DD0C77A04FBB737BC945805CA5EAA6847511F378ABA8A753`。
- `tests/MapCalibration.PathChecks/Program.cs` SHA256 `723AAE1F8004BFF4BB93DB4C70CCBBD2B20BFB3A982F4A56065F02740153AAFF`。
- `tests/MapCalibration.HostChecks/Program.cs` SHA256 `B005394AA8114DC36E8251530FDE65CE697131A081497A4582843889A2926954`。
- Path失败fixture直接复制原`native/reader-8.xml`（wrong CalibOutputParam container），SHA256 `C3EE8C8378E27ED6AB13D6189B1838373FF3FA1D992BB31C5038D0A8E2E31496`；同hash原module-file-results case8双侧E0000018且旧matrix保留。成功.payload36bytes另有227d0静态binary分派证明；未拿托管短binary安全门充原失败事实。

## 执行命令（获得managed槽、候选应用和全src冻结后）

从当前worktree根目录运行；每个项目单独artifacts，只有生产ProjectReference，无Compile本地候选同名类。

```powershell
$batch = Join-Path (Get-Location) ('analysis/map-calibration/map-path-host-final-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $batch | Out-Null
dotnet build tests/MapCalibration.PathChecks/MapCalibration.PathChecks.csproj --artifacts-path "$batch/path" -p:UseSharedCompilation=false -nodeReuse:false
# 构建退出0才运行；工作目录保持worktree根，参数为私有报告目录。
dotnet "$batch/path/bin/MapCalibration.PathChecks/debug/MapCalibration.PathChecks.dll" "$batch/path-report"
dotnet build tests/MapCalibration.HostChecks/MapCalibration.HostChecks.csproj --artifacts-path "$batch/host" -p:UseSharedCompilation=false -nodeReuse:false
dotnet "$batch/host/bin/MapCalibration.HostChecks/debug/MapCalibration.HostChecks.dll"
```

实际执行需每步独立保留stdout/stderr/退出码，首失败立即定位，不放宽期望、不回旧fallback。Host固定私有输出`analysis/map-calibration/host-checks`，两原图fixture位于现worktree `analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp`与frame-4.bmp；不触原安装图像。若实际artifacts配置目录由SDK版本生成不同，仅据build实际输出定位，不改测试逻辑。Rotate后续，不抢Map完整优先级。

## 2026-10-20 实际运行及新增退出门

根线已应用base patch。Path生产PASS30；Host首次14项后Save失败80100203，诊断显示exists=false/scopeDisplay=false。发现Map `.images`伪moduleId被真实宿主RetainProcedureNodes剪除，根线批准改同一真实Id/node的ScopedState(File+Images)，standalone保持兼容；配置初始化不清成功结果，RemoveNode一起释放。成功旧固定未闭合Message已按根线授权移除，输出/NativeCode/状态未变。

修复后Path30和真实Host21退出0，Save exists=true、document/state/effective path一致、scopeDisplay=true，配置初始化/Save大小写/Export→history→Success/保存重开/RemoveNode通过。两份原fail日志留在map-path-host-final-85e727e1500d4ca98d0a2187e539e18a；不覆盖首差。

**仍不提交完整计数**：Host最终DIAG有GetFrontParamItemsOperation/ImageSource_0/1 Unsupported。实际控件通过shared ImageEnumeration直接service.Enumerate/Bind而可运行，但Map State.Initialize确缺Enum dispatch，这不是无service测试上下文。专属image-source-enumeration.patch SHA640EA2B5254994DBFCD9539F88D0B1A727840FADDA9EFD967C1DFBA16CC0E072已交根线审，等统一编辑窗。Host测试已改为真实WPF ImageEnumeration IMAGE A/B选中绑定，取消两条doc.Connect图像捷径，并新增State无该Unsupported断言；本版尚未运行。当前Path/Host通过是上一个范围的真证据，不能给新测试版本贴PASS。

Map完整退出还需该专属dispatch应用、冻结后新Host含真实两个IMAGE下拉绑定/无未实现初始化通过，再与前述完整矩阵核对+当前source/DLL/artifact manifest提交根线。Rotate已明确交B，本线不运行。

### 初始化结果运输修正（未跑）

根线进一步核Operate明确写EnumEntrysA，故仅调用Enumerate后丢弃tree的640EA中间分派不够。需共享typed tree结果运输/State保存/控件初建消费；由root统一落，不由Map复制实现。新版Host现增加每侧下拉打开前已具IMAGE A/B typed候选断言（Program SHA2C74CEF0C17B0CEDEC35C699EBCC33AB88C462521492D95631B7968E5F543182）。此前“候选应用后新Host”退出条件改为**含结果保存/消费闭环**再跑；本线继续保持未完整。
