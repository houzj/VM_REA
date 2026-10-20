# 映射标定 CalibOriPoint* 显示字段源码审计

## 结论

原厂结果显示 XML 引用了 `CalibOriPointX/Y` 与四个轴向量分量，但当前检查到的 Map 原厂模块契约、原生 `ProcessOutput` 生产函数和托管 `MapCalibResult` 结果装配类型中均没有这些字段的声明/写入路径。原生模块 DLL 与托管 DLL 的 ASCII、UTF-16LE 字符串扫描也没有命中这些字段名。因而，现有静态证据不支持它们通过 Map 模块常规输出被生产。

这仍不足以证明显示对象必然是遗留项或永远不可见：尚未取得原厂 UI 的运行结果面板/坐标对象截图和该模块在 VM 壳层中的完整动态输出 trace；也不能排除未检查的外部/通用坐标接口在某个运行模式中提供值。没有为这些字段伪造输出值，产品实现保持不变。

## 源码和资源证据

| 证据 | 可复核内容 | SHA-256 |
|---|---|---|
| 原厂 `IMVSMapCalibModu.xml` | Output 列表在 `ModuRunTime` 后声明 `ModuStatus`、`CalibStatus`、比率、误差、三组角点、计数、ROI、状态及像素比例；无 `CalibOriPoint*` | `228BF45A24691C0CD13C2201E516941F20E6B1E5352478CCB60ED01C774B4F5F` |
| 原厂 `IMVSMapCalibModuDisplay.xml` | 第 53–59 行将 `CenterX/Y`、`CoorDX/DYVecX/Y` 和提示文本绑定到这组 `CalibOriPoint*` 名称；这是显示侧引用证据 | `D630451CD94D6D6EB4B1A69F5B9204707B18B083799B6218E4493A84700CA0A4` |
| 原厂 `IMVSMapCalibModu.dll` | Ghidra 反编译 `CIMVSMapCalibModu::ProcessOutput` 位于 `analysis/map-calibration/ghidra/module-analyzed/1800220b0.c`。第 44–45 行写 `ModuStatus`、`CalibStatus`；第 110–190 行写 X/YRatio、MapError、三组角点及计数、CalibStatus1/2、PixelScale1/2；函数中没有 origin 字段写出。原生 DLL 中未发现目标字段的 ASCII 或 UTF-16LE 字符串 | `658D63E2C4E2378E7EB1776D27B966990C0D0E01E29CA57ED82FE9ACC740FDE8` |
| `ProcessOutput` 反编译产物 | 上述函数来自本工作树 Ghidra `module-analyzed` 输出，便于定位检查行；它是反编译结果，不是原厂源码 | `12F085ABAC1773CD4D3D93A7B158FDFE527317B3A0E0FFCDF9C177CE344DC9F4` |
| 原厂 `IMVSMapCalibModuCs.dll` 的 IL | 对 `MapCalibResult` 类型作定向 IL 导出：字段/构造器只含状态、比率、误差、角点列表/计数、ROI、像素比例，没有 origin/axis properties。`IMVSMapCalibModuTool.ModuResult` 创建此类型，再 `QueryModuleOneResult`；有结果时交给 `ModuleResultManager.SetModuleResult`。安装目录与 Development 目录中的此 DLL 字节一致。该 DLL 中目标字段名也未命中 ASCII 或 UTF-16LE 扫描 | `6C8EA8FD06A2FA0D9E2C2F9A2F4B23D735E0D4FCB4D838721592B401B1C429F6` |
| SDK `ModuleResultManager.cs` | 第 1407–1413 行的一般点映射把 `PosXVector`、`PosYVector` 分量映射为 `CalibOriPointXVecX/Y`、`CalibOriPointYVecX/Y`。这是可疑的通用类型映射候选，但不是 Map 字段值生产证据：Map 的公开结果类没有这些源属性，Map 原生结果输出函数也未写这些字段。该映射没有 `CalibOriPointX/Y` 原点本身 | `FD3960EE308FB739729AF43589035D628544BB5B26941BE5EB6F3250A41739E7` |

## 运行 trace 与未知

对当前 `analysis/map-calibration` 下已保存的文本证据检索这六个确切输出名，没有发现 Map 原厂 runtime 记录其值；现存算法 probe 不是原厂桌面动态输出/显示链的完整 trace，所以不能据此断言其运行时必定为空。

要判定显示对象运行时是否会取值，最小证据是同一原厂 Map 节点在成功双图运行后保存的：

1. 原厂结果/显示面板中 Coordinate System 对象的可见状态和对应数值；
2. VM 壳层实际返回的 Map 动态输出名、类型和值（包括六个目标名，即使值缺省也需记录）；
3. 若这些动态输出由订阅的点/向量对象产生，记录实际对象类型和绑定/写值调用链，特别是 SDK `PosXVector`/`PosYVector` 到模块输出的传递点。

在得到这些证据前，保留显示 XML 引用与当前产品实现，不增加目标输出端口、不填充默认值，也不将显示元素标成“遗留/无效”。本报告仅登记源证据边界，没有修改共享框架、实现代码或测试。
