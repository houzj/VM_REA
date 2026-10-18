# Group 循环间隔控件闭合记录

原始证据：`analysis/full-audit/subprogram-evidence/IMVSGroupAlgorithmTab.xml` 的 `LoopTimeGap` 是 `Integer`，取值 0～10000、步长 1、`SubscribeVisible=false`；`analysis/decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml` 的 `Integer_Sub` 使用 `PCListView:SliderArrange`，滑杆值双向绑定 `CurValueA`，值变更调用 `ValueChangedCommand`。`LoopStartCount` 和 `LoopEndCount` 则是前置 int 订阅的 `ButtonSelecter`，本轮没有把它们臆改为数字编辑框。

独立 WPF 实现：Group 配置的循环间隔由纯文本框改为整数滑杆与同步数值框；拖动完成、轨道点击或键盘失焦时写入 `LoopTimeGap`，数值框失焦时验证 0～10000。为避免拖动中每格重建整个配置 Tab，拖动过程仅同步显示，操作结束后提交。此事件时机是独立实现，尚未证明与原生 `SliderArrange` 的即时事件完全一致。

专项验收：`tests/VisionEditor.GroupLoopGapChecks` 真实 WPF 窗口操作通过 7 项：界限/步长、滑杆显示、参数提交、撤销/重做、保存重开；实际 Group 内工具消费 LoopIndex，设置 500 ms 间隔与 150 ms 超时后只执行索引 0，下一轮前状态为 TimedOut。原生真实 VM 的同状态截图、拖动连续值回调和视觉尺寸尚未差分验收。
