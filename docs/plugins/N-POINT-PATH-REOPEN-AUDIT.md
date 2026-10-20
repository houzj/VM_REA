# N点工具路径重开完整自审核

2026-10-19。复用最终435文件快照SHA82A789C3305AEE17AE459671630D88AC4AE4C83EB94C953B76D8DD9055E2197C及7日志，未重跑数学。本页更正先前整工具“无缺口”的结论：以下实际UI路径差异修复/验证前不得恢复计数。

| 活动链 | 自审结论与现有证据 |
|---|---|
| 资源/Reset/数组/教学/算法 | 复用N-POINT-FINAL-IMPLEMENTATION-EXIT中的Reset36/Numeric12/Text43/Alias35/Plugin5817，无新算法差异 |
| OpenFile路径 | 原OpenFileForCalibDialog→ValueChanged同Translation；现Attach已接source门，picker无条件history/force重试。root五消费者专项在跑，尚不提前写通过 |
| 路径成功通知 | NPointConfiguration旧专属Lang_SetSuc与新Attach.Completed重复。原只有一次；候选删专属一次，必须实际验证 |
| 显式Save | 原SaveFileDialog.SaveLocalFile先Filter.Split长度2、FileName.EndsWith第一段（CurrentCulture大小写敏感），再IUserStringData→CalibPathName GetParam→history→Success。产品旧Export OrdinalIgnoreCase会放行.XML，且额外UI长度260门无本体证据。候选按真实SaveCalibPath/FileOption复原前置门，专属Config补大写拒绝 |
| 状态/保存 | 原有Configuration53覆盖真实成功保存及取消，Host32整方案重载/点表与样式身份；PrepareSave把32×5表从scope写入文档，保第6内部角度不新增持久化字段 |
| 释放/失败恢复 | Host32已核配置关闭disposed、重开、clear退化失败隐藏旧path/axis、恢复成功、再次关闭；execution.Changed解绑并DisposeAsync。未声称native地址相同 |

候选在analysis/n-point-calibration/path-reopen，未改冻结产品、不build。旧Config wrong.txt回滚断言同步改成attempt/backend分离；不是继续保留旧行为。远程/root仍显式Unsupported，设备/任意native heap和外界provider不扩成本工具数学缺口。人工验收不计进度。

## Save路径读回原指令

原SetParam 18003e560 SaveCalibPath：33420校验.xml→WriteXmlData成功0后才清this+15da1共30c字节并memcpy_s(path,30b)。原GetParam 180039c20.asm的39ef4..39f2d（SaveCalibPath）及39f66..39f9f（CalibPathName）均把this+15da1放入格式化参数，二者同缓存。因此正常短路径导出成功后的selected path确为模块读回值；失败保旧值。native超长buffer/invalid-parameter handler不从托管路径长度规则猜造等价。
