# 六种标定板的公共阈值参数块

2026-10-19。此切片补齐上一轮仅圆点候选入口的板型覆盖；不代表六种识别算法均使用相同阈值。

`BoardTypeOptionsProbe.cpp`复用原模块构造、SetParam、ProcessAlg及原SDK，同一个实例顺序运行XML暴露的板型1、2、6、7、8、9，每种分别设置Low/High为20/40、0/40、20/80。仅替换既有8个模块IO导入，不启动平台或设备。自有线程硬件执行断点观察SDK setter、算法RVA `0x7e4ff0`和`0x815560`入口，不改算法指令或输入参数。

18次ProcessAlg均code=0/status=1；18次pipeline与18次dispatch参数块的`+0x28/+0x2c`逐次精确等于设定Low/High。公共块`+0x24`均为Circularity20转换后的float位1045220556。板型到核心kind的实测关系为1→1、2→3、6→6、7→7、8→8、9→9；不能把XML圆点2误当核心case2。

源链：`analysis/calibration-board/core-live/output/1807f69e0.c`从engine `+0x1ac/+0x1b0/+0x1b4`构造公共块的圆度/Low/High，`1807e4ff0.c`传入`180815560.c`分派。case1按选项`+0x44`选择不同分支，其中873de0接收投影后的紧凑参数；该投影没有复制公共Low/High。case3为812930圆点分支，前轮已追到877230候选阈值。case6/7/8/9分别进入80f9a0/80e690/80cfc0/80bcd0。这里的分派名称来自反编译源码，当前新断点只观测公共入口，未逐个捕获下游入口。7dda50属于核心case3的后续自动精修路径；默认样本未命中不能证明不可达。

因此，已确认六种板型都把用户阈值带入公共识别分派，尚不能据此宣称各下游检测、精修或任意图像都消耗这两个字段。不得把圆点阈值算法无证据推广到棋盘或编码板。

可复核产物：`analysis/calibration-board/board-type-options-b20de5dfa108435baf63a02143e253c5/{runtime.jsonl,summary.json,build.log}`；runtime SHA256为`72c449a4e3da60de1e93f08bd3482f64ef3eebbb1fb13e389f5455263f05e3f9`。summary含三原厂DLL、探针、复用文件、脚本及四张图的身份哈希，`verified=true`。重跑入口为`tests/CalibrationBoard.NativeProbe/RunBoardTypeOptions.ps1`。生成目录不作为可移植Git证据；探针和本报告可交接。无产品代码、公共契约或另一工作树变更，无新的宿主桌面验收。
