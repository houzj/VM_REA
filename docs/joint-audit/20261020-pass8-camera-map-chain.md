# 第八轮：相机映射→映射标定文件模式→标定加载的联合链

当前目标仅含“标定”十工具。此切片对相机映射和映射标定分别核原入口/端口与文件格式，再在当前真实图把**有依据的文件载荷**交给标定加载、Group 和坐标系。图像源仅为坐标系必需夹具；共用 GraphExecutor/Group 原理引用前轮，不重新计十次。原件只读，`tool-plugins` 工作树未修改。

## 两工具不是同一种输入/输出

原安装 `IMVSCameraMapModu.dll` SHA-256 `9455BC133B200E7956EBD38969998E5DC76151A99D8139EEF7A2E6B817383E8E`，主 XML SHA-256 `8488358D302D4B48C6C08207380DB8D0BD2BDC7566ED8F00ECE8FDD299B7FD61`。主 XML 的输入是 `Target/Object` 点对（前两组 POINT，后六组按活动标记启用），**没有 IMAGE 或 ROI 输入口**；输出含 `CalibStatus:int` 和比例/平移/旋转等标量，**没有 `CalibMatrix:float[]` 输出端口**。UI 资源 `IMVSCameraMapModuAlgorithmTab.xml` SHA-256 `9832E201BC27E31413BD54BD92D55464B35C0657F681F7CA0059A695E0A77DFB`。原 `GetProcessInput` RVA `0xa710` 按活动组两遍读目标/对象点叶（C 体 SHA-256 `92517CF0643E21FC9166E8552470F6C44637CA85D838EC18FE34BD84084AED8F`），`ProcessAlg` RVA `0xbd20` 创建 CameraMap SDK 工具、装载点、运行并在成功与开关允许时调用 `WriteCalibFile`（体 SHA-256 `C38D617D9C7C2AB775D1B76C29091A6D7F0BDD807DD1E2DEA6568AB13F345FE9`）。写函数 RVA `0xd450` 以 `wb` 写 0x24=36 字节九项 float，而非 XML；体 SHA-256 `864CC55A130A48B25943E65D399414E68DCA6F5BAFE8CF9F21210AD06D03E35D`。求解 SDK 0x66/op0xb 的浮点拟合细节已有 `docs/plugins/CAMERA-MAPPING-B1-SOLVE-SOURCE.md`，本轮未重跑全域算法差分。

原安装 `IMVSMapCalibModu.dll` SHA-256 `658D63E2C4E2378E7EB1776D27B966990C0D0E01E29CA57ED82FE9ACC740FDE8`，主 XML SHA-256 `228BF45A24691C0CD13C2201E516941F20E6B1E5352478CCB60ED01C774B4F5F`：模式 1 有 `InputImage1/2` IMAGE 与 `InputROI1/2` ROIBOX、教学相关叶；模式 2 由 `CalibPath0/1` 两文件参数读取矩阵，输出 XML 由 `CalibPathName/RefreshFileEnable` 控制。两模式的正常结果端口含状态、数量、比例和角点/ROI，但**没有可直接连坐标系的 `CalibMatrix` 端口**。UI 资源 `IMVSMapCalibModuAlgorithmTab.xml` SHA-256 `CC91E61790265B3DAD51EC37BB87CF65EDD6B1FAD718630253B390DDC0A46BF2`。

映射标定原 `ReadFileMatrix` RVA `0x227d0` 对非 `.xml` 子串路径进入原始二进制读取，两个 side 存到独立矩阵缓存（C 体 SHA-256 `4ABF886B99912352F541CC5A2DF537CBA0C2164B2447178630864F388AC6CD94`）；模块 `ProcessAlg` RVA `0x21c50` 的文件分支按 source、target 两指针调用 SDK +0x10/+0x20，成功时满足开关和路径条件调用 `WriteXmlData`（体 SHA-256 `643F1892E704C1039C234485163CB653E61D01CC7F67F8B88C9C3A7DEFAE3972`）。核心实际矩阵关系是 `inverse(target) * source`，来自 SDK `0x7d4f20` 及调用 ABI（原体 SHA-256 `5528EBCAA6A9E87284AD6EDC355F1D8EDA5F7E79125B7D4DD85F3149A339331D`，详见 `docs/plugins/MAP-CALIBRATION-D4-FILE-DIRECTION-SOURCE.md`）；文件模式 `ProcessOutput` RVA `0x220b0` 发布状态，不把内部九项结果声明为直接矩阵端口（体 SHA-256 `2AEC0A29A129353229B67668E614A28054265D7727631682B5E917C587A65549`）。原导出 XML 有 `CalibOutputParam/CalibFloatListParam` 九值；`docs/plugins/MAP-CALIBRATION.md` 与原受控导出 `worktrees/tool-plugins/analysis/map-calibration/native/module-export.xml` SHA-256 `4F7DCE3CC67B2B2ED46D33A5D1F08B10C0437A9DFCB07B7C814C6AC499BED58E` 记录此结构。图像/ROI 特有入口另见 `Process` RVA `0x21690` SHA-256 `31F6AA7C8E51A1AA97C55C2320F1DF010C6B777BA00E830EEB12752FCCA80D11`、`GetInputImage` RVA `0x203d0` SHA-256 `5E9F0933E3B806D32E7EBE6F067F05013573F965F4563D9FAC40176581697402`、`RoiFix` RVA `0x22f40` SHA-256 `CE52268DB8B112D4F84FF3A9EB5974C1C0784EC0BC3602E8B29111F8E22F69AC`；本切片**未将图像/ROI 模式声称为已完成联合图测试**。

因此合法组合是相机映射生成两份 36 字节文件 → 映射标定文件模式消费 source/target → 映射标定写 XML → 标定加载解析 `CalibMatrix:float[]` → Group → 坐标系。原单模块源码与文件格式分别支持这些交接，但**原版 VisionEditor 未运行本同构整图**；组合运行结论仅来自当前实现探针。

## 当前真实图与保存重开

探针 `tests/CalibrationMapProducer.Checks/Program.cs` SHA-256 `4627CF233D8578B46043AC308625CC4DC189F6A8E0202408A46E271DA6F1A53D`；工程 SHA-256 `57A40B8014C9A101E1BF4A2D8DCA3B8EFADA772A7213270307A62F3DC8FE8489`。命令 `dotnet run --project tests/CalibrationMapProducer.Checks/CalibrationMapProducer.Checks.csproj -c Release` 退出 0，原始 stdout `analysis/joint-audit/pass8-cal-map/checks.log` SHA-256 `91A75D6C263B0DAF08D9463578F14D39A6D7A1C9FC5413CFCB21C87DD6A5725B`。探针先删除私有目录的三份旧输出文件，排除前轮缓存掩盖生产顺序。真实 `GraphEditorDocument` 在 owned Group 内放两相机映射、映射标定、标定加载；相机输入点叶由 `WorkflowSubscription.NumberArrayConstant` 各绑定两组四点，真实 GraphExecutor 读取；相机 B 的 `CalibStatus` 触发 Map `RefreshSignal`，Map `CalibStatus` 触发标定加载 `RefreshSignal`，并用执行边保证文件生产先于读取。Group 输出 `Matrix:NumberArray` 给父流程坐标系 `CalibMatrix`，父流程 ImageSource 提供 2×2 GrayImage。

首跑四个内层模块、Group、坐标系全为 Success；相机 A/B 文件均 36 字节，Map XML 656 字节；标定加载输出九项，坐标系输出九项。文件 SHA-256 分别为 A `E2196EC26DEC21D729B86D14FA52A70AF217D9B4C87D7B40681AF01B66BFC06D`、B `0D965A2D20B81C97A526A11E629DDD8D5112F2F10D47017CF1E09A1E4A38D6C0`、XML `BDA8736F696E5B8721F811B31BE8486D3C09B436BD32CFE30A1587221275E967`，均在忽略目录。测试另直接核 `MapCalibrationFiles.Read(camera-a.iwcal)` 成功、`ReadCalibFileData.Read(camera-a.iwcal)` 返回 `0xe0000201`；Map XML 由标定加载成功解析为九项。当前两工具 Descriptor 均无 `CalibMatrix` 直接端口，不能把内部矩阵绕过文件链强连。

保存整个图为 `camera-map-chain.vmx.json` 后再次删除三份私有输出文件，新建 GraphEditorDocument 和 ModuleExecutionScope，重新注入 GrayImage，再跑四内层模块、Group、坐标系仍全 Success，且三份文件重新生成。此项证明当前方案绑定、节点参数、owned Group 连接与文件链可重开；不证明搬迁到其他路径或原版同构整图。原始产物保留供根线复验；重复运行会重写文件和方案，内容哈希可能随实现与序列化变化。

## 仍未闭合的独有边

1. 映射标定**图像模式**的双 IMAGE 到达顺序、ROI1/2 形状所有权、特征点拟合、教学/示教、结果图层及原版同条件数值差分，须按原 `21690→203d0/22f40→21c50→220b0` 与现 `MapCalibrationPlugin.Image/MapCalibrationImageSolver` 专查；文件模式通过不能关闭它。
2. 相机映射及映射标定文件模式的任意原生异常/设备范围、源输出矩阵在本次特定四点输入下与原版**同数据集**逐项数值差分未运行。本轮只证明当前链输送正确、文件载荷可相互解析，不把既往其他夹具回归贴到本图。
3. 原版 Platform 对两个相机结果、Map 写文件与 ReadCalib 刷新在 Group 中的实际调度/历史回调，以及 Save/Close 后失败文件的 UI 呈现仍缺同构运行证据；已有 `docs/plugins/MAP-CALIBRATION-RESULT-WRAPPER-AUDIT.md` 明确原生 Platform 回调边界未知。

无产品修改：已核范围内没有需要主框架独占且有完整原合同支持的差异修复。算法与专属 UI 的后续修复归插件项目所有，当前只交源码级问题与可复验图，不动其在制工作树。
