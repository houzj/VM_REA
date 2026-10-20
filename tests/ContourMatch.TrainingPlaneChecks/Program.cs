using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using VisionPlugins.ContourMatch;
static class Program
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void Gradient(IntPtr image,int stride,IntPtr dx,IntPtr dy,ulong wh);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void Magnitude(IntPtr dx,IntPtr dy,int stride,ulong wh,IntPtr result);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate float Maximum(IntPtr data,int stride,ulong wh);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate float MaskMaximum(IntPtr data,int stride,IntPtr mask,int maskStride,ulong wh);
    sealed class Buffer:IDisposable
    {
        readonly IntPtr allocation;readonly int length;public IntPtr Data{get;}
        public Buffer(int bytes){length=bytes;allocation=Marshal.AllocHGlobal(checked(bytes+256));Data=allocation+128;Marshal.Copy(Enumerable.Repeat((byte)0xa5,bytes+256).ToArray(),0,allocation,bytes+256);}
        public void Set(byte[] bytes)=>Marshal.Copy(bytes,0,Data,bytes.Length);
        public void Set(float[] values)=>Marshal.Copy(values,0,Data,values.Length);
        public short[] Shorts(int count){var a=new short[count];Marshal.Copy(Data,a,0,count);return a;}
        public float[] Floats(int count){var a=new float[count];Marshal.Copy(Data,a,0,count);return a;}
        public void CheckGuard(){var a=new byte[128];Marshal.Copy(allocation,a,0,128);if(a.Any(v=>v!=0xa5))throw new Exception("Native prefix output overwrite");Marshal.Copy(Data+length,a,0,128);if(a.Any(v=>v!=0xa5))throw new Exception("Native suffix output overwrite");}
        public void Dispose()=>Marshal.FreeHGlobal(allocation);
    }
    static int Main(string[] args)
    {
        if(!args.Contains("--native-leaf"))return RunManagedOnly();
        const string sha="85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549";
        const string path="D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll";
        string actual=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));if(actual!=sha)throw new Exception("Original core identity mismatch");
        Console.WriteLine("Native pure-leaf target SHA256="+actual+"; RVAs c03e10/c04af0/c03c80/c03d30 only");
        var library=NativeLibrary.Load(path,typeof(Program).Assembly,DllImportSearchPath.UseDllDirectoryForDependencies|DllImportSearchPath.System32);
        try{
            T At<T>(int rva)where T:Delegate=>Marshal.GetDelegateForFunctionPointer<T>(library+rva);
            var gradient=At<Gradient>(0xc03e10);var magnitude=At<Magnitude>(0xc04af0);var maximum=At<Maximum>(0xc03c80);var maskMaximum=At<MaskMaximum>(0xc03d30);
            int checks=0;void Check(bool ok,string why){if(!ok)throw new Exception(why);checks++;}
            foreach(var shape in new[]{(w:2,h:2),(w:3,h:3),(w:4,h:4),(w:17,h:19),(w:32,h:16)}.Where((_,i)=>!args.Contains("--quick-native")||i==0)){
                int w=shape.w,h=shape.h,stride=w+5,count=w*h;ulong wh=((ulong)(uint)h<<32)|(uint)w;
                var image=Enumerable.Repeat((byte)239,stride*h).ToArray();for(int y=0;y<h;y++)for(int x=0;x<w;x++)image[y*stride+x]=(byte)((x*37+y*73+x*y*11)&255);
                var product=ContourTrainingPlaneKernels.Compute(image,stride,w,h);
                using var input=new Buffer(image.Length);using var dx=new Buffer(count*2);using var dy=new Buffer(count*2);using var mag=new Buffer(count*4);input.Set(image);
                gradient(input.Data,stride,dx.Data,dy.Data,wh);magnitude(dx.Data,dy.Data,w,wh,mag.Data);
                var nativeX=dx.Shorts(count);var nativeY=dy.Shorts(count);var nativeM=mag.Floats(count);
                for(int i=0;i<count;i++)Check(nativeX[i]==product.Horizontal[i]&&nativeY[i]==product.Vertical[i],$"{w}x{h} gradient first difference x={i%w} y={i/w}: native {nativeX[i]}/{nativeY[i]} product {product.Horizontal[i]}/{product.Vertical[i]}");
                for(int i=0;i<count;i++)Check(BitConverter.SingleToInt32Bits(nativeM[i])==BitConverter.SingleToInt32Bits(product.Magnitude[i]),$"{w}x{h} magnitude first difference index={i}: native {nativeM[i]:R} product {product.Magnitude[i]:R}");
                Check(BitConverter.SingleToInt32Bits(maximum(mag.Data,w,wh))==BitConverter.SingleToInt32Bits(ContourTrainingPlaneKernels.Maximum(product.Magnitude,[],false)),"plain float maximum");
                foreach(int kind in new[]{0,1,2}){
                    var mask=new byte[count];for(int i=0;i<count;i++)mask[i]=kind==0?(byte)0:kind==1?(byte)255:i%4==0?(byte)1:i%4==1?(byte)128:(byte)0;
                    using var selected=new Buffer(count);selected.Set(mask);float original=maskMaximum(mag.Data,w,selected.Data,w,wh);float managed=ContourTrainingPlaneKernels.Maximum(product.Magnitude,mask,true);
                    Check(BitConverter.SingleToInt32Bits(original)==BitConverter.SingleToInt32Bits(managed),$"{w}x{h} mask kind={kind} maximum {original:R} vs {managed:R}");selected.CheckGuard();
                }
                input.CheckGuard();dx.CheckGuard();dy.CheckGuard();mag.CheckGuard();Check(true,"prefix/suffix canaries intact");
                Console.WriteLine($"LEAF_PASS {w}x{h}; inputStride={stride}; full XY/magnitude bits, plain/mixed/all/empty max, guards");
            }
            var ramp=ContourTrainingPlaneKernels.Compute(new byte[]{1,2,3,4,5,6,7,8,9},3,3,3);
            Check(ramp.Horizontal.SequenceEqual(new short[]{4,8,4,4,8,4,4,8,4}),"source boundary/interior literal horizontal ramp");
            Check(ramp.Vertical.SequenceEqual(new short[]{12,12,12,24,24,24,12,12,12}),"source four edges/corners literal vertical ramp");
            Console.WriteLine($"PASS {checks} training-plane original-leaf differential checks");return 0;
        }finally{if(args.Contains("--skip-native-free")){Console.WriteLine("EXPLICIT_NATIVE_FREE_SKIPPED_PROCESS_EXIT");Console.Out.Flush();}else{Console.WriteLine("BEFORE_NATIVE_FREE");Console.Out.Flush();NativeLibrary.Free(library);Console.WriteLine("AFTER_NATIVE_FREE");Console.Out.Flush();}}
    }
    static int RunManagedOnly()
    {
        int n=0;void Check(bool ok,string why){if(!ok)throw new Exception(why);n++;}
        var p=ContourTrainingPlaneKernels.Compute(new byte[]{1,2,3,4,5,6,7,8,9},3,3,3);
        Check(p.Horizontal.SequenceEqual(new short[]{4,8,4,4,8,4,4,8,4}),"literal X boundaries/corners/interior");
        Check(p.Vertical.SequenceEqual(new short[]{12,12,12,24,24,24,12,12,12}),"literal Y boundaries/corners/interior");
        var zero=ContourTrainingPlaneKernels.Compute(new byte[4],2,2,2);
        Check(zero.Magnitude.All(v=>BitConverter.SingleToInt32Bits(v)==0),"constant minimum plane zero gradient magnitude");
        Check(BitConverter.SingleToInt32Bits(ContourTrainingPlaneKernels.Maximum(p.Magnitude,new byte[9],true))==unchecked((int)0xff7fffff),"empty selection original negative FLT_MAX");
        Check(ContourTrainingPlaneKernels.Maximum(new float[]{2,9,4},new byte[]{128,0,1},true)==4,"every nonzero mask selects; excluded peak ignored");
        Check(BitConverter.SingleToInt32Bits(ContourTrainingPlaneKernels.Maximum(new float[]{-0.0f,+0.0f},[],false))==unchecked((int)0x80000000),"max equal retains previous negative zero");
        bool rejected=false;try{ContourTrainingPlaneKernels.Compute([0],2,2,2);}catch(ArgumentException){rejected=true;}Check(rejected,"short accessible image safe refusal");
        rejected=false;try{ContourTrainingPlaneKernels.Maximum([float.NaN],[],false);}catch(NotSupportedException){rejected=true;}Check(rejected,"arbitrary NaN outside generated finite gradient domain");
        Console.WriteLine($"PASS {n} managed training-plane source checks; native execution opt-in only");return 0;
    }
}


