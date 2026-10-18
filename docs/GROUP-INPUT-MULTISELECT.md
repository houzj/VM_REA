# Group 多选输入的原生命名边界

## 原始代码证据

`D:/MyWork/REA逆向测试/analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/GroupParam.cs` 的 `AddCommandCmdExecute` 在“多选”状态打开 IO 选择器，逐个选择叶 IO 或组合 IO，并把是否包含模块名传入 `AddLeafIO` / `AddCombinationIO`。`FixName` 在初始命名时截取前 32 个 .NET 字符；`GetUniqueName` 从同一 Group 的输入和输出全集查重，重名时从后缀 `0` 开始尝试。成功添加后调用 `SetIoParams` 更新原生接口。

## 本次闭合范围

之前的独立 WPF Group 输入多选实现把来源名截到 24 字符，且仅与已选输入查重；若来源端口与现有 Group 输出同名，会生成不符合原始命名规则的接口。现在多选名称先截到 32 字符，查重范围包括 Group 现有输入、输出和本次已选项，首个冲突使用 `0` 后缀。调用端仍通过现有 `ConfigureOwnedGroupInputs` 把新增接口、父级订阅和稳定 ID 一次提交；保存/撤销与底层校验未被 UI 替代。

独立 `tests/VisionEditor.GroupSourceChecks` 在真实 Group 配置窗口中取消“包含模块名”，多选父级工具的 `out` 与 40 字符长端口。内部 Group 已有 `out` 输出，实际新输入分别生成 `out0` 和长端口前 32 字符；父级生成两条订阅。完整父流程运行时，父级真实模块先产出数据，Group 真实节点执行得到 `out=5`；保存独立方案、重开再运行仍得到 5。专项累计 20 项通过。

## 尚未声称还原

原生组合 IO 会按模板生成多层叶字段，并用叶数检查 1024 上限；当前独立框架只对已实现的端口和扁平多选接口闭合，不能把平面列表称为原生复合 IO 完整复刻。原生 UI 资源像素、SDK 的 `SetIoParams`、全部组合类型和 `.sol` 往返仍需验证。测试中的数值模块仅作框架数据流夹具。

## 2026-10-18 单一类型添加上限

原始 `GroupParam.AddCommandCmdExecute` 对单一叶类型先添加，再检查 `owner.Inputs.GetAllLeafIO().Count() > 1024`，超出时移除刚添加的接口并显示错误；多选分支则在创建前比较现有叶数与新增叶数。重建窗口的单一类型入口现在于已有 1024 个扁平输入时直接显示上限错误，不创建接口。底层 `WorkflowConfiguration.Validate` 的 1024 项防护继续有效，避免非 UI 路径绕过。

专项真实 WPF 检查将已有输入扩充到 1024 项，点击类型选择与“添加”按钮后确认窗口提示、接口数量及旧 ID 不变，保存重开仍为相同 1024 项；达到限额前，专项已经验证多选来源的父流程实际执行和重开后 `out=5`。本次专项累计 22 项通过。这里的前置拒绝是独立实现，不宣称与原生“添加后回滚”的短暂对象生命周期完全一致；组合 IO 的叶数限额仍属开放范围。
