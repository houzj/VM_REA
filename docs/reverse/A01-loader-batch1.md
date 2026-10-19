# A01：原始 UpdateLoad 的隔离托管运行证据

2026-10-19；main 基线 `2649c3b268df351a1585c30546a0115c4fb3753e`。本批运行原厂未修改的 `VMControls.Data.ToolBoxData.UpdateLoad`，不以独立重写状态机代替原方法。17 个断言、10 条观察通过，执行器最终退出 0。A01 一级包仍开放：本批没有执行完整工具箱 UI、LoadData/Reload 或 Group SDK 导入。

## 身份与取证边界

- 原文件：`D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VMControls.Data.dll`。
- SHA256：`A0321EB4669C5114C791B32AF8D0CE5AC17A690210BC87319908658F7457A9F8`；MVID `8e3bf4de-1ba9-4d75-adc1-10ec9b2523f8`；assembly 1.0.0.0，.NET Framework 4.6.1、AnyCPU。
- REA 身份证据：`ev_bf0242bcea12b015c7793a0fae62b7d2ef8114378a17786e8fb5ac39435231e7`。
- REA 成员/CIL 证据：`ev_0dd9b2693de8c6d62eba0086d85c7cacb827c47fdf8e5f9d58ca3e99db6c2377`。原始 JSON 保留在 `analysis/full-audit/batch1-20261019/A01/rea-*.json`，不是仅保存终端摘要。
- 反编译源码入口：`analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs:77`；本批用实际程序集执行验证其分支。Group 回调静态链沿用 `docs/LOADER-ROUND2-20261019.md`，不重复宣布为本批动态证据。
- 探针：`tests/NativeEvidence/A01/LoaderStateProbe.cs`、`Run.ps1`；仅以反射设置探针创建对象的 group/placeholder/flattened collection 初态；调用原 `UpdateLoad`，不修改方法体或安装 XML。

REA 静态工具不执行目标；运行结果来自独占 .NET Framework 探针进程，二者分别标识。没有打开 native 分析会话、启动平台、初始化设备、接管桌面或触碰 tool-plugins。

## 构造、依赖和副作用的源码核对

执行前按实际依赖追踪，而非假设“托管类无副作用”：

1. `ToolBoxData::.ctor` token `06000050` 只设置命令/布尔字段并调用 `Apps.Data.NotifyPropertyChangedBase::.ctor`；`.cctor` token `06000051` 创建同步对象和静态名称数组。
2. `ToolBoxItemData::.ctor` → `Apps.Data.ItemData::.ctor` → `NotifyPropertyChangedBaseNode::.ctor` → `Hik.Xml.BaseNode::.ctor`。后者创建 AES helper/NameList，设置 nodeName；AES 单例构造没有文件或平台调用。`BaseNode::.cctor` 建立本进程 startup/config 路径、ThicknessConverter 和同步对象。
3. 原 `BaseNode.loadXml(string,bool)` **会把输入文件 Attributes 设为 Normal**，且设置 `ActivatorWrap.IgnoreUnknown`，因此即使是“读 XML”也不能直接用安装原件。本探针只传自有合成 XML。
4. `Hik.Util.ActivatorWrap::.cctor` 会尝试 startup/myLibs/Librarys.Config；缺失会读取注册表回退至 SDK 路径，然后装载配置列出的程序集。执行器在独占 exe 基目录建立空 `myLibs/Librarys.Config`，使这段加载列表为空。没有替换厂商类、没有使用安装的全量装载清单。
5. `Apps.Log.LogHelper::.cctor` 会读取 HKLM 的 VisionMaster AssemblyFoldersEx 默认路径，并优先使用该目录下 `myLibs/Apps.BaseLog.config`；本机该文件确实存在。还会创建后台日志线程。仅设置探针工作目录不足以假定记录器隔离，因此本批不执行会调用 LogHelper.Info 的 LoadData/Reload，不实例化该日志服务。

对应本地 CIL：`analysis/full-audit/batch1-20261019/A01/Apps.Data.il`、`VM.Xml.il`、`VM.Util.il`、`Apps.Log.il`、`VM.AppData.il`。通过已有 `dotnet-ildasm 0.12.2.0` 导出；它默认要求 .NET Core 3，首次启动失败，随后仅在当前命令进程设置 `DOTNET_ROLL_FORWARD=Major` 后成功，没有安装/升级运行时。未使用该工具的空 `-i` 输出作为源码证据。

补充 REA 成员证据：Apps.Data `ev_6b090bcc7e8e2d75e7f306578daba8bb1dfa0a5f253668b95a297c0ead3e2ff6`；VM.AppData `ev_e53d49372f31754c73c98069575e4b68dc677d7ce949b658378f71898ee49d67`；Apps.Log `ev_947e7b8f95f6d81b852abe4c85089a35928a22b4f95d3749c2ac718abf1b3684`。这些静态调用不证明相关构造已运行。

## 本批真实执行

最终目录：`analysis/full-audit/batch1-20261019/A01/replay-373f6ddc25334451bc209272886d9273/`。

`observations.json` SHA256：`F3469AEB7BD3517FF0604160FDE9990EAD2737075F5B2B9B36FB10E347740F78`。

| 操作与输入 | 原方法实测 | 判定边界 |
|---|---|---|
| 探针创建普通 group，初始 1 placeholder，两个 XML priority=2/1 | IsInit false→true；children/flat 均为 Second、First；placeholder 移除；子项 Parent 引用同一个 group | 原方法首次成功装载、优先级排序和 parent 身份已观察 |
| 原对象装载成功后新增第三 XML，再调用 UpdateLoad | 无异常，仍保留原两个条目 | 已初始化对象没有重新扫描 |
| 两目录，实测枚举最后一项 XML 损坏 | XmlException；IsInit=true；children 只含第一项，flat 仍为原 placeholder | 初始化标志前置、两集合分阶段变更已观察；不宣称目录枚举顺序普遍固定 |
| 修复损坏文件，再调用同一对象 | 无异常，但仍为部分 children 与旧 placeholder | 修文件不会触发同对象重试 |
| 探针另建全新 group/flat 后调用 | 两项成功装载，placeholder 消失 | 这是探针重新创建初态，**不是**执行原 Reload |
| 目录不存在 | DirectoryNotFoundException；IsInit=true；原 children/flat 未清空 | 目录枚举发生在 clear 之前 |
| 目录有 XML，但初始 children 为空 | IsInit=true；children/flat 仍空，无异常 | 空初态在取得目录后提前返回 |
| Name=Lang_Tools，目录不存在 | 无异常；IsInit 仍 false；placeholder 保持 | 全局类别跳过发生在目录访问及初始化写入之前 |
| null 参数、非 CollectionViewGroup 参数 | null 无操作；普通 object 抛 NullReferenceException | 未增加探针侧类型容错 |

17 个断言是上述固定场景中的字段、集合和身份检查，不是 17 个完整 UI 功能。探针使用自定义 CollectionViewGroup 子类仅提供 Name，没有构建 WPF CollectionView/Popup/Expander；实际 UI 能否进入空组仍未知。

加载身份核查：CLR 对 Apps.Data、VM.Xml、Apps.Interface、VMControls.Interface、VM.Util、VM.Compress 使用本机 GAC；最终 `loaded-assemblies-sha256.csv` 记录其路径和 SHA，并与 myLibs 同名原件核对一致。VMControls.Data 原件自身也一致。Mvvm 从安装 `3rdLib/MVVM` 装入并记录 SHA `5685C13C79321E7D7EC4ED85E764D00C915CA54401D5CCB6314414D54F76E120`；不能把“版本号一样”当作字节身份核查。已加载清单没有 Apps.Log、VM.Core、VM.PlatformSDKCS 或原生 SDK；没有主动平台/设备初始化。

执行命令：`& ./tests/NativeEvidence/A01/Run.ps1`。编译使用系统 .NET Framework csc，只编译探针；没有跑共享 dotnet/WPF 构建。安装 myLibs 下 DLL 执行前后哈希一致，保存在 original-before/after-sha256.csv；输出另有 output-sha256.csv。此核查范围是这些 DLL，不扩大宣称整个安装目录字节未变。

首次 replay `replay-e6a3b129c7da4a79be25011d75c692a7` 的原方法也通过 17 断言，但执行器对输出目录边枚举边写哈希 CSV，误将该 CSV 纳入且遇到文件占用，执行器退出 1。已保留该目录并修成先计算哈希再写 CSV；最终新目录完整执行退出 0，未隐去首次报告器故障。

## 原始 CIL 锚点与限制

| ToolBoxData 成员 | token / RVA | raw CIL SHA256 |
|---|---|---|
| UpdateLoad | 06000045 / 0x2bf8 | 66f717294d448df43c1f18a3c1674a8cb0c815f030bc1d838c22086c072dc4d0 |
| Reload | 06000044 / 0x2bbc | 9f9b16c9bebaae66f87ebfed301a2da173790348e9792edc63f08639be853d40 |
| LoadData | 0600004f / 0x33d8 | 0b037e40a929f7a0b375fec77a0418abd2664e1027f8d19d684cdbf01f749026 |

raw CIL 哈希不包含方法头、局部签名和异常段。UpdateLoad 有 foreach 枚举器释放的 finally（IL384–440，handler440–455），**没有**失败后重置 IsInit 或回滚集合的 finally；不能把“存在finally”解释成故障恢复。

Source-confirmed：Reload 清三个缓存、通知两个属性；LoadData 普通组创建占位项，UserTools/GroupTools 首项策略。Runtime-observed：仅上表 UpdateLoad 分支。Independent-reconstruction：探针构造的最小初态、空装载清单、结果序列化与断言。Unknown：LoadData/Reload 实际对象/CollectionView 变化、原机鼠标/展开/右键事件、Group SDK 回调时序、无 Cs 与构造异常的真实宿主差异、平台导入失败恢复。

main 产品仍使用独立 metadata scanner，本批不改产品、不模仿原厂失败残留、不改跨项目合同。后续 A01 子项应先把日志配置及依赖列表完全限定在自有宿主，再执行实际 LoadData/Reload；Group 回调须独占原生实例并记录 callback→AddModuleByInfo→Modules/Children→decorator 的顺序。完整 A01 尚未达到关闭门槛。
