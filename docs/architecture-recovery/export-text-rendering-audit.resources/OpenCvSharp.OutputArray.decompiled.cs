using System;
using System.Collections.Generic;

namespace OpenCvSharp;

public class OutputArray : DisposableCvObject
{
	private readonly object obj;

	internal OutputArray(Mat mat)
	{
		if (mat == null)
		{
			throw new ArgumentNullException("mat");
		}
		ptr = NativeMethods.core_OutputArray_new_byMat(mat.CvPtr);
		GC.KeepAlive(mat);
		obj = mat;
	}

	internal OutputArray(IEnumerable<Mat> mat)
	{
		if (mat == null)
		{
			throw new ArgumentNullException("mat");
		}
		using (VectorOfMat vectorOfMat = new VectorOfMat(mat))
		{
			ptr = NativeMethods.core_OutputArray_new_byVectorOfMat(vectorOfMat.CvPtr);
		}
		obj = mat;
	}

	protected override void DisposeUnmanaged()
	{
		NativeMethods.core_OutputArray_delete(ptr);
		base.DisposeUnmanaged();
	}

	public static implicit operator OutputArray(Mat mat)
	{
		return new OutputArray(mat);
	}

	public bool IsMat()
	{
		return obj is Mat;
	}

	public virtual Mat GetMat()
	{
		return obj as Mat;
	}

	public bool IsVectorOfMat()
	{
		return obj is IEnumerable<Mat>;
	}

	public virtual IEnumerable<Mat> GetVectorOfMat()
	{
		return obj as IEnumerable<Mat>;
	}

	public virtual void AssignResult()
	{
		if (!IsReady())
		{
			throw new NotSupportedException();
		}
		if (!IsMat())
		{
			throw new OpenCvSharpException("Not supported OutputArray-compatible type");
		}
	}

	public void Fix()
	{
		AssignResult();
		Dispose();
	}

	public bool IsReady()
	{
		if (ptr != IntPtr.Zero && !IsDisposed && obj != null)
		{
			return IsMat();
		}
		return false;
	}

	public void ThrowIfNotReady()
	{
		if (!IsReady())
		{
			throw new OpenCvSharpException("Invalid OutputArray");
		}
	}

	public static OutputArray Create(Mat mat)
	{
		return new OutputArray(mat);
	}

	public static OutputArrayOfStructList<T> Create<T>(List<T> list) where T : struct
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		return new OutputArrayOfStructList<T>(list);
	}

	public static OutputArrayOfMatList Create(List<Mat> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		return new OutputArrayOfMatList(list);
	}
}
