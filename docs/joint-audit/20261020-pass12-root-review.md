# 第十二轮根线自审：显示失败切换与动态端口身份

当前目标仍限十项标定工具。B 线针对标定板/畸变的 J08，C 线针对标定加载的 J04/J09，各以原显示资源或动态 IO 成员为边界，完成实际当前主窗或真实 owned Group 图探针；主线顺序独立复跑。没有产品修复，`tool-plugins` 在制文件未动。

| 链段 | 根线实测与审计结论 | 尚需的原件/运行边 |
|---|---|---|
| 标定板、畸变显示成功→失败→切选 | [显示报告](20261020-pass12-board-distortion-display.md)。真实离屏 MainWindow 11/11；根线日志 `analysis/joint-audit/pass12-cal-b-display/root-run.log` SHA-256 `0DA4B51325A5A0AFFBEFA6FB8629F5D858F18FAECCA2E4C29A5CBA4E56AF16B2`。板空白 `NoMatch` 清旧检测图形，保本轮输入图；畸变自动导出坏扩展名 `Failed` 后，owned display 保留前轮两图层/170 图形并更新失败记录；切到图像源均清 presenter。畸变差异作为 FCR-034 合同风险登记，未据此清图。 | 原畸变 `Display.xml` 的 `PostCorrectionImageData` 与 `ProcessOutput` 非原子发布有源证；缺同条件原厂 `GetDisplayList`/结果回调可见值。板 NoMatch 的原可见图层/记录同图差分也未证。 |
| 标定加载动态叶改名/改型→Group→坐标系 | [动态 schema 报告](20261020-pass12-readcalib-schema.md)。真实图先 `OldMatrix:FloatList` 成功；同 scope 文件改名后 loader descriptor 为 `NewMatrix:NumberArray`，但 Group 仍选择旧源并 `SubprogramFailed`，坐标系跳过；改型 `int` 时 Group 类型错误；保存重开仍保持旧选择，恢复原名/类型后两 scope 均成功。根线在自有隔离 cwd 独立复跑，日志 `analysis/joint-audit/pass12-cal-c-schema/root-run.log` SHA-256 `C38E33FBC930C93E7DA743B1FECB81B33A8AE3F106E54CC373037A2D91F2B97D`。 | 原 `LoadFile/Process` 证文件输出名/类型更新；原 `DynamicVmIO.Name/TypeName` 是**现有对象 UI 编辑**路径，不能推作文件刷新自动迁移。仍缺原 ReadCalibFile 文件刷新→SDK IO 信息→托管对象复用/重建与 Group 订阅通知的具体调用边。 |

显示探针首编/夹具/提交输入失误在 `analysis/joint-audit/pass12-cal-b-display/first-run.log` 至 `fourth-run.log` 留存，最终第五次通过；动态 schema 探针首轮通过，不能把两者构建尝试混算。两条只关闭上述当前具名链段，未使相关工具整项 J01–J12 全部通过。原 Group 正常矩阵、工具目录和普通配置窗已有证，不重复补报告。

主框架具名实施进度仍为 **41/96 = 42.71%**；本轮是已实现代码的原件对账与实际差异定位，没有闭合新的实施单元。人工验收独立，历史标定插件 10/10 实施自审不等于当前联合代码审核全闭合。
