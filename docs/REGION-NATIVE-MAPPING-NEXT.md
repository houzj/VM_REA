# Region / Polygon / Fixture 下一批最小闭合方案

2026-10-17。本文为证据约束下的设计，不表示产品已实现；没有修改产品代码或运行原生 VM。

## 三个概念必须分开

1. **通用运行结果**：`IVmROIPolygon` 的 Num / Points；`IVmROIAnnulus` 的中心、内外径、StartAngle / AngleExtend；`IVmFixture` 的基准和运行姿态。
2. **模块 XML 名称**：Blob 的 BlindPolygon、RegionCopy 的 ROIAnnulus、输入/输出不同的 Fixture 前缀。这是模块 schema，不能推广成全局类型名。
3. **配置文件 / ROI 序列化结构**：`VM.PlatformSDKCS.RoiPolygon`、`RoiAnnulus` 是另一路归一化结构，不能当作运行结果 ABI。

## 新增精确源码证据

| 来源 | 确认内容 | 边界 |
|---|---|---|
| `analysis/decompiled/VM.Core/VM.Core/IVmROIPolygon.cs:7..17` | Num:int、Points:IVmPoint[] | 无模块字段名称、无坐标空间声明 |
| `analysis/decompiled/VM.Core/VM.Core/DynamicOutput.cs:374..408` GetOutputPolygonArray | 从原生多边形数组逐对象构造 Polygon；每对象按 Num 分配点数组，保序 | 不能用一个无边界点数组代替 |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/Polygon.cs:10..24` | 通用结果 VertexPoints:PointF[] | 与 ROI 配置结构不同 |
| 同文件:27..73 Get/SetDataForMVDConvert | float 点传输，带点数；导出头第二项1，导入判断第二项0 | 不应机械复制为已验证双向 codec；需单独核对版本约定 |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/RoiPolygon.cs:6..21` | 注释归一化；Pack=1；nVertexNum、ByValArray32；按实际点数传长度 | 固定32声明不等于所有原生Polygon最多32点 |
| `analysis/decompiled/VM.Core/VM.Core/IVmROIAnnulus.cs:7..35` | InnerRadius、OuterRadius、StartAngle、AngleExtend | 末角不是该运行接口字段 |
| `analysis/decompiled/VM.Core/VM.Core/DynamicOutput.cs:588..615` | GetOutputAnnulusArray直接以AngleExtend构造Annulus | 可确认运行结构保持起始角+跨度 |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/RoiAnnulus.cs:6..25` | 归一化ROI结构含fStartAngle、fEndAngle | 不能直接令fEndAngle=AngleExtend；跨零/多圈语义待核对 |
| `analysis/decompiled/VMBaseControls/VMBaseControls.Base.ImageView.ROI/PolygonROI.cs:242..273` | 绘制完成：先除ControlToImageRadio，INormalizedROI再除ImageWidth/Height | 确认归一化图形绘制路径使用W/H，不是W-1/H-1；不自动证明所有算法输入同约定 |
| `analysis/decompiled/VM.Core/VM.Core/DynamicOutput.cs:559..584` | GetOutputFixtureArray：FixtureInfo→Init，UnFixtureInfo→Run，点/角/两个尺度 | 可直接确认两姿态字段顺序 |
| `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/Fixture.cs:36..47` | SDK默认构造四个尺度为0 | 当前FixturePose校验>0仅覆盖有效可逆姿态，不能无损接收全部SDK状态 |

## 最小数据模型（建议，非原生API声明）

- `NativeCompoundSchema`：ModuleId/Version、XML输出根名、NativeStyle、Visible/CanSubscribe、Cardinality、按源码顺序的 children。类型与模块实例的名称分离。
- `PolygonResult`：不可变有序 Point2[]，声明坐标空间；批量为 PolygonResult[]。不复用受“继承区域最多16个”约束的 RoiRegionValue 作为跨轮结果容器。
- `AnnulusResult`：Center、InnerRadius、OuterRadius、StartAngle、AngleExtent、坐标空间；不预先规范化角度，保持原始值。不把坐标空间未知的运行结果强行标为归一化。
- `FixtureSample`：Reference/Run两个原始姿态，允许保存原始浮点字段；另行 `TryCreateInvertibleTransform` 检查有限/非零尺度。若暂不扩展契约，只支持当前有效FixtureValue并明确拒绝零尺度，无需伪造恒等尺度。
- 坐标上下文：ImageWidth/Height、所引用Fixture的身份和快照。投影只是数据传输，不自动做坐标变换。转换只能由显式操作执行并保留原值。

以上名称属于建议的独立托管模型，不是反编译恢复的类名。若增加共享契约需主线记录到 CONTRACT-REQUESTS，插件线不应并发修改。

## 可无损闭合与不能宣称无损的部分

**可在当前托管精度内无损**：Polygon点顺序与对象分段、Annulus原值、Fixture已允许有效姿态十叶；保存重开/复制/Group回传后保持相同值。double→native float 必然可能降精度，单独标记并做有限/范围验证，不能叫数值无损。

**不可直接无损**：混合ROI及Exclusion降成单个ROIBOX；归一化点无图像上下文转像素；旋转非均匀尺度后把环形强制还原为圆环；任意Polygon强制压入32固定顶点；SDK零尺度丢失；多Polygon展平后丢Num；配置RoiAnnulus的EndAngle直接当运行AngleExtend。

Region继承的16上限与结果累积大小是不同契约。保留16限制用于单次继承消费者，跨Group积累采用独立结果批容器和显式安全上限；不得为了积累放松全部工具输入约束。

## 跨Group累积

1. 数值叶可采用当前逐轮追加，但一个复合对象的关联叶必须原子追加：Polygon的Num与X/Y一起，Fixture十叶一起；任一缺叶/错误不得产生长度不一致的半帧。
2. Polygon每轮Counts如[3,4]、下一轮[3]，合并Counts=[3,4,3]，X/Y总长10；只允许sum(Counts)==X.Count==Y.Count。保留每轮对象计数[2,1]可重建帧，但这是独立补充元数据，不冒充native叶。
3. Region不同形状不能拼接为同一语义的ROI数组。分形状批或带判别字段的独立批，禁止静默筛选掉另一形状。
4. 中断轮只有成功完成时才提交累计；取消/失败不提交半帧；每次顶层运行清空前次累计，是否与原生所有错误边界一致仍需VM差分。
5. 原生逐叶报告模式仍须遵循源码：聚合开关分发所有叶。以上原子事务为独立一致性增强，不宣称恢复原生异常事务细节。

## 可交付测试数据及断言

| 数据 | 操作 | 必须验证 |
|---|---|---|
| P0=[(0,0),(4,0),(0,3)]，P1=[(10,10),(12,10),(12,13),(10,13)] | 导出Counts/X/Y，再回组装 | Counts[3,4]；点顺序/对象边界不变；无重复闭合点擅自添加 |
| 第二轮P2=[(-1,-2),(1,-2),(0,1)] | Group两轮累计 | Counts[3,4,3]，总点10；重开与复制相同 |
| 0、1、2个点；Counts=[3,4]但点数6；NaN；超过安全点上限 | 输入绑定/运行 | 定位明确错误，不截断/补零/越界 |
| Annulus中心(25,40)，r=[3,9]，start350，extent30 | 传输与保存 | 保持350/30；不得意外变350/380或350/-320 |
| 同上Normalized中心(.25,.5)，图像100×80 | 显式转换 | UI绘制证据下中心25/40；无图像上下文时拒绝隐式转换 |
| Fixture Ref=(1,2,30,2,3)，Run=(10,20,-15,4,5) | 十叶导出并重开 | Init和Run不交换；非默认角和尺度保留 |
| Fixture原始尺度0 | 数据查看与请求变换 | 保留或明确拒绝契约；不得悄悄变1；变换不可执行 |
| Rectangle+Polygon+Exclusion混合Region | 请求ROIBOX叶 | 有明确不支持诊断，无静默丢形状 |
| 多轮中第2轮取消/某叶失败 | 运行后检查 | 不存在Num已追加而X/Y未追加的损坏批 |

实际原生UI入口、结果树隐藏规则和同输入差分仍需验收；模型测试通过只能证明独立实现内部一致。
