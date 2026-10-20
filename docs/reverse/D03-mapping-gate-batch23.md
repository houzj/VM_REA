# 第二十三轮 D03.1：原生关系映射门槛复核

基线：`a6aac9d`。本轮复核 `SemanticMap`、`SubscribeParamData`、`SubscribeUnRedoAction`、`SubscribeParam` 及已有 `.sol` 外层证据。

当前 main 已有显式 XML 所属、Following/Front 双向连线声明、UI 坐标候选及三类身份域隔离。现有订阅模型只确认 `ModuleName/ParamData`、撤销/重做/更新委托释放、`Type/CurValue/ParamName`。

原生 `Relation` token 的解析、owner 转换、订阅写回、撤销生命周期及 `.sol` 往返没有同版源码或受控差分证据。因此保留 raw Relation 并标记 `unresolved-native-relation-semantics` 是当前实现边界；本轮没有产品修改，D03.1 继续 partial，实施增量 `+0`。解除条件是同版原/native producer-consumer 链和受控保存重载差分，不能猜测 parser、合并 GUID 或扩展 JSON。

