# Map路径与真实主壳最终验证

2026-10-20。本线源码→实施→自动验证自审通过，交根线审计；不自行更改完整工具计数。人工验收单列，不是完成门。完整工具范围沿CURRENT-WHOLE-TOOL-EXIT-MATRIX，不以本次两项目替代旧源码/数值证据。

## 实际结果

| 项目 | 实际结果 | 意义 |
|---|---|---|
| Map PathChecks | PASS30，build/run退出0 | 原derived输出路径suffix/目录前门与尝试值/history；原base双输入手输可达binary路径、原case8 XML失败缓存/路径副作用、UI保留尝试值、同坏值picker两次history |
| 真实MainWindow HostChecks最新版 | PASS26，最终增量build0警告/0错误，run退出0 | 实际catalog/注册/原模板、两模式、POINT数组、两个真实IMAGE控件Initers候选在dropdown前存在、打开刷新和typed选择、两原图168+168、显示、配置初始化保缓存、Save大小写门与实际XML导出/readback/history/Success、方案保存重开、RemoveNode释放 |
| 首Host与诊断 | 真实退出1，保留 | Save80100203，file不存在、scopeDisplay=false；发现.images伪模块ownership被Retain清理，批准专属aggregate修复后成功 |
| 首新版IMAGE Host | 真实退出1，保留 | 模式切换异步CanTriggerRun尚忙，测试立即POINT Bind遭原宿主保护；仅tests补Dispatcher有界idle等待，不清busy/不改产品同步语义 |

最终SAVE-DIAG：file存在，document/state/effective路径相同，scopeDisplay=true；事件select→history→Success。State诊断只余故意大写.XML拒绝产生的Save OperationError，不再有GetFrontParamItems Unsupported。故意失败诊断不清空以伪造全绿。

## 本轮修复及原链

- base路径：OpenFileDialog先写CurValue，SetParam失败不回滚模型；NameValueItem.Check默认空，native失败只标错误，原DoOpenFile null→path后无条件history。Map SetInputPath仍仅成功更新原native path，并保reader既有Replacement失败副作用。配置专属KeepAttempted/成功通知/无条件history已接；未套derived前置校验。
- Save：原SaveFileDialog双字段Filter、CurrentCulture EndsWith(fields[0])，拒绝不export/history；成功SaveFile→CalibPathName GetParam→history→SaveSuccess。生产对应顺序通过真实有效图像seed验证。
- 所有权：原Map一个模块对象拥有文件与图像缓存；独立宿主必须以实际moduleId/node生命周期运输。ScopedState(File+Images)取代Id+'.images'伪模块键，配置初始化/doc.Changed保同一执行结果，真实Retain/RemoveNode一起释放，standalone兼容字典保留。
- 图像源初始化：原GetFrontParamItemsOperation→GetFrontIOWithLeafItemsByName(IMAGE)→EnumEntrysA赋值。根线统一接OperationResult.SubscriptionTree→State节点保留→ImageEnumeration建控件立即消费typed候选；Map返回原query tree。测试不再用doc.Connect图像捷径，两原图由实际IMAGE下拉绑定。共享引用身份不从caption反推。
- 原过期成功Message移除，不改输出、NativeErrorCode、算法状态；有限实现边界保留在证据文档。

## 全矩阵复核

| 活动 | 当前退出依据 |
|---|---|
| 输入配置/模式/typed读取/ROI | 旧原10例oracle+生产Typed121/InputState48/Region25；本轮真实UI两模式/POINT/IMAGE接通。InputReader、RegionInput源码与typed最终manifest完全同hash |
| 图像解码/格网/匹配/数值 | R03/R04真实caller原体及旧source2046/ROI7361/6-8 axis12 mask4 full4证据；本轮实际两图168输出/显示。ImageSolver/Evaluation/Solver源码与typed manifest同hash，无新数值改动，不泛化宣称全域parity |
| 两文件/方向/cache/刷新 | 已取原ReadFileMatrix/SetParam/7d4f20方向，既有File/Session证据；本轮两个base UI输入失败/成功正常协议补齐。Files/FileSession/ImageFiles均与typed manifest同hash |
| Teach/导出/失败/等待 | 原Process读取/哨兵/模式门、既有ImageFiles/InputState；本轮正常成功图像缓存保持、Save原选择门和读回顺序、故意Save失败可见。未改算法等待/错误输出 |
| 原资源/Reset/显示/释放/持久化 | 旧Display15/Reset27与原有效Algorithm名单同hash；本轮真实MainWindow显示、配置关闭、缓存不被剪除、Save/load、真实moduleId RemoveNode覆盖正常活动 |
| 主壳注册/依赖/目录项 | 实际MainWindow注册、VisionEditor项目引用、catalog ModuleId均在当前DLL运行验证；不以独立Graph替代 |

未支持/独立边界仍明确：Region优先级无原证时使用根线批准窄独立互操作规则；binary不安全长度不重放、远程FTP没有伪造本地等价；方案JSON格式是独立运输，不称原native ABI。原Platform Init/设备未执行，原桌面视觉人工验收待用户；这些记录不以测试通过伪造原行为。

## 身份与留存

批目录：analysis/map-calibration/map-path-host-final-85e727e1500d4ca98d0a2187e539e18a。
最终manifest SHA256：`cc86e1612d925bac0e8b1d9f169d2885714fd5aae74e51e96bf2af23dd8425ac`。
450个src源码/资源快照逐项复核0漂移；manifest包括当前Map全源码、共享路径/tree/真实壳来源、7个实际DLL/PDB产物、全部通过和失败日志、旧证据manifest及旧source逐项比较。

- 当前Map DLL：`914d02faa181554add1f6cc07ecaaf80af92a4330151dea00a157dfcfbb69e98`。
- 当前Host DLL：`04e0834d1b34462aa9e6c8e3a998f4e0c8d0d4d2bbdae8188e965b463564ce42`。
- Map Plugin.cs：`1762c2dc14312f4829685910c8c8199bc37292dc13eead27e39940c108e94bc3`。
- Map Configuration.cs：`d995346993b7069d93c0b68335768c8ee0e47eed33e34c15f33b3cd1a37cccc5`。

Path30是枚举typedtree变更前冻结产物的真实结果，路径逻辑未受其影响，根线明确不重复该项目；最终Host26消费当前生产DLL。完整构建先前有2条既存MainWindow nullable警告，最后仅tests idle修正的增量build无警告；原日志均保留，不把增量0警告解释成源码从无警告。managed已释放，后续仅docs/analysis，无源修改、提交或推送。
