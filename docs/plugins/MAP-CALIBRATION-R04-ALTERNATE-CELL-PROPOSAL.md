# R04 首个正常 crop 分叉：6/8相邻cell入口

主线原真实生产回归在board6/index0抛prefix Unsupported，日志 analysis/calibration-board/r04-final-9efa7e501bad4310b05a64ffb5aa5305/encoded-axis-runtime.log。不能用7/9双图或Map9ROI通过覆盖此正常活动分支。当前未修改共享产品。

## 实际诊断

R04PrefixChecks只ProjectReference生产，按原fixture相同WPF Gray8读取/裁剪/30..140检测。首次遗漏System.IO导致构建失败、第一次运行JSON大小写属性冲突，均保留日志；修正后build/run0、0warnings/0errors，first-case.json保存完整cells。

首例6/index0原PNG，bbox[300,300,1400,1400]，格网13×13，min(1,1)，169点全部占用。radius2(row2,col1)已知missing=0、未知=1、合法跨行4，阈值17。无论未知pair是否含INT_MIN，总missing都在[0,1]，必然跳过。后续负列都是上一行末cell并已知有效。该满格网的缺槽搜索不会产生任何opening，因此修prefix后仍不能识别；根因还包括把7/9入口误用于6/8，必须单独修。

## prefix 修正与证明

D2已证rowPtr[y]=B+y*w*8，无逐行padding。linear=y*w+x处于[0,Cells.Count)时就是原合法dense cell，即使x为-1；只有负linear才未知B前段。每个未知pair的原OR-of-two-INT_MIN谓词仅贡献0或1。knownMissing>=threshold时全部可能prefix都decode；knownMissing+unknownCount<threshold时全部都skip；仅区间跨阈值才保留Unsupported。没有指定未知内容、解引用未知地址或模拟heap。

r04-prefix-interval.patch SHA256 18fcc8f4b90bde19c8f808cf2961d16de6358768be8051fc5b52805935c2e4ea，apply-check通过。候选unit同时比较未知与显式占用/缺槽两种completion的完整proposal调用序；straddle16+1/17必须拒绝，且两completion会产生不同调用序。

## 6/8 实际源码入口

复用主线A1 core-live-93e5dec0a8ab4550843ccedc08eabf51/output/18080f9a0.c、18080cfc0.c及对应已取asm。原C hash分别8c7cc4837ce3675a4a815ec7a570fe90aedc9994156ce4644efa320faf630c27、4788fef3a0fd1e5ace2bf21f79cee0da7cb971c53b37fdda38e973006cd79604。我在主线告知复用位置前已经启动的两体私有只读定向导出已exit0，与上述原件逐字相同；没有全量导入或新样本。

两wrapper内联搜索row0..h-2、col0..w-2；检查TL/TR/BL/BR的首ID非INT_MIN，取dense原float XY，opening按TL/TR/BR/BL。正4循环再反4循环调用同80a7f0，首次status1停止。没有813ca0缺槽门，也没有本步7ff3c0 grid-ID→XY拟合。组合候选仅board6/8改为此入口，7/9保持原fit+缺槽搜索。

成功时原C循环已将row/col各加一再进入direction switch。化简后anchor d1/5=左上、d2/6=右上、d3/7=右下、d4/8=左下（均原min+格位置），不是再减一；后续payload解析和813020 mapper复用。新候选unit覆盖8向首成功/调用数、anchor、原XY首角和4点保留，仅是控制流单元，不冒充原真实8向识别。

r04-cell-proposal.patch SHA256 ffd99fc1f6c2a053478e82bc6e9281b3fb508e3cdadcc7864860c7f75a49d4e4，apply-check通过，仅两文件；不含prefix修正hunk，可独立审/apply。完整候选源在原proposals目录。unit已准备但尚未构建；生产原axis12与mask分支仍须主线统一应用后严格回归，不恢复legacyfallback。
