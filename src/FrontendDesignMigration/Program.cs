using System.IO;
using System.Text;
using FrontendDesignMigration;
if(args.Length!=5){Console.Error.WriteLine("Usage: FrontendDesignMigration <v4.0-xml> <new-output> <DefaultDesign.xaml> <DefaultSubDesign.xaml> <explicit-encoding>");return 2;}
try {
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    var converter=new DesignConverter(Path.GetFullPath(args[2]),Path.GetFullPath(args[3]),Encoding.GetEncoding(args[4]));
    var result=converter.ConvertV40ToV44(File.ReadAllText(args[0]));
    // Owned output only; no overwrite or vendor construction. This differs deliberately
    // from the original overwrite behavior and protects the source/template evidence.
    using var stream=new FileStream(Path.GetFullPath(args[1]),FileMode.CreateNew,FileAccess.Write,FileShare.None);
    using var writer=new StreamWriter(stream,new UTF8Encoding(false));writer.Write(result);
    Console.WriteLine("Converted managed frontend XML; native solution conversion and runtime parity unverified.");return 0;
} catch(Exception e){Console.Error.WriteLine("Conversion failed: "+e.Message);return 1;}
