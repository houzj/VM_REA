# C01 工具箱类型优先级链路（2026-11-12）

## 结论

已按原厂源码证据补齐当前壳的工具箱优先级消费机制。Guide 卡片写入的 `ToolBoxType` 现在会在工具目录读取时参与组优先级计算；当前目录未包含原厂 `ToolGroup.xml` 的 `PriorityStrategyInfo` 数值，因此没有填入四类场景的推测排序。

## 原厂证据

- `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/GuideWindow.xaml.cs:128`：Guide 卡片点击写入 `ConfigurationHelper.SaveConfig("ToolBoxType", ...)` 后关闭窗口。
- `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs:231-263`：`ChangePriorityByInfo()` 读取 `ToolBoxType`，按 `PriorityStrategyInfo` 的 `type:value;...` 项匹配类型并设置组 `Priority`；`GetItems()` 随后按优先级排序。
- `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxItemData.cs:222`：`PriorityStrategyInfo` 是工具组元数据字段。
- `analysis/decompiled/VisionMaster.Exe.Audit/app.config:56`：默认 `ToolBoxType=General`。

## 实施

- 新增 `src/VisionEditor/ToolboxPriorityStrategy.cs`，只实现上述已确认的分号/冒号解析、匹配和浮点优先级读取。
- `src/VisionEditor/ToolNavigation.cs` 为组元数据增加可选 `PriorityStrategyInfo`，读取 `StartupSettings.ToolBoxType` 后按证据算法重排组；缺失字段保持原目录优先级。
- 没有增加 `General/Position/Defect/Recognition` 的具体数值，也没有把卡片点击推断成自动创建方案或节点。

## 验证

- `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore`：0 警告、0 错误。
- `tests/ToolCatalogLifecycleChecks15`：19/19 通过。新增两项用合成元数据验证 `General` 与 `Position` 的匹配重排；测试范围是当前 MainWindow 路由与合成目录，不代表原厂 DLL 或桌面像素等价。

## 未确认与解除条件

四类场景的真实排序仍为“运行未确认”。解除条件：取得对应版本原厂 `Modules/*/ToolGroup.xml`，记录 SHA-256，并在同一目录结构下复跑工具箱加载与顺序截图/控件树比对。
