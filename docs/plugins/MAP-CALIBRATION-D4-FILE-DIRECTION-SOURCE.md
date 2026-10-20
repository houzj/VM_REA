# D4 文件矩阵方向有限源码切片（2026-10-19）

本切片补回此前缺失的原厂 `1807d4f20`，连接模块文件缓存、SDK 双文件接口、核心实际 caller 与乘法 ABI。只做源码/汇编审查，无产品修改、原厂执行或新增图像样本。D4 仍仅 S0 完整；本切片不单独增加阶段分，D 总 5/32，整卡 0/4。

## 原件与获取

原件 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll`，SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。先在当前 `analysis/map-calibration`、原项目 `D:/MyWork/REA逆向测试/analysis` 递归检索同名 C，均无 `1807d4f20.c`；没有用旧报告或托管实现冒充函数体。REA provider 不可用沿用已记录状态，采用身份匹配的私有 Ghidra 12.1.4/JDK21 工程、`-noanalysis -readOnly` 定向导出，未修改全局配置。无 REA Evidence ID。

新原始产物位于 `analysis/map-calibration/d4-file-direction/source`：`1807d4f20`、`180730be0`、实际 caller `1807fbec0` 的 C/asm/cookie tails；`file-direction-references.txt` 记录唯一直接 CALL `1807fbf35`，另一 DATA 引用不作为操作号分派证据。源码/汇编 SHA256、原 PE 常量字节由 `ExportD4FileDirectionIndex.py` 生成 `source-index.json`。复现入口 `RunD4FileDirectionExport.ps1`，仅使用现有私有工程。

## 从文件到原始矩阵实参

| 链路 | 具体源码确认 |
|---|---|
| 模块 `ReadFileMatrix`，module/1800227d0.c | `side==0` 写 `this+0x17360`；其他 side 写 `+0x17368`。XML 分支逐项写至多 9 个 float；二进制分支读文件原始字节，不在此处做转置或求逆。此处只核方向，完整文件失败/缓存生命周期另列。 |
| 模块 `ProcessAlg`，module-analyzed/180021c50.c:114–115 | 模式 2 取得双文件参数对象后，`virtual+0x10(object, this[0x17360])`、`virtual+0x20(object, this[0x17368])`，源码显式保留两个实参。 |
| 原 SDK `MapCalibCpp.h:137–174` | `ITwoFileMapBasicParam` 析构槽 0，GetSourceCalibFileData +8、SetSource +0x10、GetTarget +0x18、SetTarget +0x20。source/target 命名来自原头文件。 |
| SDK wrapper/180005860.c | 依次 GetSource(+8)、GetTarget(+0x18)，构造 16 字节 `[source pointer,target pointer]`；`MVD_Algorithm_Process(handle,0x6f,&input,&output)`，输出 44 字节先清 0；成功后复制 9 个矩阵 float 和 2 个比例 float 到结果对象。 |
| 核心 7fbec0 / asm 7fbee8–7fbf35 | 要求输入 size=0x10、输出 size=0x2c；`RCX=[input]`、`RDX=[input+8]`、`R8=output`，调用 7d4f20。因此实际输入第 1 指针是 source，第 2 指针是 target；不是按旧方向名称猜参数。 |

本切片没有重新恢复整个 `MVD_Algorithm_Process` 的 0x6f 内部分派表。wrapper 操作号和核心 caller 的各自 ABI 是本次原体直接证据；二者的操作注册/转发关联复用既有 SDK 文件模式证据，尚不能称本次静态体逐跳闭合。`SetParam(CalibPath*)` 某些反编译调用漏掉第 2 实参，本报告不使用该缺参表达式证明缓存绑定，改用 `ProcessAlg` 显式调用。

## 7d4f20 与 730be0 的实际运算

修正后的核心签名为 `uint32 core(float* source, float* target, float* out9, float* scaleX, float* scaleY, int* success)`；反编译 `void` 由 cookie 后汇编纠正：空参数返回 `0x10000014`，非空路径最终返回 0。`out9` 的 16 字节数组类型只是 SIMD 恢复类型，不代表矩阵元素间距16。

1. `7d5054` 起只从 RDX（target）读取 9 个行主序 float。行列式按原标量 float 顺序为 `(((t22*(t00*t11) + t20*(t01*t12) + t21*t02*t10) - t21*t00*t12) - t22*t01*t10) - t20*t11*t02`。不改为 double 行列式或重排公式。
2. 有限数分支要求 `abs(det)>=double epsilon`（PE 181628540=2.220446049250313e-16）。`7d514c COMISD epsilon,absDet; 7d5151 JBE inverse` 是实际谓词；unordered 也跳入 inverse，不能把 C 的有序 `<=` 外推为 NaN 拒绝策略。
3. `1.0f/det` 与伴随矩阵元素以 float 生成局部 inverse(target)。`7d5241` 先写 success=1；奇异分支 `7d5153` 只写 success=0，不写 out9、scaleX/Y，核心仍返回0。
4. `7d5247–7d529b` 传乘法 ABI：RCX=局部 inverse、EDX=3（左行步长）、R8D=3（内积长度）、R9=入口保留的 source；栈上传 source 行步长3、尺寸(3,3)、out9、输出行步长3、尺寸(3,3)。`730be0` 的地址索引是左矩阵行 × 右矩阵列，故原体确认 `H=inverse(target)*source`。其中内积先以 `0+a0*b0+a1*b1` 累计，再加第3项；本报告未用数学乘法可交换性代替该地址证据。
5. 原 PE `18162854c` 与 SIMD `181628530` 四槽均为 `1.1920928955078125e-7f`。输出9项除以 `oldH[8]+epsilon_float`，包括 h33 自身；使用 DIVPS/DIVSS，无另一次精确单位化。正常不重叠 out9 的地址谓词走 `7d52c3` 分支；另一 SIMD 常量分支数值常量相同，但不据此宣称所有别名输入安全。
6. scaleX=`sqrt(H[0]^2+H[3]^2)`，scaleY=`sqrt(H[1]^2+H[4]^2)`，均在归一化写回后读矩阵，标量 MULSS/ADDSS/SQRTSS。

## caller 的失败与输出

`7fbec0`：尺寸不符返回 `0x1000000e`；第5参数为空返回 `0x10000002`；两矩阵/输出为空返回 `0x10000014`（输入结构基址自身不额外判空，不能据此承诺任意空调用安全）。初始化两个比例临时值与 success 为0，调用核心；核心非0则透传，核心0但 success!=1 返回 `0x10103002`。只有 success==1 才将 scaleX/Y 写到 output+0x24/+0x28，返回0。矩阵内容由核心直接写到 output 前36字节。

这解释源零矩阵无需源行列式检查、目标奇异由外层状态转错的源码条件；没有新增原厂运行来扩展既有72例有限验收。SDK wrapper 预清 output，与直接 caller 输出缓存是否预清不同，不能把 caller 奇异路径说成主动清9项。

## 严格对照与剩余缺口

旧报告 `inverse(target)*source` 和 epsilon 结论由新原体独立确认；source/target 方向来自模块虚表、SDK头文件和实际 caller 的输入指针顺序。托管 `MapCalibrationSolver.SolveFiles/Compose` 仅作为后续产品对照位置，不是本切片证据来源。

仍开放：0x6f 注册/转发的整段静态桥、文件解析返回码与缓存替换全部分支、双ROI形状所有权/空ROI setter行为、模块复用、显示/释放完整链及真实宿主整卡验收。无需为本文件方向切片扩展所有生命周期，也不因本切片把 D4 S1/S2 标为完成。
