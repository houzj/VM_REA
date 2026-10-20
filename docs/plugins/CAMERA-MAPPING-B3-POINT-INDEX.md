# B3：点组、逐叶订阅与索引源码切片

2026-10-19，有限源码切片。复位及其返回链沿用已有B3报告，本切片不重复。只读原厂资源/源码/原指令；无新原厂运行、产品修改或托管构建。证据索引见同名JSON及analysis/camera-mapping/b3-point-index-source-20261019；Index-B3PointSource.ps1复核所有新native导出体哈希。

## 整卡 S1/S2 退出缺口

| 范围 | 已有证据 | 尚缺，不能视为整卡退出 |
|---|---|---|
| Data1/2固定、Data3…8增删/显示顺序 | 原XML、PointPair/AddButton_PointPair、WPF模板、native SetParam/GetParam，既有专属验证 | 把各成员/token/RVA与真实IO名统一索引；不把DisplayIndex当数据组名或数组下标 |
| 点模式/坐标模式 | InputWay两个Trigger对8组设置IsPointMode；setter及GetSelected…读取体已取得 | 扩到初始化/持久化/失败分支完整映射后才能判本范围S2完成；本次只确认选定链 |
| 组合POINT逐叶与索引 | SetCombinationSourceOperation→VmIO→VmModule→SDK托管结构体路径 | native注册/索引选择实际callee、失败后的native有效订阅状态；不能用托管边界探针替代 |
| 模块数组展开 | 既有GetProcessInput/ProcessAlg及合成ModuleFrame记录 | RelateIndex如何变成真实ModuleFrame数组、负/越界/动态索引数量规则与恢复路径；合成数组只验证选择后的部分 |
| 文件/最后帧等B3余项 | 既有文件/复位有限切片 | 完整成功/失败/重开/作用域释放源链；不因单个点组切片通过判整卡闭合 |

因此整卡仍仅S0满足，S1/S2有独立已取得部分，尚未整体退出。内部B线阶段仍16/32；这不是整体完成率。用户最新总口径只计完整工具，当前0/10（0%）。

## 选定具体链：Data3.ObjectPoint → ObjectInput3 → X3/Y3

原AlgorithmTab的Data3组合按钮触发 `SetCombinationSourceOperation(OperationParams=ObjectInput3)`；初始化为 `GetSelectedCombinationOperation(ObjectInput3)`。Data1/2对应 `ObjectPointInput1/2`，Data3…8是 `ObjectInputN`，不能按名字模板统一补`Point`。原模块定义ObjectInput3的子叶依序为ObjectPointX3、ObjectPointY3。resource-map.json被动抽取全部8组×6控件、模式触发和16个POINT输入定义。

1. `SetCombinationSourceOperation.Operate`把来源字符串首个`[`后内容取出并去掉方括号，以剩余路径取得输出IO。先赋 `input.RelateIO=output`，后赋 `RelateIndex=GetBindIndex(...)`；来源不存在显示失败并return。它没有声明整组事务。
2. `VmIO.RelateIO`先解绑旧来源（无组合来源时逐子清理），再赋当前来源；`VmModule.Subscribe`对非叶只检查类型适配及子数相同，按Children位置逐项 `source.Childs[i].RelateIO=target.Childs[i]`，不按子字段名字匹配。故POINT实际注册两条叶，而不是把组合本身交给native。
3. 随后的 `VmIO.RelateIndex`再次解绑已有来源，`SetRelateIndex`递归赋相同索引对象到所有子节点，再重新Subscribe。组合getter只是取首个非null子索引，不证明所有子索引天然一致。失败只清当前对象的 `_relateIO/_relateIndex`，不能据此宣称已回滚所有叶的native状态。
4. `GetBindIndex`优先把含`.`字符串当IO路径，否则MathHelper.IsNumber成立时创建Name保持原串的VmConstantIO；空索引为null。`GetPubInfo`将null编码为constant=1、index="All"；常量保留Name、constant=1；动态索引保留Root.ID和Name。这里没有+1/-1重编号。
5. `ModuleManager.GetSubscribeResult`按UTF8固定256字节复制子叶名、来源名、index文本及三个标志/ID字段，调用 `IMVS_SubscribeResultEx`。成功后释放临时结构；非零抛VmException。native实际解释不在托管层。

对没有先前订阅的新组合，源码顺序会先尝试两叶的来源绑定，再执行整个组合的索引解绑/重绑。若已有叶来源/索引，额外解绑/调用次数由各setter的leaf相等判断、组合RelateIO重建和当前状态决定；不把这一无旧状态例子泛化成固定四次调用合同。

## 固定组、显示序号与模式

原Data1/2设置Fixed=True、CanDelete=False；其余Fixed使用字段默认false。PointPair的删除按钮只由模板CanDeleteA决定可见；ModifyDeleteBtnVisibility仅给非Fixed项开启删除，DeleteCommandCmdExecute本身没有Fixed保护。这里确认的是原UI路径，不能把隐藏按钮推成所有API入口的强制拒绝。

AddButton_PointPair第一次读取时缓存PointPairSource列表，之后取缓存列表第一个隐藏项，将该对象移至AddButton前，再写CustomVisible=True。PointPair的DisplayIndex根据当前可见集合从1重排，但Name/DataN不改；模块SetParam以DataN#flag写原N组标志。视觉顺序改变不能推成native数组按视觉重新编号。

InputWay=0/1分别向8组写IsPointMode=True/False。PointPair.set_IsPointModeA没有“值相同则return”的保护：点模式按TargetPoint、ObjectPoint重新初始化；坐标模式按TargetX、TargetY、可选TargetAngle、ObjectX、ObjectY、可选ObjectAngle重新初始化。GetSelectedCombination/Filter读取当前订阅并用SetPropertyValueByNameNotrigger回填CurValue，因此这一读回路径不触发来源写入；没有清另一模式订阅的代码。其他普通CurValue编辑的失败/回填路径另列，不能从这项读回结论概括所有切换副作用。

## 当前 native 下探断点

已取得平台 `IMVS_SubscribeResultEx` RVA0x6461b0→0x6653d0；UnSubscribe对应0x6462b0→0x6658e0。结构原偏移为subID+0、subName+4、pubID+0x104、pubName+0x108、pubConstant+0x208、indexModuleID+0x20c、indexText+0x210、indexConstant+0x310；SDK原体将这些字段原序送文本command0x3f3/0x3f4，随后调用已取SendCommand。

复用PlatformManager ClientMsgDeal的case0x3f3，实际转0x61b780；该体去掉文本头，再调用FilterNodeManager单例的0x5d6bd0。实际执行链为5d6bd0→5e2910→5e8db0（原日志名`FilterNodeManagerForSP::SPAddSubRelationShipList`）。5d6bd0在底层注册成功后还调用5d69d0更新关系记录，后者失败会覆盖前面的成功码；未发现可据以宣称整条链事务回滚的证据。

5e8db0以精确分隔符` . `拆出至少8字段，输出引用的来源kind=3，索引再分三种：constant=1且文本精确`All`→kind1；其他常量→kind2并`atoi`原串；动态→kind3并保存indexModuleID/indexName。这里没有加减1。它每批最多128记录，通过VM_PROXY_RecvServerCommand的第5参数传二进制列表。67ce60构造JSON命令0xbc5；Proxy4a610的case0xbc5要求该列表非空，调用24460，后者尾跳对象+0x100。Proxy加载体21590将GetProcAddress(`VM_M_SubScribe`)存到同一+0x100（数组槽0x20），而非根据函数名猜入口。

## 实际 ModuleFrame 选择体及反编译限制

取得VM_M_GetFloat入口1301f0→单例08e690→11f5f0，VM_M_BatchGetFloat入口130b00→11fde0。二者从订阅表fb040取记录；来源kind3进入124d40计算实际索引，再经ca720→f61e0读取指定输出名的数据。11fde0的循环变量从0开始，每轮加1；它是选择后的调用序号，不是PointPair.DisplayIndex。

124d40的Ghidra默认原型为void，会漏掉关键结果，已用线性原指令核对：

| 记录index kind | 原指令 | 已确认返回 |
|---|---|---|
| 0或未知 | 124d87→125014；125014 `MOV EAX,R13D`，R13D入口清0 | 0 |
| 1（All） | 12500f `MOV EAX,R8D` | 调用者给的当前序号 |
| 2（常量） | 12500a `MOV EAX,[RDX+4]` | 注册记录中的常量原值 |
| 3（动态） | ca500读取整数，124fbc检查返回码；125005读取局部整数 | 读取成功返回该整数；失败记录日志后走125014返回0 |

这是实际selector的源码/指令结论；动态读取失败返回0不能改写成“订阅失败”或自行补抛异常。ca720按模块ID查找数据管理器，f61e0按输出名字查找对象，找不到分别返回E0000314/E0000003，再调用fd90取指定索引的float。

fd90实际叶体已取：负index立即返回E0000014；数据块为空返回E0000314；否则先将数据块+0x40的总count写出，再检查`index<count`。越界返回E0000014，count=0时为E0000314；成功从`*(block+0x48)+8+4*index`读取float并返回0。这里的+8是存储头偏移，index乘4直接寻址，没有减1；所选GetFloat数据路径确认0基。错误发生后调用者如何覆盖/保留旧值仍须按11f5f0与模块读取函数分别核对，不能把叶体错误直接等同整个模块执行错误。

注册端真实VM_M_SubScribe为Frame RVA13b480：每批最多128项，每记录步长0x2d4，取submoduleID/subName，经143f80转换来源记录后调用fadb0存储；第一个非零停止，已有记录不在此循环回滚。143f80先清0x110字节内部记录，sourcekind3写首DWORD=3，再调用144080转换索引；该体保留kind1，kind2复制原整数，kind3复制模块ID/名称。最终内部记录布局与11f5f0读取相符：source kind+0，index kind+4、常量或动态ID+8、动态名称+0xc，pubmoduleID+0x8c，pubName+0x90。

fadb0加锁后按submoduleID查/建记录集合，调用d9620；d9620创建0x110字节共享记录副本，以subName为键查/建节点并替换原共享记录，再释放旧记录，返回0。该写入没有再次重编号索引。组合两叶在前述托管循环中分别进入这一注册链；未据此宣称多叶原子更新。

模式补证：ButtonSelecter.NeedInit=False仅影响NameValueItem.NotifyTrigger的通知路径；显式NameValueItem.InniterBusiness仍逐个调用Initers/ExternIniters。不能用NeedInit=False否定PointPair模式setter中的显式读回。

## 相机映射整工具剩余退出条件

| 卡 | 已取得且不重复运行 | 整工具仍须完成 |
|---|---|---|
| B1 | 原归一化/加权求解链和4/16点首分叉；专属最小修复；24例H位同及118例通过 | 把已审查证据纳入同一宿主验收基线；异常/失败域保持明确，不以样本通过扩为全域等价 |
| B2 | 同H分解原指令；Runtime reciprocal及专用inverse修复；24例216字段固定0.003通过，95例独立分解诊断已记录 | 在实际宿主配置/执行/显示路径确认采用同一生产代码和数值基线；独立公式诊断不替代宿主 |
| B3 | reset/返回链；本次点组/模式/逐叶/注册布局/原值索引至float叶体 | 模块包装层count/失败聚合；动态整数取值叶及未选非float分支；初始化/保存重开/模式失败/文件与最后帧/释放完整生命周期；实际原UI行为确认仍留S3 |
| B4 | B1/B2已批准的最小修复与固定回归已有证据 | 仅对B3审查后确认的真实差异提出有限修复；实际宿主配置、运行显示、保存重开、释放及Group自动回放并自审。人工验收单列，不计完成进度、不阻挡完成 |

优先继续本条索引链包装层与B3宿主生命周期，不增加无关设备/泛化索引实验。全工具闭合以前整体仍计0/10。

本切片验证：被动资源8组×6控件及16个POINT定义，3个托管程序集成员token/RVA，27个新native函数体、4个原指令窗口；JSON往返及全部新体SHA256校验通过。Ghidra独立导出成功；无目标运行、无产品构建，不能计作实际桌面或宿主通过。
