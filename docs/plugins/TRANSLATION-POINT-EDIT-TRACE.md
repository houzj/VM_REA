# 平移旋转标定点编辑提交顺序

## 原厂证据

原厂 `NPointList.xaml` 的 `ImagePointX`、`ImagePointY`、`WorldPointX`、`WorldPointY`、`DeltaAngle` 列都使用 `UpdateSourceTrigger=LostFocus` 绑定到对应 `PointList` 属性。`PointList.cs` 中五个属性 setter 结构一致：比较原字符串；变化时按 `属性名 + ID` 写入 `IUserStringData`；写入成功后才更新 backing field 并发出 `PropertyChanged`。写入异常被 setter 捕获，字段不更新。因而原厂的编辑单位是单个字段，成功 setter 先于后续编辑提交生效。

原厂少于五列数字行的导入异常与此表格编辑路径分开记录在 [导入源码对照](TRANSLATION-POINT-IMPORT-UI-TRACE.md)：构造对象初始化按 X、Y、WorldX、WorldY、DeltaAngle 求值，读取第五列时先抛 `IndexOutOfRangeException`，前四个 setter 已执行，DeltaAngle setter 和后续 setter 均未执行；外层 `ButtonImport_Click` 捕获异常并显示错误提示，且在 `GetPointListByText` 正常返回前不会替换旧列表。

## 实现差异与修复

独立实现此前在 `TranslationTableWindow.Commit` 中遍历所有行和字段，修改本地 JSON 值后只调用一次 `apply()`。这与原厂逐个 setter 发布不同：若较后的字段解析失败，较早字段虽然已改变本地字典，却没有被发布到宿主。

现在 `PointRow` 记录最后成功提交的原始字符串。`Commit` 按行、按五列顺序检查变更；每个已变化且有效的字段单独写入并调用 `apply()`，然后才标记该字符串为已提交。未变化字段不触发写入。若后续字段解析失败，前面成功字段已逐项发布，失败字段及后续字段不发布。这复现了源码能够确认的 setter 边界；宿主回调若拒绝写入时的原厂 `IUserStringData` 异常表现，独立 `apply` 契约没有逐属性拒绝结果，仍不能声称一致。

## 专属验证

WPF 检查构造两行点表，把第一行五列改为有效新值、第二行前三列改为有效新值、第四列设成非法文本，并调用窗口提交路径。断言解析在第八个单字段发布之后失败、第二行前三值已生效、第四列与第五列仍保留旧值。这是独立实现的顺序回归，不是原厂模块运行证明。

`dotnet run --project tests/TranslationCalibration.PluginChecks/TranslationCalibration.PluginChecks.csproj -c Release` 通过，`TranslationCalibration.PluginChecks: 7464 checks passed`，退出码 0。运行前产品源码 SHA-256 为 `8b2216c99ca46c0ba9cf8dcdf14887bc318212201ac60472995c1a95f02d61ee`，本次修改后及验证时为 `aaf789a9efd17afeadec197b73287ceffcba922236e17dc6738085ba71925cec`（`sourceChanged`，修改来自本次修复）；检查源码 SHA-256 为 `ca7fd2f31d13a4c8309dd0e198dd81106688e7a9e8a712e67b8dc4fb77a6c53c`。原厂反编译 `PointList.cs` SHA-256 为 `cf0a718f5a12e085fb018ee628b07129c4bec290c5f5ae4acce4838c583bb624`；原厂点表 XAML SHA-256 为 `1156090a91cecfa8dc09042943202b5a7b4a3b19c17b913f31a210b722e644a5`。

本项只验证插件线 WPF 点编辑和参数发布回调，不做设备测试，也未修改主框架工作树。真实宿主参数服务拒绝单字段写入的弹窗/回滚仍需原厂或宿主运行跟踪；当前报告不据此宣称完整逐像素或全部生命周期等价。

## 清空按钮的 UI 快照同步

原厂 `NPointList.xaml` 的“清空标定点”按钮绑定 `ButtonClear_Click`。反编译 `NPointList.xaml.cs` 中，该处理器先写 `IUserStringData["Clear"]="0"`；成功后按当前 `PointList` 顺序逐行设置 `ImagePointX`、`ImagePointY`、`WorldPointX`、`WorldPointY`、`DeltaAngle` 为 `"0.000"`。`TranslationCalibModu::SetParam`（RVA `0x3d740`）识别 `Clear` 后立即清内部当前点数和点/角缓存；之后逐字段参数写入更新各自缓存。

插件配置打开编辑器时会把外层配置 `values` 复制成 `TranslationTableWindow` 独立字典。原先按钮只调用 provider 的清空回调，再从编辑器自己的旧副本 `Load()`，因此宿主参数与插件状态已清空、编辑表格却继续显示旧快照。现按钮在清空回调成功后将编辑器本地五组点字段置零，再重载行；回调抛错时不会继续覆盖本地编辑器值。离屏 WPF 专属检查分别验证回调接收侧的外层参数快照与编辑器字典，并确认重载后行显示 `0.000`。

源锚哈希：原厂 `NPointList.xaml.cs` `AF17CDAB8A3B4E57F94097F948DB7C9A4E1070BCBA328319683B0E98085B3835`；原生 `18003d740.c` (`SetParam`, RVA `0x3d740`) `ED0A1466E3E5B9114E51604AF4AED172CE559B98B67C10801E6D245ED49B433A`。实现 `TranslationWindow.cs` 修改后 SHA-256 `D74E18309439BB27323B91158EB4F03FBCA87BF24C5C60E62936AE28F4D3E3B9`；专属检查 `UiChecks.cs` SHA-256 `BABD57CA2BA32234465C162BD37F46547B5E9F00BB9609DE8092A340B8729973`。`dotnet run --project tests/TranslationCalibration.PluginChecks/TranslationCalibration.PluginChecks.csproj -c Release`：`7466 checks passed`，退出码 0。该测试为插件 WPF 清空回归；原厂调用顺序由反编译源码确认，不是原厂桌面实操记录。
