# 第六轮根线自审：目标收束为标定十工具

日期：2026-10-20。用户将本目标限定为目录“标定”十工具及必要框架链，其他工具独有逆向移至下一目标。主线即刻停止 Binary 后续独有取证；此前第五轮 Binary 证据保留但不再作为本轮关闭条件。当前[标定十工具对象与去重索引](20261020-calibration-only-joint-audit.md)取代原 18 工具＋Group 的当前范围，J01–J12 证据门不降级。`tool-plugins` 工作树未修改，原件证据只读。

## 实际新增闭环与源证

1. **原版身份和当前共同发现/实例化边：** 原安装“标定”十项各有 `ToolItemInfo.xml`、主 XML、原生及 Cs 载体，十项静态身份均按原件 SHA 核对；标定板 `ToolItemInfo.name=CalibBoardCalib` 与目录/主 XML 的 `IMVSCalibBoardCalibModu` 不同，不能合并字段。见[原件身份报告](20261020-pass6-calibration-original-identity.md) SHA-256 `1307138D4C2C4FAB204F49A1BA8F4762E91D94952ABDABAD0415B8D7132505CC`，机器清单 SHA-256 `57AADBBE06D6B47153DC88CD7F234E44F32871D917454A731D1AE449C384E561`。原版共同入口 `ToolBoxData.UpdateLoad` 懒读各资源，`VmModuleSet.CreatModuleByInfo → CreateModuleToolRe` 依据 SDK 模块名创建；动态实例 ID 尚未逐项核。受控真实 WPF `MainWindow` 从当前“标定”分类读取十个 `ModuleId`，逐项匹配注册插件、manifest/descriptor/version，调用真实 `ModuleRegistry.Create` 工厂和 `GraphEditorDocument.AddNode`；十项均成功且各有专用 WPF 工厂或共享配置 provider。报告[目录/宿主](20261020-pass6-calibration-catalog-host.md)，探针 `tests/CalibrationCatalogHost.Checks/`，最终 stdout SHA-256 `0BEB1FBB9647B008CFA18379FFE801F0612051EB0BDB9CFD58378AA13D23CEC9`。两项当前目录映射属于已有未提交工作区，未被本轮修改或当作 HEAD 完成状态。
2. **N 点特有编辑链：** 原 `NPointList.Init` 每点五次读，`PointList` 五个 setter 分别写对应 `IUserStringData` 键；当前逐字段初始化会对每格整表复制并调用文档 Apply。真实配置＋图文档探针测 4/9/32 点开窗分别 20/45/160 次 Apply，单格修改各一次；成本增幅成立，永久卡死根因与原版时延比未证。见[N 点专项](20261020-pass6-npoint-ui.md) SHA-256 `DE3CD19573568A9BA9C8662A0CD144A33500E856C0294480CC6C4F8A795A3B9E`，日志 SHA-256 `E1B7ED168CE665FE8CCA65B760DE51D8BAEAC6123C9FECDA7D1FA1BB2C7CC192`。专属配置属于插件侧，已在 `FCR-033` 登记生产者、消费者与历史边界，不在 main 复制修补。
3. **标定结果真实链：** 原 `IMVSNPointCalibModu.ProcessAlg/WriteXmlData` 写 XML 九项矩阵，`ReadCalibFileModu.Process` 由 `RefreshSignal` 触发重读并发布 `float[]`，`CoordinateModule.Process/GetProcessCalibMatrixInput` 在 Matrix 模式读 1..9 项；原 N 点 XML 并无 `CalibData` 端口。当前真实 `N点 → XML → 标定加载 → owned Group → 坐标系` 的九项矩阵首跑和保存重开复跑均成功；直接将 `NPoint.CalibData` 接 `Coordinate.CalibMatrix` 被精确 `Port type mismatch.` 拒绝。见[结果链](20261020-pass6-calibration-result-chain.md) SHA-256 `0726E64F5F0FDDAAE7C282966D6BE0ABF29C11641FE469E932441017A0D6DF05` 及 `tests/CalibrationResultChain.Checks/`，日志 SHA-256 `AB5D993788D43D92287431E17D303E71555A08F413340AB346A15381682E8B0F`。原厂同一整图、异常与少于九项矩阵仍未证；当前便利对象端口不冒称原版规格。

## 自审边界和下一条可关闭工作

这几项分别关闭**原件十项静态身份**、**当前目录创建共享边**、**N 点编辑成本特有边**和**一个正常九项标定矩阵链**；没有使任何一整工具的 J01–J12 全部完成。后续先核 SDK 动态实例身份，完成 N点/标定加载/坐标系的异常与短矩阵/原版 Group 边，再推广到其他七个工具的独有算法和结果结构；共享目录/GraphExecutor 不重复取十遍。第五轮无响应证据只能定位配置窗同步 Show 停顿，未有用户长期卡死的可归属 UI 栈。

本轮没有有充分原版合同依据且属于 main 独占的产品修改。框架实施完成指标仍 **41/96=42.71%**；审核切片和通过测试不增加分子，人工验收仍另列。保留所有失败日志和未验证范围，未推送。
