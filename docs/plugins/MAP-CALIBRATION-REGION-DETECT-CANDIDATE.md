# Map Region互操作候选与Detect正常路径修正

2026-10-19。状态：供根线审查，产品src冻结中，未应用、未构建。

## 原证据及Detect补丁

原模块SHA658d63e2c4e2378e7eb1776d27b966990c0d0e01e29ca57ed82fe9acc740fde8。ctor1f190建立两份stride0x128 image-member；6f80清member+a4..b4，ProcessOutput220b0读this+1695c..1696c、第二份+128。RoiFix22f40写SDK shape而非这五字段。静态dump在d4-detect-writer/module-disasm.txt，相关导出索引见D4-DISPLAY-REGION-MEMBER-AUDIT。

新增自有原厂模块进程d4-detect-box-oracle/image-results.jsonl：三轮均SetParam0/Process0；正常全图168点，双非零normalized ROI(.5,.5,.8,.8,0)154点，同ROI且提供Roi1/2五标量仍154点。每轮字段before/after及10次Detect输出均为0；calls无Roi*读取。不是仅用识别通过推断成员；同时被动读取真实字段和原ProcessOutput。provenance.json记录原件/helper/图像/输出hash。synthetic ModuleFrameIO九导入仅提供受控宿主数据；原模块代码不改，无Platform Init/设备调用。证据仅限这条正常Process链，不排除尚未取得的外部接口写成员。

候选proposals/map-region-detect/detect-member-zero.patch只将Solve成功Boxes从配置矩形几何改成两项default，保留检测ROI、状态及失败路径；git apply --check通过。Detect输出对应零框，ROI输出因此为空。显示ROI不能继续冒用Detect框；需要配置ROI图元应走独立配置对象。该修正不改变缓存或等待frame。

## Region合同候选（独立实现选择，非原生优先级）

Producer为已声明DataKind.Region的上游/宿主，consumer为Map Runtime.Image InputROI1/InputROI2。载荷沿用PortValue.Region/RoiRegionValue，读取Regions克隆；不增加共享ABI。每侧坐标必须RoiCoordinateSpace.ImagePixels，属于该侧原输入图像像素，非crop局部/世界坐标；拒绝其他空间，禁止隐式Fixture变换。

支持空列表或单个非排除FullImage/Rectangle；空列表表示本互操作合同的全图，不表示原raw解析失败空vector。Rectangle采用既有RoiDocument中心、宽高、角度，以现有检测入口进行float转换和mask/crop，不先normalize。配置同样先校验。缺端口沿用配置；出现端口时，只有配置为空/FullImage或双方同几何才接受。配置为非全图且输入不同（包括输入空全图）返回明确RegionConfigurationConflict。比较Kind、CoordinateSpace、Exclusion及Rectangle五几何值，忽略Id/显示样式，不将角度360视作0，不用像素容差掩盖冲突。无多矩形/排除或其它shape隐式取首项。

生命周期：仅在现有配对求解时解析当次ModuleContext，和现有RoiGroups读取时点一致；不把Region另存session，不添加跨轮优先级/缓存。单侧到图等待不提前消费Region；调用者必须在最终配对轮提供所需绑定。既有配置更新也在该轮生效。若要Region随第一张图跨轮锁存，属于额外宿主合同，不能猜原生行为。错误不得触发legacy解码fallback，不主动清历史display。

验收：adapter纯逻辑覆盖端口缺失/空/全图/单矩形、双侧独立、同几何不同Id、非全图冲突、排除/多形/错误坐标；产品集成复用已证ROI图像fixture，检测点/H保持原结果且Detect五数组两项0；显示资源测试验证不制造检测框。追加半帧→配对轮Region选择测试以固定上述独立生命周期。

## 整工具剩余必需工作

应用经审Detect+Region专属补丁并跑实际生产测试；DisplayChecks验证真实资源消费；复用正常图/ROI生产差分确保检测未退化；审配置资源、运行输出/缓存当前矩阵并更新manifest。Frame等待/失败保持已证现状，未取得Platform无回调时显示历史规则单列未知，不用主动清空替代。原厂未支持CPU后端仍明确边界，不继续无限采叶。人工验收另列，不阻塞用户规定的源码+实施+自审完成标准。

## 审查后准备状态

根线已批准上述独立规则及配对求解轮读取生命周期。完整Runtime候选region-runtime.patch（含新专属helper）普通git apply --check通过；Detect候选独立以便审查。RegionChecks仅ProjectReference生产Map DLL，不Compile候选helper，反射只定位生产内部Resolve做边界检查；实际Plugin两次求解复用frame0/4，对比配置ROI baseline与Region输入，检查首轮冲突Region不锁存、最终轮Region生效、source矩形/target全图独立、两项Detect零及目标侧显式冲突。未构建/未运行，src冻结待根线释放。
