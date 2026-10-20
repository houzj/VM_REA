# 第十五轮根线复核：标定两条失败显示与动态端口历史

范围仍为标定十工具的框架→工具 UI/后台/算法→结果链。N 点与平移旋转的原版显示资源/原生 `Process/ProcessOutput` 对当前帧、owned Group 分别核证；标定加载的原版动态叶编辑历史与文件按钮路径分开追。离屏 MainWindow/Group 探针证明当前实现，不能充当原机显示或人工桌面验收；未改 `tool-plugins`。

## 具名新增边

1. [N 点成功→退化点失败→切选](20261020-pass15-npoint-display.md) SHA-256 `16359AF8BC20B8D7E7839A3824E3009578DEB3E7D128B6BBA0ED60C29A3C1E41`：原 `Display.xml` 的两类点图形、状态/进度记录与三列表到当前 `LastDisplay` 的映射已对账。根线用已构建探针独立复跑 exit 0、6/6；`analysis/joint-audit/pass15-cal-npoint-display/root-rerun/summary.json` SHA-256 `E525D941FDBA763B54ACEA94669728EBAC8C87AA3BD0C2642A1440970E39B09E`。当前合法四点成功有两图形，退化点求解 `0x10103007` 后无旧图形且有诊断记录；切到图像源清三 presenter。原同条件失败后显示对象内容、选择态、错误码仍无动态证据。
2. [平移旋转标定四点→owned Group/显示→清空/恢复](20261020-pass15-translation-display-chain.md) SHA-256 `060FBC995DF9727FED6F7593F32AD24CF9FC68899F22AF682607C6202B4ED3DA`：原 `Display.xml` 路径/Record/四列与 `Process` 早退/`ProcessOutput` 正常写叶已核。根线独立复跑 exit 0、10/10：正常 Group 导出点数组；无捕获输入失败时子节点与 Group 均失败且当前显示清空；明确重编辑点参数后恢复。首次失败日志保留，它暴露**仅增加 `ClearRevision`、保留原点参数**这一程序化更新导致 `Points` 缓存清零而参数仍非零的状态分叉。正常“清空标定点”入口在原版及当前配置宿主均同时写零点参数，故不能把此探针称为正常 UI 点击缺陷，也不能无原版对应输入语义就直接改插件。需要区分清空动作的参数原子性与低层节点参数 API 的独立写入合同。
3. [原版动态叶历史与文件按钮分界](20261020-pass15-readcalib-dynamic-history-boundary.md) SHA-256 `AE5DD49204AE84185D4B3EAFFD46107F5D7AC156B403FEC03C79B4318CF8CA27`：`DynamicVmIO.UpdateDynamciIOInfo` 的显式编辑在 ModifyConfig 上下文登记 IO/订阅历史；`VmModule.IOData_Undo/Redo→Done_IOData→RecoverSubscribe` 会尝试按名复原连接。原文件选择按钮没有到这组方法的已证直接边，不能推原版“重选 XML 自动迁移旧 Group 连线”。SDK 写参/运行之后可能的回调订阅者仍是精确未知。

## 剩余与计数

本轮给 N 点 J08、平移旋转 J08/J10 和标定加载 J04/J09 增加**有限边**；没有把三篇报告或测试断言数当成 96 个计划实施单元。相机映射、N 图像、标定加载的当前失败显示，原厂同输入显示与动态 Group 绑定，长期无响应时的 UI 栈，以及算法有限分支仍需各自源码/运行证据。工具代码的历史实施自审 10/10 独立于这次十工具 J01–J12 联审闭合数。主框架实施仍 **41/96 = 42.71%**；人工/设备验收不计入实施率。保留既有用户工作区改动，本轮只整理审计证据和探针，不推送。
