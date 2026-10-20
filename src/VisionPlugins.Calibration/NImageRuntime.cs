using VisionCore;
namespace VisionPlugins.Calibration;

public sealed record NImageRecognitionBatch(NImageSequenceSnapshot Sequence,uint ErrorCode,IReadOnlyList<NImageDetectedBoard> Boards){public bool Ready=>Sequence.Ready&&ErrorCode==0&&Boards.Count==Sequence.Total;}

/// <summary>Owned collection/recognition lifecycle; numerical solving and module output are subsequent stages.</summary>
public sealed class NImageRuntime
{
 readonly object gate=new();readonly NImageSequence sequence=new();int clearRevision;
 public NImageRecognitionBatch Add(GrayImage image,NImageSequenceParameters parameters,int boardType,int revision=0,double? physicalX=null,double? physicalY=null,double? angle=null,CancellationToken cancellationToken=default,int filterStatus=1,bool runRecognition=true,NImageSourceExecution? sourceExecution=null){
  cancellationToken.ThrowIfCancellationRequested();lock(gate){if(revision!=clearRevision){sequence.Clear();clearRevision=revision;}
   var snapshot=sequence.Add(image,parameters,physicalX,physicalY,angle);if(!snapshot.Ready||!runRecognition)return new(snapshot,0,Array.Empty<NImageDetectedBoard>());
   return RecognizeSnapshot(snapshot,parameters,boardType,filterStatus,cancellationToken,sourceExecution);
  }
 }
 public static NImageRecognitionBatch RecognizeSnapshot(NImageSequenceSnapshot snapshot,NImageSequenceParameters parameters,int boardType,int filterStatus=1,CancellationToken cancellationToken=default,NImageSourceExecution? sourceExecution=null){
  if(!snapshot.Ready)return new(snapshot,0,Array.Empty<NImageDetectedBoard>());
   // These are algorithm validation conditions, deliberately deferred until the original batch invocation point.
   if(boardType is not (6 or 7 or 8 or 9)||parameters.TranslationCount is >0 and <4||parameters.RotationCount==1||snapshot.Frames.Any(f=>f.Image.Width<64||f.Image.Height<64||f.Image.Width!=snapshot.Frames[0].Image.Width||f.Image.Height!=snapshot.Frames[0].Image.Height))return new(snapshot,0x10000019,Array.Empty<NImageDetectedBoard>());
   var execution=sourceExecution??NImageSourceExecution.Current();var recognized=new List<NImageDetectedBoard>();foreach(var frame in snapshot.Frames){cancellationToken.ThrowIfCancellationRequested();var recognition=NImageSourceRecognition.Find(frame.Image,boardType,execution,filterStatus:filterStatus,token:cancellationToken);if(recognition.UnsupportedReason is not null)throw new NotSupportedException(recognition.UnsupportedReason);var board=recognition.Board;if(board==null)return new(snapshot,0x10103000,Array.Empty<NImageDetectedBoard>());recognized.Add(board);}
   return new(snapshot,0,recognized.AsReadOnly());
 }
 public NImageSequenceSnapshot Inspect(int total){lock(gate)return sequence.Inspect(total);}
 public void Clear(){lock(gate)sequence.Clear();}
}
