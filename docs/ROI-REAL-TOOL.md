# ROI 源码还原与实际工具验证

最新显示、继承和位置修正范围见 [ROI-DISPLAY-FIXTURE.md](ROI-DISPLAY-FIXTURE.md)。本交付不是完整原生等价证明。

原 ROIStack、选择器和样式资源见 roi-source-inventory.json、roi-style-assets.json、roi-runtime-evidence.json、roi-control-template-sources.json。共享 VisionUi 已接入多类数值模板、端点/卡尺/多边形交互、归一化坐标保存及 Group 专用窗口。Box3D 仅保留源代码确认的数值界面，不虚构2D投影。

实际 ImageBinary 工具运行 MONO8 硬阈值，消费 ROI 并集、屏蔽多边形、输入掩膜以及 ROI 外置黑/保留原图。专用参数窗口与真实主宿主、输入订阅和 Group 已接通。其他阈值模式仍不支持，不能把工具注册视为算法已全部实现。

CPreproMaskTool 的扫描线方法已恢复并用安全托管缓冲区实现，保留单精度、截断、交点配对和切线行指针行为。120组原SDK扇形/圆环样例均无像素差异，此前11像素问题已解决。该结论只覆盖已运行样例，不证明所有ROI边界或Fixture变换等价。

显示页现已接通眼睛、OK/NG颜色、透明度和粗细弹层，默认/高级取色器采用原资源和事件代码；样式独立保存，不改变算法像素。组合框、滚动条、滑块、单选、布尔开关和关联/编辑入口迁移原资源。实际安装的ColorStyleSet.cfg选择ColorStyle1，已核对其画刷与反编译资源一致。

区域继承区分矩形/圆环的区域输入和参数输入，屏蔽区独立继承。Fixture区分信息、点、坐标输入并实际改变输出掩膜；仿射逆映射和舍入是独立实现，尚无原VM差分结论。原InputBlindPolygonString编码、Fixture直接交互编辑、全部选择树模板、复合滑块模式及辅助交互仍有验收缺口。

测试捕获代码仅存在于tests。原SDK探针仅用于本地对照，产品无原SDK依赖。完整检查、真实窗口操作和退出检查结果见HANDOFF及本地HTML报告；通过测试不等于完整原生UI验收。
