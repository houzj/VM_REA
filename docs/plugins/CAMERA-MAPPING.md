# 相机映射逆向清单

对应原厂 `IMVSCameraMapModu`，属于独立工具，不是坐标系。当前工具专属实现已完成下列可验证范围；公共生效参数读回已接入；整体仍待真实宿主完整验收，不能据测试通过宣称全部原厂等价。机器证据见 [camera-mapping-evidence.json](camera-mapping-evidence.json)。

| 清单 | 当前结果 | 验证与边界 |
|---|---|---|
| 原厂身份、声明、语言、图标 | 已取证 | REA托管身份、原厂/产品文件SHA；嵌入AlgorithmTab与安装文件字节一致 |
| PointPair配置与顺序 | 已实现 | 8组、前2组固定、原始48子字段身份；增删、缓存初始顺序挑隐藏项、显示编号与模式切换 |
| 删除顺序与失败 | 已实现并区分 | 按源码先Object/Target组合，再ObjectX/TargetX/ObjectY/TargetY；服务失败后停止并保持显示属于托管独立安全规则，非原生整组回滚证明 |
| POINT分量别名与数组 | 已接入公共能力 | 全16目的名与原XML一致；1/2为PointInputN，3–8为InputN；实际Graph全8组32条叶、16点执行通过；SDK封装名称差异单列 |
| 原厂SDK算法 | 118组差分通过 | 固定0.003绝对矩阵/比例阈值；2点反向相似、3点正向仿射、float归一化/逆投影/IRLS/嵌套正规方程；不是位级一致声明 |
| 官方模块受控运行 | 已补证 | 正常官方算法对象+synthetic ModuleFrame IO，不调用平台Init；透视/数组9输出、文件、缓存与异常顺序 |
| 模块错误边界 | 已实现 | active但全空0xe0000309；无active组0xe00000ff；部分叶缺失/数量错0xe0000001；1实际点0x80100005；MatrixToParam零尺度0x1000010f |
| 标定文件与缓存 | 已实现 | 36字节9float；路径只设置及PreExecuteRun、Refresh写入；显式保存按当前参数重算；原厂缓存容量不缩小的尾点行为已复现 |
| 保存失败资格 | 已实现 | MatrixToParam失败不可保存0xe0000018；自动写IO失败后算法标记仍有效，可换路径显式保存 |
| 配置reset与读回 | 工具范围已修正 | 单事务宿主复位；清3个原厂ISubInfo参数的模式/缓存绑定；取消/拒绝不改私有值；真实Core复位删参数绑定、保留32个POINT叶及输入模式 |
| 缺失组合force操作 | 原厂忽略已确认 | 官方parser实际加载16个null操作触发无异常，工具专用无操作rule匹配；不发明必填投影 |
| 当前WPF/Graph检查 | 109项通过 | 实际生产窗口/控件、配置执行、全8组、模式、reset、路径与导出、跨流程隔离、官方模块比较 |
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

统一生效参数读回已消费。工具注册、流程执行与窗口生命周期已接线；109项相机专属检查、118组原厂SDK差分和23项实际宿主检查通过。CMD桌面窗口启动后，用户已人工检查并通过三个页签、点/坐标模式、分组、执行、复位及保存文件。本轮相机映射工具闭合并提交；设备测试未执行。

公共显示已声明DisplayInputBindings/DisplayObjectSettings并注册共享对象控件；Core位置常量27与实际显示PositionX=27已验证，Reset保留显示设置。实际普通窗口物化三个标签页后Diagnostics无未注册模板。人工验收证明当前窗口基本交互可用，不声称原厂桌面逐像素完全一致。
