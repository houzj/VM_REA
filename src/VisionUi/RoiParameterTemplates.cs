using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using VisionFramework;
namespace VisionUi;
public sealed record RoiFieldSpec(string Key,string Label,double Minimum=-30000,double Maximum=30000,bool Integer=false);
// Shape-specific order/limits from Apps.XmlUI *ROIStack.xaml (source inventory in docs).
public static class RoiParameterTemplates {
 static RoiFieldSpec F(string key,string label,double min=-30000,double max=30000,bool integer=false)=>new(key,label,min,max,integer);
 static readonly RoiFieldSpec X=F("CenterX","中心 X"),Y=F("CenterY","中心 Y"),W=F("Width","宽度",0),H=F("Height","高度",0),A=F("AngleDegrees","角度",-180,180),R=F("Radius","半径",0,10000),I=F("InnerRadius","内半径",0,10000),EX=F("EndX","终点 X"),EY=F("EndY","终点 Y"),CX=F("CenterX","起点 X"),CY=F("CenterY","起点 Y"),CW=F("CaliperWidth","卡尺宽度",1,500),CH=F("CaliperHeight","卡尺高度",1),N=F("CaliperCount","卡尺数量",2,1000,true);
 public static IReadOnlyList<RoiFieldSpec> Fields(RoiDocument r)=>r.Kind switch{
  RoiKind.Rectangle or RoiKind.LineBox or RoiKind.SequentialBox or RoiKind.PolygonCaliper=>[X,Y,W,H,A],
  RoiKind.UprightRectangle=>[X,Y,W,H], RoiKind.Circle=>[X,Y,R],RoiKind.Annulus or RoiKind.SectorRing=>[X,Y,R,I],
  RoiKind.Sector or RoiKind.OverSector=>[X,Y,R,I,F("StartAngle","起始角度",-180,180),F("SweepAngle","角度范围",0,r.Kind==RoiKind.OverSector?720:360)],
  RoiKind.Point=>[X,Y],RoiKind.Line=>[CX,CY,EX,EY],
  RoiKind.LineCaliper=>[CX,CY,EX,EY,N,CH with{Minimum=3},CW],
  RoiKind.CircleCaliper=>RoiGeometry.Parameters(r).ShowCaliperParameters?[X,Y,R,N with{Minimum=3},CW,CH]:[X,Y,R,N with{Minimum=3}],
  RoiKind.SectorCaliper=>[CX,CY,EX,EY,R,N,CH,CW],
  RoiKind.MatrixCircle=>[F("Columns","列数 M",1,5,true),F("Rows","行数 N",1,5,true),X,Y,W,H,A,R,I],
  RoiKind.Coordinate=>[X with{Label="原点 X"},Y with{Label="原点 Y"},A],
  RoiKind.Box3D=>[X,Y,F("CenterZ","中心 Z"),F("AngleX","角度 X",-180,180),F("AngleY","角度 Y",-180,180),A with{Label="角度 Z"},W with{Label="长度"},H with{Label="宽度"},F("Depth","高度",0)], _=>[]};
 public static double Get(RoiDocument r,string key)=>key switch{"CenterX"=>r.CenterX,"CenterY"=>r.CenterY,"Width"=>r.Width,"Height"=>r.Height,"Radius"=>r.Radius,"InnerRadius"=>r.InnerRadius,"AngleDegrees"=>r.AngleDegrees,_=>Convert.ToDouble(typeof(RoiShapeParameters).GetProperty(key)?.GetValue(RoiGeometry.Parameters(r))??throw new InvalidOperationException("Unknown ROI field: "+key))};
 public static RoiDocument Put(RoiDocument r,string key,double n){var field=Fields(r).Single(f=>f.Key==key);if(!double.IsFinite(n)||n<field.Minimum||n>field.Maximum||field.Integer&&n!=Math.Truncate(n))throw new InvalidDataException(field.Label+" 超出范围 "+field.Minimum+".."+field.Maximum);return key switch{"CenterX"=>r with{CenterX=n},"CenterY"=>r with{CenterY=n},"Width"=>r with{Width=n},"Height"=>r with{Height=n},"Radius"=>r with{Radius=n},"InnerRadius"=>r with{InnerRadius=n},"AngleDegrees"=>r with{AngleDegrees=n},_=>PutShape(r,key,n,field.Integer)};}
 static RoiDocument PutShape(RoiDocument r,string key,double n,bool integer){var json=JsonSerializer.SerializeToNode(RoiGeometry.Parameters(r))!.AsObject();json[key]=integer?JsonValue.Create(checked((int)n)):JsonValue.Create(n);return r with{Shape=json.Deserialize<RoiShapeParameters>()};}
}
