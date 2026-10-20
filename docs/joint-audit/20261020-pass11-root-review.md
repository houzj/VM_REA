# 第十一轮根线自审：N 图像失败恢复、坐标系 ROI、N 点主窗成本

目标仍限定“标定”十工具及其必要的框架交接。本轮三个独占探针都从原源码/资源入口追到当前实现，再用真实图或真实 WPF 配置宿主验证；根线串行复跑两个子线探针，并直接实跑 N 点主窗探针。无产品修复，未触碰 `tool-plugins` 在制文件。

| 联合链 | 实际复验 | 原件边与尚未证实的范围 |
|---|---|---|
| N 图像第九帧失败→新批次→XML/Group/坐标系 | [状态链报告](20261020-pass11-nimage-state-chain.md)。原子线与根线各 exit 0、26/26；根线日志 `analysis/joint-audit/pass11-cal-nimage-state/root-run.log` SHA-256 `35D7BA8AA4A9A2CCED4517CDDF0456CAE800EEFA75EDD0ED2708E639573F261E`。失败时公开 `CurNum` 缺席、owned 数=9、无 XML；下一帧重置到 1，完成新九帧后文件被 Group/Coordinate 使用，保存重开再跑成功。 | 原 `Process 0x180020b90 → ProcessAlg 0x180021450 → ProcessOutput 0x180021850` 及 `WriteXmlData 0x1800230e0` 的批次与结果分支有体/哈希；原 SDK 同图返回码、矩阵位等价和未完会话持久化未证。 |
| 坐标系 Draw ROI→图像/矩阵/显示→Undo/重开 | [ROI 报告](20261020-pass11-coordinate-roi-chain.md)。子线与根线各 exit 0；根线日志 `analysis/joint-audit/pass11-cal-coordinate-roi/root-run.log` SHA-256 `EC830A794CF398A8EF9B93A40FDBE984C5C15B91C4C19CE087817CE8F224C84B`。当前 Normalized ROI 对 100×80 与 200×80 图给像素原点 X=25/50，缺 ROI 暖态保旧，撤销及保存重开恢复。 | 原 `CoordinateModuleAlgorithmTab.xml` 与 native `0x18003f4a0/0x180040320/0x180040550` 确认模式门、矩阵、输出；原 ROI 的编辑→序列化→native 坐标空间尚未取证。归一化缩放仅为当前宿主适配。 |
| N 点实际 MainWindow 配置窗按钮→点表→文档 | [主窗成本报告](20261020-pass11-npoint-mainwindow-cost.md)。根线实际点击 4/12/32 行点表，`Revision/Changed` 均 +20/+60/+160，单格各 +1，最终 exit 0；日志 `analysis/joint-audit/pass11-cal-npoint-mainwindow/final-run.log` SHA-256 `85D8940022B6495D9A5899C067F272A6B1291F471B2DA51D40E8D60FA9D3AAF6`。 | 原 `NPointList.Init` 与 `PointList` setter 逐字段读写已核；原厂文档历史/同机时延未知。当前开窗调用放大为实测事实，但不能独立归因用户的长期无响应。FCR-033 更新，尚未跨项目实施。 |

子线首次编译失败仍留在 `analysis/joint-audit/pass11-cal-nimage-state/`；N 点第一次仅 4/12 行的日志留在 `analysis/joint-audit/pass11-cal-npoint-mainwindow/first-run.log`。原始程序集、反编译体和探针产物的身份分别在三份专属报告；根线没有把当前测试通过冒称原机同构图或算法全域等价。

十工具联合审核的 J01–J12 仍有特有未证边，按[去重索引](20261020-calibration-only-joint-audit.md)逐项继续。具名主框架实施进度仍为 **41/96 = 42.71%**：本轮补的是已实现代码的跨层审核，未闭合新的 96 项实施单元。人工验收独立，不计此分母；历史插件 10/10 实施自审亦不等于十工具联合审核完成。
