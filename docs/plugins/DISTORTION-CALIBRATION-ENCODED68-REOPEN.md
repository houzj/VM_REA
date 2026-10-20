# 畸变编码板6/8重开与有界复验

2026-10-19。此前7/9六模型、151旋转ROI、Host21证据仍有效，但不覆盖资源中活动板型6/8。已重开整工具退出；本片不自行调整根线全局计数。

源链与影响：Runtime Execute:249调用同一Recognize(source,Crop,context)。6/8与7/9一样由CalibBoardType枚举直接可选。原core7f69e0→7e4ff0→815560按core kind6→80f9a0、kind8→80cfc0分派；现R04桥曾将6/8套7/9的hole-search，完整无洞15×15/13×13源grid不满足该proposal语义，导致错误失败/错误身份。该缺口影响三模型的raw点生成、grid来源/顺序、拟合文件和所有后续评估，不是“低层额外API”。原corrected wrapper7f7450:160-201继续取同engine类型/模式，经7e4ff0或mask7e56b0，再次受6/8分派影响。

当前产品第二个具体接缝：axis Runtime helper传region.Image至DistortionCalibrationRecognition.RecognizeCropped(crop,origin)，其Board调用走无original-source的RecognizeLocal；encoded decode可能提前用local坐标/像素。共用Board已有source+crop版本正确先投影原图网格再解码。提供两文件最小candidate：adapter新增source+crop重载调用该Board方法；Runtime axis分支传source首参。旧裸crop入口保留独立兼容，不做全局改变。Mask分支已传source，无需再改；正确图再次检测复用同一helper，自动获得相同运输修复。源证/候选均在本目录，产品只由rootbatchapply。

独立oracle：复用原Board官方SDK抓取calibration-board-encoded-{full,axis}-output-fixtures.jsonl与encoded-roi-rotated-fixtures.jsonl；不是调用Board产品互相比较。图像为已有hik-generated-2-large.png(type6)与hik-generated-4-large.png(type8)，各原SHA来自encoded-roi-rotated-evidence。测试项目DistortionCalibration.Encoded68Checks：每type full1/axis3/mask2（原indices0,1），共12原case逐点GridID/原序/XY floatbit比较。axis与mask用reflection进入本工具真实private Crop→Recognize helper，防止只测孤立adapter漏掉Runtime接线；GridPitch始终1，不能套其他工具物理pitch。

--runtime-smoke另在两个相同原图执行本工具model1/Weight1、Evaluation+OutputPostCorrection=true，走solve→warp→corrected-redetection→发布；仅确认真实链成功与输出存在，不把产品自己生成的corrected图/点当原厂golden。没有原C6/8 corrected oracle，故不声明这两个分支完整原生校正数值parity。当前无需额外原厂运行：先用12已有oracle定位识别、2smoke核真实可达，若差分只剩corrected数值再提具体单case原SDK问题，不泛化跑新样本。

根线/D先完成共享6/8cell proposal/anchor/prefix边界并跑原Board axis/mask；C取得槽后串行上述Encoded68(+smoke)→既有Plugin6/Rotated151/Host21必要复验。新原oracle失败先报firstDifference/count/code及前三点，不扩大容差/重写fixture。没有修改共享产品，没有构建或运行原厂。

首轮测试自身有两处修正：输出报告保留JsonElement需Clone避免原document销毁；RoiDocument第7位置参数是Radius，旋转角现显式命名AngleDegrees。此前mask5°实际169/原153差异来自该测试未将角传入实际ROI，不可登记为已确认产品mask缺陷。正确命名版仍待串行复验；原失败日志保留。此前full6/8及axis各3原点/序/floatbit均同，不能替代正确旋转mask重跑。

正确AngleDegrees版最终Encoded68Checks退出0，总28断言：12原case（type6/8各full1/axis3/mask2）全ID/序/XY float位同，其中5°各153点、微角各169点；两type完整runtime solve/warp/corrected-redetection链成功并有校正显示输出。原厂corrected数值oracle仍未新增，不声明这两分支校正结果逐位parity。最终日志encoded68-reset-final/encoded68-final.log；differences.json当前全12通过。未运行原厂。
