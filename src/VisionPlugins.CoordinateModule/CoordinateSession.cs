namespace VisionPlugins.Calibration;

// Dedicated runtime kernel candidate. No original pointer ABI is exposed.
// Initial zero matrix is an explicit memory-safety adaptation; original ctor does
// not initialize its matrix. Never claim first-run uninitialized-tail equivalence.
internal sealed class CoordinateSession
{
    public readonly float[] Matrix = new float[9];
    readonly bool[] written = new bool[9];
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Angle { get; private set; }
    public int Status { get; private set; }
    public bool HasCompleteMatrix => written.All(v=>v);
    public string? UnsupportedDiagnostic => HasCompleteMatrix?null:"Original matrix contains slots not yet written by this session; native initial heap values are unknown.";
    public readonly record struct NumericRead(uint ErrorCode,int Count,float Value,bool DestinationWritten);

    // Returns inner native diagnostic. Outer source Process publishes retained
    // state despite this return; adapter must not convert it into clear outputs.
    public uint Process(int create,int inputWay,(float X,float Y,float Angle)? roi,
        System.Func<string,NumericRead> read,System.Func<(uint ErrorCode,float[] Values)> readMatrix)
    {
        Status=0;
        if(create==0)
        {
            if(roi is {} point)SetPose(point.X,point.Y,point.Angle);
            // Missing ROI error falls through original timing tail and is overwritten.
        }
        else if(create==1)
        {
            if(inputWay is 0 or 1)
            {
                var x=read("SrcPointX");if(x.ErrorCode!=0)return x.ErrorCode;
                var y=read("SrcPointY");if(y.ErrorCode!=0)return y.ErrorCode;
                var a=read("SrcAngle");if(a.ErrorCode!=0)return a.ErrorCode;
                // Source does not test Count. Transport must track whether the native
                // destination slot was written; no-write leaves its initial zero.
                SetPose(x.DestinationWritten?x.Value:0f,y.DestinationWritten?y.Value:0f,a.DestinationWritten?a.Value:0f);
            }
            else if(inputWay==2)
            {
                var source=readMatrix();if(source.ErrorCode!=0)return source.ErrorCode;
                if(source.Values.Length is <1 or >9)return 0xe0000306;
                source.Values.CopyTo(Matrix,0);Array.Fill(written,true,0,source.Values.Length);
                // SDK checks all-zero before core call; SDK exceptions are caught by
                // module and continue into successful timing tail, preserving old pose.
                if(HasCompleteMatrix&&Matrix.Any(v=>v!=0f)&&MathF.Abs(Matrix[8])>=1.1920928955078125e-7f)
                {
                    var result=NPointSolver.MatrixToNativeParameters(Matrix);
                    if(result.ErrorCode==0&&result.Parameters is {} p)
                    {
                        X=Negate((float)p.DeltaX);Y=Negate((float)p.DeltaY);
                        Angle=Negate((float)p.Theta); // SDK radians, no unit conversion.
                    }
                }
            }
        }
        // Native VM_M_SetModuleRuntimeInfo success overwrites catch/draw diagnostic.
        Status=1;return 0;
    }
    static float Negate(float value)=>BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value)^unchecked((int)0x80000000));
    void SetPose(float x,float y,float angle)
    {
        X=x;Y=y;Angle=angle;
        double t=Math.Atan(1d)*4d*(double)angle/180d;
        Matrix[0]=(float)Math.Cos(t);Matrix[1]=(float)(1d*Math.Sin(t));
        Matrix[2]=(float)(-Math.Cos(t)*(double)x-Math.Sin(t)*(double)y);
        Matrix[3]=(float)-Math.Sin(t);Matrix[4]=(float)Math.Cos(t);
        Matrix[5]=(float)(Math.Sin(t)*(double)x-Math.Cos(t)*(double)y);
        Matrix[6]=0f;Matrix[7]=0f;Matrix[8]=1f;Array.Fill(written,true);
    }
    public float[] Axes()=>[50f*Matrix[0],50f*Matrix[1],50f*Matrix[3],50f*Matrix[0]];
}
