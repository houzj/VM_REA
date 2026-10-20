# R04 A / 整链下一独立验证命令

从worktree根 `D:\MyWork\REA逆向测试\worktrees\tool-plugins` 运行。两入口无命令行入参，读取既有原始fixture/trace；必须串行并先获得根线managed槽。当前未构建/未执行A。

```powershell
dotnet build analysis/map-calibration/proposals/r04-source-recognition/R04OpeningChecks.csproj --artifacts-path analysis/map-calibration/r04-a-validation/artifacts --ignore-failed-sources -m:1 -p:UseSharedCompilation=false -v:minimal
dotnet analysis/map-calibration/r04-a-validation/artifacts/bin/R04OpeningChecks/debug/R04OpeningChecks.dll

dotnet build analysis/map-calibration/proposals/r04-source-recognition/R04SourceBoardChecks.csproj --artifacts-path analysis/map-calibration/r04-a-validation/artifacts --ignore-failed-sources -m:1 -p:UseSharedCompilation=false -v:minimal
dotnet analysis/map-calibration/r04-a-validation/artifacts/bin/R04SourceBoardChecks/debug/R04SourceBoardChecks.dll
```

不要用`dotnet run`隐含构建跳过槽；构建成功后运行DLL，失败时保存原退出码。根线可将stdout/stderr分别重定向到r04-a-validation/<project>-build.log和<project>-run.log。上面引用生产Calibration项目与生产R03源码，不应用A/B共享补丁也可独立编译候选。

## 输入与CPU

两Program都读取 `analysis/map-calibration/r04-opening-oracle/probe.jsonl` 首行原厂CPU记录并要求 **cpu==11**。Opening原先直接传Cpu11的测试缺口已修正为先核record，再选择该有限分派；SourceBoard也先核CPU后才跑检测。未检查HostAvx或按托管ISA名称猜原class。

原灰图`image-input/native-source.gray`，原A定点proposal在OpeningProgram为已捕获的sequence1八个float；原B payload取oracle首行bytes，不再由测试自行补值。原200×200warp、四100×100 crop/scoreMap来自r04-opening-oracle。整链期望从image-anchor-caller-trace/tag-trace.jsonl的sequence1/map-return读取全部168点。

## 输出和不放宽的失败界限

- Opening输出 `analysis/map-calibration/r04-opening-managed/comparison.json`，并保存normalized.gray。要求原refinement0分支一致（候选Refined必须false）、全部40000 normalized byte一致、原四crop的共40000 score int一致、全部12 payload byte一致；任一差异/显式unsupported/无Sample均失败，不能只12byte碰巧相同便PASS。原crop scoreMap比较隔离前定位；自身normcrop的prelocation另存，能定位fit/warp首分叉。没有容差、无legacyfallback，出现source FitNativePoints复用数值差异就记录并继续修该分叉。
- SourceBoard输出 `analysis/map-calibration/r04-source-board-managed/comparison.json`。必须有SourceGrid→proposal→A→B成功，全部168点数量/顺序/ID和XY floatbits精确相同；unsupported、prefix未知、fit失败、少/多点或任何floatbit差异均失败。没有运行独立Find fallback或8向mapper微核替代真实图像链。
- 若底层异常发生在比较JSON写出之前，run log/退出码为失败证据，不能宣称没有JSON即没有差异。两个候选均有明确Source CPU/axis/prefix安全边界，任一在现有fixture触发便算本次入口未通过，不能为了通过测试改成default opening或放宽预期。
- 通过仅闭合这幅既有CPU11实际caller和逐操作检查，不证明任意图像/全部CPU/非正常浮点环境，更不能覆盖尚未接产品的SourceGrid-only adapter门和生产CPU策略。
