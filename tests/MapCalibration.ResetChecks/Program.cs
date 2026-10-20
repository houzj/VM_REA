using System.Text.Json;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using VisionFramework;
using VisionPluginContracts;
using VisionPlugins.MapCalibration;

static class Program
{
    static int checks;
    static JsonElement J<T>(T value) => JsonSerializer.SerializeToElement(value);
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); checks++; }
    [STAThread] static int Main()
    {
        try { _ = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown }; Scenario(false); Scenario(true); Console.WriteLine($"PASS {checks} map source reset checks"); return 0; }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
    static void Scenario(bool reject)
    {
        var plugin = new MapCalibrationPlugin();
        var backing = plugin.Descriptor.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue!.Value.Clone());
        backing["CalibPathName"] = J("preserved.xml"); backing["RefreshFileEnable"] = J(true);
        backing["InputType"] = J(2); backing["TeachEnable"] = J(true); backing["TeachFlag"] = J("preserve"); backing["HomoFreedom"] = J(3); backing["WeightFactor"] = J(71);
        backing["ConfigurationUserData"] = J(JsonSerializer.Serialize(new Dictionary<string,string> { ["@WeightFactor"]="True", ["Keep"]="unchanged" }));
        var confirmations = new Confirmations(); var notifications = new Notifications(); var subscriptions = new Subscriptions();
        int runs = 0, atomic = 0; bool record = false;
        string[] subNames=["TeachEnable","TeachFlag","CalibPathName","RefreshFileEnable","WeightFactor"];
        var userModes=JsonSerializer.Deserialize<Dictionary<string,string>>(backing["ConfigurationUserData"].GetString()!)!;
        foreach(string name in subNames)userModes["@"+name]="True";
        backing["ConfigurationUserData"]=J(JsonSerializer.Serialize(userModes));
        var writes = new List<string>();
        var context = new ToolEditorContext(new("board", MapCalibrationPlugin.Id, "1", "Board", backing), Environment.CurrentDirectory,
            next => {
                if (record) {
                    var changed = next.Keys.Where(k => !backing.TryGetValue(k, out var prior) || prior.GetRawText() != next[k].GetRawText()).ToArray();
                    if (reject && changed.Contains("HomoFreedom")) throw new InvalidOperationException("host rejected HomoFreedom");
                    writes.AddRange(changed);
                }
                backing = next;
            }, _ => { runs++; throw new Exception("reset must not run"); }) {
            ReadEffectiveParameters = () => backing, ConfigurationSubscriptions = subscriptions,
            ConfigurationConfirmations = confirmations, ConfigurationNotifications = notifications,
            ResetConfigurationParameters = _ => { atomic++; throw new Exception("reset must follow XML list"); }
        };
        var currentImages=typeof(MapCalibrationPlugin).GetMethod("CurrentImages",BindingFlags.NonPublic|BindingFlags.Instance)!;
        object Cached()=>currentImages.Invoke(plugin,["board",Environment.CurrentDirectory,null])!;
        var cached=Cached();
        using var session = plugin.CreateConfiguration(context);
        var state = session.State;
        // Do not materialize a subscription widget: persisted mode alone must be enough.
        subscriptions.Clears = 0;
        record = true; state.Invoke(state.Find("Button_ResetAlgoParams")!);
        Check(writes.Count == 0 && subscriptions.Clears == 0, "cancel leaves state untouched");
        confirmations.Allow = true; state.Invoke(state.Find("Button_ResetAlgoParams")!); record = false;
        Check(subscriptions.Clears == 5, "all five supported ISubInfo parameter modes clear without widget materialization");
        Check(runs == 0 && atomic == 0, "source sequential reset never invokes run or atomic host reset");
        Check(backing["CalibPathName"].GetString() == "preserved.xml" && backing["RefreshFileEnable"].GetBoolean(), "non-list path and refresh survive");
        Check(backing["InputType"].GetInt32()==2 && backing["TeachEnable"].GetBoolean() && backing["TeachFlag"].GetString()=="preserve", "mode/teach fields outside six defaults survive");
        var user = JsonSerializer.Deserialize<Dictionary<string,string>>(backing["ConfigurationUserData"].GetString()!)!;
        Check(subNames.All(n=>user["@"+n]=="False") && user["Keep"] == "unchanged", "only source subscription mode changes");
        using var resource = typeof(MapCalibrationPlugin).Assembly.GetManifestResourceStream("VisionPlugins.MapCalibration.Resources.Algorithm.xml")!;
        var names = XDocument.Load(resource).Root!.Elements("ParamItem").Select(e => e.Element("Name")!.Value).ToArray();
        Check(names.Length == 6 && !names.Contains("CalibPathName") && !names.Contains("RefreshFileEnable"), "actual active source defaults list");
        Check(backing["WeightFactor"].GetInt32() == 20, "later defaults continue after intermediate rejection");
        Check(backing["HomoFreedom"].GetInt32() == (reject ? 3 : 1), "host failure retains only rejected field");
        Check(notifications.Items.Last().Kind == (reject ? ConfigurationNotificationKind.Error : ConfigurationNotificationKind.Success), "final notification reflects sequential result");
        if (reject) Check(notifications.Items.Last().Message.Contains("host rejected HomoFreedom"), "write failure reported");
        Check(ReferenceEquals(cached,Cached()),"algorithm parameter reset preserves runtime cache identity");
        session.Dispose();Check(ReferenceEquals(cached,Cached()),"configuration close preserves runtime cache");
        plugin.Reset("board");Check(!ReferenceEquals(cached,Cached()),"explicit tool Reset removes own runtime cache");
    }
    sealed class Confirmations : IToolConfigurationConfirmationService { public bool Allow; public bool Confirm(Window? owner, ConfigurationConfirmation request) => Allow; }
    sealed class Notifications : IToolConfigurationNotificationService { public List<ConfigurationNotification> Items = []; public ConfigurationNotificationResult Notify(Window? owner, ConfigurationNotification value) { Items.Add(value); return new(ConfigurationNotificationStatus.Displayed); } }
    sealed class Subscriptions : IToolConfigurationSubscriptionService {
        public int Clears; readonly HashSet<string> bound = new(["TeachEnable","TeachFlag","CalibPathName","RefreshFileEnable","WeightFactor"]);
        ConfigurationSubscriptionBinding Binding => new(new ConfigurationConstantReference("int", "71"), ConfigurationSubscriptionIndex.All);
        public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query) => new(ConfigurationSubscriptionStatus.Available, []);
        public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination) => new(ConfigurationSubscriptionStatus.Available, bound.Contains(destination.Name) ? Binding : null);
        public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination, ConfigurationSubscriptionBinding? binding) { if (binding is null && bound.Contains(destination.Name)) { Clears++; bound.Remove(destination.Name); } return Read(destination); }
    }
}
