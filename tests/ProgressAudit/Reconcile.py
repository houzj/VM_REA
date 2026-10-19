"""Validate and render the versioned framework progress evidence matrix.

No reverse target, test application, external process or other worktree is run.
Statuses are reviewed judgments in JSON, never inferred from keyword counts.
"""
import collections
import csv
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOC = ROOT / "docs"
AUDIT = ROOT / "analysis/full-audit/progress-reconciliation-20261019"
STAGES = ("source", "implementation", "validation")
LABELS = {"source": "源码", "implementation": "实现", "validation": "验证"}
STATUS = {"done": "达成", "partial": "部分", "blocked": "阻塞",
          "not_started": "未取得交付", "unverified": "未验证", "failed": "失败"}


def main():
    AUDIT.mkdir(parents=True, exist_ok=True)
    data = json.loads((DOC / "framework-progress-audit.json").read_text(encoding="utf-8"))
    plan = json.loads((DOC / "framework-reverse-plan.json").read_text(encoding="utf-8"))
    packages = data["packages"]
    assert len(packages) == 32
    assert {p["taskId"] for p in packages} == {t["id"] for t in plan["tasks"]}
    assert all(len(p["units"]) == 3 for p in packages)
    units = [u for p in packages for u in p["units"]]
    assert len(units) == len({u["id"] for u in units}) == 96
    assert {x for p in packages for x in p["domains"]} == {f"F-{i:02}" for i in range(1, 24)}
    for unit in units:
        assert unit["criterion"] and unit["limitation"]
        assert all(unit[s] in STATUS for s in STAGES)
        if any(unit[s] in ("blocked", "failed") for s in STAGES):
            assert unit["blockerIds"], unit["id"]
    known_units = {u["id"] for u in units}
    for blocker in data["blockers"]:
        assert set(blocker["units"]) <= known_units
        assert blocker["unblockCondition"]
        assert all((ROOT / ref).is_file() for ref in blocker["evidence"])
    counts = {s: dict(collections.Counter(u[s] for u in units)) for s in STAGES}
    done = {s: counts[s].get("done", 0) for s in STAGES}
    complete = sum(all(u[s] == "done" for s in STAGES) for u in units)
    for package in packages:
        planned = next(t for t in plan["tasks"] if t["id"] == package["taskId"])
        for stage in STAGES:
            assert planned["reconciledProgress"][stage]["done"] == sum(
                u[stage] == "done" for u in package["units"])
    for stage in STAGES:
        assert plan["latestReconciliation"][stage]["done"] == done[stage]
    assert plan["latestReconciliation"]["allThree"]["done"] == complete
    summary = {"denominatorVersion": data["denominatorVersion"], "units": 96,
               "packages": 32, "domains": 23, "baseline": data["baseline"],
               "layers": {s: {"done": done[s], "total": 96,
                               "percent": round(done[s] / 96 * 100, 1),
                               "remaining": 96 - done[s], "statuses": counts[s]} for s in STAGES},
               "allThreeDone": complete, "allThreeRemaining": 96 - complete,
               "allThreePercent": round(complete / 96 * 100, 1),
               "closedPackages": sum(t["status"] == "closed" for t in plan["tasks"]),
               "newProductTestsRun": 0,
               "overallCompletionPercent": round(done['implementation'] / 96 * 100, 1),
               "overallDone": done['implementation'],
               "overallRemaining": 96 - done['implementation'],
               "overallPolicy": "implementation-self-review-v1",
               "meaning": "用户指定：只按实施完成并自审核的单元统计总进度；人工验收不参与，源码/验证及三层交集仅保留为证据维度"}
    data["summary"] = summary
    (DOC / "framework-progress-audit.json").write_text(
        json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

    lines = ["# 框架逆向成果全面对账", "", f"2026-10-19；冻结基线 `{data['baseline']}`。仅 main；tool-plugins 未访问、未计入。", "",
             f"**当前实施总进度：{done['implementation']}/96 = {summary['overallCompletionPercent']}%。**", "",
             "用户最新要求：实施完成并经自审核即计完成；人工验收安排在所有实施完成之后，不作为进度门槛。以下源码/验证状态及历史三层交集只保留审计用途，不参与总进度计算。", "",
             "这是本次逐项证据对账的结果，不是重跑测试后的原机等价结论。以前0/32只统计新计划的整包最终关闭，未回填早已合入main的子集成果；因此它不能代表当前整体逆向进度。此次修正统计，不将历史成果说成本轮新增。", "",
             "| 层次 | 严格达成 | 进度 | 尚未达成 |", "|---|---:|---:|---:|"]
    for s in STAGES:
        q = summary["layers"][s]
        lines.append(f"| {LABELS[s]} | {q['done']}/96 | **{q['percent']}%** | {q['remaining']} |")
    lines += ["", "## 分母与计数规则", "",
              "总计划32包，每包3个具名验收单元，共96项；每项分别审核源码、实现、验证，因此每层分母均为96。下表列明全部单元，不按文件/方法/测试数量计权，不给部分完成折半。", "",
              "总进度仅计算implementation=done的单元/96；源码取证数量、人工验收及历史三层交集均不参与。实施须有明确交付及自审核依据；部分实施不折半。统计口径修正不计本批新增实施；D08仍为部分。", "",
              "实现包括当前main的明确独立重构及计划要求的可执行逆向辅助工具，不能直接称VM等价。验证为有证据的已列操作范围；有已知反例则记失败。原方法合成输入、离屏WPF、原机差分分别保留边界。", "",
              "历史验证未在本次重新执行；最近产品源码改动为6a2cf54的画布方案替换，后续批次主要是报告与探针。旧的framework-acceptance.json基线较旧，以专题报告和后续失败更正，不覆盖旧记录。", "",
              "该分母是总计划的交付单元，不是原程序所有入口/函数的穷举，也不是工时预测。未来发现新范围须升级分母版本并保留前后差异，不能临时拆小已完成项抬高进度。", "",
              f"完整包仍为{summary['closedPackages']}/32关闭：这是最终收口门槛，不再作为唯一整体逆向进度。真实桌面、native ABI、设备和原生容器最终等价尚未通过。", "",
              "## 32包汇总", "", "| 包 | 范围 | 源码/3 | 实现/3 | 验证/3 | 三层同达成/3 |",
              "|---|---|---:|---:|---:|---:|"]
    for p in packages:
        c = [sum(u[s] == "done" for u in p["units"]) for s in STAGES]
        n = sum(all(u[s] == "done" for s in STAGES) for u in p["units"])
        lines.append(f"| {p['taskId']} | {p['title']} | {c[0]} | {c[1]} | {c[2]} | {n} |")
    lines += ["", "## 已知失败与主要剩余工作", "",
              "- **D02.2：FCR-024资产Undo失败**。跨目录另存后Undo恢复插件相对路径却保留新基目录。需要纯资产重定位合同及用户选择的跨项目交接，不能硬编码插件私有字段。", 
              "- **A08.2：Shell默认键鼠/弹层验收失败**。控件事件模式通过不覆盖它；真实输入桌面/DPI矩阵仍缺。",
              "- **A03/B01/C01/C02/C03/C04**：受控原实例、原日志装载/watch/output、真实参数捕获/回调/结果消费和释放。局部注册表映射已证明，但完整隔离未证明。",
              "- **D01/D03/D04/D05**：native provider、writer/reader、mdata语义、旧版本/密码样本和原厂保存重开往返。只读外层解析不能当导入器。",
              "- **A06/C06/B05**：运行界面编辑、全局变量/通信/触发/脚本服务、gvar等产品能力未交付。",
              "- **B04/B06/B07/D07**：全部原生类型/ABI、完整候选/ROI/专用模板、新插件公共框架交接。FCR-015/018/019/021/022/024仍有未闭合范围；不自动采用插件线来信。",
              "- **C07/D06/D07**：长稳/并发资源基线、干净机器和全依赖部署矩阵未完成。", "",
              "## 全部96项逐项证据", "",
              "达成/部分/阻塞/未取得交付/未验证/失败分别保留。每包的证据是已有报告及其原成员/指令/运行记录，代码位置是当前main的审查对象；路径存在本身不构成通过。"]
    for p in packages:
        lines += ["", f"### {p['taskId']} {p['title']}", "",
                  "前置：" + ("、".join(p["planDependencies"]) or "无") + "；完整包未因本表计分关闭。", "",
                  "证据：" + "、".join(f"[{Path(x).name}]({Path(x).relative_to('docs').as_posix()})" for x in p["sourceAndOperationReports"]), "",
                  "当前实现/工具位置：" + ("、".join(f"`{x}`" for x in p["implementationArtifacts"]) or "未取得对应产品实现证据"), "",
                  "| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |", "|---|---|---|---|---|---|"]
        for u in p["units"]:
            lines.append(f"| {u['id']} | {u['criterion']} | {STATUS[u['source']]} | {STATUS[u['implementation']]} | {STATUS[u['validation']]} | {u['limitation']} |")
    lines += ["", "## 精确阻塞与解除条件", "",
              "| 编号 | 类型/涉及单元 | 解除条件 |", "|---|---|---|"]
    for blocker in data["blockers"]:
        lines.append(f"| {blocker['id']} | {blocker['kind']}：{', '.join(blocker['units'])} | {blocker['unblockCondition']} |")
    lines += ["", "## 复算与审计", "",
              "源数据：[framework-progress-audit.json](framework-progress-audit.json)。命令：`python tests/ProgressAudit/Reconcile.py`（本机使用明确的bundled Python路径）。", "",
              "脚本验证32包/23域/96单元、状态、分母、报告/代码引用，复算百分比与交集，保留证据哈希和摘要。它不从关键词自动判定完成，不运行产品，不访问其他工作树。", "",
              "本次证据快照、排除旧计分口径的原因和检查结果位于 `analysis/full-audit/progress-reconciliation-20261019/`。不存在以当前审计时间冒充历史运行时间的记录。"]
    (DOC / "FRAMEWORK-PROGRESS-RECONCILIATION.md").write_text("\n".join(lines) + "\n", encoding="utf-8")

    refs = sorted({x for p in packages for x in p["sourceAndOperationReports"] + p["implementationArtifacts"]})
    snapshots = []
    for ref in refs:
        path = ROOT / ref
        assert path.is_file(), ref
        raw = path.read_bytes()
        text = raw.decode("utf-8-sig")
        assert "\ufffd" not in text, ref
        snippets = [{"line": i, "text": line[:320]} for i, line in enumerate(text.splitlines(), 1)
                    if any(term in line for term in ("analysis/decompiled", "::", "通过", "exit", "失败", "未实现", "未知"))][:6]
        snapshots.append({"path": ref, "sha256": hashlib.sha256(raw).hexdigest(), "bytes": len(raw), "excerpts": snippets})
    (AUDIT / "evidence-index.json").write_text(json.dumps(snapshots, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    (AUDIT / "audit-checks.json").write_text(json.dumps(summary, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    with (AUDIT / "remaining-units.csv").open("w", newline="", encoding="utf-8-sig") as f:
        writer = csv.writer(f)
        writer.writerow(["unit", "criterion", *STAGES, "remaining_or_boundary"])
        for u in units:
            if not all(u[s] == "done" for s in STAGES):
                writer.writerow([u["id"], u["criterion"], *[u[s] for s in STAGES], u["limitation"]])
    print(json.dumps(summary, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
