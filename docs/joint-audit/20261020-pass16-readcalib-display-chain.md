# 第十六轮：标定加载动态输出、Group 与显示刷新

仅核标定十工具中的 `ReadCalibFileModu`：当前实际 owned Group 图中有效文件、文件失效、输出叶改名及恢复时的状态记录与旧显示。第十二轮已核动态 schema 的 Group 连线三态，本轮只追加 `J08/J10` 显示交接和一个当前有限值安全失败边，不把它说成原厂 NaN 行为。

## 原件依据

- 安装原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/ReadCalibFileModu/ReadCalibFileModu.dll` SHA-256 `86B8B112ADED58DE27A41A5EDE6EC6B41117D7DB8D0C5934647FA9514E6FA403`；原 `ReadCalibFileModu.xml` SHA-256 `D51DD352C7E7CA03781876625E0473AAF2BE74FC276761E588B85AC922D4CB14`。原 `ReadCalibFileModuDisplay.xml` SHA-256 `2CEFA54D36374DD83E15742AFD18574C64A04DC0603B2D77177EAD9EA5F29DF1` 活动对象只有 `Data Record`，Content 为 `Module Status:{0}`、映射 `ModuStatus`；Show Text 位于 XML 注释，不是活动图层。此显示资源不包含动态矩阵名、图像或列表对象。
- 原 `CMyAlgorithmManager::Process` RVA `0xc520` 取 `RefreshSignal` 后可经 `SetParam(FilePath)` 刷新；按已缓存输出集合逐项发布，再以集合非空写 `ModuStatus`。取体 `worktrees/tool-plugins/analysis/full-audit/read-calib-file/native/18000c520.c` SHA-256 `E3C6315832CE99019996B39A5BDC7B4CB10867E8C35C7C1F132092353750DC2A`。原 `LoadFile` RVA `0xab50` 读盘失败在清集合前早退，合法 XML 但缺 `CalibOutputParam` 可清旧输出集合，取体 `18000ab50.c` SHA-256 `EC0CFDED878E3B054A65F8F83550364D74503B59D792069D0566C42D91231626`。原自有实例对应事件记录于 `docs/plugins/READ-CALIB-FILE.md` 与 `docs/plugins/read-calib-file-evidence.json`；这些证明原动态集合刷新分支，不证明原 MainWindow 在失败运行后如何保留上一帧显示。

## 当前链和实测

`src/VisionPlugins.Calibration/ReadCalibFilePlugin.cs` 的 `Runtime.ExecuteAsync` 在刷新后从 `state.Data.Values()` 建本轮输出，动态生成 `UpdatedDescriptor`；`ModuStatus` 仅由 `state.Data.Outputs.Count>0` 决定，成功末尾才更新 `state.LastInputs/LastOutputs`。`LastDisplay` 用此快照按公共 `ToolDisplayService` 建 Data Record，并声明 `TextVisible=false`、`ImageVisible=false`、`ResultListVisible=false`。主框架 `src/VisionEditor/ToolDisplays.cs` 的 `RefreshToolDisplay` 会先 `Show(null)` 清三个 presenter，但只要本轮 `results.ContainsKey(node.Id)`，包括失败记录，仍会调用 provider `LastDisplay`；若插件在失败早退时不更新快照，旧帧可回到主窗。这里最后一步是源码数据流判断，非用户可见桌面画面实测。

`tests/CalibrationReadCalibDisplayChain.Checks/` 构造 ReadCalibFile 位于 owned Group 内，Group 的 `Matrix:NumberArray` 固定关联子节点 `CalibMatrix`，每轮发送 `RefreshSignal=1`。首轮有效九 float 文件：子节点和 Group 成功、Record `Module Status:1`，图像/文本/列表均隐藏。把同路径改为畸形 XML：读取失败保留此前可用集合，Group 仍成功且 Record 仍为 1。改为合法但无输出节：子节点本身成功并发布状态 0，Group 因固定矩阵叶消失而 `SubprogramFailed` 且无出口，Record 被本轮 `Module Status:0` 替换。改名为 `RenamedMatrix`：集合非空，子节点状态及 Record 回到 1，但 Group 旧矩阵选择仍失败；这并非显示错误，资源只显示集合状态。文件名/类型恢复后 Group 再成功。

另外，将有效文件的矩阵首值写 `NaN`，当前 `ReadCalibFileData.Values()` 在有限值 PortValue 边界失败，子节点 `Failed/INVALID_CALIBRATION_NUMBER`、Group `SubprogramFailed`。由于该早退分支不更新 `state.LastOutputs`，`LastDisplay` 实际仍返回此前的 **`Module Status:1`**。当前主窗的失败记录存在门会让 provider 被调用，因此这是可证的当前**旧状态记录回灌风险**；不能把该记录视为本次失败结果，也不能推断原 native 对 NaN 的解析或展示。再次恢复有效文件时同 scope 成功。另一个未执行 ReadCalib 节点 ID 的 `LastDisplay` 为 null，表明该旧记录来自同节点 State 而非跨节点串用。

独占构建槽首轮运行 `dotnet run --project tests/CalibrationReadCalibDisplayChain.Checks/CalibrationReadCalibDisplayChain.Checks.csproj -c Release` exit 0，共 12 项 PASS。日志 `analysis/joint-audit/pass16-cal-read-display/first-run.log` SHA-256 `FC18F519D6F0C57A815FFB36679E457E783BF3851F56369E5EEEB5F244180C00`；探针源 `tests/CalibrationReadCalibDisplayChain.Checks/Program.cs` SHA-256 `A304AA75C5D9EF3142C50A62081DD249120398E5A725AC00AAF18F8B6C5C57E9`，项目文件 SHA-256 `7D1D99B7A5D7E975CB7D94C6876D64F6AD7692639BA938247A3E9478E362099A`。

## 收口与限制

可关闭当前有效/畸形/合法无输出/动态改名/恢复的状态→Group→Data Record 代码级交接。`NaN` 早退留下旧显示是当前独立有限值安全分支的真实缺口，应由插件所有者在其既有显示快照框架内审查；主项目不新增第二套插件显示处理。原生非有限值、原厂失败时历史端口与真实主窗显示时序未证；当前 MainWindow 用户桌面切选也未做实际可见窗口验收。原件与 tool-plugins 在制文件只读，未改产品/shared，未提交。
