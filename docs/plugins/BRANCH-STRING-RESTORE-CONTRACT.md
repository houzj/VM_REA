# BranchString：真实恢复的窄合同请求

Producer：工具专属BranchString表参数恢复；consumer：ToolPluginRegistry与真实GraphEditorDocument.Load事务。只请求源已证的参数变换，不做设备、平台初始化或插件算法运行。

## 原链与可达反例

- String vtable Save 1800196c0→1df90→ModuleFrame.CAbstractUserModule::SaveModuleData(18012fca0)。Frame原指令12fcd2取vtable、12fcd5调用槽+20，RDX仍为Save参数结构；String该槽19390把RDX转R8并传BranchStringCpmLAlgorithm.xml至VM_M_GetCurrentConfigByFile。该XML唯一参数ModuleInfoList。
- ModuleFrame VM_M_GetCurrentConfigByFile(132fe0)→1257a0；核心按XML参数名在输出ParamList建立Name/Value条目，虚槽+10调用String GetParam(17ff0)，value容量0x400，随后计数递增。GetParam→18610按当前vector序列化id#text#flag$，text空仍保留两个#。
- 拓扑更新18e30→18ec0直接构造text空、flag0的新vector行；未经过SetParam。故新增正常连接后GetParam的默认行是id##0$。
- Restore 193b0→1e0a0先调用槽+40=193d0。193d0对保存ParamList逐项调用槽+18=181e0，再做native数字ID映射；181e0→18970在$和#两层均strtok、跳过空token。因此恢复id##0$会成为id#0#0$。Copy原Cs同样先ModuleOperate，再把mapped行提交Params setter，需相同setter语义。
- 这不是希望的修正，而是原有正常可达差异：默认行在保存前匹配输入"0"失败，保存本身不改变当前vector；真实重开后该行text成为"0"并匹配。不能在PrepareSave提前归一化，因为本产品Save把返回document作为当前文档提交，会错误地提前改变当前运行。

## 最小合同

Contracts建议可选：

```csharp
public interface IToolNodeRestoreProvider {
    NodeDocument PrepareRestoredNode(NodeDocument node);
}
```

插件纯方法只变ModuleInfoList参数，保留node ID/module/version/布局/订阅身份与其它参数，不调用SetParam SDK、运行、邻接事件或读取设备。当前NodeId稳定身份不转native numericID。

Core建议可选`PrepareLoad(SolutionDocument loaded, string baseDirectory)`（名称由根线统一），在SolutionStore.Load与Separate完成后、document/FilePath/BaseDirectory/Undo栈/Revision与SolutionReplaced提交前调用。宿主用ToolPluginRegistry遍历有此可选接口的节点。返回准备后文档；异常保持旧文档、旧路径、旧Undo/配置窗口与executionScope，只有成功提交后走原SolutionReplaced释放旧scope。

只在真实文件Load触发一次。NewSolution/AddNode/配置打开/Save/Paste/Undo/Redo不调用；Paste已有专属复制provider按原顺序自行应用setter。默认null和未实现接口工具保持旧行为，不全局反序列化修参数。

## 最小验收

1. 新连接生成id##0$，配置打开不改变；输入"0"运行不选。
2. Save返回后当前表仍id##0$、当前运行仍不选；Load同一文件后表id#0#0$，输入"0"可选。
3. 非空text与0/1flag重开稳定；多行保持顺序、NodeId不变化；空text+flag1经setter变text1/flag0。
4. Undo/Redo和配置重开不模拟nativeLoad；复制调用既有完整nodeMap与copiedSuccessors并只处理mapped行。
5. callback抛错时旧document/FilePath/Revision/Undo与scope保持，未注册provider工具载入不变。

原件：BranchString native SHA8DE8FE3CA4931B8201F52A310939A7C580D2566FF5606A9A7C31033C97B0B9A2；ModuleFrame SHA45C57BC46F72FD1F9E0B3A6F9183FF974D14590CFD05ECF4E9FA97A4FF95AE9B，MD5B5A2A63FA6CDCD718C0BCCBB84B50C87与被复用旧DB的import日志一致。具体原C/asm均在analysis/branch-string/source-20261020，Frame复制DB仅在本工具专属project只读处理，未修改旧Camera项目。
