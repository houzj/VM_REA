using VisionPlugins.Calibration;
public static class NImageIdentityChecks
{
 public static int Run(){int count=0;void Check(bool condition,string name){count++;if(!condition)throw new Exception(name);}byte[] code=[21,37,1,2,3,4];var ordinary=NImageBoardIdentity.FromDecodedBytes(code,7);Check(ordinary.OriginX==21&&ordinary.OriginY==37&&Math.Abs(ordinary.GridPitch-102.0304)<1e-5,"ordinary I byte semantics");Check(NImageBoardIdentity.FromDecodedBytes(code,6).GridPitch==ordinary.GridPitch,"ordinary II uses same coordinate payload layout");for(int x=0;x<=1023;x+=31)for(int y=0;y<=1023;y+=31){byte[] data=[(byte)(x>>6),(byte)((x&63)*4+(y>>8)),(byte)y,1,2,3];var decoded=NImageBoardIdentity.FromDecodedBytes(data,9);Check(decoded.OriginX==x&&decoded.OriginY==y,"expanded 10-bit origin packing");Check(Math.Abs(decoded.GridPitch-102.03)<1e-5,"expanded decimal pitch layout");}
  (int,int)[] expected=[(2,3),(3,-2),(-2,-3),(-3,2),(3,2),(-2,3),(-3,-2),(2,-3)];for(int mode=1;mode<=8;mode++){var p=ordinary.WorldIndex(9,14,7,11,mode);Check(p==(expected[mode-1].Item1+21,expected[mode-1].Item2+37),"source eight-way orientation mapping");}
  return count;
 }
}
