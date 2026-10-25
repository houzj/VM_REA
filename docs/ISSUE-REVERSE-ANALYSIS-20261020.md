# 当前反馈的源码级问题分析（2026-10-20）

本记录只覆盖用户指出的 N 点标定编辑延迟和轮廓匹配输入源状态。截图是待解释的观察，不替代原版规格。`tool-plugins` 工作树未改动；未做原机计时差分，也未把局部检查称为工具完成。

## 每个问题的收口门槛

1. **身份和版本**：确认实际启动的构建、节点类型、原始程序集与资源；排除同名但未使用的编辑器。
2. **原版入口和数据流**：从资源、绑定/事件追到选择或编辑操作，记录准确类型、成员、入参与副作用。
3. **状态及持久化**：查断开/连接、初始化/刷新、写入/保存/重开、错误和释放路径，不由控件外观推断。
4. **当前实现差异**：沿实际宿主调用链定位差异，区分原版确认行为、独立兼容行为和未证实行为。
5. **操作验证**：用受控样本测候选、选中、持久化和调用/耗时；失败日志原样保留。代码修改须与已确认差异一一对应。
6. **边界复核**：工具专属及插件配置归插件线；主线只负责通用流程连接和集成。跨项目改动先写契约请求，不在 main 另做一份配置系统。

## 轮廓匹配：`输入源`

**原版证据。** `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/Group.xml` 的 `ImageSourceGroup/EnumerationG[Name=ImageSource]` 声明：初始化 `GetFrontParamItemsOperation(IMAGE)` 写 `EnumEntrys`，随后 `GetSelectedCombinationOperation(InputImage)` 写 `CurValue`；`CurValue` 变化调用 `SetCombinationSourceOperation(InputImage)`。轮廓匹配的 `IMVSContourMatchModuAlgorithmTab.xml` 以 `GroupLinkItem` 引用该组。`analysis/full-audit/renderer-decompiled/Apps.XmlParser.decompiled.cs` 中 `GetFrontParamItemsOperation.Operate` 调 `VmModule.GetFrontIOWithLeafItemsByName("IMAGE")` 并写父节点 `EnumEntrysA`；`GetSelectedCombinationOperation.Operate` 从 `InputImage` 的 `SubscriptionInfo`/`GetSubscriptionInfo()` 取当前选择；`SetCombinationSourceOperation.Operate` 通过 `GetIoByPathFromAround` 找源，设置 `InputImage.RelateIO` 和 `RelateIndex`，空值则清除关系。`EnumerationG.EnumEntrysA` 改变时清掉当前 `CurValue`，因此候选更新后还需重新读取选择。用户截图中的 `13 图像源1.图像` 和 `.灰度图像` 是具体连接样本，不能硬编码为通用候选。

**现有实现。** `src/VisionPluginContracts/ToolConfigurationSubscription.cs` 的 `FromElement` 能识别此 `EnumerationG` 组合订阅。`src/VisionPluginContracts/ToolConfigurationState.cs` 将上述初始化/读/写操作分别映射为 `EnumerateInputs`、`ReadInputBinding`、`WriteInputBinding`。`src/VisionPlugins.ContourMatch/ContourConfiguration.cs` 把枚举转给订阅服务；`src/VisionPluginContracts/WorkflowConfigurationSubscriptionService.cs` 从当前流程的有效祖先节点和可订阅输出端口生成候选，读写真实绑定；`src/VisionPluginContracts/ToolConfigurationSubscriptionControls.cs` 在下拉打开时重查候选，从绑定身份选中项，显示文字只作呈现。`src/VisionEditor/ToolExecutionScope.cs` 把流程订阅服务注入编辑器。该流程是独立兼容实现，尚未证明原版的候选排序、编号前缀、断线后刷新时机和所有图像子输出完全一致。

**当前阻断。** `src/VisionEditor/tool-catalog.json` 中轮廓匹配 `ModuleId` 仍为 `null`；`src/VisionPlugins.ContourMatch/ContourMatchPlugin.cs` 的匹配内核明确未实现，因此目前不能以产品宿主截图声称此链已通过轮廓匹配端到端验证。下一验证应在受控测试流程构造图像源的两个不同 `IMAGE` 输出：断开、连接第一个、切换第二个、保存重开、再断开，逐次核对 `SubscriptionTree`、显示文案、持久化的输出身份。编号 `13` 的算法需从原版显示/排序源码或受控原机证据确认。

## N 点标定：编辑窗口和单元格延迟

**实际入口。** 当前 `NPointPlugin.CreateWpfEditor` 返回 `ToolConfigurationModelWindow`，配置中的 `EditButton_Calib` 通过 `NPointConfigurationControls.Register` 打开 `NPointConfigurationPointEditor`；`src/VisionPlugins.Calibration/NPointWindow.cs` 旧表格不在此路径，不能把旧表格的全表 `Commit()` 当成现行原因。主宿主 `src/VisionEditor/ToolEditorHost.cs` 的 `CreateScopedToolEditorContext` 把 `Apply` 接到 `doc.SetProcedureNodeParameters` 并失效化调试结果。

**原版证据。** `analysis/full-audit/renderer-gap-projects/IMVSNPointCalibModuControl/IMVSNPointCalibModuControl/NPointList.xaml.cs` 的 `UserControl_Loaded -> Init` 读两个点数、逐行读五个 `IUserStringData` 参数，然后创建 `PointList`。`PointList.cs` 的属性 setter 在值变更时仅写对应单个键并发 `PropertyChanged`；删除写 `DeletePointByIndex`，确定按钮只关闭窗口。初始化对象的属性赋值也会经过 setter，所以不能宣称原版加载完全无写入；源码能确认的是**单键写入粒度**。

**现有差异。** `src/VisionPlugins.Calibration/NPointConfigurationPointEditor.cs` 构造每行五个单元格时调用 `commit(i,a,formatted)`；`src/VisionPlugins.Calibration/NPointConfiguration.cs` 的回调每次调用 `PointValues()`，它为全部 32×5 个点调用 `Snapshot(Current())` 生成参数，再调用 `Apply(next)`；`Apply` 通过宿主写整份参数并失效化调试结果。若显示 `n` 行，构造期有 `5n` 次回调，每次都遍历 160 个点字段，即至少 `800n` 次点字段复制/序列化尝试，另有 `5n` 次整份参数提交；这只是源码操作计数，不是毫秒延迟测量。单格编辑也走同一全量路径。原版每次 setter 单键写入的底层 native 耗时尚未实测，不能据此给出速度倍率。

**验证与修复门槛。** 先用隔离的配置会话在 4、9、32 点记录打开和单格修改的回调次数、`Apply` 次数与耗时，并确认读取快照不触发执行/持久化副作用；再设计仅改变目标键的提交，同时保持原版初始化、导入、删除和清空语义，测试保存重开及失败回滚。插件代码的修改应在插件所属工作线上完成或经用户指定合流；主线当前不修改 `tool-plugins`。

## 先前快速改动的状态

`src/VisionEditor/tool-catalog.json` 当前有未提交的工具映射改动。它只解释“已注册工具仍置灰”的本地条件，尚未按上面门槛核对原版模块注册、启用条件和当前工具运行路径；不把它计作已闭合逆向单元，后续应独立复核或撤回，而不是连同这两个问题直接提交。

## 原始证据校验（SHA-256）

- 安装目录 `Group.xml`：`9129B82A0F9906DDE591347498A80E2E8C019C0FB959A721ECFD24FBBB5207DC`
- `analysis/full-audit/renderer-decompiled/Apps.XmlParser.decompiled.cs`：`A476F6266BE680F524A71003CE4A340F66378675637E34AC57721E861C674B49`
- `analysis/full-audit/renderer-gap-projects/IMVSNPointCalibModuControl/IMVSNPointCalibModuControl/NPointList.xaml.cs`：`B972D12AF77B27DC3A19E00629C77F3BED58D3FD7A266DD0AA28050A281B5A46`
- 同目录 `PointList.cs`：`A396060C73DBF56B1A7FB7849A4BCD17FE5CBC3D04AD8356F68D4B3B1C9B3813`
