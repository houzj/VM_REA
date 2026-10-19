# 第十三批 B05.2 原全局变量模块完整实施边界

只读取证；没有实施三标量替代、启动原厂/SDK、构建或访问 tool-plugins。REA 认证实际安装 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/GlobalVariableModule/GlobalVariableModuleCs.dll`，SHA256 `f6fe474557546d0db1cad3a8c5c30ecc0706a3526cd088647fe4f49d14195e1a`。身份 Evidence `ev_324c95b3150e3a8314baeb23737975257175ae2ac6411945875f2051dc55ce86`；成员 Evidence `ev_8ae96a67cb20660730a94d37f306221b1d358a3c24d687f1e425d768cbd0add6` 的 coverage=partial 如实保留。完整原始 REA 返回在本线审计目录 rea-identity.json/rea-members.json。

使用仓库现有 `.tools/ilspycmd.exe -t GlobalVariableModuleCs.GlobalVariableModuleTool <上述原件>` 静态反编译，完整类体在 `analysis/full-audit/batch13-20261019/B/module-source/GlobalVariableModuleTool.cs`。这是 ILSpy 补充材料，不另称 REA 源码 Evidence；没有加载执行原类。

## 原用户入口到执行/存储

`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs::GlobalVariable` → `VmModuleExtern.GetGlobalVarModule`（当前 VmSolution.Modules 中 ID=13000）→ 缺模块时 Assembly.LoadFile 上述 GlobalVariableModuleCs.dll，找 IVmModule 类型，以 VmSolution.Instance 构造，再重取模块 → GlobalVariableDialog.DecorateSource/ShowDialog。不能把局部变量 AddLocalVariableModule 的 type25 路径当成全局创建。

原 GlobalVariableModuleTool : VmModule, IVarModule；object 构造器 base(param,"GlobalVariableModule") 并创建 VariableManager；InitGlobalIo 先 base 再 GlobalVarHelper.InitGlobalVarOutputIO(IsParastic?HostModule:this)。ReloadModuleData 反向 Dispose 全部 Outputs，再 InitDynamicIO，catch吞异常。恢复来自原数据源动态 IO，不是 main 局部变量列表。

`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VariableManager.cs::AddVar` 校验名字、查重复、生成GUID，打包 IMVS_VARIABLE，经 DataSource.Params IUserBytesData[AddVar] 发送原生处理，然后 AddIoByType 并设置索引。类型是 LeafIOTypeEnum 与 CombinationIOTypeEnum、可选扩展的支持集合；包含数组、图像和复合ROI，不是 int/float/string 三标量。

`GlobalVarHelper.InitGlobalVarOutputIO` 从 GetAllVar（VarCount/GlobalValueList）重建动态输出、处理过期输出并恢复索引。修改/删除、改型、分组和绑定也经 Params 原命令。VariableManager.SetRunParamBind 调用目标模块 ParamSubManager.SetModuleParamBinding，类型和能力必须匹配，设置 UserData[@参数] 并调用 ISubInfo.SetInit；旧占用需解绑。不能只在执行前给 Parameters 填一个值替代完整订阅生命周期。

原模块 IVarModule.VarValueReportEnable 使用引用计数：0→1 SetVarReportState(ID,1)，1→0 关报值。VarValueReport 接 List<VmIO>，对 int?/float?/原字符串结果数组转换，发模块事件，再对同名输出 OnVarIOChanged。UI Loaded 创建 VariableManager/GetDataAsync；值上报通过 Dispatcher 更新 Items，并暂时移除自身 ValueChanged handler，避免回写反馈。

`VM.Core/VmSolution.LoadGlobleModules` 通过 native GetAllGlobalModule 还原模块表，已有ID可ReloadModule，缺失走 CreatModuleByInfo/PreAddModuleTo/集合添加。GlobalVariableDialog.LightSave_Click 调原 LightSaveSolutionEx。main自有格式可以显式独立实现，但不能把未恢复的 native .sol 或 .gvar 编码说成已等价。

## 不实施局部替代的理由及解除条件

main 全局入口仍为 null，ModuleDescriptor没有全局动态模块身份与全类型变量服务。完整 B05.2 需要方案级全局模型、动态输出/双向订阅与改名改型清理、数组/复合支持域、执行读取与写回、事件引用计数/释放、持久恢复的统一合同。它不是只新增独占文件即可闭合的三标量UI；也不能借此在 main 复制插件配置框架。根线明确不做该 subset，本线无产品改动，实施分子增量0。

最小后续是基于现有框架数据类型/订阅合同明确全局服务及完整支持域，主智能独占修改 SolutionDocument、GraphEditorDocument、执行上下文和主入口；插件动态参数目标只消费已交付能力。原支持域未实现的类型不得隐去后把完整单元关闭。人工验证后置不解除上述尚缺的实施。
