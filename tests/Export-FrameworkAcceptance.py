"""Build the portable acceptance report from explicit scope and recorded operations.

Run from the repository root. Capture and reporting stay outside product assemblies.
This does not infer native parity or desktop-input success from test results.
"""
from pathlib import Path
import html
import json
import shutil
import hashlib
import re

root = Path(__file__).resolve().parents[1]
audit = root / 'analysis/full-audit'
docs = root / 'docs'
evidence = docs / 'acceptance-evidence'
evidence.mkdir(exist_ok=True)
data = json.loads((docs / 'framework-acceptance.json').read_text(encoding='utf-8'))
ledger = json.loads((audit / 'wpf-workflow-parity.json').read_text(encoding='utf-8'))
if not ledger.get('operations') or not ledger.get('passed'):
    raise RuntimeError('Missing completed operation ledger; do not publish a summary-only run as current evidence')
lifecycle = audit / data.get('lifecycleReport', 'framework-acceptance-final-lifecycle/exit-results.json')
exit_rows = json.loads(lifecycle.read_text(encoding='utf-8-sig'))
if not isinstance(exit_rows, list) or not exit_rows or any(not r.get('ExitedWithinTimeout') or r.get('ExitCode') != 0 for r in exit_rows):
    raise RuntimeError('Latest monitored run did not complete successfully; retain previous report rather than publishing stale success')
operation_log = (lifecycle.parent / 'WpfChecks-1.out.txt').read_text(encoding='utf-8-sig', errors='replace')
if not re.search(r'PASS ' + str(ledger['checks']) + r' actual WPF checks', operation_log):
    raise RuntimeError('Operation ledger does not match the completed monitored run')
images = [
    ('wpf-workflow-parity.png', '主界面与状态栏'),
    ('wpf-maximized-work-area.png', '最大化工作区'),
    ('wpf-toolbox-expanded.png', '工具箱展开与不可用工具'),
    ('wpf-group-native-dual-pane.png', '修复后的Group双栏配置器'),
    ('wpf-group-tab-return.png', 'Group内部画布与返回'),
    ('wpf-group-input-editing.png', 'Group输入与实际运行结果'),
    ('wpf-workflow-local-variables.png', '局部变量配置'),
    ('wpf-wire-midpoint-preview.png', '连线中点预览'),
    ('wpf-node-group-copy-preview.png', '多节点复制预览'),
    ('roi-display-real-host.png', '实际ROI显示配置'),
    ('roi-fixture-real-host.png', 'Fixture实际掩膜输出'),
]
images += [tuple(item) for item in data.get('extraImages', [])]
files = [name for name, _ in images] + ['wpf-workflow-parity.json',
    'framework-acceptance-real-host.log', 'framework-acceptance-desktop.txt'] + data.get('runtimeLogs', [])
manifest = []
for name in files:
    source = audit / name
    if not source.exists():
        raise FileNotFoundError(source)
    shutil.copyfile(source, evidence / name)
    manifest.append({'file': name, 'sha256': hashlib.sha256(source.read_bytes()).hexdigest()})
if lifecycle.exists():
    shutil.copyfile(lifecycle, evidence / 'exit-results.json')
    data['lifecycle'] = json.loads(lifecycle.read_text(encoding='utf-8-sig'))
release_lifecycle = audit / data.get('releaseLifecycleReport', 'framework-acceptance-release-lifecycle/exit-results.json')
release_rows = json.loads(release_lifecycle.read_text(encoding='utf-8-sig'))
if isinstance(release_rows, dict):
    release_rows = [release_rows]
if not release_rows or any(not r.get('ExitedWithinTimeout') or r.get('ExitCode') != 0 for r in release_rows):
    raise RuntimeError('Release startup/exit verification failed')
shutil.copyfile(release_lifecycle, evidence / 'release-exit-results.json')
data['releaseLifecycle'] = release_rows
data['operationCount'] = len(ledger['operations'])
data['operationRecordedAt'] = ledger.get('recordedAt')
data['evidenceManifest'] = manifest
(docs / 'framework-acceptance.json').write_text(json.dumps(data, ensure_ascii=False, indent=2), encoding='utf-8')
E = html.escape
columns = [('id', '编号'), ('name', '范围'), ('status', '整体状态'),
    ('implemented', '已完成子集'), ('nativeGap', '未完成及原生差异'),
    ('sourceEvidence', '原始依据 / 独立实现边界'), ('entry', '当前实现入口'),
    ('acceptance', '操作及预期'), ('result', '当前验收结论')]
table = ''.join('<tr>' + ''.join('<td>' + E(str(row[key])) +
    ('<br><small>对应窗口操作：' + ', '.join(map(str, row.get('operationNumbers', []))) + '</small>' if key == 'result' and row.get('operationNumbers') else '') +
    '</td>' for key, _ in columns) + '</tr>' for row in data['rows'])
methods = ''.join('<li><b>' + E(key) + '</b>：' + E(value) + '</li>' for key, value in data['verificationMethods'].items())
operations = ''.join('<tr><td>' + str(op['number']) + '</td><td>' + ('通过' if op['passed'] else '失败') + '</td><td>' + E(op['operation']) + '</td></tr>' for op in ledger['operations'])
fixes = ''.join('<li>' + E(f['issue'] + '；' + f['change'] + '；' + f['conclusion']) + '</li>' for f in data['fixes'])
figures = ''.join(f'<figure><a href="acceptance-evidence/{name}"><img loading="lazy" src="acceptance-evidence/{name}" alt="{E(title)}"></a><figcaption>{E(title)}（重建程序渲染，非VM截图）</figcaption></figure>' for name, title in images)
page = '''<!doctype html><html lang="zh-CN"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>框架逆向验收表</title><style>
body{font:15px/1.6 system-ui;margin:24px;color:#243349;background:#f3f5f8}h1{margin-bottom:4px}a{color:#075da0}.notice{background:#fff1d9;border-left:5px solid #d58a10;padding:16px}.scroll{overflow:auto;max-height:72vh}table{border-collapse:collapse;background:white;width:100%}td,th{padding:10px;border:1px solid #d6deea;text-align:left;vertical-align:top}th{background:#23344c;color:white;position:sticky;top:0}#domains td{min-width:140px}#domains td:first-child{min-width:45px}input{padding:12px;margin:14px 0;width:70%}figure{display:inline-block;vertical-align:top;width:46%;margin:1%}img{width:100%;border:1px solid #b5bfce}li{margin:8px 0}summary{cursor:pointer;font-weight:bold}@media(max-width:700px){figure{width:98%}}@media print{.scroll{max-height:none;overflow:visible}th{position:static}input{display:none}}
</style></head><body><h1>框架逆向与实际操作验收表</h1>'''
page += '<p>2026-10-17 · main · 实施基线 ' + E(data['baseline']) + ' + 本次界面修复</p>'
page += '<p>发布入口：' + E(data.get('release', '未发布')) + '；<a href="acceptance-evidence/release-exit-results.json">发布产物启动/退出记录</a>。</p>'
page += '<p class="notice">23个框架域均仍有开放范围。已完成子集不等于整项完成；本轮没有全域原生等价验收。绿色或通过记录只针对已列操作，不能外推未执行项。</p><h2>验收方法及限制</h2><ul>' + methods + '</ul>'
page += '<h2>逐域状态</h2><input id="search" placeholder="搜索编号、功能、差异"><span id="count"></span><div class="scroll"><table id="domains"><thead><tr>' + ''.join('<th>' + title + '</th>' for _, title in columns) + '</tr></thead><tbody>' + table + '</tbody></table></div>'
page += '<h2>发现并修复的问题</h2><ul>' + fixes + '</ul><h2>实际界面目检</h2><p>逐张查看以下渲染图的文字、背景、操作状态、预览及输出；不作为真实VM同状态对照证据。</p>' + figures
page += '<h2>可复查操作记录</h2><p>记录时间：' + E(str(ledger.get('recordedAt'))) + '；共 ' + str(len(ledger['operations'])) + ' 项。部分边界使用测试模块；本轮Group Tab联调使用真实图像源。未选择图像显示的Group示例预览为空，实际整数输出见结果表。</p><ul><li><a href="acceptance-evidence/wpf-workflow-parity.json">完整窗口操作JSON</a></li><li><a href="acceptance-evidence/framework-acceptance-real-host.log">既有真实ROI宿主24项记录（本轮未重跑）</a></li><li><a href="acceptance-evidence/framework-acceptance-desktop.txt">桌面输入阻塞记录</a></li><li><a href="acceptance-evidence/exit-results.json">受控进程退出记录</a></li></ul><details><summary>展开逐项窗口事件操作</summary><div class="scroll"><table><thead><tr><th>序号</th><th>结果</th><th>实际检查操作及结果</th></tr></thead><tbody>' + operations + '</tbody></table></div></details>'
if data.get('closureReport'):
    page += '<p><a href="' + E(data['closureReport'], quote=True) + '">本批次逐功能源码对照及未关闭范围</a></p>'
for name in data.get('runtimeLogs', []):
    page += '<p><a href="acceptance-evidence/' + E(name, quote=True) + '">' + E(name) + '</a></p>'
page += '<p>实际数据/存储复核：<a href="acceptance-evidence/framework-acceptance-arrays.log">28项数组契约记录</a>，<a href="acceptance-evidence/framework-acceptance-persistence.log">23项持久化记录</a>。</p>'
page += '<h2>仍未验收</h2><p>完整桌面鼠标拖放、真实VM同状态视觉和操作差分、多屏DPI、真实设备及长稳、原生.sol往返、完整类型和调度ABI未通过本轮验收。后续逐项满足原方法/资源、界面事件、数据结果、保存重开和释放证据后再关闭。</p><script>const rs=[...document.querySelectorAll("#domains tbody tr")];function filter(){const s=document.querySelector("#search").value.toLowerCase();let n=0;rs.forEach(r=>{r.hidden=!r.textContent.toLowerCase().includes(s);if(!r.hidden)n++});document.querySelector("#count").textContent=n+" / "+rs.length}document.querySelector("#search").addEventListener("input",filter);filter()</script></body></html>'
(docs / 'framework-acceptance.html').write_text(page, encoding='utf-8')
(audit / 'framework-acceptance.html').write_text(page.replace('acceptance-evidence/', '../../docs/acceptance-evidence/').replace('href="' + data.get('closureReport', '') + '"', 'href="../../docs/' + data.get('closureReport', '') + '"'), encoding='utf-8')
markdown = '# 框架逆向与验收清单\n\n更新：2026-10-17。以framework-acceptance.json作为逐项状态源，HTML见[验收表](framework-acceptance.html)。\n\n当前23个领域均有未关闭范围；已完成子集不代表完整原生等价。窗口事件、截图目检、桌面输入及原生差分分别记录。\n\n'
markdown += '|编号|领域|已完成子集|未完成及原生差异|本轮结论|\n|---|---|---|---|---|\n'
for r in data['rows']:
    markdown += '|' + '|'.join(r[k].replace('|','/') for k in ['id','name','implemented','nativeGap','result']) + '|\n'
markdown += '\n## 验证方法\n\n' + '\n'.join('- ' + k + '：' + v for k,v in data['verificationMethods'].items())
markdown += '\n\n## 修复与复验\n\n' + '\n'.join('- ' + f['issue'] + '；' + f['change'] + '；' + f['conclusion'] for f in data['fixes'])
markdown += '\n\n完整原始依据/当前代码入口、操作步骤、未关闭范围、逐项事件记录、实际运行输出及退出证据见HTML和结构化JSON。没有把目录图标、窗口可打开或测试数当作原生验收。截图与报告生成器只在tests；插件工作树未修改。\n'
(docs / 'FRAMEWORK-REVERSE-CHECKLIST.md').write_text(markdown, encoding='utf-8')
print(f'Exported {len(data["rows"])} domains and {len(ledger["operations"])} recorded operations')
