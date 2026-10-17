# Group 来源选择的类型与基数边界

## 原始证据和适用范围

- `D:/MyWork/REA逆向测试/analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/GroupParam.cs` 的 `AddCommandCmdExecute` 按选中叶 IO 建立 Group 输入/输出，并按叶数量检查 1024 上限。
- `D:/MyWork/REA逆向测试/analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl.UI.Data/RunParamPickDataExtern.cs` 的 `GetRunParamTreeNode` 根据参数类型、可见性和可配置权限构建运行参数树。
- 上述托管代码证实来源和目标有类型筛选；并未证实全部原生复合 IO、图像子类型或数组选择器的最终呈现。本次继续使用项目已定义的 `DataKinds.CanFlow` 作为**独立兼容规则**，不称为原生算法逐语句移植。

## 闭合的实际边界

`GroupInputRunControls.cs` 的 Group 输入来源和运行参数代理来源现在同时检查目标基数。标量目标的弹层不列出数组端口；数组目标也不会混入标量来源。参数来源仍按参数类型精确匹配；节点输入保留已有的受控类型兼容关系。Group 输入来源弹层使用同样的基数过滤。执行器和 `WorkflowSubscriptions.Validate` 的检查仍生效，手工编辑文件不能绕过类型/基数校验。

独立的 `tests/VisionEditor.GroupSourceChecks` 实际打开两个 Group 来源弹层，检查列表，然后选择标量节点输出，运行内部真实模块得到 `out=11`，保存为独立 `.vmx.json`、重开再运行得到 `out=11`，并确认强行写入数组到标量订阅会被运行前验证拒绝。9 项检查通过。现有 Group 编辑和 SelfRun 33 项、完整 WPF 操作 456 项通过；完整 WPF 首次运行因新工作树缺少测试输出目录中断，创建本工作树的 `analysis/full-audit` 后复验通过。

## 未关闭

原生 VM 对复合叶 IO、图像子类型、数组选择器的精确视觉和状态行为还需要真实 VM 同场景差分。多层 Group、全部插件端口及原生 `.sol` 往返不在本次证据范围内。本次检查中的两个简单模块是**框架数据流夹具**，不冒充 VM 工具插件或其算法。
