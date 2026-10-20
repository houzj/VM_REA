# 平移旋转标定与旋转标定配置重建

## 证据边界

本文区分三类证据：原厂反编译源码/资源明确声明的内容；本项目根据该声明接入公共配置模型后的独立实现；尚未通过原厂桌面窗口对照的内容。自动化模型测试和本项目截图不能单独证明与原厂像素或运行时完全一致。

## 原厂源码与资源

`analysis/full-audit/plugin-coverage.json` 中记录了原厂 XML 资源路径及原始 SHA-256：

| 工具 | 原厂资源 | 原始 SHA-256 |
|---|---|---|
| 平移旋转标定 | `Calibration/TranslationCalibModu/TranslationCalibModuAlgorithmTab.xml` | `3d626213bab65b7192fc27cfe3b0d5a2e13e595b5f349424adc327fe2aead062` |
| 旋转标定 | `Calibration/RotateCalibModu/RotateCalibModuAlgorithmTab.xml` | `a6d2e3c283597ac044ec49d572a50c743f5221e0b858488f2a14d559e432e73e` |

审计文件保存了 XML 解析树，因此随仓库提交的 `src/VisionPlugins.Calibration/Resources/*CalibModuAlgorithmTab.xml` 是由该树重建的语义副本，不能声称字节级等同原始二进制资源。当前副本 SHA-256 分别为：平移旋转 `11a2fc389098112ea40f45b0684affa19e5102be557ac1cbcefa4590cf97d8fe`、旋转 `88a38cb5a7d047d4d67c9bbba25ebb46b21a8a18670ee0f312770f505e1bbd5e`。

反编译模板证据来自：

- `analysis/full-audit/renderer-gap-projects/TranslationCalibModuControl/Apps.XmlParser.ParamTab/EditButton_Calib.cs`，SHA-256 `fc61d0531dcb51fe5965774bae23a742c56ffd57b41b49916a09b2a69f43ed49`。类型继承 `Integer`，并暴露 `EditDialog`。
- `analysis/full-audit/renderer-gap-projects/TranslationCalibModuControl/datatemplate/paramusercontroldatatemplate.xaml`，SHA-256 `e9326ae92f779cbde9f77178bf40b7516218686e52e52bf0cf90c69131db4778`。`EditButton_Calib` 由 `LeftRightControl`、`TextLeft`、`Integer_Sub` 与编辑图标按钮组成；图标按钮绑定 `EditDialogA.ClickCommand`，悬停时换图。
- 同一反编译项目的 `TranslationCalibModuControl/NPointListWindow.cs` 将点表窗口宽度设为 520；`NPointList.xaml.cs` 在加载后将高度设为 399。XML 中的 `EditDialog` 另声明 `DialogWidth=493`、`DialogHeight=700`，运行时包装器覆盖了这两个数值。`NPointList.xaml` 明确列出六列、30px 行高、删除按钮在恰好 4 行时隐藏，以及底部“导出 / 导入 / 清空标定点 / 确认”操作。
- `src/VisionPlugins.Calibration/Resources/ConfigurationGroups.xml` 的 `TextDisplayGroup` 原始配置定义 `Content`、`OKColor`、`NGColor`、`CharacterSize`、`Transparency`、`PositionX`、`PositionY`。前五项分别路由到显示对象或其属性；位置字段走订阅选择器。
- 两个 AlgorithmTab XML 均声明 `Tab_Basic Params` 与 `ResultShow`，并链接共享 `TextDisplayGroup`。旋转标定还链接 `ImageSourceGroup` 和 `BottomCommandGroup`。旋转标定的 `InputWay` 值 `0` 显示 `ImagePoints`；值 `1` 显示 `ImagePointXs`、`ImagePointYs`。这些是 XML 中的明确触发器，不是推测。
- `analysis/decompiled/Apps.XmlParser/Apps.XmlParser.ParamTab.AlgorithmTabRoot.decompiled.cs` 声明 `CustomTabCHWidth=420`、`CustomTabHeight=500`，因此这两个中文调试窗使用 420×500 默认尺寸有源码依据；尺寸不是从 XML 根节点推断出来的。

审计索引 `analysis/full-audit/plugin-ui-renderer-contract.json` 把 `EditButton_Calib` 的模板定位到同一资源，并标注模板“静态链已找到，实例化布局未验证”。它提供静态关联证据，不证明原厂窗口的实际像素结果。

## 本项目实现

两个插件都经 `IToolConfigurationProvider` 创建 `ToolConfigurationModelWindow`，并由公共 `ToolConfigurationView` 按原厂 XML 模型解析页签、类别、控件模板、触发器和订阅操作。旋转标定的 `InputWay` 是 XML 中没有 `AccessMode` 的 `RadioSelecter`；反编译模型在初始化阶段从 `IUserStringData` 读取 `Param.<name>`。公共状态模型因此允许此类单选控件写回配置用户数据，不把它误当成只读算法参数。

`EditButton_Calib` 使用源资源确认的左标签、数字编辑器和编辑图标布局，并继续使用既有平移标定点表弹窗。其图标来自仓库现有 N 点控件资源，形状和悬停切换与反编译模板一致；没有原始 `Apps.XmlUI` PNG 资源可供字节或颜色逐像素对比。共享显示组使用本项目已实现的对象绑定、颜色选择和订阅控件；未实现或没有契约的源操作必须暴露诊断，不能伪装成已支持。

## 验证与未知项

- `tests/TranslationCalibration.PluginChecks`：7458 项通过，覆盖 XML 页签/字段、`EditButton_Calib`、点编辑器源码尺寸、模式触发器、POINT 选择元数据、显示对象路由与公共窗口。
- `tests/RotateCalibration.Checks`：1190 项通过；公共 WPF 回归为 1250 项通过，覆盖反编译 `AlgorithmTabRoot` 默认窗口尺寸、源定义页签、`ImageSourceGroup` 的 IMAGE 选择、POINT 订阅元数据、`InputWay` 两种状态及 `Param.InputWay` 用户数据持久化。
- `src/VisionEditor/VisionEditor.csproj -c Release` 构建成功，0 错误、0 警告。
- 自动化截图保存在 `analysis/full-audit/translation-calibration/checks/template-model-window.png` 和 `analysis/full-audit/rotate-calibration/checks/template-model-window.png`。它们是本项目运行证据，不是原厂截图。

仍待用户在可见桌面上验收，并与原厂同页对照尺寸、页签、控件间距、按钮图标、颜色选择弹层、点表编辑和关闭行为。设备取帧不属于本轮验证。当前证据不支持宣称像素级原厂 UI 完全复现。
