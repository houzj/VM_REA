# VM 逆向并行执行设计

## 固定边界

所有任务使用当前 `main` worktree。任务不创建分支、不创建新 worktree、不修改现有 `tool-plugins` worktree。只有主任务可以写 `docs/VM-LOCAL-RESTORATION-STATUS.json` 和 C03 证据索引。

## 三条并行证据线

| 线 | 目标 | 只读输入 | 独占输出 | 汇合条件 |
|---|---|---|---|---|
| S 静态入口线 | 恢复 `IMVSBlobFindModu.dll+0x9C80` 的函数边界、调用约定和分支 | 原厂 DLL、已保存反编译、PE/vtable | `analysis/full-audit/.../C03/static-entry-*` | 得到函数边界和至少一条参数流证据 |
| R 运行时差分线 | 验证 ROI、Mask、运行区域图像对 BlobCount/TotalArea 的影响 | 原厂 VM、固定 `1.jpg`、现有 SDK 探针 | `analysis/full-audit/.../C03/runtime-roi-*` | 得到一组可重复的 VM/SDK 差分结果 |
| L 本地契约线 | 准备 ROI/Mask/运行区域对象的本地数据契约 | 已确认 XML、ProcessAlg、ModuleIO 证据 | `analysis/full-audit/.../C03/local-contract-*` | 契约字段和生命周期与 S/R 证据对齐 |

## 主任务汇合顺序

主任务先校验三条线的输入哈希，再合并各自 JSON 和报告。只有同时满足静态入口、运行时差分和本地契约三项证据，才进入实现修改。每次汇合只更新一次状态文件和索引。

## 存储规则

每条线只保留最终 JSON、必要的短日志和哈希。原厂安装文件按路径加 SHA-256 引用。单条线临时目录上限 512 MB，C03 批次新增证据上限 2 GB。临时工程、导入副本、重复截图和缓存必须在汇合后删除。

## 当前批次排程

1. S 线处理 `0x9C80`，先完成 `.pdata` 函数边界，再恢复寄存器参数和调用目标。
2. R 线只做固定图像和固定参数的 ROI/Mask 对照，不重复全 UI 操作。
3. L 线根据 `InputROI`、`InputMask`、`BlindPolygon` 和 `OutputMask` 形成最小字段契约。
4. 主任务合并后决定是否修改本地 Blob 实现。

## 停止条件

某条线连续两次只能产生重复证据时暂停该线，转移到另一条线。遇到缺失运行时状态时记录为未决项，不通过创建新 worktree 或复制原厂目录来绕过。
