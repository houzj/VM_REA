# Group 复合输出原生控件模板核验

日期：2026-10-17。范围：现有反编译 XAML、托管代码、资源引用与当前框架实现的静态对照。本报告没有运行或构建产品，没有操纵真实 VM，没有修改插件或共享产品代码。本文的“原生确认”只表示本地已恢复代码支持，不表示已完成实际 VM 差分验收。

## 关键结论

1. **ShowChildWin 不是已确认可见的复合字段树入口。** `VmIODecorate.cs:144` 明确 `IsShowChildButton => false`。对全部反编译 `.xaml`、`.cs` 检索 `DisplayChildrenCommand`，只发现其在 `VmIODecorate.cs:231-247` 定义，以及 `:362-367` 的执行方法；`DynamicIOConfigControl.xaml:34-36` 类型单元格没有双击、命令或菜单绑定。不能把自建双击叶字段窗口称为原生复刻。
2. **实际可见的层级入口是订阅树。** `ButtonSelecter_sub` 由正文索引 Popup 与右侧链图标订阅 Popup 组成，树的叶节点双击提交。当前整格 Button 打开带勾选和确认按钮的 `WorkflowSourcePicker` 是独立实现，结构和提交方式不同。
3. **ShowChildWin 本身是只读叶名称列表，不是可编辑 TreeView。** 其内容宽370、高420，取消按钮隐藏、CenterOwner；内容是 `LeafIOs` 的只读 TextBox，没有显示值、勾选或单叶累积编辑。
4. **复合累积是对所有原生叶 IO 逐一设置 report mode。** 不应把显示树的层级名称误认为原生 ABI 传输结构；显示字段、订阅路径、叶 IO 名称、report mode 键应明确区分。

## 原生行模板逐项清单

以下相对路径均以 `analysis/decompiled` 为根。

|控件/行为|源文件与行|绑定、条件、资源|当前实现及差异|
|---|---|---|---|
|行布局|`DynamicObjectConfigControl/DynamicObjectConfigControl/DynamicIOConfigControl.xaml:16-24`|高32，Margin 0,0,5,6；列70、68、星号、Auto、42；微软雅黑14；主题前景|`src/VisionEditor/GroupNativeControls.cs:17` 高度和大部分列一致，但累积列固定53，不是 Auto|
|名称|同文件`:25-33`|68×32；一向 Name 绑定；LostFocus -> TextChangedCommand(Text)；最大名长 VmIO.MaxNameLenth；完整文本 tooltip|`GroupConfigurationControls.cs:49` LostKeyboardFocus 后写配置，未复用原生完整输入模板、最大长度与本地化回译|
|类型|同文件`:34-36`|只读、68×32、Margin8；ValueType 一向绑定；tooltip；**没有双击入口**|当前`:50` 同为只读；不得在这里加入声称原生的字段树入口|
|订阅正文|同文件`:37-43`；`Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:255-306`|正文列星号，右侧32；DeleteTextBox.CurValueA 双向；ReadOnlyA；OnTBFoucsChanged -> FocusedCommand；正文 Popup.IsTextDropDownOpen 双向；索引树180×180，IsTextPopupVisible 控制|当前`:51` 整格 Button + 模态单选对话框；无正文删除、光标索引、正文/链接双入口|
|订阅链按钮|`paramusercontroldatatemplate.xaml:307-335`|HorizontalOffset=-150；IsDropDownOpen 双向；DropDownOpened -> DropDownCommand；180×180树；叶双击 -> ClickCommand|当前没有贴附 Popup 和按需刷新链路；模态 picker 是兼容实现|
|订阅高亮|同文件`:261-275`|IsSelectedSubscribeParam 时边框厚3、主题 Col_Theme_Mouse；否则厚0|当前未看到对应独立订阅选中状态|
|独立订阅窗入口|同文件`:324`|27宽侧栏，25×25展开图标按钮；仅 SubscribeInPopWindowEnableA 可见；PopSubscribeWindowCommand|不能无条件增加；还依赖全局 IsSubscribeInWindow 状态|
|显示勾选|`DynamicIOConfigControl.xaml:44-60`；`VmIODecorate.cs:105-141`|IsShowDisplayCheckBox 为true才生成；输出按 OutputDisplayDic 是否包含类型；输入仅 IMAGE/PIXELIMAGE；IsDisplayForImage 写显示对象并 ChangedCommandExecute|当前`:51` 仅 GrayImage；原生输出条件不是单纯 image，也不能无证据把所有类型都开放|
|累积开关|`DynamicIOConfigControl.xaml:65-76`；`VmIODecorate.cs:204`|仅 Owner.Owner 为 IMVSGroup 且 DecorateSource.IsOutput；Content 绑定 Boolean 对象，不是 bool|当前`:52` CheckBox+位图；功能是自有事件写配置，非原生对象及SDK|
|删除|`DynamicIOConfigControl.xaml:80-93`|DeleteCommand；normal图像，Tag=click图像；祖先 ParamsTabUserControl.IsEditState=false 时隐藏|当前`:53` 已有图像按钮；编辑态隐藏需要继续对照宿主状态，不可仅禁用替代|
|表头|同文件`:108-119`|左边距18；原生累积标题宽95、ellipsis；Visibility 取父级 IsShowAccumulatedOutput|当前`:47` 与行共用五列，标题跨度2；不等同原生四列表头|
|新增|同文件`:131-148,155-157`|类型Combo68×32，SelectedValuePath=Item2；InputOutputTypes；SelectedType双向；AddCommand；非编辑态整个 AddStack隐藏|当前`:55` 类型清单硬编码；Group/插件复合类型注册映射需另核对，不能假设原生枚举就这7种|
|空白处提交焦点|同文件`:101`及 `.xaml.cs:16-19`|Focusable Grid；MouseDown 调用 Focus()|当前应核实点击空白能否触发重命名提交，与保存/刷新状态同步|

## Boolean_sub：外观与事件不是一回事

`Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:63-78`：BooleanControl.IsChecked 与 CurValueA 双向；OffContent/OnContent 分别是 `/Apps.XmlUI;component/Resources/swith_off.png`、`swith_on.png`，Stretch=None；OnCheckedChange -> ValueChangedCommand。

`Apps.XmlUI/Apps.XmlUI.ParamTabControl/BooleanControl.xaml:4-11` 只在 IsChecked=true 时替换 ContentPresenter 内容；没有文字、系统勾选框或背景标签。`.xaml.cs:43-50` 在 IsChecked 属性变化时发出 OnCheckedChange；不只鼠标 Click 才触发。当前 `NativeGroupSwitch` 通过 CheckBox 模板重建像素外观，业务回调却在 Click 上，程序化状态变化的事件语义需独立验证，不能仅截图认定一致。

## 原生层级树及数组索引事件

- `Apps.XmlUI/Apps.XmlUI.ParamTab/SubTreeView.xaml:85` 给 TreeViewItem 安装 MouseDoubleClick；`.xaml.cs:107-116` 只允许 Items.Count==0 的叶节点产生 OnTreeNodeDoubleClick。分支双击不能直接订阅。
- `.xaml:135-143` 使用 EnumEntrysA 的 HierarchicalDataTemplate；复选框受 IsMultiCheck 控制。`.xaml.cs:99` 注册默认 false，故普通单选订阅不应始终出现勾选框。
- `.xaml.cs:118-162` 根据当前 ModulePathName（移除 `[]`）及 LastItemName[type] 寻找上次订阅项；不是每次都从默认根开始。
- `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ButtonSelecter.cs:321-345` 订阅展开先执行 DropOpenSetter，初始化必要 Operation，再处理独立订阅窗口模式，刷新而非固定 mock 树。
- `:347-382` 选 VmIO 后写 ModulePathName；已有方括号时保留索引后缀，否则补 `[]`；null Holder 解绑。禁止把路径展平为只有显示标签。
- `:455-485` IndexClickCommand 仅替换当前方括号内容；null清空索引。与整个源订阅更换是不同命令。
- `:517-586` FocusedCommand：只在可编辑、光标处于匹配方括号中时准备索引枚举、记 SelectionStart、弹出索引树；失焦关闭。不能把点击正文无条件设为索引选择。
- `Apps.XmlUI/Apps.XmlUI.ParamTab/DeleteTextBox.xaml.cs:60-68` KeyUp Delete 清空文本；`:71-92` 发送 Text、IsFocus、SelectionStart；IsNumberOnly 场景另有输入法/字符/粘贴限制，不能套用到全部复合路径。
- `ButtonSelecter.cs:598-608` 独立订阅窗设置 SelectedSubscribeParam 和 IsSubscribeInWindow；`Apps.XmlUI/Apps.XmlUI/ParamSubscribeWindow.xaml:2,21-45` 窗口240×480、不显示任务栏、无最大最小按钮，两棵互斥索引/订阅树；标题含默认方式勾选框。这不是 ShowChildWin。

## 累积输出实际数据链

`VmIODecorate.cs:162-200` 懒建 Boolean：默认false；叶读取 IOModes[Name]，复合读取 GetAllLeafIO 的首个命中 mode；mode=0 为累积。`:279-329` 每次更改重新遍历 Owner.ItemsA，叶直接写，复合 GetAllLeafIO 逐叶写，最多1024记录；每项 nMode 为 true->0、false->1，strParamName UTF8固定128字节；`:331-345` SetGroupIOReportMode 失败记录错误并弹窗。当前自有数据类型、独立调度和JSON保存没有原生ABI保证。

`ShowChildWin.xaml:2,11-17` 只读列表的 LeafIOs 来源是 `VmIODecorate.cs:228-229` 的 GetAllLeafIO.Select(Name)。列表没有 value、array index、accumulate checkbox、树枝展开或叶选择提交。源中不能推出“点击类型格打开可选字段树”的交互。

## 资源依赖与可复用资产

|资源|精确来源|状态/依赖|
|---|---|---|
|switch|`Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:66-70`|swith_off.png / swith_on.png，无文字|
|链按钮|`Apps.PcUI.ImageSourceAudit/styles/pcuistyle.xaml:3-26`|32×32，Apps.Style combo_ic_关联_normal.png；MouseOver->hover、IsChecked->click；项目 `src/VisionUi/SourceAssets` 已有三张|
|独立窗展开按钮|`Apps.Style/buttonstyle/normalbuttonstyle.xaml:1830-1856`|ic_dialog_expandArrows_normal/disable/click.png；25×25；图像24×24|
|增删|`DynamicIOConfigControl.xaml:80-85,140-145`|Apps.Style Resources/add_normal/add_click/delete_normal/delete_click；BaseImage_ButtonStyle `normalbuttonstyle.xaml:1398-1411` 是 ContentPresenter，MouseOver换Tag，不是随意用通用按钮hover|
|主题|`DynamicIOConfigControl.xaml:5-12`|ColorThemesResourceDictionary、RecScrollBarStyle、NormalButtonStyle、ItemsControlStyle、ListViewComboBoxStyle、ParamUserControlDataTemplate、CheckBoxStyle|
|订阅 Popup|`paramusercontroldatatemplate.xaml:3-13,295-334`|主题 TextBox.Static.Border、ComboBoxBorderS、Col_Theme_Mouse、FgConverter、bool2VisibilityConverter、PopupButton、SubTreeView；必须核实资源作用域，不可只复制一小段 XAML|

## 可独立闭合的实现包及验收

**A. 原生订阅单元格控件（优先）**：新建独立 WPF 控件文件，输入当前路径、只读态、订阅/索引树提供器、当前索引、可见展开窗能力；输出明确“换源/清源/换索引”事件。还原星号+32列、链图标三态、180×180非模态树、叶双击提交、分支不提交、展开时刷新、失焦关闭、保留路径索引。接入 Group 后需验证真实叶输出运行数据；单控件像素和事件测试不够。

**B. 行模板收口**：独立资源字典实现边框/名称/只读类型/增删/Auto累积列/编辑态隐藏；宿主只负责业务命令。验证非编辑态无删除和新增入口，空白焦点提交名称，错误名恢复，不泄漏不属于原生的控件。

**C. 开关事件一致化**：Boolean对象/状态和 ChangedCommand 各一次；鼠标、键盘、重新绑定、程序化赋值都检查；避免初始化时误写所有 mode。使用位图保持无文字，按现有数据模型做源指导的独立实现而非声称原生运行模块。

**D. 独立订阅窗口**：后续独立包，240×480、单选叶双击、索引树/源树互斥、默认方式持久化、当前目标高亮同步；没有完整 SelectedSubscribeParam 生命周期前不应只做按钮外壳。

**E. ShowChildWin**：保留为已恢复但入口隐藏的证据；可以用于专用测试查看叶名列表，但不得作为产品新入口声称原生。若未来VM实际操作显示入口，需补版本、截图、调用证据后再决定。

上述包尚未在本报告实现。当前完成的是静态源码核验，真实 VM 主题/DPI/弹层关闭边界、缺失程序集内动态绑定及全局订阅窗生命周期仍待实际差分验证。

## 本次原始证据 SHA-256

原始文件未写入本次报告提交；下列摘要定位本次读取的本地证据，Git 仅有报告不能替代原始源码。

|本地证据路径|SHA-256|
|---|---|
|`analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/DynamicIOConfigControl.xaml`|`d3e824a8a2aa832188e98d29c3133c0c58b7791c6c089811f74e35a16dfff79a`|
|`analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/DynamicIOConfigControl.xaml.cs`|`94914c2ed29e383c84b625325df452ab6687d4e84bdfe0053e10ad046bd3f812`|
|`analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/VmIODecorate.cs`|`76cc8ad13647773172a15309a0971c27fd05aa59e05a9ff242f678cbeb47a4f8`|
|`analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl.UI/ShowChildWin.xaml`|`d804edbba7a76e9da94a5300a16377a4b62499eb394a310dcc5094933cb004c2`|
|`analysis/decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml`|`96b98f06fbaf9bc2cf1f481b2a24323ecfc11831c05698e9fa5b18ec294e0f63`|
|`analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTabControl/BooleanControl.xaml`|`dc32518f4f08c24619ea8da7e7c6657b75b61d3c074e233e7f19c6fa92bc3c7a`|
|`analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTabControl/BooleanControl.xaml.cs`|`8760a110405d4cf95890433b999b1d5dfeefbe0e5eff8afc6a720c02685657dc`|
|`analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTab/SubTreeView.xaml`|`6ecb778a669c66f23d9880d80e3cd388c4c497aee5791eaef86af2f70ec0da65`|
|`analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTab/SubTreeView.xaml.cs`|`125d85828d6abfc811ce41106b790065393814d79eb339b0c7b2d32254e8c3ab`|
|`analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTab/DeleteTextBox.xaml.cs`|`303385857c5c26740d3d2634d149e90bf610c1677cb94d99e3ed39bfa069525a`|
|`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ButtonSelecter.cs`|`a04c04dc557328269a06d67c194f6c3a64b68659acc1f76e4de778f5da858931`|
|`analysis/decompiled/Apps.PcUI.ImageSourceAudit/styles/pcuistyle.xaml`|`6f65b4bf5bc604dd966dc71b8c2f48c1f8a81f2a8392851d9d1867f54bc2e4d2`|
|`analysis/decompiled/Apps.Style/buttonstyle/normalbuttonstyle.xaml`|`0ca27d6dd39d1ac0943b526664af4a07306a6a344833fa15f50e4c3041b33ea8`|
|`analysis/decompiled/Apps.XmlUI/Apps.XmlUI/ParamSubscribeWindow.xaml`|`146d6411e3aedf19f70b167411114fa87f9a7d70cec1edb199f50bc04eae93a0`|

## 本轮独立控件交付（主线待串行验证）

经主线授权，已新增 `src/VisionEditor/NativeCompoundSourceSelector.cs` 与独立验收 `tests/VisionEditor.WpfChecks/NativeCompoundSelectorChecks.cs`。只闭合有实际 WorkflowSourceOption 提供器支持的订阅侧：32宽原资源链图标、打开刷新、180×180非模态树、分支不提交、叶双击一次回调并关闭、显式解除订阅、owner卸载关闭。原生索引正文依赖光标位置与索引元数据，本控件正文明确只读，不伪造索引树；原生独立订阅窗模式亦未实现。主线负责接入 Group、构建、运行、截图检查和实际数据验收；本分支未运行这些检查，不能标记已通过。
