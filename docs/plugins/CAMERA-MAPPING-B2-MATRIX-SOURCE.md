# B2 MatrixToParam固定H的首分叉与最小修复建议

2026-10-19，先经S1/S2/S3回收源码和中间量，再经根线审查实施两个有限S4修复并完成S5；以下前半为取证和审查时的历史基线，最终结果见末节。原厂核心身份和程序内嵌SHA保持`85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`。专属原始源码/汇编/硬件断点结果位于`analysis/camera-mapping/b2-matrix-source-20261019/`，机器索引见[证据记录](CAMERA-MAPPING-B2-MATRIX-SOURCE.json)。

## 实际函数链

原厂MatrixToParam wrapper Process@1be0 → MVD_Algorithm_Process@152b0，id0x6b → core dispatcher1c4e0 → 20480 → 7c6ba0(op0x12) → 7f5ca0 → 7efd00 → **800af0**。本轮复用已有核心反编译并以只读Ghidra CLI重取20480/7f5ca0/7efd00/800af0函数、引用和汇编；没有共享REA活动会话切换。前文“MatrixToParam内部opaque”的证据边界由这条具体函数链更新，不能继续把800af0当作未取得。

7efd00检查input/output指针、input[8]绝对值门槛后调用800af0；失败分别传播0x10000014/0x10000019，800af0非1则转0x1000010f。800af0以float系数计算行列式和逆矩阵，使用double epsilon保护行列式，以float epsilon保护逆矩阵[8]。其后读取逆矩阵[8]，**先单独float计算倒数，再对前8个逆矩阵元素做float乘法**，再调用atan2f/cosf/sinf形成六个标量。第9项不进入六标量计算。

原指令锚点：

- `180800de6 DIVSS XMM6,XMM7`：常量1除以逆矩阵[8]。
- `180800e55 MULPS XMM1,XMM6`，`180800e58 MULPS XMM9,XMM6`：前8项乘同一个已舍入float倒数；`e5c/e64`写回栈矩阵。
- `180800e91 CALL 18157243c`：atan2f，x64参数XMM0=归一化inverse[3]，XMM1=inverse[0]。
- `180800ea6 CALL 181572430`：cosf；`ec5 DIVSS`得到scale。
- `180800ed9 CALL 18157243c`：第二次atan2f并减rotate，按源码±π/2π边界处理skew。
- `180800f30 CALL 181572436`：sinf；其后构造aspect的float乘加/除法。
- `180800f65/f68`写TranslateX/Y，从归一化结果位拷贝；没有三角函数运算介入偏移值。

18157243c/430/436是原厂导入跳板（分别atan2f/cosf/sinf），现有反编译文本的自递归外观是间接import尾跳显示，不是算法递归。全域CRT与MathF位等价未由两样本证明。

## 定点原厂运行与第一分叉

仅选B1后保留偏移差异的两个既有实际CameraMap输出H：最大差`seed-33-extent-1000-grid-4`和`seed-15-extent-10000-grid-4`。由正常SDK创建IMatrixToParamTool，Import同9float、Run，并用线程局部硬件执行断点捕获800af0入口、800e0b（原始逆矩阵已完整存栈且倒数已计算）、800e6d（前8项归一化完成）、800e91（首个atan2调用参数）。只操作本探针线程，不改原DLL字节/算法状态或设备。

两样本**原始逆矩阵9项与当前产品Inverse全部float位相同**；到归一化才分叉：

| 样本 | 原始inverse[2] | 原始inverse[8] | 原厂float倒数 | 原厂TranslateX | 当前逐元素除法 |
|---|---:|---:|---:|---:|---:|
| seed33 extent1000 grid4 | -4115133 | 6.866306304931641 | 0.14563870429992676 | -599322.625 | -599322.6875 |
| seed15 extent10000 grid4 | 93895.2578125 | 0.823679506778717 | 1.2140644788742065 | 113994.8984375 | 113994.890625 |

保留当前产品逆矩阵和后续MathF标量表达式，只把独立诊断的归一化改为原厂倒数乘法：两样本归一化前8项全部位同原厂、六个最终字段也全部位同。此处不是依据“差一个ULP”猜算法，而是输入、归一化前、倒数、归一化后、输出的对应实际数值和原指令共同确定第一分叉。

## 提交根线的最小方案与边界

建议仅将CameraMappingPlugin.Runtime当前`float normalization=inv[8]; (float)v/normalization`改为`float reciprocal=1f/(float)inv[8]; (float)v*reciprocal`，保留逆矩阵和三角表达式。已证明的偏移差异不需要引入新分解公式或改公共求解器，也不应放宽0.003。

原厂指针/行列式/input[8]/inverse[8]检查已分别索引，但模块对所有极端错误码的映射仍须逐分支补证；本最小方案不夹带未知错误生命周期调整。两样本之外全域位同尚未验证。经根线审查后再实施并重跑同24例实际生产Runtime、118例和95例直接矩阵诊断；失败保留。真实宿主/Graph/原厂桌面/设备本轮未运行。

复现：`Run-B2MatrixTrace.ps1`执行两例原厂hardware采集；PowerShell7执行`B2ProjectionSnapshot.ps1`生成归一化前后逐位比较。原始结果`results.jsonl`，独立对照`normalization-only-diagnostic.json`。本轮未提交推送。

## 归一化修复后剩3例：逆矩阵原指令次序

经根线批准已仅将Runtime归一化改成float倒数乘法；实际24例仍3例失败，216字段阈值不变，exit1。剩余逐字段位证为：seed15/extent1000/grid4 TranslateX 原厂114024.1328125 (`47DEB411`)，产品114024.140625 (`47DEB412`)；seed16/extent10/grid4 TranslateY 原厂-151163.875 (`C8139EF8`)，产品-151163.859375 (`C8139EF7`)；seed33/extent1000/grid2 TranslateY 原厂-34481.52734375 (`C706B187`)，产品-34481.5234375 (`C706B186`)。原始表`three-residual-field-bits.json`保留，不将其四舍五入隐藏。

继续在最大残差例seed16/extent10/grid4补取原厂800c2e硬件断点（行列式已算完）：真实float行列式8.189539909362793，而现通用Inverse三因子左结合得到约8.18954086303711。此时原始9项逆矩阵全部不同，早于归一化和trig调用，不能用三角函数差异解释。

实际汇编800b92/b9b/ba0/bc6/bd7/bdb先计算并保存 `p04=m0*m4`、`p15=m1*m5`、`p05=m0*m5`、`p13=m1*m3`、`p23=m2*m3`、`p42=m4*m2`；随后按float顺序 `(((m8*p04+m6*p15+m7*p23)-m7*p05)-m8*p13)-m6*p42`。通用Inverse的三个因子逐项相乘不是同一指令顺序。独立源指令诊断使用这些hoisted产品后，最大残差例的真实行列式、原始逆矩阵9项和六最终字段全部位同。

根线已审查批准新增**仅MatrixToParam投影**使用的专属inverse，按800af0次序保留float运算及原厂double/float epsilon保护；通用CameraMappingLinearAlgebra.Inverse和B1求解器保持不变。7efd00输入检查与模块错误映射仍单列，不夹带新生命周期行为。先记录本段源码/位证，再落最小修复；后续实际验证追加，不预报全过。

## 最终实施、验证与仍开放的边界（S4/S5）

两项均经根线源码审查批准：Runtime改float倒数乘法；新增`CameraMappingMatrixToParamInverse`按800af0的hoisted乘积/行列式/余子式顺序，只由Runtime投影消费。保护常量通过只读PE取数再次确认：162b7c0为double epsilon、162b804为float epsilon、162b808为1，±π/2π常量也保留原始字节于constants.json。保留现有三角表达式及模块错误码边界，不修改通用Inverse、B1求解或公共NPointSolver。

本轮追加的原厂定点共5个，都是既有24点集的实际H，覆盖最初两个归一化差例和剩余三个inverse次序差例。每例原厂hardware的逆矩阵9项、六个最终字段与指令顺序诊断均float位同。这是有限分支证据，不是全域位等价声明。

最终实际生产Runtime回放24/24 Success、216字段全部满足固定`<0.003`，0例失败，命令exit0。归一化单修后的3例exit1历史保存在`actual-plugin-after-reciprocal-only.json`和`three-residual-field-bits.json`，没有删除失败。专属配置/WPF/runtime142检查再次PASS，含已成功运行后的显式导出/失败帧及奇异矩阵边界；测试窗口属于自动检查，不代表新的用户手动验收。

既有118组SDK求解差分为118匹配/0差异；专属inverse后来没有改变其Solver/LinearAlgebra依赖，因此未无谓重复。95个既有直接矩阵中，93个原厂成功样本通过“当前产品专属inverse源码＋原有标量表达式”的隔离诊断，全部满足原固定0.003，85例六字段float位同；2个原厂异常作为原始记录保留，诊断不冒充SDK异常实现。95组不是实际宿主/Runtime直接输入路径，24组实际Runtime结果独立记录。

最终源码SHA256：Plugin `F85DDFFC5258D96F86A4A833E521B012235F15DF8EC75D733BF3B5B1E8A21445`，新投影Inverse `441445A9A31A494CEAFD1230FA554C9414AF432D78A89EA971D53267775010EC`。B1 Solver仍`B108B09E7BDF10A5318DCC76C8FF0B5EF99AFF6E833E96B9A1DC30CCDACA0E4C`，通用LinearAlgebra仍`A02B1F61C1A49D9E448B44E77D23E478FE00381CF4F0CE122E7C323BD5F091AD`，公共NPointSolver仍`F39DE6E04388B6BC9AA1B0C4B1B97B65E5C9B174380C7F8BCAA767A1C2A624CC`。共享构建时段已释放，未提交推送。

继续开放：直接SDK特殊输入错误到原厂模块的完整映射、原生订阅索引、reset对缓存/最后帧影响、真实宿主/Graph及原厂桌面同状态验证。测试通过不代表这些分支闭合或完整原厂等价。
