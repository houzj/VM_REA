using VisionCore;
namespace VisionFramework;

// Field names follow recovered module XML; this is a managed projection, not native ABI.
public sealed record CompoundLeaf(string Path,DataKind Kind,ValueCardinality Cardinality);
public static class CompoundOutputFields
{
 static readonly string[] ComponentNumbers=["Area","CentroidPoint.CentroidX","CentroidPoint.CentroidY","BlobRect.RectPoint.BlobRectX","BlobRect.RectPoint.BlobRectY","BlobRect.BlobRectWidth","BlobRect.BlobRectHeight"];
 static readonly string[] BoxNumbers=["ROI.DetectRectCenterPoint.DetectCenterX","ROI.DetectRectCenterPoint.DetectCenterY","ROI.DetectWidth","ROI.DetectHeight","ROI.DetectAngle"];
 static readonly string[] AnnulusNumbers=["ROIAnnulus.DetectAnnulusCenterPoint.DetectAnnulusCenterX","ROIAnnulus.DetectAnnulusCenterPoint.DetectAnnulusCenterY","ROIAnnulus.DetectAnnulusInnerRadius","ROIAnnulus.DetectAnnulusOuterRadius","ROIAnnulus.DetectAnnulusStartAngle","ROIAnnulus.DetectAnnulusAngleExtend"];
 public static IReadOnlyList<CompoundLeaf> Leaves(DataKind kind,ValueCardinality cardinality=ValueCardinality.Scalar)=>kind switch{
  DataKind.Components=>new[]{new CompoundLeaf("BlobNum",DataKind.Integer,ValueCardinality.Scalar),new CompoundLeaf("TotalArea",DataKind.Number,ValueCardinality.Scalar)}.Concat(ComponentNumbers.Select(p=>new CompoundLeaf(p,DataKind.Number,ValueCardinality.Array))).ToArray(),
  DataKind.Region=>BoxNumbers.Concat(AnnulusNumbers).Select(p=>new CompoundLeaf(p,DataKind.Number,ValueCardinality.Array)).ToArray(),
  _=>[]};
 public static ModuleDescriptor Describe(ModuleDescriptor descriptor){var outputs=descriptor.Outputs.ToList();foreach(var port in descriptor.Outputs)foreach(var leaf in Leaves(port.Kind,port.Cardinality)){var name=port.Name+"."+leaf.Path;if(!outputs.Any(p=>p.Name==name))outputs.Add(new(name,leaf.Kind,false){Cardinality=leaf.Cardinality});}return descriptor with{Outputs=outputs.AsReadOnly()};}
 public static IReadOnlyDictionary<string,PortValue> Expand(IReadOnlyDictionary<string,PortValue> outputs){var result=outputs.ToDictionary(p=>p.Key,p=>p.Value);foreach(var pair in outputs)foreach(var leaf in Project(pair.Value))result.TryAdd(pair.Key+"."+leaf.Key,leaf.Value);return result;}
 public static IReadOnlyDictionary<string,PortValue> Project(PortValue value){var result=new Dictionary<string,PortValue>();
  if(value.Kind==DataKind.Components){var c=value.As<IReadOnlyList<Component>>();result["BlobNum"]=PortValue.Integer(c.Count);result["TotalArea"]=PortValue.Number(c.Sum(v=>(double)v.Area));Func<Component,double>[] fields=[v=>v.Area,v=>v.CenterX,v=>v.CenterY,v=>v.MinX,v=>v.MinY,v=>(double)v.MaxX-v.MinX+1,v=>(double)v.MaxY-v.MinY+1];for(int i=0;i<fields.Length;i++)result[ComponentNumbers[i]]=PortValue.NumberArray(c.Select(fields[i]));}
  else if(value.Kind==DataKind.Region){var regions=value.As<RoiRegionValue>().Regions;
   // Do not silently discard unsupported shapes or convert non-pixel coordinates without a fixture/image context.
   if(regions.All(r=>r.Kind==RoiKind.Rectangle&&r.CoordinateSpace==RoiCoordinateSpace.ImagePixels&&!r.Exclusion)){Func<RoiDocument,double>[] fields=[r=>r.CenterX,r=>r.CenterY,r=>r.Width,r=>r.Height,r=>r.AngleDegrees];for(int i=0;i<fields.Length;i++)result[BoxNumbers[i]]=PortValue.NumberArray(regions.Select(fields[i]));}
   else if(regions.All(r=>r.Kind==RoiKind.Annulus&&r.CoordinateSpace==RoiCoordinateSpace.ImagePixels&&!r.Exclusion&&RoiGeometry.Parameters(r).StartAngle==0&&RoiGeometry.Parameters(r).SweepAngle==360)){Func<RoiDocument,double>[] fields=[r=>r.CenterX,r=>r.CenterY,r=>r.InnerRadius,r=>r.Radius,r=>RoiGeometry.Parameters(r).StartAngle,r=>RoiGeometry.Parameters(r).SweepAngle];for(int i=0;i<fields.Length;i++)result[AnnulusNumbers[i]]=PortValue.NumberArray(regions.Select(fields[i]));}}
  return result;
 }
}
