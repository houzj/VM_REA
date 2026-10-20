# 第九轮根线自审：标定特有图像、文件与 POINT 载荷

范围仍是用户指定的十个标定工具及其必需主框架交接。第九轮各审一条前轮未闭合的**特有载荷**，没有把共享 Group、目录、配置逻辑重复算作三个工具的全面完成。根线核对三个探针与原始证据的结论边界，并串行独立复跑；没有修改产品源码或 `tool-plugins`。

| 特有链 | 本轮实际结果 | 原版依据与保留界限 |
|---|---|---|
| 畸变 IMAGE/ROI→畸变修正图→N图像 | 真实 WPF 宿主 11/11，1000×1000 纠正图、76B `.iccal`、N图像首帧、重开、错误扩展名和恢复。根线独立复跑退出0，日志 `analysis/joint-audit/pass9-cal-distortion/root-run.log` SHA-256 `249D6AE320BD2FD1CEF71D0A35DE10D316DD8B3989E7F480EEDA84415CFABB00`。 | [畸变专项](20261020-pass9-distortion-chain.md) 精确区分原组合端口 `PostCorrectionImage` 与发布成员/显示键 `PostCorrectionImageData`；原厂同构运行、全部 ROI/算法/显示分支未证。 |
| 相机映射 36B 文件→旋转标定 | 真实图在一个 scope 连续 3 轮，`remainCount=2/1/0`；缺文件 `E0000008`，恢复后同 scope 再成功。根线独立复跑退出0，日志 `analysis/joint-audit/pass9-cal-rotate/root-run.log` SHA-256 `9A2162AFE00615D8CCD2646A43D8ABD67A247E38EB833BC07C16B680B1CFF6ED`。 | [旋转专项](20261020-pass9-rotate-consumer-chain.md) 的原 `SetParam/LoadCalibFile/LoadFile/Process` 分支与原指令核对；三个 XML 仅复用既有文件，不冒充本轮同图生产。原厂同构运行和旋转输出下游未证。 |
| 平移旋转标定点数组→Group→坐标系 POINT | `SrcPointX/Y` 真实运输，角度订阅 `[0]`，两只坐标系分别走矩阵/点模式；首跑和保存重开通过。日志 `analysis/joint-audit/pass9-cal-coordinate-point/checks.log` SHA-256 `FB002704126E5CF4E45A5B95C22AEC82A3ABF0CE3C3497B4883FE3E55D0DAB78`。 | [坐标系专项](20261020-pass9-coordinate-point-chain.md) 的原 `18003f4a0/180040320` 确认读取顺序和矩阵公式；原厂同构图、ROI/缺项与显示未证。 |

本轮构建有共享 `obj` 写锁的首试失败（两个子线错峰成功）；坐标系新工程首次 `--no-restore` 因缺 assets 文件失败，正常 restore 后成功。这些是测试环境失败，不能篡成产品缺陷或从日志删除。各子线的源码路径、资源 SHA 和失败文件在专项报告及自有 `analysis/joint-audit/pass9-*` 下。根线自审只确认上述有限切片；本目标的完整 J01–J12 全链尚未逐项关闭，尤其原版同构执行、UI 订阅/显示与算法差分。旧插件项目的 10/10 是既有**实施自审**，不是当前联合审核 10/10。

框架具名单元实施仍为 **41/96 = 42.71%**；第九轮为已实现代码链的审核，没有证据充分的新完整框架实施单元，分子不增加。人工桌面验收单列，当前未执行。
