using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

internal static class Native
{
    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern int IMVS_PF_CreateHandle(out nint handle, string? serverPath);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_PF_DestroyHandle(nint handle);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern nint IMVS_PF_GetErrorMsg(uint code);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_PF_GetBinaryDataLength(nint handle, ref BinaryLengthInfo info);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_PF_GetBinaryData(nint handle, ref BinaryDataInfo info);

    [DllImport("iMVS-6000PlatformSDK.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int IMVS_PF_GetAllProcessList(nint handle, nint processList);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct BinaryLengthInfo
    {
        internal uint ModuleId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] internal byte[] Name;
        internal uint ReceiveWaitTime;
        internal uint DataLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] internal uint[] Reserved;

        internal static BinaryLengthInfo Create(uint moduleId, string name, uint wait)
        {
            var value = new BinaryLengthInfo
            {
                ModuleId = moduleId,
                Name = new byte[128],
                ReceiveWaitTime = wait,
                Reserved = new uint[4]
            };
            Encoding.ASCII.GetBytes(name + "\0").CopyTo(value.Name, 0);
            return value;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct BinaryDataInfo
    {
        internal uint ModuleId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] internal byte[] Name;
        internal uint ReceiveWaitTime;
        internal uint AllocatedSize;
        internal nint Data;
        internal uint DataLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] internal uint[] Reserved;

        internal static BinaryDataInfo Create(uint moduleId, string name, uint wait, uint size, nint data)
        {
            var value = new BinaryDataInfo
            {
                ModuleId = moduleId,
                Name = new byte[128],
                ReceiveWaitTime = wait,
                AllocatedSize = size,
                Data = data,
                Reserved = new uint[4]
            };
            Encoding.ASCII.GetBytes(name + "\0").CopyTo(value.Name, 0);
            return value;
        }
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
        var idArgs = args.Where(static arg => !arg.StartsWith("--output=", StringComparison.Ordinal)).ToArray();
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
        var create = Native.IMVS_PF_CreateHandle(out var handle, null);
        result["createCode"] = create;
        result["createError"] = Native.Error(create);
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
                var lengthInfo = Native.BinaryLengthInfo.Create(moduleId, name, 1000);
                var lengthCode = Native.IMVS_PF_GetBinaryDataLength(handle, ref lengthInfo);
                if (lengthCode != 0 || lengthInfo.DataLength == 0)
                {
                    observations.Add(new(moduleId, name, lengthCode, Native.Error(lengthCode), lengthInfo.DataLength, 0, "", 0, "", ""));
                    continue;
                }
                var capacity = checked((int)lengthInfo.DataLength);
                var memory = Marshal.AllocHGlobal(capacity);
                try
                {
                    var dataInfo = Native.BinaryDataInfo.Create(moduleId, name, 1000, lengthInfo.DataLength, memory);
                    var dataCode = Native.IMVS_PF_GetBinaryData(handle, ref dataInfo);
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
            result["destroyCode"] = Native.IMVS_PF_DestroyHandle(handle);
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
            var code = Native.IMVS_PF_GetAllProcessList(handle, pointer);
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
