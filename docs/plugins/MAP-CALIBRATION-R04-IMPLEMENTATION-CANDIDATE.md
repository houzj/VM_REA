# R04 当前18/24路径实施候选与有限接入清单

2026-10-19。原core SHA256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。只读共享产品，新候选在 `analysis/map-calibration/proposals/r04-source-recognition/NImageSourceRecognitionBridge.cs`，尚未编译/上架。此候选实现caller控制流，不声称图像decoder已经源等价。

## 已准备的实施部分

候选从新 `NImageLattice.SourceGrid` 消费真实Width/Height/minima/Cells，而非从独立CodeQuadrilaterals推断锚点。按813ca0 radius1→2、row从radius、column始终1、跳中心、任一ID分量INT_MIN计缺槽，门槛float常量 `0.6665999889373779f`（同PE VA181642ef8）与±0.5后截断，得到6/17。按四正序再四反序调用decoder，严格首次状态1返回，保留radius/center/direction/Opening身份。

80e690/80bcd0实际param12成功写入为[col+radius,row+radius]；wrapper按方向1/5左上、2/6右上、3/7右下、4/8左下扣2radius得到原整数ID anchor，未调用Nearest。解码6byte原origin/pitch由现源确认Identity解析。813020按dense cell读取最后写入XY，首ID INT_MIN跳过；输出遍历1/5 row+col+，2/6 col−row+，3/7 row−col−，4/8 col+row−，再WorldIndex，不按world identity重新排序。无“所有成功opening逐个屏蔽”步骤。

源码复用：d2-padding-address/source/180813ca0.c/.asm及18080e690.c，ghidra/core/180813020.c与320例既有直接微核；当前候选未重复八向微核或新增实图。候选所有8方向只接受真实callback返回的成功，不从微核成功开放产品识别。

radius2/column1负列的原prefix值在SourceGrid合同中缺失。候选通过可选prefix只读供应具体ID；未供应时显式UnsupportedReason，不能默认为INT_MIN、0或安全跳过再宣称源搜索等价。这是有界托管安全选择。其他无实参、纯函数控制流可直接评审，不依赖新增公共合同。

## 当前共享产品与候选接口

| 位置 | 当前行为 | 源候选与精确剩余 |
|---|---|---|
| NImageBoardRecognition.Recognize | 独立quad→FindFiducials→TryDecode，过滤orientation!=0 | SourceGrid非null时进入候选；没有SourceGrid时保留明确命名legacy分支，不无声fallback |
| proposal投影 | NPointSolver以独立默认参数拟合，Nearest第一角 | wrapper 7ff3c0拟合实际整数ID→XY，800670每操作float投影；候选project回调，需绑定已证共享拟合入口配置，不能默认double同值 |
| opening producer | NImageFiducials independent ring | 回调必须实现80a7f0：80ac20以200×200规范角恢复opening；返回0走20×20及[4,4],[16,4],[12,16],[4,16]源fallback；否则按PE缩放常量；再7ff3c0映射实际proposal并86ff00。**不能直接传现TryDecode** |
| word decode | R03根线接入中 | 86ff00固定18/24接R03源word层，orientation只在外层813ca0，不在内部再遍历8次 |
| final points | mask所有解码tag、固定方向1、world排序 | 候选选中一个身份，dense traversal +实际direction/anchor，所有有效dense cell输出；使用后写XY，不按opening再删点 |
| results/tags | rings被当作已识别fiducials | 只使用实际80a7f0/86ff00输出记录中的points；候选Selection只承诺proposal Opening，不伪造Fiducials |

## 有限剩余直接窗口（不是继续扩全图像库）

1. **80a7f0 opening适配**：已有完整C，需按实际call site把80ac20的10参、7ff3c0尾部参数与返回状态标为可实施结构。目标固定200×200、4corner、当前model1/strategy2；不展开所有shape/base。缺口是ABI栈连接与现有源fit/feature实现的绑定，不是缺少整函数。已取得80ac20内实际870b90链，D1已有有限feature/fit源码报告可复用。
2. **86ff00 warp/preprocess绑定**：已有C/asm。直接窗口`8702xx..870357`完整12参8b1710与`870407..87045c`9参85c8a0；尺寸16*cell，窗口size/4、offset0、mode0。现有TryOrientation double homography+floor/ix+1与原float+floor/ceil公式不同；整数坐标ceil==floor带除0域不能按典型bilinear替换。已有8a5e30分派与large-kernel归一化源码，绑定当前路径即可，不扩CPU全库。
3. **来源/显示**：回调返回原record+f0/status、12payload、实际四个输出float points；Selection与最终输出同次调用关联。原函数参数无参伪签名不可当ABI；显示Fiducials不可拿独立ring填充声称源证据。

退出条件为上述两图像适配回调真实实现并接入，固定当前已有实图比较选中proposal/radius/direction/anchor/12payload及最终完整点序，保持R03失败词回归；合法没有proposal/decoder失败返回失败，安全prefix未知明确拒绝。人工验收独立，不阻挡已实施且自审通过的完整工具计数；当前尚未达到完整。

验证状态：未构建C#、未改共享、未执行新增native/图像probe；候选需根线审查并安排槽。它不是可无条件应用的全R04补丁，接口中的project/decode/prefix未绑定即不可替换产品入口。

## 已核实的直接调用窗口（本轮补齐）

`86ff00.asm 180870280..180870357`：RCX=四个reference float点（[2c,2c],[14c,2c],[10c,14c],[2c,14c]），RDX=原item四点，R8D=4，R9=source pixels；stack+20 source stride，+28 source packedW/H，+30 deststride=16c，+38 packed output W/H=16c，+40 floatH[9]，+48 warped buffer，+50 status int*，+58 solver scratch。对应8b1710的12参签名已足以移植当前入口，不再列作缺ABI。8b1710调用89faa0求输出→输入floatH，随后800670逐pixel float projection，floor/ceil四个角及逐项float除法，不是floor+1版本。

`180870407..18087045c`：85c8a0(sourceWarped,stride16c,binary,stride16c,packed16c,packedWindow(4c,4c),offset0,mode0,scratch)，window为两维同16c/4；source和binary分离。8a5e30以原CPU编号11/7/10/other分派，不能把当前double integral mean+clamp边缘称作源算法。

`80a7f0.asm 18080a88e..18080a931`：80ac20(source,stride,packedSourceDims,packed200x200,canonicalOuter4,proposal4,outInner4,param6,param7,param8)。param6/7/8是813ca0从wrapper透传的三workspace，具体容量沿既有D1workspace报告；不传主程序/平台Init对象。canonicalOuter是(0,0),(200,0),(200,200),(0,200)；返回0之后改20×20+默认inner，返回非0两套点均除10。该窗口无新缺ABI；剩余是把已恢复函数体译进共享核。

因此当前最小实施量不是补更多来源，而是两个有限适配体：A=80a7f0/80ac20当前窗口+fit/feature绑定，B=86ff00当前cell域+8b1710+85c8a0。桥候选可在A/B完成后直接接入；A/B未实现时仅桥不能通过整工具自审。
