using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.ContourMatch;
using VisionPlugins.CalibrationBoard;
using VisionPlugins.DistortionCalibration;

internal static class Program
{
    static readonly List<string> checks = [];
    static void Check(bool value, string name) { if (!value) throw new InvalidOperationException(name); checks.Add(name); }
    [STAThread]
    static int Main(string[] args)
    {
        var output = Path.GetFullPath(args.FirstOrDefault() ?? Path.Combine("analysis/full-audit/framework-numeric-ranges", Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(output);
        try
        {
            foreach (bool floating in new[] { false, true }) { CheckRange(floating); CheckControlObjects(floating); CheckReset(floating); }
            CheckCompositeBoundaries();
            CheckRealTool(output);
            var board = new CalibrationBoardPlugin();
            CheckShippedTool(board, CalibrationBoardPlugin.Id, board.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone()), output);
            CheckShippedTool(new DistortionCalibrationConfigurationProvider((_, _) => throw new InvalidOperationException("No export requested")), DistortionCalibrationMetadata.ModuleId, DistortionCalibrationMetadata.Defaults(), output);
            File.WriteAllText(Path.Combine(output, "summary.json"), JsonSerializer.Serialize(new {
                scope = "Headless shared numeric range model, source-resource integration, subscriptions, failure recovery and persistence; no windows, original SDK or matching kernel",
                passed = checks.Count, checks, windowsCreated = 0, originalSdkExecuted = false
            }, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"PASS {checks.Count} headless numeric range checks");
            return 0;
        }
        catch (Exception error)
        {
            File.WriteAllText(Path.Combine(output, "failed.json"), JsonSerializer.Serialize(new { passed = checks.Count, checks, error = error.ToString() }, new JsonSerializerOptions { WriteIndented = true }));
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    static string Xml(bool floating, int count = 2)
    {
        string scalar = floating ? "Float" : "Integer", range = scalar + "Bettween", collection = floating ? "Floats" : "Integers";
        var children = Enumerable.Range(0, count).Select(i => new XElement(scalar, new XAttribute("Name", "V" + i),
            new XElement("AccessMode", "RW"), new XElement("DisplayName", "Value " + i),
            new XElement("CurValue", i == 0 ? "2" : "8"), new XElement("MinValue", "-10"), new XElement("MaxValue", "10"),
            new XElement("IncValue", "1"), new XElement("DecimalDigits", "3")));
        XElement Setter(string target, string operation) => new("Setter", new XElement("TargetName", target), new XElement("OperationName", operation));
        var expand = new XElement("Expand", new XAttribute("Name", "Advanced"), new XElement("CurValue", "True"),
            new XElement("Triggers", new XElement("Trigger", new XElement("Property", "CurValue"), new XElement("Value", "True"), new XElement("Setters", Setter("Range", "VisibleOperation"))),
                new XElement("Trigger", new XElement("Property", "CurValue"), new XElement("Value", "False"), new XElement("Setters", Setter("Range", "HiddenOperation")))));
        var rangeElement = new XElement(range, new XAttribute("Name", "Range"), new XElement("AccessMode", "RW"),
            new XElement("MinValue", floating ? "-10" : "0xfffffff6"), new XElement("MaxValue", "10"),
            new XElement("IncValue", floating ? "0.25" : "2"), new XElement(collection, children));
        var category = new XElement("Category", new XAttribute("Name", "C"), new XElement("Items", expand, rangeElement));
        var tab = new XElement("Tab", new XAttribute("Name", "T"), new XElement("Categorys", category));
        return new XElement("AlgorithmTabRoot", new XElement("Tabs", tab)).ToString();
    }

    static void CheckRange(bool floating)
    {
        string label = floating ? "float" : "integer";
        var probe = new Probe();
        var state = new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(floating)), probe);
        state.Initialize();
        var range = state.Find("Range")!;
        var children = state.GetNumericRangeChildren(range);
        Check(children.Select(n => n.Name).SequenceEqual(new[] { "V0", "V1" }), label + " preserves declared child identities and order");
        Check(ReferenceEquals(children[0], state.Find("V0")) && children.All(n => ReferenceEquals(n.Parent, range)), label + " child lookup uses the owned configuration objects");
        Check(children.Select(n => n.Value).SequenceEqual(new[] { "2", "8" }), label + " initializes each child from backend parameters");
        Check(probe.Requests.Count(r => r.Kind == ConfigurationOperationKind.ReadParameter && r.Source.Name == "Range") == 0, label + " does not query a phantom composite backend parameter");
        Check(probe.Requests.Any(r => r.Kind == ConfigurationOperationKind.ReadUserValue && r.Parameters.FirstOrDefault() == "@V0") && probe.Requests.Any(r => r.Kind == ConfigurationOperationKind.ReadUserValue && r.Parameters.FirstOrDefault() == "@V1"), label + " initializes both subscription modes separately");
        Check(children.All(n => n.Template.TemplateKey == (floating ? "Float" : "Integer")), label + " retains scalar template identities");
        if (!floating) Check(range.Get("MinValue") == "-10" && !range.IsInteger32, "integer range normalizes signed metadata without becoming a scalar parameter");

        int writes = probe.Writes.Count;
        state.CommitUser(children[0], floating ? "3.125" : "3");
        Check(probe.Writes.Count == writes + 1 && probe.Writes[^1] == "V0", label + " commits only the selected child");
        Check(children[1].Value == "8" && probe.Values["V1"] == "8", label + " leaves the other endpoint unchanged");
        state.CommitUser(children[0], "NaN"); state.CommitUser(children[0], "11");
        if (!floating) state.CommitUser(children[0], "1.5");
        Check(probe.Writes.Count == writes + 1, label + " rejects nonfinite, range and scalar type violations before backend write");
        state.SetProgrammatic(children[0], "Enable", "False"); state.CommitUser(children[0], "4");
        Check(probe.Writes.Count == writes + 1, label + " rejects disabled writes");
        state.SetProgrammatic(children[0], "Enable", "True"); state.SetProgrammatic(children[0], "AccessMode", "R"); state.CommitUser(children[0], "4");
        Check(probe.Writes.Count == writes + 1, label + " rejects read-only writes");
        state.SetProgrammatic(children[0], "AccessMode", "RW");
        var before = children[0].Value;
        probe.FailWrites = true; state.CommitUser(children[0], "4");
        Check(children[0].Value == before && probe.Values["V0"] == before, label + " failed backend write restores committed endpoint");
        probe.FailWrites = false; state.CommitUser(children[0], "4");
        Check(children[0].Value == "4" && probe.Values["V0"] == "4", label + " failed write can be corrected and retried");
        writes = probe.Writes.Count; state.CommitUser(range, "5");
        Check(probe.Writes.Count == writes && state.Diagnostics.Any(d => d.Code == "CompositeParameter"), label + " refuses direct composite writes explicitly");

        for (int i = 0; i < 2; i++)
        {
            var binding = new ConfigurationSubscriptionBinding(new ConfigurationOutputReference("p", "source" + i, "out", floating ? "float" : "int"), ConfigurationSubscriptionIndex.All);
            Check(state.ApplySubscription(children[i], new(ConfigurationSubscriptionStatus.Available, binding, "source" + i + ".out"), true), label + " binds endpoint " + i);
            state.PersistSubscriptionMode(children[i], true);
        }
        Check(children[0].SubscriptionBinding != children[1].SubscriptionBinding && probe.User["@V0"] == "True" && probe.User["@V1"] == "True", label + " endpoint subscriptions and stored modes remain independent");
        state.CommitUser("Advanced", "False");
        Check(children.All(n => !n.CustomVisible && !n.IsDeleteSubscribe) && !range.CustomVisible, label + " collapsing advanced parameters propagates visibility and preserves bindings");
        Check(probe.Clears.Count == 0 && children.All(n => n.SubscriptionBinding is not null), label + " advanced collapse does not remove either subscription");
        state.CommitUser("Advanced", "True");
        Check(children.All(n => n.CustomVisible && n.IsDeleteSubscribe), label + " expanding restores child flags before later visibility changes");
        probe.FailClearName = "V0";
        state.SetProgrammatic(range, "CustomVisible", "False");
        Check(children[0].SubscriptionBinding is not null && children[1].SubscriptionBinding is null, label + " unbind failure retains only failed endpoint identity");
        Check(probe.Clears.SequenceEqual(new[] { "V0", "V1" }), label + " visibility unbinds children in declaration order and never the composite");
        Check(state.Diagnostics.Any(d => d.Code == "SubscriptionClearFailed") && !state.Diagnostics.Any(d => d.Code == "UnsupportedExpansion"), label + " reports concrete removal failure without unsupported range fallback");
        state.SetProgrammatic(range, "CustomVisible", "True"); probe.FailClearName = null; state.SetProgrammatic(range, "CustomVisible", "False");
        Check(children.All(n => n.SubscriptionBinding is null), label + " later visibility transition retries retained binding successfully");
        state.SetProgrammatic(range, "CustomVisible", "True");
        probe.Values["V0"] = "1"; probe.Values["V1"] = "9";
        Check(state.RefreshParameter(range) && children[0].Value == "1" && children[1].Value == "9", label + " explicit composite refresh reads both child values");
        probe.FailReadName = "V0"; int reads = probe.Requests.Count(r => r.Kind == ConfigurationOperationKind.ReadParameter);
        Check(!state.RefreshParameter(range) && probe.Requests.Count(r => r.Kind == ConfigurationOperationKind.ReadParameter) == reads + 1, label + " explicit composite refresh stops at the first failed child");
    }

    static void CheckCompositeBoundaries()
    {
        foreach (int count in new[] { 0, 1, 3 })
        {
            var state = new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(true, count)), new Probe());
            var range = state.Find("Range")!;
            Check(state.GetNumericRangeChildren(range).Count == count, "declared range collection count " + count);
            if (count < 2) Check(!state.RefreshParameter(range) && state.Diagnostics.Any(d => d.Code == "InvalidNumericRange"), "malformed range is diagnosed without null dereference " + count);
            else { state.CommitUser("Advanced", "False"); Check(state.GetNumericRangeChildren(range).All(n => !n.IsDeleteSubscribe), "expansion visits every declared subchild, including third member"); }
        }
        var first = new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(true)));
        var second = new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(true)));
        bool rejected = false; try { first.GetNumericRangeChildren(second.Find("Range")!); } catch (ArgumentException) { rejected = true; }
        Check(rejected, "range enumeration rejects foreign model ownership");
        rejected = false; try { first.GetNumericRangeChildren(first.Find("V0")!); } catch (ArgumentException) { rejected = true; }
        Check(rejected, "range enumeration does not guess composite capability from scalar kind");
        Check(first.Definition.SourceXml == Xml(true), "configuration source XML remains unchanged by state materialization");
    }

    static IEnumerable<T> Descendants<T>(System.Windows.DependencyObject root) where T : System.Windows.DependencyObject
    {
        if (root is T match) yield return match;
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); i++)
            foreach (var child in Descendants<T>(System.Windows.Media.VisualTreeHelper.GetChild(root, i))) yield return child;
    }

    static void CheckControlObjects(bool floating)
    {
        string label = floating ? "float renderer" : "integer renderer";
        var probe = new Probe(); var state = new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(floating)), probe); state.Initialize();
        var controls = new ToolConfigurationControlRegistry(); ToolConfigurationStandardControls.Register(controls);
        // Build the real control objects without creating a Window, HWND, popup or application process.
        using var view = new ToolConfigurationView(state, controls);
        view.Measure(new System.Windows.Size(600, 500)); view.Arrange(new System.Windows.Rect(0, 0, 600, 500)); view.UpdateLayout();
        var spinners = Descendants<ConfigurationNumericSpinner>(view).ToArray();
        Check(spinners.Length == 2 && !view.Diagnostics.Any(d => d.Code == "UnsupportedControl"), label + " resolves the production composite factory to two numeric child controls");
        Check(spinners.Select(s => ((ConfigurationNodeState)s.Tag).Name).SequenceEqual(new[] { "V0", "V1" }), label + " production controls retain exact source child identities");
        Check(spinners.All(s => s.Increment == (floating ? 0.25 : 2) && s.DecimalDigits == 2), label + " consumes composite increment and display precision");
        Check(spinners[0].Maximum == 8 && spinners[1].Minimum == 2, label + " couples bounds to the other endpoint");
        int writesBeforeBounds = probe.Writes.Count;
        state.SetProgrammatic("Range", "MinValue", "-6"); state.SetProgrammatic("Range", "MaxValue", "6");
        Check(spinners[0].Minimum == -6 && spinners[0].Maximum == 6 && spinners[1].Maximum == 6, label + " intersects composite limits with child and sibling limits");
        Check(probe.Writes.Count == writesBeforeBounds && state.Find("V1")!.Value == "8", label + " metadata refresh does not silently write an out-of-range backend value");
        state.SetProgrammatic("Range", "MinValue", "-10"); state.SetProgrammatic("Range", "MaxValue", "10");
        spinners[0].Step(1);
        Check(state.Find("V0")!.Value == (floating ? "2.25" : "4") && state.Find("V1")!.Value == "8", label + " real spinner action writes only its own parameter");
        Check(spinners[1].Minimum == (floating ? 2.25 : 4), label + " refreshes coupled bounds after endpoint commit");
        state.SetProgrammatic("Range", "SubscribeVisible", "False");
        Check(spinners.All(s => ((System.Windows.Controls.Grid)s.Parent).Children.OfType<System.Windows.Controls.Primitives.ToggleButton>().Single().Visibility == System.Windows.Visibility.Collapsed), label + " composite subscription visibility controls both mode buttons");
        state.SetProgrammatic("V0", "AccessMode", "R"); var value = state.Find("V0")!.Value; spinners[0].Step(1);
        Check(spinners[0].IsReadOnly && state.Find("V0")!.Value == value, label + " real control rejects read-only stepping");
        var binding = new ConfigurationSubscriptionBinding(new ConfigurationOutputReference("p", "src", "out", floating ? "float" : "int"), ConfigurationSubscriptionIndex.All);
        state.ApplySubscription(state.Find("V1")!, new(ConfigurationSubscriptionStatus.Available, binding, "src.out"), true);
        Check(spinners[1].Visibility == System.Windows.Visibility.Collapsed && spinners[0].Visibility == System.Windows.Visibility.Visible, label + " subscription replaces only the selected numeric editor");
        Check(spinners.All(s => !s.SliderPopup.IsOpen) && System.Windows.PresentationSource.FromVisual(view) is null, label + " does not create a presentation source or open any popup");
    }

    static void CheckReset(bool floating)
    {
        string label = floating ? "float reset" : "integer reset";
        var probe = new Probe(); var state = new ToolConfigurationState(ToolConfigurationDefinition.Parse(Xml(floating)), probe); state.Initialize();
        var service = new Subscriptions();
        foreach (var child in state.GetNumericRangeChildren(state.Find("Range")!))
        {
            var binding = new ConfigurationSubscriptionBinding(new ConfigurationOutputReference("p", "source", child.Name!, floating ? "float" : "int"), ConfigurationSubscriptionIndex.All);
            state.ApplySubscription(child, new(ConfigurationSubscriptionStatus.Available, binding, "source." + child.Name), true);
            state.PersistSubscriptionMode(child, true);
        }
        var descriptor = new ModuleDescriptor("range-test", "1", "Range", [], [],
            [new("V0", floating ? ParameterKind.Number : ParameterKind.Integer, true), new("V1", floating ? ParameterKind.Number : ParameterKind.Integer, true)]);
        string Algorithm(int a, int b) => $"<Algorithm><ParamItem><Name>V0</Name><DefaultValue>{a}</DefaultValue></ParamItem><ParamItem><Name>V1</Name><DefaultValue>{b}</DefaultValue></ParamItem></Algorithm>";
        var result = ToolConfigurationAlgorithmReset.Execute(descriptor, state, probe, service, Algorithm(4, 6));
        Check(result.Error is null && service.Cleared.SequenceEqual(new[] { "V0", "V1" }), label + " shared reset unbinds both declared scalar children in order");
        Check(probe.User["@V0"] == "False" && probe.User["@V1"] == "False" && state.GetNumericRangeChildren(state.Find("Range")!).All(n => n.SubscriptionBinding is null), label + " shared reset clears each binding identity and stored mode");
        Check(probe.Values["V0"] == "4" && probe.Values["V1"] == "6" && state.Find("V0")!.Value == "4" && state.Find("V1")!.Value == "6", label + " backend defaults are reread into both child models");
        Check(probe.Writes.SequenceEqual(new[] { "V0", "V1" }), label + " reset never writes the composite parameter name");
        probe.FailWriteName = "V1";
        result = ToolConfigurationAlgorithmReset.Execute(descriptor, state, probe, service, Algorithm(3, 7));
        Check(result.Error is not null && probe.Values["V0"] == "3" && probe.Values["V1"] == "6", label + " later reset failure preserves earlier successful backend write");
        probe.FailWriteName = null;
        result = ToolConfigurationAlgorithmReset.Execute(descriptor, state, probe, service, Algorithm(3, 7));
        Check(result.Error is null && state.Find("V0")!.Value == "3" && state.Find("V1")!.Value == "7", label + " failed reset can be rerun and read back consistently");
    }

    sealed class Subscriptions : IToolConfigurationSubscriptionService
    {
        public readonly List<string> Cleared = [];
        public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query) => new(ConfigurationSubscriptionStatus.Available, []);
        public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination) => new(ConfigurationSubscriptionStatus.Available, null);
        public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination, ConfigurationSubscriptionBinding? binding)
        { Cleared.Add(destination.Name); return new(ConfigurationSubscriptionStatus.Available, binding, ""); }
    }

    static void CheckRealTool(string output)
    {
        var plugin = new ContourMatchPlugin();
        var values = plugin.Descriptor.Parameters.Where(p => p.DefaultValue.HasValue).ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
        values["ScaleStart"] = JsonSerializer.SerializeToElement(0.75); values["ScaleEnd"] = JsonSerializer.SerializeToElement(1.25);
        var node = new NodeDocument("contour", plugin.Descriptor.Id, plugin.Descriptor.Version, "Contour", values);
        int runCalls = 0;
        ToolConfigurationSession Open(NodeDocument target, Action<Dictionary<string, JsonElement>> apply) => plugin.CreateConfiguration(new(target, output, apply, _ => { runCalls++; throw new InvalidOperationException("Execution is outside this configuration test."); }, 0));
        using (var session = Open(node, next => values = next))
        {
            var state = session.State;
            var ranges = state.Nodes.Where(n => n.Element.Tag is "IntegerBettween" or "FloatBettween").ToArray();
            Check(ranges.Length >= 8 && ranges.Any(n => n.Element.Tag == "FloatBettween"), "real contour configuration materializes its declared mixed range family");
            foreach (var range in ranges)
            {
                var children = state.GetNumericRangeChildren(range);
                Check(children.Count == 2 && children.All(n => plugin.Descriptor.Parameters.Any(p => p.Name == n.Name)), "real source endpoints map to backend descriptor: " + range.Name);
            }
            Check(state.Find("ScaleStart")!.Value == "0.75" && state.Find("ScaleEnd")!.Value == "1.25", "real contour reads persisted floating endpoints");
            state.CommitUser("ScaleStart", "0.85"); state.CommitUser("ScaleEnd", "1.45");
            Check(values["ScaleStart"].GetDouble() == 0.85 && values["ScaleEnd"].GetDouble() == 1.45, "real provider applies both floating endpoints through public configuration callbacks");
            Check(!values.ContainsKey("RangScale"), "real provider never invents a composite execution parameter");
            state.CommitUser("AngleStart", "-20");
            Check(values["AngleStart"].GetInt32() == -20, "real provider retains integer endpoint operation");
        }
        node = node with { Parameters = values };
        var path = Path.Combine(output, "numeric-ranges.vmx.json");
        SolutionStore.Save(path, new(SolutionStore.CurrentVersion, "solution", "Ranges", [new("p", "P", [node], [])], []));
        var reopened = SolutionStore.Load(path).Procedures.Single().Nodes.Single();
        using (var session = Open(reopened, next => values = next))
        {
            Check(session.State.Find("ScaleStart")!.Value == "0.85" && session.State.Find("ScaleEnd")!.Value == "1.45", "real tool floating endpoint state survives solution save and reopen");
            Check(session.State.Find("AngleStart")!.Value == "-20", "real tool integer endpoint state survives solution save and reopen");
            session.State.CommitUser("ScaleStart", "99");
            Check(values["ScaleStart"].GetDouble() == 0.85 && session.State.Find("ScaleStart")!.Value == "0.85", "real tool rejects invalid edit after reopen without losing prior state");
        }
        Check(runCalls == 0, "configuration and persistence never execute the unavailable contour matching kernel");
    }

    static void CheckShippedTool(IToolConfigurationProvider provider, string module, Dictionary<string, JsonElement> defaults, string output)
    {
        var values = defaults;
        var node = new NodeDocument(module, module, "1.0.0", module, values);
        bool reject = false; int runCalls = 0;
        ToolConfigurationSession Open(NodeDocument target) => provider.CreateConfiguration(new(target, output,
            next => { if (reject) throw new InvalidOperationException("Rejected range edit"); values = next; },
            _ => { runCalls++; throw new InvalidOperationException("No algorithm execution requested"); }, 0));
        using (var session = Open(node))
        {
            var state = session.State;
            var range = state.Find("EdgeThresh")!;
            Check(state.GetNumericRangeChildren(range).Select(n => n.Name).SequenceEqual(new[] { "EdgeThreshLow", "EdgeThreshHigh" }), module + " actual shipped provider owns source EdgeThresh members");
            state.CommitUser("CalibBoardType", "2");
            Check(range.Visible && state.Find("EdgeThreshLow")!.Visible && state.Find("EdgeThreshHigh")!.Visible, module + " circle mode exposes both child models through source visibility rules");
            state.CommitUser("EdgeThreshLow", "25");
            Check(values["EdgeThreshLow"].GetInt32() == 25 && values["EdgeThreshHigh"].GetInt32() == 40, module + " low threshold writes exact leaf without replacing high threshold");
            state.CommitUser("EdgeThreshHigh", "45");
            Check(values["EdgeThreshHigh"].GetInt32() == 45 && values["EdgeThreshLow"].GetInt32() == 25, module + " high threshold writes exact leaf without replacing low threshold");
            Check(!values.ContainsKey("EdgeThresh"), module + " no composite backend parameter is invented");
            reject = true; state.CommitUser("EdgeThreshLow", "30"); reject = false;
            Check(values["EdgeThreshLow"].GetInt32() == 25 && state.Find("EdgeThreshLow")!.Value == "25", module + " rejected host apply preserves committed endpoint");
            state.CommitUser("CalibBoardType", "1");
            Check(!range.Visible && !state.Find("EdgeThreshLow")!.CustomVisible && !state.Find("EdgeThreshHigh")!.CustomVisible, module + " source branch hides both declared child models");
        }
        node = node with { Parameters = values };
        var path = Path.Combine(output, module + ".vmx.json");
        SolutionStore.Save(path, new(SolutionStore.CurrentVersion, "s", module, [new("p", "P", [node], [])], []));
        using (var reopened = Open(SolutionStore.Load(path).Procedures.Single().Nodes.Single()))
        {
            Check(reopened.State.Find("EdgeThreshLow")!.Value == "25" && reopened.State.Find("EdgeThreshHigh")!.Value == "45", module + " actual provider reconstructs both endpoints after solution save and reopen");
            Check(!reopened.State.Find("EdgeThresh")!.Visible, module + " source visibility branch survives reopen");
        }
        Check(runCalls == 0, module + " configuration verification never invokes a calibration algorithm");
    }

    sealed class Probe : IToolConfigurationDispatcher
    {
        public readonly Dictionary<string, string> Values = new() { ["V0"] = "2", ["V1"] = "8", ["V2"] = "8" };
        public readonly Dictionary<string, string> User = [];
        public readonly List<ConfigurationOperationRequest> Requests = [];
        public readonly List<string> Writes = [], Clears = [];
        public bool FailWrites;
        public string? FailReadName, FailClearName, FailWriteName;
        public ConfigurationOperationResult Dispatch(ConfigurationOperationRequest request)
        {
            Requests.Add(request);
            string name = request.Source.Name ?? "";
            return request.Kind switch
            {
                ConfigurationOperationKind.ReadParameter => name == FailReadName ? new(true, Error: "read failed") : new(true, Values.GetValueOrDefault(name)),
                ConfigurationOperationKind.ReadUserValue => new(true, User.GetValueOrDefault(request.Parameters.FirstOrDefault() ?? "", "False")),
                ConfigurationOperationKind.PersistUserValue => Persist(request),
                ConfigurationOperationKind.WriteParameter => Write(request),
                ConfigurationOperationKind.ClearParameterBinding => Clear(request),
                _ => new(false)
            };
        }
        ConfigurationOperationResult Persist(ConfigurationOperationRequest request) { User[request.Parameters[0]] = request.Value!; return new(true); }
        ConfigurationOperationResult Write(ConfigurationOperationRequest request)
        {
            Writes.Add(request.Source.Name!);
            if (FailWrites || request.Source.Name == FailWriteName) return new(true, Error: "write failed");
            Values[request.Source.Name!] = request.Value!;
            return new(true, request.Value);
        }
        ConfigurationOperationResult Clear(ConfigurationOperationRequest request)
        {
            Clears.Add(request.Source.Name!);
            return request.Source.Name == FailClearName ? new(true, Error: "clear failed") : new(true);
        }
    }
}
