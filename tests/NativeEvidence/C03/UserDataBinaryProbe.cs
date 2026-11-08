using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

internal static class Native
{
    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern int IMVS_PF_CreateHandle(out nint handle, string? serverPath);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_CreateHandle(out nint handle, ref BasicInfo info);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_PF_DestroyHandle(nint handle);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_DestroyHandle(nint handle, uint takeoverType);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern nint IMVS_PF_GetErrorMsg(uint code);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_GetBinaryLength(nint handle, uint moduleId, ref OriginalBinaryLengthInfo info, uint receiveWaitTime);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_GetBinaryData(nint handle, uint moduleId, ref OriginalBinaryDataInfo info, uint receiveWaitTime);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_PF_GetAllProcessList(nint handle, nint processList);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_GetAllProcessList(nint handle, nint processList);

    [StructLayout(LayoutKind.Sequential)]
    internal struct BasicInfo
    {
        internal uint Ip;
        internal ushort Port;
        internal uint SdkIp;
        internal ushort SdkPort;
        internal uint NetAdapterIp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] internal byte[] UserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] internal byte[] Password;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] internal byte[] VendorName;
        internal uint MacLow;
        internal ushort MacHigh;
        internal uint ClientType;
        internal uint ServerType;
        internal uint HandleType;
        internal uint ServerRepIp;
        internal ushort ServerRepPort;
        internal uint ServerProcessId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] internal uint[] Reserved;

        internal static BasicInfo Local()
            => new() { Ip = 0x7F000001, Port = 5556, ClientType = 0, HandleType = 0,
                UserName = new byte[16], Password = new byte[32], VendorName = new byte[16], Reserved = new uint[2] };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OriginalBinaryLengthInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] internal byte[] Name;
        internal uint DataLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] internal uint[] Reserved;

        internal static OriginalBinaryLengthInfo Create(string name)
            => new() { Name = NameBytes(name), Reserved = new uint[4] };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OriginalBinaryDataInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] internal byte[] Name;
        internal uint AllocatedSize;
        internal nint Data;
        internal uint DataLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] internal uint[] Reserved;

        internal static OriginalBinaryDataInfo Create(string name, uint size, nint data)
            => new() { Name = NameBytes(name), AllocatedSize = size, Data = data, Reserved = new uint[4] };
    }

    private static byte[] NameBytes(string name)
    {
        var bytes = new byte[256];
        Encoding.UTF8.GetBytes(name).CopyTo(bytes, 0);
        return bytes;
    }

    internal static string Error(int code)
    {
        var pointer = IMVS_PF_GetErrorMsg(unchecked((uint)code));
        return pointer == 0 ? "" : Marshal.PtrToStringAnsi(pointer) ?? "";
    }
}

internal sealed record BinaryObservation(
    uint ModuleId,
    string Name,
    int LengthCode,
    string LengthError,
    uint Length,
    int DataCode,
    string DataError,
    uint ReturnedLength,
    string Sha256,
    string PreviewHex);

internal static class Program
{
    private static readonly string[] Names = ["DrawImageSize", "RoiType", "ExternRoiType", "DynamicInData", "DynamicOutData"];

    private static int Main(string[] args)
    {
        var outputPath = args.FirstOrDefault(static arg => arg.StartsWith("--output=", StringComparison.Ordinal))?[9..];
        var useBasicHandle = args.Any(static arg => arg.Equals("--basic", StringComparison.Ordinal));
        var idArgs = args.Where(static arg => !arg.StartsWith("--output=", StringComparison.Ordinal) && !arg.Equals("--basic", StringComparison.Ordinal)).ToArray();
        var moduleIds = idArgs.Length == 0
            ? Enumerable.Range(21000, 21).Select(Convert.ToUInt32).ToArray()
            : idArgs.Select(uint.Parse).Distinct().ToArray();
        var result = new Dictionary<string, object?>
        {
            ["scope"] = "read-only platform SDK UserData binary probe",
            ["moduleIds"] = moduleIds,
            ["names"] = Names
        };
        var observations = new List<BinaryObservation>();
        var basic = Native.BasicInfo.Local();
        var create = useBasicHandle
            ? Native.IMVS_CreateHandle(out var handle, ref basic)
            : Native.IMVS_PF_CreateHandle(out handle, null);
        result["createCode"] = create;
        result["createError"] = Native.Error(create);
        result["handleKind"] = useBasicHandle ? "IMVS_CreateHandle(ServerSetting=127.0.0.1:5556)" : "IMVS_PF_CreateHandle";
        if (create != 0 || handle == 0)
        {
            Emit(result, outputPath);
            return 2;
        }
        try
        {
            result["processes"] = ReadProcesses(handle);
            foreach (var moduleId in moduleIds)
            foreach (var name in Names)
            {
                var lengthInfo = Native.OriginalBinaryLengthInfo.Create(name);
                var lengthCode = Native.IMVS_GetBinaryLength(handle, moduleId, ref lengthInfo, 1000);
                if (lengthCode != 0 || lengthInfo.DataLength == 0)
                {
                    observations.Add(new(moduleId, name, lengthCode, Native.Error(lengthCode), lengthInfo.DataLength, 0, "", 0, "", ""));
                    continue;
                }
                var capacity = checked((int)lengthInfo.DataLength);
                var memory = Marshal.AllocHGlobal(capacity);
                try
                {
                    var dataInfo = Native.OriginalBinaryDataInfo.Create(name, lengthInfo.DataLength, memory);
                    var dataCode = Native.IMVS_GetBinaryData(handle, moduleId, ref dataInfo, 1000);
                    var bytes = dataCode == 0 && dataInfo.DataLength <= capacity
                        ? new byte[dataInfo.DataLength]
                        : Array.Empty<byte>();
                    if (bytes.Length != 0) Marshal.Copy(memory, bytes, 0, bytes.Length);
                    observations.Add(new(moduleId, name, lengthCode, Native.Error(lengthCode), lengthInfo.DataLength,
                        dataCode, Native.Error(dataCode), dataInfo.DataLength,
                        bytes.Length == 0 ? "" : Convert.ToHexString(SHA256.HashData(bytes)),
                        bytes.Length == 0 ? "" : Convert.ToHexString(bytes.AsSpan(0, Math.Min(64, bytes.Length)))));
                }
                finally { Marshal.FreeHGlobal(memory); }
            }
            result["observations"] = observations;
        }
        finally
        {
            result["destroyCode"] = useBasicHandle ? Native.IMVS_DestroyHandle(handle, 0) : Native.IMVS_PF_DestroyHandle(handle);
            result["destroyError"] = Native.Error((int)result["destroyCode"]!);
        }
        Emit(result, outputPath);
        return 0;
    }

    private static void Emit(Dictionary<string, object?> result, string? outputPath)
    {
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
            File.WriteAllText(outputPath, json + Environment.NewLine, new UTF8Encoding(false));
        }
        Console.WriteLine(json);
    }

    private static object ReadProcesses(nint handle)
    {
        const int maxProcesses = 1000;
        const int nameLength = 64;
        const int reservedWords = 4;
        const int entrySize = 4 + nameLength + reservedWords * 4;
        var pointer = Marshal.AllocHGlobal(4 + maxProcesses * entrySize + 16);
        try
        {
            Span<byte> zero = stackalloc byte[256];
            for (var offset = 0; offset < 4 + maxProcesses * entrySize + 16; offset += zero.Length)
                Marshal.Copy(zero[..Math.Min(zero.Length, 4 + maxProcesses * entrySize + 16 - offset)].ToArray(), 0, pointer + offset, Math.Min(zero.Length, 4 + maxProcesses * entrySize + 16 - offset));
            var code = Native.IMVS_GetAllProcessList(handle, pointer);
            var count = code == 0 ? Math.Min(Marshal.ReadInt32(pointer), maxProcesses) : 0;
            var processes = new List<object>();
            for (var i = 0; i < count; i++)
            {
                var entry = pointer + 4 + i * entrySize;
                var id = unchecked((uint)Marshal.ReadInt32(entry));
                var nameBytes = new byte[nameLength];
                Marshal.Copy(entry + 4, nameBytes, 0, nameBytes.Length);
                processes.Add(new { ModuleId = id, Name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0') });
            }
            return new { Code = code, Error = Native.Error(code), Count = count, Items = processes };
        }
        finally { Marshal.FreeHGlobal(pointer); }
    }
}
