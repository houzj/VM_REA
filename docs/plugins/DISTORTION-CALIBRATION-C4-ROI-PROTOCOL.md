# C4 全屏/Box 原厂线协议与原生读取有限切片

2026-10-19。收束：当前空AdditionPath供应商分支已由配置/注册源码关闭；WPF ItemsControl内部绑定释放单列运行库依赖，不以其未取源码无限阻塞vendor源码阶段。旧ROI事件所有权与显示属性/持久化整卡残余仍保留。

本片只被动读取原件、当前IL、私有Ghidra readOnly源码/原指令；无原厂执行、产品修改、托管构建。C总保持6/32，C4 S1/S2整卡仍开放。

## 写入端：不是SDK shape ABI

当前原XML的RoiType为Single/Box/FullScreenEnable。SelectFullScreen清NormalizeROIList后SetValue，NormalizeReversParse在默认空列表（Count<1且SelectType!=Avoid）写两个byte：`01 00`。Avoid空列表另写`00 00`，不是本配置普通全屏值。Count非零但所有项均无支持序列化分支时不自动补全屏，因为最终判断原列表Count而非已写记录数。

Box经NormalizeReversParse的RectROIModel分支调用RoiParamExtern.GetNormalizeParseBytes(IRectROI)。IUroRectROI版本byte=1，其余0；普通Box的ROIBox.GetIROI(version=0,非Line/Seq)返回NormalizedRectROIModel，因此本工具普通矩形使用版本0。没有count或总长字段；多记录是各自序列化数组直接拼接，BinaryWriter.Write(byte[])不加长度前缀。

| 字节偏移 | 长度 | 原厂写入 |
|---|---|---|
| 0 | 1 | 类型2 (Box) |
| 1 | 1 | 版本0；IUroRectROI为1 |
| 2 | 4 | `(float)roi.CenterPoint.X` |
| 6 | 4 | `(float)roi.CenterPoint.Y` |
| 10 | 4 | `(float)roi.Width` |
| 14 | 4 | `(float)roi.Height` |
| 18 | 4 | `(float)roi.Angle` |

总22字节（0x16）。float由BinaryWriter.Write(Single)写入；当前Windows x64读取端直接memcpy并作原生float消费，无字节翻转，故布局为小端IEEE754单精度。此序列化方法直接强转当前属性，没有除图尺寸、角度转换、裁边或中心→左上变换；归一化来自其具体Normalized模型，不应在writer额外做第二次归一化。

## IUserBytesData到原生边界

ROISelecter.SetValue先比较PreValue，仅变化时更新PreValue/CurValueA；SetParamUnit在写权限内SetBinaryData→InnerSetValue→((IUserBytesData)ParamsSource)[ComName]。ModuleParam显式IUserBytesData setter在Parent.IsOnDispose时返回；普通模块调用ModuleManager.SetBinaryData(Parent.ID,key,value,TimeOut)，VmModuleSet且%键走另一个DynamicBinaryParamValue分支，本RoiType不是该键。非0返回抛异常。

ModuleManager byte[]重载拒绝null，AllocHGlobal(data.Length)→Marshal.Copy原字节→IntPtr重载→FreeHGlobal；没有try/finally，所以调用抛异常时不能宣称必释放。IntPtr重载组装IMVS_SET_BINARY_DATA_INFO（UTF8固定256字节strName、pBinaryData、nBinaryLenth）并调用ServerSDKAPI.IMVS_SetBinaryData；成功设置ParamSeted。当前程序集IL已校核这条实参/调用链，未以旧public SDK包装推导额外参数范围。P/Invoke到底层平台投递的全部线程/异常路径尚未展开。

## 畸变自身的读取与存储对应

不是套Board模块RVA：本片私有脚本先严格匹配IMVSImageCalibModu.dll SHA256=0A6BAC886F1AA1CCC41433EF9D35FC1091B3D43354DE5794563454D8BBA03BCB，再只读打开本线既有工程，导出畸变SetParam基类及parser；2次Ghidra作业退出0，未执行DLL。

派生SetParam@0x21560已有体调用基类0x17900。本片0x17900原指令/数据导出确认`RoiType\0`字面量位于0x2c4a8；匹配后以this、原bytes、原len、&this+0x38调用0xa0c0。0xa0c0日志实名CVmAlgModuleBase::AnalyzeMultipleROIMsg。不能把ProcessAlg后续vtFixRoiShapeObj的指针ABI当这层字节格式。

parser每条先读取类型byte，要求至少两个头字节；case1消费2字节，构造中心.5/.5、宽高1/1、angle0的本地记录。case2要求累计长度至少+0x16，byte1零扩展存入扩展记录+0x14，从payload+2直接memcpy_s 0x14字节，调用0x1ae90追加。该helper实际记录stride0x28（40字节），所以**线协议22字节/本地记录40字节/后续像素矩形20字节**是三种不同布局。原指令a26a写版本，a270..a283复制20字节，a298追加；全屏a1fe..a213写0x3f000000/0x3f800000常量。

本片还从畸变自身GetProcessInput@0x10db0取体/原指令：普通模式（ROIAreaType==0）先尝试订阅ROI；本地矩形fallback读取+0x40..+0x48记录vector，每条stride0x28，按Cx*W、Cy*H、Width*W、Height*H形成5float输出，angle直接复制。11272/11285/11298/112a8四次MULSS，没有W−1/H−1或中途整型舍入。选用的W/H依前面图像/DrawImageSize状态分支，不把本地fallback缩放无条件施加给订阅ROI。全屏本地记录于是成为(W/2,H/2,W,H,0)。其他ROIAreaType和位置修正后shape消费者另属残余。

## 长度与失败边界

不能概括成严格整包消费。parser按记录计数，至多64项；不足Box所需字节跳公共收尾，若之前已有成功前缀可能按前缀更新存储并返回0，首项不足则已成功数0错误。额外1字节/不足头的尾部路径也须看累计循环条件；不采用“所有尾字节一律拒绝”规则。类型0不是普通全屏，当前switch不支持它；非法类型走Unsupported路径。多条成功时最终type改0x80。此为静态原体，不是畸形输入运行验证。

## 证据索引与未闭合项

`analysis/distortion-calibration/c4-source/roi-protocol-evidence.json`含所有原件哈希、源文件哈希、关键当前IL、native c/asm/bin和作业输出。托管生成器InspectC4RoiProtocol.ps1；原生RunC4RoiProtocol.ps1与DistortionRoiProtocol.java只读私有工程，严格SHA守卫。复用的Board报告只用来提示取体目标，没有作为畸变字段依据。

仍开放：平台IMVS_SetBinaryData到模块投递全链、UI模型角度单位与规范化属性内部实现、非普通模式/订阅/Fixture全分支、SDK形状克隆与裁边；C4整卡显示特征/持久化仍未完整。通过静态锚点不表示运行/宿主/人工验收。
