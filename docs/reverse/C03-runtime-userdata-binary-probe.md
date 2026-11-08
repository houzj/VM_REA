# C03 原厂 UserData 二进制读取探针

本轮新增只读探针 `tests/NativeEvidence/C03/UserDataBinaryProbe.cs`。它通过已安装的 `iMVS-6000PlatformSDK.dll` 建立 SDK handle，调用原厂托管链对应的 `IMVS_GetAllProcessList`，再按 `IMVS_GetBinaryLength(handle,moduleId,...)` → `IMVS_GetBinaryData(handle,moduleId,...)` 读取指定模块的 `DrawImageSize`、`RoiType`、`ExternRoiType`、`DynamicInData` 和 `DynamicOutData`。输出只保留长度、SHA-256 和最多 64 字节十六进制预览。

2026-11-09 的执行结果写入 `analysis/full-audit/batch1-20261107/C03/runtime-userdata-binary-probe.json`。`IMVS_PF_CreateHandle` 和销毁句柄均成功，但当前 SDK 服务返回 `IMVS_EC_MODULE_PROCEDURE_NOT_FOUND`，进程列表为空。随后按原厂 `CVmObjectBase` 的 `ServerSetting=127.0.0.1:5556` 路径执行 `IMVS_CreateHandle`，结果相同，记录在 `runtime-userdata-binary-probe-basic.json`。对模块 0 和 1 的十个查询均返回 `IMVS_EC_MODULE_NOT_FOUND`，因此本次没有把空结果误记成 UserData 字节。

这条结果是运行时边界证据。它证明探针 ABI、结构体和清理路径可以执行，但当前连接的 SDK 服务没有可查询流程。实际 payload、模块 ID 和原厂 VM 窗口中已有流程的关联仍未确认。下一次在同一 VM 服务实际注册流程后重跑同一探针，不需要改动探针或请求截图。

早期提交中的 `IMVS_PF_GetBinaryData*` 仅作为接口契约对照，不能代表原厂 Blob UserData 的托管调用入口。本轮已将执行代码切换到 `IMVS_GetBinaryLength/GetBinaryData`，并保留原厂 256 字节 `strName` 结构布局。

证据等级为 `runtime-confirmed`，范围限于本次 SDK 返回值和错误码。它不能替代真实 payload，也不能证明原厂 VM 当前窗口状态。
