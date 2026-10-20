# D4有限源码切片：双ROI入口两侧向量与setter

2026-10-19。只读当前同版本ProcessAlg原始C、SDK MapCalibCpp.h及既有双ROI探针源码，没有执行探针或增加图像。身份/文件hash见D4 S0索引。

## 类型与虚表字段

`ITwoImageMapBasicParam`在MapCalibCpp.h中无其他基类，首虚函数为析构；x64每槽8字节。按原头顺序，+20=SetSourceImage、+30=SetSourceROI、+50=SetTargetImage、+60=SetTargetROI（均十六进制）。GetSourceRegionImage/SetSourceRegionImage在+38/+40，target region在+68/+70，不能把ROI shape setter与mask image setter混为一个字段。

原生模块`CIMVSMapCalibModu::ProcessAlg`（module-analyzed/180021c50.c）先按this+168b0设模式；mode1取得GetTwoImageMapBasicParam。非空后数据流为：

| 模块字段 | C体具体操作 | SDK槽/含义 |
|---|---|---|
| this+168c8 | 读取8字节指针传参 | +20 SetSourceImage |
| this+169f0 | 读取8字节指针传参 | +50 SetTargetImage |
| this+17330 / +17338 | begin/end，(end−begin)>>3非0时读begin[0] | +30 SetSourceROI |
| this+17348 / +17350 | begin/end，(end−begin)>>3非0时读begin[0] | +60 SetTargetROI |

两ROI向量是独立的指针序列；合法向量非空时各侧只传首个IMvdShape*，该段没有遍历全部元素/合并ROI。shape内部字段并未在这段拆成center/width/height/angle float，因此不能把“指针槽位置确认”写成“ROI原生结构布局已确认”。SetSource/TargetRegionImage不是这几条调用；没有从slot+30推断传的是mask。

## 清空与生命周期的精确边界

任一侧向量为空时，此段跳过该侧SetROI，**没有在此处调用SetROI(nullptr)**。这只确认ProcessAlg这一段未显式清空；SDK setter是否保留/克隆/借用shape、SetParam或ResetMember是否已清SDK状态尚未知。不能据空向量直接断言原厂一定沿用旧ROI，也不能擅自实现“每次清空所有ROI”作为原厂规范。

两侧image与可用ROI设置后才调用算法Run/GetResult。mode2走独立双文件参数路径，不执行上述双ROI赋值。SDK两侧指针和模块缓存如何在mode切换/异常/销毁时释放，留D4后续owner链。

## 当前产品映射与可复用验收

MapCalibrationPlugin.Runtime.Image分别读RoiType_0和RoiType_1；无第一组时可回落c.Rois，第二组缺席为全图。产品限制每图至多一个矩形/全图，再分别传ImageSolver.Solve的sourceRoi/targetRoi，匹配两侧独立来源，但该托管值结构不是SDK IMvdShape* ABI。

已有ImageRoiProbe调用SetSourceROI/SetTargetROI分别设形状，包含仅source/仅target的roiCase7/8及旋转矩形；CompareRoiMask和RoiRuntimeChecks保留对应有限域证据。本轮不重跑，也不增加样本。可先复用这些输入核side字段/空侧；清空历史ROI需要先补具体setter/SetParam源码，而不是直接新增未知缓存输入探针。

该有限切片闭合入口的数据结构、侧别和首元素选择，不闭合angle单位、shape深拷贝/所有权、空ROI缓存和完整宿主显示。D4阶段暂不增加，等待根线审查S0清单。
