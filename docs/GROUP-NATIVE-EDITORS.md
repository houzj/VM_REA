# Group 双栏配置器、图形 ROI 与真实工具接入

本次工作属于 main 框架线；没有修改插件实现或另一工作树。原始证据文件和哈希见 group-native-editors-source-manifest.json。不能将本轮结果称为原生 Group 全部完成。

## 原始源码确认

- DynamicRunParamConfigWin.xaml：1100×680，取消按钮隐藏。DynamicRunParamConfigPanal.xaml：左栏370，标题/筛选各37；右侧数据源150、自定义名称150、类型90、关联150、同步70、操作90。
- ParamTypeList.cs：全部、数值、枚举、开关、单选、ROI、文件、训练模型八类。没有对应编辑元数据的类型不能伪装成已经提供该类工具。
- ParamConfigDataBase.AddGroupCommandExecute / GetNewGroupName：分组0起，新增后选中。AddParam：无类别时创建，新增到选中类别；模块勾选逐项加入，达到32项停止。UpDownParam只在同类别排序。DeleteAllParams与DelGroupCommandExecute要求是/否确认；单参数删除无确认。
- GroupRunParam.cs：初始化选中首个类别。原生没有每个参数下方的类别下拉行；类别重归属需要取消选中后在目标类别重新添加。
- ROIConfigControl.xaml：内容1000×600，左380，右图像区上方40工具栏；RoiSelectControl.xaml和roiselectdatatemplate.xaml：ROI参数默认折叠，选图形后编辑。
- CircleROI.Parent_MouseMove：按下点为圆心，鼠标距离为半径。RectROI：矩形拖绘、移动、角点/边/旋转控制。PolygonROI：单击顶点、右键撤点、双击结束。
- ROIControlBaseStyle：蓝色#0F6AF3轮廓、橙色#F76900锚点；原生删除/复制命令。ImageViewBox：棋盘格、最近邻图像、左键空白平移、放大1.5/缩小0.5、光标锚定滚轮缩放、Init重置视图。
- 原始按钮PNG已复制为构建资源（逐个源/目标路径及哈希在清单中）：矩形/圆/环形/多边形、关联、同步、上下排序及状态图。资源来源在上述XAML中；资源相同不代表整体视觉已等同。

## 已接通的产品行为

- Group运行参数“编辑”打开独立双栏配置器，含类型/文本筛选、模块批量勾选、类别选中/新增/重命名/排序/删除、参数勾选/名称/排序/删除、多关联与同步。运行页显示分组参数，管理控件移入配置器。
- 去除ROI表格适配器，改为图像坐标中的矩形、圆、圆环、多边形图形编辑，支持移动、角点尺寸、矩形旋转、圆/环半径、多边形顶点、撤点、复制/删除、缩放/平移/全屏、数值校验、保存及取消回滚。参数区按原生默认折叠。图形绘制和事件桥接是独立WPF实现，并非原厂控件直接运行。
- ROI预览读取该工具唯一的图像输入绑定或其唯一图像输出；不会从其他节点任取图像。NodeDebugExecutor.ResolveInput只读连接/订阅/流程输入，不执行生产者；缺失、失败、重复或类型不符均拒绝。
- Group内部真实图像源工具通过既有IWpfToolEditorFactory创建专用窗口，宿主提供内部流程的参数应用、独立执行、结果缓存与关闭清理；流程保持在父级。没有以通用参数窗替代图像源专用窗口。
- 实际验证路径：Group内图像源→专用窗口修改InitialSN=17→关闭→Group独立运行→真实BMP宽度1296、节点SNCode=17。此为已集成插件现有能力验证，未合并插件工作线后续提交，也未验证相机硬件。

## 明确未完成的部分

1. 当前main只有图像源专用插件，且它没有ROI参数。ROI图形到文档/多目标/框架执行的验证使用测试模块；**尚未接通真实ROI算法工具**。图像源完整工具窗口入口不等于原生Group的单参数专属窗口。
2. 当前ROI契约只有矩形、圆、圆环、多边形。原生扇形、卡尺、线、点、矩阵圆、3D及屏蔽模式未提供；图形控件的全部边/旋转手柄、归一化坐标、夹取和原生事件状态机尚未完全等价。
3. Fixture缺少变换时明确拒绝，不把其数值当像素编辑。原生多边形捕获恢复、全部焦点/剪贴板行为与独立取消策略仍需差分验证。
4. 窗口外框、主题与阴影没有完成逐像素对照；截图是测试渲染的WPF客户区，不能验证系统/原生非客户区。原生SliderArrangeFloat的弹出滑轨/数值模板尚未完整迁移；当前ROI数值框、普通数值/范围/文件编辑器不是原生全模板恢复。模型训练与真实工具专属复杂编辑器缺少集成。
5. 关联选择仍使用现有来源树，未完成原生DynamicRunParamRelationModulesConfigWin全部布局；输入代理/历史未分组数据为已有独立兼容行为。
6. 没有本轮真实VM对照截图或差分运行记录。生成截图全部来自本项目测试渲染；图像中的BMP是固定样图。WPF检查通过不能证明原生UI/算法完全一致。

## 验证与查阅

实际WPF操作结果、界面截图、生命周期结果位于 analysis/full-audit/group-native-editors.html 及相邻文件。源码范围：GroupRunConfigurationWindow、GroupRoiEditor、GroupInputRunControls、GroupRunCategories、ToolEditorHost、NodeDebugExecutor。截图代码仅在tests。

下一步的实质依赖是插件线提供一个经过原始源码审计的ROI工具、明确ROI类型/坐标/图像来源及专属编辑协议；请求见CONTRACT-REQUESTS.md FCR-007。不能通过新增测试模块宣称已完成这一步。

## 本轮最终验证

- 330项实际WPF检查：双栏操作、类型/文本筛选、选类别后新增、两个真实图像源的批量勾选/32项截止/整模块取消、类别与参数管理、清空是/否及文档结果、图形绘制/移动/半径/顶点/复制/校验与保存、真实图像源InitialSN→SNCode/Width。
- 28项输入解析/独立调试检查；115项核心语义、18项现有图像源插件检查通过。
- 发布：dist/VisionEditor-WpfGroupNativeEditorsFinal，Start-Flow-Editor.cmd已指向该版本。
- group-native-editors-release-lifecycle/exit-results.json：Core/Plugin/WPF/EditorClose均退出码0，0.586/2.044/39.002/5.186秒。仅管理本次测试创建的进程；没有终止其他任务进程。此结果不能推广为所有外部驱动无泄漏保证。
- 实际结果：BMP宽1296，SNCode17；结果JSON与可打开的本机示例位于analysis/full-audit/group-native-editors-real-tool-results.json和group-native-editors-example.vmx.json。
- 已查看生成的双栏与ROI客户区图；未做真实VM画面/运行差分。24份原始源码/资源定义文件哈希、21份复制PNG的源/目标/哈希已保存。截图辅助仍全部在tests。
- NU1900：缓存的NuGet漏洞源不可达警告；没有代码编译错误。
