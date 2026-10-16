# Group 输入编辑、运行参数代理及独立运行

2026-10-16。框架 main；工具插件实现与另一工作树未修改。前阶段说明见 GROUP-BREAK-ACCUMULATION.md，全范围见 GROUP-FULL-RECONSTRUCTION-CHECKLIST.md。

## 直接源码证据

原始反编译文件在 `analysis/decompiled`；本轮读取文件的SHA256见 `group-input-runtime-source-manifest.json`。

|来源|确认行为|本轮接入|
|---|---|---|
|Apps.XmlParser.ParamTab/GroupInputParam.cs|Owner.Inputs、默认in、DynamicInputParam|Group拥有型输入，不再复用只读跨流程页|
|DynamicObjectConfigControl/GroupParam.cs AddCommandCmdExecute|类型新增、多选、包含模块名、总数1024校验|现有数据合同类型新增、批量选择、唯一命名和原子保存|
|DynamicIOConfigControl.xaml、VmIODecorate|70/68/Star/42输入行、32高度、6间隔、名称LostFocus、订阅选择、删除|WPF实际输入行和原始加删图标；改名/删除引用传播|
|DynamicRunParamConfigControl.xaml|44高标题/编辑入口、参数内容模板；非标签清单|真正可编辑的内部参数代理，实际读写内部节点参数或输入订阅|
|GroupRunParam、RunParamConfigDecorate、DynamicNameValueItemOfGroup|参数克隆、名称、关联列表、值传播和多参数同步|基础参数/输入代理编辑、重命名、订阅/解除、删除；原生类别/多关联配置仍开放|
|DynamicParamsTabControl.xaml.cs（145..189附近）|ContinuousSelfExcuteModule、StopSelfExcuteModule、SelfExcuteModule|Group单次、连续、停止命令；拥有型Group不再调用整个父流程|

## 输入与数据流

- 可新增当前合同支持的int/float/string/image/circle/components输入，类型只读，名称编辑校验；批量选择父级来源，继承标量/数组基数。
- 父级节点输出、输入和局部变量能绑定到Group公共输入。输入值通过稳定ID注入内部WorkflowInput订阅。
- 名称改变按稳定ID更新父级入边和订阅目标；删除清理父级绑定、内部输入订阅及失效中断引用；保存/重开、复制沿已有稳定ID链路。
- 批量和改绑使用ConfigureOwnedGroupInputs单个编辑事务；校验失败不留下部分接口；一次Undo恢复接口和父级绑定。
- 现有合同之外的原生复合输入仍未实现。父流程节点来源必须遵守已有执行先后约束，不靠自运行自动重排。

## 运行参数代理

- 编辑入口多选内部参数/输入；去重添加代理，实际编辑基础类型值或选择输入来源。
- 参数元数据的类型、必填、数值范围由框架验证；无效值保留旧参数并显示错误。bool使用原始开关位图。
- 标量参数可订阅Group输入、私有LoopIndex、局部变量和内部节点输出；已订阅值显示来源且只读，解除后恢复已保存常量。
- 代理删除只移除代理配置，不重置内部节点真实参数。
- 这不是完整原厂运行参数配置器：原生类别增删/排序、一个代理关联多个节点及同步按钮、复杂参数专属弹窗尚未恢复。当前编辑入口使用已有选择器，不能宣称DynamicRunParamConfigPanal已逐控件等价。

## 独立运行与生命周期

GroupDebugExecutor准备当前Group的真实输入和参数订阅，隔离父级其它节点后复用SolutionExecutor的同一Group循环/中断/累积路径。缓存来源需明确为成功节点输出；父级常量变量无需执行父流程。缺少来源报错，不执行上游、不用0代替。

- 拥有型Group运行只包含目标节点与它拥有/调用的内部图；未配置的上下游/兄弟不影响独立运行；不会触发其模块。
- 独立运行保留内部Group输入快照，但父级会话保留原父级输入，避免临时Group注入参数泄漏导致第二次失败。
- 清理目标/下游旧结果并保留可用上游缓存；单次、连续执行均按完成结果等待，失败停止；Stop/关闭合作取消并等待内部释放。主流程运行会避让Group调试。
- 旧跨顶层流程调用保留原兼容路径，不标记为原生拥有型Group。
- 原生SelfRun首次准备/缓存失效、回调ABI、设备和全部错误码没有完成动态差分。显式缓存政策、Task调度、连续失败停止是独立实现政策，不能称原生调度完全等价。

## 实际验收

专项验证包括：隔离执行次数、输入值7加参数2得到9；缺少缓存不执行任何节点；改名仍传值；删除与Undo恢复引用；参数改5后得到12；保存重开；重复独立运行；常量变量来源；取消；批量事务回滚及一次Undo。

WPF验证实际点击输入新增/订阅/改名/删除/批量选择/Undo、运行参数编辑/订阅/解除、单次/连续/停止/重跑及关闭。关键窗口操作：父级输入3，代理值5，输出8；参数订阅同一输入后输出6；解除后恢复8；故意未配置兄弟节点未执行。不是只验证配置字段。

`analysis/full-audit/wpf-group-input-editing.png`、`wpf-group-runtime-proxy.png` 为tests离屏渲染的本项目真实WPF控件，已读取检查；不是VM截图。截图/导出代码仍在tests。测试和发布最终结果见HANDOFF。
最终结果：专项18、实际WPF操作300及现有回归通过。发布版与Core/Plugin/WPF测试均在60秒内退出且exit0；新入口指向dist/VisionEditor-WpfGroupInputSelfRun。
