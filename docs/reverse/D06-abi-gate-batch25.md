# D06.1 ABI 消费合同门槛（第二十五轮）

日期：2026-10-20。基线：`ea64cb0e573a9e658e656c3eebc951e9daf8a514`。

本轮只审查 main 工具无关边界是否具备可实施的原 SDK 导出、结构体和 ABI 消费合同。没有修改产品代码、没有访问或修改 `tool-plugins`，也没有调用安装目录中的 SDK 或设备。

## 已确认的源码证据

* `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/NativeMethods.cs` 声明 `IMVS_SaveSolution(handle,path,password,pData,nDataLen)`、`IMVS_LoadSolution(handle,path,password,checkFlag,silentFlag)` 以及版本查询入口，DLL 名为 `iMVS-6000PlatformSDK.dll`。这是托管 P/Invoke 声明，不是 native 函数体或完整 ABI 证明。
* `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ImvsSdkDefine.cs:822-851` 给出 SAVE_BEGIN/SAVE_END/SAVE_PROCESS 的托管结构字段（`strSolPath[780]`、保留字段和状态字段）。当前证据没有核对 native packing、对齐、字符编码、所有权或生命周期。
* `docs/reverse/D01-save-binding-batch9.md` 仅将 SDK 对象 `+2398h` 的 `PF_M_MsgDeal` 指针静态连到 `PlatformManager.dll`；`43dh` 消息的序列化消费者、ModuleFrame writer、reader 和落盘事务仍未知。
* `src/VisionEditor` 只包含主窗口工作区的 Win32 `user32.dll` 结构声明；未发现 main 对 VisionMaster SDK 的 P/Invoke 或导出消费入口。故在 main 增加 SDK ABI wrapper 会制造无证合同并越过插件/原生边界。

## 退出判断

`D06.1` 本轮保持 **partial / 不计实施完成**。可实施的最小合同至少还需要同版本 native 导出函数体、调用约定和结构布局的交叉核对，以及一个不触碰安装原件的受控消费实例；当前 REA native provider 未配置，原生 writer/reader 与 SDK 运行条件也未解除。托管声明、PE 坐标和现有独立 JSON 持久化不能替代这些证据。

解除条件：取得匹配版本的 native 反编译/调用约定/布局证据，并明确 main 需要消费的工具无关字段；随后由 root 设计单一合同和自审核夹具。未满足前不写 P/Invoke、Marshal 结构或“兼容”适配器。

实施增量：`+0`。D06.2 的既有 win-x64 子集启动记录不扩展为 D06.1 或 D06.3。
