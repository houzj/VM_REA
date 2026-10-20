# 平移旋转标定点表导入 UI 源码对照

## 原厂成员与动作顺序

原厂模板资源 `TranslationCalibModuControl/datatemplate/paramusercontroldatatemplate.xaml` 中 `EditButton_Calib` 的按钮命令绑定 `EditDialogA.ClickCommand`。`EditButton_Calib : Integer` 暴露 `EditDialogA`，其 XML `EditDialog` 指向 `TranslationCalibModuControl.NPointList`。

点表窗口在 `NPointListWindow.Init` 中将宽度设为 520，初始高度根据点数计算；`NPointList.UserControl_Loaded` 再将窗口高度设为 399。`NPointList.Init` 按 `CalibPointTotalNum + RotPointTotalNum` 创建行，初始 getter 通过 `float.Parse(...).ToString("f3")` 格式化点值。

原厂导入路径的关键顺序位于 `NPointList.ButtonImport_Click` → `GetPointListByText`：

1. 读取当前 DataGrid 的点数作为容量，按 `\r\n` 拆分文件，最多处理当前容量的行。
2. `GetPointListByText` 在本地新建 `ObservableCollection<PointList>`。逐行先对所有空格分隔字段调用 `MathHelper.IsNumber`；然后按原字符串创建 `PointList`，只用前五列赋给图像 X/Y、物理 X/Y 和角度。少于当前容量的有效文件会返回较短集合；空文件返回空集合；多余行截断。
3. `PointList` 五个属性 setter 会分别写 `IUserStringData[paramName + ID]`。所以处理到后续坏行前，之前成功创建的行已经写入参数存储。
4. 只有 `GetPointListByText` 返回非 null 后，调用者才执行 `PointList = pointListByText` 替换 DataGrid 集合。坏行返回 null 或解析异常时，旧集合仍显示；此前成功 setter 的参数副作用不回滚。
5. 成功导入后，DataGrid 直接绑定新集合。表格显示行数等于成功解析的行数，导入文本保留原字符串；导入行之外的总点数参数不在此处修改。

## 发现并修复的独立实现差异

旧版 `TranslationTableWindow` 导入使用 `finally { Load(); }`，不论解析成功或失败，都按参数里的点总数重建 DataGrid。它因此与原厂成功导入后的集合行数不符；失败时也会把已发生的有效前缀参数写入重新加载并显示，而原厂保留当前 DataGrid 的旧 `PointList` 集合。成功导入时旧实现还将解析出的浮点值重新格式化为三位小数，原厂导入行则显示 `PointList` 中的原始字符串。

现在 `TranslationTableWindow.ImportPointText` 先验证当前行所有已有 token，再按原厂对象初始化器的 `ImagePointX → ImagePointY → WorldPointX → WorldPointY → DeltaAngle` 次序逐字段更新参数；每个 setter 对应一次即时参数发布。只有完整行结束后才计入待显示的新集合。解析中途失败时，已完成行和当前行已执行的属性 setter 副作用保留，但不调用 `Load`，所以旧 DataGrid 集合继续显示。成功时按完整行数替换 DataGrid，并保留导入原始文本；成功的空文本将表格替换为空集合，点数量参数维持原值，符合原厂 `GetPointListByText` 不改计数和成功后直接赋集合的顺序。

专属检查在真实 WPF `TranslationTableWindow` 上确认：短文件导入后显示两行但配置点数仍是 9；高精度原始文本如 `1.23456` 原样显示；第二行含非数字 token 时，当前坏行无字段写入、之前完整行副作用保留，旧 UI 集合不变。少于五列但所有现有 token 都是数字时，原厂先完成全部现有 token 校验，再按对象初始化顺序写 `ImagePointX0`、`ImagePointY0`、`WorldPointX0`、`WorldPointY0`；求值 `DeltaAngle = array2[4]` 时抛 `IndexOutOfRangeException`，因此 `DeltaAngle0` 不变且尚未完成构造的当前 `PointList` 不加入集合，旧 UI 集合仍被保留。检查复现前四个值已写入、角度保持旧值及 UI 原集合不变。空文件成功后表格为空。

## 证据身份

| 文件/制品 | SHA-256 |
|---|---|
| 原厂 `TranslationCalibModu.dll` | `a85a7ca08ea265b2cf881b4ff01bf7538a60e9edb7466478476584b08fe25183` |
| 原厂 `TranslationCalibModuControl.dll` | `6353098907df95e3e4f430b3372a97a9c7a5fedab8f656fba50bbb1a1417b0be` |
| 工作树 AlgorithmTab 语义副本 `TranslationCalibModuAlgorithmTab.xml` | `11a2fc389098112ea40f45b0684affa19e5102be557ac1cbcefa4590cf97d8fe`（原始资源 SHA 见 [translation-calibration-definition.json](translation-calibration-definition.json)：`3d626213bab65b7192fc27cfe3b0d5a2e13e595b5f349424adc327fe2aead062`） |
| 反编译 `NPointList.xaml.cs` | `af17cdab8a3b4e57f94097f948db7c9a4e1070bcba328319683b0e98085b3835` |
| 反编译 `PointList.cs` | `cf0a718f5a12e085fb018ee628b07129c4bec290c5f5ae4acce4838c583bb624` |
| 反编译 `NPointList.xaml` | `1156090a91cecfa8dc09042943202b5a7b4a3b19c17b913f31a210b722e644a5` |
| 反编译 `NPointListWindow.cs` | `41d9a177d1facacaa6294b94ec0f198e7830f77c8ea90b7a749b624efc247b5d` |
| 当前 `TranslationWindow.cs` | `0bfc35b29f49804b07d6c2ea14fa1f2503963042d058a24fd271baf64a33275a` |
| `TranslationCalibration.PluginChecks/UiChecks.cs` | `dbfa3734f7a36fc428a86f313ea769f52cd805e6b9b02f10a42ac23ca41873e7` |

## 边界与验证

这次闭合点表导入后 DataGrid 集合、原始文本显示和少于五列数字行的逐字段副作用顺序。异常发生在当前行 `DeltaAngle` 数组索引读取处；它不是整行原子拒绝。由 `PointList` setter 内部捕获的参数存储写入异常会停止对应属性 setter 更新，但原厂桌面点表异常提示/本地化消息框没有在独立 UI 中像素复刻。

- `dotnet run --project tests/TranslationCalibration.PluginChecks/TranslationCalibration.PluginChecks.csproj -c Release`：`TranslationCalibration.PluginChecks: 7463 checks passed`。
- 无设备测试；没有修改主框架工作树。
