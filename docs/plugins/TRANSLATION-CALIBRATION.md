# 平移旋转标定 TranslationCalibModu

本工具依据原厂模块、SDK、配置资源和自有进程受控运行实现。没有设备测试；原厂完整桌面参数面板尚未成功渲染。受控输入输出宿主与真实平台运行分别记录，不能由回放通过推导全部原生等价。

## 原厂身份与来源

- 安装目录：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/TranslationCalibModu`，13 个文件逐项 SHA-256、长度、XML 配置见 [静态清单](translation-calibration-definition.json)。
- `TranslationCalibModu.dll` SHA-256：`a85a7ca08ea265b2cf881b4ff01bf7538a60e9edb7466478476584b08fe25183`，410624 字节。原生导出类仍叫 `CIMVSNPointCalibModu`，不能仅凭类名将其当成 N点模块。
- 原厂托管控制与 SDK 辅助反编译：本工作树 `analysis/full-audit/calibration-dependencies/decompiled/TranslationCalibModuControl`、`TranslationCalibModuCs`。原始代码不纳入 Git。
- REA 托管静态证据：`ev_c9e003eb9b1b760c6e3c973ae1c3a3c9b9ae2b05bfde4b899c01820c5ab8eb2a`，完整原始返回保存于本工作树 `analysis/full-audit/translation-calibration/managed-rea-evidence.json`。静态元数据检查没有执行控件。
- 独立 Ghidra 数据库及导出：本工作树 `analysis/full-audit/translation-calibration/ghidra-project`、`native`，没有打开其他任务的 N点数据库。原厂 DLL、算法和授权代码保持原样。

| 原生方法 | RVA | 本次依据 |
|---|---:|---|
| 构造/析构 | `0x37760` / `0x37940` | 实例大小、成员默认值、算法对象所有权 |
| GetAllParamList / GetParam | `0x37a60` / `0x382f0` | 参数名称及受控 getter 对照 |
| GetProcessInput | `0x39540` | 采点批次、数组前缀、平移和旋转边界 |
| Init | `0x3a140` | 真实平台初始化边界；本宿主不假装完成 |
| MatrixInv | `0x3a310` | 组合标定中的单精度逆矩阵与 `1e-7f` 阈值 |
| Process / ProcessAlg | `0x3a8c0` / `0x3b0e0` | 状态机、示教、组合标定与真实求解调用 |
| ProcessOutput | `0x3c3f0` | 原始输出存在性、类型与数值 |
| ResetMember / SetParam | `0x3d6d0` / `0x3d740` | 清空、模式及参数写入行为 |
| WriteXmlData | `0x3e660` | 模块标定文件结构及示教字段副作用 |

可重放入口：`tests/NativeEvidence/Invoke-OriginalTranslationProbe.ps1`、`Invoke-OriginalTranslationUiProbe.ps1`。生成器：`tests/Export-TranslationDefinitions.py`、`Export-TranslationEvidence.py`。所有原始运行、截图和编译输出均在本工作树分析目录。

## 与 N点的实际区别

1. 新增 `CalibType`：0 平移标定、1 平移旋转标定；相机 UI 只有 1 静止和 3 运动。N点的自动物理坐标网格、相对坐标、基准步长及可配置标定原点不属于本工具。
2. 图像坐标成功、物理坐标缺省时，本工具保留已有物理点值；新实例对应零。它不会像 N点工具那样生成蛇形物理网格。
3. `CalibType` 控制显示及组合求解分支。隐藏的旋转数量不会被自动清零：受控运行中在类型 0 下写入旋转数量仍参与采点与求解。
4. `UnionCalibEnable && CalibType != 0` 时，先求平移矩阵，以其逆矩阵把各旋转物理位置到中间旋转位置的平移差转换为图像位移，再重建旋转输入并求解。普通分支直接提交采集的旋转点。
5. 完成输出包含 `IsRightCoorA`，其计算使用矩阵左上 2×2 行列式符号。未完成分支仍写 `CalibErrStatus=0`，不能据声明表把两者无条件同时发布。
6. 原厂 `TeachFlag` 直接作为字符串输入，本工具没有 N点的 `TeachFlagInput` 端口名。
7. 文件包含额外 `CameraMode` 字段：原厂 `DataType="int"`，内容却是 `CameraStatic` / `CameraMove`。保留此原文，不擅自改为数字。

普通数值内核复用 `NPointSolver`；组合标定补偿、状态、导出和 UI 由 `Translation*` 专属代码实现，没有复制或修改 N点状态机作为共享契约。

## 状态机与输出

[受控证据](translation-calibration-evidence.json) 保存全部事件顺序、参数写入、输入数组/字符串、getter 返回、输出调用和值，以及逐个原厂 XML 原文与哈希。当前覆盖 48 个 Process / 原始成员场景和 26 次文件导出尝试；其中包括采点不完整、空数组、无绑定、计数不等、批次边界、0/1/2/3 旋转点、两种相机、组合开关、示教、重复执行、清空和输出回调失败。

- 原厂原生默认：类型 0、触发获取、9 个平移点、0 个旋转点；组合、示教、自动刷新均关闭。自由度 2、权重 3；权重因子 20、距离阈值 30、样本比例 60。Algorithm XML 中刷新 True 与实际 getter False 分开记录。
- 原厂输入 getter 在未绑定夹具中返回 `0xe000031c`，Process 原样返回且没有输出调用；成功的零长度图像数组则返回成功，发布未完成状态与空点前缀。
- 图像 X/Y 或物理 X/Y 数量不等返回 `0xe0000001`；相等的短物理前缀被接受，其余点保留原值。
- 单次批量只采到当前平移或旋转阶段末尾；多余输入不在同次调用顺延至下一阶段。点已满后重新计算并发布旧数据，不再读取输入。
- 旋转数量为 0 时，两路输入角度不会被计入图像/物理角缓存。旋转阶段缺角度时保留该位置已有值。
- 未完成时写 `WorldRotateAngle=0` 与 `Tilt=0`，缺少 `Rotate`；完成时改写 `Rotate`、`Tilt`，不写 `WorldRotateAngle`。声明与执行差别保留。
- 清空会清计数、图像/物理点及 `0x163b0` 角缓存，并取消示教采获；`0x16430` 角缓存保留。方向与文件内部原点也不凭空复位。
- 字符串示教匹配后采获拍照与物理姿态，不累计标定点。物理 X/Y/角度缺省的失败在输出前发生。

原厂回调失败场景只能证明已执行的输出前缀和状态没有回滚；独立框架的结果字典不是逐端口 ModuleFrame 写入服务，该合成失败没有伪造为真实平台故障。

## 文件与界面

专属 `TranslationFile` 使用原厂 `CalibInfo/CalibInputParam/CalibOutputParam`、点列表、9 个矩阵系数及姿态节点。PixelPrecisionX/Y 是矩阵**列范数**。旋转中心 R 均为 -999；文件不是算法 SDK `ExportCalibFile` 的结构替代品。

示教节点由已采获状态决定，禁用示教后节点仍存在：SnapImage 回退图像点，其余两姿态为 -999。`WriteXmlData` 内部图像回退原点初始为 0，先用于 SnapImage，再改成 `(平移数-1)/2` 用于 SnapWorld；因此首次缺拍照坐标导出与再次导出不同。专属插件保存这一文件状态，未把 N点的用户可配置原点混入本工具。

原厂 UI 证据使用原始 XML、解析器和 Control.dll，在明确合成的参数模型下记录 12 个模式快照，并成功渲染原厂五数据列点表控件。完整参数面板的原厂 BAML 类型解析抛 `NotImplementedException`，失败保留在证据中；没有把独立实现截图或合成参数默认值当成原厂桌面截图/原生结果。独立配置窗口、模式切换、点表、订阅边界和操作按钮由专属 WPF 检查覆盖。

## 集成与验证边界

沿用现有 NumberArray、Text、Calibration 载体、模块作用域状态、点表持久化和工具执行上下文，无新增共享接口。`CalibData` 是独立框架对原厂 `MapData` 参数的类型化出口，原生 Process 输出清单未把它当成已发布端口。宿主注册、目录映射、解决方案及主清单由主代理统一集成。

数值内核覆盖与已知退化/强稳健权重边界跟随 N点内核报告；不能以本工具有限场景推导任意标定数据逐位等价。组合标定的严格重合旋转点已由共享内核依据原厂 SDK 对照修正。最新专属检查为 7445 项通过，构建 0 警告、0 错误，日志与文件哈希见 [验证索引](translation-calibration-verification.json)。宿主集成及主线回归由主代理继续完成。

再次匹配示教会先将拍照坐标置为 `(-99999,0,0)`，随后读取物理姿态；即使缺少 Y 输入失败，之前成功示教留下的 HasTeach 仍保留。此时导出使用文件内部原点回退拍照坐标。该失败顺序已补入原厂受控场景与独立运行回放。

主线集成补充：已接通WPF/WinForms注册及工具目录；真实WPF宿主267项检查通过，验证拖入默认值、数组订阅、自执行、缺物理坐标不生成N点网格和撤销。点表导入按原厂保留有效前缀，后续行失败不回滚已写入行。
