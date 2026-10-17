# 公共配置定义模型回归

`dotnet run --project tests/VisionConfiguration.ModelChecks/VisionConfiguration.ModelChecks.csproj`

模型独立于执行 `ModuleDescriptor`，不创建窗口、不读插件参数、不运行原厂模块。夹具为原厂 AlgorithmTab XML 与 Group.xml 的完整资源副本，来源及原始文件 SHA-256 见 Fixtures/sources.json；模型 SourceSha256/GroupsSourceSha256 对传入字符串的 UTF-8 编码计算（与带 BOM 原始文件哈希可能不同）。

源码依据：

- `D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/AlgorithmTabRoot.cs`：Tabs、BottomExcuteButtons、BottomExcuteButtonsLeft、ExternParamsList；420/680/500 默认值。
- 同目录 `GroupLinkItem.cs` 的 AfterAddedToArray：移除链接，克隆 GroupInfoItem，Items 逐项加入当前集合并重设 parent。解析器按声明的逐项构建顺序展开，不把组包装成额外可见层级。
- 同目录 `GroupsRoot.cs` 的 InitItems：重复 Name 仅首项进入索引。
- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF/ParamsConfigControlBase.xaml.cs` 的 OnParamsConfigChanged：仅 Language==English 走英文，语言自定义宽非零时才应用对应非零高度；否则使用相应默认语言宽并保留 CustomTabHeight。

原始 Declaration 与解析 Root 均保留标签、属性、子树。组实例各自独立；Source/ReferenceChain 和 Parent 可供模板/状态引擎定位。未知普通标签保留供类型分发诊断。未知组、循环组与坏 XML 显式失败；原厂未知组原本静默移除，此处为重建项目明确诊断策略，不声称相同行为。原始注释/空白由 SourceXml 保留，元素树供语义操作。

本检查验证定义与来源，不能证明模板、操作引擎、原厂窗口像素或运行时等价。
