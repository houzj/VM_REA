# Group 运行参数关联的工具身份边界

## 原始证据

- `D:/MyWork/REA逆向测试/analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/RunParamConfigDecorate.cs` 的关联命令把来源工具 `StrModuleName`、参数 `Name` 和显示名交给 `ModulesPickData`，勾选后调用 SDK `AddDynamicParamRelation`；同步命令先检查关联目标数。
- `D:/MyWork/REA逆向测试/analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl.UI.Data/ModulesPickData.cs` 的 `Init` 调用 `GetParamsOfEqualTypeTree`。
- `D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser/Apps.XmlParser.ParamTab.VmModuleExtern.decompiled.cs` 的 `GetParamsNode` 在普通工具中按相同 `StrModuleName` 与参数名选取；对嵌套 Group 的代理另以其持久化的原工具身份和参数名匹配。
- `D:/MyWork/REA逆向测试/analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl/DynamicRunParamRelationModulesConfig.xaml` 展示的是复选树，而不是任意同数值类型目标表。

## 本次实现与验证

原窗口只按 `DataKind` 过滤，同为整数的别种工具或同一工具的其他参数均能出现，选择后可能写成与原生工具身份不符的关联。现在 `GroupRunCategories.cs` 的关联候选限于相同工具 ID、版本和参数名，并核对输入数组基数或复杂编辑器的字段与 ROI 能力。版本、字段和 ROI 能力复核是此独立托管模型的防错约束；不能把它说成已还原原生 SDK 对全部插件的身份规则。底层 `WorkflowRunParameters.Validate` 仍独立校验关联数据。

`tests/VisionEditor.GroupSourceChecks` 在实际 Group 双栏配置窗口打开关联复选树：两个相同工具实例仅呈现另一个实例的同名参数，同类型但不同工具的参数及同工具不同名参数均不作为候选。勾选后保存关系，通过 `GraphEditorDocument.SetGroupRunParameterValue` 把值 5 同步到两个真实模块，Group 实际执行输出 `out=5`，保存重开仍保留关联。该专项共 15 项检查通过，原先的来源基数、运行及保存验收也保留。

## 未验证原生行为

原生 SDK 内部 `AddDynamicParamRelation` 的全部错误码、同插件不同版本、嵌套 Group 代理关系和全部复杂编辑器同步尚未差分验证。测试模块只用于框架数据流验收，不冒充原生工具算法。原生关联复选树的像素、展开状态及告警窗仍需真实 VM 对照。
