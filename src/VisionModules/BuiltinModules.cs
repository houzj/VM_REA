using VisionCore;
using VisionIO;
using VisionFramework;
namespace VisionModules;

/// <summary>Independent built-in module adapters. Version numbers describe our contract, not VM equivalence.</summary>
public static class BuiltinModules
{
    public const string Version = "1.0.0";
    public static void RegisterAll(ModuleRegistry registry)
    {
        WorkflowCalls.Register(registry);
        foreach (var descriptor in Descriptors)
            registry.Register(descriptor, () => new Adapter(descriptor));
    }
    static ParameterDescriptor P(string name, ParameterKind kind, double? min=null,double? max=null) =>
        new(name,kind,true,null,min,max);
    public static IReadOnlyList<ModuleDescriptor> Descriptors { get; } = Array.AsReadOnly(new ModuleDescriptor[] {
        new("vision.image-file",Version,"File image source",[],[new("Image",DataKind.GrayImage)],
            [P("Path",ParameterKind.Text)]),
        new("vision.threshold",Version,"Threshold",[new("Image",DataKind.GrayImage)],[new("Image",DataKind.GrayImage)],
            [P("Threshold",ParameterKind.Integer,0,255)]),
        new("vision.components",Version,"8-connected components",[new("Image",DataKind.GrayImage)],
            [new("Components",DataKind.Components),new("Count",DataKind.Integer)],[]),
        new("vision.circle",Version,"Radial circle",[new("Image",DataKind.GrayImage)],
            [new("Circle",DataKind.Circle,false),new("Found",DataKind.Boolean),new("Radius",DataKind.Number,false)],
            [P("CenterX",ParameterKind.Number),P("CenterY",ParameterKind.Number),
             P("InitialRadius",ParameterKind.Number,0),P("SearchHalfWidth",ParameterKind.Number,0),
             P("SampleCount",ParameterKind.Integer,3,100000),P("GradientThreshold",ParameterKind.Number,0,255),
             P("Polarity",ParameterKind.Text),P("RadialStep",ParameterKind.Number,0),
             P("GradientHalfSpan",ParameterKind.Number,0),P("MinimumEdges",ParameterKind.Integer,3,100000)]),
        new("logic.number-compare",Version,"Number comparison",[new("Number",DataKind.Number)],[new("Passed",DataKind.Boolean)],
            [P("Operator",ParameterKind.Text),P("Reference",ParameterKind.Number)])
    });
    sealed class Adapter(ModuleDescriptor descriptor) : IModule
    {
        public ModuleDescriptor Descriptor => descriptor;
        public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            ModuleResult result;
            try { result=Execute(c); }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException or KeyNotFoundException or FormatException or OverflowException)
            { result=new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"InvalidInput",e.Message); }
            catch (InvalidDataException e) { result=new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"InvalidData",e.Message); }
            catch (IOException e) { result=new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"ImageReadFailed",e.Message); }
            catch (UnauthorizedAccessException e) { result=new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"ImageReadFailed",e.Message); }
            token.ThrowIfCancellationRequested();
            return ValueTask.FromResult(result);
        }
        ModuleResult Execute(ModuleExecutionContext c)
        {
            // Direct calls have the same required-parameter and type checks as engine calls.
            foreach (var p in Descriptor.Parameters)
            {
                if(!c.Parameters.TryGetValue(p.Name,out var v)) throw new ArgumentException("Missing parameter: "+p.Name);
                bool valid=p.Kind switch {
                    ParameterKind.Text => v.ValueKind==System.Text.Json.JsonValueKind.String,
                    ParameterKind.Boolean => v.ValueKind is System.Text.Json.JsonValueKind.True or System.Text.Json.JsonValueKind.False,
                    ParameterKind.Integer => v.ValueKind==System.Text.Json.JsonValueKind.Number && v.TryGetInt32(out _),
                    _ => v.ValueKind==System.Text.Json.JsonValueKind.Number && v.TryGetDouble(out var d) && double.IsFinite(d)
                };
                if(!valid) throw new ArgumentException("Invalid parameter type: "+p.Name);
                if(p.Kind is ParameterKind.Integer or ParameterKind.Number) {
                    double n=v.GetDouble();
                    if(p.Minimum is double min && n<min || p.Maximum is double max && n>max)
                        throw new ArgumentOutOfRangeException(p.Name);
                }
            }
            foreach(var input in Descriptor.Inputs)
                if(!c.Inputs.TryGetValue(input.Name,out var value)||value.Kind!=input.Kind)
                    throw new ArgumentException("Missing or incompatible input: "+input.Name);
            var output=new Dictionary<string,PortValue>();
            switch(Descriptor.Id) {
                case "vision.image-file":
                    string path=c.Text("Path");
                    if(string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path must not be empty.");
                    using(var stream=File.OpenRead(c.ResolvePath(path))) output["Image"]=PortValue.Image(BmpReader.Read(stream));
                    break;
                case "vision.threshold":
                    output["Image"]=PortValue.Image(Algorithms.Threshold(c.Input<GrayImage>("Image"),checked((byte)c.Integer("Threshold")))); break;
                case "vision.components":
                    var components=Algorithms.ConnectedComponents8(c.Input<GrayImage>("Image"));
                    output["Components"]=PortValue.Components(components); output["Count"]=PortValue.Integer(components.Count); break;
                case "vision.circle":
                    string polarity=c.Text("Polarity");
                    if(!Enum.TryParse<EdgePolarity>(polarity,false,out var p)||!Enum.IsDefined(p)||p.ToString()!=polarity)
                        throw new ArgumentException("Polarity must be DarkToLight, LightToDark or Both.");
                    var options=new RadialCircleOptions(c.Number("CenterX"),c.Number("CenterY"),c.Number("InitialRadius"),
                        c.Number("SearchHalfWidth"),c.Integer("SampleCount"),c.Number("GradientThreshold"),p,
                        c.Number("RadialStep"),c.Number("GradientHalfSpan"),c.Integer("MinimumEdges"));
                    var detection=RadialCircleDetector.Detect(c.Input<GrayImage>("Image"),options);
                    output["Found"]=PortValue.Boolean(detection.Status==CircleDetectionStatus.Success);
                    if(detection.Circle is CircleFit circle) {
                        output["Circle"]=PortValue.Circle(circle); output["Radius"]=PortValue.Number(circle.Radius);
                    } else return new(ModuleStatus.NoMatch,output,detection.Status.ToString(),"No usable circle was found.");
                    break;
                case "logic.number-compare":
                    double number=c.Input<double>("Number"), reference=c.Number("Reference");
                    bool passed=c.Text("Operator") switch {
                        "=="=>number==reference,"!="=>number!=reference,">"=>number>reference,
                        ">="=>number>=reference,"<"=>number<reference,"<="=>number<=reference,
                        _=>throw new ArgumentException("Operator must be ==, !=, >, >=, < or <=.")
                    };
                    output["Passed"]=PortValue.Boolean(passed); break;
                default: throw new InvalidOperationException("Unimplemented module.");
            }
            return new(ModuleStatus.Success,output);
        }
    }
}

