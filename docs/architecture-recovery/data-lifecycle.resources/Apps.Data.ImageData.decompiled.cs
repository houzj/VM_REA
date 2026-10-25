using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apps.Log;
using Hik.Xml;
using VMControls.RenderInterface;
using VisionDesigner;

namespace Apps.Data;

public class ImageData : BaseNode, IImageData, IImageDataExtern, IRangeImageData, IPorfileImageData
{
	public struct Point3F
	{
		public float X;

		public float Y;

		public float Z;
	}

	public static ColorPalette tempPalette;

	public byte[] ImageBuffer;

	public string MemoryAddress;

	public int Width;

	public int Height;

	public int Size;

	public System.Windows.Media.PixelFormat PixelFormat = PixelFormats.Default;

	public uint DetailedPixelFormat = 0u;

	public bool NeedConvertColor = false;

	public string PixelInfoLabel;

	private Dictionary<string, string> pixelExtraInfo = new Dictionary<string, string>();

	public const uint IMVS_PIX_MONO = 16777216u;

	public const uint IMVS_PIX_RGB = 33554432u;

	public const uint IMVS_PIX_COLOR = 33554432u;

	public const uint IMVS_PIX_CUSTOM = 2147483648u;

	public const uint IMVS_PIX_COLOR_MASK = 4278190080u;

	public const uint IMVS_PIX_OCCUPY1BIT = 65536u;

	public const uint IMVS_PIX_OCCUPY2BIT = 131072u;

	public const uint IMVS_PIX_OCCUPY4BIT = 262144u;

	public const uint IMVS_PIX_OCCUPY8BIT = 524288u;

	public const uint IMVS_PIX_OCCUPY12BIT = 786432u;

	public const uint IMVS_PIX_OCCUPY16BIT = 1048576u;

	public const uint IMVS_PIX_OCCUPY24BIT = 1572864u;

	public const uint IMVS_PIX_OCCUPY32BIT = 2097152u;

	public const uint IMVS_PIX_OCCUPY36BIT = 2359296u;

	public const uint IMVS_PIX_OCCUPY48BIT = 3145728u;

	public const uint IMVS_PIX_EFFECTIVE_PIXEL_SIZE_MASK = 16711680u;

	public const uint IMVS_PIX_EFFECTIVE_PIXEL_SIZE_SHIFT = 16u;

	public const uint IMVS_PIX_ID_MASK = 65535u;

	public const uint IMVS_PIX_COUNT = 70u;

	public const uint IMVS_PIX_UNKNOW = 0u;

	public const uint IMVS_PIX_MONO1P = 16842807u;

	public const uint IMVS_PIX_MONO2P = 16908344u;

	public const uint IMVS_PIX_MONO4P = 17039417u;

	public const uint IMVS_PIX_MONO8 = 17301505u;

	public const uint IMVS_PIX_MONO8_SIGNED = 17301506u;

	public const uint IMVS_PIX_MONO10 = 17825795u;

	public const uint IMVS_PIX_MONO10_PACKED = 17563652u;

	public const uint IMVS_PIX_MONO12 = 17825797u;

	public const uint IMVS_PIX_MONO12_PACKED = 17563654u;

	public const uint IMVS_PIX_MONO14 = 17825829u;

	public const uint IMVS_PIX_MONO16 = 17825799u;

	public const uint IMVS_PIX_BAYGR8 = 17301512u;

	public const uint IMVS_PIX_BAYRG8 = 17301513u;

	public const uint IMVS_PIX_BAYGB8 = 17301514u;

	public const uint IMVS_PIX_BAYBG8 = 17301515u;

	public const uint IMVS_PIX_BAYGR10 = 17825804u;

	public const uint IMVS_PIX_BAYRG10 = 17825805u;

	public const uint IMVS_PIX_BAYGB10 = 17825806u;

	public const uint IMVS_PIX_BAYBG10 = 17825807u;

	public const uint IMVS_PIX_BAYGR12 = 17825808u;

	public const uint IMVS_PIX_BAYRG12 = 17825809u;

	public const uint IMVS_PIX_BAYGB12 = 17825810u;

	public const uint IMVS_PIX_BAYBG12 = 17825811u;

	public const uint IMVS_PIX_BAYGR10_PACKED = 17563686u;

	public const uint IMVS_PIX_BAYRG10_PACKED = 17563687u;

	public const uint IMVS_PIX_BAYGB10_PACKED = 17563688u;

	public const uint IMVS_PIX_BAYBG10_PACKED = 17563689u;

	public const uint IMVS_PIX_BAYGR12_PACKED = 17563690u;

	public const uint IMVS_PIX_BAYRG12_PACKED = 17563691u;

	public const uint IMVS_PIX_BAYGB12_PACKED = 17563692u;

	public const uint IMVS_PIX_BAYBG12_PACKED = 17563693u;

	public const uint IMVS_PIX_BAYGR16 = 17825838u;

	public const uint IMVS_PIX_BAYRG16 = 17825839u;

	public const uint IMVS_PIX_BAYGB16 = 17825840u;

	public const uint IMVS_PIX_BAYBG16 = 17825841u;

	public const uint IMVS_PIX_RGB8_PACKED = 35127316u;

	public const uint IMVS_PIX_BGR8_PACKED = 35127317u;

	public const uint IMVS_PIX_RGBA8_PACKED = 35651606u;

	public const uint IMVS_PIX_BGRA8_PACKED = 35651607u;

	public const uint IMVS_PIX_RGB10_PACKED = 36700184u;

	public const uint IMVS_PIX_BGR10_PACKED = 36700185u;

	public const uint IMVS_PIX_RGB12_PACKED = 36700186u;

	public const uint IMVS_PIX_BGR12_PACKED = 36700187u;

	public const uint IMVS_PIX_RGB16_PACKED = 36700211u;

	public const uint IMVS_PIX_RGB10V1_PACKED = 35651612u;

	public const uint IMVS_PIX_RGB10V2_PACKED = 35651613u;

	public const uint IMVS_PIX_RGB12V1_PACKED = 35913780u;

	public const uint IMVS_PIX_RGB565_PACKED = 34603061u;

	public const uint IMVS_PIX_BGR565_PACKED = 34603062u;

	public const uint IMVS_PIX_YUV411_PACKED = 34340894u;

	public const uint IMVS_PIX_YUV422_PACKED = 34603039u;

	public const uint IMVS_PIX_YUV422_YUYV_PACKED = 34603058u;

	public const uint IMVS_PIX_YUV444_PACKED = 35127328u;

	public const uint IMVS_PIX_YCBCR8_CBYCR = 35127354u;

	public const uint IMVS_PIX_YCBCR422_8 = 34603067u;

	public const uint IMVS_PIX_YCBCR422_8_CBYCRY = 34603075u;

	public const uint IMVS_PIX_YCBCR411_8_CBYYCRYY = 34340924u;

	public const uint IMVS_PIX_YCBCR601_8_CBYCR = 35127357u;

	public const uint IMVS_PIX_YCBCR601_422_8 = 34603070u;

	public const uint IMVS_PIX_YCBCR601_422_8_CBYCRY = 34603076u;

	public const uint IMVS_PIX_YCBCR601_411_8_CBYYCRYY = 34340927u;

	public const uint IMVS_PIX_YCBCR709_8_CBYCR = 35127360u;

	public const uint IMVS_PIX_YCBCR709_422_8 = 34603073u;

	public const uint IMVS_PIX_YCBCR709_422_8_CBYCRY = 34603077u;

	public const uint IMVS_PIX_YCBCR709_411_8_CBYYCRYY = 34340930u;

	public const uint IMVS_PIX_RGB8_PLANAR = 35127329u;

	public const uint IMVS_PIX_RGB10_PLANAR = 36700194u;

	public const uint IMVS_PIX_RGB12_PLANAR = 36700195u;

	public const uint IMVS_PIX_RGB16_PLANAR = 36700196u;

	public const uint IMVS_PIX_Jpeg = 2149056513u;

	public const uint IMVS_PIX_Bmp = 2149580802u;

	public const uint IMVS_PIX_OCCUPY64BIT = 4194304u;

	public const uint IMVS_PIX_OCCUPY96BIT = 6291456u;

	public const uint IMVS_PIX_Coord3D_ABC32 = 2187341825u;

	public const uint IMVS_PIX_Coord3D_AB32f = 2185244674u;

	public const uint IMVS_PIX_Coord3D_AB32 = 2185244675u;

	public const uint IMVS_PIX_Coord3D_AC32f_64 = 37748930u;

	public const uint IMVS_PIX_Coord3D_AC32f_Planar = 37748931u;

	public const uint IMVS_PIX_Coord3D_AC32 = 2185244676u;

	public const uint IMVS_PIX_Coord3D_A32f = 18874557u;

	public const uint IMVS_PIX_Coord3D_A32 = 2166370309u;

	public const uint IMVS_PIX_Coord3D_C32f = 18874559u;

	public const uint IMVS_PIX_Coord3D_C32 = 2166370310u;

	public const uint IMVS_PIX_Coord3D_ABC16 = 36700345u;

	public const uint IMVS_PIX_Coord3D_C16 = 17825976u;

	private const uint RANGE_IMAGE_WIDTH_CHUNK_ID = 2779069697u;

	private const uint RANGE_IMAGE_HEIGHT_CHUNK_ID = 2779069698u;

	private const uint RANGE_IMAGE_INTENSITY_CHUNK_ID = 2779069699u;

	private const uint RANGE_IMAGE_THICKNESS_CHUNK_ID = 2779069700u;

	private const uint RANGE_IMAGE_RUN_DIR_CHUNK_ID = 2779069701u;

	private const uint RANGE_IMAGE_SCALE_CHUNK_ID = 2779069703u;

	private const uint RANGE_IMAGE_OFFSET_CHUNK_ID = 2779069704u;

	private const uint RANGE_IMAGE_DATA_CHUNK_ID = 1635588315u;

	private const uint RANGE_IMAGE_RGB_CHUNK_ID = 2779069707u;

	private const uint RANGE_IMAGE_RGBD_DEMAING_CHUNK_ID = 2779069713u;

	private const uint RANGE_IMAGE_RGBD_MASK_CHUNK_ID = 2779069715u;

	private const uint RANGE_IMAGE_RGBD_INSTANCESEGINFO_CHUNK_ID = 2779069718u;

	private const uint RANGE_IMAGE_RGBD_INSTANCESEGINFO_EX_CHUNK_ID = 2779069709u;

	private const uint RANGE_IMAGE_TYPE_CHUNK_ID = 1u;

	private const uint RANGE_IMAGE_RGBD_PARAM_CHUNK_ID = 2u;

	public byte[] RangeImageBuffer = null;

	public float[] PloundCloundBuffer = null;

	public IList<Point3F> ProfilePoints;

	public Dictionary<string, string> PixelExtraInfo
	{
		get
		{
			return pixelExtraInfo;
		}
		set
		{
			pixelExtraInfo = value;
		}
	}

	public string PixelFormatString => Enum.GetName(typeof(MPixelFormat), GetDetailedPixelFormatByProtocol(DetailedPixelFormat, PixelFormat));

	int IImageData.Width => Width;

	int IImageData.Height => Height;

	public byte[] Buffer
	{
		get
		{
			return ImageBuffer;
		}
		set
		{
			ImageBuffer = value;
		}
	}

	string IImageData.PixelFormat => PixelFormatString;

	string IImageData.MemoryAddress => MemoryAddress;

	bool IImageDataExtern.NeedConvertColor => NeedConvertColor;

	string IImageDataExtern.PixelInfoLabel => PixelInfoLabel;

	public bool NeedCheckExecuteCount { get; set; } = false;

	public uint ExecuteCount { get; set; } = 0u;

	byte[] IRangeImageData.RangeBuffer
	{
		get
		{
			return RangeImageBuffer;
		}
		set
		{
			RangeImageBuffer = value;
		}
	}

	uint IRangeImageData.DetailedPixelFormat
	{
		get
		{
			return DetailedPixelFormat;
		}
		set
		{
			DetailedPixelFormat = value;
		}
	}

	public int Xoffset { get; set; }

	public int Yoffset { get; set; }

	public int Zoffset { get; set; }

	public int Xscale { get; set; }

	public int Yscale { get; set; }

	public int Zscale { get; set; }

	public int OriImageWidth { get; set; }

	public int OriImageHeight { get; set; }

	static ImageData()
	{
		using (Bitmap bitmap = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
		{
			tempPalette = bitmap.Palette;
		}
		for (int i = 0; i < 256; i++)
		{
			tempPalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
		}
	}

	public ImageData()
	{
	}

	public ImageData(int width, int height, byte[] buffer, string pixformat)
	{
		Width = width;
		Height = height;
		ImageBuffer = buffer;
		switch (pixformat.ToLower())
		{
		case "gray8":
			PixelFormat = PixelFormats.Gray8;
			DetailedPixelFormat = 17301505u;
			break;
		case "rgb24":
			PixelFormat = PixelFormats.Rgb24;
			DetailedPixelFormat = 35127316u;
			break;
		case "rgb24_p3":
			PixelFormat = PixelFormats.Rgb24;
			DetailedPixelFormat = 35127329u;
			break;
		default:
			PixelFormat = PixelFormats.Gray8;
			break;
		}
	}

	void IImageDataExtern.SetPixelFormat(string format)
	{
		switch (format)
		{
		case "Gray8":
			PixelFormat = PixelFormats.Gray8;
			DetailedPixelFormat = 17301505u;
			break;
		case "Rgb24":
			PixelFormat = PixelFormats.Rgb24;
			DetailedPixelFormat = 35127316u;
			break;
		case "Rgb24_P3":
			PixelFormat = PixelFormats.Rgb24;
			DetailedPixelFormat = 35127329u;
			break;
		default:
			throw new Exception($"{format} Cant't Accepted");
		}
	}

	public static MPixelFormat GetPixelFormatCode(System.Windows.Media.PixelFormat pixelFormat)
	{
		if (PixelFormats.Gray8 == pixelFormat)
		{
			return MPixelFormat.Gray8;
		}
		if (PixelFormats.Rgb24 == pixelFormat)
		{
			return MPixelFormat.Rgb24;
		}
		return MPixelFormat.Unknow;
	}

	public static MPixelFormat GetDetailedPixelFormatByProtocol(uint pixelFormatCode, System.Windows.Media.PixelFormat format = default(System.Windows.Media.PixelFormat))
	{
		return pixelFormatCode switch
		{
			0u => (!(format == PixelFormats.Rgb24)) ? MPixelFormat.Gray8 : MPixelFormat.Rgb24, 
			17301505u => MPixelFormat.Gray8, 
			35127316u => MPixelFormat.Rgb24, 
			35127329u => MPixelFormat.Rgb24_P3, 
			_ => MPixelFormat.Gray8, 
		};
	}

	public static System.Windows.Media.PixelFormat GetPixelFormatByProtocol(uint pixelFormatCode)
	{
		switch (pixelFormatCode)
		{
		case 17301505u:
			return PixelFormats.Gray8;
		case 35127316u:
		case 35127329u:
			return PixelFormats.Rgb24;
		default:
			return PixelFormats.Gray8;
		}
	}

	public static uint GetPixelFormatProtocolCode(System.Windows.Media.PixelFormat pixelFormat)
	{
		if (pixelFormat == PixelFormats.Rgb24)
		{
			return 35127316u;
		}
		return 17301505u;
	}

	private static bool ThumbnailCallback()
	{
		return false;
	}

	public static BitmapImage CreateThumbnailByGrayBitmap(Bitmap bitmap, double panelWidth, double panelHeight)
	{
		Image image = null;
		Image image2 = null;
		try
		{
			Image.GetThumbnailImageAbort callback = ThumbnailCallback;
			image2 = bitmap;
			if (image2 == null)
			{
				return null;
			}
			double val = panelWidth / (double)image2.Width;
			double val2 = panelHeight / (double)image2.Height;
			float num = (float)Math.Min(val, val2);
			int thumbWidth = image2.Width;
			int thumbHeight = image2.Height;
			if (num < 1f)
			{
				thumbWidth = (int)Math.Round((float)image2.Width * num);
				thumbHeight = (int)Math.Round((float)image2.Height * num);
			}
			MemoryStream memoryStream = new MemoryStream();
			image = image2.GetThumbnailImage(thumbWidth, thumbHeight, callback, IntPtr.Zero);
			using (Bitmap bitmap2 = new Bitmap(image.Width, image.Height))
			{
				bitmap2.SetResolution(image.HorizontalResolution, image.VerticalResolution);
				using (Graphics graphics = Graphics.FromImage(bitmap2))
				{
					graphics.Clear(System.Drawing.Color.White);
					graphics.DrawImageUnscaled(image, 0, 0);
				}
				bitmap2.Save(memoryStream, ImageFormat.Bmp);
			}
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.DecodePixelHeight = (int)panelHeight;
			bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}
		catch
		{
			return null;
		}
		finally
		{
			image2?.Dispose();
			image?.Dispose();
		}
	}

	public static BitmapImage CreateThumbnail(string fileName, double panelWidth = 0.0, double panelHeight = 0.0)
	{
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			using (Stream streamSource = new MemoryStream(File.ReadAllBytes(fileName)))
			{
				bitmapImage.StreamSource = streamSource;
				bitmapImage.DecodePixelHeight = (int)panelHeight;
				bitmapImage.DecodePixelWidth = (int)panelWidth;
				bitmapImage.EndInit();
			}
			return bitmapImage;
		}
		catch
		{
			return null;
		}
	}

	public static ImageData GetImageDataFromFile(string strPath, int width = 0, int height = 0, bool needGC = false)
	{
		LogHelper.Info($"GetImageDataFromFile start:{strPath}");
		ImageData imageData = null;
		CMvdImage cMvdImage = new CMvdImage();
		string text = "Load Info:";
		try
		{
			cMvdImage.InitImage(strPath);
			imageData = new ImageData();
			imageData.Width = (int)cMvdImage.Width;
			imageData.Height = (int)cMvdImage.Height;
			text += $"InitImage successfully,PixelFormat={cMvdImage.PixelFormat.ToString()}.";
			MVD_PIXEL_FORMAT pixelFormat = cMvdImage.PixelFormat;
			if ((uint)(pixelFormat - 200) <= 7u)
			{
				imageData.PixelFormat = PixelFormats.Rgb24;
				imageData.DetailedPixelFormat = 35127316u;
				cMvdImage.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3);
			}
			else
			{
				imageData.PixelFormat = PixelFormats.Gray8;
				imageData.DetailedPixelFormat = 17301505u;
				cMvdImage.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
			}
			text += "ConvertImagePixelFormat successfully.";
			imageData.Buffer = cMvdImage.GetImageData(0).arrDataBytes;
			text += "GetImageData successfully.";
			if (width > 0 && height > 0)
			{
				double num = (double)imageData.Width / (double)imageData.Height;
				double num2 = (double)width / (double)height;
				if (num > num2)
				{
					height = (int)((double)width / num);
				}
				else
				{
					width = (int)(num * (double)height);
				}
				imageData = ResizeImageData(imageData, width, height);
				text += "ResizeImageData successfully.";
			}
			return imageData;
		}
		catch (MvdException ex)
		{
			LogHelper.Error(string.Concat(ex.ErrorCode, ex.Message, ex.StackTrace));
			return null;
		}
		catch (Exception ex2)
		{
			LogHelper.Error(ex2.Message + ex2.StackTrace);
			return null;
		}
		finally
		{
			LogHelper.Info(text);
			LogHelper.Info("GetImageDataFromFile end");
			cMvdImage?.Dispose();
			if (needGC)
			{
				GC.Collect();
			}
		}
	}

	public static ImageData GetMono8ImageDataFromFile(string strPath, int width = 0, int height = 0, bool needGC = false)
	{
		ImageData imageData = null;
		CMvdImage cMvdImage = new CMvdImage();
		try
		{
			cMvdImage.InitImage(strPath);
			imageData = new ImageData();
			imageData.Width = (int)cMvdImage.Width;
			imageData.Height = (int)cMvdImage.Height;
			imageData.PixelFormat = PixelFormats.Gray8;
			cMvdImage.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
			imageData.Buffer = cMvdImage.GetImageData(0).arrDataBytes;
			if (width > 0 && height > 0)
			{
				double num = (double)imageData.Width / (double)imageData.Height;
				double num2 = (double)width / (double)height;
				if (num > num2)
				{
					height = (int)((double)width / num);
				}
				else
				{
					width = (int)(num * (double)height);
				}
				imageData = ResizeImageData(imageData, width, height);
			}
			return imageData;
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			cMvdImage?.Dispose();
			if (needGC)
			{
				GC.Collect();
			}
		}
	}

	private unsafe static ImageData ResizeImageData(ImageData srcImage, int width, int height)
	{
		if (srcImage == null || srcImage.Buffer == null || width <= 0 || height <= 0)
		{
			return null;
		}
		int channel = srcImage.PixelFormat.BitsPerPixel / 8;
		if (srcImage.Buffer.Length != srcImage.Width * srcImage.Height * channel)
		{
			return null;
		}
		ImageData imageData = new ImageData();
		imageData.Width = width;
		imageData.Height = height;
		imageData.PixelFormat = srcImage.PixelFormat;
		imageData.DetailedPixelFormat = srcImage.DetailedPixelFormat;
		imageData.Buffer = new byte[width * height * channel];
		fixed (byte* buffer = srcImage.Buffer)
		{
			fixed (byte* buffer2 = imageData.Buffer)
			{
				byte* srcPtr = buffer;
				byte* dstPtr = buffer2;
				int strideSource = srcImage.Width * channel;
				int stride = width * channel;
				double xScale = (double)srcImage.Width / (double)width;
				double yScale = (double)srcImage.Height / (double)height;
				int dlen = width * channel;
				Parallel.For(0, height, (int y) =>
				{
					byte* ptr = srcPtr + (int)(yScale * (double)y) * strideSource;
					byte* ptr2 = dstPtr + y * stride;
					double num = 0.0;
					byte* ptr3 = dlen + ptr2;
					for (byte* ptr4 = ptr2; ptr4 < ptr3; ptr4 += channel)
					{
						byte* ptr5 = ptr + (int)num * channel;
						num += xScale;
						for (int i = 0; i < channel; i++)
						{
							ptr4[i] = ptr5[i];
						}
					}
				});
			}
		}
		return imageData;
	}

	private static void GetImageSize(string fileName, out int imageWidth, out int imageHeight)
	{
		using Stream bitmapStream = new MemoryStream(File.ReadAllBytes(fileName));
		BitmapFrame bitmapFrame = BitmapFrame.Create(bitmapStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
		imageWidth = bitmapFrame.PixelWidth;
		imageHeight = bitmapFrame.PixelHeight;
	}

	public static BitmapImage CreateRoiThumbnail(string fileName, double panelWidth, double panelHeight, Int32Rect rect)
	{
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.SourceRect = rect;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			using (Stream streamSource = new MemoryStream(File.ReadAllBytes(fileName)))
			{
				bitmapImage.StreamSource = streamSource;
				bitmapImage.DecodePixelHeight = (int)panelHeight;
				bitmapImage.DecodePixelWidth = (int)panelWidth;
				bitmapImage.EndInit();
			}
			return bitmapImage;
		}
		catch
		{
			return null;
		}
	}

	public void SaveGrayImage(string saveName)
	{
		if (ImageBuffer == null || ImageBuffer.Length == 0)
		{
			throw new Exception("保存图像：无图像数据");
		}
		if (Width * Height != ImageBuffer.Length)
		{
			throw new Exception("保存图像：图像数据长度错误");
		}
		Bitmap bitmap = null;
		try
		{
			bitmap = GetGrayBitmapFromGrayData(ImageBuffer, Width, Height);
			bitmap?.Save(saveName);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			bitmap?.Dispose();
		}
	}

	public static Bitmap GetGrayBitmapFromGrayData(byte[] rawValues, int width, int height)
	{
		Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
		int stride = bitmapData.Stride;
		IntPtr scan = bitmapData.Scan0;
		int num = 0;
		for (int i = 0; i < height; i++)
		{
			Marshal.Copy(rawValues, num, scan, width);
			num += width;
			scan += stride;
		}
		bitmap.UnlockBits(bitmapData);
		bitmap.Palette = tempPalette;
		return bitmap;
	}

	public static Bitmap ToGrayBitmap(byte[] rawValues, int width, int height)
	{
		Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
		int stride = bitmapData.Stride;
		int num = stride - width;
		IntPtr scan = bitmapData.Scan0;
		int num2 = stride * height;
		int num3 = 0;
		int num4 = 0;
		byte[] array = new byte[num2];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				array[num3++] = rawValues[num4++];
			}
			num3 += num;
		}
		Marshal.Copy(array, 0, scan, num2);
		bitmap.UnlockBits(bitmapData);
		ColorPalette palette;
		using (Bitmap bitmap2 = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
		{
			palette = bitmap2.Palette;
		}
		for (int k = 0; k < 256; k++)
		{
			palette.Entries[k] = System.Drawing.Color.FromArgb(k, k, k);
		}
		bitmap.Palette = palette;
		return bitmap;
	}

	public IntPtr GetImagePointer()
	{
		byte[] imageBuffer = ImageBuffer;
		int num = imageBuffer.Length;
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.Copy(imageBuffer, 0, intPtr, num);
			return intPtr;
		}
		finally
		{
		}
	}

	public static void CopyEntityImage(string oldFileName, string newFileName, double panelWidth, double panelHeight)
	{
		Image image = null;
		Image image2 = null;
		try
		{
			Image.GetThumbnailImageAbort callback = ThumbnailCallback;
			image2 = Image.FromStream(new MemoryStream(File.ReadAllBytes(oldFileName)));
			if (image2 == null)
			{
				return;
			}
			double val = panelWidth / (double)image2.Width;
			double val2 = panelHeight / (double)image2.Height;
			float num = (float)Math.Max(val, val2);
			int thumbWidth = image2.Width;
			int thumbHeight = image2.Height;
			if (num < 1f)
			{
				thumbWidth = (int)Math.Round((float)image2.Width * num);
				thumbHeight = (int)Math.Round((float)image2.Height * num);
			}
			MemoryStream memoryStream = new MemoryStream();
			image = image2.GetThumbnailImage(thumbWidth, thumbHeight, callback, IntPtr.Zero);
			using Bitmap bitmap = new Bitmap(image.Width, image.Height);
			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.Clear(System.Drawing.Color.White);
				graphics.DrawImageUnscaled(image, 0, 0);
			}
			bitmap.Save(newFileName);
		}
		catch
		{
		}
		finally
		{
			image2?.Dispose();
			image?.Dispose();
		}
	}

	public static ImageData GetImageDataFrom3DFile(string strPath, int width = 0, int height = 0)
	{
		ImageData imageData = null;
		byte[] imageBuffer = null;
		FileStream fileStream = null;
		BinaryReader binaryReader = null;
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		try
		{
			fileStream = File.OpenRead(strPath);
			binaryReader = new BinaryReader(fileStream);
			uint num2 = (uint)fileStream.Length;
			uint num3 = 0u;
			uint num4 = 0u;
			uint num5 = 0u;
			uint num6 = 0u;
			uint num7 = num2;
			uint detailedPixelFormat = 0u;
			while (num7 != 0)
			{
				num7 -= 4;
				if (num7 == 0)
				{
					break;
				}
				fileStream.Seek(num7, SeekOrigin.Begin);
				num5 = (uint)binaryReader.ReadInt32();
				num7 -= 4;
				if (num7 == 0)
				{
					break;
				}
				fileStream.Seek(num7, SeekOrigin.Begin);
				num6 = (uint)binaryReader.ReadInt32();
				num7 -= num5;
				if (num7 < 0)
				{
					break;
				}
				fileStream.Seek(num7, SeekOrigin.Begin);
				if (1 == num6)
				{
					num = binaryReader.ReadInt32();
				}
				if (-1515897599 == (int)num6)
				{
					if (num3 == 0)
					{
						num3 = (uint)binaryReader.ReadInt32();
					}
				}
				else if (-1515897598 == (int)num6)
				{
					if (num4 == 0)
					{
						num4 = (uint)binaryReader.ReadInt32();
					}
				}
				else if (-1515897597 == (int)num6)
				{
					imageBuffer = new byte[num5];
					imageBuffer = binaryReader.ReadBytes((int)num5);
				}
				else if (1635588315 == num6)
				{
					if (num3 * num4 * 6 == num5)
					{
						imageBuffer = new byte[num5];
						imageBuffer = binaryReader.ReadBytes((int)num5);
						flag = true;
					}
				}
				else if (-1515897589 == (int)num6 && num == 1)
				{
					uint num8 = (uint)binaryReader.ReadInt32();
					if (num8 != 0)
					{
						num3 = (uint)binaryReader.ReadInt32();
						num4 = (uint)binaryReader.ReadInt32();
						detailedPixelFormat = (uint)binaryReader.ReadInt32();
						imageBuffer = new byte[num8];
						imageBuffer = binaryReader.ReadBytes((int)num8);
						flag2 = true;
					}
				}
			}
			imageData = new ImageData();
			imageData.Width = (int)num3;
			imageData.Height = (int)num4;
			imageData.ImageBuffer = imageBuffer;
			if (flag2)
			{
				imageData.PixelFormat = PixelFormats.Rgb24;
				imageData.DetailedPixelFormat = detailedPixelFormat;
			}
			else
			{
				imageData.PixelFormat = PixelFormats.Gray8;
			}
			if (width > 0 && height > 0 && !flag && imageData.Height > 0)
			{
				double num9 = (double)imageData.Width / (double)imageData.Height;
				double num10 = (double)width / (double)height;
				if (num9 > num10)
				{
					height = (int)((double)width / num9);
				}
				else
				{
					width = (int)(num9 * (double)height);
				}
				imageData = ResizeImageData(imageData, width, height);
			}
			if (flag)
			{
				imageData.DetailedPixelFormat = 36700345u;
			}
			return imageData;
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			binaryReader?.Close();
			fileStream?.Close();
		}
	}
}
