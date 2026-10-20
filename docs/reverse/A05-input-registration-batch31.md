# A05.1 第三十一轮输入注册补漏与收口判断

基线 `803124bb69b970209dac946419a68760149ee211`。本轮只读原程序 4.4.0 程序集和既有反编译文本，未启动原机或 SDK，未修改产品代码或 tool-plugins。**A05.1 仍为 partial，实施增量 0/96**；本轮发现旧键盘清单遗漏的真实局部载体，因此不能凭已有 53 项菜单清单称“完整入口”。

## 有界复验

使用 Codex bundled Python 重跑 `tests/MenuEntryInventory17/Reconcile.py --output analysis/full-audit/batch31-20261020/A05.1/inventory`，退出 0：原 `menuitems.xml` 为 53 个具名节点、43 个唯一 resolver、15 个工具栏项、16 个隐藏具名项；当前锚点分类 18 已有独立本地入口、27 未实施、1 后端部分接入、7 结构容器。这样解除第 26 轮“Python 不可用”的探针运行限制，不改变服务实施状态。输出 `menu-entry-map.json` SHA256 `85CF0F45F787C658CDF390ECE1B9839AB2A7BAFC2DB8A0D56C2F2819DA1098A1`。

新增 `tests/MenuEntryInventory31/ScanInputRegistrations.py` 在 `VisionMaster.Exe.Audit`、`VisionMasterMain`、`VM.Controls`、`VMControls.WPF`、`VMControls.WPF.Release` 五个反编译树的 823 个 `.cs/.xaml` 文件上扫描键绑定、键事件和键状态判断，排除 `obj` 生成物。得到 171 个词法候选（binding 39、keyboard_hook 43、key_test 89）；其中 XAML 明确的 `KeyBinding` 19 条，`MouseBinding` 4 条。此索引是定位器，不把词法数当实际快捷键数。输出 `input-registrations.json` SHA256 `DBF24AB8C4E19DAC5587CF9FED4693B0088933375C2E8489E59F4A881A709E0A`；脚本 SHA256 `8911019721B2CF180AE7588583B27AB6DFED0D7551822A6DB2829A63FEDF80E9`。

## 新定位的原入口及消费

| 载体与绑定 | 命令、状态和效果 | 当前 main 对照 |
|---|---|---|
| `analysis/decompiled/VMControls.WPF/VMControls.WPF.GlobalUI/GlobalCommunication.xaml:98-105`：窗口局部 6 条 `LevelD/I/W/E/C/O`，文本同时有 `Gesture=Ctrl+L` 与各自 `Key`。 | 同目录 `GlobalCommunication.xaml.cs:589,1436-1467`，`KeyDownCommand` → `KeyDownCommandExecute` → `ServerSDKAPI.IMVS_SetParamValue(CVmObjectBase.mhandle, 11000u, "LogLevel", "1".."6")`。`TitleMenueItemData.CommSet` 先检查模块、按需加载 `CommManagerModuleCs.dll`，创建/复用该窗口、设置 DecorateSource/权限/语言，才 `ShowDialog`。这是通信窗口局部入口，不是主窗全局热键。 | main 无 `GlobalCommunication`/`LogLevel` 入口；归 A05.3 通信服务缺口，不把只加按键算 A05.1 完成。 |
| `analysis/decompiled/VMControls.WPF/base/commentboxstyle.xaml:90-92`：注释框模板局部 Delete。 | `VMControls.WPF.DataForUI/VmCommentDecorator.cs:76-109` 的 `DeleteCommentBoxCommand` → `DeleteCommentBoxCommandExecute` → `(DecorateSource as IVmModuleSet)?.DeleteCommentSource(Name)`。未见此方法内的运行/权限守卫，不能借别的 Delete 路径推断。 | main 无 `DeleteCommentSource`/对应注释框模型；属于画布对象合同，仍需原删除/历史链。 |

旧 `DiagramItem` 11 条、`Connection` 1 条与上述新增 7 条构成这个**五树 XAML 明示集合**的 19 条 KeyBinding；不表示完整原程序所有键盘入口。原 `MainWindow.xaml:2` 只有 `PreviewKeyDown=BaseWindow_PreviewKeyDown`，该方法 CIL `nop; ret` 已由第 17/20 轮核实；`VM.Controls.BaseWindow.cs:1455-1458` 注册四个窗口系统命令，没有证据增加主窗 F5/Ctrl+S。服务对话框的键事件、画布局部键及控件编辑键需按焦点/载体分开。

## 原资源与解析边界

原 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.WPF.dll` SHA256 `99FFA401C8784472913AD43813D68A8A5577890C00E149506E3E5664FA61C97D`。`ExtractInputBaml.ps1` 用 Windows PowerShell `ReflectionOnlyLoadFrom` 和 `ResourceReader.GetResourceData` 只取原资源字节，提取 `globalui/globalcommunication/globalcommunication.baml`（SHA256 `82EC995E708FF275111AC81C514892C3E2C7A14D6057A03A6BFF7CF114A8E636`）及 `base/commentboxstyle.baml`（SHA256 `C6F0E1D85C1506D2B16116BDBC0E54004245928D081E9986CB986FAB09106906`）。资源 manifest SHA256 `68150A7A741EF029F4BD9B8F554641E223097A45DCF78BDA4951CEB077684B15`，提取脚本 SHA256 `AC4370CE32B8269B456C7064CF5B7A9922EEECF74C51573FF9E307969C4AD602`。首次猜测 `globalui/globalcommunication.baml` 不存在，实际键由资源枚举定位，失败保留在 manifest。PowerShell 7 的 ReflectionOnly 不支持；实际用 Windows PowerShell 5.1 执行成功。

旧 `ReadBaml.py` 在两份新资源分别遇到 `Unsupported record 0 at 0x1600` 和 `0xc46`，因此本轮**没有**把 `Gesture` 与 `Key` 的 XAML 文本顺序解释成最终有效组合，也没有把局部输入伪称原机已操作验证。反编译源文件 SHA256：`GlobalCommunication.xaml` `2B36AFF9C3B863B2F1CF41C9EC73F4FA4A35B693F077F646C5EE5F5071633FBE`，其 `.xaml.cs` `351A9F99DEC7F2901CB3EF972321CDE6066BD0A4FC8554B4D4AF7B8221936E2E`；`commentboxstyle.xaml` `BE86135158197CB72DDD9EE5049388E762D961F2967AED5AB6237DDF99DE089A`，`VmCommentDecorator.cs` `7BE1D87D2C4ACC05337CCB764DF590925933C264CB4D61C57D4831A289106676`。

## 未关闭门槛

五树源码扫描不是整套动态装配/BAML/native 输入注册穷尽；`GlobalCommunication` 的 Gesture/Key 歧义尚待原 BAML 记录或标准 WPF 回放核准，注释框删除的根/历史效果尚待调用链，27 个原菜单服务入口属于 A05.3 未实施。A05.1 的“完整清点”须在边界明确的原载体集合内对每个实际输入注册追到 handler、状态和执行并给出有效键；当前只补齐两个遗漏载体，不能自审核计 done。
