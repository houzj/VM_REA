# 当前目标：标定分类十工具的框架到工具完整代码链

用户于 2026-10-20 将**当前目标**限定为工具目录“标定”分类的十项。审核从原版入口、资源、绑定、参数/状态、算法调用、输出/错误/释放一直追到本地框架注册、真实连接、执行、显示、Group 边界、保存重开和撤销。人手/设备验收单列；工具算法仍在范围内，但不把二值化、位置修正、分支、光源等非标定工具的独有实现列为当前完成条件。上游图像源与 Group 只在它们给标定工具供值或承载标定链时核交接，不独立审完所有特有分支。其他工具留待用户的下一目标。

## 唯一归属与复用

| 标定工具/ID | 此轮独有链路重点 | 既有工具源审入口（供复核，不直接计联合审核已完成） |
|---|---|---|
| N点标定 `vision.n-point-calibration` | 点表编辑→参数写入→求解→标定结果；点数与单格成本 | `docs/plugins/N-POINT-WHOLE-TOOL-SELF-AUDIT.md` |
| N图像标定 `vision.n-image-calibration` | 多帧索引/图像来源→教学/运行→结果身份 | `docs/plugins/N-IMAGE-WHOLE-TOOL-SELF-AUDIT.md` |
| 标定板标定 `vision.calibration-board` | IMAGE/ROI→标定板检测/拟合→结果/异常释放 | `docs/plugins/CALIBRATION-BOARD-WHOLE-TOOL-EXIT.md` |
| 相机映射 `vision.camera-mapping` | 点对/图像输入→映射求解→文件/结果 | `docs/plugins/CAMERA-MAPPING-IMPLEMENTATION-EXIT.md` |
| 映射标定 `vision.map-calibration` | 锚点/ROI/图像→映射结果→文件状态 | `docs/plugins/MAP-CALIBRATION-CURRENT-WHOLE-TOOL-EXIT-MATRIX.md` |
| 畸变标定 `vision.distortion-calibration` | 图像/ROI→拟合/畸变修正→输出/重开 | `docs/plugins/DISTORTION-CALIBRATION-WHOLE-TOOL-CLOSURE.md` |
| 平移旋转标定 `vision.translation-calibration` | 上游标定数据/路径→变换→结果 | `docs/plugins/TRANSLATION-IMPLEMENTATION-EXIT.md` |
| 旋转标定 `vision.rotate-calibration` | 轴心/点/类型输入→旋转结果→错误边 | `docs/plugins/ROTATE-CALIBRATION-FINAL-SOURCE-IMPLEMENTATION-AUDIT.md` |
| 坐标系 `vision.coordinate-module` | IMAGE/变量/夹具来源→坐标运输与显示 | `docs/plugins/COORDINATE-MODULE-IMPLEMENTATION-MATRIX.md` |
| 标定加载 `vision.read-calib-file` | 文件/路径来源→动态端口→真实消费者 | `docs/plugins/READ-CALIB-FILE-FINAL-SELF-AUDIT.md` |

每个 ID 只在本表出现一次。共同的 `目录/注册→创建→配置宿主→候选/绑定→调度→Group→存储` 由主线各取一条原版与当前证据，标定工具引用同一版本，不反复写十份。原版工具资源和算法按行分别核，不能因共享链通过而宣称十个算法相符。对每项维护 J01–J12 的已证边、未证的**精确函数/资源/条件**、当前差异和责任方；只有全链自审后才能标代码审核完成。

## 当前基线和待闭边

- 当前 WPF 主窗 `MainWindow.xaml.cs` 注册了十个插件类型；工作区 `tool-catalog.json` 的标定分类也有十个非空 ID。不过 `HEAD` 目录仅有其中八个，N图像标定和相机映射两项目录映射是用户已有未提交改动，**暂不作为提交基线完成证据**；主线只读核其现状，不覆盖该文件。
- 原版 `VMControls.Data.ToolBoxData`、`VMControls.WPF.Base.ToolItemGrid`、`VM.Core.VmModuleSet.CreatModuleByInfo/CreateModuleToolRe` 提供工具箱到原模块实例的源码入口；十项 `ToolItemInfo`/主 XML/原生与 Cs 载体的静态身份已在[原件身份报告](20261020-pass6-calibration-original-identity.md)逐项核对，SDK 动态实例 ID 尚未核。本地对应 `ToolNavigation.ReadCategoryTools/LoadToolCategory/CreateCatalogTile`、`MainWindow` 注册和 `GraphEditorDocument.AddNode`；真实主窗/目录/注册表十项探针见下表，作为 J01/J02 的**一次共享宿主证据**。
- 结果链优先做一个真实标定生产者→同分类结果消费者→父流程/Group→保存重开闭环，再按不同载荷族补特有反例。历史插件 10/10 自审仅证明各自限定实施范围，**不能直接代替本次跨框架联合审核**。
- 框架实施指标仍为 `41/96=42.71%`，与本次十工具审核对象数分开；人工验收不进分子。未获证据的边不因测试绿色、报告数量或目录映射存在而关闭。

## 本轮新证据索引（不重复计十次）

| 共享或特有边 | 当前取证状态 | 仍未覆盖 |
|---|---|---|
| 原安装十项静态身份 | [十个 ToolItemInfo/主 XML/二进制逐件核对](20261020-pass6-calibration-original-identity.md)；标定板 `ToolItemInfo.name` 与目录/主 XML 不同，分别保留 | SDK 动态实例 ID、选择→创建的逐项运行边 |
| 共同目录→注册→工厂→新增图节点 | [真实 WPF 宿主十项探针](20261020-pass6-calibration-catalog-host.md) 全部通过；仅作为十工具共享的当前 J01/J02 边 | 原安装版各 `ToolItemInfo` 身份逐件、实际用户拖放与配置窗 |
| N点独有点表初始化/单格编辑 | [原 setter 与当前 Apply 成本](20261020-pass6-npoint-ui.md) 有原体、真实图文档计数及 4/9/32 点负载 | 原版/当前同机时延、初始化失败副作用、真实宿主历史/重开、长期卡死根因 |
| N点→XML→标定加载→Group→坐标系矩阵 | [标定结果实际链](20261020-pass6-calibration-result-chain.md) 首跑和保存重开均成功；原始模块体支持文件 float[9] 路径，直接 `CalibData` 连接被拒 | 原厂同构整图运行、失败/短矩阵/其他生产者、坐标系全部分支 |
| Group 矩阵输出→坐标系输入的连接历史 | [第七轮撤销/重做探针](20261020-pass7-calibration-history-chain.md) 断开、撤销、重做、再撤销后四次实际执行（含保存重开）通过 | 原版同构整图、参数叶订阅历史、多层 Group 和动态端口历史 |
| 十工具共同配置宿主与两项参数写回 | [第七轮配置桥接](20261020-pass7-calibration-config-bridge.md) 实际 WPF 主窗十项普通配置窗分流通过；N图像/相机映射参数写入、历史及方案保存重开通过 | 全部参数/订阅、工具专用导出、原厂同条件权限/关闭差分和原生 SetParam |
| 标定加载失败→Group→坐标系 | [第七轮失败链](20261020-pass7-calibration-failure-chain.md) 十四个真实图状态涵盖读失败保旧值、清输出失败传播、恢复、热/冷短矩阵 | 原厂同构 Group 动态输出消失状态、原生冷短矩阵尾部及设备差分 |
