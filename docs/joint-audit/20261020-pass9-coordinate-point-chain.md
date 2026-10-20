# 第九轮：平移旋转标定点输出进入坐标系 POINT 输入

本切片只审标定类的平移旋转标定点输出，经 owned Group 到坐标系 `InputWay=1` 的当前真实执行与保存重开。矩阵文件到坐标系 `InputWay=2` 已在第八轮另审，不能把两种输入合同合并。产品和 `tool-plugins` 均未修改。

## 原件合同与边界

原 `CoordinateModu.dll` 的内层 `18003f4a0` 反编译体位于只读取证文件 `worktrees/tool-plugins/analysis/coordinate-module/native-source/output/18003f4a0.c`，SHA-256 `3420E2291FD782FFDAF6BA801F834BE8385239BEDCFF36C4EA4CC1A16C850A68`。`CordinateCreate=1` 且 `InputWay=0/1` 时，按 `SrcPointX`、`SrcPointY`、`SrcAngle` 的次序分别读第 0 项；任一读取返回非零便提前退出，三个成功后才将 pose 写入矩阵。其 `180040320` 转换为 `[cos,sin,-cos*x-sin*y; -sin,cos,sin*x-cos*y;0,0,1]`，角度输入按度数用于三角函数。第八轮已记录平移旋转标定的 `Process/ProcessAlg/WriteXmlData` 体及 XML 矩阵路径；本轮仅复用该生产者源证据，不重新声称其所有算法模式已覆盖。

## 当前执行探针

`tests/CalibrationCoordinatePointChain.Checks/Program.cs` SHA-256 `F5F7DFC84416EB286B97BC15EB166E05B27AB2FB3E44B831CD57ADEDB142FCB0`，工程 SHA-256 `34BB43761C26C5791F413357F88B89A8C654EF850043CE3A18CD1D488AF552A4`。以受控 `state-rotation-three` 点表构建平移旋转标定→标定加载的执行/刷新边，在 owned Group 中同时发布九项 `Matrix` 与生产者 `OutImagePointX/Y` 两项数组。父流程的一只坐标系走加载后的 `CalibMatrix`，另一只坐标系走 Group 的 `ImageX/Y`→`SrcPointX/Y`；它的 `SrcAngle` 订阅 `[0]`，并接一只图像源满足原运行图像前置。独立 `CalibData:Calibration` 直连 `CalibMatrix:NumberArray` 被类型系统拒绝，图未被悄悄修改。

命令 `dotnet run --project tests/CalibrationCoordinatePointChain.Checks/CalibrationCoordinatePointChain.Checks.csproj` 退出 0。首跑及保存/重开后，生产者、加载器、Group、两只坐标系均 Success；文件和 Group 矩阵同为九项且值相同。POINT 消费坐标系另输出九项矩阵，平移项符合原 `SrcPointX/Y` 第一项的负值（本夹具角度 0）。日志 `analysis/joint-audit/pass9-cal-coordinate-point/checks.log` SHA-256 `FB002704126E5CF4E45A5B95C22AEC82A3ABF0CE3C3497B4883FE3E55D0DAB78`。首次 `--no-restore` 因新工程尚无 assets 文件报 NETSDK1004，随后正常还原构建成功；该首次失败保留于执行记录，不计产品失败。

原件 `SrcPointX/Y` 的动态来源与当前 Group 投影是两层证据：原体证消费顺序和公式，探针证本地组合图能够供值。未运行原版同构整图，未核原点表以外的 `InputWay=0` ROI、三输入缺失/错误的同图传播、原生 float 逐位数值差分、显示资源与用户桌面操作。以上仍为本目标待审边，当前成功不宣称坐标系全链完成。
