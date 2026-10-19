# C02 第七批：清值后的原 Value 消费链

2026-10-19，基线 `39a54c8`。有限子项 `C02.clear-value-consumer` 完成：18 个聚焦原方法体完成 CIL 审查；原 Value getter 在合成受控图的 **5 个固定案例、35/35 断言通过、exit 0**。未修改产品，也未重复第六批清值叶32断言。**C02.3 仍为源码达成、实现部分、验证部分，不能升级为完整达成。**

## 来源与精确安全门

依 reverse-engineer-anything 工作流复用同版本 decompiled 与已保留的 REA 64MiB 成员表示预算阻塞，不重复成员清点、不虚造 REA Evidence ID。来源 `analysis/decompiled/VM.Core/VM.Core/VmIO.cs`：Root:81、RelateIO:121、Value:330、IsLeafNode:450、IsReturnRelateValue:529、LazyLoadRelateIO:837、MergeIOLoopValue:882；`VmModule.cs` 的 Root:151、Inputs:339；VmIOCollection 无参构造与 VmFrameTreeBase Parent/Name。

静态导出 `tests/NativeEvidence/C02ValueConsumer/Export.ps1` 用 ReflectionOnlyLoadFrom，最终18体位于 `analysis/full-audit/batch7-20261019/C02/static-final/VM.Core.dll.cil-disassembly.txt` 与同名cil-methods.json。实际 Core SHA256 `936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d`。

原 getter `0600051e` IL SHA `9a3d37bdd26e0989652459035d678c81b2fe7273ce8929fbd5d98e474d669db6`，merge `06000521` SHA `067ebca9412923d503fad291090b8c2549d0842de4eb279abdbcaa67f8db6a9f`。执行前硬核整程序集SHA、18体CIL哈希及各继承链 TypeInitializer。VmIO、集合与相关父链无cctor；VmModule唯一cctor `06000386` SHA `d109a35cc139ca44e9e7dd925169c0f909662961ed48748d10baafc202380c5a` 仅创建四字符串数组与Dictionary，经原CIL确认。

未调用普通 VmIO/VmModule 构造，使用 FormatterServices.GetUninitializedObject 注入原字段。仅 VmIOCollection 无参构造执行：三个Dictionary与系统ObservableCollection，未加入集合项，避开 OnCollectionChanged。原壳不等于实际VM模块。

原 getter 首步无条件 RelateIO→LazyLoadRelateIO；Root=null会在此处空解引用，不能以空Root宣称安全。因此每叶使用非null原VmModule壳，`relateIO_IsLoaded=true` 使 LazyLoad CIL_0013..002b 不可达，不进入LoadModuleRelateIO；`inputs_IsLoaded=true` 使Inputs CIL_000e..0049不可达，不访问VmSolution.IsLoading/日志/动态输入；`isSetCallBack=1` 在Value CIL_00c9跳过RefreshResult；Module.Root所经Parent=null，仅读并缓存null，不走真实流程实例。输入字典直接注入，不调用订阅setter、属性Name setter、真实SDK或Dispose，不设置原static，不在异常后继续测试。

## 数据流与操作结果

每例先读Value，再重新布置同一输入状态，调用原SetNoneValueWithoutRaiseEvent，确认_value为空，再调用原Value。前读不会污染后读的merge/list输入。各例检查前值、清值后原字段、消费后值、列表长度、merge标志、隔离门字段、ExecuteCount，共7项。

| 固定案例 | 清值前消费 | 清值后Value | 列表与标志 |
|---|---|---|---|
| merge=false，无订阅 | [9] | null | 三项列表保留，merge=false |
| merge=true，数组[1,2]、非数组、数组[3] | [1,2,3] | **[1,2,3]** | merge置false，列表清空 |
| merge=true，仅零长数组和非数组 | [9] | null | merge置false，原两项列表保留 |
| RelateIO存在、输入leaf字典命中Name哈希 | [7,8] | **[7,8]** | 返回源Value，当前列表三项保留 |
| RelateIO存在、未登记输入leaf | [9] | null | 不转发源，当前列表保留 |

原CIL精确行为：RelateIO不是无条件值转发；还需IsReturnRelateValue及Root.Inputs.leafIOs包含Name哈希。命中即直接递归返回源Value，不先清本叶缓存。Root为VmModuleSet时IsReturnRelateValue固定false（本轮仅静态，不构造该类）。否则可能RefreshResult，然后MergeIOLoopValue，最后返回_value。merge先降标志，累加所有Array.Length，正长度才逐Array.GetValue复制到object[]、清列表、替换_value；总长为0则保留列表，已降merge标志。不存在将非数组元素包装为一个值的分支。

因此Begin叶写_value=null不保证下次消费者观察为空：保留的loop数组可重新聚合，输入订阅也可能读到源值。这里只证明这两种原方法分支在声明的合成状态可达；不声称真实Begin时这些状态必然同时成立。

## 原输出、复验和失败保留

最终运行 `analysis/full-audit/batch7-20261019/C02/run-5cd8eae2e88d460bace9b6f0b51d5d54/`：probe.json、process.json、stdout/stderr、exe、哈希清单。入口 `tests/NativeEvidence/C02ValueConsumer/Run.ps1`，只用Framework csc，不构建产品。实际强名绑定Core来自GAC，SHA与原件一致；每个实际托管程序集路径/哈希及本进程原生模块清单均保留。VM.PlatformSDKCS与Apps.Log因类型/JIT依赖加载不等于API或日志初始化执行；原生模块清单未见VM/Vision/MVD/Log命名模块，执行边界同时由CIL门限定，不仅据模块名判定。

初次元数据编译因csc相对斜线参数解析失败，未执行目标，记录在static/attempts.txt。第一次运行5例35项已经通过；第二次增加逐CIL门，第三次增加最终Name覆盖与进程模块审计，均保留各自run目录。**只统计5例35项，不将重复运行累加为15例105项**。静态18体包含完整RelateIO非叶分支，但本轮只运行叶分支。

## 剩余门槛与进度

C02.3不能从partial升级：尚缺受控真实VM实例Begin→同步/队列回调→实际消费者状态，Root.RefreshResult与历史计数关联、真实loop列表生成时序、VmModuleSet/非叶订阅、显示GetValue(count,index)、异常/线程/资源生命周期均未由本轮操作闭合。未验证实际界面、设备、SDK执行、原机差分或图像释放。下一任务应取得真实初始化隔离条件后做上述操作链，不能再重复派发此5例getter消费切片。

源码、实现、验证的96项分母不变，本轮该单元状态不变；新增的是其验证证据强度，不冒称完成率上涨。tool-plugins未读取或修改。
