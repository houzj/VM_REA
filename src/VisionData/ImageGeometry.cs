namespace VisionCore;

// Independent baseline; no claim of equivalence to any vendor algorithm.
public sealed class GrayImage:IImageData
{
    private readonly byte[] pixels;
    public int Width { get; }
    public int Height { get; }
    public int Stride=>Width;
    public ImagePixelFormat Format=>ImagePixelFormat.Mono8;
    public ReadOnlySpan<byte> Pixels => pixels;
    public byte this[int x, int y] => pixels[Offset(x, y)];
    private int Offset(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) throw new ArgumentOutOfRangeException();
        return y * Width + x;
    }
    public GrayImage(int width, int height, ReadOnlySpan<byte> data)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (data.Length != checked(width * height)) throw new ArgumentException("Pixel count must equal width * height.", nameof(data));
        Width = width; Height = height; pixels = data.ToArray();
    }
}

public readonly record struct Component(int Area, int MinX, int MinY, int MaxX, int MaxY, double CenterX, double CenterY);
public readonly record struct Point2(double X, double Y);
public readonly record struct CircleFit(double CenterX, double CenterY, double Radius, double RadialRmsError);


