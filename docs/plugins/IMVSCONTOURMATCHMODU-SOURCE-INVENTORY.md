# 轮廓匹配：第一轮独立原件与入口盘点

2026-10-20；只读静态证据，无目标执行、原厂运行、托管构建、产品/共享框架修改或人工验收。三个匹配器分别建档，下面的UI继承不等同算法相同。

## 身份与配置

- 原目录：`D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Location\IMVSContourMatchModu`。
- 原符号ModuleName / ctor(object)传入模块标识：`IMVSContourMatchModu`；本工具Cs IL可核。实例数值ID经VmModule.ID读取，是运行实例身份，本轮未执行，未将程序集Guid或GuidTemp冒固定ModuleId。
- 当前产品：tool-catalog.json对应`IMVSContourMatchModu`条目ModuleId=null；无对应已注册plugin/runtime/provider。不是实现完成工具。
- 原模板入口：`PyramidFeaturepatMatchConfigPanalData`（节点名以xml-members.json实际索引为准）；Tab为 Tab_Basic Params, Tab_Feature Template, Tab_Run Params, ResultShow。
- 原配置/算法默认/Tab/Display/Cs.xml/ToolItemInfo.xml六XML的全部成员索引见xml-members.json；Librarys.Config的依赖内容保持原件。

| 原件 | SHA256 |
|---|---|
| IMVSContourMatchModu.dll | 56156A8707FA92CE758485772BEB18A5289CB97599533D86897A9391817FCF64 |
| IMVSContourMatchModu.xml | 7FC0E79EE7BC3BBF7B8A26C570E42403181796F33140A63B2E72A544035C05DF |
| IMVSContourMatchModuAlgorithm.xml | 740C75CB01A3528A190F0C6946483ADA974DD0F6A49D163755B429D19F6737F3 |
| IMVSContourMatchModuAlgorithmTab.xml | 8456331100019E9280C81DE9C1F70735B3A5020548929E382A0F146D5741DAFD |
| IMVSContourMatchModuControl.dll | 67F4FA218C54EC9203DE8E38D21AF14A0F55C9054A079791026F84F430FAD461 |
| IMVSContourMatchModuCs.dll | AEE906888A4C9CF6DF5917828665DEE83B1EA06C02F3A02E214CD9787FB96B80 |
| IMVSContourMatchModuCs.xml | 88F58A4253B2B11886627C2B29B60C6B63D89583A564500529287EB458998607 |
| IMVSContourMatchModuDisplay.xml | D96C658745A7A3ADC8C9862B55EFF08B2D6535B69B949B9C1BFF065506AD73A6 |
| Librarys.Config | 23BC117A2F75DED133D7293CE0ED0B61A6D04F6ACB3EEBF931683C710A8D5A3A |
| ToolItemInfo.xml | E6642FB7A0393940FD176E2EF3BA2EB9042BD4DD22711B30A5FB76F8CF466DE4 |

算法SDK导入是独立身份：
- `D:\Program Files\VisionMaster4.4.0\MVDAlgorithmSDK\Runtime\x64\MVDContourPatMatchCpp.dll` SHA256 `b2ee17f6d7b38e67e055c68a63f4e5ecf9149b4977bb9891f19ffbe61bfd6fdf`；仅PE导入/导出，不代表其方法体已还原。

## 源码已确认的托管运行/结果/模板链

- 本工具ctor(object)调用VmModule::.ctor(object, 模块字符串)，再由ID构造本工具Param。
- 本工具Run→ServerSDKManager.mModuleManager.SyncSelfRun(ID,true)。get_ModuResult每次new本工具Result→QueryModuleOneResult(ID,true)，HasValue时SetModuleResult(...,ModuResultMemType)，再返回本工具Result。这是已导出IL的实际调用链，不是猜测原生Process细节。
- Param构造中的ParamItem/Read/Write与具体key、Result构造全部属性/字段保存在本工具Cs.il及managed-method-index.json；本轮未逐参数语义闭合。

| 具体方法体 | IL文件/行 | RVA |
|---|---|---|
| VM.ModuleControl.FeaturepatMatchDialogDataBase Apps.XmlParser.ParamTab.PyramidFeaturepatMatchConfigPanalData::GetModelData() | IMVSContourMatchModuControl.il:135 | 0x2068 |
| IMVSContourMatchModuCs.ContourMatchResult IMVSContourMatchModuCs.IMVSContourMatchModuTool::get_ModuResult() | IMVSContourMatchModuCs.il:116 | 0x20A4 |
| System.Void IMVSContourMatchModuCs.IMVSContourMatchModuTool::Run() | IMVSContourMatchModuCs.il:172 | 0x211D |
| System.Byte[] IMVSContourMatchModuCs.IMVSContourMatchModuTool::GetModel() | IMVSContourMatchModuCs.il:193 | 0x2138 |
| System.Void IMVSContourMatchModuCs.IMVSContourMatchModuTool::ImportModel(System.Object) | IMVSContourMatchModuCs.il:342 | 0x2300 |
| System.Void IMVSContourMatchModuCs.IMVSContourMatchModuTool::ImportModelData(System.String[]) | IMVSContourMatchModuCs.il:551 | 0x2574 |

- HP和Fast本工具ClearModelData实际发ServerSDKAPI.IMVS_SetParamValue(handle,ID,"ClearCandidate","")并检查返回值；不是释放整个tool/session。Contour Cs未声明同名方法，不能用HP方法冒充本工具API。
- Contour.Control的Pyramid配置/对话数据/对话UI与Fast.Control对应类型实际继承HP.Control；HP配置再继承FeatureMatchCommonControl，HP对话Data继承VM.ModuleControl.FeaturepatMatchDialogDataBase，UI继承FeatureMatchCommonControl的UIBase。具体派生类型独立，不能将HP训练/运行算法移作其它两个。
- HPFeaturepatMatchDialogUI.Owner_Closing IL43CC：DataContext非null时移除MatchCommand handler；这只是一个已确认解绑，不能声称所有ROI/窗口释放已闭合。Contour/Fast能继承哪些路径及各自覆盖需要按类型继续核。

## 原生运行、结果、保存和释放入口（导出定位；方法体仍缺）

以下RVA来自本工具原件PE导出表；是精确下一步取体地址，尚未证明这些符号之间的真实call顺序。

| 导出符号 | RVA |
|---|---|---|
| ??0CIMVSContourMatchModu@@QEAA@XZ | 0x21410 |
| ??1CIMVSContourMatchModu@@UEAA@XZ | 0x21660 |
| ??_7CIMVSContourMatchModu@@6B@ | 0x33d18 |
| ?ClearMatchResult@CIMVSContourMatchModu@@AEAAXXZ | 0x21890 |
| ?CreateToolInstance@CIMVSContourMatchModu@@AEAAHXZ | 0x21950 |
| ?DeInit@CIMVSContourMatchModu@@AEAAXXZ | 0x21a70 |
| ?DeleteModuBuf@CIMVSContourMatchModu@@AEAAXXZ | 0x21b50 |
| ?GetParam@CIMVSContourMatchModu@@UEAAHPEBDPEADHPEAH@Z | 0x21be0 |
| ?GetResult@CIMVSContourMatchModu@@AEAAHPEAUFEATURE_MATCH_MODU_OUTPUT@@@Z | 0x22840 |
| ?Init@CIMVSContourMatchModu@@QEAAHXZ | 0x22ca0 |
| ?LimitJudge@CIMVSContourMatchModu@@AEAAHPEAUFEATURE_MATCH_MODU_OUTPUT@@@Z | 0x22ee0 |
| ?LoadModuleData@CIMVSContourMatchModu@@UEAAHPEBULOAD_MODULE_DATA_PARAM@@@Z | 0x230d0 |
| ?Process@CIMVSContourMatchModu@@UEAAHPEAX0PEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@@Z | 0x235e0 |
| ?ProcessAlg@CIMVSContourMatchModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUFEATURE_MATCH_MODU_OUTPUT@@@Z | 0x239b0 |
| ?ProcessOutput@CIMVSContourMatchModu@@AEAAHPEAU_MVDSDK_BASE_MODU_INPUT@CVmAlgModuleBase@@PEAUFEATURE_MATCH_MODU_OUTPUT@@QEBX@Z | 0x23d00 |
| ?ResetMember@CIMVSContourMatchModu@@AEAAXXZ | 0x24b50 |
| ?SaveModuleData@CIMVSContourMatchModu@@UEAAHPEAUSAVE_MODULE_DATA_PARAM@@@Z | 0x24c00 |
| ?SetInputImage@CIMVSContourMatchModu@@AEAAHPEAVIMvdImage@VisionDesigner@@@Z | 0x24f90 |
| ?SetMaskImage@CIMVSContourMatchModu@@AEAAHPEAVIMvdImage@VisionDesigner@@_N@Z | 0x24fc0 |
| ?SetMultiThread@CIMVSContourMatchModu@@AEAAHXZ | 0x25010 |
| ?SetParam@CIMVSContourMatchModu@@UEAAHPEBD0H@Z | 0x251b0 |
| ?SetROI@CIMVSContourMatchModu@@AEAAHPEAVIMvdShape@VisionDesigner@@@Z | 0x26760 |
| ?SetRunParam@CIMVSContourMatchModu@@AEAAHPEBD0@Z | 0x26790 |
| ?UpdateMatchResult@CIMVSContourMatchModu@@AEAAHXZ | 0x267c0 |
| CreateModule | 0x272d0 |
| DestroyModule | 0x273b0 |

## 显示资源与消费者边界

原Display对象：InputImage, Feature Match Contour Points, Feature Match Result, ROI, Feature Match Points, Fixtured Point, Unfixtured Point, Show Text, Data Record, Data Record, Result List, ExternROI, OutputMask。
具体Object.Type/Features/Source/状态/数量等保留原XML索引。原生ProcessOutput/UpdateMatchResult/GetResult生成这些字段的分支尚未取体，公共显示消费者也未按新匹配器完成适配；不能用现标定显示通过推定支持轮廓轮廓点/模板/结果集合。Fast首图对象实际叫InImage，另两者叫InputImage，此差异保留。

## 精确缺口及后续最短取证

1. 本工具原生CreateModule→构造/Init→CreateToolInstance及Process→ProcessAlg→GetResult→ProcessOutput实际方法体、callee、catch/成功失败清理门。导出不是源码体；当前算法/运行未闭合。
2. 本工具SetParam/SetRunParam→各参数/输入/模型创建/匹配/排序/阈值/坐标与掩膜SDK消费者；不能仅凭ParamItem范围实现算法。
3. 本工具LoadModuleData/SaveModuleData→model/solution用户数据、模板编辑/导入导出及训练生命周期；GetModel/ImportModel IL已取得，具体二进制格式/原生SDK管理仍须逐链。
4. 本工具DestroyModule/DeInit/DeleteModuBuf/ClearMatchResult与SDK所有者、结果内存、模板及ROI/window关闭订阅的真正释放链；已导出OwnClosing不等于闭合所有释放。
5. UI BAML/共享FeatureMatchCommonControl、VM.ModuleControl实际模板生命周期所需源体仍待定向补；普通原件共享哈希待消费到具体体时锁定，当前不以同名依赖替代证据。

## 取证状态/失败

- REA evidence `ev_aaf83f660273889090b7f3f81ff378bd66316ab50333f1fbc6f079c0d91fd138`，Cs SHA与原件一致；managed provider coverage=partial，因此不宣称完整托管还原。raw IL为installed dotnet-ildasm0.12.2.0只读导出，可逐方法核。
- 最初ILDASM要求已缺的.NET3；仅对子进程设置DOTNET_ROLL_FORWARD=Major后被动导出成功，未安装/升级运行时。Python商店别名不可用，改用已配置Codex bundled Python成功，无环境修改。
- 首次尝试把完整REA图写入单条Windows命令超过CreateProcess长度限制，未写成功；改存精确REA摘要+完整本地IL，完整metadata不冒称已留盘。
- 分工具原件SHA/IL/方法索引/PE导出与SDK身份均已生成；源码恢复/实施/差分/宿主/人工验收均未完成。

## 已有Control源码复用补记

发现原只读目录`D:\MyWork\REA逆向测试\analysis\full-audit\renderer-gap-projects\IMVSContourMatchModuControl`，已索引16个CS/XAML的路径与SHA到reusable-managed-source-index.json。因此前述第5项应收窄：不是所有Control源码不存在，而是尚未按本轮具体caller核完原件绑定、共享base消费者和生命周期。优先复用这些文件并对照当前IL，避免再次全量反编译。Contour当前IL两处CreatePatE明确动态调用CreateDipolePyramid；不能因其继承HP DialogUI就用HP创建算法代替。Fast/HP的具体动态创建分派继续独立核。
