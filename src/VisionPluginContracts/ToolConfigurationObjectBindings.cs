using System.Globalization;

namespace VisionPluginContracts;

public enum ConfigurationObjectProperty { Content, OkColor, NgColor, FontSize, Opacity }
public sealed record ConfigurationObjectRoute(string ObjectName,string? FeatureName,ConfigurationObjectProperty Property)
{
    public static ConfigurationObjectRoute? Parse(ConfigurationNodeState node)
    {
        var setter=node.Element.Children.FirstOrDefault(e=>e.Tag=="BrigdgeSetter");
        string? Read(string key)=>setter?.Children.FirstOrDefault(e=>e.Tag==key)?.Text;
        var args=(Read("OperationParams")??"").Split(';');var property=node.Get("PropertyName");
        // Named text FontSize is the same original feature bridge. Other object
        // routes stay unchanged unless an explicit source-aware service resolves it.
        if(args[0]!="Show Text"&&(args[0].Length==0||Read("OperationName")!="GetDisplayObjectFeatureOperation"||args.Length!=2||args[1]!="FontSize"||node.Element.Tag!="FloatConfig"||property!="Value"))return null;
        if(Read("OperationName")=="GetDisplayObjectFeatureOperation"&&args.Length==2&&property=="Value")return args[1] switch {
            "Content" when node.Element.Tag=="StringConfig"=>new(args[0],args[1],ConfigurationObjectProperty.Content),
            "FontSize" when node.Element.Tag=="FloatConfig"=>new(args[0],args[1],ConfigurationObjectProperty.FontSize),_=>null};
        if(Read("OperationName")!="GetDisplayObjectOperation"||args.Length!=1)return null;
        return (node.Element.Tag,property) switch {
            ("ColorConfig","okcolor")=>new(args[0],null,ConfigurationObjectProperty.OkColor),
            ("ColorConfig","ngcolor")=>new(args[0],null,ConfigurationObjectProperty.NgColor),
            ("FloatConfig","opacity")=>new(args[0],null,ConfigurationObjectProperty.Opacity),_=>null};
    }
}
public sealed record ConfigurationObjectEditResult(bool Accepted,string Value,string? Error=null);
public interface IToolConfigurationObjectBinding {
    string Read();
    ConfigurationObjectEditResult Write(string value);
}
public interface IToolConfigurationObjectService {
    IToolConfigurationObjectBinding? Resolve(ConfigurationObjectRoute route);
}
public sealed record ConfigurationTextPlaceholder(string Identity,string Caption,string ModulePathName);
public interface IToolConfigurationTextPlaceholderService {
    IReadOnlyList<ConfigurationTextPlaceholder> Enumerate();
}
/// <summary>Explicit display-property bridge. The host owns storage and lifetime; no algorithm parameter writes.</summary>
public sealed class ToolDisplayConfigurationObjectService : IToolConfigurationObjectService
{
    readonly Func<ToolTextDisplaySettings> read;
    readonly Func<ToolTextDisplaySettings,bool> apply;
    readonly Action? changed;
    readonly Dictionary<ConfigurationObjectRoute,IToolConfigurationObjectBinding> bindings=[];
    public ToolDisplayConfigurationObjectService(Func<ToolTextDisplaySettings> read,Func<ToolTextDisplaySettings,bool> apply,Action? changed=null)
    {this.read=read;this.apply=apply;this.changed=changed;}
    public IToolConfigurationObjectBinding? Resolve(ConfigurationObjectRoute route)
    {
        if(route.ObjectName!="Show Text"||route.FeatureName!=(route.Property switch {ConfigurationObjectProperty.Content=>"Content",ConfigurationObjectProperty.FontSize=>"FontSize",_=>null}))return null;
        if(!Enum.IsDefined(route.Property))return null;
        if(!bindings.TryGetValue(route,out var binding))bindings[route]=binding=new Binding(this,route.Property);
        return binding;
    }
    sealed class Binding(ToolDisplayConfigurationObjectService owner,ConfigurationObjectProperty property):IToolConfigurationObjectBinding
    {
        static string Format(double n)=>n.ToString("R",CultureInfo.InvariantCulture);
        string Value(ToolTextDisplaySettings s)=>property switch {ConfigurationObjectProperty.Content=>s.Content,ConfigurationObjectProperty.OkColor=>s.OkColor,ConfigurationObjectProperty.NgColor=>s.NgColor,ConfigurationObjectProperty.FontSize=>Format(s.FontSize),_=>Format(s.Opacity)};
        public string Read()=>Value(owner.read());
        public ConfigurationObjectEditResult Write(string value)
        {
            var old=owner.read();var next=old;
            if(property is ConfigurationObjectProperty.FontSize or ConfigurationObjectProperty.Opacity){
                if(!double.TryParse(value,NumberStyles.Float,CultureInfo.InvariantCulture,out var number)||!double.IsFinite(number))return new(false,Value(old),"Invalid finite number.");
                next=property==ConfigurationObjectProperty.FontSize?old with{FontSize=number}:old with{Opacity=number};
            }else next=property switch {ConfigurationObjectProperty.Content=>old with{Content=value},ConfigurationObjectProperty.OkColor=>old with{OkColor=value},_=>old with{NgColor=value}};
            if(!ToolDisplayService.Valid(next))return new(false,Value(old),"Invalid display property.");
            try {
                if(!owner.apply(next))return new(false,Value(owner.read()),"Display commit rejected.");
                var accepted=Value(owner.read());
                if(accepted!=Value(next))return new(false,accepted,"Display storage did not accept the requested value.");
                owner.changed?.Invoke();return new(true,accepted);
            }catch(Exception error){return new(false,Value(owner.read()),error.Message);}
        }
    }
}
public static class ConfigurationTextPlaceholderEditing
{
    public static bool TryRange(string text,int caret,out int start,out int end)
    {
        start=end=-1;if(text.Length<=1||caret<0||caret>text.Length)return false;
        for(int i=caret-1;i>=0;i--){if(text[i]=='}')return false;if(text[i]=='{'){start=i+1;break;}}
        if(start<0)return false;
        for(int i=caret;i<text.Length;i++){if(text[i]=='{')return false;if(text[i]=='}'){end=i;return true;}}
        return false;
    }
    public static string? Insert(string text,int caret,ConfigurationTextPlaceholder choice)=>TryRange(text,caret,out var start,out var end)?text.Remove(start,end-start).Insert(start,choice.ModulePathName):null;
}
