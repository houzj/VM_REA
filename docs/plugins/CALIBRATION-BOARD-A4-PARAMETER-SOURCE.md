# A4 当前安装参数类与保存入口有限源码切片

2026-10-19。在REA已认证的当前模块SHA/MVID、83方法token/RVA/IL身份基础上，以现有ILSpy 11.1.0.9782定向恢复CalibBoardCalibParam；另恢复当前VM.PlatformSDKCS.ParamItem，未加载执行目标程序集。重建C#是反编译推导，不替代原始CIL。没有产品或宿主行为变更。

原模块SHA `ff1408b85625819490efeb51e9e6d2714c7db003e5329aa65cc6b55a8ce8ba58`，MVID `41886863-165b-46de-83f0-ab60c218d812`；REA成员证据`ev_7eccd132b38875c6a84e8c7578581fa97cdf759ee36a41206cb5f045cc250166`。原始重建位于`analysis/calibration-board/a4-source/decompiled`，其来源文件、工具、选型与输出哈希见parameter-reconstruction-provenance.json。旧PublicAudit命名空间的包装不作为当前类源码。

## 构造、类型及读写

当前CalibBoardCalibParam(uint moduleID)先base(moduleID)，建立一个InputItem（InImage、InImageWidth、InImageHeight、InImagePixelFormat）和22个ParamItem。保存命令字段绑定SaveCalibPath，与缓存路径CalibPathName分开。以下上/下界来自构造实参，不把注释直接当运行验证。

| 字段 | 构造上下界/类型 |
|---|---|
| OriginX/OriginY | max1000/min−1000；Get/SetDoubleValue |
| RotationAngle | max180/min−180；Get/SetDoubleValue |
| PhysicalSize | max1000/min0.01；Get/SetDoubleValue |
| GrayContrast | max255/min1；Get/SetIntValue |
| SubPixelWindowSize | max150/min3；Get/SetIntValue |
| Circularity | max100/min20；Get/SetIntValue |
| EdgeThreshLow/High | max255/min默认0；Get/SetIntValue |
| WeightFactor | max200/min1；Get/SetIntValue |
| DistThreshold/SampleRatio | max100/min1；Get/SetIntValue |
| 选择项 | 转int交SetEnumValue；不是Enum.IsDefined校验 |
| CalibPathName、RefreshFileEnable、SaveCalibPath | string、bool、命令string入口；不能因ParamItem缺省max/min0就把字符串也当数值0域 |

当前VM.PlatformSDKCS.ParamItem.SetIntValue和SetDoubleValue先比较MaxValue/MinValue，超界抛VmException(0xe000030e)，再value.ToString()交Value。SetEnumValue仅ToString；Value setter调用ServerSDKAPI.IMVS_SetParamValue(handle,moduleID,Name,value)，非0抛VmException。数值转换是普通ToString，不在这里强制InvariantCulture；double分支仅两个比较，不据此声称NaN/Infinity通过原生。bool写True/False，string直接交Value。

这与旧iMVS-6000公开包装中未做数值范围检查的ParamItem不同；它们的命名空间与binary身份必须分开。旧报告只适用于原来标明的旧包装，不扩写为当前模块无范围检查。

## 显式保存

DoSaveFile(string value)以result=0开始，调用saveFile.SetStringValue；仅catch VmException并返回ex.errorCode，其他异常不在该catch覆盖。它不会因返回错误自动回滚其他参数，也不是等待下一Run的标志。当前本体没有更新CalibPathName缓存字段；底层native SaveCalibPath实际副作用沿A3/模块SetParam继续核，不能据字段名称补生命周期。

本片关闭当前参数类构造/数值读写/命令包装的有限来源；原配置Parser与该SDK类是不同入口，完整trigger/订阅/模板/保存恢复消费者和A4源→产品矩阵仍开放。S1–S7整卡不因恢复单个类自动加分；没有新增原厂执行或手动验收。

## 显式保存后的路径回读补齐

当前Apps.XmlParser.dll的SaveFileDialog已重新定向反编译，和复用源码规范化换行后逐字相同。assembly SHA78EC7AE1E5ADEFFF29197099C7109B153A3F75D092B7C3E9DC253D8C1B25A8D1；新SaveFileDialog.cs SHA BD70974CA4F946998C4DA911BFE1503FDD3A67CD7C821C84E53FA8263BDB642C；来源记录analysis/calibration-board/a4-source/current-save-dialog/provenance.json。未加载执行目标程序集。

SaveFileDialog.SaveFile先通过IUserStringData以Name=SaveCalibPath写命令，然后按Tabs顺序找到CalibPathName并调用GetParamUnit回读。原模块SetParam20d40的SaveCalibPath分支调用SDK Export后，实际清this+15da1并复制保存路径；所以“上层CalibBoardCalibParam.DoSaveFile没有自己写缓存字段”不代表整条保存链不更新缓存路径。当前产品配置成功Export后更新CalibPathName再RefreshParameter，与该成功路径相符，无需删除路径更新。导出返回码到缓存更新的原低层失败副作用另列，不能凭此声称任意Export失败都回滚原缓存。

本轮据当前原件关闭成功保存→命令→native路径缓存→UI回读的有限链，复用此前FailureOutput25的显式导出生命周期验证；未将保存对话框静态源码当作本次原厂交互证据。
