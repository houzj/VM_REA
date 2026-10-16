# VisionMaster 独立兼容重建

目标：用户授权的内部测试，独立运行、算法自行实现，最终兼容原 VM 操作、方案、模块、相机与SDK。当前处于主框架基础实现阶段，尚不是完整复刻。

## 当前主体

- VisionData：独立基础数据。
- VisionFramework：方案、节点、连接、ROI、资源、模块契约、持久化、验证和DAG执行引擎。
- VisionModules：首批五种内置模块。
- VisionGraphRunner：无界面的通用模块图执行入口。
- VisionReplica.slnx：统一工程。

架构和具体边界见 docs/architecture.md。计划和状态见 ROADMAP.md。

## 验证主体闭环

双击 Run-Architecture-Demo.cmd，加载 examples/architecture-demo.vmx.json 并执行五节点双链路方案，结果保存到 analysis/architecture-demo-result.json。发布运行器位于 dist/VisionGraphRunner；必须保留整个目录。

```powershell
dotnet build VisionReplica.slnx
dotnet run --project src/VisionGraphRunner -- catalog
dotnet run --project src/VisionGraphRunner -- validate examples/architecture-demo.vmx.json
dotnet run --project src/VisionGraphRunner -- run examples/architecture-demo.vmx.json
```

当前独立格式 *.vmx.json，与原.sol不同；编辑器坐标已保存但通用图编辑器尚未实现。SessionStatus.Success不是检测合格，需检查具体节点输出。

## 测试

- tests/VisionFramework.PersistenceChecks：多流程、保存、ROI和资源。
- tests/VisionFramework.ExecutionChecks：类型/拓扑/错误/取消/超时/生命周期/快照。
- tests/VisionModules.Checks：实际模块行为。
- tests/VisionFramework.IntegrationChecks：真实样例的两条数据链和异常传播。
- 其他tests：已有算法、BMP、参数容器和旧固定计划的回归基线。

## 既有实验及证据

Start-Workbench.cmd 是此前的专用找圆验证界面，不是新主体架构的通用编辑器。SolutionInspector保留为原方案只读分析工具。analysis/phase-2.md、phase-3.md记录历史成果；安装清单、SDK契约、REA证据和兼容矩阵同在analysis。

接下来按主框架路线建设通用节点编辑器与图像/ROI交互；随后扩展运行调试、原方案往返、算法、设备、通信和SDK兼容。完整兼容须逐项与真实VM行为验收。
