# 第十三批：纯托管前端XML迁移实施

本工具实现D04.1的已确认分支，不宣称完成整个CompatibleTool。依据同版`analysis/decompiled/CompatibleTool.Audit/CompatibleTool/ConvertHelper.cs::ConvertDesign/ConvertDesignData4_0To_4_4`与`FrontendUIUtils.cs::FrontendUITo_VM4_2Convert/DesignDocFromVM4_3ToVM4_4Convert/ModifyAttributeForOneXmlNode`。原始安装文件/SDK从未执行。

## 真实交付

`src/FrontendDesignMigration`是可执行CLI；读取V4.0 `Root`设计XML，剥离原特定本地化字符串，按显式模板转换为V4.4.0。GroupBox只遍历第一内容层、Tab按原特定子层递归，SubWindow解码其Document再用子模板转换；ImageControl/MultiImageControl/TableControl/PicBox添加原拼写`AutoStrechA=false`；Size替换Width/Height，根高度+60；空Background/BorderBrush改透明；未知wrapper不额外递归。保持原FirstChild dispatch，不擅自跳过XML声明。

独立差异：模板由调用方显式指定；原`ByteUtil.DefaultEncode`定义尚未取得，base64编码须显式指定，不默认声称UTF8；XML reader禁止DTD并限32Mi字符，不加载XAML控件；输出CreateNew拒绝覆盖，区别原File.WriteAllText覆盖；失败转换不创建输出。原模板缺失不伪造为原资源，测试模板明确synthetic。

```powershell
dotnet run --project src/FrontendDesignMigration -- input.xml new-output.xml DefaultDesign.xaml DefaultSubDesign.xaml utf-8
```

最后一个参数仅是调用者选择，不是已恢复的原编码。只支持已确认Root/V4.0→V4.4.0分支；不支持原.sol/.prc/.gro转换、SDK加载、执行或设计面交互。工具不接main菜单，不把格式转换宣称完整运行界面。

## 自审核和保留失败

迁移项目实际构建exit0，0告警0错误。`tests/FrontendDesignMigrationChecks13`首轮因缺System.IO编译失败；补显式using后18/18通过，覆盖GroupBox/Tab/SubWindow、Size/nullbrush、编码、错误base64、原FirstChild/未知wrapper和DTD/不支持版本拒绝。全部输出在`analysis/full-audit/batch13-20261019/D/migration-checks*.log`，合成模板与实际输出位于A/migration-checks。

实际CLI转换exit0；二次同输出调用exit1且原输出SHA不变，见D/cli/summary.json。这是实际转换产物，不是只测试迁移计划。原旧版真实设计样本、原模板与编码、native迁移和runtime差分未验证。因此D04.1由not_started改partial，不算完成分子。

独立源码复审见[A迁移复审](A-frontend-migration-review-batch13.md)。完整原源/新源码与运行日志哈希本地D目录保留。
