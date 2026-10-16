# VM 4.4 主界面反编译证据

本报告依据本机安装目录 Applications/myLibs 的程序集，通过 ilspycmd 11.1.0.9782 恢复。输入 SHA256 和输出数量见 analysis/decompiled/manifest.json。结果属于二进制重构代码，不是原始源码；本阶段没有执行原控件、编译恢复项目或验证实时界面行为。

## 主视图的实际组成

1. VisionMasterMain/MainWindow.xaml：BaseWindow 自定义窗口、TitleBar 标题栏、VmMainViewConfigControl 主体。
2. VMControls.WPF.Release/VmMainViewConfigControl.xaml：包装 VMControls.WPF.MainViewControl，绑定工作区锁定、参数打开、缩放及参数编辑状态。
3. VMControls.WPF/MainViewControl.xaml：ProcedureConfigControl 位于左侧，ModuleResultView 位于右侧。列宽默认 *、5 像素分隔、1.0000001*；右列最小宽度400。实际运行时用户配置可能改变比例。
4. ProcedureConfigControl.xaml、OneProcedureConfigControl.xaml：流程配置、ToolBox、DiagramArea。
5. VMControls.WPF.Base/DiagramArea.xaml：ZoomArea 包含 DiagramPage，底部 StatusBar。DiagramPage.xaml.cs 包含框选拖动、复制/移动和连线呈现逻辑，应继续逐方法分析，不能把可见连线直接等同于我们的类型数据连接。
6. ModuleResultView.xaml：ResultTab、图像选择与显示工具、历史结果 DataGrid、帮助页；结果区默认高度235、最小40，可调整。具体标签及可见性依赖本地化资源和模块类型。
7. RenderControlBase.xaml：实际图像显示通过 VMBaseControls.ImageViewBox，包含形状列表及 ROI 绑定。

## 已核实的交互入口

DiagramArea.xaml.cs 的 ConfigParamsCommandExecute 检查模块禁用及加载失败状态后发出 ItemDoubleClick 事件。该入口提供参数配置行为线索，最终打开方式仍需追踪事件订阅者，不能据此断言所有参数都使用独立模态窗口。

## 主题及资源

Apps.ColorFun/colorstyle1.xaml 定义暗色显示背景 #3B3E41、日志背景 #3B3C3F；其他主题文件包含浅色方案。Apps.Style 和各控件程序集的 XAML 与图片已恢复。资源数量仅表示提取成功，不表示每个资源已经人工核对。

## 独立重建的落地顺序

- 用真实主视图分区替换当前原型的工具箱/固定右侧参数栏布局。
- 对照 DiagramPage、节点模板和 ZoomArea 恢复节点外观、缩放、选择与连线路由规则。
- 恢复图像/模块结果、历史/帮助切换及状态栏交互。
- 沿 ItemDoubleClick 的订阅者恢复参数配置生命周期。
- 独立实现对应控件及服务，不把原 VM DLL 作为交付运行依赖。

## 尚未完成

恢复项目仍面临 .NET Framework 4.6.1 引用程序集和其他依赖问题；没有宣称能独立编译。本次主要恢复界面实现及资源，原生算法 DLL 没有反编译。现有独立编辑器也尚未整体改造成这里的布局。
