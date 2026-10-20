# 畸变标定 C3：19-float文件来源、方向与保存门控

2026-10-19有限切片。复用7ed0c0、SDK Run/Export和模块ProcessAlg/SetParam，定向补五个既有方法的原指令。没有新原厂执行或产品修改。源文件/指令哈希由ExportC3FileEvidence.ps1导出；S0–S7整卡审计另见DISTORTION-CALIBRATION-STAGE-AUDIT.md，不能因本文件切片完成抬高整卡完成率。

## 载荷构造与字段

7ed0c0原7ee38e开始写19项blob；op0xf的7f7050将该blob指针设为输出+24，1e330在0x67返回对象+28位置复制0x4c字节（另有9-float主H复制到+4），SDK Run2a70再从local1a0复制0x4c到HomoEstInfo对象+2c。19项文件blob与另一个9-float主H输出是不同存储，不能用同名“矩阵”混称。

| float索引 / byte偏移 | 写入来源 | 原厂重映射消费 |
|---|---|---|
| 0 / 00 | 7ee396写0x40000000，即2f | 这里称首字段2；没有仅凭数值建立全历史格式版本规范 |
| 1..9 / 04..24 | auStack98/88及uStack78九float；初次拟合得到的文件H。模型2 caller将其初始化为单位阵，后面的独立主H拟合写其他缓冲 | 按行H00,H01,H02,H10,H11,H12,H20,H21,H22；模型1/3消费，模型2忽略 |
| 10..11 / 28..2c | uStack158/154；模型2/3由853820的center输出写回；模型1初置0 | cx/cy |
| 12..15 / 30..3c | auStack58四float；模型2/3为853820系数输出，模型1初置0 | kx/ky/aspect/k2 |
| 16..17 / 40..44 | 7ed0c0第三位置参数中width/height int32转float | 83cef0要求与输入图像尺寸相符 |
| 18 / 48 | options+18模型整数转float | 对应1/2/3模型；实际映射caller传模型选择，不能只读取末值就跳过caller |

这些字段不包含估计误差、评估误差、点数、OK/NG、像素精度或workspace历史prefix。SDK GetWarpMatrix的1ed0仅检查out非null并memcpy对象+2c的0x4c字节，没有额外数值检查。

## 方向：输出像素到输入采样位置

83cef0从1基输出像素(u=x+1,v=y+1)计算输入采样坐标。模型1取q=H*[u,v,1]，按分母条件取倒数后得到源图像x/y；模型2直接作径向映射；模型3先同样透视，再径向。

径向使用dx=x-cx，dy=y-cy，r2=dx²+aspect*dy²；源坐标x'=x+kx*dx*r2+kx*k2*dx*r2²，y'=y+ky*dy*r2+ky*k2*dy*r2²。原厂具体double/float关联、分母epsilon/采样边界已在既有像素报告分开验证，本切片仅确认字段对应及pull-map方向；不能称为把输入图像点主动scatter到输出，也不能把文件H全局反转以配合另一9float输出。

## SDK导出入口与失败门

实际工具vtable+70→3b20 ExportCalibFile。拒绝null/空路径（80100006）、后缀不精确为小写iccal（8010000e）、result对象null或其中HomoEstInfo指针null（80100003）。路径helper47e0/46a0处理目录存在及编码分支；文件用fopen(...,"wb")或_wfopen(...,L"wb")，打开失败80100010。完整编码失败返回码沿原helper，不凭一般Windows路径规则改写。

导出在3f51通过HomoEstInfo vtable+10取得76bytes，3f68调用fwrite(buf,0x4c,1,file)，随后fflush/fclose；原指令未检查这三次返回值。没有临时文件/rename事务。文件按现有Win-x64内存float字节直接写出，不附文本头、额外长度或尾部校验。

SDK Run开始先清this+40有效标记，完成才置1；GetResult2860只在该标记非0时返回result。Export直接读this+30及其+28，绕开此GetResult有效性门，不检查SDK本轮状态/最新Run标志，也不校验19项是否有限或模型合法。首次尚无HomoEstInfo时拒绝。SDK Run只有在MVD_Algorithm_Process成功后才把新blob拷入HomoEstInfo；Process非0会在拷贝前抛异常。若之前已有HomoEstInfo，该失败路径没有清空旧blob，单独手动Export的对象存在门仍可能满足。这是正常失败路径的源码结论，不是本轮新增失败恢复运行；其他异常/并发不能概括为一律保留。

## 模块自动保存与显式保存

模块ProcessAlg20820先调用SDK Run，然后取GetResult；null时本轮status0并跳过保存。非null时，设path为this+15da1，refresh为this+15da0：仅当path非空且(路径不存在或refresh开启)调用vtable+70自动Export。路径存在且refresh关闭则不覆盖；路径不存在即使refresh关闭仍首次创建。存在判断20c0使用_waccess(path,0)，不是检查文件内容或矩阵版本。

RefreshFileEnable.SetParam用strncmp(value,"True",4)==0设置bool，故不是完整字符串严格等于True。自动导出正常返回后status才置1；若SDK Run/Export抛异常，不会正常走到该赋值。模块外层异常catch到最终输出状态的全部转换仍须单列，不由此段猜测所有错误码。

显式SaveCalibPath.SetParam先1b720检查后缀“.iccal”，成功才立即Export；原21706把实际传入path放RDX、21709调用+70。它不等待下一Run，也不受RefreshFileEnable控制。Export正常返回后才清空并复制缓存path；失败抛异常时后面的path更新不会执行。CalibPathName.SetParam仅检查/创建目录并缓存path，不立即导出；其完整目录/权限/编码异常处理沿已有体，不把所有入口合并成一个自动保存选项。

## 产品对照与明确残余

现有DistortionCalibrationFile要求76bytes、全部finite、首字段2、正整数宽高和model1..3，是独立产品校验；原SDK Export没有这些payload拒绝步骤。File.WriteAllBytes也会报告写失败，而原Export未检查写/flush/close结果。记录差异，不在本切片改产品或放宽校验；应由后续S4按兼容策略审查。

既有72项文件检查、12组像素和六组runtime回放只作已留存运行支撑，不冒称新增测试。本轮保留未知：模块catch完整错误输出映射、全部底层拟合失败到SDK错误的传播、所有路径编码/权限/磁盘满实际运行及并发/持久化事务。没有通过写历史prefix或扩大容差掩盖未知。


最新残余缩小：模块catch到发布已由C3-CATCH-OUTPUT有限切片取得16 handler及6续点。SDK Run异常属于第一try，GetResult/自动Export异常属于第二try；错误码、模块成功标志、ProcessOutput返回与Process最终返回彼此分开。先前“catch映射未知”以该新报告已列范围为准，EH外层/handler内部和真实异常注入仍开放。
