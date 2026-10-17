# 公共对象属性桥接与 N 点显示配置

原厂证据：`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/BridgeItemBase.cs`（BindingPathA、懒解析缓存、GetParamUnit/SetParamUnit 空操作）；同目录 FloatConfig.cs、StringConfig.cs；`Apps.XmlUI/styles/paramusercontroldatatemplate.xaml` 2557–2623（32 高 LeftRightControl，StringConfig_sub/DeleteTextBox/180×180 树，ColorPicker CurrentColor，Float SliderArrangeFloat StartValue）。精确五条显示对象路径延续 configuration-object-binding-contract-evidence.json。

实现：ToolConfigurationObjectBindings.cs 将 Show Text 对象和 Content/FontSize 特征解析为五个显式枚举属性，无任意 CLR 字符串反射。服务缓存 binding，不缓存可失效的设置值；宿主持有显示对象，read/apply/changed 回调负责持久化和重绘；不调用 State.CommitUser、不写算法 CurValue。宿主拒绝、未读回所请求值不发布 Changed。跨窗口重开从宿主存储重读。

ToolConfigurationObjectControls.cs 注册三种真实公共 leaf。Float 使用既有共享 spinner/slider，String 按原厂括号内 caret 范围请求 typed 选择项，Color 使用共享层供应的颜色面板（建议默认+更多页），缺失对象或面板明确禁用；不构造第二个配置 Window。字体6–72、透明度0–1、RGB色限制来自现有 ToolDisplayService 托管显示契约；透明色等原生扩展未验证。

验证：tests/VisionConfiguration.ObjectChecks，实际 WPF 测量/布局/数值输入及对象持久化检查。此为独立托管实现验证；原厂桌面像素差分、完整 SubTreeView 分层、原生对象 ABI 和所有动态对象类型尚未闭合。只允许明确五个 Show Text 路由，其他对象显示 UnsupportedObjectBinding；无猜测性参数回退。

后续补齐：原厂 DeleteTextBox.xaml/.cs 确认没有清空按钮；KeyUp Delete 清空全部内容，零边框/左10内距，仅空选择或全选触发焦点检查。已移植到 ConfigurationObjectTextBox，外层圆角4、#CCCCCD边框和Popup原厂渐变/上4间距/上下6内距一并恢复。CreateColorTabs 组合默认与更多页；主机传入 SourceColorPalette 和 SourceAdvancedColorPicker 工厂，避免 Contracts 对 VisionUi 反向依赖。45项验证包含 Delete 实际键事件、选择焦点门控、默认/更多回调和初始色保持。原厂 HikTextBox 基类完整主题仍由既有共享主题近似承接，未宣称像素一致。

主机示例（在拥有 VisionUi 引用的项目，不放在 Contracts）：
```csharp
ToolConfigurationObjectControls.CreateColorTabs(initialColor, selected,
    callback => { var p = new SourceColorPalette(); p.Selected += callback; return p; },
    (initial, callback) => {
        var p = new SourceAdvancedColorPicker {
            SelectedCurrentColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(initial)) };
        p.ColorChanged += color => callback(color.ToString());
        return p;
    }, new Uri("/VisionUi;component/SourceColorPickerStyles.xaml", UriKind.Relative));
```

颜色初始值接线更新：Register 新增末尾可选 `colorPicker: Func<string,Action<string>,FrameworkElement>`，保留旧 colorPalette 回调兼容。每次展开从该 leaf 的 binding.Read() 提取初值，再创建默认/更多页；因此 OK/NG 各有独立初值，重开也观察宿主外部更新。48项验证已覆盖两色独立初值和再次打开的新色。
