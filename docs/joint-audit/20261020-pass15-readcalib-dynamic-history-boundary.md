# 第十五轮独立源审：标定加载动态叶与原版撤销重做的入口边界

本项只处理标定加载 FilePath 文件选择之后的动态输出与旧 Group 订阅疑问，不把动态 IO 编辑器的机制移植到文件按钮。原件为 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll`，SHA-256 `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D`；对应原反编译 `analysis/decompiled/VM.Core/VM.Core/DynamicVmIO.cs` SHA-256 `23C0A2B880706338298B826D810FE9CD118908E46A60CC1387B7ECA5032CC9A8`、`VmModule.cs` SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`。

`DynamicVmIO.UpdateDynamciIOInfo(VmIOCollection)` 在**动态 IO 集合编辑**路径识别输入/输出所属模块；`ModifyConfig` 撤销上下文下调用 `RegisterInputIOToIoc` 或 `RegisterOutputIOToIoc` 以及 `RegisterSubscribeToIoc`，再把当前集合编入 SDK `SetModuInputIOInfo/SetModuOutputIOInfo`。`VmModule.RegisterOutputIOToIoc` 保存旧输出 IO 信息表达式；其 `IOData_Undo/Redo` 在输出侧先 `QueryModuleIOBeSubscribed`/clone，再 `RemoveDynamicOut → SetModuOutputIOInfo → InitDynamicIO(list, Outputs) → Done_IOData`。`Done_IOData` 对 Group/Procedure 先 `SetIoParams`，若有 clone 再 `RecoverSubscribe`，随后 `UpdatDynamicIO("DynamicOutputParam")` 更新配置视图。`RecoverSubscribe` 根据原保存的模块/输入/输出名取叶并给 `RelateIO` 赋值，实际缺叶、重名或类型变化的异常/失败如何处理不能靠这段正常路径下结论。

这是**显式动态叶编辑的撤销/重做路径**，并非任意参数变化的全局通知。第十三、十四轮标定加载文件控件 override→父类 `SetParamUnit/PreExecuteRun`→`ReadData(false)` 的已证路径均未出现 `DynamicVmIO.UpdateDynamciIOInfo`、`RegisterOutputIOToIoc` 或上述 `IOData_Undo/Redo` 调用。`VmModule.Outputs` 仅首次懒加载时初始化输出。因此不能把原版动态叶编辑撤销能尝试订阅恢复，外推成“重选 XML 自动迁移旧 Group 连接”。原 SDK `SetParamValue`/运行后若以其他回调进入动态叶更新仍未知；解除条件是找到该回调/订阅者的精确原函数体与触发前后 SDK 端口/订阅身份，或自有隔离原机同状态差分。

当前项目第十二轮的原名、改名/改型、Group 失败及恢复已经实测，见 [当前 schema 链](20261020-pass12-readcalib-schema.md)。它只证明当前行为，没有给原版自动迁移加一票。此项为 J04/J09 原源边界收敛，未修改产品或 `tool-plugins`，主框架实施仍 **41/96 = 42.71%**。
