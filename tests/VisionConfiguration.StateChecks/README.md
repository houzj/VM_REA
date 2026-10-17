# 公共配置状态规则检查

运行：`dotnet run --project tests/VisionConfiguration.StateChecks`。

`Fixtures/NImageOriginalStates.json` 来自 `docs/plugins/configuration-model-evidence.json` 的受控原厂解析器记录。它使用真实原厂 XML、Apps.XmlParser 与 NImage 控制程序集，参数和 IVmModule owner 明确为合成对象；不包含原厂完整桌面、设备或真实 IO 图操作。XML/Group 复用相邻 ModelChecks 已跟踪原厂资源及来源哈希。

检查逐项回放全部 13 个状态（默认、两种坐标输入、定位开关与输入模式、旋转数、四种板型），比较全部命名行的值与显隐、各组显隐；另验证初始化、程序赋值和用户提交分离，成功写后自动执行、busy/global开关、失败回滚、未知操作/方法拒绝、订阅删除继承、ReOperate→OnTrigger、重复名称按原始元素寻址和目录驱动的模板分派。

状态引擎不加载原厂 DLL，不反射执行 XML 指定方法。外部服务均通过显式 `IToolConfigurationDispatcher`，插件专属可逆规则通过 `IToolConfigurationRule` 注册；未注册的操作保留诊断。NImage 的 HidePhysicInfoOperation 规则仅作为本检查中的独立重实现样例，未硬编码到公共框架。运行、保存、清除、关闭、恢复默认及真实 IO 关联仍需要宿主 typed dispatcher 实现；诊断列表不能当作已实现功能。模板目录识别不表示独立控件渲染已验证。

此检查证明以上范围的状态行为和源证据可对照，不证明完整原厂 UI/运行等价。
