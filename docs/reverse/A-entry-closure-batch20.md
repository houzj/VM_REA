# A05.1 第20批：快捷键原始记录核对

结论：**A05.1 保持 partial，本批实施完成分子不增加。** 本批交付可复验的原 BAML 属性顺序取证及11项有效快捷键清单，不把已交的第17批53菜单节点、43执行解析项、15工具栏项重复算进度；27项服务实施仍属于 A05.3。探针已冻结，不继续扩大扫描以拖延本批收口。

## 新解决的实际歧义

`VMControls.WPF.Base/DiagramItem.xaml` 的7个绑定同时含 Gesture="Ctrl+L" 和另一个 Key。不能从反编译 XAML 文本猜最终键值。ReflectionOnlyLoadFrom 只取资源流，ResourceReader.GetResourceData 保留原始 BAML 字节；未实例化厂商控件、未构造 XAML schema、未执行厂商 SDK。字节解析器保留偏移和 payload，211条记录到达精确 EOF，11个 KeyBinding 类型号为 -337。原记录先设置 Gesture 后设置 Key。自有标准 Microsoft WPF KeyBinding 逐条同序赋值，实际11/11通过。

|有效键|命令参数|原最终消费|
|---|---|---|
|Ctrl+M|M|explorer.exe 打开 XmlPath 所在目录，异常吞掉|
|Ctrl+C|C|CheckRunning 后清空并写入 Clipboard CopyInfo 和 Position|
|Ctrl+F / Ctrl+A|F / A|排除 DataQueueModule、GlobalCameraModule 后设置 IsForbidden true / false|
|Ctrl+T / D / I / W / E / R / O|LevelT / D / I / W / E / C / O|IMVS_SetParamValue(handle, module.ID, LogLevel, 0..6)|

特别是 **LevelC 对应 Ctrl+R**。每项原偏移、属性顺序、标准框架版本/哈希在 `analysis/full-audit/batch20-20261020/A/standard-wpf/resolved-bindings.json`，离线门槛输出在 `resolved-entry-gate.json`。原命令 getter 无 CanExecute 谓词；不能给日志分支补造运行锁条件。SDK 内部作用仍未知。当前 main 的 Ctrl+C 有画布焦点条件，Ctrl+F/A 有框选条件，不能据键名相同宣称一致；Ctrl+M 与七个日志键没有对应原操作实现。

## 其余已追到的上下文入口

下表路径均相对 `analysis/decompiled/VMControls.WPF`；它们是实际事件/绑定链，不是词法命中计数。

|上下文|绑定、条件与消费链|
|---|---|
|节点 Delete|`VMControls.WPF.Base/DiagramItem.xaml.cs:946`：未锁工作区、未开参数、管理员；`base/diagramitemtemplate.xaml:1788` 将 DeleteCommand 绑定 DeleteItemCommand；`VmModuleBaseDecorator.cs:831` 检查运行状态、集合删除确认、清剪贴板/登记 undo 后继续模型删除。|
|连线 Delete|原 `base/connection/connection.baml` 64记录、1绑定；`Connection.xaml` DeleteConnectionCommand → `VmConnectionDecorator.DeleteConnectionCommandExecute` → 非全局登记 RootOperation；DeleteConnection 检查两端运行状态后 Source.NextModules.Remove(Target)，清 PathPoints。|
|页面 Ctrl+V|`ItemsPage.KeyVdown` 在未锁工作区时处理，DoPaste(Point(0,0)) → DiagramPage.xaml OnPasteItems → PasteItemsCommand → CheckRunning、登记复制操作、ProcedurePaste、位置处理。0,0 是键盘粘贴分支哨兵，不是最终位置。|
|页面 Ctrl+F|同一 KeyVdown，DataContext 的 IsShowImportGroup 不为 false；SearchOperation 显示搜索窗口，确认路径后 OnSearchModule → ProcedureConfigControl.SearchModuleCommand → VmProcedureDecorator.SearchModuleCommandExecute，设置显示容器、定位模块。|
|框选 Delete/Ctrl+C/F/A|RectDragPanalBase_KeyDown → OnDelete/Copy/Forbidden/ActivateItems；RectDragPanal.xaml:102–112 的 ParamsEventCommand 将 CurBoxOfItemsOfData 传给对应 command。DeleteItemsCommand 经 DecoratorBase.CheckRunning、集合确认/undo、逐项 DeleteItemCommandExecute(false)；CopyItemsCommand 进入 VmModuleBaseDecorator.CopyItemsCommandExecute；Procedure/Group 的 Forbidden/ActivateItemsExecute 逐项设置 IsForbidden。事件方法本身不包含管理员/运行守卫，不能套用节点 Delete 条件。|
|搜索下拉 Up/Down/Return|SearchDryPopup.SearchableTextBox_PreviewKeyDown：popup 开启且列表非空；上下键改索引并滚入视野，Return 选中后关 popup、Handled=true；SetSelectedSearchItem 调 JumpCommand。ZoomArea.xaml:103 绑定 ProcedureConfigControl.JumpTargetCommand → JumpTargetCommandExecute 按 FullPath 首段定位 procedure → SearchModulePopupCommand。属于局部控件键。|
|缩放/平移修饰键|ZoomArea 模板接鼠标事件；Ctrl+左键捕获拖动并按 Scale 改 ViewArea；Shift+滚轮水平滚动，Ctrl+滚轮改缩放；原先调用 OnHoriScroll 再 OnChangeZoom，两修饰键并存可都生效。属于鼠标修饰键，不冒充全局键盘快捷键。|

当前 main 消费入口集中 `WorkflowCanvas.CanvasKeyDown`：文本/组合框焦点先返回、画布焦点删除/复制/粘贴、框选禁用/启用、未运行且未锁时 Undo/Redo。其 Esc、Ctrl+Z/Y 等现有独立实现不能反向当作原壳全局快捷键证据。第17批 MainWindow BaseWindow_PreviewKeyDown 的原 CIL 只有 nop/ret，只能证明这个具体 handler，无权证明全程序没有其他 dispatcher。

## 固定门槛与剩余

已满足本批有限门槛：取原记录而非按 XAML 属性文本猜键；11项绑定→命令→消费；局部事件入口条件/下游绑定；标准 WPF 自有回放；当前缺口显式映射；失败和哈希保留。

A05.1 完整门槛尚缺：**壳/全局 routed input、命令注册与动态资源载体的完整覆盖证明**。目前仅选定 MainWindow、BaseWindow、DiagramItem、Connection、ItemsPage、RectDragPanal、SearchDryPopup、ZoomArea，未获得全程序 dispatcher/native 输入注册穷尽证据。因此不将有限清单伪称“全部快捷键”，也不以11 PASS关闭整个单元。下一次仅在发现明确的新注册载体/调用链时重开此缺口。原 SDK/native 日志与模型内部行为单独未知，纯 WPF 回放不是原机验收。

## 复验和证据

新脚本在 `tests/EntryClosureChecks20`：ExtractBaml.ps1（原资源只读）；ReadBaml.py（有界纯字节解析）；ResolveStandardBindings.ps1（仅标准 WPF）；VerifyResolved.py（离线现存证据门槛）。本批未 build、未改产品、未启动原厂控件或设备。原程序集 VMControls.WPF SHA256 `99ffa401c8784472913ad43813d68a8a5577890c00e149506e3e5664fa61c97d`，MainWindow carrier 沿用第17批 SHA256 `319618b9f82dfc30546c900c1c19a665e0fce1d87d2a7ffa7bb4345915b180c1`。最终文件哈希见 A 审计目录 manifest。

BAML 字段格式参考微软 [BamlRecords.cs](https://raw.githubusercontent.com/dotnet/wpf/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Markup/BamlRecords.cs)，并核对本地 net48 Baml2006Reader CIL；解析器只支持本次证据需要的记录，遇未知类型失败，不宣称通用格式工具。
