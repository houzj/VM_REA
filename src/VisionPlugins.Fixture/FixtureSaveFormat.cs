using System.Globalization;
namespace VisionPlugins.Fixture;
// Source GetParam: float->double then MSVCR120 sprintf_s("%f").
// Preserves the parsed float rather than native textual spelling.
// C-locale CRT leaf probe confirms half-away tie and signed zero; VM process locale remains external.
internal static class FixtureSaveFormat {
 internal static float RoundTrip(float value){
  if(value==0)return value;
  return (float)Math.Round((double)value,6,MidpointRounding.AwayFromZero);
 }
}

