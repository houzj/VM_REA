using System.Text.Json;
using VisionEditor.Core;
using VisionFramework;

var results = new List<string>();
void Check(bool condition, string name) { if (!condition) throw new Exception(name); results.Add(name); }
GraphEditorDocument Fresh() => new(new ModuleRegistry());
void Write(GraphEditorDocument d, string name) => d.SetWorkflowConfiguration(d.CurrentProcedureId,
    d.CurrentProcedure.Configuration with { Groups = [new("g", name)] });
string Value(GraphEditorDocument d) => d.CurrentProcedure.Configuration.Groups.FirstOrDefault()?.Name ?? "";
void Throws(Action action, string name)
{
    try { action(); } catch (InvalidOperationException e) when (e.Message == "fixture") { Check(true, name); return; }
    throw new Exception("Expected fixture exception: " + name);
}

var normal = Fresh();
long revision = normal.Revision;
normal.ExecuteConfigurationEdit(d => { Write(d, "first"); Write(d, "second"); });
Check(Value(normal) == "second" && normal.Revision == revision + 2, "Suppressed parts still write actual workflow configuration");
normal.Undo();
Check(Value(normal) == "" && !normal.CanUndo, "Two configuration writes occupy one lazily created history root");
normal.Redo();
Check(Value(normal) == "second" && !normal.CanRedo, "Redo restores final configuration");
Write(normal, "third"); normal.Undo();
Check(Value(normal) == "second", "Normal edit records after scope completion");

var empty = Fresh();
empty.ExecuteConfigurationEdit(_ => { });
Check(!empty.CanUndo && !empty.CanRedo, "Empty configuration does not manufacture a root");
empty.ExecuteConfigurationEdit(d => d.SetWorkflowConfiguration(d.CurrentProcedureId, d.CurrentProcedure.Configuration));
Check(!empty.CanUndo, "Equal configuration does not manufacture a root");
Throws(() => empty.ExecuteConfigurationEdit(_ => throw new InvalidOperationException("fixture")), "Exception before first write propagates");
Check(!empty.CanUndo, "Exception before first write leaves empty history");
empty.SetHistoryRecordingForbidden(true);
Write(empty, "one"); Write(empty, "two"); empty.Undo();
Check(Value(empty) == "one", "Raw forbid on empty stack has no later effect");

var failure = Fresh();
Throws(() => failure.ExecuteConfigurationEdit(d => { Write(d, "kept"); throw new InvalidOperationException("fixture"); }), "Exception after write propagates");
Check(Value(failure) == "kept", "Exception cleanup does not invent a configuration rollback");
Write(failure, "later"); failure.Undo();
Check(Value(failure) == "kept", "Finally resets recording after failed configuration callback");
failure.Undo();
Check(Value(failure) == "", "Failed configuration's completed write remains undoable");
failure.Redo(); failure.Redo();
Check(Value(failure) == "later", "Redo survives callback failure and subsequent edit");

var notification = Fresh();
void FailNotification() => throw new InvalidOperationException("fixture");
notification.Changed += FailNotification;
Throws(() => notification.ExecuteConfigurationEdit(d => Write(d, "written")), "Actual Changed subscriber exception propagates");
notification.Changed -= FailNotification;
Write(notification, "next"); notification.Undo();
Check(Value(notification) == "written", "Changed subscriber exception also clears root prohibition");

var existing = Fresh();
existing.RenameProcedure(existing.CurrentProcedureId, "renamed");
existing.ExecuteConfigurationEdit(d => { Write(d, "a"); Write(d, "b"); });
existing.Undo();
Check(Value(existing) == "" && existing.CurrentProcedure.Name == "renamed", "Configuration root preserves preceding unrelated history");
existing.Undo();
Check(existing.CurrentProcedure.Name == "Main", "Older root is separately undoable");
existing.Redo(); existing.Redo();
Check(Value(existing) == "b" && existing.CurrentProcedure.Name == "renamed", "Multiple roots redo in order");
existing.Undo();
existing.ExecuteConfigurationEdit(d => Write(d, "branch"));
Check(!existing.CanRedo, "Actual configuration edit invalidates redo branch");

var nested = Fresh();
nested.ExecuteConfigurationEdit(d => {
    Write(d, "outer");
    d.ExecuteConfigurationEdit(inner => Write(inner, "inner"));
    Write(d, "after-inner");
});
nested.Undo();
Check(Value(nested) == "inner", "Inner false re-enables outer recording: boolean, not nesting counter");
nested.Undo();
Check(Value(nested) == "" && !nested.CanUndo, "Outer and inner writes before inner false share their root");

var nestedEmpty = Fresh();
nestedEmpty.ExecuteConfigurationEdit(d => {
    d.ExecuteConfigurationEdit(_ => { });
    Write(d, "first"); Write(d, "second");
});
nestedEmpty.Undo();
Check(Value(nestedEmpty) == "first", "Empty inner call still sets boolean false before first lazy write");

var reset = Fresh();
reset.ExecuteConfigurationEdit(d => { Write(d, "old"); d.NewSolution(); Write(d, "new-one"); Write(d, "new-two"); });
reset.Undo();
Check(Value(reset) == "new-one", "Replacing solution does not carry configuration recording suppression");

var modal = Fresh();
string target = modal.AddProcedure("bound"); modal.SelectProcedure("main");
modal.ExecuteConfigurationEdit(d => {
    d.EditProcedure(target, draft => Write(draft, "modal-one"));
    d.EditProcedure(target, draft => Write(draft, "modal-two"));
});
string BoundValue(GraphEditorDocument d) => d.Document.Procedures.Single(p => p.Id == target).Configuration.Groups.FirstOrDefault()?.Name ?? "";
Check(modal.CurrentProcedureId == "main" && BoundValue(modal) == "modal-two", "Modal bound edit updates target without switching canvas");
modal.Undo();
Check(BoundValue(modal) == "" && modal.Document.Procedures.Any(p => p.Id == target), "Modal bound edits share one root above target creation");
modal.Redo();
Check(BoundValue(modal) == "modal-two", "Modal bound root replays its final target data");

var modeless = Fresh();
modeless.ExecuteConfigurationEdit(d => d.EditProcedure(d.CurrentProcedureId, draft => Write(draft, "modeless-one")));
modeless.ExecuteConfigurationEdit(d => d.EditProcedure(d.CurrentProcedureId, draft => Write(draft, "modeless-two")));
modeless.Undo();
Check(Value(modeless) == "modeless-one", "Modeless callback adapter creates a separate root per action");
modeless.Undo();
Check(Value(modeless) == "", "Modeless action roots stay independently undoable");

Console.WriteLine(JsonSerializer.Serialize(new { passed = true, checks = results.Count, results }, new JsonSerializerOptions { WriteIndented = true }));
