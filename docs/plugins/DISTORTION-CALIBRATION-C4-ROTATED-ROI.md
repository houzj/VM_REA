# 畸变 C4 旋转矩形 ROI 实施与源码证据

2026-10-19。此报告只闭合实际旋转 Box 识别路径；完整工具仍 0/10。人工验收单列。

## 原厂直接链

- SDK MVDImageCalibCpp.dll SHA256 AE4D64C5505E25E192EC39A5DF105DA16A2C69DE51D23F8E5B66C97A771FA120。已有 wrapper/180003fe0.c 构造预处理对象；180002680.c 克隆 ROI 并置 dirty；180002a70.c 的 Run 对矩形 float angle 转 double 后绝对值大于 0.0001 走 PreproMask。常量 RVA 0x7db8 字节 2d431cebe2361a3f。轴对齐分支直接 bbox；旋转分支生成 mask，随原图和 bbox 传 core op0x67。
- core MVD_Algorithm.dll SHA256 85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549。c1-workspace-source/core/output/18001e330.c 将原图/mask/bbox 传 op0xe；矫正图重检沿用 ROI/mask。1807c6ba0.c 分派至 1807f69e0.c；该体校验 mask 同尺寸，经6817d0取非零 bbox，mask 分支调用7e56b0。
- 本次定向被动取体：c4-source/rotated-roi-core/output/1807e56b0.c、1807e56b0-asm.tsv、1807e56b0-calls.tsv。目标严格匹配上述 core 哈希。7e59dd/7e5cb8 调用816650。
- 同 core 哈希只读复用 Board 源体：analysis/calibration-board/core-live-97401c10fc6645cbbd8ea48882369679/output/180816650.c 按 board7 分派80e690；core-live-93e5dec0a8ab4550843ccedc08eabf51/output/18080e690.c 对未改写 bbox 像素识别，在编码身份解析前按 mask[(int)(y+.5f),(int)(x+.5f)]筛点，再以 float 加 bbox 原点。不是旋转重采样，也不是拟合后筛点。

以上本工具相对路径前缀均为 analysis/distortion-calibration。复用 Board 文件仅为同原件公共 callee 证据，不冒充本工具新运行。

## 独立实施

DistortionCalibrationRecognition 新增 RecognizeMasked，复用已有 CalibrationBoardSolver.RecognizeMasked 和 RectangleRoiMaskRasterizer。运行入口 Crop 对实际旋转矩形保留原图及 mask；原图和矫正图均经同一 Recognize 分支。编码板先筛候选后解码身份，再回投原图坐标。轴对齐原路径保留。

完全在图外的空 mask 采用产品安全错误 0x80100025；这是独立安全映射。既有原厂 PreproMask 全外样例存在 fail-fast，不能将此错误码称为原厂等价。未新增原厂执行，既有公共 mask 原厂 fixtures 只支持公共基元证据，不证明所有旋转畸变场景等价。

## 离线验证

DistortionCalibration.RotatedRoiChecks：151 checks，通过；15度包围框原图/矫正重检168点；0.8倍尺寸旋转子框144点，逐点检查原图 mask 成员及输出角度/运行状态。输入为既有SDK fixture.bmp。原6模型回归结果另见 companion validation JSON。

## 残余

历史未初始化 prefix 的安全兼容策略仍需明确，不能清零或硬编码伪等价；完整支持域/退化与路径失败、保存恢复及真实宿主自审仍按整工具清单推进。此片不因平台/WPF 内部未知扩大边界，也不计整工具完成。
