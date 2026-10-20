2026-10-20 最新生产专项：module134/configuration16/file257/outputmetrics229全部通过。ProcessAlg自动路径已实际改WriteModule且非.xml自动导出.dat用例通过；显式保存仍按源后缀门。真实Host构建/运行进行中，本工具尚未计完整。下方待实施叙述为历史。

# N图像显式导出与SDK结果生命周期

## 原厂证据

Module SHA256 68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC。源入口：SetParam 1800229d0、WriteXmlData 1800230e0、GetParam 180020260、ProcessAlg 180021450；新后缀helper 18001b7d0 的汇编位于 analysis/n-image-calibration/file-export-source/output。helper按字节比较末尾 .xml，区分大小写，错误E0000204，不是旧伪代码显示的未知RDI。

SDK vtable 180010f88+10为GetResult 180003b70：+148标记非0才返回+140；SDK Run 180003d60在进入时清标记、成功时置1。Module Clear仅释放frame列表，不调用SDK Run/Reset。MatrixFix结果有自己生命周期；HomoFixEnable在显式保存时重新选择原SDK矩阵或MatrixFix结果。

WriteXmlData首先检查当前frameCount >= 当前TransNum+RotNum，否则E0000517；再检查SDK GetResult，否则80100011；GetParam(MapData)读当前HomoFixEnable选择矩阵，无对应结果E0000001。图像轨迹/误差来自保留SDK结果，世界轨迹及TransNum/RotNum来自当前模块，两个列表分别完整写入，不截成当前数量。

## 定点原厂运行

analysis/n-image-calibration/retained-result-probe/module-runtime.jsonl：正常自有Module + NImage SDK + MatrixFix SDK，原ProcessAlg返回0、首Save0，Clear后count0且resultStillSame=true，新增9个物理坐标偏移100/200的frame不调SDK Run，Save仍0。两XML矩阵及图像轨迹不变、首世界点变成100/200。

这里新frame阶段actualProcessCalled=false：是源码已证明的修正输入失败后未进入ProcessAlg状态的受控重放，不冒称完整平台运行。缺ASCII alias与未装MatrixFix实例的首失败原样保留；未调用Platform Init或设备。

## 当前实施及待验证

NImagePlugin独立保留CachedBatch/Solve/Metrics与CachedFixedMatrix。pending/Clear/修正读取失败不清SDK结果；真正进入整批识别时清NImage缓存；成功后重建，MatrixFix仅在自身Run尝试时更新。Dispose重置全部。显式Export实现源顺序错误及当前world/旧image组合；NImageFile.WriteModule保持独立列表，公共Document/Read严格校验不变。

若恰好三张旋转关系按当前count访问旧SDK轨迹越界，明确NotSupported，不模拟原生越界内存；这条安全边界未作为等价覆盖。

已新增实际模块回归：Clear后数量码、大小写后缀优先、修正失败保留矩阵与旧轨迹/新world、当前HomoFix开关、改当前总数不截列表、真正SDK失败清旧结果。旧Module missing-field预期已按实际reader源修成E0000314，未调数值阈值。目前专项尚未执行，待B/D串行构建后root运行module-only/configuration-only，不计N图像工具完成。
额外已源核活动差异：ProcessAlg自动刷新直接调用WriteXmlData，不调用显式Save的1b7d0后缀helper。当前自动路径仍用了NImageFile.Write会误拒绝非.xml；已备原语义测试自动写.dat，待D当前冻结构建批结束后改调用WriteModule，不在构建中改产品。

