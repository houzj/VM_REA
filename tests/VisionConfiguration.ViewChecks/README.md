# 公共紧凑配置壳回归

`dotnet run --project tests/VisionConfiguration.ViewChecks/VisionConfiguration.ViewChecks.csproj`

本程序在 STA 中构造独立 WPF Window 以验证树结构和惰性 Owner，但不 Show，不声称原厂桌面运行或像素比对。五工具原 XML 夹具复用 ModelChecks/Fixtures。测试显式注册的简化 Button 是测试 factory，不能作为真实参数模板支持证明。当前 registry 没有默认输入框兜底。

来源：

- 原厂 `Apps.XmlUI/Apps.XmlUI/ParamsTabUserControl.xaml`：三行（tab内容、extern、footer），左右 Height=32，Left仅 Tab_Run Params，页内 ParamListView。
- `Apps.XmlUI/Apps.XmlUI/ParamsTabUserControl.xaml.cs`：中文 ChildMargin=32,1,32,0 / Header=32,0,0,8；英文24,1,24,0 /24,0,0,8。
- `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/Tab.cs`：ParamWidth中文76/英文110。
- `Apps.Style/itemscontrolstyle/itemscontrolstyle.xaml`：TopTabItemStyle内部头高35。
- `Apps.XmlUI/resourcedictionaries/globalresources.xaml`：ParamGroupItemStyle底边距5、标题14粗体Microsoft YaHei、header顶间距20、分隔线E0DFDE、右边距18，第一组无线，CategoryNoHead隐藏标题。
- `Apps.XmlUI/Apps.XmlUI/ParamListView.xaml`：垂直滚动、横向禁用。

以上反编译资源基目录为 `D:/MyWork/REA逆向测试/analysis/decompiled`。产品共同壳没有附加宽屏渲染列；特殊内容只能以 FrameworkElement 注册，返回 Window 明确失败。未知模板显示诊断，不猜控件类型。模板键运行时改变不会自动重建和丢焦点，产生 TemplateChanged，结构性刷新需宿主另行显式处理。

尚未支持/未证实：完整原厂主题、每项模板、TabType=Panal专用内容、完整ROI/订阅浮层、原厂像素一致性。测试通过不代表这些能力已实现。

样式修正：ConfigurationShell.xaml 的页签样式完整搬运 ParamsTabControlPanal 的 TopTabItemStyle（只替换 ParamWidth/外部类型绑定），TabControl 结构搬运 itemscontrolstyle 的 TabControlStyle（ItemsSource.Count 改为公共壳显式单页标志）；主题橙色 E76900/20E76900 来自 Apps.ColorFun/colorstyle1.xaml，footer E4E5E7 来自 colorstyle.xaml。测试逐项断言内部35高度、独立2px下划线、选中底色/字重/前景、标题分隔线/边距、单页隐藏高度。输出 analysis/full-audit/configuration-shell/source-derived-shell.png 为独立共同壳+测试factory渲染，绝非原厂桌面截图或逐像素对比。
