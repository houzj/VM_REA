# 分支字符（用户称分支字符串）：第一轮身份盘点

2026-10-20，只读取证，不实现/构建/算法运行，无验收或提交。

原目录D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Logic/BranchStringCpmL。Native BranchStringCpmL.dll SHA8DE8FE3CA4931B8201F52A310939A7C580D2566FF5606A9A7C31033C97B0B9A2；Cs 3BE0C1017DAA7F7F001295A9C548636A545CA5BC84B73429E3381EC04C513A96；Control 0BF9271E5157D7CFB92C73046D9EFDB429A883F2AE5B08ABCA71F180E3491797。

| 身份/入口 | 证据与缺口 |
|---|---|
| 原Module身份 | DLL/目录/Tool类型BranchStringCpmL，但ToolItemInfo.name和主XML.ModuleName均BranchStringCpm（无L）；Cs ctor却base(param,"BranchStringCpmL")。保持原差异，不把catalog Id当原数字ID。VmModule.ID为实例，未创建就不能填固定numeric ModuleId；当前catalog ModuleId=null |
| 配置 | 原AlgorithmTab.xml ButtonSelecter TextInput与ListParamStr ModuleInfoList；GetFrontFilter/GetSelected/SetFilter与ExcuteMethod操作。Control含ListParamItemStr、ListParamStr，全部方法token/RVA在managed-index；具体list序列化/比较/命令/BAML体待取 |
| 运行 | BranchStringCpmLTool.Run token06000007/RVA213d→SyncSelfRun(ID)；native仅CreateModule196d0、DestroyModule197b0导出。真正Process/参数/比较输入虚函数与字符串编码/容量/状态均未知 |
| 结果 | get_ModuResult token06000004/RVA20c4→QueryModuleOneResult→SetModuleResult；BranchStringResult包含BranchMatch/StringMatch/ModuStatus，初值空/空/0，不等同已观察native失败分支 |
| 显示 | 原Display.xml Show Text、Data Record；主XML BRANCH_BEGIN_MODULE、ToolItemInfo IFBRANCH。具体显示mapping及完整控件在resource-index，原资源全部hash |
| 释放/复制 | 继承VmModule.Dispose/InnerDispose；native销毁工厂体未取。BranchStringTool保有ModuleInfoList复制映射方法，须后续追踪，不按通常字符串相等规则实现 |

证据在analysis/branch-string/identity-20261020：12原件artifacts、native-pe、native-exports、managed-index、resource-index。原主目录BranchStringCpmL.WorkflowAudit已读；引用源hash见analysis/branch-module/identity-20261020/managed-source-references.json。完整原生调用链、Control/BAML体及原安装与旧decompile来源闭合仍缺。本轮只完成身份，未将源码列表冒称行为已恢复。
