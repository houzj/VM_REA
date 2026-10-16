using System.Security.Cryptography;
using VisionCore;
namespace VisionFramework;
public enum RoiKind { Rectangle, Circle, Annulus, Polygon }
public enum RoiCoordinateSpace { ImagePixels, Fixture }
public sealed record RoiDocument(string Id,RoiKind Kind,double CenterX,double CenterY,double Width=0,double Height=0,double Radius=0,double InnerRadius=0,double AngleDegrees=0,IReadOnlyList<Point2>? Vertices=null,RoiCoordinateSpace CoordinateSpace=RoiCoordinateSpace.ImagePixels);
public static class RoiValidator {
    public static void Validate(RoiDocument roi){ArgumentNullException.ThrowIfNull(roi);if(string.IsNullOrWhiteSpace(roi.Id)||!Enum.IsDefined(roi.Kind)||!Enum.IsDefined(roi.CoordinateSpace)||new[]{roi.CenterX,roi.CenterY,roi.Width,roi.Height,roi.Radius,roi.InnerRadius,roi.AngleDegrees}.Any(v=>!double.IsFinite(v)))throw new InvalidDataException("Invalid ROI identity or numeric value.");
        if(roi.Kind==RoiKind.Rectangle&&(roi.Width<=0||roi.Height<=0)||roi.Kind==RoiKind.Circle&&roi.Radius<=0||roi.Kind==RoiKind.Annulus&&(roi.InnerRadius<0||roi.Radius<=roi.InnerRadius)||roi.Kind==RoiKind.Polygon&&(roi.Vertices is null||roi.Vertices.Count<3||roi.Vertices.Count>10000||roi.Vertices.Any(p=>!double.IsFinite(p.X)||!double.IsFinite(p.Y))))throw new InvalidDataException("Invalid ROI geometry.");
    }
}
public sealed class ResourceCatalog {
    readonly string baseDirectory;readonly Dictionary<string,ResourceDocument> resources;
    public ResourceCatalog(string baseDirectory,IEnumerable<ResourceDocument> resources){this.baseDirectory=Path.GetFullPath(baseDirectory);this.resources=new(StringComparer.Ordinal);foreach(var r in resources){if(string.IsNullOrWhiteSpace(r.Id)||string.IsNullOrWhiteSpace(r.Path)||!this.resources.TryAdd(r.Id,r))throw new InvalidDataException("Invalid resource identity.");if(r.Sha256 is not null&&(r.Sha256.Length!=64||r.Sha256.Any(c=>!Uri.IsHexDigit(c))))throw new InvalidDataException("Invalid resource hash.");}}
    public string Resolve(string id)=>resources.TryGetValue(id,out var r)?Path.GetFullPath(r.Path,baseDirectory):throw new KeyNotFoundException("Unknown resource: "+id);
    // Caller owns the returned stream. Hash identity is checked before bytes reach a consumer.
    public Stream OpenVerified(string id){var r=resources.TryGetValue(id,out var value)?value:throw new KeyNotFoundException("Unknown resource: "+id);var stream=File.OpenRead(Resolve(id));try{if(r.Sha256 is not null){string actual=Convert.ToHexString(SHA256.HashData(stream));if(!string.Equals(actual,r.Sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Resource digest mismatch: "+id);stream.Position=0;}return stream;}catch{stream.Dispose();throw;}}
}
