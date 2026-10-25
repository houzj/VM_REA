using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using VisionFramework;
using VisionCore;

if(args.Length!=2)throw new ArgumentException("Usage: NonImageProbe <workspace> <exclusive-report-path>");
string workspace=Path.GetFullPath(args[0]),reportPath=Path.GetFullPath(args[1]);
var cases=new List<object>();
void Check(bool condition,string name,object observation){if(!condition)throw new InvalidOperationException("CHECK FAILED: "+name);cases.Add(new{name,passed=true,observation});}
string Hash(string path)=>Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
var numberSequence=PortValue.Numbers(new double[]{2,4});
var typedNumberArray=PortValue.NumberArray(new double[]{2,4});
Check(numberSequence.Kind==DataKind.NumberArray&&numberSequence.Cardinality==ValueCardinality.Scalar&&numberSequence.ElementCount==1,"NumberArray kind retains scalar cardinality",new{numberSequence.Kind,numberSequence.Cardinality,numberSequence.ElementCount,storedCount=numberSequence.As<IReadOnlyList<double>>().Count});
Check(typedNumberArray.Kind==DataKind.Number&&typedNumberArray.Cardinality==ValueCardinality.Array&&typedNumberArray.ElementCount==2,"Number kind with Array cardinality is a separate contract",new{typedNumberArray.Kind,typedNumberArray.Cardinality,typedNumberArray.ElementCount});
Check(ReferenceEquals(numberSequence.Element(0),numberSequence),"Scalar sequence Element(0) retains the same PortValue",new{sameReference=true});

var integers=PortValue.IntegerArray(new long[]{1,2});
var target=new PortDescriptor("Input",DataKind.Number,AcceptInteger:true){Cardinality=ValueCardinality.Array};
bool accepted=PortTypes.AcceptsInput(target,integers.Kind,integers.Cardinality);Exception? promotionError=null;
try{_=DataKinds.Coerce(integers,target);}catch(Exception e){promotionError=e;}
Check(accepted&&promotionError is InvalidOperationException,"Integer Array passes declaration gate but scalar Coerce read fails",new{accepted,errorType=promotionError?.GetType().FullName,errorMessage=promotionError?.Message,target.Kind,target.Cardinality});
long originalInteger=16_777_217;
var promoted=DataKinds.Coerce(PortValue.Integer(originalInteger),new PortDescriptor("Input",DataKind.Number,AcceptInteger:true));
Check(promoted.As<double>()==16_777_216d&&promoted.As<double>()!=originalInteger,"Integer promotion uses float before double",new{input=originalInteger,actual=promoted.As<double>(),exactDouble=(double)originalInteger});
var unchanged=PortValue.Text(new string(new[]{'a','b','c'}));
Check(ReferenceEquals(DataKinds.Coerce(unchanged,DataKind.Text),unchanged),"Same-kind Coerce preserves PortValue reference",new{sameReference=true});

var numbers=new double[]{3,5};var copied=PortValue.Numbers(numbers);numbers[0]=99;
Check(copied.As<IReadOnlyList<double>>()[0]==3&&!ReferenceEquals(copied.Value,numbers),"Numbers copies the source container",new{source=numbers[0],stored=copied.As<IReadOnlyList<double>>()[0]});
var text=new string(new[]{'x','y','z'});var texts=new[]{text};var copiedTexts=PortValue.TextArray(texts);texts[0]="replacement";
Check(copiedTexts.As<IReadOnlyList<string>>()[0]==text&&ReferenceEquals(copiedTexts.As<IReadOnlyList<string>>()[0],text)&&!ReferenceEquals(copiedTexts.Value,texts),"TextArray copies its container and retains string references",new{source=texts[0],stored=copiedTexts.As<IReadOnlyList<string>>()[0],sameElementReference=ReferenceEquals(copiedTexts.As<IReadOnlyList<string>>()[0],text)});
var points=new[]{new Point2(1,2),new Point2(3,4)};var pointArray=PortValue.PointArray(points);points[0]=new Point2(90,91);
Check(pointArray.As<IReadOnlyList<Point2>>()[0]==new Point2(1,2),"PointArray copies current value-struct elements",new{source=points[0],stored=pointArray.As<IReadOnlyList<Point2>>()[0]});
var projected=CompoundOutputFields.Project(pointArray);
Check(projected["X"].Kind==DataKind.NumberArray&&projected["X"].Cardinality==ValueCardinality.Scalar&&projected["X"].As<IReadOnlyList<double>>().SequenceEqual(new double[]{1,3}),"Point array columns use NumberArray plus Scalar",new{projected["X"].Kind,projected["X"].Cardinality,values=projected["X"].As<IReadOnlyList<double>>()});
var components=new[]{new Component(5,1,2,3,4,2,3)};var componentValue=PortValue.Components(components);components[0]=new Component(9,0,0,0,0,0,0);var componentFields=CompoundOutputFields.Project(componentValue);
Check(componentValue.As<IReadOnlyList<Component>>()[0].Area==5&&componentFields["Area"].Kind==DataKind.Number&&componentFields["Area"].Cardinality==ValueCardinality.Array,"Components copies structs and projects Number plus Array",new{storedArea=componentValue.As<IReadOnlyList<Component>>()[0].Area,componentFields["Area"].Kind,componentFields["Area"].Cardinality});
var outputs=new Dictionary<string,PortValue>{{"Points",pointArray}};var expanded=CompoundOutputFields.Expand(outputs);
Check(!ReferenceEquals(expanded,outputs)&&ReferenceEquals(expanded["Points"],pointArray),"Compound expansion copies dictionary and preserves parent PortValue",new{sameDictionary=false,sameParentReference=true});
var joined=PortValue.JoinArray(DataKind.Text,new[]{copiedTexts,PortValue.Text(text)});
Check(joined.Cardinality==ValueCardinality.Array&&joined.ElementCount==2&&!ReferenceEquals(joined.Value,copiedTexts.Value)&&ReferenceEquals(joined.As<IReadOnlyList<string>>()[0],text),"JoinArray creates a new text container while retaining string objects",new{joined.ElementCount,sameContainer=false,sameElementReference=true});

var assemblies=new List<object>();var sourcePaths=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach(var assembly in new[]{typeof(PortValue).Assembly,typeof(Point2).Assembly,Assembly.GetExecutingAssembly()}.Distinct()){
 string dll=assembly.Location,pdb=Path.ChangeExtension(dll,"pdb");
 using var peStream=File.OpenRead(dll);using var pe=new PEReader(peStream);var debug=pe.ReadDebugDirectory().Single(e=>e.Type==DebugDirectoryEntryType.CodeView);var codeView=pe.ReadCodeViewDebugDirectoryData(debug);
 using var pdbStream=File.OpenRead(pdb);using var provider=MetadataReaderProvider.FromPortablePdbStream(pdbStream);var reader=provider.GetMetadataReader();var pdbId=new BlobContentId(reader.DebugMetadataHeader!.Id);
 if(codeView.Guid!=pdbId.Guid||debug.Stamp!=pdbId.Stamp)throw new InvalidDataException("Loaded DLL and PDB identity mismatch: "+dll);
 var documents=new List<object>();foreach(var handle in reader.Documents){var doc=reader.GetDocument(handle);string name=reader.GetString(doc.Name);Guid algorithm=reader.GetGuid(doc.HashAlgorithm);byte[] expected=reader.GetBlobBytes(doc.Hash);if(!File.Exists(name))throw new FileNotFoundException("Compiled source unavailable",name);byte[] bytes=File.ReadAllBytes(name);byte[] actual=algorithm==new Guid("8829d00f-11b8-4213-878b-770e8597ac16")?SHA256.HashData(bytes):algorithm==new Guid("ff1816ec-aa5e-4d10-87f7-6f4963833460")?SHA1.HashData(bytes):throw new InvalidDataException("Unsupported PDB source checksum");if(!expected.SequenceEqual(actual))throw new InvalidDataException("Current source differs from compiled PDB: "+name);sourcePaths.Add(name);documents.Add(new{path=name,checksumAlgorithm=algorithm,checksum=Convert.ToHexString(expected).ToLowerInvariant(),sha256=Hash(name),currentFileMatched=true});}
 assemblies.Add(new{path=dll,sha256=Hash(dll),mvid=assembly.ManifestModule.ModuleVersionId,pdbPath=pdb,pdbSha256=Hash(pdb),pdbGuid=pdbId.Guid,pdbStamp=pdbId.Stamp,pdbMatchesLoadedDll=true,sourceDocuments=documents});
}
string[] required={"src/VisionFramework/Contracts.cs","src/VisionFramework/PortArrays.cs","src/VisionFramework/CompoundOutputFields.cs","src/VisionData/ImageGeometry.cs"};foreach(string rel in required)if(!sourcePaths.Contains(Path.Combine(workspace,rel.Replace('/',Path.DirectorySeparatorChar))))throw new InvalidDataException("Required real implementation missing from loaded PDB: "+rel);
var report=new{schemaVersion=1,scope="Finite actual-current-main nonimage transport probe",passed=cases.Count,failed=0,cases,assemblies,requiredImplementationSources=required.Select(p=>new{path=p,sha256=Hash(Path.Combine(workspace,p))}),runtimeExecuted=true,vendorRuntimeExecuted=false,guiStarted=false,deviceCalled=false,productModified=false,otherWorktreeRead=false,implementationCredit=0,limitations=new[]{"Only listed managed factory/coercion/projection cases executed; no vendor/native code, GUI, devices, GC/peak-memory or end-to-end workflow run.","Integer Array declaration acceptance and actual Coerce exception are separate observed results; GraphExecutor failure-code routing remains source evidence rather than an executed graph in this probe.","Current Point/Components structures do not prove original pointset ABI or behavior parity.","Checksums bind the actually loaded DLL/PDB to every current compiled source file; this does not establish vendor parity."}};
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);File.WriteAllText(reportPath,JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine(JsonSerializer.Serialize(new{passed=cases.Count,failed=0,assemblies=assemblies.Count,currentSourceDocuments=sourcePaths.Count,report=reportPath}));
