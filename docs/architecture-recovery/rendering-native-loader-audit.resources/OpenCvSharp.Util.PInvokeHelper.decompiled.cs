using System;

namespace OpenCvSharp.Util;

public static class PInvokeHelper
{
	public static void TryPInvoke()
	{
		try
		{
			NativeMethods.core_Mat_sizeof();
		}
		catch (DllNotFoundException ex)
		{
			DllImportError(ex);
		}
		catch (BadImageFormatException ex2)
		{
			DllImportError(ex2);
		}
	}

	public static void DllImportError(Exception ex)
	{
		throw CreateException(ex);
	}

	public static OpenCvSharpException CreateException(Exception ex)
	{
		return new OpenCvSharpException(ex.Message, ex);
	}
}
