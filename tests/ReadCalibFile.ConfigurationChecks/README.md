# 标定加载：公共配置模型迁移验证

`dotnet run --project tests/ReadCalibFile.ConfigurationChecks/ReadCalibFile.ConfigurationChecks.csproj`

独立模型适配使用产品程序集内嵌的原厂 AlgorithmTab.xml 和 Group.xml，不依赖安装目录。原文件及路径图标来源为 VisionMaster4.4.0/Applications/Module(sp) 与反编译 Apps.XmlUI/resources。`ReadCalibFileConfiguration.cs` 不创建 Window；宿主通过 `IToolConfigurationProvider` 创建公共 `ToolConfigurationModelWindow`。旧 ReadCalibFileWindow 仅为兼容入口保留。

本检查构造隐藏公共窗口，运行真实 GraphExecutor；订阅树测试使用显式合成 typed service，不能当作原厂平台订阅运行证据。覆盖初始参数读取、源 operation destination（SnapImagePointX1 而非caption）、10个订阅控件、实际文件读取、动态OutputSchema、72个只读坐标框、失败路径回退、真实图自动运行与结果、宿主busy/ready/loading/RunEnabled拒绝、缺服务明确诊断、关闭路由、独立配置用户数据持久化。旧 810 项原厂受控运行/像素数值回归由 ReadCalibFile.PluginChecks 保留。

`ConfigurationUserData` 是新项目的显式隐藏JSON参数，承载原厂独立 UIUserData（如 @SaveCalibDataEnable），不宣称与原厂保存字节布局相同。缓存/输出schema/动态I/O仍按既有专属执行实现；配置界面只适配状态与操作。

产品专属叶依据：

- OpenFileDialog4ReadFile.ValueChangedCmdExecute：base 写参数后 ReadFileConfigData.ReadData。
- ReadFileConfigData.GetParamUnit/ReadData/AnalyzeString：模块原数据描述按类型分组，显示名称通过原Lang解析。
- 原 datatemplate/paramusercontroldatatemplate.xaml：OpenFileDialog4ReadFile 高34，内部OpenFileDialog_sub；StaticParam边距30,5；CalibPointList标题30,5,0,0、表34,10,0,0、三列只读框高30。
- OpenFileDialog_sub 的文本/图标/四圆角外框与原3张文件路径图标。

截图 `analysis/full-audit/read-calib-file/configuration/source-derived-read-model*.png` 是新公共模型运行结果；没有原厂桌面对照，不证明像素等价。

待统一宿主完善：默认打开目录历史和通知现已接公共服务（独立JSON存储与WPF默认通知外观映射）；原厂远程 IsUploadServer 分支未映射；工具语言当前随此适配默认中文（公共宿主语言注入待接）；完整叶主题、真实平台订阅、原厂桌面逐像素比对及用户手动验收仍未闭合。缺少执行状态服务时不猜Ready；图校验Ready是独立框架边界，非原SDK IsReady 的已测等价。

公共文件/通知接入后为41项：额外验证同文件重选强制重读、原厂选择/写参/通知/发起运行/目录保存顺序，存在但无效XML的错误通知后仍记录所选目录，纯键盘提交不改history。每次测试以worktree内独立history目录注入真实WpfConfigurationFileService、注入通知presenter；不弹出系统选择框/消息框。

文档事务补充为44项：拒绝宿主写入时，FilePath/OutputSchema与运行数据恢复旧值，不发自动运行；普通参数提交失败后重新初始化仍读取旧的已接受值。这是独立宿主事务保证，不冒充原厂SDK自动写后读回。
