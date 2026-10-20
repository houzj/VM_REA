# C2：畸变 IRLS、median 排序与负索引边界源码

2026-10-19；本轮有限S1/S2及有界离线补证。目标是解释原指令的循环、元素数和越界前提，不将原生历史内存写入公共安全核。没有新增原厂执行、设备测试、产品修改或宿主验收。

## 原件、范围与ABI

原core SHA256：`85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`。复用上一轮`positive-workspace/source/output/1807fdcff.c`（入口7fd4e0）及`1807ff184.c`（入口7fec40）、seq4/56/63/67/76/83的median快照和原厂回放结果。新原指令/常量/函数范围位于本worktree `analysis/distortion-calibration/c2-sort-source/output/`，哈希见`distortion-calibration-c2-sort-evidence.json`。

PE `.pdata`记载的runtime function范围分别为`[7fd4e0,7fec40)`和`[7fec40,7ff240)`。此前Ghidra把security-cookie helper当noreturn，未解码若干退出epilog；本轮在只读私有分析中恢复cookie后的原指令，并按这些范围输出冷分支/退出段。范围含padding，不代表每个字节执行。一次返回类型注解试验触发locked-storage空参数签名，已取消该原型改写；反编译签名不能替代下列调用点ABI。返回值以真实EAX写入为准：median失败`7feee6 XOR EAX,EAX`、成功`7ff1df MOV EAX,1`；IRLS一般失败`7febe9 XOR EAX,EAX`；median失败另经`7fdc03 MOV EAX,ESI`传回其0，正常epilog`7fe9f3 MOV EAX,EBX`。

调用关系：7ff3c0→7fd4e0→7fec40。7fd4e0入口RCX=context、EDX=n、R8=options、R9=H输出，entry `[RSP+28]`为阈值收敛标志输出指针。context+10/+18为成对float源/目标，+40为n个点误差，+48为2n个权重，+50为排序缓冲。`7fdb86..7fdbdf`调用点把RCX/RDX/R8D/R9设为源、目标、n、系数；median入口另有entry栈参数+28自由度、+30排序区、+38误差区、+40 sigma指针。这与已有entry捕获一致，不能从反编译函数参数被省略推断无参。

## 7fec40逐操作分支表

| 操作 | 原指令位置 | 精确读取/写入与条件 | 证据状态 |
|---|---|---|---|
| 系数映射 | 7fece0..7fed88 | freedom1取8系数；2取6；3由4系数构相似形式，其反号项由XOR signmask生成；固定H33=1 | 源码/原指令确认 |
| 逆矩阵门槛 | 7fee92..7fef31 | float行列式/伴随逆；abs(det)转换double后与`2^-52`比较。有限有序下abs(det)<门槛返回0；相等可继续。NaN的unordered flags不能用普通数学不等式代替 | 原指令确认；非有限运行未覆盖 |
| 每点误差 | 7ff022..7ff0b1 | i=0..n-1，目标经H逆投影后减源；float平方、相加、SQRTSS，写一个欧氏误差`r[i]` | 源码/原指令确认 |
| 成对复制 | 7ff0b7..7ff120 | 正n时pairCount=floor(n/2)，每次复制误差2k和2k+1，共n或n-1项。仅当0>r时翻符号；有序非负值直接保存 | 源码/原指令确认 |
| 奇数尾项 | 7ff124..7ff14f | n奇数时复制r[n-1]；偶数跳过。n=1只有此尾项，无pair循环 | 源码确认；n=1仅独立分支见证，非完整SDK运行 |
| 外循环起点 | 7ff156..7ff164 | EDX=0；n>1才进入。当前key=s[i+1]、previous=s[i]，i依次0..n-2 | 原指令确认 |
| 是否插入 | 7ff171..7ff179 | COMISS previous,key；JBE跳过，故只对ordered previous>key插入。MOVSXD RCX,EDX不改flags，紧随的第二个JBE复用同一flags，未形成任何下界检查 | 原指令确认 |
| 移位/负索引 | 7ff17f..7ff197 | s[j+1]=s[j]；DEC j；立刻读s[j]；仅在ordered s[j]>key时继续。不含j≥0或其他数组边界谓词 | 原指令确认 |
| 插回 | 7ff199 | 首个不大于key（或unordered）的值处，写s[j+1]=key；j可以小于-1 | 原指令确认 |
| 下一key | 7ff19f..7ff1a5 | ++i，与n-1比较，i<n-1才继续 | 原指令确认 |
| 中位数/sigma | 7ff1a7..7ff1c3 | 读取s[floor(n/2)]，乘float `0x3fbdc52c`（1.4825797080993652）。偶数n取上中位，不读取下中位、不平均；奇数取唯一中位 | 源码/原指令确认 |
| 出口 | 7feede..7fef29；7ff1d7..7ff22c | 失败EAX=0、成功EAX=1；恢复寄存器后RET，不能把cookie标注造成的缺段当作无返回值 | 新补原指令确认 |

有限有序输入下的等价控制流（这是源码整理，不是产品代码）：

```text
复制 n 个 r 的非负形式到 s；
for i = 0 .. n-2:
    key = s[i+1]
    if s[i] <= key: continue
    j = i
    do:
        s[j+1] = s[j]
        j = j-1
    while s[j] > key       // 此处先读，且没有 j >= 0
    s[j+1] = key
sigma = float(s[n/2] * 1.4825797080993652f)
```

## 读写median[-1]的必要区分

key依次小于所有左侧被扫描项，移位经过s[0]后就会先读s[-1]。**即使prefix为零或负值，也可能已发生这次负索引读**；它只是对有限非负key停止继续移位，不能称没有越界访问。

若该时刻s[-1]>key，则继续把它搬到s[0]，再读取s[-2]；在可读内存内，无论下一次停止还是继续，随后都会产生s[-1]写。若更深前缀仍大于key，读写可以继续向前。代码没有由n给出的通用负索引下界；捕获`2n+32`个prefix只是已有fixture边界，不能升级为原厂容器尺寸或通用安全保证。

s[-1]为正并非必然改变每次输出：还需实际key/移位路径满足比较条件。C1负前缀案例结果不变与本条一致。非有限值还受COMISS flags及MXCSR控制；本轮只记录指令，不声称覆盖NaN/Inf/异常mask的运行行为。

## 7fd4e0：n、2n与权重/迭代

`n`始终是点数；每点只产生一个欧氏r。设计矩阵有X/Y两行，因此权重数组是**2n个float**。freedom1/2/3分别用8/6/4个系数，不能把系数数或矩阵行数当作排序数量。

令`eps=0x34000000=2^-23`（float FLT_EPSILON），`T=float(options.Factor * sigma)`。Factor在options+14按传入float使用；上游UI整数到Factor的缩放属于C3，不能在此擅自再除10。下表公式只写有限有序输入，且每个float运算保持原顺序。

| 分支 | 原指令/字段 | 实际行为 |
|---|---|---|
| 迭代入口 | options+0c；7fdb25..7fdbdf | limit>0才进入IRLS；每轮先复制旧系数，再调用median，要求返回1，否则返回失败。不因零残差跳过首轮 |
| WeightFun1 | options+8=1；7fddb4及对齐向量循环 | 填满2n个1f，与sigma/T无关。不是n项，也不填reserve padding |
| WeightFun2 | 7fdc58..7fdd02 | 每点e=float(r+eps)。若−T≤e≤T，w=1；否则w=float(T/abs(e))。7fdcf5/7fdcfb把相同w写weights[2i]和[2i+1] |
| WeightFun3 | 7fdd13..7fdda3 | 每点q=float(1−float(r*r)/float(T*T+eps))，再q*q；r在[−T,T]内取该q平方，范围外取eps（不是0）；7fd944将0x34000000装入XMM8，7fdd85用XMM8作为范围外值，同样成对写两项。eps加在T²后，不能与Weight2的r+eps互换 |
| 其它weight值 | 7fdd13比较不等3→7fde61 | 当前方法没有对应权重填充分支；不能猜成常权/全零或直接推SDK支持。畸变上游WeightFun4另走832830，WeightFun1走8345e0，不得拿本方法分支代替工具分派 |
| 权重消费 | 7fde63之后、730be0/8580e0调用 | 每点X/Y行使用同一权重；随后进入加权正规方程。此次只确认n/2n及权重数据流，不宣称已审计所有矩阵乘加顺序 |
| 线性求解返回 | 7fe4c4..7fe4d7 | 原8580e0返回0或0x1000001d可继续；其它返回走失败。对该码不凭名称猜数值语义 |
| 收敛门槛 | 7fe510..7fe59c | 用float按顺序累加各系数delta²、SQRTSS，与options+10严格比较；norm<threshold才置输出标志1。相等不提前停止 |
| 轮数耗尽 | 7fe5a2..7fe5b3 | 计数加1后与options+0c比较，达到上限转结果构造。该输出标志初值0、只在delta停止时置1；0不等于方法失败，返回值与它独立 |

零sigma也不是统一捷径：有限r=0、T=0时Weight2先得e=eps，故w=0；Weight3的分母是eps，r=0时w=1。这是分支公式的结果，不据此跳过后续正规方程或伪造成功。

## 既有快照的离线补证

新`c2-sort-audit.cpp`只读取现有快照，在带显式边界的vector中按上述原指令顺序复制/排序；**不加载原DLL**，不创建SDK对象，不提供生产workspace。读取越过已捕获prefix会显式失败。负索引统计是离线模型轨迹，不冒充本轮硬件观测。

6个原始case×零/捕获context共12个sigma逐float位相同；捕获context的493个排序结果float与既有原厂输出逐位相同。4个独立分支见证检查n=1、奇数、偶数上中位及正prefix负写，它们不是新原厂运行。

| seq | n / median索引 | 零context最低读/写 | 捕获context最低读/写 | 捕获sigma |
|---|---|---|---|---|
| 4 | 99 / 49 | −1 / 无负写 | −1 / 无负写 | .00999284443 |
| 56 | 77 / 38 | −1 / 无负写 | −98 / −97 | 1.48257971 |
| 63 | 80 / 40 | −1 / 无负写 | −33 / −32 | .00104970054 |
| 67 | 80 / 40 | −1 / 无负写 | −30 / −29 | 3.61378813 |
| 76 | 77 / 38 | −1 / 无负写 | −97 / −96 | 1.48257971 |
| 83 | 80 / 40 | −1 / 无负写 | −33 / −32 | .00106894167 |

结合C1布局，在这6个captured fixture里最低访问仍处于当次2n权重实际写区之后的reserve padding；这只解释这些样本，没有证明其他输入不会触及更多子块。先前seq46写weights→后seq56 prefix、seq63排序写prefix→后seq67 prefix的原硬件证据保留，其生命周期来源不因本轮离线回放而改变。

验证命令：`RunC2SortSource.ps1`取得缺失体/原指令；`RunC2SortAudit.ps1`运行离线模型；`ExportC2SortEvidence.ps1`严格检查binary/常量/指令锚点、12 sigma、493字段和4分支见证并导出索引。不要为了只复核索引重新导入原生项目；已有产物可直接执行导出检查。

## 完成状态与剩余项

本有限C2切片完成了源循环、n/2n、奇偶复制/中位索引、负索引前提和三种权重的分支还原；补证仅复用既有原厂结果并增加安全离线轨迹。没有实现原生越界，没有给安全核硬编码prefix，没有声称任意输入或真实宿主等价。

仍开放：非有限/FP异常的实际执行、非法n/非法枚举的外部调用合同、所有权重上游分派与模型归一化完整数据流（C3）、所有矩阵乘加顺序/病态退化，以及C1 Init异常/尺寸helper和真实宿主池并发。直接调用7fec40的n≤0没有入口参数拒绝可据；离线模型主动拒绝该范围是独立分析边界，不能当作原厂行为。


