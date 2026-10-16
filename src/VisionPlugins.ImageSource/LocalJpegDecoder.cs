using System.IO;
using System.Runtime.InteropServices;
using VisionIO;

namespace VisionPlugins.ImageSource;

/// <summary>Independent upstream TurboJPEG backend; never loads a vendor DLL.</summary>
internal static class LocalJpegDecoder
{
 internal static (int Width,int Height,byte[] Rgb) DecodeRgb(Stream stream,int flags=0)
 {
  if(!OperatingSystem.IsWindows()||RuntimeInformation.ProcessArchitecture!=Architecture.X64)throw new NotSupportedException("JPEG 后端当前仅支持 Windows x64。");
  if(stream.CanSeek&&stream.Length>256L*1024*1024)throw new InvalidDataException("JPEG exceeds encoded resource limit.");
  using var input=new MemoryStream();stream.CopyTo(input);byte[] encoded=input.ToArray();
  Api api;
  try {api=Backend.Value;}catch(Exception ex)when(ex is DllNotFoundException or BadImageFormatException or EntryPointNotFoundException){throw new NotSupportedException("JPEG 解码后端未正确部署。",ex);}
  IntPtr decoder=api.Init();if(decoder==IntPtr.Zero)throw new InvalidDataException("JPEG decoder creation failed.");
  try {
   if(api.Header(decoder,encoded,checked((uint)encoded.Length),out int width,out int height,out _,out _)!=0)throw Error(api,decoder);
   if(width<=0||height<=0||(long)width*height>BmpReader.MaxPixels)throw new InvalidDataException("JPEG exceeds image resource limit.");
   byte[] pixels=new byte[checked(width*height*3)];
   // Original FUN_18038c8a0 -> tjDecompress2: TJPF_RGB=0, pitch=0, flags=0.
   int result=api.Decode(decoder,encoded,checked((uint)encoded.Length),pixels,width,0,height,0,flags);
   // Original wrapper accepts TurboJPEG warning=0, rejects fatal=1.
   if(result!=0&&api.ErrorCode(decoder)!=0)throw Error(api,decoder);
   return(width,height,pixels);
  } finally {api.Destroy(decoder);}
 }
 static InvalidDataException Error(Api api,IntPtr decoder)=>new(Marshal.PtrToStringUTF8(api.Error(decoder))??"Invalid JPEG.");
 static readonly Lazy<Api> Backend=new(()=>new Api());
 sealed class Api {
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]internal delegate IntPtr InitDelegate();
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]internal delegate int HeaderDelegate(IntPtr handle,byte[] input,uint size,out int width,out int height,out int sampling,out int colorSpace);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]internal delegate int DecodeDelegate(IntPtr handle,byte[] input,uint size,[Out]byte[] output,int width,int pitch,int height,int format,int flags);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]internal delegate int DestroyDelegate(IntPtr handle);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]internal delegate IntPtr ErrorDelegate(IntPtr handle);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]internal delegate int ErrorCodeDelegate(IntPtr handle);
  internal readonly InitDelegate Init;internal readonly HeaderDelegate Header;internal readonly DecodeDelegate Decode;internal readonly DestroyDelegate Destroy;internal readonly ErrorDelegate Error;internal readonly ErrorCodeDelegate ErrorCode;
  internal Api(){
   string path=Path.Combine(Path.GetDirectoryName(typeof(LocalJpegDecoder).Assembly.Location)!,"runtimes","win-x64","native","turbojpeg.dll");
   IntPtr library=NativeLibrary.Load(path);
   try {Init=Load<InitDelegate>("tjInitDecompress");Header=Load<HeaderDelegate>("tjDecompressHeader3");Decode=Load<DecodeDelegate>("tjDecompress2");Destroy=Load<DestroyDelegate>("tjDestroy");Error=Load<ErrorDelegate>("tjGetErrorStr2");ErrorCode=Load<ErrorCodeDelegate>("tjGetErrorCode");}
   catch {NativeLibrary.Free(library);throw;}
   T Load<T>(string name)where T:Delegate=>Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(library,name));
   System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(typeof(LocalJpegDecoder).Assembly)?.Unloading+=_=>NativeLibrary.Free(library);
  }
 }
}
