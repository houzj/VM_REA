# 相机映射配套接口与验收

公共配置模型能力已由插件线实现并纳入相机映射验收：有效运行参数快照、显式编辑/复位覆盖、解绑保留最后生效值。实现限于本工作树的插件公共宿主与契约；未修改另一工作树。

| 需求 | Producer / Consumer | Payload / 生命周期 | 兼容与验收 |
|---|---|---|---|
| PointPair复合子状态 | 公共State/View → 相机映射叶子 | 原始ConfigurationElement身份、无Name子字段；局部作用域，随Session销毁 | 根线已提供CreateChild/GetNode；48个子字段，不同Data组目的IO不混淆，实际窗口验证 |
| 原厂内存顺序 | State → View/PointPair | 同父节点MoveBefore/GetChildren；不修改声明、不自创持久化PairOrder | 根线已提供；Data3删除再添须复用缓存初始顺序挑项，最终显示在Data4后；实际Button事件验证 |
| 标定路径公共叶 | 公共UI → 多个标定工具 | ChoosePath回调、源路径图标、参数订阅模式；不引入工具间程序集依赖 | 已用ToolConfigurationCalibrationPathControls；OpenFile选择CheckFileExists=false，保存独立SaveFiles |
| 组合POINT数组 | 图数据契约/订阅 → 相机映射Runtime | Scalar或Array Point及逐X/Y数量一致性；最多32对；8组加总及失败规则待补更多原厂模块对照 | 公共PointAlias通过组合字段投影Array为X/Y NumberArray；真实工具两组数组4点及原厂模块9输出通过，组合端口Scalar保留兼容；单下标Unsupported |
| 状态和清理 | ModuleExecutionScope → Runtime/Save/Display | 当前Solution/Procedure/Node限定状态；算法有效标记存在才能保存，失败清除显示和保存资格 | 使用ForProcedure；实际双流程同NodeId保存隔离检查已通过，不另建全局缓存 |
| 宿主事务 | 编辑宿主 → 配置模型 | Apply、ResetConfigurationParameters、订阅写入、运行完成回调、活动Session检查 | 已接入真实宿主；有效参数读取/复位/解绑及相机映射真实Graph执行经23项宿主检查和本轮桌面人工验收 |

证据来源：原厂PointPair.cs/AddButton_PointPair.cs、AlgorithmTab、Display、CameraMapCpp.h及受控ModuleProcess/SDK数值输出。源码确认、独立托管实现与原生平台未知分别保留在机器证据，不以测试通过宣称等价。

### 缺失组合force操作已确认原厂忽略

原始AlgorithmTab含16处SetCombinationForcePropertyOperation，但安装类型表没有此类。官方parser工厂在AlgorithmTabRoot的IgnoreUnknown=true加载阶段保持Operation=null，真实相机XML逐个调用触发无异常。工具以专用无操作rule复现，不需要发明公共必填投影。公共未知操作的通用策略仍由根线决定，见CAMERA-MAPPING-PARSER-INDEX.md。

解析器补证：缺失组合force操作已确认原厂IgnoreUnknown加载时保留null并忽略，不再列为待发明的必填投影。专用产品规则与真实WPF已验证。索引候选/手工路径/托管原生载荷及仍未知的原生取元素边界见 [解析器与索引](CAMERA-MAPPING-PARSER-INDEX.md)，该更新优先于本文早期未知记录。

文件/缓存补证及修复见 [标定文件与缓存](CAMERA-MAPPING-FILES.md)：源容量不缩小、显式保存当前模型重算、IO失败保存资格均已官方探针与产品验证；2026-10-19相机专属检查扩展为110项。

## 生效参数读回契约（2026-10-18，已接入）

Producer：公共运行/参数编辑宿主；Consumer：相机映射配置参数读回、取消订阅与显式标定导出。已提供接口 `ToolEditorContext.ReadEffectiveParameters: Func<IReadOnlyDictionary<string,JsonElement>?>`，返回当前模块实际应用参数的只读Clone，未应用/作用域失效返回null。工具不自建另一份UI参数缓存。

优先级按真实应用顺序：最新显式SetParam/Reset覆盖此前值；运行解析并应用的订阅覆盖对应参数；求解失败但此前已设置的参数仍有效；输入/订阅在应用前失败不能伪造新有效值。取消订阅后原厂ValueBase.IsSubA先清SubData绑定，再IsGetFromModule=true/GetParamUnit，因此应保留最后实际模块参数，不回退文档seed。相机映射ProcessCalibMatrix使用SDK当前HomoFreedom/WeightFun/WeightFactor，不能单以旧seed重算。原厂完整平台的异常边界仍单列未知。

生命周期限定Solution/Procedure/Node及执行会话，跨流程同NodeId不得泄露；Reset成功应更新读回，失败不得清旧值。真实Graph已验证：订阅WeightFactor运行、更改seed不冒充应用、取消订阅读回当前值、显式编辑/Reset覆盖旧值。公共实现包含运行入口记录、显式变更与Seed差分合并，以及解绑在一个文档编辑事务中保留当前有效值。相机专属配置/运行检查现为110项；既有118组原厂SDK差分及23项真实宿主检查均通过；另有本轮桌面人工验收通过。相机SDK数值容差/原生索引边界见专项说明；这些检查不等价于原厂桌面及native索引全覆盖。

## 复位与点对失败顺序补充

原厂ResetRunParamBind对ISubInfo切回编辑模式再ResetParam/GetItemsValueUnit。工具已清WeightFactor、RefreshFileEnable、CalibPathName三种原厂ISubInfo参数的持久模式及控件Binding缓存，保留InputWay用户模式；宿主仍负责单事务删除参数订阅。配置拒绝/取消时不提前修改私有状态。PointPair按源码顺序清Object组合、Target组合、ObjectX、TargetX、ObjectY、TargetY。托管服务某叶失败时停止并保留显示标记属于显式独立安全规则，原生逐叶失败后的整体状态未验证。
