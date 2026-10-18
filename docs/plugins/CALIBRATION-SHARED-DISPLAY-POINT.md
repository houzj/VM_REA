# 标定公共显示、点绑定与图像转换

本轮用于 N 点标定、N 图像标定、标定板标定和相机映射四个独立工具。设备测试不在范围内。通过托管检查不等于原厂桌面或原生 ABI 完整等价。

## 原厂依据

- `analysis/decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml` 的 ImageViewConfig 模板：对象眼睛、颜色、透明度与线宽；`Apps.XmlParser/Object.cs` 的 StrokeThicknessConfigEnabled 类型表。实际证据根在项目外的 `D:/MyWork/REA逆向测试/analysis`。
- `VMControls.WPF/ModuleResultView.xaml.cs` 的对象显示与逐点提示，`VMBaseControls/Shape` 的 PointShape、ArrowPathShape、CoordinateShape、RectShape。路径取每段 0.2～0.8 部分、固定图像长度箭头；点十字随线宽及显示缩放调整。
- `Apps.Data.dll` 的 Path.GetArrowPath：不足两个点不构造路径。REA 成员证据 `ev_6b090bcc7e8e2d75e7f306578daba8bb1dfa0a5f253668b95a297c0ead3e2ff6`，方法 token `06000369`，IL SHA256 `cd73f5c5097b1e7c61081611fb469d37ef6276a6d04858d0f9de78540457a9a9`。CLI 反编译输出位于本工作树 `analysis/display-object-source`，不作为 REA 运行结果冒称。
- Object.GetValueList 及 FeatureBase.GetMapIO：对象数量来自实际映射的输出元素数，不能将 Num 的固定 Value 当成绘图数量。保留原 XML 重复的 Data Record 定义；不擅自合并为一个对象。
- POINT 组合与分量订阅依据见 CAMERA-MAPPING-POINT-ALIASES.md。组合操作拆写分量，单独修改 X 不应抹掉 Y；原厂连续写入的失败行为不构成托管原子事务的依据。
- `MVD_Algorithm` 的 0x1807ff3c0 归一化常量 0x18162b69c 为 float 1.0；0x1807fcf60 初始设计矩阵进入 0x1808580e0/0x1807075e0，IRLS 的 0x1807fd4e0 则把正规矩阵送入该核。初始、迭代不能使用同一双精度消元近似。末端反归一化必须以完整 float 行列式/伴随矩阵求 Tw 逆，不能使用 1/scale 和原中心的解析简式；四变体隔离证据见 n-image-float-inverse-evidence.json。

## 公共实现与接口

- ToolDisplayObjectStore 保存独立 `DisplayObjectSettings`，严格回读宿主接受值；ToolConfigurationImageObjectControls 适配原 XML 的操作名，复用 VisionUi 的 RoiDisplayStyleEditor。路径不增加原厂未提供的线宽设置。
- ToolDisplayGraphics 从源定义、真实输入输出和样式生成 point/path/coordinate/rect；ToolDisplayPresenter 消费几何并清除旧结果。样式状态不写入算法参数 CurValue。
- IToolPointInputAliasProvider 声明组合与 X/Y 真实端口；PointAliasConfigurationSubscriptions 以两条叶订阅为唯一持久化来源。混合来源显示只读结构，保持两侧真实身份；旧组合绑定在编辑时迁移。
- IToolConfigurationSubscriptionBatchService 和 GraphEditorDocument.SetProcedureInputSubscriptions 提供一次验证、一次撤销事务。这是托管安全设计，不声称复现原厂逐次写失败副作用。
- CompoundOutputFields 显式投影 POINT.X/Y；数组使用现有 NumberArray 向量传输。普通执行和配置自运行都必须使用此投影，不能只在选择器中展示可订阅项。
- VisionCore.ImageColorConversion.ToMono8 提供 RGB24 的 `(76R+150G+30B)>>8`，支持输入行跨度；与文件解码器的灰度策略分开。N 图像标定原厂彩色/显式 Mono 对照见 n-image-rgb-evidence.json。

## 验证与边界

本工作树 `analysis` 下保留实际输出与截图：GraphicsChecks 23 项、PointAliasChecks 25 项；NPointSolver.Checks 的 404 个原厂案例、365 个矩阵、11066 项字段检查在初始浮点修正后无意外失败，仍有 24 个明确记录的差异字段与 788 个病态/近似并列观察。不得将这些差异计为精确通过。

标定板原厂 80 圆点曾触发托管 0x10103007；修正初始浮点路径后成功，完整 float 逆修正后矩阵最大绝对差约 3.13e-9（原始 JSON 十进制量化量级）；误差统计也恢复 float 逆投影、sqrtf 和左结合累计，默认误差对应原厂 7.76673478e-5。该结果仅说明相同原厂点输入的求解修复，不能证明圆点检测或亚像素算法等价。

仍待闭合：公共外置 ROI 会话桥接、完整原厂桌面缩放/高亮交互、原生订阅索引 ABI；N 图像标定三旋转文件的矩阵微差已由完整 float 求逆修正解决，恢复原 1e-4 正式断言并新增 9/9 float 位相等断言；9+0 平移原点也按 float epsilon/除法/乘减顺序恢复，官方 SDK 与静止/移动文件均9/9位相等，81/81 板矩阵分量亦相同。其余原生范围仍须按各工具证据限定。各独立工具报告保留其剩余项。
