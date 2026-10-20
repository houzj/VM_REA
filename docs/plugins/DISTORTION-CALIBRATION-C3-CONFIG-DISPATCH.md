# 畸变标定 C3：配置到拟合分派，第一有限切片

2026-10-19。本切片为源码/原指令索引，不改产品，不执行原厂DLL，不使用历史prefix实现。复用C1/C2和既有SDK trace，仅针对真实caller补取SDK参数包装、core字符串适配与两个不同拟合入口。REA报告无活动target、Ghidra未配置；使用既有专属私有Ghidra数据库只读导出，不改全局配置、不生成REA Evidence ID。

## 模块与SDK入口

模块ResetDefaultParam经180017040读取18001e9d0指定的Algorithm.xml再逐项SetParam；这条既有主链确定新模块默认来自Algorithm.xml，不能采用裸SDK默认或AlgorithmTab CurValue。配置资源为src/VisionPlugins.DistortionCalibration/Resources/DistortionAlgorithm{,Tab}.xml。

模块SetParam=180021560，GetParam=18001fb30：WeightFun别名均映射CalibBoardWeightFun。SetParam对原名/别名且strncmp(value,"4",1)==0时直接返回0，不传SDK；这包括首字符为4的字符串，不能改写成数值解析后仅拒绝整数4。模块本体没有在此对其余数字枚举统一作有效性校验。

SDK实际vtable为180007af8，元数据指针180007af0不是vtable首项：+48→2870 GetRunParam，+50→28e0 SetRunParam，+58→2990 SaveConfigData，+60→2a00 LoadConfigData，+68→2a70 Run。模块ProcessAlg 20820先SetInputImage、传ROI，随后调用+68。包装28e0仅将RCX换为[this+20]调用MVD_Algorithm_SetValueByString，RDX/R8仍为name/value；2870明确调用GetValueByString(handle,name,0x100,out)。反编译单参签名不证明name/value丢失。

core字符串设置22ab0：1d6c0解析name → 2af0解析value → 7020经7c6830操作6校验数字ID和值并缓存为dirty → 2e70更新schema DOM；下一Process的7120经操作1应用到algorithm context。读取22890经6d30查name，再2c60格式化。参数schema取值范围/名称到完整数字ID由配置管理器提供；本切片没有把1d6c0/2af0/2e70及全部schema资源展开，故不把XML低位AlgorithmIndex与core ID的对应当作已闭合的字符串解析全过程。

## 逐字段与层级

下表并列配置声明与确证的core写/读。core字段位于algorithm context，不是SDK工具对象本体，也不是C1的core外层对象同偏移。

| 字段 | XML默认/可见枚举 | core数字ID→context | 7f7050读→7ecea0 options | 后续分支 |
|---|---|---|---|---|
| CorrectModel | 1；1透视、2径向、3径向+透视；Tab index0401 | 40401→b4 | b4→options+18 | 7ed0c0读取+18分派1/2/3 |
| HomoFreedom | 1；1投影8系数、2仿射6、3相似4；Tab CurValue2而Default1 | 40001→9c | 9c→options+1c | 7ecea0限制1..3；1至少4点、2至少3点；7ff3c0 freedom!=3才做两侧非共线检测 |
| CalibBoardWeightFun / WeightFun别名 | 2；UI仅1 LeastSqure、2 Huber、3 Tukey | 40002→a0；写4时core改成3（单写和批写均如此） | a0→options+14 | 1→8345e0；2/3→7ff3c0；低层直接options4→832830 |
| WeightFactor | 20；UI1..200 | 40003→a4 | float(a4)*常量18162b60c(.1f)→options+20 | IRLS factor；20实际变2f；不是20次迭代数 |
| DistThreshold | 30；UI1..100 | 40004→a8 | float(a8)*.1f→options+24 | 低层Weight4的832830参数之一；UI不开放Weight4 |
| SampleRatio | 60；UI范围由资源保留 | 40005→ac | float(ac)*常量18162b610(.01f)→options+28 | 低层Weight4参数之一 |
| FilterStatus | 1 | 40006→b0 | 本切片不延伸识别过滤消费者 | 不误作拟合权重 |

7f7050 options+2c写50；它属于低层Weight4载荷，不能误作7fd4e0的实际IRLS迭代上限。7ed0c0各IRLS调用本地options+0c另写20、+10写0x38d1b717（1e-4f）。7ecea0检查options+0c为1..2（板相关字段）、+10非负、+14为1..4、+18与+1c均1..3；不能把不同options结构同偏移混称。

Weight4可达性有三层：模块字符串首字符4被忽略；core参数ID40002数值4在应用到算法context时改成3（schema/缓存原值不能据此改称3）；直接调用7ecea0/7ed0c0且手造options+14=4才有832830分支。只见低层分支存在，不证明真实模块可走它。此更正也限定C2讨论的非法WeightFun：C2是IRLS内部，不是模块入口承诺。

## 模型与实际拟合路径

7ed0c0 options+18读取模型后，源体中四处同样的weight分派都取得原指令call：832830 / 8345e0 / 7ff3c0；并非把所有拟合合并成单个IRLS。

- 模型1：先构造尺度/平移调整后的理想点，与图像点作初次拟合；输出载荷初装后，853380重建另一组目标点并再次按weight分派生成输出H。没有853820径向LM。
- 模型2：以单位H初始化后调用853820径向拟合，再逐点857a60逆径向；这些点进入一次按weight分派的后续矩阵拟合。输出缩放由所得矩阵两列范数计算。不能称为“仅四个径向系数、完全不拟合矩阵”。
- 模型3：先853820（独立call site；全部初值来源仍待下一切片逐表达式闭合），再逐点857a60逆径向，随后按weight分派拟合理想点与校正点。径向与透视组合不是单独模型1矩阵加一组任意径向参数。

本段确认callee与数据缓冲方向，完整网格点构造/中心及尺度选择的每条算术分支仍需下一切片。模型2/3的853820内部active掩码和LM迭代复用既有径向报告；不重新以本次方法体数量声称其全部覆盖。

## 归一化与同名权重

7ff3c0先对输入两组float点分别累计float质心，减质心后累计float欧氏距离，scale=1/(平均距离)，正距离时生效，否则scale=1；将中心化点乘scale并生成两组3×3变换，再800170构造设计矩阵。freedom!=3在前段用double二阶矩/行列式做退化检查；freedom3跳过该检查。options[0]==0才调用7fcf60初值，否则复用传入初值，随后7fd4e0 IRLS并反归一化。确切float运算次序和初值反归一化/再归一化复用C2及既有固定核证据。

畸变Weight1实际进入8345e0独立OLS，再8006e0计算误差；与映射工具直接进入7ff3c0、Weight1为2n个常权IRLS不是同一路算法。新取8345e0和832830方法体用于区分入口，不声称其全部内部数学及采样规则已在本切片闭合。C2已确认IRLS Weight3超界值EPS，不把UI名称Tukey替换为教科书0权公式。

既有probe JSON中的warp0/1是测试图像直板/桶形像素生成选择；trace-image-probe.cpp没有SetRunParam("warp",...)。它不是原厂新增配置枚举。GetWarpMatrix是结果API，CorrectModel才是本切片源证据支持的模型配置。文件/重映射消费留后续切片。

## 验证与明确缺口

ExportC3DispatchEvidence.ps1验证源体/汇编锚点、常量原字节、资源默认值及每个文件SHA-256后导出机器索引。本轮无原厂运行、无托管构建、无产品修改；C1/C2原厂快照与数值检查只作既有支撑。源码确认、独立实现、宿主验收保持分开。

下一最小缺口：字符串name→schema数字ID/范围与value转换/持久化读写完整体；7ed0c0各模型理想点构造、中心与初值和尺度精确表达式；8345e0与832830全数学分支；真实模块默认配置运行验证及真实宿主载入绑定。文件、图像重映射不在本切片完成范围。C1异常/容量与C2非有限/非法n仍保持原未知，未以硬编码prefix规避。



C3-2修正：schema name/ID/范围与缓存→Process应用已在 DISTORTION-CALIBRATION-C3-SCHEMA-STATE.md 补齐。上述首切片对7020立即写算法context的叙述已修正；参数缓存/DOM当前值与算法a0必须分开，原厂XML206包含Ransac4。


