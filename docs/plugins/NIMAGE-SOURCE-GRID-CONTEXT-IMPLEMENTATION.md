# Source-grid上下文运输实施与自审

2026-10-19，CAL-SRC-003/R02内部载荷部分实施。原件core SHA256为85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549；来源为既有80e690/80bcd0源码，必需体索引沿用map-calibration/d2-exit-audit/source-index.json。

80e690在密集ID双槽先写INT_MIN，求输入Column/Row minima；逐点按(row-minRow,column-minColumn)寻址，写原两个ID及float XY。按原输入序覆盖，重复ID后写XY覆盖，未按XY去重。未写float槽的原生工作区内容未知。

实施：新增不可变NImageSourceGridContext，保存Width/Height/minima、原始顺序点以及dense cell。缺cell保留两INT_MIN，XY以nullable明确未知，不以零模拟原生内存。有效点以源序写cell，重复后写覆盖；异常外部尺寸/ID显式拒绝，禁止复制原生越界写。NImageLattice增加可选SourceGrid，SourceLatticeAdapter生产入口同步挂接，原公开构造签名保持。

本次只运输已确认载荷，旧Pitch/CodeQuadrilaterals、identity选择、mask、Nearest anchor与方向路径未替换。没有把这一载荷接入等同原厂proposal/方向/decoder已经闭合；后续R04必须从该上下文获取真实anchor和方向，不能只删除Orientation拒绝门。外部使用record with修改Points时，SourceGrid仍是生产时快照；不得把不匹配的人工快照作为当前点集合的原厂上下文。

自动验证与自审：analysis/nimage-source-context/2fdc0978528e439db9480cd929d4399f包含源码哈希、build/check日志、boundary.json和summary.json。构建运行退出0，sourceStable=true。合成边界验证非零/负min offset、缺槽INT_MIN/未知XY、重复覆盖、输入列表后改不污染快照及非法尺寸拒绝；两个既有1000×1000灰度fixture验证实际source adapter上下文与点列表的ID/XY顺序逐项对应。HIK7/9、normal/reverse共8例旧识别仍168点，挂接/移除SourceGrid不改变当前输出点序列；既有all/proposal-prefix比较继续通过。以上为本产品验证，不是新增原厂运行或完整原生等价证明。

根线审查：无全局可变状态或方案字段；不可变上下文仅随本次识别，取消和旧fallback入口保持原路径。载荷部分实施通过；R01首成功mask边界、R03源纠错与R04 proposal/anchor/方向仍单列实际缺口。当前完整实施工具0/10，人工验收不计分。
