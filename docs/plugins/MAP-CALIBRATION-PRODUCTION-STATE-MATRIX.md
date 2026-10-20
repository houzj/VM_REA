# 映射标定生产入口状态矩阵与专属修复（2026-10-19）

本轮直接核对MapCalibrationPlugin/ImageSolver/FileSession实际入口，不再扩纠错叶或base/shape。原证据复用D4文件方向、ROI所有权、MODULE-ROI-STATE及STATE-LIFECYCLE；共享R01–R04归根线。整体完成计数仍0/10；本文局部实施不计完整工具，人工验收单列且不阻挡计数。

## 原→产品及当前可触发性

| 项目 | 原厂成员/动作 | 当前产品位置 | 判定/有限动作 |
|---|---|---|---|
| 模式1/2 | Process21690按本地InputType分派；ProcessAlg21c50设置SDK模式再装配双图/双文件 | Runtime.ExecuteAsync→Image/File | 正常1/2有对应。descriptor限制1..2；直调非法mode不作为当前支持域，不按未执行分支猜返回码补丁。 |
| 无新帧 | GetInputImage203d0两标志均0则Process返回E0000314，不到ProcessOutput | Image早返Fail，ExecuteAsync强制ModuRunTime | 现有原模块/插件检查已覆盖；强制计时属已记录宿主契约，不以空writes推断平台显示自动清除。 |
| 单帧与配对 | 每侧新图更新其image/pose/pending；凑齐两侧才清pending并运行RoiFix/SDK | ImageSession两图、两pose、两pending；Image wait与pair路径 | 每侧保留最近图、独立pending有对应；本轮最小回归只走wait，避免把检测差异混入状态。 |
| 新帧缺省pose | 203d0每收到一侧新帧，先将X/Y/Angle置-999999/-999/-999，再读各可选float | Runtime.Image.Pose | **确定、正常可触发差异已修：** 旧产品缺Angle沿用previous.Angle（首帧0），现每新帧缺Angle=-999f；其余X/Y默认未变。未收到新帧则保持旧pose。 |
| 原float常量 | source角度字段+17384：20661写0xc479c000；target+17390：20921同值；随后VM_M_GetFloat可选覆盖 | 新Pose缺省与InputStateChecks逐bit断言 | 0xc479c000对应-999f；有值按float存储，缺值不继承旧角。该值进入SnapWorldCur/Obj的XML R字段。 |
| Teach | Process匹配触发→读必需物理坐标与角→设置Teach标记；pending时只输出等待，否则WriteXmlData | Image TeachPose/TeachTriggered/Save | 现有PluginChecks与ImageFileChecks可复用；不按空字符串长度猜VM_M_GetString返回的count语义。 |
| Image→File→Image pending | 同一模块pending未被纯mode更新清除；已有定向六步trace | 分开的file/image session，File不改image pending | 已有受控原模块trace确认失败File路径保留pending；本轮最小回归验证File无矩阵失败不破坏待配对pose。成功File后Save/旧image显示属于更细共享SDK结果可见性，不能从失败File外推。 |
| 双ROI配置 | 正常type1生成全图rect；其他合法rect按图宽高换算；RoiFix非空首shape进双setter | Runtime.Group两组，ImageSolver.Detect值式单矩形/全图 | 当前合法配置空group作为全图，与正常type1语义对应；不能为模拟解析失败后的空native vector而让正常取消ROI保留旧框。 |
| 错误raw ROI报文 | SetParam对非null载荷先Reset raw，再解析；失败后RoiFix可空vector跳setter保留旧SDK clone | 产品无暴露原raw二进制SetParam报文API | 属额外原生API边界，不把它当RoiDocument合法空group。未制造不安全裸指针或添加缓存猜实现。 |
| Region输入线 | 原XML InputROI1/2是ROIBOX组合的Center/Width/Height/Angle标量订阅 | descriptor声明Region端口；Runtime实际读RoiGroups/Rois，未直接消费Inputs[InputROI1/2]；GraphExecutor把Inputs与RoiGroups分别传入 | **当前可触发接口空隙：** 直接接Region端口可能未生效。但原结构标量订阅→raw ROI及宿主ROI控制器绑定优先级未在此闭合；不能自行猜Region优先于/合并到group。根线需审明确adapter合同/接入位置，本轮不改共享合同或暗设优先级。 |
| 不支持ROI | SDK setter接受rect/circle/annular；模块RoiFix实际只处理raw rect/annular分支；原配置选择器支持面与SDK接口不同 | descriptor和Runtime只支持一个包含矩形或全图，reject exclusion/其他shape | 显式产品支持边界；不得仅因SDK header有circle/annular就开放当前UI或假造其栅格语义。 |
| 文件路径/刷新 | ReadFileMatrix可先替换缓冲后报错；成功SetParam才提交路径；Process Refresh失败提前返，不进SDK Run | FileSession.SetInputPath Replacement即提交、成功才paths；Runtime.File按次序early return | 已有File/Session/Plugin回放覆盖，复用，不重写reader。Save输出路径空值拒绝与保存IO返回0边界保持。 |
| 算法结果失败 | SDK Image失败GetResult空，模块写0/0；文件read失败早返仍可保留旧SDK结果 | Image结果失败session.Result=null；File Run清LastResult，setter失败不进入Run | 与现有受控原模块失败trace对应；平台订阅历史值/显示是否清除独立于这里的缓存与writes。 |
| Image输出 | ProcessOutput220b0生产状态、三组点/计数、ratio/error/scale、检测框等 | Runtime.ImageResult、ImageSolver.SolveBoards | 点/矩阵核心仍受共享R01–R04边界影响，不在此重做。ImageSolver当前Boxes取RoiDocument；原模块Detect*来自image描述块，不可凭ROI几何认定producer相同，须定向对照，未猜修。 |
| File输出 | 文件分支只写状态，不把SDK ratio强行变成模块动态端口 | Runtime.File成功/失败仅ModuStatus/CalibStatus | 已有回放对应；ModuRunTime作为强制框架字段额外加入。 |
| 显示 | Display.xml以目标图InImage2、计数/比例/误差/点绘制；CalibOriPoint*只有资源引用，未有已查生产者 | DisplayDefinition/LastDisplay及原资源控制器 | 目标图绑定有对应；不造原点轴数值。等待/失败LastDisplay结果可见性不可等同真实VM历史输出，单列未知。 |
| reset/释放 | DeInit销毁工具及ROI clones，尾跳ResetMember；重复Init还可能装载默认配置 | plugin.Reset移除standalone cache；scope.State的销毁由共享执行scope拥有 | 不给Map另建第二套scope生命周期；公共session销毁/真实宿主重置由根线对应。 |

## 专属补丁与最小回归

`MapCalibrationPlugin.Runtime.Image`将 `Pose(previous)` 改为当前帧 `Pose()`，缺少WorldRotateAngle时固定-999，去掉旧angle继承。仅改该生产路径，不修改共享NImage/Contracts或原生payload。

新增 `tests/MapCalibration.InputStateChecks`：每侧独立节点反复只输入一张2×2图，永不凑齐图像对，不触发检测；验证显式pose→全缺省→部分pose→漏Angle四轮、哨兵float位、无帧早返、File模式失败保留pending/pose、等待无合成display，以及XML R字段为-999。测试读专属私有session仅用于验证缓存，不向产品注入测试钩子。

构建采用独立 `analysis/map-calibration/d4-production-state/artifacts`，根线串行授权。最终结果与hash在同目录验证记录；首次测试编译缺System.IO已修，不隐去该失败。没有原厂运行或设备测试新增。

## 整工具最小剩余

已确定的新帧angle差异完成专属修复；其余分别归当前可触发的Region接入空隙/检测框producer待证、额外raw报文API边界、共享R01–R04及scope生命周期。下一应按这些明确成员实施和自动验证，不重新泛化全部SDK函数或将人工验收作为计分门槛。

验证结果：独立artifacts增量构建退出0，0警告/0错误；InputStateChecks PASS48、退出0。记录与文件hash见analysis/map-calibration/d4-production-state/verification.json。构建槽已交还根线。

