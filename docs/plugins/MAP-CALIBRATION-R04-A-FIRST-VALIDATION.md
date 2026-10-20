# R04 A 首轮候选验证与像素首差

首次验证严格失败被保留，不能用整链 payload/点集通过掩盖归一化像素差异。无共享生产修改；本轮仅候选验证。

日志目录 `analysis/map-calibration/r04-a-validation`。最初 Opening 构建 NU1201，因为候选 net10.0 与 Calibration 的 net10.0-windows 不兼容；两候选 csproj 已改 windows。Opening 重建及 SourceBoard 构建均 exit0，0warnings/0errors。Opening 运行写出比较后因严格像素门失败，其异常进程未自行结束；仅向本工具创建的 session76590 发 Ctrl+C，最终 exit1。未操作他人进程。

Opening 比较：Refined=false，与原80ac20返回0一致；四份原crop的40000 score ints全部一致，候选12 payload字节一致；normalized有12/40000个字节差，每个±1，故测试失败。首点(x164,y19)候选237/原238。原始 comparison 保存在 r04-opening-managed，全部差异清单在 r04-a-validation/normalized-differences.json。

SourceBoard 直接 SourceGridPyramid→Context→A→B→方向/anchor→WorldIndex 通过168点，全部ID、原序、XYfloatbit一致，运行exit0。其首轮日志没有记录SelectedRefined，不能据此推断精定位成功；后续候选增加该诊断。首轮二进制哈希见 initial-binary-identities.json；当前源码已有后续诊断/修正，与首轮二进制不是相同版本。

## 原始 H 定点补证

只对原有同quad取7ff3c0与730e90，不扩图像。7ff3c0 ABI由80ac20汇编80aeb3..80afca确认8参：q、canonical、4、H、rms、state、config、workspace；config=[0,1,3,20,0x38d1b717,0x40000000]。730e90 ABI为H、3、packed3x3、workspace。独立原厂进程 exit0，无平台Init/设备使用。

原件同hash，输出在 r04-opening-fit-oracle，包含 provenance、原H与逆H的f32原始字节。fitReturn=1，fitState=1，rms=1.52587891e-05；inverseReturn=1。私有只读工程补7ff3c0体至 r04-opening-fit-source，Ghidra进程exit0；730e90复用 ghidra/core 既有原体。

## 首差修正

独立 Python 将原H带入候选完整pivot逆序，九个逆矩阵float bits全相同。用原逆H和原图复演候选warp仍恰好12字节差，因此该输入不应先归罪于拟合或逆核。

80ac20 汇编80b230..80b24e先算每行 h1*y+h2，80b280..80b294再加 h0*x，80b298..80b29c做除法。C反编译把它呈现成左结合加法，候选通用 Project 采用了(h0*x+h1*y)+h2。仅Opening warp改成h0*x+(h1*y+h2)，三行一致，不改变B其他来源核。

独立复演修改后40000归一化字节0差，报告 original-fit-independent-replay.json；脚本 replay_opening_fit.py。这只是独立算术复演，尚需重新构建运行C#确认；严格门没有放宽。

当前源码/补丁身份见 proposals/r04-source-recognition/r04-a-index.json；managed槽已交还根线，后续重建等待预约。
