# 相机映射逆向清单

对应原厂 `IMVSCameraMapModu`，属于独立工具，不是坐标系。当前工具专属实现已完成下列可验证范围；公共生效参数读回已接入；整体仍待真实宿主完整验收，不能据测试通过宣称全部原厂等价。机器证据见 [camera-mapping-evidence.json](camera-mapping-evidence.json)。

| 清单 | 当前结果 | 验证与边界 |
|---|---|---|
| 原厂身份、声明、语言、图标 | 已取证 | REA托管身份、原厂/产品文件SHA；嵌入AlgorithmTab与安装文件字节一致 |
| PointPair配置与顺序 | 已实现 | 8组、前2组固定、原始48子字段身份；增删、缓存初始顺序挑隐藏项、显示编号与模式切换。2026-10-19复核 `AddButton_PointPair.AddPointPair`：先将隐藏组移动到添加按钮前，再写可见状态；插件已按相同次序执行 |
| 删除顺序与失败 | 已实现并区分 | 按源码先Object/Target组合，再ObjectX/TargetX/ObjectY/TargetY；服务失败后停止并保持显示属于托管独立安全规则，非原生整组回滚证明 |
| POINT分量别名与数组 | 已接入公共能力 | 全16目的名与原XML一致；1/2为PointInputN，3–8为InputN；实际Graph全8组32条叶、16点执行通过；SDK封装名称差异单列 |
| 原厂SDK算法 | 118组差分通过 | 固定0.003绝对矩阵/比例阈值；2点反向相似、3点正向仿射、float归一化/逆投影/IRLS/嵌套正规方程；不是位级一致声明 |
| 官方模块受控运行 | 已补证 | 正常官方算法对象+synthetic ModuleFrame IO，不调用平台Init；透视/数组9输出、文件、缓存与异常顺序 |
| 模块错误边界 | 已实现 | active但全空0xe0000309；无active组0xe00000ff；部分叶缺失/数量错0xe0000001；1实际点0x80100005；MatrixToParam零尺度0x1000010f |
| 标定文件与缓存 | 已实现 | 36字节9float；路径只设置及PreExecuteRun、Refresh写入；显式保存按当前参数重算；原厂缓存容量不缩小的尾点行为已复现 |
| 保存失败资格 | 已实现 | MatrixToParam失败不可保存0xe0000018；自动写IO失败后算法标记仍有效，可换路径显式保存 |
| 配置reset与读回 | 工具范围已修正 | 单事务宿主复位；清3个原厂ISubInfo参数的模式/缓存绑定；取消/拒绝不改私有值；真实Core复位删参数绑定、保留32个POINT叶及输入模式 |
| 缺失组合force操作 | 原厂忽略已确认 | 官方parser实际加载16个null操作触发无异常，工具专用无操作rule匹配；不发明必填投影 |
| 当前WPF/Graph检查 | 110项通过 | 实际生产窗口/控件、配置执行、全8组、模式、reset、路径与导出、跨流程隔离、官方模块比较；新增真实添加按钮回归，确认节点移动先于可见标记提交 |
| 生效参数读回 | 已接入公共API | ReadEffectiveParameters同步既有参数，导出按有效模型重算；20seed/50effective解绑及无关UserData写入保留50；根线另报实际宿主19检查通过 |
| 原生索引取值 | 未闭合 | 托管来源/载荷已取证；独立平台MVD Init被0x80100015阻断且观察共享ImageTemp清理副作用，已停止启动；索引仍Unsupported |
| 原厂桌面/实际宿主验收 | 待根线与用户 | 原厂parser模型不等于原厂桌面截图；源派生截图只证明渲染。原生平台/远端文件协议与完整像素等价未验证 |
| 设备测试 | 不执行 | 用户明确不做设备测试 |

## 重现与证据

- `tests/CameraMapping.NativeProbe/Run.ps1`：118例SDK；`-Module`：独立受控模块，包含8组配置之外的输入与文件边界。
- `RunParser.ps1`：官方parser、原XML副本及synthetic Owner/参数，无原生模块；组合操作工厂/触发和安全索引载荷。
- `RunGhidra.ps1`：只读复用已有Ghidra/JDK，数据库/缓存/输出全部在本工作树；CLI证据与REA明确区分。
- `dotnet run --project tests/CameraMapping.SolverChecks`、`dotnet run --project tests/CameraMapping.PluginChecks`；失败返回非零，不下调阈值。
- `ExportEvidence.py`：源/资源/CLI哈希、官方输出与36字节样例、实际检查manifest。

专项说明：[文件与缓存](CAMERA-MAPPING-FILES.md)、[POINT关联](CAMERA-MAPPING-POINT-ALIASES.md)、[原厂parser与索引](CAMERA-MAPPING-PARSER-INDEX.md)、[接口请求及验收](CAMERA-MAPPING-CONTRACTS.md)。

## 实际宿主验收结果与边界

统一生效参数读回已消费。工具注册、流程执行与窗口生命周期已接线；当前110项相机专属检查、既有118组原厂SDK差分和23项实际宿主检查通过。CMD桌面窗口启动后，用户已人工检查并通过三个页签、点/坐标模式、分组、执行、复位及保存文件。本轮相机映射工具此前已闭合提交；本次仅修复新增节点操作顺序并增加回归，设备测试未执行。

2026-10-19添加顺序复核：反编译 `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/AddButton_PointPair.cs` 的 `AddPointPair()` 先将隐藏项从 `Category.ItemsA` 和 `Tab.Collection` 移除、插至 AddButton 之前，再执行 `pointPair.CustomVisibleA = true`；`PointPair.cs` 的 `CustomVisibleA` setter 随后写 `Name#1` 并刷新显示序号/删除状态。两原始文件 SHA256 分别为 `46c164ac53d170c95d4c7767d69f09a3075c6be753cdfb471b058dd161095645` 与 `78997b8da0cb63e64976d63a2d47f36c2f1d1f5a10706b3822de720c7cdc0017`。新增回归通过真实 `ToolConfigurationModelWindow` 点击“添加点对”，在实际配置参数 Apply 回调检查 Data3 已移动到 Data8 之后且紧邻 AddButton、同时仍隐藏；随后才提交 Data3#1。此次目标控件文件 SHA256 为 `a8162a618a04480795feb10351756f5c28392819762f54a42deedc2c86577a26`。

公共显示已声明DisplayInputBindings/DisplayObjectSettings并注册共享对象控件；Core位置常量27与实际显示PositionX=27已验证，Reset保留显示设置。实际普通窗口物化三个标签页后Diagnostics无未注册模板。人工验收证明当前窗口基本交互可用，不声称原厂桌面逐像素完全一致。

2026-10-19 四线续进：直接 MatrixToParam 输入域新增95例高精度差分，发现4例大坐标透视偏移超过0.003，最大0.0625；仅是产品公式快照与原厂SDK的精度差异，实际CameraMap求解和宿主可达性未验证，生产算法未改。详情见 [专项差分及交接](CAMERA-MAPPING-MATRIX-DIFFERENTIAL.md)。

第二轮实际可达性：默认两个固定活动组、每组等长数组的24组原厂Process与实际生产Runtime全部执行成功；216发布字段固定0.003对照18组存在差异（4点6/12、16点12/12），已确认不能由旧118组样本外推全域等价。未改产品算法；详见[真实求解输出及生产Runtime差分](CAMERA-MAPPING-REACHABLE-MATRIX.md)。

B1源码批：已补原厂7fcf60→7fd4e0之间float反归一化/再归一化，24组求解H和比例源码诊断逐位同，真实Runtime仍9组分解偏移差异（由18降9，阈值不变），118组SDK对照通过。见[源码批B1及残余边界](CAMERA-MAPPING-B1-SOLVE-SOURCE.md)。

B2源码批已补MatrixToParam核心800af0及原指令，修复float归一化和投影专属逆矩阵乘积顺序。最终实际Runtime24例/216字段全部满足固定0.003，142专属检查通过；118 SDK求解118/0。95直接矩阵只作源码诊断（93成功全匹配，2异常单列），不称全域/宿主等价。见[B2源码、实施及剩余边界](CAMERA-MAPPING-B2-MATRIX-SOURCE.md)。
