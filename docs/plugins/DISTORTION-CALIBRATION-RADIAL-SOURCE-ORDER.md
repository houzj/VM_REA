# 畸变径向 LM 原线性代数顺序

2026-10-19。最终回归结果见本报告末尾与validation.json。目标core SHA256 85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549；原件仅被动Ghidra读，不执行原厂。专属输出 analysis/distortion-calibration/radial-source-order/output。

实际854e90选择param9=0时固定14维，非活动参数导数写0；并非删除行列的紧凑活动系统。先730d20由J转置与J计算正规矩阵，再加mu*单位阵，731610原地求逆，730d20计算J转置*残差，再730d20求逆矩阵*梯度得到delta。每项double，尺寸/stride以元素为单位，不可直接用公共float inverse。

731610体与汇编：RCX矩阵/RDX int stride/R8低32列高32行/R9 pivot索引工作区。每轮按行再列扫描右下子矩阵，绝对值严格大于当前最大才更新索引（并列保留先项），最大精确0失败；760a60按双字8byte交换行，760730交换列。主元倒数为double DIVSD 1/pivot，缩放行、消其它非主元项、负倒数乘主元列，逆序按列索引换行、行索引换列恢复。没有原先独立解法的1e-300近零阈值。

730d20体按输出行/列遍历，每次两个乘积依序相加(sum+prod0)+prod1，奇数补一项；asm为MULSD/ADDSD，无FMA。82a5b0按原两坐标顺序累加平方和并返回sqrt(sum)；854e90用(oldNorm²-newNorm²)/predicted计算rho，并非RMS平方乘count，也不是省略sqrt后的raw sum。

实施：DistortionCalibrationRadialLinearAlgebra是本工具double全主元inverse→multiply窄helper，不改公共核。RadialFit使用完整14维、inactive导数零、正规矩阵/梯度按两个坐标的原累加顺序；优化器存储sqrt(sum)范数并在rho中平方，不再RMS平方乘count；径向Evaluate残差按原q+k*d*r+d*(k*k2)*r*r逐项关联。解析Jacobian的数学分支已存在，精确最终浮点结果由8原seed径向fixture和6模型runtime核查，不凭“数学等价”提前断言位等价。

取体脚本RunRadialSourceOrder.ps1使用自有core工程readOnly；DistortionRadialSourceOrder.java严格核SHA，导出asm改用decompiler基本块范围，避免既有函数body只含入口块时漏指令。731610/730d20及两个swap的完整c/asm均在专属目录；82a630/895150前次输出是辅助体，其旧入口body asm不是完整汇编证据，不混入“全部asm完整”结论。

失败门补齐：854e90求逆返回!=1后走85563c cookie尾，另取85564c段实际855654 XOR EAX,EAX，返回0而不走正常输出复制。853820每阶段比较return==1，否则853d67→853d7f XOR EAX,EAX；成功尾854e18 MOV EAX,1。7ed0c0两模型调用失败尾7edb18/7edc8f的PE原字节+8均B8 0B 30 10 10，即MOV EAX,0x1010300b，后仅恢复寄存器RET；ReadRadialTailBytes.py及tail-raw.json严格核core身份。产品不能用null break正常采纳早期值，现返回阶段失败并由Detailed传播1010300b/无File。

实际两变量消费者：854e90 param9=1固定2维（normal stride2；残差x/y双坐标），Jᵀr方向为observed-forward，delta按inverse*gradient后加到当前点，非负号。857a60构造两个active=1并以原observed点初始化输入缓冲；调用854e90返回1才取优化输出，否则原样写初始observed，不能吞失败返回最后迭代点，也不能把14变量外层1010300b套到此点API。InverseRadial现将2×2解析determinant求解换同double helper，null走初始observed fallback；14变量则typed失败1010300b。其余2变量状态机保持，待既有200点及6fixture回归。

2维残差/Jacobian补充：实际param9=1体中x先缓存kx*k2、y先缓存k2*ky，再乘offset；Jacobian保持rx=dx+dx、ry=(dy+dy)*aspect、(r+r)*rx/ry及原分项累加，未假设与14维分支关联完全相同。PixelChecks增加受控逆失败返回初始observed检查，只核产品失败门，不声称原厂非有限输入全域等价。

757490对全矩阵逐项取raw最大，初值18161d1d0=-DBL_MAX（原bytesFFFFFFFFFFFFEFFF）；两个分支均按全矩阵max，不以正规矩阵PSD推断只看对角。step绝对值由caller先895bd0后再取max。此处新primary覆盖此前将82a5b0称raw平方和的临时判断，详见完整c/asm与tail-raw.json常量。


最终已完成前三项：RadialChecks48项及阶段失败1010300b通过，47/48最终float同位，单近零k2差保留、maxmap2.8167050913907588E-9；PixelChecks12×192000像素全同、200逆点400/400float同，受控2维失败fallback通过；RadialFileChecks12原点集全成功，10组19/19同位，board2/model2/warp0为17/19且最大1.9588993998448165E-26（均值同），board2/model3/warp0为9/19最大.0001678464177530259、均值4.373962E-05。前者不归heap，后者沿历史prefix证据；都未清零/硬编码或扩大阈值。最终PluginChecks结果见validation.json。



## 2026-10-19 中心数组修复后的最终实施自审

最终串行验证已全部退出0：CenterInputChecks54、PointAliasChecks14、PluginChecks原6/6、实际MainWindow HostChecks21/21。真实Tee日志为analysis/distortion-calibration/c4-source/center-{input-checks,point-alias,plugin,host}-run.log；Host包含ROI编辑→失败→恢复、DrawOverlay实际Pen/GlyphRun、样式整方案重载及关闭释放。Host构建仅已有MainWindow83 CS8603警告。最终源码/证据哈希见analysis/distortion-calibration/radial-source-order/final-validation.json。

此前中心数组遗漏已实施并验证；14D/2D原逆矩阵再乘、残差关联、全矩阵max、sqrt范数及失败传播均已按具体原体收敛。按当前工具活动UI、模型/权重、文件、ROI、状态、显示、持久化、释放范围自审，未发现已确认源码仍未实施的活动路径，可交根线作整工具完成审计。人工验收独立，不作前置。

此结论不是完整原生bit parity：47/48近零系数差异及12文件中的17/19近零差异仍保留，未证明来自heap；历史未初始化越界读取采用明确内存安全适配，不能重现所有历史prefix。19float另一9/19历史相关差异仍公开。平台.sol编码、通用WPF内部和额外native ABI属于已列依赖边界；不以此抹去本工具真实支持域数值差分。旧validation与修前数值基线保留，数值源未再改，未重复大Pixel/12File。
