using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

// Runs only original managed leaf helpers; never initializes VM/SDK or devices.
class Probe
{
    static void Check(bool value, string name) { if (!value) throw new Exception(name); Console.WriteLine("PASS " + name); }
    static int Main(string[] args)
    {
        try
        {
            var a = Assembly.LoadFrom(Path.GetFullPath("VM.Utility.dll"));
            var t = a.GetType("VM.Utility.ConfigurationHelper", true);
            var get = t.GetMethod("GetConfigValue", new[] { typeof(string) });
            string actual = (string)get.Invoke(null, new object[] { "IsClearLastExecuteResult" });
            Console.WriteLine("value=" + (actual ?? "<null>"));
            Check(actual == (args[0] == "null" ? null : args[0]), "original-config-value");
            // The caller's exact comparison, not an invocation of Program or VmSolution.
            Check((actual == "1") == (args[0] == "1"), "startup-comparison");
            File.WriteAllText("Probe.cfg", "<Config><IsClearLastExecuteResult><CurValue>changed</CurValue></IsClearLastExecuteResult></Config>");
            Check((string)get.Invoke(null, new object[] { "IsClearLastExecuteResult" }) == actual, "loaded-document-cache");
            var util = Assembly.LoadFrom(Path.GetFullPath("VM.Util.dll"));
            var wait = util.GetType("Hik.Util.SyncWaitor", true).GetMethod("PrecisionWait", new[] { typeof(bool[]), typeof(uint), typeof(uint), typeof(bool) });
            // UI-pump branch operates solely on this probe's current thread queue.
            var flags = new[] { true };
            wait.Invoke(null, new object[] { flags, 0u, 0u, true });
            Check(flags[0], "already-complete-return");
            flags[0] = false;
            var clock = Stopwatch.StartNew();
            wait.Invoke(null, new object[] { flags, 0u, 5u, true });
            Check(!flags[0], "timeout-returns-without-success-flag");
            Console.WriteLine("timeout-elapsed-ms=" + clock.ElapsedMilliseconds);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) if (!asm.IsDynamic) Console.WriteLine("loaded=" + asm.FullName + "|" + asm.Location);
            Console.WriteLine("assertions=5");
            return 0;
        }
        catch (Exception e) { Console.WriteLine(e); return 1; }
    }
}
