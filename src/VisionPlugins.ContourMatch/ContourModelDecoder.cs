using System;
using System.Collections.Generic;
namespace VisionPlugins.ContourMatch;
public static class ContourModelDecoder
{
    // RVA1140 + strict-hash original alphabet at VA1800306b0. Unlike
    // Convert.FromBase64String, unknown characters become 0xff and no
    // arbitrary whitespace is skipped. Trailing incomplete groups are ignored.
    public static byte[] Decode(ReadOnlySpan<byte> text)
    {
        const string alphabet="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        int terminator=text.IndexOf((byte)0);if(terminator>=0)text=text[..terminator];
        var result=new List<byte>();
        static byte Find(byte value,string alphabet){for(byte i=0;i<64;i++)if(alphabet[i]==value)return i;return 255;}
        for(int start=0;start+3<text.Length;start+=4){
            byte a=Find(text[start],alphabet),b=Find(text[start+1],alphabet),
                c=Find(text[start+2],alphabet),d=Find(text[start+3],alphabet);
            result.Add(unchecked((byte)(((b>>4)&3)|(a<<2))));
            if(text[start+2]=='=')break;
            result.Add(unchecked((byte)(((c>>2)&15)|(b<<4))));
            if(text[start+3]=='=')break;
            result.Add(unchecked((byte)((d&63)|(c<<6))));
        }
        return result.ToArray();
    }
}
