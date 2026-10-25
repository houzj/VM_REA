using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using VisionFramework;
using VisionCore;

if(args.Length!=2)throw new ArgumentException("Usage: ScopeRetirementProbe <workspace> <report>");
string workspace=Path.GetFullPath(args[0]),reportPath=Path.GetFullPath(args[1]);
var checks=new List<object>();
void Check(bool condition,string id,string name,object observation){if(!condition)throw new InvalidOperationException("CHECK FAILED: "+id+" "+name);checks.Add(new{id,name,passed=true,observation});}
bool Throws(Action action){try{action();return false;}catch(InvalidOperationException){return true;}}
bool Closed(Action action){try{action();return false;}catch(ObjectDisposedException){return true;}}
IEnumerable<object> PrivateList(ModuleExecutionScope scope,string field)=>(IEnumerable<object>)(typeof(ModuleExecutionScope).GetField(field,BindingFlags.Instance|BindingFlags.NonPublic)?.GetValue(scope)??throw new InvalidDataException("Private field not found: "+field));
string Hash(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
var prior=JsonDocument.Parse(File.ReadAllBytes(Path.Combine(workspace,"docs/architecture-recovery/current-group-data-probe.json")));
var priorAssemblies=prior.RootElement.GetProperty("assemblies").EnumerateArray().ToArray();
foreach(var type in new[]{typeof(ModuleExecutionScope),typeof(GrayImage)}){
 string name=Path.GetFileName(type.Assembly.Location);var entry=priorAssemblies.Single(a=>Path.GetFileName(a.GetProperty("path").GetString())==name);
 if(Hash(type.Assembly.Location)!=entry.GetProperty("sha256").GetString())throw new InvalidDataException("Reused production DLL differs: "+name);
}

// Replacement mutation happens before the old service's failing Dispose returns.
{
 var scope=new ModuleExecutionScope();var old=new DisposableProbe("old-service",true);var next=new DisposableProbe("new-service");scope.RegisterService<DisposableProbe>(old);
 bool threw=Throws(()=>scope.RegisterService<DisposableProbe>(next));
 Check(threw&&ReferenceEquals(scope.Service<DisposableProbe>(),old)&&!PrivateList(scope,"ownedServices").Contains(old)&&!PrivateList(scope,"ownedServices").Contains(next)&&old.DisposeCalls==1&&next.DisposeCalls==0&&scope.CleanupFailures.Count==0,"replacement.partial-state","Failed service replacement keeps the old mapping after removing its owned marker",new{threw,oldMappingStillPresent=ReferenceEquals(scope.Service<DisposableProbe>(),old),oldOwned=false,newOwned=false,oldCalls=old.DisposeCalls,newCalls=next.DisposeCalls,failures=scope.CleanupFailures.Count});
 scope.Dispose();
 Check(old.DisposeCalls==1&&next.DisposeCalls==0&&scope.Service<DisposableProbe>()is null&&scope.CleanupFailures.Count==0,"replacement.no-retry","Root close does not retry this removed service ownership marker",new{oldCalls=old.DisposeCalls,newCalls=next.DisposeCalls,serviceAbsent=true,failures=scope.CleanupFailures.Count});
}
// A failed RemoveNode has already removed that entry; root close sees the replacement only.
{
 var scope=new ModuleExecutionScope();var failed=scope.State("probe","node",()=>new DisposableProbe("removed-state",true));var replacement=new DisposableProbe("replacement-state");
 bool threw=Throws(()=>scope.RemoveNode("probe","node"));int created=0;var current=scope.State("probe","node",()=>{created++;return replacement;});
 Check(threw&&created==1&&ReferenceEquals(current,replacement)&&failed.DisposeCalls==1&&scope.CleanupFailures.Count==0,"remove.removed-before-error","RemoveNode propagates failure after removing the old state",new{threw,recreated=created,oldCalls=failed.DisposeCalls,failures=scope.CleanupFailures.Count});
 scope.Dispose();Check(failed.DisposeCalls==1&&replacement.DisposeCalls==1,"remove.root-close","Root close disposes the current replacement rather than retrying the removed object",new{oldCalls=failed.DisposeCalls,replacementCalls=replacement.DisposeCalls});
}
// Two inserted states expose the observed first-error boundary; order is this finite runtime's observation.
{
 var scope=new ModuleExecutionScope();var first=scope.State("probe","first",()=>new DisposableProbe("first",true));var second=scope.State("probe","second",()=>new DisposableProbe("second"));
 bool threw=Throws(()=>scope.RetainProcedureNodes(Array.Empty<(string Procedure,string Module,string Node)>()));
 var retained=scope.State<DisposableProbe>("probe","second",()=>throw new InvalidOperationException("Second state was unexpectedly removed"));
 Check(threw&&first.DisposeCalls==1&&second.DisposeCalls==0&&ReferenceEquals(retained,second)&&scope.CleanupFailures.Count==0,"retain.first-error","RetainProcedureNodes stops at the observed first disposal failure, leaving the later state",new{threw,firstCalls=first.DisposeCalls,secondCalls=second.DisposeCalls,secondStillSame=true,failures=scope.CleanupFailures.Count,qualification="Observed insertion/iteration order for these two states; not a general ordering guarantee."});
 scope.Dispose();Check(first.DisposeCalls==1&&second.DisposeCalls==1,"retain.later-root-close","The remaining state can be retired by a later root close",new{firstCalls=first.DisposeCalls,secondCalls=second.DisposeCalls});
}
// Root closing blocks new leases/state, but Service is a distinct query until release.
{
 var scope=new ModuleExecutionScope();var state=scope.State("probe","node",()=>new DisposableProbe("leased-state"));var service=new DisposableProbe("leased-service");scope.RegisterService<DisposableProbe>(service);var lease=scope.Enter();scope.Dispose();
 bool stateRejected=Closed(()=>scope.State("probe","node",()=>new DisposableProbe("unexpected"))),enterRejected=Closed(()=>scope.Enter());
 Check(stateRejected&&enterRejected&&ReferenceEquals(scope.Service<DisposableProbe>(),service)&&state.DisposeCalls==0&&service.DisposeCalls==0,"closing.before-lease-exit","Closing rejects State and Enter while a live lease keeps the old service queryable",new{stateRejected,enterRejected,serviceStillSame=true,stateCalls=state.DisposeCalls,serviceCalls=service.DisposeCalls});
 lease.Dispose();Check(state.DisposeCalls==1&&service.DisposeCalls==1&&scope.Service<DisposableProbe>()is null,"closing.after-lease-exit","Final lease exit releases both state and owned service and clears lookup",new{stateCalls=state.DisposeCalls,serviceCalls=service.DisposeCalls,serviceAbsent=true});
 lease.Dispose();scope.Dispose();Check(state.DisposeCalls==1&&service.DisposeCalls==1,"closing.idempotent-exit","Repeated lease and root Dispose do not run release again",new{stateCalls=state.DisposeCalls,serviceCalls=service.DisposeCalls});
}
// Cancellation and resource failures are diagnostic retention, not a retry queue.
{
 var scope=new ModuleExecutionScope();var failed=scope.State("probe","bad",()=>new DisposableProbe("bad-resource",true));var later=scope.State("probe","later",()=>new DisposableProbe("later-resource"));int cancellationCalls=0;
 var registration=scope.Stopping.Register(()=>{cancellationCalls++;throw new InvalidOperationException("synthetic cancellation callback failure");});
 scope.Dispose();var phases=scope.CleanupFailures.Select(f=>f.Phase).ToArray();
 Check(cancellationCalls==1&&failed.DisposeCalls==1&&later.DisposeCalls==1&&phases.SequenceEqual(new[]{"cancel","resource"}),"release.continue-after-errors","Cancellation callback and one resource failure do not prevent the later resource disposal",new{cancellationCalls,failedCalls=failed.DisposeCalls,laterCalls=later.DisposeCalls,phases});
 var held=PrivateList(scope,"failedResources").ToArray();Check(held.Length==2&&held.Contains(failed),"release.failed-holders","Private failedResources retains the actual failed resource plus cancellation source",new{retainedObjectCount=held.Length,hasFailedResourceReference=true,resourceTypes=held.Select(o=>o.GetType().FullName),inspection="Read-only reflection of current managed scope fields; no mutation."});
 scope.Dispose();scope.Dispose();Check(cancellationCalls==1&&failed.DisposeCalls==1&&later.DisposeCalls==1&&scope.CleanupFailures.Count==2&&PrivateList(scope,"failedResources").Count()==2,"release.no-retry","Repeated root Dispose preserves diagnostics without retrying failed cleanup",new{cancellationCalls,failedCalls=failed.DisposeCalls,laterCalls=later.DisposeCalls,failures=scope.CleanupFailures.Count,retainedObjectCount=PrivateList(scope,"failedResources").Count()});
}

var assemblies=new List<object>();var sourcePaths=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach(var assembly in new[]{typeof(ModuleExecutionScope).Assembly,typeof(GrayImage).Assembly,Assembly.GetExecutingAssembly()}.Distinct()){
 string dll=assembly.Location,pdb=Path.ChangeExtension(dll,"pdb");using var stream=File.OpenRead(dll);using var pe=new PEReader(stream);var debug=pe.ReadDebugDirectory().Single(e=>e.Type==DebugDirectoryEntryType.CodeView);var cv=pe.ReadCodeViewDebugDirectoryData(debug);using var ps=File.OpenRead(pdb);using var provider=MetadataReaderProvider.FromPortablePdbStream(ps);var reader=provider.GetMetadataReader();var id=new BlobContentId(reader.DebugMetadataHeader!.Id);if(cv.Guid!=id.Guid||debug.Stamp!=id.Stamp)throw new InvalidDataException("Loaded DLL/PDB mismatch");
 var docs=new List<object>();foreach(var handle in reader.Documents){var doc=reader.GetDocument(handle);string name=reader.GetString(doc.Name);var algorithm=reader.GetGuid(doc.HashAlgorithm);var expected=reader.GetBlobBytes(doc.Hash);var bytes=File.ReadAllBytes(name);var actual=algorithm==new Guid("8829d00f-11b8-4213-878b-770e8597ac16")?SHA256.HashData(bytes):algorithm==new Guid("ff1816ec-aa5e-4d10-87f7-6f4963833460")?SHA1.HashData(bytes):throw new InvalidDataException("Unknown checksum");if(!expected.SequenceEqual(actual))throw new InvalidDataException("Compiled source drift: "+name);sourcePaths.Add(name);docs.Add(new{path=name,checksumAlgorithm=algorithm,checksum=Convert.ToHexString(expected).ToLowerInvariant(),sha256=Hash(name),currentFileMatched=true});}
 var priorEntry=priorAssemblies.SingleOrDefault(a=>Path.GetFileName(a.GetProperty("path").GetString())==Path.GetFileName(dll));assemblies.Add(new{path=dll,sha256=Hash(dll),mvid=assembly.ManifestModule.ModuleVersionId,pdbPath=pdb,pdbSha256=Hash(pdb),pdbGuid=id.Guid,pdbStamp=id.Stamp,pdbMatchesLoadedDll=true,reusedPriorProductionArtifact=priorEntry.ValueKind!=JsonValueKind.Undefined,priorPath=priorEntry.ValueKind==JsonValueKind.Undefined?null:priorEntry.GetProperty("path").GetString(),sourceDocuments=docs});
}
string[] required={"src/VisionFramework/ModuleExecutionScope.cs","src/VisionFramework/ModuleExecutionScope.Parameters.cs"};foreach(var p in required)if(!sourcePaths.Contains(Path.Combine(workspace,p.Replace('/',Path.DirectorySeparatorChar))))throw new InvalidDataException("Required source missing from PDB: "+p);
var report=new{schemaVersion=1,scope="Finite synthetic exceptional-cleanup observations using previously compiled current-main ModuleExecutionScope",classification="independent-implementation",passed=checks.Count,failed=0,cases=checks,assemblies,currentSourceDocumentCount=sourcePaths.Count,requiredImplementationSources=required.Select(p=>new{path=p,sha256=Hash(Path.Combine(workspace,p))}),priorArtifactManifest=new{path="docs/architecture-recovery/current-group-data-probe.json",sha256=Hash(Path.Combine(workspace,"docs/architecture-recovery/current-group-data-probe.json"))},runtimeExecuted=true,moduleExecutionScopeExecuted=true,productBuildExecuted=false,projectReferenceUsed=false,vendorRuntimeExecuted=false,guiStarted=false,deviceCalled=false,productModified=false,otherWorktreeRead=false,implementationCredit=0,limitations=new[]{"Only the previously compiled current ModuleExecutionScope is exercised with synthetic IDisposable resources and a synthetic throwing cancellation callback; no vendor/native runtime, real camera/tool resource, GUI or device.","Read-only reflection observes ownedServices and failedResources to distinguish ownership markers from service lookup; no private field is changed.","Local variables and audit observations intentionally retain synthetic resources. This does not measure last-owner release, GC, unmanaged pixel retirement or memory peaks.","The two-state Retain order is observed for this finite runtime and setup; it does not establish a universal Dictionary/disposal ordering contract.","No concurrency race, process-failure, OOM or exception thrown by the diagnostic recorder itself is injected.","HintPath references reuse the GroupDataProbe production DLL bytes. This project has no ProjectReference and does not rebuild product assemblies; PE/PDB/source checksum identities are checked separately."}};
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);File.WriteAllText(reportPath,JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(new{passed=checks.Count,failed=0,assemblies=assemblies.Count,currentSourceDocuments=sourcePaths.Count,report=reportPath}));
sealed class DisposableProbe(string name,bool fail=false):IDisposable{public string Name=>name;public int DisposeCalls{get;private set;}public void Dispose(){DisposeCalls++;if(fail)throw new InvalidOperationException("synthetic dispose failure: "+name);}}

