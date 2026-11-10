/* 逆向计划站点 · 共享交互。无网络请求、无外部依赖；脚本失效时页面仍可完整阅读。 */
/*__PLAN_SEARCH_INDEX__*/
(function () {
  'use strict';

  var index = window.PLAN_SEARCH_INDEX || [];
  var $ = function (selector, scope) { return (scope || document).querySelector(selector); };
  var $$ = function (selector, scope) { return Array.prototype.slice.call((scope || document).querySelectorAll(selector)); };

  function element(tag, text, className) {
    var node = document.createElement(tag);
    if (text !== undefined && text !== null) node.textContent = String(text);
    if (className) node.className = className;
    return node;
  }

  /* ---- 跨页搜索：数据内联在 plan.js，不 fetch，file:// 直接可用 ---- */
  function initSearch() {
    var bar = $('#plan-search-bar');
    var input = $('#plan-search-input');
    if (!bar || !input || !index.length) return;
    bar.hidden = false;
    var list = element('ul', null, 'search-results');
    list.id = 'plan-search-results';
    list.setAttribute('role', 'listbox');
    list.setAttribute('aria-label', '跨页搜索结果');
    list.hidden = true;
    bar.appendChild(list);

    var active = -1;
    var current = [];

    function close() { list.hidden = true; active = -1; }
    function target(item) { return item.p + (item.h || ''); }

    function render(query) {
      var needle = query.trim().toLowerCase();
      list.replaceChildren();
      active = -1;
      if (!needle) { close(); return; }
      current = index.filter(function (item) {
        return (item.t + ' ' + (item.k || '')).toLowerCase().indexOf(needle) !== -1;
      }).slice(0, 40);
      if (!current.length) {
        var empty = element('li', '无匹配（可尝试工具名、单元 ID、阶段或层名）', 'search-empty');
        list.appendChild(empty);
      } else {
        current.forEach(function (item, i) {
          var li = element('li');
          li.setAttribute('role', 'option');
          var link = element('a');
          link.href = target(item);
          link.dataset.resultIndex = String(i);
          link.appendChild(element('span', item.t));
          link.appendChild(element('small', item.p + (item.h || '')));
          li.appendChild(link);
          list.appendChild(li);
        });
      }
      list.hidden = false;
    }

    function move(step) {
      if (!current.length) return;
      active = (active + step + current.length) % current.length;
      $$('#plan-search-results a').forEach(function (link, i) {
        link.classList.toggle('active', i === active);
      });
      var selected = $$('#plan-search-results a')[active];
      if (selected && selected.scrollIntoView) selected.scrollIntoView({ block: 'nearest' });
    }

    input.addEventListener('input', function () { render(input.value); });
    input.addEventListener('keydown', function (event) {
      if (event.key === 'ArrowDown') { event.preventDefault(); move(1); }
      else if (event.key === 'ArrowUp') { event.preventDefault(); move(-1); }
      else if (event.key === 'Enter') {
        var link = $$('#plan-search-results a')[active];
        if (link) { event.preventDefault(); window.location.href = link.href; }
      } else if (event.key === 'Escape') { close(); input.blur(); }
    });
    document.addEventListener('click', function (event) {
      if (!bar.contains(event.target)) close();
    });
  }

  /* ---- 表格列头排序（纯前端，不改数据） ---- */
  function initSorting() {
    $$('th[data-sort]').forEach(function (header) {
      header.setAttribute('tabindex', '0');
      header.setAttribute('aria-label', header.textContent.trim() + '，点击排序');
      function sortTable() {
        var table = header.closest('table');
        var body = table && table.tBodies[0];
        if (!body) return;
        var column = Array.prototype.indexOf.call(header.parentNode.children, header);
        var numeric = header.dataset.sort === 'num';
        var rows = $$('tr', body);
        var ascending = header.dataset.direction !== 'asc';
        rows.sort(function (a, b) {
          var av = (a.children[column] || {}).textContent || '';
          var bv = (b.children[column] || {}).textContent || '';
          if (numeric) {
            var an = parseFloat(av.replace(/[^\d.\-]/g, '')) || 0;
            var bn = parseFloat(bv.replace(/[^\d.\-]/g, '')) || 0;
            return ascending ? an - bn : bn - an;
          }
          return ascending ? av.localeCompare(bv, 'zh-CN') : bv.localeCompare(av, 'zh-CN');
        });
        header.dataset.direction = ascending ? 'asc' : 'desc';
        rows.forEach(function (row) { body.appendChild(row); });
      }
      header.addEventListener('click', sortTable);
      header.addEventListener('keydown', function (event) {
        if (event.key === 'Enter' || event.key === ' ') { event.preventDefault(); sortTable(); }
      });
    });
  }

  /* ---- 阶段手风琴：同一时刻只展开一个 ---- */
  function initAccordion() {
    $$('.accordion').forEach(function (group) {
      var stages = $$('details.stage', group);
      stages.forEach(function (stage) {
        stage.addEventListener('toggle', function () {
          if (!stage.open) return;
          stages.forEach(function (other) { if (other !== stage) other.open = false; });
        });
      });
    });
  }

  /* ---- 162 工具筛选 ---- */
  function initToolFilter() {
    var query = $('#tool-query');
    var category = $('#tool-category');
    var batch = $('#tool-batch');
    var status = $('#tool-status');
    var counter = $('#tool-count');
    var rows = $$('[data-tool-row]');
    if (!query || !rows.length) return;
    function apply() {
      var needle = query.value.trim().toLowerCase();
      var cat = category ? category.value : '';
      var bat = batch ? batch.value : '';
      var sta = status ? status.value : '';
      var shown = 0;
      rows.forEach(function (row) {
        var text = (row.dataset.toolName || '') + ' ' + (row.dataset.toolKey || '');
        var ok = (!needle || text.toLowerCase().indexOf(needle) !== -1)
          && (!cat || row.dataset.toolCategory === cat)
          && (!bat || row.dataset.toolBatch === bat)
          && (!sta || row.dataset.toolStatus === sta);
        row.hidden = !ok;
        if (ok) shown++;
      });
      if (counter) counter.textContent = shown + ' / ' + rows.length;
    }
    [query, category, batch, status].forEach(function (control) {
      if (!control) return;
      control.addEventListener('input', apply);
      control.addEventListener('change', apply);
    });
    apply();
  }

  /* ---- 任务卡筛选与卡内折叠（渐进增强，脚本失效时卡片仍静态可读） ---- */
  function initTaskFilter() {
    var ids = ['task-query', 'task-state', 'task-package', 'task-batch', 'task-owner', 'task-category'];
    var controls = ids.map(function (id) { return document.getElementById(id); }).filter(Boolean);
    var cards = $$('[data-task-card]');
    var counter = $('#task-count');
    if (!controls.length || !cards.length) return;
    var value = function (id) { var node = document.getElementById(id); return node ? node.value : ''; };
    // 目标源码树节点可关联多个层/阶段/工作包，用逗号分隔；单值页面行为不变。
    function matches(dataValue, filter) {
      if (!filter) return true;
      return String(dataValue || '').split(',').indexOf(filter) !== -1;
    }

    function apply() {
      var needle = value('task-query').trim().toLowerCase();
      var state = value('task-state'), pkg = value('task-package'), batch = value('task-batch');
      var owner = value('task-owner'), category = value('task-category');
      var shown = 0;
      cards.forEach(function (card) {
        var data = card.dataset;
        var ok = (!needle || (data.taskName || '').toLowerCase().indexOf(needle) !== -1)
          && matches(data.taskState, state)
          && matches(data.taskPackage, pkg)
          && matches(data.taskBatch, batch)
          && matches(data.taskOwner, owner)
          && matches(data.taskCategory, category);
        card.hidden = !ok;
        if (ok) shown++;
      });
      if (counter) counter.textContent = shown + ' / ' + cards.length;
      if (typeof CustomEvent === 'function') {
        document.dispatchEvent(new CustomEvent('plan:filter-applied'));
      }
    }
    controls.forEach(function (control) {
      control.addEventListener('input', apply);
      control.addEventListener('change', apply);
    });

    function setSections(open) {
      $$('[data-task-card] details.task-section').forEach(function (item) {
        var card = item.closest('[data-task-card]');
        if (card && !card.hidden) item.open = open;
      });
    }
    if ($('#task-expand-all')) $('#task-expand-all').addEventListener('click', function () { setSections(true); });
    if ($('#task-collapse-all')) $('#task-collapse-all').addEventListener('click', function () { setSections(false); });
    apply();
  }

  /* ---- 目标源码树：筛选后自动展开含命中节点的层，避免结果藏在折叠层里 ---- */
  function initTreeView() {
    var layers = $$('.tree-layer');
    if (!layers.length) return;
    var ids = ['task-query', 'task-state', 'task-package', 'task-batch', 'task-owner', 'task-category'];
    document.addEventListener('plan:filter-applied', function () {
      var active = ids.some(function (id) {
        var control = document.getElementById(id);
        return control && control.value;
      });
      if (!active) return;
      layers.forEach(function (layer) {
        var visible = $$('[data-task-card]', layer).some(function (card) { return !card.hidden; });
        layer.open = visible;
      });
    });
  }

  /* ---- 真实文件结构树：按工程过滤与全部展开/收起 ---- */
  function initFileTree() {
    var groups = $$('[data-file-project]');
    if (!groups.length) return;
    var query = $('#file-query');
    var kind = $('#file-kind');
    var counter = $('#file-count');
    function apply() {
      var needle = query ? query.value.trim().toLowerCase() : '';
      var wanted = kind ? kind.value : '';
      var shown = 0;
      groups.forEach(function (group) {
        var text = ((group.getAttribute('data-file-project') || '') + ' ' +
          (group.getAttribute('data-file-purpose') || '')).toLowerCase();
        var ok = (!needle || text.indexOf(needle) !== -1)
          && (!wanted || group.getAttribute('data-file-kind') === wanted);
        group.hidden = !ok;
        if (ok) shown++;
      });
      if (counter) counter.textContent = shown + ' / ' + groups.length;
    }
    if (query) query.addEventListener('input', apply);
    if (kind) kind.addEventListener('change', apply);
    var expand = $('#file-expand-all');
    var collapse = $('#file-collapse-all');
    if (expand) expand.addEventListener('click', function () {
      groups.forEach(function (group) { group.open = true; });
    });
    if (collapse) collapse.addEventListener('click', function () {
      groups.forEach(function (group) { group.open = false; });
    });
    apply();
  }

  /* ---- 原厂程序集源码树：过滤与全部展开/收起 ---- */
  function initOriginalTree() {
    var groups = $$('[data-original-assembly]');
    if (!groups.length) return;
    var query = $('#original-tree-query');
    var counter = $('#original-tree-count');
    function apply() {
      var needle = query ? query.value.trim().toLowerCase() : '';
      var shown = 0;
      groups.forEach(function (group) {
        var text = ((group.getAttribute('data-original-assembly') || '') + ' ' +
          (group.getAttribute('data-original-tree') || '')).toLowerCase();
        var ok = !needle || text.indexOf(needle) !== -1;
        group.hidden = !ok;
        if (ok) shown++;
      });
      if (counter) counter.textContent = shown + ' / ' + groups.length;
    }
    if (query) query.addEventListener('input', apply);
    var expand = $('#original-tree-expand-all');
    var collapse = $('#original-tree-collapse-all');
    if (expand) expand.addEventListener('click', function () {
      groups.forEach(function (group) { group.open = true; });
    });
    if (collapse) collapse.addEventListener('click', function () {
      groups.forEach(function (group) { group.open = false; });
    });
    apply();
  }

  /* ---- 原厂工具目录树：按工具/分类/路径过滤并联动分类折叠块 ---- */
  function initToolLayer() {
    var items = $$('[data-tool-layer-key]');
    var groups = $$('.tool-layer-group');
    if (!items.length) return;
    var query = $('#tool-layer-query');
    var counter = $('#tool-layer-count');
    function apply() {
      var needle = query ? query.value.trim().toLowerCase() : '';
      var shown = 0;
      items.forEach(function (item) {
        var text = ((item.getAttribute('data-tool-layer-key') || '') + ' ' +
          (item.getAttribute('data-tool-layer-category') || '') + ' ' +
          (item.getAttribute('data-tool-layer-path') || '')).toLowerCase();
        var ok = !needle || text.indexOf(needle) !== -1;
        item.hidden = !ok;
        if (ok) shown++;
      });
      groups.forEach(function (group) {
        var children = Array.prototype.slice.call(group.querySelectorAll('[data-tool-layer-key]'));
        group.hidden = !children.some(function (item) { return !item.hidden; });
      });
      if (counter) counter.textContent = shown + ' / ' + items.length;
    }
    if (query) query.addEventListener('input', apply);
    var expand = $('#tool-layer-expand-all');
    var collapse = $('#tool-layer-collapse-all');
    if (expand) expand.addEventListener('click', function () {
      groups.forEach(function (group) { group.open = true; });
    });
    if (collapse) collapse.addEventListener('click', function () {
      groups.forEach(function (group) { group.open = false; });
    });
    apply();
  }

  /* ---- 依赖图：悬停/聚焦工程节点时高亮其关联边（脚本失效时图仍静态可读） ---- */
  function initDiagramHighlight() {
    var svg = $('svg[data-diagram="dependency"]');
    if (!svg) return;
    var nodes = $$('[data-diagram-node]', svg);
    var edges = $$('[data-diagram-edge]', svg);
    function focus(key) {
      edges.forEach(function (edge) {
        var hit = !!key && (edge.getAttribute('data-edge-from') === key || edge.getAttribute('data-edge-to') === key);
        edge.classList.toggle('is-active', hit);
        edge.classList.toggle('is-dimmed', !!key && !hit);
      });
      nodes.forEach(function (node) {
        node.classList.toggle('is-active', !!key && node.getAttribute('data-diagram-node') === key);
      });
    }
    nodes.forEach(function (node) {
      var key = node.getAttribute('data-diagram-node');
      node.setAttribute('tabindex', '0');
      ['mouseenter', 'focus'].forEach(function (name) {
        node.addEventListener(name, function () { focus(key); });
      });
      ['mouseleave', 'blur'].forEach(function (name) {
        node.addEventListener(name, function () { focus(null); });
      });
    });
  }

  /* ---- 闭合链：点击或键盘选中同一段，跨门禁对照 ---- */
  function initChainHighlight() {
    var nodes = $$('.chain-node');
    if (!nodes.length) return;
    function toggle(node) {
      var segment = node.dataset.chainSegment;
      var turnOn = !node.classList.contains('selected');
      nodes.forEach(function (item) { item.classList.remove('selected'); });
      if (!turnOn) return;
      nodes.forEach(function (item) {
        if (item.dataset.chainSegment === segment) item.classList.add('selected');
      });
    }
    nodes.forEach(function (node) {
      node.addEventListener('click', function () { toggle(node); });
      node.addEventListener('keydown', function (event) {
        if (event.key === 'Enter' || event.key === ' ') { event.preventDefault(); toggle(node); }
      });
    });
  }

  /* ---- 页内目录滚动高亮 ---- */
  function initToc() {
    var links = $$('.toc a[href^="#"], .side-toc a[href^="#"]');
    if (!links.length) return;
    var map = {};
    links.forEach(function (link) {
      var id = decodeURIComponent(link.getAttribute('href').slice(1));
      (map[id] = map[id] || []).push(link);
    });
    var targets = Object.keys(map).map(function (id) { return document.getElementById(id); }).filter(Boolean);
    if (!targets.length) return;
    if (!('IntersectionObserver' in window)) return;
    var observer = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (!entry.isIntersecting) return;
        links.forEach(function (link) { link.classList.remove('active'); });
        (map[entry.target.id] || []).forEach(function (link) { link.classList.add('active'); });
      });
    }, { rootMargin: '-12% 0px -72% 0px', threshold: 0 });
    targets.forEach(function (target) { observer.observe(target); });
  }

  /* ---- 打印时展开全部折叠块 ---- */
  function initPrint() {
    var opened = [];
    function before() {
      opened = $$('details:not([open])');
      opened.forEach(function (item) { item.open = true; });
    }
    function after() { opened.forEach(function (item) { item.open = false; }); opened = []; }
    window.addEventListener('beforeprint', before);
    window.addEventListener('afterprint', after);
  }

  function start() {
    initSearch();
    initSorting();
    initAccordion();
    initToolFilter();
    initTreeView();
    initFileTree();
    initOriginalTree();
    initToolLayer();
    initTaskFilter();
    initDiagramHighlight();
    initChainHighlight();
    initToc();
    initPrint();
  }
  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', start);
  else start();
})();
