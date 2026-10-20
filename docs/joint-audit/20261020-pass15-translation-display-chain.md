# 第十五轮：平移旋转标定失败后的 Group 与显示刷新

范围仅为标定十工具中 `TranslationCalibModu` 的结果输出、四点路径图形，以及一次失败→恢复的当前框架交接。此工具原端口没有图像输入，本轮“真实图”指实际 `GraphEditorDocument` owned Group 执行图；不伪造上游图像消费者，不重复既有 XML→ReadCalib→Coordinate 成功链。

## 原件成员

- 安装 `TranslationCalibModu.xml` SHA-256 `EBE8DD996875914ED971DC8C8488405FDDA94C15C0A30DBF1A4813C999C1A45E` 声明输出状态、点数组、中心、误差、进度。原 `TranslationCalibModuDisplay.xml` SHA-256 `F0944125563D2D01DC13FE2759E5F378F4A5121789653E1C25CB49067C6675FC`：`Image Point` path 的 CenterX/Y 映射 `OutImagePointX/Y`，Tips 同时读图像与物理四数组；`RotAxisPoint` 取旋转中心；Data Record 的进度读 `CalibCurIndex/CalibTotalNum`；Result List 声明四列 `ModuStatus/CalibStatus/TransWorldError/RotWorldError`。资源路径 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/TranslationCalibModu/`。
- 原 `CIMVSNPointCalibModu::Process` RVA `0x3a8c0` 对 `GetProcessInput` 和 `ProcessAlg` 的非零返回在调用 `ProcessOutput` 前离开；`ProcessOutput` RVA `0x3c3f0` 正常入口按状态、运行时、进度、点数组和结果成员发布。原取体为 `worktrees/tool-plugins/analysis/full-audit/translation-calibration/native/18003a8c0.c` SHA-256 `BD39FDC8FDAA99709F88FF7084E2E3DDF6190F288090CC2B1E14F6832B14F9F3`、`18003c3f0.c` SHA-256 `FCE52A352B3FE6013C736157208D175234483C790C69D807FCC742713BB20D47`；原字段次序/空 SDK getter 的已知界限见 `docs/plugins/TRANSLATION-PROCESS-OUTPUT-TRACE.md`。原主框架如何对失败后旧端口值做历史保留，此源码链未证明；不能把当前清空语义称原厂等价。

## 当前框架和受控执行

`src/VisionPlugins.Calibration/TranslationPlugin.cs` 的 `Runtime.ExecuteAsync` 在本轮成功时建立四点数组与结果，在失败时以空输出更新 `state.DisplayOutputs`；`LastDisplay` 用最新 State 经 `ToolDisplayService.Build` 和原资源映射构造记录、四列与路径图形。`GraphEditorDocument` 的 owned Group 对该子节点输出映射 `CalibStatus` 和 `OutImagePointX`；`SolutionExecutor` 在子节点失败时将 Group 标 `SubprogramFailed`，不将旧输出当本轮出口。主窗 `RefreshToolDisplay` 的清旧对象和按当前节点/执行 scope 重拉源码链已于第十四轮逐成员核过；本轮不重复用户可见桌面实验。

探针 `tests/CalibrationTranslationDisplayChain.Checks/` 用四个可解点 `(0,0)→(10,-5)、(10,0)→(30,-5)、(0,10)→(10,15)、(10,10)→(30,15)` 建 owned Group。手动模式首轮正常：Group `CalibStatus=1`、`ImageXs=[0,10,0,10]`；显示 `Image Point` 路径四点、四列结果、Record `CalibProgress:4/4`。随后同 scope **直接通过 `SetParameters` 只增加 `ClearRevision` 并切自动采点，故意保留原点参数值**，且不提供输入；受控当前代码给子节点 `E000031C`、Group `SubprogramFailed`、无本轮 Group 出口；显示帧无图形、四列空值、Record 为空且红色。另一个未执行节点 ID 的 `LastDisplay` 为 null，不会取到前节点 State。这是构造的程序化状态转换，**不是正常清空按钮动作**。

首轮探针在直接切回手动模式时揭示**仅限上述程序化不一致状态的缓存行为**，完整失败日志已保留：`ClearRevision` 使 `TranslationPlugin.Synchronize` 先 `ClearState`（包括 `Points` 归零），随后对未变的节点点参数只比较 `state.Applied` 旧值并跳过回填。切回手动后同参数四点仍全零，求解返回 `0x10103007`，Group/显示继续失败；重新写入每个点参数（探针把四组坐标均偏移 1）后，同一 scope 才恢复。不能由此称用户界面清空存在缺陷。

原厂 `NPointList.ButtonClear_Click` 先写 `IUserStringData["Clear"]="0"`，成功后逐行将五列点值写成 `"0.000"`；`TranslationCalibModu::SetParam` 对 `Clear` 清内部点缓存，后续逐字段写入同步参数。原托管体 `worktrees/tool-plugins/analysis/full-audit/calibration-dependencies/decompiled/TranslationCalibModuControl/TranslationCalibModuControl/NPointList.xaml.cs` SHA-256 `AF17CDAB8A3B4E57F94097F948DB7C9A4E1070BCBA328319683B0E98085B3835`，原 native `18003d740.c` SHA-256 `ED0A1466E3E5B9114E51604AF4AED172CE559B98B67C10801E6D245ED49B433A`。当前 `CalibrationToolConfigurationProvider.Clear()` 在**同一次 Apply** 中将 32 行五组点参数全置 0 并增加 `ClearRevision`，`TranslationWindow` 清空按钮也同步其编辑器本地点值；当前 provider 源 SHA-256 `2524267BA875C4882E4DC9D124E4F833D4EF0C0EE18710BDFD7065790C4749CE`、窗口源 SHA-256 `D74E18309439BB27323B91158EB4F03FBCA87BF24C5C60E62936AE28F4D3E3B9`。既有原/当前路径及 WPF 清空专项见 `docs/plugins/TRANSLATION-POINT-EDIT-TRACE.md`（SHA-256 `BF97A75E1C03B0AC26204DD11397743FEA60795DEA2E23017CBF1858DC24F90E`）。因此正常按钮不会构造本探针的“修订号变化而点配置仍旧值”前提；本轮不提出产品修复。

首次失败日志 `analysis/joint-audit/pass15-cal-translation-display/first-run.log` SHA-256 `5E54274CABF5A5A01A31E15E8D9B8B16B5BAF02FF0804C29B595EBDE843D2206`；保留原断言失败和 `0x10103007`。修改探针以显式断言该状态及重新编辑恢复后，第二次 `dotnet run --project tests/CalibrationTranslationDisplayChain.Checks/CalibrationTranslationDisplayChain.Checks.csproj -c Release` exit 0、10 PASS，日志 `analysis/joint-audit/pass15-cal-translation-display/second-run.log` SHA-256 `D7B50F4D6568A45C336CA68B094024E282041D3EAD3D89D3CA21AAE78D53EB1C`。运行时测试源 SHA-256 `4650FC06057DB4A76C024974A2A840776AADEB74A53617658FC758B2544AA9F7`；项目文件 SHA-256 `7D1D99B7A5D7E975CB7D94C6876D64F6AD7692639BA938247A3E9478E362099A`。事后仅补探针注释和断言文字以标明人工构造前提，执行步骤与日志不变；最终源 SHA-256 `6E69B507B6CAF882986CB554FCE2B05DDF4679C7461EF0DB0F36CE8E1B8EC2FC`。

## 可关闭边与残余

本轮可证当前正常四点结果→Group/显示，以及**构造程序化不一致状态**后的失败清旧本轮输出、重新编辑点值恢复。既有源码与 UI 专测支持正常按钮同时清参数，不把本轮失败当用户可达缺陷或待修复产品问题。原失败后旧值是否在 VM 主窗保留、原 native 部分输出写入失败后的宿主状态、实际用户桌面切选刷新仍需另证。未改产品、共享文件或 tool-plugins，未提交。
