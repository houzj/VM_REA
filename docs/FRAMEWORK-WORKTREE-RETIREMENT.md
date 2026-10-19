# 三个 framework 工作树审查与收尾

日期：2026-10-19。用户明确授权审查未提交内容、保全证据、按需合入 main、验证后清理 framework-shell/framework-canvas/framework-group；tool-plugins 不修改。

## 基线与保全

main 起点为 `c5806f784c857de27328f870dfec6a75ed304401`，已有未提交修改，已先保存原始补丁、状态及每个在制文件副本。三个工作树均无 `main..HEAD` 提交，但各有未提交/未跟踪内容，不能直接删除。

|工作树|HEAD|已跟踪修改 / 新文件|保全文件数|
|---|---|---|---|
|framework-shell|99fc6dc9a1cd0f116becfb1846a8971db9d127d2|3 / 12|1727|
|framework-canvas|0720a57ef5db349f8c6bfa791e8f72db6ab46671|6 / 6|1495|
|framework-group|7dcead3189fdda475784d46b8e1c431fa187a315|12 / 2|1533|

本地备份在 `analysis/full-audit/framework-retirement-20261019/<worktree>/`。每份包含源码、文档、资源、analysis 审计文件、artifacts、working.patch、status.txt、HEAD.txt；仅排除 `.git` 元数据和 bin/obj 可再生成缓存。4755个原始文件逐一与备份比较 SHA-256，无缺失或差异。清单为 `preserved-files-sha256.csv`，其 SHA-256 为 `30B0EE80F0D0A40B5E5CABE81D6B076B9ADB62947DCC4006E5D68E95AB98F0B5`。Git忽略这些证据备份；它们只在本机，不宣称已远程归档。分支保留以便追溯已提交历史。

## 逐项审查决定

所有候选代码/测试增量均纳入，按各自 HEAD 三方合并，未整文件覆盖 main 的在制修改。文档冲突合并两边证据；汇总表中过时失败记录不覆盖 main 新进度，以本报告及原始日志为准。

|范围|原始证据 / 精确入口|合入行为及界线|
|---|---|---|
|Shell历史按钮|VisionMasterMain工具栏资源；审计见 MF01-TOOLBAR-UNDO-REDO.md|真实禁用资源随 IsEnabled 切换；未重建 VM.UnRedo 服务|
|运行策略|VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/ProcedureRunData.cs::ReloadProcRunStragety/SetRunPolicyConfig；app.config::RunningMode|0/1选择及状态写入；JSON偏好、简化窗口属独立适配，告警后端未实现；右上静态文字不擅自接线|
|在线帮助|VisionMaster.Exe.Audit/app.config::OnlineHelpDoc；MF01-HELP-DOC-SOURCE-AUDIT.md|菜单入口按随包地址打开，测试注入启动委托，不访问网络|
|关闭等待|VisionMaster.Exe.Audit/VisionMaster/ApplicationOperation.cs::Dispose/RunSyncWithEvent|本地等待自有 activeRun 释放后关闭；不宣称等于SDK/设备释放|
|固定路由/缓存|VMControls.WPF.Base/DiagramPage.xaml.cs::GetPath/GetPathMap；Astar/APoint.cs::CurTimeEnableFlag/CheckTime|5000页面、1032×1032网格，复用并逐搜索清理；单缓存串行锁为本地适配，不是原生每页实例生命周期|
|复制/插线|Connection.xaml.cs::Shape_Drop；VmConnectionDecorator.cs::InsertModule；VmModuleBaseDecorator.cs::CopyItemsByDropCommandExecute|Copy拖到线时重接原节点、保留原位置；多复制按首项锚点；真实文档Undo及保存重开覆盖|
|预览与小地图|Connector预览释放链；ZoomArea.xaml.cs::GridEagleEye_MouseDown/Move/Up|提交后清理临时Path引用，小地图不吞原始路由事件；详见MF02各审计文档|
|Group数量配置|DynamicObjectConfigControl/Apps.XmlParser.ParamTab/ParamConfigDataBase.cs::.cctor/AddParam/AddGroupCommandExecute；app.config::MaxRunParamsNum|默认32、应用配置覆盖；XML查找为独立读取适配，新增FCR-020；配置2时UI拒绝第三项|
|Group专项与未知项|GROUP-LOOP-BOUNDARIES.md、GROUP-ADVANCED-RUNTIME.md、GROUP-INDEX-COMPOUND.md、GROUP-NATIVE-EDITORS.md|取消/异常不发布局部成功输出、失败后重跑、确认/X即时写回、逐次Undo/Redo覆盖；native状态转换、ABI、撤销服务仍未知。FCR-021候选资格、FCR-022撤销服务保持开放|

未将原生缺失能力凭意图补写；Frontend UI IL/BAML 审计、原始失败日志及所有中间证据一并保留。跨项目契约更新只在 main，未触碰插件工作树。

## 本轮验证

日志位于上述备份根下的 `validation-*.log`。命令使用 `dotnet run --project <工程> -p:BuildInParallel=false -p:UseSharedCompilation=false`；已有资产时附 `--no-restore`。

|验证|结果|
|---|---|
|VisionEditor build|0错误；既有MainWindow nullable警告|
|VisionFramework.GroupLoopChecks|23项通过，exit 0|
|VmRouting.Checks|68项通过，exit 0，含原始AStar/PathFinder对照|
|VisionEditor.GroupCategoryExpansionChecks|30项通过，exit 0；首次缺project.assets未执行，正常还原后通过|
|VisionEditor.CanvasChecks|73项通过，exit 0|
|MinimapCaptureChecks|实际WPF路由事件通过，exit 0|
|VisionEditor.ShellChecks默认模式|未通过：File/Recent键鼠焦点与弹层未打开，原失败日志保留|
|ShellChecks --control-events-only|76项通过，exit 0；明确排除菜单弹层输入/渲染验收，继续验证实际Click数据链和关闭排空；两次调试中间失败日志保留|

控件事件模式是显式测试参数，默认断言保持；不将控件事件测试改称桌面输入通过。主程序没有加入截图代码。目检本轮画布障碍图可见三个节点与橙色连线；Group双栏图可见候选、类别及确认按钮，均非真实VM差分截图。通过数只表明已列场景，不代表完整原生等价。

整体 WPF 首轮通过451项后，在关闭测试超时：夹具人为注入 activeRun，却没有运行任务调用 FinishWorkflowRun。修正夹具，在 Dispatcher 显式完成该伪流程的正常收尾，默认产品关闭等待保持。第二轮完整456项通过、exit 0；含真实图像、Group执行、订阅、保存重开、流程与调试会话关闭。日志 validation-wpf.log / validation-wpf-final.log 均保留；仅将本次夹具增量纳入提交，main原有WPF测试修改不夹带。

## 最终清理结果

集成提交：086299774d42e79b7e03bc47a95ea9d447f97e9d。删除前再次逐一核对4755份源文件和备份哈希、实际文件数、未提交状态与HEAD祖先关系，均通过；只读进程检查没有三个工作树的活动进程，目录内无reparse链接。随后git worktree remove --force仅移除三个命名工作树，保留原分支和本地备份。

git worktree list最终只剩main和tool-plugins（后者HEAD仍为828428426e89dfbacece18e371d0d0126e1687da）；三个framework目录均不存在。没有终止任何进程，没有修改插件工作树，没有推送远程。main未提交文件清单与操作前逐项一致。本次提交只包含审查增量及必要夹具修正；main原有在制源码/文档/测试仍未提交。产品/测试/项目文件没有引用已删除工作树路径。
