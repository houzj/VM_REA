# 第十四轮根线复核：标定显示与文件写参

范围仍是标定十工具和必要 ImageSource/Group 交接。根线检查 B/C 两线的原件、当前代码、探针和失败记录；不把离屏窗口当用户桌面验收。没有产品修复或改动 `tool-plugins` 在制文件。

## 本轮新增的具名边

1. [四工具显示链](20261020-pass14-b-four-display.md) SHA-256 `D4ABC925D16579E0F725A503A2C1E902A357681A90E958E92B94FB809618DDC8`：N 点、N 图像、相机映射、映射标定的原 `Display.xml`/`ProcessOutput` 字段到当前 `LastDisplay` 各自核对。映射标定真实双图→主窗成功、空白第二图失败、切图像源六项实测。根线以已构建产物独立复跑 exit 0、6/6；独占输出 `analysis/joint-audit/pass14-cal-b-display/root-rerun/summary.json` SHA-256 `C3C892017AF82903CFD714BBCF1925124649300905CE037D6408A54DE31A1924`。当前成功层数 1、图形 4；失败 `Frame=null`，切源后图/记录/列表清空。探针的六项断言没有单列“失败帧为空”为断言，该值是记录的观测，不能偷算第七项。原版同图失败显示仍未知。
2. [旋转标定图像与结果](20261020-pass14-c-display-chain.md) SHA-256 `25EA3A33892B512532A028C7390AD89E7EE96ABF8AAB7FDFBC69D40E2AE047F5`：原 XML 四个 `IsForce=true` IMAGE 叶、`Display.xml` 的 `InImage`、原 `Process` 的非法 RotNum 清样本路径，与当前聚合可选 `InputImage`/结果快照分开核。根线独立复跑 exit 0、9/9：真实 80×60 ImageSource 首轮成功，`RotNum=2` 返回 `E0000001` 后旧中心/剩余次数清掉、错误记录显示本轮图，同 scope 恢复；其他未运行节点不能读上一节点快照。原 native `Process` 未证明无图时算法拒绝，已证差异限于声明、连线及显示来源合同，登记 [FCR-035](../CONTRACT-REQUESTS.md)。
3. [标定加载文件控件父类 CIL](20261020-pass14-readcalib-parent-cil-bridge.md) SHA-256 `71E445DFF214B9733E2E6C10A55078AE649FA2D0E1F382C964C9B2074C6CB3A7`：安装版 `Apps.XmlParser.dll` 经已安装 IL 工具定点导出，父类 `ValueChangedCmdExecute→SetParamUnit→IUserStringData.set_Item→PreExecuteRun` 无直接 `InitDynamicIO`；后段 `VM.Core.dll` CIL 核 `ModuleParam.set_Item→OnModuleParamChanged` 和 Outputs 首次懒加载。此证据排除文件按钮直接重建动态叶，不排除 SDK 写参或运行后的间接回调/订阅者；不能把第十二轮当前旧 Group 绑定失败冒充原版自动迁移裁决。REA 大程序集超限的原失败证据仍保留。

## 未闭与进度

十工具 J01–J12 不是本轮 6+9 项测试数；各工具仅更新上述有限 J08/J10/J04/J09 边。N 点/N 图像/相机映射各自的连续失败显示未实测；Translation/ReadCalib 的显示失败和原版动态 Group 订阅仍有确切缺口。原版十项动态创建 ID、同条件 SDK 数值/显示、用户长期无响应 UI 栈仍分别记录，不能以当前探针绿色替代。当前共享/插件职责未变；算法和后台仍属本目标，不把其他分类工具加入分母。

主框架实施仍 **41/96 = 42.71%**，历史标定工具实施自审 **10/10** 独立。本轮未关闭 96 中新的实施单元；人工验收不计入实施率。保留主目录原有未提交五项用户改动，本轮审计增量单独提交，不推送。
