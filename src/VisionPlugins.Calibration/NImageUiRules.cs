namespace VisionPlugins.Calibration;

/// <summary>Rules from original HidePhysicInfoOperation and AlgorithmTab XML; no device operations.</summary>
public static class NImageUiRules
{
 public static bool ShowAutomaticCoordinates(int calibrationPointInput,string? pointBinding,string? xBinding,string? yBinding,string? angleBinding)=>string.IsNullOrEmpty(angleBinding)&&(calibrationPointInput==0?string.IsNullOrEmpty(pointBinding):string.IsNullOrEmpty(xBinding)&&string.IsNullOrEmpty(yBinding));
 public static int OtherCountMinimum(int changedCount)=>changedCount==0?1:0;
 public static bool ShowRotationCoordinates(int rotationCount)=>rotationCount!=0;
 public static bool ShowFixturePointSelectors(bool fixEnabled,int inputMode)=>fixEnabled&&inputMode==0;
 public static bool ShowFixtureCoordinateSelectors(bool fixEnabled,int inputMode)=>fixEnabled&&inputMode==1;
 public static readonly IReadOnlyDictionary<int,string> BoardTypes=new System.Collections.ObjectModel.ReadOnlyDictionary<int,string>(new Dictionary<int,string>{{7,"海康I型"},{6,"海康II型"},{9,"海康扩展I型"},{8,"海康扩展II型"}});
}
