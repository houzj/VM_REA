> 后续更新：输入编辑、基础运行参数代理和Group独立运行见 [GROUP-INPUT-RUNTIME-SELFRUN.md](GROUP-INPUT-RUNTIME-SELFRUN.md)。本文“父流程运行”等限制描述的是本阶段交付时状态。

# Group 中断、累积输出与显示联动

2026-10-16，框架 main。当前完成的是下列类型和调用链的独立重建，不是整个原生 Group 已完成。全范围缺口见 [完整清单](GROUP-FULL-RECONSTRUCTION-CHECKLIST.md)。

## 原始证据

- `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Logic/IMVSGroup/IMVSGroupAlgorithmTab.xml` 及相邻 XML：EnableLoop、StartIndex、EndIndex、间隔0..10000、EnableBreak、DataType=int/float/string、六种比较、基准/目标订阅与目标常量。
- `analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/VmIODecorate.cs`：报告 mode0 累积、mode1 普通；图像输出勾选创建/取消关联 DynamicObject；删除输出删除关联显示并清理订阅。
- `DisplayObjectDecorate`、`VM.Core/VmIO.MergeIOLoopValue`、`VmSolution`：图像替换、primitive 数组按轮合并、执行批次隔离。源文件保留在本地 analysis/decompiled，未上传原始反编译程序集。
- 原生 `ModuleFrame.dll` SHA256：`45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b`。本轮通过隔离 REA/Ghidra 会话读取函数和汇编，未修改原厂文件/进程。

|函数/地址|已确认结论|
|---|---|
|CGroupNode::DoWork `0x1800a0930`|更新当前索引，先判断中断，再执行内部节点、处理输出、等待间隔、递增；index小于end|
|GetLoopRangeFromSubRecord `0x1800a53a0`|解析起止订阅|
|IsBreakLoop `0x1800a55b0` / 首轮依赖检查 `0x1800a6730`|内部节点结果在首次执行前跳过比较；前轮完成后读取；比较失败走未触发路径|
|整数分支 `0x1800a5cee..0x1800a5d1e`|原始 CMP/SETG/SETL/SETZ/SETNZ/SETGE/SETLE 为 int32 直接比较；不采用误导性的浮点反编译表达式|
|float/string 分支|float EQ 使用严格小于 FLT_EPSILON，NE 是精确不等；字符串按 UTF-8 字节比较|
|ProcessGroupOutputV44 `0x1800a7830`|mode1 清空后追加，mode0 保留再追加；图像总是替换|
|ProcessIntIOEx `0x180013450` / helper `0x18000fc50`|追加数值并更新集合数量|
|SetGroupIOReportMode `0x1800a2b50`|写入输出对象同一模式字段；上游入口 `0x18013fb70` → `0x180046330`|
|SaveModuleData `0x1800a36d0` / LoadModuleData `0x1800a3e70`|报告模式与Group数据持久化；不代表本项目已兼容原生 .sol|
|PrepareGroupRst `0x1800a4a90` / PrepareGroupOutputSubscribe `0x1800a74c0` / ReleaseSolMem `0x1800a7110`|准备订阅/结果与释放原生数组；未据此宣称原生所有资源生命周期等价|
|SetParam `0x1800a3200`|LoopStep 接受非零整数；本实现尚未加入该能力，未模拟负步长可能导致的异常循环|

证据目录 `analysis/full-audit/group-native-audit/` 保留 C/汇编/JSON；新证据包 `break-accum-evidence-bundle.json`。18个带 CGroupNode 日志名的入口只是已定位清单，不代表原生引擎全代码已恢复。原生私有 double 分支未冒充 XML 公开的三种比较。

## 实际接通的功能

1. Group 循环页开启中断，选择 int/float/string、六运算、订阅或目标常量、固定数组索引。类型改变清除不兼容引用。支持父级输入/变量/结果、内部节点结果/变量/输入、私有 LoopIndex，以及 Group 自身公共输出。
2. 每轮内部执行之前判断；内部节点和自身输出首轮无结果时跳过。之后读取上一轮结果或本次调用的累积缓冲。父级来源在调用时快照，不读取以前一次调用的缓存。
3. 输出页实际新增/改名/订阅/解除/删除、累积开关。int/float/string 标量和数组按轮及元素顺序展平；开关关闭输出最后一轮。公共端口基数跟随变化，不兼容下游连接会拒绝配置，而不是偷偷取第一个值。
4. 输出改名/删除传播到父级连接、公共接口、变量和中断引用；复制嵌套 Group 时重映射内部与自身引用；支持撤销和保存重开。
5. 显示页实际配置图像、Circle、Components；非图像显示可累积，图像始终最后一帧。图像输出的显示勾选建立真实关联显示，删除任一侧同步取消关系。自动复合输出显示尚不支持，明确拒绝。
6. 窗口真实 Run 按钮进入框架调用链，显示执行次数、中断索引、值数组与叠加图形。零轮、失败和新运行会清理旧预览。运行中关闭等待内部模块释放；主窗口关闭同时清理打开的 Group 配置会话。
7. 开关采用原始位图模板，输出行宽/控件顺序及加减图标对照原始资源；资产来源及哈希在 `group-ui-resources.json`。订阅树、数字控件及全部状态主题仍非完整原生控件。

## 独立实现政策及限制

- 本项目执行格式仍为独立 JSON，不是原生ABI/.sol兼容。私有整数比较限定 int32、float32，公共 primitive 数据仍使用现有 long/double 合同；非有限浮点明确拒绝。
- 子节点失败即停；失败时已完成轮的值仅作失败诊断，下游不会继续消费。原生所有状态码及部分失败/取消回调尚未完整还原。
- 每次调用独立缓冲，最多10000轮及1000000累积元素/显示帧；这是独立资源保护。每轮模块和上下文仍重建，尚非原生对象持续存活语义。
- 运行通过后台任务保持WPF可停止；当前 Group 窗口仍运行拥有它的父工作流，不是原生隔离 SelfRun，也没有原生连续配置运行闭环。
- 支持固定元素索引，未实现动态索引订阅；复合输出累积、完整输入编辑、运行参数代理编辑、LoopStep、完整回调/设备生命周期仍开放。

## 验证与图像来源

54项中断/累积专项检查；17项范围检查；282项实际WPF控件操作检查。另回归调用34、变量数据31、数组28、调试21、编辑器核心115。检查覆盖整数精度边界、UTF8排序、EQ/NE差异、首轮/前轮/自身输出、嵌套、失败、复制/改名/删除/撤销、重新运行隔离和关闭等待。

WPF 验证实际创建窗口、点击开关/选择器/按钮、等待调用结果，并检查实际图像和叠加图形；不是仅断言配置字段。`wpf-group-break-condition.png`、`wpf-group-accumulated-output.png`、`wpf-group-array-break.png` 来自 tests 的 RenderTargetBitmap 离屏渲染，已人工读取检查。图像为已有真实BMP样本；两个绿色圆由测试模块生成以核对显示累积，不是 VM 算法结果或 VM 截图。截图代码没有加入产品程序集。

这些验证建立独立实现的闭环，不建立真实 VM 运行差分通过或全部 Group 功能等价。