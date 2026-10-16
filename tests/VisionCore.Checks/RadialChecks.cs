using VisionCore;

static class RadialChecks
{
    public static void Run(Action<bool,string> check)
    {
        GrayImage Disk(int w,int h,double cx,double cy,double r,bool brightInside=true)
        {
            var pixels = new byte[w*h];
            for(int y=0;y<h;y++) for(int x=0;x<w;x++) {
                double d=Math.Sqrt((x-cx)*(x-cx)+(y-cy)*(y-cy));
                // Smooth one-pixel edge gives a known geometric radius independent of sampling.
                double intensity=Math.Clamp((r-d)+.5,0,1)*255;
                pixels[y*w+x]=(byte)Math.Round(brightInside?intensity:255-intensity);
            }
            return new(w,h,pixels);
        }
        var options = new RadialCircleOptions(64.2,63.7,25,5,128,40,EdgePolarity.LightToDark);
        var found=RadialCircleDetector.Detect(Disk(128,128,64.2,63.7,26.3),options);
        check(found.Status==CircleDetectionStatus.Success && found.Circle is not null,"Radial detector returns fit and edge points");
        var circle=found.Circle!.Value;
        check(Math.Abs(circle.CenterX-64.2)<.3 && Math.Abs(circle.CenterY-63.7)<.3 && Math.Abs(circle.Radius-26.3)<.4 && found.EdgePoints.Count==128,"Radial known disk center and radius");
        found=RadialCircleDetector.Detect(Disk(128,128,64.2,63.7,26.3),options with {Polarity=EdgePolarity.DarkToLight});
        check(found.Status==CircleDetectionStatus.InsufficientEdges && found.Circle is null && found.EdgePoints.Count==0,"Opposite polarity rejects bright disk edge");
        found=RadialCircleDetector.Detect(Disk(128,128,64.2,63.7,26.3,false),options with {Polarity=EdgePolarity.DarkToLight});
        check(found.Status==CircleDetectionStatus.Success && Math.Abs(found.Circle!.Value.Radius-26.3)<.4,"Reverse contrast uses outward dark-to-light polarity");
        found=RadialCircleDetector.Detect(new GrayImage(128,128,Enumerable.Repeat((byte)123,128*128).ToArray()),options);
        check(found.Status==CircleDetectionStatus.InsufficientEdges && found.Circle is null,"Flat image never reports a circle");
        found=RadialCircleDetector.Detect(Disk(80,80,10,40,25),options with {CenterX=10,CenterY=40,InitialRadius=25});
        check(found.Status==CircleDetectionStatus.Success && found.OutOfBoundsSamples>0 && found.EdgePoints.Count<128 && Math.Abs(found.Circle!.Value.Radius-25)<.5,"Partial image bounds skip unavailable rays and fit observed edges");
        found=RadialCircleDetector.Detect(Disk(80,80,10,40,25),options with {CenterX=-200,CenterY=-200});
        check(found.Status==CircleDetectionStatus.InsufficientEdges && found.OutOfBoundsSamples==128,"Wholly out-of-image search has no fabricated edges");
        var invalid=new[]{options with {CenterX=double.NaN}, options with {InitialRadius=1}, options with {SearchHalfWidth=0}, options with {RadialStep=0}, options with {GradientThreshold=0}, options with {SampleCount=2}, options with {MinimumEdges=129}, options with {Polarity=(EdgePolarity)99}};
        foreach(var bad in invalid) {
            bool rejected=false;
            try { RadialCircleDetector.Detect(new GrayImage(1,1,new byte[1]),bad); } catch(ArgumentOutOfRangeException) {rejected=true;}
            check(rejected,"Invalid radial option rejected");
        }
        bool failed=false;
        try { Algorithms.FitCircle(new[]{new Point2(0,0),new Point2(1,0),new Point2(double.NaN,1)}); } catch(ArgumentException) {failed=true;}
        check(failed,"Nonfinite circle point rejected");
        failed=false;
        try { Algorithms.FitCircle(new[]{new Point2(0,0),new Point2(1,0)}); } catch(ArgumentException) {failed=true;}
        check(failed,"Insufficient circle point set rejected");
    }
}
