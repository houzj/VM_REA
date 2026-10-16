using VisionFramework;
namespace VisionUi;
public static class RoiStyleAssets {
 public static (string Normal,string Pressed) For(RoiKind kind)=>kind switch{
  RoiKind.FullImage=>("roi_full-screen_normal.png","roi_full-screen_click.png"),
  RoiKind.Rectangle=>("roi_retangle_normal.png","roi_retangle_click.png"),
  RoiKind.UprightRectangle=>("roi_retangle_normal.png","roi_retangle_click.png"),
  RoiKind.LineBox=>("roi_retangle_normal.png","roi_retangle_click.png"),
  RoiKind.SequentialBox=>("roi_retangle_normal.png","roi_retangle_click.png"),
  RoiKind.PolygonCaliper=>("roi_多边形卡尺_click.png","roi_多边形卡尺_click.png"),
  RoiKind.Circle=>("geometry_circle_clickoff.png","geometry_circle_clickon.png"),
  RoiKind.Annulus=>("roi_circle_normal.png","roi_circle_click.png"),
  RoiKind.SectorRing=>("roi_circle_normal.png","roi_circle_click.png"),
  RoiKind.Sector=>("roi_扇形_normal.png","roi_扇形_click.png"),
  RoiKind.OverSector=>("roi_扇形_normal.png","roi_扇形_click.png"),
  RoiKind.Polygon=>("roi_polygon_normal.png","roi_屏蔽polygon_normal.png"),
  RoiKind.Point=>("geometry_point_clickoff.png","geometry_point_clickon.png"),
  RoiKind.Line=>("geometry_line_clickoff.png","geometry_line_clickon.png"),
  RoiKind.LineCaliper=>("roi_直线卡尺_normal.png","roi_直线卡尺_click.png"),
  RoiKind.CircleCaliper=>("roi_圆形卡尺_normal.png","roi_圆形卡尺_click.png"),
  RoiKind.SectorCaliper=>("roi_圆弧卡尺_normal.png","roi_圆弧卡尺_click.png"),
  RoiKind.MatrixCircle=>("ic_dialog_roi_阵列圆_normal.png","ic_dialog_roi_阵列圆_focus.png"),
  RoiKind.Coordinate=>("ic_dialog_坐标系_normal.png","ic_dialog_坐标系_click.png"),
  RoiKind.Box3D=>("roi_retangle_normal.png","roi_retangle_click.png"),
 _=>throw new ArgumentOutOfRangeException(nameof(kind))};
}
