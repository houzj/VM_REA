// Deterministic input generation shared by the original SDK probe and plugin checks.
// No decoding or expected image algorithm lives in this helper.
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public static class ImageSourceCodecFixtures
{
    public const int Width=17,Height=19;
    public static byte[] ColorBytes() {
        byte[] data=new byte[Width*Height*3];
        byte[][] primary={new byte[]{0,0,0},new byte[]{255,255,255},new byte[]{0,0,255},new byte[]{0,255,0},new byte[]{255,0,0},new byte[]{0,255,255},new byte[]{255,0,255},new byte[]{255,255,0}};
        for(int i=0;i<Width*Height;i++) {
            byte[] color=i<primary.Length?primary[i]:new byte[]{(byte)((i*199+17)%256),(byte)((i*151+31)%256),(byte)((i*73)%256)};
            Array.Copy(color,0,data,i*3,3);
        }
        return data;
    }
    public static string[] Create(string directory) {
        Directory.CreateDirectory(directory);List<string> files=new List<string>();byte[] bgr=ColorBytes();
        foreach(string name in new[]{"color24.bmp","topdown24.bmp","alpha32.bmp","gray8.bmp","inverted8.bmp","palette8.bmp"}) {
            string path=Path.Combine(directory,name);WriteBmp(path,bgr,name);files.Add(path);
        }
        BitmapSource source=BitmapSource.Create(Width,Height,96,96,PixelFormats.Bgr24,null,bgr,Width*3);
        foreach(string extension in new[]{"png","tif","jpg"}) {
            BitmapEncoder encoder=extension=="png"?(BitmapEncoder)new PngBitmapEncoder():extension=="tif"?(BitmapEncoder)new TiffBitmapEncoder():new JpegBitmapEncoder{QualityLevel=90};
            encoder.Frames.Add(BitmapFrame.Create(source));string path=Path.Combine(directory,"color."+extension);
            using(FileStream output=File.Create(path))encoder.Save(output);files.Add(path);
        }
        string disguisedBmp=Path.Combine(directory,"bmp-as-png.png"),disguisedPng=Path.Combine(directory,"png-as-bmp.bmp");
        File.Copy(Path.Combine(directory,"color24.bmp"),disguisedBmp,true);File.Copy(Path.Combine(directory,"color.png"),disguisedPng,true);files.Add(disguisedBmp);files.Add(disguisedPng);
        string truncated=Path.Combine(directory,"truncated.bmp");byte[] complete=File.ReadAllBytes(files[0]);Array.Resize(ref complete,complete.Length-30);File.WriteAllBytes(truncated,complete);files.Add(truncated);
        string invalid=Path.Combine(directory,"not-image.bmp");File.WriteAllText(invalid,"not an image");files.Add(invalid);
        return files.ToArray();
    }
    static void WriteBmp(string path,byte[] bgr,string name) {
        bool palette=name.EndsWith("8.bmp"),alpha=name=="alpha32.bmp",top=name=="topdown24.bmp";int bits=palette?8:alpha?32:24,stride=((Width*bits+31)/32)*4,offset=54+(palette?1024:0);
        using(BinaryWriter writer=new BinaryWriter(File.Create(path))) {
            writer.Write((ushort)0x4d42);writer.Write(offset+stride*Height);writer.Write(0);writer.Write(offset);writer.Write(40);writer.Write(Width);writer.Write(top?-Height:Height);writer.Write((ushort)1);writer.Write((ushort)bits);writer.Write(0);writer.Write(stride*Height);writer.Write(0);writer.Write(0);writer.Write(palette?256:0);writer.Write(0);
            if(palette)for(int i=0;i<256;i++) {
                byte b=(byte)i,g=(byte)i,r=(byte)i;
                if(name=="inverted8.bmp")b=g=r=(byte)(255-i);
                if(name=="palette8.bmp"){b=(byte)((i*199+17)%256);g=(byte)((i*151+31)%256);r=(byte)((i*73)%256);}
                writer.Write(b);writer.Write(g);writer.Write(r);writer.Write((byte)0);
            }
            for(int row=0;row<Height;row++) {
                int y=top?row:Height-1-row;
                for(int x=0;x<Width;x++) {
                    int i=y*Width+x;
                    if(palette)writer.Write((byte)(i%256));
                    else {writer.Write(bgr[i*3]);writer.Write(bgr[i*3+1]);writer.Write(bgr[i*3+2]);if(alpha)writer.Write((byte)(i%256));}
                }
                for(int pad=Width*(bits/8);pad<stride;pad++)writer.Write((byte)0xa5);
            }
        }
    }
}
