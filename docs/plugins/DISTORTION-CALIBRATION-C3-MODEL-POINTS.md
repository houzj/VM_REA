# 畸变标定 C3：模型1/3选点、尺度与中心初值有限切片

2026-10-19。只复用7ed0c0及其真实caller，定向补89ec80距离helper、853820径向初始化原指令和两块常量，不扩展全部拟合数学。没有新增原厂运行、产品变更或历史prefix。REA原生provider仍未配置，本切片复用私有只读Ghidra；REA资源证据仍引用C3-2，不冒称本次新增运行证据。

## 输入路径与两个中心

模块Process20550取得输入CorrectCenterPointX/Y后经basic-param vtable+20写入。X成功或缺失才继续读取Y；Y成功才调用setter，X缺失时其局部初值0仍可参与，Y缺失则本次不调用setter。因此缺失输入不能一律称作自动取图像中心，SDK basic-param构造3fe0将两坐标初置0，已设置值又可被后续调用保留。

SDK Run2a70从basic-param vtable+18取中心；原指令2f0e与2f29分别CVTTSS2SI两坐标，再CVTDQ2PS还原float，传进0x67输入+30/+34。对有限且int32可表示的值这是向零截断后float，不是保留小数或四舍五入；超界/非有限实际输入合同未在本轮验证。

core1e330把0x67输入+30/+34复制到op0xf输入+38（打包两float）；7f7050从param1[7]传入7ecea0/7ed0c0第四位置参数。7ed0c0用该两float比较检测图像点的平方距离来选a。图像点来自SDK检测/排序结果uStack208；整数网格Q在scratch param13中，由前面的网格方向/原点重建生成，不能把文件图像坐标当成Q。

径向853820是另一条中心来源：caller以7ed0c0第三位置参数（图像宽高int32）传为853820第四位置参数。853820将width/height分别转double乘原double0.5（181643f50 bits3fe0000000000000），写入14参数的index12/13。用户CorrectCenterPoint不是这里的径向中心初值。该中心后续可由LM活动掩码更新；本切片只确认初始化及来源，不重述全部优化阶段。

## a选择：严格更近，保留先索引

只在(model & 0xfffffffd)==1即模型1/3进入此段。a初始0，best初值float最大有限值（181629a44 bits7f7fffff），从i=0按输入顺序扫描：

`d=float(float(P[i].x-C.x)^2 + float(P[i].y-C.y)^2)`；仅d<best时更新a/best。原指令7eeb01 COMISS、7eeb0d JBE保证等值不更新。并列最近点保留先索引，不按网格坐标或后一个点打破并列。这里不调用sqrt，不将double距离或一般排序用于重建。

## b选择：方向顺序、首匹配与并列

b初始0，distanceBest同为最大有限float。以Q[a]=(gx,gy)依次搜索：

| 顺序 | 网格候选 | 搜索/更新 |
|---|---|---|
| 1 | (gx-1,gy) | 从索引0起，首个匹配即调用89ec80并终止该方向扫描；仅距离严格更小更新b和best |
| 2 | (gx+1,gy) | 同上 |
| 3 | (gx,gy-1) | 同上 |
| 4 | (gx,gy+1) | 同上；只需更新b，后面不再消费distanceBest |

89ec80确认为float差→两次MULSS→ADDSS→SQRTSS，算图像点P[a]/P[candidate]的欧氏距离；不是整数网格距离，也不是平方距离。前三方向原JBE跳过更新，第四方向COMISS后CMOVA，仅有序严格小才替换。等长时较早方向保留；同方向有重复网格标签，只评估最早索引，不会在同方向内再选更短的第二个。无四邻域匹配时保留b=0；本段没有额外“找不到邻居”错误分支。不能把b默认改为a或泛选最近非邻点。

原指令地址：左7eebe9/7eec09/7eec1a；右7eecf9/7eed19/7eed2a；上7eec4d/7eec6d/7eec7e；下7eecb1/7eecd1/7eece6。这里“上/下”指Q的y-1/y+1，不推断屏幕/板姿态的物理方位。

## 模型1与3尺度及平移

先将整数网格以当前gridScale转float缓冲U。模型1用P原始图像点，模型3先用853820/857a60得到逆径向后的P'；a/b索引来自前述原始P到用户中心的选择，模型3不会在逆径向后重新选a。

令L=sqrt(float((U[a].x-U[b].x)^2+(U[a].y-U[b].y)^2))。若L>EPS（181629a40 bits34000000=2^-23），取scale=sqrt(float((V[a].x-V[b].x)^2+(V[a].y-V[b].y)^2))/L；否则scale=1f（181629a58 bits3f800000）。V在模型1是P、模型3是P'。逐点目标为float((U[i]-U[a])*scale+V[a])，写回scale输出后调用所选拟合核。等于EPS落在默认1分支，不能改为≥；无邻居或重合点可能进入该默认分支，不据此新增错误。

这里确认了选定a/b之后的表达式。前面旋转/网格原点推断、gridScale来源的全部数值分支继续开放；没有把gridScale臆定为固定板间距。

## 径向H与系数初值澄清

853820自己把整数网格转换为float，并按options权重调用832830/8345e0/7ff3c0生成本地初始H；随后将其8个系数转double放入14参数向量。853820的param5实际为4个径向系数输出、param6为两坐标中心输出；它没有接收caller的auStack98 H缓冲。caller在模型2调用前将auStack98置单位阵，不能据此声称LM使用该单位阵作为实际初始H。C3-1的“单位H初始化后调用853820”只描述相邻caller写入，本函数初始H来自内部拟合。

原常量181643ed0/ed8/ee0/ee8初始化径向参数index8..11；原字节收录证据，四项为0/0/1/0。center index12/13为图像半宽/半高。freedom非1时还禁用H向量index6/7的活动标记；不以常识替代这些原始掩码。完整阶段掩码/采纳规则沿用先前径向报告，本轮不扩大覆盖。

## 验证与明确剩余

ExportC3ModelPointEvidence.ps1只做静态binary、常量、源码/原指令锚点及哈希检查；没有SDK运行、没有独立算法改动或宿主验收。原厂点序/网格来源、非有限/溢出、角度/原点推断全部分支、完整LM拟合数学、上下游并发和失败清理仍保持未闭合。原厂证据、源码推导和独立实现保持分开。


## 审查补记：控制流顺序与地址布局分开

方向次序仍为(-x,+x,-y,+y)，按处理块地址排序会错误地得到(-x,-y,+y,+x)。实际扫描主干依次在7eeb40生成gx-1→7eeb65生成gx+1→7eeb8c生成gy-1→7eebb0对gy加1；匹配时跳到离散处理块，处理完又跳回主干下一方向，完整边如下：

| 扫描字段 | 匹配跳转 | 距离处理块 | 更新/不更新后的共同续点 |
|---|---|---|---|
| gx-1且gy相等 | 7eeb54 JZ 7eebe9 | 7eebe9，call7eec09 | 7eec35/7eec48 JMP7eeb62（生成gx+1） |
| gx+1且gy相等 | 7eeb7b JZ7eecf9 | 7eecf9，call7eed19 | 7eed45/7eed58 JMP7eeb89（生成gy-1） |
| gx相等且gy-1 | 7eeba2 JZ7eec4d | 7eec4d，call7eec6d | 7eec99/7eecac JMP7eebb0（生成gy+1） |
| gx相等且gy+1 | 7eebc7 JZ7eecb1 | 7eecb1，call7eecd1 | 7eecf4 JMP7eebd7（选点结束） |

这些是原始直接跳转边，并非按反编译排版猜测。各扫描无匹配时同样落到相同续点，所以等长保留先方向的结论不变。
