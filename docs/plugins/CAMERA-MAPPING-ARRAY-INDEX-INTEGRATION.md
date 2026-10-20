> 2026-10-19 Count层次更正：普通NodeOutput完整VM_M_GetFloat在ca720失败时不把叶Count写回，11f5f0返回Count0；旧“正越界保底层Count”结论撤回。最小缺陷修复与真实Graph回归见READ-CALIB-FILE-FRAME-COUNT-CORRECTION.md。成功All/count1规则不变。

# 相机映射 ArrayIndex 实际接入方案

最终实施已由根线应用全部候选并通过真实Index257、共享58/122，24例216字段固定0.003保持无差异。下面保留当时方案/候选准备过程；当前权威退出结论与最终SHA见`CAMERA-MAPPING-IMPLEMENTATION-EXIT.md`和`analysis/camera-mapping/array-index-regression/final-verification-complete.json`，不再按“尚未应用/未构建”理解当前进度。

2026-10-19，根线已授权在`analysis/camera-mapping/proposals/array-index`制作21文件完整共享候选patch，尚未应用共享生产、尚未构建或运行。23个候选/测试C#文件仅语法检查零错误，git apply --check退出0；详见该目录README/manifest。**下文早期小数直接可达的推断由CAMERA-MAPPING-ARRAY-INDEX-LITERAL-GATE.md纠正：GetBindIndex先Contains('.')路径门，未找到提示后两叶All；`-`常量才正常取0。** 人工验收不计完成门。整体工具数量由根线重审，本线不自行增加。

## 已有入口与实际阻断

原ButtonSelecter的IndexOpenSetter使用GetFrontFilterTreeOperation，filter=int；IndexClickCommand插入选中VmIO的ModulePathName，null清空方括号。SetCombinationSourceOperation先绑定两叶来源，再递归RelateIndex；索引为空是All、数值是VmConstantIO.Name原串、有点号是IO路径。原Apps.XmlParser.MathHelper.IsNumber的正则允许负号与小数；Manager5e8db0最终atoi原串，不可把当前At(int>=0)当原厂完整限制。原点索引报告和本轮Count上层窗继续有效。

当前ToolConfigurationSubscriptionSelector已有OpenIndex、SetIndex、整数树选择和括号内键入；无需新做第二套索引控件。阻断在：

- WorkflowConfigurationSubscriptionService：IndexOnly枚举直接Unsupported，Bind/BindMany拒绝非All；WorkflowValueReference没有索引字段。
- PointAliasConfigurationSubscriptions.Component拒绝非All；Parent虽然检查X/Y索引相同，但组合读回未呈现索引后缀；组合IndexOnly也被拒绝。
- GraphExecutor在Runtime前Resolve全部订阅，任何失败直接SubscriptionUnavailable。仅提前切成单元素数组会丢失原失败Count与动态索引读取失败回退0，不能复用CM-02/03正确的两遍读取。

完整GetFloat成功Count门：Frame11f91d取indexkind；11f9d6确认读取成功；11f9da比较kind1（All），其他kind在11f9e3覆写Count=1，11fd62…11fd6a写回。fd90负值错误Count=0；正越界在错误前写原数组Count；这些不能被统一“索引错误→空数组”替代。

## 建议最小类型化合同

1. 在现WorkflowValueReference增加**可选**ArrayIndex属性。缺省/null为All。使用共享不可变WorkflowArrayIndex表示互斥的LiteralText或IndexSource（类型化WorkflowValueReference）。不写私有caption字符串，不用隐藏输入端口偷运索引。
2. LiteralText保留原输入数字文本以便显示/保存。保留现At(int)调用兼容，补确切四regex的源数字文本构造；非负限制不能无声保留。正常UI无dot的`-`取0、`-1`取-1；dot数字先按GetBindIndex路径门处理，未找到提示后来源保持/两叶All。独立DTO/primitive小数原串可按底层atoi取整数前缀（如1.5→1），不可外推UI可达。int32溢出无现有原证，明确拒绝为托管边界；不猜C overflow。
3. PortDescriptor增加默认false的SupportsDeferredNumericRead能力，仅相机映射32个float坐标叶启用。未启用消费者继续拒绝该索引入口，不把所有工具悄悄改成延迟读取。
4. ModuleExecutionContext增加可选每次执行的ReadNumericInput(name,index)委托及Framework层Primitive结果(uint ErrorCode,int Count,double Value)。该委托由GraphExecutor捕获当前节点raw输入和本次workflow.Resolve，生命周期只到ExecuteAsync结束；不序列化、不存全局scope服务。现Contracts.IToolNumericInputReader优先级保持最高，委托是Graph接入适配，避免Framework反向依赖Contracts。
5. GraphExecutor对启用能力的数字输入保留raw数组、不提前套索引，调用时再按绑定选择：All使用当前遍历index；literal固定其解析整数；reference从int来源读取首值，读取失败按原124d40回退0。成功indexed返回Count1；错误保留上述Count差异。相机映射原两遍流程不变，只把Framework结果转换成既有ToolNumericInputReadResult。

普通执行调度、Group失败传播不在此重写。动态index来源必须是现root-flow桥已支持的前驱int输出；图执行策略使整节点未执行时，不声称能观察native fallback。针对“来源节点Success但可选int输出缺失”的合法执行轮，必须实际走reader并回退0，不能在Runtime前报SubscriptionUnavailable。

## 精确共享文件与变更

| 文件 | 必须做的有限变更 | 所有权/兼容 |
|---|---|---|
| src/VisionFramework/WorkflowSubscriptions.cs | 可选类型化ArrayIndex DTO；验证literal/reference互斥、int来源、禁止递归索引；给出所有来源依赖与统一递归重映射辅助方法 | Framework数据合同，根线记录跨项目请求；旧JSON/现三参构造保持All |
| src/VisionFramework/Contracts.cs | 第13行PortDescriptor增加能力；第61行ModuleExecutionContext增加每轮委托及primitive结果 | 默认false/null，不改变其他模块 |
| src/VisionFramework/WorkflowRunContext.cs、可新增WorkflowNumericInputRead.cs | 当前Resolve第19行保持普通引用逻辑；带selector的引用必须显式走numeric helper或明确拒绝，不能静默忽略；helper负责有界解析/raw读/动态首值失败0/Count | Graph adapter明确独立实现，不宣称原内存ABI |
| src/VisionFramework/GraphExecutor.cs | 对声明能力的indexed numeric订阅保留raw值并给Context委托；真实可执行缺失index数据由reader处理 | 不提前slice；不改其他消费者的失败路径 |
| src/VisionEditor/ToolExecutionScope.cs、src/VisionFramework/NodeDebugExecutor.cs | 配置自运行与单节点调试原本会将订阅材料化为isolated WorkflowInput从而丢selector；候选以每轮numeric helper加Graph末尾可选逐节点callback保留索引，动态失败不提前throw，配置入口捕获本轮上游记录快照 | 只本轮闭包、不新增全局服务；实际MainWindow Execute按钮回归已准备待运行 |
| src/VisionFramework/GraphValidator.cs | 同时验证数据和index来源执行顺序/循环；缺失来源给明确错误 | 不能只验数据来源而忽略index来源 |
| src/VisionFramework/WorkflowConfiguration.cs | Snapshot/RemapNodes/删除节点清理递归保留/重映射IndexSource | 持久化及复制兼容，不能留下旧NodeId |
| src/VisionEditor.Core/GraphEditorDocument.cs | fragment粘贴和source节点删除/调用签名变更检查使用统一递归引用辅助方法 | 现Undo命令模型保持，不增第二套历史 |
| src/VisionEditor.Core/GraphEditorDocument.Groups.cs、GroupInterfacePropagation.cs | 片段跨父过程筛选及接口引用重映射不能只检查主Source.Id，需检查IndexSource；当前root桥仍拒绝GroupOwner，先保证复制/清理不遗留悬空引用 | 保留现Group能力范围，不扩索引到父Group |
| src/VisionPluginContracts/WorkflowConfigurationSubscriptionService.cs | IndexOnly枚举int前驱；Encode/Decode/BindMany保留类型化selector；呈现[]/[literal]/[typed index caption]；移除本支持域的ArrayIndex Unsupported | 保留Global/Local/父Group等未实施能力Unsupported |
| src/VisionPluginContracts/PointAliasConfigurationSubscriptions.cs | 索引随POINT投影复制到两叶；同源同索引合成Parent；IndexOnly转交底层int树；组合读回显示索引 | 不新增POINT私有存储，混合叶tuple仍只读 |
| src/VisionPluginContracts/ToolConfigurationSubscription.cs、Controls.cs | 补有界LiteralText与文本提交；保持At(int)调用；不从caption反推Reference | 必须支持负索引错误路径，不能继续>=0拦截替代原运行错误 |

ParameterBinding.cs与PointInputBindings.cs已读：两者保留整个WorkflowValueReference并调用配置/图校验，当前没有拆丢子字段，原则上不用改；相关用例应验证这一点。上表是需要审查的完整链，不等于授权本线写共享文件。Framework→Contracts依赖环明确禁止。根线应登记producer=配置桥/GraphExecutor、consumer=CameraMapping numeric reader、payload=selector+每轮结果、lifecycle=持久化DTO/非持久执行委托、compatibility=缺省All、acceptance=下列端到端证据。

## 本线专属可审改动与验证

共享合同落盘后，本线只改CameraMapping坐标叶能力声明和InputReader fallback委托接入，保留scope注入provider优先。准备CameraMapping.IndexChecks，使用实际GraphEditorDocument→PointAlias桥→序列化/Undo→GraphExecutor→生产Runtime，不只测公式/selector DTO：

1. POINT数组All保持已过24例；Data1选0、Data2选1分别只供一个点，X/Y读回同索引且Count1，实际相似变换输出不变。
2. 改组合index同步两叶；改X来源或X index保留Y，并呈现只读tuple；Undo及保存重开保持类型化身份与原数字文本。
3. int前驱输出=1动态选第二点；改值=0选第一点；来源Success但缺失可选int输出时回退0；后续轮恢复不保留旧index缓存。
4. 负literal、越界literal、单叶越界分别验证Count0/原数组Count及最终模块E309/E1路径，不能提前变SubscriptionUnavailable。`-`原串保存并运行取0；`1.5/.5/-.5`正常UI路径未找到时提示Warning、两叶All、来源不回滚；小数parser仅独立API域；溢出明确拒绝。
5. 无索引/旧JSON回读All；scope注入reader仍优先；跨过程、非int index引用、index依赖循环、未声明能力的目标仍明确拒绝。

参数订阅与字符串路径的ArrayIndex不是上述float叶能力的隐含完成项；若要求一并覆盖，应单独选择其普通参数Resolve合同，不把数值两遍reader硬套给字符串。本轮先实际打通相机映射点/坐标域，随后按源列其余真实差异，不将人工验收用作完成门。
