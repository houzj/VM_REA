# 分支模块：当前声明工具范围实施退出自审

2026-10-20，根线复核原件身份、原生运行与托管配置/复制链、生产源码、44项专属与15项真实宿主记录、186份PDB源码哈希无差异后，确认本工具在当前声明配置和正常宿主范围内完整实施并通过自审，计入下一批工具数。原厂全域/位等价不作结论。人工验收独立，未执行、不是实施退出门。

最终证据：`analysis/b-branch/final-manifest.json` SHA256 **8BDF10B887FFB7CF651A23358960A2D3A55D7D17EBD218079175309E5EDF02A2**；5程序集PDB共186源码文档与当时当前文件hash **0差异**。原native SHA256 **47E5AE68E1744C0A899D70A7F98DB1723C1C4B921616B462D3B12D94A1F9AC73**。源码/指令与资源索引见 `BRANCH-MODULE-SOURCE-RECOVERY.md`。

|路径|原始证据→实施|实际自动验证/自审结论|
|---|---|---|
|条件整数输入|Process 1800078f0按GetInt(index0)读错误和Count，失败/Count<1清后继且不写结果→专属typed reader优先与空SelectedSuccessors|Plugin44含scope优先、真实Graph运输、错误和显式成功Count0早退；materialized空值E314是完整Frame运输边界|
|按值/按位|78f0行序扫描；mode0 signed相等，mode1按行索引SAR/TEST、x86掩码31，其它mode不选→BranchModuleRuntime|Plugin44覆盖真实已连后继顺序、按值多匹配、按位、空选择、未知mode；生产Graph抑制未选后继|
|结果/显示|原Display XML固定ModuStatus与BranchMatch，Process结果→现公共display与scope状态|真实Host15配置运行有显示且结果BranchMatch准确；不虚构原隐藏OutputForEnd数值|
|配置/模式/表格|原ListParam/Item/Radio/HeadBar和6原PNG→专属leaf注册，共用WPF宿主|Plugin44真实View创建/Measure/Arrange无缺模板，真实radio与移动按钮；模式/排序无PreRun|
|N的后端与编辑器状态|native SetParam atoi(N)=0、GetParam返回数字；ListParam当前行memo与ValueChanged OldValue独立→后端表canonical0、当前State保attemptN|Plugin44实际bit排序后持久化0；同编辑器切回逐行恢复memo、列表写先于IndexUserSet；重开切回值为0；无隐式运行|
|邻接变化与32门|ModuleOperate→NextModuleListChange、8d60过滤；SpModuleCheck现Count>=32拒新增→纯provider+宿主事务hook|Host15真实连接/断开保值、32允许/33拒绝且无半提交；打开/Load/Undo/Redo不造邻接事件；超限外部文档允许删除|
|配置self-run|原Tool.Run→SyncSelfRun仍读取真实native邻接→Graph只读successor override|Plugin44实际isolated生产Runtime/Host15真实配置Execute均见原图后继；isolated不调度不存在节点|
|复制|Tool.Paste先ModuleOperate再仅mapped原行覆盖，PasteFinish再写缓存→IToolNodeCopyProvider与完整copiedSuccessors/map|Plugin44 rawN/值/终止符及无映射种子；Host15真实Paste部分目标重映射、单节点复制无旧ID，Undo/Redo保持准备表|
|订阅元数据|原主XML三个CanSubscribe=False→VmModule.InnerAnalysisIOInfo→GetOutputVmIOTreeNode门→三PortDescriptor.False与共享过滤|Plugin44实际树不列Branch三个禁用输出、伪造Bind拒绝；根线共享WorkflowSubscription60/合同9另有记录；ModuRunTime按原默认true|
|保存重开|原两参数与native参数保存/Load映射→当前文档参数与NodeId复制映射|Host15真实Save/Load保表；资源无另需资产；N归一化按后端保存而非瞬态Defaults文本|
|关闭/释放/Reset|原7830/a310释放属native所有权；原BottomGroup无Reset按钮→不造按钮，现stateless Runtime/scope结果与会话Dispose|Host正常关闭退出无残留测试进程；配置release解绑事件并DisposeAsync停止执行；根线既有scope/node生命周期合同复用。无设备/native heap实例，不能把托管清理称native ABI复制|

最终真实结果：`build-mode.log` 0警告/0错误，`run-mode.log` **44 PASS**；`host-build-mode.log` 0错误、两条既有MainWindow CS8603，`host-run-mode.log` **15 PASS**。首Color编译失败、ListParam目录缺失、误解几何的失败日志全部保留。几何完整源链：原ParamsConfigControlBase中文fallback强制CustomTabWidth=CustomTabCHWidth默认420，故当前420×420符合实际宿主，XML原380不直接代表最终宽。

## 明确有限边界

- 宿主仅稳定NodeId，没有原native实例numeric ModuleId来源。已映射NumericId才输出原`ID%d;`；当前真实宿主输出明确`NodeId:<id>;`且消息标独立身份适配，不atoi GUID、不虚构0。数值角色过滤仅在NumericId已知时按8d60原谓词；未知NodeId纳入正常连接属明确独立运输。
- 原Process三参ABI由RCX/RDX/R8指令确认，伪码LARGE_INTEGER不是真签名。原SetNext/输出SDK写失败仅日志/最终返回0；现Graph返回的选择集是独立执行合同，不冒原平台SDK写错误及native日志ABI。
- bf30动态透传与DynamicInData/OutData保存体已取；本工具原声明XML/AlgorithmTab、Cs类没有相应正常配置入口。额外SDK setter、外部native动态blob/远程provider不自动扩作本工具声明UI必需功能，具体恢复可达性未验证，未称支持。
- 原危险char256拼接、畸形native指针/越界堆、未初始化结果不复刻。安全托管字符串与异常、document原子事务Undo不同于原SetParam部分提交，独立边界明确。正常UI签名int域与32连接门实施；不泛称任意atoi溢出串等价。
- 原DLL没有装载/受控执行，本次证据为原反编译体、原指令、资源与当前产品实际运行；通过专项不替代原厂全域运行比对。人工验收、提交、推送及跨项目采用均另列。
