using VisionCore;
using VisionPlugins.Calibration;
if(args.Contains("--generated-only")){Console.WriteLine($"Generated checks passed: {NImageGeneratedBoardChecks.Run(args.Contains("--export-rotated-fixtures"))}");return;}
if(args.Contains("--filter-only")){Console.WriteLine($"Filter checks: {NImageFilterChecks.Run()}");return;}
int checks=0;
void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
void Throws(Action action,string message){checks++;try{action();}catch(ArgumentException){return;}throw new Exception(message);}
var p=new NImageSequenceParameters();
double[] x=[-1,0,1,1,0,-1,-1,0,1],y=[-1,-1,-1,0,0,0,1,1,1];
for(int axis=1;axis<=2;axis++)for(int i=0;i<9;i++){var m=NImageSequence.Motion(i,p with{FirstAxis=axis});Check(m.Kind==1&&m.Angle==0,"translation kind");Check(m.X==(axis==1?x[i]:y[i])&&m.Y==(axis==1?y[i]:x[i]),"source serpentine trajectory");}
for(int i=9;i<12;i++){var m=NImageSequence.Motion(i,p);Check(m.Kind==2&&m.X==0&&m.Y==0&&m.Angle==i-9,"rotation sequence");}
for(int bits=0;bits<8;bits++){var m=NImageSequence.Motion(0,p,(bits&1)!=0?8:null,(bits&2)!=0?9:null,(bits&4)!=0?10:null);Check(bits==7?m.X==8&&m.Y==9&&m.Angle==10:m.X==-1&&m.Y==-1&&m.Angle==0,"all three subscription presence required");}
for(int direction=1;direction<=16;direction++)for(int index=0;index<15;index++){var m=NImageSequence.Motion(index,p with{TranslationCount=15,DirectionChangeCount=direction});int row=index/direction,col=index%direction;Check(m.X==(row%2==0?col:direction-col-1)-1&&m.Y==row-1,"arbitrary source direction count");}
var sequence=new NImageSequence();var pixels=new byte[]{12};var image=new GrayImage(1,1,pixels);
for(int i=0;i<24;i++){var s=sequence.Add(image,p);Check(s.Current==i%12+1&&s.Total==12&&s.Ready==(i%12==11),"batch reset and readiness");Check(s.Frames[^1].Image[0,0]==12,"owned pixel copy");}
sequence.Clear();Check(sequence.Inspect(12).Current==0,"explicit clear");
for(int mode=0;mode<=1;mode++)for(int mask=0;mask<16;mask++){string? a=(mask&1)!=0?"bound":null,b=(mask&2)!=0?"bound":null,c=(mask&4)!=0?"bound":null,d=(mask&8)!=0?"bound":null;Check(NImageUiRules.ShowAutomaticCoordinates(mode,a,b,c,d)==(d==null&&(mode==0?a==null:b==null&&c==null)),"original HidePhysicInfoOperation");}
Check(NImageUiRules.OtherCountMinimum(0)==1&&NImageUiRules.OtherCountMinimum(1)==0,"original SetMutualExclusion");Check(NImageUiRules.BoardTypes.Keys.Order().SequenceEqual(new[]{6,7,8,9}),"only source-supported coded boards");
Throws(()=>NImageSequence.Motion(0,p with{DirectionChangeCount=0}),"zero divisor rejected");Throws(()=>NImageSequence.Motion(0,p with{TranslationCount=0,RotationCount=0}),"empty batch rejected safely");Throws(()=>NImageSequence.Motion(0,p with{BaseX=double.NaN}),"nonfinite config rejected");Throws(()=>NImageSequence.Motion(0,p,double.PositiveInfinity,1,1),"nonfinite input rejected");
checks+=NImageEditorChecks.Run();
checks+=NImageFileChecks.Run();
checks+=NImageDetectorChecks.Run();
checks+=NImageIdentityChecks.Run();
checks+=NImageParserChecks.Run();checks+=NImageRuntimeChecks.Run();checks+=NImageFilterChecks.Run();checks+=NImageGeneratedBoardChecks.Run();
Console.WriteLine($"NImageCalibration checks passed: {checks}. Coded-board solver remains incomplete and is not registered as a complete tool.");





