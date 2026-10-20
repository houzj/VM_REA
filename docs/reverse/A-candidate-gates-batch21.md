# 第21批 A 线实施单元可关闭性审查

只读基线 `1174604e5713994bc10eae5209a907d74f302129`。审查 A01.3、A05.3、A06.1、A06.2，**没有可以在本批完整关闭的单元，实施完成分子 +0**。不重做 A05.1，不创建假服务/空编辑窗口。未修改共享产品、未构建、未访问 tool-plugins 或设备。本报告不把历史阻塞换个名字当新成果。

|固定单元|现实现与原链核对|完整关闭的确切缺口|
|---|---|---|
|A01.3 原Group导入回调到模型落地|当前 ToolNavigation 负责本地 catalog/module 注册及创建，没有原 gro 导入消费者。原 `DecoratorBase.cs:198–211` 从 GroupTools 目录组成 gro；`VmSolution.cs:1711–1859` 检查版本/父节点、登记undo、调用 SDK 并等待；4583–4612 回调先发公共事件，再成功构造模型/设Parent/Ready，最后完成flag。`VmModuleSet.cs:660–674` 查重后替换集合。|未有可独立解析的原 gro 数据合同及 native 返回 ID/模型元信息消费者；不能以新 JSON Group 导入替代。原事件先于模型落地及失败后 flag 恢复仍未运行确认。解除需取得原 gro 解码/导入返回合同，或用户明确授权独立格式；本批没有新解除条件。|
|A05.3 全局变量、通信、触发、脚本等专用服务|第17批入口已定位；当前通用节点/参数字典不等于专用全局会话、通信连接、脚本宿主、触发服务。|需要逐服务配置→会话→执行→错误/关闭完整链与后端，不是一个命令按钮。通信/触发还有设备边界；不因可建空对话框而关闭。没有新增真实服务后端，不能完整交付该固定单元。|
|A06.1 编辑入口/状态/关闭|第13批原 CIL：OnClosing→FrontendEdit.Close→DoSave；DoSave 在脏检查前就 Save 快照，NeedSave 比较 temp/solution MD5；成功关闭解绑真实设计面事件。当前仅 FrontendDesignMigration 的离线 XML 转换，不存在实际 DesignSurface/DocumentManager。|真实可编辑文档快照、temp/solution/designer 三缓冲身份与保存/取消消费缺失；不能把 IsBusy+确认框等价为原关闭。须先满足 A06.2 可用设计面和 A06.3 必要缓冲合同。|
|A06.2 完整布局/资源/BAML绑定|新增解除条件：第20批纯字节 parser 可复用来安全检查已保全 Frontend BAML。本批选定 editfrontendwindow 资源实际解析168条记录到 EOF，无 CLR schema/控件构造。确认类型表含 EditFrontendWindow、FrontendEdit；命名记录含 PART_View、mainEditView，与原 Connector 相符。|这只解决一个外层资源可读取性；未解读其余编辑面/模板/扩展引用/绑定表达式的完整语义，也没有重建 ICSharpCode 设计面、属性编辑和快照序列化。单个资源记录清单不等于完整布局实施。|

## 本批唯一新增源码证据

使用既存保全文件 `analysis/full-audit/framework-retirement-20261019/framework-shell/analysis/full-audit/frontend-ui-inspect/resources/FrontendUI.Design.g.resources.editfrontendwindow.baml.resource-data.bin`，SHA256 `FA787F2CFCF158F493CBC2A106F3E1C76F4A1C9C3D1CF91AEAE206AC5DF935D7`。复用 `tests/EntryClosureChecks20/ReadBaml.py`，实际退出0：168 records、0 keyboard bindings。0 bindings 仅为此解析器筛选键盘属性的结果，不表示没有数据绑定。本次未请求加载原程序集、XamlReader 或设计器；输出 `analysis/full-audit/batch21-20261020/A/editfrontendwindow-records.json` 保留全部原记录偏移和 payload。没有把记录类型表误当完整 XAML 对象树。

复验命令：

```powershell
& 'C:/Users/houzhongjie/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe' tests/EntryClosureChecks20/ReadBaml.py 'analysis/full-audit/framework-retirement-20261019/framework-shell/analysis/full-audit/frontend-ui-inspect/resources/FrontendUI.Design.g.resources.editfrontendwindow.baml.resource-data.bin' 'analysis/full-audit/batch21-20261020/A/editfrontendwindow-records.json'
```

后续选题建议：A06.2 的安全资源解读已有新工具解除条件，但应另批固定有限资源/调用链目标，不能把本批静态成功包装为实施完成。其余三项只有取得上表缺失合同/后端后再重派，避免重复旧报告。总进度由根线按96分母汇总，本线本批增量0。
