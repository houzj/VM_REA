using VisionWorkflow;
using VisionCore;
namespace VisionWorkbench;
sealed partial class MainForm {
    public void VerifyPlanPersistenceForSmoke(string path){var expected=Plan();OfflinePlanStore.Save(path,expected with {ImagePath=Path.GetRelativePath(Path.GetDirectoryName(Path.GetFullPath(path))!,expected.ImagePath)});var actual=OfflinePlanStore.Load(path);if(actual.Options!=expected.Options||actual.ImagePath!=expected.ImagePath)throw new InvalidOperationException("Plan persistence mismatch.");}
    public void RunSynchronouslyForSmoke(){ShowRun(OfflineCircleExecutor.Run(Plan()));if(last!.Detection.Status!=CircleDetectionStatus.Success)throw new InvalidOperationException("Sample detection failed.");}
}
