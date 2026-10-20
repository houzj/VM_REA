# C4 Category.Items 关闭转发缺口：由Tab扁平集合闭合

2026-10-19；仅当前原程序集被动元数据、复用源码及原资源解析，无原厂执行/产品修改/构建。C4整卡S1/S2继续开放。

结论：本工具RoiType在Category.Items内，正常初始化后直接进入Tab.Collection；关闭DoFinish不需要Category递归转发。之前ROI-CLOSE将Category转发列未知，现在由Tab.InitCollection的实际体消除这一疑点。

原配置路径为AlgorithmTabRoot/Tabs/Tab[@Name='Tab_Basic Params']/Categorys/Category[@Name='Tab_Image Input']/Items/ROISelecter[@Name='RoiType']，无DeIniters子项。机器索引 `analysis/distortion-calibration/c4-source/roi-finish-chain.json` 记录该解析结果、5份复用源码哈希、7个关键当前IL方法。

实际调用链：Tab.Init (`0x06000f77`) 调base.Init后InitCollection；InitCollection (`0x06000f78`) 先清Collection，Categorys为null则返回，否则foreach Category，Items为null跳过，其余每个item先Collection.Add(item)，后面才按NameValueItem/名字/重复名条件决定hashtable记录。因此RoiType是否被加入Collection不依赖名称唯一性、CustomVisible、AccessMode或当前选中Tab；无任何Category.OnShownFinish递归必要。

窗口关闭已取得的AlgorithmTabRoot.DoFinish (`0x0600039c`) 在Tabs非null时逐Tab.DoFinish (`0x06000f7d`)。Tab在Collection非null时foreach NameValueItem调用虚OnShownFinish；原ROISelecter的override (`0x06000e71`) 先base.OnShownFinish再StopDrawROI (`0x06000e72`)。NameValueItem.OnShownFinish (`0x06000bef`) 在DeIniters为null时直接返回，否则逐ITrigger.OnTrigger；当前RoiType XML无DeIniters。StopDrawROI执行SelectNone并设IsStateForDrawROI=false。这是停止绘制与选择状态清理，方法体不清ROI集合、不清DataChangedCommand、不移除ROISelecter自身PropertyChanged订阅。

“必达”仅限已初始化原配置、Collection未被后续改变且之前关闭操作正常返回的控制流。Root/Tab没有每项try/finally继续保护，先前节点的OnShownFinish抛异常会阻止后续节点；动态修改Collection/DeIniters、类型不符或关闭取消也不由此片证明。当前XML静态节点和正常关闭路径已闭合，不把它扩大成所有异常下必达。

验证：被动元数据选取三程序集共1308方法；7个关键体有token/RVA/原IL SHA256与成员锚点，源版本与C4身份一致。生成器 `tests/DistortionCalibration.NativeProbe/InspectC4RoiFinish.ps1`，原结果 `managed/roi-finish-metadata.json`。这只解决容器转发缺口；真实ImageViewBox.ROIList/SelectedSubROIList换源回调/集合事件释放及旧ROI命令所有者仍待取体。NormalizeReversParse未展开。
