# 轮廓匹配：独立SDK实际分派

2026-10-20，当前MVDContourPatMatchCpp.dll SHA b2ee17f6d7b38e67e055c68a63f4e5ecf9149b4977bb9891f19ffbe61bfd6fdf。analysis/contour-match/sdk-source/project/contour-sdk为独立只读取体工程，原件仅导入一次；两轮headless均exit0。初始3export加其一层callee6体、Tool原vtable245d8按模块实际调用槽定向11体，均输出C/asm/calls。没有原厂执行/产品构建，不能以方法存在冒实施完成。

| 原模块已证调用 | 实际SDK地址 | 当前源证 |
|---|---|---|
| CreateContourPatMatchToolInstance |10b0→14d90→17300| 分配0x128实例，Tool vtable245d8；17300 CreateHandle(type0x201)，创建prepro-mask/basicparam/result对象；空outPtr80100006 |
| DestroyContourPatMatchToolInstance |1070→vtable0=14e60→14e90| 空ptr80100006；scalar deleting destructor flags&1才operator_delete；14e90实际成员销毁体尚待补 |
| SetInputImage/SetRegionImage/SetROI |+10=14fb0/+98=14fd0/+20=14ff0| 具体指针/ROI入口体已取得；SDK内部ROI映射及mask预处理Run helper未全审，不套典型crop算法 |
| GetBasicParam |+28=1a9f0| 返回Tool成员BasicParam，由模块OutLine flags控制；首字节开关需要按当前体实际消费 |
| SetRunParam |+70=15630| 实际MVD_Algorithm_SetValueByString(handle+8)；asm只改RCX，原RDX/R8键值保持，伪码缺参不可照抄；非0包装IMVDException(code,type7)抛出 |
| ClearPatterns/AddPatternBuffer |+108=15ac0/+f0=15a40| Clear分别清+90与+c0容器；Add非空buffer及长度写入+c0记录，空buffer抛80100006；容器ABI/buffer所有权清理尚需callee17c50/17d20/17b90 |
| Run |+88=162c0| 首先success+60=0、AlgTime清0；输入image为空抛80100004；16000/178e0处理参数，组装图像/ROI/mask输入；调用MVD_Algorithm_Process(handle,0x20d,input,output)；将结果写wrapper对象；BasicParam开关非0才再Process0x20e补轮廓点；尾部成功flag+60=1 |
| GetResult |+40=15330| 仅success+60非0返回+20结果对象，否则null；不能失败后仍暴露旧对象 |
| CreateContourPatternInstance |1000→9100| 分配0x120 Pattern，创建prepro-mask并MVD_Algorithm_CreateHandle(type0x200)，不是匹配Tool的0x201；模块UI训练CreateDipolePyramid路径仍需与此句柄具体操作对应 |

核心入口原件MVD_Algorithm.dll SHA85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549；PE实际CAPI RVAs：CreateHandle14930、Process152b0、SetValueByString15460、DestroyHandle15270，见core-api-identities.json。复用本线既有C1源14930/152b0，不能拿标定专用1c4e0/20480当此匹配router。14930中0x1f5<=type<0x203一组分配0x6a0→5aeb0 ctor，因此pattern200/tool201都走该对象族；具体vtable+18 Process分派到20d/20e的目标体仍待取，已开始只读复用保留核心工程定向5aeb0，不重导核心全DLL。

## 仍需闭合

1. 原core0x201对象的vtable Process/SetValue与20d主匹配/20e轮廓细化具体callee、参数结构、模板读写/训练与匹配算法；目前只是精确caller，不猜常规NCC/边缘匹配。
2. wrapper Run16000/178e0/19510：参数预检、ROI/mask图构造、图像format与stride、模型固定prefix/缓冲及结果坐标映射；原模块Current source还需完整对应。
3. SDK结果对象的初始化/拷贝/边缘点权重/score与输出mask，14e90所有权/原PE异常路径；现头部示意C不证明所有边界。
4. 模块SetParam786行与原56默认/114命名控件/68strcmp anchors的完整字段链审查；declared-field-surface.json只原锚点，不把声明当完整语义。
5. 原模块UpdateMatchResult用SDK隐藏结构返回：26908 LEA RDX,[RSP+60]→GetPosition+10，2699c..269b1复制两个float；伪码local_1d8/1d4貌似未初始化实为隐藏out目的槽，不能据此实现未知值/0。已用原asm核ABI，后续产品应保真实字段方向。

未改产品/共享框架、无托管构建或新原厂样本；HP/Fast仍独立，未以继承关系复用本算法。