using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace OpenCvSharp;

public sealed class WindowsLibraryLoader
{
	private class ProcessArchitectureInfo
	{
		public string Architecture { get; set; }

		private List<string> Warnings { get; set; }

		public bool HasWarnings => Warnings.Count > 0;

		public ProcessArchitectureInfo()
		{
			Warnings = new List<string>();
		}

		public void AddWarning(string format, params object[] args)
		{
			Warnings.Add(string.Format(format, args));
		}

		public string WarningText()
		{
			return string.Join("\r\n", Warnings.ToArray());
		}
	}

	private const string ProcessorArchitecture = "PROCESSOR_ARCHITECTURE";

	private const string DllFileExtension = ".dll";

	private const string DllDirectory = "dll";

	private readonly List<string> loadedAssemblies = new List<string>();

	private readonly Dictionary<string, string> processorArchitecturePlatforms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "x86", "x86" },
		{ "AMD64", "x64" },
		{ "IA64", "Itanium" },
		{ "ARM", "WinCE" }
	};

	private readonly Dictionary<string, int> processorArchitectureAddressWidthPlatforms = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
	{
		{ "x86", 4 },
		{ "AMD64", 8 },
		{ "IA64", 8 },
		{ "ARM", 4 }
	};

	private readonly object syncLock = new object();

	private const CharSet DefaultCharSet = CharSet.Auto;

	public static WindowsLibraryLoader Instance { get; } = new WindowsLibraryLoader();

	public List<string> AdditionalPaths { get; private set; }

	private WindowsLibraryLoader()
	{
		AdditionalPaths = new List<string>();
	}

	public bool IsLibraryLoaded(string dllName)
	{
		lock (syncLock)
		{
			return loadedAssemblies.Contains(dllName);
		}
	}

	public bool IsCurrentPlatformSupported()
	{
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			return Environment.OSVersion.Platform == PlatformID.Win32Windows;
		}
		return true;
	}

	public void LoadLibrary(string dllName, IEnumerable<string> additionalPaths = null)
	{
		if (!IsCurrentPlatformSupported())
		{
			return;
		}
		if (additionalPaths == null)
		{
			additionalPaths = new string[0];
		}
		try
		{
			lock (syncLock)
			{
				if (loadedAssemblies.Contains(dllName))
				{
					return;
				}
				ProcessArchitectureInfo processArchitecture = GetProcessArchitecture();
				foreach (string additionalPath in additionalPaths)
				{
					if (LoadLibraryRaw(dllName, additionalPath) != IntPtr.Zero)
					{
						return;
					}
				}
				string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				if (LoadLibraryInternal(dllName, directoryName, processArchitecture) != IntPtr.Zero)
				{
					return;
				}
				directoryName = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
				if (LoadLibraryInternal(dllName, directoryName, processArchitecture) != IntPtr.Zero)
				{
					return;
				}
				directoryName = Path.GetFullPath(Directory.GetCurrentDirectory());
				if (LoadLibraryInternal(dllName, directoryName, processArchitecture) != IntPtr.Zero)
				{
					return;
				}
				if (HttpContext.Current != null)
				{
					directoryName = Path.GetFullPath(HttpContext.Current.Server.MapPath("bin"));
					if (LoadLibraryInternal(dllName, directoryName, processArchitecture) != IntPtr.Zero)
					{
						return;
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Failed to find dll \"{0}\", for processor architecture {1}.", dllName, processArchitecture.Architecture);
				if (processArchitecture.HasWarnings)
				{
					stringBuilder.AppendFormat("\r\nWarnings: \r\n{0}", processArchitecture.WarningText());
				}
				throw new Exception(stringBuilder.ToString());
			}
		}
		catch (Exception)
		{
		}
	}

	private ProcessArchitectureInfo GetProcessArchitecture()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
		ProcessArchitectureInfo processArchitectureInfo = new ProcessArchitectureInfo();
		if (!string.IsNullOrEmpty(environmentVariable))
		{
			processArchitectureInfo.Architecture = environmentVariable;
		}
		else
		{
			processArchitectureInfo.AddWarning("Failed to detect processor architecture, falling back to x86.");
			processArchitectureInfo.Architecture = ((IntPtr.Size == 8) ? "x64" : "x86");
		}
		int num = processorArchitectureAddressWidthPlatforms[processArchitectureInfo.Architecture];
		if (num != IntPtr.Size)
		{
			if (string.Equals(processArchitectureInfo.Architecture, "AMD64", StringComparison.OrdinalIgnoreCase) && IntPtr.Size == 4)
			{
				processArchitectureInfo.Architecture = "x86";
				processArchitectureInfo.AddWarning("Expected the detected processing architecture of {0} to have an address width of {1} Bytes but was {2} Bytes, falling back to x86.", processArchitectureInfo.Architecture, num, IntPtr.Size);
			}
			else
			{
				processArchitectureInfo.AddWarning("Expected the detected processing architecture of {0} to have an address width of {1} Bytes but was {2} Bytes.", processArchitectureInfo.Architecture, num, IntPtr.Size);
			}
		}
		return processArchitectureInfo;
	}

	private IntPtr LoadLibraryInternal(string dllName, string baseDirectory, ProcessArchitectureInfo processArchInfo)
	{
		string platformName = GetPlatformName(processArchInfo.Architecture);
		string baseDirectory2 = Path.Combine(Path.Combine(baseDirectory, "dll"), platformName);
		return LoadLibraryRaw(dllName, baseDirectory2);
	}

	private IntPtr LoadLibraryRaw(string dllName, string baseDirectory)
	{
		IntPtr intPtr = IntPtr.Zero;
		string text = FixUpDllFileName(Path.Combine(baseDirectory, dllName));
		if (File.Exists(text))
		{
			try
			{
				intPtr = Win32LoadLibrary(text);
				if (intPtr != IntPtr.Zero)
				{
					loadedAssemblies.Add(dllName);
				}
			}
			catch (Exception)
			{
				Marshal.GetLastWin32Error();
			}
		}
		return intPtr;
	}

	private string FixUpDllFileName(string fileName)
	{
		if (!string.IsNullOrEmpty(fileName))
		{
			PlatformID platform = Environment.OSVersion.Platform;
			if ((platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE) && !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
			{
				return fileName + ".dll";
			}
		}
		return fileName;
	}

	private string GetPlatformName(string processorArchitecture)
	{
		if (string.IsNullOrEmpty(processorArchitecture))
		{
			return null;
		}
		if (processorArchitecturePlatforms.TryGetValue(processorArchitecture, out var value))
		{
			return value;
		}
		return null;
	}

	[DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = "LoadLibrary", SetLastError = true, ThrowOnUnmappableChar = true)]
	private static extern IntPtr Win32LoadLibrary(string dllPath);
}
