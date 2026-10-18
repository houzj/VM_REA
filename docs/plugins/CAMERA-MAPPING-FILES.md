# 相机映射标定文件、缓存与失败资格

原厂安装模块CLI静态证据位于当前工作树 `analysis/camera-mapping/ghidra/module`：GetProcessInput=0x18000a710、ProcessAlg=0x18000bd20、ProcessCalibMatrix=0x18000c200、SetParam=0x18000ce30、WriteCalibFile=0x18000d450、ResetMember=0x18000caf0。它们是独立CLI结果，不冒充REA原生提供器输出。受控模块探针使用正常创建官方算法对象与synthetic ModuleFrame IO，无设备和平台Init。

| 操作/边界 | 原厂证据 | 当前实现与验证 |
|---|---|---|
| CalibPathName | SetParam只存路径；OpenFileForCalibDialog验证后SetParamUnit及PreExecuteRun | 公共State按源CanTriggerRun执行；Refresh=false时不立即写cached矩阵，实际Graph检查通过 |
| RefreshFileEnable | ProcessAlg完成MatrixToParam后置两个有效标记，再WriteCalibFile | 有效2点运行写文件；无效父目录返回0xe0000001，不发布输出；产品同边界 |
| 显式SaveCalibPath | 要求this+0x18有效；规范化路径、先更新同一内部路径，再ProcessCalibMatrix重算并写 | 配置保存更新路径读回；重新使用缓存点及当前HomoFreedom/Weight参数，而非直接复制旧矩阵 |
| 文件格式 | WriteCalibFile用_wfopen(wb)，GetResult取得9float，fwrite 0x24字节 | 小端36字节，无自创头；原厂透视、重算仿射、自动保存及IO失败后导出样例保留哈希/字节 |
| 模型编辑后保存 | 4点透视→只Set HomoFreedom2→SaveCalibPath | 原厂导出变仿射；产品9系数逐项比较固定0.003通过 |
| 点缓存生命周期 | GetProcessInput仅在容量0x70小于当前总点数时重新分配；否则覆盖前缀。ProcessAlg循环当前数量，ProcessCalibMatrix循环容量 | 保留旧尾点，未擅自修复原厂行为；4点→2点后导出与官方文件9系数匹配 |
| 自动保存IO失败 | ProcessAlg在WriteCalibFile前已置有效标记，失败不清该标记 | 原厂自动写失败0xe0000001后，换有效路径显式Save成功；产品同样可重新保存 |
| MatrixToParam失败 | 置有效标记之前失败；两点零尺度常量矩阵返回0x1000010f | 随后显式Save返回0xe0000018；未生成singular文件 |
| 缺少输入 | Process开头清有效标记，输入缺失后没有可保存结果 | 原厂显式Save返回0xe0000018；产品失效保存资格并清显示 |
| 清理与复位 | ResetMember清标记/点缓冲/路径；配置ResetAlgoParams是独立公共宿主动作，不能擅自等同析构ResetMember | 实际配置确认复位参数并恢复两组固定点通过；作用域销毁释放工具状态，不清别的流程 |

原厂导出文件：`module.iwcal.perspective.iwcal`、`module.iwcal.recalculated-affine.iwcal`、`module.iwcal.refresh.iwcal`、`module.iwcal.after-io-failure.iwcal`。原厂SetParam/Process结果均在 `analysis/camera-mapping/native/module-results.jsonl`。109项生产WPF/Graph检查和118组SDK差分通过；仍不证明完整原生平台订阅或桌面像素一致。

2026-10-18补证：官方受控模块one-pair-other-group-absent返回0x80100005；no-active-groups返回0xe00000ff；active但所有叶都缺失返回0xe0000309。产品按整组0点与部分数量错误分别处理，不把模块输入失败都报成求解器点数错误。

显式导出已消费ReadEffectiveParameters；UserData写入构造next前同步既有values，避免解绑已保留50后被旧seed20覆盖，生产控件检查已验证。
