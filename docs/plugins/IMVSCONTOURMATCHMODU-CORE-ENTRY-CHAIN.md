# 轮廓匹配：核心对象、实际算法入口和ABI

2026-10-20，原MVD_Algorithm.dll SHA85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549；当前轮廓SDK与原模块各自哈希见SDK-DISPATCH及NATIVE-RUNTIME-CHAIN。复用本线既有analysis/distortion-calibration/core-project/nimage-core，全部-noanalysis -readOnly定向取体，输出仅写analysis/contour-match/core-source。原工程变更丢弃，不切REA共享target、不重复导入核心、无目标执行/managed构建/产品修改。

## 具体源码链

- SDK Tool17300 CreateHandle0x201 → 原CAPI14930 → type[0x1f5,0x203)分配0x6a0/ctor5aeb0。Pattern0x200也在同一factory区间，但保留type，不能把训练与匹配操作合并。
- ctor5aeb0写vtable181ebaba8。原字节严格PE映射core-handle-vtable.json：+8 Init5b180、+18 Process5b4a0、+28 SetValue5fde0、+0 deletingDtor5b0c0→5b0f0。
- CAPI152b0判空80100006、模块全局就绪门不满足80100015，随后virtual+18；不是直接调用标定1c4e0。15460 SetValueByString经+28；15270 DestroyHandle经虚析构并DestroyLogInstance。
- Process5b4a0先003680把外部操作映射内部模式；20d→2b且20e→2c（type201并不等同mode2b）。进入007120锁/线程上下文后，20d→5d750、20e→5eb90；各分支随后5c590清理。
- 主匹配5d750做5bfd0配置/缓冲准备，B746B0查询最大数量，再按count*0x5c+模型索引空间分配输出；调用B748F0(engine,mode2b,input,0x38,output,0x18)。轮廓5eb90对应input0x50/output0x8。Win64原指令5da4f..5da74与5f096..5f0b9明确RCX/RDX/R8/R9及两个栈参数；伪码将0x18误显示为double最小值，不能当浮点参数。
- B748F0核magic、null/输入输出指针，能力字段engine+90的0x8000必须存在；mode2b→C01130、mode2c→C025B0，不满足返回1000000D。
- B748F0尾跳前B74BCB/B74BA6将原RCX engine写到恢复后调用者第5参数槽(RSP+28)，再将input/inputLen/output/outputLen移入RCX/RDX/R8/R9。因此C01130/C025B0真实有第5个engine，原C显示4参数调用是反编译签名遗漏，不是native未初始化参数。
- C01130按0x38/0x18精确结构长与图像mono/宽高>=16/最大维度/stride/ROI/Mask门读取输入，按engine字段构造主匹配参数，最终调用BD1120；C025B0按0x50/0x8结构与同类前置校验，调用BD7F30恢复轮廓。两者独立源体，未以普通NCC/常规模板匹配猜写。

## 返回流与分析器注释边界

初导出的kernel C被推为void且函数体在cookie-check处截断，原caller却读取EAX，不能照此吞错误。原cookie函数181571B80的noReturn本来就是false（不是已证明cookie被标为noreturn）。专属ExportContourKernelReturn在只读分析副本中清call flow override、从各cookie后续地址disassemble/fixup函数体，并按caller读取EAX将两个kernel返回类型标为uint32；所有分析注释记录kernel-control-flow-annotations.txt，原始C/asm保留，新增*-return-annotated.c/asm单列。

返回型修正后的C01130可见1000000E结构长、10000007图像format、10000008尺寸、10000009 stride、1000000A data、1000001B冗余mask stride门、10000019 mask→ROI提取成功标志门、1000002D ROI/mask边界、10100040/10100044等模型/缓冲限制，实际算法返回码及成功0；这些值是原指令恢复结果，具体全部field谓词仍须逐项审查后才实施，不用一句generic error代替。原始伪码void不是源码行为。

## 已取得与具体残余

首轮17个core具体体+两kernel返回流注释版本、原vtable字节索引、实际CAPI身份均在专属core-source/output；源码体存在不等于字段/分支已完整审查。下一只补已证直接callee BD1120(主匹配)、BD7F30(轮廓点)、C9AE50(计时频率辅助)，不无边界恢复全DLL。

仍需：本工具训练CreateDipolePyramid→Pattern200具体操作与模板格式；0x201参数schema及engine字段读写；SDK ROI/mask/format/裁剪回投/结果对象全部字段；主匹配和轮廓内核必要callee/模式/精度/超时/排序/模型集合；模块SetParam六组异常与所有输出显示对象消费者；释放各内核buffer和模板所有权。当前无产品实现，也未做差分/宿主/人工验收，不能计完整工具。
## 直接算法callee补证

BD1120/BD7F30/C9AE50三体已取得，现累计20个core原体。BD1120验证模板magic8340/8341、be3c60状态、正尺寸与参数缓冲；仅一个模板走BCDA80，多模板走BD14A0；错误清输出count。BD7F30按模型/匹配数/参数路径走BD82B0/BD9D40。C9AE50实际仅QueryPerformanceFrequency，不是图像/参数处理器，先前未审查时的用途标签据实收窄。下一必须分别恢复单模板、多模板及轮廓两分支，不因默认UseMatchAllMode=False忽略UI可达多模板。
