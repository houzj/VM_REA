# C4 IAdditionService 实际注册与当前安装空扩展分支

2026-10-19；仅被动原件/配置/IL取证，无Assembly.Load目标、无原厂执行、无产品修改。保持C总6/32，C4 S1/S2未完整。

## 注册入口与返回类型边界

原VMControls.Interface.dll SHA256=47C11FF2239BD2577378C4EB202EF6B2AEE8FC0E99C3B7F0692C2209EB69C304。REA evidence `ev_94e7251f01d24281bffa0e9c0300c8c5f0bd0b9e995a679e1e4847075df3db3f` 完整被动成员证据保存在addition-interface-rea.json。

具体入口是VMControls.Interface.CheckAdditionExist..cctor，token0x060000b3，RVA0x2374，144字节IL SHA256=F43D1CE8157DDEC125B9800BF04CEEB6E349995725873C4BED7A76ED7CFCF9B6。它先IsExistAddition=false；读取ConfigurationManager.AppSettings["AdditionPath"]；空值分支保持false。非空值拼接AppDomain.CurrentDomain.BaseDirectory与该配置，再File.Exists；不存在则提前返回。存在才Assembly.LoadFrom→GetTypes→FirstOrDefault(predicate)，predicate token0x0600020b调用type.GetInterfaces().Contains(typeof(IAdditionService))；接着Ioc.Set(typeof(IAdditionService),选出的Type,true,null,null)，再标志true。没有过滤抽象/不可构造类型的附加谓词、没有catch。无匹配类型、Load/GetTypes/Set异常的最终行为不以“可选所以忽略”补齐；静态构造异常可能阻断后续HotGet。

Ioc.HotGet<T>实际先检查CheckAdditionExist.IsExistAddition，再容器IsRegistered<T>，注册才Resolve<T>(空参数数组)，否则default(T)。本次MethodSpec精确解析补出了IsRegistered/Resolve、FirstOrDefault和Contains目标，避免把MethodSpecification标签当已还原目标。Ioc.Set内部生命周期策略未完整恢复，本片不把bool=true解释成某个具体容器生命周期。

ImageViewBox.OnApplyTemplate取Ioc.HotGet<IAdditionService>()?.GetImageViewAdditionInstance(this)。声明返回IImageViewAddition，具体运行时类型由上述配置DLL中的实现方法决定，不在接口声明中硬编码。IImageViewAddition.ROIListChangedOut(object)是无方法体接口成员；该接口当前声明成员列表无Dispose方法，不据此推导实现类不实现IDisposable。没有具体提供DLL时无法取得其ROIListChangedOut/Dispose体。

## 当前安装实际分支

`D:/Program Files/VisionMaster4.4.0/Applications/VisionMaster.exe.config` SHA256=9C5DBC6B41D66EC4E902400299808F5A68356FE35438FBC621444B053F4FEA9D，129行有效项AdditionPath为空，130行仅注释示例myLibs\VM.Addition.dll。该示例路径文件当前不存在。因此以这份exe配置及原程序BaseDirectory启动、没有其他配置替换/进程状态改变的静态分支下，IsExistAddition=false，HotGet<IAdditionService>返回null，ImageBoxAddition=null，ROIListChangedOut扩展调用不执行。未启动进程核实际AppSettings，因此这是安装配置确认的条件分支，不能推广到其他宿主或配置。

仅对Applications顶层、myLibs、ApplyModule、ModuleProxy、ServerApp五处非递归DLL/EXE（<=100MB）76原件做方法名元数据字节候选筛选，哈希清单addition-candidate-scan.json；命中接口和既有调用方，没有新增提供者。该筛选不是类型实现证明。此前递归全Applications扫描时间过长，已中止自有扫描进程，没有形成完整结果，不能声称遍历全安装无提供者。确认空配置分支依赖配置与静态注册体，不依赖该不完整负搜索。

## 对上片释放缺口的收束

当前安装空配置条件下，扩展分支不存在，不能再要求不存在的实现类Dispose作为该分支闭合前提；但WPF ItemsSource内部订阅释放、旧ROI命令/PropertyChanged所有者仍保持独立未知。若将来提供非空AdditionPath，必须取得其实际目标原件哈希、服务实现与返回IImageViewAddition具体类型，再取ROIListChangedOut和释放体；不能借用本空分支或候选holder宣称其完整释放。

选型元数据及MethodSpec索引在managed/addition-with-specs.json，生成器InspectC4AdditionSpecs.ps1；原REA完整IL含分支锚点，选型工具仅辅助解析成员/字面量，未执行反射加载。新增原配置快照哈希与存在性结果在addition-environment.json。
