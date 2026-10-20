# OpenFileForCalibDialog 路径控件完整门

2026-10-19，根线当前计数2/10=20%；Translation尚未计数。新确认活动路径差异已保留，之前数值/Host通过不等于该分支已实现。

原 `Apps.XmlParser.ParamTab.AlgorithmTabRoot.Init` 577起把 `BaseNode.defaultNameSpace` 临时设为 `Apps.XmlParser.ParamTab` 再 `base.Init(node)`，恢复后通知加载。实际资源标签 `OpenFileForCalibDialog` 对应原 `Apps.XmlParser.ParamTab.OpenFileForCalibDialog : OpenFileDialog, ISubInfo`；父 `OpenFileDialog : NameValueItem` 的 CurValueA 先赋值再同步调用 ValueChangedCmdExecute。不是普通 OpenFileDialog 的加载策略。

原控件源码SHA256 `16B255C28C1CC21E57FFC162E349C5CC77A4B3F0CBF3F206BDEDC97E796D8EE4`；父类 `129D9185E0FBDCCD5146EA57C1CA9033AB1D4D0AAF8EEFE29FCA2B1921C40BC4`。源位置是 REA主目录 analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab。已有NImage专属反编译副本同类型、同完整方法，不以同名API猜测。

| 资源工具/字段 | 原实际标签 | FileOption.FilterName |
|---|---|---|
| Board CalibPathName | OpenFileForCalibDialog | `%.iwcal|*.iwcal%` |
| NPoint CalibPathName | OpenFileForCalibDialog | `%.xml|*.xml%` |
| NImage CalibPathName | OpenFileForCalibDialog | `.xml|*.xml` |
| Map CalibPathName | OpenFileForCalibDialog | `%.xml|*.xml%` |
| Translation CalibPathName | OpenFileForCalibDialog | `%.xml|*.xml%` |
| Rotate FilePath | OpenFileForCalibDialog | `.xml|*.xml;*.iwcal` |
| Map CalibPath0/1 | **OpenFileDialog** | `.xml;.iwcal|*.xml;*.iwcal`，不适用子类校验 |

资源与三个存在的自定义Control.dll元数据只读审计见 `analysis/translation/audit-20261019/path-controls.json`：NPoint、NImage、Translation Control.dll均没有文件控件派生/同全名覆盖；Board/Map/Rotate安装目录无专属Control.dll。Translation控制扩展实际只定义EditButton_Calib及点表，不覆写此文件控件。Distortion由根线独立核的第七资源与控制身份补入其跨工具报告，不在此伪称本线读取过。

ValueChangedCmdExecute 284–345完整顺序：

1. CurValueA为null直接返回；取本地化FilterName，`Split('|')`，按第二段`;`拆patterns，使用 `text.EndsWith(pattern.Substring(1))`，即当前文化且区分大小写。没有通过则错误消息并返回，**不SetParam**。
2. `/root`分支取最后`/`前缀并调用FtpCheckFileName。当前公共本地policy明确Unsupported，不用本地Directory.Exists冒充远端。其它分支只找最后反斜杠`\\`截前缀，若无反斜杠则检查原串；Directory.Exists失败返回。不能代数替换成Path.GetDirectoryName/绝对化。
3. 校验后SetParamUnit失败显示模块错误、成功显示Lang_SetSuc再PreExecuteRun。保留CurValue尝试值与OldValue/模块参数的区别；宿主Apply拒绝事务回滚另列为独立适配。

DoOpenFile本地273–278在 `CurValueA=null; path=(CurValueA=selected)` 之后**无条件**记默认目录；Remote245–250同样。校验失败仅return，不阻止随后history。文件选择同值仍通过null→path强制重试；普通文本同值不变不重试、不写picker history。不能根据尝试值与文档值相等推断已成功，也不能把history条件擅改为只成功才写。

根线拟采用State可选ParameterWriteValidator委托；七provider仅接同一公共helper并保自身通知。helper严格检查实际标签/Filter，不按moduleId硬编码，不拦Loader派生OpenFileDialog4ReadFile，也不拦Map两个base输入。AlgorithmReset直接backend adapter，不经过UI前置门，保持原空默认写入语义。候选与专属测试位于 analysis/translation/proposals/path-policy 和 tests/TranslationCalibration.PathPolicyChecks；尚未验证前不预报通过。

Translation资源还引用 HidePhysicInfoOperation，但其目标 `Tab_Phys Coord Params` 在原树中不存在（实际仅Tab_Calib Params、Tab_Align File及结果TextDisplayGroup）。不据该遗留无目标声明虚构N点物理网格类别。
