using System;
using VisionPlugins.ContourMatch;
public static class ContourTrainingScalarChecks
{
    public static int Main()
    {
        int checks=0;void Check(bool value,string message){if(!value)throw new Exception(message);checks++;}
        var mask=new byte[40*20];mask[3*40+2]=1;mask[15*40+20]=128;mask[0*40+33]=255;
        Check(ContourTrainingScalars.NonzeroMaskBounds(mask,40,32,20)==new ContourTrainingScalars.Rectangle(2,3,19,13),"all nonzero values select; row padding ignored");
        Check(ContourTrainingScalars.NonzeroMaskBounds(new byte[20*20],20,20,20) is null,"empty mask status has no usable ROI");
        var narrow=new byte[8*16];narrow[15*8+7]=255;
        Check(ContourTrainingScalars.NonzeroMaskBounds(narrow,8,8,16)==new ContourTrainingScalars.Rectangle(7,15,1,1),"narrow branch bottom right inclusive");
        Check(ContourTrainingScalars.ExpandMaskBounds(new(0,0,16,16),64,64)==new ContourTrainingScalars.Rectangle(0,0,24,24),"minimum margin8 clips to image");
        Check(ContourTrainingScalars.ExpandMaskBounds(new(100,50,1000,400),2000,1000)==new ContourTrainingScalars.Rectangle(75,40,1050,420),"2.5 percent separate margins and max25");
        Check(ContourTrainingScalars.ExpandMaskBounds(new(32,24,1,1),64,64)==new ContourTrainingScalars.Rectangle(24,16,17,17),"single selected pixel expansion inclusive");
        var image=new byte[32];var selected=new byte[32];selected[0]=255;selected[1]=1;image[0]=100;image[1]=101;image[2]=255;
        Check(!ContourTrainingScalars.HasMaskedContrast(image,32,selected,32,32,1),"threshold2 rejects one level contrast; unselected bright excluded");
        image[1]=102;Check(ContourTrainingScalars.HasMaskedContrast(image,32,selected,32,32,1),"exact two levels accepted");
        Check(!ContourTrainingScalars.HasMaskedContrast(image,32,new byte[32],32,32,1),"empty mask max0 min255 fails");
        Check(BitConverter.SingleToInt32Bits(ContourTrainingScalars.ExplicitPyramidSupport(16,16))==0x3fb504f3,"source SSE float sqrt2 result");
        bool refused=false;try{ContourTrainingScalars.ExplicitPyramidSupport(50000,50000);}catch(NotSupportedException){refused=true;}Check(refused,"signed int area overflow separate safe admission");
        refused=false;try{ContourTrainingScalars.NonzeroMaskBounds([1],16,16,16);}catch(ArgumentException){refused=true;}Check(refused,"short buffer rejected without unsafe native pointer read");
        checks+=ContourTrainingLayoutChecks.Run();Console.WriteLine($"PASS {checks} training scalar source checks");return 0;
    }
}

