# D1第四切片：局部边缘与当前圆拟合路径

2026-10-19。仅沿80ac20→870b90→6a3a70→70f4e0的实际配置追684b50和70d2e0；没有扩大模型集合、实图样本或产品实现。前定位vector/scalar零幅值差异仍按第三切片保持未知，不增加假设过滤。

## 身份、获取与索引

原件为`D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll`，SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。REA provider此前报告Ghidra未配置/Java缺失，未切换活动session或改全局配置。复用身份一致的私有Ghidra12.1.4工程，以readOnly/noanalysis导出；这些不是REA Evidence ID。

原始C/汇编、headless日志与索引在`analysis/map-calibration/d1-source-chain/`。`source-index.json`逐体保存C、汇编和cookie-tail哈希、调用/地址引用、源码行与缺体列表。当前91条入口记录包含10条拆块，不等于91个独立算法；18068a618是18068a2f0共用尾块。18068b8f0原工程没有函数定义，本轮只在readOnly session中定向反汇编/createFunction，导出名称D1_18068b8f0为本地注释，工程保存被丢弃。

本轮相对69条快照新增22条：6c9d90、704de0、68ad10、68ae00、68aec0、68a2f0、68b930、6911c0、691450、6b7c40、6b7c70、71fce0、71fe20、6c6cd0、68a618、68b8f0、6fc190、72d6b0、6b7ea0、6bfe30、693230、72d110（地址均前缀180）。其中另一轴68ad10/6911c0只获取不继续递归；当前模型不达的其他拟合模型也不展开。

## 当前caller配置与派生输入

70f4e0接收17个32位槽，I/F表示整数/float位解释：

`[I1,I2,I0,I2,F8,I1,I10,I50,F0,F2π,I30,F5,I1,I2,I20,I0,F5]`

证据为80ac20配置写入、870b90转发和70f4e0读槽；不能将所有槽统一强转float。slot2=0，外层只一轮；slot12=1选择70d2e0的model1，slot13=2选鲁棒策略2，maxiter20，trim0，cutoff5，nominalRadius30/tolerance20。

半径lo10、hi50，派生`a=max(lo−1,1)=9`；窗口中心半径`(a+hi)/2=29.5`。完整角域0..2π；采样数`min(30,int(.5+(a+hi)*2π))=30`，角步2π/30。径向长度50−9=41，切向长度`max(1,min(5,59*2π/30−1))=5`。684b50把尺寸补成偶数42×6。slot1=2使窗口方向再转π；第五切片进一步确认负半步起角与逐次float递减，具体分派须按 [旋转采样报告](MAP-CALIBRATION-D1-SAMPLER-SOURCE.md) 的谓词读取。后续边缘坐标以原图窗口中心和该窗口角还原。

边缘配置当前轴1、峰模式2、半核1、结果限1、强度门8。score flags=1，映射模式0，分段点0/0/255、增益0→1。70f4e0只收取局部检测status==1的点，原图界内条件再筛；因此最多30个拟合输入点，而非返回全部profile峰。

## 实际边缘链与共用节点

| 谓词 | 当前可达链 | 已确认/缺口 |
|---|---|---|
| CPU class7/10/11，axis1 | 684b50→6c9d90→6b7c40/6b7c70→71fce0→71fe20 | dims/sampler/profile/导数入口取得；底层旋转采样分派未全取得 |
| 其他CPU，axis1 | 684b50→704de0→68ae00→68aec0 | 普通采样主体、列均值和导数体已取 |
| 共用峰提取/选择 | 68a2f0→68a618→68b930→68b8f0→691450 | 当前模式2/flags1/限1已追；另一轴不展开 |

68ae00每列字节求和乘1/height，71fce0为对应优化列均值。半核1时两导数路径都选择3系数`[1,0,−1]`，以夹取端点进行相关运算，内部点为`P[i−1]−P[i+1]`。71fe20伪代码漏掉693230参数，汇编18071fe68..70明确ECX=3、RDX=系数scratch、R8=输出scratch。普通68aec0的switch后共用卷积亦使用相同系数；default系数生成中的*.5不属于当前3tap输出，不能误称普通核为反向半幅。两路采样与累计顺序未证明位等价。

模式2在j=1..len−2检查：`threshold−D[j]<eps`、`eps<D[j]−D[j−1]`、`D[j+1]−D[j]<eps`。node为28字节：type2、初分1、位置j−len/2、原始索引ushort；尾块记录profile值P[j]、导数D[j]。令`den=2D[j]−D[j+1]−D[j−1]`，abs(den)>.001时`delta=.5*(D[j+1]−D[j−1])/den`，否则0；位置加delta，峰强度为`D[j]+.25*(D[j+1]−D[j−1])*delta`。常数由PE原始字节确认，索引保留VA/file offset。

当前flags1只启用峰强度绝对值分数；模式0的0/0/255分段把强度映到0..1，除数含eps，达到上段饱和1。68b930 qsort回调68b8f0：分数差绝对值>eps按分数降序，否则按原始索引升序。691450最多保留1项，score<=eps停止；输出28字节包含x/y、偏移、type、score、profile值、导数值。坐标为center+(cos(angle),sin(angle))*offset，输出数量单独写入。

## 旋转采样的已知边界

6b7c70当前selector2、boundary2、fill0；中心越界→6bfe30→6c2d50（后者缺体），中心界内→6b7ea0。6b7ea0已取得，按窗口4角是否严格在界内、角度桶及CPU再分派；6bc000角点生成、6b53e0等底层核未全取得。仅拿到上层体不能宣布优化采样全闭合。

6b7ea0尾部对“角点全在内”的标志写0、否则写1，这不是通用的“1成功”。6c9d90仍继续构建profile，后续复用局部status存峰数量。不能用同名临时状态把采样裁边、峰数量与最终模型状态合并。

## 70d2e0当前model1/strategy2

输入为收集到的XY float点与配置`[I1,I2,I20,I0,F5,F30,F20]`。model1初始拟合用double累计N、Sx/Sy、Sxx/Syy/Sxy、Sxxx/Syyy/Sxxy/Sxyy。定义：

```
A=N*Sxx−Sx²; B=N*Syy−Sy²; C=N*Sxy−Sx*Sy
Dx=N*(Sxxx+Sxyy)−Sx*(Sxx+Syy)
Dy=N*(Syyy+Sxxy)−Sy*(Sxx+Syy)
det=A*B−C²
u=(B*Dx−C*Dy)/det; v=(A*Dy−C*Dx)/det
q=u²+v²−4*(Sx*u+Sy*v−Sxx−Syy)/N
center=(u/2,v/2); radius=sqrt(q)/2
```

abs(det)和q分别受DBL_EPS门。中心/半径输出再转float；初始拟合失败会清模型、RMS和残差掩码并返回0。其他模型2/3不据名称补实现。

strategy2用绝对径向残差`abs(sqrt((x−cx)²+(y−cy)²)−r)`；6fc190取中位数，奇数取中间、偶数取两中间均值。72d6b0复制到scratch再调用72d110选择第k个，后者小分区(<64)插入排序、大分区选择/分割；这些只改scratch。trim0表示选择上限FLT_MAX，不能读为“残差上限0”。实际接受残差严格<5；N>3时scale=median*2.96516+eps，否则DBL_MAX。残差<scale权1，否则scale/(residual+FLT_MIN)。

加权矩重新拟合需至少3接受点、权重和>=DBL_EPS，并重复det/q门。最多20次；RMS与相对变化的1e−5条件控制继续。更新成功status1；精化失败可回退初始模型并status2。最终有效点/RMS、半径与状态分开写回；不能宣称任一失败必然清所有输出，精化/最终失败存在保留模型的路径。失败回退中残差掩码与模型的写序已由 [第七切片](MAP-CALIBRATION-D1-FIT-OUTPUT-LIFECYCLE.md) 补核：模型恢复M0、mask保留最后轮、最终残差/RMS重算，未发现当前指针物理重叠。

当前nominal30/tolerance20产生10..50范围，70f4e0也检查半径。此局部模型门不能代替更上层80ac20的门。

## 修正：80ac20真正检查的是半径[15,21]

80ac20→870b90调用的stack+20是center(RSP+5f4)、+28是radius(5fc)、+30是RMS(f8)、+38诊断列表、+40点数、+48配置；870b90继续转发到6a3a70。汇编18080b4ee起从5fc取float，与PE181642ea4的15、21做COMISS/JA门。

故先前报告“质量值门”现明确修正为**拟合半径[15,21]门**。对普通有限值，半径通过后给center加局部象限偏移形成四点；这里未同时gate检测status/RMS。NaN的COMISS行为不能用通常数学区间替代，未执行NaN补证。6a3a70返回0表示参数接受，检测status、center/radius、RMS另传；不能把它们当同一种成功。

## 生命周期、严格缺口与下一步

70f4e0/70d2e0及当前峰链使用caller传入workspace、局部数组和输出指针；median另用scratch，qsort原位重排候选。所追路径内未见独立malloc/free，不能推广到未展开旋转叶体或所有模型。80ac20仍负责其外层workspace生命周期（见前两切片），失败需要按层读取输出，而非统一抹零。

| 精确缺口 | 已有边界 | 后续有限动作 |
|---|---|---|
| 优化旋转采样 | selector2/boundary2，6b7ea0角度桶/界内谓词 | 按实际角度/边界只补选中核；优先6bc000、6c2d50及6b53e0有关路径 |
| 采样精度/CPU差异 | 普通704de0与优化profile/导数体已有 | 先对齐叶体和取样坐标，需运行时只设计该疑点微探针 |
| 拟合失败别名 | model1/robust2更新/回退已见 | 汇编核对回退模型、残差mask、RMS的写序/别名 |
| 非有限/浮点环境 | COMISS与float/double混合已见 | 不推断NaN/ambient rounding完整语义 |
| 前定位零幅值 | 第三切片普通/vector差异保留 | 不猜补零过滤，不以当前边缘切片掩盖 |

验证仅为身份哈希、只读源码导出、索引/PE常数重建与源码/汇编交叉审查。没有新增泛化实图probe、托管构建、产品公共修改或宿主验收；S1/S2均为当前配置的有限切片，不表示八向识别或全算法等价。
