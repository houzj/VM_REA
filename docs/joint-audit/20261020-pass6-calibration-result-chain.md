# 第六轮：标定结果生产→加载→Group→坐标系的源码链

范围仅为标定分类的 N点标定、标定加载、坐标系，以及它们必需的主框架传输。图像源只提供坐标系必需的图像夹具，本报告不审核图像源工具。此处核的是代码、资源、受控探针，未在原版 VisionEditor 桌面复刻同一整图。

## 原版入口与数据形状

| 链段 | 原始证据与具体行为 | 边界 |
|---|---|---|
| N点求解→文件 | 安装原件 `IMVSNPointCalibModu.dll` SHA-256 `F8E07023A816C55FF07906E8889F45B93658F59CE88F040AD2204A70922F4839`；其 `ProcessAlg` RVA `0x3caf0` 在求解成功、取得结果、`RefreshFileEnable` 且路径非空时调用 `WriteXmlData`。反编译体 `worktrees/tool-plugins/analysis/full-audit/n-point-calibration/native/18003caf0.c` SHA-256 `7122FB3E237265EF0CDC5769F17FEC155C15E776147CA0FEFC54EBB6FE91C31E`。`WriteXmlData` RVA `0x3f990` 构造 `CalibOutputParam/CalibMatrix/FloatList`，循环写九项，体 SHA-256 `E486388CFE53391EC7A756C221D6A5EF156DAB9C5F4F88727B6579C5688DB8F2`。 | 原 `IMVSNPointCalibModu.xml` SHA-256 `246014967ABD4311C77A904FBF02636F557BF61C25BECA8F051243DC0C07C25F` 声明 `CalibStatus` 等输出，**不声明**当前独立实现的 `CalibData` 对象端口。不能把 `CalibData` 直接当作原版矩阵叶。原自有进程文件证据为 `docs/plugins/n-point-file-evidence.json`，不是原桌面整图。 |
| 文件→标定加载 | 安装原件 `ReadCalibFileModu.dll` SHA-256 `86B8B112ADED58DE27A41A5EDE6EC6B41117D7DB8D0C5934647FA9514E6FA403`；`Process` RVA `0xc520` 读 `RefreshSignal`，非零时经 vtable `SetParam(FilePath)` 触发重载，再按已解析结果发布 int/float/string/FloatList。体 `worktrees/tool-plugins/analysis/full-audit/read-calib-file/native/18000c520.c` SHA-256 `E3C6315832CE99019996B39A5BDC7B4CB10867E8C35C7C1F132092353750DC2A`。原 `ReadCalibFileModu.xml` SHA-256 `D51DD352C7E7CA03781876625E0473AAF2BE74FC276761E588B85AC922D4CB14` 声明可选 `RefreshSignal:int`、必需输出 `CalibMatrix:float[]`。 | 受控模块事件、刷新失败保留、动态 schema 见 `docs/plugins/read-calib-file-evidence.json` 和 `docs/plugins/READ-CALIB-FILE.md`。原动态 IO 在真实 VM Group 内的所有状态未在此探针中运行。 |
| 矩阵→坐标系 | 安装原件 `CoordinateModule.dll` SHA-256 `8547DF5CEF905FB824A068359DD49B7B5FEFA5100A061D6555A993D882666AF6`；原 `CoordinateModule.xml` SHA-256 `116D4BCB2CDFEF0E2E380C527B6666F1F11C7D1B4F6D5640B1C281E2745E9218` 声明可选 `CalibMatrix:float[]`、必需 `InputImage` 和输出 `CorCalibMatrix:float[]`。`Process` RVA `0x3f4a0` 在 `InputWay=2` 调 `GetProcessCalibMatrixInput` RVA `0x40180`：Count 只接受 1..9，底层首项读取再批量读取；不是 `CalibrationData` 对象入参。两体 SHA-256 分别 `3420E2291FD782FFDAF6BA801F834BE8385239BEDCFF36C4EA4CC1A16C850A68`、`A05318B23DD5A51B6D1CE7179E05D3DAD300619BEDCF3C118323ADEA425C7FC5`。 | 原首轮少于九项的尾部矩阵堆内容不可断定；该不定状态未作为当前实现的等价要求。另见 `docs/plugins/COORDINATE-MODULE-RUNTIME-SOURCE.md`。 |

三个原版模块的公共链是**标定文件中的 float 数组**。在本测试图中，把 N点 `CalibStatus:int` 连到标定加载 `RefreshSignal:int` 仅作执行顺序和刷新触发；矩阵经文件重读进入加载器输出，再跨 Group 送坐标系。此组合连法由各模块原源码和声明支持，但原版完整 VM 同图运行尚未取证，不能把当前图通过称作原桌面证明。

## 当前源码与实际探针

当前 `NPointPlugin.Runtime` 求解后写 `NPointFile.Write`，生成 `CalibMatrix` 九项；`ReadCalibFilePlugin.Runtime` 按 `FilePath/RefreshSignal` 重读，发布 `NumberArray`；`GraphExecutor` 用数据边排序、校验类型和动态描述；`WorkflowRunContext` 完成 Group 输出；`CoordinateModulePlugin.ReadMatrix` 消费 `NumberArray` 并调用 `CoordinateSession.Process`。主框架和插件之间的传输使用独立的 `DataKind.NumberArray`；原版 `float[]` 在每个插件边界按 float32 处理。

探针 `tests/CalibrationResultChain.Checks/Program.cs` SHA-256 `88DDC56263294C11952F437FCFD0A3BA626979AA461D30CF7874B44B6BAC2212`，工程 SHA-256 `34BB43761C26C5791F413357F88B89A8C654EF850043CE3A18CD1D488AF552A4`。命令：`dotnet run --project tests/CalibrationResultChain.Checks/CalibrationResultChain.Checks.csproj -c Release`，退出码 0；原始日志 `analysis/joint-audit/pass6-cal-result/checks.log` SHA-256 `AB5D993788D43D92287431E17D303E71555A08F413340AB346A15381682E8B0F`。探针由已有 SDK fixture `state-rotation-three` 填入 N点手动点表，受控路径下实际执行两次：

1. 初次：N点成功、`CalibStatus=1`、写出 XML；标定加载成功并发布九项矩阵；Group 成功且所传九项与加载器输出逐项完全一致；坐标系成功并发布九项 `CorCalibMatrix`。生产者矩阵与文件矩阵按现 `G8` float 文本精度比较，非强行字节相等。
2. 保存整个方案为 `calibration-chain.vmx.json`，新建 `GraphEditorDocument` 加载、重建运行 scope 后复跑，四节点状态和上述载荷条件再次通过。保存方案和 XML 均位于被忽略的 `analysis/joint-audit/pass6-cal-result/`。本探针保留源文件；不证明搬迁方案或删源文件后的缓存恢复。
3. 独立负例在空图尝试 `NPoint.CalibData → Coordinate.CalibMatrix`，`Connect` 拒绝且连接数仍为零。这阻止把当前独立 `CalibrationData` 误当成原 `float[]`。`CalibData` 是当前便利合同，原 `NPoint` XML 无此端口。

运行产物 `npoint-generated.xml` SHA-256 `C04A0EB0B5D3C6DD45F62ABEF63184E286D0234D5FC1911012123E36E2175601`，`calibration-chain.vmx.json` SHA-256 `8C1A769AD8318ACCFFB1989C67CBD1C49A0B2DDD350F4233CF21418927E83CAD`。这些哈希只标识当前构建的实例，不是原版文件的预期 SHA；重复运行的生成时间会改变它们。

## 本切片判定及下步

当前正常九项矩阵链及保存重开自审通过，**无需无依据的产品修补**。该结论仅覆盖一个 N点数据集、文件正常读写、Group 单次输出和坐标系 Matrix 模式。原版整图 Group/订阅的同状态执行、路径不存在或刷新失败跨 Group、不同标定生产者的文件矩阵、1..8 项坐标矩阵及原生完整算法差分仍未由本探针证明。后续十工具链审核应复用本链的类型/文件/Group 事实，不为每个工具重复审核通用 GraphExecutor；各工具只查独有结果形状和异常分支。`tool-plugins` 在制工作树只读取既有原始反编译证据，未修改。
