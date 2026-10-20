# 灰度匹配点积backend源码及原生定点补证

2026-10-20。原核心 `MVD_Algorithm.dll` SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。当前1390正常训练/识别链中的c96130设置实际函数指针，未依据典型dot/NCC算法选核。

## 实际分派与累计

| 原条件 | 函数 | 原累计 |
|---|---|---|
| CPU class11且原整数rowMetric/stride>32 | c9ff70 | 每行int32乘积和，转signed double，按行ADDSD累计 |
| CPU7，或class11未超过32 | c9fa70 | 每行int32和，sign-extend到64，按uint64模累加，最后unsigned64→double |
| 其他class | ca0310 | 与上述uint64累计语义相同，普通指令实现 |

7参x64 ABI由C及明确跨度汇编确认：两个byte基址、两个int32偏移数组、int32长度数组、signed int行数、double*out。原输入指针/长度为正常有效域；函数返回void，probe exit0不是捏造原函数成功返回码。源码SIMD的byte零扩展→word乘积→int32分组加法可按mod2^32结合律重排，但**不能跨越每行signed转换及最终uint64/double累计边界**。

原.pdata首片不是整函数。已另保c9fa70..c9fd00、c9ff70..ca01d0、ca0310..ca0470明确跨度汇编。scalar结尾TEST总和、CVTSI2SD和unsigned高位分支；AVX结尾VCVTSI2SD行int与VADDSD逐行可直接核。索引 `analysis/gray-match/core-boundary/source-index.json` 当前27体，SHA256 `b9c4c3d54091d2b1810713dc167ce9c454ae1b06a77b1e7df99f48f386e6e2f7`，保分片限制，非完整源码数量进度。

## 自有进程的实际oracle

`tests/GrayMatch.NativeProbe/DotRowsProbe.cpp`只加载同hash核心并直调三纯算术叶；不CreateHandle、不Platform Init、不设备/授权操作。所有byte/int/double缓冲归probe自有、容量足够。先CPUID+OSXSAVE/XGETBV核AVX2可用，再调用AVX叶。10种长度0/1/7/8/31/32/33/33025/33026/33027，分别1/3行，全255字节，共20case/60个double位值。最终build0/run0，AVX2 guard=true。

首次行int32溢出在长度33026：

| case | scalar/SSE double bits | AVX double bits |
|---|---|---|
| 33026×1行 | 43effffffff00010 | c1dfffe0bf800000 |
| 33026×3行 | 43efffffffd0002f | c1f7ffe88fa00000 |

原始 `oracle-final.json`、逐核phase日志、原件/源码/显式asm/probe hash合并在 `analysis/gray-match/dot-native/manifest.json`，SHA256 `7fd88d5da6b3d0a7af66b4166262f884e0afd4be1156e809346ced33a97bfc97`。另独立Python按上述源码整数/浮点累计复演60/60相符；尚未冒称C#运行验证。

## 失败及生命周期边界保留

1. 首LoadLibraryEx error126，未进核，日志保留。
2. 改私有SetDllDirectory+LoadLibrary后旧probe无flush，精确自有PID25504/路径复核并经根线批准结束。其它进程未操作。
3. 加phase/flush和自有20s watchdog后，20个case对象全部完成并已写盘；末尾FreeLibrary进入独立核心卸载等待，watchdog自退124，缺结束括号的原JSON完整保留，没有修补成通过日志。
4. 最终在完整JSON/phase flush后按既有CodewordFailureProbe同法仅终止GetCurrentProcess(self)，exit0；不测试本纯叶不需要的核心卸载链。不能据此称原SDK整体释放已验证。

## 实施状态

根线审后已落 `src/VisionPlugins.GrayMatch/GrayMatchDotKernel.cs`，SHA256 `96F95B1B763DA640FB6782D64BEDB49B837161FF13130F94096A3E1F8690AE37`，逐源码实现两种累计及显式backend选择，坏span范围独立拒绝。`tests/GrayMatch.DotChecks`已改ProjectReference真实生产DLL，未Compile同名候选；独占串行build/run均exit0，0warning/0error，60原生位对照+4静态源分派边界+1安全检查共65项严格通过。源码before/after快照0漂移，实际DLL/PDB/log/原oracle及hash在 `analysis/gray-match/dot-managed/manifest.json`。

已生产限值leaf的13项与本oracle独立。完整工具仍缺训练/搜索编排、归一化/图像及模型布局、配置模型list/文件封装、显示及真实Host闭合，不把纯点积当灰度匹配完成。
