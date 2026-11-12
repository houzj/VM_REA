using System.Windows;
using VisionEditor.Core;
using VisionFramework;
using DragEventArgs = System.Windows.DragEventArgs;
using DragDropEffects = System.Windows.DragDropEffects;
using DataFormats = System.Windows.DataFormats;

namespace VisionEditor;

public partial class MainWindow
{
    (Guid Scope, string Procedure, string Node)? firstImageRequest;
    bool CanEditImageList => !windowClosed && !isLocked && activeRun is null && activeConfigurationRuns == 0;
    void ApplyImageList(NodeDocument original, NodeDocument changed)
    {
        if (!CanEditImageList) return;
        if (original.Parameters.Count == changed.Parameters.Count && original.Parameters.All(p =>
                changed.Parameters.TryGetValue(p.Key, out var value) && value.GetRawText() == p.Value.GetRawText())) return;
        InvalidateSourceResults(original);
        doc.SetParameters(original.Id, changed.Parameters);
    }
    void ImageFilesDragOver(object sender, DragEventArgs e)
    {
        e.Effects = CanEditImageList && e.Data.GetData(DataFormats.FileDrop) is string[] paths && ImageSourceListOperations.CanDrop(paths)
            ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }
    void ImageFilesDrop(object sender, DragEventArgs e)
    {
        if (CanEditImageList && e.Data.GetData(DataFormats.FileDrop) is string[] paths && ImageSourceListOperations.CanDrop(paths)) AddImages(paths);
        e.Handled = true;
    }
    void DeleteSourceImage(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (!CanEditImageList || sender is not FrameworkElement { DataContext: ImageListEntry entry } || SelectedImageSource() is not { } source) return;
        ApplyImageList(source, ImageSourceListOperations.Remove(source, entry.Path));
    }
    void ReturnToFirstSourceImage(object sender, RoutedEventArgs e)
    {
        if (!CanEditImageList || SelectedImageSource() is not { } source) return;
        var changed = ImageSourceListOperations.ReturnToFirst(source);
        if (ReferenceEquals(changed, source)) return;
        firstImageRequest = (executionScope.Id, doc.CurrentProcedureId, source.Id);
        ApplyImageList(source, changed);
        RefreshImageSourcePanel(doc.CurrentProcedure.Nodes.Single(n => n.Id == source.Id));
        if (ImageList.Items.Count > 0) ImageList.ScrollIntoView(ImageList.Items[0]);
    }
}
