# Region / Components 复合叶证据复核

日期：2026-10-17。范围：原始反编译 C# / XAML、本机 VM 4.4 模块 XML，以及当前 `CompoundOutputFields.cs`。本报告未运行原生 VM，不证明算法、像素边界、浮点舍入或 UI 的完整原生等价性。未修改产品程序集。

## 结论和应保留的边界

当前 Components 字段名及 Rectangle / Annulus 的 Detect 输出路径有原始 XML 支持，未发现这些路径的拼写错误。但它们是**独立托管数据向原生字段名称的部分投影**。Region 是 ROI 定义列表，Components 是独立连通域结果列表；二者都不是已经证明与原生 ABI 同一的数据类型。原生字段的可见性、基数、分组顺序必须另行保留，不能由类型枚举推断。

### 原生数据类型与树

- `analysis/decompiled/VM.Core/VM.Core/DataType.cs`：存在 POINTSET、ROIBOX、ROIANNULUS、POLYGON 等；没有当前托管名 Region / Components。
- `analysis/decompiled/VM.Core/VM.Core/DynamicOutput.cs:767` 起：ROIBOX→RECT_BOX，POINT→POINT_F，CIRCLE→CIRCLE，FIXTURE→FIXTURE，ANNULUS / ROIANNULUS→ANNULUS，POLYGON / ROIPOLYGON→POLYGON。不能把全部复合对象降为 pointset。
- `analysis/decompiled/VM.Core/VM.Core/VmIO.cs:1041`，`GetAllLeafIO`：递归 Childs，叶节点按遍历顺序加入列表。它不是当前点分隔路径串的规范定义；当前路径串是独立可持久化寻址方案。

## Components

原始证据根路径：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Location/IMVSBlobFindModu/IMVSBlobFindModu.xml`。

| XML 行 / 节点 | 当前映射 | 判断 |
|---|---|---|
| 104 BlobNum:int | 列表 Count | 名称、标量匹配；算法筛选语义未差分 |
| 105 Area:float, IsArray=true | Component.Area → double[] | 字段存在；当前整数面积再转 double，不是原生 float 精度复刻 |
| 106 TotalArea:float | 对独立整数面积求和 | 名称匹配；累计舍入/溢出/过滤语义未证实 |
| 107..112 CentroidPoint / CentroidX,Y | Component.CenterX,Y | 类型结构匹配；像素坐标约定未证实 |
| 142..153 BlobRect / RectPoint / BlobRectX,Y,Width,Height | MinX,MinY,Max-Min+1 | 名称匹配；inclusive 宽高为独立实现约定，不是原生等价证据 |
| 113..125 MinBoudingRect | 未投影 | 正确保留未知；不可把轴对齐盒冒充旋转最小盒 |
| 129..141 MaxAreaInnerRect | 未投影 | 当前 Component 无对应值，不应补零 |
| 208..213 ContourPoints / ContourInfo:pointset / ContourNum | 未投影 | 当前 Component 无轮廓点，不可制造 |

后续可补：XML 154 起轴对齐左右上下角字段，但只能显式声明采用当前整数像素边界；Angle / Perimeter / 轴长 / 圆度等仍须实际算法产出。

## Region、Polygon、Fixture

### Rectangle / Annulus

`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSRegionCopyModu/IMVSRegionCopyModu.xml:110..136`：

- ROI.DetectRectCenterPoint.DetectCenterX/Y、DetectWidth/Height/Angle。
- ROIAnnulus.DetectAnnulusCenterPoint.DetectAnnulusCenterX/Y、DetectAnnulusInnerRadius/OuterRadius/StartAngle/AngleExtend。
- 此模块这些输出显式 `IsArray=false`；Blob XML:181..207 同名结构却 `IsArray=true`。因此当前 Region 所有叶统一数组是**托管 Region 列表聚合策略**，不能称原生固定基数。
- 当前仅全 Rectangle 或全 Annulus、ImagePixels、非 Exclusion 才产生投影，避免静默丢掉混合形状或擅自转换 Fixture / Normalized；这是可解释的支持边界。必须在选择和运行错误中说明缺叶，不能显示为零。
- 对空 Region 的 `All(...)` 结果为 true，当前会得到 Rectangle 空数组；这是独立空集合策略，原生究竟保留何种树未知。不能用其推断原生类型。

### Polygon

Blob XML:226..236：`BlindPolygon` Style=ROIPOLYGON、IsArray=true，子叶 `BlindPolygonPointNum:int[]`，以及 `BlindPolygonPoints.BlindPolygonPointsX/Y:float[]`；237 另有 `BlindPolygonString:string[]`。

这是可实施的下一批部分映射：每多边形点数数组 + 按原顺序展平的 X/Y 数组，保留分段；不可只有 X/Y 而丢失每个 Polygon 的边界。当前 ROI 顶点可供投影，但 Exclusion、闭合重复末点、方向、串格式和 native 浮点约定尚未证实。BlindPolygon 本身是模块命名，不是任意 ROI Polygon 的必然名称。应按模块 schema 映射而非全局硬编码所有名称。

### Fixture

Blob XML:66..87 输入 `Position Correction Info` Style=FIXTURE：

- `Fixtured Point.FixtureInitPointX/Y`、FixtureInitAngle、FixtureInitScaleX/Y。
- `Unfixtured Point.FixtureRunPointX/Y`、FixtureRunAngle、FixtureRunScaleX/Y。

当前 `src/VisionFramework/RoiRuntimeValues.cs:5..9` 的 Reference / Run 可表达这些十个数值叶。但 Transform 明确是独立仿射解释；叶映射存在不能证明变换的角度方向、坐标变换次序及采样与原生一致。

Blob XML:246..267 的输出 `Output Position Correction Info` 和全部子节点 `Visible=false`。输出叶带 Output 前缀，与输入叶不相同。不能把输入字段路径直接标成原生公开输出树。

## 原生模板与入口的真实可见性

- `analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/VmIODecorate.cs:144`：`IsShowChildButton => false`。
- 同文件:228 `LeafIOs` 仅选取 `GetAllLeafIO()` 的 Name；:231..247 提供 DisplayChildrenCommand；:362..367 创建 ShowChildWin、赋 DataContext、ShowDialog。
- `.../DynamicObjectConfigControl.UI/ShowChildWin.xaml:2,11..18`：CenterOwner，CancelVisible=Collapsed；ListBox 内容区 370×420，纵向滚动；行是只读、无边框 TextBox。它是叶名称列表，不是编辑叶值的树。
- `.../DynamicObjectConfigControl/DynamicIOConfigControl.xaml:17..38`：行高32，列70/68/*/Auto/42，名称 LostFocus 提交，类型只读；:64..80 累积 Boolean_sub 模板；:98..112 表头；当前导出模板没有 DisplayChildrenCommand 绑定。不可凭命令存在就凭空添加“原生可见”的子字段按钮。
- `.../Apps.XmlParser.ParamTab/GetInsideAndSelfInputFilterTreeOperation.cs:74..75`：叶过滤包含 `CanSubscribeA && Visible`。不是所有可解码字段都会在源选择 UI 公开。
- CircleFind XML:95..103：OutputCircle、Radius、Center Point、CenterX/Y 都 Visible=false。当前 Circle 托管投影可以存在，但其公开可见性是独立界面选择，不能称这个模块原生选择 UI。
- VmIODecorate.cs:175..190 / :280..310：复合输出累积开关读取首个已有模式叶，写入时遍历所有叶。当前托管整对象数组与原生逐叶报告模式并非天然等同。

## 下一批闭合建议

1. 先保留当前已证实路径，报告中明确“模块字段命名 + 独立聚合投影”；选择树区分公开 native 叶与独立扩展叶。
2. 模块 schema 携带 Visible / CanSubscribe / Cardinality / child 顺序，而非仅 DataKind 全局目录。不同 XML 同名类型的标量/数组与输出前缀不可抹平。
3. Polygon 先实现点数、X/Y 的有界组合；用至少两个不同顶点数的多边形验证保存重开、复制、Group 两轮累积后的分段一致性。字符串序列化未知则不伪造。
4. Fixture 先映射十个可表示叶并测试 Reference/Run 非恒等值，保持仿射算法差分未验证状态。
5. 原生隐藏叶不擅自加原生按钮；如需要验收调试入口，明确标为独立辅助并放在测试。
6. 原生截图和同输入输出差分仍需完成；本证据复核不能代替实际 VM 运行验收。

## 后续结构设计补充

已补充 `docs/REGION-NATIVE-MAPPING-NEXT.md`：区分通用Polygon运行结果与归一化RoiPolygon、Annulus的EndAngle与AngleExtend、Fixture原始状态和可逆变换；列出最小模型、Group原子累积分段、坐标转换边界及具体数据。设计尚未实施，不提高当前验收完成度。

## 2026-10-17 并行静态代码复审（未构建）

复审文件：`src/VisionFramework/CompoundOutputFields.cs`、`src/VisionEditor.Core/GroupInterfacePropagation.cs`、`tests/VisionWorkflow.CallChecks/CompoundLeafRuntimeChecks.cs`。以下为读取当时快照；主线后续可能修复，提交前应重新核对。

### 需要修复的真实边界

1. **投影异常逃出节点错误隔离**：`GraphExecutor.cs:20` 在模块执行 try/catch 之后直接调用 Expand。`PortValue.Components`只复制列表，不检查CenterX/Y；包含NaN/Infinity的Component会令Project中的NumberArray抛异常。结果不是单节点InvalidOutput，而是整个RunAsync异常退出。null输出字典值同样会先在Project访问Kind时NullReference。应在投影/输出校验边界受控捕获并记录节点InvalidOutput，保留运行结果可诊断性；不能仅靠真实内置算法当前输出正常来忽略插件边界。
2. **Annulus叶与实际区域不一致**：Project发出Shape.StartAngle/SweepAngle，但`RoiGeometry.Contains`对Annulus只检查内外径，忽略角范围。Validator也不限制Annulus的Sweep，所以可创建合法start350/sweep30的Annulus：实际为全环，叶却声称30度原生Annulus。建议仅投影当前算法真正表达的整环，或对带非默认角Shape明确报告不支持，直到原生证据和独立模型闭合；不宜顺手修改整个历史ROI算法。

### 当前检查为正确的行为

- Region混合形状、Fixture/Normalized坐标、Exclusion当前不制造叶值；无结果不补零。图校验只证明字段可以引用，运行缺失后WorkflowRunContext产生Current-run source unavailable，导出失败或订阅节点失败，是可诊断路径。
- 根Group导出改名根据接口Id建立映射，派生路径按旧/新schema同名同类型同基数匹配；不匹配删除，避免把Region叶误保留到Components。
- Interface、Subscriptions、Variables、connections及GroupBreak引用都参与传播；根kind/cardinality变化由后续GraphValidator约束，不应伪造类型转换。
- 当前移除隐藏Circle/未证实Point全局投影符合证据限制。

### 测试覆盖不足

`CompoundLeafRuntimeChecks`覆盖真实BMP/面积两轮累计、BlobNum、派生叶根改名/删除、保存重开，但未覆盖：

- 投影异常节点隔离（NaN、null）。
- Region矩形→圆环→不支持/混合类型时同一schema叶的运行缺失。
- 空Region的类型模糊：All真使其投影为空Rectangle数组，不能代表原生类型已知。
- Annulus非默认角Shape和实际掩膜的一致性。
- 多层Group、下游局部变量/订阅/Break索引等各引用分支同时传播。
- Group根类型改变及标量/数组改变被拒绝时，文档撤销栈和原绑定保持原状。

这些都是建议补充的实质数据边界，不是要求镜像实现的低价值断言。
