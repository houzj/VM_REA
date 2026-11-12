using System.Security.Cryptography;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionModules;
using VisionPluginContracts;
using VisionPlugins.ImageSource;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: C01.ReplayChecks <archived C01 directory> <1.jpg> <new output directory>");
    return 1;
}

try
{
    var archive = Path.GetFullPath(args[0]);
    var image = Path.GetFullPath(args[1]);
    var output = Path.GetFullPath(args[2]);
    if (Directory.Exists(output)) throw new IOException("The replay output directory must be new.");
    const string expectedInput = "3A77112B982DF494A74CD756CD412D045210BADE88604BE52AB875C972CA303E";
    string Digest(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
    if (Digest(image) != expectedInput) throw new InvalidDataException("The C01 sample SHA-256 differs.");

    Directory.CreateDirectory(output);
    using var history = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(archive, "runtime-report.json")));
    string[] Expected(params string[] names)
    {
        var entry = history.RootElement;
        foreach (var name in names) entry = entry.GetProperty(name);
        return entry.EnumerateArray().Select(x => x.GetString()!).Order(StringComparer.Ordinal).ToArray();
    }
    var registry = new ModuleRegistry();
    BuiltinModules.RegisterAll(registry);
    new ToolPluginRegistry().Register(new ImageSourcePlugin(), registry);
    var executor = new SolutionExecutor(registry);
    var checks = new List<string>();
    var fixtures = new Dictionary<string, string>();
    void Check(bool passed, string label)
    {
        if (!passed) throw new InvalidDataException(label);
        checks.Add(label);
    }
    SolutionDocument ReplaceImage(SolutionDocument doc, string? file) => doc with
    {
        Procedures = doc.Procedures.Select(p => p with
        {
            Nodes = p.Nodes.Select(n => n.ModuleId != "vision.image-source" ? n : n with
            {
                Parameters = new Dictionary<string, JsonElement>(n.Parameters)
                {
                    ["CurrentImagePath"] = JsonSerializer.SerializeToElement(file ?? ""),
                    ["ImagePaths"] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(
                        file is null ? Array.Empty<string>() : new[] { file }))
                }
            }).ToArray()
        }).ToArray()
    };
    SolutionDocument Load(string name)
    {
        var file = Path.Combine(archive, $"image-source{name}-vertical-slice.vmx.same-input-20261110.json");
        fixtures.Add(Path.GetFileName(file), Digest(file));
        return ReplaceImage(SolutionStore.Load(file), image);
    }
    Task<ExecutionSessionResult> Run(SolutionDocument doc, ModuleExecutionScope scope)
        => executor.RunAsync(new(doc, output), doc.Procedures[0].Id, scope: scope);
    string PixelHash(PortValue value)
    {
        var frame = value.As<IImageData>();
        Check(frame.Width == 1280 && frame.Height == 960 && frame.Stride == 1280 &&
            frame.Format == ImagePixelFormat.Mono8, "Decoded frame dimensions, stride and Mono8 format");
        return Convert.ToHexString(SHA256.HashData(frame.Pixels));
    }
    bool Outputs(IReadOnlyDictionary<string, PortValue> actual, string[] expected)
        => actual.Keys.Order(StringComparer.Ordinal).SequenceEqual(expected);

    var source = Load("");
    using var sourceScope = new ModuleExecutionScope();
    var first = await Run(source, sourceScope);
    var second = await Run(source, sourceScope);
    Check(first.Status == SessionStatus.Success && first.Nodes.Single().Status == NodeRunStatus.Success &&
        Outputs(first.Nodes.Single().Outputs, Expected("first", "outputs")), "ImageSource first run matches archived output names");
    Check(second.Status == SessionStatus.Success && second.Nodes.Single().Status == NodeRunStatus.Success &&
        Outputs(second.Nodes.Single().Outputs, Expected("second", "outputs")), "ImageSource repeat matches archived output names");
    var pixels = PixelHash(first.Nodes.Single().Outputs["Image"]);
    Check(PixelHash(second.Nodes.Single().Outputs["Image"]) == pixels, "ImageSource repeats the same pixels");

    var group = Load("-group");
    using var groupScope = new ModuleExecutionScope();
    var groupResult = (await Run(group, groupScope)).Calls.Values.Single();
    Check(groupResult.Status == SessionStatus.Success && Outputs(groupResult.WorkflowOutputs, Expected("group", "outputs")) &&
        groupResult.WorkflowOutputs["width"].As<long>() == 1280 &&
        PixelHash(groupResult.WorkflowOutputs["image"]) == pixels, "Group publishes the same image and width");
    var saved = Path.Combine(output, "group-roundtrip.vmx.json");
    SolutionStore.Save(saved, group);
    using var reloadScope = new ModuleExecutionScope();
    var reloaded = (await Run(SolutionStore.Load(saved), reloadScope)).Calls.Values.Single();
    Check(reloaded.Status == SessionStatus.Success && Outputs(reloaded.WorkflowOutputs, Expected("group", "reloadedOutputs")) &&
        reloaded.WorkflowOutputs["width"].As<long>() == 1280 &&
        PixelHash(reloaded.WorkflowOutputs["image"]) == pixels, "Saved and reopened Group preserves the output and pixels");
    using var failureScope = new ModuleExecutionScope();
    var failure = await Run(ReplaceImage(group, null), failureScope);
    Check(failure.Status == SessionStatus.Failed && failure.Calls.Values.Single().WorkflowOutputs.Count == 0,
        "Missing source input fails Group without publishing outputs");

    var blob = Load("-blob");
    var validation = GraphValidator.Validate(blob.Procedures[0], registry);
    var blobStatus = "blocked";
    if (validation.IsValid)
    {
        var expected = history.RootElement.GetProperty("blob");
        void CheckBlob(ExecutionSessionResult run, string label)
        {
            var node = run.Nodes.Single(n => n.NodeId == "blob");
            Check(run.Status == SessionStatus.Success && node.Status == NodeRunStatus.Success &&
                Outputs(node.Outputs, Expected("blob", "outputs")) &&
                node.Outputs["Count"].As<long>() == expected.GetProperty("count").GetInt64() &&
                node.Outputs["TotalArea"].As<long>() == expected.GetProperty("totalArea").GetInt64(), label);
        }
        using var blobScope = new ModuleExecutionScope();
        CheckBlob(await Run(blob, blobScope), "Blob output names, count and area match archived C01");
        var savedBlob = Path.Combine(output, "blob-roundtrip.vmx.json");
        SolutionStore.Save(savedBlob, blob);
        using var blobReloadScope = new ModuleExecutionScope();
        CheckBlob(await Run(SolutionStore.Load(savedBlob), blobReloadScope), "Saved and reopened Blob matches archived C01");
        blobStatus = "passed";
    }
    var summary = new
    {
        scope = "Current independent implementation replay from archived C01 fixtures",
        input = image,
        inputSha256 = expectedInput,
        inputPixelsSha256 = pixels,
        fixtures,
        checks,
        blob = new { status = blobStatus, validationIssues = validation.Issues }
    };
    File.WriteAllText(Path.Combine(output, "summary.json"),
        JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine($"PASS {checks.Count} current C01 checks. Blob replay: {blobStatus}.");
    foreach (var issue in validation.Issues) Console.WriteLine($"{issue.NodeId}: {issue.Message}");
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine(error);
    return 1;
}
