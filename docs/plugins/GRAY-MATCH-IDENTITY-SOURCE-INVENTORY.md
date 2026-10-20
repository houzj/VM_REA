# 灰度匹配VA首轮身份与调用链库存

2026-10-20。只读取证，无算法实现、无native运行/Platform Init/设备、无产品或共享框架修改。Native入口目前仅PE导出定位，不能把名字排列称实际调用顺序；下一步定向恢复本模块Process→Alg→Output及释放体。

## 身份
- 原目录：`D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSGrayMatchModuVA`。
- 原ModuleName/catalog Id：`IMVSGrayMatchModuVA`。现产品catalog ModuleId为null；未发明新插件ID。原runtime instance ID由方案分配，不是固定类型身份，本轮未创建实例。
- 主native DLL SHA256：`b7327ffa6e71345f950e78b6ad221357cdf64b8fc930a90933cd12498c3b6d4d`，PE64，CLI不存在。
- x64算法SDK候选：`D:\Program Files\VisionMaster4.4.0\MVDAlgorithmSDK\Runtime\x64\MVDGrayPatMatchV2Cpp.dll`，SHA256 `8893e01240a2face9d7d7f610e8f483101cd9c0d03465b9e045f19913976c0c9`。静态依赖名匹配不等于已经证实实际loader解析路径。
- 全部原目录文件、资源/图标、PE导入导出、SDK候选及header SHA见专属inventory.json。
- 本目标是IMVSGrayMatchModuVA，不混旧IMVSGrayMatchModu；同目录IMVSGrayMatchVAModu.dll为另一VmModuleFrame接口边界，单独hash，不能据名称直接代替主模块体。

## 配置与资源入口
- 原`IMVSGrayMatchModuVA.xml`：Input/Output/TransmitInfo完整声明及IsArray字段已保存；InputImage的InImage/Width/Height/PixelFormat、ROI和Fixture输入在此声明，读取模式/失败谓词仍需原体。
- 原Algorithm.xml有效XML ParamItem共33项（含可能由控件/框架进一步处理的项，不据计数声称Reset已实现），AlgorithmTab tabs：Tab_Basic Params, Tab_Gray Scale Template, Tab_Run Params, ResultShow。
- GroupLinkItem依赖与全部operation名单在inventory.configuration；公共组实际模板和解绑/Reset继承仍需后续逐个确认。
- 专属ParamsTabControl4GaryMatch继承ParamsTabControl4FeatureMatch；GrayMatchVAConfigPanalData选GrayMatchVADialogData/GrayMatchVADialogUI，模型文件过滤*.gmvaxml；GrayMatchVADialogData继承HPFeaturepatMatchDialogData，pyramid/angle参数setter可触OnMatchCommand。这些是已有renderer-gap反编译源，需后续按当前Control DLL方法/资源身份逐项复核。

## 运行、结果、显示与释放链边界
| 阶段 | 已证入口 | 精确缺口 |
|---|---|---|
| 托管Run | 原Cs DLL .Run token06000007→serverSDKManager/ID/SyncSelfRun；REA CIL call_edges已存 | SyncSelfRun到当前native Process的dispatcher归属/错误返回仍需桥体 |
| 原native Process | 0x180023f60 | GetInput图像/ROI/fixture/空帧/失败分支待体 |
| 原native ProcessAlg | 0x180024320 | PreproMask/PositionFix/算法Run实际顺序、实参/释放待体 |
| 原native ProcessOutput | 0x1800245d0 | 成功/失败集合Count、顺序、缺叶、首写、运行显示缓存待体 |
| Cs结果读取 | get_ModuResult token06000004→QueryModuleOneResult→HasValue/Value→SetModuleResult；属性集合见managed-chain.json | 具体结果marshal、缺输出/空结果、数组释放需桥体，不能把属性数量当算法输出数量 |
| 显示 | 原Display.xml 12个对象，name/type/feature映射逐项保存 | native producer及每帧发布/失败保留需ProcessOutput/框架显示链 |
| 释放 | 原DeInit 0x180022620；DestroyModule 0x1800261b0 | 当前未恢复具体体，不能声称所有临时结果/图像所有权已闭合 |

算法依赖明确导入MVDGrayPatMatchV2Cpp、MVDPreproMaskCpp、MVDPositionFixCpp；头文件GrayPatMatchV2Cpp声明Create/DestroyGrayPatMatchV2ToolInstance、SetInputImage/ROI、Run/GetResult。**导入和header只建立候选边界**，不证明预处理或金字塔/匹配分数算法已恢复。Cs结果包括MatchNum/Status、ModelIndex/Name、MatchRect/Point/Score、ROI、BlindPolygon、OutputMask；Display另有MatchBox/FixturedPoint等明确映射。

## 证据与失败/未知
- REA execution-free inspect_managed_artifact及inspect_managed_members已留完整JSON；metadata token/IL hash/anchors/call_edges可回查。members coverage报告partial，不能当完整C#反编译；本轮不加载或执行程序集。
- 原生PE导入导出由专属静态parser读取（源码同目录），不跑DLL、不切换共享native session。首次误用inspect_artifact(path)被输入schema拒绝；随后使用正确的目标自由managed inspector成功。
- 当前缺体不是算法已知：预处理、阈值/匹配、CC/候选过滤/排序、输出生命周期均需同hash native具体体；下一有限任务优先Process、ProcessAlg、ProcessOutput、DeInit、Init/SetParam必要入口，不能从SDK名或显示字段补算法。
- 每工具独立analysis目录；本轮不提交、不启动人工验收，不改变工具完成计数。
