namespace VisionFramework;
// ImageViewConfig/IMVSBinaryModuDisplay.xml: presentation is separate from algorithm parameters.
public sealed record RoiDisplayStyle(bool Visible=true,string OkColor="#0000FF",string NgColor="#FF0000",double Opacity=1,double StrokeThickness=2,string? FillColor=null){
 public void Validate(){static bool Color(string? s)=>s is not null&&s.Length is 7 or 9&&s[0]=='#'&&s.Skip(1).All(Uri.IsHexDigit);if(!Color(OkColor)||!Color(NgColor)||FillColor is not null&&!Color(FillColor)||!double.IsFinite(Opacity)||Opacity<0||Opacity>1||!double.IsFinite(StrokeThickness)||StrokeThickness<1||StrokeThickness>10)throw new ArgumentException("显示样式颜色、透明度或线宽无效。");}
}
public static class RoiDisplayStyles {
 public static IReadOnlyDictionary<string,RoiDisplayStyle> Defaults=>new Dictionary<string,RoiDisplayStyle>{{"ROI",new()},{"ExternROI",new(Opacity:.5,FillColor:"#FF5400")},{"Annular",new()}};
 public static IReadOnlyDictionary<string,RoiDisplayStyle> Snapshot(IReadOnlyDictionary<string,RoiDisplayStyle>? source){var result=Defaults.ToDictionary(p=>p.Key,p=>p.Value);if(source is not null)foreach(var pair in source){if(!result.ContainsKey(pair.Key))throw new ArgumentException("未知显示对象："+pair.Key);if(pair.Value is null)throw new ArgumentException("显示样式不能为空。");pair.Value.Validate();result[pair.Key]=pair.Value;}return new System.Collections.ObjectModel.ReadOnlyDictionary<string,RoiDisplayStyle>(result);}
 public static string Key(RoiDocument r)=>r.Exclusion?"ExternROI":r.Kind is RoiKind.Annulus or RoiKind.Sector or RoiKind.SectorRing or RoiKind.OverSector?"Annular":"ROI";
}
