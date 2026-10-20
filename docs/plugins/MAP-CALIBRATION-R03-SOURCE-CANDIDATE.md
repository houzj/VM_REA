# R03 当前24符号纠错实施候选（2026-10-19）

按根线要求，候选放在 `analysis/map-calibration/proposals/r03-source-codeword24`，尚未改共享文件。`integration.patch`含新增内部类与现有TryCorrect方法的整段替换，当前共享快照上 `git apply --check --ignore-space-change` 通过；before/after快照及hash在该目录索引。B占构建槽，本轮未构建或运行C#候选。

逐操作翻译8b0260/8b11a0/8b0670固定18/24实际入口，复用本轮已取必要叶体：保留六块128-byte scratch的descriptor推进、未写零tail与端对齐xor顺序；原根数等degree门、两次反序及截取偏移、逐位置纠正量、最终24项xor；不再用独立Gaussian求幅值，不新加degree1..9或post-syndrome拒绝门。

931c60分子0的Log[0]读取仍未知，候选显式 `NativeStatus=null` 与未支持原因，不填0冒充原厂输出。固定span长度、独立managed缓冲、不可变表是明确安全选择，不声称原生任意指针/别名/并发语义一致。现有MSB bit打包与12 byte反序导出已对应，补丁不改其入口。

独立 `replay_candidate.py` 以相同系数/缓冲顺序通过既有同hash原厂40个0..9损坏正例，共960输出符号逐项一致。`independent-replay.json`明确这是Python独立复演，不能称C#候选运行成功。最小失败词byte[0..23]复演degree9/rootCount1、-1000；一条原8b0260(bits,18,24,out,scratch)记录即可取得失败oracle，目前未运行该词。目录Program.cs准备40正例逐符号/导出byte检查及该根数不匹配分支，计划PASS122；原厂失败trace仍独立。

生产接入只需根线审查/应用共享补丁、取得构建槽编译候选及现有NImageCode.Checks、必要Map识别回归。R04真实方向/anchor、warp/采样差异不因R03候选而闭合。Map专属新帧角度修复已单独通过InputStateChecks PASS48，矩阵见MAP-CALIBRATION-PRODUCTION-STATE-MATRIX.md。

整体仍未宣布新完整工具；源、实施、自动验证和自审全部完成才计数，人工验收单列。
