using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Native 28-byte candidate semantics: XY floats, integer radius, then two direction float pairs.
/// Contrast/response scores of independent lattice detectors are deliberately not represented here.</summary>
public readonly record struct NImageSourceCandidate(float X,float Y,int Radius,float DirectionAX=0,float DirectionAY=0,float DirectionBX=0,float DirectionBY=0);

/// <summary>Source-order 8bb3a0 candidate direction phase. Product lattice selection is a separate algorithm.</summary>
public static class NImageSourceCandidateDescriptor
{
    static float F(uint bits)=>BitConverter.Int32BitsToSingle((int)bits);
    static readonly float Pi=F(0x40490fda),HalfPi=F(0x3fc90fdb),BinScale=F(0x41098287),BinWidth=F(0x3dee4bae);
    static readonly float[] Smooth=[F(0x3c360282),F(0x3e0a9555),F(0x3f1b4598),1,F(0x3f1b4598),F(0x3e0a9555),F(0x3c360282)];
    static readonly int[][] Kernels=[[1,0,-1,2,0,-2,1,0,-1],[1,2,1,0,0,0,-1,-2,-1]];
    public static NImageSourceCandidate? Describe(GrayImage image,NImageSourceCandidate candidate,int count,float factor,float minAngle,float maxAngle)
    {
        ArgumentNullException.ThrowIfNull(image);if(count<0)throw new ArgumentOutOfRangeException(nameof(count));float x=candidate.X,y=candidate.Y;int originalRadius=candidate.Radius,width=image.Width,height=image.Height;
        if(!float.IsFinite(x)||!float.IsFinite(y)||originalRadius<0)return null;
        int radius=x-11<0||x+11>=width-1||y-11<0||y+11>=height-1?originalRadius:11;
        if(x-radius<0||y-radius<0||x+radius>=width||y+radius>=height)return null;
        int size=radius*2+1;var patch=new float[size*size];
        for(int py=0;py<size;py++)for(int px=0;px<size;px++)patch[py*size+px]=image[(int)x+px-radius,(int)y+py-radius];
        var gradients=new float[2][];
        for(int axis=0;axis<2;axis++)
        {
            var values=new float[patch.Length];gradients[axis]=values;
            for(int py=0;py<size;py++)for(int px=0;px<size;px++)
            {
                float value=0;
                for(int ky=0;ky<3;ky++)for(int kx=0;kx<3;kx++)value=value+Kernels[axis][ky*3+kx]*patch[Math.Clamp(py+ky-1,0,size-1)*size+Math.Clamp(px+kx-1,0,size-1)];
                values[py*size+px]=value;
            }
        }
        var histogram=new float[27];
        for(int i=0;i<patch.Length;i++)
        {
            float gx=gradients[0][i],gy=gradients[1][i];
            float angle=MathF.Atan2(gy,gx);angle=angle<=Pi?(angle>=0?angle:angle+Pi):angle-Pi;
            float magnitude=MathF.Sqrt(gx*gx+gy*gy),shift=angle+HalfPi;
            if(shift>Pi)shift=shift-Pi;
            int bin=(int)MathF.Floor(shift*BinScale);histogram[bin]=histogram[bin]+magnitude;
        }
        var smoothed=new float[27];
        for(int i=0;i<27;i++)
        {
            int dest=(i+3)%27;float value=0;
            for(int k=0;k<7;k++)value=histogram[(i+k)%27]*Smooth[k]+value;
            smoothed[dest]=value;
        }
        float maximum=smoothed.Max();if((double)maximum<.0001)return null;
        var visited=new bool[27];var peaks=new float[27];
        for(int i=0;i<27;i++)
        {
            if(visited[i])continue;
            int current=i,previous,next;
            do
            {
                while(true)
                {
                    next=(current+1)%27;previous=(current+26)%27;visited[current]=true;
                    if(smoothed[next]<smoothed[current]||smoothed[next]<smoothed[previous])break;
                    current=next;
                }
                if(!(smoothed[current]<smoothed[previous]&&smoothed[next]<smoothed[previous]))break;
                current=previous;
            }while(true);
            peaks[current]=smoothed[current];
        }
        int first=0,second=0;float top=0,runner=0,third=0;
        for(int i=0;i<27;i++)if(top<peaks[i]){top=peaks[i];first=i;}
        for(int i=0;i<27;i++)if(runner<peaks[i]&&peaks[i]<top){runner=peaks[i];second=i;}
        for(int i=0;i<27;i++)if(third<peaks[i]&&peaks[i]<runner)third=peaks[i];
        int peakCount=maximum>0?1:0;
        if(runner>0&&maximum<factor*runner)peakCount++;
        if(third>0&&maximum<third*factor&&(double)runner<(double)third*1.2)peakCount++;
        float angleA=(first+.5f)*BinWidth,angleB=(second+.5f)*BinWidth;
        if(angleA>angleB)(angleA,angleB)=(angleB,angleA);
        float separation=F(0x40490fdb)+angleA-angleB;
        if(angleB-angleA<=separation)separation=angleB-angleA;
        bool angles=F(0x3c8ef77f)*minAngle<separation&&separation<F(0x3c8ef77f)*maxAngle;
        if(count>1000){if(peakCount!=2||!angles)return null;}
        else if(minAngle!=30||maxAngle!=135||factor!=3){if(peakCount<=1||!angles)return null;}
        float ca=MathF.Cos(angleA),sa=MathF.Sin(angleA),cb=MathF.Cos(angleB),sb=MathF.Sin(angleB);
        var matrixA=new double[4];var matrixB=new double[4];
        for(int i=0;i<patch.Length;i++)
        {
            float gx=gradients[0][i],gy=gradients[1][i],xx=gx*gx,yy=gy*gy,length=MathF.Sqrt(xx+yy);
            if(length<.1f)continue;
            double da=(double)ca*(double)(gx/length)+(double)sa*(double)(gy/length);
            double db=(double)(gx/length)*(double)cb+(double)(gy/length)*(double)sb;
            void Add(double[] matrix){matrix[0]=(double)xx+matrix[0];matrix[1]=matrix[1]+(double)(gx*gy);matrix[2]=matrix[2]+(double)(gx*gy);matrix[3]=(double)yy+matrix[3];}
            if(Math.Abs(da)<.25)Add(matrixA);if(Math.Abs(db)<.25)Add(matrixB);
        }
        var a=Eigen(matrixA);var b=Eigen(matrixB);if(a==null||b==null)return null;
        double ax=a[2],ay=a[3],bx=b[2],by=b[3];
        if(ax*by-ay*bx>0){(ax,bx)=(bx,ax);(ay,by)=(by,ay);}
        bx=-bx;by=-by;
        if(ax*by-ay*bx>0){(ax,bx)=(bx,ax);(ay,by)=(by,ay);}
        return new(x,y,radius,(float)ax,(float)ay,(float)bx,(float)by);
    }
    static double[]? Eigen(double[] matrix)
    {
        double a=matrix[0],b=matrix[1],c=matrix[2],d=matrix[3],trace=a+d,negative=-trace;
        double disc=negative*negative-(d*a-c*b)*4;if(disc<0)return null;
        double e0,e1;if(disc==0)e0=e1=trace*.5;else{e0=-(negative+Math.Sqrt(disc))*.5;e1=(Math.Sqrt(disc)-negative)*.5;if(e1>e0)(e0,e1)=(e1,e0);}
        var vectors=new double[4];
        if(Math.Abs(b)<=.0001||Math.Abs(c)<=.0001){if(Math.Abs(a)<=.0001){if(Math.Abs(d)>.0001){vectors[1]=vectors[2]=1;}}else vectors[0]=vectors[3]=1;return vectors;}
        for(int i=0;i<2;i++)
        {
            double value=i==0?e0:e1,denom=i==0?-(b*c):-b*c;
            double x=((d-value)*(value-a))/denom,y=((value-a)*x)/b;
            if(Math.Abs(x)>=.0001||Math.Abs(y)>=.0001){double norm=Math.Sqrt(x*x+y*y);x=x/norm;y=y/norm;if(Math.Abs(y)>Math.Abs(x)&&y<0){x=-x;y=-y;}}
            vectors[i*2]=x;vectors[i*2+1]=y;
        }
        return vectors;
    }
}
