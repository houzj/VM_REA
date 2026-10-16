using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using VisionCore;
static class OverlayPng {
    public static void Write(string path,GrayImage image,CircleDetectionResult result) {
        int w=image.Width,h=image.Height; var rgb=new byte[checked(w*h*3)];
        for(int i=0;i<image.Pixels.Length;i++) rgb[i*3]=rgb[i*3+1]=rgb[i*3+2]=image.Pixels[i];
        void Dot(int x,int y,byte r,byte g,byte b,int size=1) {for(int dy=-size;dy<=size;dy++)for(int dx=-size;dx<=size;dx++){int xx=x+dx,yy=y+dy;if((uint)xx>=(uint)w||(uint)yy>=(uint)h)continue;int at=(yy*w+xx)*3;rgb[at]=r;rgb[at+1]=g;rgb[at+2]=b;}}
        if(result.Circle is CircleFit c) {int count=(int)Math.Ceiling(c.Radius*8)+64;for(int i=0;i<count;i++){double a=i*2*Math.PI/count;Dot((int)Math.Round(c.CenterX+c.Radius*Math.Cos(a)),(int)Math.Round(c.CenterY+c.Radius*Math.Sin(a)),255,0,120);}for(int d=-8;d<=8;d++){Dot((int)Math.Round(c.CenterX)+d,(int)Math.Round(c.CenterY),255,0,120);Dot((int)Math.Round(c.CenterX),(int)Math.Round(c.CenterY)+d,255,0,120);}}
        foreach(var p in result.EdgePoints)Dot((int)Math.Round(p.X),(int)Math.Round(p.Y),0,220,80,2);
        using var output=File.Create(path);output.Write(new byte[]{137,80,78,71,13,10,26,10});var header=new byte[13];BinaryPrimitives.WriteInt32BigEndian(header,w);BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4),h);header[8]=8;header[9]=2;Chunk(output,"IHDR",header);
        using var compressed=new MemoryStream();using(var zlib=new ZLibStream(compressed,CompressionLevel.Fastest,leaveOpen:true)){for(int y=0;y<h;y++){zlib.WriteByte(0);zlib.Write(rgb,y*w*3,w*3);}}Chunk(output,"IDAT",compressed.ToArray());Chunk(output,"IEND",[]);
    }
    private static void Chunk(Stream output,string name,byte[] data){Span<byte> field=stackalloc byte[4];BinaryPrimitives.WriteInt32BigEndian(field,data.Length);output.Write(field);var type=Encoding.ASCII.GetBytes(name);output.Write(type);output.Write(data);uint crc=0xffffffff;foreach(byte b in type.Concat(data)){crc^=b;for(int bit=0;bit<8;bit++)crc=(crc&1)!=0?(crc>>1)^0xedb88320:crc>>1;}BinaryPrimitives.WriteUInt32BigEndian(field,~crc);output.Write(field);}
}
