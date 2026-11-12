# W01 工作区基线契约（源码冻结版）

日期：2026-11-11
范围：左工具箱 → 中流程画布 → 执行 → 右图像/模块结果 → 下当前结果/历史结果/帮助 → 底部日志区
证据口径：**全部来自原厂反编译源码与随附资源字典**；运行态推断一律标「运行未确认」，不以截图为契约依据。

## 0. 证据目录

| 缩写 | 路径（相对 `d:/MyWork/Rea逆向/`） |
|---|---|
| MVR | `analysis/decompiled/VMControls.WPF/VMControls.WPF/MainViewControl.xaml` / `.xaml.cs` |
| PC | `analysis/decompiled/VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml` / `.xaml.cs` |
| MRV | `analysis/decompiled/VMControls.WPF/VMControls.WPF/ModuleResultView.xaml` / `.xaml.cs` |
| TB | `analysis/decompiled/VMControls.WPF/VMControls.WPF/ToolBox.xaml` / `.xaml.cs` |
| LV | `analysis/decompiled/VMControls.WPF/VMControls.WPF/LogView.xaml` / `.xaml.cs` |
| RB | `analysis/decompiled/VMControls.WPF/VMControls.WPF/RunButtonControl.xaml` |
| REL | `analysis/decompiled/VMControls.WPF.Release/VmMainViewConfigControl.xaml` / `.xaml.cs` |
| COLOR | `analysis/decompiled/Apps.ColorFun/colorstyle1.xaml` |
| LANG | `analysis/decompiled/VisionMaster.Exe.Audit/resources/zh-cn.xaml` |
| BAML | `analysis/control-appearance-baml-fullinstall-20261027/decoded/`（1757 条解码 BAML，含 103 条 VMControls.WPF 系列） |

另有全安装 BAML 解码报告 `analysis/control-appearance-baml-fullinstall-20261027/decode-report.json`，其中 `VMControls.WPF.Release.g.resources` 的 `vmmainviewconfigcontrol.baml` 来自 `Applications/myLibs/VMControls.WPF.Release.dll`（assembly SHA-256 `c016bd0a…5b9b6`）。

## 1. 窗口与四块区域几何

```
MainWindow (1366×768, MinWidth 800 / MinHeight 600, FontSize 13, Microsoft YaHei)
└─ VmMainViewConfigControl (REL) —— 仅转发，不布局
   └─ MainViewControl (MVR) —— 真正的工作区
      └─ Grid PART_View [Auto | *]
         └─ Grid workarea [ * | Auto | 1.0000001*(MinWidth=400) ]
            ├─ col0  workareaConfig → ProcedureConfigControl   （工具箱 + 画布 + 流程页签）
            ├─ col1  workareaSplitter  Width=5  BorderThickness=1,0  BorderBrush=#FF000000  Opacity=0.5
            └─ col2  ModuleViewHost    （ModuleResultView + LogView + 日志栏）
```

来源：MVR:18-60；REL:2-3。

**左右比例 1:1，右列最小 400；分隔条 5px。** 这与当前实现把「历史结果」放在整窗底部跨全宽的做法不同，属必须纠正的结构偏差。

`VmMainViewConfigControl` 转发 5 个属性到 `MainViewControl`：`IsLockWorkArea`、`IsOpenParams`(TwoWay)、`IsShowProgressMask`(TwoWay)、`ZoomIndex`(TwoWay)、`IsParamsEditState`(OneWay)。来源：REL:3。

## 2. 左侧：ProcedureConfigControl（工具箱 + 页签 + 画布）

### 2.1 分区

```
Grid PART_View  Background=Col_ProcessBar_Background(#3B3E40)
├─ col0 Auto : ToolBox x:Name="toolBox"   IsGlobleMode={Binding BoolOfDisplayType}
│             IsLockWorkArea=True → IsEnabled=False（PC:571-573）
│             DropShadowEffect BlurRadius=5 Direction=360 Opacity=0.30 ShadowDepth=2 Color=#FF0F1010
└─ col1 *    : Grid paramControl → ContentPresenter cp
               ContentTemplate 由 3 个 Trigger 切换（PC:582-595）：
                 BoolOfDisplayType=True                     → allProcess（全局视图）
                 DecoratedProcedures.Count=0                → noProcess（空流程）
                 Count=0 && IsForSingleModulesetMode=True   → noSource
                 默认                                       → tabProcess
```

### 2.2 工具箱双态（源码确认的数值）

| 状态 | 常量 | 来源 |
|---|---|---|
| 折叠分组块 | `Border 51×48`，图标 `52×50` | TB（BAML `toolbox.baml`） |
| 折叠态悬停 | `Popup Placement=Right`，内容 `Grid Width=235`，`MaxHeight={Binding ActualHeight, ToolBox}` | TB |
| 展开态 | `GroupItem Width=230`，`ItemsPanel Width=250` | TB |
| 工具磁贴 | `ToolItemGrid 68×90` | TB |
| 展开开关 | `ToggleButton switchButton Height=22`，`IsChecked={Binding UnFoldSwitch}`；`IsGlobleMode=True` 时 `Collapsed` | TB |
| 默认值 | `UnFoldSwitch` 默认 `false`（`SetCurrentValue(..., false)`） | `ToolBox.xaml.cs` |

### 2.3 流程页签与画布

- `tabControl`：`ItemsSource={Binding DecoratedProcedures}`，`SelectedItem={Binding DecoratorSelectedTabItem}`，`SelectionChanged=m_ProcessTabControl_SelectionChanged`，`Loaded=TabControl_Loaded`（PC:523）。
- 页签标题是 `HeadReNameTextBox`：`MaxLength=64`、`MaxWidth=240`、`FontSize=14`、支持重命名（PC:529）。
- 页签内容模板是 **`DiagramArea`**，关键绑定（PC:538）：
  - `SelectedItem ↔ DecoratorSelectedItem`（TwoWay）
  - `ItemsSource = DisplayModuleSet.DecorateSource.(IChildren.Children)`
  - `Procedure = DisplayModuleSet`（流程级）
  - `ZoomIndex`（TwoWay）、`SearchModuleInfo = SelectedModule`、`SubSource = {Binding}`
  - `IsEnabled = IsActive`、`IsLockWorkArea`、`IsOpenParams`
- `DiagramArea` 内部 = `ZoomArea` + `DiagramPage(5000)` + 底部 28 高 `StatusBar`（BAML `diagramarea.baml`）。

### 2.4 运行按钮在页签上，不在全局工具栏

`RunButtonControl`（RB:11-24）：

| 按钮 | 样式 | 提示 | 四态图标 | 命令 |
|---|---|---|---|---|
| 单次 | `SuperImageButtonRunOnceStyle` | `Lang_SigRun` | `ic_play_多流程_{normal,hover,click,disable}.png` | `RunOnceCommand` |
| 连续 `Name=cs` | `ContentPresenterContinousRunStyle`，`Margin=4,0,0,0` | `Lang_MutiRun` | `ic_多次运行_多流程_*.png` | `RunContinuousCommand` |

`DataTrigger Binding={Binding DecorateSource.ContinuousRunEnable} Value=True` → 第二个按钮的 ToolTip 改 `Lang_StopRun`，四态图整体换成 `ic_停止运行_多流程_*.png`（RB:16-23）。

**即「停止」不是第三个按钮，是连续按钮的双态切换**；且运行按钮由页签选中态注入，`DecorateSource.RunEnable=False` 时隐藏（PC 页签模板）。当前实现把运行/连续/停止做成三个全局工具栏按钮，属偏差。

### 2.5 选中态转发（节点切换的总开关）

`ProcedureConfigControl.xaml.cs:886-935` `OnDecoratorSelectedItemChanged`：

1. 把 `IDecorate` 解包成 `DecorateSource`；
2. 若解包结果是 `IVmProcedure` → `DecoratorSelectedTabItem = decorate2`（PC:899-902）；
3. `LoadingFlag == false` 时 `SetCurrentValue(SelectedItemProperty, obj)`（PC:904-907）；
4. 二选一互斥：流程 → `SelectedModule=null` + `SelectedPro=obj`；模块 → `SelectedModule=obj` + `SelectedPro=null`（PC:908-917）；
5. 旧值变 null 时清空 `SelectedItem / SelectedModule / SelectedPro`（PC:919-935）；
6. `IsOpenParams` 由 True→False 时把 `SelectedItem` 重新设为 `decorateSource`（PC:966-971）。

`MainViewControl.xaml:33` 把 `SelectedItem` 绑到 `ModuleResultView.ModuleSource`。**因此「切换节点」= SelectedItem 变化 → 右侧整块换绑源**，这是全链路主干。

## 3. 右侧：ModuleResultView

### 3.1 上区 ResultTab（图像 / 模块结果）

- `TabControl Name=ResultTab`，`Height=38`、`FontSize=14`、`SelectionChanged=ResultTab_SelectionChanged`（MRV:40-41）。
- 两页：图像（`Result_ImageData` / 流程图像 `Process_Image`）与模块结果（`Result_ModuleResult`，树形 `treeTabItem`）。
- 工具栏：`ImageListComboBox Height=32 Width=142`、`DoubleImageToggleButton 35×35`、1:1 / 全屏 `34×34`、翻页 `btnLeft/btnRight 26×54`（右钮 `ScaleX=-1`）。
- 底部信息行：`1366*768 | R:… G:… B:… | X:… Y:…` 坐标与像素值。

### 3.2 下区 InfoTab —— 五页，非常驻三页

`TabControl Name=InfoTab`，`SelectionChanged=InfoTab_SelectionChanged`（MRV:285），页签高 40 / `FontSize=14`：

| # | Name | 文案键 / 中文 | 默认可见 | 承载 |
|---|---|---|---|---|
| 1 | `CommonParamTabItem` | `Tab_Common Params` 常用参数 | **Collapsed** | `Grid ParamsView` |
| 2 | `DataqueueTabItem` | `Lang_DataQueue` 数据队列 | **Collapsed** | `Grid QueueView` |
| 3 | `CurResultTabItem` | `Lang_CurRslt` **当前结果** | Visible | `DataGrid m_ResultDataGrid` + `HikHistogram m_HikHistogram`（默认 Collapsed） |
| 4 | `hstTab` | `Lang_HstRslt` **历史结果** | Visible | `DataGrid m_DataGrid` |
| 5 | `helpTab` | `Lang_Help` **帮助**，`Width=90` | Visible | `ScrollViewer → TextBlock moduleHelp` |

右上角 `ToggleButton` 绑定 `IsInfoTabExpand` 控制下区折叠（MRV:336）。

`SetCommonModuleView`（MRV.xaml.cs:1078-1095）决定「当前结果」是否显示：

```
flag = ModuleParams.GetDisplayList().Any(x => x is Apps.XmlParser.Object && x.Type == "datalist")
CurResultTabItem.Visibility = flag ? Visible : Collapsed
hstTab.Visibility = Visible
m_ResultDataGrid.Visibility = Visible
InitSpecialParams()
InfoTab.SelectedIndex = curTabIndex（若该页可见），否则 SetFirstVisibleTab()
```

**页签是按当前工具的输出能力动态显隐的**，不是固定五页或固定三页。

### 3.3 当前结果：列由工具的 `Colume` 特征动态生成

这是「选择不同节点，结果表格不一样」的源码根因（MRV.xaml.cs）：

1. `RefreshDataListResult` 从显示列表取 `Type=="datalist"` 的对象，读 `Features.First(p => p.Name == "Colume").Value.Split(',')` 得到列名数组（:1494-1495）。
2. 无 `Colume` → `ClearCurrentResultList()` + `m_ResultDataGrid.Columns.Clear()`（:1496-1501）。
3. `NeedRebuildColume`：列数 ≠ `columeArr.Length+1`，或按 `AppLocalizationService.GetLang(key)` 找不到表头 → 重建（:1560-1574）。
4. `RebuildDataGridColume`（:1576-1593）：
   - 首列固定 `Header = GetLang("Number")`、`Binding("序号")`
   - 其余每列 `Header = GetLang(text)`、`Binding = new Binding(text)`
5. `RefreshCurrentResultList(curDatas, array, array2)` 填行值；按 `columMaxLens` 做 `SizeToHeader → Auto` 列宽自适应（:1519-1528）。
6. 选中行 `m_ResultDataGrid_SelectionChanged` → `selectIndex` + `ModuleParams.GetDisplayList()`（:2022-）。

`m_ResultDataGrid.ItemsSource = {Binding ModuleCurrentResultList, IsAsync=True}`（MRV:299）。

### 3.4 历史结果：固定三列

`m_DataGrid.ItemsSource = m_CurrentInfo.ModuleDisplayInfoList`（MRV:304），列固定：

| 列 | 绑定 | 最小宽度 | 文案 |
|---|---|---|---|
| 执行序号 | `RowIndex` | 30 | `Lang_ExecuteNum` |
| 时间 | `AddTime` | 150 | `Lang_Time` = 时间 |
| 模块数据 | `Content` | 200 | `Lang_ModuData` = 模块数据 |

右键菜单两项：`Menu_Clear`（清除）/ `Menu_Save`（保存）（MRV:323-328）。

### 3.5 帮助页

`TextBlock moduleHelp`，`Text={Lang:ResourceBinding m_CurrentInfo.HelperContent}`（MRV:332）——**帮助内容来自工具自身**，不是全局静态页。

### 3.6 换绑源时的完整清理链

`ModuleSourceChanged`（MRV.xaml.cs:416-437）→ `DoSourceChanged`（:439-469）：

```
oldValue != null → isDecoratorRebuilding=true; m_CurrentInfo?.Dispose(); m_CurrentInfo=null
ClearDataStore()  // curHsiListShowCount/curCharBoxShowCount/isRendering/datalist 全清
newValue != null → RefreshExecuteCount(newValue)
                   m_CurrentInfo = new ModuleDisplayDecorator { DecorateSource=newValue, ParentUI=this }
                   m_CurrentInfo.Init()
                   resultGrid.DataContext = localImageGrid.DataContext = m_CurrentInfo
                   ImageAlgorithmTabRoot = m_CurrentInfo.ImageAlgorithmTabRoot
newValue == null → m_CurrentInfo = null
UpdateModuleView()  // AddCrossLine；IsProcedure → IsShowDoubleImage=false
```

`ClearDataGird()`（:1301-1308）另做：清当前结果列表、清空列、`columMaxLens=null`、清直方图类型/数据点/DataPoints。

**这条是契约里最容易被漏掉的：换节点必须 Dispose 旧装饰器、清缓存、清列、清直方图，否则上一个工具的数据会残留。**

## 4. 底部：日志区（不是「当前结果/历史结果/帮助」）

`ModuleViewHost` 内（MVR:34-57）：

- `LogView m_LogView`：`Margin=4,0,4,30`、`VerticalAlignment=Bottom`、`Height=180`、`Background=#FF3B3B3B`、`Visibility={Binding IsLogFold, Converter=antiBoolToVisibleConverter}`、`LogSource={Binding LogData}`。
- `LogView.xaml`：`DataGrid Name=logshow`，`RowHeight=20`、`FontSize=12`、`IsReadOnly=True`、`GridLinesVisibility=None`、**列头 `Visibility=Collapsed`**；列绑定 `LogType`（图标转换器 `LogManageTypeImageConvert`）、`LogInfoTime`（`TimeConvertor`）、`LogEvent`。
- 底部 28 高日志栏 `Background=Col_LogBar_Background(#3B3C3F)`：
  - 展开态可见四个 RadioButton：`logAllBtn / logAwardBtn / logErrorBtn / logTipsBtn`，`CommandParameter` 分别 `1 / 2 / 3 / 4`，命令 `LogTypeCommand`。
  - 折叠态可见 `logContent`：20×20 类型图标 + 62 宽时间 + `TextTrimming=CharacterEllipsis` 的内容。
  - 右侧 `logShow`：`VersionInfo` + 20×20 `ToggleButton LogBtn`，`Command=LogCommand`，`CommandParameter={Binding IsChecked, ElementName=LogBtn}`。

code-behind（MVR.xaml.cs）：

- `_isLogFold = true` → **默认折叠**（:83）。
- `LogCommandExecute(bool flag)` → `IsLogFold = !flag`；若 `logType != ""` 清空并 `OnPropertyChanged("LogData")`（:458-469）。
- `LogTypeCommandExecute(string)` → `1:""`（全部）、`2:"警告"`、`3:"错误"`、`4:"信息"`，最后 `OnPropertyChanged("LogData")`（:471-489）。
- `LogInfoShow_CollectionChanged` 在 `NotifyCollectionChangedAction.Add` 时更新 `CurrentLogContent/Time/Type`，再 `OnPropertyChanged("LogData")`（:405-412）。
- `IsShowProgressMask` 变化时 `AddMask()/RemoveMask()`：`ProgressMask` 绑定 `MaskMessage`（:347-367），与启动链的 `MainWindowData.IsShowProgressMask` 是同一机制。

## 5. 色值与文案（有确切原厂值）

| 键 | 值 | 来源 |
|---|---|---|
| `Col_DisplayImage_Background` | `#3B3E41` | COLOR |
| `Col_ProcessBar_Background` | `#3B3E40` | COLOR |
| `Col_LogBar_Background` | `#3B3C3F` | COLOR |
| `Col_LogBar_Foreground` | `#BEBEBE` | COLOR |
| `Col_LogBar_Popup_Background` | `#3B3B3B` | COLOR |
| `Lang_CurRslt` / `Lang_HstRslt` / `Lang_Help` | 当前结果 / 历史结果 / 帮助 | LANG |
| `Lang_DataQueue` / `Lang_Time` / `Lang_ModuData` | 数据队列 / 时间 / 模块数据 | LANG |

## 6. 工具接入基线（给其他工具开发的六项契约）

一个工具要能在这条完整链路上工作，必须提供下列六组能力；任何一组缺失，校验器应直接报错而不是静默降级。

| # | 能力 | 契约要求 | 源码依据 |
|---|---|---|---|
| 1 | **发现与元数据** | 注册唯一 `ModuleId`、分类、优先级、显示名、图标；参与 `DecoratedProcedures` / 工具箱分组 | PC:523,564；TB |
| 2 | **节点外观** | 提供节点图标/名称/输入输出端口，服从 `DiagramArea` 的 `SelectedItem` 选中与连线规则 | PC:538 |
| 3 | **参数与显示列表** | `ModuleParams.GetDisplayList()`；若含 `Type="datalist"` 的对象，其 `Features` 必须有 `Name="Colume"` 且值以逗号分隔列名 | MRV.xaml.cs:1083,1494-1495 |
| 4 | **执行** | 支持 `RunOnceCommand` / `RunContinuousCommand` / `ContinuousRunEnable` 双态；维护 `ExecuteCount`、耗时与错误状态 | RB:13-23；MRV.xaml.cs:451,664 |
| 5 | **结果投影** | 当前结果行数据（`ModuleCurrentResultList`，列名与 `Colume` 对齐）；历史结果三字段 `RowIndex/AddTime/Content`；图像输出（供图像列表与预览）；帮助文本 `HelperContent` | MRV:299,304-320,332 |
| 6 | **生命周期** | 换绑源时可 `Dispose`；清空缓存、列、直方图与事件订阅，不残留旧工具数据 | MRV.xaml.cs:439-469,1301-1308 |

**基线校验器判定口径**：以 Blob 为参考实现跑通 1→6；再用第二个工具（非 Blob）按同六项接入一次，若能在不修改宿主代码的情况下完成「工具箱出现 → 拖入画布 → 执行 → 右侧结果与列正确 → 换节点无残留」，基线成立。

## 7. 证据等级与未确认项

| 类别 | 状态 |
|---|---|
| 窗口/四块控件树、几何常量、绑定路径、事件名 | 源码确认（XAML + code-behind） |
| 工具箱双态数值、运行按钮双态、日志折叠与筛选、结果列动态生成、换源清理链 | 源码确认 |
| 主题色、页签文案 | 源码确认（COLOR / LANG） |
| 图像资源二进制（节点图、工具磁贴图、运行四态图、日志图标） | 部分可取（115 个工作区相关 PNG 在 `analysis/ui-assets/`），缺失项标独立实现 |
| SDK 回调时序、真实相机/设备输出、原生引擎结果 | **运行未确认**（对应台账 `ORIGINAL-INSTANCE` / `DEVICE-RELEASE`） |
| 画布手势细节、连线渲染、缩略图导航 | 需读 `diagramarea.baml` / `connection.baml` / `zoomarea.baml` 的 code-behind 后再补（本契约未冻结） |

## 8. 与当前实现的主要偏差（供实现阶段逐项处置）

1. 运行按钮在页签上，且「停止」是连续按钮的双态；当前实现是全局工具栏三个按钮。
2. 「当前结果/历史结果/帮助」在右侧下半区 `InfoTab`（共 5 页、2 页默认折叠），当前实现在整窗底部 `BottomTabs`（2 页）。
3. 窗口底部应为日志区（180 + 28），当前实现把结果表格放在那里。
4. `m_ResultDataGrid` 列必须由 `Colume` 动态生成；当前实现 `HistoryGrid` 是固定三列。
5. 换节点必须走 `Dispose + ClearDataStore + 清列 + 清直方图`；当前实现未见等价清理链。
6. 工具箱双态数值、悬停 `Popup Width=235`、展开开关 `Height=22` 未按源码对齐。

## 9. 实现进度（2026-11-11）

| 区域 | 状态 | 证据 |
|---|---|---|
| 右侧当前结果页 | 已落地 | `src/VisionEditor.Core/NodeResultProjection.cs`、`MainWindow.WorkspaceProjection.cs`；列由工具输出能力生成（连通域 6 列 / 圆 5 列 / 点 3 列 / 标量 2 列），无表格输出自动收起，`NodeProjectionSession` 按列签名增量重建 |
| 底部日志区 | 已落地 | `ShellLog.cs`、`MainWindow.ShellLog.cs`；180 高面板 + 28 高筛选栏，筛选码 1/2/3/4，默认折叠，上限 500 条，筛选不丢记录 |
| 左侧工具箱 | 已核对 | 折叠 52×50、展开分组 230、磁贴 68×90、悬停弹层 235、磁贴面板 250 均与 §2.2 一致；折叠态缺少 51×48 背衬边框，属纯装饰差异，记为独立近似 |
| 断言 | 通过 | `tests/NodeProjectionChecks` 40 项、`tests/VisionEditor.WorkspaceProjectionChecks` 25 项 |

未落地：上区「模块结果」树形页、常用参数/数据队列两页、换源 Dispose 清理链（当前由投影层换列代替）、画布手势、执行按钮上页签。
### 9.1 画布与执行（2026-11-12 补充）

| 项 | 原厂 | 现状 |
|---|---|---|
| 画布结构 | `DiagramArea = ZoomArea + 28 高 StatusBar`（DiagramArea.xaml:52-65） | 一致 |
| 节点本体 | `ItemWidth=120 / ItemHeight=40`（DiagramConstInfo） | 已由 110×30 改为 **120×40** |
| 节点外框 | `ItemRectWidth=130 / ItemRectHeight=50` | 卡片 130 宽、本体偏移 10,10，等价 |
| 栅格步进 | `GridSize=5` | `Snap(v)=Round(v/5)*5`，一致 |
| 节点热区 | `66×22`（DiagramItem.xaml.cs:846-847） | 一致 |
| 运行控件位置 | 页签模板 `RunButtons`，仅选中页签显示（PC:390-420） | 一致：单次运行 + 连续/停止双态，未选中页签不显示 |
| 页签红灯 | `Ellipse 8×8 #FFEC0000`，`WorkState=Ready` 时隐藏（PC:387/413-415） | 一致：`ready` 时 Collapsed |
| 工具箱偏移 | `leftOffset = isFold ? 52 : 230`（DiagramItem.xaml.cs:616） | 一致 |

结论：**执行链无结构性偏差**；此前判断的「运行按钮只在全局工具栏」不成立——页签内联控件一直存在，且全局工具栏的 SigRun/Run 在原厂 `menuitems.xml` 中同样定义（group3），两者并存符合原厂。
### 9.2 工具接入基线（2026-11-12）

`src/VisionEditor.Core/ToolBaseline.cs` 把 §6 六项能力落成可执行校验：

| Code | 校验内容 |
|---|---|
| `discovery` | 已注册、DisplayName 非空、Id 与注册键一致 |
| `node-appearance` | 输出端口不重名（零输入/零输出的控制类工具属合法形态） |
| `params-display-list` | 表格型输出能被投影层识别 |
| `execution` | 输出端口声明存在（由投影列校验间接覆盖） |
| `result-projection` | 空记录下仍能生成列；表格型输出启用当前结果页；图像输出进入图像列表 |
| `lifecycle` | 首次投影重建列、`Clear` 后复位 |

验收：`ToolBaseline.Validate` 对 **Blob（vision.components）** 与 **第二个工具（vision.circle）** 均为零违规；`ValidateAll` 扫描注册表全部工具同样零违规；伪造的空壳工具被 `discovery` 拦下。

回归汇总（2026-11-12）：构建 0 警告 0 错误；`NodeProjectionChecks` 40 项、`VisionEditor.WorkspaceProjectionChecks` 44 项、`VisionEditor.ShellMenuChecks` 65 项、`StartupShellChecks`/`CanvasImplementationChecks`/`ContainerZoomChecks` 退出码 0；`VisionEditor.WpfChecks` 为既有失败（与本次改动无关，已用回退节点尺寸对照验证）。
### 9.3 执行链路打通（2026-11-12）

| 项 | 实现 | 依据 |
|---|---|---|
| 模块级执行账本 | `ModuleRunLedger`：每节点每次执行一条 `ModuleRunEntry{执行序号,时间,状态,耗时,输出端口}`，`DataText` 输出 `[模块名(端口 值; …)]` | 原厂 hstTab 绑 `ModuleDisplayInfoList` |
| 历史结果页 | 改绑模块级记录，切节点即换历史；未执行节点为空 | 同上 |
| 图像源下拉 | 选中节点具备供图能力时只列自身输出，否则回落全流程供图节点 | `ImageListComboBox` 语义 |
| 模块结果页 | 树形：节点 → 输出端口 · 类型 → 值 | 原厂树形结果 |
| 下区五页 | 常用参数/数据队列默认折叠；当前结果按 `HasTable` 显隐并回落历史 | `InfoTab` 五页 |
| 帮助页 | 由工具描述符真实生成端口/参数说明，随节点切换 | `moduleHelp` 绑 `HelperContent`，本项目无该文本源，改为描述符生成 |
| 节点状态角标 | 左侧 26 宽角标，未执行灰 / 运行中蓝 / 成功绿 / NG 橙 / 失败红 | 原厂 `StateFlag Width=26`；色值为独立近似 |
| 节点本体 | 110×30（原厂模板），外框 130，栅格吸附 5 | `diagramitemtemplate.xaml:1126`、`DiagramConstInfo` |

回归：构建 0 错误；`NodeProjectionChecks` 54 项、`VisionEditor.WorkspaceProjectionChecks` 77 项、`VisionEditor.ShellMenuChecks` 65 项、`StartupShellChecks`/`CanvasImplementationChecks`/`ContainerZoomChecks` 退出码 0。