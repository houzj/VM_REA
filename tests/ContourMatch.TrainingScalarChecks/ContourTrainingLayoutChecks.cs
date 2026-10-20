using System;
using VisionPlugins.ContourMatch;
public static class ContourTrainingLayoutChecks
{
    // Hook into the eventual dedicated scalar check Main; not built/executed.
    public static int Run()
    {
        int n=0;void Check(bool ok,string why){if(!ok)throw new Exception(why);n++;}
        Check(ContourTrainingPlaneOperations.EstimateLevelBound(12,12)==1,"dimension==6 after half stops");
        Check(ContourTrainingPlaneOperations.EstimateLevelBound(13,13)==2,"odd ceiling half permits7x7");
        Check(ContourTrainingPlaneOperations.EstimateLevelBound(28,28)==3,"repeat ceiling half stops at4");
        Check(ContourTrainingPlaneOperations.EstimateLevelBound(13,13,50)==1,"area49 below threshold50");
        byte[] plane=[1,2,3,99,4,5,6,99,7,8,9,99];
        var cut=ContourTrainingPlaneOperations.CopyRegion(plane,4,3,3,new(1,1,2,2));
        Check(cut.AsSpan().SequenceEqual(new byte[]{5,6,8,9}),"ROI copy has tight rows and excludes padding");
        cut[0]=0;Check(plane[5]==5,"owned copy does not mutate original");
        var zero=ContourModelPartitionPlan.Create(0,0,0,0,0);
        Check(zero.TotalBytes==256&&zero.Payload20==256&&zero.Payload12==256,"two128byte descriptor areas");
        var one=ContourModelPartitionPlan.Create(1,1,1,1,1);
        Check(one.Payload12==384&&one.Field38==512&&one.Field40==640,"20/12/pixel blocks independently align128");
        Check(one.Field48==768&&one.Field50==896&&one.Field58==1024&&one.Field60==1152&&one.TotalBytes==1280,"two8byte and three4byte blocks");
        bool failed=false;try{ContourModelPartitionPlan.Create(-1,0,0,0,0);}catch(ArgumentOutOfRangeException){failed=true;}
        Check(failed,"negative raw count independent safe rejection");
        failed=false;try{ContourModelPartitionPlan.Create(0,0,int.MaxValue,int.MaxValue,0);}catch(OverflowException){failed=true;}
        Check(failed,"total size checked overflow without wrapped addresses");
        return n;
    }
}
