# 第十五批：原UI缩放字段只读语义与关系映射接入

根线唯一修改`SolutionInspector/Program.cs`、`UiParameters.cs`，接入B线`SemanticMap.Build`实际CLI输出。真实CircleFind静态读取成功，新增声明图含2模块/1流程/1条显式连线，完整样本SHA在D/circle-inspection-summary.json核对读取前内置hash与读取后hash一致。UI候选不强合并XML Guid与UserData Guid，八段订阅raw原样保留unknown；不声明可导入、执行或原格式往返。

新增`ProceZoomIndex`解码依据：`VMControls.WPF.DataForUI.VmModuleSetDecorator.ProceZoomIndex`读取所属UserData后int.Parse；`VM.PlatformSDKCS.StringUtil.UTF8GetString`按UTF8取字符串并TrimEnd NUL；setter用UTF8GetBytesPadZero写入。原getter对缺键缓存default9，对损坏已存字节则parse，不得吞错填9；原setter不限制0..19，UI档位范围另属控件。

检查器保留rawBase64，新增decoded状态：UTF8整数、无效整数、严格UTF8无效。整数使用InvariantCulture只读解释（原int.Parse为当前文化），不夹紧0..19；负数/20仍保留整数。严格UTF8对非法字节不作原Encoding.UTF8 replacement fallback，此为明确的检查器诊断差异，不冒称原错误分支。未知键保持opaque，不推测Position以外任意二进制结构。

`SolutionUiZoomChecks15`20/20通过；`SemanticMapChecks15`15项通过；原SolutionInspector.Checks19项回归含真实CircleFind通过。样本原件未修改，未加载XAML/厂商DLL或SDK。D03.1由未实施改partial；native Relation含义、隐式owner、UI filename真实reader和完整身份链仍未知，不因检查器增强计done。
