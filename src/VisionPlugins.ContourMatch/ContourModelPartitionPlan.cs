using System;
namespace VisionPlugins.ContourMatch;
// d215d0 relative layout. This describes owned logical blocks; it never emits
// vendor pointers or claims a persisted model format.
public readonly record struct ContourModelPartitionPlan(long DescriptorA,long DescriptorB,
    long Payload20,long Payload12,long Field38,long Field40,long Field48,long Field50,
    long Field58,long Field60,long TotalBytes)
{
    static long Align128(long value)=>checked((value+127)&~127L);
    public static ContourModelPartitionPlan Create(int count20,int count12,int width,int height,int countAt1c)
    {
        // Explicit independent bounds/overflow admission; original helper uses
        // signed native arithmetic and writes raw addresses without validation.
        if(count20<0||count12<0||width<0||height<0||countAt1c<0)
            throw new ArgumentOutOfRangeException(nameof(count20));
        long a=Align128(checked(count20*20L)),b=Align128(checked(count12*12L));
        long pixels=Align128(checked(width*(long)height*8L));
        long c=Align128(countAt1c*8L),d=Align128(countAt1c*4L);
        long p20=256,p12=checked(p20+a),f38=checked(p12+b),f40=checked(f38+pixels);
        long f48=checked(f40+c),f50=checked(f48+c),f58=checked(f50+d),f60=checked(f58+d);
        return new(0,128,p20,p12,f38,f40,f48,f50,f58,f60,checked(f60+d));
    }
}
