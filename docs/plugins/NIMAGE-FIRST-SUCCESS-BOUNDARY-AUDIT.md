# NImage 首成功前缀的有限托管诊断

2026-10-19，根线处理 CAL-SRC-002。原813ca0首成功即停已有D2源码；当前共享Recognize仍解全部quad并以所有成功opening过滤点。本次只回答：既有fixture中截掉首成功之后的proposal是否改变当前托管输出。

`tests/NImage.SourceRecognitionBoundaryChecks`读取现有source/target各1000×1000字节图，分别HIK7/HIK9、normal/reverse，共8组。两次调用同一未修改的RecognizeLattice：一套完整quad，另一套截至首有效identity的前缀；其余lattice完全相同。结果为实际托管执行，不是原厂执行，也不是源码级decoder恢复。

| 样本域 | 完整proposal数 | 首成功前缀数 | 点集合/identity |
|---|---:|---:|---|
| source/target × HIK7/HIK9 normal | 45 | 12 | 均168点，集合和首identity相同 |
| source/target × HIK7/HIK9 reverse | 45 | 10 | 均168点，集合和首identity相同 |

原始8行及added/removed点集在`analysis/nimage-boundary/first-success-prefix.json`；脚本、实际共享产品源、两输入及输出SHA在`evidence-hashes.json`。脚本保证新输出不覆盖旧证据；本轮未新增图像样本。构建最终0警告/0错误，运行退出0。最初沙箱内编译器写DLL被拒绝；重试暴露诊断遗漏System.IO引用，补齐后才构建成功。失败不记为通过，旧输出目录保留。

这说明当前fixture中后成功opening没有额外删除有效点，不证明任意图像相同，也不证明原厂proposal/decoder或八向实图。首成功前缀与全扫描的执行工作量不同；本次不据8组结果直接break产品、删除兼容掩码或声称CAL-SRC-002已完整实现。CAL-SRC-003的source-grid上下文仍是单独前置，安全负索引过滤不变。

产品程序集源码未改，现有验收程序未操作。当前D2 S3/S4/S5整阶段不因这次独立边界诊断自动加分。
