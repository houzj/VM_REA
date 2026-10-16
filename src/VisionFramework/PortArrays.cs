namespace VisionFramework;

// Managed reconstruction contract, not the native SDK's pointer layout or numeric ABI.
public enum ValueCardinality { Scalar, Array }
public static class PortTypes
{
 public static bool Valid(DataKind kind,ValueCardinality cardinality)=>Enum.IsDefined(kind)&&Enum.IsDefined(cardinality)&&(cardinality==ValueCardinality.Scalar||kind is DataKind.Integer or DataKind.Number or DataKind.Text);
 public static bool Matches(PortDescriptor port,PortValue? value)=>value is not null&&port.Kind==value.Kind&&port.Cardinality==value.Cardinality;
}
public sealed partial class PortValue
{
 public static PortValue IntegerArray(IEnumerable<long> values){ArgumentNullException.ThrowIfNull(values);return new(DataKind.Integer,System.Array.AsReadOnly(values.ToArray()),ValueCardinality.Array);}
 public static PortValue NumberArray(IEnumerable<double> values){ArgumentNullException.ThrowIfNull(values);var copy=values.ToArray();if(copy.Any(v=>!double.IsFinite(v)))throw new ArgumentException("Non-finite number array element.",nameof(values));return new(DataKind.Number,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
 public static PortValue TextArray(IEnumerable<string> values){ArgumentNullException.ThrowIfNull(values);var copy=values.ToArray();if(copy.Any(v=>v is null))throw new ArgumentException("Null text array element.",nameof(values));return new(DataKind.Text,System.Array.AsReadOnly(copy),ValueCardinality.Array);}
}
