# 畸变中心数组来源、首项与读取生命周期

2026-10-19。本片补关闭此前scalar-only遗漏的实际活动域，不以缺IndexOpenSetter推断数组被禁止。

原XML组合CorrectCenterPoint/POINT含float叶CorrectCenterPointX/Y；其GetFrontCombinationTreeOperation只传predicate=true与POINT。复用Apps.XmlParser.ParamTab.VmModuleExtern：GetCombinationNode:903-916仅GetAllTrunkIO且TypeName/Visible；GetOutputTreeNode:1112-1121仅type/CanSubscribeA/Visible/非private/timestamp，无IsArray/Count。VM.Core.VmIO.CanSubscribeA:644仅字段canSubscribe；SetCombinationSourceOperation直接RelateIO/RelateIndex，SetFilterSourceOperation同样直接绑定。两树均允许数组，属于必要实施，不是裸API额外能力。

本模块新定向原体c4-source/center-input-native/output/18001e2b0.c/asm（严格原模块SHA0A6BAC886F1AA1CCC41433EF9D35FC1091B3D43354DE5794563454D8BBA03BCB）：VM_M_GetFloat(handle,name,0,&value,&localCount)，原指令1e2ef XOR R8D,R8D确认index0。localCount被丢弃，调用者输出的是status flag：成功1、031c为2、031b为3、其他0，不是数量。Process20550只允许X成功或031b后继续Y；Y成功才SetPerspectiveCenterPoint，X缺失用初值0；Y缺失不更新；任何非031b错误映射0314。Count不作为循环/匹配检查。

空payload与未绑定严格区分：复用CameraMapping B3 Frame fd90/11f5f0原体，空数据/Count0是0314，未绑定才可能031b。故空Y本轮失败0314、中心保持旧值；不能当成功missing。2个可选口现NumberArray+AcceptInteger+SupportsDeferredNumericRead；复用现公共All/typed selector运输，不改全局CanFlow。scalar Number/Integer按已有Coerce兼容。优先scope IToolNumericInputReader，随后context.ReadNumericInput(name,0)，最后已物化array首项；显式空array不能被旧combined POINT回填。裸combined/scalar/POINT-array混源只列独立兼容，原文档唯一来源仍两叶。

原Process先设置center再ProcessAlg。产品Center已移到有效Mono图捕获后、尺寸/识别前，修复原先只有识别成功后才读取的顺序遗漏。因此成功读center后即使本轮识别失败，下一轮未绑定仍使用新center；X/Y读取失败则不更新。

验证：CenterInputChecks54项通过，以实际运行导出19float核中心而非只检查Stored。覆盖数组首项、空Y0314及旧artifact/center保留、失败后未绑定恢复、X031b/Y成功X0、X/Y各非missing错误短路与映射、scope reader最高优先级、原设置center后识别失败、真实Graph POINT-array/float-array/动态index每轮更新/空source失败/解绑恢复。PointAliasChecks14项通过，实际可选树含POINT/float数组、组合落两叶、改X与两模式切换保留Y。Plugin6/6完整原fixture通过；新Host结果见final-validation。没有新增原厂运行或设备。

原第一个scalar-only退出结论以本片及最终manifest为准；本片只关闭这一已确认遗漏，不把此前数值差分改称heap或全部bit parity。


## 2026-10-19 中心数组修复后的最终实施自审

最终串行验证已全部退出0：CenterInputChecks54、PointAliasChecks14、PluginChecks原6/6、实际MainWindow HostChecks21/21。真实Tee日志为analysis/distortion-calibration/c4-source/center-{input-checks,point-alias,plugin,host}-run.log；Host包含ROI编辑→失败→恢复、DrawOverlay实际Pen/GlyphRun、样式整方案重载及关闭释放。Host构建仅已有MainWindow83 CS8603警告。最终源码/证据哈希见analysis/distortion-calibration/radial-source-order/final-validation.json。

此前中心数组遗漏已实施并验证；14D/2D原逆矩阵再乘、残差关联、全矩阵max、sqrt范数及失败传播均已按具体原体收敛。按当前工具活动UI、模型/权重、文件、ROI、状态、显示、持久化、释放范围自审，未发现已确认源码仍未实施的活动路径，可交根线作整工具完成审计。人工验收独立，不作前置。

此结论不是完整原生bit parity：47/48近零系数差异及12文件中的17/19近零差异仍保留，未证明来自heap；历史未初始化越界读取采用明确内存安全适配，不能重现所有历史prefix。19float另一9/19历史相关差异仍公开。平台.sol编码、通用WPF内部和额外native ABI属于已列依赖边界；不以此抹去本工具真实支持域数值差分。旧validation与修前数值基线保留，数值源未再改，未重复大Pixel/12File。
