# 标定板圆点低对比度边界追踪

## 结论范围

针对已采集的合成图，低对比度圆/椭圆失败发生在圆点候选提取阶段，早于 `FUN_180817c50` 的全分辨率窗口和 `FUN_1808609a0` 梯度/亚像素细化调用。此结论不外推到任意图像、照明条件、ROI或 SDK 未覆盖的参数。

探针 `CircleProbe.cpp` 构造 480×400 MONO_08 图，背景灰度255，8×10图案中心距30像素。这里的“contrast”是合成输入参数，图形内像素为 `255-contrast`，不是原厂 `GrayContrast` 运行参数。原厂模块资源把 `GrayContrast` 标为隐藏/废弃；不要混为同一个量。默认factor5时，低对比度样本在每个候选阶段先生成 96×80 的缩小图。

## 原厂源码链

公开SDK的圆点分支通过 `0x1807e33c0`（公开板型2→core kind3）进入 `0x1807dda50`，然后是 `0x180817c50`。初始提取链为 `0x180812930 → 0x180877f90 → 0x180877230`：

1. `0x180812930` 按factor构造缩小灰图；`0x180727a50 → 0x180727470`提供重采样。圆点候选坐标之后仍基于全分辨率图裁局部窗口。
2. `0x180877230`对缩小灰图调用 Otsu `0x180724460 → 0x180724880`；`0x180724740`按 `pixel < threshold` 写0，否则写255，接着 `0x180720200`反相，再由 `0x18066bdf0`建立连通区域。
3. `0x180877230`通过 `0x180759880`读取每个连通区域面积，首先只把面积 `>4` 的区域计入平均值；反向过滤时删除面积 `<5` 或大于该阶段计算的相对上限。故任何面积不超过4的低对比候选都在亚像素局部窗口前被删除。
4. 存活区域随后进行轮廓相对面积和 `0x180722bf0` 圆度筛选，再计算裁剪框：以缩小图区域的像素边界乘factor，减6作为局部起点，宽高为区域宽高乘factor加12，按原图边界夹紧。`0x180877b20`以边缘记录拟合中心，失败时才进入 `0x1808620c0` fallback。
5. `0x180817c50`中只有已通过候选筛选的区域才会在callsite `0x180818c1a`调用 `0x1808609a0`。该helper再生成梯度及幅值平面并交给 `0x18085fcd0`做亚像素峰值、强弱标签和连接。候选列表为空时没有局部灰图或梯度输入，也不会执行这条细化链。

## fixture逐像素复核

原厂 `calibration-board-preprocess-fixtures.json` 对三种形状的contrast2输入保留factor5输出图和Otsu二值图；现有 `PreprocessChecks` 将当前实现的缩小图、掩码和原厂fixture逐像素比较。依据该已核验96×80掩码重算8邻接连通域，可复现：

| 合成形状 | Otsu阈值 | `<阈值` 像素数 | 区域面积分布 | 原厂 `>4` 初筛存活数 |
|---|---:|---:|---|---:|
| 圆，半径9 | 254 | 320 | 80个区域，每个4像素 | 0 |
| 椭圆，9×5 | 254 | 240 | 80个区域，每个3像素 | 0 |
| 方形，边长约18 | 254 | 640 | 80个区域，每个8像素 | 80 |

因此SDK fixture中contrast2圆/椭圆对所有被测圆度及四组边缘阈值均返回 `0x10000107`，与候选记录数为0一致；同一contrast的方形保留80点并成功。此“区域面积分布”是由原厂预处理像素fixture与反编译区域过滤规则推导的阶段重建值，不是对SDK内部组件scratch缓冲的直接动态捕获。

方形control还说明阈值确会影响走到细化路径的输出：`EdgeThreshLow/High=0/1`的SDK首点为 `(89.9690857,79.9624481)`；`20/40`、`100/200`、`127/255`的首点均为 `(92,82)`。现有360组合候选fixture及 `CandidateChecks -AllCandidates`覆盖相应输入与首点差分。这个观测证明这些SDK setter输入与输出有关联，但并没有证明XML具名值具体写入 `param_10+0x28/+0x2c` 的哪一槽；映射仍需同实例setter→运行参数块trace。

## 实现判断和未知边界

当前 `CalibrationBoardCircleCandidates` 已按factor5缩图、Otsu、阈值二值化、连通域面积 `>4`、候选局部窗口和细化/回退分支运行；`CandidateChecks`以原厂初始候选fixture复核坐标、边缘拟合标志、尺寸、点数及SDK最终首点/失败状态。当前证据未显示本边界存在可安全落地的产品代码修正；因此本轮不更改产品算法。把contrast2圆/椭圆失败“修成”成功会违背当前原厂输入fixture。

尚未捕获的原厂运行字段包括：实际SDK对象传给 `FUN_1807dda50` 的ROI矩形和参数块；`0x180877230`各阶段的实测Otsu阈值、组件面积数组及被过滤ID；每个候选 `0x180818c1a`的局部crop原始字节、stride、强/弱阈值槽；`0x1808609a0`返回记录、工作缓冲和fallback选择。现有360行公共SDK日志只保留输入case、异常码或点数/首点；不能当作这些中间缓冲的动态trace。

如果后续要闭合square等非空低对比样本的阈值/亚像素路径，最低限度需要在原厂SDK进程对同一图像、同一实例记录：①调用 `EdgeThreshLow`、`EdgeThreshHigh` setter前后的参数块原始字节；② `0x1807dda50`入口的ROI四元组、factor和图像尺寸/stride；③ `0x180818c1a`入口的局部灰图、crop origin/dimensions及float阈值三元组；④`0x1808609a0`返回码、success标志、结果结构长度/字节、工作区；⑤ caller选择的contour ID、`0x180877b20`返回值与中心、fallback是否调用。对contrast2圆/椭圆只需trace到 `0x180877230`组件过滤后为空即可确认提前退出，无需伪造不存在的细化输入。

## 证据索引与验证

机器证据摘要：[circle-low-contrast-evidence.json](../../analysis/calibration-board/circle-low-contrast-evidence.json)。原厂模块/SDK哈希、反编译函数文件hash、fixture与探针hash均列在该JSON中；反编译符号为Ghidra地址命名，不是厂商原生符号。

验证使用 `tests/CalibrationBoard.Checks/Run.ps1 -AllCandidates`：其中 `PreprocessChecks`覆盖factor2/factor5预处理fixture逐像素比较，`CandidateChecks`覆盖360组合原厂候选和SDK最终结果差分。设备测试与原厂桌面验收不在此范围。专项全量结果应以本轮运行摘要中的 `sourceChangedDuringBuild`、`exitCode`和`result`为准；通过fixture只证明受测合成输入，不证明未采集图像全等。
