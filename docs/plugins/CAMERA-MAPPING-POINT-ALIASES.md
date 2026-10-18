# 相机映射：POINT 组合与分量别名证据

此文供公共框架线落合同。**源码确认、托管兼容行为和未验证原生行为分开**；不能把现有独立 Point 端口类型支持等同于本协议已经复现。

## 字段及调用链

原始安装 `Calibration/IMVSCameraMapModu/IMVSCameraMapModu.xml` 的第1组 `Combination TargetPointInput1(POINT)` 包含 `TargetPointX1/TargetPointY1` 两个 float Filter；Object组相同。AlgorithmTab 第124行起 `PointPair Data1` 的 `ObjectPoint/TargetPoint` 子元素调用 `GetFrontCombinationTreeOperation(POINT)`、`SetCombinationSourceOperation(ObjectPointInput1/TargetPointInput1)`、`GetSelectedCombinationOperation`。逐轴子元素调用 `GetFrontFilterTreeOperation(float)`、`SetFilterSourceOperation(ObjectPointX1/...Y1)`、`GetSelectedFilterOperation`。

原厂反编译源基目录：`D:/MyWork/REA逆向测试/analysis/decompiled`。

| 源码位置 | 确认行为 |
|---|---|
| `Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/SetCombinationSourceOperation.cs` | 通过 `VmModule.GetIO(组合名)` 查订阅方；解析来源路径/下标后设置 `iO.RelateIO`，再设置 `RelateIndex`；清空则设 RelateIO=null。这里不是创建另一份X/Y数值 |
| 同目录 `SetFilterSourceOperation.cs` | 单独查 `GetIO(轴名)`，设置该叶 RelateIO/RelateIndex；来源预检失败会回写ButtonSelecter.OldCurValue；这不证明底层写入异常可回滚 |
| `VM.Core/VM.Core/VmModule.cs:1823` Subscribe | 两边都是叶则调用原生 SubscribeResult；组合两边 Childs.Count相同则**按顺序逐项**执行 `source.Childs[i].RelateIO=target.Childs[i]`；数目不同抛 SubscriptionsNotAligned |
| `VM.Core/VM.Core/VmIO.cs:121` RelateIO getter | 组合从各子叶重建：每叶必须有来源且来源有Parent；所有来源属于同一个Parent、来源叶不重复且数量相等、类型适配，才返回该共同组合Parent；否则返回null |
| 同文件 SubscriptionInfo / GetSubscriptionInfo（约795行） | 可重建共同组合时显示共同来源及下标；不能重建时 GetSelectedCombinationOperation 使用 GetSubscriptionInfo，按叶生成 `(X来源,Y来源)` 组合文本，并把组合选择框置ReadOnly=true |
| 同文件 RelateIndex / SetRelateIndex（约268/828行） | 组合getter取首个非null子下标；setter递归给全部子叶设置。对不一致子下标的完整原厂运行语义仍未验证 |
| `PointPair.cs:145` IsPointModeA | 切到按点重新 InniterBusiness 两个组合子选择器；切到坐标重新初始化X/Y（以及存在的Angle）子选择器。不是另存一套坐标 |

## 失败原子性不能擅自推断

`VmIO.RelateIO` setter（约157–205行）先登记Undo，再调用旧来源UnSubscribe；无共同来源时逐项清空子绑定；设置新 `_relateIO` 后 Subscribe，异常分支将 `_relateIO=null` 再抛出。`VmModule.Subscribe` 对组合逐叶处理，没有在上述托管代码中看到整组旧值恢复；`UnSubscribe` 则捕获并记日志。

因此本专项**没有证据证明原厂整组失败是原子回滚**。来源路径预检失败、ButtonSelecter恢复显示、原生第2叶订阅失败是三种不同边界。公共框架若采用原子文档事务，可以作为明确的独立安全实现，不能标记为原厂确认。低层失败后实际订阅状态需要原厂运行探针才能闭合。

## 声明中已有的名称差异

主声明XML第3–8组名称是 `TargetInputN/ObjectInputN`；AlgorithmTab操作参数和公开SDK CameraMapParam却为 `TargetPointInputN/ObjectPointInputN`。第1–2组一致。当前独立实现使用UI/SDK名字支持交互，原始XML未改；尚未证明原厂平台是否在动态IO阶段消除该差异，不能静默称两者完全相同。

## 公共接口建议与当前边界

Producer 为统一图数据/订阅框架；Consumer 为相机映射及其余含Combination的工具。Payload 应显式携带组合身份、源/目标有序字段映射、下标身份、每个叶的来源和只读派生组合展示。Session关闭只释放编辑器，绑定由流程文档持有；执行、保存、撤销、流程切换都使用同一份绑定。

验收至少覆盖：绑定完整POINT后X/Y读回各自叶来源；单改X保留Y；来源来自不同组合时显示派生元组而非虚构单一POINT；完整重新绑定替换两轴；清空组合清两轴；Index.All/单元素/索引引用；不同流程同NodeId隔离；第一/第二叶拒绝时的可观测状态。原厂失败边界没有运行证据的项应保留未知。

当前产品实现 `IToolPointInputAliasProvider`，声明16组（8个示教点与8个运行点）UI/SDK原名组合到X/Y映射。主宿主消费公共 `PointAliasConfigurationSubscriptions`；生产PointPair在模式变化时重新读取可见子选择器。真实工具检查已覆盖完整POINT拆成叶、按坐标读回、改X保留Y、组合只读元组、Undo恢复共同POINT，以及数组POINT两组展开成4点经GraphExecutor运行。全部9项数组运行输出与受控官方模块比较通过0.003固定阈值。

Runtime保留旧独立组合端口和逐轴覆盖作为兼容路径；正常公共别名路径只存X/Y叶绑定，不把两个来源当作独立可冲突的权威。批事务原子性是托管文档安全规则，仍不是原厂逐叶失败回滚证明。单下标/索引引用继续明确Unsupported。原厂缺失组合force操作忽略已另行补证；原厂桌面和完整原生订阅平台未验证。

解析器补证：缺失组合force操作已确认原厂IgnoreUnknown加载时保留null并忽略，不再列为待发明的必填投影。专用产品规则与真实WPF已验证。索引候选/手工路径/托管原生载荷及仍未知的原生取元素边界见 [解析器与索引](CAMERA-MAPPING-PARSER-INDEX.md)，该更新优先于本文早期未知记录。
