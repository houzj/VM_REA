using System.Security.Cryptography;
using VisionCore;
namespace VisionFramework;
public enum RoiKind { Rectangle, Circle, Annulus, Polygon, FullImage, Point, Line, Sector, SectorRing, OverSector, LineCaliper, CircleCaliper, SectorCaliper, MatrixCircle, Coordinate, Box3D, PolygonCaliper, UprightRectangle, LineBox, SequentialBox }
public enum RoiCoordinateSpace { ImagePixels, Fixture, Normalized }
public sealed record RoiDocument(string Id,RoiKind Kind,double CenterX,double CenterY,double Width=0,double Height=0,double Radius=0,double InnerRadius=0,double AngleDegrees=0,IReadOnlyList<Point2>? Vertices=null,RoiCoordinateSpace CoordinateSpace=RoiCoordinateSpace.ImagePixels){[System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)] public RoiShapeParameters? Shape{get;init;}[System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)] public bool Exclusion{get;init;}};
public static class RoiValidator {
 public static void Validate(RoiDocument roi){ArgumentNullException.ThrowIfNull(roi);if(string.IsNullOrWhiteSpace(roi.Id)||!Enum.IsDefined(roi.Kind)||!Enum.IsDefined(roi.CoordinateSpace)||new[]{roi.CenterX,roi.CenterY,roi.Width,roi.Height,roi.Radius,roi.InnerRadius,roi.AngleDegrees}.Any(v=>!double.IsFinite(v)))throw new InvalidDataException("Invalid ROI identity or numeric value.");
 if(roi.Kind is (RoiKind.Rectangle or RoiKind.UprightRectangle or RoiKind.LineBox or RoiKind.SequentialBox or RoiKind.PolygonCaliper)&&(roi.Width<=0||roi.Height<=0)||roi.Kind==RoiKind.Circle&&roi.Radius<=0||roi.Kind==RoiKind.Annulus&&(roi.InnerRadius<0||roi.Radius<=roi.InnerRadius)||roi.Kind==RoiKind.Polygon&&(roi.Vertices is null||roi.Vertices.Count<3||roi.Vertices.Count>10000||roi.Vertices.Any(p=>!double.IsFinite(p.X)||!double.IsFinite(p.Y))))throw new InvalidDataException("Invalid ROI geometry.");
 var s=RoiGeometry.Parameters(roi);if(new[]{s.StartAngle,s.SweepAngle,s.EndX,s.EndY,s.CaliperWidth,s.CaliperHeight,s.CenterZ,s.Depth,s.AngleX,s.AngleY}.Any(v=>!double.IsFinite(v)))throw new InvalidDataException("Non-finite ROI shape value.");
 if(roi.Kind is RoiKind.Sector or RoiKind.SectorRing or RoiKind.OverSector or RoiKind.CircleCaliper or RoiKind.SectorCaliper or RoiKind.MatrixCircle)if(roi.Radius<=0||roi.InnerRadius<0||roi.InnerRadius>=roi.Radius)throw new InvalidDataException("Invalid ROI radii.");
 if(roi.Kind is RoiKind.Sector or RoiKind.SectorRing or RoiKind.SectorCaliper or RoiKind.OverSector)if(s.SweepAngle<=0||s.SweepAngle>(roi.Kind==RoiKind.OverSector?720:360))throw new InvalidDataException("Invalid ROI angular range.");
 if(roi.Kind is (RoiKind.Line or RoiKind.LineCaliper)&&s.EndX==roi.CenterX&&s.EndY==roi.CenterY)throw new InvalidDataException("Zero-length ROI line.");
 if(roi.Kind is RoiKind.LineCaliper or RoiKind.CircleCaliper or RoiKind.SectorCaliper or RoiKind.PolygonCaliper)if(s.CaliperCount<1||s.CaliperCount>100000||s.CaliperWidth<=0||s.CaliperHeight<=0)throw new InvalidDataException("Invalid ROI calipers.");
 if(roi.Kind==RoiKind.MatrixCircle&&(s.Rows<1||s.Columns<1||(long)s.Rows*s.Columns>10000||roi.Width<0||roi.Height<0))throw new InvalidDataException("Invalid ROI matrix.");
 if(roi.Kind==RoiKind.Box3D&&(roi.Width<=0||roi.Height<=0||s.Depth<=0))throw new InvalidDataException("Invalid 3D ROI dimensions.");
 }
}
public sealed class ResourceCatalog {
    readonly string baseDirectory;readonly Dictionary<string,ResourceDocument> resources;
    public ResourceCatalog(string baseDirectory,IEnumerable<ResourceDocument> resources){this.baseDirectory=Path.GetFullPath(baseDirectory);this.resources=new(StringComparer.Ordinal);foreach(var r in resources){if(string.IsNullOrWhiteSpace(r.Id)||string.IsNullOrWhiteSpace(r.Path)||!this.resources.TryAdd(r.Id,r))throw new InvalidDataException("Invalid resource identity.");if(r.Sha256 is not null&&(r.Sha256.Length!=64||r.Sha256.Any(c=>!Uri.IsHexDigit(c))))throw new InvalidDataException("Invalid resource hash.");}}
    public string Resolve(string id)=>resources.TryGetValue(id,out var r)?Path.GetFullPath(r.Path,baseDirectory):throw new KeyNotFoundException("Unknown resource: "+id);
    // Caller owns the returned stream. Hash identity is checked before bytes reach a consumer.
    public Stream OpenVerified(string id){var r=resources.TryGetValue(id,out var value)?value:throw new KeyNotFoundException("Unknown resource: "+id);var stream=File.OpenRead(Resolve(id));try{if(r.Sha256 is not null){string actual=Convert.ToHexString(SHA256.HashData(stream));if(!string.Equals(actual,r.Sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Resource digest mismatch: "+id);stream.Position=0;}return stream;}catch{stream.Dispose();throw;}}
}
