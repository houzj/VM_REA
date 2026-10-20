# 分支模块：第一轮身份盘点

2026-10-20，只读取证，不实现/构建/运行算法，无验收或提交。

原目录D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Logic/BranchModule_STD。Native BranchModule_STD.dll SHA47E5AE68E1744C0A899D70A7F98DB1723C1C4B921616B462D3B12D94A1F9AC73；Cs EFCBFE6CB5ED450DDB2ED9B26C7E71F434393F90B10F1A2101098B58B258E2E7；Control 2B7182273CFCAE26F608101C621C100AC1D5B2410E08105B0088E609A1E5CC45。

| 身份/入口 | 证据与缺口 |
|---|---|
| 原Module身份 | ToolItemInfo.name/XML ModuleName/Cs构造base(param,"BranchModule_STD")一致。原实例数值ID来自VmModule.ID，并非目录固定数字；本轮未实例化，不伪造numeric ModuleId。当前catalog ModuleId=null，不能宣称插件已注册 |
| 配置 | 原AlgorithmTab.xml，ConditionInput订阅inputint、IndexUserSet模式和ListParam(ModuleInfoList)；4种operation。VmModule.cs:1809设置Params.XmlPath为ModuleDir/strModuleNameAlgorithmTab.xml。Control含Apps.XmlParser.ParamTab.ListParam（Str2List token06000009/RVA2134、List2Str0600000a/21fc、ResetListItemValue0600000b/22e8）。Control具体体/BAML尚未恢复 |
| 运行 | 原BranchModule_STDTool.Run token06000007/RVA213d→mModuleManager.SyncSelfRun(ID)，读取已有decompiled source。native只导出CreateModule RVA94c0、DestroyModule95a0；Process/ProcessAlg/参数/输入虚槽未定位，不能从算法名猜实现 |
| 结果 | get_ModuResult token06000004/RVA20c4→QueryModuleOneResult(ID)→SetModuleResult。BranchResult为outputint/ModuStatus/BranchMatch；模块XML还声明ModuRunTime。非原生真实输出观察 |
| 显示 | 原Display.xml单Data Record，mapping保留原资源。XML声明DisplayType BRANCH_BEGIN_MODULE，ToolItemInfo IFBRANCH，属于两层标记，不擅自视为一个枚举 |
| 释放/复制 | 继承VmModule.Dispose→InnerDispose→清引用/CommonDispose等，具体源已读；native DestroyModule已定位但体未取。BranchTool自身ModuleOperate/Paste/PasteFinish有ModuleInfoList重映射源，应后续纳入，不能只做路由算法 |

原目录12文件逐项SHA见analysis/branch-module/identity-20261020/artifacts.json；native-pe.json核PE32+/AMD64；native-exports.json从原PE被动解析；managed-index.json含Cs/Control全部方法token/RVA和resource名称；resource-index.json列全部控件/操作/显示。已复用主目录BranchModule_STD.WorkflowAudit源码，source引用hash在managed-source-references.json。本轮未替换旧反编译源；它与当前安装文件的完整构建来源关系尚未单独复核，直接PE索引身份独立可信。

结果：文件/工厂/托管入口/资源可复核；缺失native虚函数体、Control行为/BAML、真实参数错误/路由/结果生命周期。未称完整逆向。最初metadata GetToken重载歧义失败一次，改按TypeDef所属MethodDef顺序递增表行生成token后成功；脚本与结果均保留，不运行原程序集。
