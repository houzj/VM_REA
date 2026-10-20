# D01 batch22：保存回调实施门槛复核

基线 `403d8e4af09dca4ebc9bc4f971b1b3cb41cb4c40`。本轮只复核 D01 的未闭实施入口，未修改主框架、未访问或修改 `tool-plugins`。

## 结果

本轮 **没有可按源码证据完整关闭的 D01 实施单元**，实施进度 `+0`。D01.1 的托管保存回调静态子项和 D01.2 的外层只读记录解析已经在历史台账中完成；剩余 D01.3 仍要求原生 writer/reader、字段归因和一次最小方案的保存差分，当前不能用 `GraphEditorDocument.Save` 或既有 `.vmx.json` round-trip 代替。

## 逐入口核对

- `src/VisionEditor.Core/GraphEditorDocument.cs::Save` 先调用 `PrepareSave`，再调用 `SolutionStore.Save`，仅在写入成功后更新 `document/BaseDirectory/FilePath/IsDirty/Revision` 并发出 `Changed`。这是当前 main 自有格式的原子状态提交链，不能证明原 `.sol`/SDK writer 的字段、压缩、加密或回调事务。
- 原始 `VM.PlatformSDKCS/SolutionManager.cs::OnSolutionSaveEndCallBack` 的 CIL 证据（D01 batch5）确认回调顺序为：先发外部回调，再写 `nSolutionSaveErrorCode`，最后写 `mWaitSaveSolutionFlag=true`；对应 Core 事件在 `_isSaving=false` 后继续日志。两层方法都没有 EH/finally。该顺序是源证据，尚未映射到 main 的产品保存入口，不能凭空补一个“兼容回调”。
- 原生绑定证据（D01 batch9）只追到 SDK 对象 `+2398h` 的 `PF_M_MsgDeal` 函数指针和 `PlatformManager!PF_M_MsgDeal -> 180602df0`；`43dh` 消息的实际序列化消费者、reader 和落盘字段仍未知。REA native provider 缺失条件没有解除。

## 阻塞与解除条件

精确阻塞仍为：`D01.3` 的 writer/reader 函数体及同最小方案单操作保存/重开差分缺失。需提供同版本 native writer/reader 的 REA 反编译（或已获授权的等价原始函数证据）并在自有受控实例取得节点/连接/绑定/改名各一次写入差分；不自动安装 native 工具链、不执行安装原件、不猜未知 mdata 字段。

本轮没有新增产品代码或测试，故不计实施单元；失败/缺证据沿用既有 D01 审计目录和 SHA，不覆盖历史结果。
