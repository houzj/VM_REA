"""Publish the source/implementation boundaries and manual acceptance steps; no product hooks."""
from pathlib import Path
import html
root=Path(__file__).resolve().parents[1]
report=(root/'docs/COMPOUND-LEAF-MAPPING.md').read_text(encoding='utf-8-sig')
steps='''1. 启动 Start-Flow-Editor.cmd，打开 analysis/full-audit/compound-leaf-roundtrip.vmx.json。
2. 运行父流程，进入 Group；本样例 count=784，areas 两轮共1568项，总面积1710536。父输出 total 读取 renamed.TotalArea=855268。结果仅针对当前独立算法和样例。
3. Group配置→输出设置：检查区域/连通域类型；点击根复合来源右侧链按钮，确认贴附180×180树，叶双击选源；关闭弹层不改变订阅。运行检查数据。
4. 修改Group根输出 renamed 名称，父输出 total 的派生叶路径应同步；删除根输出应解除其叶引用；撤销后重跑。
5. 小地图：Shift滚轮左右横滚；Ctrl滚轮缩放；Ctrl+Shift先横滚再缩放。
6. 保存到新文件，关闭重开并运行，检查数据与引用保持。

待人工验收：真实VM同状态差分、多屏DPI。正文数组索引Popup、全控件模板、Polygon/混合/排除ROI和原生ABI仍未完整复刻。'''
page='<!doctype html><html lang="zh-CN"><meta charset="utf-8"><title>复合输出与小地图人工验收</title><style>body{font:16px/1.7 system-ui;max-width:1100px;margin:32px auto;padding:0 20px;background:#f3f5f8;color:#243349}pre{white-space:pre-wrap;background:white;padding:20px;border:1px solid #ccd5df}a{color:#0869ad}</style><h1>本批人工验收</h1><p><a href="framework-acceptance.html">框架总验收表</a> · <a href="compound-leaf-sources.json">原始证据摘要</a></p><pre>'+html.escape(steps)+'</pre><h2>源码依据、实现及未闭合范围</h2><pre>'+html.escape(report)+'</pre></html>'
(root/'docs/compound-leaf-acceptance.html').write_text(page,encoding='utf-8')
