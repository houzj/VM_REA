# 映射标定真实搜索到锚定 caller 捕获（2026-10-19）

本轮在实际 SDK 图像执行中关联 `813ca0` 的 opening/payload 与 `813020` 的方向、锚点、原点、完整格网和最终 SDK 点序列。前轮 320 例微核结论保留其独立范围；本轮没有修改产品入口或共享识别器，不开放八向产品识别。

## 原始证据和身份

目标为本机 VisionMaster 4.4.0 `MVD_Algorithm.dll`，SHA-256 `85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549`。在 probe 自己创建的 `MVDMapCalibCpp` SDK 进程设置可恢复软件断点，未修改磁盘 SDK、其他进程或设备。

沿原厂 Map `7d3e90 → 7e3f50 → 816650 → 80e690(HIK7)/80bcd0(HIK9) → 813ca0 → 813020`。最终 caller 返回地址 RVA 观测为 HIK7 `0x80f8ed`、HIK9 `0x80cf17`。本地反编译身份：

| 函数 | 当前导出 C 文本 SHA-256 |
|---|---|
| 18080e690 | ea3ac6e70964dedf4fdcd98115d98ad8ad88e6462d1ecaac34b273f962443ff5 |
| 18080bcd0 | 8cc66bd8bd89c70c74061d176d0547518befa73672f5317a9b56b55dea1cd920 |
| 180813ca0 | 10fc7623e3e1bdd0ae0110d244b1f83be33d848506b37ccdc80a9766e2625012 |
| 180813020 | b82cdb0817acbc5181f96844c465970a102c4df764ee10a784a9e4ee7b45d5fb |

沿用实际本地 fixture `analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp` 与 `frame-4.bmp`，文件 SHA-256 分别为 `86e175cd993f9737380690a60f7bada49935196691ca4509c93ff0066fadcc40` 和 `d1ed36d2cf24a57ab541191e1fde147c826ab182f0d4e6560b5fc69a304e73af`；补充现有 `frame-r0.bmp/frame-r4.bmp`，未新造图像。各输入身份保存于独立输出目录 `identity.json`。

## 捕获相位及严格核对

每次以同一 sequence 记录 `search-entry → decode-attempt → selected-tag → map-entry → map-return`：

- search-entry：完整整数格网、width/height、caller 搜索偏移；
- selected-tag：解码状态、尝试次数、六字节 payload、选定 opening 八个 float、搜索 anchor；
- map-entry：相同序号的方向、decode 方向、搜索 radius、调整后 anchor、origin、所有格点槽的整数索引和像素；
- map-return：状态、尺寸、依据输入第一整数哨兵计数的完整点序列；另与 SDK 公开 Source/Target 点序列核对。

对比以捕获的 map-entry 槽和原厂八向微核规则计算输出，全部 ID/顺序精确比对，像素按 float32 位比较；不是容差验收。公开 SDK 点从 SDK 原生输出读取，不使用托管产品作为 golden。基础图 4 次真实调用 **5429 个字段零差异**；补充 r 图 4 次 **5285 个字段零差异**，合计 8 次、10714 个检查字段。两组不重放既有 source-grid/18 组/7361 ROI 回归。

## 源码确认并在这组调用观察到的关系

两图×HIK7/9 每次一次解码就成功，payload 均为 `[3,3,0,10,0,0]`，方向均为 **1**；搜索选定 anchor 为 `(4,4)`。caller 搜索偏移 `(1,1)`、radius=1，因此传给 mapper 的 anchor 是 `(3,3)`：方向 1 的源码调整为 `offset + selectedAnchor - 2*radius`。这个减 1 结果来自实际偏移和 radius，不能将“搜索 anchor 一律减 1”写成一般规则。源码其他方向组合见 caller switch，但本轮未命中它们。

D1 后续源码复核纠正字段命名：`813ca0` 的栈 param5（入口 RSP+0x28）贡献 Y，param6（RSP+0x30）贡献 X；本轮原始 trace 曾分别标为 offsetColumn/offsetRow，两个值恰都为1，故原始数值与本轮核对结果不变，但不能据这个对称输入证明标签正确。保留原始 trace 不重写；未来 header 已交换标签并增加 `offsetAxisLabels=source-confirmed`，比对器明确兼容旧标签。详见 D1 源码链报告。

HIK7 origin `(3,3)`；同 payload 的 HIK9 origin 为 **(192,768)**，由 `(b0 & 15)*64 + (b1 >> 2)` 和 `(b1 & 3)*256 + b2` 得到，与实际 mapper 参数完全一致。这闭合了一个非平凡原点的真实 payload→caller→mapper 路径，不是只在微核中人为传 origin。

基础两图的格网均 14×14、196 槽，其中 28 个第一整数为 INT_MIN；mapper 发出 168 点。**search-entry 与 map-entry 的全部整数槽逐项一致**，说明这些具体调用中的 28 个缺失槽在 `813ca0` 搜索之前已经存在，该阶段没有新增/删除格点哨兵。不能把“选定 opening 后 mask 所有成功标签”当作本轮原厂行为，也不能仅凭这些槽推断更早阶段为什么排除它们。

基础源图 opening 第一角为 `(222.496078,227.963120)`，最近有效原生格点槽为 `(2,2)`、格点整数 `(3,3)`、像素 `(222.485886,227.975266)`，距离约 0.01586 像素。目标图相应约 0.01588 像素。这是图像上的几何关联观察；opening 是拟合提案坐标，不能把它直接当最终精化格点或据此规定任意 opening 的角索引。mapper 直接复制槽内像素，和 SDK 最终点逐位相同。

## 未知及交接边界

补充 r 图仍全部方向 1，图像文件名带 r 不证明原厂走其他方向。本轮未建立方向 2..8 的真实图像/解码方向对应关系、失败或多成功 proposal 搜索顺序、任意 payload 锚定、缺失哨兵的更早生成原因、共享独立 opening/decoder 与原厂 proposal 的对应关系。不能由微核八向通过和本轮方向 1 的真实 caller 通过推导全部八向图像等价。

共享 `WorldIndex` 的整数公式无需修改。本轮无产品/共享接口/持久化变更，无提交 SHA；根线统一向用户指定的“框架层逆向”聊天通知文档和证据，由用户决定集成。收到通知不等于另一工作树已经同步。

## 复现与输出

```powershell
& tests/MapCalibration.NativeProbe/RunImageAnchorCallerTrace.ps1
& "$env:USERPROFILE/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe" tests/MapCalibration.NativeProbe/CompareAnchorCaller.py
& tests/MapCalibration.NativeProbe/RunImageAnchorCallerTrace.ps1 -SourceRelative analysis/full-audit/n-image-calibration/solver-fixture/frame-r0.bmp -TargetRelative analysis/full-audit/n-image-calibration/solver-fixture/frame-r4.bmp -OutputName image-anchor-caller-rotated-trace
& "$env:USERPROFILE/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe" tests/MapCalibration.NativeProbe/CompareAnchorCaller.py image-anchor-caller-rotated-trace
```

当前工作树 `analysis/map-calibration/image-anchor-caller-trace/` 与 `image-anchor-caller-rotated-trace/` 保存 `tag-trace.jsonl`（原始相位）、`image-results.jsonl`（SDK 公开结果）、`identity.json`、`comparison.json`（严格对照与原始 trace/函数哈希）。这些为 ignored 本地证据；原厂 SDK 与本机 Ghidra 导出不随 Git 分发。SDK 头文件两条既有 C4091 声明警告不改变运行结果；probe 不请求托管构建或桌面/设备测试。
