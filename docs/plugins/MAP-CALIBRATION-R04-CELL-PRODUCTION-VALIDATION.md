# R04 6/8真实cell入口与四型生产验证

主线已审查并应用prefix interval与6/8 cell proposal两个补丁；并同批冻结C轴原图/crop运输及公共数值读取/文字首项修正。D仅执行生产回归，不直接改共享产品。上一批7/9双图、Map9ROI通过仍为历史证据；没有用它们覆盖新发现的6/8漏接。

一次生产构建exit0、0warnings/0errors。项目是现有CalibrationBoard.Checks，仅ProjectReference生产Board程序集，不Compile任何D候选源码。运行cwd使用D独占r04-cell-production/work布局，docs/tests及只读frame fixture经junction访问，所有差异JSON写入该私有cwd的analysis/calibration-board，未覆盖主线首FAIL或旧历史文件。

串行实际结果全部exit0：

| 生产入口 | 原fixture范围 | 严格结果 |
|---|---|---|
| SolveCropped，--encoded-axis-runtime-only | 6/7/8/9各3个bbox，共12例 | Error=0；全部点ID/顺序/XYfloatbits、H、14个标量逐位相同；aggregate PASS1 |
| SolveMasked，--encoded-mask-runtime-only | 四型原index1 mask，共4例 | Error=0；全部点ID/顺序/XYfloatbits、H、14个标量逐位相同；aggregate PASS1 |
| 真CalibrationBoardPlugin运行，--encoded-full-product-only | 四型原fullimage，共4例 | 全部公开端口存在，点/H/14个公开标量逐位相同；PASS12 |

首失败正常6/index0现恢复；其13×13满格网必须走80f9a0内联四邻cell候选，不能走813ca0缺槽窗口。8型同样走80cfc0。具体原source/anchor/prefix全可能值证明见MAP-CALIBRATION-R04-ALTERNATE-CELL-PROPOSAL.md。既有原fixture足以验证该首差，无新增原生未知内存输入、无legacyfallback。

全部真实日志、生产源码/DLL/fixture/report哈希与结果见 `analysis/map-calibration/r04-cell-production/final-manifest.json`，原始报告在其work/analysis/calibration-board。manifest限定当前测试源码/消费者身份，依赖以实测DLL身份绑定，不外推全CPU/精定位成功/任意图像。该批通过后全部进程退出，managed槽已释放给主线。

Map显示原资源接入候选仍未应用，失败/等待frame策略与InputROI/Detect成员writer仍按确切原模块路径推进。整工具完成由主线依用户标准统一审计，人工验收单列。
