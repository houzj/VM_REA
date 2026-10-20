2026-10-20 根线已复核路径重开项并通过实施自审；完整计数已纳入6/10=60%。新增审计见TRANSLATION-NPOINT-PATH-FINAL-REVIEW.md；原数值/宿主编译基线与有限边界保留，人工验收独立。

# 平移旋转标定整工具最终自审

2026-10-19。根线当前整体2/10=20%，本工具待根线审核计数；不自行更改总数。人工验收独立记录。

原TranslationCalibModu.dll SHA256 `A85A7CA08EA265B2CF881B4FF01BF7538A60E9EDB7466478476584B08FE25183`。原体位于本工作树analysis/full-audit/translation-calibration/native；缺失callee只读定向导出见analysis/translation/audit-20261019。源码身份见source-manifest.json，输入ABI见同目录INPUT-ABI.md。

| ID | 源→实施 | 实际自动证据/退出结论 |
|---|---|---|
| T01 资源/模式 | 原AlgorithmTab、八项Algorithm默认、Display的path/axis已接入；未创造资源不存在的物理坐标页 | Plugin7469、Host19，已实施 |
| T02 Reset | 原六ISubInfo清绑定、八默认逐项写；四无UI默认走descriptor后端写，失败继续；保点/路径/示教 | Reset42；State1692；已实施，Reset不经UI文件门 |
| T03 数值输入 | 39540→35bd0/35db0两阶段读序、Count比较后cap32；scope优先、失败Count0、真实材料化空数组E314 | Numeric18及真实Graph All/index；已实施 |
| T04 Teach/text | Trigger/TeachFlag各512首项、两读、部分状态；rot0不读世界角而写0 | Text45；已实施 |
| T05 数学 | 原ProcessAlg/MatrixInv/右手坐标fixture保留；本轮不改数学/阈值 | Plugin7469及numeric-differences为空；172个历史floatbit差异仍在既定误差域，未定位SDK逐位差异，不称逐位等价 |
| T06 点表 | 源逐行setter、清空与导入前缀副作用复用已有源映射；公共表入口真实接入 | Plugin7469、Host19；已实施 |
| T07 文件 | 原XML导出/原点副作用/Refresh错误处理；正常OpenFileForCalibDialog按实际Filter与原目录门，不混同backend SetParam | 原26导出fixture包含于Plugin；新PathPolicy30，已实施 |
| T08 显示/输出 | 源ProcessOutput及path/RotAxisPoint展示、状态发布 | Plugin7469、真实Host19；已实施 |
| T09 保存/缓存/释放 | PrepareSave、同scope配置重开、文档重载、RemoveNode重建与Dispose | Host19；已实施，托管生命周期不声称native地址相同 |
| T10 框架输入/路径宿主 | typed float/text、POINT别名、自运行/debug；真实commonprovider文件picker成功/失败通知顺序 | 旧Host19及新PathPolicy30；已实施 |

## 最新路径专项

原控件具体体及资源继承证据见TRANSLATION-PATH-POLICY-SOURCE.md。实际shared Attach依据controlTag启用：CurrentCulture大小写敏感后缀、原始最后反斜杠父目录；失败模型保尝试文本、不SetParam。真实Translation配置窗口验证：成功select→apply→SuccessNotify→run→history；失败select→ErrorNotify→history；失败picker同值仍重试，typed同值不重复；bad→旧accepted重试。AlgorithmReset直写绕过该UI门。

`analysis/b-tpath/logs/build-2.log`为0警告0错误；`run.log` exit0，30项；首build Rotate Manifest.DisplayName编译失败保留于build.log，root修复后复跑。未修改数学或放宽断言。

## 身份与有限边界

六套旧批Numeric18/Text45/Reset42/Host19/Plugin7469/State1692的实际编译快照保留于analysis/b-translation/compiled-source-manifest.json（231源），不拿后来shared更新冒充旧测试基线。新路径批实际PDB源快照为analysis/b-tpath/compiled-source-manifest.json SHA256 `F0B4AEFA7C7DCD8174B398E49D456ABA053D2710EE40D9A3CB1E4308C70A3B27`；日志manifest SHA256 `46D62934C0214FE015D52C35F824AE94863582287E8BCA640EFEAC51EB0BECC7`。

已知独立边界：原Batch ABI由不可变托管输入快照逐index运输，不宣称任意外部可变provider原子性；负Count为托管安全域；Host Apply拒绝回滚与native错误保CurValue分开；远程/root校验明确Unsupported，不调用远端。Global/Local/父Group由合同提供者承担；没有新增已证本工具正常本地UI活动缺口。原受控数值与文件fixture不是设备/原厂完整桌面验收。当前选定本地工具范围逆向+实施+自动自审可交根线最终审计。

