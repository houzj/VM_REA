# D4整卡S0身份、复用与缺体清单

2026-10-19。任务范围为双图/双ROI、模块缓存、文件矩阵方向、显示与释放。此清单提交根线审查，暂不增加S0或其他阶段分；当前D组仍4/32、整卡0/4。本轮只读原件/源码和产品，未开设备、原厂平台或新图像运行。

## 身份与可复用原件

`ExportD4S0Index.py`生成`analysis/map-calibration/d4-s0/identity-index.json`，记录安装目录11文件、SDK头/两DLL共14原件的长度/SHA256，以及10个可读原生C体、1个缺失C路径、9个Map产品C#文件。

| 原件 | SHA256 |
|---|---|
| IMVSMapCalibModu.dll | 658d63e2c4e2378e7eb1776d27b966990c0d0e01e29ca57ed82fe9acc740fde8 |
| IMVSMapCalibModuCs.dll | 6c8ea8fd06a2fa0d9e2c2f9a2f4b23d735e0d4fcb4d838721592b401b1c429f6 |
| MVDMapCalibCpp.dll | fb373e8ef51572f1ea377a1218bc3fcc230e61f8fa72264d6715445467b4b75a |
| MVD_Algorithm.dll | 85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549 |

安装目录为`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu`，SDK为同VM4.4安装的MVDAlgorithmSDK。XML/Display/AlgorithmTab/语言相关资源、MapCalibCpp.h均重新hash；没有混入另一版本。来源为安装原件与既有Ghidra导出，不新造REA Evidence ID。

## 按D4五项的复用、产品与精确缺口

| 子项 | 已有原始源码/资源及运行证据 | 当前产品位置 | 仍需获取/核对 |
|---|---|---|---|
| 双图 | Process 180021690、ProcessAlg 180021c50；SDK ITwoImageMapBasicParam；11步受控模块image-results | MapCalibrationPlugin.cs CurrentImages/ImageSession、Runtime.Image；ImageSolver.Solve | 单帧待处理期间切模式的状态、SDK结果与模块历史结果关系；受控IO host不是VM平台 |
| 双ROI | ProcessAlg的两ROI向量/setter；安装XML InputROI1/2；image-roi/image-roi-mask/ROI7361既有载荷 | Plugin.Runtime.Image的RoiGroups RoiType_0/1；ImageSolver.Detect；Configuration独立预览 | SetParam写/清两vector与shape所有权；SDK具体ROI setter和GetProcessInput形状/角度转换体。入口有限切片另见DUAL-ROI-ENTRY |
| 模块缓存 | SetParam 180023870、ReadFileMatrix 1800227d0、ResetMember 180022d50、Init 180021340；File279/Session100和模式切换旧trace | MapCalibrationFileSession、Plugin的standalone/images/scope.State及Reset | 原厂图像pending全部重置路径、换mode是否清ROI、释放/异常所有权；产品scope销毁与原生对象析构不能合并 |
| 文件矩阵方向 | MapCalibCpp.h双文件接口、ReadFileMatrix；旧72 SDK案例及XML1790字节比较；历史报告给出inverse(target)*source | MapCalibrationSolver.SolveFiles/Compose、MapCalibrationFiles、ImageFiles/FileSession | 历史core/1807d4f20.c当前不存在，需定向取体核当前文件方向/归一化；不以托管实现替代该缺体 |
| 显示/释放 | ProcessOutput 1800220b0；Display.xml；ORIGIN-DISPLAY与RESULT-WRAPPER审计；ResetMember已取 | Plugin.LastDisplay/Runtime.ImageResult、Metadata/Configuration；公共ToolDisplayService与scope生命周期由根线拥有 | CalibOriPoint*资源无已查生产字段，外部provider未知；模块/SDK shape、图像、结果析构及平台历史结果释放/清理路径；真实宿主视觉尚未验收 |

可复用专属报告：STATE-LIFECYCLE-AUDIT、RESULT-WRAPPER-AUDIT、ORIGIN-DISPLAY-AUDIT、UI-COVERAGE、CONTRACTS、DELIVERY。它们的字段与错误码结论按原记录范围复用，不因本轮只读清单刷新测试数量。

## 当前磁盘缺体/路径情况

旧文档的`D:/MyWork/REA逆向测试/analysis/full-audit/calibration-dependencies/decompiled/IMVSMapCalibModuCs`当前不存在；其父目录目前只有dynamic-schema.vmx.json。安装托管DLL仍在并已hash，但不能说MapCalibParam/MapCalibResult托管反编译已在可读目录复核。后续需定位或定向恢复这些具体类型/成员，既有报告hash仅作复用线索。

原生Process/ProcessAlg/ProcessOutput/SetParam/ResetMember/ReadFileMatrix/Init的C仍可读；module-analyzed ProcessAlg同目录asm未取得，本轮明确只按C+SDK接口顺序解析虚表调用。必须确认具体setter内存/ownership或角度字段时，再定向获取callee/汇编，不将整个SDK所有CPU列为D4前置。

## 范围与下一步

先选一个实际双ROI入口有限切片：ProcessAlg图像模式将哪一侧vector的哪一个元素传给SDK的哪个setter。它可用现有C与同版本头完成；不因此宣称ROI二进制ABI、angle单位或清空生命周期都闭合。

S0退出目标仅身份一致、复用与缺体/产品位置明确；本文完成后交根线审核。S1–S6仍按上述精确缺口推进，SDK未知部分不以托管清0/删除缓存/伪造显示值填补；D2负flags安全过滤保持原有明确边界。
