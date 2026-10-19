# 第十三批 A06.1 关闭流程的真实设计器依赖

基线 `b11594396061226f669f2bfe49d8378f0af27b21`。没有创建占位窗口或修改产品；没有访问 tool-plugins，也没有加载/执行原厂程序集。使用 REA 技能复用同版本身份及既有原CIL，补充工具仅执行现有 dotnet-ildasm 的静态读取。

## 本批新增证据

旧报告的 `analysis/full-audit/frontend-ui-inspect` 路径已迁移。真实保全位置为 `analysis/full-audit/framework-retirement-20261019/framework-shell/analysis/full-audit/frontend-ui-inspect`，其中 `FrontendEdit-simple.il` 与 `EditFrontendWindow-simple.il` 可直接复核。

本批对原件 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/FrontendUI.Design.dll` 再次校验 SHA256：`60c0c7b1f404ad8bab4504b9b928148f84c7694a335331c4e4340b7c8017cdf5`，与旧取证载体一致。原MVID见既有报告 `0de25f5a-f9cd-4617-9954-2ea69f747be1`。定向导出 `DocumentManager` 到 `analysis/full-audit/batch13-20261019/A/DocumentManager.il`，工具退出0；这是独立静态工具材料，不冒称新 REA Evidence。

| 函数 | 原RVA | 本批确认的关闭依赖 |
|---|---|---|
| EditFrontendWindow.OnClosing | 0x7CB8 | IsCodeCreating 时取消；否则 mainEditView.Close(true)，成功后才 BaseWindow.OnClosing、RemoveHandlerVM |
| FrontendEdit.Close(bool) | 0xAF3C | 先 DoSave，再按返回值3取消；成功关闭时解除实际设计面选择、网格、删除、加载事件，释放ToolBox/PropertyGrid/设计数据字典 |
| FrontendEdit.DoSave(bool) | 0xB0B0 | **在 IsSubDesign、NeedSave、确认/取消判断前无条件调用 Save(false,null,true,false)**，不是单纯脏标志询问 |
| FrontendEdit.Save(...) | 0xB158 | 真实DesignContext存在时 SaveDesignerToXamlCode→UTF8→TempDesignerBuffer；save2Solution=true 时再复制到 SolutionDesignerBuffer 并清temp。无DesignContext仅走退化空路径 |
| DocumentManager.NeedSave | 0x1A9F4 | temp MD5空返回false，否则比较 _tmd5 != _smd5 |
| DocumentManager.NeedSaveForDesigner | 0x1AA2C | temp MD5空返回false，否则比较 _tmd5 != _dmd5；两种保存目标不同 |
| DocumentManager.set_SolutionDesignerBuffer | 0x1ABD4 | Dispose旧流→更新_solutionxml和_smd5→写 VmSolution.Instance.UserData["DESIGNERBUFFER"]；流字节经过UTF8GetBytesPadZero；SDK写异常仅记日志并吞掉 |
| FrontendEdit.ResetBuffer | 0xB301 | 清TempDesignerBuffer并将isSolutionDesignerBuffer_Loaded=false；不能简化成清任意IsDirty |

## 独立实施的精确障碍

A06.1 的实际关闭结果取决于可编辑 DesignSurface 产生的XAML快照、temp/solution/designer三份缓冲身份及MD5，以及关闭成功后真实事件/控件的解绑。现main没有这些对象和数据消费者。仅增加窗口、IsBusy和通用确认框只能测试原源码“无DesignContext”退化分支，无法满足具名的真实编辑关闭流程，也不能称其完整实施。

解除条件：先建立可编辑设计文档及可复验的快照输出合同（A06.2的可用设计面子集），落实temp/solution缓冲及保存/拒绝/取消的消费（A06.3的必要子集），再接模态入口、Owner、DisplayMode=1、运行busy与代码生成关闭保护、load/save事件订阅及关闭解绑。完整视觉布局和原厂native .sol编码仍可单列，但真实文档不能用空字符串或固定脏标志代替。

本批不增加实施完成数；入口/生命周期源码不重复计分，新增的DocumentManager消费链用于解除后续真实实施依赖。未进行构建、手验或设备运行。
