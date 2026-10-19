# 第十五批 B：原样本关系与身份映射实施

本片实现 `src/SolutionInspector/SemanticMap.cs`，没有改共享 Program/UiParameters，没有构建或执行SDK，没有访问tool-plugins。测试 `tests/SemanticMapChecks15` 链接真实实现，根线串行执行；本报告编写时未运行，不预报通过。

## 依据与实际行为

只读复用安装 CircleFind.sol，原样本路径和SHA见 `docs/COMPATIBLETOOL-VERSION-MIGRATION.md`。样本 VmServer.xml 明确列 ModulesInfo.ModuleBase（Index/Name/Type/DisplayName/Guid）、ProcedureBase、ProcedureInsideModules.Inside、ModuleCommonConnect 的 Front/Following。新映射保留原字符串身份、列显式成员与归属、两向连接声明来源，诊断重复ID、悬空成员、多个显式owner和不对称前后连接。声明图不能证明实际执行顺序、SDK父链或所有隐式容器。

UI文件名样本为 `_0+0+ImageSourceModule`、`_1+0+IMVSCircleFindModu`、`_10000+1+IMVSProcessControl`、`_70000+100+solution`。新类仅输出坐标解析与相同 XML Index 的候选关联，明确 authority=样本相关，未假定 native reader合同；无法匹配的solution记录保留unknown，不删掉。反斜线仅在路径识别处规范化，原路径输出保持。

原托管语义证据：`analysis/decompiled/VM.Core/VM.Core/VmModule.cs::Init(IMVS_MODULE_INFO, ModelBase)`（1754/1764附近）读取 UserData[GUID] 作为new Guid(byte[])，不存在才产生新Guid并存ToByteArray；其GUID域不能与XML Guid合并，已有样本实际不同。Position在VmModuleBaseDecorator中按UI字节结构读写；ProceZoomIndex在VmModuleSetDecorator中UTF8+NUL字符串存取。本片不重复decode这些值，root独占UiParameters消费；新类显式区分XML与UserData身份。

原父链恢复是 `VmModuleSet.LoadModules/CreatModuleByInfo/PreAddModuleTo` 根据原SDK模块信息/容器创建，不是直接读本片XML。`VmSolution.LoadGlobleModules`处理全局列表；某些模块并不属于样本普通 ProcedureInsideModules。本片仅对样本显式声明建立映射，不能把缺owner当必然坏文件。

订阅 Relation 样本确实有八段，例如 `1 . InImage . 0 . Image . 0 . 0 . All . 1`；既有报告明确native writer/reader未恢复。新类保留raw与精确分隔tokens，但状态始终 unresolved-native-relation-semantics，不把若干数字猜作index/cardinality。托管 ParamSubManager的SDK结构不证明磁盘八段同语义。

## 完整门槛及自审核结论

D03.1固定为“原XML/UI关系、身份和字段映射”，完整关闭需要各身份域关系、实际父子/隐式容器、所有订阅字段与UI键消费可说明；这次有真实解析检查器交付，但未知段仍在，不能为完成分数把原单元改成“列XML字段”。本片建议实施仍partial，明确缺的是原native订阅序列化与UI路径/隐式容器读写合同，不是待人工验收。

自审核检查：原字符串不转数字丢前导信息；Guid域不合并；未知订阅不猜类型；双向声明保持来源；不建立新产品对象图、不执行模块、不写回原格式。根线须验证15测试和原样本检查器输出后再记录实测结果。此独立映射不能外推原.sol可导入/往返等价。

本线哈希索引：`analysis/full-audit/batch15-20261019/B/evidence-hashes.json`。第一次只读GetEntry使用正斜线未匹配原ZIP反斜线返回null，改为枚举规范化路径后读成功；没有提取、执行或改原件。
