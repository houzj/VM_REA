using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

// Original managed helpers only; no VM application, SDK, window or device initialization.
class Probe
{
    static int count;
    static Type helper;
    static void Check(bool value, string name) { if (!value) throw new Exception(name); count++; Console.WriteLine("PASS " + name); }
    static object Call(string name, Type[] types, params object[] args) { return helper.GetMethod(name, types).Invoke(null, args); }
    static string Get(string key) { return (string)Call("GetConfig", new[] { typeof(string) }, key); }
    static string Cfg() { return (string)Call("GetConfigValue", new[] { typeof(string) }, "IsClearLastExecuteResult"); }
    static string XmlValue(string path, string key) { return XDocument.Load(path).Root.Element(key).Element("CurValue").Value; }
    static int Main(string[] args)
    {
        try
        {
            helper = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VM.Utility.dll")).GetType("VM.Utility.ConfigurationHelper", true);
            Console.WriteLine("cwd=" + Environment.CurrentDirectory);
            Console.WriteLine("base=" + AppDomain.CurrentDomain.BaseDirectory);
            if (args[0] == "restart")
            {
                Check(Get("Flag") == "new", "restart-appsettings-value");
                Check(Cfg() == "2", "restart-cfg-value");
            }
            else if (args[0] == "duplicate")
            {
                Check(Cfg() == null, "duplicate-cfg-null");
                Call("SaveConfig", new[] { typeof(string), typeof(string) }, "Flag", "new");
                Check(XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Probe.exe.config")).Descendants("add").Where(x => (string)x.Attribute("key") == "Flag").All(x => (string)x.Attribute("value") == "new"), "duplicate-appsettings-all-updated");
            }
            else
            {
                Check(Get("Flag") == "old", "appsettings-initial");
                Check(Cfg() == "0", "cfg-initial");
                Call("SaveConfig", new[] { typeof(string), typeof(string) }, "Flag", "new");
                Check(Get("Flag") == "new", "appsettings-refresh-after-string-save");
                Check(Cfg() == "0" && XmlValue("Probe.cfg","IsClearLastExecuteResult") == "0", "string-save-does-not-write-cfg");
                Check(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Probe.exe_temp.config")), "string-save-backup");
                Call("SaveConfig", new[] { typeof(string), typeof(string) }, "Absent", "x");
                Check(Get("Absent") == null, "string-save-does-not-insert-missing-key");
                var doc = XDocument.Parse("<Config><IsClearLastExecuteResult><CurValue>1</CurValue></IsClearLastExecuteResult></Config>");
                Call("SaveConfig", new[] { typeof(XDocument) }, doc);
                Check(XmlValue("Probe.cfg","IsClearLastExecuteResult") == "1", "xdocument-save-cwd-cfg");
                Check(XmlValue("Probe_temp.cfg","IsClearLastExecuteResult") == "1", "cfg-backup-is-new-value");
                Check(Cfg() == "0", "xdocument-save-retains-loaded-cache");
                var xml = new XmlDocument(); xml.LoadXml("<Config><IsClearLastExecuteResult><CurValue>2</CurValue></IsClearLastExecuteResult></Config>");
                Call("SaveConfig", new[] { typeof(XmlDocument) }, xml);
                Check(XmlValue("Probe.cfg","IsClearLastExecuteResult") == "2", "xmldocument-save-cwd-cfg");
                Check(Cfg() == "0", "xmldocument-save-retains-loaded-cache");
                Check(Get("Flag") == "new", "cfg-save-does-not-write-appsettings");
                // A directory at the target causes FileStream creation to fail. The helper catches it.
                File.Move("Probe.cfg", "Probe.saved.cfg");
                Directory.CreateDirectory("Probe.cfg");
                Call("SaveConfig", new[] { typeof(XDocument) }, doc);
                Check(Directory.Exists("Probe.cfg"), "cfg-write-failure-swallowed");
                Directory.Delete("Probe.cfg"); File.Move("Probe.saved.cfg", "Probe.cfg");
            }
            foreach (var method in helper.GetMethods(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic).Where(m => m.DeclaringType == helper))
            {
                var body=method.GetMethodBody(); if(body==null) continue;
                using(var sha=SHA256.Create()) Console.WriteLine("cil="+method.MetadataToken.ToString("x8")+"|"+method+"|"+BitConverter.ToString(sha.ComputeHash(body.GetILAsByteArray())).Replace("-", ""));
            }
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) if(!assembly.IsDynamic) Console.WriteLine("loaded="+assembly.FullName+"|"+assembly.Location);
            Console.WriteLine("assertions="+count); return 0;
        }
        catch(Exception ex) { Console.WriteLine(ex); return 1; }
    }
}
