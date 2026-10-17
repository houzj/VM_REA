namespace VisionPlugins.Calibration;

/// <summary>Physical coordinate interpretation after a HIK tag has actually passed decoding.</summary>
public readonly record struct NImageBoardIdentity(int OriginX,int OriginY,float GridPitch,int BoardType)
{
 public static NImageBoardIdentity FromDecodedBytes(ReadOnlySpan<byte> bytes,int boardType){if(bytes.Length<6)throw new ArgumentException("Six decoded data bytes required.",nameof(bytes));return boardType switch{
  6 or 7=>new(bytes[0],bytes[1],bytes[2]*100f+bytes[3]+bytes[4]*.01f+bytes[5]*.0001f,boardType),
  8 or 9=>new((bytes[0]&15)*64+(bytes[1]>>2),(bytes[1]&3)*256+bytes[2],bytes[3]*100f+bytes[4]+bytes[5]*.01f,boardType),
  _=>throw new ArgumentOutOfRangeException(nameof(boardType))};}
 /// <summary>Original 813020's eight orientation mappings; anchor is the corresponding lattice corner of the decoded tag.</summary>
 public (int X,int Y) WorldIndex(int column,int row,int anchorColumn,int anchorRow,int orientation){int x=column-anchorColumn,y=row-anchorRow;var local=orientation switch{1=>(x,y),2=>(y,-x),3=>(-x,-y),4=>(-y,x),5=>(y,x),6=>(-x,y),7=>(-y,-x),8=>(x,-y),_=>throw new ArgumentOutOfRangeException(nameof(orientation))};return(local.Item1+OriginX,local.Item2+OriginY);}
}
