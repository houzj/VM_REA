# B01 batch2：原厂参数选择器缓存与确认链

2026-10-19。续接 `B01-tab-batch1.md`，本批不重跑13项根解析器检查；只推进 `RunParamsSelectData` 的缓存/选择子问题。没有修改产品、共享台账、已交根解析器探针或 tool-plugins，没有构建产品。**20个具名谓词通过仅覆盖下列原托管方法及合成输入，不关闭真实模块/窗口门槛。**

## 身份与取证方式

- 原件：`D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlUI.dll`，SHA256 `639d0fb9dfca81e38df690e201b73874ce006d1361b689ddc6d5f41f7b0b04fd`，MVID `a00f8b40-7a51-4c6d-8aeb-cad134724f1f`，.NET Framework4.6.1、AnyCPU。REA `inspect_managed_artifact` 身份 Evidence `ev_0b720b58c29ebc598cf14b4033d1cf52c913afcb9c9d066a53d694437e997b36`。
- REA `inspect_managed_members` Evidence `ev_704887f3d378d65afcfa43caa1d2135676c1162d67043b31ebb8b30f485faf64` 明确 unavailable：metadata representation 超过67108864字节预算。完整失败记录保留 `analysis/full-audit/batch2-20261019/B01/rea-member-limit.json`，没有把空methods当“没有实现”。
- 继续采用已有同版本反编译源、实际CLR加载的原方法运行以及Reflection原CIL。最终 probe.json记录76个方法的build-local token/CIL（含选择器闭包和窗口事件；提取窗口方法不实例化窗口）。这部分不是REA decompile结果。静态源哈希见同目录 `source-hashes.csv`。
- CLR实际载入Apps.XmlUI安装副本；Apps.XmlParser、VM.Core来自GAC。原件与安装副本同哈希：Parser `78ec7ae1e5adefff29197099c7109b153a3f75d092b7c3e9dc253d8c1b25a8d1`，VM.Core `936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d`。probe记录实际路径/MVID，不凭LoadFrom参数推定加载位置。

## 源码级状态与谓词

以下源码均相对 `analysis/decompiled/`，保留原拼写 `OtherSelectedRunParmas`。

| 入口/成员 | 源码确认行为 | 本批实际边界 |
|---|---|---|
| `Apps.XmlUI/Apps.XmlUI.ParamTab/RunParamsSelectData.cs:96` GetSelectRunParams | 索引经GetValueType取得display-type名单；名单null才是全部类型。重新设置树，但右侧RunParamsA只在null时展平缓存一次。 | 索引映射实跑；完整GetSelectRunParams会进CheckLocalModule/SDK，未运行。 |
| 同文件193 GetSubRunParamsTree，首次缓存缺失 | 集合节点递归；普通模块拿Params.GetParamRoot/GetRunParamsTree。只有树非null且isAllType才生成RunParamsBaseNode并加入字典。**首次直接选分型不会填缓存**，即使取得原树。 | 此首次构造分支静态确认，未冒充原实例实跑。 |
| 同文件230–240，占用排除 | 只在IsFromVarBind且other列表非null时生效；若已有选择，先在局部变量中去掉与当前模块已选名称重合的other项，再排除其他占用。没有回写OtherSelectedRunParmas字段。 | 未执行此首次填充路径。原闭包CIL保留。 |
| 同文件241–251，已选恢复 | 按已选项ModuleID匹配当前moduleid，再按Name找**当前树第一项**，复制OutputEnabled。不存在的项不补入缓存；同名不按TargetObject身份恢复。 | 静态确认；本批不创建“失踪项仍显示”的兼容规则。 |
| 同文件252–265，变量类型裁剪 | varType非空才裁剪：other/当前selected中匹配ModuleID+Name者优先保留，否则保留`!IsSupportSub || RunParamIoType == varType`。这是首次缓存形成时的规则。 | 保留原谓词；不能简化成“所有项必须可订阅”。 |
| 同文件269–290，缓存命中 | 分型按`valueType.Contains(DisplayType?.ToLower())`创建投影list，全部类型直接用缓存list。每个显示node的ParentTree改到新Holder；缓存原node身份/勾选保留。**不重新读取参数树，不重算other占用/变量类型裁剪。**空结果不主动清空既有Holder.EnumEntrysA。 | 原方法缓存命中实跑，Integer/Float/String合成节点；重用身份与重设父指针通过。 |
| 同文件111 GetRunParams | 展平缓存中全部非null值，不按当前显示类型筛选。 | 实跑；null字典值跳过。 |
| 同文件127 GetSelectedRunParams | 从整个缓存收集OutputEnabled；普通上限32，变量绑定上限int.MaxValue；超限→other重复→可订阅但IO类型不同，依次消息窗后false。 | 上限数值/成功路径实跑；三种拒绝分支未打开消息窗，结论来自原源码/CIL。 |
| 同文件确认成功尾部 | 若旧SelectRunParams非null先Clear，再将字段替换为新list，返回true。**外部持有旧list别名会看到它被清空**；节点仍是缓存原对象。 | 实跑清空旧别名、新list、节点身份保留。 |
| `RunParamsBaseNode.cs:138–183` | ModuleID来自NameValueItem.GetVmModule()?.ID或DynamicIO根ID；IsSupportSub要求NameValueItem实现ISubInfo且SubscribeVisibleA；RunParamIoType来自ISubInfo.ComType。 | 最小null TargetObject的绑定节点不支持订阅，即使SelectedVarType不匹配也被确认接受。这只是合成输入对原谓词的运行验证。 |
| `RunParamsSelectWindow.xaml.cs:24/59/68/76` | 变量构造器保留旧选择列表引用、设置other/type/bind；确认成功才DialogResult=true并Close；删除按钮只改缓存node.OutputEnabled；组合框传索引。 | 窗口和消息框均未创建；原事件方法CIL提取。 |

## 确认、取消与实际调用方

`VMControls.WPF/VMControls.WPF.GlobalUI/GlobalVariableItem.cs:728–756`：ModifyRunParamsExecute先把 `_OldSelectedRunParams` 重置并从原选择取得绑定记录，随后新建RunParamsSelectData；DecorateSource取VmIO.Root；other由父集合中 **Name不等于自身Name** 的项收集，不是对象ReferenceEquals判定；ShowDialog==true才把GetSelectedParams写回SelectedRunParams。

所以不能笼统声称“取消完全无副作用”：旧绑定记录已在弹窗前重建。另一方面，首次树把Holder变成新的RunParamsBaseNode，只复制原选择的OutputEnabled；窗口勾选/删除改这些新包装而非原旧选择对象，取消分支没有本地提交/解绑代码。本批没有运行原窗口、全局变量SelectedRunParams setter或SDK绑定写入，不把静态取消链称为真实绑定回滚验证。

`TrigerOutPutItem.cs:217`同样检查ShowDialog，不成功直接return；其输入路径另用 `ModuleInputSelectData`。后者有自己的IoallType/int/float/string及GetInputTreeNode规则，不能混入GlobalVariableItem的基础RunParamsSelectData合同。

## 探针安全边界及运行结果

`tests/NativeEvidence/B01Selection/SelectionProbe.cs`直接反射调用原方法，不重新实现选择器。缓存为合成列表，TargetObject为原Integer/Float/String普通构造对象；无需实际方案、XML、窗口或设备。

缓存命中必须通过原方法的IVmModule检查及ID读取，故使用 `FormatterServices.GetUninitializedObject(VmModule)` 的**ID-only未初始化壳**，明确不是可运行VmModule。使用前已核对：

1. VmModule.ID只读值类型moduleInfo.nNodeID，无Params/SDK/集合调用；探针以17字节原CIL结构及ResolveField身份强制检查，不符合即停止。
2. moduleInfo是值类型、nNodeID为uint，默认0；无自定义VmModule finalizer；额外SuppressFinalize。没有访问Name、Params、Dispose、模块集合或实例构造器。
3. GetSubRunParamsTree在字典已含key0后只读取缓存、DisplayType、重设ParentTree和Holder集合；没有走首次Params分支。使用的Holder属性及原节点父通知链已核对；未订阅事件。

最终运行 `analysis/full-audit/batch2-20261019/B01/run-413218311b5a44309e281d3872e5e5bc/`，exit0，20个具名谓词全true。进程模块记录未见iMVS/原生PlatformSDK/图像源模块；这不等于证明所有托管静态构造均无任何副作用。运行前后被列入清单的原DLL和探针源码SHA完全一致，未写安装日志。

保留两次ID CIL门槛拒绝的开发运行 `run-93e05...`、`run-7e013...`（debug-style nop/局部返回结构修正前即退出，未创建壳）；`run-054459...`是基础成功版，最终版仅增加闭包/窗口原CIL清单。首次PowerShell脚本受本机ExecutionPolicy拦截；复验使用进程级 `-ExecutionPolicy Bypass`，没有修改系统策略。未累计开发次数作为检查数量。

- 最终probe.json SHA256 `C42E1382570B2C2FCE7B3B7ED8C2E0668A6DEA2EA7067870063058E19FE16D80`。
- SelectionProbe.cs SHA256 `470D08787D4D52355EEF62B0A83AD1E5EBB3E8F72C5A558071AD320A1212B24B`。
- Run.ps1 SHA256 `4BF25D1C405BA0E80A44153555A8010F5DA7D10B9D88EC9260FCFF7BAA3E2666`。
- 复验：`powershell -NoProfile -ExecutionPolicy Bypass -File tests/NativeEvidence/B01Selection/Run.ps1`。独占GUID目录、Framework64 csc编译、45秒超时；仅终止该脚本启动的自有进程，不使用共享dotnet构建。

## 未关闭与接续任务

B01仍开放：真实模块Params/Data/XmlPath和专用装配；首次all-type缓存的实际模块/占用/已选恢复；真实窗口取消和确认拒绝；SDK订阅写入及变量类型变化后的窗口重开。

下一探针先指定任务自有原模块/方案，固定模块ID和Name，分别取“本变量已选/他变量已占/树中消失/可订阅不同ComType/不可订阅”五类，记录首次缓存前后与索引切换后的对象身份。拒绝路径会创建原消息框，须使用自有交互实例及明确关闭记录；取消前后分别记录旧绑定快照、SelectedRunParams与SDK绑定，不只比较列表。没有原实例资源时继续提取GlobalVariableItem setter、GlobalVarExtern和绑定调用者CIL，不用独立模型替代。

给main/FCR-019的建议仅是待采用字段与生命周期：区分DisplayType筛选与ComType绑定、IsSupportSub、ModuleID+Name身份、首次缓存快照、选择包装身份、确认旧list别名清空和取消前快照更新。不能据本批直接改插件侧配置或删除main兼容候选。
