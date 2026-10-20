# 分支字符串：最终源→实施→自审退出

2026-10-20。专属实施与本轮自动验证完成，提交根线审计；不自行计入全局工具数量。人工验收另列，不作实施完成门。未装载/执行原厂DLL、未设备测试、未提交推送。

## 原件身份与证据入口

原件目录 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Logic/BranchStringCpmL`。

| 原件 | SHA256 |
|---|---|
| native DLL | 8DE8FE3CA4931B8201F52A310939A7C580D2566FF5606A9A7C31033C97B0B9A2 |
| Cs DLL | 3BE0C1017DAA7F7F001295A9C548636A545CA5BC84B73429E3381EC04C513A96 |
| Control DLL | 0BF9271E5157D7CFB92C73046D9EFDB429A883F2AE5B08ABCA71F180E3491797 |
| ToolItemInfo.xml | 3650CCCF4A710F796B66921ECEC3D3E6FD6265228B5E4DA03DD01D7B0D6CD394 |
| ModuleFrame.dll | 45C57BC46F72FD1F9E0B3A6F9183FF974D14590CFD05ECF4E9FA97A4FF95AE9B |

ToolItemInfo name/主XML ModuleName为BranchStringCpm；目录/DLL/Cs构造base为BranchStringCpmL。既有产品catalog保持后者，映射ModuleId=vision.branch-string，不伪造原统一名称。

主证据 `analysis/branch-string/source-20261020/`：cs-decompiled、control-decompiled完整托管源码及BAML-XAML；resources原四XML；native-signatures校正IAT函数签名后的C与asm；frame为Frame保存/GetString具体体。初次native/17890.c误推GetNext仅一个参数导致优化删比较块，保留为失败证据，不能作算法依据；校正GetNext2、SetNext2、GetString7参数并以原asm复核后使用native-signatures/180017890.c。

## 完整退出矩阵

| 项 | 原具体活动链 | 产品实施 | 实际验证/结论 |
|---|---|---|---|
| 身份/注册 | 原XML/ToolItemInfo、Cs ctor | BranchStringPlugin及真实catalog/MainWindow注册 | Host1..2 |
| 配置/模板 | AlgorithmTab→HeadBar/ListParamStr；StringA ButtonSelecter；IndexUserSet整段注释；仅Confirm | 原资源内嵌、共享HeadBar/模板目录、专属ListParamStr工厂；不造执行/Reset/模式 | Plugin39..49、Host12..14；Template1474 |
| 文本/表写入 | Row.InputValue UTF16<=30禁#/$允许空；TextChanged/debug/up/down→ValueChanged→SetParam，无PreRun | ListModel/Controls/Configuration；保留UI attempt与backend canonical不同 | Plugin25..31、43..47；源码恢复setter18970 strtok空token跳过 |
| 输入/错误 | Process17890→GetString StringA,index0,capacity256；Frame11d440 outLen含NUL；error或Count<1不写输出并清下一表 | typed scope reader优先、Graph callback次之、materialized末位；独立透传Count/RequiredBytes | Plugin各输入/数组首项/NUL/容量/E314/E12/E14/Count0/outLen0；Contract16 |
| 选路 | byte equality+同长度；flag==1追加(D)，再找真实后继；结果非空判status；GetNext=0保空结果 | Process原条件、SelectedExecutionSuccessors，真实Graph调度 | Plugin比较/顺序/debug孤悬/空串；Host5/9/11/16 |
| 拓扑 | NextModuleListChange→18e30→183c0过滤→18ec0 merge；保旧行、追加空text/flag0；Cs连接32门 | IToolExecutionTopologyProvider事务；相同顺序不重建，真实连接事件更新 | Plugin32..34、Host3/24 |
| 复制 | Cs Paste先ModuleOperate，再map存活目标；PasteFinish重写；Setter实际空字段位移 | IToolNodeCopyProvider，先seed后映射并ApplySetter | Plugin35..37、Host21..23 |
| Save/Load | Save196c0→1df90→Frame12fca0 virtual+20；CurrentConfig1257a0 GetParam(cap0x400)；Load193b0/1e0a0→193d0→SetParam | PrepareSave不改变参数；真实文件Load的IToolNodeRestoreProvider仅参数roundtrip；打开配置/Undo不假模拟Load | Plugin54..56；Host6..11。原空默认id##0保存、重开变id#0#0保留 |
| 输出/显示 | Runtime、StringMatch、BranchMatch、ModuStatus；后三CanSubscribe=false；Show Text与Data Record分离 | 描述符元数据、scoped display、原文字模板 | Plugin52..53；Host17..20，真实debug同procedure scope |
| 生命周期 | ctor/init、Reset cause2跳默认、destructor177c0→1a260/1bf40；无正常Reset UI | 无虚构Reset操作；scope所有权/lease，节点删除释放snapshot | Host25；原native堆销毁不在托管实现域 |

## 本轮验证和快照

严格串行、独立短artifacts根 `analysis/b-string/artifacts`。最终均exit0：BranchSelection.ContractChecks16；VisionConfiguration.TemplateChecks1474（294 XML/group、189模板）；BranchString.PluginChecks56；BranchString.HostChecks25。首两构建0warning/error；Host首次完整宿主编译保留既有MainWindow CS8603两条，后续增量无新增生产警告。

实际MainWindow检查覆盖真实doc连接、RunProcedure、Save/Load、配置创建/关闭、NodeDebug原图后继与scope、Paste/Undo/Redo、Disconnect/Delete；不是手工代入插件输出。模板元数据通过不等于像素一致。编译PDB五程序集184文档与当时当前源码SHA256核对0差异，见analysis/b-string/{compiled-sources,source-review}.json。最终源码/资源/日志/DLL/PDB清单为final-manifest.json，记录精确哈希。

失败历史完整保留：host-run-first-path-failure.log测试错误catalog路径；host-run-2.log测试误把catalog根对象当数组；host-run-3.log测试未安装DispatcherSynchronizationContext导致Task.Yield在线程池触发UI Guard；仅专属测试修路径、Groups入口及与既有Host测试相同Dispatcher环境，最终host-run-4.log25通过。未修改产品断言以隐藏失败、未修改算法容差。

## 明确有限边界与独立适配

- 原Process/Frame只做静态源码+原指令核实，未执行原厂native。P/Invoke ABI、原平台/远端服务、设备、原生heap、输出setter实际故障注入未运行验证；SetNext/SetOutput错误原体仅log后继续已记录，托管不可失败输出容器不伪装这些故障。
- 宿主稳定NodeId目前无原厂numeric instance ID提供者。NumericId已提供才输出原ID%d;并应用原数值过滤；缺失输出明确NodeId:<id>;，消息标独立适配。不得GUID atoi。选路基于真实连接NodeId，非伪造数字ID。
- 原Save GetParam固定0x400容量，长表失败只log且仍保存参数entry；GUID字符串字节长与原数值ID串不相等，现JSON保存不按GUID长度冒原native1024门。此容量/身份域明确独立，不称全域持久化等价。
- 正常UI文本为有效.NET/UTF8，Ordinal完整相等对应有效编码字节比较。非法原始UTF8、任意native setter畸形flag/溢出和隐藏OutputForEnd native ID通道不伪造；插件只暴露原声明活动输入/结果及正常配置/复制/恢复路径。
- Runtime测时为独立托管耗时，非原CPU逐位计时。UI资源/活动尺寸和绑定已源确认，不宣称整个原桌面像素/本地化全域一致。
- 无已确认尚未实施的当前选定正常工具活动缺口；额外动态SDK setter不自动扩大为该工具正常UI要求。以上边界不由PASS测试消除。完整工具是否计数由根线独立审核。
