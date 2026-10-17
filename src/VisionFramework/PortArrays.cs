namespace VisionFramework;

// Managed reconstruction contract, not the native SDK's pointer layout or numeric ABI.
public enum ValueCardinality { Scalar, Array }
public static class PortTypes
{
 public static bool Valid(DataKind kind,ValueCardinality cardinality)=>Enum.IsDefined(kind)&&Enum.IsDefined(cardinality)&&(cardinality==ValueCardinality.Scalar||kind is DataKind.Integer or DataKind.Number or DataKind.Text or DataKind.Circle or DataKind.Point or DataKind.Fixture);
 public static bool Matches(PortDescriptor port,PortValue? value)=>value is not null&&port.Kind==value.Kind&&port.Cardinality==value.Cardinality;
}
public sealed partial class PortValue
{
 public static PortValue CircleArray(IEnumerable<VisionCore.CircleFit> values){var copy=values.ToArray();foreach(var v in copy)_=Circle(v);return new(DataKind.Circle,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
 public static PortValue PointArray(IEnumerable<VisionCore.Point2> values){var copy=values.ToArray();foreach(var v in copy)_=Point(v);return new(DataKind.Point,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
 public static PortValue FixtureArray(IEnumerable<FixtureValue> values){var copy=values.ToArray();foreach(var v in copy)_=Fixture(v);return new(DataKind.Fixture,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
 public int ElementCount=>Cardinality==ValueCardinality.Scalar?1:((System.Collections.ICollection)Value).Count;
 public PortValue Element(int index){if(index<0||index>=ElementCount)throw new InvalidDataException("Array element unavailable.");if(Cardinality==ValueCardinality.Scalar)return this;return Kind switch{DataKind.Integer=>Integer(As<IReadOnlyList<long>>()[index]),DataKind.Number=>Number(As<IReadOnlyList<double>>()[index]),DataKind.Text=>Text(As<IReadOnlyList<string>>()[index]),DataKind.Circle=>Circle(As<IReadOnlyList<VisionCore.CircleFit>>()[index]),DataKind.Point=>Point(As<IReadOnlyList<VisionCore.Point2>>()[index]),DataKind.Fixture=>Fixture(As<IReadOnlyList<FixtureValue>>()[index]),_=>throw new InvalidDataException("Unsupported array type.")};}
 public IEnumerable<PortValue> Elements()=>Enumerable.Range(0,ElementCount).Select(Element);
 public static PortValue JoinArray(DataKind kind,IEnumerable<PortValue> values){var flat=values.SelectMany(v=>v.Kind==kind?v.Elements():throw new InvalidDataException("Mixed output kinds.")).ToArray();return kind switch{DataKind.Integer=>IntegerArray(flat.Select(v=>v.As<long>())),DataKind.Number=>NumberArray(flat.Select(v=>v.As<double>())),DataKind.Text=>TextArray(flat.Select(v=>v.As<string>())),DataKind.Circle=>CircleArray(flat.Select(v=>v.As<VisionCore.CircleFit>())),DataKind.Point=>PointArray(flat.Select(v=>v.As<VisionCore.Point2>())),DataKind.Fixture=>FixtureArray(flat.Select(v=>v.As<FixtureValue>())),_=>throw new InvalidDataException("Unsupported accumulated type.")};}
 public static PortValue IntegerArray(IEnumerable<long> values){ArgumentNullException.ThrowIfNull(values);return new(DataKind.Integer,System.Array.AsReadOnly(values.ToArray()),ValueCardinality.Array);}
 public static PortValue NumberArray(IEnumerable<double> values){ArgumentNullException.ThrowIfNull(values);var copy=values.ToArray();if(copy.Any(v=>!double.IsFinite(v)))throw new ArgumentException("Non-finite number array element.",nameof(values));return new(DataKind.Number,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
 public static PortValue TextArray(IEnumerable<string> values){ArgumentNullException.ThrowIfNull(values);var copy=values.ToArray();if(copy.Any(v=>v is null))throw new ArgumentException("Null text array element.",nameof(values));return new(DataKind.Text,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
}
