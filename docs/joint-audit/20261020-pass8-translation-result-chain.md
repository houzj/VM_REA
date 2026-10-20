# 第八轮：平移旋转标定文件矩阵进入标定加载与坐标系

本切片只审核“标定”分类的平移旋转标定，以及它把结果交给标定加载、owned Group、坐标系所必需的主框架边。复用第六轮已核的标定加载/坐标系原成员和当前 Group 合同；不重做十工具共同目录与配置审查。

## 原版生产者源码

安装原件 `TranslationCalibModu.dll` SHA-256 `A85A7CA08EA265B2CF881B4FF01BF7538A60E9EDB7466478476584B08FE25183`，原生类名虽为 `CIMVSNPointCalibModu`，此处按工具目录/模块身份区分于 N 点。`Process` RVA `0x3a8c0` 的输入/算法失败在输出调用前返回；`ProcessAlg` RVA `0x3b0e0` 在求解成功、刷新开关与非空路径满足时调用 `WriteXmlData` RVA `0x3e660`。原始反编译体依次位于 `worktrees/tool-plugins/analysis/full-audit/translation-calibration/native/18003a8c0.c` SHA-256 `BD39FDC8FDAA99709F88FF7084E2E3DDF6190F288090CC2B1E14F6832B14F9F3`、`18003b0e0.c` SHA-256 `5110E0BC7C211EB2EB3789C788B69AF1A0F3CB24B2B10E82632739CBEC804087`、`18003e660.c` SHA-256 `B3C974C6480FD5E9ADF3C113FDA20E2492E38742C62F3EAA26D650BBFDD12F37`。专属文件多了 `CameraMode`、示教/姿态字段，并写 `CalibOutputParam/CalibMatrix` 九项；源级详细点位/字段见 `docs/plugins/TRANSLATION-CALIBRATION.md`。这与 N 点不是一个可互换的工具状态机。

## 当前实际消费者链

探针 `tests/CalibrationTranslationChain.Checks/Program.cs` SHA-256 `CFD8AD78B90C960E41964C3EC7896D8C654E0F6D6249CDAFB30B9AF8AA1F19D9`，工程 SHA-256 `34BB43761C26C5791F413357F88B89A8C654EF850043CE3A18CD1D488AF552A4`，以受控点表 `state-rotation-three`、`CalibPointGet=1` 构造真实图：平移旋转标定成功写 XML；其 `CalibStatus` 触发标定加载刷新；加载器发布九项 `CalibMatrix:NumberArray`；owned Group 输出 `Matrix`；坐标系 `InputWay=2` 从 Group 读该矩阵，图像源仅交一个必需的 2×2 GrayImage。当前插件内部独立 `CalibData:Calibration` 直连坐标系 `CalibMatrix:NumberArray` 被拒，不能把便利对象冒充原版矩阵端口。

命令 `dotnet run --project tests/CalibrationTranslationChain.Checks/CalibrationTranslationChain.Checks.csproj -c Release` 退出 0。初次与方案保存重开后，生产者、标定加载、Group、坐标系均 Success；文件/Group/坐标系均为九项矩阵，生产者与文件按 `G8` float 文本精度比较，文件与 Group 精确一致。原始日志 `analysis/joint-audit/pass8-cal-translation/checks.log` SHA-256 `909B59085C9BE6791989FA71ED18B493203281010F7C3920E5A5AFD22F7D1783`；文件与方案产物在同一忽略目录，重复运行的生成时间可能改变实例哈希。

这是第二个**不同原版生产者**到同一标定消费者链的当前正常路径证据，不等于原版同构整图已运行。组合标定、示教、旋转累计、失败文件/路径、原生数值逐位差分和 UI 交互未由本图覆盖；它们继续按平移旋转工具特有证据审，不因共享 Group 成功而关闭。本轮没有新增主框架产品修复，未修改 `tool-plugins`。
