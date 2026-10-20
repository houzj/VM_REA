# 灰度匹配 SDK 实际入口与运行边界

2026-10-20。承接身份库存及模块26体；本轮只静态取证，不实施算法、不执行原厂 DLL。原件为 Runtime/x64/MVDGrayPatMatchV2Cpp.dll，SHA256 `8893e01240a2face9d7d7f610e8f483101cd9c0d03465b9e045f19913976c0c9`。私有 Ghidra 工程取得12个具体体和汇编，全部 decompile=true，三次命令退出0；索引与各体哈希见 `analysis/gray-match/sdk-boundary/source-index.json`。没有切换共享 REA session。

## 实际链

模块 CreateTool → SDK 180001070：空 out 返回80100006；分配0xd8，构造1800101a0，虚表18001dc08。构造调用180011490，创建 PreproMask 到 this+20，MVD_Algorithm_CreateHandle(this+8, **0x1f6**, 0)。失败抛带原返回码异常。pattern 工厂180001000分配0xf8，初始化180007dc0，独立 PreproMask this+18，CreateHandle(this+8, **0x1f5**,0)。不能把两种handle视为同一个算子。

虚表字节直接来自同hash PE，构造汇编确认写入地址。模块实际槽与目标：SetInputImage +10→180010390；SetPatternBuffer +30→180010740；SetROI +40→1800103c0；Run +98→180010a20；GetResult +a0→180010750。原头 GrayPatMatchV2Cpp.h 的声明顺序与这些槽相符。

SetInputImage 直接存 this+28，不clone；非空时转发给 PreproMask +10，并写dirty=1；空时只替换指针。SetPatternBuffer直接存指针/长度到+50/+58，当前体不复制或验证。GetResult仅 this+48成功标志非零时返回this+18，否则null；因此旧结果内存存在不意味着本轮结果可获取。

Run首先清成功标志，再检查输入图像和模型，空时抛80100004。读取输入宽、高、像素格式、plane0地址与步长，调用1800125d0准备预处理。无mask时ROI null使用整图范围；非null使用具体shape位置/尺寸，越界分离条件抛80100025，再裁剪整数范围。有mask走独立image结构，不能替代为统一几何ROI。正常单模型调用180011230；+74多模型分支按原容器顺序逐个设置buffer、调用同桥，首次结果数量非0停止。仅成功返回末尾写成功标志1。注意这是 SDK 内部多模型分支，不与外模块100槽/UseMatchAllMode循环混淆。

SetROI有类型5、finite和角度检查；同指针则跳过。替换前剔除PreproMask中特定标记项；null释放旧shape并清指针，非null经1800119b0生成新shape后销毁旧shape，再交PreproMask。clone/生成的确切规则尚缺该callee，不依据方法名下结论。角度阈值及ROI四舍五入常数尚须PE常量/汇编核对，不填典型epsilon或0.5。

共用destroy导出1800010d0调用虚析构；matcher虚析构180010250→180010280，flags&1时delete。具体释放顺序在180010280尚待取体。

## 下一有限缺口

后续同一私有只读命令另补5体（共17体），退出0：

- **180011230**：实际 `MVD_Algorithm_Process(this+8,0x1f7,input,output)`；非0抛原码。成功后懒分配0x18结果对象，18000fe50按output+8数量准备存储；遍历output首指针的0x5c字节记录，前五float经box setter写入，其后+14..+2c逐字段拷贝。字段语义需result/header再核，不能凭偏移全部命名。缺info抛801000ff。
- **1800125d0**：不是已证的像素预处理核。它先零32字节结构调用UpdateHandle(mode0)，再写首DWORD线程数并UpdateHandle(mode1)，然后以首QWORD为CPU mask的40字节结构调用Process(op0x3e9,out=null)。参数来自Run中的PerformanceAssistant getter；每一步非0即抛。没有据dirty标志推断它处理图像。
- **1800119b0**：源非null且目标槽初始null，类型5或0xb，否则E80100006/80100001；调用shape虚表+88生成并写out。这将ROI生成为独立shape；确切虚接口名称以同hash头再核，仍不把原输入指针直接当拥有对象。
- **180010280**：先1800117f0实际清理，再释放两套引用计数缓冲、模型链表节点/哨兵、迭代器代理；最终写base虚表。具体matcher/ROI/handle销毁顺序仍取决117f0，尚不称全部释放闭合。
- **1800136d0**：仅将明确枚举1..3、100..105、200..205、300..307映射为同值，其他返回0。原模块本活动入口已有pixelFormat==1门；不能据SDK映射表扩产品RGB支持。

下一有限缺口：1800117f0、18000fe50及同hash核心op1f6/1f7注册，另补资源33项到SetParam/SDK键的完整链。不得按“灰度匹配”名称猜NCC或相关公式。显示与Frame输出生命周期仍未完整，Blob不混入此链。所有结论是源码边界，未取得数值或运行等价证明，工具未完成。

### 释放与Count续片（同轮追加）

又定向补1800117f0/18000fe50，共19体，命令退出0。117f0依次销毁非null算法handle、PreproMask、performance/debug对象；输入图像+28、mask图像+40、pattern对象+10及外部buffer+50仅清指针；结果对象+18虚析构，ROI+30 DestroyShapeInstance；最后释放SDK内部模型列表各自malloc buffer/描述对象和节点。由此可区分借用的输入/外部模型buffer与拥有的ROI/结果，不由析构名称猜测。

fe50明确容量与活动数分离：负数量抛80100005；0仅写活动数+0xc=0，保容量及对象；数量超过容量先10110清理再ff90分配并更新容量+8；不超容量时遍历**整个容量**重置各info，空info抛801000ff，最后写活动数。10110/ff90/fbb0具体分配/重置叶体仍未取，不据此宣称内部每一字段清零。

本轮下一优先转33资源→模块setter→SDK键/模型入口字段链和核心op注册定位；已获生命周期顶层足够识别借用/拥有边界，不无界展开所有容器析构。
