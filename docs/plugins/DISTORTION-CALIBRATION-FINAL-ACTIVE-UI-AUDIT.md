# 畸变三页活动UI最终有界清单

2026-10-19。仅静态复核当前生产源码/原资源，未新构建或原厂运行。45个展开控件（包括两Group、EdgeThresh两叶及底部命令）、16种Operation、8个Display对象全部逐项记录于final-active-ui-inventory.json。当前C专属没有新确认的活动未实施成员；完整工具仍等待活动编码识别R04共享整合，不能仅因本表全有consumer宣布原生全域bit parity。

IsAbandon是三个节点的属性，不是CustomVisible同义词。此处三节点同时初始隐藏、没有其它活动节点显示它们，native setter成功忽略；不从属性名猜算法初值。真实engine188=2已由构造asm/表核认。相反SaveCalibPath、POINT/XY、OutputPostCorrectionEnable、PointErrorThreshold、WeightFun/WeightFactor、StopExecute虽有隐藏初值，均有具体触发/命令消费者，仍计必需活动。DistThreshold/SampleRatio无显现setter、只低层Weight4消费，模块忽略4，归额外域而非活动缺口。

| 页/区域 | 原控件 | 分类 | 当前生产链 |
| --- | --- | --- | --- |
| Tab_Basic Params | ImageSource | 条件/普通活动 | SubscriptionControls→ConfigurationSubscriptionValidation/WorkflowConfigurationSubscriptions→InputImage；IMAGE通用同cardinality运输，runtime OwnedMono核Mono8。 |
| Tab_Basic Params | RoiType | 条件/普通活动 | RoiControls.Register(InputImage)→RoiService lease；SingleBox/FullScreen与旋转mask→Runtime.Crop/Recognize；关闭Dispose。 |
| Tab_Basic Params | SaveCalibPath | 条件/普通活动 | 初始隐藏但底部SaveFile的ExcuteMethod调用，Provider.Save→ArtifactSession.Export；不是废弃。 |
| Tab_Basic Params | CalibPathName | 条件/普通活动 | CalibrationPathControls→Provider验证/Apply→Runtime FileLifecycle自动写入；当前19float，不把平台.sol编码移入工具。 |
| Tab_Basic Params | RefreshFileEnable | 条件/普通活动 | ParameterAdapter→Runtime WriteAutomatic(refresh)，覆盖/不覆盖及合法后缀I/O错误typed映射。 |
| Tab_Run Params | CorrectCenterPointInput | 条件/普通活动 | State Radio+Visible/DoInit→ConfigurationUserData；共享PointAlias唯一两叶，切换不丢Y。 |
| Tab_Run Params | CorrectCenterPoint | 条件/普通活动 | POINT tree+Set/GetCombination→PointAliasConfigurationSubscriptions投影X/Y；数组允许。 |
| Tab_Run Params | CorrectCenterPointX | 条件/普通活动 | float tree+Set/GetFilter→Graph deferred/index/All→scope reader/context→首项；X→Y错误门。 |
| Tab_Run Params | CorrectCenterPointY | 条件/普通活动 | 同X；Y031b不更新，空数组0314失败保旧center，Y成功且X缺则X0。 |
| Tab_Run Params | CorrectModel | 条件/普通活动 | ParameterAdapter→Runtime SolveDetailed→模型1/2/3专属拟合与反投影核。 |
| Tab_Run Params | DistortionEvaluationEnable | 条件/普通活动 | State VisibleOperations→Runtime evaluation分支；控制阈值/校正定位UI。 |
| Tab_Run Params | OutputPostCorrectionEnable | 条件/普通活动 | 条件隐藏可显现；Runtime correctedPoints控制校正图定位；不是只影响显示图开关。 |
| Tab_Run Params | PointErrorThreshold | 条件/普通活动 | 条件显现→Runtime rounded点误差/Status比较；Display逐index状态颜色。 |
| Tab_Run Params | CalibBoardType | 条件/普通活动 | State显隐circle/filter→Runtime Recognize→共享BoardSolver1/2/6/7/8/9；R04仍真实依赖。 |
| Tab_Run Params | HomoFreedom | 条件/普通活动 | ParameterAdapter→SolveDetailed/PointEvaluation自由度；Algorithm默认1覆盖Tab显示默认2。 |
| Tab_Run Params | GrayContrast | 废弃元数据 | IsAbandon+初始隐藏；module21560成功忽略；产品保留元数据不作算法额外实现。 |
| Tab_Run Params | FilterStatus | 条件/普通活动 | 板型条件显示→Runtime Recognize共享Filter路径。 |
| Tab_Run Params | SubpixelWinAuto | 废弃元数据 | IsAbandon+初始隐藏；module忽略，engine188真实默认2；mode1仅额外SDK候选。 |
| Tab_Run Params | SubPixelWindowSize | 废弃元数据 | IsAbandon+初始隐藏；module忽略，非Pyramid maxExponent。 |
| Tab_Run Params | Circularity | 条件/普通活动 | 板2显现→Runtime Recognize→CircleCandidates圆度源比值；Algorithm默认50覆盖Tab60。 |
| Tab_Run Params | EdgeThresh | 条件/普通活动 | IntegerBettween模板→两Integer叶提交；不创建单个伪参数。 |
| Tab_Run Params | EdgeThreshLow | 条件/普通活动 | 0..255范围由源模板/descriptor→Runtime Recognize→CircleEdges阈值。 |
| Tab_Run Params | EdgeThreshHigh | 条件/普通活动 | 同Low；源IntegerBettween约束与当前执行参数分别保留。 |
| Tab_Run Params | CircleBoardCircleMode | 条件/普通活动 | 板2显示→Runtime Recognize dotMode→黑白反相。 |
| Tab_Run Params | (unnamed) | 条件/普通活动 | Expand→State VisibleOperation显现WeightFun。 |
| Tab_Run Params | CalibBoardWeightFun | 条件/普通活动 | Expand条件显现；UI1/2/3→拟合核；模块字符串首字符4成功忽略，低层4非活动域。 |
| Tab_Run Params | WeightFactor | 条件/普通活动 | WeightFun2/3且显现时可调→拟合IRLS；源码历史越界prefix按安全适配另列。 |
| Tab_Run Params | DistThreshold | 条件/普通活动 | 初始隐藏且无显示setter；低层Weight4参数，模块UI不开放4；元数据保存非活动算法缺口。 |
| Tab_Run Params | SampleRatio | 条件/普通活动 | 同DistThreshold，低层Weight4消费。 |
| ResultShow | CalibPoint | 条件/普通活动 | ImageViewConfig→ImageObjectControls/ObjectStore→Graphics点样式/IsDisplay→实际Presenter与ROI DrawOverlay。 |
| ResultShow | ErrorText | 条件/普通活动 | 同上，对Show Evaluation Error Text具名对象样式。 |
| ResultShow | ErrorTextFontSize | 条件/普通活动 | FeatureStore(FontSize)→BuildNamedTexts→Presenter/DrawOverlay GlyphRun实际字号。 |
| ResultShow | Content | 条件/普通活动 | TextDisplayGroup StringConfig→NamedTextObjectService FeatureStore→ToolDisplayService文本格式。 |
| ResultShow | OKColor | 条件/普通活动 | TextDisplayGroup ObjectService→Show Text legacy设置→状态颜色。 |
| ResultShow | NGColor | 条件/普通活动 | 同OKColor NG分支。 |
| ResultShow | CharacterSize | 条件/普通活动 | TextDisplayGroup Show Text FontSize→Feature/legacy→真实DrawText。 |
| ResultShow | Transparency | 条件/普通活动 | Show Text opacity→legacy设置→绘制Brush opacity。 |
| ResultShow | PositionX | 条件/普通活动 | GetFrontAndSelfFilterTree/SetResultIO/GetSelectedDisplayFilter→DisplayInputBindings→DisplayService坐标。 |
| ResultShow | PositionY | 条件/普通活动 | 同PositionX；源绑定与动态显示运输由共享消费者。 |
| BottomExcuteButtonsLeft | Button_ResetAlgoParams | 条件/普通活动 | Provider ResetParameters→确认→Atomic reset callbacks；保留UserData/Display字段。 |
| BottomExcuteButtons | SaveFile | 条件/普通活动 | ExcuteMethod(SaveCalibPath)→Provider.Save显式文件导出与成功后路径更新。 |
| BottomExcuteButtons | ContinuousExecute | 条件/普通活动 | ExcuteMethod ContinuousSelfExcuteModule→ConfigurationExecution.StartContinuous→宿主busy/readiness。 |
| BottomExcuteButtons | StopExecute | 条件/普通活动 | 条件隐藏/执行状态显示→StopAsync；不能因CustomVisibleFalse排除。 |
| BottomExcuteButtons | Execute | 条件/普通活动 | ExcuteMethod SelfExcuteModule→ConfigurationExecution.RunOnceAsync；主宿主调度。 |
| BottomExcuteButtons | (unnamed) | 条件/普通活动 | Bottom Confirm→ExcuteMethod CloseParamsTabControl→Session.RequestClose→宿主关闭Dispose。 |

16种Operation逐一对应：VisibleOperation/HiddenOperation/DoInitOperaion由ToolConfigurationState.Operate处理；GetFrontParamItems/GetFrontCombinationTree/GetFrontFilterTree/GetFrontAndSelfFilterTree由SubscriptionControls查询；SetCombinationSource/SetFilterSource/SetResultIOToModule及GetSelectedCombination/GetSelectedFilter/GetSelectedDisplayFilter由Provider dispatcher→共用订阅权威读回（图像/POINT/两叶/显示坐标域分别解析）；GetDisplayObject/GetDisplayObjectFeature由Object/ImageObject/Feature桥；ExcuteMethod由State映射Run/Continuous/Stop/Close/SaveCalibration后Provider执行。Reset按钮由标准控件派发ResetParameters，不要求原XML显式Operation。

原CalibBoardType每分支还指向不存在的ThreshType，原资源没有此控件，产品State对缺失目标可出MissingTarget诊断；本清单不凭该悬空原目标发明一个阈值控件。Host21此前检查的是实际视图template diagnostics，不等于所有State原悬空target都不存在；两者严格区分。目前没有确认它影响本工具算法/可见必要控件。

| 原Display对象 | 生产者→消费者/配置 |
| --- | --- |
| InImage | Runtime捕获当前有效输入；ToolDisplayImageLayers按ObjectStore IsDisplay；原图尺寸/Mono桥。 |
| Display Post-Correction Image | Runtime Warp产生PostCorrectionImage→LastDisplay桥PostCorrectionImageData→ImageLayers；初始IsDisplay=false仍可配置。 |
| Display Calibration Point | CalibPointX/Y/Status/Num→Graphics；每point.Color→Presenter与ROI DrawOverlay，path连线保原色。 |
| Show Evaluation Error Text | roundedCalibPointError+位置/Status→BuildNamedTexts→实际GlyphRun字号/颜色。 |
| Show Text | ModuStatus/EstimationError+TextDisplayGroup Content/颜色/字/opacity/位置绑定→DisplayService文本。 |
| ROI | Crop输出DetectCenterX/Y/Width/Height/Angle→Graphics rect；单Box/全屏；旋转mask已实施。 |
| Data Record | DisplayDefinition ModuStatus/EstimationError→Frame记录consumer；没有活动named-visible编辑器，不将裸ObjectStore API覆盖全部legacy语义。 |
| Result List | DisplayDefinition EstimationError列→Frame结果列表；同上legacy边界。 |

生命周期：Provider读真实Node.Parameters合并Algorithm后端默认，State.Initialize触发源链；PointAlias权威两叶在共用document订阅管理；ROI Register(InputImage)→实际lease挂1000图→更新ROI→本轮失败→删除ROI恢复，实际Host21已覆盖。Session关闭先移execution.Changed、roiControls.Dispose，再DisposeAsync停止执行；公共宿主detach viewport并清OverlayRenderer/lease，Host实际断言。Scope State.Dispose清Artifacts/Input/Output；失败状态发布门保留原旧字段但未发布，Plugin成功→失败→恢复与首次失败覆盖。整方案JSON保存Node参数/ROI/样式/绑定，重开后实际workflow run重挂订阅/图像，Host21覆盖。平台.sol私有编码另列依赖，不声称原字节方案往返。

R04整合后的最小复验（暂不申请managed）：先核新共享源hash/真正生产接入；串行跑DistortionCalibration.PluginChecks，复用原board7/9×模型1/2/3六组对19float、168raw/corrected点、双evaluation、roundederror/status的既有严格比较；再HostChecks21，复用三页template、1000图ROI编辑→失败→恢复、实际Pen/GlyphRun、样式整方案重载与关闭释放。若decoder改变原索引/XY，则出现差分必须回R04源码解决，不能改fixture/阈值掩盖。Center54/alias14已验证且此变更不触合同，可不重复；不改数值核故不重跑大Pixel/12File。人工验收另列，不计完成前置。


2026-10-19 R04真实生产整合最终补正：见DISTORTION-CALIBRATION-R04-FINAL-EXIT.md。当前Plugin原6/6、RotatedROI151、实际Host21/21均退出0，已解除此前明确共享R04阻塞；当前未发现已确认源码尚未实施的必需活动路径，交根线整工具计数审计。原始近零/历史prefix安全适配差分、额外SDK mode1与未新增Refined=true原厂运行边界保留。最终新manifest为analysis/distortion-calibration/r04-production-final/validation.json；旧pending结论仅历史。
