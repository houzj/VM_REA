using System.Text.Json;
using VisionFramework;
namespace VisionPlugins.Calibration;

/// <summary>Original module/AlgorithmTab defaults; UI-only selectors are explicitly included.</summary>
public static class NImageParameters
{
 static ParameterDescriptor P<T>(string n,ParameterKind k,T v,double? min=null,double? max=null)=>new(n,k,true,JsonSerializer.SerializeToElement(v),min,max);
 public static IReadOnlyList<ParameterDescriptor> Definitions{get;}=Array.AsReadOnly(new[]{
 P("CalibPointTotalNum",ParameterKind.Integer,9,0,15),P("RotPointTotalNum",ParameterKind.Integer,3,0,9),
 P("BasePointX",ParameterKind.Number,0d,-9999999,9999999),P("BasePointY",ParameterKind.Number,0d,-9999999,9999999),P("MoveAlignX",ParameterKind.Number,1d,-99999,99999),P("MoveAlignY",ParameterKind.Number,1d,-99999,99999),P("MoveFirstType",ParameterKind.Integer,1,1,2),P("ChangeDirectionMoveTime",ParameterKind.Integer,3,1,16),P("BaseAngle",ParameterKind.Number,0d,-180,180),P("MoveAngle",ParameterKind.Number,1d,-180,180),
 P("HomoFixEnable",ParameterKind.Boolean,false),P("RefreshFileEnable",ParameterKind.Boolean,false),P("CalibPathName",ParameterKind.Text,""),P("CareraMove",ParameterKind.Boolean,false),P("CalibBoardType",ParameterKind.Integer,7),P("FilterStatus",ParameterKind.Integer,1,1,2),
 P("CalibPointInput",ParameterKind.Integer,0,0,1),P("InputMode",ParameterKind.Integer,0,0,1),P("ClearRevision",ParameterKind.Integer,0,0,int.MaxValue)});
 public static Dictionary<string,JsonElement> Defaults()=>Definitions.ToDictionary(p=>p.Name,p=>p.DefaultValue!.Value.Clone());
 public static NImageSequenceParameters Sequence(IReadOnlyDictionary<string,JsonElement> p)=>new(p["CalibPointTotalNum"].GetInt32(),p["RotPointTotalNum"].GetInt32(),p["BasePointX"].GetDouble(),p["BasePointY"].GetDouble(),p["MoveAlignX"].GetDouble(),p["MoveAlignY"].GetDouble(),p["MoveFirstType"].GetInt32(),p["ChangeDirectionMoveTime"].GetInt32(),p["BaseAngle"].GetDouble(),p["MoveAngle"].GetDouble());
}
