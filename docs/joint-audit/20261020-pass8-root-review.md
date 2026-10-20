# 第八轮根线自审：四类标定生产者的文件/图像载荷

范围仍是用户指定的“标定”十工具及必需框架边。四个生产者类别按实际载荷分开审，共同目录、配置、Group 历史及坐标系矩阵消费者引用第六、七轮证据，不重复计数。原安装源码/资源只读，`tool-plugins` 在制工作树未改。

| 生产者链 | 本轮源码和当前实际结果 | 明确不计完成的边 |
|---|---|---|
| 平移旋转标定 | 原 `Process/ProcessAlg/WriteXmlData` RVA `0x3a8c0/0x3b0e0/0x3e660` 支持成功后九项文件矩阵；[独占报告](20261020-pass8-translation-result-chain.md)。当前真实图 `平移旋转→XML→标定加载→owned Group→坐标系` 首跑及方案保存重开均 Success，九项矩阵逐边核对；日志 SHA-256 `909B59085C9BE6791989FA71ED18B493203281010F7C3920E5A5AFD22F7D1783`。 | 原版同构整图、组合/示教/异常及逐位数值。 |
| 相机映射＋映射标定文件模式 | 原相机模块 `GetProcessInput/ProcessAlg/WriteCalibFile` 形成 36B float32 文件，Map 文件模式 `ReadFileMatrix/ProcessAlg/WriteXmlData` 形成 XML；两工具均无直接 `CalibMatrix` 输出端口。[独占报告](20261020-pass8-camera-map-chain.md) SHA-256 `694A49C55012D7CF265C8EDCE4918F9CAA326DC4F1332ABBD047C2F81CCC91EA`。当前双相机文件→Map XML→标定加载→owned Group→坐标系全部 Success；删除三份产物后保存重开重新生成并成功，根线独立 Release 复跑 exit 0；分线 stdout SHA-256 `91A75D6C263B0DAF08D9463578F14D39A6D7A1C9FC5413CFCB21C87DD6A5725B`。 | Map 的图像/ROI 模式、同夹具原生数值差分、原版同构调度。相机 `.iwcal` 不能直接交 XML 标定加载。 |
| N 图像标定 | 原 `IMVSNImageCalibModu::WriteXmlData` RVA `0x230e0` 输出 `CalibOutputParam/CalibMatrix` 九项；原 XML 的 IMAGE/点位端口与当前独立 `CalibData` 分开。[独占报告](20261020-pass8-nimage-board-chain.md) SHA-256 `C618A2353CB469DFCA2A062B6C61907995A254A39240495F36C7E05D431E7DB0`。既有真实主窗九帧专项此轮 49 项通过，实际 XML 被新图的标定加载/坐标系消费；根线另外独立复跑主窗专项 exit 0，日志 `analysis/joint-audit/pass8-cal-image/root-host-rerun.log` SHA-256 `6364F879D589B1D5DF89AAF0F416EBBF61B74544ECDB27266BCE495A93403E35`。 | 原厂同构图、N 图像 `ProcessAlg/ProcessOutput` 全分支、其他运动帧/失败；不能用旧专项通过替代这些分支。 |
| 标定板 | 原 `ProcessAlg/ProcessOutput` 输入 IMAGE/ROI，输出点列；自有 SDK 导出 36B `.iwcal`。当前 Gray8 图真实执行检测 99 点、写二进制矩阵，XML 加载器精确拒绝 `.iwcal`。同一探针保存重开保留三条 typed 边；分线 stdout SHA-256 `A207E61CFDE120B09690792F0A15FC1DAFD423B8E9D947536A3234651AC7FE92`，根线复跑 exit 0。首次错连灰度图的可选 `ImageMono8` 导致 `UpstreamUnavailable`，旧失败日志 SHA-256 `358A598D37CFC1CA3398AABD756C36CE2BA13608BD37D4D0D8BD799409CBAA57` 保留，修成普通 `Image` 后通过。 | 其他板型、彩色/ROI、Group 与失败后旧结果、原版同构整图；板二进制与 N 图像 XML 不可互换。 |

第八轮只关闭上述**具名结果交接切片**，不宣称十工具完整 J01–J12 或算法全域符合。无原版依据支持且属于 main 独占的产品修复，框架实施进度仍 **41/96=42.71%**；人工验收单列。下一步优先畸变标定的独有图像/ROI/文件与旋转标定的文件消费者，再核标定加载/坐标系剩余动态分支；非标定工具放下一目标。
