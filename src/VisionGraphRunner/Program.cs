using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisionCore;
using VisionFramework;
using VisionModules;
var registry=new ModuleRegistry();BuiltinModules.RegisterAll(registry);
var jsonOptions=new JsonSerializerOptions{WriteIndented=true,Converters={new JsonStringEnumConverter()}};
void Print(object value)=>Console.WriteLine(JsonSerializer.Serialize(value,jsonOptions));
Dictionary<string,JsonElement> Parameters(params (string Name,object Value)[] items)=>items.ToDictionary(x=>x.Name,x=>JsonSerializer.SerializeToElement(x.Value));
try {
    if(args.Length==1&&args[0]=="catalog"){Print(registry.Descriptors);return 0;}
    if(args.Length==3&&args[0]=="create-demo"){
        string image=Path.GetFullPath(args[1]),path=Path.GetFullPath(args[2]);using var file=File.OpenRead(image);string hash=Convert.ToHexString(SHA256.HashData(file)).ToLowerInvariant();string relative=Path.GetRelativePath(Path.GetDirectoryName(path)!,image);
        var nodes=new[]{
            new NodeDocument("source","vision.image-file","1.0.0","文件图像源",Parameters(("Path",relative)),40,160),
            new NodeDocument("threshold","vision.threshold","1.0.0","阈值分割",Parameters(("Threshold",128)),280,60),
            new NodeDocument("components","vision.components","1.0.0","连通域统计",Parameters(),520,60),
            new NodeDocument("circle","vision.circle","1.0.0","圆查找",Parameters(("CenterX",717.0),("CenterY",507.0),("InitialRadius",375.0),("SearchHalfWidth",12.0),("SampleCount",64),("GradientThreshold",15.0),("Polarity","Both"),("RadialStep",.5),("GradientHalfSpan",1.0),("MinimumEdges",6)),280,260),
            new NodeDocument("compare","logic.number-compare","1.0.0","半径比较",Parameters(("Operator",">"),("Reference",370.0)),520,260)
        };
        var connections=new[]{new ConnectionDocument("source","Image","threshold","Image"),new ConnectionDocument("threshold","Image","components","Image"),new ConnectionDocument("source","Image","circle","Image"),new ConnectionDocument("circle","Radius","compare","Number")};
        var procedure=new ProcedureDocument("main","图像分析与尺寸判定",nodes,connections);var check=GraphValidator.Validate(procedure,registry);if(!check.IsValid)throw new InvalidDataException(JsonSerializer.Serialize(check.Issues));
        SolutionStore.Save(path,new(1,"architecture-demo","独立模块图示例",new[]{procedure},new[]{new ResourceDocument("sample",relative,hash)}));Print(new{saved=path,nodes=nodes.Length,connections=connections.Length,parameterSource="manual sample ROI; not original VM parameter mapping"});return 0;
    }
    if(args.Length>=2&&args.Length<=3&&args[0] is "validate" or "run"){
        var workspace=SolutionStore.OpenWorkspace(args[1]);var procedure=args.Length==3?workspace.Document.Procedures.Single(p=>p.Id==args[2]):workspace.Document.Procedures.FirstOrDefault()??throw new InvalidDataException("No procedure.");
        var check=GraphValidator.Validate(procedure,registry);if(args[0]=="validate"||!check.IsValid){Print(check);return check.IsValid?0:2;}
        using var cancel=new CancellationTokenSource();Console.CancelKeyPress+=(_,e)=>{e.Cancel=true;cancel.Cancel();};
        var session=await new SolutionExecutor(registry).RunAsync(workspace,procedure.Id,cancellationToken:cancel.Token);
        object Summary(PortValue v)=>v.Kind switch {
            DataKind.GrayImage=>new{kind=v.Kind,width=v.As<GrayImage>().Width,height=v.As<GrayImage>().Height,pixelsSha256=Convert.ToHexString(SHA256.HashData(v.As<GrayImage>().Pixels)).ToLowerInvariant()},
            DataKind.Components=>new{kind=v.Kind,count=v.As<IReadOnlyList<Component>>().Count,preview=v.As<IReadOnlyList<Component>>().Take(10).ToArray(),omitted=Math.Max(0,v.As<IReadOnlyList<Component>>().Count-10)},
            _=>new{kind=v.Kind,value=v.Value}
        };
        Print(new{solution=workspace.Document.Id,procedure=procedure.Id,session.Status,nodes=session.Nodes.Select(n=>new{n.NodeId,n.Status,n.ElapsedMilliseconds,n.ErrorCode,n.Message,outputs=n.Outputs.ToDictionary(k=>k.Key,k=>Summary(k.Value))}),compatibility="independent architecture; vendor equivalence unverified"});return session.Status==SessionStatus.Success?0:3;
    }
    Console.Error.WriteLine("VisionGraphRunner catalog | create-demo <image.bmp> <solution.vmx.json> | validate <solution> [procedure-id] | run <solution> [procedure-id]");return 1;
} catch(Exception ex) when(ex is IOException or ArgumentException or JsonException or InvalidOperationException){Console.Error.WriteLine(ex.Message);return 1;}

