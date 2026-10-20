namespace VisionPlugins.DistortionCalibration;
/// <summary>Module 180020b70 rounds each raw SDK point error to hundredths before
/// publishing it and comparing the strict greater-than NG threshold.</summary>
public static class DistortionCalibrationPointEvaluation {
 public static (float[] Errors,float[] Statuses) Project(IReadOnlyList<float> rawErrors,float threshold){
  var errors=new float[rawErrors.Count];var statuses=new float[rawErrors.Count];for(int i=0;i<errors.Length;i++){errors[i]=MathF.Round(rawErrors[i]*100f,MidpointRounding.AwayFromZero)/100f;statuses[i]=errors[i]>threshold?0f:1f;}return(errors,statuses);
 }
}
