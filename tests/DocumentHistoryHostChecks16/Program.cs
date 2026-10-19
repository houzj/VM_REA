using VisionEditor.Core;
using VisionFramework;

int checks=0;
void Check(bool condition,string name){if(!condition)throw new Exception(name);checks++;Console.WriteLine("PASS "+name);}
GraphEditorDocument Fresh()=>new(new ModuleRegistry());
var doc=Fresh();string id=doc.CurrentProcedureId;
doc.SetContainerZoomIndex(id,2);doc.RenameProcedure(id,"first");doc.SetContainerZoomIndex(id,17);
doc.Undo();Check(doc.CurrentProcedure.Name=="Main"&&doc.CurrentProcedure.CanvasZoomIndex==2,"Real document part restores structural old and initial view");
doc.SetContainerZoomIndex(id,6);doc.Redo();
Check(doc.CurrentProcedure.Name=="first"&&doc.CurrentProcedure.CanvasZoomIndex==17,"Redo restores structural new payload");
doc.Undo();Check(doc.CurrentProcedure.CanvasZoomIndex==6,"View-only change between Undo/Redo becomes reverse view destination");
doc.Redo();doc.SetContainerZoomIndex(id,11);doc.Undo();doc.Redo();
Check(doc.CurrentProcedure.Name=="first"&&doc.CurrentProcedure.CanvasZoomIndex==11,"Late view change after Redo remains view data without replacing structure");
doc.RenameProcedure(id,"second");doc.Undo();doc.RenameProcedure(id,"branch");
Check(!doc.CanRedo&&doc.CurrentProcedure.Name=="branch","Admitted ordinary document part invalidates redo branch");
var limited=Fresh();for(int i=1;i<=20;i++)limited.RenameProcedure(limited.CurrentProcedureId,"name"+i);
int undos=0;while(limited.CanUndo){limited.Undo();undos++;}
Check(undos==15&&limited.CurrentProcedure.Name=="name5","Real document uses original default fifteen-root capacity");
for(int i=0;i<15;i++)limited.Redo();Check(limited.CurrentProcedure.Name=="name20","All retained roots redo in original order");
var failure=Fresh();failure.RenameProcedure(failure.CurrentProcedureId,"written");
void RejectNotification()=>throw new InvalidOperationException("fixture-notification");
failure.Changed+=RejectNotification;failure.Undo();failure.Changed-=RejectNotification;
Check(failure.LastHistoryError?.Message=="fixture-notification"&&failure.CurrentProcedure.Name=="Main"&&failure.CanRedo,"Action notification failure is retained after stack transfer without invented rollback");
failure.Redo();Check(failure.LastHistoryError is null&&failure.CurrentProcedure.Name=="written","Replay admission resets after a caught action failure");
var grouped=Fresh();grouped.ExecuteConfigurationEdit(d=>{d.RenameProcedure(d.CurrentProcedureId,"one");d.RenameProcedure(d.CurrentProcedureId,"two");});
grouped.Undo();Check(grouped.CurrentProcedure.Name=="Main"&&!grouped.CanUndo,"Configuration scope consumes one real document root/part");
grouped.Redo();Check(grouped.CurrentProcedure.Name=="two","First Undo captured final configuration new data");
grouped.NewSolution();Check(!grouped.CanUndo&&!grouped.CanRedo&&grouped.LastHistoryError is null,"Solution replacement clears actual engine roots and diagnostics");
Console.WriteLine($"PASS {checks}/{checks}; actual GraphEditorDocument engine adapter, no SDK payload or native parity");
