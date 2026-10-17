# 逐插件逆向任务表

> 当前最高优先级已调整为[插件与节点公共框架逆向清单](../PLUGIN-FRAMEWORK-CHECKLIST.md) PF-001～014。配置模型、公共契约、模板、订阅/执行/ROI/宿主桥接也是本项目的正式开发任务，并与主框架同步；不是仅提交接口请求等待外线实现。下面162项插件声明清单保留，插件 UI 交付必须通过新公共框架迁移和人工验收。旧的“代码范围已实现”仅描述历史算法/代码节点，不代表本轮配置架构闭合。

> 最新双项目边界见 [职责与任务配套表](../PLUGIN-DEVELOPMENT-BOUNDARY.md)：本线独占工具及其必需节点框架；主项目负责工具无关的主界面/流程/画布/Group/生命周期与最终集成，由用户自行管控。每个启用的插件任务必须补“原厂证据 + 公共框架 PF 能力 + 主框架接口/单一交叉文件提交所有者 + 实际产品验收”四栏；当前在制工具对应关系已在配套表登记。下方162项是声明清单，未启动项不因表格存在而算任务已配齐或已实现。

当前四条独立工具开发线：N点标定（主控）、N图像标定、标定板标定、相机映射。不是单个工具的四个子任务；各线工具专属文件独立，公共框架变动统一协调。

机器可读完整声明、二进制身份、依赖声明和证据索引见 [inventory.json](inventory.json)。P0 图像源先完成；2026-10-17按用户要求将标定类10个工具提升至P1，优先于尚未开始的输出图像；P2 暂未开始，不表示已确定最终实施顺序。涉及流程和全局资源的插件先与框架线明确归属。

当前后续开发优先标定类。用户指定 **N点标定（IMVSNPointCalibModu）、N图像标定（IMVSNImageCalibModu）为最高优先级**，按此顺序逐个推进；**坐标系（CoordinateModule）放在标定类最后**。其余标定工具顺序随实际依赖确认，不把表格顺序当作已确认源码依赖。先核查源码、资源和原厂受控运行，再实现与验证；仍逐个工具闭合，不做设备测试，必要共享调整由插件线在本worktree完成。

| 优先级 | 类别 | 插件 | 名称 | 参数/输入/输出 | UI类型 | 逆向状态 |
|---|---|---|---|---|---|---|
|P0|Collection|ImageSourceModule|图像源|23/5/11|Boolean, BooleanTips, ButtonSelecter, ButtonSelecter_ImageList, Enumeration, EnumerationCameraForImage, Integer, OpenFolderDialog|实机取帧/参数/计数/恢复、JPEG边界、RGB、SDK、过滤、目录监听与内嵌已有原厂对照；767插件/242 WPF/460其他回归通过；待用户协助原厂桌面弹层/远程环境，明确覆盖限制，不标记完整等价|
|P1|Collection|ImageAcquisitionModule|多图采集|7/0/6|Enumeration, EnumerationCameraForAcq, ImageConfig, Integer, OpenFileForAcquisition|代码范围已实现：源码/资源、双模式SDK UI与用户相机桌面图、循环/恢复/内嵌重载、逐图配置、748字节光源边界及8路宿主预览；设备测试依用户要求排除；全场景原生等价不作断言|
|P1|Collection|ImageBufferModule|缓存图像|0/3/17|ButtonSelecter|代码范围已实现：原生FIFO/边界/混存/格式/恢复、55组逐像素序列及无输入对照、原厂完整SDK参数区、整数常量与15路宿主预览；不做设备测试，不宣称原厂全部共享弹层/线程时序等价|
|P1|Collection|LightModule|光源|32/7/26|Boolean, ButtonSelecter, Enumeration, EnumerationLightForLight, Integer, RadioSelecter|代码范围已实现：源码/资源、246组受控原厂运行、24张SDK界面、8通道748字节命令与暂存/失败语义；设备测试排除，全局管理服务及原生等价边界见LIGHT.md|
|P1|Collection|SaveImage|输出图像|44/8/8|AddList_SaveImage, Boolean, Boolean_SaveImage, ButtonSelecter, ColorConfigEx, Enumeration, Float, Integer, IPString, OpenFolderDialogEx, PasswordString, String, StringImgName_Selecter|声明已清点；语义与运行待逆向|
|P2|Calculation|CalculatorModule|变量计算|0/0/4|ButtonSelecter_CalculatorPro, Command|声明已清点；语义与运行待逆向|
|P2|Calculation|CoordinateTransform|坐标转换|1/4/3|Boolean, ButtonSelecter, RadioSelecter, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Calculation|IMVSCalibTransformModu|标定转换|2/5/12|ButtonSelecter, Enumeration, ImageViewConfig, OpenFileForCalibDialog, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Calculation|IMVSLineAlignModu|线对位|3/32/5|AddButton_LinePair, Enumeration, LinePair, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Calculation|IMVSMultiPointAlignModu|点集对位|2/16/5|AddButton_PointPair, PointPair, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Calculation|IMVSRotateCalculateModu|旋转计算|1/5/7|ButtonSelecter, ImageViewConfig, RadioSelecter, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Calculation|IMVSScaleTransformModu|单位转换|1/4/11|ButtonSelecter, OpenFileForCalibDialog|声明已清点；语义与运行待逆向|
|P2|Calculation|IMVSSinglePointAlignModu|单点对位|0/4/5|PointPair, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Calculation|SinglePointGrabModu|单点抓取|4/9/10|Boolean, ButtonSelecter, Command, Enumeration, OpenFileForCalibDialog, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Calculation|SinglePointMapAlignModu|单点映射对位|2/10/8|ButtonSelecter, OpenFileForCalibDialog, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Calculation|SinglePointRectifyModu|单点纠偏|4/9/10|Boolean, ButtonSelecter, Command, Enumeration, OpenFileForCalibDialog, RadioSelecter|声明已清点；语义与运行待逆向|
|P1|Calibration|IMVSNPointCalibModu|N点标定|6/6/28|Boolean, ButtonSelecter, Command, EditButton_Calib, Enumeration, Expand, Float, Integer, OpenFileForCalibDialog, RadioSelecter, SaveFileDialog, String|代码范围已实现：原厂源码/资源、103条导出方法/23状态场景/33次文件调用与27XML、10组UI规则和点表取证；独立求解/状态/UI/共享接口，22程序32088检查；数值等优/病态与单组Huber差异、桌面/WinForms截图边界见N-POINT-CALIBRATION.md，不宣称全部原生等价|
|P1|Calibration|IMVSNImageCalibModu|N图像标定|3/5/33|Boolean, ButtonSelecter, Command, Enumeration, EnumerationG, Float, Integer, OpenFileForCalibDialog, RadioSelecter, SaveFileDialog|并行推进中：源码/资源、SDK与模块方法运行、168组跨9图像角点、序列/专属UI/文件597项阶段检查；HIK编码板独立检测/解译和完整工具尚未闭合|
|P1|Calibration|IMVSCalibBoardCalibModu|标定板标定|16/1/16|Boolean, Enumeration, EnumerationG, Expand, Float, ImageViewConfig, Integer, IntegerBettween, OpenFileForCalibDialog, ROISelecter, SaveFileDialog|声明已清点；语义与运行待逆向|
|P1|Calibration|IMVSCameraMapModu|相机映射|7/16/10|AddButton_PointPair, Boolean, Enumeration, Integer, OpenFileForCalibDialog, PointPair, RadioSelecter, SaveFileDialog|声明已清点；语义与运行待逆向|
|P1|Calibration|IMVSImageCalibModu|畸变标定|15/2/10|Boolean, ButtonSelecter, Enumeration, EnumerationG, Expand, Float, FloatConfig, ImageViewConfig, Integer, IntegerBettween, OpenFileForCalibDialog, RadioSelecter, ROISelecter, SaveFileDialog|声明已清点；语义与运行待逆向|
|P1|Calibration|IMVSMapCalibModu|映射标定|6/9/17|Boolean, ButtonSelecter, Enumeration, EnumerationG, Expand, ImageViewConfig, Integer, OpenFileDialog, OpenFileForCalibDialog, RadioSelecter, RadioSelecterEx2, ROISelecter, SaveFileDialog, String|声明已清点；语义与运行待逆向|
|P1|Calibration|ReadCalibFileModu|标定加载|2/10/12|Boolean, ButtonSelecter, OpenFileDialog4ReadFile, ReadFileConfigData|声明已清点；语义与运行待逆向|
|P1|Calibration|RotateCalibModu|旋转标定|1/5/7|Boolean, ButtonSelecter, OpenFileForCalibDialog, RadioSelecter|声明已清点；语义与运行待逆向|
|P1|Calibration|TranslationCalibModu|平移旋转标定|8/6/28|Boolean, ButtonSelecter, Command, EditButton_Calib, Enumeration, Integer, OpenFileForCalibDialog, RadioSelecter, SaveFileDialog|代码范围已实现：48原厂场景/26文件尝试24XML/12原厂UI规则和点表，专属7447检查、实际WPF267检查；独立组合补偿/示教/文件副作用，数值/桌面边界见TRANSLATION-CALIBRATION.md|
|P1|Calibration|CoordinateModule|坐标系|2/4/6|ButtonSelecter, RadioSelecterEx2, RadioSelecterEx3, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ColorProcessing|IMVSColorExtractModu|颜色抽取|4/4/7|Boolean, ColorExtractList, FloatBettween, GroupLinkItem, ImageViewConfig, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ColorProcessing|IMVSColorImageGenerationModu|彩图生成|1/3/3|Enumeration, EnumerationG|声明已清点；语义与运行待逆向|
|P2|ColorProcessing|IMVSColorMeasureModu|颜色测量|37/4/20|Boolean, Enumeration, FloatBettween, GroupLinkItem, ImageViewConfig, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ColorProcessing|IMVSColorRecognitionModu|颜色识别|8/4/17|Boolean, ColorRecognitionData, Enumeration, GroupLinkItem, ImageViewConfig, Integer, ROISelecter, String|声明已清点；语义与运行待逆向|
|P2|ColorProcessing|IMVSColorSegmentModu|颜色分割|6/4/9|Boolean, ColorSegmentList, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ColorProcessing|IMVSColorTransformModu|颜色转换|9/1/3|Enumeration, Expand, Float, Integer|声明已清点；语义与运行待逆向|
|P2|Communication|CameraIOModule|相机IO通信|5/5/3|Boolean, ButtonSelecter, Enumeration, Enumeration_CameraSelector, Integer|声明已清点；语义与运行待逆向|
|P2|Communication|DataAnalysisModule|协议解析|2/1/2|BooleanInt, ButtonSelecterAnaly, ComboxEditDataAnalysis, OpenFileForDataShellDialogAnaly, ProtocolOutputParam, ProtocolOutputParam_Byte, ProtocolOutputParam_Shell, RadioSelecterAnaly|声明已清点；语义与运行待逆向|
|P2|Communication|DataAssembleModule|协议组装|3/0/4|ComboxEditDataAssembly, HeadBar, OpenFileForDataShellDialog, ProtocolInputParam, RadioSelecterEx2|声明已清点；语义与运行待逆向|
|P2|Communication|ReadDatasModule|接收数据|7/1/3|CommunicationItemReceive, EnumerationCommunication, EnumerationDataCenter, GlobalVarItemOut, HeadBar, Integer, ModbusItemReceive, PLCItemReceive, QueueItemOut, RadioSelecterEx3|声明已清点；语义与运行待逆向|
|P2|Communication|SendDatasModule|发送数据|98/48/3|AddButton_GlobalVar, AddButton_QueueItem, BasicItemsSend, Boolean, BooleanTip, ButtonSelecter, ComboxEditSendData, Enumeration, EnumerationCommunication, EnumerationDataCenter, EnumerationLightForSendData, EnumerationSendEvent, GateWaySendEventItem, GlobalVarItemIn, HeadBar, IOSendData16, IOSendData20, IOSendData4, IOSendData7, IOSendData8, ModbusItemsSend, QueueItemIn, RadioSelecterEx3|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSCircleEdgeInspModu|圆弧边缘缺陷检测|21/5/28|Boolean, ButtonSelecter, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSCircleEdgePairInspModu|圆弧对缺陷检测|24/6/34|Boolean, ButtonSelecter, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, Integer_CaliperHeight, Integer_IdeaWidth, IntegerBettween, RadioSelecter, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSEdgeFlawInspModu|边缘模型缺陷检测|23/2/14|Boolean, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, Panel_EdgepatMatch, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSEdgeInspGroupModu|边缘组合缺陷检测|0/2/20|AddList_EdgeInspGroup, Boolean, ButtonSelecter, Combination, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, RadioSelecter3, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSEdgePairFlawInspModu|边缘对模型缺陷检测|47/2/28|Boolean, CircleSelector, Command, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, Panel_EdgePairpatMatch, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSEdgePairInspGroupModu|边缘对组合缺陷检测|1/2/27|AddList_EdgeInspGroup, Boolean, ButtonSelecter, Combination, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, RadioSelecter, RadioSelecter3, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSEdgePairPosTrendAnalyModu|边缘对位置趋势分析|13/3/26|Boolean, Enumeration, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSEdgePosTrendAnalyModu|边缘位置趋势分析|10/3/21|Boolean, Enumeration, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSInspectModu|异常检测|23/3/10|Enumeration, Expand, Float, FloatBettween, ImageViewConfig, InspectData, Integer, IntegerBettween, RadioSelecterEx2, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSLineEdgeInspModu|直线边缘缺陷检测|26/5/27|Boolean, ButtonSelecter, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter3, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSLineEdgePairInspModu|直线对缺陷检测|28/6/32|Boolean, ButtonSelecter, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, RadioSelecter3, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSMarkInspModu|IMVSMarkInspModu|50/3/11|Boolean, Enumeration, Expand, Float, FloatBettween, FloatSum, GroupLinkItem, HeadBar, ImageViewConfig, Integer, IntegerBettween, MarkInspData, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSMarkInspModuVA|字符缺陷检测|41/9/15|Boolean, ButtonSelecter, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, HeadBar, ImageViewConfig, Integer, IntegerBettween, MarkInspDataVA, ROISelecter|声明已清点；语义与运行待逆向|
|P2|DefectDetection|IMVSSurfaceDefectFilterModu|表面缺陷滤波|12/5/6|Enumeration, Float, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Global|CommDeviceModule|CommDeviceModule|13/0/0|Boolean, ComboxEdit, EnumeraionComDevice, String, TextNormal|声明已清点；语义与运行待逆向|
|P2|Global|CommManagerModule|CommManagerModule|23/0/0|Boolean, ComboxEdit, EnumeraionComDevice, Integer, String, TextNormal|声明已清点；语义与运行待逆向|
|P2|Global|DataQueueModule|DataQueueModule|3/0/0|Command, DataQueueData, TriggerProcedureData|声明已清点；语义与运行待逆向|
|P2|Global|DynamicParamCameraModule|DynamicParamCameraModule|3/4/5|BooleanBxTips, CameraPreview, ClockSource, CommandWithTip, DynamicCameraParamList, EnumerationLxN, MvsFeatureTree|声明已清点；语义与运行待逆向|
|P2|Global|GlobalCameraModule|GlobalCameraModule|62/4/5|Boolean, BooleanBx, BooleanBxTips, BooleanLx, CameraPreview, ClockSource, Command, CommandWithTip, DynamicCameraParamList, Enumeration, EnumerationBx, EnumerationGain, EnumerationIP, EnumerationLxN, FloatBx, Integer, IntegerBx, IPString, MvsFeatureTree, OpenFileForCameraDialog, RadioSelecter, String, StringBx|声明已清点；语义与运行待逆向|
|P2|Global|GlobalTriggerModule|GlobalTriggerModule|0/0/0||声明已清点；语义与运行待逆向|
|P2|Global|GlobalVariableModule|GlobalVariableModule|0/0/0||声明已清点；语义与运行待逆向|
|P2|Global|ImageCollectDalsaModu|ImageCollectDalsaModu|3/0/2|EnumerationLx, Integer, OpenFileForCameraDialog|声明已清点；语义与运行待逆向|
|P2|Global|ImageCollectHikModu|ImageCollectHikModu|6/0/5|EnumerationBx, EnumerationHikLx, EnumerationLx, IntegerBx, OpenFileDialog|声明已清点；语义与运行待逆向|
|P2|Global|ImageCollectIKModu|ImageCollectIKModu|2/0/2|EnumerationLx, OpenFileForCameraDialog|声明已清点；语义与运行待逆向|
|P2|Global|ImageCollectModu|ImageCollectModu|9/2/5|EnumerationBx, EnumerationGain, EnumerationLx, FloatBx, OpenFileForCameraDialog, String, StringBx|声明已清点；语义与运行待逆向|
|P2|Global|LightControl|LightControl|0/0/0||声明已清点；语义与运行待逆向|
|P2|Global|LocalVariableModule|LocalVariableModule|0/0/0||声明已清点；语义与运行待逆向|
|P2|Global|ProcedureComm|ProcedureComm|0/0/0||声明已清点；语义与运行待逆向|
|P2|Global|ProcedureModule|ProcedureModule|0/0/1|GroupCommunicationParam, GroupDisplayParam, GroupInputParam, GroupOutputParam, GroupRunParam|声明已清点；语义与运行待逆向|
|P2|Global|SCGlobalCameraModule|SCGlobalCameraModule|5/0/5|EnumerationBx, EnumerationLx, IntegerBx|声明已清点；语义与运行待逆向|
|P2|Global|SolutionModule|SolutionModule|0/0/0|GlobalRunParam|声明已清点；语义与运行待逆向|
|P2|ImageGeneration|GeometryCreate|几何创建|0/6/16|Boolean, ButtonSelecter, ImageViewConfig, RadioSelecter, RadioSelecter3, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageGeneration|IMVSCircleFitModu|圆拟合|20/2/11|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, ImageViewConfig, Integer, IntegerBettween, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|ImageGeneration|IMVSEllipseFitModu|椭圆拟合|13/1/9|Boolean, ButtonSelecter, Expand, FloatBettween, ImageViewConfig, Integer, IntegerBettween, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|ImageGeneration|IMVSLineFitModu|直线拟合|17/2/11|Boolean, ButtonSelecter, Enumeration, Expand, Float, FloatBettween, ImageViewConfig, Integer, IntegerBettween, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSAffineTransformModu|仿射变换|12/3/5|Boolean, Enumeration, Float, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSBinaryModu|图像二值化|15/6/11|Enumeration, Float, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSFrameMeanModu|帧平均|1/1/5|CommandSelecter_Frame, ImageViewConfig|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSGeometricTransformModu|几何变换|2/1/3|Enumeration|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageCombineProcessModu|图像组合|0/6/10|Enumeration, Expand, Float, GroupLinkItem, ImageCombineProcessData, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageCorrectCalibModu|畸变校正|1/2/3|ButtonSelecter, Enumeration, OpenFileForCalibDialog|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageCorrectManualModu|图像矫正|2/1/3|Integer|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageEnhanceModu|图像增强|7/6/9|Enumeration, Float, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageFilterModu|图像滤波|6/6/9|Enumeration, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageFixtureModu|图像修正|0/2/4|ButtonSelecter, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageMathModu|图像运算|8/5/5|Enumeration, EnumerationG, Expand, Float, GroupLinkItem, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageMorphModu|形态学处理|5/6/9|Enumeration, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageNormlizeModu|图像归一化|7/6/9|Enumeration, Float, FloatBettween, FloatSum, GroupLinkItem, ImageViewConfig, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageResizeModu|图像缩放|4/3/5|Boolean, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImageSharpnessModu|清晰度评估|2/6/10|Enumeration, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSImgStitchCalibModu|图像拼接|13/2/6|AddList_ImageStitch, Boolean, ButtonSelecter, Combination, Enumeration, EnumerationG, GroupLinkItem, ImageSourceList, ImgStitchInteger, Integer, NumberMultiply, Pannel_ImgStitch, RadioSelecterEx2, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSInverseAffineTransformModu|逆仿射变换|2/6/5|ButtonSelecter, EnumerationG, ImageViewConfig, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSMultiImageFusionModu|多图融合|8/2/8|Boolean, Enumeration, EnumerationG, Expand, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSPolarUnwarpModu|圆环展开|3/3/6|Enumeration, GroupLinkItem, ImageViewConfig, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSRegionCopyModu|拷贝填充|3/4/7|Enumeration, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|ImageProcessing|IMVSShadeCorrectModu|阴影校正|5/6/9|Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSAngleBisectorFindModu|角平分线查找|0/3/14|ButtonSelecter, ImageViewConfig, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Location|IMVSBlobFindLabelsModu|Blob标签分析|78/7/26|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter, ThresholdAreaList|声明已清点；语义与运行待逆向|
|P2|Location|IMVSBlobFindModu|Blob分析|105/6/33|Boolean, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSCaliperCornerModu|边缘交点|21/6/20|Boolean, ButtonSelecter, Enumeration, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSCaliperEdgeModu|边缘查找|17/3/13|Boolean, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSCaliperModu|卡尺工具|98/4/27|Boolean, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSCircleFindModu|圆查找|28/5/16|Boolean, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, Integer_CircleFind, IntegerBettween, IntegerBettween_CircleFind, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSContourMatchModu|轮廓匹配|56/5/22|Boolean, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, PyramidFeaturepatMatchConfigPanalData, RadioSelecterEx2, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSEllipseFindModu|椭圆查找|9/4/11|Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSFastFeatureMatchModu|快速匹配|41/5/17|Boolean, Enumeration, Expand, FastFeaturepatMatchConfigPanalData, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSFixtureModu|位置修正|8/4/3|ButtonSelecter, Command_FixtureModule, ImageViewConfig, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSGluePathConductModu|路径提取|2/2/27|Boolean, Enumeration, GroupLinkItem, ImageViewConfig, Integer, PathDrawingGuide|声明已清点；语义与运行待逆向|
|P2|Location|IMVSGrayMatchModu|IMVSGrayMatchModu|31/3/13|Boolean, Enumeration, Expand, Float, FloatBettween, GrayMatchConfigPanalData, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSGrayMatchModuVA|灰度匹配|33/3/13|Boolean, Enumeration, Expand, Float, FloatBettween, GrayMatchVAConfigPanalData, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSHPFeatureMatchModu|高精度匹配|52/5/19|Boolean, Enumeration, Expand, FeaturepatMatchConfigPanalData, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, RadioSelecterEx2, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSLineFindGroupModu|直线查找组合|14/1/13|AddList_EdgeInspGroup, Boolean, Combination, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, Integer_CircleFind, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSLineFindModu|直线查找|24/4/15|Boolean, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, Integer_CircleFind, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSMarkFindModu|图形定位|47/5/19|Boolean, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, MarkFindConfigPanalData, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSMatrixCircleFindModu|阵列圆查找|27/5/20|Boolean, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, IntegerBettween_CircleFind, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSMedianLineFindModu|中线查找|0/3/8|ButtonSelecter, ImageViewConfig, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Location|IMVSMultiLineFindModu|多直线查找|19/4/16|Boolean, Enumeration, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSPairLineModu|平行线查找|22/5/22|Boolean, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSParallelCalculateModu|平行线计算|2/3/8|ButtonSelecter, Enumeration, Float, ImageViewConfig, RadioSelecter, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Location|IMVSPeakFindModu|顶点检测|13/5/17|Boolean, Enumeration, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSQuadrangleFindModu|四边形查找|59/8/62|Boolean, ButtonSelecter, Enumeration, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, ROISelecter_Fix|声明已清点；语义与运行待逆向|
|P2|Location|IMVSRectFindModu|矩形检测|30/5/12|Boolean, Enumeration, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSTargetTrackModu|目标跟踪|13/3/14|Boolean, ButtonSelecter, Float, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Location|IMVSVerticalLineFindModu|垂线查找|1/3/8|ButtonSelecter, ImageViewConfig, RadioSelecter, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Logic|AndModule|逻辑|2/0/4|ButtonSelecter_LogicItem, EnumerationLogicType|声明已清点；语义与运行待逆向|
|P2|Logic|BranchModule_STD|分支模块|2/1/4|ButtonSelecter, HeadBar, ListParam, RadioSelecter_Branch|声明已清点；语义与运行待逆向|
|P2|Logic|BranchStringCpmL|分支字符|1/1/4|ButtonSelecter, HeadBar, ListParamStr|声明已清点；语义与运行待逆向|
|P2|Logic|DataClassificationModule|数据分类|0/0/2|DataClassificationConfig, DataClassificationParam|声明已清点；语义与运行待逆向|
|P2|Logic|DataFilterModule|数据筛选|0/0/3|DataFilterConfig, DataFilterParam|声明已清点；语义与运行待逆向|
|P2|Logic|DataRecordModule|数据记录|0/1/2|ButtonSelecter, ButtonSelecter_IOVar_Record, HeadBar|声明已清点；语义与运行待逆向|
|P2|Logic|DataSetModule|数据集合|0/1/2|ButtonSelecter, ButtonSelecter_IOVar, HeadBar|声明已清点；语义与运行待逆向|
|P2|Logic|DataSortModule|数据排序|0/0/2|DataSortConfig, DataSortParam|声明已清点；语义与运行待逆向|
|P2|Logic|FormatModule|格式化|0/0/3|Format|声明已清点；语义与运行待逆向|
|P2|Logic|GraphicsSetModule|图形收集|0/0/2|GroupDisplayParamForGraphicsSet|声明已清点；语义与运行待逆向|
|P2|Logic|IfBranchModule|条件分支|0/0/3|ButtonSelecter_IfBranch, HeadBar|声明已清点；语义与运行待逆向|
|P2|Logic|IfModule|条件检测|1/0/6|ButtonSelecter_RangeConF, Enumeration_IfModule, HeadBar|声明已清点；语义与运行待逆向|
|P2|Logic|IMVSGroup|Group|1/8/3|Boolean, ButtonSelecter, ButtonSelecter_LoopControl, Enumeration, GroupDisplayParam, GroupInputParam, GroupOutputParam, GroupRunParam, Integer|声明已清点；语义与运行待逆向|
|P2|Logic|PointSetMODU_STD|点集|17/16/3|AddButton_PointSet, Boolean, ImageViewConfig, PointSet|声明已清点；语义与运行待逆向|
|P2|Logic|PyShellModule|Python脚本|0/0/2|ButtonSelecter_PyShell|声明已清点；语义与运行待逆向|
|P2|Logic|SaveTextModule|文本保存|114/103/4|AddButton_SaveText, Boolean, ButtonSelecter, Enumeration, HeadBar, Integer, OpenFolderDialog, RadioSelecterEx2, SaveTextItem, String, StringName|声明已清点；语义与运行待逆向|
|P2|Logic|ShellModule|脚本|0/0/3|ButtonSelecter_Shell|声明已清点；语义与运行待逆向|
|P2|Logic|StringCompareModule|字符比较|1/1/4|ButtonSelecter, ButtonSelecter_StringComp, HeadBar|声明已清点；语义与运行待逆向|
|P2|Logic|TimeStatisticModule|耗时统计|0/0/2|TimeConfig|声明已清点；语义与运行待逆向|
|P2|Logic|TriggerModule|触发模块|1/0/2|EnumerationTrigger|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSC2CMeasureModu|圆圆测量|46/5/15|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, ROISelecter_Com|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSEdgeWidthFindModu|间距检测|28/3/27|Boolean, Enumeration, Expand, Float, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSHistToolModu|直方图工具|24/5/16|Boolean, FloatBettween, GroupLinkItem, ImageViewConfig, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSIntensityMeasureModu|亮度测量|15/5/12|Boolean, FloatBettween, GroupLinkItem, ImageViewConfig, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSL2CMeasureModu|线圆测量|50/5/15|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, RadioSelecter3, ROISelecter_Com|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSL2LMeasureModu|线线测量|37/5/18|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, RadioSelecter, RadioSelecter3, ROISelecter_Com|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSP2CMeasureModu|点圆测量|14/4/10|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, ImageViewConfig, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSP2LMeasureModu|点线测量|20/4/11|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, ImageViewConfig, RadioSelecter, RadioSelecter3|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSP2PMeasureModu|点点测量|14/4/8|Boolean, ButtonSelecter, Enumeration, Expand, FloatBettween, ImageViewConfig, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSPixelCountModu|像素统计|9/5/9|Boolean, FloatBettween, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Measurement|IMVSPixelCountModuVA|像素统计|6/5/8|Boolean, FloatBettween, GroupLinkItem, ImageViewConfig, IntegerBettween, PixelModuRangeConfigList, ROISelecter|声明已清点；语义与运行待逆向|
|P2|Recognition|IMVS2dBcrModu|二维码识别|23/3/12|Boolean, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter, String|声明已清点；语义与运行待逆向|
|P2|Recognition|IMVSBcrModu|条码识别|23/3/12|Boolean, Expand, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, ROISelecter, String|声明已清点；语义与运行待逆向|
|P2|Recognition|IMVSMachineLearningClassifierModu|ML分类|25/3/7|Boolean, GroupLinkItem, ImageViewConfig, IntegerBettween, MLClassifyData, OpenFileForCNNDialog, ROISelecter, String|声明已清点；语义与运行待逆向|
|P2|Recognition|IMVSOcrModu|字符识别|38/3/16|Boolean, Enumeration, Expand, GroupLinkItem, ImageViewConfig, Integer, IntegerBettween, OpenDialogF, ROISelecter, String|声明已清点；语义与运行待逆向|
|P2|SplitCombination|IMVS2dArrayCorrectModu|二维阵列|2/1/6|ButtonSelecter, ImageViewConfig, Integer|声明已清点；语义与运行待逆向|
|P2|SplitCombination|IMVSBoxFilterModule|Box过滤|1/3/5|ButtonSelecter, Float, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|SplitCombination|IMVSBoxMergeModu|Box融合|7/3/5|ButtonSelecter, Enumeration, Float, Integer, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|SplitCombination|IMVSBoxOverlapCalculationModu|Box重叠率计算|0/2/3|ButtonSelecter, RadioSelecter|声明已清点；语义与运行待逆向|
|P2|SplitCombination|IMVSDivideImageModu|划片拆分|4/3/6|GroupLinkItem, Integer, ROISelecter|声明已清点；语义与运行待逆向|
|P2|SplitCombination|IMVSMultiLabelFilterModu|多标签筛选|0/3/3|ButtonSelecter|声明已清点；语义与运行待逆向|

## 每项验收顺序

1. 确认二进制身份、注册信息、版本、资源和依赖，追踪托管到原生的调用入口。
2. 记录参数默认值、范围、显隐、输入输出类型、数组、订阅和持久化语义。
3. 跟踪真实执行路径、状态机、错误、取消及资源释放；未知项保持未知。
4. 实现插件算法、配置模型与专属内容控件；所需公共框架在本 worktree 实施，并将契约/迁移/状态写入 ../CONTRACT-REQUESTS.md，同步主框架，避免重复修改共享文件。
5. 用原始程序与重建程序的同输入结果、模式界面和生命周期验证，记录差异。
6. 完成跨节点回归并启动程序供人工验收；验收后更新 HANDOFF.md、中文提交并推送本分支。测试通过不能单独证明原生等价。
