# 原厂 Int32 配置元数据对照

运行：`dotnet run --project tests/VisionConfiguration.IntegerMetadataChecks`。

原始 `IMVSCaliperModuAlgorithmTab.xml` 未改写，来源与 SHA-256 在 `Fixtures/sources.json`。实际 PositionNegX0 字段：Cur/Default `0xffffff9c` → -100，Min `0xfffe7960` → -100000，Max `0x186a0` → 100000，Inc `0x1` → 1。

`Fixtures/original-integer-metadata.json` 来自原厂 VM.Util.dll 的 `Hik.Util.StringUtil.ParseInt` 直接托管运行，另反射原厂 Apps.XmlParser.Integer 的五个字段均为 System.Int32。探针源：`tests/NativeEvidence/OriginalConfigurationIntegerMetadataProbe.cs`。原函数使用 Int32 HexNumber；无效/越界输入返回0，不把0xffffffff当4294967295。未使用设备、算法或原厂桌面。

92 项检查覆盖：实际原XML；五字段×12组原厂解析结果；8种源码确认的 Integer 派生类型；非Float/String/Int64的严格范围；EnumerationDataCenter已确认Int32哨兵；生产 ToolConfigurationStandardControls 创建的 ConfigurationNumericSpinner 的真实元数据和值读取、-100上调为-99，以及动态元数据更新。定义树继续保留原始hex，规范化仅发生在独立实例状态中。

截图 `analysis/full-audit/configuration-review/caliper-integer-source-derived.png` 是用实际原厂单字段定义隔离生成的独立配置控件截图，展示初始-100，不是原厂完整窗口或像素等价证据。

复跑原厂解析探针（Windows .NET Framework csc，使用反斜杠路径）：

```powershell
& C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /target:exe /r:System.Core.dll /r:System.Web.Extensions.dll '/out:analysis\full-audit\configuration-review\OriginalConfigurationIntegerMetadataProbe.exe' 'tests\NativeEvidence\OriginalConfigurationIntegerMetadataProbe.cs'
& '.\analysis\full-audit\configuration-review\OriginalConfigurationIntegerMetadataProbe.exe' 'D:/Program Files/VisionMaster4.4.0/Applications/myLibs' 'analysis/full-audit/configuration-review/original-integer-metadata.json'
```
