# Gray/Contour model-list renderer evidence and proposed boundary

Original Gray `Apps.XmlParser.ParamTab.GrayMatchVAConfigPanalData` and Contour `PyramidFeaturepatMatchConfigPanalData` both inherit `FeaturepatMatchConfigPanalData`. Their `DisplayType` getter is exactly `((object)this).GetType().BaseType.Name`, with an empty setter. Both therefore resolve to `FeaturepatMatchConfigPanalData`. Original files are under the matching `IMVSGrayMatchModuVAControl` and `IMVSContourMatchModuControl` directories in `analysis/full-audit/renderer-gap-projects`. This supports one shared renderer, not shared training or file formats: Gray overrides `.gmvaxml`, `GrayMatchVADialogData` and `GrayMatchVADialogUI`; Contour overrides `.contourmxml` and its pyramid-specific dialog/data.

Current `src/VisionPlugins.ContourMatch/ContourModelControls.cs` directly captures `ContourModelCollection`, reads `Storage.Snapshot`, and calls Select/Rename/Move/Delete/Clear. The renderer never reads the model's binary Data. Minimal proposed injected backend:

```csharp
record ModelListItem(int Index, byte[] RawName, bool Selected);
interface IModelListBackend {
    IReadOnlyList<ModelListItem> Snapshot();
    uint Select(int index, bool selected);
    uint Rename(int index, byte[] utf8Name);
    uint Move(int index, bool up);
    uint Delete(int index);
    void Clear();
}
```

This is an independent transport contract, not an original SDK ABI. Snapshot bytes must be owned copies. Each adapter owns its native-equivalent mutation, model sidecar operations and persistence callback; the renderer must not persist a second time. Contour currently persists within collection Move/Delete/Clear, but separately after Storage.Select/Rename. An adapter normalizes that call boundary while retaining actual failure/partial-commit behavior; it must not invent transaction rollback. Gray uses its own model state and training actions. Training, load and edit remain separate injected `Action build`, `Action load`, `Action<int> edit`; running predicate, confirmation, notification/localization and operation-error caption are injected too. No `Contour` label or concrete storage type belongs in shared code.

Original `FeatureMatchCommonControl.ModelItem.ModelNameTextChangedCommandExcute` checks TextBox input and text length >32, selects `ChooseSpareModelIndex = Id-1`, then checks whitespace and writes UTF8 `ChangeSpareModelName`. It has no running guard in this method. Consequently the current Contour rename handler's absence of `Run` wrapper must not be accidentally changed while extracting the renderer. Its current whitespace-before-select implementation is not proof of original selection side effects. Button and selection running guards are separate original paths. The existing TextBox MaxLength63 and rename limit32 are distinct; do not collapse them based on intent.

This report proposes the smallest shared extraction only. Root owns shared implementation and Contour adaptation. Gray model slots, training and file payload remain D-owned; successful model-slot arithmetic checks do not complete training/search, original file interoperability or full plugin execution.
