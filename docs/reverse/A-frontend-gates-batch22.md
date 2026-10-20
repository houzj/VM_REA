# 第22批 A06.1 / A06.2 固定门槛复核

基线 `403d8e4af09dca4ebc9bc4f971b1b3cb41cb4c40`。本批没有可完整关闭的实施单元，**实施分子增量0**。没有修改共享产品、构建或运行厂商控件/SDK；tool-plugins 未动。

上一批只确认 editfrontendwindow 外层资源的168条字节记录可读，不能证明内部 FrontendEdit 完整布局可读或可用。本批对已保全的真正 `frontendedit.baml.resource-data.bin` 作一次有限离线核验，得到新的精确阻塞，而非重复原结论：

- 输入：`analysis/full-audit/framework-retirement-20261019/framework-shell/analysis/full-audit/frontend-ui-inspect/resources/FrontendUI.Design.g.resources.frontendedit.baml.resource-data.bin`。
- SHA256：`1E9C1F7775CF05BA7635A45EC5F9BE1B712CAAE659D97F4FB8923546E7F7350C`。
- 复用 `tests/EntryClosureChecks20/ReadBaml.py`；解析在 **0x1929** 报 `Unsupported record 0`，未输出成功 JSON；失败原文保留在 `analysis/full-audit/batch22-20261020/A/parse-attempt.txt`。
- 这说明当前有界解析器不覆盖该编辑面记录/边界；尚不能分辨是之前的记录长度解释不适用还是新记录语义缺失。**不据此声称原资源损坏、不补造该偏移后的节点、不将之前部分读取作为成功。** 未调用 XamlReader、schema、MarkupExtension 或厂商程序集执行。

A06.2 的完整门槛仍要求实际编辑面、模板资源及绑定/事件全部解读并还原可编辑消费者。本次解析失败使“第20批工具可直接读取全部 Frontend 资源”的假设被否证；修复应先用原 Baml2006Reader CIL 核对失败前记录的长度规则，再建立纯字节边界探针。不能为得分删掉未知部分或只生成外层窗口。

A06.1 原入口本身已由原 MainWindow→EditFrontendWindow(DisplayMode=1, Owner, ShowDialog) 证实；关闭仍走 FrontendEdit.Close→DoSave→真实快照 Save→temp/solution/designer 缓冲 MD5→保存/拒绝/取消及成功事件解绑。当前 main 仅有离线 FrontendDesignMigration XML 转换，没有上述真实设计面和状态消费者；单独增加入口/忙标志/确认框不能完成此固定单元。第13批的原 CIL 及关闭依赖报告仍是具体依据，本批未伪造新运行证据。

冻结点：一次编辑面离线解析失败后即停止，不扩大资源扫描、不循环重试。下批仅在明确承接记录长度修复和可用设计文档快照消费者时重开；否则应改选其他可实施单元。审计目录保留基线、命令、输入哈希、失败和输出清单，实施完成数由根线统一汇总。

复验命令：

```powershell
& 'C:/Users/houzhongjie/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe' tests/EntryClosureChecks20/ReadBaml.py 'analysis/full-audit/framework-retirement-20261019/framework-shell/analysis/full-audit/frontend-ui-inspect/resources/FrontendUI.Design.g.resources.frontendedit.baml.resource-data.bin' 'analysis/full-audit/batch22-20261020/A/frontendedit-records.json'
```
