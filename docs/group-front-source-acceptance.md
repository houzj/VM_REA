# Group 前置来源选择验收

## 原始证据与边界

- `analysis/full-audit/subprogram-evidence/IMVSGroupAlgorithmTab.xml` 的 `LoopStartCount`、`LoopEndCount` 选择器调用 `GetFrontFilterTreeOperation`。
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/VmModuleExtern.cs` 中 `GetFrontFilterTree` 从 `vmModule.GetFrontParamItems()` 建立候选。
- `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` 中 `DeGetFrontOutputItems` 递归读取 `GetPreModules()`，用已加入列表防止重复遍历；`GetFrontOutputItems` 另外加入父模块集。后置、无连接模块不属于前置模块递归结果。

当前框架独立实现以同一流程的执行边与数据连接边反向遍历，取得 Group 的前置祖先，且从候选中去掉 Group 自身。原始 `PreModules` 由 VM 内部图维护；两类边映射到它的全部细节、异常环图处理和父模块集中的原生字段树，仍需真实 VM 差分确认。局部变量、流程输入与 `Group.LoopIndex` 仍按原有范围呈现。本改动同时约束共用来源提供器的 Group 输入与循环边界，未改变其它流程的选择器。

## 本线实际验收

运行 `dotnet run --project tests/VisionEditor.GroupFrontSourceChecks/VisionEditor.GroupFrontSourceChecks.csproj`：真实 WPF Group 循环结束值选择器只出现前置节点输出，不出现后置与无连接节点；选择前置值 2 后，Group 内部索引工具实际运行两轮并获得索引 0、1；撤销恢复一轮，重做恢复两轮，保存重开后仍为两轮。专项程序输出 11 项通过。测试所用模块为本地可执行验收模块，不能当作 VM 原生算法证明。
