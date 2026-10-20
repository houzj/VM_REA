# 映射标定 813020 八向锚定的原生边界（2026-10-19）

本切片闭合身份映射微核的有限输入域，未接入产品八向识别。没有修改共享 Calibration、Map 产品入口或主程序；不重复既有 18 组图像/7361 项 ROI 回归。

## 证据身份与调用链

原厂 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll`，SHA-256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。已保存反编译 `analysis/map-calibration/ghidra/core/180813020.c` 的 SHA-256 为 `b82cdb0817acbc5181f96844c465970a102c4df764ee10a784a9e4ee7b45d5fb`。

源链为 Map `7d3e90 → 7e3f50 → 816650 → 80e690(HIK7)/80bcd0(HIK9) → 813ca0 → 813020`。两 caller 的 `813020` 在已成功选定 payload 后使用：HIK7 caller 第 671 行、HIK9 caller 第 674 行（当前本地反编译文本）。这次只直接调用最后的 `813020`，不能将直接调用等同于 SDK 图像检测或 proposal 解码生命周期。

`813020` 的 x64 参数按直接调用实测为像素行指针、格点整数行指针、打包 width/height、anchorRow、anchorColumn、originX、originY、orientation、输出 dimensions、输出整数点、输出像素点。原厂函数没有输出点数指针；probe 预填输出哨兵并据首次未写槽记录数量。输入/输出缓冲区均为本 probe 自有，不修改磁盘二进制或其他进程。

## 源码确认并经直接执行核对的规则

令 `dx=column-anchorColumn`、`dy=row-anchorRow`。世界整数坐标在下表 local 的基础上分别加 originX/originY；像素坐标仅从对应输入槽复制，不作旋转计算。

| orientation | local X,Y | 输出槽遍历顺序 |
|---|---|---|
| 1 | dx,dy | 行递增，列递增 |
| 2 | dy,-dx | 列递减，行递增 |
| 3 | -dx,-dy | 行递减，列递减 |
| 4 | -dy,dx | 列递增，行递减 |
| 5 | dy,dx | 行递增，列递增 |
| 6 | -dx,dy | 列递减，行递增 |
| 7 | -dy,-dx | 行递减，列递减 |
| 8 | dx,-dy | 列递增，行递减 |

格点输入第一整数为 `INT_MIN` 时跳过该槽，其他点保持对应遍历顺序。有效 orientation 返回 1；输出 dimensions 只有 2/4 写为 `[height,width]`，1/3/5/6/7/8 写为 `[width,height]`。**6/8 不交换尺寸**是原厂函数实际行为，不按几何直觉补成 2/4 的规则。orientation 0/9 返回 0，并保持预填的尺寸和点缓冲区；没有对全部其他整数方向值作穷尽执行。

原生实际运行 320 例：宽高各 1..4、完整与 `(x+y)%3==0` 第一整数为 INT_MIN 的稀疏网格、orientation 0..9。使用不等 anchor(12,24)、origin(101,203)、非整数二进制可精确表示的像素以区分参数/槽位。对照 Python 独立标量转录，1304 个点记录、6176 个检查字段零差异。偶数/奇数行数和非方形输入均覆盖，确认反编译的双行展开与尾行分支。

## 与现实现的关系和同步要求

共享 `NImageBoardIdentity.WorldIndex` 已有的八向整数公式与该原生边界一致，无需修改。共享 `NImageBoardRecognition.Recognize` 当前只接受 decoder orientation 0，并固定调用 WorldIndex orientation 1；最终按世界 IndexY/IndexX 排序。这个识别入口没有携带原厂 `813020` 的方向、行指针网格、稀疏哨兵及原生输出尺寸契约。因此不能仅扩展 decoder 的 orientation 接受条件就宣布八向闭合，也不能用当前产品排序替代原厂八向输出序列。

后续最小共享接入前置证据为：在真实 `813ca0 → 813020` caller 上捕获 orientation、anchor、origin、网格尺寸/整数槽、最终输出序列；与同图 opening/payload 结果建立关系。当前 probe 使用人为指定参数，没有证明 payload 到 origin 的生成、orientation 与 decoder orientation 的映射、opening 第一角锚定或 decoder 自身的正确性。未知输入包括零/负尺寸、越界行指针、算术溢出、非有限像素、任意内存残留；不向这些范围推广本次通过结论。

交付给主程序的内容是原生映射规则和剩余接入前置条件，**没有共享接口/持久化变更，也无需主程序合并算法补丁**。尚无提交 SHA；遵守本轮不提交推送要求，由根线统一通知，通知不代表另一工作树已同步。

## 复现

```powershell
& tests/MapCalibration.NativeProbe/RunAnchorMapping.ps1
& "$env:USERPROFILE/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe" tests/MapCalibration.NativeProbe/CompareAnchorMapping.py
```

输出位于当前工作树 `analysis/map-calibration/anchor-mapping-native/`：`results.jsonl` 为原生执行原始结果；`process-status.json` 为进程退出和二进制/反编译身份；`comparison.json` 为对照及原始结果哈希。输入黄金值不由产品托管算法生成。该目录为 ignored 本地证据，单独拉取 Git 不包含机器上的原厂 SDK、Ghidra 导出或本次运行结果。
