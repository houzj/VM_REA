# 轮廓匹配：模块原生运行与所有权首切片

2026-10-20，被动Ghidra导入原IMVSContourMatchModu.dll一次，严格SHA56156A8707FA92CE758485772BEB18A5289CB97599533D86897A9391817FCF64，x64。独立analysis/contour-match/native-source/project；未切共享REA target、未执行DLL、未构建产品。导出25本类/Create/Destroy函数的C、原指令与直接callee表；source-index.tsv及completed.txt记录25，headless退出0。首轮project父目录缺失失败保first-missing-project.log，创建专属父目录后成功；未修改其它工程。

## 已取得实际方法体与路径

| 路径 | 原具体体 | 已确认行为 |
|---|---|---|
| 创建 | CreateModule272d0→ctor21410→Init22ca0 | 分配0x16080；设置this+8句柄；Init失败SetLastError并走虚析构返回null，失败分配E0000003 |
| Init/SDK所有者 |22ca0→CreateToolInstance21950| VM_M_GetModuleId写this+34；CreateContourPatMatchToolInstance写this+15d98，只在空时创建；随后vtable+60默认设置（具体目标待补）及CFeaturePattern构造5e00/分配0xd9f8存+15da0 |
| 外层运行 | Process235e0 | param1/param2/输入image指针为空E0000001；本轮status+15d94置0；输入图像vcall+20必须为1否则E0000503；仅ProcessAlg==0再LimitJudge==0才ProcessOutput，各错误直接返回，绝非无条件发布旧结果 |
| 算法前置 | ProcessAlg239b0 | CreateToolInstance→SetMultiThread25010→SetInputImage24f90→SetMaskImage24fc0(input+118, input+110!=0)；ROI vector非空只消费首个SetROI26760；每步非0早返，最终GetResult22840 |
| 模型/匹配 | GetResult22840 | ClearMatchResult先清旧结果；工具BasicParam+28与标志处理；清模型索引map并调用tool+108；扫描CFeaturePattern列表最多100槽，每槽stride0x228，buffer+210非空且+218标志非0才tool+f0加入buffer，记录原槽号；this+15e50为0遇首有效槽即break；tool+88运行→UpdateMatchResult267c0→结果vector计数→status/timing |
| 判定/状态 | LimitJudge22ee0 | 按结果数增长StatusArray；计数判定helper1c6d0与九float判定helper1c690；每点status为各项AND，总status逐点AND；不能把匹配得到candidate即等同各限制OK |
| 发布 | ProcessOutput23d00 | 先VM_M_SetModuleRuntimeInfo；实际MatchNum截到this+15edc；顺序发布ModuStatus/MatchNum，再各点ModelIndex/Name、Box、MatchPoint、ScaleXY、Score/Status，后按flags发布MatchOutline/额外边缘信息。每transport返回检查，具体辅助21020/20fb0/20fd0/20ff0/21010/21030尚待取体核ABI |
| 释放 | DestroyModule273b0→虚析构21660→DeInit21a70 | DeInit free状态数组+15da8；DeleteModuBuf21b50销毁tool、模型列表及outline buffer；ClearMatchResult21890清vector内每项+140动态buffer(每条stride0x150)，清vector逻辑end；析构另释放vector容量与map，最后base析构 |

## SDK名称与ABI核对

原模块实际导入MVDContourPatMatchCpp.dll SHA b2ee17f6d7b38e67e055c68a63f4e5ecf9149b4977bb9891f19ffbe61bfd6fdf；未拿HP/Fast SDK冒充。CreateToolInstance日志文本虽遗留CreateHPFeaturePatMatchToolInstance字样，实际IAT调用和原import明确CreateContourPatMatchToolInstance，不能用日志名推算法身份。

原同安装ContourPatMatchCpp.h SHA247BCFFA386548DD3E493736517ADBBCE8DB9B702422D58A11BD12133E3DC5F4，Tool接口以虚析构起槽：+28 GetBasicParam，+88 Run，+f0 AddPatternBuffer，+108 ClearPatterns，与本模块调用顺序吻合；这只是接口槽定位，不代表已恢复SDK内算法。DeleteModuBuf伪码将DestroyContourPatMatchToolInstance印成无参，原asm21b6b将this+15d98送RCX，21b77经IAT调用后清指针，ABI明确传实际tool指针，不能按伪码漏参实现。

## 尚未闭合，继续取证

- Process/ProcessAlg/GetResult等原PE catch/清理funclet尚未从异常元数据恢复；C伪码中缺catch不能宣称没有异常路径，也不能据此实现恒定错误码。
- Init虚槽+60具体默认参数体、CFeaturePattern5e00/5f10、caller所用读取/发布helper、Limit helpers仍需定向补体。
- UpdateMatchResult267c0、ProcessOutput23d00及SetParam251b0方法体已实际取得，但逐字段/排序/轮廓拼接/边缘状态/坐标修正/参数分派尚未完整source审查，不把“已有大体”当实施完成。
- SDK Create/Run/Result/PatternBuffer内部及训练CreateDipolePyramid→原模板格式/训练内核仍未取体；原模块25体不代表算法核已还原。
- 本工具还无产品plugin/provider/runtime；未实施、未差分或宿主运行。HP/Fast仍各自独立身份盘点，不套用本体。
## 辅助体/异常恢复补证（同原件、同私有工程）

已追加17辅助/catch体和3个由原vtable确定的体。原vtable33d18+60=17cf0确为CVmAlgModuleBase.ResetDefaultParam：VM_M_GetCreateModuleCause，cause!=2时读取默认配置列表，再按列表顺序虚槽+18写参数，不是全descriptor初始化；+40=11b40为输入前置。CFeaturePattern5e00初始化模型区/criticalsection，5f10逐100槽free buffer再DeleteCriticalSection。Limit float helper1c690仅enable==1时检查[min-eps,max+eps]，int helper1c6d0闭区间；原常量330a4字节00000034即floatEPS=1.1920928955078125e-7。

PE EH严格匹配6函数：GetResult22840四catch分别2d690(int)、2d711(uint)、2d792(IMVDException.GetErrorCode虚槽+10)、2d83f(unknown→E00000FF)。三个typed返回22b64恢复MOV EDI,[RSP+b0]；catch-all返回22b71；22b78进入日志与同一尾部，原22c8b MOV EAX,EDI返回真实错误，ProcessAlg/Process遂早返不发Output。成功路径C伪码显示return0不能代替此异常事实。Init22ca0 catch-all2d8d6先DeInit，再22ecb MOV EAX,[RSP+78]返回保存的错误。Process235e0/239b0 EH仅unwind flags2、无tryBlocks，并非把GetResult catch移到这两层。

辅助typed传输实为20fb0→VM_M_SetFloat、20fd0→VM_M_SetInt、20ff0→VM_M_SetString、21020→VM_M_BatchSetInt；伪码void与尾跳返回寄存器尚需按实际调用ABI约束，不因void声明忽略原caller检查EAX。SetParam自身6组4catch已从元数据定位但未逐体审，仍为残余；完整模型训练/SDK内部与各原字段语义仍未完成实施。
