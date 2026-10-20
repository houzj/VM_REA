import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.listing.*;
import java.nio.file.*;
public class BoardAudit extends GhidraScript {
 public void run() throws Exception {
  var out=Paths.get(getScriptArgs()[0]);Files.createDirectories(out);var it=currentProgram.getFunctionManager().getFunctions(true);var functions=new java.util.ArrayList<Function>();var index=new StringBuilder();
  while(it.hasNext()){var f=it.next();String name=f.getName(true);index.append(f.getEntryPoint()).append("\t").append(name).append("\n");if(name.contains("CalibBoard")&&(name.contains("ProcessAlg")||name.contains("ProcessOutput")||name.contains("GetResult")||name.contains("SetParam")))functions.add(f);if(name.contains("security_check_cookie"))f.setNoReturn(false);}
  Files.writeString(out.resolve("functions.tsv"),index.toString());var dec=new DecompInterface();var options=new DecompileOptions();options.setEliminateUnreachable(false);dec.setOptions(options);dec.openProgram(currentProgram);
  for(var f:functions){var result=dec.decompileFunction(f,30,monitor);if(result.decompileCompleted())Files.writeString(out.resolve(f.getEntryPoint()+".c"),result.getDecompiledFunction().getC());}dec.dispose();
 }
}
