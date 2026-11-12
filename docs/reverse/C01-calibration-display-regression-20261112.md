# C01 标定显示消费回归

日期：2026-11-12

- `CalibrationCameraMappingDisplay.Checks`：7 项通过，覆盖成功结果、记录/结果列、纯文本显示、错误清理、修复后恢复和切换节点释放。
- `CalibrationNPointDisplay.Checks`：6 项通过，覆盖成功图形、记录/结果列、退化输入错误诊断和切换节点清理。
- `CalibrationMapDisplay.Checks`：因当前证据资产只有不足两帧，未满足测试前置“two real calibration frames present”，未修改产品代码。

Map 显示缺口属于测试资产/证据前置，不以合成图像补齐。原厂 native 求解和人工视觉差分保持“运行未确认”。
