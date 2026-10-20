// Source reconstruction: IMVSFixtureModu Process9a80 / SetParam a6e0.
namespace VisionPlugins.Fixture;
internal readonly record struct FixturePoseFields(float X,float Y,float Angle,float ScaleX,float ScaleY);
internal readonly record struct FixtureRead(uint ErrorCode,int Count,float Value);
internal readonly record struct FixtureProcessResult(uint ErrorCode,int? Status,FixturePoseFields? Reference,FixturePoseFields? Run);
internal sealed class FixtureState {
 internal FixturePoseFields Reference,Run;
 internal int InitFlag,BasisInitDataFlag;
 internal FixtureProcessResult Process(System.Func<string,FixtureRead> read){
  // Source ignores successful Count; each native getter is called exactly at index0.
  FixtureRead x=read("InPointX");int xs=Classify(x.ErrorCode);if(xs==0)return Error();
  FixtureRead y=read("InPointY");int ys=Classify(y.ErrorCode);if(ys==0)return Error();
  FixtureRead a=read("InAngle");int ast=Classify(a.ErrorCode);if(ast==0)return Error();
  float angle=a.ErrorCode==Missing?0:a.Value;if(a.ErrorCode==Missing)ast=1;
  FixtureRead sx=read("MatchScaleX");int xst=Classify(sx.ErrorCode);if(xst==0)return Error();
  float scaleX=sx.ErrorCode==Missing?1:sx.Value;if(sx.ErrorCode==Missing)xst=1;
  if(scaleX>10000f||scaleX<0f)return new(0xe000032e,null,null,null);
  FixtureRead sy=read("MatchScaleY");int yst=Classify(sy.ErrorCode);if(yst==0)return Error();
  float scaleY=sy.ErrorCode==Missing?1:sy.Value;if(sy.ErrorCode==Missing)yst=1;
  if(scaleY>10000f||scaleY<0f)return new(0xe000032e,null,null,null);
  if((xs&ys&ast&xst&yst)!=1){BasisInitDataFlag=0;return new(0,0,null,null);}
  BasisInitDataFlag=1;
  Run=new(x.ErrorCode==Missing?0:x.Value,y.ErrorCode==Missing?0:y.Value,angle,scaleX,scaleY);
  if(InitFlag==0)Reference=Run;
  return new(0,1,Reference,Run);
 }
 internal uint CreateReference(){if(BasisInitDataFlag==0){InitFlag=0;return 0xa0000002;}if(InitFlag==1)Reference=Run;InitFlag=1;return 0;}
 internal const uint Missing=0xe000031b,Unavailable=0xe000031c;
 static int Classify(uint error)=>error==0?1:error==Missing?3:error==Unavailable?2:0;
 static FixtureProcessResult Error()=>new(0xe0000001,null,null,null);
}

