namespace OpenCvSharp;

public class InputOutputArray : OutputArray
{
	internal InputOutputArray(Mat mat)
		: base(mat)
	{
	}

	public static implicit operator InputOutputArray(Mat mat)
	{
		return new InputOutputArray(mat);
	}
}
