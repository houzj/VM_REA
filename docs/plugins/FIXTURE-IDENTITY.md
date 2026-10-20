# 位置修正：第一轮身份盘点

2026-10-20，只读取证，不实现算法、不构建/启动原厂或手验、不提交。

原目录D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Location/IMVSFixtureModu。Native IMVSFixtureModu.dll SHAC52E691E2F7266F8D2B2CF0BF805AD1915368C594DEB231BC009288EFB70B37A；Cs 1C7D9E4163AD97135D444660444A502F0B669F6C133FCEE3E29E4FA7DF57A3D6；Control 86227DF2FC93522CB746FB19C12BC4BCFD1FBF791E58FABA8088D0A6E457E4B0。原件12项精确hash在analysis/fixture/identity-20261020/artifacts.json。

| 身份/入口 | 原安装直接证据及缺口 |
|---|---|
| Module身份 | ToolItemInfo.name、主XML.ModuleName、Cs类型均IMVSFixtureModu；无已证固定numeric ModuleId，VmModule.ID为实例。catalog现ModuleId=null，不用共享FixtureValue代替工具身份 |
| 配置 | 原AlgorithmTab两页含Select、StandPoint/StandPointX/Y/StandAngle/ScaleX/Y、Command_FixtureModule FixtureInit；全部操作见resource-index。Control Command_FixtureModule.ClickCommandCmdExecute token06000004/RVA2088、CreateDatumPoint06000005/20ec、GetParamUnit06000006/216d、CheckInputParameters06000007/2178；具体体待恢复 |
| Cs运行/参数 | 原PE metadata IMVSFixtureModuCs.IMVSFixtureModuTool.Run token06000007/RVA211d；get_ModuResult06000004/20a4；FixtureParam.DoFixtureInit0600000c/2170。当前未找到该精确Cs旧decompile，只有iMVS-6000公共ObjectSDK同名包装，不能冒充真实本工具托管体 |
| native工厂/初始化 | AMD64 PE32+、imagebase见native-pe；CreateModule RVAaf30、DestroyModuleafc0。CMyAlgorithmManager Init8d70、DynamicIOInit8fb0、ctor8cc0/dtor8d40，均仅导出地址已确认，体未恢复 |
| native运行/配置/状态 | Process9a80、GetParama430、SetParama6e0、GetAllParamListae80、SetAllParamListad10、ResetMemberad90、DeInitade0、LoadModuleDataaa70、SaveModuleDataab60；具体函数体/ABI分支/数值顺序未知，本轮不猜位置变换算法 |
| 结果/显示 | 原XML Position Correction Info为FIXTURE，基准/运行两POINT+角度/尺度十float；Display三对象Fixtured Point/Unfixtured Point/Data Record。Cs FixtureResult属性元数据在managed-index；原getter及QueryModuleOneResult具体体尚缺。已有FIXTURE-NATIVE-FIELD-AUDIT仅字段封送证据，不补数学/单位/零尺度域 |
| 释放 | nativeDeInit/ResetMember/dtor/Destroy已地址索引；托管本工具无独立Dispose方法见metadata，继承VmModule公共销毁链需与精确父类型关系及native桥后续闭合。公共ObjectSDK FixtureModuTool.Dispose空体并非该Cs工具销毁证明 |

结果：原件/资源/完整导出表和managed方法资源身份盘点已成。缺失本Cs/Control/BAML实际体、native函数体、真实输入Count/错误/发布/重置与释放副作用。没有新runtime/设备证据；不把目前独立FixturePose.Transform或ROI使用方式当原工具规格。

证据专属analysis/fixture/identity-20261020包含artifacts/native-pe/native-exports/managed-index/resource-index。PE导出/metadata为原文件只读静态解析，没有执行原程序集。通用盘点脚本放analysis/branch-module/identity-20261020，后续复用不会修改安装目录。
