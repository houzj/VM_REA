# T5 工作区链路合流验收（2026-11-12）

执行：CodeBuddy ｜ 任务：T5.4 ｜ 目的：确认 `31acdb4c` 工作区执行链路与 Codex 保真度回归在同一工作树上互不回退。

## 1. 复跑结果

| 检查 | 退出码 | 结果 | 日志 |
|---|---|---|---|
| `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore -p:UseSharedCompilation=false -p:BuildInParallel=false` | 0 | 0 警告 / 0 错误，2.47s | 控制台回执 |
| `NodeProjectionChecks` | 0 | 54 项投影断言全部通过 | `artifacts/codebuddy-verify/t5-merge-NodeProjectionChecks.log` |
| `VisionEditor.WorkspaceProjectionChecks` | 0 | 79 项投影接线断言全部通过 | `t5-merge-VisionEditor.WorkspaceProjectionChecks.log` |
| `VisionEditor.ShellMenuChecks` | 0 | 65 项入口清点断言全部通过 | `t5-merge-VisionEditor.ShellMenuChecks.log` |
| `VisionEditor.ShellChecks` | `-532462766` | **失败（与 T0 同一位置 `Program.cs:275`）** | `t5-merge-VisionEditor.ShellChecks.log` |

## 2. 逐项核对

| 工作区链路项 | 依据 | 结论 |
|---|---|---|
| 模块级执行账本 `ModuleRunLedger` / 节点数据投影 `NodeResultProjection` | `NodeProjectionChecks` 54 项 | 未回退（E4） |
| 历史结果列 `RowIndex / AddTime / Content` 与模块级数据行 | `WorkspaceProjectionChecks` 79 项 | 未回退（E4） |
| 模块结果树形展示、旧三列表格常隐、图像源下拉跟随选中节点 | 同上 | 未回退（E4） |
| 帮助页由工具描述符生成、下区五页对象判定 | 同上 | 未回退（E4） |
| 菜单/工具栏数据驱动、50/42/15/4 自检、25 命令显式禁用 | `ShellMenuChecks` 65 项 | 未回退（E0+E4） |
| 最近方案菜单顺序与条目数 | `ShellChecks:275` | **仍失败**；T2 §5 已定位原厂语义与当前侧分散写入点，但首个多出条目未定位；未改产品或夹具 |

## 3. 结论

- **核心工作区链路未回退**：Codex 在本轮之后对 `ModuleRunLedger.cs`、`NodeResultProjection.cs`、`WorkflowCanvas.cs` 等文件的未提交修改没有破坏 `31acdb4c` 已交付的节点级结果、历史结果、模块结果树、帮助页与菜单数据驱动。
- **`ShellChecks` 保持开放**：两次复跑（T0、T5）失败位置与退出码一致，说明既不是新回退也不是已修复；按 `noEvidenceAction` 不掩盖、不放宽断言。
- 通过项只证明当前实现合同（E4），**不证明原厂等价**；E3 原厂宿主运行与 E5 人工验收仍缺。

## 4. 继续标为独立近似（不改称 `parity-closed`）

| 项 | 当前状态 | 解除条件 |
|---|---|---|
| 节点边框 `Path + NormalModulePathStyle` | 当前为 `Border`，独立近似 | 原厂模板/资源逐成员取证 |
| 节点状态角标真实资源与状态图 | 色值为独立近似 | 原厂状态资源证据 |
| 参数窗口字段级对齐 | 未逐项完成 | 原厂模板、绑定、事件、参数键 |
| 真机原厂差分 | 未执行 | 匹配原厂宿主与 `test1108.sol` |

## 5. 与 G9 续作门槛的关系

CHECK-FR 未通过（`docs/reverse/T4-first-fidelity-slice-20261112.md` §5），因此 **G9 工具批次续作保持 `blocked`**，本合流验收通过不构成解除条件。T5.1 取得的真实 `ToolGroup.xml` 优先级数值同样只登记证据，未接入产品配置。
