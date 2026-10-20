# 畸变标定 C3：gridScale、旋转网格与原点来源有限切片

2026-10-19。复用既有7ed0c0方法体/原指令，新增的原始字节仅为SDK常量180007db0；没有新原厂运行或产品修改。四邻域次序审查先完成：MODEL-POINTS报告已有扫描→离散处理块→续点的直接跳转表，不能按处理块地址排序。

## gridScale是独立输入，不是旋转角或检测边长

SDK Run2a70设置0x67输入+0c为DAT180007db0；直接读原PE的RVA7db0，file offset27568，位值3f800000=1f。binary SHA为AE4D64C5505E25E192EC39A5DF105DA16A2C69DE51D23F8E5B66C97A771FA120，原字节与PE映射元数据在analysis/distortion-calibration/c3-grid-source。

core1e330把该输入param2[3]复制到op0xf输入+24；7f7050将+24复制为options+10；7ed0c0原7ed301加载[RBX+10]到XMM11，随后将原网格G和重建网格Q的整数x/y分别转float再乘XMM11。正常SDK入口为1f，但不能把直接core调用的该字段硬编码为1。7ecea0要求它非负，未在此单独要求严格正。

7f7050另将输入+30的角度乘18162b614写options+8，它不是options+10。此前同一结构中角度、板类型、gridScale、模型、自由度位置相邻，不能因伪码复用float变量或堆栈别名混同。

## X邻接边扫描与分支

记原始整数标签G[i]=(gx,gy)，图像点P[i]。对i递增，内层只扫描j>i；遇首个|gx[j]-gx[i]|==1且gy相等即取该边并break内层。外层继续，所以方向角最后来自最后一个有匹配的i所取的首边，而非全局最短边。

距离用float sqrt(dx²+dy²)，X扫描累计max初0、min初float最大值；angleX=acos(abs(dx)/distance)，同时保留按标签x方向定向的dy符号。以float (max-min)/min与原常量181629a48=0.1f(bits3dcccccd)比较：严格大于0.1时，直接memcpy Q=G，不重定原点；等于阈值进入旋转重建分支。非有限/无邻接/零距离的实际合同不从正常数学推断。

## 旋转分支的Y边、角度与量化步长

继续按同样i/j顺序找首个|gy[j]-gy[i]|==1且gx相等的Y边；最后一个有匹配的i提供angleY=acos(abs(dx)/distance)及按标签y方向定向的dy。用angleY≤angleX的分支选Y角：定向dyY<0时+angleY，否则-angleY；若angleY>angleX则用X角：定向dyX≥0时-angleX，否则+angleX。由cosf/sinf对原P绕坐标原点旋转：Rx=x*cos-y*sin，Ry=x*sin+y*cos。

旋转后累计minRx/minRy用于平移。量化步长d来自最后匹配Y边的float距离；若没有Y匹配，沿用X扫描留下的距离值。它不是X距离min或max，也不是上节gridScale。缺少邻接或零步长的异常分支仍不宣称有效。

每点量化候选qx=trunc((Rx-minRx)/d±0.5f)、qy同理，符号非负加0.5、负减0.5；有限且可表示范围内等价于半点远离0取整。不能替换成默认banker's rounding。

## 原点选择及SIMD并列

原点o默认0，score初999999；候选score=|qx|+|qy|。标量尾段只在score严格更小时更新，等值保留先索引。四点SIMD前缀按lane保留严格最小score及索引，再归约最小score、在同score lane里归约最小输入索引；未胜lane使用INT_MAX，不是自动按lane序取最后。常规有限且score<999999时，前缀与标量尾段共同保留全局最先达到最小score的索引。若所有score均不小于初值，仍o=0，不扩大为任意溢出/非有限值也有相同保证。关键原指令7ed78a..7ed7f0、7ed876/7ed880及PCMPGTD/PCMPEQD/PANDN。

量化坐标只用于选原点；最终Q并不是直接保存这些qx/qy。随后再次遍历原标签的Y邻接，按该边旋转后abs(dRy)<abs(dRx)确定swap；每个i仍只取首个j，但外层继续，因此最后匹配决定swap。无匹配保留false；相等落false。

对每个i≠o，先取abs(Gx[i]-Gx[o])与abs(Gy[i]-Gy[o])。swap=false时分别作为Qx/Qy幅值，swap=true时交换幅值。符号由旋转位置与原点位置决定：Rx[i]≤Rx[o]取负，否则正；Ry同理。最后显式Q[o]=(0,0)。故原始标签差的绝对量级被保留，不能将此重建简化成量化网格输出。整数差/abs的溢出不在正常有限路径断言之内。

## 与后段选点和尺度的衔接

模型1/3的a/b在最终Q上按MODEL-POINTS控制流选择；a的中心距离仍用原图像P。随后U=Q*gridScale；模型1以P、模型3以逆径向P'估两点scale和平移。以上区分了输入gridScale、旋转量化步长d、两点图像/理想尺度ratio三个不同量；它们不能合并成一个“板间距”。

## 验证边界

ExportC3GridEvidence.ps1检查SDK原PE映射/1f常量、0.1f/0.5f、源与原指令锚点及文件哈希。不执行原厂DLL，不改产品/公共核，不宣称宿主等价。完整非有限/整数溢出、无邻接/零距离、量化异常值与全部失败路径仍开放；本切片只还原所列分支及其真实来源，不扩大到全部拟合数学。
