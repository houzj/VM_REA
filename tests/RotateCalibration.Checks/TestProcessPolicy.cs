using System.Runtime.InteropServices;
public static class TestProcessPolicy
{
 [DllImport("kernel32.dll")]static extern uint SetErrorMode(uint mode);
 public static void Configure(){SetErrorMode(0x0001|0x0002|0x8000);AppDomain.CurrentDomain.UnhandledException+=(_,e)=>Console.Error.WriteLine("UNHANDLED TEST PROCESS: "+e.ExceptionObject);}
}
