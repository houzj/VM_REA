using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using VisionDesigner;
using VisionDesigner.ImageBinary;

internal static class ModesProbe
{
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern bool SetDllDirectory(string path);

    private static int Main(string[] args)
    {
        string sdkRoot = args[0];
        string output = args[1];
        Directory.CreateDirectory(output);
        SetDllDirectory(Path.Combine(sdkRoot, "Runtime", "x64"));
        try
        {
            string inputPath = Path.Combine(output, "input.bmp");
            using (var bitmap = new Bitmap(64, 48))
            {
                for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int value = (x * 4 + y * 3 + ((x / 8 + y / 6) % 2) * 37) & 255;
                    bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
                bitmap.Save(inputPath, ImageFormat.Bmp);
            }

            using (var image = new CMvdImage())
            {
                image.InitImage(inputPath, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                using (var tool = new CImageBinaryTool())
                {
                    tool.InputImage = image;
                    tool.SetRunParam("LowThreshold", "91");
                    tool.SetRunParam("HighThreshold", "183");
                    tool.SetRunParam("CompareType", "0");
                    tool.SetRunParam("ThresholdOffset", "7");
                    tool.SetRunParam("KernelWidth", "3");
                    tool.SetRunParam("KernelHeight", "5");
                    tool.SetRunParam("GaussKernelSize", "5");
                    tool.SetRunParam("GaussSigma", "1.2");
                    tool.SetRunParam("SauvolaAdjustCoef", "0.2");
                    tool.SetRunParam("SauvolaRange", "128");
                    tool.SetRunParam("SauvolaSegType", "0");
                    tool.SetRunParam("SauvolaWinWidth", "15");
                    tool.SetRunParam("SauvolaWinHeight", "15");
                    int[] values = { 1, 2, 3, 8, 4 };
                    string[] names = { "hard", "mean", "gaussian", "sauvola", "otsu" };
                    using (var log = new StreamWriter(Path.Combine(output, "modes.txt")))
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            tool.SetRunParam("BinaryType", values[i].ToString());
                            tool.Run();
                            string file = names[i] + ".bmp";
                            tool.Result.OutputImage.SaveImage(Path.Combine(output, file));
                            log.WriteLine(values[i] + "," + names[i] + ",success");
                        }
                    }
                }
            }
            return 0;
        }
        catch (Exception error)
        {
            File.WriteAllText(Path.Combine(output, "failure.txt"), error.ToString());
            Console.Error.WriteLine(error);
            return 1;
        }
    }
}
