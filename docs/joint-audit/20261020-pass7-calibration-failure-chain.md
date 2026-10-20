# 第七轮：标定文件失败、短矩阵与 Group 传递

范围：标定分类的标定加载与坐标系，以及两者之间必需的 owned Group/GraphExecutor 边；图像源仅作坐标系必需的 2×2 GrayImage 夹具。复用第六轮 N点写出正常九项矩阵的证据，本轮专查随后文件重读与矩阵长度变化。没有在原版 VisionEditor 桌面运行本整图。

## 原源码链与适用边界

原 `ReadCalibFileModu.dll` SHA-256 `86B8B112ADED58DE27A41A5EDE6EC6B41117D7DB8D0C5934647FA9514E6FA403`。`Process` RVA `0xc520` 在 `RefreshSignal` 非零时调用虚表 `SetParam("FilePath", storedPath, length)`，忽略重读错误而继续按当前缓存集合发布输出，`ModuStatus` 由输出集合是否非空决定；C 体 `worktrees/tool-plugins/analysis/full-audit/read-calib-file/native/18000c520.c` SHA-256 `E3C6315832CE99019996B39A5BDC7B4CB10867E8C35C7C1F132092353750DC2A`。`SetParam` RVA `0xd120` 对非空路径调用 `LoadFile`，空路径清缓存；体 SHA-256 `35DC0C1699E08DFCE863173B529FBE8BA4E0579BE5D6CE1D3167AEE92D2F57F5`。`LoadFile` RVA `0xab50` 在打开/解析文件失败时先返回 `0xe0000201`，旧输出集合尚未清除；成功解析后清集合，缺 `CalibOutputParam` 返回 `0xe0000017`，因此这是**清除后的失败**，不是与读失败相同的保留分支；体 SHA-256 `EC0CFDED878E3B054A65F8F83550364D74503B59D792069D0566C42D91231626`。这些分支也有既存自有 ModuleFrame 受控原厂探针 `docs/plugins/read-calib-file-evidence.json`，不是原平台 Group 运行。

原 `CoordinateModule.dll` SHA-256 `8547DF5CEF905FB824A068359DD49B7B5FEFA5100A061D6555A993D882666AF6`。`GetProcessCalibMatrixInput` RVA `0x40180` 接受 `Count=1..9`，将该数量传入 `0x395b0`，只覆盖矩阵前缀；C 体 `worktrees/tool-plugins/analysis/coordinate-module/native-source/output/180040180.c` SHA-256 `A05318B23DD5A51B6D1CE7179E05D3DAD300619BEDCF3C118323ADEA425C7FC5`。`0x395b0` 对 `Count=1` 写首项，较长数组以原 `VM_M_BatchGetFloat` 批取；体 SHA-256 `7A22FAEF27B95568F04E119A3058C8314FEEE80E44D7A1F2C4C61443ABF7C9A1`。原实例矩阵初始尾部未初始化，不能从允许短输入推断首轮完整矩阵/坐标可用；已有九项后接短项的尾部保留则由写入范围支持。当前坐标系的未知尾部显式拒绝是独立安全适配，**不冒称原厂报同一错误码**。

## 当前真实图复验

探针 `tests/CalibrationFailureChain.Checks/Program.cs` SHA-256 `425BA91ADBD1775720F4D059AEBD9DDF93CCFAB47F5E1C9EDBDAD2F0D6F48B18`，工程 SHA-256 `34BB43761C26C5791F413357F88B89A8C654EF850043CE3A18CD1D488AF552A4`。命令：`dotnet run --project tests/CalibrationFailureChain.Checks/CalibrationFailureChain.Checks.csproj -c Release`，exit 0。原始 stdout `analysis/joint-audit/pass7-cal-failure/checks.log` SHA-256 `5934D860FD609E4EC44C65C31118ACE9F9565014B67125D5EAA0B38397E66222`。图为 `标定加载(RefreshSignal=1 订阅常数) → Group.Matrix → 坐标系(InputWay=2)`，同一 `ModuleExecutionScope` 依次运行六次，后对长度 1..8 每次新建冷 scope。逐状态如下：

| 情形 | 标定加载 | Group | 坐标系 | 可见含义 |
|---|---|---|---|---|
| 九项有效文件 | Success、Status=1、9 项 | Success | Success、9 项 | 基线成立。 |
| 同路径文件删除后刷新 | Success、旧 9 项 | Success | Success、9 项 | 读失败保留接受过的缓存。 |
| 同路径坏 XML 后刷新 | Success、旧 9 项 | Success | Success、9 项 | 解析失败也保留。 |
| 合法 XML 但缺输出节 | Success、Status=0、无矩阵 | Failed `SubprogramFailed`，子流程 `Matrix: Current-run source unavailable` | Skipped `ExecutionPredecessorUnavailable` | 模块本身正常完成但 Group 必需输出缺失，整 Group 失败。 |
| 修复为九项并刷新 | Success、9 项 | Success | Success、9 项 | 同 scope 恢复成功。 |
| 已有九项后改为八项 | Success、8 项 | Success、8 项 | Success、完整 9 项 | 新前缀第 3 项为 5，旧尾第 9 项为 1：同时证实前缀更新与已写尾项保留。 |
| 新 scope 首次读取 1..8 项 | Success、对应长度 | Success | Failed `NativeUninitializedMatrix` | 当前安全适配不发布不定尾部；八个长度均逐项测过。 |

`GraphExecutor` 的动态输出核验和 `WorkflowRunContext.Complete` 的 Group 输出读取共同决定缺输出节的失败传播。状态与错误码是**当前实现实测**；原版真实 VM Group 在同样动态输出消失时的调度、节点状态和界面呈现未取证，不能直接称等价。当前实现 `ReadCalibFilePlugin.Runtime` 忽略 `LoadError` 后按缓存发布，与原模块可观察分支一致；`CoordinateSession` 的短矩阵尾部跟踪复用已知九项，首轮未知尾不伪造数值。没有本轮有充分原版合同依据且属于 main 独占的产品修复。

## 后续具体缺证

1. 原版 Group 用标定加载动态 `CalibMatrix` 输出消失时的 `SetGroupIO`/执行/状态实际调用链或自有受控宿主复验，才能给主框架此失败状态下的等价结论。
2. 原版坐标系短矩阵首轮的完整结果受未初始化 native 堆尾部限制；不把任意一次堆值当通用规格。若需数值差分，先固定九项已写状态，再比较 1..8 项更新及 SDK 输出。
3. 当前 `GraphExecutor` 对 Group 内动态端口消失的旧连线/订阅传播，本探针仅测 Group 输出引用无值的路径；端口改名、改型等另一条动态旧连线分支已有 `ReadCalibFile.PluginChecks`，不能由本报告一案替代。

本轮只增专属可复验探针与审计报告；未修改共享产品、原件或 `tool-plugins` 工作树。自动测试不计原机人工验收。
