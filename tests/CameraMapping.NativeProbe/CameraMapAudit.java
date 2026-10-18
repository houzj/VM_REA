import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
public class CameraMapAudit extends GhidraScript {
 public void run() throws Exception {
  Path out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);StringBuilder names=new StringBuilder();var functions=currentProgram.getFunctionManager().getFunctions(true);
  DecompInterface dec=new DecompInterface();dec.openProgram(currentProgram);
  while(functions.hasNext()){Function f=functions.next();names.append(f.getEntryPoint()).append("\t").append(f.getName()).append("\n");if(f.isExternal()||f.isThunk())continue;
   boolean selected=getScriptArgs().length==1;for(int i=1;i<getScriptArgs().length;i++)if(f.getEntryPoint().toString().equals(getScriptArgs()[i]))selected=true;if(currentProgram.getName().equals("IMVSCameraMapModu.dll") && java.util.Arrays.asList("GetProcessInput","ProcessAlg","ProcessOutput","ProcessCalibMatrix","WriteCalibFile").contains(f.getName()))selected=true;if(!selected)continue;
   var d=dec.decompileFunction(f,60,monitor);if(d.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),d.getDecompiledFunction().getC());
  }Files.writeString(out.resolve("functions.tsv"),names.toString());dec.dispose();
 }
}
