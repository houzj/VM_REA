# Group 整数中断比较的 float32 边界

## 原始证据

- `analysis/full-audit/group-native-audit/break.c`，`CGroupNode::IsBreakLoop`（0x1800a55b0）第 77–101 行：`DataType=1` 的两个输入经 `FUN_1800a5e50` 取得后，反编译结果先转 `float`。`CmpType=3/4` 直接比较两个 float；`1/2/5/6` 分别将 float 转回 `int` 后比较。
- `analysis/full-audit/group-native-audit/break-int-input.c`：读取的字段名是 `CmpSrcInt` 和 `CmpDstInt`。`break2.c` 的 `CLoopNode::IsBreakLoop` 同样显示整数操作数存为 float 后进行判断；这是交叉佐证，不能替代真实 VM 动态差分。
- `analysis/full-audit/group-native-audit/group-run.c` 第 314–319、534–539、1007–1008 行：LoopStep 以 int32 字段参与符号运算与索引推进。原生可能发生 int32 环绕；当前主框架用 long 推进、越界检测与 10000 轮上限，是明确的独立保护策略，本次未改。

这些 `.c` 文件是二进制分析器恢复的伪代码，不是厂商原始 C++；尤其极值的转换与状态码仍需真实 VM 差分。

取证时 SHA256：`break.c` 81d836cd4ee66f4b89587fda995bb27f0e755e4136f7973d22ea837daa29da7f，`break2.c` 93daa17e4bd42916c88d471af5ce85657083996c47778648e0e2c85feed8b076，`group-run.c` d2ff2dd4dd61a2ec42952b7e68d0b9f8390620daaf77dcbd97540d00923ccff0。

## 改动与复验

`GroupBreak.Compare` 的整数分支先检查输入在 int32 范围，再按原生转换为 float32。等于/不等于使用转换后的数值；大小关系使用转换后再取 int 的值。这样 `16777216` 与 `16777217` 在此比较中相等，`16777218` 仍可区分。此前实现直接比较 int32，误将高位精度当成原生行为。

`tests/VisionFramework.GroupBreakChecks/Program.cs` 新增直接比较与实际 Group 执行断言：相邻整数因 float32 舍入而触发首次中断，内部模块执行次数为零。复验结论仅限这组源确认的边界，不声称全部 Group 中断等价。

同一专项测试原有“Circle 累积必须拒绝”断言已过时：现行托管契约明确允许 Circle，且运行累积为强类型数组。测试改为验证当前独立能力的两轮结果；它不构成原生 Circle 累积的证据。

## 未确认

原生 int32 极值经过 float32 再转 int 的溢出细节、数值来源的隐式类型转换、运行状态码、真实 VM 同配置的逐结果对照仍开放。当前实现对源值超出 int32 的独立输入明确拒绝。
