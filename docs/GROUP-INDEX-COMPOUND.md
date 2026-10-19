# Group动态数组索引与复合累积

实施基线：dd9727b。完整源码路径和SHA256见group-index-compound-sources.json。原始文件保留本地analysis，不提交原始程序集与全量反编译。

## 源码对照

|原始位置|确认行为|当前恢复|
|---|---|---|
|ButtonSelecter.IndexClickCommandExecute / FocusedCommandExecute；SetFilterSourceOperation.GetBindIndex|方括号内可输入常量或VmIO路径，RelateIndex独立于RelateIO|GroupOperand可选IndexSource，旧Index常量继续兼容；选择整数来源、输入常量解除订阅、撤销和保存均接通|
|GetFrontFilterTreeOperation|索引树过滤int，仅前置来源，排除自身和内部|当前索引选择器取父级前置标量整数、输入及局部变量；不擅自加入自身/内部索引递归；核心也校验标量与执行顺序|
|VmIO.SetRelateIndex|复合IO的索引继续下发到Children|当前数值比较数组的索引单独建模；不把索引误当成比较值或普通常量。全原生组合IO字段树尚未恢复|
|VmIODecorate.IsAccumulatedOutput / ParamValueChanged|组合输出开关遍历GetAllLeafIO，累积为mode0，非累积为mode1|圆/点/Fixture以对应的强类型记录数组传递，整条复合记录字段同步追加，嵌套数组展开；原生叶字段名、ABI及全部类型不因此完成|
|ProcessGroupOutputV44 0x1800a7830，case1/2/3/6/7|mode1清除数值/字符串/bytearray等再写入，mode0保留并追加|当前已建模的数值、字符串、圆/点/Fixture可累积；图片不伪装数组|
|同方法case4/5|image/pointset不论mode都先清除|保留图片替换规则。未给现有Region/Components强行声明原生pointset映射或启用追加；这些类型仍需独立映射证据|

### 2026-10-19 累积配置到嵌套结果的调用链复核

原生链从配置字段到运行收集可追到 `iMVS-6000PlatformSDK.dll` 边界：

- `DynamicObjectConfigControl/DynamicObjectConfigControl/VmIODecorate.cs::IsAccumulatedOutput` 从 `GroupOutputParam.IOModes` 取字段模式，`nMode == 0` 映射为累积开关打开；非叶IO按 `DecorateSource.GetAllLeafIO()` 查找叶字段名。`ParamValueChanged` 对 `Owner.ItemsA` 生成最多1024项 `IMVS_GROUP_IO_REPORT_MODE_INFO`，累积写0、替换写1，复合项把同一开关展开到所有叶名，随后调 `ModuleManager.SetGroupIOReportMode`。
- `Apps.XmlParser.ParamTab/GroupOutputParam.cs::IOModes` 延迟查询 `ModuleManager.GetGroupIOReportMode`，以 `strParamName → nMode` 保存已返回的项。`VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs::SetGroupIOReportMode/GetGroupIOReportMode`、`ServerSDKAPI` 和 `NativeMethods` 把该结构传至原生 SDK；现有托管代码没有在这条配置读写链中给出 native 结果结构ABI。
- `analysis/full-audit/group-native-audit/group-report-mode-map.c` 的 `FUN_1800a2b50`（日志名 `CGroupNode::SetGroupIOReportMode`）按字段名匹配输出条目并写入模式；未匹配的模式项数量与成功匹配数不等时返回错误。`prepare-output-native.txt` 所记录的 `CGroupNode::ProcessGroupOutputV44` 依输出分类分派 int、float、string、image、pointset、bytearray 等处理函数。case 1/2/3/6/7 在模式为1时先清理旧内容后处理当前轮；case 4 image 和 case 5 pointset 无条件清理旧内容。反编译能确认这个分派与清理条件，但不能据此推断 pointset case 在所有底层类型/结构上的累积含义，也未恢复叶字段序列化 ABI。

本地独立链为 `GroupConfigurationControls.BuildGroupOutputs` 将 `WorkflowInterfaceItem.Accumulate` 的WPF复选框写入Group配置；`SolutionExecutor.Execute.Call` 每轮只在执行成功后调用 `GroupOutputAccumulator.Add`，最终 `Finish` 生成累积数组；`WorkflowCalls.From` 根据callee Group输出的 `Accumulate` 为已建模类型把调用输出签名改为数组，因此父级/嵌套调用使用同一 `WorkflowOutputs` 数据。显示累积是单独的 `WorkflowConfiguration.Displays[].Accumulate`，由 `GroupDisplayControls.BuildGroupDisplays` 配置并由 `GroupOutputAccumulator.Add/FinishDisplays` 收集；图像显示在本地与原生显示开关中均保持替换/不可累积行为。这个链路属于本地实现，当前成功结果/数组签名不能证明native ABI完全等价。

**本次可证差异与边界：** 现有本地 `DataKind` 和 `PortArrays` 没有 `PointSet`、`ByteArray` 类型；本地 `DataKind.Point` 是独立的单点/点记录数组，不能据名字当作native pointset。`Components`、`Region` 根输出也未被本地累积器接受；字段投影与插件算法输出需要先有独立原生类型/叶ABI证据。它们不属于可以单改Group累积器安全闭合的差异，本次没有运行代码修复。当前数值/字符串及已单独建模的Circle/Point/Fixture实际数据通路有之前记录的WPF/运行证据，见下文验证项；本轮仅做静态源码链复核，未启动构建或重复运行。

### 2026-10-19 Region / Components 类型生产与项目边界复核

本次将字段树/结果生产链与 Group 接收链分开核对，结论是：**当前没有发现能够单独落在 Group 累积器中的安全修复**。可见源能证明字段和 SDK 数据形状的一部分，但不能证明插件结果如何被转换为 VM Group 的动态输出 ABI。

|阶段|精确反编译证据|可证内容|尚不能推出|
|---|---|---|---|
|工具声明|`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Location/IMVSBlobFindModu/IMVSBlobFindModu.xml` 的 `Category Output/Items`|`BlobNum:int`、`TotalArea:float` 是标量；`Area`、`BlobStatus`、`CentroidPoint(POINT)`、`BlobRect(Rect)`、检测 `ROI(ROIBOX)`、`ROIAnnulus(ROIANNULUS)` 显式声明 `IsArray=true`。轮廓另含 `ContourInfo:pointset`，不是 Component/Region 根对象|XML 声明是否逐项保留到运行时端口、插件输出序列如何组织以及输出数组在 Group 中的 ABI|
|类型识别|`analysis/decompiled/VM.Core/VM.Core/DataType.cs::DataType`；`DynamicOutput.cs::GetAllOutputNameInfo/ConvertTypeName`|引擎数据枚举有 `POINTSET/ROIBOX/ROIANNULUS/POLYGON`；动态端口转换逐个把已支持的 `ROIBOX→RECT_BOX`、`POINT→POINT_F`、`ANNULUS/ROIANNULUS→ANNULUS`、`POLYGON/ROIPOLYGON→POLYGON`。方法注释明确排除 `POINTSET`、`CONTOURPOINTS` 与小写 image；`ConvertTypeName` 未给 `Components` 或 `Region` 这两个宿主复合名映射|不能把 `Components` 容器当 native POINTSET，也不能把 Region 列表直接当 native ROIBOX 数组|
|Group端口装载|`analysis/decompiled/VM.Core/VM.Core/IMVSGroup.cs::SetIoParams/GetGroupIoList/AddFilterToList`|`GetGroupIoList` 遍历 `GetAllLeafIO()` 的 `DynamicVmIO`；`AddFilterToList` 只按叶名类型写 `int/float/string/image/pointset/byte/double` 的 SDK 类型码，并将 `item.Name` UTF-8 固定为128字节|该 Group SDK 入口没有 `Components` / `Region` 根类别码，也未表明复合根按“每个 CompoundOutputFields 虚拟叶”注册；Group 叶端口声明如何与真实 native 节点结果相接仍未知|
|本机字段结果解析|`analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleResultManager.cs::SetModuleResult`、`SetPointFResultData`、`SetRectIResultData`、`SetRectBoxResultData`、`SetAnnulusResultData`|管理器按托管 `CModuleResultBase` 的结果属性类型分派解析；多结果路径覆盖 `List<PointF/RectBox/RectI/Annulus/...>`。`CentroidPoint` 用 `CentroidX/Y` 两叶；`BlobRect` 用 `BlobRectX/Y/Width/Height` 四叶；环形字段使用中心、内外半径、起始角和角延伸等并行数值列|这属于 SDK 结果重建，不等于 VM.Core 的动态 Group IO注册或累积实现；数值列对齐/缺列报错行为还需逐分支验证|
|工具结果对象/结构|`analysis/decompiled/iMVS-6000PlatformSDKCS.PublicAudit/iMVS_6000PlatformSDKCS/ImvsSdkPFDefine.cs::IMVS_PF_BLOBFIND_MODU_INFO/IMVS_PF_SINGLEBLOB_INFO/IMVS_PF_REGION`；`ObjectPlatformSDKCS/BlobFindResult.cs` 与 `BlobFindResultIn.cs`|底层 Blob 结构有 `fTotalArea`、`iBlobNum` 和指向 Blob 记录的指针；单 Blob 有 float Area、float 质心、连接域矩形。结构包装器按 `iBlobNum/iDetectNum` 复制并释放指针内存；高层 `BlobFindResultIn` 是标量输出属性集合|这些类型来自具体工具 SDK结果包装，不证明通用 Group 的组件容器传输合同；也没有能证明所有 XML 输出都由此对象生产的转换器闭环|

职责归属：Group 通用接口/循环/跨轮聚合/稳定叶路径消费者属于 **main/framework**；将工具 XML 与真实 SDK 结果映射为工具输出属性、工具编辑器与类型模板属于 **tool-plugins 的插件-facing framework及具体工具适配**。当前 main 的 `CompoundOutputFields.Describe/Project/Expand` 只是已有托管叶投影消费者；它不会产生真实原厂 Blob/Region 算法值，也不能替代插件侧解析。现有 [FCR-012](CONTRACT-REQUESTS.md#fcr-012框架复合叶端口2026-10-17) 已约定框架投影入口以及根输出保留，并要求插件扩充字段时提交真实 XML/数据依据；本轮不重复登记第二份相同合同，也不修改插件工作树。

**当前证据状态：** 源码/资源层确认了 Blob XML 的字段与基数、可见字段解析函数以及 Group 叶 IO 支持类型；本机托管投影及测试投影存在。原生实际 Group 中这些输出如何从工具 SDK 结果变成 Group 的动态节点 IO，以及 `Region`/`Components` 的根/叶报告模式、累计时序、错误回滚和 ABI 仍未知。已有独立运行记录证明本项目测试投影可跨 Group 运行，不能作为真实 VM 插件生产链证据。本轮只静态审计、未改代码、未 build、未启动 VM；无需通过测试来宣称闭合。

本次读取证据的 SHA-256：`VM.Core/DataType.cs` `DE3D1EF4DB67614DEDDA1A686A0A578CF72DF3DF42E823CF344BFFB643FC7491`；`VM.Core/DynamicOutput.cs` `F995E91E30AE21C1B17E9746D5657C4AB6FC9A868F315D4DFB9031B264DDF92D`；`VM.Core/IMVSGroup.cs` `DC772AABA433316B7D0F4DC3B0B3C80C3C63769334D7D013896AFE7B54C367D3`；`VM.PlatformSDKCS/ModuleResultManager.cs` `FD3960EE308FB739729AF43589035D628544BB5B26941BE5EB6F3250A41739E7`；`ImvsSdkPFDefine.cs` `079ECE0C0C94F67F59AF42A8DCD28E92D50BF1B576BBB97DE0E5196A8457AFD4`；`ObjectPlatformSDKCS/BlobFindResult.cs` `A73A6181055DC3D6A3C80B09597BC83A218B6B1365BC55ECEF87BA6335582C9D`；`BlobFindResultIn.cs` `9AE7CB64A42A8AF496FC6536FF1FD7915B4AED5F2918C2CE49F92B9A09AEF0F3`；安装 XML `IMVSBlobFindModu.xml` `F1B2F279717DCE3EC61C11C41F858BD100359095BCF13672422A3C73EFE47F6C`、`IMVSRegionCopyModu.xml` `1F4566DD7E81B1313445DB0BF2CD33F9ACCF72DF1867DA431E41C4149156BD7F`。这些原始证据位于本机安装/反编译目录，不进入 Git。

## UI、数据和运行链

- 索引来源独立保存，执行当前Group前解析当前父级值。整型负值、非标量、非整型和越界不读旧结果：配置类型错误在提交前拒绝；运行数据不可用记录诊断并不触发中断。此处快照时机是独立宿主策略；原生每轮订阅缓存/回调时序仍未差分。
- 依赖图把整数索引生产者也作为前置依赖。复制同时重映射数组来源和索引来源；只删除索引生产者也清除失效比较，不默默退回元素0；撤销恢复精确引用。
- 当前窗口增加索引整数来源选择入口，选择后显示引用名称；改输入数字解除订阅。该独立字段布局尚不是原生ButtonSelecter内嵌方括号/悬浮树模板的逐像素还原。
- 输出配置可新增圆/点/Fixture，累积开关更新实际调用签名，父级接口递归传播。显示结果使用紧凑几何格式，数组显示绘制逐元素展开；截图和导出代码只在tests。
- 强类型数组使用只读快照；嵌套累积展开各轮元素，图像仍为最终标量；输出和显示均使用独立100万元素资源上限。

## 验证证据的分工

- 动态索引协议检查使用测试数据生产者验证0/1/2选择、缺值、越界、复制、删除、撤销和隔离SelfRun，不将该生产者称为原生工具。
- 复合执行使用真实BMP和现有vision.circle径向找圆实现，两层Group内3轮外2轮，圆和半径数组均为6项，字段逐项匹配。点/Fixture检查使用明确标为测试用途的字段投影模块，从实际找圆结果构造记录并跨同样两层调用。没有宣称已恢复原生点/Fixture算法插件。
- 窗口运行使用真实图像源SNCode累积数组和真实找圆；父级局部变量作为索引，选择自身序号数组第1项，等待该项存在后于索引2中断；复合输出、编辑/撤销、保存及关闭累积后的实际标量结果另列操作记录。
- 所有WPF验证为真实窗口控件事件、部分宿主方法及截图目检；没有把它们称为桌面鼠标或原VM视觉差分。

## 保留边界

全原生复合字段树、混合叶报告模式、Region/Components与pointset映射、native PointSet/bytearray数据合同与实际工具生产者、其他原生复合类型、原生缓存时序、显示模板像素差分仍开放。当前圆记录含独立算法的RMS字段，Fixture采用已有托管参考/运行位姿协议；二者不是原SDK结构体ABI。图像及已证实的pointset替换路径不应为追求“全部累积”而改变。源码可证的本地 Group 标量/已实现记录数据路径没有发现可单独修复且不跨插件/ABI合同的安全差异。

## 实际截图发现及修复

首轮389项窗口事件完成后，目检发现复合结果表的第二个圆记录被裁切、已绑定数组下仍有空白常量框。结果列改为换行并保留完整值提示；仅未订阅比较值时保留常量编辑器。增加实际行高/完整提示、无空白常量框以及两圆叠加绘制检查，重新执行和目检，不以首轮通过代替修复后的结果。

本批动态索引的入口针对Group中断操作数，不代表已给所有普通插件输入端口恢复通用方括号索引编辑。该范围仍与全原生字段树一并开放。
