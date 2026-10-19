using System;
using System.IO;
using System.Reflection;
using VMControls.Interface;
using VM.UnRedo;

// Synthetic actions; the scheduler/service are the unmodified shipped implementation.
class Root : IUnRedoOperation, IRootParam, IDisposable {
 public bool IsPart {get;set;} public bool ForbidBuild {get;set;} public string RootOperType {get;set;}
 public void Undo(){} public void Redo(){} public void Dispose(){}
}
class Part : DisplayNotify, IUnRedoOperation, IUnRedoActionOperation, IDisposable {
 public string Name; public bool ThrowUpdate; public int Updates, Undos, Redos;
 public bool IsPart {get;set;} public IUnRedoParams Params {get;set;} public IBuilder ParentNode {get;set;}
 public bool IsDataComplete {get;set;} public ExecutePriority ExecutePriority {get;set;}
 public Part(string n){Name=n;IsPart=true;}
 public void Update(){Updates++;Probe.Log("update:"+Name+":complete="+IsDataComplete);if(ThrowUpdate)throw new Exception("probe update failure");}
 public void Undo(){Undos++;Probe.Log("undo:"+Name);}
 public void Redo(){Redos++;Probe.Log("redo:"+Name);}
 public void Dispose(){Probe.Log("dispose:"+Name);}
}
class Probe {
 static int checks; public static void Log(string s){Console.WriteLine(s);}
 static void Check(bool b,string s){Log((b?"PASS ":"FAIL ")+s);if(!b)throw new Exception(s);checks++;}
 static object Stack(UnRedoService s,string name){return s.GetScheduleInstance().GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(s.GetScheduleInstance());}
 static int Count(UnRedoService s,string name){return (int)Stack(s,name).GetType().GetProperty("Count").GetValue(Stack(s,name),null);}
 static IBuilder Top(UnRedoService s){return (IBuilder)Stack(s,"_undoStack").GetType().GetMethod("Peek").Invoke(Stack(s,"_undoStack"),null);}
 static void R(UnRedoService s){s.Register(new Root{RootOperType="ModifyConfig"},"probe",null);}
 [STAThread] static int Main(){try{
  string baseDir=Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory); string marker=Path.Combine(baseDir,"C01-owned-directory.marker");
  if(!File.Exists(marker)||!baseDir.Contains("batch1-20261019")||!baseDir.Contains("C01"))throw new Exception("Refusing unowned BaseDirectory");
  Log("base="+baseDir);Log("clr="+Environment.Version);Log("service="+typeof(UnRedoService).Assembly.FullName);Log("service-path="+typeof(UnRedoService).Assembly.Location);
  var s=new UnRedoService();Check(!Directory.Exists(Path.Combine(baseDir,"TempFiles")),"constructor deleted only probe-owned TempFiles sentinel");
  s.SetForbidRecord(true);R(s);Check(!((IRootParam)Top(s)).ForbidBuild,"empty-stack forbid has no effect on later root");
  var root1=Top(s);s.SetForbidRecord(true);s.SetForbidRecord(true);s.SetForbidRecord(false);Check(!((IRootParam)root1).ForbidBuild,"forbid is bool, not nested scope");
  s.SetForbidRecord(true);var rejected=new Part("rejected");s.Register(rejected,"reject",null);Check(root1.Actions.Count==0,"forbidden root rejects part");
  R(s);var root2=Top(s);Check(!Object.ReferenceEquals(root1,root2),"root register allowed while prior root forbidden");s.SetForbidRecord(false);Check(((IRootParam)root1).ForbidBuild&&!((IRootParam)root2).ForbidBuild,"false changes current root only");
  var p=new Part("normal");s.Register(p,"part",null);Check(root2.Actions.Count==1&&p.Updates==0,"Register part does not Update");s.Undo(null);Check(p.IsDataComplete&&p.Updates==1&&p.Undos==1,"first Undo captures before action");Check(Count(s,"_redoStack")==1,"Undo moves root to redo");s.Redo(null);Check(p.Redos==1&&p.Updates==1,"Redo does not recapture");
  R(s);var bad=new Part("throwing"){ThrowUpdate=true};s.Register(bad,"bad",null);int before=Count(s,"_undoStack");s.Undo(null);Check(bad.IsDataComplete&&bad.Updates==1&&bad.Undos==0,"throw Update marks complete, swallows exception before action");Check(Count(s,"_undoStack")==before&&!s.IsInUnRedo(),"failed Update leaves root on undo and clears counter");s.Undo(null);Check(bad.Updates==1&&bad.Undos==1,"second Undo skips failed capture and executes Undo");
  Log("checks="+checks);foreach(var a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name.StartsWith("VM"))Log("loaded="+a.FullName+"|"+a.Location);return 0;
 }catch(Exception e){Console.WriteLine(e);return 1;}}
}
