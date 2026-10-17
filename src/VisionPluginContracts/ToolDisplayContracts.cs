using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using VisionCore;
using VisionFramework;
namespace VisionPluginContracts;

/// <summary>Display.xml objects are presentation metadata, never algorithm result ports.</summary>
public sealed record ToolDisplayDefinition(string ContentTemplate, IReadOnlyList<string> ContentMapping,
    IReadOnlyList<string> ResultColumns, string ImageInput = "InputImage")
{
    public string OkColor { get; init; } = "#66ff00";
    public string NgColor { get; init; } = "#ff0000";
    public double FontSize { get; init; } = 10;
    public double Opacity { get; init; } = 1;
    public bool TextVisible { get; init; } = true;
    public bool ImageVisible { get; init; } = true;
    public bool RecordVisible { get; init; } = true;
    public bool ResultListVisible { get; init; } = true;
}
public sealed record ToolTextDisplaySettings(string Content, string OkColor, string NgColor,
    double FontSize, double Opacity, double PositionX, double PositionY);
public sealed record ToolDisplayCell(string Column, string Value);
public sealed record ToolDisplayDiagnostic(uint NativeErrorCode, string ErrorInfo);
public sealed record ToolDisplayFrame(VisionCore.IImageData? Image, string Text, string Color,
    double FontSize, double Opacity, double PositionX, double PositionY, string RecordText,
    IReadOnlyList<ToolDisplayCell> ResultList)
{
    public bool TextVisible { get; init; }
    public bool ImageVisible { get; init; }
    public bool RecordVisible { get; init; }
    public bool ResultListVisible { get; init; }
}
public interface IToolDisplayProvider
{
    ToolDisplayDefinition DisplayDefinition { get; }
    /// <summary>Rebuild current presentation from captured inputs/results and persisted display settings.
    /// No algorithm execution or file loading. Null means this node has no execution snapshot.</summary>
    ToolDisplayFrame? LastDisplay(NodeDocument node, string baseDirectory, ModuleExecutionScope? scope);
}
public static class ToolDisplayService
{
    public const string SettingsParameter = "DisplaySettings";
    public const string PositionXInput = "DisplayoutputPositionX";
    public const string PositionYInput = "DisplayoutputPositionY";
    public static ToolTextDisplaySettings Defaults(ToolDisplayDefinition definition) => new(definition.ContentTemplate,
        definition.OkColor, definition.NgColor, definition.FontSize, definition.Opacity, 0, 0);
    public static ToolTextDisplaySettings Read(IReadOnlyDictionary<string, JsonElement> parameters, ToolDisplayDefinition definition)
    {
        if (parameters.TryGetValue(SettingsParameter, out var value) && value.ValueKind == JsonValueKind.String)
            try { var settings = JsonSerializer.Deserialize<ToolTextDisplaySettings>(value.GetString() ?? ""); if (settings is not null && Valid(settings)) return settings; }
            catch (JsonException) { }
        return Defaults(definition);
    }
    public static JsonElement Serialize(ToolTextDisplaySettings settings)
    {
        if (!Valid(settings)) throw new ArgumentException("文本显示参数无效。", nameof(settings));
        return JsonSerializer.SerializeToElement(JsonSerializer.Serialize(settings));
    }
    public static bool Valid(ToolTextDisplaySettings s) => s.Content is not null &&
        Regex.IsMatch(s.OkColor ?? "", "^#[0-9a-fA-F]{6}$") && Regex.IsMatch(s.NgColor ?? "", "^#[0-9a-fA-F]{6}$") &&
        double.IsFinite(s.FontSize) && s.FontSize is >= 6 and <= 72 && double.IsFinite(s.Opacity) && s.Opacity is >= 0 and <= 1 &&
        double.IsFinite(s.PositionX) && double.IsFinite(s.PositionY);
    static string Value(PortValue? value) => value?.Kind switch
    {
        DataKind.Number => ((float)value.As<double>()).ToString("G7", CultureInfo.CurrentCulture),
        DataKind.Integer => value.As<long>().ToString(CultureInfo.InvariantCulture),
        DataKind.Text => value.As<string>(),
        DataKind.Boolean => value.As<bool>().ToString(),
        DataKind.NumberArray => string.Join(",", value.As<IReadOnlyList<double>>().Select(n => ((float)n).ToString("G7", CultureInfo.CurrentCulture))),
        _ => ""
    };
    /// <summary>Independent formatter. Named placeholders may use an explicit host resolver;
    /// numeric placeholders use the original Display.xml mapping order.</summary>
    public static ToolDisplayFrame Build(ToolDisplayDefinition definition, ToolTextDisplaySettings settings,
        IReadOnlyDictionary<string, PortValue> inputs, IReadOnlyDictionary<string, PortValue> outputs,
        Func<string, PortValue?>? resolve = null, ToolDisplayDiagnostic? diagnostic = null)
    {
        if (!Valid(settings)) throw new ArgumentException("文本显示参数无效。", nameof(settings));
        PortValue? Get(string name) => outputs.GetValueOrDefault(name) ?? inputs.GetValueOrDefault(name) ?? resolve?.Invoke(name);
        string Format(string template)
        {
            bool placeholders = false, anyValue = false;
            string text = Regex.Replace(template, @"\{([^{}]+)\}", match =>
            {
                placeholders = true; string token = match.Groups[1].Value;
                string name = int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out int index) && index >= 0 && index < definition.ContentMapping.Count ? definition.ContentMapping[index] : token;
                var value = Get(name); anyValue |= value is not null; return Value(value);
            });
            return placeholders && !anyValue ? "" : text;
        }
        double Number(string name, double fallback) => Get(name) is {} value ? value.Kind switch
        { DataKind.Number => value.As<double>(), DataKind.Integer => value.As<long>(), DataKind.NumberArray when value.As<IReadOnlyList<double>>().Count > 0 => value.As<IReadOnlyList<double>>()[0], _ => fallback } : fallback;
        string text = Format(settings.Content);
        var image = inputs.GetValueOrDefault(definition.ImageInput);
        return new(image is not null && DataKinds.IsImage(image.Kind) ? image.As<IImageData>() : null,
            text, Number("ModuStatus", 0) == 0 ? settings.NgColor : settings.OkColor,
            settings.FontSize, settings.Opacity, Number(PositionXInput, settings.PositionX), Number(PositionYInput, settings.PositionY),
            diagnostic is null ? Format(definition.ContentTemplate) : $"模块状态:0 错误码:{diagnostic.NativeErrorCode:X} 错误信息:{diagnostic.ErrorInfo}", definition.ResultColumns.Select(name => new ToolDisplayCell(name, Value(Get(name)))).ToArray())
        { TextVisible = definition.TextVisible, ImageVisible = definition.ImageVisible, RecordVisible = definition.RecordVisible, ResultListVisible = definition.ResultListVisible };
    }
}

