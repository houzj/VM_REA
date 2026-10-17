using System.Globalization;
using System.IO;
using System.Xml.Linq;
namespace VisionPlugins.Calibration;

/// <summary>RotateCalibModu's file reader, independent of the N-point XML writer.</summary>
public static class RotateCalibrationFile
{
    public static uint Read(string path, float[] matrix, out bool loaded)
    {
        loaded = false;
        if (string.IsNullOrEmpty(path)) return 0xe0000204;
        // Native SetParam tests the last two bytes exactly, not an extension enum.
        bool xml = path.EndsWith("ml", StringComparison.Ordinal);
        bool binary = path.EndsWith("al", StringComparison.Ordinal);
        if (!xml && !binary) return 0xe0000204;
        try
        {
            if (binary)
            {
                var bytes = File.ReadAllBytes(path);
                if (bytes.Length != 36) return 0xe0000204;
                for (int i = 0; i < 9; i++) matrix[i] = BitConverter.ToSingle(bytes, i * 4);
                loaded = true;
                return 0;
            }
            var document = XDocument.Load(path);
            var output = document.Root?.Element("CalibOutputParam");
            if (output is null) return 0xe0000017;
            float last = 0;
            foreach (var item in output.Elements())
            {
                string? type = (string?)item.Attribute("DataType");
                if (type is "float" or "CalibPointF") { foreach (var value in item.Elements()) last = ParseNative(value.Value, last); continue; }
                if (type != "FloatList") continue;
                var values = item.Elements().Select(x => last = ParseNative(x.Value, last)).ToArray();
                // The original writes beyond its 9-float array for oversized lists.
                // Bound the independent implementation rather than reproduce memory corruption.
                for (int i = 0; i < Math.Min(9, values.Length); i++) matrix[i] = values[i];
                loaded = true;
            }
            return 0;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Xml.XmlException or ArgumentException)
        { return 0xe0000201; }
    }
    private static float ParseNative(string text, float fallback)
    {
        text = text.TrimStart();
        // Native TinyXML QueryFloatAttribute/QueryFloatText accepts a numeric prefix.
        var match = System.Text.RegularExpressions.Regex.Match(text, @"^[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?");
        return match.Success && float.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : fallback;
    }
}
