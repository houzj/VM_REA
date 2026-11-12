# VisionMaster 高保真逆向交接（交给 CodeBuddy）

更新时间：2026-11-12

## 交接目标

继续完成 VisionMaster 的高保真本地逆向和整体落地。最终目标是：启动链、解决方案实体、工具载体、公共执行/停止/结果/释放、配置/ROI/显示、保存重开、Group 和选定真实工具形成可复核闭环，并在有原厂证据时做同输入差分。

用户的硬性要求：**所有修复和优化必须有逆向证据。** 原厂源码、资源、PE/汇编/ABI 或匹配运行证据是实现依据；截图和用户反馈只能指出差异，不能直接定义语义。证据不足时必须保持 `unknown` / `运行未确认`，不能用常见产品意图补齐。

## 先读这些文件

1. `AGENTS.md`：主框架与 tool-plugins 的所有权、证据标准、人工验收和进度口径。
2. `docs/VM-REVERSE-GATE-MATRIX-20261112.json`：当前门禁。尤其是 `implementationRule` 和 `noEvidenceAction`。
3. `docs/VM-COMPLETE-REVERSE-IMPLEMENTATION-PLAN-20261112.md`：完整实施设计、证据等级、S0-S7、VS0-VS8。
4. `docs/VM-EXECUTION-GOALS-20261112.md`：G0-G10 目标和退出条件。
5. `docs/reverse/VM-fidelity-ledger-20261112.md`：当前保真度账本和最大差异排序。
6. `docs/PARALLEL-WORK.md`、`docs/PLUGIN-FRAMEWORK-BOUNDARY.md`、`docs/CONTRACT-REQUESTS.md`：并行边界和跨项目合同。

原厂静态证据位于：

- `D:/MyWork/REA逆向测试/analysis/decompiled`
- `D:/MyWork/REA逆向测试/analysis/full-audit`

## 当前真实状态

### 保真度分类

- 启动入口、单实例和部分初始化：`source-shaped`；原厂初始化服务、认证和同机时序仍未运行确认。
- Welcome/Guide：资源和主要 XAML 已恢复，但 Guide 与原厂桌面视觉/交互仍有差异，状态为 `divergent`。
- 方案选择、最近列表和保存重开：当前是 `independent-contract`；原厂 `.sol` 容器、字段、资源目录和场景直启未确认。
- 主窗口工具载体：`divergent`；当前工具箱、画布可构建，但原厂动态工具载体、配置入口和布局状态未逐成员对齐。
- 公共执行生命周期：`source-shaped + independent-contract`；当前 Run/Stop/Callback/Result/Cancel/Dispose 有回归，原厂实例级调度和 native 资源边界仍未确认。
- ROI/Mask/显示：`divergent`；原厂 ROI 集合、Mask 生成、像素缓冲和显示发布链未完整闭合。
- 直接 SDK Blob：已有固定样图的可重复差分；这不能代替原厂宿主。
- 原厂宿主 Blob：`unknown`；历史记录 `45/971486`、`47/973202` 和静态调用边存在，但 `test1108.sol` 缺失，首处分歧未定位。
- Group/子流程：当前独立合同可运行；原厂复杂数据、模板、回调和生命周期未完整差分。

当前权威进度仍按 AGENTS 口径记录：框架 **41/96（42.71%）**，工具 **16/162（9.88%）**。测试数量不能直接提升该百分比；完整 162 个目录工具也不能直接宣称完成。

## 最近已落地、可保留的修改

以下修改已经在当前工作树中，CodeBuddy 不要回滚或用无证据重写：

1. `src/VisionEditor/WorkflowCanvas.cs`：图重绘后旧 `MouseLeave` 对节点卡片的访问改为安全查找，避免陈旧引用。
2. `src/VisionEditor/SubprogramCallWindow.xaml.cs`：Stop 不再误取消当前 SelfRun；关闭时仍取消并排空执行。
3. `src/VisionPluginContracts/ToolConfigurationCalibrationPathPolicy.cs` 及六个标定提供者：按提供者选择是否在校验失败时保留候选文本；对应路径回归已有证据。
4. `src/VisionEditor/ToolboxPriorityStrategy.cs`：按原厂 `ToolBoxData.ChangePriorityByInfo()` 的已确认格式解析 `type:value;...`。
5. `src/VisionEditor/ToolNavigation.cs`：`CatalogGroup` 支持可选 `PriorityStrategyInfo`，工具箱读取 `StartupSettings.ToolBoxType` 后按该证据算法重排组；元数据缺失时保留原目录优先级。
6. `tests/ToolCatalogLifecycleChecks15/Program.cs`：增加 `General`/`Position` 的合成元数据验证。

工具箱这条链路的证据和限制见：

- `docs/reverse/C01-toolbox-type-priority-chain-20261112.md`
- 原厂 `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/GuideWindow.xaml.cs:128`
- 原厂 `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs:231-263`
- 原厂 `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxItemData.cs:222`
- 原厂 `analysis/decompiled/VisionMaster.Exe.Audit/app.config:56`

注意：当前仓库没有原厂对应版本 `Modules/*/ToolGroup.xml` 的真实 `PriorityStrategyInfo` 数值。因此不能把 `General/Position/Defect/Recognition` 的排序值写死。取得原件后只补元数据并复跑，不改解析算法。

## 已验证命令

```powershell
dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore
dotnet run --project tests/ToolCatalogLifecycleChecks15/ToolCatalogLifecycleChecks15.csproj -c Release -- src/VisionEditor
```

最近一次结果：Release 构建 0 警告、0 错误；工具目录生命周期测试 **19/19** 通过。该测试使用合成目录，只证明当前壳的证据化解析和缓存/失败生命周期，不证明原厂 DLL、原厂桌面像素或四类真实排序等价。

已有批次报告集中在 `analysis/full-audit/fidelity/20261112/`，对应摘要写在 `tasks/todo.md`。重要的开放项包括：

- `test1108.sol` 和匹配原厂宿主环境缺失；
- Map/NImage/Distortion 等部分 native fixture、原始 XML、solver trace 缺失；
- WpfChecks 在 104 项后因测试流程缺少 ImageSource 节点停止；
- 原厂 `.sol`、设备/认证运行状态、原厂 Blob 宿主首处分歧仍未闭合。

## CodeBuddy 下一步（按效率排序）

### 1. 先做证据批量索引，不逐工具盲改

批量建立以下索引：

- 原厂 `ToolGroup.xml` / `ToolItemInfo.xml` 路径、哈希和 `PriorityStrategyInfo`；
- Guide 卡片点击后的配置写入和主窗口首次工具箱加载入口；
- 方案实体从创建、打开、Procedure、Node、资源目录到保存/关闭的成员链；
- `MainWindow` 当前入口与原厂 `MainWindowData`/HostShell 的对应矩阵。

每条索引写入证据路径、成员/RVA、观察、未知和复跑方法。

### 2. 优先修复启动和方案载体差异

先做 G2/VS0/VS2 的完整设计审查，再集中修改：

- Welcome → Guide → 空载主窗口 → 关闭释放；
- 新建方案与打开方案必须共享同一个解决方案实体；
- 最近方案、Procedure、Node、资源路径和执行上下文不能走旁路；
- 原厂 `.sol` 未确认的字段保持不透明，不将 JSON 伪装成 `.sol` 兼容。

### 3. 再处理公共执行合同和真实工具切片

沿 `Run → callback → result → stop/cancel → dispose` 逐成员对照源码。先用 ImageSource → 一个证据最完整的真实工具做 VS1/VS3/VS5/VS2 垂直切片；Blob 原厂宿主差分继续单独记录，不因缺方案停掉源码取证。

### 4. 最后统一构建、测试和对照审核

不要每改一个小工具就启动 UI。集中完成设计和实现后，统一运行：

- 相关源码/ABI/资源身份审计；
- 正常、失败、重复、保存重开、停止、释放和错误恢复；
- 主窗口人工验收（用户明确要求时才在 `WinSta0\\Default` 启动）；
- 原厂同输入差分（只有匹配原件和运行环境齐备时执行）。

## 禁止事项

- 不因截图直接改尺寸、颜色、点击语义或自动创建方案。
- 不把“测试通过”写成“VM 等价”。
- 不用 `test1108.sol` 缺失作为整个目标的卡点；只将依赖它的原厂差分标为 `unknown`。
- 不修改 tool-plugins 工作树，不静默整合其未提交代码。
- 不引入第二套公共配置/执行合同。
- 不删除历史证据、失败记录或未确认项。
- 不启动用户界面作为自动测试替代；人工验收另行记录。

## 交接完成判定

CodeBuddy 每次交付必须同时给出：

1. 变更文件和责任边界；
2. 每项变更对应的原厂证据路径和证据等级；
3. 实现、测试、构建命令及结果；
4. 未确认行为和解除条件；
5. 更新后的保真度账本、目标状态和总体进度百分比。

只有拿到源码/资源、匹配运行或 native/ABI 证据并通过对应门禁，才允许把状态从 `unknown` 或 `independent-contract` 提升；否则保持原状态。
