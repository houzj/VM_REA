# 相机映射：真实求解输出的精度差异（2026-10-19，第二轮）

本轮将上一轮4个直接输入矩阵的精度差异推进到**真实 CameraMap 求解与原厂模块发布结果**，并增加实际产品 `CameraMappingPlugin.CreateRuntime().ExecuteAsync` 回放。源码和算法均未修改；固定 `<0.003` 对照如实保留失败。便携完整记录见 [输入、原厂输出、产品输出及哈希](CAMERA-MAPPING-REACHABLE-MATRIX.json)。

## 确认的路径

原厂 `CIMVSCameraMapModu::ProcessAlg`（`18000bd20.c`）按模块点缓存向正常创建的 CameraMap SDK BasicParam 添加点对、执行 Run，读取目标矩阵送入 MatrixToParam，执行其 Process；`ProcessOutput`（`18000c450.c`）把 CameraMap SDK 比例和 MatrixToParam 六字段发布到 ModuleFrame。既有 `ModuleProbe.cpp` 的受控宿主路径只替换 ModuleFrame 输入/输出/运行时 imports，没有修改模块算法代码、SDK、许可证或平台身份。

新探针 `ReachableMatrixModuleProbe.cpp` 复用这一已验证路径，给每个样本创建独立原厂模块和正常 SDK 对象，通过真实模块 `SetParam/Process` 执行。输入与输出经过 synthetic ModuleFrame；随后直接读取同一模块持有的 CameraMap `GetTargetMatrix` 和 MatrixToParam 六个 Getter，均输出17位数值。没有执行平台 Init，也没有设备操作。

取上一轮 seeded-projective-15/16/33/61 四矩阵，在 object 范围10、1000、10000上各生成2×2及4×4网格，用矩阵投影生成 target 后转 float。24组点集各4或16点，全部已保存，不以输入矩阵当作算法精确真值。小object范围与大target关系包含刻意放大的病态情形；原厂与产品各自求解后才比较。

## 点数组与原厂配置域

原厂 XML 的 `Data1/Data2 Fixed=True`，添加按钮 `MaxCount=8` 限制点组数量。最终回放使用这两个固定默认活动组，将点序列前半放 Data1、后半放 Data2，其他组关闭；每组的 Object/Target X/Y 均为等长 float 数组，保持原厂按组展平顺序。4点样本是每组2点，16点样本是每组8点。

产品 Descriptor 将这些叶声明为 NumberArray，`CameraMappingSolver.Solve` 接受总计2..32点；本轮全部输入通过实际产品 Runtime 成功执行。16是点数而非组数，没有越过8组限制。本轮没有打开 WPF 来源选择器，故不把 API/运行输入域的成功升级为原厂桌面交互验收。

## 分开的两类差异

1. **固定使用原厂实际求得H，再比较分解。** 原厂24/24次Process成功，同一模块的六个发布字段与同一 MatrixToParam 结果对象 Getter 在24组全部 float 位相同。将该实际H送入当前产品源码的独立MathF快照，只有5组六字段全部位同，9组偏移不满足 `<0.003`。最大例 `seed-33-extent-1000-grid-4`：原厂 TranslateX `-599322.625`，快照 `-599322.6875`，差 `0.0625`。9组的首个越阈值字段为 TranslateX 或 TranslateY，完整先后顺序见记录。这证明上一轮差异域可由真实CameraMap点集求解达到。
2. **相同float点集走当前产品完整求解及分解。** 隔离源码快照显示14组H系数不满足0.003、18组分解不满足0.003。实际产品 Runtime 在默认固定两组回放中24/24均成功，核验216个原厂发布字段，18组不满足`<0.003`；4点12组中6组存在差异，16点12组全部存在差异。这些差异不能全部归因于一个ULP。最明显的16点病态样本 `seed-15-extent-10-grid-4`，首个越阈值字段为 ScaleX（差约0.2097636），TranslateX差 `104105.375`。此处已有求解阶段差异；未定位原生核心指令前不能仅修改分解公式声称修复。

`tests/CameraMapping.ReachableChecks` 是实际项目引用，调用生产 CreateRuntime/ExecuteAsync，不调用复制Runtime或窗口模拟。输出JSON记录每次状态、点/组数、九字段、绝对差和首次差异；差异时退出1。该诊断不覆盖GraphExecutor、WPF或原厂平台订阅。

## 复现和当前边界

- 原厂：`powershell -NoProfile -ExecutionPolicy Bypass -File tests/CameraMapping.NativeProbe/Run-ReachableMatrix.ps1`。
- 源码快照：用PowerShell 7执行 `tests/CameraMapping.NativeProbe/Compare-ReachableMatrixSnapshot.ps1`。
- 实际产品：`dotnet run --project tests/CameraMapping.ReachableChecks -c Release --artifacts-path analysis/camera-mapping/reachable-matrix-20261019/artifacts`。在协调的共享构建时段运行，独立bin/obj；当前退出1是原厂对照差异，不是构建失败。

产品三源文件哈希在运行前后不变；原厂/产品源、原厂三个DLL、探针、脚本、每组点输入及输出均在便携记录中保留SHA256。没有修改公共求解器、产品算法、容差或另一worktree，没有提交推送。

需要后续闭合：原厂MatrixToParam核心求逆/归一化指令顺序；CameraMap归一化/加权迭代在病态样本中的首个分叉；实际Graph/宿主和原厂桌面同状态验证。reset、原生订阅索引仍未取得新证据。此前118组求解差分只证明其旧样本，本轮不得据其通过宣称全域等价。
