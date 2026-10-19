# D01 batch1：保存调用与原生边界复核

2026-10-19。本轮是静态源码与同版本映像坐标复核；D01仍开放。没有调用SDK、设备、原机、XAML或写入安装目录，未触碰tool-plugins。

## 已确认的调用与回调

路径以下相对仓库根；原始源码哈希见本轮 `analysis/full-audit/batch1-20261019/D01/review/source-sha256.csv`。

- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs` 166–211：正常保存先SaveCalibUserData，逐流程SaveCalibData，再进入方案导出。不能只监视InternalExport而漏掉保存前准备。
- `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs` 1551–1597：TempVmSolution.Init，防重入、密码/路径选择，SyncSaveSolution(path,text,IntPtr.Zero,0u)。取消路径清_isSaving；异常清_isSaving和SolutionPath。正常调用后更新密码/路径，但不是托管打包MoudleFrame。1882附近ExportAs走相同原生保存签名，但不含正常路径更新，需分入口记录。
- `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/SolutionManager.cs` 305–323：清错误码和wait flag，调用IMVS_SaveSolution；立即错误抛异常；随后清ParamSeted、等待；只有SDKMode才再检查异步错误。不能以返回0等同磁盘保存已成功。
- 同目录 `ServerSDKAPI.cs` 286–288直接转发；`NativeMethods.cs` 115–116的五参数为handle/path/password/pData/nDataLen。**只有path显式使用UTF8Marshaler**。`UTF8Marshaler.cs` 26–41实现UTF8字节加NUL、AllocHGlobal，CleanUpNativeData释放。password没有同样属性，不能标为同一编码。
- `CVmObjectBase.cs` 493注册平台回调；653起delegateOutputCallBackFunc先Marshal.PtrToStructure读取IMVS_OUTPUT_PLATFORM_INFO，按nInfoType分派：16/17/18为保存开始/结束/进度，19/20/21为加载。数据来自pData，不能把这个回调指针当方案字节流。
- `ImvsSdkDefine.cs` 822–851：SAVE_BEGIN为nClientType、strSolPath[780]、nReserved[4]；SAVE_END增加uint nStatus；SAVE_PROCESS为uint nProcess及nReserved[4]。这里为托管声明证据，未用运行时布局测量冒充原生ABI证明。
- `SolutionManager.cs` 1075–1080：先Invoke OnSolutionSaveEndCallBack，再写nSolutionSaveErrorCode，最后置wait flag。`TempVmSolution.cs` 16订阅VmSolution.MSolutionManager_OnSolutionSaveEndCallBack；后者3435–3449转发显示事件后清_isSaving。**不是finally保护**：订阅者抛异常可能跳过状态更新。CVmObjectBase1193–1200吞VmException，其他异常记日志后重抛。此为静态控制流结论，尚未制造异常验证。

## 原生坐标及可复用证据

root生成的 `native-coordinates.json` 保留PE导出RVA/原始字节，非反编译结果：

| 原件 / SHA256 | 入口RVA |
|---|---|
| Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll / b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1 | IMVS_SaveSolution 0x646470；IMVS_LoadSolution 0x6466a0 |
| Applications/ModuleProxy/x64/ModuleFrame.dll / 45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b | VM_M_SaveSolutionToFile 0x13cc60；LoadSolutionFromFile 0x13cf40；CancelSave 0x13d330；CancelLoad 0x13d220 |

安装根为 `D:/Program Files/VisionMaster4.4.0/`。不能因名字相近推断两DLL间已有调用边。

同SDK还导出IMVS_PF_SaveSolution/_U（均0x641a50）、IMVS_PF_SyncSaveSolution（0x641c30）。随包Includes/iMVS-6000PlatformSDKC.h 480/495/525声明PF入口为(handle, IMVS_PF_SAVE_SOLUTION_INPUT*)，**不是托管五参数签名**，不可套PF结构去调用IMVS_SaveSolution。

既有 `analysis/full-audit/group-native-audit/save-group-native.json` 包含同hash ModuleFrame的Ghidra12.1.4证据，ID `ev_4dcd6f25f42d6501d44c004da041a9c45ea7e4493fcaa34fc10c7d5a2aa481dc`；其反编译函数为FUN_1800a36d0（save-group-native.c），不是本轮SaveSolutionToFile入口。可复用映像身份与Group邻接背景，不能作为方案writer证明。既有native-runtime-selected-linkage/public-sdk-surface-coverage也保存同SDK导出；不是函数体或运行命中证据。

## 阻塞与下一片

本轮REA会话无已配置的Ghidra/Java/IDA；root查到候选Ghidra目录为空，未安装或修引擎。历史Ghidra证据不证明本会话仍可反编译。字符串坐标不当作writer/xref；尚未获得SDK→ModuleFrame→记录名/长度/载荷写入点。SOL-ROUND2的8样本122条记录仍只闭合外层候选边界。

下一片先在可用静态引擎中分别反编译上述保存入口并取实际xref/间接调用，保留每层参数和返回码，再在独占平台实例上执行最小方案基线、节点新增、连线、变量绑定、更名，每步另存自有路径；记录原生保存入口实参、16/17/18回调、nStatus、磁盘哈希与变化记录。保存完成必须同时看到end状态及产物，不以调用返回或窗口关闭代替。探针回调不得抛异常或写共享日志。

不初始化SDK的托管隔离探针可单独验证end事件顺序：反射调用原GetSolutionSaveEndCallBackInfo，观察订阅期间旧error/wait字段及调用后字段；先审完整SolutionManager构造/静态初始化（构造订阅CVmObjectBase多个静态事件，故不能仅凭无显式SDK调用就宣称无副作用），或明确使用未初始化实例只覆盖该方法读写的字段与事件。本轮不运行这种隔离模型，也不把它作为真实SDK保存验收。

未知：native writer/reader、mdata内部结构、密码与压缩策略、失败中途文件事务、回调跨线程/超时、取消后的资源状态、同操作原机差分，均未闭合。本轮无产品实现。
