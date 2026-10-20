# 标定加载typed读取接入（候选待应用）

2026-10-19。产品源保持根线冻结，未编译。共享11文件、专属消费者3文件候选位于`analysis/read-calib-file/proposals/typed-input`，两份manifest按实际base/candidate bytes计算；`prepare.py`与`prepare-consumer.py`只写候选。去换行噪音后shared为76+/27-、consumer67+/28-，git apply --check均0；这不等于编译通过。

三通道独立：float/int/text各有显式默认false端口能力、每执行callback、可选scope服务。scope服务优先，然后callback，再材料化。双float/int能力只保raw integer container；不会进入旧Coerce的scalar As<long>()。IntegerArray由Elements逐项读int32，GetFloat必须失败后消费者才调用GetInt。未启用能力的目标仍严格校验kind/cardinality；参数仍严格scalar。

## UTF8、文本读取和Count

真实VM.Core.VmIO.SetValueFromServer消费VM.PlatformSDKCS的结果结构。原`VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/COriginalModuleResult.cs:157`和`ModuleManager.cs:650`均对native string bytes调用StringUtil.UTF8GetString；同目录`StringUtil.cs:60-67`使用Encoding.UTF8与GetByteCount+1。不是用默认ANSI或.NET字符数猜容量。

原Frame12ff70→11d440→ca000不转码，按已有std::string字节数+1写required，capacity小于required返回E12且不写success Count；11d440成功后All保Count、selector成功归1。模块c100先传0x1ff，E12时再传required+1。因此候选text结果保留ErrorCode/Count/Value/RequiredBytes，Read带capacity，消费者按这两次调用，而非仅传最终string。原600字符受控模块探针的合成GetString提供者自报required不含nul，不能替代本次完整Frame的required规则；两者最终文本相同而容量trace不同。

完整数值Count失败写回更正见`READ-CALIB-FILE-FRAME-COUNT-CORRECTION.md`。scope服务透传自身原协议Count；普通NodeOutput适配按完整Frame失败Count0，不把fd90/f5f40叶Count越界写回偷渡到上层。

## 逐项输出与类型

原SetFloat f3fe0/SetInt f3ee0：已有index覆盖，否则只append一个，不填洞、不截尾。消费者每次读后用最新Count决定下一轮，只有第一项要求ret0且count>0；之后ret0即写。float失败才读int，成功Count0不触发fallback。元数据IsArray不限制原c100循环数量；实际写出的输出descriptor按本轮kind/cardinality更新。文件9项后额外2项保后7项，失败项保文件旧值，新字段稀疏写则依原append规则收紧长度。

## 正常重名门和明确unsupported

原正常动态IO改名`Apps.XmlParser.ParamTab.DynamicIO.NameA:85`同时查Root.Inputs/Outputs；另一界面`VmIODecorate.CheckName:257`查Inputs与IVmModule.IsExistName，后者`VmModule:1591`查allIos。Add侧`DynamicIOContainer.GetUniqueName:333`及`GroupParam.GetUniqueName:367`检查已有全IO名称。ReadCalibFileModu.xml已声明float CalibMatrix，因此正常UI不能另建同名int CalibMatrix。

原Frame int/float/string map确实分离。任意文件XML与额外元数据可在底层API制造同名跨类型；一个PortValue无法同时发布这几个channel。当前明确拒绝此类组合，DescribeNode/Runtime报告INVALID_DYNAMIC_DESCRIPTOR，不静默挑最后一种，也不声称全部native API等价。正常结果UI`VmIOManager.GetLeafIOResultByCallBack`按注册VmIO.TypeName与nParamType匹配；普通ReadFileConfigData.ReadData默认needAddoutput=false，不能假定未知XML输出自动成为可新增/冲突的普通UI端口。本产品动态发布任意XML输出是既有独立typed适配；其跨通道冲突保持显式unsupported。

## 待执行清单

根线先应用shared+consumer；将tests/ReadCalibFile.InputChecks的TypedTransportChecks.cs.pending、ExtraTransportChecks.cs.pending启用并接Main。单一独立artifacts串行运行Input（含真实MainWindow context.Run、配置Execute、缓存debug、LastDisplay）、既有Plugin846回放和Configuration48。旧回放按原getter首成功相位构造Integer/IntegerArray，不再把原GetInt fixture全部伪装NumberArray；值比较仍保原float32位同/int精确，不调阈值。

另外RL02文件失败Requested/已提交Path分支独立候选待审，保存重开/作用域释放仍须补自审；不把未跑测试预报为通过。Loader资源无活动Reset/ROI，不因公共Reset新增能力虚构工具入口。
