using VisionEditor;

if (args.Length != 1) throw new ArgumentException("Pass an owned audit directory.");
var root = Path.Combine(Path.GetFullPath(args[0]), "run-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
int passed = 0;
void Check(bool value, string name)
{
    if (!value) throw new InvalidOperationException(name);
    Console.WriteLine("PASS " + name);
    passed++;
}
string Wrap(string field) => "<Config>" + field + "</Config>";
string Field(string text) => "<IsClearLastExecuteResult><CurValue>" + text + "</CurValue></IsClearLastExecuteResult>";
var cases = new (string Name, string Xml, bool Expected)[]
{
    ("enabled", Wrap(Field("1")), true),
    ("disabled", Wrap(Field("0")), false),
    ("bool-text-is-not-one", Wrap(Field("true")), false),
    ("leading-space-is-not-one", Wrap(Field(" 1")), false),
    ("missing-field", "<Config/>", false),
    ("missing-value", Wrap("<IsClearLastExecuteResult/>"), false),
    ("duplicate-field", Wrap(Field("1") + Field("1")), false),
    ("duplicate-value", Wrap("<IsClearLastExecuteResult><CurValue>1</CurValue><CurValue>1</CurValue></IsClearLastExecuteResult>"), false),
    ("nested-field-is-not-direct", Wrap("<Nested>" + Field("1") + "</Nested>"), false),
    ("namespace-does-not-match", "<Config xmlns='urn:other'>" + Field("1") + "</Config>", false),
    ("malformed", "<Config>", false),
    ("root-name-not-constrained", "<Other>" + Field("1") + "</Other>", true),
};
foreach (var c in cases)
{
    var path = Path.Combine(root, c.Name + ".cfg");
    File.WriteAllText(path, c.Xml);
    var preferences = new FrameworkExecutionPreferences(path);
    Check(preferences.IsClearLastExecuteResult == c.Expected, c.Name);
}
var missing = Path.Combine(root, "missing.cfg");
var cachedMissing = new FrameworkExecutionPreferences(missing);
Check(!cachedMissing.IsClearLastExecuteResult, "missing-file-is-false");
File.WriteAllText(missing, Wrap(Field("1")));
Check(!cachedMissing.IsClearLastExecuteResult, "read-failure-property-cache-does-not-retry");
Check(new FrameworkExecutionPreferences(missing).IsClearLastExecuteResult, "new-startup-snapshot-reloads");
var ownPath = Path.Combine(root, "Owned.cfg");
File.WriteAllText(ownPath, Wrap(Field("1")));
var policy = FrameworkExecutionPreferences.FromWorkingDirectory("Owned", root);
Check(policy.IsClearLastExecuteResult, "explicit-application-name-and-working-directory");
File.WriteAllText(ownPath, Wrap(Field("0")));
Check(policy.IsClearLastExecuteResult, "successful-property-cache-does-not-watch");
Check(!policy.ShouldClearAtBegin(0, 1), "no-count-not-begin-clear");
Check(!policy.ShouldClearAtBegin(1, 1), "first-count-not-cleared");
Check(!policy.ShouldClearAtBegin(2, 0), "end-status-not-cleared");
Check(policy.ShouldClearAtBegin(2, 1), "second-begin-clears");
Check(policy.ShouldClearAtBegin(uint.MaxValue, 3), "nonzero-status-and-count-guard");
policy.IsClearLastExecuteResult = false;
Check(!policy.ShouldClearAtBegin(2, 1), "in-memory-disabled-keeps-result");
policy.IsClearLastExecuteResult = true;
Check(File.ReadAllText(ownPath) == Wrap(Field("0")), "setter-never-writes-config");
Console.WriteLine($"PASS {passed}/{passed}; owned audit: {root}");
