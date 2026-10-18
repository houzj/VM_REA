# MF-01 打开方案命令验收

## 原始证据

- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `OpenSolution` 先调用 `StopRun`，再调用 `ProjectManager.LoadSolution`；`StopRun` 仅将 `ContinuousRunEnable` 置为 `false`，不取消已经执行的一轮。
- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs` 的 `LoadSolution` 从 `OpenSol` 读取选择器初始目录，选择后先保存所选文件目录，然后进入方案加载。

## 独立 WPF 实现

- 文件菜单和工具栏“打开方案”共用一条命令；进入选择器前停止连续运行，取消选择不修改当前方案和目录记忆。
- 选择路径后记住目录；当前单次运行尚未结束时等待其结束，不在运行中替换独立执行器的文档。随后才确认待保存修改、加载方案并刷新最近记录。
- 选择器通过 `ISolutionFilePicker` 隔离，专项测试实际点击 WPF 菜单并注入可观察选择器，验证先停连续运行、未取消单次运行、取消不变、运行中保护、成功加载、目录记忆及最近记录。

## 未验证原生差异

- 原版 WinForms `.sol` 文件选择器的像素样式和方案数据格式未做一致性声明；当前工程选择 `.vmx.json`/`.json`。
- 原版对未保存修改的提示由 SDK 流程执行；当前独立编辑器在路径选择及运行状态检查后调用自己的待保存确认。原版活动单次运行期间是否阻止更换方案尚未动态核实，本实现采用文档安全保护。
