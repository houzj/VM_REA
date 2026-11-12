using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;

namespace VisionPlugins.Blob;

/// <summary>Current-version managed Blob baseline. Native VM equivalence remains a separate C03 differential.</summary>
public sealed class BlobPlugin : IVisionToolPlugin, IToolDisplayProvider
{
    public const string Id = "vision.blob";
    static JsonElement J<T>(T value) => JsonSerializer.SerializeToElement(value);
    static ParameterDescriptor I(string name, int value, int min, int max) => new(name, ParameterKind.Integer, true, J(value), min, max);
    public ToolManifest Manifest { get; } = new(Id, "1.0.0", "Blob分析", "Location", "catalog/IMVSBlobFindModuImageEnable.png");
    public ModuleDescriptor Descriptor { get; } = new(Id, "1.0.0", "Blob分析",
        [new PortDescriptor("Image", DataKind.GrayImage), new PortDescriptor("InputMask", DataKind.GrayImage, false), new PortDescriptor("Region", DataKind.Region, false)],
        [new PortDescriptor("Components", DataKind.Components), new PortDescriptor("BinaryImage", DataKind.GrayImage, false), new PortDescriptor("OutputMask", DataKind.GrayImage, false)],
        [I("ThresholdType", 2, 0, 3), I("LowThreshold", 100, 0, 255), I("HightThreshold", 150, 0, 255), I("Polarity", 0, 0, 1), I("Connectivity", 8, 4, 8), I("MinArea", 10, 0, int.MaxValue), I("MaxArea", 9999999, 0, int.MaxValue), I("FindNum", 100, 1, 100000), new("OutputBinaryImage", ParameterKind.Boolean, true, J(true)), new("OutputMaskImage", ParameterKind.Boolean, true, J(false))]);
    public ToolDisplayDefinition DisplayDefinition { get; } = new("BlobCount:{0} Area:{1}", ["BlobNum", "TotalArea"], ["BlobNum", "TotalArea"], "Image");
    public ToolDisplayFrame? LastDisplay(NodeDocument node, string baseDirectory, ModuleExecutionScope? scope) => null;
    public IModule CreateRuntime() => new Runtime(this);
    public System.Windows.Forms.Form CreateEditor(ToolEditorContext context) => throw new InvalidOperationException("Use the configuration host.");
    public void Reset(string nodeId) { }

    sealed class Runtime(BlobPlugin owner) : IModule
    {
        public ModuleDescriptor Descriptor => owner.Descriptor;
        public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c, CancellationToken token)
        {
            try
            {
                var input = c.Input<GrayImage>("Image");
                if (c.Integer("ThresholdType") != 2)
                    return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed, new Dictionary<string, PortValue>(), "BlobModeUnavailable", "当前 managed 基线只支持单阈值模式，其他原厂模式尚未验证。"));
                int low = c.Integer("LowThreshold"), high = c.Integer("HightThreshold"), polarity = c.Integer("Polarity"), min = c.Integer("MinArea"), max = c.Integer("MaxArea"), limit = c.Integer("FindNum");
                if (low > high || polarity is < 0 or > 1 || min < 0 || max < min || limit < 1) throw new InvalidDataException("Blob参数范围无效。");
                byte[]? activeMask = null;
                if (c.Inputs.TryGetValue("InputMask", out var maskValue))
                {
                    var mask = maskValue.As<GrayImage>();
                    if (mask.Width != input.Width || mask.Height != input.Height) throw new InvalidDataException("Blob输入掩膜尺寸不匹配。");
                    activeMask = mask.Pixels.ToArray();
                }
                if (c.Inputs.TryGetValue("Region", out var regionValue))
                {
                    var regionMask = RoiRasterizer.CreateMask(regionValue.As<RoiRegionValue>().Regions, input.Width, input.Height, token);
                    activeMask = activeMask is null ? regionMask : activeMask.Zip(regionMask, (a, b) => (byte)(a != 0 && b != 0 ? 255 : 0)).ToArray();
                }
                var pixels = new byte[input.Pixels.Length];
                for (int i = 0; i < pixels.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    int value = polarity == 0 ? input.Pixels[i] : 255 - input.Pixels[i];
                    pixels[i] = (activeMask is null || activeMask[i] != 0) && value >= low && value <= high ? (byte)255 : (byte)0;
                }
                var binary = new GrayImage(input.Width, input.Height, pixels);
                var components = ConnectedComponents(binary, c.Integer("Connectivity")).Where(cmp => cmp.Area >= min && cmp.Area <= max).Take(limit).ToArray();
                var outputs = new Dictionary<string, PortValue> { ["Components"] = PortValue.Components(components) };
                if (c.Boolean("OutputBinaryImage")) outputs["BinaryImage"] = PortValue.Image(binary);
                if (c.Boolean("OutputMaskImage") && activeMask is not null) outputs["OutputMask"] = PortValue.Image(new GrayImage(input.Width, input.Height, activeMask));
                return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success, outputs));
            }
            catch (Exception error) when (error is InvalidDataException or ArgumentException or InvalidOperationException)
            {
                return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed, new Dictionary<string, PortValue>(), "InvalidBlobInput", error.Message));
            }
        }

        static IReadOnlyList<Component> ConnectedComponents(GrayImage image, int connectivity)
        {
            int w = image.Width, h = image.Height;
            var seen = new bool[image.Pixels.Length];
            var result = new List<Component>();
            var directions = connectivity == 4 ? new[] { (-1, 0), (1, 0), (0, -1), (0, 1) } : new[] { (-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1) };
            var queue = new Queue<int>();
            for (int seed = 0; seed < seen.Length; seed++)
            {
                if (seen[seed] || image.Pixels[seed] == 0) continue;
                seen[seed] = true; queue.Enqueue(seed);
                int area = 0, minX = w, minY = h, maxX = 0, maxY = 0; long sumX = 0, sumY = 0;
                while (queue.TryDequeue(out int index))
                {
                    int x = index % w, y = index / w; area++; sumX += x; sumY += y;
                    minX = Math.Min(minX, x); minY = Math.Min(minY, y); maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
                    foreach (var (dx, dy) in directions)
                    {
                        int nx = x + dx, ny = y + dy;
                        if ((uint)nx >= (uint)w || (uint)ny >= (uint)h) continue;
                        int neighbor = ny * w + nx;
                        if (seen[neighbor] || image.Pixels[neighbor] == 0) continue;
                        seen[neighbor] = true; queue.Enqueue(neighbor);
                    }
                }
                result.Add(new Component(area, minX, minY, maxX, maxY, (double)sumX / area, (double)sumY / area));
            }
            return result;
        }
    }
}
