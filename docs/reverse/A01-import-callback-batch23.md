# A01.3 第23轮：原 Group 导入回调到模型落地复核

## 结论

本轮实施增量 **0**。A01.3 仍不能完整关闭；没有新增可实施的 main 代码。当前 `main` 基线为 `a6aac9d0aaad027ff4c1bf107b4701a230b4caa7`，`tool-plugins` 未访问、未修改。

## 本轮核对的源码链

复用已保全的原始反编译源码，不把上一轮静态结论改名为新成果：

| 原路径和成员 | 已确认行为 | 仍缺的 main 交付合同 |
|---|---|---|
| `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/DecoratorBase.cs`，`DecoratorBase` 生成 GroupTools `.gro` 清单段 | 导入载荷来自 GroupTools 目录，不是通用 JSON 节点快照 | `.gro` 编码、版本和目录条目到 native 导入参数的解析 |
| `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs`，`LoadGroup`/`internalImportGroup` | 传入目标父 ID、`nIdAllocateWay=0`，设置导入状态并等待 native 完成标志 | native 返回 ID/进程 ID/名称的消费者及等待/超时语义 |
| 同文件 `MModuleManager_OnImportGroupEndCallBack` | 回调先通知公共 `OnGroupImportEndCallBack`；错误为 0 时才构造 `IMVS_MODULE_INFO`，经 `AddModuleByInfo` 建模，设 Parent/Ready，最后置完成标志 | main 没有该回调入口、公共事件顺序、失败时 flag 恢复合同 |
| `analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs`，`AddModuleByInfo` | 按 Name+ID 复用或构造节点，不以 GUID 去重 | main 无 SDK module-info 到节点模型的适配器 |

上述文件 SHA256：`VmSolution.cs` `C141E2C53913B233B768782D6757FAE5997ECAF638B35DB2DDF98F5EF951A13E`；`VmModuleSet.cs` `F3B98C4436686B0A9BABF8CDE4F5F6C83814664B8F7835DFABBEB02E2A64A346`；`DecoratorBase.cs` `25C71C878E2EA21678DB2FBF1E69FEEF0D2892F9356CAE443F747F79D0A83299`。

## main 现状与退出门槛

main 目前只有 `ModuleRegistry`、`OwnedGroups` 等独立托管模型；没有 `.gro` reader、native `ImportGroup` 适配器、回调载荷（new ID/process/name）或 `AddModuleByInfo` 等价消费者。`OwnedGroups` 的 JSON 所有权校验不能替代原 SDK 导入时序，因此本轮不写入独立兼容实现，也不计实施单元。

完整关闭至少需要取得并实施：

1. 可复验的 `.gro` 解码/版本合同；
2. native 导入返回的 ID、进程 ID、名称及错误/完成标志；
3. 公共事件先于模型落地、失败不落地和完成标志恢复的 main 入口；
4. Name+ID 复用规则、Parent/Ready 更新及后续引用映射。

本轮没有运行安装原件 SDK、设备、WPF 主窗口或原生导入；这些人工/原机验收仍不计实施分子。

## 自审核

- [x] 未修改 `src/`，未触碰 `tool-plugins`。
- [x] 未重复派发已关闭的 A01.2 palette 单元。
- [x] 每个未决点绑定原始源码路径和精确成员。
- [x] 未把 JSON Group 或截图当作原 `.gro` 合同。
- [x] A01.3 实施增量保持 0。
