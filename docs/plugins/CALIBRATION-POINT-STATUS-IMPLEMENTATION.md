# 逐点状态颜色实施与自审

2026-10-19。当前源码差异：ToolDisplayGraphics.Build原先用Status第0项给整个点集合设一个颜色；畸变CalibPointStatus是逐点数组，混合OK/NG因此显示成同色。

原厂直接成员：Apps.XmlParser.Object.SetBaseProperty按shape index读取Status并选择ngcolor/okcolor；缓存GetValueByType对非空越界index回0，直接VM.Core.VmIO.GetValue对非空数组同样回首项，scalar直接返回值，空数组返回null后转换默认0。源码当前身份沿用C4显示证据，不以函数名猜测。

实施：ToolDisplayPoint追加可选Color；Build对point按原索引保存颜色与Index，单项广播、非空短数组回首项、空状态NG；Presenter逐marker消费点色。Path连线及非point图形保持原消费，既有调用可不提供Color。此修复针对整数状态0/1点集合，不宣称所有shape特征的完整逐索引投影已经实现。

自动验证：VisionConfiguration.NamedTextChecks在独立artifacts目录构建/运行，退出0、38项通过；新增混合状态、源index、真实WPF Path颜色、单项广播、空状态，原字体/配置持久化/Presenter检查继续通过。没有启动原厂或设备，没有修改/关闭用户验收实例。摘要与当前源码哈希在analysis/point-status-checks/summary.json。

根线自审：可选字段兼容既有构造；仅point设置Color，Renderer未提供点色时回原graphic.Color；源码类别与安全边界明确。本项实施通过，不代表畸变整工具已完成。完整工具计分要求范围内逆向与实施全部闭合、自审核通过，人工验收单列不阻挡。
