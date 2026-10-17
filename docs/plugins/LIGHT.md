# 光源（LightModule）

代码入口为 `vision.light@1.0.0`，与图像源、多图采集、缓存图像共用当前插件程序集。按用户要求不做设备测试。本记录分别说明原厂源码、原厂算法的受控执行、独立实现和未验证边界，不把检查通过等同于原生全场景等价。

## 原厂身份、源码与资源

- 目标目录：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/LightModule`，12个文件的身份见 [light-evidence.json](light-evidence.json)。原生DLL SHA256：`9ad006589ed6082973c0b991105f80a7bda6397ef95b33d003836a235a8043c3`。
- REA检查 `LightModuleControl.dll` 的托管身份及资源，证据ID `ev_d368c73298a4f06ee09da334b3a6a10ed90d9baf8f2c68abded4485e4af16e22`，见 [managed-evidence](light-managed-evidence.json)。ILSpy提取 SDK 与专用控件到当前 `analysis/full-audit/light/managed-*`；产品不引用这些原厂程序集。
- 原生分析使用本地 **Ghidra CLI**，不是REA深度提供器。关键地址：构造 `0x7e80`、初始化 `0x9cf0`、设备重置 `0x9fb0`、亮度范围 `0x8600`、输入更新 `0x8fa0`、Process `0xa790`、GetParam `0x9110`、SetParam `0xb510`、类型迁移 `0xc6d0`、保存 `0xb320`、加载 `0xa1d0`。反编译及关键函数汇编位于当前 `analysis/full-audit/light/native`，保留不确定的反编译签名，并用汇编核对下发参数和整数输出寄存器。
- 原厂静态声明是32参数、7输入、26输出；原生实现实际读取8路亮度并发布8路使能/亮度，另有8路状态/沿/持续时间的代码路径。受控SDK运行确认第7/8路输入能设置、参与计算并输出，未把旧XML声明当成运行上限。
- `LightModuleAlgorithm.xml` 确认运行默认 `TriggerTime=500`、`OutputType=1`；界面XML的默认0/2会被运行参数读取覆盖。SDK `LightParam` 的32位整数范围、VB状态别名与原生读写实现已核对。
- `LightModuleAlgorithmTab.xml` 含58个控件、10种设备可见性规则；生成 `Resources/LightDefinition.json`，保留顺序、名称、显示规则和范围。`SetBrightnessRangeOperation` 确认参数1对应上限100、参数2对应255。专用 `EnumerationLightForLight.OpenGlobalLight` 调用公共资源管理入口；`EnumerationLight` 和 `LightSourceHelper` 确认旧全局资源型号迁移及选择字符串 `index#moduleId#type#guid`。
- 原厂 `Apps.Style/RadioButtonStyle/RadioButtonStyle.xaml` 与两张 `check-box_*.png` 作为输入类型单选控件依据；本地化后的样式只使用当前程序集资源。开关、数值控件、组合框和链接图标复用既有原厂资源适配。

## 不连接设备的原厂运行证据

`tests/NativeEvidence/OriginalLightProbe.cs` 在自有SDK进程加载只读样例、新建自己的流程和模块。样例报告 `0xe0000333` 部分加载错误，已保留；自己的目标模块能够创建和执行，不称为整个样例加载成功。

**246组算法运行、24张原厂SDK控件截图** 已导出。截图覆盖未选择设备、101/102/103/104/105/107/109/110/113/114及SVC类型输入，两种输入模式均执行原厂 `ValueChangedCommand`。同时记录每个控件的可见性、当前值、最小/最大值。在该版本中，切换“输入/订阅”后亮度数值字段仍显示，单独的BrightnessSub字段仍隐藏；实现保留这一观测，没有凭名称改换布局。手动模式及换设备会清除前6路亮度订阅，与XML一致。

下发成功/失败分支通过**明确标注的测试传输替身**执行：探针校验原厂DLL哈希、确认自有进程 `LightModule.dll` 的 `VM_M_SetBinayData` 导入指针，再仅替换本进程导入槽RVA `0xf2c0`，捕获命令并返回受控结果；退出前恢复指针。原厂DLL磁盘内容未改，不访问其他进程，不连接控制器。设备目标统一为不存在的模块ID `2147418112`。这证明原厂算法在指定传输结果下的行为，不证明物理设备行为。

每次记录参数、实际输入、原厂原始整数输出、错误码、下发次数、目标、命令及完整748字节内容。独立实现逐字节对照，没有忽略保留字段。探针使用15字节ASCII标识符以避免原厂短字符串复制未初始化尾部的行为；独立编码器对短标识符补零，并按原厂固定16字节UTF-8字段截断长标识符。

## 已确认行为及实现

| 条件 | 原厂依据与当前行为 |
|---|---|
| 新模块未选择设备 | 无下发，错误码0、ModuStatus=0；8路使能/暂存亮度仍输出 |
| 新格式触发 | OutputType=1匹配`OK`，2匹配`NG`，包含结束零字节的比较；`OKtail`、大小写不同和短字符串不触发 |
| 不匹配触发 | 不更新暂存亮度，不下发；继续输出上次触发暂存值 |
| 匹配触发 | 8路订阅亮度优先，否则取各路手动参数，然后发送一次`LightDeviceParamSetTrigger` |
| 下发成功/失败 | 成功ModuStatus=1；传输失败为0，原生错误码仍可为0；两种情况都保留本次暂存亮度 |
| 订阅亮度越界 | **先下发，再验证输出**；返回`0xe000030e`，SDK观测失败状态为0，保留8路使能及出错通道之前已产生的亮度输出 |
| 亮度范围 | 103/104/105/107/109/110/114算法范围0～100，其余当前类型通常0～255；UI某些通道范围与算法范围有差异，分别依据资源和原生实现 |
| 切换有效设备 | 暂存亮度清零，使能关闭、状态/沿复位1、持续时间复位0；手动亮度参数保留。由255范围切到100范围后，原有150仍能在下次触发进入发送，再报范围错误 |
| 同节点跨次运行 | 暂存值保留；不同节点/会话隔离；删除节点或关闭会话清理。状态不嵌入VMX保存 |
| 未产生输出 | 端口缺席，不用SDK结果类的默认0伪造未发布状态/沿/持续时间。当前新类型的这些字段可能不发布 |

命令结构为748字节：索引+0、类型+4、标识符+8（16字节）、触发时长+24、首通道+28。每通道44字节，通道内序号+0、使能字节+4、亮度+8、光源状态+12、沿+16、持续时间+20。完整保留剩余零字段。亮度输出表示暂存值，不是设备回读或成功应用的证明。

运行端口扩展为9输入、42输出，补足原生第7/8通道及可选状态字段；配置是32个算法参数加UI/原生补充字段，共46项。`InputType`作为界面持久化值保存。SDK原始文本参数接口、任意DynamicInData/DynamicOutData XML及SOL二进制并非VMX接口。

## 公共接口与宿主

- 复用 `IAcquisitionLightModuleTransport` 的资源目录、模块目标和二进制命令调用边界。设备目录、连接、驱动和管理弹层由清单中的全局 `LightControl` / `CommManager` 提供，本工具不推断串口或网口协议。缺少传输服务时按未成功下发处理，不伪造设备成功。
- `ILightResourceConfiguration` / `ToolEditorContext.ConfigureLights` 接通公共管理入口；宿主注册对应资源管理服务后启用设置按钮。共享管理弹层的完整实现及逐像素等价不在此次证据内；无该服务时按钮禁用。
- 新增末尾枚举 `WorkflowSourceKind.TextConstant`，Id存储JSON字符串，支持空白、中文、内嵌零字符的无损持久化，严格校验类型及JSON。`InputText/SetInputText`支持触发字符串键入、清空、订阅切换与文档撤销。参数订阅继续使用现有整数接口。
- WPF与兼容宿主注册第四个工具；光源不作为图像源加入预览选择器。模块执行/连续执行/停止使用已有当前模块自执行入口，不重跑上游采集模块。
- 改动只在插件worktree，契约、生命周期和迁移在 [CONTRACT-REQUESTS.md](../CONTRACT-REQUESTS.md) LT-001～004。新TextConstant文档需要更新后的宿主，既有枚举数值保持不变。

## 验证与边界

入口：`Invoke-OriginalLightProbe.ps1`、`Export-LightDefinitions.py`、`Export-LightEvidence.py`、`Light.PluginChecks`。最终命令、日志、源码、资源和截图身份见 [light-verification.json](light-verification.json)。原始DLL、反编译工程、Ghidra数据库及完整SDK运行输出留在当前analysis，按仓库规则不随Git分发；本地原厂安装是重放前提。

设备测试按用户要求排除。传输替身不证明控制器连接、响应、发光、时序或电气行为；当前资源管理服务和原厂全部共享弹层未逐像素核对。旧SOL的版本小于340时存在绕过字符串触发并使用-1触发时长的兼容路径，当前新建VMX按新格式执行，未实现SOL二进制导入。原厂短标识符尾部未初始化复制、非法内存及内存分配失败不复刻。独立调度、计时、共享选择器及这些边界均不宣称原生完全等价。

最终全解决方案构建0警告、0错误。20个检查程序共15,270项通过：光源11,836、实际WPF宿主257、图像源767、多图采集327、缓存图像1,623、其他15程序460。检查通过只证明列出的代码与受控证据范围，不据此推断全场景原生等价。
