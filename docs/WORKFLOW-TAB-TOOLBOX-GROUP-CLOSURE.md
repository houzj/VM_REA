# 工作流 Tab、工具箱与 Group 编辑上下文闭环

日期：2026-10-17。框架 main；不修改插件工作树及工具实现。此文只关闭列明的功能子集，未声明三个领域全部原生等价。

## 原始依据与实际修改

|功能|原始代码依据|重建实现|
|---|---|---|
|Tab 运行与显示分离|`VmProcedureDecorator.RunOnceCommandExe/RunContinuousCommandExe` 运行所属 VmProcedure，未改变 DisplayModuleSet|内部画布点击 Tab 单次/连续/停止，目标仍是所属根流程，界面留在当前 Group；递归结果同步刷新当前内部画布|
|当前 Group 的 Tab 配置|`VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml` 的 ConfigButton 向 ConfigParamsCommand 传 DisplayModuleSet；`.xaml.cs` 的 ConfigParamsCommandExecute/OpenParamsTabControl 按所选模块集打开窗口，使用 Show|内部画布上的配置按钮打开实际 Group 五页配置器，不再打开普通流程配置；保持当前内部画布|
|Group Tab 的菜单|`VmProcedureDecorator.IsProcedure` 取 DisplayModuleSet.DecorateSource 类型；TopTabItemStyle 只在 IsProcedure=True 绑定 OperateMenu|内部 Group 不显示顶层流程复制、删除、改名菜单；返回根流程恢复菜单；参数窗口期间禁用新增和工具拖放|
|配置修改目标与显示目标分离|原生模块及其参数对象各自持有身份，并不靠切换当前画布寻址|独立托管 EditProcedure 使用隔离草稿，确认后一次提交；失败完全不提交；撤销记录仍指向原来可见的画布。Group 父级输入、循环、中断和代理编辑采用明确的所属流程|
|专用工具编辑器|Group 参数代理与原 DynamicParamsTabControl 调用链|从内部画布的 Group 配置打开图像源专用 WPF 编辑器；绑定会话认可所属父流程或当前内部画布，仍拒绝其他流程及失效方案|
|工具箱弹层|`ToolBox.xaml.cs.OnMouseEnter/OnMouseLeave` 与 XAML IsMouseLeave/IsMouseOver 触发器；没有120ms定时器|移除自定延时，只延后到当前 Dispatcher 输入事件交接完成；旧分类的离开事件不能关闭新分类，锁定关闭弹层；拖放期间检查方案、流程、修订和编辑状态|
|三层 Group 插入|`DecoratorBase` 的 IMVSGroup 拖入分支在当前 Group.GetLevel>=2 时阻止创建|保持 Group 工具可见可拖，插入第四层时在创建之前拒绝；不是提前隐藏或禁用工具。当前错误反馈使用状态栏，原 HikMessageBox 警告窗口外观仍未恢复|
|Group 新增即拥有内部图|工具箱 AddNewItem 创建 IMVSGroup 模块集|工具箱 Group 新增将外层节点、内部模块集和可选执行连接作为一次事务创建；不等待首次双击；一次撤销一起移除。历史独立跨流程调用接口保留|
|实时复制语义|`VmModuleBaseDecorator.CopyItemsCommandExecute` 保存 FullName；`VmModuleSet.ProcedurePaste` 粘贴时查找当前源，源不存在就跳过；Group Export→LoadGroup→CopyModuleParam|带来源身份的 UI 剪贴板在粘贴时解析当前节点与内部图，不复制旧参数快照；源删除/改名后不复活旧快照。独立显式 GraphFragment 导入仍是独立协议|
|合法上游与输入引用|`VmModule.ModulePaste` 遍历 PreModules，优先使用复制映射；未复制的旧源仅在同父级且合法时保留；随后 PasteIO/UpdateRunParamBind|同父级保留实际外部上游执行/图像连接和局部变量订阅；复制内部节点、配置代理和 Group 层级时重映射身份。跨流程不沿用外部局部变量/父级输入|
|Group 拖放几何|`VirualForToolBoxGragDrop._dragOver` 用实际宽高的一半定位；`ToolItemGrid`/`DiagramItem` 使用实际尺寸进行对齐与热区计算|Group 拖影40px、居中偏移20px；混合30/40px节点按实际边界对齐；Group移动完整限制在5000画布；热区保持70中心间隔并补偿两种高度|
|Group 名称与内部画布|原生 Group 节点和显示模块集是同一模块实体|节点、内部画布和复制后的名称同步；由内部模块集改名也更新拥有者，一次撤销可恢复|

源码文件哈希见 `workflow-context-sources.json`。原始完整代码留在本地 `analysis/decompiled`；没有把 SDK 原生调用签名当成已恢复引擎实现。

## 实际验证范围

窗口检查由真实 WPF Window、控件事件和部分宿主处理方法驱动；并人工检查渲染截图。它们不是桌面鼠标全链路，也不是原 VM 同状态截图。具体操作、输出、时间与进程退出记录由 HTML 验收表关联。

- 从 Group Tab 实际打开五页窗口，画布仍停留在内部；启用循环、用真实选择器绑定父级变量终值3。
- 从该窗口打开真实图像源专用编辑器，把初始序号改为17；实际三轮读取1296宽的BMP，累积宽度 `[1296,1296,1296]`、序号 `[17,18,19]`，末轮序号19。不是把初始化值误当作末轮值。
- 关闭窗口恢复编辑；撤销/重做父级绑定及专用参数修改时保留内部画布；保存重开保留绑定；内部画布点击Tab单次/连续/停止运行所属根流程，保持当前画布，旧根菜单不能绕过当前Group上下文。
- 复制后再改源参数为42，粘贴读取当前参数；新 Group 保留同父级循环订阅并运行三轮，独立序号为 `[42,43,44]`。
- 多层真实图像处理 Group 复制后内部身份、运行参数目标各自独立，保存重开仍能运行；源删除后再次粘贴跳过。超深粘贴拒绝且修订/历史不变。
- 真实图像上游只复制消费者时，同父级上游连接保留；复制品输出像素与原消费者一致。
- 工具箱分类交接、旧离开事件、锁定、三层插入边界、一次新增/撤销、Group40px拖影、混合高度对齐和完整底边限制均单独记录。

## 未关闭与独立策略

- 本轮未完成真实 VM 同状态视觉/鼠标操作差分；桌面捕获此前超时、点击几何信息缺失，不能将事件驱动结果记成桌面输入成功。
- 源码使用全名查找，独立协议额外以方案/节点身份守卫；同名替换、跨方案剪贴板、所有复合引用/显示类型及原生 `.gro/.sol` 格式没有因此验收完成。
- Group LoopStep、动态索引、所有复合累积、原生实例/回调/异常码仍开放；图像源在当前实现中持有跨轮序号，不证明所有模块的原生生命周期已恢复。
- `TimeOutSet` 调用 Get/SetProcedureWaitTime，SDK XML 明确称“流程等待超时”。本次 REA 成功读取 SDK 二进制身份，但原生分析返回 provider_unavailable（未配置分析器/Java）。本地仍有既存Ghidra/JDK，本轮没有修改MCP或全局配置；这是当前连接的配置边界，不是无法分析原生文件的结论。没有把该字段猜成取消整个流程的计时器，也没有修改系统或客户端配置。
- 未实现的相机、通信、全局变量、脚本、原生导出等继续保留在框架清单，不以本闭环代替全框架交付。

## 运行图像列表复验

最终截图发现运行预览已有图像但列表仍显示(0/1)。按 ImageListPanelBase.GetModuleSel / DoRefreshLocalImage 读取 CurImagePath 同步当前项；宿主刷新抑制选择事件，不修改方案或额外运行。修复后截图为(1/1)，实际窗口操作339确认修订不变。本轮完整窗口事件364项、Core115项、插件18项和52项调用检查通过；不等于原生差分验收。
