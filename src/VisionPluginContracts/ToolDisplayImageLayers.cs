using System.Text.Json;
using VisionCore;
using VisionFramework;

namespace VisionPluginContracts;

/// <summary>Projects declared image display objects from a node's captured input/output snapshot.</summary>
public static class ToolDisplayImageLayers
{
    public static IReadOnlyList<ToolDisplayImageLayer> Build(
        IReadOnlyList<ToolDisplayObjectDefinition> definitions,
        IReadOnlyDictionary<string, JsonElement> parameters,
        IReadOnlyDictionary<string, PortValue> inputs,
        IReadOnlyDictionary<string, PortValue> outputs)
    {
        var styles = ToolDisplayObjectStore.ReadStyles(parameters, definitions);
        var layers = new List<ToolDisplayImageLayer>();
        foreach (var definition in definitions.Where(item => item.Type == "image"))
        {
            var feature = definition.Features.GetValueOrDefault("Image");
            PortValue? value = feature is null ? null : outputs.GetValueOrDefault(feature.Mapping) ?? inputs.GetValueOrDefault(feature.Mapping);
            IImageData? image = value is not null && DataKinds.IsImage(value.Kind) ? value.As<IImageData>() : null;
            var style = styles.GetValueOrDefault(definition.Name) ?? definition.DefaultStyle;
            layers.Add(new(definition.Name, image, style.Visible, style.Opacity));
        }
        return layers.AsReadOnly();
    }
}
