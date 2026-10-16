using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionCore;
using VisionFramework;
using VisionUi;
static class NormalizedRoiChecks {
 static IEnumerable<DependencyObject> Children(DependencyObject root){yield return root;for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)foreach(var c in Children(VisualTreeHelper.GetChild(root,i)))yield return c;}
 public static void Run(){var roi=new RoiDocument("norm",RoiKind.Rectangle,.5,.5,.2,.4,CoordinateSpace:RoiCoordinateSpace.Normalized);var view=new GroupRoiEditor([roi],new([RoiKind.Rectangle])){ParametersExpanded=true};var image=new GrayImage(200,100,new byte[20000]);view.SetImage(image);var window=new Window{Content=view,Width=1000,Height=600,ShowInTaskbar=false,ShowActivated=false,Left=-20000,Top=-20000,WindowStartupLocation=WindowStartupLocation.Manual};window.Show();window.UpdateLayout();var x=Children(view).OfType<TextBox>().Single(b=>Equals(b.Tag,"GroupRoiField:CenterX"));Check(double.Parse(x.Text)==100,"normalized ROI displays original pixel coordinates");x.Text="120";view.CommitPending();var saved=view.Regions.Single();Check(saved.CoordinateSpace==RoiCoordinateSpace.Normalized&&Math.Abs(saved.CenterX-.6)<1e-9,"pixel edit persists original normalized coordinate space");view.SetImage(new GrayImage(400,200,new byte[80000]));x=Children(view).OfType<TextBox>().Single(b=>Equals(b.Tag,"GroupRoiField:CenterX"));Check(double.Parse(x.Text)==240&&Math.Abs(view.Regions.Single().CenterX-.6)<1e-9,"image resize preserves normalized geometry");Check(RoiRasterizer.CreateMask([saved],200,100).Count(p=>p!=0)==1600,"normalized ROI is consumed by source-derived rasterizer");var caliper=new RoiDocument("caliper",RoiKind.CircleCaliper,.5,.5,Radius:.2,CoordinateSpace:RoiCoordinateSpace.Normalized){Shape=new(CaliperWidth:.05,CaliperHeight:.1)};var pixel=RoiGeometry.ToPixels(caliper,200,100);Check(Math.Abs(pixel.Shape!.CaliperWidth-5)<1e-9&&Math.Abs(pixel.Shape.CaliperHeight-20)<1e-9,"source-specific circle caliper width/height normalization");window.Close();}
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);Console.WriteLine("PASS "+name);}
}
