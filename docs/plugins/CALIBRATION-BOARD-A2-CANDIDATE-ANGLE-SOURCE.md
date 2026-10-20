# A2有限修复：标定板候选角度参数

2026-10-19。此项从A1具体字段链发现差异，按原厂源码修复参数传递，不重写候选或排序算法。

## 原厂证据

1. `7f69e0`从engine+18c/+190转float形成公共options+48/+4c。前轮六板型18次模块/SDK同实例trace的36个公共入口观察中，该两字段均为float30/140，原始记录`board-type-options-b20de5dfa108435baf63a02143e253c5/runtime.jsonl`。
2. `815560`棋盘分支和四编码初始分支将这两字段投影至873de0 compact+10/+14。本轮四编码和分派汇编已核；该字段不是EdgeThreshLow/High。
3. `873de0`第一轮把这两位值放入aiStack58[2]/[3]传8b47c0，后者只把factor槽设为3，再传8bb3a0。第二轮另固定factor1.75、角度65/110，源码`180873de0.c:264`附近可定位。
4. `8bb3a0`从其参数块+8/+c读取两个float，转换角度门槛，并在候选数不超过1000时特别判断factor3、30/135这组三值；非此组合还要求峰数量与角度门通过。源码位于`core-live-08fc0fd54f1243db95524d69b3bd886d/output/1808bb3a0.c`约382及686行。故30/140与30/135不仅是可忽略的数值标签。

## 产品差异及修复

原`NImageSourceCandidateDetector.FindCandidates`第一轮硬编码Describe(...,3,30,135)，`NImageSourceGridPyramid`和adapter没有提供调用者角度，因此标定板经过它们时丢失30/140。

现为这三个公共类增加带minimumAngle/maximumAngle的重载；原有签名保留并委派30/135，维持既有N图像调用的行为与二进制签名。只有标定板的棋盘、编码板全图/crop/mask四处生产入口明确传30f/140f；第二轮65/110保持原厂来源。圆点不走这些入口。没有新增UI参数、方案字段、静态全局配置、解码方向或另一套候选器。

新增重载是独立托管传参接口；30/140和消费者的分支来自原厂证据。没有声称未暴露的原厂参数可在当前UI编辑，未知SDK扩展字段也未猜补。

## 验证状态

最终稳定回归：`tests/CalibrationBoard.Checks/Run.ps1 -AllCandidates`通过5228项，exit0，构建0警告/0错误，`sourceChangedDuringBuild=false`。产物`analysis/calibration-board/verification-bd73ca81bbd64093b21f4f2f66a6bed1/summary.json`。此前d7c318...虽检查通过，但因源码注释调整记录变化，未作最终稳定证据，本轮重新运行后闭合。该回归检查原厂fixture、ROI/运行/输出与候选，不能据其通过宣称全部板图或桌面等价。本轮未修改NPointSolver、持久化格式或另一工作树；无提交推送，人工验收仍独立开放。
