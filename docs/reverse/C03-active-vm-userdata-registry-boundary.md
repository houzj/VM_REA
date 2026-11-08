# C03 活动 VM UserData 注册表边界

本轮在 VisionMaster 与 vServerApp 进程仍存在时，使用原厂 SDK `IMVS_CreateHandle` 连接 `127.0.0.1:5556`。句柄创建和销毁均返回 `IMVS_EC_OK`，但 `IMVS_GetAllProcessList` 返回 `IMVS_EC_MODULE_PROCEDURE_NOT_FOUND` 且数量为 0。

随后对模块 ID 21000–21020 和 `DrawImageSize`、`RoiType`、`ExternRoiType`、`DynamicInData`、`DynamicOutData` 五个命名二进制字段执行 `IMVS_GetBinaryLength`。105 次全部返回 `IMVS_EC_MODULE_NOT_FOUND`，没有长度、哈希或缓冲区可读取。

这证明当前沙盒中的活动进程不能作为 SDK 注册过程查询源。它保留了探针 ABI 和错误码证据，但不能证明原厂 VM 没有结果缓冲区，也不能替代桌面端注册流程。

补充验证使用 IMVS_PF_CreateHandle 与 IMVS_PF_GetAllProcessList。该路径与 IMVS_CreateHandle/IMVS_GetAllProcessList 均返回相同的 IMVS_EC_MODULE_PROCEDURE_NOT_FOUND，因此结论不依赖句柄族或过程查询函数选择。对照结果见 untime-userdata-active-handle-entrypoint-comparison-20261109.json。

探针 ABI 已按托管 SDK 校正。PF 入口为 IMVS_PF_GetAllProcessList_U，IMVS_PF_PROCESS_INFO_LIST 的源确认大小为 84020 字节，条目大小为 84 字节。校正后 PF 与普通句柄仍返回同一错误。
