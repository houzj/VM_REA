# 第十六轮根线复核：标定最后三项未测的显示边

本轮只补标定十工具中 N 图像、相机映射、标定加载的特有结果/失败显示；共用主窗 `RefreshToolDisplay` 链沿第十二至十四轮证据只引用一次。三项原版资源/`Process` 写叶与当前显示分别核，不因离屏构建测试通过而称原厂失败画面等价。`tool-plugins` 在制文件未动。

1. [N 图像标定待完成→空白帧失败→新批次恢复→切选](20261020-pass16-nimage-display-recovery.md) SHA-256 `A23D18AAD3BF90A15152B75D2A116953DE6ADFD694C14EF0CC40754A93F5C540`。实际 ImageSource 图加数值订阅，前八帧保当前图/零点图形，第九空白帧返回 `0x10103000` 且记录为本轮诊断，下一有效帧重新累计，完成后出现新点图形，切源清三 presenter。B 线 10/10，根线用原构建独立复跑 exit 0、10/10；独占 `analysis/joint-audit/pass16-cal-nimage-display/root-rerun/summary.json` SHA-256 `09A3B773C13F3D975AEC7605DFA0BBAF785A4AB461C10BDD82CC29C3C01E43C0`。原版同条件失败图层和勾选态未知。
2. [标定加载文件刷新→动态输出/Group→Data Record](20261020-pass16-readcalib-display-chain.md) SHA-256 `AA2C69991C1DF2BD102D72E951791EB5BA8C86ECB07162FFAAC6D2B8F72D5C33`。有效、畸形保留、合法无输出、改名、恢复分别对上当前 Group 和状态记录；C 线 12/12，根线在独立工作目录以 `--no-build` 复跑 exit 0、12/12，未重写 C 线夹具。额外有限值安全输入 NaN 使当前 loader/Group 均失败，但 `LastDisplay` 回上一轮 `Module Status:1`；主窗对失败记录仍询问帧，故旧成功记录回灌风险登记 [FCR-036](../CONTRACT-REQUESTS.md)。原 native NaN、原同图失败显示及动态 Group 端口自动迁移仍未知，不能无据规定原版应清空什么。
3. [相机映射四对点数组→文本结果→数组失配→恢复/切选](20261020-pass16-camera-mapping-display.md) SHA-256 `DFBC0747986F7DB955262A2A959984B17770B93AEE64F2F1A0FA5E1CC769DD4A`。根线实际 WPF 主窗 7/7，成功显示状态/九列而无虚构图层；数组长度失配失败时清记录/列表，改回后恢复，切节点无旧帧。`analysis/joint-audit/pass16-cal-camera-display/run/summary.json` SHA-256 `B2053E98F8D1863681FBC0A4D8E92BE56D2FE80D3BBD0A92F7E2CB7BE94AD1EA`。原 `Process` 的输入/算法早退与 `ProcessOutput` 正常写值分别有源，原版失败时 WPF 是否仍保留上次结果未证。

本轮补齐了十工具中此前未实测的这三种**当前具名显示边**，不是十工具所有 J01–J12 全链已关；多参数、取消、原厂同输入画面/数值、人工桌面验收等依各表精确保留。原版 `Display.xml` 是映射依据而不是失败后的宿主刷新规格，当前一个明显反例即 ReadCalib 的有限值早退旧记录。主框架实施完成率仍 **41/96 = 42.71%**；插件历史 10/10 自审另列。本轮无产品修复、不推送，现存用户工作区改动不纳入提交。
