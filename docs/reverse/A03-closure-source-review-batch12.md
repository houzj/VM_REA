# 第十二批 A03.3 独立源码复审

本片只读复审，不改产品、不构建、不执行原类或 SDK、不读 tool-plugins。复用已有 REA 关联同版本反编译及第三/四批精确 CIL；没有新增 REA Evidence。审查对象是固定单元“每显示容器缩放持久化及非活动 Tab 历史”，不是缩减后的局部缩放补丁。

## 原源码支持

以下路径相对于 `analysis/decompiled`：

- `VMControls.WPF/VMControls.WPF.DataForUI/VmModuleSetDecorator.cs:17,33`：缓存默认索引 9；getter 读当前 DecorateSource.UserData[ProceZoomIndex]，无字节才回缓存，否则 int.Parse。setter 先尝试重新读取原字节，变化时先改缓存，再写 UTF8+NUL 字节；写失败弹消息，仍通知 ProceZoomIndex 与 PartentZoomIndex。显式 IZoomIndex.PartentZoomIndex 直接转此属性。没有 setter 范围校验或损坏字节一律默认9的规则。
- `VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs:23`：20 个 ticks 为 0.1 至 2.0，默认 1.0 对应索引9。OnChangeZoom 仅 Control 修饰滚轮增减且限制两端；DiagramZoomIndexChanged 非滚轮按视口中心缩放。0..19 是合法 UI 状态域，主格式拒绝越界值属于明确的数据有效性保护，不冒称原 setter 的错误分支。
- `VMControls.WPF/VMControls.WPF.Base/DiagramArea.xaml:51,56,65`：模板 DataContext 为当前 Procedure；ZoomArea 和 StatusBar 都 TwoWay 绑定 IZoomIndex.PartentZoomIndex。`ProcedureConfigControl.xaml:526,538` 的 DisplayModuleSet 决定当前显示容器，不能按顶层 Tab 单独存一份缩放。
- `VM.Core/VM.Core/UserData.cs:28,123`、`VM.Core/VM.Core/UserDataUnRedoOperation.cs`：UI 参数写入登记 IsPart=true 的部件，不创建 RootOperation；动态模块在 ModifyConfig/DeleteModule/CopyModule 根下还有整包分支。普通独立缩放不能凭该代码强行新增一个 Undo 根。
- `VM.Core/VM.Core/UserData.cs:178` 的历史恢复闭包在当前方案按 ModuleFullName 查模型，交给 `VmModule.SetUserData`；后者写 UI 参数再通知 property。缺模型不复活旧对象。`VmModule.cs:3138` 对 ProceZoomIndex 最终发模型同名通知。

原 SDK 存储边界：`VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/StringUtil.cs` 负责 UTF8+NUL；ModuleManager.SetUIParamValue 分配并复制字节，调用 IMVS_SetUIParamValue，非零错误转 VmException；GetUIParamValueByKey 返回 native 容器字节。该链没有证明原 .sol 内部编码，不能把 main JSON 字段称为原 .sol 格式。

## 原源码仍未知的语义

第四批已确认 DecoratorBase.DecorateSourcePropertyChanged 原名转发；VmModuleSetDecorator 没有把模型 ProceZoomIndex 通知额外映射成 PartentZoomIndex。直接 setter 有两个通知，历史恢复只有模型原名。尚未实际观察原绑定在同源模型 Undo 后的刷新，不得据静态链断言原厂一定刷新或一定不刷新。

原 UserData getter 非活动/Dispose 返回 null；保存重开后 native UI 容器如何恢复、Group 删除后的全事件链和原机非活动 Tab 焦点仍未做运行差分。此前日志/配置隔离失败不应被删掉或说成通过。

## main 独立适配及完整关闭条件

建议的 `ProcedureDocument.CanvasZoomIndex=9`、合法0..19、每显示容器独立字段，按 `(index+1)/10` 还原 ticks，与已证可见状态对应。main 使用自有方案格式与既有整方案历史 snapshot，不重新实现原 SDK 字节历史；缩放更新不新开根，snapshot 在后续有根操作 Undo/Redo 恢复该容器字段，是该模型中的明确适配。

非活动容器恢复后，主画布每次 RenderGraph 重读当前模型字段，而不让旧 canvasViews.scale 覆盖，是 main 的恢复策略；不声称它已复现原 PartentZoomIndex 别名通知。pan 保留会话策略，不能与原持久缩放混同。

固定实施门槛可由真实产品落地和根线自审核满足：根流程及嵌套 Group 各独立保存；切换 Tab/进入返回不串值；保存重开保持；复制/删除恢复身份正确；普通操作的 Undo/Redo 能恢复非活动容器 snapshot；同页恢复重绘不被会话缓存覆盖；旧文档默认9，非法索引明确拒绝。禁止仅证明新字段序列化就关闭整个单元。

上述行为若全部实施并自审通过，A03.3 implementation 可以记 done；原 SDK 历史格式、原别名 WPF 运行差分与人工验收单列验证未知，依据用户最新规则不混进实施分母。这不代表 A03 全包或原厂完全等价完成。此源码复审本身不增加实施完成数，最终关闭须由主智能审查实际代码与运行结果。

原件身份与精确 CIL 摘要复用 `docs/reverse/A03-zoom-history-batch3.md`、`docs/reverse/A03-binding-batch4.md`；本片材料哈希见 `analysis/full-audit/batch12-20261019/B/A03-review-hashes.json`。
