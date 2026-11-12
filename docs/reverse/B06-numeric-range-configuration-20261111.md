# 公共成对数值参数与订阅生命周期整块交付

本批范围为普通 `IntegerBettween` / `FloatBettween` 配置从资源装配、子项身份、参数读写、标准控件、订阅显隐、默认值恢复到保存重开。主线基线为 `f5051d10`，改动在当前工作树交付。C03 历史方案缺失只影响对应的原厂差分，不是本批前置条件。未启动原厂或重建桌面，未显示窗口，没有执行原厂 SDK、设备或轮廓匹配算法。

## 原始依据与调用路径

| 原类型 / 资源 | 已确认的合同 |
|---|---|
| `Apps.XmlParser.ParamTab.IntegerBettween.GetSubChildren`、`FloatBettween.GetSubChildren` | 分别返回 `Integers`、`Floats` 原集合；端点是有自己名称与参数操作的真实子对象。 |
| `ValueBettween.GetParamUnit`、`SetParamUnit` | 两个子项按声明顺序操作，遇到第一个失败即返回。容器名称不能直接当成一个普通后端数值参数。Group 的特殊 `%` 代理路径另列。 |
| `ValueBettween.CustomVisibleA` | 可见值变化时先逐个写子项的 `CustomVisibleA`，再通知容器变化。 |
| `Expand.CurValueA → GetSubscribeItem` | 遍历触发器目标。`ISubInfo` 目标加入清单，`ISubChild` 的各个子项也加入清单，再递归追目标触发器；先更新这些项的 `IsDeleteSubscribeA`，随后改变自身值并发通知。 |
| `Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:576,821` | 整数和浮点模板分别绑定前两个子项的数值、订阅模式、订阅对象及参数命令。步长取容器 `IncValueA`，浮点显示精度取容器 `DecimalDigitsA`，订阅按钮可见性取容器 `SubscribeVisibleA`。 |
| `HikPcUI.ListView.SliderArrangeBase`、`SliderArrangeFloat.xaml` | 全局上下限、左右端点范围、另一个端点的界限分别参与显示控件；左右提交分别发对应的值变化事件。 |
| 当前真实轮廓匹配资源及 `ContourMatchPlugin.CreateConfiguration` | `RangAngle`、`RangScale`、`RangScaleX/Y`、各结果限制区间实际使用普通成对控件；真实 provider 将端点写回自身 descriptor 参数和宿主回调。 |

原始类型源码位于 `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/`；控件源码位于 `analysis/decompiled/VM.PcUI/HikPcUI.ListView/`。`paramusercontroldatatemplate.xaml` 从当前 `analysis.zip` 的准确成员 `decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml` 恢复到 `analysis/decompiled/Apps.XmlUI/styles/`，SHA-256 与已有证据记录一致。没有重新生成模板或用合成资源代替原模板。

| 原件摘录 | SHA-256 |
|---|---|
| `ValueBettween.cs` | `E3148003D0F9C687D161A4C50B467CB89CF8D1E6BA4DBF194016F879701DB835` |
| `IntegerBettween.cs` | `1F49D565E7792332B3B6F8BD7815546B179B6FA56FC6432A7E82150B583AD0F9` |
| `FloatBettween.cs` | `0AC785326A7B856438BFB2FAC1515DF3E61B5402F0BC1B660F192D393EBD2302` |
| `Expand.cs` | `A41089099CA81D017DE1D38FFF32E29E7EBCCB59E5E504ECC89956B5F6BFB108` |
| `SliderArrangeBase.cs` | `CDCE54E608EEE5421A2863C700B28DDBE89242514EF44690B7B1EC83544C8FF2` |
| `paramusercontroldatatemplate.xaml` | `96B98F06FBAF9BC2CF1F481B2A24323ECFC11831C05698E9FA5B18EC294E0F63` |

## 实际实现

`ToolConfigurationState` 原先只注册整数区间子项；浮点区间中的 `Float` 没有进入状态表，读值、提交、订阅及重置都无法定位这些端点。现在两种普通区间均按准确的资源结构注册子项，由同一个状态对象承担读写和订阅身份。整数容器的有符号元数据也采用原 Int32 转换，不把容器误判为可独立提交的整数叶。

初始化只读取真实端点，不向后端查询容器名。显式刷新容器时依序刷新前两个端点，首个失败便停止；直接提交容器给出明确诊断。对缺成员、其他配置实例的状态对象和不支持类型的拒绝是重建安全边界，不声称原厂畸形 XML 有相同诊断。

高级展开先传播子项的订阅删除标志。随后容器可见性变化传给各个子项，由既有订阅服务逐项确认解绑；收起高级参数时保留两端绑定。单个解绑失败保留该端身份并报告，其他端仍独立处理。此路径复用已有参数和订阅服务，没有第二套配置状态或私有持久化格式。

标准控件注册了浮点成对模板，并与整数区间共用双端编辑实现。端点使用自身名称与访问权限；容器提供步长、显示精度、总范围及订阅按钮可见性。端点范围和相邻端点边界共同约束控件提交。刷新元数据不会静默改写后端值。当前仍使用自研的两套数值编辑/滑块组合，不宣称原双滑块的像素布局、鼠标轨迹或全域浮点等价。

## 验证与复跑

新增 `tests/VisionConfiguration.NumericRangeChecks`。覆盖整数/浮点装配、声明顺序、子项所有权、有符号元数据、真实控件工厂及绑定、总范围/端点范围/动态边界、步长与精度、只读/禁用、非法值、失败回退与重试、双端独立订阅、高级显隐、解绑部分失败与恢复、默认值恢复的顺序和部分成功、畸形集合、真实轮廓匹配 provider 写回与方案保存重开。还通过当前桌面实际随包的标定板和畸变标定 provider 验证 `EdgeThreshLow/High` 的独立写回、源模式显隐、宿主拒绝和保存重开；没有修改它们的专属算法或原生 ABI。测试中的控件对象没有挂接 `PresentationSource`，滑块弹层保持关闭；没有创建或显示 `Window`。轮廓匹配 provider 是当前仓库源码消费者，尚未列入当前桌面发布包，未擅自注册或集成它。

```powershell
pwsh -File tests/NativeEvidence/NumericRanges/Run-Regression.ps1
pwsh -File tests/NativeEvidence/C02/Run-LifecycleRegression.ps1
```

整批入口为每套检查建立独立工作目录，构建产物与日志保存在本工作树的独立 GUID 目录，核对运行退出码、实际计数及程序集哈希。已有模板检查按当前目录寻找证据文件，第一轮隔离运行因此失败；修正入口为该套复制其固定证据 JSON 后，保留失败目录再整批复跑。最初新增测试有一次括号编译错误，同样保留原日志。

最终共享配置回归 **11 套、3679 项**通过，新增专项 **142 项**；执行生命周期回归 **11 套、462 项**通过，两批共 **22 个不同测试项目、4141 项**。共享配置汇总 SHA-256 为 `844F5CD19830731DAC313E3C1F13300AA6D555A0D953B9990BD73046AFBACD09`，生命周期汇总为 `271D7442EB046502D62FC3783D6ACE332DD0BEE6462EEE1FE86B711C9C6625F4`。[配置整批](../../analysis/full-audit/framework-numeric-ranges/regression-9a2ba5888b7d4bdea2a0fd62b3499e81/summary.json) · [生命周期整批](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-2a859031089e475f800732d167d1f67f/summary.json)。源码文件未变时不重复执行已经通过的专项。

已核对 **33 对程序集/PDB、407 份仓库源码记录**与当前文件一致，包括配置专项、生命周期、共享配置模型以及桌面中的实际标定工具；该数包含跨程序集重复源码记录，不作为独立测试或完成单元数。[身份记录](../../analysis/full-audit/framework-numeric-ranges/regression-9a2ba5888b7d4bdea2a0fd62b3499e81/delivery-identity.json)，SHA-256 为 `F5F2E238D7DE41B5761D1A5F0A8334EDEA531C04057C6BCA998254144345D47E`。

桌面 Release 构建 0 错误；原有 `MainWindow.xaml.cs:122` 的 CS8603 在 WPF 临时项目和正式项目各报告一次。20 个托管运行依赖和随包 BMP 已核对。产物保持未启动。[本批 Release](../../analysis/manual-acceptance/framework-ranges-c7ab9cdea8964bf6a15040d24a0e0617/build/bin/VisionEditor/release/VisionEditor.exe) · [构建清单](../../analysis/manual-acceptance/framework-ranges-c7ab9cdea8964bf6a15040d24a0e0617/summary.json)。

## 退出界限与进度

普通成对数值配置的上述公共调用路径已落入真实框架和真实工具 provider；本批不补轮廓匹配算法，也不实现原生 Group `%` 代理、多目标 `IGroupRunParamWithMultiParams`、`IntegerBettween64` 或专用 CircleFind 变体。这些与本批普通区间不同，仍需各自证据与实施。人工桌面和原厂像素差分单列。

B06.2 的整个单元还包含 ROI 与专属 Group 装饰器，因此不因本批完成就将其改成 done。框架实施与自审仍为 **41/96（42.71%）**，分母为 96 个固定框架单元；已选标定工具 **10/10（100%）**。本批的检查数不计作完成单元数。

